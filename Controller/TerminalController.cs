using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class TerminalController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int MobileID { get; set; }
        public string Password { get; set; }
        public string MobileTerminalID { get; set; }
        public string MobileTerminalName { get; set; }

        private TerminalController setInitValue()
        {
            TerminalController model = new TerminalController();

            model.MobileID = MobileID;
            model.MobileTerminalID = MobileTerminalID;
            model.MobileTerminalName = MobileTerminalName;
            model.Password = Password;

            return model;
        }

        public TerminalController getInfo(string pDescription)
        {
            TerminalController model = new TerminalController();

            if (dbFunction.isValidDescription(pDescription))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Terminal Info", pDescription, "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.MobileID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.MobileTerminalID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        model.MobileTerminalName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        model.Password = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);

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

        public List<TerminalController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<TerminalController> mList = new List<TerminalController>();

            dbAPI.ExecuteAPI("GET", "View", "Mobile Terminal List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    TerminalController controller = new TerminalController();

                    controller.MobileID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MobileID));
                    controller.MobileTerminalID = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MobileTerminalID);
                    controller.MobileTerminalName = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_MobileTerminalName);
                    controller.Password = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Password);

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
