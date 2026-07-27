using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    internal class modelServicingActivityDetail
    {
        public int IRIDNo { get; set; }
        public int ServiceNo { get; set; }
        public int MerchantID { get; set; }
        public int ActivityID { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int ParticularID { get; set; }
        public string ParticularName { get; set; }
    }
}
