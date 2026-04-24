using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class ServiceTypeController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int ServiceTypeID { get; set; }
        public int JobType { get; set; }
        public string Description { get; set; } // FOR INSTALLATION, FOR REPROGRAMMING, FOR PULL-OUT, ...
        public string Code { get; set; }
        public int ServiceStatus { get; set; }
        public string ServiceStatusDescription { get; set; } // INSTALLED, REPROGRAMMED, PULLED-OUT, ...
        public int ServiceTypeActive { get; set; }
        public int ServiceStatusActive { get; set; }
        public string JobTypeDescription { get; set; } // SVC REQ INSTALLATION, SVC REQ REPROGRAMMING, SVC REQ PULL-OUT, ...
        public string ServiceJobTypeDescription { get; set; } // INSTALLATION, REPROGRAMMING, PULL-OUT, ...
        public int SequenceDisplay { get; set; }
        public int isActive { get; set; }

        private ServiceTypeController setInitValue()
        {
            ServiceTypeController model = new ServiceTypeController();

            model.ServiceTypeID = 0;
            model.JobType = 0;
            model.Description = "";
            model.Code = "";
            model.ServiceStatus = 0;
            model.ServiceStatusDescription = "";
            model.ServiceTypeActive = 0;
            model.ServiceStatusActive = 0;
            model.JobTypeDescription = "";
            model.ServiceJobTypeDescription = "";
            model.SequenceDisplay = 0;
            model.isActive = 0;

            return model;
        }

        public ServiceTypeController getServiceTypeInfo(string pSearchValue)
        {
            ServiceTypeController model = new ServiceTypeController();

            if (dbFunction.isValidDescription(pSearchValue))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Service Type Info", pSearchValue, "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.ServiceTypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.Description = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        model.Code = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        model.ServiceStatus = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3));
                        model.ServiceStatusDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        model.JobType = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                        model.JobTypeDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        model.ServiceJobTypeDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                       
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
