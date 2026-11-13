using System;
using System.Collections.Generic;
using System.Linq;

namespace MIS.Model
{
    public class modelSerialSearch
    {
        private static clsAPI dbAPI = new clsAPI();

        private const int MIN_LENGTH = 3;

        // ---------------------------------------------------------------
        // Serial types. To support a new one, add an entry here.
        // ---------------------------------------------------------------
        private class SerialTypeDef
        {
            public string Name;
            public string SearchBy;
            public string Maintenance;
            public string Action;
            public string HistorySearchBy;              // API p_SearchBy for the service history
            public string ModelLabel;                   // caption for the Model value
            public Func<string, string> SearchValue;
            public Func<string[]> ID;
            public Func<string[]> SerialNo;
            public Func<string[]> Model;
            public Func<string[]> Status;
            public Func<string[]> Location;
        }

        private static readonly List<SerialTypeDef> SerialTypes = new List<SerialTypeDef>
        {
            new SerialTypeDef
            {
                Name = "TERMINALSN",
                SearchBy = "TerminalSN List",
                Maintenance = "Terminal",
                Action = "ViewAdvanceTerminal",
                HistorySearchBy = "Service TerminalSN History List",
                ModelLabel = "MODEL",
                // serial | status | type | page | page size | location id | location  (0 = no filter)
                SearchValue = s => string.Join(clsFunction.sPipe, s, "0", "0", "0", "0", "0", "0"),
                ID = () => clsArray.TerminalID,
                SerialNo = () => clsArray.SerialNo,
                Model = () => clsArray.TerminalModel,
                Status = () => clsArray.TerminalStatusDescription,
                Location = () => clsArray.Location
            },
            new SerialTypeDef
            {
                Name = "SIMSN",
                SearchBy = "SIMSN List",
                Maintenance = "Terminal",
                Action = "ViewAdvanceTerminal",
                HistorySearchBy = "Service SIMSN History List",
                ModelLabel = "CARRIER",
                // serial | status | page | page size | location id | location  (0 = no filter)
                SearchValue = s => string.Join(clsFunction.sPipe, s, "0", "0", "0", "0", "0"),
                ID = () => clsArray.SIMID,
                SerialNo = () => clsArray.SerialNo,
                Model = () => clsArray.SIMCarrier,
                Status = () => clsArray.SIMStatusDescription,
                Location = () => clsArray.Location
            }
        };

        public static string[] TypeNames
        {
            get { return SerialTypes.Select(t => t.Name).ToArray(); }
        }

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
            public string ID { get; set; } = "";
            public string SerialNo { get; set; } = "";
            public string Model { get; set; } = "";
            public string Status { get; set; } = "";
            public string Location { get; set; } = "";
            public string HistorySearchBy { get; set; } = "";
            public string ModelLabel { get; set; } = "";
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

            SerialTypeDef type = SerialTypes.FirstOrDefault(t => t.Name.Equals(SerialType, StringComparison.OrdinalIgnoreCase));

            if (type == null)
            {
                Errors.Add($"Search type [{SerialType}] is not supported.");
                return false;
            }

            string sHoldBank = clsGlobalVariables.strAPIBank;
            bool fHoldPrompt = clsAPI.SuppressErrorPrompts;

            try
            {
                // One failing bank should be listed, not pop a dialog for each bank
                clsAPI.SuppressErrorPrompts = true;

                foreach (Bank bank in Banks)
                {
                    try
                    {
                        clsGlobalVariables.strAPIBank = bank.Code;
                        searchBank(bank, type, sValue);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add($"{bank.Name}: {ex.Message}");
                    }
                }
            }
            finally
            {
                clsGlobalVariables.strAPIBank = sHoldBank;
                clsAPI.SuppressErrorPrompts = fHoldPrompt;
            }

            return Results.Count > 0;
        }

        private void searchBank(Bank bank, SerialTypeDef type, string sValue)
        {
            dbAPI.ExecuteAPI("GET", "View", type.SearchBy, type.SearchValue(sValue), type.Maintenance, "", type.Action);

            if (!clsGlobalVariables.isAPIResponseOK)
            {
                Errors.Add($"{bank.Name}: no valid response from the API.");
                return;
            }

            if (dbAPI.isNoRecordFound()) return;

            // Read the arrays now: the next API call replaces them
            string[] ids = type.ID();
            string[] serials = type.SerialNo();
            string[] models = type.Model();
            string[] statuses = type.Status();
            string[] locations = type.Location();

            for (int i = 0; i < serials.Length; i++)
            {
                string sSerialNo = (serials[i] ?? "").Trim();

                if (!matches(sSerialNo, sValue)) continue;

                Results.Add(new Result
                {
                    BankCode = bank.Code,
                    Bank = bank.Name,
                    ID = ids[i],
                    SerialNo = sSerialNo,
                    Model = models[i],
                    Status = statuses[i],
                    Location = locations[i],
                    HistorySearchBy = type.HistorySearchBy,
                    ModelLabel = type.ModelLabel
                });
            }
        }

        private bool matches(string sSerialNo, string sValue)
        {
            StringComparison cmp = StringComparison.OrdinalIgnoreCase;

            switch (Algorithm)
            {
                case "EXACT": return sSerialNo.Equals(sValue, cmp);
                case "STARTS WITH": return sSerialNo.StartsWith(sValue, cmp);
                case "ENDS WITH": return sSerialNo.EndsWith(sValue, cmp);
                default: return sSerialNo.IndexOf(sValue, cmp) >= 0;    // CONTAINS
            }
        }
    }
}