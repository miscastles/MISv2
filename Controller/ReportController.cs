using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class ReportController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int ReportID { get; set; }
        public string ReportDesc { get; set; }

        private ReportController setInitValue()
        {
            ReportController model = new ReportController();

            model.ReportID = ReportID;
            model.ReportDesc = ReportDesc;

            return model;
        }
    }
}
