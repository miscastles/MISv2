using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MIS.Function;

namespace MIS.Controller
{
    public class ReasonController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int ReasonID { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }        
        public string Description { get; set; }
        
        private ReasonController setInitValue()
        {
            ReasonController model = new ReasonController();

            model.ReasonID = ReasonID;
            model.Code = Code;
            model.Type = Type;
            model.Description = Description;

            return model;
        }

        public ReasonController getInfo(int pID)
        {
            ReasonController model = new ReasonController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Reason Info", pID.ToString());
                
                try
                {
                    dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.ReasonID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReasonID));
                        model.Code = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Code);
                        model.Description = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Description);
                        model.Type = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Type);
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

        public List<ReasonController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<ReasonController> mList = new List<ReasonController>();

            dbAPI.ExecuteAPI("GET", "View", "Reason List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    ReasonController controller = new ReasonController();

                    controller.ReasonID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ReasonID));
                    controller.Code = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Code);
                    controller.Description = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Description);
                    controller.Type = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Type);

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
