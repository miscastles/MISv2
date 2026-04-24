using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Function;

namespace MIS.Model
{
    public class modelPrivacyDetail
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int isAdd { get; set; }
        public int UserID { get; set; }
        public int isView { get; set; }
        public int isPrint { get; set; }
        public string FullName { get; set; }
        public int isDelete { get; set; }
        public int isUpdate { get; set; }
        public int PrivacyID { get; set; }
        public int isChecked { get; set; }
        public string Description { get; set; }

        public modelPrivacyDetail setPrivacyDetailInfo(int pID, int pPrivacyID)
        {
            modelPrivacyDetail model = new modelPrivacyDetail();

            if (dbFunction.isValidID(pID.ToString()) || dbFunction.isValidID(pPrivacyID.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Privacy Detail Info", pID.ToString() + clsDefines.gPipe + pPrivacyID);

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                model.PrivacyID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_PrivacyID));
                model.UserID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_UserID));
                model.Description = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Description);
                model.FullName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FullName);
                model.isChecked = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isChecked));
                model.isAdd = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isAdd));
                model.isDelete = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isDelete));
                model.isUpdate = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isUpdate));
                model.isView = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isView));
                model.isPrint = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_isPrint));

            }

            // Return the filled model
            return model;
        }
    }
}
