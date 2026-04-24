using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Model
{
    public class modelStock
    {
        public int ItemID { get; set; }
        public int StockID { get; set; }
        public int UoMID { get; set; }
        public int ClientID { get; set; }
        public int LocationID { get; set; }
        public int TerminalTypeID { get; set; }
        public int TerminalModelID { get; set; }
        public int TerminalBrandID { get; set; }
        public int StockStatus { get; set; }
        public string SerialNo { get; set; }
        public string DeliveryDate { get; set; }
        public string ReceiveDate { get; set; }
        public string ReleaseDate { get; set; }
        public string Allocation { get; set; }
        public string AssetType { get; set; }
        public string PONo { get; set; }
        public string InvoiceNo { get; set; }
        public string PartNo { get; set; }
        public string DateTimeStamp { get; set; }
    }
}
