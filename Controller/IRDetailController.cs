using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS.Controller
{
    public class IRDetailController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int IRIDNo { get; set; }
        public string IRNo { get; set; }
        public int MerchantID { get; set; }
        public int ClientID { get; set; }
        public string TID { get; set; }
        public string MID { get; set; }
        public string ClientName { get; set; }
        public string MerchantName { get; set; }
        public string IRDate { get; set; }
        public string InstallationDate { get; set; }
        public int IRStatus { get; set; }
        public string IRStatusDescription { get; set; }
        public string ProcessType { get; set; }
        public string ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Requestor { get; set; }
        public int RequestTypeID { get; set; }
        public string RequestType { get; set; }
        public int IRActive { get; set; }

        // Terminal
        public int TerminalID { get; set; }
        public string TerminalSN { get; set; }
        public string TerminalType { get; set; }
        public string TerminalModel { get; set; }
        public string TerminalLocation { get; set; }
        public string TerminalStatus { get; set; }

        // SIM
        public int SIMID { get; set; }
        public string SIMSN { get; set; }
        public string SIMCarrier { get; set; }
        public string SIMLocation { get; set; }
        public string SIMStatus { get; set; }

        public string Address { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string AppVersion { get; set; }
        public string AppCRC { get; set; }

        public string DateTimeStamp { get; set; }

        private IRDetailController setInitValue()
        {
            IRDetailController model = new IRDetailController();

            model.IRIDNo = 0;
            model.IRNo = "";
            model.MerchantID = 0;
            model.ClientID = 0;
            model.TID = "";
            model.MID = "";
            model.ClientName = "";
            model.MerchantName = "";
            model.IRDate = "";
            model.InstallationDate = "";
            model.IRStatus = 0;
            model.IRStatusDescription = "";
            model.IRActive = 0;
            model.ProcessedDate = "";
            model.ProcessedBy = "";
            model.ModifiedDate = "";
            model.ModifiedBy = "";
            model.Requestor = "";
            model.RequestType = "";
            model.RequestTypeID = 0;

            model.TerminalID = 0;
            model.TerminalSN = "";
            model.TerminalType = "";
            model.TerminalModel = "";
            model.TerminalLocation = "";
            model.TerminalStatus = "";

            model.SIMID = 0;
            model.SIMSN = "";
            model.SIMCarrier = "";
            model.SIMLocation = "";
            model.SIMStatus = "";

            model.Address = "";
            model.Region = "";
            model.Province = "";
            model.AppVersion = "";
            model.AppCRC = "";
            model.DateTimeStamp = "";

            return model;
        }

        public IRDetailController getInfo(string pSearvhValue)
        {
            IRDetailController model = new IRDetailController();

            if (dbFunction.isValidDescription(pSearvhValue.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant IR Info", pSearvhValue);

                try
                {
                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.IRIDNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNo));
                        model.IRNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO);
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantID));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientID));
                        model.TID = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID);
                        model.MID = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID);
                        model.ClientName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                        model.MerchantName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME);
                        model.IRDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate);
                        model.InstallationDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReqInstallationDate);
                        model.IRStatus = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRStatus));
                        model.IRStatusDescription = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRStatusDescription);
                        model.IRActive = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRActive));
                        model.ProcessedDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ProcessedAt);
                        model.ProcessedBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ProcessedBy);
                        model.ModifiedDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ModifiedAt);
                        model.ModifiedBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ModifiedBy);
                        model.Requestor = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Requestor);
                        model.RequestType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestType);
                        model.RequestTypeID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestTypeID));

                        model.TerminalID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalID));
                        model.TerminalSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalSN);
                        model.TerminalType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType);
                        model.TerminalModel = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel);
                        model.TerminalLocation = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalLocation);
                        model.TerminalStatus = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalStatus);

                        model.SIMID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMID));
                        model.SIMSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMSN);
                        model.SIMCarrier = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMCarrier);
                        model.SIMLocation = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMLocation);
                        model.SIMStatus = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMStatus);

                        model.DateTimeStamp = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DateTimeStamp);

                    }
                }
                catch (Exception ex)
                {
                    model = setInitValue();
                    Debug.WriteLine("Exceptional error " + ex.Message);
                }
            }

            // Return the filled model
            return model;
        }

        public IRDetailController getMerchantInfo(string pSearvhValue)
        {
            IRDetailController model = new IRDetailController();

            if (dbFunction.isValidDescription(pSearvhValue.ToString()))
            {   
                dbAPI.ExecuteAPI("GET", "Search", "Merchant Info", pSearvhValue, "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.IRIDNo = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 16));
                        model.MerchantName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        model.MerchantID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.TID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        model.MID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                        model.Address = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        model.Region = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        model.Province = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        model.AppVersion = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                        model.AppCRC = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 15);                        
                    }
                }
                catch (Exception ex)
                {
                    model = setInitValue();
                    Debug.WriteLine("Exceptional error " + ex.Message);
                }
            }

            // Return the filled model
            return model;
        }

    }
}
