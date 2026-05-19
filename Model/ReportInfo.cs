using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS.Model
{
    public class ReportInfo
    {
        public ListView ListView { get; set; }
        public string SheetName { get; set; }      // Tab name
        public string Title { get; set; }          // Header title
        public string RecordCount { get; set; }          // Record Count
        public string TransCount { get; set; }          // Trans Count
        public string Total { get; set; }          // Total amount
    }

}
