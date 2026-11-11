using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MIS.Model
{
    public class modelSerialSearch
    {
        private static clsFile dbFile = new clsFile();

        private const int ROW_LIMIT = 200;
        private const int MIN_LENGTH = 3;
        private const uint CONNECT_TIMEOUT = 10000;

        // Criteria
        public string SerialType { get; set; } = ""; 
        public string Algorithm { get; set; } = ""; 
        public string SearchValue { get; set; } = "";
        public List<Bank> Banks { get; set; } = new List<Bank>();

        // Output
        public List<Result> Results { get; private set; } = new List<Result>();
        public List<string> Errors { get; private set; } = new List<string>();

        public class Bank
        {
            public string Code { get; set; } = "";
            public string Name { get; set; } = "";
        }

        public class Result
        {
            public string BankCode { get; set; } = "";
            public string Bank { get; set; } = "";
            public string Database { get; set; } = "";
            public string SerialNo { get; set; } = "";
            public string Model { get; set; } = "";     // carrier, for a SIM
            public string Status { get; set; } = "";
            public string Location { get; set; } = "";
        }

        public bool Search()
        {
            Results.Clear();
            Errors.Clear();

            string sValue = (SearchValue ?? "").Trim();

            if (sValue.Length < MIN_LENGTH)
            {
                Errors.Add($"Search value must be at least {MIN_LENGTH} characters.");
                return false;
            }

            clsINI dbSetting = new clsINI();
            string sPath = dbFile.sSettingPath + clsDefines.RESP_DATABASELIST_FILENAME;

            foreach (Bank bank in Banks)
            {
                try
                {
                    clsDatabase db = dbSetting.GetDatabaseByCode(sPath, bank.Code);

                    if (db == null)
                    {
                        Errors.Add($"{bank.Name}: database setting not found.");
                        continue;
                    }

                    searchDatabase(bank, db, sValue);
                }
                catch (Exception ex)
                {
                    Errors.Add($"{bank.Name}: {ex.Message}");
                }
            }

            return Results.Count > 0;
        }

        private void searchDatabase(Bank bank, clsDatabase db, string sValue)
        {
            string sQuery = SerialType == "SIMSN"
                ? "SELECT SIMSN AS SerialNo, Carrier AS Model, SIMStatusDescription AS Status, Location " +
                  "FROM tblsimdetail WHERE SIMSN LIKE @pattern ORDER BY SIMSN LIMIT " + ROW_LIMIT
                : "SELECT TerminalSN AS SerialNo, Model, TerminalStatusDescription AS Status, Location " +
                  "FROM tblterminaldetail WHERE TerminalSN LIKE @pattern ORDER BY TerminalSN LIMIT " + ROW_LIMIT;

            using (var conn = new MySqlConnection(connectionString(db)))
            {
                conn.Open();

                using (var cmd = new MySqlCommand(sQuery, conn))
                {
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@pattern", likePattern(sValue));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Results.Add(new Result
                            {
                                BankCode = bank.Code,
                                Bank = bank.Name,
                                Database = db.Database,
                                SerialNo = Convert.ToString(reader["SerialNo"]),
                                Model = Convert.ToString(reader["Model"]),
                                Status = Convert.ToString(reader["Status"]),
                                Location = Convert.ToString(reader["Location"])
                            });
                        }
                    }
                }
            }
        }

        private string likePattern(string sValue)
        {
            // Match the typed text literally; only the search type adds wildcards
            string s = sValue.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

            switch (Algorithm)
            {
                case "EXACT": return s;
                case "STARTS WITH": return s + "%";
                case "ENDS WITH": return "%" + s;
                default: return "%" + s + "%";    // CONTAINS
            }
        }

        private string connectionString(clsDatabase db)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = db.Server,
                Port = (uint)db.Port,
                Database = db.Database,
                UserID = db.Username,
                Password = db.Password,
                ConnectionTimeout = CONNECT_TIMEOUT
            };

            return builder.ConnectionString;
        }
    }
}