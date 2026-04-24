using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class ServicingDetailController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public string MID { get; set; }
        public string TID { get; set; }
        public int FEID { get; set; }
        public string IRNo { get; set; }
        public string IRNo_fsr { get; set; }
        public string Email { get; set; }
        public int FSRNo { get; set; }
        public int SIMID { get; set; }
        public string SIMSN { get; set; }
        public string AppCRC { get; set; }
        public string FEName { get; set; }
        public int IRIDNo { get; set; }
        public int ReasonID { get; set; }
        public string Reason { get; set; }
        public string Region { get; set; }
        public string Source { get; set; }
        public string Address { get; set; }
        public string FSRDate { get; set; }
        public string FSRTime { get; set; }
        public int JobType { get; set; }
        public string TimeEnd { get; set; }
        public string Category { get; set; }
        public int ClientID { get; set; }
        public int MobileID { get; set; }
        public string Position { get; set; }
        public string Province { get; set; }
        public int RegionID { get; set; }
        public int SourceID { get; set; }
        public string RequestNo { get; set; }
        public int ServiceNo { get; set; }
        public string TimeStart { get; set; }
        public string ActionMade { get; set; }
        public string ServiceResult { get; set; }
        public string AppVersion { get; set; }
        public int CategoryID { get; set; }
        public string ClientName { get; set; }
        public string DispatchBy { get; set; }
        public int MerchantID { get; set; }
        public string ModifiedBy { get; set; }
        public int RegionType { get; set; }
        public string SIMCarrier { get; set; }
        public int TerminalID { get; set; }
        public string TerminalSN { get; set; }
        public int isBillable { get; set; }
        public string ClientEmail { get; set; }
        public string CreatedDate { get; set; }
        public string ProcessedBy { get; set; }
        public string ReferenceNo { get; set; }
        public string RequestDate { get; set; }
        public string SubCategory { get; set; }
        public string TimeArrived { get; set; }
        public string CustomerName { get; set; }
        public string DispatchDate { get; set; }
        public string MerchantName { get; set; }
        public int ReplaceSIMID { get; set; }
        public string ReplaceSIMSN { get; set; }
        public string ScheduleDate { get; set; }
        public string TerminalType { get; set; }
        public int isDiagnostic { get; set; }
        public string CustomerEmail { get; set; }
        public string MobileVersion { get; set; }
        public int SubCategoryID { get; set; }
        public string TerminalModel { get; set; }
        public string CustomerPosition { get; set; }
        public string MobileTerminalID { get; set; }
        public string CustomerContactNo { get; set; }
        public string ReplaceSIMCarrier { get; set; }
        public int ReplaceTerminalID { get; set; }
        public string ReplaceTerminalSN { get; set; }
        public string ServiceCreatedDate { get; set; }
        public string ServiceCreatedTime { get; set; }
        public string ReplaceTerminalType { get; set; }
        public string ReplaceTerminalModel { get; set; }
        public string MerchantRepresentative { get; set; }
        public string JobTypeStatusDescription { get; set; }
        public string ServiceJobTypeDescription { get; set; }
        public string MerchantRepresentativeEmail { get; set; }
        public string MerchantRepresentativePosition { get; set; }

        public int DispatchID { get; set; }
        public string Dispatcher { get; set; }
        public int TCount { get; set; }

        public int ServiceStatus { get; set; }
        public string ServiceStatusDescription { get; set; }
        public string ServiceReqDate { get; set; }
        public string Remarks { get; set; }

        public string BillingType { get; set; }
        public int BillingTypeID { get; set; }

        private ServicingDetailController setInitValue()
        {
            ServicingDetailController model = new ServicingDetailController();

            model.MID = MID;
            model.TID = TID;
            model.FEID = FEID;
            model.IRNo = IRNo;
            model.Email = Email;
            model.FSRNo = FSRNo;
            model.SIMID = SIMID;
            model.SIMSN = SIMSN;
            model.AppCRC = AppCRC;
            model.FEName = FEName;
            model.IRIDNo = IRIDNo;
            model.ReasonID = ReasonID;
            model.Reason = Reason;
            model.Region = Region;
            model.Source = Source;
            model.Address = Address;
            model.FSRDate = FSRDate;
            model.FSRTime = FSRTime;
            model.JobType = JobType;
            model.TimeEnd = TimeEnd;
            model.Category = Category;
            model.ClientID = ClientID;
            model.MobileID = MobileID;
            model.Position = Position;
            model.Province = Province;
            model.RegionID = RegionID;
            model.SourceID = SourceID;
            model.RequestNo = RequestNo;
            model.ServiceNo = ServiceNo;
            model.TimeStart = TimeStart;
            model.ActionMade = ActionMade;
            model.ServiceResult = ServiceResult;
            model.AppVersion = AppVersion;
            model.CategoryID = CategoryID;
            model.ClientName = ClientName;
            model.DispatchBy = DispatchBy;
            model.MerchantID = MerchantID;
            model.ModifiedBy = ModifiedBy;
            model.RegionType = RegionType;
            model.SIMCarrier = SIMCarrier;
            model.TerminalID = TerminalID;
            model.TerminalSN = TerminalSN;
            model.isBillable = isBillable;
            model.ClientEmail = ClientEmail;
            model.CreatedDate = CreatedDate;
            model.ProcessedBy = ProcessedBy;
            model.ReferenceNo = ReferenceNo;
            model.RequestDate = RequestDate;
            model.SubCategory = SubCategory;
            model.TimeArrived = TimeArrived;
            model.CustomerName = CustomerName;
            model.DispatchDate = DispatchDate;
            model.MerchantName = MerchantName;
            model.ReplaceSIMID = ReplaceSIMID;
            model.ReplaceSIMSN = ReplaceSIMSN;
            model.ScheduleDate = ScheduleDate;
            model.TerminalType = TerminalType;
            model.isDiagnostic = isDiagnostic;
            model.CustomerEmail = CustomerEmail;
            model.MobileVersion = MobileVersion;
            model.SubCategoryID = SubCategoryID;
            model.TerminalModel = TerminalModel;
            model.CustomerPosition = CustomerPosition;
            model.MobileTerminalID = MobileTerminalID;
            model.CustomerContactNo = CustomerContactNo;
            model.ReplaceSIMCarrier = ReplaceSIMCarrier;
            model.ReplaceTerminalID = ReplaceTerminalID;
            model.ReplaceTerminalSN = ReplaceTerminalSN;
            model.ServiceCreatedDate = ServiceCreatedDate;
            model.ServiceCreatedTime = ServiceCreatedTime;
            model.ReplaceTerminalType = ReplaceTerminalType;
            model.ReplaceTerminalModel = ReplaceTerminalModel;
            model.MerchantRepresentative = MerchantRepresentative;
            model.JobTypeStatusDescription = JobTypeStatusDescription;
            model.ServiceJobTypeDescription = ServiceJobTypeDescription;
            model.MerchantRepresentativeEmail = MerchantRepresentativeEmail;
            model.MerchantRepresentativePosition = MerchantRepresentativePosition;

            model.DispatchID = 0;
            model.Dispatcher = "";

            model.ServiceStatus = ServiceStatus;
            model.ServiceStatusDescription = ServiceStatusDescription;
            model.ServiceReqDate = ServiceReqDate;

            model.Remarks = Remarks;
            model.BillingTypeID = BillingTypeID;
            model.BillingType = BillingType;
            return model;
        }

        public ServicingDetailController getInfo(string pSearvhValue)
        {
            ServicingDetailController model = new ServicingDetailController();

            if (dbFunction.isValidDescription(pSearvhValue.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", pSearvhValue);

                try
                {
                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.SourceID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SourceID));
                        model.Source = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Source);
                        model.CategoryID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CategoryID));
                        model.Category = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Category);
                        model.SubCategoryID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SubCategoryID));
                        model.SubCategory = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SubCategory);
                        model.BillingTypeID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_BillingTypeID));
                        model.BillingType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_BillType);
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

        public ServicingDetailController getServicingInfo(string pSearvhValue)
        {
            ServicingDetailController model = new ServicingDetailController();

            if (dbFunction.isValidDescription(pSearvhValue.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", pSearvhValue);

                try
                {
                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.JobType = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobType));
                        model.ServiceNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientID));
                        model.IRIDNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNo));
                        model.FSRNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRNO));
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantID));

                        model.TerminalID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalID));
                        model.SIMID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMID));
                        model.ReplaceTerminalID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalID));
                        model.ReplaceSIMID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMID));

                        model.ClientName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                        model.ServiceJobTypeDescription = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceJobTypeDescription);
                        model.MerchantName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME);
                        model.Address = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Address);
                        model.Province = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Province);
                        model.Region = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REGION);
                        model.AppVersion = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppVersion);
                        model.AppCRC = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppCRC);
                        model.TID = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID);
                        model.MID = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID);

                        model.TerminalSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalSN);
                        model.TerminalType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType);
                        model.TerminalModel = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel);
                        model.SIMSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMSN);
                        model.SIMCarrier = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMCarrier);

                        model.ReplaceTerminalSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalSN);
                        model.ReplaceTerminalType = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalType);
                        model.ReplaceTerminalModel = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalModel);
                        model.ReplaceSIMSN = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMSN);
                        model.ReplaceSIMCarrier = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMCarrier);

                        model.ServiceReqDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceReqDate);
                        model.RequestDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate);
                        model.ScheduleDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate);
                        model.ServiceCreatedDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedDate);
                        model.ServiceCreatedTime = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedTime);
                        
                        model.IRNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO);
                        model.IRNo_fsr = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO_fsr);
                        model.RequestNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestNo);
                        model.ReferenceNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REFERENCENO);
                        model.JobTypeStatusDescription = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobTypeStatusDescription);

                        model.FSRDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate);
                        model.FSRTime = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRTime);
                        model.TimeArrived = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeArrived);
                        model.TimeStart = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeStart);
                        model.TimeEnd = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeEnd);

                        model.ActionMade = model.ServiceResult = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ActionMade);

                        model.ReasonID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReasonID));
                        model.Reason = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Reason);

                        model.FEID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEID));
                        model.FEName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEName);
                        model.Position = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Position);
                        model.Email = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);

                        model.MerchantRepresentative = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentative);
                        model.MerchantRepresentativePosition = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativePosition);
                        model.MerchantRepresentativeEmail = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativeEmail);
                        
                        model.SourceID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SourceID));
                        model.Source = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Source);
                        model.CategoryID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CategoryID));
                        model.Category = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Category);
                        model.SubCategoryID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SubCategoryID));
                        model.SubCategory = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SubCategory);

                        model.ServiceStatus = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceStatus));
                        model.ServiceStatusDescription = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceStatusDescription);

                        model.DispatchDate = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate);
                        model.DispatchID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchID));
                        model.Dispatcher = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Dispatcher);
                        model.DispatchBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchBy);

                        model.Remarks = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Remarks);

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

        public List<ServicingDetailController> getDetailList(string pSearchBy, string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<ServicingDetailController> mList = new List<ServicingDetailController>();

            dbAPI.ExecuteAPI("GET", "View", pSearchBy, pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    ServicingDetailController model = new ServicingDetailController();

                    switch (pSearchBy)
                    {
                        case "Unclosed Ticket List":
                            model.DispatchID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_DispatchID));                           
                            model.Dispatcher = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Dispatcher);
                            model.TCount = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TCount));
                            
                            break;
                    }
                    

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
