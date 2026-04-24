using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace MIS
{
    public class clsClientConnection
    {
        private clsFunction dbFunction = new clsFunction();
        private clsFile dbFile = new clsFile();
        private clsFile dbDump;
        private string connectionString;

        // For MYSQL
        public MySqlConnection conn = null;
        public MySqlCommand command = null;
        public MySqlDataReader reader = null;

        public static string DbConnString()
        {
            return GetConnectionString();
        }

        public bool ConnectDBServerClient()
        {
            bool isConnected = false;

            connectionString = GetConnectionString();

            try
            {
                // For MYSQL
                conn = new MySqlConnection(connectionString);

                isConnected = OpenConnection();
                isConnected = true;
                //MessageBox.Show("Connected...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check connection details below" + "\n\n" +
                               "Error Number: " + ex.Message + "\n" +
                               "Source: " + clsGlobalVariables.strSource + "\n" +
                               "Server: " + clsGlobalVariables.strServer + "\n" +
                               "Port: " + clsGlobalVariables.strPort + "\n" +
                               "Database: " + clsGlobalVariables.strDatabase + "\n" +
                               "UserName: " + clsGlobalVariables.strUserName + "\n" +
                               "Password: " + clsGlobalVariables.strPassword + "\n\n" +
                               "Please contact administrator."
                               ,"Unable to connect.", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                Application.Exit();
            }
            finally
            {
                if (conn != null)
                {
                    CloseConnection();
                }
            }

            return isConnected;
        }
        static private string GetConnectionString()
        {
            string connectionString = "";

            // Retreive from configuration file
            clsINI dbSetting = new clsINI();
            dbSetting.InitDatabaseSetting();

            if (string.Compare(clsGlobalVariables.strSource, "MYSQLSERVER") == 0)
            {            
                connectionString = "Persist Security Info=" + clsGlobalVariables.strSecurity + ";" +
                                   "Server=" + clsGlobalVariables.strServer + ";" +
                                   "Port=" + clsGlobalVariables.strPort + ";" +
                                   "Database=" + clsGlobalVariables.strDatabase + ";" +
                                   "User=" + clsGlobalVariables.strUserName + ";" +
                                   "Password=" + clsGlobalVariables.strPassword + ";" +
                                   "Connect Timeout=" + clsGlobalVariables.strTimeOut + ";";     
            }

            if (string.Compare(clsGlobalVariables.strSource, "MSSQLSERVER") == 0)
            {
                connectionString = "Data Source =" + clsGlobalVariables.strServer + "," + clsGlobalVariables.strPort +
                               ";Initial Catalog =" + clsGlobalVariables.strDatabase +
                               ";User ID =" + clsGlobalVariables.strUserName +
                               ";Password =" + clsGlobalVariables.strPassword +
                               ";Integrated Security = " + clsGlobalVariables.strSecurity + ";";
            }

            if (string.Compare(clsGlobalVariables.strSource, "MARIANSERVER") == 0)
            {
                connectionString = "Server =" + clsGlobalVariables.strServer +
                               ";Port =" + clsGlobalVariables.strPort +
                               ";Database =" + clsGlobalVariables.strDatabase +
                               ";Uid =" + clsGlobalVariables.strUserName +
                               ";Pwd =" + clsGlobalVariables.strPassword + ";";
            }

            //MessageBox.Show("Connection String : " + connectionString);
            Debug.WriteLine("connectionString="+ connectionString);

            return connectionString;
        }
        public bool OpenConnection()
        {
            bool isConnected = false;

            // For MYSQL            
            try
            {
                conn.Open();
                isConnected = true;
            }
            catch (MySqlException ex)
            {
                MySQLErrorHandlerMessage(ex, "OpenConnection", "", "", "", "", "", "", "");
                isConnected = false;
                throw ex;
            }

            return isConnected;
        }

        public bool CloseConnection()
        {
            bool isDisconnected = false;

            try
            {
                conn.Close();
                isDisconnected = true;
            }
            catch (MySqlException ex)
            {
                MySQLErrorHandlerMessage(ex, "CloseConnection", "", "", "", "", "", "", "");
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return isDisconnected;
        }
        public void MySQLErrorHandlerMessage(MySqlException ex, string sFunctionName, string sStoredProcedure, string sStatementType, string sTableName, string sSearchBy, string sSearchValue, string sSQL, string sErrorMessage)
        {
            string sTemp = "";
            string sLog = "";
            string sTempErrorMessage = "";

            dbDump = new clsFile();

            // For MessageBox
            sTemp = "**Function Name: " + sFunctionName + "\n" +
                    "**Stored Procedure: " + sStoredProcedure + "\n" +
                    "**Parameter: \n" +
                    "  -Statement Type: " + "   " + sStatementType + "\n" +
                    "  -Table Name/Type: " + "   " + sTableName + "\n" +
                    "  -Search By: " + "   " + sSearchBy + "\n" +
                    "  -Search Value: " + "   " + sSearchValue + "\n\n" +
                    "**Query: \n" +
                    "  -SQL: " + "   " + sSQL + "\n\n";

            if (ex != null)
                sTempErrorMessage = "Error Description: \n" + "   " + ex.Message;
            else
                sTempErrorMessage = "Error Description: \n" + "   " + sErrorMessage;

            sTemp = sTemp + sTempErrorMessage;

            // For Dump
            sLog = "Error: " + " | " +
                    "Function Name: " + sFunctionName + " | " +
                    "Stored Procedure: " + sStoredProcedure + " | " +
                    "Statement Type: " + sStatementType + " | " +
                    "Table Name / Type: " + sTableName + "|" +
                    "Search By: " + sSearchBy + " | " +
                    "Search Value: " + sSearchValue + " | " +
                    "SQL: " + sSQL + " | ";

            if (ex != null)
                sTempErrorMessage = "Error Description: " + ex.Message;
            else
                sTempErrorMessage = "Error Description: " + sErrorMessage;

            sLog = sLog + sTempErrorMessage;

            // Display Error Message
            if (ex != null)
            {
                switch (ex.Number)
                {
                    case 0:
                        dbDump.WriteAPILog(2, sLog + " Message: " + "Connection to server failed.");
                        MessageBox.Show("Connection to server failed.", "Contact Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    case 1045:
                        dbDump.WriteAPILog(2, sLog + " Message: " + "Invalid username/password.");
                        MessageBox.Show("Invalid username/password", "Please try again", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                    default:
                        dbDump.WriteAPILog(2, sLog);
                        MessageBox.Show(sTemp, "Error Handler - " + sFunctionName, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        break;
                }
            }
            else
            {
                dbDump.WriteAPILog(2, sLog);
                MessageBox.Show(sTemp, "Error Handler - " + sFunctionName, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public DataSet GetReportWithStoredProcedure(int ReportID, string StatementType, string SearchBy, string SearchValue, string StoreProcedureName, ListView lvw)
        {
            DataSet rtnDs = new DataSet();

            Debug.WriteLine("--GetReportWithStoredProcedure--");
            Debug.WriteLine(" > ReportID="+ ReportID);
            Debug.WriteLine(" > StatementType=" + StatementType);
            Debug.WriteLine(" > SearchBy=" + SearchBy);
            Debug.WriteLine(" > SearchValue=" + SearchValue);
            Debug.WriteLine(" > StoreProcedureName=" + StoreProcedureName);
            Debug.WriteLine("--GetReportWithStoredProcedure--");

            Debug.WriteLine("--Stored Procedure(Report) --");
            string spString = "CALL " + StoreProcedureName + "(" + "\"" + StatementType + "\"" + "," + "\"" + SearchBy + "\"" + "," + "\"" + SearchValue + "\"" + ")" + ";";
            Debug.WriteLine(spString);

            dbDump = new clsFile();
            dbDump.WriteAPILog(0, "Stored Procedure(Report): " + spString);

            dbFunction.GetRequestTime("GetReportWithStoredProcedure->Start");

            dbFunction.parseDelimitedString(SearchValue, clsDefines.gPipe, 1);

            dbFile.WriteSysytemLog($"GetReportWithStoredProcedure..."); // add log

            try
            {
                if (OpenConnection())
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(StoreProcedureName, conn))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adapter.SelectCommand.Parameters.Add("@p_StatementType", MySqlDbType.VarChar).Value = StatementType;
                        adapter.SelectCommand.Parameters.Add("@p_SearchBy", MySqlDbType.VarChar).Value = SearchBy;
                        adapter.SelectCommand.Parameters.Add("@p_SearchValue", MySqlDbType.VarChar).Value = SearchValue;
                        adapter.SelectCommand.CommandTimeout = int.Parse(clsGlobalVariables.strTimeOut); // Setting command timeout

                        adapter.Fill(rtnDs);

                        clsGlobalVariables.globalDataSet = new DataSet();
                        clsGlobalVariables.globalDataTable = new DataTable();

                        clsGlobalVariables.globalDataSet = rtnDs;
                        clsGlobalVariables.globalDataTable = rtnDs.Tables[0];

                        // Populate ListView with data
                        string excludeFilePath = dbFile.sRespFullPath + "respExcludeExportColumn.json";

                        dbFile.WriteSysytemLog($"populateListViewFromDataSet..."); // add log
                        dbFunction.populateListViewFromDataSet(lvw, rtnDs, excludeFilePath);
                        dbFile.WriteSysytemLog($"populateListViewFromDataSet...complete"); // add log

                        //dbFunction.ListViewAlternateBackColor(lvw);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("GetReportWithStoredProcedure, MySqlException error message " + ex.Message);
                MySQLErrorHandlerMessage(ex, "GetReportWithStoredProcedure", StoreProcedureName, StatementType, "", SearchBy, SearchValue, "", "");
                throw;
            }
            finally
            {
                CloseConnection();
                dbFunction.GetResponseTime("GetReportWithStoredProcedure->End");
            }

            dbFile.WriteSysytemLog($"GetReportWithStoredProcedure...complete"); // add log

            return rtnDs;
        }

        public DataSet getStoredProcedureDateSet(string StatementType, string SearchBy, string SearchValue, string StoreProcedureName)
        {
            DataSet rtnDs = new DataSet();

            Debug.WriteLine("--getStoredProcedureDateSet--");
            Debug.WriteLine(" > StatementType=" + StatementType);
            Debug.WriteLine(" > SearchBy=" + SearchBy);
            Debug.WriteLine(" > SearchValue=" + SearchValue);
            Debug.WriteLine(" > StoreProcedureName=" + StoreProcedureName);
            Debug.WriteLine("--getStoredProcedureDateSet--");

            Debug.WriteLine("--Stored Procedure(Report) --");
            string spString = "CALL " + StoreProcedureName + "(" + "\"" + StatementType + "\"" + "," + "\"" + SearchBy + "\"" + "," + "\"" + SearchValue + "\"" + ")" + ";";
            Debug.WriteLine(spString);

            dbDump = new clsFile();
            dbDump.WriteAPILog(0, "Stored Procedure(Report): " + spString);

            dbFunction.GetRequestTime("getStoredProcedureDateSet->Start");

            dbFunction.parseDelimitedString(SearchValue, clsDefines.gPipe, 1);

            try
            {
                if (OpenConnection())
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(StoreProcedureName, conn))
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adapter.SelectCommand.Parameters.Add("@p_StatementType", MySqlDbType.VarChar).Value = StatementType;
                        adapter.SelectCommand.Parameters.Add("@p_SearchBy", MySqlDbType.VarChar).Value = SearchBy;
                        adapter.SelectCommand.Parameters.Add("@p_SearchValue", MySqlDbType.VarChar).Value = SearchValue;
                        adapter.SelectCommand.CommandTimeout = int.Parse(clsGlobalVariables.strTimeOut); // Setting command timeout

                        adapter.Fill(rtnDs);
                        
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("getStoredProcedureDateSet, MySqlException error message " + ex.Message);
                MySQLErrorHandlerMessage(ex, "getStoredProcedureDateSet", StoreProcedureName, StatementType, "", SearchBy, SearchValue, "", "");
                throw;
            }
            finally
            {
                CloseConnection();
                dbFunction.GetResponseTime("getStoredProcedureDateSet->End");
            }

            return rtnDs;
        }

    }
}
