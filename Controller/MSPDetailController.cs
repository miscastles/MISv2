using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class MSPDetailController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public string Remarks { get; set; }
        public int ClientID { get; set; }
        public string SubmitAt { get; set; }
        public int ControlNo { get; set; }
        public int MSPNo { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedID { get; set; }
        public int MerchantID { get; set; }
        public int ResultStatus { get; set; }
        public string ResultStatusDesc { get; set; }
        
        private MSPDetailController setInitValue()
        {
            MSPDetailController model = new MSPDetailController();

            model.ControlNo = 0;
            model.MSPNo = 0;
            model.ClientID = 0;
            model.MerchantID = 0;
            model.CreatedID = 0;
            model.ResultStatus = 0;
            model.ResultStatusDesc = "";
            model.CreatedAt = "";
            model.CreatedBy = "";
            model.SubmitAt = "";
            model.Remarks = "";

            return model;
        }

        public MSPDetailController getInfo(int pID)
        {
            MSPDetailController model = new MSPDetailController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "MSP Detail Info", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.ControlNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ControlNo));
                        model.MSPNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MSPNo));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ClientID));
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MerchantID));
                        
                        model.CreatedID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedID));
                        model.CreatedAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedAt);
                        model.CreatedBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedBy);
                        
                        model.SubmitAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SubmitAt);
                        
                        model.ResultStatus = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ResultStatus));
                        model.ResultStatusDesc = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ResultStatusDesc);
                       
                        model.Remarks = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Remarks);
                        
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

        public List<MSPDetailController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<MSPDetailController> mList = new List<MSPDetailController>();

            dbAPI.ExecuteAPI("GET", "View", "MSP Detail List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    MSPDetailController model = new MSPDetailController();

                    model.ControlNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ControlNo));
                    model.MSPNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MSPNo));
                    model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ClientID));
                    model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MerchantID));

                    model.CreatedID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedID));
                    model.CreatedAt = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedAt);
                    model.CreatedBy = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedBy);

                    model.SubmitAt = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SubmitAt);

                    model.ResultStatus = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ResultStatus));
                    model.ResultStatusDesc = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ResultStatusDesc);

                    model.Remarks = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Remarks);

                    // Add to List                    
                    mList.Add(model);

                    i++;
                }
            }

            // Return the filled model
            return mList;
        }
    }
}
