using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class ImportMasterController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int UserID { get; set; }
        public string Remarks { get; set; }
        public int TransNo { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public int FileType { get; set; }
        public string FullPath { get; set; }
        public int ControlNo { get; set; }
        public string ProcessedBy { get; set; }
        public string ProcessedDate { get; set; }
        public string ReceivedDate { get; set; }

        private ImportMasterController setInitValue()
        {
            ImportMasterController model = new ImportMasterController();

            model.ControlNo = ControlNo;
            model.UserID = UserID;
            model.Remarks = Remarks;
            model.TransNo = TransNo;
            model.FileName = FileName;
            model.FullPath = FullPath;
            model.FileSize = FileSize;
            model.FileType = FileType;
            model.ProcessedBy = ProcessedBy;
            model.ProcessedDate = ProcessedDate;
            model.ReceivedDate = ReceivedDate;

            return model;
        }

        public List<ImportMasterController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<ImportMasterController> mList = new List<ImportMasterController>();

            dbAPI.ExecuteAPI("GET", "View", "Import Master List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    ImportMasterController controller = new ImportMasterController();

                    controller.TransNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TransNo));
                    controller.ControlNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ControlNo));
                    controller.UserID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_UserID));
                    controller.FileType = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_FileType));
                    controller.Remarks = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Remarks);
                    controller.FileName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_FileName);
                    controller.FullPath = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_FullPath);
                    controller.FileSize = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_FileSize);
                    controller.ProcessedBy = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ProcessedBy);
                    controller.ProcessedDate = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ProcessedDate);
                    controller.ReceivedDate = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ReceiveDate);

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
