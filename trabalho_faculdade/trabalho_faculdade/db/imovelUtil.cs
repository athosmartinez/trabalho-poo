using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabalhopoo;
using Microsoft.Data.SqlClient;
using Azure.Identity;
using trabalhopoo.models;

namespace trabalhopoo.db;

public static class imovelUtil
{
    public static void AdicionarImovel(Imovel imovel, long idCliente)
    {
        try
        {
            using (SqlConnection connection = Util.dbConnect())
            {
                SqlTransaction transaction = connection.BeginTransaction();

                string insertImovelQuery = @"INSERT INTO db_contas_poo.dbo.imovel (matriculaImovel, tipoImovel) OUTPUT INSERTED.idImovel VALUES (@MatriculaImovel, @TipoImovel)";

                SqlCommand insertImovelCommand = new SqlCommand(insertImovelQuery, connection, transaction);
                insertImovelCommand.Parameters.AddWithValue("@MatriculaImovel", imovel.Matricula);
                insertImovelCommand.Parameters.AddWithValue("@TipoImovel", imovel.Tipo.ToString()); // Ou apenas imovel.Tipo, se TipoImovel for uma string

                long idImovel = (long)insertImovelCommand.ExecuteScalar();

                string insertClienteImovelQuery = @"INSERT INTO db_contas_poo.dbo.clienteImovel (idImovel, idCliente) VALUES (@IdImovel, @IdCliente)";

                SqlCommand insertClienteImovelCommand = new SqlCommand(insertClienteImovelQuery, connection, transaction);
                insertClienteImovelCommand.Parameters.AddWithValue("@IdImovel", idImovel);
                insertClienteImovelCommand.Parameters.AddWithValue("@IdCliente", idCliente);

                insertClienteImovelCommand.ExecuteNonQuery();

                transaction.Commit();

                Console.WriteLine($"Imóvel adicionado com sucesso e associado ao cliente de id: {idCliente}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao inserir imóvel no banco de dados: " + ex.Message);
        }
    }

    public static List<Imovel> listaImoveisCliente(long idCliente)
    {
        List<Imovel> Imoveis = new List<Imovel>();

        try 
        {
            using (SqlConnection connection = Util.dbConnect())
            {
                string selectQuery = "SELECT i.idImovel, i.matriculaImovel, i.tipoImovel " +
                                     "FROM imovel i " +
                                     "JOIN clienteImovel ci ON i.idImovel = ci.idImovel " +
                                     "WHERE ci.idCliente = @IdCliente";

                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@IdCliente", idCliente);

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Imovel imovel = new Imovel();

                        imovel.Matricula = reader.GetString(reader.GetOrdinal("matriculaImovel"));
                        imovel.Id = reader.GetInt64(reader.GetOrdinal("idImovel"));
                        imovel.Tipo = (TipoImovel)Enum.Parse(typeof(TipoImovel),
                            reader.GetString(reader.GetOrdinal("tipoImovel")));

                        Imoveis.Add(imovel);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao recuperar imóveis do banco de dados: " + ex.Message);
        }

        return Imoveis;
    }
    
    public static Imovel? GetImovelByMatricula(string? matricula)
    {
        if (string.IsNullOrEmpty(matricula) || matricula.Length > 20)
            return null;
        
        Imovel imovel = new Imovel();

        try
        {
            using (SqlConnection connection = Util.dbConnect())
            {
                string selectQuery = "SELECT " +
                                     "i.matriculaImovel, " +
                                     "i.idImovel, " +
                                     "i.tipoImovel," +
                                     "ci.idCliente " +
                                     "FROM imovel as i " +
                                     "INNER JOIN clienteImovel AS ci ON ci.idImovel = i.idImovel " +
                                     "WHERE matriculaImovel = @Matricula";
                    
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@Matricula", matricula);

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        imovel.Matricula = reader.GetString(reader.GetOrdinal("matriculaImovel"));
                        imovel.Id = reader.GetInt64(reader.GetOrdinal("idImovel"));
                        imovel.Tipo = (TipoImovel)Enum.Parse(typeof(TipoImovel),
                            reader.GetString(reader.GetOrdinal("tipoImovel")));
                        imovel.Dono = new Cliente(reader.GetInt64(reader.GetOrdinal("idCliente")));

                        imovel.Contas = contaUtil.listaContasPorImovel(imovel);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao recuperar imovel do banco de dados: " + ex.Message);
        }

        return imovel;
    }
}