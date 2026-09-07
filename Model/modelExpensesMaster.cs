using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class modelExpensesMaster
    {
        public int ExpensesNo { get; set; }
        public string ExpensesType { get; set; }
        public int ServiceNo { get; set; }
        public int IRIDNo { get; set; }
        public int MerchantID { get; set; }
        public string MerchantName { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime ExpensesDate { get; set; }
        public int FEID { get; set; }
        public string FEName { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string Location { get; set; }
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public string ServiceNoList { get; set; }
        public string IRIDNoList { get; set; }
        public string IRNoList { get; set; }
        public string ReceiptList { get; set; }
        
    }
}
