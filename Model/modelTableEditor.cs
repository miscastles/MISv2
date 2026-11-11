using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MIS.Model
{
    public class modelTableEditor
    {
        private static clsAPI dbAPI = new clsAPI();
        private static clsFunction dbFunction = new clsFunction();

        public int TCurrentPage { get; set; } = 1;
        public int TPageCount { get; set; } = 0;
        public int TPageRows { get; set; } = 0;

        public class Table
        {
            public List<string> Fetch()
            {
                int i = 0;

                List<string> rTableList = new List<string>();

                dbAPI.ExecuteAPI("GET", "View", "Table List", "", "Advance Detail", "", "ViewAdvanceDetail");
                if (!clsGlobalVariables.isAPIResponseOK) return rTableList;

                if (!dbAPI.isNoRecordFound())
                {
                    while (clsArray.ID.Length > i)
                    {
                        if (clsArray.Description[i] != clsFunction.sDefaultSelect)
                        {
                            rTableList.Add(clsArray.Description[i]);
                        }

                        i++;
                    }
                }

                return rTableList;
            }

            public List<string> Contents(string sTable, int iPage)
            {
                int i = 0;

                List<string> rTableContents = new List<string>();

                dbAPI.ExecuteAPI("GET", "View", "Table Rows", sTable + clsFunction.sPipe + iPage, "Advance Detail", "", "ViewAdvanceDetail");
                if (!clsGlobalVariables.isAPIResponseOK) return rTableContents;

                if (!dbAPI.isNoRecordFound())
                {
                    int iHeader = Array.IndexOf(clsArray.ID, "0");
                    if (iHeader < 0) return rTableContents;

                    rTableContents.Add(clsArray.detail_info[iHeader]);

                    while (clsArray.ID.Length > i)
                    {
                        if (i != iHeader) rTableContents.Add(clsArray.detail_info[i]);
                        i++;
                    }
                }

                return rTableContents;
            }
        }

        public class Function
        {
            public List<string> Fetch()
            {
                int i = 0;

                List<string> rFunctionList = new List<string>();

                dbAPI.ExecuteAPI("GET", "View", "Function List", "", "Advance Detail", "", "ViewAdvanceDetail");
                if (!clsGlobalVariables.isAPIResponseOK) return rFunctionList;

                if (!dbAPI.isNoRecordFound())
                {
                    while (clsArray.ID.Length > i)
                    {
                        if (clsArray.Description[i] != clsFunction.sDefaultSelect)
                        {
                            rFunctionList.Add(clsArray.Description[i]);
                        }

                        i++;
                    }
                }

                return rFunctionList;
            }

            public string Contents(string sFunction)
            {
                try
                {
                    using (var conn = new MySqlConnection(clsClientConnection.DbConnString()))
                    {
                        conn.Open();

                        string sDatabase;

                        using (var cmd = new MySqlCommand("SELECT DATABASE()", conn))
                        {
                            sDatabase = Convert.ToString(cmd.ExecuteScalar());
                        }

                        string sName = "`" + sFunction.Replace("`", "``") + "`";
                        string sCreate = "";

                        using (var cmd = new MySqlCommand($"SHOW CREATE FUNCTION {sName}", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Column 2 = "Create Function" (NULL when the login may not view it)
                            if (reader.Read() && !reader.IsDBNull(2))
                            {
                                object value = reader.GetValue(2);
                                sCreate = value is byte[]? Encoding.UTF8.GetString((byte[])value) : value.ToString();
                            }
                        }

                        if (sCreate.Length == 0) return "";

                        return "DELIMITER $$\n\n" +
                               $"USE `{sDatabase}`$$\n\n" +
                               $"DROP FUNCTION IF EXISTS {sName}$$\n\n" +
                               sCreate.Replace("\r\n", "\n") + "$$\n\n" +
                               "DELIMITER ;\n";
                    }
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox(ex.Message, "Query Error", clsFunction.IconType.iError);
                }

                return "";
            }
        }

        public class Routine
        {
            public List<string> Fetch()
            {
                int i = 0;

                List<string> rRoutineList = new List<string>();

                dbAPI.ExecuteAPI("GET", "View", "Routine List", "", "Advance Detail", "", "ViewAdvanceDetail");
                if (!clsGlobalVariables.isAPIResponseOK) return rRoutineList;

                if (!dbAPI.isNoRecordFound())
                {
                    while (clsArray.ID.Length > i)
                    {
                        if (clsArray.Description[i] != clsFunction.sDefaultSelect)
                        {
                            rRoutineList.Add(clsArray.Description[i]);
                        }

                        i++;
                    }
                }

                return rRoutineList;
            }

            public string Contents(string sRoutine)
            {
                try
                {
                    using (var conn = new MySqlConnection(clsClientConnection.DbConnString()))
                    {
                        conn.Open();

                        string sDatabase;

                        using (var cmd = new MySqlCommand("SELECT DATABASE()", conn))
                        {
                            sDatabase = Convert.ToString(cmd.ExecuteScalar());
                        }

                        string sName = "`" + sRoutine.Replace("`", "``") + "`";
                        string sCreate = "";

                        using (var cmd = new MySqlCommand($"SHOW CREATE PROCEDURE {sName}", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Column 2 = "Create Procedure" (NULL when the login may not view it)
                            if (reader.Read() && !reader.IsDBNull(2))
                            {
                                object value = reader.GetValue(2);
                                sCreate = value is byte[]? Encoding.UTF8.GetString((byte[])value) : value.ToString();
                            }
                        }

                        if (sCreate.Length == 0) return "";

                        return "DELIMITER $$\n\n" +
                               $"USE `{sDatabase}`$$\n\n" +
                               $"DROP PROCEDURE IF EXISTS {sName}$$\n\n" +
                               sCreate.Replace("\r\n", "\n") + "$$\n\n" +
                               "DELIMITER ;\n";
                    }
                }
                catch (Exception ex)
                {
                    dbFunction.SetMessageBox(ex.Message, "Query Error", clsFunction.IconType.iError);
                }

                return "";
            }
        }

        public class Commands
        {
            private const int SELECT_LIMIT = 1000;

            private static readonly string[] READ_KEYWORDS = { "SELECT", "SHOW", "DESCRIBE", "EXPLAIN" };
            private static readonly string[] WRITE_KEYWORDS = { "UPDATE", "INSERT", "DELETE" };

            public string RawReturn { get; private set; } = "";

            private string ConnectionString()
            {
                return clsClientConnection.DbConnString() + "AllowBatch=false;Convert Zero Datetime=True;Allow User Variables=True;";
            }

            private string errorText(Exception ex)
            {
                MySqlException mysqlEx = ex as MySqlException;
                return mysqlEx != null ? $"Error Code: {mysqlEx.Number}. {mysqlEx.Message}" : $"Error: {ex.Message}";
            }

            private bool confirmCommand(string sMessage)
            {
                Form mainForm = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;

                if (mainForm != null && mainForm.InvokeRequired)
                {
                    return (bool)mainForm.Invoke(new Func<bool>(() => dbFunction.fPromptConfirmation(sMessage)));
                }

                return dbFunction.fPromptConfirmation(sMessage);
            }

            private bool runModify(string sQuery)
            {
                try
                {
                    int iPreview = executeModify(sQuery, -1);

                    if (!confirmCommand($"{iPreview:N0} row(s) will be affected.\n\nCommit this change?"))
                    {
                        RawReturn = "Cancelled. Nothing was saved.";
                        return false;
                    }

                    int iAffected = executeModify(sQuery, iPreview);
                    if (iAffected < 0)
                    {
                        RawReturn = "Row count changed since the preview. Nothing was saved, please run again.";
                        return false;
                    }

                    RawReturn = $"{iAffected:N0} row(s) saved.";
                    return true;
                }
                catch (Exception ex)
                {
                    RawReturn = errorText(ex);
                    return false;
                }
            }

            private int executeModify(string sQuery, int iExpected)
            {
                using (var conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand("SET SESSION sql_safe_updates = 1", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        int iAffected;

                        using (var cmd = new MySqlCommand(sQuery, conn, transaction))
                        {
                            cmd.CommandTimeout = 60;
                            iAffected = cmd.ExecuteNonQuery();
                        }

                        if (iExpected < 0 || iAffected != iExpected)
                        {
                            transaction.Rollback();
                            return iExpected < 0 ? iAffected : -1;
                        }

                        transaction.Commit();
                        return iAffected;
                    }
                }
            }

            private bool runSelect(string sQuery, out DataTable dt)
            {
                dt = new DataTable();

                try
                {
                    using (var conn = new MySqlConnection(ConnectionString()))
                    {
                        conn.Open();

                        using (var cmd = new MySqlCommand($"SET SESSION sql_select_limit = {SELECT_LIMIT}", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (var adapter = new MySqlDataAdapter(sQuery, conn))
                        {
                            adapter.SelectCommand.CommandTimeout = 60;
                            adapter.Fill(dt);
                        }
                    }

                    RawReturn = $"{dt.Rows.Count:N0} row(s) returned.";
                    return true;
                }
                catch (Exception ex)
                {
                    RawReturn = errorText(ex);
                    return false;
                }
            }

            private bool runOther(string sQuery, string sKeyword, out DataTable dt)
            {
                dt = null;

                if (!confirmCommand($"[{sKeyword}] runs immediately and cannot be rolled back.\n\nRun this statement?"))
                {
                    RawReturn = "Cancelled. Nothing was run.";
                    return false;
                }

                try
                {
                    DataTable dtResult = new DataTable();

                    using (var conn = new MySqlConnection(ConnectionString()))
                    {
                        conn.Open();

                        using (var adapter = new MySqlDataAdapter(sQuery, conn))
                        {
                            adapter.SelectCommand.CommandTimeout = 60;
                            adapter.Fill(dtResult);
                        }
                    }

                    // Statements like CALL can return rows; DDL returns none
                    if (dtResult.Columns.Count > 0)
                    {
                        dt = dtResult;
                        RawReturn = $"[{sKeyword}] executed. {dtResult.Rows.Count:N0} row(s) returned.";
                    }
                    else
                    {
                        RawReturn = $"[{sKeyword}] executed.";
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RawReturn = errorText(ex);
                    return false;
                }
            }

            private List<string> splitScript(string sScript)
            {
                List<string> statements = new List<string>();
                StringBuilder sb = new StringBuilder();
                string sDelimiter = ";";

                foreach (string sRawLine in sScript.Replace("\r\n", "\n").Split('\n'))
                {
                    string sLine = sRawLine.TrimEnd();
                    string sTrimmed = sLine.Trim();

                    if (sTrimmed.StartsWith("DELIMITER", StringComparison.OrdinalIgnoreCase) && sb.ToString().Trim().Length == 0)
                    {
                        string sNew = sTrimmed.Substring("DELIMITER".Length).Trim();
                        sDelimiter = sNew.Length > 0 ? sNew : ";";
                        continue;
                    }

                    if (sLine.EndsWith(sDelimiter))
                    {
                        sb.AppendLine(sLine.Substring(0, sLine.Length - sDelimiter.Length));

                        string sStatement = sb.ToString().Trim();
                        if (sStatement.Length > 0) statements.Add(sStatement);

                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(sLine);
                    }
                }

                string sLast = sb.ToString().Trim();
                if (sLast.Length > 0) statements.Add(sLast);

                return statements;
            }

            private string readDefinition(MySqlConnection conn, string sType, string sName)
            {
                try
                {
                    using (var cmd = new MySqlCommand($"SHOW CREATE {sType} {sName}", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(2))
                        {
                            object value = reader.GetValue(2);
                            return value is byte[]? Encoding.UTF8.GetString((byte[])value) : value.ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    // Does not exist yet = nothing to back up
                }

                return null;
            }

            private bool runScript(string sScript)
            {
                List<string> statements = splitScript(sScript);
                if (statements.Count == 0) return false;

                if (!confirmCommand($"This script has {statements.Count:N0} statement(s). They run immediately and cannot be rolled back.\n\nRun this script?"))
                {
                    RawReturn = "Cancelled. Nothing was run.";
                    return false;
                }

                StringBuilder sbLog = new StringBuilder();
                string sBackup = null;
                bool fDropped = false;
                int iStatement = 0;

                try
                {
                    using (var conn = new MySqlConnection(ConnectionString()))
                    {
                        conn.Open();

                        foreach (string sStatement in statements)
                        {
                            iStatement++;

                            string sFirstLine = sStatement.Split('\n')[0].Trim();

                            // Before a DROP FUNCTION / DROP PROCEDURE, keep a copy of the current code
                            Match drop = Regex.Match(sStatement, @"^\s*DROP\s+(FUNCTION|PROCEDURE)\s+(IF\s+EXISTS\s+)?(.+?)\s*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                            if (drop.Success)
                            {
                                sBackup = readDefinition(conn, drop.Groups[1].Value, drop.Groups[3].Value);
                            }

                            try
                            {
                                using (var cmd = new MySqlCommand(sStatement, conn))
                                {
                                    cmd.CommandTimeout = 60;
                                    cmd.ExecuteNonQuery();
                                }

                                sbLog.AppendLine($"[{iStatement}] OK      {sFirstLine}");

                                if (drop.Success) fDropped = true;

                                // The routine is back in place, so the copy is no longer needed
                                if (sStatement.TrimStart().StartsWith("CREATE", StringComparison.OrdinalIgnoreCase))
                                {
                                    fDropped = false;
                                    sBackup = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                sbLog.AppendLine($"[{iStatement}] FAILED  {sFirstLine}");
                                sbLog.AppendLine(errorText(ex));
                                sbLog.AppendLine("Script stopped. Later statements were not run.");

                                if (fDropped && sBackup != null)
                                {
                                    try
                                    {
                                        using (var cmd = new MySqlCommand(sBackup, conn))
                                        {
                                            cmd.CommandTimeout = 60;
                                            cmd.ExecuteNonQuery();
                                        }

                                        sbLog.AppendLine();
                                        sbLog.AppendLine("The original routine was restored. Nothing changed.");
                                    }
                                    catch (Exception exRestore)
                                    {
                                        sbLog.AppendLine();
                                        sbLog.AppendLine("RESTORE FAILED - THE ROUTINE NO LONGER EXISTS. " + errorText(exRestore));
                                        sbLog.AppendLine("Original code, keep this:");
                                        sbLog.AppendLine();
                                        sbLog.AppendLine(sBackup);
                                    }
                                }

                                RawReturn = sbLog.ToString();
                                return false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sbLog.AppendLine(errorText(ex));
                    RawReturn = sbLog.ToString();
                    return false;
                }

                RawReturn = sbLog.ToString();
                return true;
            }

            public bool Run(string sQuery, out DataTable dt)
            {
                dt = null;
                RawReturn = "";

                sQuery = (sQuery ?? "").Trim();
                if (sQuery.Length == 0) 
                {
                    return false;
                }

                if (Regex.IsMatch(sQuery, @"^\s*DELIMITER\b", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    return runScript(sQuery);
                }

                if (!sQuery.EndsWith(";"))
                {
                    RawReturn = "Missing terminator. End the statement with a semicolon ( ; ).";
                    return false;
                }

                sQuery = sQuery.TrimEnd(';').Trim();
                if (sQuery.Length == 0)
                {
                    return false;
                }

                string sKeyword = sQuery.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpper();

                if (Array.IndexOf(READ_KEYWORDS, sKeyword) >= 0)
                {
                    return runSelect(sQuery, out dt);
                }

                if (Array.IndexOf(WRITE_KEYWORDS, sKeyword) >= 0)
                {
                    return runModify(sQuery);
                }

                return runOther(sQuery, sKeyword, out dt);
            }
        }
    }
}
