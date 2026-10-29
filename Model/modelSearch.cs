using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public static class modelSearch
    {
        // Search
        public static int ClientID { get; set; }
        public static int UserID { get; set; }
        public static int ParticularID { get; set; }
        public static int ServiceNo { get; set; }
        public static int FSRNo { get; set; }
        public static int MerchantID { get; set; }
        public static int AssistNo { get; set; }
        public static int ProblemNo { get; set; }
        public static string TID { get; set; }
        public static string MID { get; set; }
        public static string Merchant { get; set; }
        public static string ParticularName { get; set; }
        public static int IRIDNo { get; set; }
        public static string IRNo { get; set; }

        // IR
        public static string RequestDate { get; set; }
        public static string InstallationDate { get; set; }
        public static string RequestType { get; set; }
        public static string Instruction { get; set; }
        public static string Setup { get; set; }
        public static string RequestFor { get; set; }
        public static string Comments { get; set; }
        public static string Remarks { get; set; }
        public static string AppVersion { get; set; }
        public static string AppCRC { get; set; }

        // Merchant
        public static string Address { get; set; }
        public static string Province { get; set; }
        public static string Region { get; set; }
        public static string ContactPerson { get; set; }
        public static string TelNo { get; set; }
        public static string MobileNo { get; set; }
        public static string Email { get; set; }
        public static string PrimaryNum { get; set; }
        public static string SecondaryNum { get; set; }
        public static int ZoneID { get; set; }

        // POS
        public static int RentalFeeID { get; set; }
        public static string POSType { get; set; }
        public static string RentalFee { get; set; }

        // JSON info
        public static string ProfileDataInfo { get; set; }
        public static string RawDataInfo { get; set; }
        public static string ProfileConfigInfo { get; set; }

        // Current
        public static int TerminalID { get; set; }
        public static string TerminalSN { get; set; }
        public static int SIMID { get; set; }
        public static string SIMSN { get; set; }

        // Replace
        public static int ReplaceTerminalID { get; set; }
        public static string ReplaceTerminalSN { get; set; }
        public static int ReplaceSIMID { get; set; }
        public static string ReplaceSIMSN { get; set; }

        // Service
        public static int JobType { get; set; }
        public static string ActionMade { get; set; }

        // Hold
        public static string PreviousData { get; private set; } = "{}";
        public class HoldData
        {
            // Search
            public int ClientID { get; set; }
            public int ParticularID { get; set; }
            public string TID { get; set; } = "";
            public string MID { get; set; } = "";
            public string Merchant { get; set; } = "";
            public string ParticularName { get; set; } = "";
            public int IRIDNo { get; set; }
            public string IRNo { get; set; } = "";

            // IR
            public string RequestDate { get; set; } = "";
            public string InstallationDate { get; set; } = "";
            public string RequestType { get; set; } = "";
            public string Instruction { get; set; } = "";
            public string Setup { get; set; } = "";
            public string RequestFor { get; set; } = "";
            public string Comments { get; set; } = "";
            public string Remarks { get; set; } = "";
            public string AppVersion { get; set; } = "";
            public string AppCRC { get; set; } = "";

            // Merchant
            public string Address { get; set; } = "";
            public string Province { get; set; } = "";
            public string Region { get; set; } = "";
            public string ContactPerson { get; set; } = "";
            public string TelNo { get; set; } = "";
            public string MobileNo { get; set; } = "";
            public string Email { get; set; } = "";
            public string PrimaryNum { get; set; } = "";
            public string SecondaryNum { get; set; } = "";
            public int ZoneID { get; set; }

            // POS
            public int RentalFeeID { get; set; }
            public string POSType { get; set; } = "";
            public string RentalFee { get; set; } = "";

            // JSON info
            public string ProfileDataInfo { get; set; } = "";
            public string RawDataInfo { get; set; } = "";
            public string ProfileConfigInfo { get; set; } = "";

            // Current
            public int TerminalID { get; set; }
            public string TerminalSN { get; set; } = "";
            public int SIMID { get; set; }
            public string SIMSN { get; set; } = "";

            // Replace
            public int ReplaceTerminalID { get; set; }
            public string ReplaceTerminalSN { get; set; } = "";
            public int ReplaceSIMID { get; set; }
            public string ReplaceSIMSN { get; set; } = "";

            // Service
            public int JobType { get; set; }
            public string ActionMade { get; set; } = "";

            public static void Save(HoldData data)
            {
                data = data ?? new HoldData();

                if (!string.IsNullOrEmpty(data.TID)) data.TID = data.TID.Trim().PadLeft(clsFunction.TID_LENGTH, '0');
                if (!string.IsNullOrEmpty(data.MID)) data.MID = data.MID.Trim().PadLeft(clsFunction.MID_LENGTH, '0');

                // only write fields that were set
                PreviousData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                Debug.WriteLine("PreviousData=" + PreviousData);
            }
        }

        public static void HoldClear()
        {
            PreviousData = "{}";
        }

        public static void DebugSearch()
        {
            Debug.WriteLine("========== modelSearch ==========");

            // Search
            Debug.WriteLine("----- Search -----");
            Debug.WriteLine($"ClientID       : {ClientID}");
            Debug.WriteLine($"UserID         : {UserID}");
            Debug.WriteLine($"ParticularID   : {ParticularID}");
            Debug.WriteLine($"ParticularName : {ParticularName}");
            Debug.WriteLine($"ServiceNo      : {ServiceNo}");
            Debug.WriteLine($"FSRNo          : {FSRNo}");
            Debug.WriteLine($"MerchantID     : {MerchantID}");
            Debug.WriteLine($"Merchant       : {Merchant}");
            Debug.WriteLine($"AssistNo       : {AssistNo}");
            Debug.WriteLine($"ProblemNo      : {ProblemNo}");
            Debug.WriteLine($"TID            : {TID}");
            Debug.WriteLine($"MID            : {MID}");
            Debug.WriteLine($"IRIDNo         : {IRIDNo}");
            Debug.WriteLine($"IRNo           : {IRNo}");

            // IR
            Debug.WriteLine("----- IR -----");
            Debug.WriteLine($"RequestDate      : {RequestDate}");
            Debug.WriteLine($"InstallationDate : {InstallationDate}");
            Debug.WriteLine($"RequestType      : {RequestType}");
            Debug.WriteLine($"Instruction      : {Instruction}");
            Debug.WriteLine($"Setup            : {Setup}");
            Debug.WriteLine($"RequestFor       : {RequestFor}");
            Debug.WriteLine($"Comments         : {Comments}");
            Debug.WriteLine($"Remarks          : {Remarks}");
            Debug.WriteLine($"AppVersion       : {AppVersion}");
            Debug.WriteLine($"AppCRC           : {AppCRC}");

            // Merchant
            Debug.WriteLine("----- Merchant -----");
            Debug.WriteLine($"Address        : {Address}");
            Debug.WriteLine($"Province       : {Province}");
            Debug.WriteLine($"Region         : {Region}");
            Debug.WriteLine($"ContactPerson  : {ContactPerson}");
            Debug.WriteLine($"TelNo          : {TelNo}");
            Debug.WriteLine($"MobileNo       : {MobileNo}");
            Debug.WriteLine($"Email          : {Email}");
            Debug.WriteLine($"PrimaryNum     : {PrimaryNum}");
            Debug.WriteLine($"SecondaryNum   : {SecondaryNum}");
            Debug.WriteLine($"ZoneID         : {ZoneID}");

            // POS
            Debug.WriteLine("----- POS -----");
            Debug.WriteLine($"RentalFeeID    : {RentalFeeID}");
            Debug.WriteLine($"POSType        : {POSType}");
            Debug.WriteLine($"RentalFee      : {RentalFee}");

            // JSON info
            Debug.WriteLine("----- JSON Info -----");
            Debug.WriteLine($"ProfileDataInfo   : {ProfileDataInfo}");
            Debug.WriteLine($"RawDataInfo       : {RawDataInfo}");
            Debug.WriteLine($"ProfileConfigInfo : {ProfileConfigInfo}");

            // Current
            Debug.WriteLine("----- Current -----");
            Debug.WriteLine($"TerminalID     : {TerminalID}");
            Debug.WriteLine($"TerminalSN     : {TerminalSN}");
            Debug.WriteLine($"SIMID          : {SIMID}");
            Debug.WriteLine($"SIMSN          : {SIMSN}");

            // Replace
            Debug.WriteLine("----- Replace -----");
            Debug.WriteLine($"ReplaceTerminalID : {ReplaceTerminalID}");
            Debug.WriteLine($"ReplaceTerminalSN : {ReplaceTerminalSN}");
            Debug.WriteLine($"ReplaceSIMID      : {ReplaceSIMID}");
            Debug.WriteLine($"ReplaceSIMSN      : {ReplaceSIMSN}");

            // Service
            Debug.WriteLine("----- Service -----");
            Debug.WriteLine($"JobType        : {JobType}");
            Debug.WriteLine($"ActionMade     : {ActionMade}");

            // Held data
            Debug.WriteLine("---- Hold Data ----");
            Debug.WriteLine($"Hold Data : {PreviousData}");

            Debug.WriteLine("=================================");
        }
    }
}