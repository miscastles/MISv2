using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    internal class modelLastServiceAttempt
    {
        public int ServiceNo { get; set; }
        public string RequestID { get; set; }
        public string ActionMade { get; set; }
        public string Reason { get; set; }
        public string FSRDate { get; set; }
        public string RequestDate { get; set; }
        public string ScheduleDate { get; set; }
        public string Dependency { get; set; }
        public string StatusReason { get; set; }
        public string Remarks { get; set; }
        public int FunctionID { get; set; }
        public bool IsMerchantReschedule { get; set; }
    }
}