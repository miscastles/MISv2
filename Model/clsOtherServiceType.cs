using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace MIS
{
    public class clsOtherServiceType
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _OtherServiceTypeID;
        public static string _Description;

        private clsINI dbINIAPI;
        private clsAPI dbAPI;

        public static int ClassOtherServiceTypeID
        {
            get { return _OtherServiceTypeID; }
            set { _OtherServiceTypeID = value; }
        }

        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
        public void ProcessAPI(string APIMethod, string StatementType, string SearchBy, string SearchValue, string Action)
        {
            clsServiceProvider.RecordFound = false;
            dbINIAPI = new clsINI();
            dbINIAPI.InitAPISetting();

            dbAPI = new clsAPI();
            dbAPI.APIGETRequest(APIMethod, StatementType, SearchBy, SearchValue, Action);

            // Validate Response
            ValidateResponse Response = JsonConvert.DeserializeObject<ValidateResponse>(clsGlobalVariables.strJSONResponse);

            if (Response.resp_code.CompareTo("200") != 0)
            {
                MessageBox.Show(Response.message);
            }
            else
            {
                // Parse JSON Format
                OtherServiceTypeDetailOnline Detail = JsonConvert.DeserializeObject<OtherServiceTypeDetailOnline>(clsGlobalVariables.strJSONResponse);

                switch (StatementType)
                {
                    case "Search":
                        foreach (var element in Detail.data)
                        {
                            clsOtherServiceType.RecordFound = true;
                            clsOtherServiceType.ClassOtherServiceTypeID = element.OtherServiceTypeID;
                            clsOtherServiceType.ClassDescription = element.Description;
                        }

                        break;
                    case "View":

                        List<string> IDCol = new List<String>();
                        List<string> DescriptionCol = new List<String>();

                        foreach (var element in Detail.data)
                        {
                            clsOtherServiceType.RecordFound = true;
                            IDCol.Add(element.OtherServiceTypeID.ToString());
                            DescriptionCol.Add(element.Description);
                        }

                        clsArray.OtherServiceTypeID = IDCol.ToArray();
                        clsArray.Description = DescriptionCol.ToArray();

                        break;
                }
            }
        }
    }
}
