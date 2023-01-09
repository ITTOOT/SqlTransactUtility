using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTransactUtility
{
    internal class DatabaseConnection
    {
        public void SqlSetupImportTable(string connectionString, string spCmdText)
        {
            SqlConnection dbConnection = null;
            try
            {
                // "Server=localhost;Database=Test;Trusted_Connection=True;"
                using (dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();

                    //SQL Command
                    //Free cashe
                    SqlCommand importSP = new SqlCommand(spCmdText, dbConnection);
                    importSP.CommandType = CommandType.Text;
                    Console.WriteLine("{0} -- Truncating the import table, clearing the caches, etc...", DateTime.Now);
                    importSP.ExecuteNonQuery();
                    Console.WriteLine("{0} -- Done set-up!", DateTime.Now);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
            return;
        }
    }
}
