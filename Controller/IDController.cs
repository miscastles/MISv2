using MIS.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Controller
{
    public class IDController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        private modelID setInitValue()
        {
            return new modelID();
        }

        public modelID getInfo(string pSearchBy, string pSearchValue)
        {
            modelID model = new modelID();

            if (dbFunction.isValidDescription(pSearchValue))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "GetID Info", $"{pSearchBy}{clsDefines.gPipe}{pSearchValue}");

                if (dbFunction.isValidDescription(pJSONString))
                {
                    try
                    {
                        model.UniqueID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ID));
                    }
                    catch (Exception ex)
                    {
                        model = setInitValue();

                        Debug.WriteLine(
                            "IDController: Exceptional error " + ex.Message
                        );

                        dbFunction.SetMessageBox(
                            "IDController: Exceptional error ex = " + ex.Message,
                            clsDefines.FIELD_CHECK_MSG,
                            clsFunction.IconType.iError
                        );
                    }

                }
            }

            return model;
        }
    }
}
