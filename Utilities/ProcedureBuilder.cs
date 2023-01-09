using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace SqlTransactUtility.Utilities
{
    //Interface
    public interface ISqlParams
    {
        string Table { get; set; }
        string Column { get; set; }
        List<string> SelectColumns { get; set; }
        SqlString NewValue { get; set; }
        string IdRow { get; set; }
        SqlString IdRowValue { get; set; }
    }

    //Build stored procedures programatically
    public class ProcedureBuilder : ISqlParams
    {
        public string Table { get; set; }
        public string Column { get; set; }
        public List<string> SelectColumns { get; set; } = new List<string>();
        public SqlString NewValue { get; set; }
        public string IdRow { get; set; }
        public SqlString IdRowValue { get; set; }

        //Update one cells value
        [SqlProcedure]
        public int UpdateSingleCell(string connectionString)
        {
            //string cs = string.Join(connectionString, " context connection=true;");
            SqlConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            //Command
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbConnection;
            cmd.CommandText = $"UPDATE {Table} SET {Column} = @newValue WHERE {IdRow} = @rowIdValue";

            //Adve vales to SQL command string
            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("@newValue", NewValue)); //New column value
            //WHERE
            paramList.Add(new SqlParameter("@rowIdValue", IdRowValue)); //through the value for the ID            //SqlParameter table = new SqlParameter("@table", name);
            //
            if (paramList != null)
                cmd.Parameters.AddRange(paramList.ToArray());

            //EXECUTE
            int i = cmd.ExecuteNonQuery();

            dbConnection.Close();
            //If ex "requires a SqlClr context" run
            //SqlContext.Pipe.Send(i.ToString());

            return i;
        }

        //Update one cells value
        [SqlProcedure]
        public int GetRecords(string connectionString, SqlString WHERE, out DataTable dataTable)
        {
            //string cs = string.Join(connectionString, " context connection=true;");
            SqlConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            string result = "*";
            //
            result = String.Join(", ", SelectColumns);

            //Command
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbConnection;

            if (WHERE.IsNull)
            {
                cmd.CommandText = $"SELECT {result} FROM {Table}";
            }
            else
            {
                cmd.CommandText = $"SELECT {result} FROM {Table} WHERE {WHERE}";
            }

            //EXECUTE
            SqlDataReader reader = cmd.ExecuteReader();

            //Load to data table
            dataTable = reader.GetSchemaTable();

            //SqlContext.Pipe.Send(reader);
            reader.Close();

            dbConnection.Close();
            //If ex "requires a SqlClr context" run
            //SqlContext.Pipe.Send(i.ToString());

            return 888;
        }

        //Update one cells value
        [SqlProcedure]
        public int FromFile(string connectionString, string pathToQuery, string dateQuery = "")
        {
            //string cs = string.Join(connectionString, " context connection=true;");
            SqlConnection dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();
            //Read file
            string SpQuery = File.ReadAllText(pathToQuery);
            //Split script on GO command
            IEnumerable<string> query = Regex.Split(SpQuery, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            //Command
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = dbConnection;
            cmd.CommandText = string.Concat(dateQuery, query);
            cmd.CommandTimeout = 600;

            //EXECUTE
            int i = cmd.ExecuteNonQuery();

            dbConnection.Close();
            //If ex "requires a SqlClr context" run
            //SqlContext.Pipe.Send(i.ToString());

            return i;
        }
    }
}
