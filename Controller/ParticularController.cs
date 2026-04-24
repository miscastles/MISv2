using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class ParticularController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int ParticularID { get; set; }
        public string Name { get; set; }
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        public int PositionID { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string TelNo { get; set; }

        private ParticularController setInitValue()
        {
            ParticularController model = new ParticularController();

            model.ParticularID = 0;
            model.Name = "";
            model.DepartmentID = 0;
            model.Department = "";
            model.PositionID = 0;
            model.Position = "";
            model.Email = "";
            model.MobileNo = "";
            model.TelNo = "";

            return model;
        }

        public ParticularController getInfo(int pID)
        {
            ParticularController model = new ParticularController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Particular", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.ParticularID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.Name = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        model.DepartmentID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2));
                        model.Department = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        model.PositionID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4));
                        model.Email = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                        model.MobileNo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        model.Position = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);

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
    }
}
