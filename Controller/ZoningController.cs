using System;
using System.Diagnostics;
using MIS.Model;

namespace MIS.Controller
{
    public class ZoningController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        private modelZoning setInitValue()
        {
            return new modelZoning();
        }

        public modelZoning getInfo(int pID)
        {
            modelZoning model = new modelZoning();

            if (dbFunction.isValidID(pID.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON(
                    "Search",
                    "Zoning Detail",
                    $"{pID}"
                );

                if (dbFunction.isValidDescription(pJSONString))
                {
                    try
                    {
                        model.ZoneID = pID;

                        model.Cluster = dbAPI.GetValueFromJSONString(
                            pJSONString,
                            clsDefines.TAG_Cluster
                        );

                        model.Zone = dbAPI.GetValueFromJSONString(
                            pJSONString,
                            clsDefines.TAG_Zone
                        );

                        model.Region = dbAPI.GetValueFromJSONString(
                            pJSONString,
                            clsDefines.TAG_Region
                        );

                        model.Area = dbAPI.GetValueFromJSONString(
                            pJSONString,
                            clsDefines.TAG_Area
                        );

                        model.CityMunicipal = dbAPI.GetValueFromJSONString(
                            pJSONString,
                            clsDefines.TAG_CityMunicipal
                        );
                    }
                    catch (Exception ex)
                    {
                        model = setInitValue();

                        Debug.WriteLine(
                            "ZoningController: Exceptional error " + ex.Message
                        );

                        dbFunction.SetMessageBox(
                            "ZoningController: Exceptional error ex = " + ex.Message,
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