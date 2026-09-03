using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MIS.Model;

namespace MIS.Controller
{
    public class ExpensesController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        private modelExpensesMaster setInitMasterValue()
        {
            return new modelExpensesMaster();
        }

        private modelExpensesDetail setInitDetailValue()
        {
            return new modelExpensesDetail();
        }

        public modelExpensesMaster geMastertInfo(int pID)
        {
            modelExpensesMaster model = new modelExpensesMaster();

            if (dbFunction.isValidID(pID.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search","Expenses Master",$"{pID}");

                if (dbFunction.isValidDescription(pJSONString))
                {
                    try
                    {
                        model.ExpensesNo = pID;

                        model.ServiceNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SERVICENO));
                        model.IRIDNo = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRIDNO));
                        model.ClientID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientID));
                        model.MerchantID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantID));
                        model.FEID = int.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEID));

                        model.MerchantName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME);
                        model.ClientName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                        model.FEName = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEName);

                        model.ReferenceNo = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REFERENCENO);
                        model.TotalAmount = decimal.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TAmount));

                        model.CreatedBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedBy);
                        model.CreatedDate = DateTime.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                        model.UpdatedBy = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_UpdatedBy);
                        model.UpdatedDate = DateTime.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_UpdatedDate));

                        model.ServiceNoList = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceNoList);
                        model.IRNoList = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNoList);
                        model.ReceiptList = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReceiptList);

                        model.DateTimeStamp = DateTime.Parse(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DateTimeStamp));


                    }
                    catch (Exception ex)
                    {
                        model = setInitMasterValue();

                        Debug.WriteLine(
                            "ExpensesController: Exceptional error " + ex.Message
                        );

                        dbFunction.SetMessageBox(
                            "ExpensesController: Exceptional error ex = " + ex.Message,
                            clsDefines.FIELD_CHECK_MSG,
                            clsFunction.IconType.iError
                        );
                    }
                }
            }

            return model;
        }

        public List<modelExpensesDetail> getDetailList(string pSearchBy, string pSearchValue)
        {
            int i = 0;

            // Create an empty list to store Expenses Detail models
            List<modelExpensesDetail> mList = new List<modelExpensesDetail>();

            dbAPI.ExecuteAPI("GET","View", pSearchBy, pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!dbAPI.isNoRecordFound())
            {
                while (clsArray.ID.Length > i)
                {
                    string detail_info = clsArray.detail_info[i];

                    modelExpensesDetail model = new modelExpensesDetail();

                    switch (pSearchBy)
                    {
                        case "Expense Reference List":

                            model.DetailID = int.Parse(dbAPI.GetValueFromJSONString(detail_info,clsDefines.TAG_DetailID));
                            model.ExpensesNo = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ExpensesNo));
                            model.ExpensesID = int.Parse(dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ExpensesID));
                            model.ExpensesReferenceNo = dbAPI.GetValueFromJSONString(detail_info, clsDefines.TAG_ExpensesReferenceNo);

                            break;
                    }

                    // Add model to list
                    mList.Add(model);

                    i++;
                }
            }

            return mList;
        }
    }
}
