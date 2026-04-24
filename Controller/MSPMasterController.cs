using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class MSPMasterController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int MSPNo { get; set; }
        public string BusType { get; set; }
        public string NoBType { get; set; }
        public string Remarks { get; set; }
        public string Category { get; set; }
        public int ClientID { get; set; }
        public string SubmitAt { get; set; }
        public string SubmitBy { get; set; }
        public int BusTypeID { get; set; }
        public string CheckedAt { get; set; }
        public string CheckedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public int MSPStatus { get; set; }
        public int NoBTypeID { get; set; }
        public string UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string ClientName { get; set; }
        public int MerchantID { get; set; }
        public string SchemeType { get; set; }
        public string ReferenceNo { get; set; }
        public string MerchantName { get; set; }
        public string ReferralType { get; set; }
        public int SchemeTypeID { get; set; }
        public string MSPStatusDesc { get; set; }
        public int CategoryTypeID { get; set; }
        public int ReferralTypeID { get; set; }
        public string RegisteredName { get; set; }
        public string TradeName { get; set; }
        public string TINNo { get; set; }

        public int AcqTypeID { get; set; }
        public string AcquirerType { get; set; }
        public string NoOfPOS { get; set; }
        public string NoOfYear { get; set; }
        public string RentalAmt { get; set; }
        public string BankAcntName { get; set; }
        public string BankAcntNo { get; set; }
        public string BankSettlement { get; set; }
        public string BankReferring { get; set; }

        public int MDRCreditTypeID { get; set; }
        public string MDRCreditType { get; set; }

        public int MDRDebitTypeID { get; set; }
        public string MDRDebitType { get; set; }

        public int MDRInstTypeID { get; set; }
        public string MDRInstType { get; set; }
        public int CreatedID { get; set; }
        public int UpdatedID { get; set; }
        public int SubmitID { get; set; }
        public int CheckedID { get; set; }
        public int NoOfDocument { get; set; }


        private MSPMasterController setInitValue()
        {
            MSPMasterController model = new MSPMasterController();

            model.MSPNo = 0;
            model.BusType = "";
            model.NoBType = "";
            model.Remarks = "";
            model.Category = "";
            model.ClientID = 0;
            model.SubmitAt = "";
            model.SubmitBy = "";
            model.BusTypeID = 0;
            model.CheckedAt = "";
            model.CheckedBy = "";
            model.CreatedAt = "";
            model.CreatedBy = "";
            model.MSPStatus = 0;
            model.NoBTypeID = 0;
            model.UpdatedAt = "";
            model.UpdatedBy = "";
            model.ClientName = "";
            model.MerchantID = 0;
            model.SchemeType = "";
            model.ReferenceNo = "";
            model.MerchantName = "";
            model.ReferralType = "";
            model.SchemeTypeID = 0;
            model.MSPStatusDesc = "";
            model.CategoryTypeID = 0;
            model.ReferralTypeID = 0;
            model.RegisteredName = "";
            model.TradeName = "";
            model.TINNo = "";
            model.AcqTypeID = 0;
            model.AcquirerType = "";
            model.NoOfPOS = "";
            model.NoOfYear = "";
            model.RentalAmt = "";
            model.BankAcntName = "";
            model.BankAcntNo = "";
            model.BankSettlement = "";
            model.BankReferring = "";
            model.MDRCreditTypeID = 0;
            model.MDRCreditType = "";
            model.MDRDebitTypeID = 0;
            model.MDRDebitType = "";
            model.MDRInstTypeID = 0;
            model.MDRInstType = "";
            model.CreatedID = 0;
            model.UpdatedID = 0;
            model.SubmitID = 0;
            model.CheckedID = 0;
            model.NoOfDocument = 0;

            return model;
        }

        public MSPMasterController getInfo(int pID)
        {
            MSPMasterController model = new MSPMasterController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "MSP Master Info", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.MSPNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MSPNo));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ClientID));
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MerchantID));
                        model.ReferenceNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_REFERENCENO);

                        model.ClientName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ClientName);
                        model.MerchantName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MERCHANTNAME);
                        model.RegisteredName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_RegisteredName);
                        model.TradeName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TradeName);
                        model.TINNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_TINNo);

                        model.AcquirerType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_AcquirerType);
                        model.BusType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_BusType);
                        model.NoBType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_NoBType);
                        model.SchemeType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SchemeType);
                        model.ReferralType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ReferralType);
                        model.Category = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Category);

                        model.MDRCreditType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MDRCreditType);
                        model.MDRDebitType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MDRDebitType);
                        model.MDRInstType = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MDRInstType);

                        model.Category = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Category);

                        model.CreatedID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedID));
                        model.CreatedAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedAt);
                        model.CreatedBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CreatedBy);

                        model.SubmitID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SubmitID));
                        model.SubmitAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SubmitAt);
                        model.SubmitBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_SubmitBy);

                        model.UpdatedID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_UpdatedID));
                        model.UpdatedAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_UpdatedAt);
                        model.UpdatedBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_UpdatedBy);

                        model.RentalAmt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_RentalAmt);
                        model.NoOfPOS = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_NoOfPOS);
                        model.NoOfYear = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_NoOfYear);

                        model.BankAcntName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_BankAcntName);
                        model.BankAcntNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_BankAcntNo);
                        model.BankSettlement = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_BankSettlement);
                        model.BankReferring = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_BankReferring);

                        model.MSPStatusDesc = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_MSPStatusDesc);
                        model.Remarks = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Remarks);

                        model.CheckedID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CheckedID));
                        model.CheckedBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CheckedBy);
                        model.CheckedAt = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_CheckedAt);

                        model.NoOfDocument = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_NoOfDocument));
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

        public List<MSPMasterController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<MSPMasterController> mList = new List<MSPMasterController>();

            dbAPI.ExecuteAPI("GET", "View", "MSP Master List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    MSPMasterController controller = new MSPMasterController();
                    
                    controller.MSPNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MSPNo));
                    controller.ClientID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ClientID));
                    controller.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MerchantID));
                    controller.ReferenceNo = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_REFERENCENO);

                    controller.ClientName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ClientName);
                    controller.MerchantName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MERCHANTNAME);
                    controller.RegisteredName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_RegisteredName);
                    controller.TradeName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TradeName);

                    controller.BusType = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_BusType);
                    controller.NoBType = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_NoBType);
                    controller.SchemeType = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SchemeType);
                    controller.ReferralType = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ReferralType);
                    controller.Category = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Category);
                    
                    controller.CreatedID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedID));
                    controller.CreatedAt = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedAt);
                    controller.CreatedBy = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_CreatedBy);

                    controller.SubmitID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SubmitID));
                    controller.SubmitAt = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SubmitAt);
                    controller.SubmitBy = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SubmitBy);

                    controller.UpdatedID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_UpdatedID));
                    controller.UpdatedAt = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_UpdatedAt);
                    controller.UpdatedBy = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_UpdatedBy);

                    controller.MSPStatusDesc = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MSPStatusDesc);
                    controller.Remarks = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Remarks);

                    // Add to List                    
                    mList.Add(controller);

                    i++;
                }
            }

            // Return the filled model
            return mList;
        }


    }
}
