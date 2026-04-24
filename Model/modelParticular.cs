using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIS.Function;

namespace MIS.Model
{
    public class modelParticular
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public string Name { get; set; }
        public string Email { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string ParticularID { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPosition { get; set; }

        public modelParticular setParticularInfo(int pID)
        {
            modelParticular model = new modelParticular();

            if (dbFunction.isValidID(pID.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Particular Basic Info", pID.ToString());

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                model.Name = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Name);
                model.Address = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Address);
                model.ContactPerson = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactPerson);
                model.ContactPosition = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactPosition);
                model.ContactNumber = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ContactNumber);
                model.Email = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);
                model.Region = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REGION);
                model.Province = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Province);
                
            }

            return model;
        }
    }
}
