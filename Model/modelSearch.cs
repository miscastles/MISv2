using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Model
{
    public static class modelSearch
    {
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

            Debug.WriteLine("=================================");
        }

    }
}
