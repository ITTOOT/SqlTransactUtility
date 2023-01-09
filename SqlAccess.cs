using System;
using System.Collections.Generic; // general IEnumerable and IEnumerator
using System.Data.SqlClient; // SqlConnection, SqlCommand, and SqlParameter
using System.Data; // DataTable and SqlDataType
using System.IO; // StreamReader
using System.Collections; // IEnumerator and IEnumerable
using Microsoft.SqlServer.Server; // SqlDataRecord and SqlMetaData
using System.Data.Common;
using System.Data.Odbc;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using static SqlTransactUtility.SqlAccess;
using System.Diagnostics;
//

namespace SqlTransactUtility
{
    internal class SqlAccess
    {
        // multi-row DML operations which are much more efficient than iterative statements.
        // In most cases this method will consume the same amount of memory as the iterative
        // method since you will likely have the data collected to send to the database at that moment.

        public int FullImport(string connectionString, string SpPath)
        {
            int returnCode = 0;
            DateTime startTime;
            double elapsedSeconds;
            SqlConnection dbConnection = null;

            string SP_TVP = "ImportTestDataTVP";
            string TVP_PARAM = "@ImportTable";
            string TVP_TYPE = "dbo.ImportTestStructure";

            try
            {
                startTime = DateTime.Now;
                Console.WriteLine("{0} -- Connecting to the Database...", startTime);

                //Database Connection "Server=localhost;Database=Test;Trusted_Connection=True;"
                using (dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();
                    SqlCommand commandSP = new SqlCommand(SP_TVP, dbConnection);
                    commandSP.CommandType = CommandType.StoredProcedure;
                    commandSP.CommandTimeout = 300;
                    SqlParameter importTable = new SqlParameter();
                    importTable.ParameterName = TVP_PARAM;
                    importTable.TypeName = TVP_TYPE;
                    importTable.SqlDbType = SqlDbType.Structured;
                    importTable.Value = new FullStreamingDataRecord(SpPath);
                    commandSP.Parameters.Add(importTable);
                    Console.WriteLine("{0} -- Calling proc to read and import the data...", DateTime.Now);
                    commandSP.ExecuteNonQuery();
                    elapsedSeconds = ((Convert.ToDouble(DateTime.Now.Ticks - startTime.Ticks)) / 10000000.0D);
                    Console.WriteLine("{0} -- Proc is done reading and importing the data via Streamed TVP ({1} Seconds))", DateTime.Now, elapsedSeconds);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                returnCode = 7;
            }
            finally
            {
                dbConnection.Close();
            }
            return returnCode;
        }

        //SQL Export
        public int FullExport(string connectionString, string SpPath)
        {
            int returnCode = 0;
            DateTime startTime;
            double elapsedSeconds;
            SqlConnection dbConnection = null;

            string SP_TVP = "ImportTestDataTVP";
            string TVP_PARAM = "@ImportTable";
            string TVP_TYPE = "dbo.ImportTestStructure";

            try
            {
                startTime = DateTime.Now;
                Console.WriteLine("{0} -- Connecting to the Database...", startTime);

                using (dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();
                    SqlCommand commandSP;
                    SqlDataReader dataReader;
                    string output = "";

                    using (commandSP = new SqlCommand(SpPath, dbConnection))
                    {
                        //If from DB = CommandType.StoredProcedure, from file = CommandType.Text
                        commandSP.CommandType = CommandType.StoredProcedure;
                        //commandSP.CommandType = CommandType.Text;
                        commandSP.CommandTimeout = 300;
                        SqlParameter importTable = new SqlParameter();
                        importTable.ParameterName = TVP_PARAM;
                        importTable.TypeName = TVP_TYPE;
                        importTable.SqlDbType = SqlDbType.Structured;
                        importTable.Value = new FullStreamingDataRecord(SpPath);
                        commandSP.Parameters.Add(importTable);
                        Console.WriteLine("{0} -- Calling proc to read and import the data...", DateTime.Now);
                        commandSP.ExecuteNonQuery();
                        elapsedSeconds = ((Convert.ToDouble(DateTime.Now.Ticks - startTime.Ticks)) / 10000000.0D);
                        Console.WriteLine("{0} -- Proc is done reading and importing the data via Streamed TVP ({1} Seconds))", DateTime.Now, elapsedSeconds);
                    }
                }
                elapsedSeconds = ((Convert.ToDouble(DateTime.Now.Ticks - startTime.Ticks)) / 10000000.0D);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                returnCode = 7;
            }
            finally
            {
                dbConnection.Close();
            }
            return returnCode;
        }

        //Basic Export to DataTable
        public int SP_ToDataTable(string connectionString, string spQuery, out DataTable dataTable)
        {
            int returnCode = 0;
            var startTime = DateTime.Now;

            SqlConnection dbConnection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            // split script on GO command
            //IEnumerable<string> commandStrings = Regex.Split(query, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //cmd.Parameters.AddWithValue("@COLUMN_ONE", SqlDbType.NVarChar).Value = columnOne;
            Console.WriteLine("Connect to database...");
            SqlDataAdapter adapter = new SqlDataAdapter(spQuery, dbConnection);
            adapter.SelectCommand.CommandTimeout = 60;
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            adapter.ContinueUpdateOnError = true;

            dataTable = new DataTable();
            try
            {
                Console.WriteLine("Fill datatable...");
                adapter.Fill(dataTable);

                //Get database error data
                if (dataTable.Rows.Count > 0)
                {
                    Console.WriteLine($"Datatable Row Count: {dataTable.Rows.Count}");
                }
                else
                {
                    Debug.WriteLine($"Datatable Row Count: {dataTable.Rows.Count}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                returnCode = 7;
                //System.Threading.Thread.ResetAbort();
            }
            finally
            {
                dbConnection.Close();
                var endTime = DateTime.Now;
                returnCode = 0;
                Console.WriteLine("Upload time elapsed: {0} seconds", (endTime - startTime).TotalSeconds);
            }
            return returnCode;
        }
    }


    //Data Streamer
    public class FullStreamingDataRecord : IEnumerable<SqlDataRecord>
    {
        string filePath;
        public FullStreamingDataRecord(string FilePath)
        {
            filePath = FilePath;
        }

        //Get Rows
        public IEnumerator<SqlDataRecord> GetEnumerator()
        {
            //Output column structure
            SqlMetaData[] columnStructure = new SqlMetaData[4];
            columnStructure[0] = new SqlMetaData("NewSerialNo", SqlDbType.Int);
            columnStructure[1] = new SqlMetaData("NewName", SqlDbType.VarChar, 16);
            columnStructure[2] = new SqlMetaData("NewStatus", SqlDbType.VarChar, 4);
            columnStructure[3] = new SqlMetaData("NewTimestamp", SqlDbType.DateTime);

            //Streamreader
            StreamReader fileReader = null;
            try
            {
                using (fileReader = new StreamReader(filePath))
                {
                    string inputRow = "";
                    string[] inputColumns = new string[4];
                    while (!fileReader.EndOfStream)
                    {
                        //Table data
                        inputRow = fileReader.ReadLine();
                        inputColumns = inputRow.Split('\t');

                        //Record (row) structure
                        SqlDataRecord dataRecord = new SqlDataRecord(columnStructure);
                        dataRecord.SetInt32(0, Int32.Parse(inputColumns[0]));
                        dataRecord.SetString(1, inputColumns[1]);
                        dataRecord.SetString(1, inputColumns[2]);
                        dataRecord.SetString(1, inputColumns[3].ToString());
                        yield return dataRecord;
                    }
                }
            }
            // no catch block allowed due to the "yield" command
            finally
            {
                fileReader.Close();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
