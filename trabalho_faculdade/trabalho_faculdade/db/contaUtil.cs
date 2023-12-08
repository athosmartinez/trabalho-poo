using Microsoft.Data.SqlClient;
using trabalhopoo.models;

namespace trabalhopoo.db;

public class contaUtil
{
  public static void AdicionarConta(Conta conta)
  {
    try
    {
      using (SqlConnection connection = Util.dbConnect())
      {
        SqlTransaction transaction1 = connection.BeginTransaction();

        string insertContaQuery = "INSERT INTO conta (idImovel, leitura, dataLeitura, tipoConta) VALUES (@IdImovel, @Leitura, @DataLeitura, @TipoConta)";

        SqlCommand insertContaCommand = new SqlCommand(insertContaQuery, connection, transaction1);
        insertContaCommand.Parameters.AddWithValue("@IdImovel", conta.Imovel.Id);
        insertContaCommand.Parameters.AddWithValue("@Leitura", conta.Leitura);
        insertContaCommand.Parameters.AddWithValue("@DataLeitura", conta.DataLeitura);
        insertContaCommand.Parameters.AddWithValue("@TipoConta", conta.TipoConta.ToString());

        insertContaCommand.ExecuteNonQuery();
        
        transaction1.Commit();
        
        long idConta = procuraIdContaPorMesAnosAndImovelAndTipo(conta.DataLeitura, conta.Imovel.Id, conta.TipoConta);

        long idContaAnterior = procuraIdContaPorMesAnosAndImovelAndTipo(conta.DataLeitura.AddMonths(-1), conta.Imovel.Id, conta.TipoConta);

        if (idContaAnterior != 0)
        {
          SqlTransaction transaction2 = connection.BeginTransaction();
          
          string updateContaAnteriorQuery = "UPDATE conta SET idContaAnterior = @IdContaAnterior WHERE idConta = @IdContaAtual";

          SqlCommand updateContaAnteriorCommand = new SqlCommand(updateContaAnteriorQuery, connection, transaction2);
          updateContaAnteriorCommand.Parameters.AddWithValue("@IdContaAtual", idConta);
          updateContaAnteriorCommand.Parameters.AddWithValue("@IdContaAnterior", idContaAnterior);

          updateContaAnteriorCommand.ExecuteNonQuery();

          transaction2.Commit();
        }

        Console.WriteLine("Conta adicionada com sucesso!");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Erro ao inserir conta no banco de dados: " + ex.Message);
    }
  }
  
  public static List<Conta> listaContasPorImovel(Imovel imovel)
  {
    List<Conta> contas = new List<Conta>();

    try 
    {
      using (SqlConnection connection = Util.dbConnect())
      {
        string selectQuery = "SELECT * FROM conta as c WHERE idImovel = @IdImovel ORDER BY dataLeitura DESC";

        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@IdImovel", imovel.Id);

        using (SqlDataReader reader = selectCommand.ExecuteReader())
        {
          while (reader.Read())
          {
            Conta c = new Conta();

            c.Id = reader.GetInt64(reader.GetOrdinal("idConta"));
            c.Imovel = imovel;
            c.Leitura = reader.GetInt64(reader.GetOrdinal("leitura"));
            c.DataLeitura = reader.GetDateTime(reader.GetOrdinal("dataLeitura"));
            c.TipoConta = (TipoConta)Enum.Parse(typeof(TipoConta),
              reader.GetString(reader.GetOrdinal("tipoConta")));
            
            try {
              c.ContaAnterior = new Conta(reader.GetInt64(reader.GetOrdinal("idContaAnterior")));
            } catch(Exception ex) {
            }

            if (c.ContaAnterior != null && c.ContaAnterior.Id != null)
              c.ContaAnterior = procuraContaPorId(c.ContaAnterior.Id, imovel);

            contas.Add(c);
          }
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Erro ao recuperar contas do banco de dados: " + ex.Message);
    }

    return contas;
  }
  
  public static Conta procuraContaPorId(long idConta, Imovel imovel)
  {
    Conta c = new Conta();

    try
    {
      using (SqlConnection connection = Util.dbConnect())
      {
        string selectQuery = "SELECT * FROM conta as c WHERE c.idConta = @idConta";
                    
        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@idConta", idConta);

        using (SqlDataReader reader = selectCommand.ExecuteReader())
        {
          if (reader.Read())
          {
            c.Id = reader.GetInt64(reader.GetOrdinal("idConta"));
            c.Imovel = imovel;
            c.Leitura = reader.GetInt64(reader.GetOrdinal("leitura"));
            c.DataLeitura = reader.GetDateTime(reader.GetOrdinal("dataLeitura"));
            c.TipoConta = (TipoConta)Enum.Parse(typeof(TipoConta),
              reader.GetString(reader.GetOrdinal("tipoConta")));
            
            try {
              c.ContaAnterior = new Conta(reader.GetInt64(reader.GetOrdinal("idContaAnterior")));
            } catch(Exception ex) {
            }

            if (c.ContaAnterior != null && c.ContaAnterior.Id != null)
              c.ContaAnterior = procuraContaPorId(c.ContaAnterior.Id, imovel);
          }
        }
      }
    }
    catch (Exception ex)
    {
      return c;
    }

    return c;
  }
  
  
  public static long procuraIdContaPorMesAnosAndImovelAndTipo(DateTime data, long idImovel, TipoConta tipo)
  {
    try
    {
      using (SqlConnection connection = Util.dbConnect())
      {
        string selectQuery = "SELECT c.idConta FROM conta as c WHERE idImovel = @IdImovel AND tipoConta = @TipoConta AND YEAR(dataLeitura) = @Ano AND MONTH(dataLeitura) = @Mes";
                    
        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@IdImovel", idImovel);
        selectCommand.Parameters.AddWithValue("@TipoConta", tipo.ToString());
        selectCommand.Parameters.AddWithValue("@Ano", data.Year);
        selectCommand.Parameters.AddWithValue("@Mes", data.Month);

        using (SqlDataReader reader = selectCommand.ExecuteReader())
        {
          if (reader.Read())
            return reader.GetInt64(reader.GetOrdinal("idConta"));
        }
      }
    }
    catch (Exception ex)
    {
      return 0;
    }

    return 0;
  }
}