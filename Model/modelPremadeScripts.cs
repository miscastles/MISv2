using System.Collections.Generic;

namespace MIS.Model
{
    public static class modelPremadeScripts
    {
        public static readonly List<(string Name, string Query)> Get = new List<(string Name, string Query)>
        {
            ("TERMINAL BY SN",          "SELECT * FROM tblterminaldetail WHERE TerminalSN = '{SN}';"),
            ("UPDATE TERMINAL REMARKS", "UPDATE tblterminaldetail SET Remarks = '{REMARKS}' WHERE TerminalSN = '{SN}' LIMIT 1;"),
            ("SIM BY SN",               "SELECT * FROM tblsimdetail WHERE SIMSN = '{SN}';"),
            ("IR BY TID / MID",         "SELECT * FROM tblirdetail WHERE TID = '{TID}' AND MID = '{MID}';"),
            ("SERVICING BY SERVICE NO", "SELECT * FROM tblservicingdetail WHERE ServiceNo = {SERVICE_NO};"),
            ("CLEAR MERCHANT TESTING", "CALL spDeleteCollectionDetail('Delete', '', '1|1|XYZ MERCH', 'Clear Merchant Testing');"),        };
    }
}