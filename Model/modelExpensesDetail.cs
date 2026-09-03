using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class modelExpensesDetail
    {
        public int DetailID { get; set; }
        public int ExpensesNo { get; set; }
        public string ExpensesReferenceNo { get; set; }
        public int ServiceNo { get; set; }
        public int ExpensesID { get; set; }
        public string ExpensesType { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string ExpensesDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public int TAIDNo { get; set; }
        public int IRIDNo { get; set; }

        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
    }
}
