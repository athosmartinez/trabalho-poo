using System.Collections;
using System.Data;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using trabalhopoo.models;

namespace trabalhopoo.db
{
    public static class Util
    {

        public static SqlConnection dbConnect()
        {
            try
            {
                string connectionString = "Server=tcp:dbjoao.database.windows.net,1433;Initial Catalog=db_contas_poo;Persist Security Info=False;User ID=adminjoao;Password=Joao.pucminas22;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;

            }
            catch (SqlException e)
            {
                System.Console.WriteLine(e.ToString());
                throw;
            }

        }        
    }
}
