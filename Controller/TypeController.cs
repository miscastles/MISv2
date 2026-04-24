using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MIS.Function;

namespace MIS.Controller
{
    public class TypeController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int TypeID { get; set; }
        public int Type { get; set; }
        public int Price { get; set; }        
        public string Remarks { get; set; }
        public int RentFee { get; set; }
        public int TypeValue { get; set; }
        public string Description { get; set; }
        public string QueryString { get; set; }
        public int SequenceDisplay { get; set; }

        private TypeController setInitValue()
        {
            TypeController model = new TypeController();

            model.TypeID = TypeID;
            model.Type = Type;
            model.Price = Price;
            model.RentFee = RentFee;
            model.TypeValue = TypeValue;
            model.Remarks = Remarks;            
            model.Description = Description;
            model.QueryString = QueryString;
            model.SequenceDisplay = SequenceDisplay;

            return model;
        }

        public TypeController getInfo(int pID)
        {
            TypeController model = new TypeController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Type Info", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.TypeID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.Description = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                        model.Remarks = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        model.Type = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3));
                        model.QueryString = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        model.TypeValue = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5));
                        model.SequenceDisplay = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6));
                        model.Price = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7));
                        model.RentFee = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8));
                        
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

        public List<TypeController> getDetailList(string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store modelExpenses objects
            List<TypeController> mList = new List<TypeController>();

            dbAPI.ExecuteAPI("GET", "View", "Type List", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    TypeController controller = new TypeController();
                    
                    controller.TypeID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TypeID));
                    controller.Type = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Type));
                    controller.Price = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Price));
                    controller.RentFee = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_RentFee));
                    controller.TypeValue = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_TypeValue));
                    controller.Remarks = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Remarks);
                    controller.Description = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_Description);
                    controller.QueryString = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_QueryString);
                    controller.SequenceDisplay = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_SequenceDisplay));

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
