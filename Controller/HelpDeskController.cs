using System;
using System.Collections.Generic;
using System.Diagnostics;
using MIS.AppMainActivity;

namespace MIS.Controller
{
    public class HelpDeskController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        // Masster
        public int AssistNo { get; set; }
        public int JobType { get; set; }
        public string RequestID { get; set; }
        public string Ref_No { get; set; }
        public string ServiceJobTypeDescription { get; set; }
        public string RequestDate { get; set; }
        public string Requestor { get; set; }
        public int SourceID { get; set; }
        public string Source { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public int SubCategoryID { get; set; }
        public string SubCategory { get; set; }
        public int CreatedID { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; }

        // Details
        public int ProblemNo { get; set; }
        public int ProblemID { get; set; }        
        public int MerchantID { get; set; }
        public int HelpDeskID { get; set; }
        public string HelpDeskName { get; set; }
        public int TeamLeadID { get; set; }
        public string TeamLeadName { get; set; }
        public string CreatedDate { get; set; }
        public int ReasonID { get; set; }
        public string Reason { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNo { get; set; }
        public string ContactPosition { get; set; }
        public string ContactEmail { get; set; }
        public string Representative { get; set; }
        public string RequestedBy { get; set; }
        public string TerminalSN { get; set; } // Optional
        public string SimSN { get; set; }// Optional
        public string DockerSN { get; set; }// Optional
        public string ProblemReported { get; set; }
        public string ActualProblem { get; set; }
        public string ActionTaken { get; set; }        
        public string RemarksHelpDesk { get; set; }
        public string Remarks { get; set; }        
        public int IRIDNo { get; set; }
        public int ClientID { get; set; }
        public int TerminalID { get; set; }
        public int SIMID { get; set; }
        public string TimeAssisted { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string AppVersion { get; set; }
        public string AppCRC { get; set; }

        public string MerchantName { get; set; }
        public string TID { get; set; }
        public string MID { get; set; }

        public string Dependency { get; set; }
        public string StatusReason { get; set; }

        private HelpDeskController setInitValue()
        {
            HelpDeskController model = new HelpDeskController();
            
            // Master
            model.Ref_No = "";
            model.RequestID = "";
            model.RequestDate = "";
            model.Requestor = "";
            model.SourceID = 0;
            model.Source = "";
            model.CategoryID = 0;
            model.Category = "";
            model.SubCategoryID = 0;
            model.SubCategory = "";
            model.CreatedID = 0;
            model.CreatedAt = "";
            model.Status = "";

            // Details
            model.ProblemID = 0;
            model.AssistNo = 0;
            model.MerchantID = 0;
            model.HelpDeskID = 0;
            model.HelpDeskName = "";
            model.TeamLeadID = 0;
            model.TeamLeadName = "";
            model.CreatedDate = "";
            model.ReasonID = 0;
            model.Reason = "";
            model.ContactPerson = "";
            model.ContactNo = "";
            model.ContactPosition = "";
            model.ContactEmail = "";
            model.Representative = "";
            model.RequestedBy = "";
            model.TerminalSN = "";
            model.SimSN = "";
            model.DockerSN = "";
            model.ProblemReported = "";
            model.ActualProblem = "";
            model.ActionTaken = "";            
            model.RemarksHelpDesk = "";
            model.Remarks = "";
            model.IRIDNo = 0;
            model.ClientID = 0;
            model.TerminalID = 0;
            model.SIMID = 0;
            model.TimeAssisted = "";
            model.TimeStart = "";
            model.TimeEnd = "";
            model.AppVersion = "";
            model.AppCRC = "";

            model.JobType = 0;
            model.ServiceJobTypeDescription = "";

            model.Dependency = "";
            model.StatusReason = "";

            return model;
        }

        public HelpDeskController getInfo(int pID)
        {
            HelpDeskController model = new HelpDeskController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Helpdesk-Details", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (dbFunction.isValidDescription(clsSearch.ClassOutParamValue))
                    {
                        // IDs
                        model.JobType = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_JobType));
                        model.AssistNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_AssistNo));
                        model.ProblemNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_ID));
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_MerchantID));
                        model.IRIDNo = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_IRIDNo));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ClientID));

                        model.HelpDeskID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_HelpdeskID));
                        model.HelpDeskName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_HelpdeskName);

                        model.TeamLeadID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TeamLeadID));
                        model.TeamLeadName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TeamLeadName);

                        model.ReasonID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ReasonID));
                        model.Reason = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_Reason);

                        model.SourceID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_SourceID));
                        model.Source = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_Source);

                        model.CategoryID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_CategoryID));
                        model.Category = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_Category);

                        model.SubCategoryID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_SubCategoryID));
                        model.SubCategory = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_SubCategory);

                        model.ProblemID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ProblemID));
                        model.ProblemReported = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ProblemReported);
                        model.ActualProblem = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ActualProblem);
                        model.ActionTaken = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_RemarksService);
                        model.Remarks = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_RemarksHelpDesk);

                        model.TerminalID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TerminalID));
                        model.SIMID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_SIMID));

                        model.RequestID = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_RequestID);
                        model.Ref_No = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ReferenceNo);
                        model.CreatedDate = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_CreatedDate);
                        model.RequestDate = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_RequestDate);
                        model.TimeAssisted = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TimeAssisted);
                        model.TimeStart = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TimeStart);
                        model.TimeEnd = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_TimeEnd);

                        model.AppVersion = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_AppVersion);
                        model.AppCRC = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_AppCrc);
                        
                        model.ContactPerson = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ContactPerson);
                        model.ContactNo = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ContactNo);
                        model.ContactPosition = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ContactPosition);
                        model.ContactEmail = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_ContactEmail);

                        model.Representative = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_Representative);
                        model.RequestedBy = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_HD_RequestedBy);

                        model.Dependency = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_Dependency);
                        model.StatusReason = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_StatusReason);


                    }
                }
                catch (Exception ex)
                {
                    model = setInitValue();
                    Debug.WriteLine("Exceptional error " + ex.Message);
                }
            }
            
            return model;
        }

        public List<HelpDeskController> getDetailList(string pSearchValue)
        {
            int i = 0;
            
            List<HelpDeskController> mList = new List<HelpDeskController>();

            dbAPI.ExecuteAPI("GET", "View", "Helpdesk Problem List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    HelpDeskController controller = new HelpDeskController();

                    controller.AssistNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_AssistNo));
                    controller.JobType = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_JobType));
                    controller.ProblemNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_ProblemNo));
                    controller.ClientID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ClientID));
                    controller.IRIDNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_IRIDNo));
                    controller.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MerchantID));
                    controller.ReasonID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ReasonID));
                    controller.ProblemID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_ProblemID));
                    controller.RequestID = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_RequestID);
                    controller.Ref_No = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_ReferenceNo);
                    controller.MerchantName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MERCHANTNAME);
                    controller.TID = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TID);
                    controller.MID = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MID);
                    controller.HelpDeskName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_HelpdeskName);
                    controller.TeamLeadName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_TeamLeadName);
                    controller.CreatedDate = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_CreatedDate);
                    controller.RequestDate = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_RequestDate);
                    controller.ProblemReported = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_ProblemReported);
                    controller.Remarks = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_HD_RemarksHelpDesk);
                    controller.ServiceJobTypeDescription = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ServiceJobTypeDescription);


                    // Add to List                    
                    mList.Add(controller);

                    i++;
                }
            }

            // Return the filled model
            return mList;
        }

        public void HelpdeskServiceReport(string Id)
        {
            if (Id == "0") return;

            clsSearch.ClassSearchValue = Id;
            AppReports.ReportPreview(58);

        }
    }
}
