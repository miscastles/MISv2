using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class modelDashboard
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string Link { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserKey { get; set; }
        public string Folder { get; set; }
        public string StatementType { get; set; }
        public string SearchBy { get; set; }
        public string SearchValue { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public int MerchantID { get; set; }
        public int FEID { get; set; }
        public int DispatchID { get; set; }
        public int UserID { get; set; }
        public int ParticularID { get; set; }
        public int TCount { get; set; }
    }
}
