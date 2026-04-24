using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MIS.AppConnection
{
    class ServerDatabase
    {
        private string ConnString;

        public ServerDatabase()
        {
            // Get Connection String
            ConnString = clsClientConnection.DbConnString();
        }

        public string Sprocedure { get; set; }
        public string Statement { get; set; }
        public string SearchBy { get; set; }
        public string SearchVal { get; set; }
        public string Sql { get; set; }

        // MySql Connection
        public async Task<MySqlConnection> AsyncMySqlConnect()
        {
            try
            {
                var Conn = new MySqlConnection(ConnString);
                await Conn.OpenAsync();
                return Conn;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening connection: {ex.Message}");
                throw;
            }
        }

        // Procedure
        public async Task<DataTable> ExeAsyncProcedure()
        {
            Debug.WriteLine($"\nExecuting Stored Procedure: {Sprocedure}");
            Debug.WriteLine($"Call {Sprocedure}(\"{Statement}\",\"{SearchBy}\",\"{SearchVal}\");\n");

            try
            {
                using (var Conn    = await AsyncMySqlConnect()) // Select Connection to be used
                using (var Cmd     = new MySqlCommand(Sprocedure, Conn)) // Create Command
                using (var Adapter = new MySqlDataAdapter(Cmd)) // Call Command
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@p_StatementType", Statement);
                    Cmd.Parameters.AddWithValue("@p_SearchBy",      SearchBy);
                    Cmd.Parameters.AddWithValue("@p_SearchValue",   SearchVal);
                    Cmd.Parameters.AddWithValue("@p_Sql", Sql);
                    Cmd.CommandTimeout = 600;
                    //Cmd.CommandTimeout = 1800; // Setting command timeout to 1800 seconds (30 mins)

                    var Dt = new DataTable();
                    await Task.Run(() => Adapter.Fill(Dt));
                    return Dt;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<DataTable> ExeProcedure()
        {
            Debug.WriteLine($"\nExecuting Stored Procedure: {Sprocedure}");
            Debug.WriteLine($"Call {Sprocedure}(\"{Statement}\",\"{SearchBy}\",\"{SearchVal}\");\n");

            try
            {
                using (var Conn = await AsyncMySqlConnect()) // Select Connection to be used
                using (var Cmd = new MySqlCommand(Sprocedure, Conn)) // Create Command
                using (var Adapter = new MySqlDataAdapter(Cmd)) // Call Command
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@p_Statement", Statement);
                    Cmd.Parameters.AddWithValue("@p_SearchBy", SearchBy);
                    Cmd.Parameters.AddWithValue("@p_SearchVal", SearchVal);
                    Cmd.Parameters.AddWithValue("@p_Sql", Sql);
                    Cmd.CommandTimeout = 600;
                    //Cmd.CommandTimeout = 1800; // Setting command timeout to 1800 seconds (30 mins)

                    var Dt = new DataTable();
                    await Task.Run(() => Adapter.Fill(Dt));
                    return Dt;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
