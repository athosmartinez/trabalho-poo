using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabalhopoo;
using Microsoft.Data.SqlClient;
using Azure.Identity;
using Microsoft.IdentityModel.Tokens;
using trabalhopoo.models;

namespace trabalhopoo.db
{
    public static class clienteUtil
    {
        public static void AdicionarCliente(string nome, string cpf)
        {
            try
            {
                using (SqlConnection connection = Util.dbConnect())
                {
                    string insertQuery = "INSERT INTO cliente (nome, cpf) VALUES (@Nome, @Cpf)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Nome", nome);
                    insertCommand.Parameters.AddWithValue("@Cpf", cpf);
                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Cliente adicionado com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("Falha ao adicionar cliente.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao inserir usuário no banco de dados: " + ex.Message);
            }
        }

        public static List<Cliente> listaClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                using (SqlConnection connection = Util.dbConnect())
                {
                    string selectQuery = "SELECT * FROM cliente";
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente();
                            cliente.Id = reader.GetInt64(reader.GetOrdinal("idCliente"));
                            cliente.Nome = reader.GetString(reader.GetOrdinal("nome"));
                            cliente.Cpf = reader.GetString(reader.GetOrdinal("cpf"));
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao recuperar clientes do banco de dados: " + ex.Message);
            }

            return clientes;
        }


        public static long procuraIdClienteCpf(string cpf)
        {
            Cliente cliente = procuraClienteCpf(cpf);

            if (cliente == null)
                return 0;

            return cliente.Id;
        }

        public static Cliente procuraClienteCpf(string cpf)
        {
            Cliente cliente = new Cliente();

            try
            {
                using (SqlConnection connection = Util.dbConnect())
                {
                    string selectQuery = "SELECT * FROM cliente as c WHERE cpf = @Cpf";
                    
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@Cpf", cpf);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente.Id = reader.GetInt64(reader.GetOrdinal("idCliente"));
                            cliente.Nome = reader.GetString(reader.GetOrdinal("nome"));
                            cliente.Cpf = reader.GetString(reader.GetOrdinal("cpf"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao recuperar clientes do banco de dados: " + ex.Message);
            }

            if (cliente != null)
            {
                cliente.Imoveis = imovelUtil.listaImoveisCliente(cliente.Id);

                if (!cliente.Imoveis.IsNullOrEmpty())
                {
                    foreach (Imovel i in cliente.Imoveis) 
                        i.Contas = contaUtil.listaContasPorImovel(i);
                }
            }

            return cliente;
        }
    }
}