using Microsoft.SqlServer.Server;
using SqlTransactUtility.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SqlTransactUtility
{
    internal class Program
    {
        static int Main(string[] args)
        {
            //SQL Server Set-up
            //Set up test DB & test table
            //MSSMS
            //Login with windows authentication
            //>Right click >Properties >security >SQL Server & Windows Authentiction Mode
            //>Security >Logins >username >Right click >Properties
            //>General >add a password
            //>User Mapping >TestDBName >db_owner
            //Logout/Login

            //>Execute >SP_CREATE_GetTestItem
            //>Databases >TestData >Programability >Stored Procedures
            //Change CREATE to ALTER in stored procedure

            //Run DataInfoFinderSql
            //Insert the authnetication details into the connection string e.g.
            //Server=DESKTOP-KJL4B46\\SQLEXPRESS;
            //Database=TestData; User=datawrite; Password=readwrite;
            //Application Name=SqlTransactUtility; Trusted_Connection=True;

            //Insert the stored procedure name into spEXEC e.g.
            //SP_GetTestItemsByStatus

            //Add a break point at //DONE and RUN
            //Check the consol for row count

            //Error idents
            bool error = false;
            int returnCode = 0;

            string streamingType = "get";
            string connectionString = "Server=DESKTOP-KJL4B46\\SQLEXPRESS; Database=TestData; " +
                "User=datawrite; Password=readwrite; Application Name=SqlTransactUtility; Trusted_Connection=True;";
            //string connectStr = "Server=localhost;Database=Test;Trusted_Connection=True;";

            //PUT STORED PROCEDURES IN THIS LOCATION ON  THE .exe COMPUTER
            string dirname = Directory.GetCurrentDirectory();
            // "Server=DESKTOP-KJL4B46\SQLEXPRESS; Database=TestData; User=datawrite; Password=readwrite; Application Name=SqlTransactUtility"
            //string SetUpCmdText = File.ReadAllText(Path.Combine(dirname, "C:\\Users\\Player_1\\Desktop\\WORK\\SqlTransactUtility\\SqlItems\\ImportDataTable.sql"));
            string SetUpCmdText = "CHECKPOINT ; TRUNCATE TABLE dbo.ImportTest ; DBCC FREEPROCCACHE ; DBCC FREESYSTEMCACHE('ALL') ; CHECKPOINT ;";
            //string SpPATH = Path.Combine(dirname, "SqlItems\\SP_GetTestItemsByStatus.sql"); TestTable
            //string SpEXEC = "SP_GetTestItemsByStatus";
            string SpEXEC = "SP_GetTestItemsByStatus";

            ProcedureBuilder procedureBuilder = new ProcedureBuilder();
            DataTable dataTable;

            if (streamingType.Length < 2)
            {
                error = true;
                returnCode = 1;
            }
            else
            {
                //RUN
                switch (streamingType.ToLower())
                {
                    case "full":
                        Console.WriteLine("Start full streaming...");

                        //Make connection and create import table
                        //DatabaseConnection dbConnection = new DatabaseConnection();
                        //dbConnection.SqlSetupImportTable(connectStr, SetUpCmdText);
                        Console.WriteLine("Full streaming done...");
                        break;
                    case "basic":
                        Console.WriteLine("Start basic data retrieval...");

                        //Stored Procedures
                        SqlAccess sqlAccess = new SqlAccess();
                        returnCode = sqlAccess.SP_ToDataTable(connectionString, SpEXEC, out dataTable);
                        Console.WriteLine("Basic data retrieval done...");
                        break;
                    case "insert":
                        Console.WriteLine("Start insert data...");
                        ////Stored Procedures
                        //procedureBuilder.Table = "TestTable";
                        //procedureBuilder.Column = "Name";
                        //procedureBuilder.NewValue = "One Changed!";
                        //procedureBuilder.IdRow = "SerialNo";
                        //procedureBuilder.IdRowValue = "100";
                        //returnCode = procedureBuilder.UpdateSingleCell(connectionString);
                        Console.WriteLine("Basic insert data done...");
                        break;
                    case "get":
                        Console.WriteLine("Start get data...");
                        //Stored Procedures
                        procedureBuilder.Table = "TestTable";
                        procedureBuilder.SelectColumns.Add("SerialNo");
                        procedureBuilder.SelectColumns.Add("Description");
                        //procedureBuilder.IdRow = "SerialNo";
                        //procedureBuilder.IdRowValue = "100";
                        returnCode = procedureBuilder.GetRecords(connectionString, out dataTable);
                        Console.WriteLine("Basic get data done...");
                        break;
                    default:
                        error = true;
                        returnCode = 2;
                        break;
                }
            }
            //DONE
            Console.WriteLine("{0} -- Done!", DateTime.Now);
            if (error)
            {
                //Starts the import and stored procedures
                Console.WriteLine("Usage: Database Importer {0} {1}", streamingType, SpEXEC);
            }
            //Provide status of operation
            return returnCode;
        }
    }
}
