using MIS.Controller;
using MIS.Enums;
using MIS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static MIS.Function.AppUtilities;

namespace MIS
{
    public class clsAPI
    {
        private static int ResponseCode;
        private static string ResponseDescription;

        private clsINI dbINIAPI;
        private clsINI dbINISystem;
        private clsAPI dbAPI;        
        private clsFile dbDump;
        private clsFunction dbFunction;    
        
        private int iAPIResponseCounter;

        // Controller
        private ServicingDetailController _mServicingDetailController = new ServicingDetailController();
        private TypeController _mTypeController = new TypeController();
        private TerminalController _mTerminalController = new TerminalController();

        public enum JobType
        {
            iServicing, iPullOut, iReplacement, iReprogramming, iInstallation, iCancelled, iInstalled, iNegative, iDispatch, iAllocated, iHold, iCancel
        }
        public enum JobTypeStatus
        {
            iPending, iReProcessing, iReadyToProcess, iProcessing, iCompleted
        }        

        public enum ServiceType
        {
            iInstalled, iNegative, iReprogrammed, iPullOut, iDiagnostic, iReplacement, iServicing
        }
        
        public enum ParticularSearchType
        {
            iMerchant, iFE, iSP, iClient, iSupplier
        }

        public enum ImportActionType
        {
            iImport, iIgnore
        }

        public enum ImportReasonType
        {
            iValid, iIvalidTID, iInvalidMID, iInvalidTIDMID, iInvalidSN, iRecordFound, iRecordNotFound, iServiceNotFound, iServiceFound
        }

        public enum SearchType
        {
            iTerminal, iSIM, iMerchant, iReason, iFSRAttempt, iIR, iProvince, iRegion, iFE, iSP, iClient, iTerminalStatus, iSIMStatus, iDashboard
        }

        public enum ReasonType
        {
            iReason, iResolution
        }

        public enum UserFunctionType
        {
            isAdd, isDelete, isUpdate, isView, isPrint
        }

        class jsonObj
        {
            public object outParamValue { get; set; }
        }

        public static int ClassResponseCode
        {
            get { return ResponseCode; }
            set { ResponseCode = value; }
        }
        public static string ClassResponseDescription
        {
            get { return ResponseDescription; }
            set { ResponseDescription = value; }
        }

        public void APIGETRequest(string sMethod, string StatementType, string SearchBy, string SearchValue, string Action)
        {
            string sAPIURL = clsGlobalVariables.strAPIURL;
            string sAPIPath = clsGlobalVariables.strAPIPath;
            string sAPIKey = clsGlobalVariables.strAPIKeys;
            string sAPIAuthUser = clsGlobalVariables.strAPIAuthUser;
            string sAPIAuthPassword = clsGlobalVariables.strAPIAuthPassword;
            string sAPIContentType = clsGlobalVariables.strAPIContentType;
            string sAPIBank = clsGlobalVariables.strAPIBank;
            string baseAddress = "";
            string api_key = "";
            string apiPath = "";
            string sAPI = "";
            string sContentBody = "";
            string sResponse = "";
            string p_outResult = "@p_outResult";
            bool isHasOutput = false;

            dbDump = new clsFile();

            //Debug.WriteLine("--APIGETRequest==>Start--");

            clsSearch.ClassStatementType = StatementType;
            clsSearch.ClassSearchBy = SearchBy;
            clsSearch.ClassSearchValue = SearchValue;

            //Debug.WriteLine("APIGETRequest->sMethod=" + sMethod);
            //Debug.WriteLine("APIGETRequest->StatementType=" + StatementType);
            //Debug.WriteLine("APIGETRequest->SearchBy=" + SearchBy);
            //Debug.WriteLine("APIGETRequest->SearchValue=" + SearchValue);
            //Debug.WriteLine("APIGETRequest->Action=" + Action);

            if (Action.Equals("GetInfoDetail") ||
                Action.Equals("DeleteFileInfo") ||
                Action.Equals("CheckFileInfo"))
            {
                isHasOutput = true;
                p_outResult = "@p_outResult";
            }

            if (Action.Equals("CheckRecordExist") ||
                Action.Equals("CheckFileExist"))
            {
                isHasOutput = true;
                p_outResult = "@p_outCount";
            }

            if (Action.Equals("CheckControlID"))
            {
                isHasOutput = true;
                p_outResult = "@p_outID";
            }
            
            //Debug.WriteLine("isHasOutput=" + isHasOutput);

            Debug.WriteLine("--Stored Procedure--");
            string spString = "CALL sp" + Action + "(" + "\"" + dbFunction.CheckAndSetStringValue(StatementType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchBy) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchValue) + "\"" + (isHasOutput ? "," + p_outResult : "") + ")" + ";";
            Debug.WriteLine(spString);

            if (isHasOutput)
                Debug.WriteLine("SELECT " + p_outResult + ";");
        
            dbDump.WriteAPILog(0, "Stored Procedure: " + spString);

            try
            {
                baseAddress = getAPISSLEnable() + sAPIURL;
                api_key = sAPIKey + "&bank=" + sAPIBank;

                switch (Action)
                {
                    case "ViewAdvanceFSR":
                        apiPath = sAPIPath + "?api_key=" + api_key + 
                                  "&p_StatementType=" + clsSearch.ClassStatementType + 
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy + 
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +                                  
                                  "&action=" + Action;
                        break;
                    case "ViewAdvanceTerminal":
                        if (SearchBy.CompareTo("TerminalSN List") == 0 ||
                            SearchBy.CompareTo("SIMSN List") == 0 ||
                            SearchBy.CompareTo("Stock Detail List") == 0)
                        {
                            apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +                                  
                                  "&action=" + Action;
                        }
                        else
                        {
                            apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +
                                  "&p_TerminalSN=" + clsSearch.ClassTerminalSN +
                                  "&p_TerminalType=" + clsSearch.ClassTerminalType +
                                  "&p_TerminalModel=" + clsSearch.ClassTerminalModel +
                                  "&p_TerminalBrand=" + clsSearch.ClassTerminalBrand +
                                  "&p_TerminalStatus=" + clsSearch.ClassTerminalStatusType +
                                  "&action=" + Action;
                        }
                        
                        break;
                    case "ViewAdvanceParticular":

                        if (SearchBy.CompareTo("Advance Particular") == 0 ||
                            SearchBy.CompareTo("Merchant List") == 0 ||
                            SearchBy.CompareTo("Client List") == 0 ||
                            SearchBy.CompareTo("Field Engineer List") == 0 ||
                            SearchBy.CompareTo("Supplier List") == 0 ||
                            SearchBy.CompareTo("Service Provider List") == 0)
                        {
                            apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +                                  
                                  "&action=" + Action;
                        }
                        else
                        {
                            apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +
                                  "&p_ParticularName=" + clsSearch.ClassParticularName +
                                  "&action=" + Action;
                        }
                        
                        break;
                    case "ViewAdvanceIR":
                        apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +                                  
                                  "&action=" + Action;
                        break;
                    case "ViewAdvanceTA":
                        apiPath = sAPIPath + "?api_key=" + api_key +
                                  "&p_StatementType=" + clsSearch.ClassStatementType +
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy +
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue +
                                  "&action=" + Action;
                        break;
                    default:
                        apiPath = sAPIPath + "?api_key=" + api_key + 
                                  "&p_StatementType=" + clsSearch.ClassStatementType + 
                                  "&p_SearchBy=" + clsSearch.ClassSearchBy + 
                                  "&p_SearchValue=" + clsSearch.ClassSearchValue + 
                                  "&action=" + Action;                        
                        break;
                }
                
                sAPI = baseAddress + apiPath;

                //Debug.WriteLine("sAPI=" + sAPI);
                //Debug.WriteLine(sAPI);
                Debug.WriteLine("----------------------------------------------------------------");
                Debug.WriteLine("[APIGETRequest]");
                Debug.WriteLine("EndPoint=" + sAPI);              
                Debug.WriteLine("----------------------------------------------------------------");

                clsGlobalVariables.strJSONRequest = sAPI;

                dbDump.WriteAPILog(0, "APIGETRequest Request->" + clsGlobalVariables.strJSONRequest);

                if (clsSystemSetting.ClassSystemPromptAPIRequest > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONRequest, "APIGETRequest: API Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sAPI);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sAPIAuthUser + ":" + sAPIAuthPassword));
                request.Method = sMethod;
                request.ContentType = "application/json";
                request.Timeout = Timeout.Infinite;
                request.KeepAlive = true;

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                sResponse = responseReader.ReadToEnd();

                // save response to file
                saveResponseToFile(sAPI, sResponse, sContentBody);

                // Dump Request Log
                dbDump.WriteAPILog(0, clsGlobalVariables.strJSONRequest);
                
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }

            clsGlobalVariables.strJSONResponse = sResponse;

            if (clsSystemSetting.ClassSystemPromptAPIResponse > 0)
                MessageBox.Show(clsGlobalVariables.strJSONResponse, "APIGETRequest: API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Dump Response Log
            dbDump.WriteAPILog(1, clsGlobalVariables.strJSONResponse);

            // Dump Response As File
            //Debug.WriteLine("clsSearch._isWriteResponse="+ clsSearch._isWriteResponse);
            if (clsSearch._isWriteResponse)
            {
                GetReponseFileName(StatementType, SearchBy, SearchValue, Action);
                if (clsSearch.ClassResponseFileName != null)
                {
                    if (clsSearch.ClassResponseFileName.Length > 0)
                    {
                        Debug.WriteLine("Write response file =" + clsSearch.ClassResponseFileName);
                        JObject json2 = JObject.Parse(clsGlobalVariables.strJSONResponse);
                        dbDump.WriteResponse(clsSearch.ClassAPIMethod, clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassAction, json2);
                    }
                }
            }
                                    
            //Debug.WriteLine("--APIGETRequest==>End--");
        }

        public void APIPUTRequest(string sMethod, string StatementType, string SearchBy, string SearchValue, string Action)
        {
            string sAPIURL = clsGlobalVariables.strAPIURL;
            string sAPIPath = clsGlobalVariables.strAPIPath;
            string sAPIKey = clsGlobalVariables.strAPIKeys;
            string sAPIAuthUser = clsGlobalVariables.strAPIAuthUser;
            string sAPIAuthPassword = clsGlobalVariables.strAPIAuthPassword;
            string sAPIContentType = clsGlobalVariables.strAPIContentType;
            string sAPIBank = clsGlobalVariables.strAPIBank;
            string baseAddress = "";
            string api_key = "";
            string apiPath = "";
            string sAPI = "";
            string sContentBody = "";
            string sRequest = "";
            string sResponse = "";
            
            //Debug.WriteLine("--APIPUTRequest==>Start--");

            //Debug.WriteLine("sMethod="+ sMethod);
            //Debug.WriteLine("StatementType=" + StatementType);
            //Debug.WriteLine("Action=" + Action);

            Debug.WriteLine("--Stored Procedure--");
            string spString = "CALL sp" + Action + "(" + "\"" + dbFunction.CheckAndSetStringValue(StatementType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchBy) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchValue) + "\"" + ")" + ";";
            Debug.WriteLine(spString);

            dbDump = new clsFile();
            dbDump.WriteAPILog(0, "Stored Procedure: " + spString);
            
            try
            {
                baseAddress = getAPISSLEnable() + sAPIURL;
                api_key = sAPIKey + "&bank=" + sAPIBank;
                apiPath = sAPIPath + "?api_key=" + api_key + "&action=" + Action;
                sAPI = baseAddress + apiPath;

                // Set Content Type
                switch (Action)
                {
                    case "UpdateEquipment":
                        break;
                    case "UpdateCity":
                        sContentBody = clsContent.SetCityContentData("Update", clsCity.ClassCityID.ToString(), clsCity.ClassCity);
                        break;
                    case "UpdateClient":
                        break;
                    case "UpdateFE":
                        sContentBody = clsContent.SetFEContentData("Update", clsFE.ClassFEID.ToString(), clsFE.ClassName, clsFE.ClassAddress, clsFE.ClassContactNo);
                        break;
                    case "UpdateMasterFile":
                        break;
                    case "UpdateMerchant":
                        break;
                    case "UpdateProvince":
                        sContentBody = clsContent.SetProvinceContentData("Update", clsProvince.ClassProvinceID.ToString(), clsProvince.ClassProvince);
                        break;
                    case "UpdateReason":
                        break;
                    case "UpdateServiceType":
                        sContentBody = clsContent.SetServiceTypeContentData("Update", clsServiceType.ClassServiceTypeID.ToString(), clsServiceType.ClassDescription, clsServiceType.ClassCode);
                        break;
                    case "UpdateStatus":
                        sContentBody = clsContent.SetUpdateStatusContentData("Update", clsSearch.ClassMaintenanceType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassStatus.ToString(), clsSearch.ClassStatusDescription);
                        break;
                    case "UpdateTerminalType":
                        sContentBody = clsContent.SetTerminalTypeContentData("Update", clsTerminalType.ClassTerminalTypeID.ToString(), clsTerminalType.ClassDescription);
                        break;
                    case "UpdateTerminalModel":
                        sContentBody = clsContent.SetTerminalModelContentData("Update", clsTerminalModel.ClassTerminalModelID.ToString(), clsTerminalModel.ClassDescription);
                        break;
                    case "UpdateTerminalBrand":
                        sContentBody = clsContent.SetTerminalBrandContentData("Update", clsTerminalBrand.ClassTerminalBrandID.ToString(), clsTerminalBrand.ClassDescription);
                        break;
                    case "UpdateTerminalStatus":
                        sContentBody = clsContent.SetTerminalStatusContentData("Update", clsTerminalStatus.ClassTerminalStatusID.ToString(), clsTerminalStatus.ClassDescription);
                        break;
                    case "UpdateUoM":
                        break;
                    case "UpdateUser":
                        sContentBody = clsContent.SetUserContentData("Update", clsUser.ClassUserID.ToString(), clsUser.ClassUserName, clsUser.ClassPassword, clsUser.ClassUserFullName, clsUser.ClassUserType);
                        break;
                    case "UpdateParticular":
                        sContentBody = clsContent.SetParticularContentData("Update", clsParticular.ClassParticularID.ToString(), clsParticular.ClassProvinceID.ToString(), clsParticular.ClassCityID.ToString(), clsParticular.ClassParticularTypeID.ToString(), clsParticular.ClassParticularDescription, clsParticular.ClassParticularName, clsParticular.ClassAddress, clsParticular.ClassAddress2, clsParticular.ClassAddress3, clsParticular.ClassAddress4, clsParticular.ClassContactPerson, clsParticular.ClassTelNo, clsParticular.ClassMobile, clsParticular.ClassFax, clsParticular.ClassEmail, clsParticular.ClassContractTerms);
                        break;
                    case "UpdateCollectionDetail":
                        sContentBody = clsContent.SetUpdateCollectionDetailContentData(clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue);
                        break;
                    case "UpdateBulkCollectionDetail":
                        sContentBody = clsContent.SetUpdateBulkCollectionDetailContentData(clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue);
                        break;
                }

                sRequest = "Method : " + sMethod + "\n" +
                           "StatementType : " + StatementType + "\n" +
                           "Action : " + Action + "\n\n" +
                           "Content: " + sContentBody + "\n" + 
                           "API " + "[" + sAPI + "]" + "\n";

                Debug.WriteLine("sRequest=" + sRequest);

                Debug.WriteLine("----------------------------------------------------------------");
                Debug.WriteLine("[APIPUTRequest]");
                Debug.WriteLine("EndPoint="+sAPI);
                Debug.WriteLine("Content=" + sContentBody);
                Debug.WriteLine("----------------------------------------------------------------");

                clsGlobalVariables.strJSONRequest = sRequest;

                if (clsSystemSetting.ClassSystemPromptAPIRequest > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONRequest, "APIPUTRequest: API Request", MessageBoxButtons.OK, MessageBoxIcon.Information);


                dbDump.WriteAPILog(0, "APIPUTRequest Request->" + clsGlobalVariables.strJSONRequest);

                // Dump Request Log
                dbDump.WriteAPILog(0, clsGlobalVariables.strJSONRequest);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sAPI);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sAPIAuthUser + ":" + sAPIAuthPassword));
                request.Method = sMethod;
                request.ContentType = "application/json";
                request.Timeout = Timeout.Infinite;
                request.KeepAlive = true;

                // Include Content
                ASCIIEncoding encoding = new ASCIIEncoding();                
                Byte[] bytes = encoding.GetBytes(sContentBody);

                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                sResponse = responseReader.ReadToEnd();

                clsGlobalVariables.strJSONResponse = sResponse;

                if (clsSystemSetting.ClassSystemPromptAPIResponse > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONResponse, "APIPUTRequest: API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // save response to file
                saveResponseToFile(sAPI, sResponse, sContentBody);

                // Dump Response Log
                dbDump.WriteAPILog(1, clsGlobalVariables.strJSONResponse);

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }

            //Debug.WriteLine("--APIPUTRequest==>End--");
        }

        public void APIDELETERequest(string sMethod, string StatementType, string SearchBy, string SearchValue, string MaintenanceType, string Action)
        {
            string sAPIURL = clsGlobalVariables.strAPIURL;
            string sAPIPath = clsGlobalVariables.strAPIPath;
            string sAPIKey = clsGlobalVariables.strAPIKeys;
            string sAPIAuthUser = clsGlobalVariables.strAPIAuthUser;
            string sAPIAuthPassword = clsGlobalVariables.strAPIAuthPassword;
            string sAPIContentType = clsGlobalVariables.strAPIContentType;
            string sAPIBank = clsGlobalVariables.strAPIBank;
            string baseAddress = "";
            string api_key = "";
            string apiPath = "";
            string sAPI = "";
            string sContentBody = "";
            string sRequest = "";
            string sResponse = "";
            
            //Debug.WriteLine("--APIDELETERequest==>Start--");

            //Debug.WriteLine("sMethod="+ sMethod);
            //Debug.WriteLine("StatementType=" + StatementType);
            //Debug.WriteLine("Action=" + Action);

            Debug.WriteLine("--Stored Procedure--");
            string spString = "CALL sp" + Action + "(" + "\"" + dbFunction.CheckAndSetStringValue(StatementType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchBy) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchValue) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(MaintenanceType) + "\"" + ")" + ";";
            Debug.WriteLine(spString);

            dbDump = new clsFile();
            dbDump.WriteAPILog(0, "Stored Procedure: " + spString);
            
            try
            {
                baseAddress = getAPISSLEnable() + sAPIURL;
                api_key = sAPIKey + "&bank=" + sAPIBank;
                apiPath = sAPIPath + "?api_key=" + api_key + "&action=" + Action;
                sAPI = baseAddress + apiPath;

                // Set Content Type
                switch (Action)
                {                    
                    case "DeleteCollectionDetail":
                        sContentBody = clsContent.SetDeleteCollectionDetailContentData(clsSearch.ClassStatementType, clsSearch.ClassSearchBy, clsSearch.ClassSearchValue, clsSearch.ClassMaintenanceType);
                        break;

                }

                sRequest = "Method : " + sMethod + "\n" +
                           "StatementType : " + StatementType + "\n" +
                           "Action : " + Action + "\n\n" +
                           "Content: " + sContentBody + "\n" +
                           "API " + "[" + sAPI + "]" + "\n";

                Debug.WriteLine("sRequest=" + sRequest);

                clsGlobalVariables.strJSONRequest = sRequest;

                Debug.WriteLine("----------------------------------------------------------------");
                Debug.WriteLine("[APIDELETERequest]");
                Debug.WriteLine("EndPoint=" + sAPI);
                Debug.WriteLine("Content=" + sContentBody);
                Debug.WriteLine("----------------------------------------------------------------");

                if (clsSystemSetting.ClassSystemPromptAPIRequest > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONRequest, "APIDELETERequest: API Request", MessageBoxButtons.OK, MessageBoxIcon.Information);


                dbDump.WriteAPILog(0, "APIDELETERequest Request->" + clsGlobalVariables.strJSONRequest);

                // Dump Request Log
                dbDump.WriteAPILog(0, clsGlobalVariables.strJSONRequest);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sAPI);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sAPIAuthUser + ":" + sAPIAuthPassword));
                request.Method = sMethod;
                request.ContentType = "application/json";
                request.Timeout = Timeout.Infinite;
                request.KeepAlive = true;

                // Include Content
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(sContentBody);

                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                sResponse = responseReader.ReadToEnd();

                clsGlobalVariables.strJSONResponse = sResponse;

                if (clsSystemSetting.ClassSystemPromptAPIResponse > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONResponse, "APIDELETERequest: API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // save response to file
                saveResponseToFile(sAPI, sResponse, sContentBody);

                // Dump Response Log
                dbDump.WriteAPILog(1, clsGlobalVariables.strJSONResponse);

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }

            //Debug.WriteLine("--APIDELETERequest==>End--");
        }

        public void APIPOSTRequest(string sMethod, string StatementType, string SearchBy, string SearchValue, string MaintenanceType, string SQL, string Action)
        {
            string sAPIURL = clsGlobalVariables.strAPIURL;
            string sAPIPath = clsGlobalVariables.strAPIPath;
            string sAPIKey = clsGlobalVariables.strAPIKeys;
            string sAPIAuthUser = clsGlobalVariables.strAPIAuthUser;
            string sAPIAuthPassword = clsGlobalVariables.strAPIAuthPassword;
            string sAPIContentType = clsGlobalVariables.strAPIContentType;
            string sAPIBank = clsGlobalVariables.strAPIBank;
            string baseAddress = "";
            string api_key = "";
            string apiPath = "";
            string sAPI = "";
            string sContentBody = "";
            string sRequest = "";
            string sResponse = "";
            string spString = "";
            string p_outErrorMessage = "@p_outErrorMessage";
            string p_outLastInsertID = "@p_outLastInsertID";

            //Debug.WriteLine("--APIPOSTRequest==>Start--");

            //Debug.WriteLine("sMethod="+ sMethod);
            //Debug.WriteLine("StatementType=" + StatementType);
            //Debug.WriteLine("MaintenanceType=" + MaintenanceType);
            //Debug.WriteLine("SQL=" + SQL);
            //Debug.WriteLine("Action=" + Action);

            Debug.WriteLine("--Stored Procedure--");

            if (SearchBy.Equals("TimeSheet"))
                spString = "CALL sp" + Action + "(" + "\"" + dbFunction.CheckAndSetStringValue(StatementType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchBy) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchValue) + "\""  + "," + "\"" + dbFunction.CheckAndSetStringValue(MaintenanceType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SQL) + "\"" + "," + p_outErrorMessage + "," + p_outLastInsertID + ")" + ";";
            else
                spString = "CALL sp" + Action + "(" + "\"" + dbFunction.CheckAndSetStringValue(StatementType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchBy) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SearchValue) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(MaintenanceType) + "\"" + "," + "\"" + dbFunction.CheckAndSetStringValue(SQL) + ")" + ";";

            Debug.WriteLine(spString);
            Debug.WriteLine("SELECT " + p_outErrorMessage + ", " + p_outLastInsertID + ";");

            dbDump = new clsFile();
            dbDump.WriteAPILog(0, "Stored Procedure: " + spString);
            
            try
            {
                baseAddress = getAPISSLEnable() + sAPIURL;
                api_key = sAPIKey + "&bank=" + sAPIBank;
                apiPath = sAPIPath + "?api_key=" + api_key + "&action=" + Action;
                sAPI = baseAddress + apiPath;

                clsSearch.ClassAPIData = sAPI;

                sRequest = "\nMethod : " + clsSearch.ClassAPIMethod + "\n" +
                           "StatementType : " + clsSearch.ClassStatementType + "\n" +
                           "MaintenanceType : " + clsSearch.ClassMaintenanceType + "\n" +
                           "SearchBy : " + clsSearch.ClassSearchBy + "\n" +
                           "SearchValue : " + clsSearch.ClassSearchValue + "\n" +
                           "SQL : " + clsSearch.ClassSQL + "\n" +
                           "Action : " + clsSearch.ClassAction + "\n\n" +
                           "API " + "[" + clsSearch.ClassAPIData + "]" + "\n";

                Debug.WriteLine("sRequest=" + sRequest);

                clsGlobalVariables.strJSONRequest = sRequest;

                if (clsSystemSetting.ClassSystemPromptAPIRequest > 0)
                    MessageBox.Show(clsGlobalVariables.strJSONRequest, "APIPOSTRequest: API Request", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dbDump.WriteAPILog(0, "APIPOSTRequest Request->" + clsGlobalVariables.strJSONRequest);

                // Dump Request Log
                dbDump.WriteAPILog(0, clsGlobalVariables.strJSONRequest);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(clsSearch.ClassAPIData);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(sAPIAuthUser + ":" + sAPIAuthPassword));
                request.Method = sMethod;
                request.ContentType = "application/json";
                request.Timeout = Timeout.Infinite;
                request.KeepAlive = true;

                // Include Content
                ASCIIEncoding encoding = new ASCIIEncoding();
                sContentBody = clsContent.InsertMaintenanceMasterContentData(clsSearch.ClassStatementType, clsSearch.ClassMaintenanceType, clsSearch.ClassSQL);

                // Debug
                Debug.WriteLine("POST Request [" + clsGlobalVariables.strJSONRequest + "]" + "\n");
                Debug.WriteLine("POST Content [" + sContentBody + "]" + "\n");

                Byte[] bytes = encoding.GetBytes(sContentBody);

                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                
                try
                {
                    WebResponse webResponse = request.GetResponse();
                    Stream webStream = webResponse.GetResponseStream();
                    StreamReader responseReader = new StreamReader(webStream);
                    sResponse = responseReader.ReadToEnd();
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {                        
                        MessageBox.Show("API Response Timeout.", "APIPOSTRequest: API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string message = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        MessageBox.Show(message, "APIPOSTRequest: API Response" + "\nError Message:\n" + p_outErrorMessage, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine("[APIPOSTRequest]");
            Debug.WriteLine("EndPoint=" + sAPI);
            Debug.WriteLine("Content=" + sContentBody);
            Debug.WriteLine("----------------------------------------------------------------");

            clsGlobalVariables.strJSONResponse = sResponse;

            if (clsSystemSetting.ClassSystemPromptAPIResponse > 0)
                MessageBox.Show(clsGlobalVariables.strJSONResponse, "APIPOSTRequest: API Response", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Debug
            Debug.WriteLine("POST 'Response [" + clsGlobalVariables.strJSONResponse + "]" + "\n");

            // save response to file
            saveResponseToFile(sAPI, sResponse, sContentBody);

            // Dump Response Log
            dbDump.WriteAPILog(1, clsGlobalVariables.strJSONResponse);

            //Debug.WriteLine("--APIPOSTRequest==>End--");
        }

        public void ExecuteAPI(string APIMethod, string StatementType, string SearchBy, string SearchValue, string MaintenanceType, string SQL, string Action)
        {
            clsInternet dbInternet = new clsInternet();
            string pParseDelimetedString = "";
            
            // Check internet connection
            if (clsSystemSetting.ClassSystemCheckNetLink > 0)
            {
                // Check internet connection            
                if (!dbInternet.CheckInternetConnection(clsSystemSetting.ClassSystemNetLink))
                {
                    MessageBox.Show("Please check internet connection.", "Internet unavailable.", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                    return;
                }
            }

            dbFunction = new clsFunction();

            dbINIAPI = new clsINI();
            dbINIAPI.InitAPISetting();

            dbINISystem = new clsINI();
            dbINIAPI.InitSystemSetting();

            dbAPI = new clsAPI();

            dbDump = new clsFile();
            
            clsGlobalVariables.sAPIResponseCode = "";
            clsGlobalVariables.isAPIResponseOK = false;
            clsGlobalVariables.strJSONRequest = "";
            clsGlobalVariables.strJSONResponse = "";

            clsSearch.ClassRecordFound = true;
            clsSearch.ClassAPIMethod = APIMethod;
            clsSearch.ClassStatementType = StatementType;
            clsSearch.ClassMaintenanceType = MaintenanceType;
            clsSearch.ClassSearchBy = SearchBy;
            clsSearch.ClassSearchValue = SearchValue;
            clsSearch.ClassSQL = SQL;
            clsSearch.ClassAction = Action;
            
            string sLog =   "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "--ExecuteAPI--" + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "[API]" + "\n" +
                            ">URL=" + clsSearch.ClassAPIURL + "\n" +
                            ">Path=" + clsSearch.ClassAPIPath + "\n" +
                            ">ContentType=" + clsSearch.ClassAPIContentType + "\n" +
                            ">UserName=" + clsSearch.ClassAPIAuthUser + "\n" +
                            ">Password=" + clsSearch.ClassAPIAuthPassword + "\n" +
                            ">Keys=" + clsSearch.ClassAPIKeys + "\n" +
                            ">BankCode=" + clsSearch.ClassBankCode + "\n" +
                            ">BankName=" + clsSearch.ClassBankName + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "[PARAMETERS]" + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            ">Method=" + clsSearch.ClassAPIMethod + "\n" +
                            ">Action=" + clsSearch.ClassAction + "\n" +
                            ">StatementType=" + clsSearch.ClassStatementType + "\n" +
                            ">SearchBy=" + clsSearch.ClassSearchBy + "\n" +
                            ">SearchValue=" + clsSearch.ClassSearchValue + "\n" +
                            ">MaintenanceType=" + clsSearch.ClassMaintenanceType + "\n" +
                            ">SQL=" + clsSearch.ClassSQL + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "--ExecuteAPI--" + "\n" +
                            clsFunction.sLineSeparator + "\n";

            Debug.WriteLine(sLog);
            dbDump.WriteAPILog(0, "ExecuteAPI Log" + sLog);

            dbFunction.GetRequestTime("Method=" + dbFunction.AddBracketStartEnd(APIMethod) +
                                                                    ",Action="+dbFunction.AddBracketStartEnd(Action) +
                                                                    ",StatementType=" + dbFunction.AddBracketStartEnd(StatementType) +
                                                                    ",SearchBy=" +dbFunction.AddBracketStartEnd(SearchBy) + 
                                                                    ",MaintenanceType="+dbFunction.AddBracketStartEnd(MaintenanceType));

            try
            {
                // GET - Search / View
                if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_GET) == 0)
                {
                    pParseDelimetedString = dbFunction.parseDelimitedString(SearchValue, clsDefines.gPipe, 1);
                    dbDump.WriteAPILog(0, "ExecuteAPI > GET METHOD [Parse]" + pParseDelimetedString);

                    APIGETRequest(APIMethod, StatementType, SearchBy, SearchValue, Action);

                    // Create logs
                    dbDump.WriteSysytemLog($"API GET REQUEST....[{clsGlobalVariables.strJSONRequest}]"); // add log
                    dbDump.WriteSysytemLog($"API GET RESPONSE...[{clsGlobalVariables.strJSONResponse}]"); // add log
                    dbDump.WriteSysytemLog($"API GET --------------------------------------------------"); // add log
                }

                // PUT - Update
                if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_PUT) == 0)
                {
                    pParseDelimetedString = dbFunction.parseDelimitedString(SearchValue, clsDefines.gPipe, 1);
                    dbDump.WriteAPILog(0, "ExecuteAPI > PUT METHOD [Parse]" + pParseDelimetedString);

                    APIPUTRequest(APIMethod, StatementType, SearchBy, SearchValue, Action);

                    dbDump.WriteSysytemLog($"API PUT REQUEST....[{clsGlobalVariables.strJSONRequest}]"); // add log
                    dbDump.WriteSysytemLog($"API PUT RESPONSE...[{clsGlobalVariables.strJSONResponse}]"); // add log
                    dbDump.WriteSysytemLog($"API PUT --------------------------------------------------"); // add log
                }

                // DELETE - Delete
                if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_DELETE) == 0)
                {
                    pParseDelimetedString = dbFunction.parseDelimitedString(SearchValue, clsDefines.gPipe, 1);
                    dbDump.WriteAPILog(0, "ExecuteAPI > DELETE METHOD [Parse]" + pParseDelimetedString);

                    APIDELETERequest(APIMethod, StatementType, SearchBy, SearchValue, MaintenanceType, Action);

                    dbDump.WriteSysytemLog($"API DELETE REQUEST....[{clsGlobalVariables.strJSONRequest}]"); // add log
                    dbDump.WriteSysytemLog($"API DELETE RESPONSE...[{clsGlobalVariables.strJSONResponse}]"); // add log
                    dbDump.WriteSysytemLog($"API DELETE --------------------------------------------------"); // add log
                }

                // POST - Insert
                if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_POST) == 0)
                {
                    pParseDelimetedString = dbFunction.parseDelimitedString(SearchValue, clsDefines.gComma, 1);
                    dbDump.WriteAPILog(0, "ExecuteAPI > POST METHOD [Parse]" + pParseDelimetedString);

                    APIPOSTRequest(APIMethod, StatementType, SearchBy, SearchValue, MaintenanceType, SQL, Action);

                    dbDump.WriteSysytemLog($"API POST REQUEST....[{clsGlobalVariables.strJSONRequest}]"); // add log
                    dbDump.WriteSysytemLog($"API POST RESPONSE...[{clsGlobalVariables.strJSONResponse}]"); // add log
                    dbDump.WriteSysytemLog($"API POST --------------------------------------------------"); // add log
                }
            }
            catch (Exception ex)
            {
                dbDump.WriteSysytemLog($"API FAILED...Message[{ex.Message}]"); // add log

                dbFunction.SetMessageBox("Error executing API" + "\n\n" +
                                          ">Method:" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIMethod) + "\n" +
                                           ">Action:" + dbFunction.AddBracketStartEnd(clsSearch.ClassAction) + "\n" +
                                           ">StatementType:" + dbFunction.AddBracketStartEnd(clsSearch.ClassStatementType) + "\n" +
                                           ">SearchBy:" + dbFunction.AddBracketStartEnd(clsSearch.ClassSearchBy) + "\n" +
                                           ">SearchValue:" + dbFunction.AddBracketStartEnd(clsSearch.ClassSearchValue) + "\n" +
                                           ">MaintenanceType:" + dbFunction.AddBracketStartEnd(clsSearch.ClassMaintenanceType) + "\n" +
                                           ">SQL:" + dbFunction.AddBracketStartEnd(clsSearch.ClassSQL) + "\n" +
                                            clsFunction.sLineSeparator + "\n\n" +
                                            "Error Message:" + dbFunction.AddBracketStartEnd(ex.Message) + "\n\n" +
                                            clsDefines.CONTACT_ADMIN_MESSAGE, "API execution failed.", clsFunction.IconType.iError);
            }
            
            ValidateResponse Response = new ValidateResponse();

            dbFunction.GetResponseTime("Method=" + dbFunction.AddBracketStartEnd(APIMethod) +
                                                                    ",Action=" + dbFunction.AddBracketStartEnd(Action) +
                                                                    ",StatementType=" + dbFunction.AddBracketStartEnd(StatementType) +
                                                                    ",SearchBy=" + dbFunction.AddBracketStartEnd(SearchBy) +
                                                                    ",MaintenanceType=" + dbFunction.AddBracketStartEnd(MaintenanceType));

            if (Response != null)
            {
                Debug.WriteLine("[3]Response is not equal to NULL");
                // do nothing
            }
            else
            {
                dbDump.WriteSysytemLog($"API FAILED...[Unable to receive response from server]"); // add log

                dbFunction.SetMessageBox("Unable to receive response from API WebServer. " +
                                     "\n" +
                                     "-Check is WebServer is running." +
                                     "\n" +
                                     "-Client is connected to WebServer" +
                                     "\n" +
                                     "Please contact administrator. Application will exit.",
                                     "No response from API WebServer", clsFunction.IconType.iError);

                Application.Exit();
            }

            try
            {             
                // Validate Response
                Response = JsonConvert.DeserializeObject<ValidateResponse>(clsGlobalVariables.strJSONResponse);

                //GetBeautifyJSON(clsGlobalVariables.strJSONResponse);

                if (clsGlobalVariables.strJSONResponse.Length <= 0)
                {
                    dbDump.WriteAPILog(2, "ExecuteAPI Log Response 1->" + clsGlobalVariables.strJSONResponse);

                    clsGlobalVariables.sAPIResponseCode = clsGlobalVariables.API_RESPONSE_ERROR;
                    PromptAPIMessage(true, clsGlobalVariables.API_RESPONSE_ERROR);                    

                    iAPIResponseCounter++;
                    
                    return;
                }
                
            }
            catch (Exception ex)
            {
                dbDump.WriteSysytemLog($"API FAILED...Message[{ex.Message}]"); // add log

                dbDump.WriteAPILog(2, "ExecuteAPI Log Excetion 1->" + ex.Message);
                
                Debug.WriteLine("[1]ExecuteAPI encountered error "+ex.Message);
                clsGlobalVariables.isAPIResponseOK = false;
                clsGlobalVariables.ExceptionMessage = ex.Message;
                clsGlobalVariables.sAPIResponseCode = clsGlobalVariables.UNDEFINED_ERROR;
                PromptAPIMessage(true, clsGlobalVariables.API_RESPONSE_ERROR);

                return;
            }

            try
            {
                clsGlobalVariables.sAPIResponseCode = Response.resp_code;
                clsGlobalVariables.isAPIResponseOK = true;
            }
            catch (Exception ex)
            {
                dbDump.WriteSysytemLog($"API FAILED...Message[{ex.Message}]"); // add log

                dbDump.WriteAPILog(2, "ExecuteAPI Log Excetion 2->" + ex.Message);

                Debug.WriteLine("[2]ExecuteAPI encountered error " + ex.Message);
                clsGlobalVariables.isAPIResponseOK = false;
                clsGlobalVariables.ExceptionMessage = ex.Message;
                clsGlobalVariables.sAPIResponseCode = clsGlobalVariables.UNDEFINED_ERROR;
                PromptAPIMessage(true, clsGlobalVariables.API_RESPONSE_ERROR);

                return;
            }

            if (clsGlobalVariables.isAPIResponseOK == true)
            {
                dbDump.WriteAPILog(1, "ExecuteAPI Log Response->" + clsGlobalVariables.strJSONResponse);

                PromptAPIMessage(true, clsGlobalVariables.sAPIResponseCode);               

                if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.SUCCESS_RESPONSE) != 0)
                    return;
            }
            else
            {
                PromptAPIMessage(true, clsGlobalVariables.sAPIResponseCode);                
                return;
            }

            try
            {                
                if (Response.resp_code.CompareTo(clsGlobalVariables.SUCCESS_RESPONSE) != 0)
                {
                    clsGlobalVariables.isAPIResponseOK = false;                   

                    //MessageBox.Show(Response.message);
                }
                else
                {
                    clsGlobalVariables.isAPIResponseOK = true;

                    if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_PUT) == 0)
                    {
                        //MessageBox.Show(Response.message);
                    }
                    else if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_DELETE) == 0)
                    {
                        //MessageBox.Show(Response.message);
                    }
                    else if (APIMethod.CompareTo(clsGlobalVariables.sAPIMethod_POST) == 0)
                    {
                        try
                        {                            
                            switch (Action)
                            {
                                case "InsertCollectionMaster": // Get Last Inserted ID                                
                                    LastIDDetailOnline DetailLastInsert1 = JsonConvert.DeserializeObject<LastIDDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                    clsLastID.RecordFound = false;

                                    foreach (var element in DetailLastInsert1.data)
                                    {
                                        clsLastID.RecordFound = true;
                                        clsLastID.ClassLastInsertedID = element.LastInsertID;
                                        clsLastID.ClassLastTableName = element.LastTableName;
                                    }
                                    break;
                                case "InsertMaintenanceMaster":
                                    LastIDDetailOnline DetailLastInsert2 = JsonConvert.DeserializeObject<LastIDDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                    clsLastID.RecordFound = false;
                                    clsLastID.ClassLastInsertedID = 0;
                                    clsLastID.ClassLastInsertedCityID = 0;
                                    clsLastID.ClassLastInsertedProvinceID = 0;

                                    foreach (var element in DetailLastInsert2.data)
                                    {
                                        clsLastID.RecordFound = true;
                                        clsLastID.ClassLastInsertedID = element.LastInsertID;
                                        clsLastID.ClassLastTableName = element.LastTableName;
                                    }

                                    switch (MaintenanceType)
                                    {
                                        case "City":
                                            clsLastID.ClassLastInsertedCityID = clsLastID.ClassLastInsertedID;
                                            break;
                                        case "Province":
                                            clsLastID.ClassLastInsertedProvinceID = clsLastID.ClassLastInsertedID;
                                            break;
                                    }

                                    break;
                                case "InsertCollectionDetail": // Get Last Inserted ID                                
                                    LastIDDetailOnline DetailLastInsert3 = JsonConvert.DeserializeObject<LastIDDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                    clsLastID.RecordFound = false;

                                    foreach (var element in DetailLastInsert3.data)
                                    {
                                        clsLastID.RecordFound = true;
                                        clsLastID.ClassLastInsertedID = element.LastInsertID;
                                        clsLastID.ClassLastTableName = element.LastTableName;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(Response.message, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                            return;
                        }
                        
                    }
                    else
                    {
                        // Local Array Variable
                        List<string> IDCol = new List<String>();
                        List<string> DescriptionCol = new List<String>();
                        List<string> CodeCol = new List<String>();
                        List<string> ServiceStatusCol = new List<String>();
                        List<string> ServiceStatusDescriptionCol = new List<String>();
                        List<string> ReferenceNoCol = new List<String>();

                        List<string> ProviderIDCol = new List<String>();
                        List<string> NameCol = new List<String>();
                        List<string> AddressCol = new List<String>();
                        List<string> TelNoCol = new List<String>();
                        List<string> MobileCol = new List<String>();
                        List<string> FaxCol = new List<String>();
                        List<string> EmailCol = new List<String>();
                        List<string> ContractTermsCol = new List<String>();

                        List<string> UserIDCol = new List<String>();
                        List<string> UserNameCol = new List<String>();
                        List<string> PasswordCol = new List<String>();
                        List<string> FullNameCol = new List<String>();
                        List<string> UserTypeCol = new List<String>();
                        List<string> MD5PasswordCol = new List<String>();
                        List<string> MobileIDCol = new List<String>();
                        List<string> MobileTerminalIDCol = new List<String>();
                        List<string> MobileTerminalNameCol = new List<String>();

                        List<string> LogIDCol = new List<String>();
                        List<string> ComputerIPCol = new List<String>();
                        List<string> ComputerNameCol = new List<String>();
                        List<string> SessionStatusCol = new List<String>();
                        List<string> SessionStatusDescriptionCol = new List<String>();
                        List<string> LogInDateCol = new List<String>();
                        List<string> LogInTimeCol = new List<String>();
                        List<string> LogOutDateCol = new List<String>();
                        List<string> LogOutTimeCol = new List<String>();
                        List<string> LogPublishVersionCol = new List<String>();

                        List<string> ContactNoCol = new List<String>();
                        List<string> CityCol = new List<String>();
                        List<string> ProvinceCol = new List<String>();
                        List<string> RegionCol = new List<String>();

                        List<string> ParticularIDCol = new List<String>();
                        List<string> ProvinceIDCol = new List<String>();
                        List<string> CityIDCol = new List<String>();
                        List<string> ParticularTypeIDCol = new List<String>();
                        List<string> ParticularDescriptionCol = new List<String>();
                        List<string> ParticularNameCol = new List<String>();                        
                        List<string> Address2Col = new List<String>();
                        List<string> Address3Col = new List<String>();
                        List<string> Address4Col = new List<String>();
                        List<string> ContactPersonCol = new List<String>();

                        List<string> MapIDCol = new List<String>();
                        List<string> MapFromCol = new List<String>();
                        List<string> MapToCol = new List<String>();
                        List<string> DelimeterCol = new List<String>();
                        List<string> ColumnIndexCol = new List<String>();
                        List<string> isMustCol = new List<String>();
                        List<string> FormatCol = new List<String>();

                        List<string> FSRNoCol = new List<String>();
                        List<string> FSRIDCol = new List<String>();
                        List<string> NoCol = new List<String>();
                        List<string> MerchantCol = new List<String>();
                        List<string> MIDCol = new List<String>();
                        List<string> TIDCol = new List<String>();
                        List<string> TimeArrivedCol = new List<String>();
                        List<string> TimeStartCol = new List<String>();
                        List<string> InvoiceNoCol = new List<String>();
                        List<string> BatchNoCol = new List<String>();                        
                        List<string> FSRCol = new List<String>();
                        List<string> FSRDateCol = new List<String>();
                        List<string> FSRTimeCol = new List<String>();
                        List<string> TxnAmtCol = new List<String>();
                        List<string> TimeEndCol = new List<String>();
                        List<string> TerminalSNCol = new List<String>();
                        List<string> MerchantContactNoCol = new List<String>();
                        List<string> MerchantRepresentativeCol = new List<String>();
                        List<string> FENameCol = new List<String>();
                        List<string> FEEmailCol = new List<String>();
                        List<string> SerialNoCol = new List<String>();
                        List<string> SerialNoListCol = new List<String>();
                        List<string> FSRStatusDescriptionCol = new List<String>();
                        List<string> FSRServiceStatusCol = new List<String>();
                        List<string> FSRServiceStatusDescriptionCol = new List<String>();
                        List<string> FSRRemarksCol = new List<String>();
                        List<string> ProcessTypeCol = new List<String>();
                        List<string> DateFromCol = new List<String>();
                        List<string> DateToCol = new List<String>();
                        List<string> DateCol = new List<String>();
                        List<string> TimeCol = new List<String>();

                        List<string> AuthCodeCol = new List<String>();
                        List<string> RefNoCol = new List<String>();
                        List<string> NRICCol = new List<String>();
                        List<string> AdditionalInformationCol = new List<String>();
                        
                        List<string> TerminalIDCol = new List<String>();
                        List<string> TIIDCol = new List<String>();
                        List<string> TerminalTypeIDCol = new List<String>();
                        List<string> TerminalModelIDCol = new List<String>();
                        List<string> TerminalBrandIDCol = new List<String>();                        
                        List<string> TypeCol = new List<String>();
                        List<string> ModelCol = new List<String>();
                        List<string> BrandCol = new List<String>();
                        List<string> DeliveryDateCol = new List<String>();
                        List<string> ReceiveDateCol = new List<String>();
                        List<string> TerminalStatusCol = new List<String>();
                        List<string> TerminalStatusTypeCol = new List<String>();
                        List<string> TerminalStatusDescriptionCol = new List<String>();
                        List<string> TerminalCountCol = new List<String>();
                        List<string> TerminalPartNoCol = new List<String>();
                        List<string> TerminalPONoCol = new List<String>();
                        List<string> TerminalInvNoCol = new List<String>();
                        List<string> TerminalTypeDescriptionCol = new List<String>();
                        List<string> TerminalModelsDescriptionCol = new List<String>();
                        List<string> TerminalBrandDescriptionCol = new List<String>();


                        List<string> MerchantIDCol = new List<String>();                        
                        List<string> MerchantNameCol = new List<String>();
                        List<string> MerchantEmailCol = new List<String>();

                        List<string> IRIDNoCol = new List<String>();
                        List<string> IRIDCol = new List<String>();
                        List<string> IRNoCol = new List<String>();
                        List<string> IRDateCol = new List<String>();
                        List<string> InstallationDateCol = new List<String>();
                        List<string> IRImportDateCol = new List<String>();                        

                        List<string> TAIDNoCol = new List<String>();
                        List<string> TAIDCol = new List<String>();                        
                        List<string> ClientIDCol = new List<String>();
                        List<string> ServiceProviderIDCol = new List<String>();                        
                        List<string> FEIDCol = new List<String>();                        
                        List<string> ServiceTypeIDCol = new List<String>();
                        List<string> OtherServiceTypeIDCol = new List<String>();                       
                        List<string> ClientNameCol = new List<String>();
                        List<string> ServiceProviderNameCol = new List<String>();
                        List<string> TypeDescriptionCol = new List<String>();
                        List<string> ModelDescriptionCol = new List<String>();
                        List<string> BrandDescriptionCol = new List<String>();                        
                        List<string> TADateTimeCol = new List<String>();
                        List<string> TAProcessedByCol = new List<String>();
                        List<string> TAModifiedByCol = new List<String>();
                        List<string> TAProcessedDateTimeCol = new List<String>();
                        List<string> TAModifiedDateTimeCol = new List<String>();
                        List<string> TARemarksCol = new List<String>();
                        List<string> TACommentsCol = new List<String>();
                        List<string> ServiceTypeDescriptionCol = new List<String>();
                        List<string> OtherServiceTypeDescriptionCol = new List<String>();
                        List<string> IRImportDateTimeCol = new List<String>();
                        List<string> RegionIDCol = new List<String>();
                        List<string> ServiceTypeStatusCol = new List<String>();
                        List<string> ServiceTypeStatusDescriptionCol = new List<String>();
                        List<string> PowerSNCol = new List<String>();
                        List<string> DockIDCol = new List<String>();
                        List<string> DockSNCol = new List<String>();
                        List<string> ServiceCodeCol = new List<String>();

                        // SIM
                        List<string> SIMIDCol = new List<String>();
                        List<string> SIMSerialNoCol = new List<String>();
                        List<string> SIMCarrierCol = new List<String>();
                        List<string> AssignedToCol = new List<String>();
                        List<string> RemarksCol = new List<String>();
                        List<string> SIMStatusCol = new List<String>();
                        List<string> SIMStatusDescriptionCol = new List<String>();

                        // Report
                        List<string> ReportIDCol = new List<String>();
                        List<string> ReportDescCol = new List<String>();
                        List<string> ReportTypeCol = new List<String>();
                        List<string> ReportOrderDisplayCol = new List<String>();

                        // Header
                        List<string> HeaderIDCol = new List<String>();
                        List<string> Header1Col = new List<String>();
                        List<string> Header2Col = new List<String>();
                        List<string> Header3Col = new List<String>();
                        List<string> Header4Col = new List<String>();
                        List<string> Header5Col = new List<String>();

                        // IR
                        List<string> IRStatusCol = new List<String>();
                        List<string> IRStatusDescriptionCol = new List<String>();

                        // System
                        List<string> SysIDCol = new List<String>();
                        List<string> PublishDateCol = new List<String>();
                        List<string> PublishVersionCol = new List<String>();

                        // Reason
                        List<string> ReasonIDCol = new List<String>();
                        List<string> ReasonCodeCol = new List<String>();
                        List<string> ReasonDescriptionCol = new List<String>();
                        List<string> ReasonTypeCol = new List<String>();
                        List<string> ReasonIsInputCol = new List<String>();

                        // Region Detail
                        List<string> RegionTypeCol = new List<String>();

                        // Service Call
                        List<string> SCNoCol = new List<String>();
                        List<string> SCDateTimeCol = new List<String>();
                        List<string> ReferralIDCol = new List<String>();
                        List<string> CustomerNameCol = new List<String>();
                        List<string> CustomerContactNoCol = new List<String>();
                        List<string> ReportedProblemCol = new List<String>();
                        List<string> ArrangementMadeCol = new List<String>();
                        List<string> SCReqDateCol = new List<String>();
                        List<string> SCReqTimeCol = new List<String>();
                        List<string> SCShipDateCol = new List<String>();
                        List<string> SCShipTimeCol = new List<String>();
                        List<string> TrackingNoCol = new List<String>();
                        List<string> SCStatusCol = new List<String>();

                        // Servicing Detail
                        List<string> ServiceNoCol = new List<String>();
                        List<string> CounterNoCol = new List<String>();
                        List<string> RequestNoCol = new List<String>();
                        List<string> ServiceDateTimeCol = new List<String>();
                        List<string> ServiceDateCol = new List<String>();
                        List<string> ServiceReqDateCol = new List<String>();
                        List<string> ServiceTimeCol = new List<String>();                      
                        List<string> ServiceReqTimeCol = new List<String>();
                        List<string> LastServiceRequestCol = new List<String>();
                        List<string> NewServiceRequestCol = new List<String>();
                        List<string> ReplaceTerminalSNCol = new List<String>();
                        List<string> ReplaceSIMSNCol = new List<String>();
                        List<string> ReplaceDockSNCol = new List<String>();
                        List<string> JobTypeCol = new List<String>();
                        List<string> JobTypeDescriptionCol = new List<String>();
                        List<string> JobTypeSubDescriptionCol = new List<String>();
                        List<string> JobTypeStatusDescriptionCol = new List<String>();
                        List<string> ServiceJobTypeDescriptionCol = new List<String>();
                        List<string> ActionMadeCol = new List<String>();

                        List<string> CurTerminalSNStatusCol = new List<String>();
                        List<string> CurSIMSNStatusCol = new List<String>();
                        List<string> CurDockSNStatusCol = new List<String>();
                        List<string> CurTerminalSNStatusDescriptionCol = new List<String>();
                        List<string> CurSIMSNStatusDescriptionCol = new List<String>();
                        List<string> CurDockSNStatusDescriptionCol = new List<String>();

                        List<string> RepTerminalSNStatusCol = new List<String>();
                        List<string> RepSIMSNStatusCol = new List<String>();
                        List<string> RepDockSNStatusCol = new List<String>();
                        List<string> RepTerminalSNStatusDescriptionCol = new List<String>();
                        List<string> RepSIMSNStatusDescriptionCol = new List<String>();
                        List<string> RepDockSNStatusDescriptionCol = new List<String>();

                        List<string> ProcessedByCol = new List<String>();
                        List<string> ModifiedByCol = new List<String>();
                        List<string> ProcessedDateTimeCol = new List<String>();
                        List<string> ModifiedDateTimeCol = new List<String>();

                        // Expenses
                        List<string> ExpensesIDCol = new List<String>();
                        List<string> TExpensesCol = new List<String>();

                        // FSR
                        List<string> ProblemReportedCol = new List<String>();
                        List<string> ActualProblemReportedCol = new List<String>();
                        List<string> ActionTakenCol = new List<String>();
                        List<string> AnyCommentsCol = new List<String>();

                        // Type                        
                        List<string> QueryStringCol = new List<String>();
                        List<string> TypeValueCol = new List<String>();

                        // Holiday
                        List<string> HolidayIDCol = new List<String>();
                        List<string> HolidayDateCol = new List<String>();
                        List<string> isActiveCol = new List<String>();

                        // LeaveType
                        List<string> LeaveNoCol = new List<String>();
                        List<string> LeaveTypeIDCol = new List<String>();                        
                        List<string> CreditLimitCol = new List<String>();
                        List<string> LeaveCreditCol = new List<String>();

                        // Department
                        List<string> DepartmentIDCol = new List<String>();
                        List<string> DepartmentCol = new List<String>();

                        // Position
                        List<string> PositionIDCol = new List<String>();
                        List<string> PositionCol = new List<String>();

                        List<string> EmploymentStatusCol = new List<String>();

                        // Movement
                        List<string> DurationCol = new List<String>();
                        List<string> DateTypeCol = new List<String>();
                        List<string> LeaveCodeCol = new List<String>();
                        List<string> LeaveDescCol = new List<String>();

                        List<string> AppVersionCol = new List<String>();
                        List<string> AppCRCCol = new List<String>();

                        List<string> PrimaryNumCol = new List<String>();
                        List<string> SecondaryNumCol = new List<String>();

                        // TimeSheet
                        List<string> TimeSheetIDCol = new List<String>();
                        List<string> TimeSheetDateCol = new List<String>();
                        List<string> TimeInCol = new List<String>();
                        List<string> TimeOutCol = new List<String>();
                        List<string> THoursCol = new List<String>();
                        List<string> TTimeCol = new List<String>();
                        List<string> TimeStatusCol = new List<String>();
                        List<string> LocalIPCol = new List<String>();
                        List<string> RemoteIPCol = new List<String>();
                        List<string> CountryCodeCol = new List<String>();
                        List<string> CountryNameCol = new List<String>();
                        List<string> TerminalNameCol = new List<String>();

                        // WorkType
                        List<string> WorkTypeIDCol = new List<String>();
                        List<string> WorkTypeCol = new List<String>();

                        // Work Arrangement
                        List<string> WorkArrangementIDCol = new List<String>();
                        List<string> DateFromToCol = new List<String>();

                        // Leave Movement                        
                        List<string> LeaveRemarksCol = new List<String>();
                        List<string> TLeaveTakenCol = new List<String>();

                        // Privacy
                        List<string> PrivacyIDCol = new List<String>();
                        List<string> FormCol = new List<String>();
                        List<string> isAddCol = new List<String>();
                        List<string> isDeleteCol = new List<String>();
                        List<string> isUpdateCol = new List<String>();
                        List<string> isViewCol = new List<String>();
                        List<string> isPrintCol = new List<String>();
                        List<string> isCheckedCol = new List<String>();

                        // Country
                        List<string> CountryIDCol = new List<String>();
                        List<string> CountryCol = new List<String>();

                        List<string> LocationCol = new List<String>();
                        List<string> AllocationCol = new List<String>();
                        List<string> AssetTypeCol = new List<String>();

                        // Movement
                        List<string> TransNoCol = new List<String>();
                        List<string> TransDateCol = new List<String>();
                        List<string> TransTimeCol = new List<String>();
                        List<string> ReleaseDateCol = new List<String>();                    
                        List<string> FromLocationIDCol = new List<String>();
                        List<string> FromLocationCol = new List<String>();
                        List<string> ToLocationIDCol = new List<String>();
                        List<string> ToLocationCol = new List<String>();

                        // POS Rental
                        List<string> RentalFeeCol = new List<String>();
                        List<string> InvoiceIDCol = new List<String>();
                        List<string> AccountNoCol = new List<String>();
                        List<string> CustomerNoCol = new List<String>();
                        List<string> InvoiceDateCol = new List<String>();
                        List<string> DateCoveredFromCol = new List<String>();
                        List<string> DateCoveredToCol = new List<String>();
                        List<string> DateDueCol = new List<String>();
                        List<string> TAmtDueCol = new List<String>();
                        List<string> ModeOfPaymentCol = new List<String>();
                        List<string> NoteToCustomerCol = new List<String>();
                        List<string> NoteToSelfCol = new List<String>();

                        List<string> Detail_InfoCol = new List<String>();

                        switch (MaintenanceType)
                        {
                            case "Service Type":
                                ServiceTypeDetailOnline Detail1 = JsonConvert.DeserializeObject<ServiceTypeDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsServiceType.RecordFound = false;
                                clsServiceType.ClassServiceStatus = clsFunction.iZero;

                                switch (StatementType)
                                {
                                    case "Search":

                                        if (SearchBy.CompareTo("JobTypeDescription") == 0)
                                        {
                                            foreach (var element in Detail1.data)
                                            {
                                                clsServiceType.RecordFound = true;                                                
                                                clsServiceType.ClassServiceStatus = element.ServiceStatus;                                                
                                            }
                                        }
                                        else
                                        {
                                            foreach (var element in Detail1.data)
                                            {
                                                clsServiceType.RecordFound = true;
                                                clsServiceType.ClassServiceTypeID = element.ServiceTypeID;
                                                clsServiceType.ClassDescription = element.Description;
                                                clsServiceType.ClassCode = element.Code;
                                                clsServiceType.ClassServiceStatus = element.ServiceStatus;
                                                clsServiceType.ClassStatusDescription = element.ServiceStatusDescription;
                                            }
                                        }
                                        
                                        break;
                                    case "View":
                                        string sServiceTypeList = "";
                                        string sServiceStatusList = "";

                                        foreach (var element in Detail1.data)
                                        {
                                            clsServiceType.RecordFound = true;
                                            IDCol.Add(element.ServiceTypeID.ToString());
                                            DescriptionCol.Add(element.Description);
                                            CodeCol.Add(element.Code);
                                            ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                            ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                            JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                            ServiceJobTypeDescriptionCol.Add(element.ServiceJobTypeDescription);

                                            // Service Type List
                                            sServiceTypeList = sServiceTypeList + element.Description + Environment.NewLine;

                                            // Service Status List
                                            sServiceStatusList = sServiceStatusList + element.ServiceStatusDescription + Environment.NewLine;

                                        }

                                        clsArray.ServiceTypeID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.ServiceTypeDescription = DescriptionCol.ToArray();
                                        clsArray.Code = CodeCol.ToArray();
                                        clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                        clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                        clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                        clsArray.ServiceJobTypeDescription = ServiceJobTypeDescriptionCol.ToArray();

                                        // Service Type List
                                        clsSearch.ClassServiceTypeList = sServiceTypeList;

                                        // Service Status List
                                        clsSearch.ClassServiceStatusList = sServiceStatusList;

                                        break;
                                }
                                break;
                            case "Terminal Type":
                            case "Terminal Base":
                                TerminalTypeDetailOnline Detail2 = JsonConvert.DeserializeObject<TerminalTypeDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminalType.ResetClass();
                                clsTerminalType.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail2.data)
                                        {
                                            clsTerminalType.RecordFound = true;
                                            clsTerminalType.ClassTerminalTypeID = element.TerminalTypeID;
                                            clsTerminalType.ClassDescription = element.Description;
                                        }
                                        break;
                                    case "View":
                                        clsSearch.ClassTerminalTypeList = "";
                                        foreach (var element in Detail2.data)
                                        {
                                            clsTerminalType.RecordFound = true;
                                            IDCol.Add(element.TerminalTypeID.ToString());
                                            DescriptionCol.Add(element.Description);

                                            clsSearch.ClassTerminalTypeList = clsSearch.ClassTerminalTypeList + element.Description + Environment.NewLine;
                                        }

                                        clsArray.TerminalTypeID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.TypeDescription = DescriptionCol.ToArray();
                                        break;
                                }
                                break;
                            case "Service Provider":
                                ServiceProviderDetailOnline Detail3 = JsonConvert.DeserializeObject<ServiceProviderDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsServiceProvider.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail3.data)
                                        {
                                            clsServiceProvider.RecordFound = true;
                                            clsServiceProvider.ClassProviderID = element.ProviderID;
                                            clsServiceProvider.ClassName = element.Name;
                                            clsServiceProvider.ClassAddress = element.Address;
                                            clsServiceProvider.ClassTelNo = element.TelNo;
                                            clsServiceProvider.ClassMobile = element.Mobile;
                                            clsServiceProvider.ClassFax = element.Fax;
                                            clsServiceProvider.ClassEmail = element.Email;
                                            clsServiceProvider.ClassContactTerms = element.ContractTerms;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail3.data)
                                        {
                                            clsServiceProvider.RecordFound = true;
                                            ProviderIDCol.Add(element.ProviderID.ToString());
                                            NameCol.Add(element.Name);
                                            AddressCol.Add(element.Address);
                                            TelNoCol.Add(element.TelNo);
                                            MobileCol.Add(element.Mobile);
                                            FaxCol.Add(element.Fax);
                                            EmailCol.Add(element.Email);
                                            ContractTermsCol.Add(element.ContractTerms);
                                        }

                                        clsArray.ProviderID = ProviderIDCol.ToArray();
                                        clsArray.Name = NameCol.ToArray();
                                        clsArray.Address = AddressCol.ToArray();
                                        clsArray.TelNo = TelNoCol.ToArray();
                                        clsArray.MobileNo = MobileCol.ToArray();
                                        clsArray.Fax = FaxCol.ToArray();
                                        clsArray.Email = EmailCol.ToArray();
                                        clsArray.ContractTerms = ContractTermsCol.ToArray();
                                        break;
                                }
                                break;
                            case "Other Service Type":
                                OtherServiceTypeDetailOnline Detail4 = JsonConvert.DeserializeObject<OtherServiceTypeDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsOtherServiceType.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail4.data)
                                        {
                                            clsOtherServiceType.RecordFound = true;
                                            clsOtherServiceType.ClassOtherServiceTypeID = element.OtherServiceTypeID;
                                            clsOtherServiceType.ClassDescription = element.Description;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail4.data)
                                        {
                                            clsOtherServiceType.RecordFound = true;
                                            IDCol.Add(element.OtherServiceTypeID.ToString());
                                            DescriptionCol.Add(element.Description);
                                        }

                                        clsArray.OtherServiceTypeID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        break;
                                }
                                break;
                            case "User":
                                UserDetailOnline Detail5 = JsonConvert.DeserializeObject<UserDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsUser.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail5.data)
                                        {
                                            clsUser.RecordFound = true;
                                            clsUser.ClassUserID = element.UserID;
                                            clsUser.ClassParticularID = element.ParticularID;
                                            clsUser.ClassUserName = element.UserName;
                                            clsUser.ClassUserFullName = element.FullName;
                                            clsUser.ClassPassword = element.Password;
                                            clsUser.ClassUserType = element.UserType;
                                            clsUser.ClassisActive = (element.isActive > 0 ? true : false);
                                            clsUser.ClassisAppVersion = (element.isAppVersion > 0 ? true : false);
                                            clsUser.ClassMD5Password = element.MD5Password;
                                        }
                                        break;
                                    case "View":

                                        switch (SearchBy)
                                        {
                                            case "User Log":
                                                foreach (var element in Detail5.data)
                                                {
                                                    clsUser.RecordFound = true;
                                                    UserIDCol.Add(element.UserID.ToString());
                                                    UserNameCol.Add(element.UserName);
                                                    FullNameCol.Add(element.FullName);
                                                    PasswordCol.Add(element.Password);
                                                    UserTypeCol.Add(element.UserType);
                                                    LogIDCol.Add(element.LogID.ToString());
                                                    SessionStatusCol.Add(element.SessionStatus.ToString());
                                                    ComputerIPCol.Add(element.ComputerIP);
                                                    ComputerNameCol.Add(element.ComputerName);
                                                    SessionStatusDescriptionCol.Add(element.SessionStatusDescription);
                                                    LogInDateCol.Add(element.LogInDate);
                                                    LogInTimeCol.Add(element.LogInTime);
                                                    LogOutDateCol.Add(element.LogOutDate);
                                                    LogOutTimeCol.Add(element.LogOutTime);
                                                    LogPublishVersionCol.Add(element.PublishVersion);

                                                }

                                                clsArray.UserID = UserIDCol.ToArray();
                                                clsArray.UserName = UserNameCol.ToArray();
                                                clsArray.FullName = FullNameCol.ToArray();
                                                clsArray.Password = PasswordCol.ToArray();
                                                clsArray.UserType = UserTypeCol.ToArray();
                                                clsArray.LogID = LogIDCol.ToArray();
                                                clsArray.SessionStatus = SessionStatusCol.ToArray();
                                                clsArray.ComputerIP = ComputerIPCol.ToArray();
                                                clsArray.ComputerName = ComputerNameCol.ToArray();
                                                clsArray.SessionStatusDescription = SessionStatusDescriptionCol.ToArray();
                                                clsArray.LogInDate = LogInDateCol.ToArray();
                                                clsArray.LogInTime = LogInTimeCol.ToArray();
                                                clsArray.LogOutDate = LogOutDateCol.ToArray();
                                                clsArray.LogOutTime = LogOutTimeCol.ToArray();
                                                clsArray.LogPublishVersion = LogPublishVersionCol.ToArray();

                                                break;
                                            case "Who Is Online":
                                                foreach (var element in Detail5.data)
                                                {
                                                    clsUser.RecordFound = true;
                                                    UserIDCol.Add(element.UserID.ToString());
                                                    UserNameCol.Add(element.UserName);
                                                    FullNameCol.Add(element.FullName);                                                    
                                                    UserTypeCol.Add(element.UserType);
                                                    LogIDCol.Add(element.LogID.ToString());                                                    
                                                    ComputerIPCol.Add(element.ComputerIP);
                                                    ComputerNameCol.Add(element.ComputerName);
                                                    LogInTimeCol.Add(element.LogInTime);
                                                }

                                                clsArray.UserID = UserIDCol.ToArray();
                                                clsArray.UserName = UserNameCol.ToArray();
                                                clsArray.FullName = FullNameCol.ToArray();                                                
                                                clsArray.UserType = UserTypeCol.ToArray();
                                                clsArray.LogID = LogIDCol.ToArray();                                                
                                                clsArray.ComputerIP = ComputerIPCol.ToArray();
                                                clsArray.ComputerName = ComputerNameCol.ToArray();                                                                                                
                                                clsArray.LogInTime = LogInTimeCol.ToArray();                                                

                                                break;
                                            default:
                                                foreach (var element in Detail5.data)
                                                {
                                                    clsUser.RecordFound = true;
                                                    UserIDCol.Add(element.UserID.ToString());
                                                    ParticularIDCol.Add(element.ParticularID.ToString());
                                                    UserNameCol.Add(element.UserName);
                                                    FullNameCol.Add(element.FullName);
                                                    PasswordCol.Add(element.Password);
                                                    UserTypeCol.Add(element.UserType);
                                                    MD5PasswordCol.Add(element.MD5Password);
                                                    MobileIDCol.Add(element.MobileID);
                                                    MobileTerminalIDCol.Add(element.MobileTerminalID);
                                                    MobileTerminalNameCol.Add(element.MobileTerminalName);
                                                }

                                                clsArray.UserID = UserIDCol.ToArray();
                                                clsArray.ParticularID = ParticularIDCol.ToArray();
                                                clsArray.UserName = UserNameCol.ToArray();
                                                clsArray.FullName = FullNameCol.ToArray();
                                                clsArray.Password = PasswordCol.ToArray();
                                                clsArray.UserType = UserTypeCol.ToArray();
                                                clsArray.MD5Password = MD5PasswordCol.ToArray();
                                                clsArray.MobileID = MobileIDCol.ToArray();
                                                clsArray.MobileTerminalID = MobileTerminalIDCol.ToArray();
                                                clsArray.MobileTerminalName = MobileTerminalNameCol.ToArray();
                                                break;
                                        }                                        
                                        break;
                                }
                                break;
                            case "Terminal Status":
                                TerminalStatusDetailOnline Detail6 = JsonConvert.DeserializeObject<TerminalStatusDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminalStatus.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail6.data)
                                        {
                                            clsTerminalStatus.RecordFound = true;
                                            clsTerminalStatus.ClassTerminalStatusID = element.TerminalStatusID;
                                            clsTerminalStatus.ClassDescription = element.Description;
                                            clsTerminalStatus.ClassTerminalStatusType = element.TerminalStatusType;
                                        }
                                        break;
                                    case "View":
                                        clsSearch.ClassTerminalStatusList = "";
                                        foreach (var element in Detail6.data)
                                        {
                                            clsTerminalStatus.RecordFound = true;
                                            IDCol.Add(element.TerminalStatusID.ToString());
                                            DescriptionCol.Add(element.Description);
                                            TerminalStatusTypeCol.Add(element.TerminalStatusType.ToString());

                                            clsSearch.ClassTerminalStatusList = clsSearch.ClassTerminalStatusList + element.Description + Environment.NewLine;
                                        }

                                        clsArray.TerminalStatusID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.TerminalStatusDescription = DescriptionCol.ToArray();
                                        clsArray.TerminalStatusType = TerminalStatusTypeCol.ToArray();
                                        break;
                                }
                                break;
                            case "Terminal Model":
                                TerminalModelDetailOnline Detail7 = JsonConvert.DeserializeObject<TerminalModelDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminalModel.ResetClass();
                                clsTerminalModel.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail7.data)
                                        {
                                            clsTerminalModel.RecordFound = true;
                                            clsTerminalModel.ClassTerminalModelID = element.TerminalModelID;
                                            clsTerminalModel.ClassDescription = element.Description;

                                        }
                                        break;
                                    case "View":
                                        clsSearch.ClassTerminalModelList = "";
                                        foreach (var element in Detail7.data)
                                        {
                                            clsTerminalModel.RecordFound = true;
                                            IDCol.Add(element.TerminalModelID.ToString());
                                            DescriptionCol.Add(element.Description);
                                            TerminalTypeIDCol.Add(element.TerminalTypeID.ToString());
                                            TerminalTypeDescriptionCol.Add(element.TerminalTypeDescription);

                                            clsSearch.ClassTerminalModelList = clsSearch.ClassTerminalModelList + element.Description + Environment.NewLine;
                                        }

                                        clsArray.TerminalModelID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.ModelDescription = DescriptionCol.ToArray();
                                        clsArray.TerminalTypeID = TerminalTypeIDCol.ToArray();
                                        clsArray.TypeDescription = TerminalTypeDescriptionCol.ToArray();
                                        break;
                                }
                                break;
                            case "FE":
                                FEDetailOnline Detail8 = JsonConvert.DeserializeObject<FEDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsFE.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail8.data)
                                        {
                                            clsFE.RecordFound = true;
                                            clsFE.ClassFEID = element.FEID;
                                            clsFE.ClassName = element.Name;
                                            clsFE.ClassAddress = element.Address;
                                            clsFE.ClassContactNo = element.ContactNo;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail8.data)
                                        {
                                            clsFE.RecordFound = true;
                                            IDCol.Add(element.FEID.ToString());
                                            NameCol.Add(element.Name);
                                            AddressCol.Add(element.Address);
                                            ContactNoCol.Add(element.ContactNo);
                                        }

                                        clsArray.FEID = IDCol.ToArray();
                                        clsArray.Name = NameCol.ToArray();
                                        clsArray.Address = AddressCol.ToArray();
                                        clsArray.ContactNo = ContactNoCol.ToArray();
                                        break;
                                }
                                break;
                            case "City":
                                CityDetailOnline Detail9 = JsonConvert.DeserializeObject<CityDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsCity.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail9.data)
                                        {
                                            clsCity.RecordFound = true;
                                            clsCity.ClassCityID = element.CityID;
                                            clsCity.ClassCity = element.City;
                                        }
                                        break;
                                    case "View":
                                        clsSearch.ClassCityList = "";
                                        foreach (var element in Detail9.data)
                                        {
                                            clsCity.RecordFound = true;
                                            IDCol.Add(element.CityID.ToString());
                                            CityCol.Add(element.City);

                                            clsSearch.ClassCityList = clsSearch.ClassCityList + element.City + Environment.NewLine;
                                        }

                                        clsArray.CityID = IDCol.ToArray();
                                        clsArray.City = CityCol.ToArray();
                                        break;
                                }
                                break;
                            case "Province":
                                ProvinceDetailOnline Detail10 = JsonConvert.DeserializeObject<ProvinceDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsProvince.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail10.data)
                                        {
                                            clsProvince.RecordFound = true;
                                            clsProvince.ClassProvinceID = element.ProvinceID;
                                            clsProvince.ClassProvince = element.Province;
                                        }
                                        break;
                                    case "View":
                                        string sProvinceList = "";
                                        clsSearch.ClassProvinceList = "";

                                        foreach (var element in Detail10.data)
                                        {
                                            clsProvince.RecordFound = true;
                                            IDCol.Add(element.ProvinceID.ToString());
                                            ProvinceCol.Add(element.Province);

                                            sProvinceList = sProvinceList + element.Province + Environment.NewLine;
                                        }

                                        clsArray.ProvinceID = IDCol.ToArray();
                                        clsArray.Province = ProvinceCol.ToArray();

                                        clsSearch.ClassProvinceList = sProvinceList;
                                        break;
                                }
                                break;
                            case "Particular":
                                ParticularDetailOnline Detail11 = JsonConvert.DeserializeObject<ParticularDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsParticular.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail11.data)
                                        {
                                            clsParticular.RecordFound = true;
                                            clsParticular.ClassParticularID = int.Parse(element.ParticularID.ToString());
                                            clsParticular.ClassProvinceID = int.Parse(element.ProvinceID.ToString());
                                            clsParticular.ClassCityID = int.Parse(element.CityID.ToString());
                                            clsParticular.ClassParticularTypeID = int.Parse(element.ParticularTypeID.ToString());
                                            clsParticular.ClassParticularName = element.ParticularName.ToString();
                                            clsParticular.ClassAddress = element.Address.ToString();
                                            clsParticular.ClassAddress2 = element.Address2.ToString();
                                            clsParticular.ClassAddress3 = element.Address3.ToString();
                                            clsParticular.ClassAddress4 = element.Address4.ToString();
                                            clsParticular.ClassContactPerson = element.ContactPerson.ToString();
                                            clsParticular.ClassTelNo = element.TelNo.ToString();
                                            clsParticular.ClassMobile = element.Mobile.ToString();
                                            clsParticular.ClassFax = element.Fax.ToString();
                                        }

                                        break;
                                    case "View":

                                        clsSearch.ClassFENameList = "";
                                        clsSearch.ClassClientNameList = "";
                                        clsSearch.ClassSPNameList = "";

                                        if (SearchBy.CompareTo("Particular List") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);
                                                AddressCol.Add(element.Address);
                                                MobileCol.Add(element.Mobile);
                                                TelNoCol.Add(element.TelNo);
                                                DepartmentCol.Add(element.Department);
                                                PositionCol.Add(element.Position);
                                                EmploymentStatusCol.Add(element.EmploymentStatus);
                                                ContactPersonCol.Add(element.ContactPerson);
                                                EmailCol.Add(element.Email);
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRNoCol.Add(element.IRNo);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                IRStatusCol.Add(element.IRStatus.ToString());
                                                IRStatusDescriptionCol.Add(element.IRStatusDescription);
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                RegionCol.Add(element.Region);
                                                ProvinceCol.Add(element.Province);

                                            }

                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.Address = AddressCol.ToArray();
                                            clsArray.MobileNo = MobileCol.ToArray();
                                            clsArray.TelNo = TelNoCol.ToArray();
                                            clsArray.DepartmentDesc = DepartmentCol.ToArray();
                                            clsArray.PositionDesc = PositionCol.ToArray();
                                            clsArray.EmploymentStatus = EmploymentStatusCol.ToArray();
                                            clsArray.ContactPerson = ContactPersonCol.ToArray();
                                            clsArray.Email = EmailCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.IRStatus = IRStatusCol.ToArray();
                                            clsArray.IRStatusDescription = IRStatusDescriptionCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                            clsArray.Province = ProvinceCol.ToArray();

                                        }
                                        else if (SearchBy.CompareTo("Merchant List") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);
                                            }

                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Client List") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);

                                                clsSearch.ClassClientNameList = clsSearch.ClassClientNameList + element.ParticularName + Environment.NewLine;
                                            }

                                            clsArray.ClientID = ParticularIDCol.ToArray();
                                            clsArray.ClientName = ParticularNameCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Field Engineer List") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);

                                                clsSearch.ClassFENameList = clsSearch.ClassFENameList + element.ParticularName + Environment.NewLine;
                                            }

                                            clsArray.FEID = ParticularIDCol.ToArray();
                                            clsArray.FEName = ParticularNameCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Service Provider List") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);

                                                clsSearch.ClassSPNameList = clsSearch.ClassSPNameList + element.ParticularName + Environment.NewLine;
                                            }

                                            clsArray.ServiceProviderID = ParticularIDCol.ToArray();
                                            clsArray.ServiceProviderName = ParticularNameCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Particular List 2") == 0)
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                Detail_InfoCol.Add(element.detail_info);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.detail_info = Detail_InfoCol.ToArray();
                                        }
                                        else
                                        {
                                            foreach (var element in Detail11.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                ProvinceIDCol.Add(element.ProvinceID.ToString());
                                                CityIDCol.Add(element.CityID.ToString());
                                                ParticularTypeIDCol.Add(element.ParticularTypeID.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                RegionTypeCol.Add(element.RegionType.ToString());
                                                ParticularDescriptionCol.Add(element.ParticularDescription);
                                                ParticularNameCol.Add(element.ParticularName);
                                                AddressCol.Add(element.Address);
                                                Address2Col.Add(element.Address2);
                                                Address3Col.Add(element.Address3);
                                                Address4Col.Add(element.Address4);
                                                ContactPersonCol.Add(element.ContactPerson);
                                                TelNoCol.Add(element.TelNo);
                                                MobileCol.Add(element.Mobile);
                                                FaxCol.Add(element.Fax);
                                                EmailCol.Add(element.Email);
                                                ContractTermsCol.Add(element.ContractTerms);
                                                ProvinceCol.Add(element.Province);
                                                RegionCol.Add(element.Region);
                                                CityCol.Add(element.City);
                                            }

                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.ProvinceID = ProvinceIDCol.ToArray();
                                            clsArray.CityID = CityIDCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.RegionType = RegionTypeCol.ToArray();
                                            clsArray.City = CityCol.ToArray();
                                            clsArray.Province = ProvinceCol.ToArray();
                                            clsArray.ParticularTypeID = ParticularTypeIDCol.ToArray();
                                            clsArray.ParticularDescription = ParticularDescriptionCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.Address = AddressCol.ToArray();
                                            clsArray.Address2 = Address2Col.ToArray();
                                            clsArray.Address3 = Address3Col.ToArray();
                                            clsArray.Address4 = Address4Col.ToArray();
                                            clsArray.ContactPerson = ContactPersonCol.ToArray();
                                            clsArray.TelNo = TelNoCol.ToArray();
                                            clsArray.MobileNo = MobileCol.ToArray();
                                            clsArray.Fax = FaxCol.ToArray();
                                            clsArray.Email = EmailCol.ToArray();
                                            clsArray.ContractTerms = ContractTermsCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                        }

                                        break;
                                }
                                break;
                            case "Mapping":
                                MappingDetailOnline Detail12 = JsonConvert.DeserializeObject<MappingDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsMapping.RecordFound = false;
                                clsMapping.ClassRecordCount = 0;

                                // Clear
                                IDCol.Clear();
                                MapFromCol.Clear();
                                MapToCol.Clear();
                                DelimeterCol.Clear();
                                ColumnIndexCol.Clear();
                                isMustCol.Clear();
                                FormatCol.Clear();
                              
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail12.data)
                                        {
                                            clsMapping.RecordFound = true;
                                            clsMapping.ClassMapID = element.MapID;
                                            clsMapping.ClassMapFrom = element.MapFrom;
                                            clsMapping.ClassMapTo = element.MapTo;
                                            clsMapping.ClassDelimeter = element.Delimeter;
                                            clsMapping.ClassColumnIndex = element.ColumnIndex;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail12.data)
                                        {
                                            clsMapping.RecordFound = true;
                                            IDCol.Add(element.MapID.ToString());
                                            MapFromCol.Add(element.MapFrom);
                                            MapToCol.Add(element.MapTo);
                                            DelimeterCol.Add(element.Delimeter);
                                            ColumnIndexCol.Add(element.ColumnIndex);
                                            isMustCol.Add(element.isMust.ToString());
                                            FormatCol.Add(element.Format);


                                        }

                                        // Loop And Store To Array
                                        clsArray.MapID = IDCol.ToArray();
                                        clsArray.MapFrom = MapFromCol.ToArray();
                                        clsArray.MapTo = MapToCol.ToArray();
                                        clsArray.MapDelimeter = DelimeterCol.ToArray();
                                        clsArray.MapColumnIndex = ColumnIndexCol.ToArray();
                                        clsArray.isMust = isMustCol.ToArray();
                                        clsArray.Format = FormatCol.ToArray();

                                        clsMapping.ClassRecordCount = clsArray.MapID.Length;

                                        break;
                                }
                                break;
                            case "Last Insert ID":
                                LastIDDetailOnline Detail13 = JsonConvert.DeserializeObject<LastIDDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsLastID.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail13.data)
                                        {
                                            clsLastID.RecordFound = true;
                                            clsLastID.ClassLastInsertedID = element.LastInsertID;
                                        }
                                        break;
                                    case "View":
                                        List<string> LastInsertedIDCol = new List<String>();

                                        foreach (var element in Detail13.data)
                                        {
                                            clsLastID.RecordFound = true;
                                            LastInsertedIDCol.Add(element.LastInsertID.ToString());
                                        }

                                        clsArray.LastInsertedID = LastInsertedIDCol.ToArray();

                                        break;
                                }
                                break;
                            case "FSR":
                                FSRDetailOnline Detail14 = JsonConvert.DeserializeObject<FSRDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsFSR.RecordFound = false;

                                FSRNoCol.Clear();
                                FSRIDCol.Clear();
                                MerchantCol.Clear();
                                MIDCol.Clear();
                                TIDCol.Clear();
                                TimeArrivedCol.Clear();
                                TimeStartCol.Clear();
                                FSRCol.Clear();
                                FSRDateCol.Clear();
                                FSRTimeCol.Clear();
                                MerchantContactNoCol.Clear();
                                MerchantRepresentativeCol.Clear();
                                DateFromCol.Clear();
                                DateToCol.Clear();
                                TxnAmtCol.Clear();
                                ServiceNoCol.Clear();
                                RequestNoCol.Clear();

                                switch (StatementType)
                                {
                                    case "Search":
                                    case "View":

                                        if (SearchBy.CompareTo("Download Completed FSR") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                FSRIDCol.Add(element.FSRID.ToString());
                                                MerchantCol.Add(element.Merchant);
                                                MIDCol.Add(element.MID);
                                                TIDCol.Add(element.TID);
                                                TimeArrivedCol.Add(element.TimeArrived);
                                                TimeStartCol.Add(element.TimeStart);
                                                FSRCol.Add(element.FSR);
                                                FSRDateCol.Add(element.FSRDate);
                                                FSRTimeCol.Add(element.FSRTime);
                                                TxnAmtCol.Add(element.TxnAmt);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                            }
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.FSRID = FSRIDCol.ToArray();
                                            clsArray.Merchant = MerchantCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.TimeArrived = TimeArrivedCol.ToArray();
                                            clsArray.TimeStart = TimeStartCol.ToArray();
                                            clsArray.FSR = FSRCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.FSRTime = FSRTimeCol.ToArray();
                                            clsArray.TxnAmt = TxnAmtCol.ToArray();
                                            clsArray.TimeEnd = TimeEndCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("FSR Temp Detail") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                NoCol.Add(element.No.ToString());
                                                MerchantCol.Add(element.Merchant);
                                                MIDCol.Add(element.MID.Trim());
                                                TIDCol.Add(element.TID);
                                                InvoiceNoCol.Add(element.InvoiceNo);
                                                BatchNoCol.Add(element.BatchNo);
                                                FSRCol.Add(element.FSR);
                                                FSRDateCol.Add(element.FSRDate);
                                                FSRTimeCol.Add(element.FSRTime);
                                                TxnAmtCol.Add(element.TxnAmt);
                                                TimeEndCol.Add(element.TimeEnd);
                                                TerminalSNCol.Add(element.TerminalSN);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                                FENameCol.Add(element.FEName);
                                                SerialNoCol.Add(element.SerialNo);
                                            }
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.No = NoCol.ToArray();
                                            clsArray.MerchantName = MerchantCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.InvoiceNo = InvoiceNoCol.ToArray();
                                            clsArray.BatchNo = BatchNoCol.ToArray();
                                            clsArray.FSR = FSRCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.FSRTime = FSRTimeCol.ToArray();
                                            clsArray.TxnAmt = TxnAmtCol.ToArray();
                                            clsArray.TimeEnd = TimeEndCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("FSR Temp Detail Date Filter") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                DateFromCol.Add(element.DateFrom);
                                                DateToCol.Add(element.DateTo);
                                            }

                                            clsArray.DateFrom = DateFromCol.ToArray();
                                            clsArray.DateTo = DateToCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("FSR FillUp") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                ProblemReportedCol.Add(element.ProblemReported);
                                                ActualProblemReportedCol.Add(element.ActualProblemReported);
                                                ActionTakenCol.Add(element.ActionTaken);
                                                AnyCommentsCol.Add(element.AnyComments);
                                                ReasonIDCol.Add(element.ReasonID.ToString());
                                                ReasonCodeCol.Add(element.ReasonCode);
                                                ReasonDescriptionCol.Add(element.ReasonDescription);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);

                                            }
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.ProblemReported = ProblemReportedCol.ToArray();
                                            clsArray.ActualProblemReported = ActualProblemReportedCol.ToArray();
                                            clsArray.ActionTaken = ActionTakenCol.ToArray();
                                            clsArray.AnyComments = AnyCommentsCol.ToArray();
                                            clsArray.ReasonID = ReasonIDCol.ToArray();
                                            clsArray.ReasonCode = ReasonCodeCol.ToArray();
                                            clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("FSR") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                FSRDateCol.Add(element.FSRDate.ToString());
                                                IRNoCol.Add(element.IRNo);
                                                MerchantCol.Add(element.Merchant);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                TerminalSNCol.Add(element.TerminalSN);
                                                SIMSerialNoCol.Add(element.SIMSN);
                                                PrimaryNumCol.Add(element.PrimaryNum);
                                                SecondaryNumCol.Add(element.SecondaryNum);
                                                AppVersionCol.Add(element.AppVersion);
                                                AppCRCCol.Add(element.AppCRC);
                                                RequestNoCol.Add(element.RequestNo);
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                FENameCol.Add(element.FEName);
                                                ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                ServiceDateCol.Add(element.ServiceDate);
                                                ServiceTimeCol.Add(element.ServiceTime);
                                                ServiceReqDateCol.Add(element.ServiceReqDate);
                                                ServiceReqTimeCol.Add(element.ServiceReqTime);


                                            }
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.Merchant = MerchantCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.PrimaryNum = PrimaryNumCol.ToArray();
                                            clsArray.SecondaryNum = SecondaryNumCol.ToArray();
                                            clsArray.AppVersion = AppVersionCol.ToArray();
                                            clsArray.AppCRC = AppCRCCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                            clsArray.ServiceDate = ServiceDateCol.ToArray();
                                            clsArray.ServiceTime = ServiceTimeCol.ToArray();
                                            clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                            clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("FSRNo") == 0)
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                FSRIDCol.Add(element.FSRID.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                IRNoCol.Add(element.IRNo);
                                                MerchantCol.Add(element.Merchant);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                TerminalSNCol.Add(element.TerminalSN);
                                                SIMSerialNoCol.Add(element.SIMSN);
                                                PowerSNCol.Add(element.PowerSN);
                                                DockSNCol.Add(element.DockSN);
                                                TimeArrivedCol.Add(element.TimeArrived);
                                                TimeStartCol.Add(element.TimeStart);
                                                FSRCol.Add(element.FSR);
                                                FSRDateCol.Add(element.FSRDate);
                                                FSRTimeCol.Add(element.FSRTime);
                                                TimeEndCol.Add(element.TimeEnd);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                                FENameCol.Add(element.FEName);
                                                SerialNoCol.Add(element.SerialNo);
                                                ActionMadeCol.Add(element.ActionMade);
                                                ProblemReportedCol.Add(element.ProblemReported);
                                                ActualProblemReportedCol.Add(element.ActualProblemReported);
                                                ActionTakenCol.Add(element.ActionTaken);
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription);

                                            }

                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.FSRID = FSRIDCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.Merchant = MerchantCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.PowerSN = PowerSNCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.TimeArrived = TimeArrivedCol.ToArray();
                                            clsArray.TimeStart = TimeStartCol.ToArray();
                                            clsArray.FSR = FSRCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.FSRTime = FSRTimeCol.ToArray();
                                            clsArray.TimeEnd = TimeEndCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.ActionMade = ActionMadeCol.ToArray();
                                            clsArray.ProblemReported = ProblemReportedCol.ToArray();
                                            clsArray.ActualProblemReported = ActualProblemReportedCol.ToArray();
                                            clsArray.ActionTaken = ActionTakenCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();                                            

                                        }
                                        else
                                        {
                                            foreach (var element in Detail14.data)
                                            {
                                                clsFSR.RecordFound = true;
                                                FSRIDCol.Add(element.FSRID.ToString());
                                                NoCol.Add(element.No);
                                                MerchantCol.Add(element.Merchant);
                                                MIDCol.Add(element.MID);
                                                TIDCol.Add(element.TID);
                                                InvoiceNoCol.Add(element.InvoiceNo);
                                                BatchNoCol.Add(element.BatchNo);
                                                TimeArrivedCol.Add(element.TimeArrived);
                                                TimeStartCol.Add(element.TimeStart);
                                                FSRCol.Add(element.FSR);
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription);
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                FSRDateCol.Add(element.FSRDate);
                                                FSRTimeCol.Add(element.FSRTime);
                                                TxnAmtCol.Add(element.TxnAmt);
                                                TimeEndCol.Add(element.TimeEnd);
                                                TerminalSNCol.Add(element.TerminalSN);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                                FENameCol.Add(element.FEName);
                                                SerialNoCol.Add(element.SerialNo);
                                                FSRStatusDescriptionCol.Add(element.FSRStatusDescription);
                                                ProcessTypeCol.Add(element.ProcessType);
                                                IRNoCol.Add(element.IRNo);
                                                ClientNameCol.Add(element.ClientName);
                                                SIMSerialNoCol.Add(element.SIMSN);
                                                PowerSNCol.Add(element.PowerSN);
                                                DockSNCol.Add(element.DockSN);
                                                TypeDescriptionCol.Add(element.TypeDescription);
                                                ModelDescriptionCol.Add(element.ModelDescription);
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                RequestNoCol.Add(element.RequestNo);
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                ServiceJobTypeDescriptionCol.Add(element.ServiceJobTypeDescription);

                                            }

                                            // Loop And Store To Array
                                            clsArray.FSRID = FSRIDCol.ToArray();
                                            clsArray.No = NoCol.ToArray();
                                            clsArray.Merchant = MerchantCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.InvoiceNo = InvoiceNoCol.ToArray();
                                            clsArray.BatchNo = BatchNoCol.ToArray();
                                            clsArray.TimeArrived = TimeArrivedCol.ToArray();
                                            clsArray.TimeStart = TimeStartCol.ToArray();
                                            clsArray.FSR = FSRCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.FSRTime = FSRTimeCol.ToArray();
                                            clsArray.TxnAmt = TxnAmtCol.ToArray();
                                            clsArray.TimeEnd = TimeEndCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.FSRStatusDescription = FSRStatusDescriptionCol.ToArray();
                                            clsArray.ProcessType = ProcessTypeCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.PowerSN = PowerSNCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.TypeDescription = TypeDescriptionCol.ToArray();
                                            clsArray.ModelDescription = ModelDescriptionCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.ServiceJobTypeDescription = ServiceJobTypeDescriptionCol.ToArray();
                                        }                                        
                                        break;
                                }
                                break;                            
                            case "Terminal Brand":
                                TerminalBrandDetailOnline Detail15 = JsonConvert.DeserializeObject<TerminalBrandDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminalBrand.ResetClass();
                                clsTerminalBrand.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail15.data)
                                        {
                                            clsTerminalBrand.RecordFound = true;
                                            clsTerminalBrand.ClassTerminalBrandID = element.TerminalBrandID;
                                            clsTerminalBrand.ClassDescription = element.Description;
                                        }
                                        break;
                                    case "View":
                                        clsSearch.ClassTerminalBrandList = "";
                                        foreach (var element in Detail15.data)
                                        {
                                            clsTerminalBrand.RecordFound = true;
                                            IDCol.Add(element.TerminalBrandID.ToString());
                                            DescriptionCol.Add(element.Description);

                                            clsSearch.ClassTerminalBrandList = clsSearch.ClassTerminalBrandList + element.Description + Environment.NewLine;
                                        }

                                        clsArray.TerminalBrandID = IDCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.BrandDescription = DescriptionCol.ToArray();
                                        break;
                                }
                                break;
                            case "Terminal":
                            case "Stock Detail":
                                TerminalDetailOnline Detail16 = JsonConvert.DeserializeObject<TerminalDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminal.RecordFound = false;

                                TerminalIDCol.Clear();
                                SerialNoCol.Clear();
                                TypeCol.Clear();
                                ModelCol.Clear();
                                BrandCol.Clear();
                                TerminalPartNoCol.Clear();
                                TerminalStatusCol.Clear();
                                TerminalStatusDescriptionCol.Clear();
                                SIMIDCol.Clear();
                                SerialNoCol.Clear();
                                SIMCarrierCol.Clear();
                                SIMStatusCol.Clear();
                                SIMStatusDescriptionCol.Clear();

                                if (StatementType.CompareTo("Search") == 0 ||
                                    StatementType.CompareTo("View") == 0)

                                {
                                    if (SearchBy.CompareTo("TerminalSN List") == 0 ||
                                        SearchBy.CompareTo("Stock Detail List") == 0)
                                    {
                                        foreach (var element in Detail16.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            TerminalIDCol.Add(element.TerminalID.ToString());
                                            SerialNoCol.Add(element.SerialNo);
                                            TypeCol.Add(element.TerminalType);
                                            ModelCol.Add(element.TerminalModel);
                                            BrandCol.Add(element.TerminalBrand);
                                            TerminalPartNoCol.Add(element.PartNo);
                                            TerminalStatusCol.Add(element.TerminalStatus.ToString());
                                            TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription);
                                            MerchantNameCol.Add(element.MerchantName);
                                            TIDCol.Add(element.TID);
                                            MIDCol.Add(element.MID);
                                            IRNoCol.Add(element.IRNo);
                                            ClientNameCol.Add(element.ClientName);
                                            LocationCol.Add(element.Location);
                                            AllocationCol.Add(element.Allocation);
                                            AssetTypeCol.Add(element.AssetType);
                                            DeliveryDateCol.Add(element.DeliveryDate);
                                            ReceiveDateCol.Add(element.ReceiveDate);
                                            ReleaseDateCol.Add(element.ReleaseDate);
                                        }

                                        clsArray.TerminalID = TerminalIDCol.ToArray();
                                        clsArray.SerialNo = SerialNoCol.ToArray();
                                        clsArray.TerminalSN = SerialNoCol.ToArray();
                                        clsArray.TerminalType = TypeCol.ToArray();
                                        clsArray.TerminalModel = ModelCol.ToArray();
                                        clsArray.TerminalBrand = BrandCol.ToArray();
                                        clsArray.TerminalPartNo = TerminalPartNoCol.ToArray();
                                        clsArray.TerminalStatus = TerminalStatusCol.ToArray();
                                        clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                        clsArray.MerchantName = MerchantNameCol.ToArray();
                                        clsArray.TID = TIDCol.ToArray();
                                        clsArray.MID = MIDCol.ToArray();
                                        clsArray.IRNo = IRNoCol.ToArray();
                                        clsArray.ClientName = ClientNameCol.ToArray();
                                        clsArray.Location = LocationCol.ToArray();
                                        clsArray.Allocation = AllocationCol.ToArray();
                                        clsArray.AssetType = AssetTypeCol.ToArray();
                                        clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                        clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                        clsArray.ReleaseDate = ReleaseDateCol.ToArray();
                                    }
                                    else if (SearchBy.CompareTo("SIMSN List") == 0)
                                    {
                                        foreach (var element in Detail16.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            SIMIDCol.Add(element.SIMID.ToString());
                                            SerialNoCol.Add(element.SerialNo);
                                            SIMCarrierCol.Add(element.Carrier);
                                            SIMStatusCol.Add(element.SIMStatus.ToString());
                                            SIMStatusDescriptionCol.Add(element.SIMStatusDescription);                                            
                                            MerchantNameCol.Add(element.MerchantName);
                                            TIDCol.Add(element.TID);
                                            MIDCol.Add(element.MID);
                                            IRNoCol.Add(element.IRNo);
                                            ClientNameCol.Add(element.ClientName);
                                            LocationCol.Add(element.Location);
                                            AllocationCol.Add(element.Allocation);
                                            DeliveryDateCol.Add(element.DeliveryDate);
                                            ReceiveDateCol.Add(element.ReceiveDate);
                                            ReleaseDateCol.Add(element.ReleaseDate);
                                        }

                                        clsArray.SIMID = SIMIDCol.ToArray();
                                        clsArray.SerialNo = SerialNoCol.ToArray();
                                        clsArray.TerminalSN = SerialNoCol.ToArray();
                                        clsArray.SIMSerialNo = SerialNoCol.ToArray();
                                        clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                        clsArray.SIMStatus = SIMStatusCol.ToArray();
                                        clsArray.SIMStatusDescription = SIMStatusDescriptionCol.ToArray();
                                        clsArray.MerchantName = MerchantNameCol.ToArray();
                                        clsArray.TID = TIDCol.ToArray();
                                        clsArray.MID = MIDCol.ToArray();
                                        clsArray.IRNo = IRNoCol.ToArray();
                                        clsArray.ClientName = ClientNameCol.ToArray();
                                        clsArray.Location = LocationCol.ToArray();
                                        clsArray.Allocation = AllocationCol.ToArray();
                                        clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                        clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                        clsArray.ReleaseDate = ReleaseDateCol.ToArray();
                                    }
                                    else if (SearchBy.CompareTo("Terminal Status Description") == 0)
                                    {
                                        foreach (var element in Detail16.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            TerminalIDCol.Add(element.TerminalID.ToString());
                                            TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription);
                                        }

                                        clsArray.TerminalID = TerminalIDCol.ToArray();
                                        clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                    }
                                    else if (SearchBy.CompareTo("SIM Status Description") == 0)
                                    {
                                        foreach (var element in Detail16.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            SIMIDCol.Add(element.SIMID.ToString());
                                            SIMStatusDescriptionCol.Add(element.SIMStatusDescription);
                                        }

                                        clsArray.SIMID = SIMIDCol.ToArray();
                                        clsArray.SIMStatusDescription = SIMStatusDescriptionCol.ToArray();
                                    }
                                    else
                                    {
                                        foreach (var element in Detail16.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            TerminalIDCol.Add(element.TerminalID.ToString());
                                            TIIDCol.Add(element.TIID.ToString());
                                            TerminalTypeIDCol.Add(element.TerminalTypeID.ToString());
                                            TerminalModelIDCol.Add(element.TerminalModelID.ToString());
                                            TerminalBrandIDCol.Add(element.TerminalBrandID.ToString());
                                            NoCol.Add(element.No);
                                            SerialNoCol.Add(element.SerialNo);
                                            TypeCol.Add(element.TerminalType);
                                            ModelCol.Add(element.TerminalModel);
                                            BrandCol.Add(element.TerminalBrand);
                                            DeliveryDateCol.Add(element.DeliveryDate);
                                            ReceiveDateCol.Add(element.ReceiveDate);
                                            TerminalStatusCol.Add(element.TerminalStatus);
                                            TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription);
                                            TerminalPartNoCol.Add(element.PartNo);
                                            TerminalPONoCol.Add(element.PONo);
                                            TerminalInvNoCol.Add(element.InvNo);
                                            RemarksCol.Add(element.Remarks);

                                        }

                                        clsArray.TerminalID = TerminalIDCol.ToArray();
                                        clsArray.TIID = TIIDCol.ToArray();
                                        clsArray.TerminalTypeID = TerminalTypeIDCol.ToArray();
                                        clsArray.TerminalModelID = TerminalModelIDCol.ToArray();
                                        clsArray.TerminalBrandID = TerminalBrandIDCol.ToArray();
                                        clsArray.No = NoCol.ToArray();
                                        clsArray.SerialNo = SerialNoCol.ToArray();
                                        clsArray.TerminalSN = SerialNoCol.ToArray();
                                        clsArray.TerminalType = TypeCol.ToArray();
                                        clsArray.TerminalModel = ModelCol.ToArray();
                                        clsArray.TerminalBrand = BrandCol.ToArray();
                                        clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                        clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                        clsArray.TerminalStatus = TerminalStatusCol.ToArray();
                                        clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                        clsArray.TerminalPartNo = TerminalPartNoCol.ToArray();
                                        clsArray.TerminalPONo = TerminalPONoCol.ToArray();
                                        clsArray.TerminalInvNo = TerminalInvNoCol.ToArray();
                                        clsArray.Remarks = RemarksCol.ToArray();
                                    }
                                }
                                break;
                            case "Particular Count":
                                ParticularDetailOnline Detail17 = JsonConvert.DeserializeObject<ParticularDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsParticular.RecordFound = false;

                                foreach (var element in Detail17.data)
                                {
                                    clsParticular.RecordFound = true;
                                    clsParticular.ClassParticularID = element.ParticularID;
                                    clsParticular.ClassParticularName = element.ParticularName;
                                }

                                break;
                            case "CheckRecordExist":
                            case "CheckFileExist":                            
                                CheckRecordExistDetailOnline Detail18 = JsonConvert.DeserializeObject<CheckRecordExistDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsCheckRecordExist.RecordFound = false;

                                foreach (var element in Detail18.data)
                                {
                                    clsCheckRecordExist.RecordFound = true;
                                }

                                break;
                            case "Merchant":
                                MerchantDetailOnline Detail19 = JsonConvert.DeserializeObject<MerchantDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsMerchant.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                    case "View":
                                        foreach (var element in Detail19.data)
                                        {
                                            clsMerchant.RecordFound = true;
                                            MerchantIDCol.Add(element.MerchantID.ToString());
                                            CityIDCol.Add(element.CityID.ToString());
                                            ProvinceIDCol.Add(element.ProvinceID.ToString());
                                            MerchantNameCol.Add(element.MerchantName.ToString());
                                            AddressCol.Add(element.Address.ToString());
                                            CityCol.Add(element.City.ToString());
                                            ProvinceCol.Add(element.Province.ToString());
                                            ContactPersonCol.Add(element.ContactPerson.ToString());
                                            ContactNoCol.Add(element.ContactNo.ToString());

                                        }

                                        // Loop And Store To Array
                                        clsArray.MerchantID = MerchantIDCol.ToArray();
                                        clsArray.CityID = CityIDCol.ToArray();
                                        clsArray.ProvinceID = ProvinceIDCol.ToArray();
                                        clsArray.MerchantName = MerchantNameCol.ToArray();
                                        clsArray.Address = AddressCol.ToArray();
                                        clsArray.City = CityCol.ToArray();
                                        clsArray.Province = ProvinceCol.ToArray();
                                        clsArray.ContactPerson = ContactPersonCol.ToArray();
                                        clsArray.ContactNo = ContactNoCol.ToArray();

                                        break;
                                }
                                break;
                            case "IR":
                                IRDetailOnline Detail20 = JsonConvert.DeserializeObject<IRDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsIR.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                    case "View":

                                        if (SearchBy.CompareTo("IRNo List") == 0 || SearchBy.CompareTo("IRNo List2") == 0)
                                        {
                                            foreach (var element in Detail20.data)
                                            {
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                MerchantNameCol.Add(element.MerchantName.ToString());
                                            }

                                            // Loop And Store To Array
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Dispatch Servicing 2") == 0)
                                        {

                                            foreach (var element in Detail20.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                MerchantNameCol.Add(element.MerchantName);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                ClientNameCol.Add(element.ClientName);
                                                FENameCol.Add(element.FEName);
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRNoCol.Add(element.IRNo);
                                                IRDateCol.Add(element.IRDate);
                                                InstallationDateCol.Add(element.InstallationDate);
                                                RequestNoCol.Add(element.RequestNo.ToString());
                                                ServiceDateCol.Add(element.ServiceDate);
                                                ServiceReqDateCol.Add(element.ServiceReqDate);                                             
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                ActionMadeCol.Add(element.ActionMade);
                                                ReferenceNoCol.Add(element.ReferenceNo);
                                                FSRDateCol.Add(element.FSRDate);
                                                ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                TerminalSNCol.Add(element.TerminalSN);
                                                SIMSerialNoCol.Add(element.SIMSerialNo);
                                                ReplaceTerminalSNCol.Add(element.ReplaceTerminalSN);
                                                ReplaceSIMSNCol.Add(element.ReplaceSIMSN);
                                                
                                            }

                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.ActionMade = ActionMadeCol.ToArray();
                                            clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                            clsArray.ServiceDate = ServiceDateCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.ReplaceTerminalSN = ReplaceTerminalSNCol.ToArray();
                                            clsArray.ReplaceSIMSN = ReplaceSIMSNCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Multi-Merchant Info") == 0)
                                        {
                                            foreach (var element in Detail20.data)
                                            {
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                IRNoCol.Add(element.IRNo);
                                                IRStatusCol.Add(element.IRStatus.ToString());
                                                IRStatusDescriptionCol.Add(element.IRStatusDescription);

                                            }

                                            // Loop And Store To Array
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.IRStatus = IRStatusCol.ToArray();
                                            clsArray.IRStatusDescription = IRStatusDescriptionCol.ToArray();

                                        }
                                        else if (SearchBy.CompareTo("POS Rental IR List") == 0)
                                        {
                                            foreach (var element in Detail20.data)
                                            {
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                DescriptionCol.Add(element.Description);
                                                RentalFeeCol.Add(element.RentalFee.ToString());


                                            }

                                            // Loop And Store To Array
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.Description = DescriptionCol.ToArray();
                                            clsArray.RentalFee = RentalFeeCol.ToArray();

                                        }
                                        else
                                        {
                                            foreach (var element in Detail20.data)
                                            {
                                                clsIR.RecordFound = true;
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRIDCol.Add(element.IRID.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                IRDateCol.Add(element.IRDate.ToString());
                                                InstallationDateCol.Add(element.InstallationDate.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                ParticularNameCol.Add(element.ParticularName.ToString());
                                                AddressCol.Add(element.Address.ToString());
                                                Address2Col.Add(element.Address2.ToString());
                                                Address3Col.Add(element.Address3.ToString());
                                                Address4Col.Add(element.Address4.ToString());
                                                ContactPersonCol.Add(element.ContactPerson.ToString());
                                                TelNoCol.Add(element.TelNo.ToString());
                                                MobileCol.Add(element.Mobile.ToString());
                                                EmailCol.Add(element.Email.ToString());
                                                CityCol.Add(element.City.ToString());
                                                ProvinceCol.Add(element.Province.ToString());
                                                TIDCol.Add(element.TID.ToString());
                                                MIDCol.Add(element.MID.ToString());
                                                IRStatusCol.Add(element.IRStatus.ToString());
                                                IRStatusDescriptionCol.Add(element.IRStatusDescription.ToString());
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription.ToString());
                                                IRImportDateCol.Add(element.ImportDateTime.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                TerminalSNCol.Add(element.TerminalSN.ToString());
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo.ToString());
                                                DockIDCol.Add(element.DockID.ToString());
                                                DockSNCol.Add(element.DockSN.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                ClientNameCol.Add(element.ClientName.ToString());
                                                ServiceProviderIDCol.Add(element.ServiceProviderID.ToString());
                                                ServiceProviderNameCol.Add(element.ServiceProviderName.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                FENameCol.Add(element.FEName.ToString());
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime.ToString());
                                                ProcessedByCol.Add(element.ProcessedBy.ToString());
                                                ProcessTypeCol.Add(element.ProcessType.ToString());
                                                ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                RequestNoCol.Add(element.RequestNo.ToString());
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription.ToString());
                                                JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription.ToString());
                                                ServiceDateTimeCol.Add(element.ServiceDateTime.ToString());
                                                ReasonIDCol.Add(element.ReasonID.ToString());
                                                ReasonCodeCol.Add(element.ReasonCode);
                                                ReasonDescriptionCol.Add(element.ReasonDescription);
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                TimeArrivedCol.Add(element.TimeArrived);
                                                TimeStartCol.Add(element.TimeStart);
                                                FSRDateCol.Add(element.FSRDate);
                                                FSRTimeCol.Add(element.FSRTime);
                                                TimeEndCol.Add(element.TimeEnd);
                                                MerchantContactNoCol.Add(element.MerchantContactNo);
                                                MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                                SerialNoListCol.Add(element.SerialNoList);
                                                PrimaryNumCol.Add(element.PrimaryNum);
                                                SecondaryNumCol.Add(element.SecondaryNum);
                                                AppVersionCol.Add(element.AppVersion);
                                                AppCRCCol.Add(element.AppCRC);

                                            }

                                            // Loop And Store To Array
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRID = IRIDCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.Address = AddressCol.ToArray();
                                            clsArray.Address2 = Address2Col.ToArray();
                                            clsArray.Address3 = Address3Col.ToArray();
                                            clsArray.Address4 = Address4Col.ToArray();
                                            clsArray.ContactPerson = ContactPersonCol.ToArray();
                                            clsArray.TelNo = TelNoCol.ToArray();
                                            clsArray.MobileNo = MobileCol.ToArray();
                                            clsArray.Email = EmailCol.ToArray();
                                            clsArray.City = CityCol.ToArray();
                                            clsArray.Province = ProvinceCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.IRStatus = IRStatusCol.ToArray();
                                            clsArray.IRStatusDescription = IRStatusDescriptionCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();
                                            clsArray.IRImportDateTime = IRImportDateCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.DockID = DockIDCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
                                            clsArray.ServiceProviderName = ServiceProviderNameCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessType = ProcessTypeCol.ToArray();
                                            clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                            clsArray.ReasonID = ReasonIDCol.ToArray();
                                            clsArray.ReasonCode = ReasonCodeCol.ToArray();
                                            clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();
                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.TimeArrived = TimeArrivedCol.ToArray();
                                            clsArray.TimeStart = TimeStartCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.FSRTime = FSRTimeCol.ToArray();
                                            clsArray.TimeEnd = TimeEndCol.ToArray();
                                            clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                            clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                            clsArray.SerialNoList = SerialNoCol.ToArray();
                                            clsArray.PrimaryNum = PrimaryNumCol.ToArray();
                                            clsArray.SecondaryNum = SecondaryNumCol.ToArray();
                                            clsArray.AppVersion = AppVersionCol.ToArray();
                                            clsArray.AppCRC = AppCRCCol.ToArray();
                                        }

                                        break;
                                }
                                break;
                            case "TA":
                                TADetailOnline Detail21 = JsonConvert.DeserializeObject<TADetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTA.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail21.data)
                                        {
                                            clsTA.RecordFound = true;
                                            clsTA.ClassIRNo = element.IRNo;
                                            clsTA.ClassIRDate = element.IRDate;
                                            clsTA.ClassMerchantName = element.MerchantName;
                                            clsTA.ClassClientName = element.ClientName;
                                            clsTA.ClassServiceProviderName = element.ServiceProviderName;
                                            clsTA.ClassFEName = element.FEName;
                                            clsTA.ClassTADateTime = element.TADateTime;
                                            clsTA.ClassServiceTypeDescription = element.ServiceTypeDescription;
                                        }
                                        break;
                                    case "View":

                                        if (SearchBy.CompareTo("Advance SIM") == 0 ||
                                            SearchBy.CompareTo("SIM") == 0)
                                        {
                                            foreach (var element in Detail21.data)
                                            {
                                                clsTA.RecordFound = true;
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                BatchNoCol.Add(element.BatchNo.ToString());
                                                SerialNoCol.Add(element.SerialNo.ToString());
                                                TerminalSNCol.Add(element.TerminalSN.ToString());
                                                SIMCarrierCol.Add(element.Carrier.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                AssignedToCol.Add(element.AssignedTo.ToString());
                                                RemarksCol.Add(element.Remarks.ToString());
                                                DeliveryDateCol.Add(element.DeliveryDate.ToString());
                                                ReceiveDateCol.Add(element.ReceiveDate.ToString());
                                                SIMStatusCol.Add(element.SIMStatus.ToString());
                                                SIMStatusDescriptionCol.Add(element.SIMStatusDescription.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                IRDateCol.Add(element.IRDate.ToString());
                                                InstallationDateCol.Add(element.InstallationDate.ToString());
                                                ClientNameCol.Add(element.ClientName.ToString());
                                                MerchantNameCol.Add(element.MerchantName.ToString());
                                                ServiceProviderNameCol.Add(element.ServiceProviderName.ToString());
                                                FENameCol.Add(element.FEName.ToString());
                                                TADateTimeCol.Add(element.TADateTime.ToString());
                                            }

                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.BatchNo = BatchNoCol.ToArray();
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.AssignedTo = AssignedToCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                            clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                            clsArray.SIMStatus = SIMStatusCol.ToArray();
                                            clsArray.SIMStatusDescription = SIMStatusDescriptionCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.ServiceProviderName = ServiceProviderNameCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.TADateTime = TADateTimeCol.ToArray();

                                        }
                                        else if (SearchBy.CompareTo("Advance Terminal") == 0)
                                        {
                                            foreach (var element in Detail21.data)
                                            {
                                                clsTA.RecordFound = true;
                                                SerialNoCol.Add(element.SerialNo.ToString());
                                                DeliveryDateCol.Add(element.DeliveryDate.ToString());
                                                ReceiveDateCol.Add(element.ReceiveDate.ToString());
                                                TerminalStatusCol.Add(element.TerminalStatus.ToString());
                                                TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                TAIDCol.Add(element.TAID.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                ServiceProviderIDCol.Add(element.ServiceProviderID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                TerminalTypeIDCol.Add(element.TerminalTypeID.ToString());
                                                TerminalModelIDCol.Add(element.TerminalModelID.ToString());
                                                TerminalBrandIDCol.Add(element.TerminalBrandID.ToString());
                                                ServiceTypeIDCol.Add(element.ServiceTypeID.ToString());
                                                OtherServiceTypeIDCol.Add(element.OtherServiceTypeID.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                TerminalSNCol.Add(element.TerminalSN.ToString());
                                                MerchantNameCol.Add(element.MerchantName.ToString());
                                                TIDCol.Add(element.TID.ToString());
                                                MIDCol.Add(element.MID.ToString());
                                                ClientNameCol.Add(element.ClientName.ToString());
                                                ServiceProviderNameCol.Add(element.ServiceProviderName.ToString());
                                                FENameCol.Add(element.FEName.ToString());
                                                TypeDescriptionCol.Add(element.TypeDescription.ToString());
                                                ModelDescriptionCol.Add(element.ModelDescription.ToString());
                                                BrandDescriptionCol.Add(element.BrandDescription.ToString());
                                                IRDateCol.Add(element.IRDate.ToString());
                                                InstallationDateCol.Add(element.InstallationDate.ToString());
                                                FSRDateCol.Add(element.FSRDate.ToString());
                                                TADateTimeCol.Add(element.TADateTime.ToString());
                                                TAProcessedByCol.Add(element.TAProcessedBy.ToString());
                                                TAModifiedByCol.Add(element.TAModifiedBy.ToString());
                                                TAProcessedDateTimeCol.Add(element.TAProcessedDateTime.ToString());
                                                TAModifiedDateTimeCol.Add(element.TAModifiedDateTime.ToString());
                                                TARemarksCol.Add(element.TARemarks.ToString());
                                                TACommentsCol.Add(element.TAComments.ToString());
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription.ToString());
                                                OtherServiceTypeDescriptionCol.Add(element.OtherServiceTypeDescription.ToString());
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo.ToString());
                                                SIMCarrierCol.Add(element.SIMCarrier.ToString());
                                                IRImportDateTimeCol.Add(element.IRImportDateTime.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                RegionCol.Add(element.Region.ToString());
                                                ServiceTypeStatusCol.Add(element.ServiceTypeStatus.ToString());
                                                ServiceTypeStatusDescriptionCol.Add(element.ServiceTypeStatusDescription.ToString());
                                                DockIDCol.Add(element.DockID.ToString());
                                                DockSNCol.Add(element.DockSN.ToString());
                                            }

                                            // Loop And Store To Array
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                            clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                            clsArray.TerminalStatus = TerminalStatusCol.ToArray();
                                            clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.TAID = TAIDCol.ToArray();
                                            clsArray.IRID = IRIDCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalTypeID = TerminalTypeIDCol.ToArray();
                                            clsArray.TerminalModelID = TerminalModelIDCol.ToArray();
                                            clsArray.TerminalBrandID = TerminalBrandIDCol.ToArray();
                                            clsArray.ServiceTypeID = ServiceTypeIDCol.ToArray();
                                            clsArray.OtherServiceTypeID = OtherServiceTypeIDCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.ServiceProviderName = ServiceProviderNameCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.TypeDescription = TypeDescriptionCol.ToArray();
                                            clsArray.ModelDescription = ModelDescriptionCol.ToArray();
                                            clsArray.BrandDescription = BrandDescriptionCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.TADateTime = TADateTimeCol.ToArray();
                                            clsArray.TAProcessedBy = TAProcessedByCol.ToArray();
                                            clsArray.TAModifiedBy = TAModifiedByCol.ToArray();
                                            clsArray.TAProcessedDateTime = TAProcessedDateTimeCol.ToArray();
                                            clsArray.TAModifiedDateTime = TAModifiedDateTimeCol.ToArray();
                                            clsArray.TARemarks = TARemarksCol.ToArray();
                                            clsArray.TAComments = TACommentsCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();
                                            clsArray.OtherServiceTypeDescription = OtherServiceTypeDescriptionCol.ToArray();
                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                            clsArray.IRImportDateTime = IRImportDateTimeCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                            clsArray.ServiceTypeStatus = ServiceTypeStatusCol.ToArray();
                                            clsArray.ServiceTypeStatusDescription = ServiceTypeStatusDescriptionCol.ToArray();
                                            clsArray.DockID = DockIDCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Advance Servicing") == 0 ||
                                                 SearchBy.CompareTo("TA Detail") == 0)
                                        {
                                            foreach (var element in Detail21.data)
                                            {
                                                clsTA.RecordFound = true;
                                                SerialNoCol.Add(element.SerialNo.ToString());
                                                DeliveryDateCol.Add(element.DeliveryDate.ToString());
                                                ReceiveDateCol.Add(element.ReceiveDate.ToString());
                                                TerminalStatusCol.Add(element.TerminalStatus.ToString());
                                                TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                TAIDCol.Add(element.TAID.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                ServiceProviderIDCol.Add(element.ServiceProviderID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                TerminalTypeIDCol.Add(element.TerminalTypeID.ToString());
                                                TerminalModelIDCol.Add(element.TerminalModelID.ToString());
                                                TerminalBrandIDCol.Add(element.TerminalBrandID.ToString());
                                                ServiceTypeIDCol.Add(element.ServiceTypeID.ToString());
                                                OtherServiceTypeIDCol.Add(element.OtherServiceTypeID.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                TerminalSNCol.Add(element.TerminalSN.ToString());
                                                MerchantNameCol.Add(element.MerchantName.ToString());
                                                TIDCol.Add(element.TID.ToString());
                                                MIDCol.Add(element.MID.ToString());
                                                ClientNameCol.Add(element.ClientName.ToString());
                                                ServiceProviderNameCol.Add(element.ServiceProviderName.ToString());
                                                FENameCol.Add(element.FEName.ToString());
                                                TypeDescriptionCol.Add(element.TypeDescription.ToString());
                                                ModelDescriptionCol.Add(element.ModelDescription.ToString());
                                                BrandDescriptionCol.Add(element.BrandDescription.ToString());
                                                IRDateCol.Add(element.IRDate.ToString());
                                                InstallationDateCol.Add(element.InstallationDate.ToString());
                                                FSRDateCol.Add(element.FSRDate.ToString());
                                                TADateTimeCol.Add(element.TADateTime.ToString());
                                                TAProcessedByCol.Add(element.TAProcessedBy.ToString());
                                                TAModifiedByCol.Add(element.TAModifiedBy.ToString());
                                                TAProcessedDateTimeCol.Add(element.TAProcessedDateTime.ToString());
                                                TAModifiedDateTimeCol.Add(element.TAModifiedDateTime.ToString());
                                                TARemarksCol.Add(element.TARemarks.ToString());
                                                TACommentsCol.Add(element.TAComments.ToString());
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription.ToString());
                                                OtherServiceTypeDescriptionCol.Add(element.OtherServiceTypeDescription.ToString());
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo.ToString());
                                                SIMCarrierCol.Add(element.SIMCarrier.ToString());
                                                IRImportDateTimeCol.Add(element.IRImportDateTime.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                RegionCol.Add(element.Region.ToString());
                                                ServiceTypeStatusCol.Add(element.ServiceTypeStatus.ToString());
                                                ServiceTypeStatusDescriptionCol.Add(element.ServiceTypeStatusDescription.ToString());
                                                DockIDCol.Add(element.DockID.ToString());
                                                DockSNCol.Add(element.DockSN.ToString());
                                                JobTypeCol.Add(element.JobType.ToString());
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription.ToString());
                                                JobTypeSubDescriptionCol.Add(element.JobTypeSubDescription.ToString());
                                                JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription.ToString());
                                                ServiceDateTimeCol.Add(element.ServiceDateTime.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());

                                            }

                                            // Loop And Store To Array
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                            clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                            clsArray.TerminalStatus = TerminalStatusCol.ToArray();
                                            clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.TAID = TAIDCol.ToArray();
                                            clsArray.IRID = IRIDCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalTypeID = TerminalTypeIDCol.ToArray();
                                            clsArray.TerminalModelID = TerminalModelIDCol.ToArray();
                                            clsArray.TerminalBrandID = TerminalBrandIDCol.ToArray();
                                            clsArray.ServiceTypeID = ServiceTypeIDCol.ToArray();
                                            clsArray.OtherServiceTypeID = OtherServiceTypeIDCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.ServiceProviderName = ServiceProviderNameCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.TypeDescription = TypeDescriptionCol.ToArray();
                                            clsArray.ModelDescription = ModelDescriptionCol.ToArray();
                                            clsArray.BrandDescription = BrandDescriptionCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.TADateTime = TADateTimeCol.ToArray();
                                            clsArray.TAProcessedBy = TAProcessedByCol.ToArray();
                                            clsArray.TAModifiedBy = TAModifiedByCol.ToArray();
                                            clsArray.TAProcessedDateTime = TAProcessedDateTimeCol.ToArray();
                                            clsArray.TAModifiedDateTime = TAModifiedDateTimeCol.ToArray();
                                            clsArray.TARemarks = TARemarksCol.ToArray();
                                            clsArray.TAComments = TACommentsCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();
                                            clsArray.OtherServiceTypeDescription = OtherServiceTypeDescriptionCol.ToArray();
                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                            clsArray.IRImportDateTime = IRImportDateTimeCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                            clsArray.ServiceTypeStatus = ServiceTypeStatusCol.ToArray();
                                            clsArray.ServiceTypeStatusDescription = ServiceTypeStatusDescriptionCol.ToArray();
                                            clsArray.DockID = DockIDCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.JobType = JobTypeCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.JobTypeSubDescription = JobTypeSubDescriptionCol.ToArray();
                                            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("TA List") == 0)
                                        {
                                            foreach (var element in Detail21.data)
                                            {
                                                clsTA.RecordFound = true;
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                ServiceProviderIDCol.Add(element.ServiceProviderID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                RegionTypeCol.Add(element.RegionType.ToString());
                                                RequestNoCol.Add(element.RequestNo);
                                                ReferenceNoCol.Add(element.ReferenceNo);
                                                ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                ServiceDateCol.Add(element.ServiceDate);
                                                ServiceTimeCol.Add(element.ServiceTime);
                                                ServiceReqDateCol.Add(element.ServiceReqDate);
                                                ServiceReqTimeCol.Add(element.ServiceReqTime);
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                IRNoCol.Add(element.IRNo);
                                                MerchantNameCol.Add(element.MerchantName);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                TerminalSNCol.Add(element.TerminalSN);
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo);
                                                DockIDCol.Add(element.DockID.ToString());
                                                DockSNCol.Add(element.DockSN);
                                                PrimaryNumCol.Add(element.PrimaryNum.ToString());
                                                SecondaryNumCol.Add(element.SecondaryNum.ToString());
                                            }

                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.RegionType = RegionTypeCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                            clsArray.ServiceDate = ServiceDateCol.ToArray();
                                            clsArray.ServiceTime = ServiceTimeCol.ToArray();
                                            clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                            clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                            clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.DockID = DockIDCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.PrimaryNum = PrimaryNumCol.ToArray();
                                            clsArray.SecondaryNum = SecondaryNumCol.ToArray();
                                            
                                        }                                            
                                        else
                                        {
                                            foreach (var element in Detail21.data)
                                            {
                                                clsTA.RecordFound = true;
                                                SerialNoCol.Add(element.SerialNo.ToString());
                                                DeliveryDateCol.Add(element.DeliveryDate.ToString());
                                                ReceiveDateCol.Add(element.ReceiveDate.ToString());
                                                TerminalStatusCol.Add(element.TerminalStatus.ToString());
                                                TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription.ToString());
                                                TAIDNoCol.Add(element.TAIDNo.ToString());
                                                TAIDCol.Add(element.TAID.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                ClientIDCol.Add(element.ClientID.ToString());
                                                ServiceProviderIDCol.Add(element.ServiceProviderID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                FEIDCol.Add(element.FEID.ToString());
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                TerminalTypeIDCol.Add(element.TerminalTypeID.ToString());
                                                TerminalModelIDCol.Add(element.TerminalModelID.ToString());
                                                TerminalBrandIDCol.Add(element.TerminalBrandID.ToString());
                                                ServiceTypeIDCol.Add(element.ServiceTypeID.ToString());
                                                OtherServiceTypeIDCol.Add(element.OtherServiceTypeID.ToString());
                                                IRNoCol.Add(element.IRNo.ToString());
                                                TerminalSNCol.Add(element.TerminalSN.ToString());
                                                MerchantNameCol.Add(element.MerchantName.ToString());
                                                TIDCol.Add(element.TID.ToString());
                                                MIDCol.Add(element.MID.ToString());
                                                ClientNameCol.Add(element.ClientName.ToString());
                                                ServiceProviderNameCol.Add(element.ServiceProviderName.ToString());
                                                FENameCol.Add(element.FEName.ToString());
                                                TypeDescriptionCol.Add(element.TypeDescription.ToString());
                                                ModelDescriptionCol.Add(element.ModelDescription.ToString());
                                                BrandDescriptionCol.Add(element.BrandDescription.ToString());
                                                IRDateCol.Add(element.IRDate.ToString());
                                                InstallationDateCol.Add(element.InstallationDate.ToString());
                                                FSRDateCol.Add(element.FSRDate.ToString());
                                                TADateTimeCol.Add(element.TADateTime.ToString());
                                                TAProcessedByCol.Add(element.TAProcessedBy.ToString());
                                                TAModifiedByCol.Add(element.TAModifiedBy.ToString());
                                                TAProcessedDateTimeCol.Add(element.TAProcessedDateTime.ToString());
                                                TAModifiedDateTimeCol.Add(element.TAModifiedDateTime.ToString());
                                                TARemarksCol.Add(element.TARemarks.ToString());
                                                TACommentsCol.Add(element.TAComments.ToString());
                                                ServiceTypeDescriptionCol.Add(element.ServiceTypeDescription.ToString());
                                                OtherServiceTypeDescriptionCol.Add(element.OtherServiceTypeDescription.ToString());
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo.ToString());
                                                SIMCarrierCol.Add(element.SIMCarrier.ToString());
                                                IRImportDateTimeCol.Add(element.IRImportDateTime.ToString());
                                                RegionIDCol.Add(element.RegionID.ToString());
                                                RegionTypeCol.Add(element.RegionType.ToString());
                                                RegionCol.Add(element.Region.ToString());
                                                ServiceTypeStatusCol.Add(element.ServiceTypeStatus.ToString());
                                                ServiceTypeStatusDescriptionCol.Add(element.ServiceTypeStatusDescription.ToString());
                                                DockIDCol.Add(element.DockID.ToString());
                                                DockSNCol.Add(element.DockSN.ToString());
                                                JobTypeCol.Add(element.JobType.ToString());
                                                JobTypeDescriptionCol.Add(element.JobTypeDescription.ToString());
                                                JobTypeSubDescriptionCol.Add(element.JobTypeSubDescription.ToString());
                                                JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription.ToString());
                                                ServiceDateTimeCol.Add(element.ServiceDateTime.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                RequestNoCol.Add(element.RequestNo.ToString());
                                                PrimaryNumCol.Add(element.PrimaryNum);
                                                SecondaryNumCol.Add(element.SecondaryNum);
                                                AppVersionCol.Add(element.AppVersion.ToString());
                                                AppCRCCol.Add(element.AppCRC.ToString());
                                            }

                                            // Loop And Store To Array
                                            clsArray.SerialNo = SerialNoCol.ToArray();
                                            clsArray.DeliveryDate = DeliveryDateCol.ToArray();
                                            clsArray.ReceiveDate = ReceiveDateCol.ToArray();
                                            clsArray.TerminalStatus = TerminalStatusCol.ToArray();
                                            clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                            clsArray.TAIDNo = TAIDNoCol.ToArray();
                                            clsArray.TAID = TAIDCol.ToArray();
                                            clsArray.IRID = IRIDCol.ToArray();
                                            clsArray.ClientID = ClientIDCol.ToArray();
                                            clsArray.ServiceProviderID = ServiceProviderIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.FEID = FEIDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalTypeID = TerminalTypeIDCol.ToArray();
                                            clsArray.TerminalModelID = TerminalModelIDCol.ToArray();
                                            clsArray.TerminalBrandID = TerminalBrandIDCol.ToArray();
                                            clsArray.ServiceTypeID = ServiceTypeIDCol.ToArray();
                                            clsArray.OtherServiceTypeID = OtherServiceTypeIDCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TerminalSN = TerminalSNCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                            clsArray.ClientName = ClientNameCol.ToArray();
                                            clsArray.ServiceProviderName = ServiceProviderNameCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.TypeDescription = TypeDescriptionCol.ToArray();
                                            clsArray.ModelDescription = ModelDescriptionCol.ToArray();
                                            clsArray.BrandDescription = BrandDescriptionCol.ToArray();
                                            clsArray.IRDate = IRDateCol.ToArray();
                                            clsArray.InstallationDate = InstallationDateCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                            clsArray.TADateTime = TADateTimeCol.ToArray();
                                            clsArray.TAProcessedBy = TAProcessedByCol.ToArray();
                                            clsArray.TAModifiedBy = TAModifiedByCol.ToArray();
                                            clsArray.TAProcessedDateTime = TAProcessedDateTimeCol.ToArray();
                                            clsArray.TAModifiedDateTime = TAModifiedDateTimeCol.ToArray();
                                            clsArray.TARemarks = TARemarksCol.ToArray();
                                            clsArray.TAComments = TACommentsCol.ToArray();
                                            clsArray.ServiceTypeDescription = ServiceTypeDescriptionCol.ToArray();
                                            clsArray.OtherServiceTypeDescription = OtherServiceTypeDescriptionCol.ToArray();
                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                            clsArray.IRImportDateTime = IRImportDateTimeCol.ToArray();
                                            clsArray.RegionID = RegionIDCol.ToArray();
                                            clsArray.RegionType = RegionTypeCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                            clsArray.ServiceTypeStatus = ServiceTypeStatusCol.ToArray();
                                            clsArray.ServiceTypeStatusDescription = ServiceTypeStatusDescriptionCol.ToArray();
                                            clsArray.DockID = DockIDCol.ToArray();
                                            clsArray.DockSN = DockSNCol.ToArray();
                                            clsArray.JobType = JobTypeCol.ToArray();
                                            clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                            clsArray.JobTypeSubDescription = JobTypeSubDescriptionCol.ToArray();
                                            clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                            clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.PrimaryNum = PrimaryNumCol.ToArray();
                                            clsArray.SecondaryNum = SecondaryNumCol.ToArray();
                                            clsArray.AppVersion = AppVersionCol.ToArray();
                                            clsArray.AppCRC = AppCRCCol.ToArray();
                                        }

                                        break;
                                }
                                break;
                            case "Report":
                                ReportDetailOnline Detail22 = JsonConvert.DeserializeObject<ReportDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsReport.ClassRecordFound = false;
                                switch (StatementType)
                                {
                                    case "View":
                                        foreach (var element in Detail22.data)
                                        {
                                            clsReport.ClassRecordFound = true;


                                            List<string> ReportID = new List<String>();
                                            List<string> ReportDesc = new List<String>();
                                            List<string> ReportType = new List<String>();
                                            List<string> ReportOrderDisplay = new List<String>();


                                            ReportIDCol.Add(element.ReportID.ToString());
                                            ReportDescCol.Add(element.ReportDesc.ToString());
                                            ReportTypeCol.Add(element.ReportType.ToString());
                                            ReportOrderDisplayCol.Add(element.ReportOrderDisplay.ToString());
                                        }

                                        // Loop And Store To Array
                                        clsArray.ReportID = ReportIDCol.ToArray();
                                        clsArray.ReportDesc = ReportDescCol.ToArray();
                                        clsArray.ReportType = ReportTypeCol.ToArray();
                                        clsArray.ReportOrderDisplay = ReportOrderDisplayCol.ToArray();

                                        break;
                                }
                                break;
                            case "Header":
                                HeaderDetailOnline Detail23 = JsonConvert.DeserializeObject<HeaderDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsHeader.ClassRecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail23.data)
                                        {
                                            clsHeader.ClassRecordFound = true;
                                            clsHeader.ClassHeaderID = element.HeaderID;
                                            clsHeader.ClassName = element.Name;
                                            clsHeader.ClassHeader1 = element.Header1;
                                            clsHeader.ClassHeader2 = element.Header2;
                                            clsHeader.ClassHeader3 = element.Header3;
                                            clsHeader.ClassHeader4 = element.Header4;
                                            clsHeader.ClassHeader5 = element.Header5;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail23.data)
                                        {
                                            clsHeader.ClassRecordFound = true;
                                            HeaderIDCol.Add(element.HeaderID.ToString());
                                            NameCol.Add(element.Name);
                                            Header1Col.Add(element.Header1);
                                            Header2Col.Add(element.Header2);
                                            Header3Col.Add(element.Header3);
                                            Header4Col.Add(element.Header4);
                                            Header5Col.Add(element.Header5);
                                        }

                                        clsArray.HeaderID = HeaderIDCol.ToArray();
                                        clsArray.Name = NameCol.ToArray();
                                        clsArray.Header1 = Header1Col.ToArray();
                                        clsArray.Header2 = Header2Col.ToArray();
                                        clsArray.Header3 = Header3Col.ToArray();
                                        clsArray.Header4 = Header4Col.ToArray();
                                        clsArray.Header5 = Header5Col.ToArray();
                                        break;
                                }
                                break;
                            case "Region":
                                RegionDetailOnline Detail24 = JsonConvert.DeserializeObject<RegionDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsRegion.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail24.data)
                                        {
                                            clsRegion.RecordFound = true;
                                            clsRegion.ClassRegionID = element.RegionID;
                                            clsRegion.ClassRegion = element.Region;
                                        }
                                        break;
                                    case "View":
                                        
                                        if (SearchBy.Equals("RegionDetail"))
                                        {
                                            foreach (var element in Detail24.data)
                                            {
                                                clsRegion.RecordFound = true;
                                                IDCol.Add(element.RegionID.ToString());
                                                ProvinceCol.Add(element.Province);
                                            }

                                            clsArray.RegionID = IDCol.ToArray();
                                            clsArray.Province = ProvinceCol.ToArray();
                                        }
                                        else
                                        {
                                            foreach (var element in Detail24.data)
                                            {
                                                clsRegion.RecordFound = true;
                                                IDCol.Add(element.RegionID.ToString());
                                                RegionCol.Add(element.Region);
                                            }

                                            clsArray.RegionID = IDCol.ToArray();
                                            clsArray.Region = RegionCol.ToArray();
                                        }

                                        
                                        break;
                                }
                                break;
                            case "System":
                                SystemSettingDetailOnline Detail25 = JsonConvert.DeserializeObject<SystemSettingDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsSystemSetting.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail25.data)
                                        {
                                            clsSystemSetting.RecordFound = true;
                                            clsSystemSetting.ClassSystemID = element.SysID;
                                            clsSystemSetting.ClassSystemPublishDate = element.PublishDate;
                                            clsSystemSetting.ClassSystemPublishVersion = element.PublishVersion;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail25.data)
                                        {
                                            clsSystemSetting.RecordFound = true;
                                            SysIDCol.Add(element.SysID.ToString());
                                            PublishDateCol.Add(element.PublishDate);
                                            PublishVersionCol.Add(element.PublishVersion);
                                        }

                                        clsArray.SysID = SysIDCol.ToArray();
                                        clsArray.PublishDate = PublishDateCol.ToArray();
                                        clsArray.PublishVersion = PublishVersionCol.ToArray();
                                        break;
                                }
                                break;
                            case "Get Count":
                                TerminalDetailOnline Detail26 = JsonConvert.DeserializeObject<TerminalDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail26.data)
                                        {
                                            clsTerminal.ClassTerminalCount = int.Parse(element.TerminalCount);
                                        }

                                        break;
                                }
                                break;
                            case "Reason":
                                ReasonDetailOnline Detail27 = JsonConvert.DeserializeObject<ReasonDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsReason.ClassRecordFound = false;
                                ReasonIDCol.Clear();
                                ReasonCodeCol.Clear();
                                ReasonDescriptionCol.Clear();
                                ReasonTypeCol.Clear();
                                ReasonIsInputCol.Clear();
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail27.data)
                                        {
                                            clsReason.ClassRecordFound = true;
                                            clsReason.ClassReasonID = element.ReasonID;
                                            clsReason.ClassReasonCode = element.Code;
                                            clsReason.ClassReasonDescription = element.Description;
                                            clsReason.ClassReasonType = element.Type;
                                            clsReason.ClassReasonIsInput = element.IsInput;

                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail27.data)
                                        {
                                            clsReason.ClassRecordFound = true;
                                            ReasonIDCol.Add(element.ReasonID.ToString());
                                            ReasonCodeCol.Add(element.Code);
                                            ReasonDescriptionCol.Add(element.Description);
                                            ReasonTypeCol.Add(element.Type);
                                            ReasonIsInputCol.Add(element.IsInput.ToString());

                                        }

                                        clsArray.ReasonID = ReasonIDCol.ToArray();
                                        clsArray.ReasonCode = ReasonCodeCol.ToArray();
                                        clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();
                                        clsArray.ReasonType = ReasonTypeCol.ToArray();
                                        clsArray.ReasonIsInput = ReasonIsInputCol.ToArray();
                                        break;
                                }
                                break;
                            case "FSR Attempt":
                                FSRDetailOnline Detail28 = JsonConvert.DeserializeObject<FSRDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsFSR.RecordFound = false;
                                FSRNoCol.Clear();
                                FSRDateCol.Clear();
                                FSRTimeCol.Clear();
                                FSRServiceStatusCol.Clear();
                                FSRServiceStatusDescriptionCol.Clear();
                                FSRRemarksCol.Clear();

                                switch (StatementType)
                                {
                                    case "View":
                                        foreach (var element in Detail28.data)
                                        {
                                            clsFSR.RecordFound = true;
                                            FSRNoCol.Add(element.FSRNo.ToString());
                                            FSRDateCol.Add(element.FSRDate);
                                            FSRTimeCol.Add(element.FSRTime);
                                            FSRServiceStatusCol.Add(element.ServiceStatus.ToString());
                                            FSRServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                            FSRRemarksCol.Add(element.Remarks);
                                        }

                                        clsArray.FSRNo = FSRNoCol.ToArray();
                                        clsArray.FSRDate = FSRDateCol.ToArray();
                                        clsArray.FSRTime = FSRTimeCol.ToArray();
                                        clsArray.FSRServiceStatus = FSRServiceStatusCol.ToArray();
                                        clsArray.FSRServiceStatusDescription = FSRServiceStatusDescriptionCol.ToArray();
                                        clsArray.FSRRemarks = FSRRemarksCol.ToArray();
                                        break;
                                }
                                break;
                            case "CheckControlID":
                                CheckControlIDDetailOnline Detail29 = JsonConvert.DeserializeObject<CheckControlIDDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsCheckControlID.RecordFound = false;

                                foreach (var element in Detail29.data)
                                {
                                    clsCheckControlID.ClassControlID = element.ControlID;
                                }

                                break;
                            case "Region Detail":
                                RegionDetailDetailOnline Detail30 = JsonConvert.DeserializeObject<RegionDetailDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsRegionDetail.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail30.data)
                                        {
                                            clsRegionDetail.RecordFound = true;
                                            clsRegionDetail.ClassRegionID = element.RegionID;
                                            clsRegionDetail.ClassRegionType = element.RegionType;
                                            clsRegionDetail.ClassProvince = element.Province;
                                            clsRegionDetail.ClassRegion = element.Region;
                                        }
                                        break;
                                    case "View":
                                        string sRegionList = "";
                                        string sProvinceList = "";
                                        foreach (var element in Detail30.data)
                                        {
                                            clsRegionDetail.RecordFound = true;
                                            RegionIDCol.Add(element.RegionID.ToString());
                                            RegionTypeCol.Add(element.RegionType.ToString());
                                            ProvinceCol.Add(element.Province);
                                            RegionCol.Add(element.Region);

                                            sProvinceList = sProvinceList + element.Province + Environment.NewLine; // Province List
                                            sRegionList = sRegionList + element.Region + Environment.NewLine; // Region List
                                        }

                                        clsArray.RegionID = RegionIDCol.ToArray();
                                        clsArray.RegionType = RegionTypeCol.ToArray();
                                        clsArray.RegionProvince = ProvinceCol.ToArray();
                                        clsArray.Region = RegionCol.ToArray();

                                        clsSearch.ClassProvinceList = sProvinceList; // Province List
                                        clsSearch.ClassRegionList = sRegionList; // Region List

                                        break;
                                }
                                break;
                            case "SIM Detail":
                                SIMDetailOnline Detail31 = JsonConvert.DeserializeObject<SIMDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsSIM.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail31.data)
                                        {
                                            clsSIM.RecordFound = true;
                                            clsSIM.ClassSIMID = element.SIMID;
                                            clsSIM.ClassSIMSN = element.SIMSerialNo;
                                            clsSIM.ClassSIMCarrier = element.SIMCarrier;
                                            clsSIM.ClassSIMStatus = element.SIMStatus;
                                            clsSIM.ClassSIMStatusDescription = element.SIMStatusDescription;
                                        }

                                        break;
                                    case "View":

                                        if (SearchBy.Equals("Service History"))
                                        {
                                            foreach (var element in Detail31.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                ParticularNameCol.Add(element.ParticularName);
                                                RemarksCol.Add(element.Remarks);
                                                DateCol.Add(element.Date);
                                                TimeCol.Add(element.Time);
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                IRNoCol.Add(element.IRNo);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);


                                            }

                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.Date = DateCol.ToArray();
                                            clsArray.Time = TimeCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                        }
                                        else if (SearchBy.Equals("Release Movement Master"))
                                        {
                                            foreach (var element in Detail31.data)
                                            {
                                                clsSearch.RecordFound = true;

                                                TransNoCol.Add(element.TransNo.ToString());
                                                TransDateCol.Add(element.TransDate);
                                                TransTimeCol.Add(element.TransTime);
                                                ReleaseDateCol.Add(element.ReleaseDate);
                                                RequestNoCol.Add(element.RequestNo);
                                                ReferenceNoCol.Add(element.ReferenceNo);
                                                RemarksCol.Add(element.Remarks);
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                ModifiedByCol.Add(element.ModifiedBy);
                                                ModifiedDateTimeCol.Add(element.ModifiedDateTime);
                                                UserIDCol.Add(element.UserID.ToString());
                                                FromLocationIDCol.Add(element.FromLocationID.ToString());
                                                FromLocationCol.Add(element.FromLocation);
                                                ToLocationIDCol.Add(element.ToLocationID.ToString());
                                                ToLocationCol.Add(element.ToLocation);

                                            }

                                            clsArray.TransNo = TransNoCol.ToArray();
                                            clsArray.TransDate = TransDateCol.ToArray();
                                            clsArray.TransTime = TransTimeCol.ToArray();
                                            clsArray.ReleaseDate = ReleaseDateCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.ModifiedBy = ModifiedByCol.ToArray();
                                            clsArray.ModifiedDateTime = ModifiedDateTimeCol.ToArray();
                                            clsArray.UserID = UserIDCol.ToArray();
                                            clsArray.FromLocationID = FromLocationIDCol.ToArray();
                                            clsArray.FromLocation = FromLocationCol.ToArray();
                                            clsArray.ToLocationID = ToLocationIDCol.ToArray();
                                            clsArray.ToLocation = ToLocationCol.ToArray();
                                        }
                                        else
                                        {
                                            foreach (var element in Detail31.data)
                                            {
                                                clsSIM.RecordFound = true;
                                                SIMIDCol.Add(element.SIMID.ToString());
                                                SIMSerialNoCol.Add(element.SIMSerialNo);
                                                SIMCarrierCol.Add(element.SIMCarrier);
                                                SIMStatusCol.Add(element.SIMStatus.ToString());
                                                SIMStatusDescriptionCol.Add(element.SIMStatusDescription);
                                            }

                                            clsArray.SIMID = SIMIDCol.ToArray();
                                            clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                            clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                            clsArray.SIMStatus = SIMStatusCol.ToArray();
                                            clsArray.SIMStatusDescription = SIMStatusDescriptionCol.ToArray();
                                        }
                                        

                                        break;
                                }

                                break;
                            case "Service Call":
                                ServiceCallDetailOnline Detail32 = JsonConvert.DeserializeObject<ServiceCallDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsServiceCall.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "View":
                                        foreach (var element in Detail32.data)
                                        {
                                            clsServiceCall.RecordFound = true;
                                            SCNoCol.Add(element.SCNo.ToString());
                                            SCDateTimeCol.Add(element.SCDateTime);
                                            ReferralIDCol.Add(element.ReferralID);
                                            CustomerNameCol.Add(element.CustomerName);
                                            CustomerContactNoCol.Add(element.CustomerContactNo);
                                            ReportedProblemCol.Add(element.ReportedProblem);
                                            ArrangementMadeCol.Add(element.ArrangementMade);
                                            SCReqDateCol.Add(element.SCReqDate);
                                            SCReqTimeCol.Add(element.SCReqTime);
                                            SCShipDateCol.Add(element.SCShipDate);
                                            SCShipTimeCol.Add(element.SCShipTime);
                                            TrackingNoCol.Add(element.TrackingNo);
                                            SCStatusCol.Add(element.SCStatus);
                                        }

                                        clsArray.SCNo = SCNoCol.ToArray();
                                        clsArray.SCDateTime = SCDateTimeCol.ToArray();
                                        clsArray.ReferralID = ReferralIDCol.ToArray();
                                        clsArray.CustomerName = CustomerNameCol.ToArray();
                                        clsArray.CustomerContactNo = CustomerContactNoCol.ToArray();
                                        clsArray.ReportedProblem = ReportedProblemCol.ToArray();
                                        clsArray.ArrangementMade = ArrangementMadeCol.ToArray();
                                        clsArray.SCReqDate = SCReqDateCol.ToArray();
                                        clsArray.SCReqTime = SCReqTimeCol.ToArray();
                                        clsArray.SCShipDate = SCShipDateCol.ToArray();
                                        clsArray.SCShipTime = SCShipTimeCol.ToArray();
                                        clsArray.TrackingNo = TrackingNoCol.ToArray();
                                        clsArray.SCStatus = SCStatusCol.ToArray();

                                        break;
                                }
                                break;
                            case "Servicing Detail":
                                ServicingDetailOnline Detail33 = JsonConvert.DeserializeObject<ServicingDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsServicingDetail.RecordFound = false;
                                
                                ServiceNoCol.Clear();
                                TAIDNoCol.Clear();
                                IRIDNoCol.Clear();
                                IRNoCol.Clear();
                                TIDCol.Clear();
                                MIDCol.Clear();
                                TerminalIDCol.Clear();
                                TerminalSNCol.Clear();
                                TerminalStatusDescriptionCol.Clear();
                                TypeDescriptionCol.Clear();
                                ModelDescriptionCol.Clear();
                                BrandDescriptionCol.Clear();                                
                                IRDateCol.Clear();
                                InstallationDateCol.Clear();
                                MerchantNameCol.Clear();
                                TIDCol.Clear();
                                MIDCol.Clear();
                                RegionCol.Clear();
                                ProvinceCol.Clear();
                                IRStatusDescriptionCol.Clear();
                                ServiceDateTimeCol.Clear();
                                ServiceReqDateCol.Clear();
                                ServiceReqTimeCol.Clear();
                                RequestNoCol.Clear();
                                ReferenceNoCol.Clear();
                                CustomerNameCol.Clear();
                                CustomerContactNoCol.Clear();
                                ClientNameCol.Clear();
                                FENameCol.Clear();
                                TerminalSNCol.Clear();
                                SIMSerialNoCol.Clear();
                                DockSNCol.Clear();
                                JobTypeDescriptionCol.Clear();
                                JobTypeStatusDescriptionCol.Clear();
                                ActionMadeCol.Clear();
                                TimeArrivedCol.Clear();
                                TimeStartCol.Clear();
                                FSRDateCol.Clear();
                                FSRTimeCol.Clear();
                                TimeEndCol.Clear();
                                RemarksCol.Clear();
                                ProblemReportedCol.Clear();
                                ActualProblemReportedCol.Clear();
                                ActionTakenCol.Clear();
                                AnyCommentsCol.Clear();
                                MerchantRepresentativeCol.Clear();
                                MerchantContactNoCol.Clear();

                                switch (StatementType)
                                {
                                    case "View":

                                        switch (SearchBy)
                                        {
                                            case "Servicing Current Terminal":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    TerminalIDCol.Add(element.TerminalID.ToString());
                                                    SIMIDCol.Add(element.SIMID.ToString());
                                                    DockIDCol.Add(element.DockID.ToString());
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    SIMSerialNoCol.Add(element.SIMSN);
                                                    DockSNCol.Add(element.DockSN);
                                                    CurTerminalSNStatusCol.Add(element.CurTerminalSNStatus.ToString());
                                                    CurSIMSNStatusCol.Add(element.CurSIMSNStatus.ToString());
                                                    CurDockSNStatusCol.Add(element.CurDockSNStatus.ToString());
                                                    CurTerminalSNStatusDescriptionCol.Add(element.CurTerminalSNStatusDescription);
                                                    CurSIMSNStatusDescriptionCol.Add(element.CurSIMSNStatusDescription);
                                                    CurDockSNStatusDescriptionCol.Add(element.CurDockSNStatusDescription);
                                                }
                                                clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                clsArray.TerminalID = TerminalIDCol.ToArray();
                                                clsArray.SIMID = SIMIDCol.ToArray();
                                                clsArray.DockID = DockIDCol.ToArray();
                                                clsArray.TerminalSN = TerminalSNCol.ToArray();
                                                clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                                clsArray.DockSN = DockSNCol.ToArray();
                                                clsArray.CurTerminalSNStatus = CurTerminalSNStatusCol.ToArray();
                                                clsArray.CurSIMSNStatus = CurSIMSNStatusCol.ToArray();
                                                clsArray.CurDockSNStatus = CurDockSNStatusCol.ToArray();
                                                clsArray.CurTerminalSNStatusDescription = CurTerminalSNStatusDescriptionCol.ToArray();
                                                clsArray.CurSIMSNStatusDescription = CurSIMSNStatusDescriptionCol.ToArray();
                                                clsArray.CurDockSNStatusDescription = CurDockSNStatusDescriptionCol.ToArray();
                                                break;
                                            case "Servicing Replace Terminal":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    TerminalIDCol.Add(element.TerminalID.ToString());
                                                    SIMIDCol.Add(element.SIMID.ToString());
                                                    DockIDCol.Add(element.DockID.ToString());
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    SIMSerialNoCol.Add(element.SIMSN);
                                                    DockSNCol.Add(element.DockSN);
                                                    RepTerminalSNStatusCol.Add(element.RepTerminalSNStatus.ToString());
                                                    RepSIMSNStatusCol.Add(element.RepSIMSNStatus.ToString());
                                                    RepDockSNStatusCol.Add(element.RepDockSNStatus.ToString());                                                    
                                                }
                                                clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                clsArray.TerminalID = TerminalIDCol.ToArray();
                                                clsArray.SIMID = SIMIDCol.ToArray();
                                                clsArray.DockID = DockIDCol.ToArray();
                                                clsArray.TerminalSN = TerminalSNCol.ToArray();
                                                clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                                clsArray.DockSN = DockSNCol.ToArray();
                                                clsArray.RepTerminalSNStatus = RepTerminalSNStatusCol.ToArray();
                                                clsArray.RepSIMSNStatus = RepSIMSNStatusCol.ToArray();
                                                clsArray.RepDockSNStatus = RepDockSNStatusCol.ToArray();                                                
                                                break;
                                            case "Download Service":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    TAIDNoCol.Add(element.TAIDNo.ToString());
                                                    IRIDNoCol.Add(element.IRIDNo.ToString());
                                                    IRNoCol.Add(element.IRNo.ToString());
                                                    TIDCol.Add(element.TID);
                                                    MIDCol.Add(element.MID);
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    SIMSerialNoCol.Add(element.SIMSN);
                                                    DockSNCol.Add(element.DockSN);
                                                    ServiceCodeCol.Add(element.ServiceCode);
                                                    ServiceDateCol.Add(element.ServiceDate);
                                                    ServiceReqDateCol.Add(element.ServiceReqDate);
                                                    JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                    RequestNoCol.Add(element.RequestNo);
                                                }
                                                clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                clsArray.TAIDNo = TAIDNoCol.ToArray();
                                                clsArray.IRIDNo = IRIDNoCol.ToArray();
                                                clsArray.IRNo = IRNoCol.ToArray();
                                                clsArray.TID = TIDCol.ToArray();
                                                clsArray.MID = MIDCol.ToArray();
                                                clsArray.TerminalSN = TerminalSNCol.ToArray();
                                                clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                                clsArray.DockSN = DockSNCol.ToArray();
                                                clsArray.ServiceCode = ServiceCodeCol.ToArray();
                                                clsArray.FSR = ServiceCodeCol.ToArray();
                                                clsArray.ServiceDate = ServiceDateCol.ToArray();
                                                clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                                clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                                clsArray.RequestNo = RequestNoCol.ToArray();

                                                break;

                                            case "Service TerminalSN List":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    //ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    TAIDNoCol.Add(element.TAIDNo.ToString());
                                                    IRIDNoCol.Add(element.IRIDNo.ToString());
                                                    IRNoCol.Add(element.IRNo.ToString());
                                                    TIDCol.Add(element.TID);
                                                    MIDCol.Add(element.MID);                                                    
                                                    TerminalIDCol.Add(element.TerminalID.ToString());
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    TerminalStatusDescriptionCol.Add(element.TerminalStatusDescription);
                                                    TypeDescriptionCol.Add(element.Type);
                                                    ModelDescriptionCol.Add(element.Model);
                                                    BrandDescriptionCol.Add(element.Brand);                                                    
                                                }
                                                //clsArray.ServiceNo = ServiceNoCol.ToArray(); // for hold
                                                clsArray.TAIDNo = TAIDNoCol.ToArray();       // for hold   
                                                clsArray.IRIDNo = IRIDNoCol.ToArray();       // for hold  

                                                clsArray.IRNo = IRNoCol.ToArray();
                                                clsArray.TID = TIDCol.ToArray();
                                                clsArray.MID = MIDCol.ToArray();                                                
                                                clsArray.TerminalID = TerminalIDCol.ToArray();
                                                clsArray.TerminalSN = TerminalSNCol.ToArray();
                                                clsArray.TerminalStatusDescription = TerminalStatusDescriptionCol.ToArray();
                                                clsArray.TypeDescription = TypeDescriptionCol.ToArray();
                                                clsArray.ModelDescription = ModelDescriptionCol.ToArray();
                                                clsArray.BrandDescription = BrandDescriptionCol.ToArray();

                                                break;
                                            case "Service IRNo List":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    IRIDNoCol.Add(element.IRIDNo.ToString());
                                                    IRNoCol.Add(element.IRNo.ToString());
                                                    IRDateCol.Add(element.IRDate);
                                                    InstallationDateCol.Add(element.InstallationDate);
                                                    MerchantNameCol.Add(element.MerchantName);                                                    
                                                    TIDCol.Add(element.TID);
                                                    MIDCol.Add(element.MID);
                                                    RegionCol.Add(element.Region);
                                                    ProvinceCol.Add(element.Province);
                                                    IRStatusDescriptionCol.Add(element.IRStatusDescription);                                                    
                                                }
                                                
                                                clsArray.IRIDNo = IRIDNoCol.ToArray();
                                                clsArray.IRNo = IRNoCol.ToArray();
                                                clsArray.IRDate = IRDateCol.ToArray();
                                                clsArray.InstallationDate = InstallationDateCol.ToArray();
                                                clsArray.MerchantName = MerchantNameCol.ToArray();
                                                clsArray.TID = TIDCol.ToArray();
                                                clsArray.MID = MIDCol.ToArray();
                                                clsArray.Region = RegionCol.ToArray();
                                                clsArray.Province = ProvinceCol.ToArray();
                                                clsArray.IRStatusDescription = IRStatusDescriptionCol.ToArray();                                                
                                                break;

                                            case "Service ServiceNo List":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());                                                    
                                                    ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                    ServiceReqDateCol.Add(element.ServiceReqDate);
                                                    ServiceReqTimeCol.Add(element.ServiceReqTime);
                                                    RequestNoCol.Add(element.RequestNo);
                                                    ReferenceNoCol.Add(element.ReferenceNo);
                                                    CustomerNameCol.Add(element.CustomerName);
                                                    CustomerContactNoCol.Add(element.CustomerContactNo);
                                                    ClientNameCol.Add(element.ClientName);
                                                    FENameCol.Add(element.FEName);
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    SIMSerialNoCol.Add(element.SIMSN);
                                                    SIMCarrierCol.Add(element.SIMCarrier);
                                                    DockSNCol.Add(element.DockSN);
                                                    ReplaceTerminalSNCol.Add(element.ReplaceTerminalSN);
                                                    ReplaceSIMSNCol.Add(element.ReplaceSIMSN);
                                                    ReplaceDockSNCol.Add(element.ReplaceDockSN);
                                                    JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                    JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                    ActionMadeCol.Add(element.ActionMade);
                                                    TimeArrivedCol.Add(element.TimeArrived);
                                                    TimeStartCol.Add(element.TimeStart);
                                                    FSRDateCol.Add(element.FSRDate);
                                                    FSRTimeCol.Add(element.FSRTime);
                                                    TimeEndCol.Add(element.TimeEnd);
                                                    RemarksCol.Add(element.Remarks);
                                                    ProblemReportedCol.Add(element.ProblemReported);
                                                    ActualProblemReportedCol.Add(element.ActualProblemReported);
                                                    ActionTakenCol.Add(element.ActionTaken);
                                                    AnyCommentsCol.Add(element.AnyComments);
                                                    MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                                    MerchantContactNoCol.Add(element.MerchantContactNo);
                                                }

                                                clsArray.ServiceNo = ServiceNoCol.ToArray();                                                
                                                clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                                clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                                clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                                clsArray.RequestNo = RequestNoCol.ToArray();
                                                clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                                clsArray.CustomerName = CustomerNameCol.ToArray();
                                                clsArray.CustomerContactNo = CustomerContactNoCol.ToArray();
                                                clsArray.ClientName = ClientNameCol.ToArray();
                                                clsArray.FEName = FENameCol.ToArray();
                                                clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                                clsArray.SIMCarrier = SIMCarrierCol.ToArray();
                                                clsArray.DockSN = DockSNCol.ToArray();
                                                clsArray.ReplaceTerminalSN = ReplaceTerminalSNCol.ToArray();
                                                clsArray.ReplaceSIMSN = ReplaceSIMSNCol.ToArray();
                                                clsArray.ReplaceDockSN = ReplaceDockSNCol.ToArray();
                                                clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                                clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                                clsArray.ActionMade = ActionMadeCol.ToArray();
                                                clsArray.TimeArrived = TimeArrivedCol.ToArray();
                                                clsArray.TimeStart = TimeStartCol.ToArray();
                                                clsArray.FSRDate = FSRDateCol.ToArray();
                                                clsArray.FSRTime = FSRTimeCol.ToArray();
                                                clsArray.TimeEnd = TimeEndCol.ToArray();
                                                clsArray.Remarks = RemarksCol.ToArray();
                                                clsArray.ProblemReported = ProblemReportedCol.ToArray();
                                                clsArray.ActualProblemReported = ActualProblemReportedCol.ToArray();
                                                clsArray.ActionTaken = ActionTakenCol.ToArray();
                                                clsArray.AnyComments = AnyCommentsCol.ToArray();
                                                clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                                clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();

                                                break;
                                            case "Service TerminalID List":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;                                                    
                                                    TerminalIDCol.Add(element.TerminalID.ToString());                                                    
                                                }
                                                
                                                clsArray.TerminalID = TerminalIDCol.ToArray();                                                
                                                break;
                                            case "Servicing List":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    IRIDNoCol.Add(element.IRIDNo.ToString());
                                                    TAIDNoCol.Add(element.TAIDNo.ToString());
                                                    ClientIDCol.Add(element.ClientID.ToString());
                                                    FEIDCol.Add(element.FEID.ToString());
                                                    MerchantIDCol.Add(element.MerchantID.ToString());
                                                    RegionIDCol.Add(element.RegionID.ToString());
                                                    RegionTypeCol.Add(element.RegionType.ToString());
                                                    RequestNoCol.Add(element.RequestNo);
                                                    ReferenceNoCol.Add(element.ReferenceNo);
                                                    ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                    ServiceDateCol.Add(element.ServiceDate);
                                                    ServiceTimeCol.Add(element.ServiceTime);
                                                    ServiceReqDateCol.Add(element.ServiceReqDate);
                                                    ServiceReqTimeCol.Add(element.ServiceReqTime);
                                                    JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                    ServiceJobTypeDescriptionCol.Add(element.ServiceJobTypeDescription);
                                                    JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                    ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                                    ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                    IRNoCol.Add(element.IRNo);
                                                    MerchantNameCol.Add(element.MerchantName);
                                                    TIDCol.Add(element.TID);
                                                    MIDCol.Add(element.MID);
                                                    TerminalSNCol.Add(element.TerminalSN);
                                                    SIMSerialNoCol.Add(element.SIMSN);
                                                    ReplaceTerminalSNCol.Add(element.ReplaceTerminalSN);
                                                    ReplaceSIMSNCol.Add(element.ReplaceSIMSN);
                                                    FSRNoCol.Add(element.FSRNo.ToString());
                                                    ActionMadeCol.Add(element.ActionMade);
                                                    PrimaryNumCol.Add(element.PrimaryNum.ToString());
                                                    SecondaryNumCol.Add(element.SecondaryNum.ToString());
                                                    ProcessedByCol.Add(element.ProcessedBy);
                                                    ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                    ModifiedByCol.Add(element.ModifiedBy);
                                                    ModifiedDateTimeCol.Add(element.ModifiedDateTime);
                                                    
                                                }

                                                clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                clsArray.IRIDNo = IRIDNoCol.ToArray();
                                                clsArray.TAIDNo = TAIDNoCol.ToArray();
                                                clsArray.ClientID = ClientIDCol.ToArray();
                                                clsArray.MerchantID = MerchantIDCol.ToArray();
                                                clsArray.FEID = FEIDCol.ToArray();
                                                clsArray.RegionID = RegionIDCol.ToArray();
                                                clsArray.RegionType = RegionTypeCol.ToArray();
                                                clsArray.RequestNo = RequestNoCol.ToArray();
                                                clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                                clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                                clsArray.ServiceDate = ServiceDateCol.ToArray();
                                                clsArray.ServiceTime = ServiceTimeCol.ToArray();
                                                clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                                clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                                clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                                clsArray.ServiceJobTypeDescription = ServiceJobTypeDescriptionCol.ToArray();
                                                clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                                clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                                clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                                clsArray.IRNo = IRNoCol.ToArray();
                                                clsArray.MerchantName = MerchantNameCol.ToArray();
                                                clsArray.TID = TIDCol.ToArray();
                                                clsArray.MID = MIDCol.ToArray();
                                                clsArray.TerminalSN = TerminalSNCol.ToArray();
                                                clsArray.SIMSerialNo = SIMSerialNoCol.ToArray();
                                                clsArray.ReplaceTerminalSN = ReplaceTerminalSNCol.ToArray();
                                                clsArray.ReplaceSIMSN = ReplaceSIMSNCol.ToArray();
                                                clsArray.FSRNo = FSRNoCol.ToArray();
                                                clsArray.ActionMade = ActionMadeCol.ToArray();
                                                clsArray.PrimaryNum = PrimaryNumCol.ToArray();
                                                clsArray.SecondaryNum = SecondaryNumCol.ToArray();
                                                clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                                clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                                clsArray.ModifiedBy = ModifiedByCol.ToArray();
                                                clsArray.ModifiedDateTime = ModifiedDateTimeCol.ToArray();
                                                

                                                break;
                                            case "ServiceNo":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());
                                                    IRIDNoCol.Add(element.IRIDNo.ToString());
                                                    TAIDNoCol.Add(element.TAIDNo.ToString());
                                                    ClientIDCol.Add(element.ClientID.ToString());
                                                    MerchantIDCol.Add(element.MerchantID.ToString());
                                                    RequestNoCol.Add(element.RequestNo);
                                                    IRNoCol.Add(element.IRNo);
                                                    ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                    ServiceDateCol.Add(element.ServiceDate);
                                                    ServiceTimeCol.Add(element.ServiceTime);
                                                    ServiceReqDateCol.Add(element.ServiceReqDate);
                                                    ServiceReqTimeCol.Add(element.ServiceReqTime);
                                                    ReferenceNoCol.Add(element.ReferenceNo);
                                                    CustomerNameCol.Add(element.CustomerName);
                                                    CustomerContactNoCol.Add(element.CustomerContactNo);
                                                    RemarksCol.Add(element.Remarks);
                                                    AppVersionCol.Add(element.AppVersion);
                                                    AppCRCCol.Add(element.AppCRC);

                                                }

                                                clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                clsArray.IRIDNo = IRIDNoCol.ToArray();
                                                clsArray.TAIDNo = TAIDNoCol.ToArray();
                                                clsArray.ClientID = ClientIDCol.ToArray();
                                                clsArray.MerchantID = MerchantIDCol.ToArray();
                                                clsArray.RequestNo = RequestNoCol.ToArray();
                                                clsArray.IRNo = IRNoCol.ToArray();
                                                clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                                clsArray.ServiceDate = ServiceDateCol.ToArray();
                                                clsArray.ServiceTime = ServiceTimeCol.ToArray();
                                                clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                                clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                                clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                                clsArray.CustomerName = CustomerNameCol.ToArray();
                                                clsArray.CustomerContactNo = CustomerContactNoCol.ToArray();
                                                clsArray.Remarks = RemarksCol.ToArray();
                                                clsArray.AppVersion = AppVersionCol.ToArray();
                                                clsArray.AppCRC = AppCRCCol.ToArray();

                                                break;
                                            case "Last Servicing Requested By":
                                                foreach (var element in Detail33.data)
                                                {
                                                    clsServicingDetail.RecordFound = true;
                                                    ServiceNoCol.Add(element.ServiceNo.ToString());                                                    
                                                    CustomerNameCol.Add(element.CustomerName);
                                                    CustomerContactNoCol.Add(element.CustomerContactNo);
                                                    RemarksCol.Add(element.Remarks);
                                                }

                                                clsArray.ServiceNo = ServiceNoCol.ToArray();                                                
                                                clsArray.CustomerName = CustomerNameCol.ToArray();
                                                clsArray.CustomerContactNo = CustomerContactNoCol.ToArray();
                                                clsArray.Remarks = RemarksCol.ToArray();

                                                break;
                                            default:
                                                    foreach (var element in Detail33.data)
                                                    {
                                                        clsServicingDetail.RecordFound = true;
                                                        ServiceNoCol.Add(element.ServiceNo.ToString());
                                                        CounterNoCol.Add(element.CounterNo);
                                                        IRNoCol.Add(element.IRNo);
                                                        RequestNoCol.Add(element.RequestNo);
                                                        ServiceDateTimeCol.Add(element.ServiceDateTime);
                                                        ServiceDateCol.Add(element.ServiceDate);
                                                        ServiceTimeCol.Add(element.ServiceTime);
                                                        CustomerNameCol.Add(element.CustomerName);
                                                        CustomerContactNoCol.Add(element.CustomerContactNo);
                                                        RemarksCol.Add(element.Remarks);
                                                        ServiceReqDateCol.Add(element.ServiceReqDate);
                                                        ServiceReqTimeCol.Add(element.ServiceReqTime);
                                                        LastServiceRequestCol.Add(element.LastServiceRequest);
                                                        NewServiceRequestCol.Add(element.NewServiceRequest);
                                                        ReplaceTerminalSNCol.Add(element.ReplaceTerminalSN);
                                                        ReplaceSIMSNCol.Add(element.ReplaceSIMSN);
                                                        ReplaceDockSNCol.Add(element.ReplaceDockSN);
                                                        JobTypeCol.Add(element.JobType.ToString());
                                                        JobTypeDescriptionCol.Add(element.JobTypeDescription);
                                                        JobTypeSubDescriptionCol.Add(element.JobTypeSubDescription);
                                                        JobTypeStatusDescriptionCol.Add(element.JobTypeStatusDescription);
                                                        ReasonIDCol.Add(element.ReasonID.ToString());
                                                        ReasonCodeCol.Add(element.ReasonCode);
                                                        ReasonDescriptionCol.Add(element.ReasonDescription);
                                                        ServiceStatusCol.Add(element.ServiceStatus.ToString());
                                                        ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                        ExpensesIDCol.Add(element.ExpensesID.ToString());
                                                        TExpensesCol.Add(element.TExpenses.ToString());
                                                        ReferenceNoCol.Add(element.ReferenceNo.ToString());
                                                    }

                                                    clsArray.ServiceNo = ServiceNoCol.ToArray();
                                                    clsArray.CounterNo = CounterNoCol.ToArray();
                                                    clsArray.IRNo = IRNoCol.ToArray();
                                                    clsArray.RequestNo = RequestNoCol.ToArray();
                                                    clsArray.ServiceDateTime = ServiceDateTimeCol.ToArray();
                                                    clsArray.ServiceDate = ServiceDateCol.ToArray();
                                                    clsArray.ServiceTime = ServiceTimeCol.ToArray();
                                                    clsArray.CustomerName = CustomerNameCol.ToArray();
                                                    clsArray.CustomerContactNo = CustomerContactNoCol.ToArray();
                                                    clsArray.Remarks = RemarksCol.ToArray();
                                                    clsArray.ServiceReqDate = ServiceReqDateCol.ToArray();
                                                    clsArray.ServiceReqTime = ServiceReqTimeCol.ToArray();
                                                    clsArray.LastServiceRequest = LastServiceRequestCol.ToArray();
                                                    clsArray.NewServiceRequest = NewServiceRequestCol.ToArray();
                                                    clsArray.ReplaceTerminalSN = ReplaceTerminalSNCol.ToArray();
                                                    clsArray.ReplaceSIMSN = ReplaceSIMSNCol.ToArray();
                                                    clsArray.ReplaceDockSN = ReplaceDockSNCol.ToArray();
                                                    clsArray.JobType = JobTypeCol.ToArray();
                                                    clsArray.JobTypeDescription = JobTypeDescriptionCol.ToArray();
                                                    clsArray.JobTypeSubDescription = JobTypeSubDescriptionCol.ToArray();
                                                    clsArray.JobTypeStatusDescription = JobTypeStatusDescriptionCol.ToArray();
                                                    clsArray.ReasonID = ReasonIDCol.ToArray();
                                                    clsArray.ReasonCode = ReasonCodeCol.ToArray();
                                                    clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();
                                                    clsArray.ServiceStatus = ServiceStatusCol.ToArray();
                                                    clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                                    clsArray.ExpensesID = ExpensesIDCol.ToArray();
                                                    clsArray.TExpenses = TExpensesCol.ToArray();
                                                    clsArray.ReferenceNo = ReferenceNoCol.ToArray();

                                                break;
                                        }
                                        
                                        break;
                                }
                                break;

                            case "Terminal Detail":
                                TerminalDetailOnline Detail34 = JsonConvert.DeserializeObject<TerminalDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsTerminal.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail34.data)
                                        {
                                            clsTerminal.RecordFound = true;
                                            clsTerminal.ClassTerminalID = int.Parse(element.TerminalID.ToString());
                                            clsTerminal.ClassTerminalSN = element.SerialNo;
                                            clsTerminal.ClassTerminalTypeID = element.TerminalTypeID;
                                            clsTerminal.ClassTerminalModelID = element.TerminalModelID;
                                            clsTerminal.ClassTerminalBrandID = element.TerminalBrandID;
                                            clsTerminal.ClassTerminalType = element.TerminalType;
                                            clsTerminal.ClassTerminalModel = element.TerminalModel;
                                            clsTerminal.ClassTerminalBrand = element.TerminalBrand;
                                            clsTerminal.ClassTerminalStatus = int.Parse(element.TerminalStatus.ToString());
                                            clsTerminal.ClassTerminalStatusDescription = element.TerminalStatusDescription;
                                        }

                                        break;
                                    case "View":

                                        if (SearchBy.Equals("Service History"))
                                        {
                                            foreach (var element in Detail34.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                TerminalIDCol.Add(element.TerminalID.ToString());
                                                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                                                ParticularNameCol.Add(element.ParticularName);
                                                RemarksCol.Add(element.Remarks);
                                                DateCol.Add(element.Date);
                                                TimeCol.Add(element.Time);
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                IRNoCol.Add(element.IRNo);
                                                TIDCol.Add(element.TID);
                                                MIDCol.Add(element.MID);


                                            }

                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.ServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.Date = DateCol.ToArray();
                                            clsArray.Time = TimeCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.TID = TIDCol.ToArray();
                                            clsArray.MID = MIDCol.ToArray();
                                        }
                                        else if (SearchBy.Equals("Release Movement Master"))
                                        {
                                            foreach (var element in Detail34.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                TransNoCol.Add(element.TransNo.ToString());
                                                TransDateCol.Add(element.TransDate);
                                                TransTimeCol.Add(element.TransTime);
                                                ReleaseDateCol.Add(element.ReleaseDate);
                                                RequestNoCol.Add(element.RequestNo);
                                                ReferenceNoCol.Add(element.ReferenceNo);
                                                RemarksCol.Add(element.Remarks);
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                ModifiedByCol.Add(element.ModifiedBy);
                                                ModifiedDateTimeCol.Add(element.ModifiedDateTime);
                                                UserIDCol.Add(element.UserID.ToString());
                                                FromLocationIDCol.Add(element.FromLocationID.ToString());
                                                FromLocationCol.Add(element.FromLocation);
                                                ToLocationIDCol.Add(element.ToLocationID.ToString());
                                                ToLocationCol.Add(element.ToLocation);
                                                
                                            }

                                            clsArray.TransNo = TransNoCol.ToArray();
                                            clsArray.TransDate = TransDateCol.ToArray();
                                            clsArray.TransTime = TransTimeCol.ToArray();
                                            clsArray.ReleaseDate = ReleaseDateCol.ToArray();
                                            clsArray.RequestNo = RequestNoCol.ToArray();
                                            clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.ModifiedBy = ModifiedByCol.ToArray();
                                            clsArray.ModifiedDateTime = ModifiedDateTimeCol.ToArray();
                                            clsArray.UserID = UserIDCol.ToArray();
                                            clsArray.FromLocationID = FromLocationIDCol.ToArray();
                                            clsArray.FromLocation = FromLocationCol.ToArray();
                                            clsArray.ToLocationID = ToLocationIDCol.ToArray();
                                            clsArray.ToLocation = ToLocationCol.ToArray();
                                        }

                                        break;
                                }

                                break;
                            case "Particular Detail":
                                ParticularDetailOnline Detail35 = JsonConvert.DeserializeObject<ParticularDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsParticular.RecordFound = false;
                                switch (StatementType)
                                {
                                    case "Search":
                                        if (SearchBy.CompareTo("Client Dashboard Detail") == 0)
                                        {
                                            foreach (var element in Detail35.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                clsParticular.ClassParticularID = int.Parse(element.ParticularID.ToString());
                                                //clsParticular.ClassParticularName = element.ParticularName;
                                                clsParticular.ClassParticularUserName = element.ParticularUserName;
                                                clsParticular.ClassParticularUserKey = element.ParticularUserKey;

                                            }
                                        }
                                        else if (SearchBy.CompareTo("Particular Leave Assignment") == 0)
                                        {
                                            foreach (var element in Detail35.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                LeaveNoCol.Add(element.LeaveNo.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                LeaveTypeIDCol.Add(element.LeaveTypeID.ToString());
                                                CreditLimitCol.Add(element.CreditLimit.ToString());
                                                LeaveCreditCol.Add(element.LeaveCredit.ToString());
                                                CodeCol.Add(element.Code);
                                                DescriptionCol.Add(element.Description);
                                                RemarksCol.Add(element.Remarks);
                                            }

                                            clsArray.LeaveNo = LeaveNoCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.LeaveTypeID = LeaveTypeIDCol.ToArray();
                                            clsArray.LeaveTypeCreditLimit = CreditLimitCol.ToArray();
                                            clsArray.LeaveCredit = LeaveCreditCol.ToArray();
                                            clsArray.LeaveTypeCode = CodeCol.ToArray();
                                            clsArray.LeaveTypeDesc = DescriptionCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                        }
                                        else if (SearchBy.CompareTo("Particular Leave Movement") == 0)
                                        {
                                            foreach (var element in Detail35.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                LeaveNoCol.Add(element.LeaveNo.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                LeaveTypeIDCol.Add(element.LeaveTypeID.ToString());
                                                DateFromCol.Add(element.DateFrom.ToString());
                                                DateToCol.Add(element.DateTo.ToString());
                                                DurationCol.Add(element.Duration.ToString());
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                ModifiedByCol.Add(element.ModifiedBy);
                                                ModifiedDateTimeCol.Add(element.ModifiedDateTime);
                                                DateTypeCol.Add(element.DateType);
                                                RemarksCol.Add(element.Remarks);
                                                isActiveCol.Add(element.isActive.ToString());
                                                LeaveCodeCol.Add(element.LeaveCode);
                                                LeaveDescCol.Add(element.LeaveDesc);
                                                ReasonIDCol.Add(element.ReasonID.ToString());
                                                ReasonCodeCol.Add(element.ReasonCode);
                                                ReasonDescriptionCol.Add(element.ReasonDesc);

                                            }

                                            clsArray.LeaveNo = LeaveNoCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.LeaveTypeID = LeaveTypeIDCol.ToArray();
                                            clsArray.DateFrom = DateFromCol.ToArray();
                                            clsArray.DateTo = DateToCol.ToArray();
                                            clsArray.Duration = DurationCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.ModifiedBy = ModifiedByCol.ToArray();
                                            clsArray.ModifiedDateTime = ModifiedDateTimeCol.ToArray();
                                            clsArray.DateType = DateTypeCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                            clsArray.isActive = isActiveCol.ToArray();
                                            clsArray.LeaveTypeCode = LeaveCodeCol.ToArray();
                                            clsArray.LeaveTypeDesc = LeaveDescCol.ToArray();
                                            clsArray.ReasonID = ReasonIDCol.ToArray();
                                            clsArray.ReasonCode = ReasonCodeCol.ToArray();
                                            clsArray.ReasonDescription = ReasonDescriptionCol.ToArray();

                                        }
                                        else if (SearchBy.CompareTo("Particular Work Arrangement") == 0)
                                        {
                                            foreach (var element in Detail35.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                WorkArrangementIDCol.Add(element.WorkArrangementID.ToString());
                                                WorkTypeIDCol.Add(element.WorkTypeID.ToString());
                                                CodeCol.Add(element.WorkTypeCode);
                                                DescriptionCol.Add(element.WorkTypeDesc);
                                                DateFromCol.Add(element.DateFrom);
                                                DateToCol.Add(element.DateTo);
                                                DurationCol.Add(element.Duration.ToString());
                                                DateTypeCol.Add(element.DateType);
                                                RemarksCol.Add(element.Remarks);
                                            }

                                            clsArray.WorkArrangementID = WorkArrangementIDCol.ToArray();
                                            clsArray.WorkTypeID = WorkTypeIDCol.ToArray();
                                            clsArray.Code = CodeCol.ToArray();
                                            clsArray.Description = DescriptionCol.ToArray();
                                            clsArray.DateFrom = DateFromCol.ToArray();
                                            clsArray.DateTo = DateToCol.ToArray();
                                            clsArray.Duration = DurationCol.ToArray();
                                            clsArray.DateType = DateTypeCol.ToArray();
                                            clsArray.Remarks = RemarksCol.ToArray();
                                        }
                                        else
                                        {
                                            foreach (var element in Detail35.data)
                                            {
                                                clsParticular.RecordFound = true;
                                                clsParticular.ClassParticularID = int.Parse(element.ParticularID.ToString());
                                                clsParticular.ClassParticularTypeID = int.Parse(element.ParticularTypeID.ToString());
                                                clsParticular.ClassParticularDescription = element.ParticularDescription.ToString();
                                                clsParticular.ClassParticularName = element.ParticularName.ToString();
                                                clsParticular.ClassAddress = element.Address.ToString();
                                                clsParticular.ClassAddress2 = element.Address2.ToString();
                                                clsParticular.ClassAddress3 = element.Address3.ToString();
                                                clsParticular.ClassAddress4 = element.Address4.ToString();
                                                clsParticular.ClassContactPerson = element.ContactPerson.ToString();
                                                clsParticular.ClassTelNo = element.TelNo.ToString();
                                                clsParticular.ClassMobile = element.Mobile.ToString();
                                                clsParticular.ClassFax = element.Fax.ToString();
                                                clsParticular.ClassEmail = element.Email.ToString();
                                                clsParticular.ClassContractTerms = element.ContractTerms.ToString();
                                                clsParticular.ClassRegionID = int.Parse(element.RegionID.ToString());
                                                clsParticular.ClassRegionType = int.Parse(element.RegionType.ToString());
                                                clsParticular.ClassRegion = element.Region.ToString();
                                                clsParticular.ClassProvince = element.Province.ToString();

                                                clsParticular.ClassEmploymentStatus = element.EmploymentStatus.ToString();
                                                clsParticular.ClassDepartmentID = int.Parse(element.DepartmentID.ToString());
                                                clsParticular.ClassDepartment = element.Department.ToString();
                                                clsParticular.ClassPositionID = int.Parse(element.PositionID.ToString());
                                                clsParticular.ClassPosition = element.Position.ToString();
                                                clsParticular.ClassCode = element.Code.ToString();

                                                clsParticular.ClassComputerName = element.ComputerName;
                                                clsParticular.ClassisActive = (element.isActive > 0 ? true : false);
                                                clsParticular.ClassisWorkArrangement = (element.isWorkArrangement > 0 ? true : false);
                                                clsParticular.ClassisTimeSheet = (element.isTimeSheet > 0 ? true : false);
                                                clsParticular.ClassisAppVersion = (element.isAppVersion > 0 ? true : false);

                                                // POS Rental
                                                clsParticular.ClassRentalType = int.Parse(element.RentalType.ToString());
                                                clsParticular.ClassRentalTerms = int.Parse(element.RentalTerms.ToString());
                                                clsParticular.ClassAccountNo = element.AccountNo.ToString();
                                                clsParticular.ClassCustomerNo = element.CustomerNo.ToString();

                                            }
                                        }
                                        
                                        break;
                                }

                                break;

                            case "Expenses":
                                ExpensesDetailOnline Detail36 = JsonConvert.DeserializeObject<ExpensesDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsExpenses.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail36.data)
                                        {
                                            clsExpenses.RecordFound = true;
                                            clsExpenses.ClassExpensesID = element.ExpensesID;
                                            clsExpenses.ClassDescription = element.Description;
                                        }
                                        break;
                                    case "View":                                        
                                        foreach (var element in Detail36.data)
                                        {
                                            clsExpenses.RecordFound = true;
                                            IDCol.Add(element.ExpensesID.ToString());
                                            DescriptionCol.Add(element.Description);                                            
                                        }

                                        clsArray.ExpensesID = IDCol.ToArray();                                        
                                        clsArray.ExpensesDescription = DescriptionCol.ToArray();
                                        break;
                                }
                                break;
                            case "ERM":
                                FSRDetailOnline Detail37 = JsonConvert.DeserializeObject<FSRDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsFSR.RecordFound = false;

                                FSRNoCol.Clear();
                                FSRIDCol.Clear();
                                MerchantCol.Clear();
                                MIDCol.Clear();
                                TIDCol.Clear();
                                TimeArrivedCol.Clear();
                                TimeStartCol.Clear();
                                FSRCol.Clear();
                                FSRDateCol.Clear();
                                FSRTimeCol.Clear();
                                MerchantContactNoCol.Clear();
                                MerchantRepresentativeCol.Clear();
                                DateFromCol.Clear();
                                DateToCol.Clear();
                                NRICCol.Clear();
                                AdditionalInformationCol.Clear();

                                if (SearchBy.CompareTo("ERM Temp Detail Date Filter") == 0)
                                {
                                    foreach (var element in Detail37.data)
                                    {
                                        clsFSR.RecordFound = true;
                                        DateFromCol.Add(element.DateFrom);
                                        DateToCol.Add(element.DateTo);
                                    }

                                    clsArray.DateFrom = DateFromCol.ToArray();
                                    clsArray.DateTo = DateToCol.ToArray();
                                }
                                else
                                {
                                    foreach (var element in Detail37.data)
                                    {
                                        clsFSR.RecordFound = true;
                                        FSRNoCol.Add(element.FSRNo.ToString());
                                        NoCol.Add(element.No.ToString());
                                        MerchantCol.Add(element.Merchant);
                                        MIDCol.Add(element.MID.Trim());
                                        TIDCol.Add(element.TID);
                                        InvoiceNoCol.Add(element.InvoiceNo);
                                        BatchNoCol.Add(element.BatchNo);
                                        FSRCol.Add(element.FSR);
                                        FSRDateCol.Add(element.FSRDate);
                                        FSRTimeCol.Add(element.FSRTime);
                                        TxnAmtCol.Add(element.TxnAmt);                                        
                                        AuthCodeCol.Add(element.AuthCode);
                                        RefNoCol.Add(element.RefNo);
                                        MerchantContactNoCol.Add(element.MerchantContactNo);
                                        MerchantRepresentativeCol.Add(element.MerchantRepresentative);
                                        NRICCol.Add(element.NRIC);
                                        AdditionalInformationCol.Add(element.AdditionalInformation);

                                    }
                                    clsArray.FSRNo = FSRNoCol.ToArray();
                                    clsArray.No = NoCol.ToArray();
                                    clsArray.MerchantName = MerchantCol.ToArray();
                                    clsArray.MID = MIDCol.ToArray();
                                    clsArray.TID = TIDCol.ToArray();
                                    clsArray.InvoiceNo = InvoiceNoCol.ToArray();
                                    clsArray.BatchNo = BatchNoCol.ToArray();
                                    clsArray.FSR = FSRCol.ToArray();
                                    clsArray.FSRDate = FSRDateCol.ToArray();
                                    clsArray.FSRTime = FSRTimeCol.ToArray();
                                    clsArray.TxnAmt = TxnAmtCol.ToArray();
                                    clsArray.AuthCode = AuthCodeCol.ToArray();
                                    clsArray.RefNo = RefNoCol.ToArray();
                                    clsArray.MerchantContactNo = MerchantContactNoCol.ToArray();
                                    clsArray.MerchantRepresentative = MerchantRepresentativeCol.ToArray();
                                    clsArray.NRIC = NRICCol.ToArray();
                                    clsArray.AdditionalInformation = AdditionalInformationCol.ToArray();

                                }
                                
                                break;

                            case "Type":
                                TypeDetailOnline Detail38 = JsonConvert.DeserializeObject<TypeDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format
                                clsType.RecordFound = false;

                                IDCol.Clear();
                                CodeCol.Clear();
                                DescriptionCol.Clear();
                                RemarksCol.Clear();
                                QueryStringCol.Clear();
                                TypeValueCol.Clear();
                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail38.data)
                                        {
                                            clsType.RecordFound = true;
                                            clsType.ClassTypeID = element.TypeID;
                                            clsType.Description = element.Description;
                                        }
                                        break;
                                    case "View":
                                        if (SearchBy.Equals("WorkType"))
                                        {
                                            foreach (var element in Detail38.data)
                                            {
                                                clsType.RecordFound = true;
                                                IDCol.Add(element.WorkTypeID.ToString());
                                                CodeCol.Add(element.Code);
                                                DescriptionCol.Add(element.Description);
                                                RemarksCol.Add(element.Remarks);
                                                QueryStringCol.Add(element.QueryString);
                                                TypeValueCol.Add(element.TypeValue.ToString());

                                            }

                                            clsArray.WorkTypeID = IDCol.ToArray();
                                            clsArray.Code = CodeCol.ToArray();
                                            clsArray.Description = DescriptionCol.ToArray();
                                            clsArray.TypeRemarks = RemarksCol.ToArray();
                                            clsArray.TypeQueryString = QueryStringCol.ToArray();
                                            clsArray.TypeValue = TypeValueCol.ToArray();

                                        }
                                        else if ((SearchBy.Equals("Type")) ||
                                                (SearchBy.Equals("Type List")) ||
                                                (SearchBy.Equals("Rental Fee List")) ||
                                                (SearchBy.Equals("All Type")))
                                        {
                                            foreach (var element in Detail38.data)
                                            {
                                                clsType.RecordFound = true;
                                                IDCol.Add(element.TypeID.ToString());
                                                CodeCol.Add(element.Code);
                                                DescriptionCol.Add(element.Description);
                                                RemarksCol.Add(element.Remarks);
                                                QueryStringCol.Add(element.QueryString);
                                                TypeValueCol.Add(element.TypeValue.ToString());

                                                clsSearch.ClassCarrierList += element.Description; // carrier list
                                            }

                                            clsArray.TypeID = IDCol.ToArray();
                                            clsArray.Code = CodeCol.ToArray();
                                            clsArray.TypeDescription = DescriptionCol.ToArray();
                                            clsArray.TypeRemarks = RemarksCol.ToArray();
                                            clsArray.TypeQueryString = QueryStringCol.ToArray();
                                            clsArray.TypeValue = TypeValueCol.ToArray();

                                        }                                       
                                        else
                                        {
                                            foreach (var element in Detail38.data)
                                            {
                                                clsType.RecordFound = true;
                                                IDCol.Add(element.TypeID.ToString());
                                                CodeCol.Add(element.Code);
                                                DescriptionCol.Add(element.Description);
                                                RemarksCol.Add(element.Remarks);
                                                QueryStringCol.Add(element.QueryString);

                                                clsSearch.ClassCarrierList += element.Description; // carrier list
                                            }

                                            clsArray.TypeID = IDCol.ToArray();
                                            clsArray.Code = CodeCol.ToArray();
                                            clsArray.TypeDescription = DescriptionCol.ToArray();
                                            clsArray.TypeRemarks = RemarksCol.ToArray();
                                            clsArray.TypeQueryString = QueryStringCol.ToArray();
                                        }
                                        
                                        break;
                                }
                                break;

                            case "Get Total":
                                TerminalDetailOnline Detail39 = JsonConvert.DeserializeObject<TerminalDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format

                                Debug.WriteLine("MaintenanceType=" + MaintenanceType);

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail39.data)
                                        {
                                            Debug.WriteLine("clsTerminal.element=" + element.TerminalCount);

                                            if (MaintenanceType.Equals("Get Count"))
                                                clsTerminal.ClassTerminalCount = int.Parse(element.TerminalCount);

                                            if (MaintenanceType.Equals("Get Total"))
                                                clsTerminal.ClassTerminalTotal = double.Parse(element.TerminalCount);
                                        }

                                        break;
                                }

                                Debug.WriteLine("clsTerminal.ClassTerminalCount=" + clsTerminal.ClassTerminalCount);
                                Debug.WriteLine("clsTerminal.ClassTerminalTotal=" + clsTerminal.ClassTerminalTotal);
                                break;

                            case "Holiday":
                                CollectionDataDetailOnline Detail40 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsHoliday.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail40.data)
                                        {
                                            clsHoliday.RecordFound = true;
                                            clsHoliday.ClassHolidayID = element.HolidayID;
                                            clsHoliday.ClassHolidayDate = element.HolidayDate;
                                            clsHoliday.ClassDescription = element.Description;
                                            clsHoliday.ClassisActive = element.isActive;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail40.data)
                                        {
                                            clsHoliday.RecordFound = true;
                                            IDCol.Add(element.HolidayID.ToString());
                                            HolidayDateCol.Add(element.HolidayDate);
                                            DescriptionCol.Add(element.Description);                                            
                                            isActiveCol.Add(element.isActive.ToString());
                                        }

                                        clsArray.HolidayID = IDCol.ToArray();
                                        clsArray.HolidayDate = HolidayDateCol.ToArray();
                                        clsArray.HolidayDesc = DescriptionCol.ToArray();
                                        clsArray.HolidayisActive = isActiveCol.ToArray();
                                        break;
                                }
                                break;

                            case "LeaveType":
                                CollectionDataDetailOnline Detail41 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsLeaveType.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail41.data)
                                        {
                                            clsLeaveType.RecordFound = true;
                                            clsLeaveType.ClassLeaveTypeID = element.LeaveTypeID;
                                            clsLeaveType.ClassCode = element.Code;
                                            clsLeaveType.ClassDescription = element.Description;
                                            clsLeaveType.ClassCreditLimit = element.CreditLimit;
                                            clsLeaveType.ClassisActive = element.isActive;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail41.data)
                                        {
                                            clsLeaveType.RecordFound = true;
                                            IDCol.Add(element.LeaveTypeID.ToString());
                                            CodeCol.Add(element.Code);
                                            DescriptionCol.Add(element.Description);
                                            CreditLimitCol.Add(element.CreditLimit.ToString());
                                            isActiveCol.Add(element.isActive.ToString());
                                        }

                                        clsArray.LeaveTypeID = IDCol.ToArray();
                                        clsArray.LeaveTypeCode = CodeCol.ToArray();
                                        clsArray.LeaveTypeDesc = DescriptionCol.ToArray();
                                        clsArray.LeaveTypeCreditLimit = CreditLimitCol.ToArray();
                                        clsArray.LeaveTypeisActive = isActiveCol.ToArray();
                                        break;
                                }
                                break;

                            case "Department":
                                CollectionDataDetailOnline Detail42 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsDepartment.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail42.data)
                                        {
                                            clsDepartment.RecordFound = true;
                                            clsDepartment.ClassDepartmentID = element.DepartmentID;
                                            clsDepartment.ClassDescription = element.Description;
                                            clsDepartment.ClassisActive = element.isActive;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail42.data)
                                        {
                                            clsDepartment.RecordFound = true;
                                            IDCol.Add(element.DepartmentID.ToString());
                                            DescriptionCol.Add(element.Description);
                                            isActiveCol.Add(element.isActive.ToString());
                                        }

                                        clsArray.DepartmentID = IDCol.ToArray();
                                        clsArray.DepartmentDesc = DescriptionCol.ToArray();
                                        clsArray.DepartmentisActive = isActiveCol.ToArray();
                                        break;
                                }
                                break;

                            case "Position":
                                CollectionDataDetailOnline Detail43 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsPosition.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail43.data)
                                        {
                                            clsPosition.RecordFound = true;
                                            clsPosition.ClassPositionID = element.PositionID;
                                            clsPosition.ClassDescription = element.Description;
                                            clsPosition.ClassisActive = element.isActive;
                                        }
                                        break;
                                    case "View":
                                        foreach (var element in Detail43.data)
                                        {
                                            clsPosition.RecordFound = true;
                                            IDCol.Add(element.PositionID.ToString());
                                            DescriptionCol.Add(element.Description);
                                            isActiveCol.Add(element.isActive.ToString());
                                        }

                                        clsArray.PositionID = IDCol.ToArray();
                                        clsArray.PositionDesc = DescriptionCol.ToArray();
                                        clsArray.PositionisActive = isActiveCol.ToArray();
                                        break;
                                }
                                break;

                            case "TimeSheet":
                                CollectionDataDetailOnline Detail44 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsSearch.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail44.data)
                                        {
                                            clsSearch.RecordFound = true;
                                        }
                                        break;
                                    case "View":

                                        if (SearchBy.Equals("TimeSheet"))
                                        {
                                            foreach (var element in Detail44.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                TimeSheetIDCol.Add(element.TimeSheetID.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                TimeSheetDateCol.Add(element.TSDate);
                                                TimeInCol.Add(element.TimeIn);
                                                TimeOutCol.Add(element.TimeOut);
                                                THoursCol.Add(element.THours);
                                                TTimeCol.Add(element.TTime);
                                                TimeStatusCol.Add(element.TimeStatus);
                                                PositionIDCol.Add(element.PositionID.ToString());
                                                PositionCol.Add(element.Position);
                                                DepartmentIDCol.Add(element.DepartmentID.ToString());
                                                DepartmentCol.Add(element.Department);
                                                WorkTypeIDCol.Add(element.WorkTypeID.ToString());
                                                WorkTypeCol.Add(element.WorkType);
                                                EmploymentStatusCol.Add(element.EmploymentStatus);
                                            }

                                            clsArray.TimeSheetID = TimeSheetIDCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.TimeSheetDate = TimeSheetDateCol.ToArray();
                                            clsArray.TimeIn = TimeInCol.ToArray();
                                            clsArray.TimeOut = TimeOutCol.ToArray();
                                            clsArray.THours = THoursCol.ToArray();
                                            clsArray.TTime = TTimeCol.ToArray();
                                            clsArray.TimeStatus = TimeStatusCol.ToArray();
                                            clsArray.PositionID = PositionIDCol.ToArray();
                                            clsArray.PositionDesc = PositionCol.ToArray();
                                            clsArray.DepartmentID = DepartmentIDCol.ToArray();
                                            clsArray.DepartmentDesc = DepartmentCol.ToArray();
                                            clsArray.WorkTypeID = WorkTypeIDCol.ToArray();
                                            clsArray.WorkType = WorkTypeCol.ToArray();
                                            clsArray.EmploymentStatus = EmploymentStatusCol.ToArray();

                                        }
                                        else
                                        {
                                            foreach (var element in Detail44.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                TimeSheetIDCol.Add(element.TimeSheetID.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                TimeSheetDateCol.Add(element.TimeSheetDate);
                                                TimeInCol.Add(element.TimeIn);
                                                TimeOutCol.Add(element.TimeOut);
                                                ComputerNameCol.Add(element.ComputerName);
                                                LocalIPCol.Add(element.LocalIP);
                                                TerminalIDCol.Add(element.TerminalID);
                                                TerminalNameCol.Add(element.TerminalName);
                                            }

                                            clsArray.TimeSheetID = TimeSheetIDCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.TimeSheetDate = TimeSheetDateCol.ToArray();
                                            clsArray.TimeIn = TimeInCol.ToArray();
                                            clsArray.TimeOut = TimeOutCol.ToArray();
                                            clsArray.ComputerName = ComputerNameCol.ToArray();
                                            clsArray.LocalIP = LocalIPCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalName = TerminalNameCol.ToArray();
                                        }

                                        break;
                                }
                                break;

                            case "Privacy":
                                CollectionDataDetailOnline Detail45 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsSearch.RecordFound = false;

                                switch (StatementType)
                                {                                    
                                    case "View":
                                        foreach (var element in Detail45.data)
                                        {
                                            clsSearch.RecordFound = true;
                                            PrivacyIDCol.Add(element.PrivacyID.ToString());
                                            FormCol.Add(element.Form);
                                            DescriptionCol.Add(element.Description);
                                            isAddCol.Add(element.isAdd.ToString());
                                            isDeleteCol.Add(element.isDelete.ToString());
                                            isUpdateCol.Add(element.isUpdate.ToString());
                                            isViewCol.Add(element.isView.ToString());
                                            isPrintCol.Add(element.isPrint.ToString());
                                            isCheckedCol.Add(element.isChecked.ToString());
                                        }

                                        clsArray.PrivacyID = PrivacyIDCol.ToArray();
                                        clsArray.Form = FormCol.ToArray();
                                        clsArray.Description = DescriptionCol.ToArray();
                                        clsArray.isAdd = isAddCol.ToArray();
                                        clsArray.isDelete = isDeleteCol.ToArray();
                                        clsArray.isUpdate = isUpdateCol.ToArray();
                                        clsArray.isView = isViewCol.ToArray();
                                        clsArray.isPrint = isPrintCol.ToArray();
                                        clsArray.isChecked = isCheckedCol.ToArray();
                                        break;
                                }
                                break;

                            case "Advance Detail":
                                CollectionDataDetailOnline Detail46 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsSearch.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "View":

                                        // Clear
                                        IDCol.Clear();
                                        TerminalIDCol.Clear();
                                        TerminalNameCol.Clear();
                                        CountryCol.Clear();
                                        ParticularNameCol.Clear();
                                        DescriptionCol.Clear();
                                        
                                        if (SearchBy.Equals("TimeSheet Terminal"))
                                        {  
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                TerminalIDCol.Add(element.TerminalID);
                                                TerminalNameCol.Add(element.TerminalName);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.TerminalID = TerminalIDCol.ToArray();
                                            clsArray.TerminalName = TerminalNameCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Country"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                CountryCol.Add(element.Country);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.Country = CountryCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Particular"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                ParticularNameCol.Add(element.ParticularName);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Invoice Master") || SearchBy.Equals("Invoice Master Range"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                InvoiceIDCol.Add(element.InvoiceID.ToString());
                                                ParticularIDCol.Add(element.ParticularID.ToString());
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                InvoiceNoCol.Add(element.InvoiceNo);
                                                AccountNoCol.Add(element.AccountNo);
                                                CustomerNoCol.Add(element.CustomerNo);
                                                ParticularNameCol.Add(element.ParticularName);
                                                InvoiceDateCol.Add(element.InvoiceDate);
                                                ReferenceNoCol.Add(element.ReferenceNo);
                                                DateCoveredFromCol.Add(element.DateCoveredFrom);
                                                DateCoveredToCol.Add(element.DateCoveredTo);
                                                DateDueCol.Add(element.DateDue);
                                                TAmtDueCol.Add(element.TAmtDue.ToString());
                                                ProcessedByCol.Add(element.ProcessedBy);
                                                ProcessedDateTimeCol.Add(element.ProcessedDateTime);
                                                ModifiedByCol.Add(element.ModifiedBy);
                                                ModifiedDateTimeCol.Add(element.ModifiedDateTime);
                                                ModeOfPaymentCol.Add(element.ModeOfPayment.ToString());
                                                NoteToCustomerCol.Add(element.NoteToCustomer);
                                                NoteToSelfCol.Add(element.NoteToSelf);

                                            }

                                            clsArray.InvoiceID = InvoiceIDCol.ToArray();
                                            clsArray.ParticularID = ParticularIDCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.InvoiceNo = InvoiceNoCol.ToArray();
                                            clsArray.AccountNo = AccountNoCol.ToArray();
                                            clsArray.CustomerNo = CustomerNoCol.ToArray();
                                            clsArray.ParticularName = ParticularNameCol.ToArray();
                                            clsArray.InvoiceDate = InvoiceDateCol.ToArray();
                                            clsArray.ReferenceNo = ReferenceNoCol.ToArray();
                                            clsArray.DateCoveredFrom = DateCoveredFromCol.ToArray();
                                            clsArray.DateCoveredTo = DateCoveredToCol.ToArray();
                                            clsArray.DateDue = DateDueCol.ToArray();
                                            clsArray.TAmtDue = TAmtDueCol.ToArray();
                                            clsArray.ProcessedBy = ProcessedByCol.ToArray();
                                            clsArray.ProcessedDateTime = ProcessedDateTimeCol.ToArray();
                                            clsArray.ModifiedBy = ModifiedByCol.ToArray();
                                            clsArray.ModifiedDateTime = ModifiedDateTimeCol.ToArray();
                                            clsArray.ModeOfPayment = ModeOfPaymentCol.ToArray();
                                            clsArray.NoteToCustomer = NoteToCustomerCol.ToArray();
                                            clsArray.NoteToSelf = NoteToSelfCol.ToArray();
                                            
                                        }

                                        else if (SearchBy.Equals("Profile Mapping") || SearchBy.Equals("eFSR Changes Mapping") || SearchBy.Equals("eFSR Changes Service"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                MapIDCol.Add(element.MapID.ToString());
                                                Detail_InfoCol.Add(element.detail_info);
                                            }

                                            clsArray.MapID = MapIDCol.ToArray();
                                            clsArray.detail_info = Detail_InfoCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Service TerminalSN History List") || SearchBy.Equals("Service SIMSN History List"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                Detail_InfoCol.Add(element.detail_info);
                                            }

                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.detail_info = Detail_InfoCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Mobile List"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.MobileID.ToString());
                                                Detail_InfoCol.Add(element.detail_info);
                                            }

                                            clsArray.MobileID = IDCol.ToArray();
                                            clsArray.detail_info = Detail_InfoCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Generate FSR List"))
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                FSRNoCol.Add(element.FSRNo.ToString());
                                                ServiceNoCol.Add(element.ServiceNo.ToString());
                                                IRIDNoCol.Add(element.IRIDNo.ToString());
                                                IRNoCol.Add(element.IRNo);
                                                MerchantIDCol.Add(element.MerchantID.ToString());
                                                MerchantNameCol.Add(element.MerchantName);
                                                MerchantEmailCol.Add(element.MerchantEmail);
                                                ServiceJobTypeDescriptionCol.Add(element.ServiceJobTypeDescription);
                                                FENameCol.Add(element.FEName);
                                                FEEmailCol.Add(element.FEEmail);
                                                FSRDateCol.Add(element.FSRDate);
                                            }

                                            clsArray.FSRNo = FSRNoCol.ToArray();
                                            clsArray.ServiceNo = ServiceNoCol.ToArray();
                                            clsArray.IRIDNo = IRIDNoCol.ToArray();
                                            clsArray.IRNo = IRNoCol.ToArray();
                                            clsArray.MerchantID = MerchantIDCol.ToArray();
                                            clsArray.MerchantName = MerchantNameCol.ToArray();
                                            clsArray.MerchantEmail = MerchantEmailCol.ToArray();
                                            clsArray.ServiceJobTypeDescription = ServiceJobTypeDescriptionCol.ToArray();
                                            clsArray.FEName = FENameCol.ToArray();
                                            clsArray.FEEmail = FEEmailCol.ToArray();
                                            clsArray.FSRDate = FSRDateCol.ToArray();
                                        }

                                        else if (SearchBy.Equals("Application Info List") || 
                                            SearchBy.Equals("Diagnostic Master List") || 
                                            SearchBy.Equals("Diagnostic Detail List") || 
                                            SearchBy.Equals("Merchant Service Status List") ||
                                            SearchBy.Equals("Particular By Position Type List") ||
                                            SearchBy.Equals("Whos Online") ||
                                            SearchBy.Equals("Whos eFSR Online") ||
                                            SearchBy.Equals("Duplicate SN Merchant List") ||                                            
                                            SearchBy.Equals("Stock Movement Detail List") ||
                                            SearchBy.Equals("Failed Service List") ||
                                            SearchBy.Equals("Type List") ||
                                            SearchBy.Equals("Reason List") ||
                                            SearchBy.Equals("Region Service Summary") ||
                                            SearchBy.Equals("Import Master List") ||
                                            SearchBy.Equals("MSP Master List") ||
                                            SearchBy.Equals("MSP Detail List") ||
                                            SearchBy.Equals("Unclosed Ticket List") ||
                                            SearchBy.Equals("Helpdesk-Details") ||
                                            SearchBy.Equals("Helpdesk-Master") ||
                                            SearchBy.Equals("Helpdesk-JobType") ||
                                            SearchBy.Equals("Mobile Terminal List") ||
                                            SearchBy.Equals("Whos Dashboard Online") ||
                                            SearchBy.Equals("Helpdesk Problem List") ||
                                            SearchBy.Equals("Report Group") ||
                                            SearchBy.Equals("Service Reversal-JobType") ||
                                            SearchBy.Equals("ERM Settlement Report-Per Month") ||
                                            SearchBy.Equals("ERM Settlement Report-Per Trans Type") ||
                                            SearchBy.Equals("ERM Settlement Report-Per Top Sales") ||
                                            SearchBy.Equals("ERM Settlement Report-Per Qtr") ||
                                            SearchBy.Equals("ERM Settlement Report-Summary") ||
                                            SearchBy.Equals("Inventory Bulk Cross-Check List") ||
                                            SearchBy.Equals("Service Bulk Cross-Check List") ||
                                            SearchBy.Equals("Billing-Type") ||
                                            SearchBy.Equals("Expenses Service Detail") ||
                                            SearchBy.Equals("FSR Service Detail")
                                            )
                                        {
                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                Detail_InfoCol.Add(element.detail_info);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.detail_info = Detail_InfoCol.ToArray();
                                        }

                                        else
                                        {
                                            // Default
                                            IDCol.Add(clsFunction.sZero);
                                            DescriptionCol.Add(clsFunction.sDefaultSelect);

                                            foreach (var element in Detail46.data)
                                            {
                                                clsSearch.RecordFound = true;
                                                IDCol.Add(element.ID.ToString());
                                                DescriptionCol.Add(element.Description);
                                            }

                                            clsArray.ID = IDCol.ToArray();
                                            clsArray.Description = DescriptionCol.ToArray();
                                        }

                                        break;
                                }
                                break;

                            case "Get Info Detail":
                                clsSearch.ClassOutParamValue = clsFunction.sNull;
                                CollectionDataDetailOnline Detail47 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsSearch.RecordFound = false;

                                switch (StatementType)
                                {
                                    case "Search":
                                        foreach (var element in Detail47.data)
                                        {
                                            clsSearch.RecordFound = true;
                                            clsSearch.ClassOutParamValue = element.outParamValue;
                                        }
                                        break;
                                }
                                break;

                            case "DeleteFileInfo":
                            case "CheckFileInfo":
                                clsSearch.ClassOutParamValue = clsFunction.sNull;
                                CollectionDataDetailOnline Detail48 = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(clsGlobalVariables.strJSONResponse); // Parse JSON Format                                
                                clsSearch.RecordFound = false;

                                foreach (var element in Detail48.data)
                                {
                                    clsSearch.RecordFound = true;
                                    clsSearch.ClassOutParamValue = element.outParamValue;
                                }

                                break;

                            default:
                                dbFunction.SetMessageBox("Undefine API \n\n" +
                                                       "MaintenanceType=" + MaintenanceType + "\n" +
                                                       "SearchBy=" + SearchBy + "\n" +
                                                       "SearchValue=" + "\n" +
                                                       "Action=" + Action, "Undefine API Call", clsFunction.IconType.iError);
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dbDump.WriteAPILog(2, "ExecuteAPI Log Excetion 3->" + ex.Message);

                Debug.WriteLine("[2]ExecuteAPI encountered error " + ex.Message);

                clsGlobalVariables.ExceptionMessage = ex.Message;
                clsGlobalVariables.sAPIResponseCode = clsGlobalVariables.UNDEFINED_ERROR;
                PromptAPIMessage(true, clsGlobalVariables.API_RESPONSE_ERROR);

                return;
            }

            //Debug.WriteLine("--ExecuteAPI==>End--");
        }

        public void GetTerminalInfo(string sTerminalID, string sTerminalSN)
        {
            clsSearch.ClassAdvanceSearchValue = sTerminalID + clsFunction.sPipe + sTerminalSN;

            Debug.WriteLine("GetTerminalInfo::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "Search", "Terminal Detail", clsSearch.ClassAdvanceSearchValue, "Terminal Detail", "", "ViewTerminalDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
                clsTerminal.RecordFound = true;
            else
                clsTerminal.RecordFound = false;

        }

        public void GetDockInfo(string sDockID, string sDockSN)
        {
            clsSearch.ClassAdvanceSearchValue = sDockID + clsFunction.sPipe + sDockSN;

            Debug.WriteLine("GetTerminalInfo::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "Search", "Terminal Detail", clsSearch.ClassAdvanceSearchValue, "Terminal Detail", "", "ViewTerminalDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                clsTerminal.RecordFound = true;
                clsSearch.ClassDockID = clsTerminal.ClassTerminalID;
                clsSearch.ClassDockSN = clsTerminal.ClassTerminalSN;

            }                
            else
                clsTerminal.RecordFound = false;

        }

        public void GetMerchantInfo(string StatementType, string SearchBy, string SearchValue)
        {           
            clsSearch.ClassStatementType = StatementType;
            clsSearch.ClassSearchBy = SearchBy;
            clsSearch.ClassSearchValue = SearchValue;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "Particular", "", "ViewAdvanceParticular");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            
        }

        public void GetIRInfo(string StatementType, string SearchBy, string SearchValue)
        {
          
            clsSearch.ClassStatementType = StatementType;
            clsSearch.ClassSearchBy = SearchBy;
            clsSearch.ClassSearchValue = SearchValue;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            
        }

        public void GetTAInfo()
        {
            Debug.WriteLine("--GetTAInfo--");
            Debug.WriteLine("clsSearch.ClassSearchValue="+ clsSearch.ClassSearchValue);

            int i = 0;            
            
            ExecuteAPI("GET", "View", "Advance TA", clsSearch.ClassSearchValue, "TA", "", "ViewAdvanceTA");            

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.TAID.Length > i)
                {
                    clsSearch.ClassTAIDNo = int.Parse(clsArray.TAIDNo[i]);
                    clsSearch.ClassClientID = int.Parse(clsArray.ClientID[i]);
                    clsSearch.ClassServiceProviderID = int.Parse(clsArray.ServiceProviderID[i]);
                    clsSearch.ClassIRIDNo = int.Parse(clsArray.IRIDNo[i]);
                    clsSearch.ClassMerchantID = int.Parse(clsArray.MerchantID[i]);
                    clsSearch.ClassFEID = int.Parse(clsArray.FEID[i]);
                    clsSearch.ClassTerminalID = int.Parse(clsArray.TerminalID[i]);
                    clsSearch.ClassTerminalTypeID = int.Parse(clsArray.TerminalTypeID[i]);
                    clsSearch.ClassTerminalModelID = int.Parse(clsArray.TerminalModelID[i]);
                    clsSearch.ClassTerminalBrandID = int.Parse(clsArray.TerminalBrandID[i]);
                    clsSearch.ClassServiceTypeID = int.Parse(clsArray.ServiceTypeID[i]);
                    clsSearch.ClassOtherServiceTypeID = int.Parse(clsArray.OtherServiceTypeID[i]);
                    clsSearch.ClassIRNo = clsArray.IRNo[i];
                    clsSearch.ClassSIMID = int.Parse(clsArray.SIMID[i]);
                    clsSearch.ClassTerminalSN = clsArray.TerminalSN[i];
                    clsSearch.ClassSIMSerialNo = clsArray.SIMSerialNo[i];
                    clsSearch.ClassSIMCarrier = clsArray.SIMCarrier[i];
                    clsSearch.ClassMerchantName = clsArray.MerchantName[i];
                    clsSearch.ClassTID = clsArray.TID[i];
                    clsSearch.ClassMID = clsArray.MID[i];
                    clsSearch.ClassClientName = clsArray.ClientName[i];
                    clsSearch.ClassFEName = clsArray.FEName[i];
                    clsSearch.ClassTypeDescription = clsArray.TypeDescription[i];
                    clsSearch.ClassModelDescription = clsArray.ModelDescription[i];
                    clsSearch.ClassBrandDescription = clsArray.BrandDescription[i];
                    clsSearch.ClassIRRequestDate = clsArray.IRDate[i];
                    clsSearch.ClassIRInstallationDate = clsArray.InstallationDate[i];
                    clsSearch.ClassTADateTime = clsArray.TADateTime[i];
                    clsSearch.ClassIRImportDateTime = clsArray.IRImportDateTime[i];
                    clsSearch.ClassServiceTypeDescription = clsArray.ServiceTypeDescription[i];
                    clsSearch.ClassOtherServiceTypeDescription = clsArray.OtherServiceTypeDescription[i];
                    clsSearch.ClassTerminalStatus = int.Parse(clsArray.TerminalStatus[i]);
                    clsSearch.ClassTerminalStatusDescription = clsArray.TerminalStatusDescription[i];
                    clsSearch.ClassRegionID = int.Parse(clsArray.RegionID[i]);
                    clsSearch.ClassRegionType = int.Parse(clsArray.RegionType[i]);
                    clsSearch.ClassRegion = clsArray.Region[i];
                    clsSearch.ClassTAProcessedBy = clsArray.TAProcessedBy[i];
                    clsSearch.ClassTAModifiedBy = clsArray.TAModifiedBy[i];
                    clsSearch.ClassTAProcessedDateTime = clsArray.TAProcessedDateTime[i];
                    clsSearch.ClassTAModifiedDateTime = clsArray.TAModifiedDateTime[i];
                    clsSearch.ClassTARemarks = (clsArray.TARemarks[i].Length > 0 ? clsArray.TARemarks[i] : clsFunction.sDash);
                    clsSearch.ClassTAComments = (clsArray.TAComments[i].Length > 0 ? clsArray.TAComments[i] : clsFunction.sDash);
                    clsSearch.ClassServiceTypeStatus = int.Parse(clsArray.ServiceTypeStatus[i]);
                    clsSearch.ClassServiceTypeStatusDescription = clsArray.ServiceTypeStatusDescription[i];
                    clsSearch.ClassDockID = int.Parse(clsArray.DockID[i]);
                    clsSearch.ClassDockSN = clsArray.DockSN[i];
                    clsSearch.ClassJobType = int.Parse(clsArray.JobType[i]);
                    clsSearch.ClassJobTypeDescription = clsArray.JobTypeDescription[i];
                    clsSearch.ClassJobTypeSubDescription = clsArray.JobTypeSubDescription[i];
                    clsSearch.ClassJobTypeStatusDescription = clsArray.JobTypeStatusDescription[i];
                    clsSearch.ClassServiceDateTime = clsArray.ServiceDateTime[i];
                    clsSearch.ClassAppVersion = clsArray.AppVersion[i];
                    clsSearch.ClassAppCRC = clsArray.AppCRC[i];
                    clsSearch.ClassPrimaryNum = clsArray.PrimaryNum[i];
                    clsSearch.ClassSecondaryNum = clsArray.SecondaryNum[i];

                    i++;

                }               
            }            
        }

        public void GetServiceTypeInfo()
        {            
            ExecuteAPI("GET", "Search", "Info", clsSearch.ClassAdvanceSearchValue, "Service Type", "", "ViewServiceType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                clsSearch.ClassServiceTypeID = clsServiceType.ClassServiceTypeID;
                clsSearch.ClassServiceTypeDescription = clsServiceType.ClassDescription;
                clsSearch.ClassServiceTypeCode = clsServiceType.ClassCode;
                clsSearch.ClassServiceTypeStatus = clsServiceType.ClassServiceStatus;
                clsSearch.ClassServiceTypeStatusDescription = clsServiceType.ClassStatusDescription;                
            }

        }
        public void GetOtherServiceTypeInfo(string StatementType, string SearchBy, string SearchValue)
        {            
            clsSearch.ClassStatementType = StatementType;
            clsSearch.ClassSearchBy = SearchBy;
            clsSearch.ClassSearchValue = SearchValue;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "Other Service Type", "", "ViewOtherServiceType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

        }

        public void GetSIMInfo(string sSIMID, string sSIMSN)
        {            
            clsSearch.ClassAdvanceSearchValue = sSIMID + clsFunction.sPipe + sSIMSN;

            Debug.WriteLine("GetSIMInfo::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "Search", "SIM Detail", clsSearch.ClassAdvanceSearchValue, "SIM Detail", "", "ViewSIMDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
                clsSIM.RecordFound = true;
            else
                clsSIM.RecordFound = false;            
        }

        public void GetServicingInfo(string sServiceNo)
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsSearch.ClassServiceStatus.ToString() + clsFunction.sPipe + 
                clsSearch.ClassServiceStatusDescription + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero;

            ExecuteAPI("GET", "View", "Servicing Detail", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {
                 
                    clsServicingDetail.ClassServiceNo = int.Parse(clsArray.ServiceNo[i].ToString());
                    clsServicingDetail.ClassServiceDateTime = clsArray.ServiceDateTime[i];
                    clsServicingDetail.ClassCounterNo = clsArray.CounterNo[i];
                    clsServicingDetail.ClassIRNo = clsArray.IRNo[i];
                    clsServicingDetail.ClassRequestNo = clsArray.RequestNo[i];
                    clsServicingDetail.ClassServiceDate = clsArray.ServiceDate[i];
                    clsServicingDetail.ClassServiceTime = clsArray.ServiceTime[i];
                    clsServicingDetail.ClassCustomerName = clsArray.CustomerName[i];
                    clsServicingDetail.ClassCustomerContactNo = clsArray.CustomerContactNo[i];
                    clsServicingDetail.ClassRemarks = clsArray.Remarks[i];
                    clsServicingDetail.ServiceReqDate = clsArray.ServiceReqDate[i];
                    clsServicingDetail.ServiceReqTime = clsArray.ServiceReqTime[i];
                    clsServicingDetail.LastServiceRequest = clsArray.LastServiceRequest[i];
                    clsServicingDetail.NewServiceRequest = clsArray.NewServiceRequest[i];
                    clsServicingDetail.ReplaceTerminalSN = clsArray.ReplaceTerminalSN[i];
                    clsServicingDetail.ReplaceSIMSN = clsArray.ReplaceSIMSN[i];
                    clsServicingDetail.ReplaceDockSN = clsArray.ReplaceDockSN[i];
                    clsServicingDetail.ClassJobType = int.Parse(clsArray.JobType[i]);
                    clsServicingDetail.ClassJobTypeDescription = clsArray.JobTypeDescription[i];
                    clsServicingDetail.ClassJobTypeSubDescription = clsArray.JobTypeSubDescription[i];
                    clsServicingDetail.ClassJobTypeStatusDescription = clsArray.JobTypeStatusDescription[i];
                    clsServicingDetail.ClassReferenceNo = clsArray.ReferenceNo[i];

                    i++;

                }             
            }
        }

        public void GetServicingCurrentTerminalInfo(string sServiceNo, string sRequestNo)
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero;

            ExecuteAPI("GET", "View", "Servicing Current Terminal", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {

                    clsServicingDetail.ClassServiceNo = int.Parse(clsArray.ServiceNo[i].ToString());
                    clsServicingDetail.ClassTerminalID = int.Parse(clsArray.TerminalID[i].ToString());
                    clsServicingDetail.ClassSIMID = int.Parse(clsArray.SIMID[i].ToString());
                    clsServicingDetail.ClassDockID = int.Parse(clsArray.DockID[i].ToString());
                    clsServicingDetail.ClassTerminalSN = clsArray.TerminalSN[i];
                    clsServicingDetail.ClassSIMSN = clsArray.SIMSerialNo[i];
                    clsServicingDetail.ClassDockSN = clsArray.DockSN[i];
                    clsServicingDetail.ClassCurTerminalSNStatus = int.Parse(clsArray.CurTerminalSNStatus[i].ToString());
                    clsServicingDetail.ClassCurSIMSNStatus = int.Parse(clsArray.CurSIMSNStatus[i].ToString());
                    clsServicingDetail.ClassCurDockSNStatus = int.Parse(clsArray.CurDockSNStatus[i].ToString());
                    clsServicingDetail.ClassCurTerminalSNStatusDescription = clsArray.CurTerminalSNStatusDescription[i];
                    clsServicingDetail.ClassCurSIMSNStatusDescription = clsArray.CurSIMSNStatusDescription[i];
                    clsServicingDetail.ClassCurDockSNStatusDescription = clsArray.CurDockSNStatusDescription[i];

                    i++;

                }
            }
        }

        public void GetServicingReplaceTerminalInfo(string sServiceNo, string sRequestNo)
        {
            int i = 0;

            clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero + clsFunction.sPipe +
                clsFunction.sZero;

            ExecuteAPI("GET", "View", "Servicing Replace Terminal", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {

                    clsServicingDetail.ClassServiceNo = int.Parse(clsArray.ServiceNo[i].ToString());
                    clsServicingDetail.ClassTerminalID = int.Parse(clsArray.TerminalID[i].ToString());
                    clsServicingDetail.ClassSIMID = int.Parse(clsArray.SIMID[i].ToString());
                    clsServicingDetail.ClassDockID = int.Parse(clsArray.DockID[i].ToString());
                    clsServicingDetail.ClassTerminalSN = clsArray.TerminalSN[i];
                    clsServicingDetail.ClassSIMSN = clsArray.SIMSerialNo[i];
                    clsServicingDetail.ClassDockSN = clsArray.DockSN[i];
                    clsServicingDetail.ClassRepTerminalSNStatus = int.Parse(clsArray.RepTerminalSNStatus[i].ToString());
                    clsServicingDetail.ClassRepSIMSNStatus = int.Parse(clsArray.RepSIMSNStatus[i].ToString());
                    clsServicingDetail.ClassRepDockSNStatus = int.Parse(clsArray.RepDockSNStatus[i].ToString());                    

                    i++;

                }
            }
        }

        public void GetParticularInfo(string sParticularID, string sParticularName)
        {
            clsSearch.ClassAdvanceSearchValue = sParticularID + clsFunction.sPipe +
                                                sParticularName;

            Debug.WriteLine("GetParticularInfo::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "Search", "Particular Detail", clsSearch.ClassAdvanceSearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
                clsParticular.RecordFound = true;
            else
                clsParticular.RecordFound = false;
        }

        public void GetSystemInfo()
        {
            int i = 0;

            Debug.WriteLine("--GetSystemInfo--");

            ExecuteAPI("GET", "View", "Publish", clsSystemSetting.ClassApplicationName, "System", "", "ViewSystem");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;
            
            while (clsArray.SysID.Length > i)
            {
                clsSystemSetting.ClassSystemID = int.Parse(clsArray.SysID[i]);
                clsSystemSetting.ClassSystemPublishDate = clsArray.PublishDate[i];
                clsSystemSetting.ClassSystemPublishVersion = clsArray.PublishVersion[i];

                i++;
            }

            Debug.WriteLine("ClassSystemID="+ clsSystemSetting.ClassSystemID);
            Debug.WriteLine("ClassSystemPublishDate=" + clsSystemSetting.ClassSystemPublishDate);
            Debug.WriteLine("ClassSystemPublishVersion=" + clsSystemSetting.ClassSystemPublishVersion);

        }

      
        public bool isNoRecordFound()
        {
            bool fFound = false;

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
                fFound = true;

            return fFound;
        }        

        public void PromptAPIMessage(bool iShow, string ResponseCode)
        {
            string sMessage = "";        

            //sBeautified = GetBeautifyJSON(clsGlobalVariables.strJSONResponse);

            clsAPI.ClassResponseCode = clsGlobalVariables.API_RESPONSE_FAILED;

            if (ResponseCode.CompareTo(clsGlobalVariables.SUCCESS_RESPONSE) == 0)
            {
                clsAPI.ClassResponseCode = clsGlobalVariables.API_RESPONSE_SUCCESS;
                sMessage = "SUCCESS_RESPONSE";
                iShow = false;
            }

            else if (ResponseCode.CompareTo(clsGlobalVariables.REQUEST_METHOD_NOT_VALID) == 0)
                sMessage = "REQUEST_METHOD_NOT_VALID";

            else if (ResponseCode.CompareTo(clsGlobalVariables.REQUEST_CONTENTTYPE_NOT_VALID) == 0)
                sMessage = "REQUEST_CONTENTTYPE_NOT_VALID";

            else if (ResponseCode.CompareTo(clsGlobalVariables.REQUEST_NOT_VALID) == 0)
                sMessage = "REQUEST_NOT_VALID";

            else if (ResponseCode.CompareTo(clsGlobalVariables.VALIDATE_PARAMETER_REQUIRED) == 0)
                sMessage = "VALIDATE_PARAMETER_REQUIRED";

            else if (ResponseCode.CompareTo(clsGlobalVariables.VALIDATE_PARAMETER_DATATYPE) == 0)
                sMessage = "VALIDATE_PARAMETER_DATATYPE";

            else if (ResponseCode.CompareTo(clsGlobalVariables.API_NAME_REQUIRED) == 0)
                sMessage = "API_NAME_REQUIRED";

            else if (ResponseCode.CompareTo(clsGlobalVariables.API_PARAM_REQUIRED) == 0)
                sMessage = "API_PARAM_REQUIRED";

            else if (ResponseCode.CompareTo(clsGlobalVariables.API_DOST_NOT_EXIST) == 0)
                sMessage = "API_DOST_NOT_EXIST";

            else if (ResponseCode.CompareTo(clsGlobalVariables.INVALID_USER_PASS) == 0)
                sMessage = "INVALID_USER_PASS";

            else if (ResponseCode.CompareTo(clsGlobalVariables.USER_NOT_ACTIVE) == 0)
                sMessage = "USER_NOT_ACTIVE";

            else if (ResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                clsAPI.ClassResponseCode = clsGlobalVariables.API_RESPONSE_SUCCESS;
                sMessage = "NO_RECORD_FOUND";
                iShow = false;
            }

            else if (ResponseCode.CompareTo(clsGlobalVariables.API_KEY_REQUIRED) == 0)
                sMessage = "API_KEY_REQUIRED";

            else if (ResponseCode.CompareTo(clsGlobalVariables.INVALID_AUTH_USER_PW) == 0)
                sMessage = "INVALID_AUTH_USER_PW";

            else if (ResponseCode.CompareTo(clsGlobalVariables.INVALID_METHOD) == 0)
                sMessage = "INVALID_METHOD";

            else if (ResponseCode.CompareTo(clsGlobalVariables.API_RESPONSE_ERROR) == 0)
                sMessage = "API_RESPONSE_ERROR";

            else
            {
                sMessage = "UNDEFINED_ERROR" +
                            "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "[API]" + "\n" +
                            ">URL=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIURL) + "\n" +
                            ">Path=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIPath) + "\n" +
                            ">ContentType=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIContentType) + "\n" +
                            ">UserName=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIAuthUser) + "\n" +
                            ">Password=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIAuthPassword) + "\n" +
                            ">Keys=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIKeys) + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "[PARAMETERS]" + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            ">Method=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAPIMethod) + "\n" +
                            ">Action=" + dbFunction.AddBracketStartEnd(clsSearch.ClassAction) + "\n" +
                            ">StatementType=" + dbFunction.AddBracketStartEnd(clsSearch.ClassStatementType) + "\n" +
                            ">SearchBy=" + dbFunction.AddBracketStartEnd(clsSearch.ClassSearchBy) + "\n" +
                            ">SearchValue=" + dbFunction.AddBracketStartEnd(clsSearch.ClassSearchValue) + "\n" +
                            ">MaintenanceType=" + dbFunction.AddBracketStartEnd(clsSearch.ClassMaintenanceType) + "\n" +
                            ">SQL=" + dbFunction.AddBracketStartEnd(clsSearch.ClassSQL) + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            "[RESPONSE]" + "\n" +
                            clsFunction.sLineSeparator + "\n" +
                            ">clsAPI.ClassResponseCode=" + dbFunction.AddBracketStartEnd(clsAPI.ClassResponseCode.ToString()) + "\n" +
                            ">Code=" + dbFunction.AddBracketStartEnd(ResponseCode) + "\n" +
                            ">Message=" + dbFunction.AddBracketStartEnd(clsGlobalVariables.strJSONResponse) + "\n" +
                            clsFunction.sLineSeparator;
            }

            if (ResponseCode.CompareTo(clsGlobalVariables.SUCCESS_RESPONSE) != 0 &&
                (ResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) != 0))
            {
                dbDump.WriteAPILog(2, "API Error " + "\n\n" + dbFunction.AddBracketStartEnd(sMessage));
            }
            
            //Debug.WriteLine("--PromptAPIMessage--");
            Debug.WriteLine(sMessage);
            //Debug.WriteLine("--PromptAPIMessage--");

            if (iShow)
                MessageBox.Show(sMessage, "API Response", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public bool isImportFileName(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool isRecordExist(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public void SaveUserLog(UserActionType iType, string sPublishVersion)
        {
            DateTime LogDateTime = DateTime.Now;
            string sLogDateTime = "";                        
            string sRowSQL = "";
            string sSQL = "";
            string sAction = "";
            int iSessionStatus = 0;
            string sSessionStatusDescription = "";

            sLogDateTime = LogDateTime.ToString("yyyy-MM-dd H:mm:ss");

            switch (iType)
            {
                case UserActionType.iLogIn:
                    sAction = "User LogIn";
                    iSessionStatus = 0;
                    sSessionStatusDescription = "FAILED";

                    sRowSQL = "";
                    sRowSQL = " ('" + sLogDateTime + "', " +
                    sRowSQL + sRowSQL + " '" + clsUser.ClassUserID + "', " +
                    sRowSQL + sRowSQL + " '" + clsUser.ClassUserName + "', " +
                    sRowSQL + sRowSQL + " '" + clsUser.ClassUserFullName + "', " +
                    sRowSQL + sRowSQL + " '" + clsGlobalVariables.strLocalIP + "', " +
                    sRowSQL + sRowSQL + " '" + clsGlobalVariables.strComputerName + "', " +
                    sRowSQL + sRowSQL + " '" + sAction + "', " +
                    sRowSQL + sRowSQL + " '" + iSessionStatus + "', " +
                    sRowSQL + sRowSQL + " '" + sSessionStatusDescription + "', " +
                    sRowSQL + sRowSQL + " '" + sPublishVersion.ToUpper() + "') ";
                    sSQL = sSQL + sRowSQL;

                    ExecuteAPI("POST", "Insert", "", "", "User Log", sSQL, "InsertMaintenanceMaster");

                    clsUser.ClassLogID = clsLastID.ClassLastInsertedID;

                    break;
                case UserActionType.iLogOut:
                    sAction = "User LogOut";
                    iSessionStatus = 1;
                    sSessionStatusDescription = "SUCCESS";

                    clsSearch.ClassAdvanceSearchValue = clsUser.ClassLogID.ToString() + clsFunction.sPipe +
                                                        clsUser.ClassUserID.ToString() + clsFunction.sPipe +
                                                        sAction + clsFunction.sPipe +
                                                        sLogDateTime + clsFunction.sPipe +
                                                        iSessionStatus.ToString() + clsFunction.sPipe +
                                                        sSessionStatusDescription + clsFunction.sPipe +
                                                        sPublishVersion.ToUpper(); 

                    dbAPI.ExecuteAPI("PUT", "Update", "User LogOut", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");

                    break;               
            }


        }

        public enum UserActionType
        {
            iLogIn, iLogOut, iSave, iDelete, iUpdate
        }

        public void UpdateStatus(string StatementType, string SearchBy, string SearchValue, string MaintenanceType)
        {
            dbAPI = new clsAPI();
            ExecuteAPI("PUT", StatementType, SearchBy, SearchValue, MaintenanceType, "", "UpdateStatus");
        }

        public bool isRequestID(string sRequestID)
        {
            bool isExist = true;

            ExecuteAPI("GET", "Search", "Request ID", sRequestID, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool isAlreadyLogIn(string sUserID)
        {
            bool isExist = true;

            ExecuteAPI("GET", "Search", "User Already Login", sUserID, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool isRecordCount(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }
        
        public void GetParticularList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewAdvanceParticular");
        }

        // Merchant
        public int GetParticularFromList(string sParticularName)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.ParticularID.Length > i)
                {
                    clsParticular.ClassParticularID = int.Parse(clsArray.ParticularID[i]);
                    clsParticular.ClassParticularName = clsArray.ParticularName[i];

                    // ROCKY - API ISSUE FIXED: FIX FOR GET MERCHANT ID AND NAME TESTING
                    /*
                    clsSearch.ClassAdvanceSearchValue = clsParticular.ClassParticularID + clsFunction.sPipe + clsParticular.ClassParticularName;

                    dbAPI.ExecuteAPI("GET", "Search", "Get Merchant ID", clsSearch.ClassAdvanceSearchValue, "Get Info Detail", "", "GetInfoDetail");

                    clsParticular.ClassParticularID = int.Parse(dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "ParticularID"));
                    clsParticular.ClassParticularName = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "Name");
                    */

                    if (sParticularName.CompareTo(clsParticular.ClassParticularName) == 0)
                    {
                        iID = clsParticular.ClassParticularID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        public void GetClientList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewAdvanceParticular");
        }
        // Client
        public int GetClientFromList(string sParticularName)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.ClientID.Length > i)
                {
                    clsParticular.ClassParticularID = int.Parse(clsArray.ClientID[i]);
                    clsParticular.ClassParticularName = clsArray.ClientName[i];

                    if (sParticularName.CompareTo(clsParticular.ClassParticularName) == 0)
                    {
                        iID = clsParticular.ClassParticularID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }

        public void GetFEList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iFE_Type.ToString() + clsFunction.sPipe + clsFunction.sZero;

            //ExecuteAPI("GET", StatementType, "Particular List", clsSearch.ClassAdvanceSearchValue, MaintenaceType, "", "ViewAdvanceParticular");
            ExecuteAPI("GET", StatementType, SearchBy, clsSearch.ClassAdvanceSearchValue, MaintenaceType, "", "ViewAdvanceParticular");

        }
        // Field Engineer
        public int GetFEFromList(string sParticularName)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.FEID.Length > i)
                {
                    clsParticular.ClassParticularID = int.Parse(clsArray.FEID[i]);
                    clsParticular.ClassParticularName = clsArray.FEName[i];

                    if (sParticularName.CompareTo(clsParticular.ClassParticularName) == 0)
                    {
                        iID = clsParticular.ClassParticularID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
           

            return iID;
        }

        // Service Provider
        public int GetSPFromList(string sParticularName)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.ServiceProviderID.Length > i)
                {
                    clsParticular.ClassParticularID = int.Parse(clsArray.ServiceProviderID[i]);
                    clsParticular.ClassParticularName = clsArray.ServiceProviderName[i];

                    if (sParticularName.CompareTo(clsParticular.ClassParticularName) == 0)
                    {
                        iID = clsParticular.ClassParticularID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }
        // City
        public void GetCityList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {            
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewCity");
        }

        // City
        public int GetCityFromList(string sCity)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.CityID.Length > i)
                {
                    clsCity.ClassCityID = int.Parse(clsArray.CityID[i]);
                    clsCity.ClassCity = clsArray.City[i];

                    if (sCity.CompareTo(clsCity.ClassCity) == 0)
                    {
                        iID = clsCity.ClassCityID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        // Province
        public void GetProvinceList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewProvince");
        }

        // Province
        public int GetProvinceFromList(string sProvince)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.ProvinceID.Length > i)
                {
                    clsProvince.ClassProvinceID = int.Parse(clsArray.ProvinceID[i]);
                    clsProvince.ClassProvince = clsArray.Province[i];

                    if (sProvince.CompareTo(clsProvince.ClassProvince) == 0)
                    {
                        iID = clsProvince.ClassProvinceID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            

            return iID;
        }

        // Region
        public void GetRegionList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewRegion");
        }

        // Region
        public int GetRegionFromList(string sRegion)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.RegionID.Length > i)
                {
                    clsRegion.ClassRegionID = int.Parse(clsArray.RegionID[i]);
                    clsRegion.ClassRegion = clsArray.Region[i];

                    if (sRegion.CompareTo(clsRegion.ClassRegion) == 0)
                    {
                        iID = clsRegion.ClassRegionID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }
        public void FillComboBoxRegion(ComboBox obj, string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            int i = 0;
            bool fSelect = false;

            GetRegionList(StatementType, SearchBy, SearchValue, MaintenaceType);

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.RegionID.Length > i)
            {
                clsRegion.ClassRegion = clsArray.Region[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsRegion.ClassRegion);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxRegionDetail(ComboBox obj, string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            int i = 0;
            bool fSelect = false;

            GetRegionList(StatementType, SearchBy, SearchValue, MaintenaceType);

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.RegionID.Length > i)
            {
                clsRegion.ClassProvince = clsArray.Province[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsRegion.ClassProvince);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        // Terminal
        public void GetTerminalList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewAdvanceTerminal");
        }
        
        // Terminal
        public int GetTerminalFromList(string sSerialNo)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.TerminalID.Length > i)
                {
                    clsTerminal.ClassTerminalID = int.Parse(clsArray.TerminalID[i].ToString());
                    clsTerminal.ClassTerminalTypeID = int.Parse(clsArray.TerminalTypeID[i].ToString());
                    clsTerminal.ClassTerminalModelID = int.Parse(clsArray.TerminalModelID[i].ToString());
                    clsTerminal.ClassTerminalBrandID = int.Parse(clsArray.TerminalBrandID[i].ToString());
                    clsTerminal.ClassNo = clsArray.No[i];
                    clsTerminal.ClassTerminalSN = clsArray.SerialNo[i];
                    clsTerminal.ClassTerminalType = clsArray.TerminalType[i];
                    clsTerminal.ClassTerminalModel = clsArray.TerminalModel[i];
                    clsTerminal.ClassTerminalBrand = clsArray.TerminalBrand[i];
                    clsTerminal.ClassDeliveryDate = clsArray.DeliveryDate[i];
                    clsTerminal.ClassReceiveDate = clsArray.ReceiveDate[i];

                    if (sSerialNo.CompareTo(clsTerminal.ClassTerminalSN) == 0)
                    {
                        iID = clsTerminal.ClassTerminalID;
                        break;
                    }

                    i++;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        // Brand
        public void GetTerminalBrandList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewTerminalBrand");
        }

        // Brand
        public int GetTerminalBrandFromList(string sBrand)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.TerminalBrandID.Length > i)
                {
                    clsTerminalBrand.ClassTerminalBrandID = int.Parse(clsArray.TerminalBrandID[i]);
                    clsTerminalBrand.ClassDescription = clsArray.BrandDescription[i];

                    if (sBrand.CompareTo(clsTerminalBrand.ClassDescription) == 0)
                    {
                        iID = clsTerminalBrand.ClassTerminalBrandID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }

        // Model
        public void GetTerminalModelList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewTerminalModel");
        }

        // Model
        public int GetTerminalModelFromList(string sModel)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.TerminalModelID.Length > i)
                {
                    clsTerminalModel.ClassTerminalModelID = int.Parse(clsArray.TerminalModelID[i]);
                    clsTerminalModel.ClassDescription = clsArray.ModelDescription[i];

                    if (sModel.CompareTo(clsTerminalModel.ClassDescription) == 0)
                    {
                        iID = clsTerminalModel.ClassTerminalModelID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }

        // Type
        public void GetTerminalTypeList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewTerminalType");
        }

        // Type
        public int GetTerminalTypeFromList(string sType)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.TerminalTypeID.Length > i)
                {
                    clsTerminalType.ClassTerminalTypeID = int.Parse(clsArray.TerminalTypeID[i]);
                    clsTerminalType.ClassDescription = clsArray.TypeDescription[i];

                    if (sType.CompareTo(clsTerminalType.ClassDescription) == 0)
                    {
                        iID = clsTerminalType.ClassTerminalTypeID;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
           
            return iID;
        }

        // Terminal Status
        public void GetTerminalStatusList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewTerminalStatus");
        }

        // Terminal Status
        public int GetTerminalStatusFromList(string sType)
        {
            int i = 0;
            int iID = 0;

            Debug.WriteLine("--GetTerminalStatusFromList--");
            Debug.WriteLine("sType="+ sType);
            Debug.WriteLine("Length="+ clsArray.TerminalStatusID.Length);

            try
            {
                while (clsArray.TerminalStatusID.Length > i)
                {
                    clsTerminalStatus.ClassTerminalStatusID = int.Parse(clsArray.TerminalStatusID[i]);
                    clsTerminalStatus.ClassDescription = clsArray.TerminalStatusDescription[i];
                    clsTerminalStatus.ClassTerminalStatusType = int.Parse(clsArray.TerminalStatusType[i]);

                    Debug.WriteLine("i="+i.ToString()+"|TerminalStatusID="+ clsArray.TerminalStatusID[i]+ "|TerminalStatusDescription="+clsArray.TerminalStatusDescription[i]+ "|TerminalStatusType="+ clsArray.TerminalStatusType[i]);

                    if (sType.Equals(clsTerminalStatus.ClassDescription))
                    {
                        iID = clsTerminalStatus.ClassTerminalStatusType;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            Debug.WriteLine("GetTerminalStatusFromList, iID=" + iID);
            
            return iID;
        }

        public int GetFSRStatusFromList(string sFSRStatus)
        {
            int i = 0;
            int iID = 0;

            List<string> FSRStatusCol = new List<String>();
            List<string> FSRStatusDescrptionCol = new List<String>();

            FSRStatusCol.Add(clsGlobalVariables.FSR_VALID_STATUS.ToString());
            FSRStatusCol.Add(clsGlobalVariables.FSR_INVALID_STATUS.ToString());

            FSRStatusDescrptionCol.Add(clsGlobalVariables.FSR_VALID_STATUS_DESC);
            FSRStatusDescrptionCol.Add(clsGlobalVariables.FSR_INVALID_STATUS_DESC);

            clsArray.FSRStatus = FSRStatusCol.ToArray();
            clsArray.FSRStatusDescription = FSRStatusDescrptionCol.ToArray();
            
            while (clsArray.FSRStatus.Length > i)
            {
                if (sFSRStatus.CompareTo(clsArray.FSRStatusDescription[i]) == 0)
                {
                    iID = int.Parse(clsArray.FSRStatus[i]);
                    break;
                }
                
                i++;
            }

            return iID;

        }

        // Service Type
        public void GetServiceTypeList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewServiceType");
        }

        // Service Type
        public int GetServiceTypeFromList(string sType)
        {
            int i = 0;
            int iID = 0;
            
            try
            {
                while (clsArray.ServiceTypeID.Length > i)
                {
                    clsServiceType.ClassServiceTypeID = int.Parse(clsArray.ServiceTypeID[i]);
                    clsServiceType.ClassDescription = clsArray.ServiceTypeDescription[i];
                    clsServiceType.ClassStatusDescription = clsArray.ServiceStatusDescription[i];
                    clsServiceType.ClassCode = clsArray.Code[i];
                    clsServiceType.ClassJobTypeDescrition = clsArray.JobTypeDescription[i];

                    if (sType.CompareTo(clsServiceType.ClassDescription) == 0)
                    {
                        iID = clsServiceType.ClassServiceTypeID;
                        break;
                    }

                    i++;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        public int GetServiceStatusFromList(string sStatus)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.ServiceStatus.Length > i)
                {
                    clsServiceType.ClassServiceStatus = int.Parse(clsArray.ServiceStatus[i]);
                    clsServiceType.ClassStatusDescription = clsArray.ServiceStatusDescription[i];
                    clsServiceType.ClassCode = clsArray.Code[i];

                    if (sStatus.CompareTo(clsServiceType.ClassStatusDescription) == 0)
                    {
                        iID = clsServiceType.ClassServiceStatus;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }

        public void GetIRNoList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewAdvanceIR");
        }

        public void GetTerminalSNList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewAdvanceTerminal");
        }

        public string[] GetCarrier()
        {
            Debug.WriteLine("--GetCarrier--");

            string[] ret = { clsFunction.sDefaultSelect };
      
            ExecuteAPI("GET", "View", "Carrier", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (clsGlobalVariables.isAPIResponseOK)
            {
                ret = clsArray.Description;             
            }
           
            return ret;
        }
        
        public string[] GetFSRStatus()
        {
            string[] ret = { clsFunction.sDefaultSelect, "VALID FSR", "INVALID FSR" };

            return ret;
        }

        public void GetFSRStatusList()
        {         
            clsSearch.ClassFSRStatusList = "";
            for (int i = 0; i < GetFSRStatus().Length; i++)
            {
                clsSearch.ClassFSRStatusList = clsSearch.ClassFSRStatusList + GetFSRStatus()[i].ToString() + Environment.NewLine;
            }
        }
        
        
        public string[] GetJobTypeDescription()
        {
            string[] ret = { clsFunction.sDefaultSelect, "SVC REQ INSTALLATION", "SVC REQ SERVICING", "SVC REQ PULL-OUT", "SVC REQ REPLACEMENT", "SVC REQ REPROGRAMMING", "SVC REQ DIAGNOSTIC", "SVC REQ DISPATCH" };

            return ret;
        }

        public void GetJobTypeDescriptionList()
        {
            clsSearch.ClassJobTypeDescriptionList = "";
            for (int i = 0; i < GetJobTypeDescription().Length; i++)
            {
                clsSearch.ClassJobTypeDescriptionList = clsSearch.ClassJobTypeDescriptionList + GetJobTypeDescription()[i].ToString() + Environment.NewLine;
            }
        }

        public void FillComboBoxJobTypeDescription(ComboBox obj)
        {
            int i = 0;
            
            obj.Items.Clear();
            while (GetJobTypeDescription().Length > i)
            {
                obj.Items.Add(GetJobTypeDescription()[i]);               

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }
        
        
        public string[] GetJobTypeStatusDescription()
        {
            string[] ret = { clsFunction.sDefaultSelect, "PENDING", "PROCESSING", "COMPLETED", "FAILED" };

            return ret;
        }

        public void GetJobTypeStatusDescriptionList()
        {
            clsSearch.ClassJobTypeStatusDescriptionList = "";
            for (int i = 0; i < GetJobTypeStatusDescription().Length; i++)
            {
                clsSearch.ClassJobTypeStatusDescriptionList = clsSearch.ClassJobTypeStatusDescriptionList + GetJobTypeStatusDescription()[i].ToString() + Environment.NewLine;
            }
        }
        public void FillComboBoxJobTypeStatusDescription(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetJobTypeStatusDescription().Length > i)
            {
                obj.Items.Add(GetJobTypeStatusDescription()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }

        // Terminal Status
        public int GetIRNoFromList(string sIRNo)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.IRNo.Length > i)
                {
                    clsIR.ClassIRIDNo = int.Parse(clsArray.IRIDNo[i]);
                    clsIR.ClassIRNo = clsArray.IRNo[i];

                    if (sIRNo.CompareTo(clsIR.ClassIRNo) == 0)
                    {
                        iID = clsIR.ClassIRIDNo;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }
            
            return iID;
        }

        public void GetViewCount(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewCount");
        }

        public void GetViewTotal(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewTotal");
        }

        public bool isCheckTable()
        {
            bool fValid = true;
            string sMessage = "No record found.";

            dbFunction = new clsFunction();

            if (!isRecordCount("Search", "Service Type Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Service Type", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Terminal Status Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Terminal Status", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Terminal Type Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Terminal Type", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Terminal Model Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Terminal Model", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Terminal Brand Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Terminal Brand", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Terminal Detail Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Terminal", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Merchant Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Merchant", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Client Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Client", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Field Engineer Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Field Engineer", clsFunction.IconType.iInformation);
                fValid = false;
            }

            else if (!isRecordCount("Search", "Service Provider Count", ""))
            {
                dbFunction.SetMessageBox(sMessage, "Service Provider", clsFunction.IconType.iInformation);
                fValid = false;
            }

            return fValid;
        }

        public void FillComboBoxServiceType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetServiceTypeList("View", "Service Type Active", "", "Service Type");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ServiceTypeID.Length > i)
            {
                clsServiceType.ClassDescription = clsArray.ServiceJobTypeDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsServiceType.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        
        public void FillComboBoxClient(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iClient_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ParticularID.Length > i)
            {                
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.ParticularName[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        public void FillComboBoxSP(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iSP_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ParticularID.Length > i)
            {                
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.ParticularName[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        public void FillComboBoxFE(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            
            clsSearch.ClassAdvanceSearchValue = clsGlobalVariables.iFE_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassAdvanceSearchValue, "Particular", "", "ViewAdvanceParticular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ParticularID.Length > i)
            {                
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.ParticularName[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }
        
        public void FillComboBoxTerminalType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetTerminalTypeList("View", "Terminal Type", "", "Terminal Type");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalTypeID.Length > i)
            {
                clsTerminalType.ClassDescription = clsArray.TypeDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalType.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        public void FillComboBoxTerminalModel(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetTerminalModelList("View", "Terminal Model", "", "Terminal Model");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalModelID.Length > i)
            {
                clsTerminalModel.ClassDescription = clsArray.ModelDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalModel.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        public void FillComboBoxTerminalModelByTerminalType(ComboBox obj, string pSearchValue)
        {
            int i = 0;
            bool fSelect = false;

            GetTerminalModelList("View", "Terminal Model By Terminal Type", pSearchValue, "Terminal Model");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalModelID.Length > i)
            {
                clsTerminalModel.ClassDescription = clsArray.ModelDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalModel.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxTerminalBrand(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetTerminalBrandList("View", "Terminal Brand", "", "Terminal Brand");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalBrandID.Length > i)
            {
                clsTerminalBrand.ClassDescription = clsArray.BrandDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalBrand.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxTerminalStatus(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsGlobalVariables.iTerminal_Type + clsFunction.sPipe + clsFunction.sNull;

            GetTerminalStatusList("View", "Status List", clsSearch.ClassAdvanceSearchValue, "Terminal Status");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalStatusID.Length > i)
            {
                clsTerminalStatus.ClassDescription = clsArray.TerminalStatusDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalStatus.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxTerminalStatusForFSR(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassAdvanceSearchValue = clsFunction.sZero + clsFunction.sPipe + clsGlobalVariables.iTerminal_Type + clsFunction.sPipe + clsFunction.sNull + clsFunction.sPipe + clsFunction.sOne;

            GetTerminalStatusList("View", "Status List For FSR", clsSearch.ClassAdvanceSearchValue, "Terminal Status");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalStatusID.Length > i)
            {
                clsTerminalStatus.ClassDescription = clsArray.TerminalStatusDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalStatus.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillSNStatusList(ComboBox obj, string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            bool fSelect = false;
            
            GetTerminalStatusList(StatementType, SearchBy, SearchValue, "Terminal Status");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalStatusID.Length > i)
            {
                clsTerminalStatus.ClassDescription = clsArray.TerminalStatusDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalStatus.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxCarrier(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            GetCarrier();
            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {
                    obj.Items.Add(clsArray.Description[i]);

                    i++;
                }
                
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        public string[] GetSCStatus()
        {
            string[] ret = { clsFunction.sDefaultSelect, "RESOLVED BY PHONE", "FOR SERVICING", "NEGATIVE", "NO ANSWER" };

            return ret;
        }

        public void FillComboBoxSCStatus(ComboBox obj)
        {
            int i = 0;
           
            obj.Items.Clear();

            while (GetSCStatus().Length > i)
            {
                obj.Items.Add(GetSCStatus()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }

        public void FillComboBoxTerminalBase(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetTerminalTypeList("View", "Terminal Base", "", "Terminal Base");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.TerminalTypeID.Length > i)
            {
                clsTerminalType.ClassDescription = clsArray.TypeDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsTerminalType.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillListViewTerminalSN(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetTerminalSNList("View", "TerminalSN List", SearchValue, "Terminal");

            if (!clsGlobalVariables.isAPIResponseOK) return;            

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalID.Length > i)
                {                    
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.ForeColor = dbFunction.GetColorByStatus(int.Parse(clsArray.TerminalStatus[i]), clsArray.TerminalStatusDescription[i]); // set forecolor per status

                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.TerminalType[i]);
                    item.SubItems.Add(clsArray.TerminalModel[i]);
                    item.SubItems.Add(clsArray.TerminalBrand[i]);
                    item.SubItems.Add(clsArray.TerminalStatus[i].ToString());
                    item.SubItems.Add(clsArray.TerminalStatusDescription[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.Location[i]);
                    item.SubItems.Add(clsArray.Allocation[i]);
                    item.SubItems.Add(clsArray.AssetType[i]);
                    item.SubItems.Add(clsArray.DeliveryDate[i]);
                    item.SubItems.Add(clsArray.ReceiveDate[i]);
                    item.SubItems.Add(clsArray.ReleaseDate[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
            
        }

        public void FillListViewSIMSN(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetTerminalSNList("View", "SIMSN List", SearchValue, "Terminal");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.SIMID.Length > i)
                {                    
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.ForeColor = dbFunction.GetColorByStatus(int.Parse(clsArray.SIMStatus[i]), clsArray.SIMStatusDescription[i]); // set forecolor per status

                    item.SubItems.Add(clsArray.SIMID[i].ToString());
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.SIMCarrier[i]);
                    item.SubItems.Add(clsArray.SIMStatus[i].ToString());
                    item.SubItems.Add(clsArray.SIMStatusDescription[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.Location[i]);
                    item.SubItems.Add(clsArray.Allocation[i]);
                    item.SubItems.Add(clsArray.DeliveryDate[i]);
                    item.SubItems.Add(clsArray.ReceiveDate[i]);
                    item.SubItems.Add(clsArray.ReleaseDate[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewMerchant(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;
            
            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetParticularList("View", "Particular List", SearchValue, "Particular");

            if (!clsGlobalVariables.isAPIResponseOK) return;
            
            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ParticularID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.ParticularName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.Address[i]);
                    item.SubItems.Add(clsArray.MobileNo[i]);
                    item.SubItems.Add(clsArray.TelNo[i]);
                    
                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }
        public void FillListViewFE(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetParticularList("View", "Field Engineer List", SearchValue, "Particular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.FEID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.FEID[i].ToString());
                    item.SubItems.Add(clsArray.FEName[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }
        public void FillListViewReason(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;
            
            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Reason", SearchValue, "Reason", "", "ViewReason");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                obj.Items.Clear();
                while (clsArray.ReasonID.Length > i)
                {
                    clsReason.ClassReasonID = int.Parse(clsArray.ReasonID[i]);
                    clsReason.ClassReasonDescription = clsArray.ReasonDescription[i];
                    clsReason.ClassReasonCode = clsArray.ReasonCode[i];
                    clsReason.ClassReasonIsInput = int.Parse(clsArray.ReasonIsInput[i]);


                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsReason.ClassReasonID.ToString());
                    item.SubItems.Add(clsReason.ClassReasonDescription);
                    item.SubItems.Add(clsReason.ClassReasonCode);
                    item.SubItems.Add(clsReason.ClassReasonIsInput.ToString());


                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewAttempt(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "", SearchValue, "FSR Attempt", "", "ViewFSRAttempt");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.FSRNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.FSRNo[i].ToString());
                    item.SubItems.Add(clsArray.FSRDate[i].ToString());
                    item.SubItems.Add(clsArray.FSRTime[i].ToString());
                    item.SubItems.Add(clsArray.FSRServiceStatus[i].ToString());
                    item.SubItems.Add(clsArray.FSRServiceStatusDescription[i].ToString());
                    item.SubItems.Add(clsArray.FSRRemarks[i].ToString());                    

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }
        public void FillListViewIRNo(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;
            
            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "IRNo List2", SearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.IRIDNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i].ToString());
                    item.SubItems.Add(clsArray.MerchantName[i].ToString());

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewRegion(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Region List", SearchValue, "Region Detail", "", "ViewRegionDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.RegionID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.RegionID[i].ToString());
                    item.SubItems.Add(clsArray.RegionType[i].ToString());
                    item.SubItems.Add(clsArray.RegionProvince[i]);
                    item.SubItems.Add(clsArray.Region[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewProvince(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Province List", SearchValue, "Region Detail", "", "ViewRegionDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.RegionID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.RegionID[i].ToString());
                    item.SubItems.Add(clsArray.RegionType[i].ToString());
                    item.SubItems.Add(clsArray.RegionProvince[i]);
                    item.SubItems.Add(clsArray.Region[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillParticularListView(ListView obj, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            Debug.WriteLine("--FillParticularListView--");
            Debug.WriteLine("SearchBy="+ SearchBy);
            Debug.WriteLine("SearchValue=" + SearchValue);

            dbFunction = new clsFunction();

            obj.Items.Clear();
            
            GetParticularList("View", "Particular List", SearchValue, "Particular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ParticularID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    if (!SearchBy.Equals(clsGlobalVariables.sMerchant_Type_List))
                        item.ForeColor = dbFunction.GetColorByStatus(int.Parse(clsArray.IRStatus[i]), clsArray.IRStatusDescription[i]); // set forecolor per status
                    
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.ParticularName[i]);

                    if (SearchBy.Equals(clsGlobalVariables.sMerchant_Type_List))
                    {
                        item.SubItems.Add(clsFunction.sDash);
                        item.SubItems.Add(clsFunction.sDash);
                    }
                    else
                    {
                        item.SubItems.Add(clsArray.TID[i]);
                        item.SubItems.Add(clsArray.MID[i]);
                    }
                   
                    item.SubItems.Add(clsArray.Address[i]);
                    item.SubItems.Add(clsArray.ContactPerson[i]);
                    item.SubItems.Add(clsArray.MobileNo[i]);
                    item.SubItems.Add(clsArray.TelNo[i]);
                    item.SubItems.Add(clsArray.Email[i]);

                    if (SearchBy.Equals(clsGlobalVariables.sMerchant_Type))
                    {
                        item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                        item.SubItems.Add(clsArray.IRNo[i]);
                        item.SubItems.Add(clsArray.IRStatusDescription[i]);
                    }
                    else if (SearchBy.Equals(clsGlobalVariables.sMerchant_Type_List))
                    {
                        item.SubItems.Add(clsFunction.sDash);
                        item.SubItems.Add(clsFunction.sDash);
                        item.SubItems.Add(clsFunction.sDash);
                    }
                    else
                    {
                        item.SubItems.Add(clsArray.DepartmentDesc[i]);
                        item.SubItems.Add(clsArray.PositionDesc[i]);
                        item.SubItems.Add(clsArray.EmploymentStatus[i]);
                    }

                    item.SubItems.Add(clsArray.ClientID[i]);

                    item.SubItems.Add(clsArray.Region[i]);
                    item.SubItems.Add(clsArray.Province[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillMerchantListView(ListView obj, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            Debug.WriteLine("--FillMerchantListView--");
            Debug.WriteLine("SearchBy=" + SearchBy);
            Debug.WriteLine("SearchValue=" + SearchValue);

            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetParticularList("View", "Particular List 2", SearchValue, "Particular");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.SubItems.Add(clsArray.ID[i].ToString());

                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Name));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Address));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ContactPerson));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ContactPosition));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ContactNumber));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Email));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_REGION));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Province));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewTerminalStatus(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Status List", clsSearch.ClassAdvanceSearchValue, "Terminal Status", "", "ViewTerminalStatus");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalStatusID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TerminalStatusID[i].ToString());                    
                    item.SubItems.Add(clsArray.TerminalStatusType[i]);
                    item.SubItems.Add(clsArray.TerminalStatusDescription[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewTerminalType(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();
            
            ExecuteAPI("GET", "View", "", SearchValue, "Terminal Type", "", "ViewTerminalType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalTypeID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TerminalTypeID[i].ToString());
                    item.SubItems.Add(clsArray.Description[i]);                    

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewTerminalModel(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "", SearchValue, "Terminal Model", "", "ViewTerminalModel");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalModelID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TerminalModelID[i].ToString());
                    item.SubItems.Add(clsArray.Description[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewTerminalBrand(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "", SearchValue, "Terminal Brand", "", "ViewTerminalBrand");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalBrandID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TerminalBrandID[i].ToString());
                    item.SubItems.Add(clsArray.Description[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewLeaveDetail(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "Search", "Particular Leave Movement", SearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.LeaveNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.LeaveNo[i].ToString());
                    item.SubItems.Add(clsArray.LeaveTypeID[i].ToString());
                    item.SubItems.Add(clsArray.LeaveTypeCode[i]);
                    item.SubItems.Add(clsArray.LeaveTypeDesc[i]);
                    item.SubItems.Add(clsArray.DateFrom[i]);
                    item.SubItems.Add(clsArray.DateTo[i]);                    
                    item.SubItems.Add(double.Parse(clsArray.Duration[i]).ToString("N"));
                    item.SubItems.Add(clsArray.DateType[i]);
                    item.SubItems.Add(clsArray.ReasonID[i].ToString()); ;
                    item.SubItems.Add(clsArray.ReasonDescription[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewWorkArrangement(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "Search", "Particular Work Arrangement", SearchValue, "Particular Detail", "", "ViewParticularDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.WorkArrangementID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.WorkArrangementID[i].ToString());
                    item.SubItems.Add(clsArray.WorkTypeID[i].ToString());
                    item.SubItems.Add(clsArray.Code[i]);
                    item.SubItems.Add(clsArray.Description[i]);
                    item.SubItems.Add(clsArray.DateFrom[i]);
                    item.SubItems.Add(clsArray.DateTo[i]);
                    item.SubItems.Add(double.Parse(clsArray.Duration[i]).ToString("N"));
                    item.SubItems.Add(clsArray.DateType[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewPrivacy(ListView obj, string SearchBy, string SearchValue, bool isClear)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", SearchBy, SearchValue, "Privacy", "", "ViewPrivacy");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.PrivacyID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    
                    item.Checked = (int.Parse(clsArray.isChecked[i]) > 0 ? true : false); // Set checkbox
                    item.SubItems.Add(clsArray.PrivacyID[i].ToString());
                    item.SubItems.Add(clsArray.Description[i]);
                    item.SubItems.Add(clsArray.Form[i]);

                    item.SubItems.Add((isClear ? clsFunction.sNo : dbFunction.setIntegerToYesNo(int.Parse(clsArray.isView[i]))));
                    item.SubItems.Add((isClear ? clsFunction.sNo : dbFunction.setIntegerToYesNo(int.Parse(clsArray.isAdd[i]))));
                    item.SubItems.Add((isClear ? clsFunction.sNo : dbFunction.setIntegerToYesNo(int.Parse(clsArray.isUpdate[i]))));
                    item.SubItems.Add((isClear ? clsFunction.sNo : dbFunction.setIntegerToYesNo(int.Parse(clsArray.isDelete[i]))));
                    item.SubItems.Add((isClear ? clsFunction.sNo : dbFunction.setIntegerToYesNo(int.Parse(clsArray.isPrint[i]))));
                    
                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewAdvanceDetail(ListView obj, string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ID[i].ToString());

                    if (SearchBy.Equals("TimeSheet Terminal"))
                    {
                        item.SubItems.Add(clsArray.TerminalID[i]);
                        item.SubItems.Add(clsArray.TerminalName[i]);
                    }
                    else if (SearchBy.Equals("Country"))
                    {
                        item.SubItems.Add(clsArray.Country[i]);
                    }
                    else
                    {
                        item.SubItems.Add(clsArray.Description[i]);
                    }
                    

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewAServiceDispatch(ListView obj, string StatementType, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ServiceNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    //Debug.WriteLine("i->" + i + ",FSRNo=" + clsArray.FSRNo[i] + ",JobTypeStatusDescription=" + clsArray.JobTypeStatusDescription[i]);
                    bool isFailed = (!dbFunction.isValidID(clsArray.FSRNo[i]) && clsArray.JobTypeStatusDescription[i].Equals(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) ? true : false);

                    int pStatus = int.Parse(clsArray.ServiceStatus[i]);

                    if (isFailed || clsArray.ActionMade[i].Equals(clsGlobalVariables.STATUS_NEGATIVE_DESC))
                    {
                        pStatus = clsGlobalVariables.STATUS_NEGATIVE;
                        clsArray.ServiceStatusDescription[i] = clsGlobalVariables.ACTION_MADE_NEGATIVE;
                    }
                    
                    item.ForeColor = dbFunction.GetColorByStatus(pStatus, clsArray.ServiceStatusDescription[i]); // set forecolor per status

                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                    item.SubItems.Add(clsArray.ClientID[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.FEID[i].ToString());
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.FEName[i]);
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);
                    item.SubItems.Add(clsArray.ServiceReqDate[i]);
                    item.SubItems.Add(clsArray.JobTypeDescription[i]);
                    item.SubItems.Add(clsArray.ActionMade[i]);
                    item.SubItems.Add(clsArray.ReferenceNo[i]);
                    item.SubItems.Add(clsArray.FSRDate[i]);
                    
                    item.SubItems.Add(!isFailed ? clsArray.ServiceStatusDescription[i] : "FAILED");
                    item.SubItems.Add(!isFailed ? clsArray.ActionMade[i] : clsFunction.sDash);

                    item.SubItems.Add(clsArray.FSRNo[i]);
                    item.SubItems.Add(clsArray.ServiceNo[i]);
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.SIMSerialNo[i]);
                    item.SubItems.Add(clsArray.ReplaceTerminalSN[i]);
                    item.SubItems.Add(clsArray.ReplaceSIMSN[i]);
                    item.SubItems.Add(clsArray.ServiceDate[i]);
                    item.SubItems.Add(clsArray.FSRDate[i]);
                    
                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public int GetControlID(string SearchBy)
        {
            int iID = 0;
            ExecuteAPI("GET", "Search", SearchBy, "", "CheckControlID", "", "CheckControlID");

            if (!clsGlobalVariables.isAPIResponseOK)
            {
                return iID;
            }

            iID = clsCheckControlID.ClassControlID;

            return iID;
        }
        public void GetRegionDetailList(string StatementType, string SearchBy, string SearchValue, string MaintenaceType)
        {
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, MaintenaceType, "", "ViewRegionDetail");
        }
        public int GetRegionDetailFromList(string sProvince)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (clsArray.RegionID.Length > i)
                {
                    clsRegionDetail.ClassRegionID = int.Parse(clsArray.RegionID[i]);
                    clsRegionDetail.ClassRegionType = int.Parse(clsArray.RegionType[i]);
                    clsRegionDetail.ClassProvince = clsArray.RegionProvince[i];
                    clsRegionDetail.ClassRegion = clsArray.Region[i];

                    if (sProvince.CompareTo(clsRegionDetail.ClassProvince) == 0)
                    {
                        iID = clsRegionDetail.ClassRegionID;
                        clsSearch.ClassRegionID = clsRegionDetail.ClassRegionID;
                        clsSearch.ClassRegionType = clsRegionDetail.ClassRegionType;
                        clsSearch.ClassRegion = clsRegionDetail.ClassRegion;
                        clsSearch.ClassProvince = clsRegionDetail.ClassProvince;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        public void FillListViewMultiMerchantInfo(ListView obj, string SearchValue)
        {


            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            try
            {
                ExecuteAPI("GET", "View", "Multi-Merchant Info", SearchValue, "IR", "", "ViewAdvanceIR");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (isNoRecordFound() == false)
                {
                    obj.Items.Clear();
                    while (clsArray.IRIDNo.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());
                        item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                        item.SubItems.Add(clsArray.TID[i].ToString());
                        item.SubItems.Add(clsArray.MID[i].ToString());
                        item.SubItems.Add(clsArray.IRNo[i].ToString());
                        item.SubItems.Add(clsArray.IRStatusDescription[i].ToString());

                        obj.Items.Add(item);

                        i++;
                    }

                    dbFunction.ListViewAlternateBackColor(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exceptional error " + ex.Message);
            }
            
        }

        public void FillListViewTerminalDetail(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Release Movement Master", SearchValue, "Terminal Detail", "", "ViewTerminalDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TransNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TransNo[i].ToString());
                    item.SubItems.Add(clsArray.TransDate[i]);
                    item.SubItems.Add(clsArray.TransNo[i].ToString());
                    item.SubItems.Add(clsArray.FromLocationID[i]);
                    item.SubItems.Add(clsArray.FromLocation[i]);
                    item.SubItems.Add(clsArray.ToLocationID[i]);
                    item.SubItems.Add(clsArray.ToLocation[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);
                    item.SubItems.Add(clsArray.ReferenceNo[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);
                    item.SubItems.Add(clsArray.ProcessedBy[i]);
                    item.SubItems.Add(clsArray.ProcessedDateTime[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewSIMDetail(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Release Movement Master", SearchValue, "Terminal Detail", "", "ViewSIMDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TransNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.TransNo[i].ToString());
                    item.SubItems.Add(clsArray.TransDate[i]);
                    item.SubItems.Add(clsArray.TransNo[i].ToString());
                    item.SubItems.Add(clsArray.FromLocationID[i]);
                    item.SubItems.Add(clsArray.FromLocation[i]);
                    item.SubItems.Add(clsArray.ToLocationID[i]);
                    item.SubItems.Add(clsArray.ToLocation[i]);
                    item.SubItems.Add(clsArray.RequestNo[i]);
                    item.SubItems.Add(clsArray.ReferenceNo[i]);
                    item.SubItems.Add(clsArray.Remarks[i]);
                    item.SubItems.Add(clsArray.ProcessedBy[i]);
                    item.SubItems.Add(clsArray.ProcessedDateTime[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewInvoiceMaster(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Invoice Master", SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.InvoiceID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.InvoiceID[i].ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.InvoiceNo[i].ToString());
                    item.SubItems.Add(clsArray.AccountNo[i].ToString());
                    item.SubItems.Add(clsArray.CustomerNo[i].ToString());
                    item.SubItems.Add(clsArray.ParticularName[i].ToString());
                    item.SubItems.Add(clsArray.InvoiceDate[i].ToString());
                    item.SubItems.Add(clsArray.ReferenceNo[i].ToString());
                    item.SubItems.Add(clsArray.DateCoveredFrom[i].ToString() + " to " + clsArray.DateCoveredTo[i].ToString());
                    item.SubItems.Add(clsArray.DateDue[i].ToString());
                    item.SubItems.Add(Convert.ToDouble(clsArray.TAmtDue[i]).ToString("N"));
                    item.SubItems.Add(clsFunction.sDefaultAmount);
                    item.SubItems.Add(clsArray.ProcessedBy[i]);
                    item.SubItems.Add(clsArray.ProcessedDateTime[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewMobileList(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Mobile List", SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.MobileID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MobileID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MobileTerminalID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MobileTerminalName));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FullName));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewChangesMapping(ListView obj, string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;
            string pTemp = "";
            string pOutValue = "";

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "eFSR Changes Mapping", SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.MapID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MapID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Description));

                    // Get info detail
                    string pFieldTable = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TableName);
                    string pFieldName = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldName);
                    string pFieldSearchKey = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldSearchKey);
                    int pFieldType = int.Parse(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldType));

                    // for json                    
                    string pFieldNestedObj = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldNestedObj);
                    string pFieldKeyValue = dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldKeyValue);

                    pOutValue = clsFunction.sDash;
                    dbAPI.ExecuteAPI("GET", "Search", "Get Info By Field",
                                                pFieldTable + clsFunction.sPipe +
                                                pFieldName + clsFunction.sPipe +
                                                pFieldSearchKey + clsFunction.sPipe +
                                                (pFieldTable.Equals(clsDefines.TAG_tblparticular) ? clsSearch.ClassMerchantID : clsSearch.ClassIRIDNo)
                                                , "Get Info Detail", "", "GetInfoDetail");

                    if (dbAPI.isNoRecordFound() == false)
                        pTemp = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, "outValue");

                    Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);
                    Debug.WriteLine("pFieldTable=" + pFieldTable);
                    Debug.WriteLine("pFieldName=" + pFieldName);
                    Debug.WriteLine("pFieldSearchKey=" + pFieldSearchKey);
                    Debug.WriteLine("pFieldType=" + pFieldType);
                    Debug.WriteLine("pFieldNestedObj=" + pFieldNestedObj);
                    Debug.WriteLine("pFieldKeyValue=" + pFieldKeyValue);
                    Debug.WriteLine("pTemp="+ pTemp);

                    pOutValue = "";
                   
                    switch (pFieldType)
                    {   
                        case (int)OptionType.Others:      // 3
                            pOutValue = pTemp;
                            break;
                        default:
                            pOutValue = dbFunction.getJSONTagValue(pTemp, pFieldKeyValue, "", "");
                            break;
                    }

                    Debug.WriteLine("pOutValue=" + pOutValue);

                    item.SubItems.Add(pOutValue);
                    item.SubItems.Add(clsFunction.sDash);
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MaxLimit));
                    item.SubItems.Add(dbFunction.CheckAndSetNumericValue(pFieldType.ToString()));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FieldName));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public void FillListViewStockMovementDetail(ListView obj, string SearchBy, string SearchValue, bool isCurrent)
        {
            int i = 0;
            int iLineNo = 0;
            int pItemID = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();
            clsSearch.ClassComponents = "";

            ExecuteAPI("GET", "View", SearchBy, SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    if (isCurrent)
                        pItemID = int.Parse(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ItemID));
                    else
                        pItemID = int.Parse(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceItemID));

                    if (pItemID > 0)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(pItemID.ToString());

                        // get component info
                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Stock Detail Info", pItemID.ToString());

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalTypeID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModelID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SerialNo));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Location));

                        if (dbFunction.isValidID(pItemID.ToString()))
                        {
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_StockStatusDescription));
                        }
                        else
                        {
                            item.SubItems.Add(clsFunction.sDash);
                        }

                        obj.Items.Add(item);

                        clsSearch.ClassComponents += dbFunction.padLeftChar(iLineNo.ToString(), "0", 2) + "." +
                                                     dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel) + " " + clsFunction.sPipe +
                                                     dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SerialNo) + Environment.NewLine;
                    }
                    
                    i++;
                    
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }
        }

        public bool CheckRegionDetail(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }
        public bool CheckTerminalDetail(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }
        public bool CheckSIMDetail(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool CheckParticular(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool CheckUser(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }
        
        public void GetJobTypeDetails(JobType iJobType)
        {
            switch (iJobType)
            {
                case JobType.iInstallation:
                    clsSearch.ClassLastServiceRequest = clsGlobalVariables.TA_STATUS_INSTALLED_DESC;
                    clsSearch.ClassJobType = clsGlobalVariables.TA_STATUS_INSTALLED;
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC;
                    break;
                case JobType.iServicing:
                    clsSearch.ClassLastServiceRequest = clsGlobalVariables.TA_STATUS_SERVICING_DESC;
                    clsSearch.ClassJobType = clsGlobalVariables.TA_STATUS_SERVICING;
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC;
                    break;
                case JobType.iPullOut:
                    clsSearch.ClassLastServiceRequest = clsGlobalVariables.TA_STATUS_PULLEDOUT_DESC;
                    clsSearch.ClassJobType = clsGlobalVariables.TA_STATUS_PULLEDOUT;
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_PULLEDOUT_SUB_DESC;
                    break;
                case JobType.iReplacement:
                    clsSearch.ClassLastServiceRequest = clsGlobalVariables.TA_STATUS_REPLACEMENT_DESC;
                    clsSearch.ClassJobType = clsGlobalVariables.TA_STATUS_REPLACEMENT;
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPLACEMENT_SUB_DESC;
                    break;
                case JobType.iReprogramming:
                    clsSearch.ClassLastServiceRequest = clsGlobalVariables.TA_STATUS_REPROGRAMMED_DESC;
                    clsSearch.ClassJobType = clsGlobalVariables.TA_STATUS_REPROGRAMMED;
                    clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                    clsSearch.ClassJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPROGRAMMED_SUB_DESC;
                    break;
            }
        }
        public void GetJobTypeStatusDetails(JobTypeStatus iJobTypeStatus)
        {
            switch (iJobTypeStatus)
            {
                case JobTypeStatus.iPending:
                    clsSearch.ClassJobTypeStatus = clsGlobalVariables.JOB_TYPE_STATUS_PENDING;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC;
                    break;
                case JobTypeStatus.iReProcessing:
                    clsSearch.ClassJobTypeStatus = clsGlobalVariables.JOB_TYPE_STATUS_REPROCESSING;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_REPROCESSING_DESC;
                    break;
                case JobTypeStatus.iReadyToProcess:
                    clsSearch.ClassJobTypeStatus = clsGlobalVariables.JOB_TYPE_STATUS_READY_TO_PROCESS;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_READY_TO_PROCESS_DESC;
                    break;
                case JobTypeStatus.iProcessing:
                    clsSearch.ClassJobTypeStatus = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC;
                    break;
                case JobTypeStatus.iCompleted:
                    clsSearch.ClassJobTypeStatus = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED;
                    clsSearch.ClassJobTypeStatusDescription = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;
                    break;
            }
        }
        
        public void GetServiceTypeDetail(ServiceType iServiceType)
        {
            switch (iServiceType)
            {
                case ServiceType.iInstalled:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_INSTALLED;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_INSTALLED_DESC;
                    break;
                case ServiceType.iNegative:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_NEGATIVE;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_NEGATIVE_DESC;
                    break;
                case ServiceType.iReprogrammed:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_REPROGRAMMED;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_REPROGRAMMED_DESC;
                    break;
                case ServiceType.iPullOut:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_PULLEDOUT;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_PULLEDOUT_DESC;
                    break;
                case ServiceType.iDiagnostic:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_DIAGNOSTIC;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_DIAGNOSTIC_DESC;
                    break;
                case ServiceType.iReplacement:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_REPLACEMENT;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_REPLACEMENT_DESC;
                    break;
                case ServiceType.iServicing:
                    clsSearch.ClassServiceTypeStatus = clsGlobalVariables.TA_STATUS_SERVICING;
                    clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.TA_STATUS_SERVICING_DESC;
                    break;
            }
        }

        public void GetReasonType(ReasonType iReasonType)
        {
            switch (iReasonType)
            {
                case ReasonType.iReason:
                    clsSearch.ClassReasonType = clsGlobalVariables.REASON_TYPE;
                    break;
                case ReasonType.iResolution:
                    clsSearch.ClassReasonType = clsGlobalVariables.RESOLUTION_TYPE;
                    break;
            }
        }

        public void ResetAdvanceSearch()
        {
            Debug.WriteLine("--ResetAdvanceSearch--");

            clsSearch.ClassStatementType = clsFunction.sNull;
            clsSearch.ClassSearchBy = clsFunction.sNull;            
            clsSearch.ClassSearchValue = clsFunction.sNull;
            clsSearch.ClassAdvanceSearchValue = clsFunction.sNull;
            clsSearch.ClassHoldAdvanceSearchValue = clsFunction.sNull;
            clsSearch.ClassServiceTypeID = clsFunction.iZero;
            clsSearch.ClassParticularID = clsFunction.iZero;
            clsSearch.ClassTerminalTypeID = clsFunction.iZero;
            clsSearch.ClassTerminalModelID = clsFunction.iZero;
            clsSearch.ClassTerminalBrandID = clsFunction.iZero;
            clsSearch.ClassTerminalID = clsFunction.iZero;
            clsSearch.ClassTerminalSN = clsFunction.sZero;
            clsSearch.ClassServiceNo = clsFunction.iZero;
            clsSearch.ClassTAIDNo = clsFunction.iZero;
            clsSearch.ClassIRIDNo = clsFunction.iZero;
            clsSearch.ClassIRNo = clsFunction.sZero;
            clsSearch.ClassClientID = clsFunction.iZero;
            clsSearch.ClassServiceProviderID = clsFunction.iZero;
            clsSearch.ClassMerchantID = clsFunction.iZero;
            clsSearch.ClassFEID = clsFunction.iZero;
            clsSearch.ClassTID = clsFunction.sZero;
            clsSearch.ClassMID = clsFunction.sZero;
            clsSearch.ClassRegionID = clsFunction.iZero;
            clsSearch.ClassRegionType = clsFunction.iZero;
            clsSearch.ClassRegion = clsFunction.sNull;
            clsSearch.ClassProvinceID = clsFunction.iZero;
            clsSearch.ClassProvince = clsFunction.sNull;            
            clsSearch.ClassTerminalStatusType = clsFunction.iZero;
            clsSearch.ClassReqDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassReqDateTo = clsFunction.sDateFormat;
            clsSearch.ClassInstDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassInstDateTo = clsFunction.sDateFormat;
            clsSearch.ClassTADateFrom = clsFunction.sDateFormat;
            clsSearch.ClassTADateTo = clsFunction.sDateFormat;
            clsSearch.ClassFSRDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassFSRDateTo = clsFunction.sDateFormat;
            clsSearch.ClassIRImportDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassIRImportDateTo = clsFunction.sDateFormat;
            clsSearch.ClassServiceDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassServiceDateTo = clsFunction.sDateFormat;
            clsSearch.ClassIRStatus = clsFunction.iZero;
            clsSearch.ClassIRStatusDescription = clsFunction.sZero;
            clsSearch.ClassSIMID = clsFunction.iZero;
            clsSearch.ClassSIMSerialNo = clsFunction.sZero;
            clsSearch.ClassSIMStatus = clsFunction.iZero;
            clsSearch.ClassSIMCarrier = clsFunction.sZero;
            clsSearch.ClassFSRStatus = clsFunction.iZero;
            clsSearch.ClassDockID = clsFunction.iZero;
            clsSearch.ClassDockSN = clsFunction.sZero;
            clsSearch.ClassFEName = clsFunction.sNull;
            clsSearch.ClassParticularName = clsFunction.sNull;
            clsSearch.ClassClientName = clsFunction.sNull;
            clsSearch.ClassMerchantName = clsFunction.sNull;
            clsSearch.ClassStatus = clsFunction.iZero;
            clsSearch.ClassStatusDescription = clsFunction.sZero;
            clsSearch.ClassJobType = clsFunction.iZero;
            clsSearch.ClassJobTypeStatusDescription = clsFunction.sZero;
            clsSearch.ClassJobTypeDescription = clsFunction.sZero;
            clsSearch.ClassUserID = clsFunction.iZero;
            clsSearch.ClassUserType = clsFunction.sZero;
            clsSearch.ClassLogDateFrom = clsFunction.sDateFormat;
            clsSearch.ClassLogDateTo = clsFunction.sDateFormat;
            clsSearch.ClassUserType = clsFunction.sZero;
            clsSearch.ClassLogSessionStatus = clsFunction.iZero;
            clsSearch.ClassTerminalStatus = clsFunction.iZero;
            clsSearch.ClassActionMade = clsFunction.sZero;
            clsSearch.ClassIsClose = clsFunction.sNone;
            clsSearch.ClassParticularUserKey = clsFunction.sNull;
            clsSearch.ClassJobTypeDescriptionList = clsFunction.sNull;
            clsSearch.ClassJobTypeStatusDescriptionList = clsFunction.sNull;
            clsSearch.ClassIsOverDue = clsFunction.sZero;
            clsSearch.ClassNoOfDayPending = clsFunction.sZero;
            clsSearch.ClassReasonType = clsFunction.sZero;

            clsSearch.ClassCurrentPage = clsFunction.iZero;
            clsSearch.ClassTotalPage = clsFunction.iZero;
        }

        public void ResetAdvanceArray()
        {

            clsArray.IRIDNo.Initialize();
            clsArray.IRNo.Initialize();
            clsArray.TerminalID.Initialize();

        }
        public int GetActionMadeFromList(string sAction)
        {
            int i = 0;
            int iID = 0;

            try
            {
                while (GetActionMade().Length > i)
                {
                    clsSearch.ClassActionMade = GetActionMade()[i];

                    if (sAction.CompareTo(clsSearch.ClassActionMade) == 0)
                    {
                        iID = 1;
                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                iID = 0;
            }

            return iID;
        }

        public string[] GetActionMade()
        {
            string[] ret = { clsFunction.sDefaultSelect, "SUCCESS", "NEGATIVE" };

            return ret;
        }

        public void FillComboBoxActionMade(ComboBox obj)
        {
            int i = 0;
            
            obj.Items.Clear();
            while (GetActionMade().Length > i)
            {
                obj.Items.Add(GetActionMade()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string[] GetBillable()
        {
            string[] ret = { clsFunction.sDefaultSelect, "BILLABLE", "NON-BILLABLE" };

            return ret;
        }

        public void FillComboBoxBillable(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetBillable().Length > i)
            {
                obj.Items.Add(GetBillable()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void GetFSRStatus(string sActionMade)
        {
            if (sActionMade.CompareTo(GetActionMade()[1]) == 0) // SUCCESS
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_INSTALLED;
                clsSearch.ClassStatusCode = clsGlobalVariables.TA_STATUS_INSTALLED_CODE;
                clsSearch.ClassStatusDescription = clsGlobalVariables.TA_STATUS_INSTALLED_DESC;
            }

            if (sActionMade.CompareTo(GetActionMade()[2]) == 0) // NEGATIVE
            {
                clsSearch.ClassStatus = clsGlobalVariables.STATUS_NEGATIVE;
                clsSearch.ClassStatusCode = clsGlobalVariables.TA_STATUS_NEGATIVE_CODE;
                clsSearch.ClassStatusDescription = clsGlobalVariables.TA_STATUS_NEGATIVE_DESC;                
            }            
        }

        //public void GetFSRServiceTypeStatus(string sJobTypeDescription)
        //{
        //    if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) == 0) // JOB_TYPE_INSTALLATION_DESC
        //    {
        //        clsSearch.ClassServiceTypeStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassTerminalStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassTerminalStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassIRActive = clsGlobalVariables.IR_ACTIVE;
        //    }

        //    if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) == 0) // JOB_TYPE_SERVICING_DESC
        //    {
        //        clsSearch.ClassServiceTypeStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassTerminalStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassTerminalStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassIRActive = clsGlobalVariables.IR_ACTIVE;
        //    }

        //    if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) == 0) // JOB_TYPE_PULLOUT_DESC
        //    {
        //        clsSearch.ClassServiceTypeStatus = clsGlobalVariables.STATUS_PULLED_OUT;
        //        clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;
        //        clsSearch.ClassTerminalStatus = clsGlobalVariables.STATUS_AVAILABLE;
        //        clsSearch.ClassTerminalStatusDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;
        //        clsSearch.ClassIRActive = clsGlobalVariables.IR_INACTIVE;
        //    }

        //    if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0) // JOB_TYPE_REPLACEMENT_DESC
        //    {
        //        clsSearch.ClassServiceTypeStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassTerminalStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassTerminalStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassIRActive = clsGlobalVariables.IR_ACTIVE;
        //    }

        //    if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC) == 0) // JOB_TYPE_REPROGRAMMING_DESC
        //    {
        //        clsSearch.ClassServiceTypeStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassServiceTypeStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassTerminalStatus = clsGlobalVariables.STATUS_INSTALLED;
        //        clsSearch.ClassTerminalStatusDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;
        //        clsSearch.ClassIRActive = clsGlobalVariables.IR_ACTIVE;
        //    }
        //}

        /*
        public void GetJobTypeDescriptionByServiceType(string sServiceType)
        {
            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_NEGATIVE_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_NEGATIVE_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_REPROGRAMMED_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_PULLEDOUT_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_DIAGNOSTIC_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_REPLACEMENT_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;

            if (sServiceType.Equals(clsGlobalVariables.TA_STATUS_DISPATCH_SUB_DESC))
                clsSearch.ClassJobTypeDescription = clsGlobalVariables.JOB_TYPE_DISPATCH_DESC;


        }
        */

        public void UpdateTerminalDetailStatus(string sTerminalID, int iStatusID, string sStatusDescription)
        {            
            int iTerminalID = int.Parse((sTerminalID.Length > 0 ? sTerminalID : clsFunction.sZero));
            if (iTerminalID > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTerminalID + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription;

                Debug.WriteLine("UpdateTerminalDetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Terminal Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateSIMDetailStatus(string sSIMID, int iStatusID, string sStatusDescription)
        {            
            int iSIMID = int.Parse((sSIMID.Length > 0 ? sSIMID : clsFunction.sZero));
            if (iSIMID > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sSIMID + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription;

                Debug.WriteLine("UpdateSIMDetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update SIM Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateIRDetailStatus(string sIRIDNo, int iStatusID, string sStatusDescription)
        {
            int iIRIDNo = int.Parse((sIRIDNo.Length > 0 ? sIRIDNo : clsFunction.sZero));
            if (iIRIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sIRIDNo + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription;

                Debug.WriteLine("UpdateIRDetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update IR Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateIRDetailActive(string sIRIDNo, int iActive)
        {
            int iIRIDNo = int.Parse((sIRIDNo.Length > 0 ? sIRIDNo : clsFunction.sZero));
            if (iIRIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sIRIDNo + clsFunction.sPipe +
                                                    iActive.ToString();

                Debug.WriteLine("UpdateIRDetailActive::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update IR Detail Active", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateTADetailStatus(string sTAIDNo, int iStatusID, string sStatusDescription)
        {            
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            if (iTAIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription;

                Debug.WriteLine("UpdateTADetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update TA Detail Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateTADetailStatus2(string sTAIDNo, int iStatusID, string sStatusDescription, string sJobTypeStatusDescription)
        {
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            if (iTAIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription + clsFunction.sPipe +
                                                    sJobTypeStatusDescription;

                Debug.WriteLine("UpdateTADetailStatus2::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update TA Detail Status2", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateServiceStatus(string sServiceNo, string sRequestNo, int iStatusID, string sStatusDescription, string sIRIDNo)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0 && sRequestNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sRequestNo + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription + clsFunction.sPipe +
                                                    sIRIDNo;

                Debug.WriteLine("UpdateServicingDetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateServiceStatus2(string sServiceNo, string sRequestNo, int iStatusID, string sStatusDescription, string sJobTypeStatusDescription)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0 && sRequestNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sRequestNo + clsFunction.sPipe +
                                                    iStatusID.ToString() + clsFunction.sPipe +
                                                    sStatusDescription + clsFunction.sPipe +
                                                    sJobTypeStatusDescription;

                Debug.WriteLine("UpdateServiceStatus2::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Status2", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateServicingDispatch(string sServiceNo, string sIRIDNo, string sDispatchDateTime, string sDispatchDate, string sDispatchTime, string sDispatchBy)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0 && sIRIDNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sIRIDNo + clsFunction.sPipe +
                                                    sDispatchDate + clsFunction.sPipe +
                                                    sDispatchTime + clsFunction.sPipe +
                                                    sDispatchBy + clsFunction.sPipe +
                                                    sDispatchDateTime;

                Debug.WriteLine("UpdateServicingDispatch::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Dispatch", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        //public void UpdateServiceClose(string sServiceNo, string sTAIDNo, string sRequestNo, string sJobTypeDescription, string sActionMade, int isClose)
        //{
        //    int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
        //    if (iServiceNo > 0 && sRequestNo.Length > 0)
        //    {
        //        clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
        //                                            sTAIDNo + clsFunction.sPipe +
        //                                            sRequestNo + clsFunction.sPipe +
        //                                            sJobTypeDescription + clsFunction.sPipe +
        //                                            sActionMade + clsFunction.sPipe +
        //                                            isClose.ToString();

        //        Debug.WriteLine("UpdateServiceClose::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

        //        ExecuteAPI("PUT", "Update", "Update Servicing Close", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        //    }
        //}

        public void UpdateServiceJobType(string sServiceNo, string sRequestNo, int JobType, string JobTypeDescription, string JobTypeStatusDescription, string JobTypeSubDescription)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0 && sRequestNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sRequestNo + clsFunction.sPipe +
                                                    JobType.ToString() + clsFunction.sPipe +
                                                    JobTypeDescription + clsFunction.sPipe +
                                                    JobTypeStatusDescription + clsFunction.sPipe +
                                                    JobTypeSubDescription;

                Debug.WriteLine("UpdateServiceJobType::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Job Type", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateServicingActionMade(string sServiceNo, string sRequestNo, string sActionMade)
        {            
            clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                sRequestNo + clsFunction.sPipe +
                                                sActionMade;

            Debug.WriteLine("UpdateServicingActionMade::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("PUT", "Update", "Update Servicing Action Made", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        }

        //public void UpdateIRClose(string sIRIDNo, string sIRNo, string sActionMade)
        //{            
        //    int isClose = clsFunction.iZero;

        //    if (sIRIDNo.Length > 0)
        //    {
        //        if (sActionMade.CompareTo(dbAPI.GetActionMade()[1]) == 0) // SUCCESS
        //        {
        //            isClose = clsGlobalVariables.SVC_REQ_CLOSE;
        //        }

        //        if (sActionMade.CompareTo(dbAPI.GetActionMade()[2]) == 0) // NEGATIVE
        //        {
        //            isClose = clsGlobalVariables.SVC_REQ_OPEN;
        //        }

        //        clsSearch.ClassAdvanceSearchValue = sIRIDNo + clsFunction.sPipe +
        //                                       sIRNo + clsFunction.sPipe +
        //                                       isClose;

        //        Debug.WriteLine("UpdateIRClose::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

        //        dbAPI.ExecuteAPI("PUT", "Update", "Update IR Close", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        //    }
        //}

        /*
        public void UpdateJobTypeDetailStatus(string sTAIDNo, string sIRIDNo)
        {
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            int iIRIDNo = int.Parse((sIRIDNo.Length > 0 ? sIRIDNo : clsFunction.sZero));

            if (iTAIDNo > 0 && iIRIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = iTAIDNo + clsFunction.sPipe +
                                                    iIRIDNo + clsFunction.sPipe +
                                                    clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED + clsFunction.sPipe +
                                                    clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC + clsFunction.sPipe +
                                                    clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC;

                Debug.WriteLine("UpdateJobTypeDetailStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Job Type TA Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }            
        }
        */

        public void UpdateServicingCurrentTerminalStatus(string sServiceNo, string sRequestNo)
        {
            if (sRequestNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                    sRequestNo + clsFunction.sPipe +
                    clsSearch.ClassCurTerminalSNStatus + clsFunction.sPipe + clsSearch.ClassCurTerminalSNStatusDescription + clsFunction.sPipe +
                    clsSearch.ClassCurSIMSNStatus + clsFunction.sPipe + clsSearch.ClassCurSIMSNStatusDescription + clsFunction.sPipe +
                    clsSearch.ClassCurDockSNStatus + clsFunction.sPipe + clsSearch.ClassCurDockSNStatusDescription;

                Debug.WriteLine("UpdateServicingCurrentTerminalStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Current Terminal Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateServicingReplaceTerminalStatus(string sServiceNo, string sRequestNo)
        {
            if (sRequestNo.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                    sRequestNo + clsFunction.sPipe +                    
                    clsSearch.ClassRepTerminalSNStatus + clsFunction.sPipe + clsSearch.ClassRepTerminalSNStatusDescription + clsFunction.sPipe +
                    clsSearch.ClassRepSIMSNStatus + clsFunction.sPipe + clsSearch.ClassRepSIMSNStatusDescription + clsFunction.sPipe +
                    clsSearch.ClassRepDockSNStatus + clsFunction.sPipe + clsSearch.ClassRepDockSNStatusDescription;

                Debug.WriteLine("UpdateServicingReplaceTerminalStatus::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Servicing Replace Terminal Status", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateTAReplacement(string sTAIDNo, string sTerminalID, string sSIMID, string sDockID, 
                                        string sTerminalTypeID, string sTerminalModelID, string sTerminalBrandID, 
                                        string sTerminalSN, string sSIMSerialNo, string sDockSN)
        {

            if (sTAIDNo.Length > 0 && sTAIDNo.CompareTo(clsFunction.sZero) != 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                    sTerminalID + clsFunction.sPipe +
                                                    sSIMID + clsFunction.sPipe +
                                                    sDockID + clsFunction.sPipe +
                                                    sTerminalTypeID + clsFunction.sPipe +
                                                    sTerminalModelID + clsFunction.sPipe +
                                                    sTerminalBrandID + clsFunction.sPipe +
                                                    sTerminalSN + clsFunction.sPipe +
                                                    sSIMSerialNo + clsFunction.sPipe +
                                                    sDockSN;

                Debug.WriteLine("UpdateTAReplacement::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update TA Replacement", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }

        }
        public void UpdateSerialNoList(string sFSRNo, string sSIMSN, string sPowerSN, string sDockSN)
        {
            clsSearch.ClassAdvanceSearchValue = sFSRNo + clsFunction.sPipe +
                                                sSIMSN + clsFunction.sPipe +
                                                sPowerSN + clsFunction.sPipe +
                                                sDockSN;

            Debug.WriteLine("UpdateSerialNoList::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            dbAPI.ExecuteAPI("PUT", "Update", "Update SerialNo List", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
        }

        public void UpdateServiceRemarks(string sServiceNo, string sRemarks)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0 )
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sRemarks;

                Debug.WriteLine("UpdateServiceRemarks::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Service Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateFSRServiceNo(string sFSRNo, string sServiceNo)
        {
            int iFSRNo = int.Parse((sFSRNo.Length > 0 ? sFSRNo : clsFunction.sZero));
            if (iFSRNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sFSRNo + clsFunction.sPipe +
                                                    sServiceNo;

                Debug.WriteLine("UpdateServiceRemarks::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update FSR ServiceNo", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateFSRDetail(string sFSRNo, string sServiceNo, string sProblemReported, string sActualProblemReported, string sActionTaken, string sAnyComments)
        {
            int iFSRNo = int.Parse((sFSRNo.Length > 0 ? sFSRNo : clsFunction.sZero));
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iFSRNo > 0 && iServiceNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sFSRNo + clsFunction.sPipe +
                                                    sServiceNo + clsFunction.sPipe +
                                                    sProblemReported + clsFunction.sPipe +
                                                    sActualProblemReported + clsFunction.sPipe +
                                                    sActionTaken + clsFunction.sPipe +
                                                    sAnyComments + clsFunction.sPipe +
                                                    clsUser.ClassModifiedBy + clsFunction.sPipe +
                                                    clsUser.ClassModifiedDateTime;

                Debug.WriteLine("UpdateServiceRemarks::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update FSR Detail", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateModifiedBy(string sTableName, string sID, string sModifiedBy, string sModifiedDateTime)
        {
            int ID = int.Parse((sID.Length > 0 ? sID : clsFunction.sZero));
            if (ID > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTableName + clsFunction.sPipe +
                                                    ID + clsFunction.sPipe +
                                                    sModifiedBy + clsFunction.sPipe +
                                                    sModifiedDateTime;

                Debug.WriteLine("UpdateModifiedBy::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Modified By", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateProcessedBy(string sTableName, string sID, string sProcessedBy, string sProcessedDateTime)
        {
            int ID = int.Parse((sID.Length > 0 ? sID : clsFunction.sZero));
            if (ID > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTableName + clsFunction.sPipe +
                                                    ID + clsFunction.sPipe +
                                                    sProcessedBy + clsFunction.sPipe +
                                                    sProcessedDateTime;

                Debug.WriteLine("UpdateModifiedBy::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Processed By", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void DeleteIRDetail(string sIRIDNo, string sIRNo)
        {
            int iIRIDNo = int.Parse((sIRIDNo.Length > 0 ? sIRIDNo : clsFunction.sZero));
            if (iIRIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sIRIDNo + clsFunction.sPipe +
                                                    sIRNo;

                Debug.WriteLine("DeleteIRDetail::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("DELETE", "Delete", "IR", clsSearch.ClassAdvanceSearchValue, "IR Detail", "", "DeleteCollectionDetail");
            }
        }

        public void DeleteTADetail(string sTAIDNo)
        {
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            if (iTAIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo;

                Debug.WriteLine("DeleteTADetail::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("DELETE", "Delete", "TA", clsSearch.ClassAdvanceSearchValue, "TA Detail", "", "DeleteCollectionDetail");
            }
        }
        public void DeleteServicingDetail(string sServiceNo)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo;

                Debug.WriteLine("DeleteServicingDetail::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("DELETE", "Delete", "SD", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "DeleteCollectionDetail");
            }
        }

        public void DeleteERMTempDetail()
        {
            ExecuteAPI("DELETE", "Delete", "ERM", "ERM Temp Detail", "ERM Temp Detail", "", "DeleteCollectionDetail");
        }

        public void ProcessERMTempDetail(string sPartircularID, string sParticularName)
        {
            int iParticularID = int.Parse((sPartircularID.Length > 0 ? sPartircularID : clsFunction.sZero));
            if (iParticularID > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sPartircularID + clsFunction.sPipe + sParticularName;

                Debug.WriteLine("ProcessERMTempDetail::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("POST", "Process", "Client", clsSearch.ClassAdvanceSearchValue, "ERM Temp Detail", "", "ProcessERMDetail");
            }

                
        }

        //public void UpdateServicingDetailStatus(JobType iJobType, string sServiceNo, string sRequestNo, string sIRIDNo)
        //{           
        //    if (sServiceNo.Length > 0)
        //    {
        //        switch (iJobType)
        //        {
        //            case JobType.iPullOut:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_PULLED_OUT, clsGlobalVariables.STATUS_PULLED_OUT_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iInstallation:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_INSTALLED, clsGlobalVariables.STATUS_INSTALLED_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iReplacement:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_REPLACEMENT, clsGlobalVariables.STATUS_REPLACEMENT_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iReprogramming:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_REPROGRAMMED, clsGlobalVariables.STATUS_REPROGRAMMED_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iServicing:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_SERVICING, clsGlobalVariables.STATUS_SERVICING_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iCancelled:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_CANCELLED, clsGlobalVariables.STATUS_CANCELLED_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iInstalled:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_INSTALLED, clsGlobalVariables.STATUS_INSTALLED_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iNegative:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_NEGATIVE, clsGlobalVariables.STATUS_NEGATIVE_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iDispatch:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_DISPATCH, clsGlobalVariables.STATUS_DISPATCH_DESC, sIRIDNo);  // Update Status
        //                break;
        //            case JobType.iAllocated:
        //                UpdateServiceStatus(sServiceNo, sRequestNo, clsGlobalVariables.STATUS_ALLOCATED, clsGlobalVariables.STATUS_ALLOCATED_DESC, sIRIDNo);  // Update Status
        //                break;
        //        }
        //    }
        //}

        public void UpdateServiceCode(string sServiceNo, string sServiceCode)
        {
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iServiceNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sServiceNo + clsFunction.sPipe +
                                                    sServiceCode;

                Debug.WriteLine("UpdateServiceCode::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update Service Code", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        /*
        public void CheckJobTypeDescription(string sJobTypeDescription)
        {
            if (sJobTypeDescription.Length > 0)
            {
                if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) == 0)
                {
                    GetJobTypeDetails(clsAPI.JobType.iInstallation);
                }
                if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) == 0)
                {
                    GetJobTypeDetails(clsAPI.JobType.iServicing);
                }
                if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) == 0)
                {
                    GetJobTypeDetails(clsAPI.JobType.iPullOut);
                }
                if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0)
                {
                    GetJobTypeDetails(clsAPI.JobType.iReplacement);
                }
                if (sJobTypeDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC) == 0)
                {
                    GetJobTypeDetails(clsAPI.JobType.iReprogramming);
                }
            }
            else
            {
                dbFunction.SetMessageBox("Invalid service request. Please contact administrator.", "Invalid Service Request", clsFunction.IconType.iExclamation);
            }

        }
        */

        public int GetStatus(string sDescription)
        {
            int iStatus = 0;

            if (sDescription.Length > 0)
            {
                if (sDescription.CompareTo(clsGlobalVariables.STATUS_AVAILABLE_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_AVAILABLE;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_ALLOCATED_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_ALLOCATED;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_REPAIR_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_REPAIR;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_DAMAGE_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_DAMAGE;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_LOSS_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_LOSS;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_BORROWED_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_BORROWED;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_INSTALLED_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_INSTALLED;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_DISPATCH_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_DISPATCH;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_NEGATIVE_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_NEGATIVE;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_REPROGRAMMED_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_REPROGRAMMED;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_PULLED_OUT_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_PULLED_OUT;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_DIAGNOSTIC_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_DIAGNOSTIC;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_REPLACEMENT_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_REPLACEMENT;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_SERVICING_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_SERVICING;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_INSTALLATION_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_INSTALLATION;
                else if (sDescription.CompareTo(clsGlobalVariables.STATUS_CANCELLED_DESC) == 0)
                    iStatus = clsGlobalVariables.STATUS_CANCELLED;                
            }
            else
            {
                iStatus = 0;
            }

            return iStatus;
        }
        public string GetStatusDescription(int iStatus)
        {
            string sDescription = "";

            if (iStatus > 0)
            {
                if (iStatus == clsGlobalVariables.STATUS_AVAILABLE)                
                    sDescription = clsGlobalVariables.STATUS_AVAILABLE_DESC;               
                else if (iStatus == clsGlobalVariables.STATUS_ALLOCATED)                
                    sDescription = clsGlobalVariables.STATUS_ALLOCATED_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_REPAIR)                
                    sDescription = clsGlobalVariables.STATUS_REPAIR_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_DAMAGE)                
                    sDescription = clsGlobalVariables.STATUS_DAMAGE_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_LOSS)                
                    sDescription = clsGlobalVariables.STATUS_LOSS_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_BORROWED)                
                    sDescription = clsGlobalVariables.STATUS_BORROWED_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_INSTALLED)                
                    sDescription = clsGlobalVariables.STATUS_INSTALLED_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_DISPATCH)                
                    sDescription = clsGlobalVariables.STATUS_DISPATCH_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_NEGATIVE)                
                    sDescription = clsGlobalVariables.STATUS_NEGATIVE_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_REPROGRAMMED)                
                    sDescription = clsGlobalVariables.STATUS_REPROGRAMMED_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_PULLED_OUT)                
                    sDescription = clsGlobalVariables.STATUS_PULLED_OUT_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_DIAGNOSTIC)                
                    sDescription = clsGlobalVariables.STATUS_DIAGNOSTIC_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_REPLACEMENT)                
                    sDescription = clsGlobalVariables.STATUS_REPLACEMENT_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_SERVICING)                
                    sDescription = clsGlobalVariables.STATUS_SERVICING_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_INSTALLATION)                
                    sDescription = clsGlobalVariables.STATUS_INSTALLATION_DESC;                
                else if (iStatus == clsGlobalVariables.STATUS_CANCELLED)                
                    sDescription = clsGlobalVariables.STATUS_CANCELLED_DESC;                
            }
            else
            {
                sDescription = clsFunction.sUndefineStatus;
            }
            

            return sDescription;
        }

        public string GetServiceDescriptionByCode(string sServiceCode)
        {
            string sDescription = "";

            if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_INSTALLED_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_NEGATIVE_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_NEGATIVE_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_REPROGRAMMED_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_REPROGRAMMED_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_PULLEDOUT_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_PULLEDOUT_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_DIAGNOSTIC_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_DIAGNOSTIC_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_SERVICING_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_REPLACEMENT_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_REPLACEMENT_SUB_DESC;
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_DISPATCH_CODE) == 0)
                sDescription = clsGlobalVariables.TA_STATUS_DISPATCH_SUB_DESC;

            return sDescription;
        }

        public string GetServiceCodeByDescription(string sServiceDescription, bool isNegative)
        {
            string sServiceCode = clsFunction.sNull;

            if (isNegative)
            {
                sServiceCode = clsGlobalVariables.TA_STATUS_NEGATIVE_CODE;
            }
            else
            {                
                if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_INSTALLED_CODE;                
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_REPROGRAMMED_CODE;
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_PULLEDOUT_CODE;
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_DIAGNOSTIC_CODE;
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_SERVICING_CODE;
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_REPLACEMENT_CODE;
                else if (sServiceDescription.CompareTo(clsGlobalVariables.JOB_TYPE_DISPATCH_DESC) == 0)
                    sServiceCode = clsGlobalVariables.TA_STATUS_DISPATCH_CODE;
            }
            
            return sServiceCode;
        }
        
        public string GetImmportReasonDescription(ImportReasonType iImportReasonType)
        {
            string sDescription = "";

            switch (iImportReasonType)
            {
                case ImportReasonType.iValid:
                    sDescription = "VALID";
                    break;
                case ImportReasonType.iIvalidTID:
                    sDescription = "INVALID TID";
                    break;
                case ImportReasonType.iInvalidMID:
                    sDescription = "INVALID MID";
                    break;
                case ImportReasonType.iInvalidTIDMID:
                    sDescription = "INVALID TID/MID";
                    break;
                case ImportReasonType.iRecordFound:
                    sDescription = "RECORD FOUND";
                    break;
                case ImportReasonType.iRecordNotFound:
                    sDescription = "RECORD NOT FOUND";
                    break;
                case ImportReasonType.iServiceNotFound:
                    sDescription = "SERVICE NOT FOUND";
                    break;
                case ImportReasonType.iServiceFound:
                    sDescription = "SERVICE FOUND";
                    break;
            }

            return sDescription;
        }

        public void GetCompletedFSR(string sDateFrom, string sDateTo)
        {
            clsSearch.ClassAdvanceSearchValue = sDateFrom + clsFunction.sPipe +
                                                sDateTo;

            Debug.WriteLine("GetCompletedFSR::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "View", "Download Completed FSR", clsSearch.ClassAdvanceSearchValue, "FSR", "", "ViewFSR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)

                clsFSR.RecordFound = true;
            else
                clsFSR.RecordFound = false;
        }

        public void DownloadService(string sDateFrom, string sDateTo, string sServiceStatusDescription, string sServiceCode)
        {
            clsSearch.ClassAdvanceSearchValue = sDateFrom + clsFunction.sPipe +
                                                sDateTo + clsFunction.sPipe +
                                                sServiceStatusDescription + clsFunction.sPipe +
                                                sServiceCode;

            Debug.WriteLine("DownloadService::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("GET", "View", "Download Service", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
                clsServicingDetail.RecordFound = true;
            else
                clsServicingDetail.RecordFound = false;
        }
        
        /*
        public void GetJobTypeDescriptionByServiceCode(string sServiceCode, ref string sJobTypeDescription, ref string sJobTypeSubDescription)
        {            
            if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_INSTALLED_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_INSTALLED_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_PULLEDOUT_CODE) == 0)                
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_PULLEDOUT_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_NEGATIVE_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_REPROGRAMMED_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPROGRAMMED_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_DIAGNOSTIC_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_DIAGNOSTIC_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_SERVICING_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_SERVICING_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_REPLACEMENT_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_REPLACEMENT_SUB_DESC;
            }
            else if (sServiceCode.CompareTo(clsGlobalVariables.TA_STATUS_DISPATCH_CODE) == 0)
            {
                sJobTypeDescription = clsGlobalVariables.JOB_TYPE_DISPATCH_DESC;
                sJobTypeSubDescription = clsGlobalVariables.TA_STATUS_DISPATCH_SUB_DESC;
            }            
        }
        */

        /*
        public string GetServiceStatus(string sJobTypeDescription)
        {
            string sStatus = clsFunction.sZero;
            clsSearch.ClassAdvanceSearchValue = sJobTypeDescription;            
            ExecuteAPI("GET", "Search", "JobTypeDescription", clsSearch.ClassAdvanceSearchValue, "Service Type", "", "ViewServiceType");

            if (!clsGlobalVariables.isAPIResponseOK) return(clsFunction.sZero);

            if (!isNoRecordFound())
            {
                sStatus = clsServiceType.ClassServiceStatus.ToString();                
            }

            return sStatus;

        }
        */

        /*
        public string GetJobTypeStatus(string sJobTypeStatusDescription)
        {
            string sTemp = clsFunction.sZero;

            if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_PENDING_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_PENDING.ToString();
            else if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_REPROCESSING_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_REPROCESSING.ToString();
            else if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_READY_TO_PROCESS_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_READY_TO_PROCESS.ToString();
            else if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_PROCESSING.ToString();
            else if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_COMPLETED.ToString();
            else if (sJobTypeStatusDescription.CompareTo(clsGlobalVariables.JOB_TYPE_STATUS_FAILED_DESC) == 0)
                sTemp = clsGlobalVariables.JOB_TYPE_STATUS_FAILED.ToString();

            return sTemp;
        }
        */

        /*
        public string GetReportDescription(clsAPI.JobType iJobType)
        {
            string sTemp = clsFunction.sNull;



            return sTemp;
        }
        */

        public int GetIDNo(string SearchBy, string SearchValue)
        {
            int iID = 0;
            ExecuteAPI("GET", "Search", SearchBy, SearchValue, "CheckControlID", "", "CheckControlID");

            if (!clsGlobalVariables.isAPIResponseOK)
            {
                return iID;
            }

            iID = clsCheckControlID.ClassControlID;

            return iID;
        }

        public void GetFSRInfo()
        {
            int i = 0;

            ExecuteAPI("GET", "Search", "FSRNo", clsSearch.ClassAdvanceSearchValue, "FSR", "", "ViewFSR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.FSRNo.Length > i)
                {

                    clsFSR.ClassFSRNo = int.Parse(clsArray.FSRNo[i]);
                    clsFSR.ClassFSRID = int.Parse(clsArray.FSRID[i]);
                    clsFSR.ClassServiceNo = int.Parse(clsArray.ServiceNo[i]);
                    clsFSR.ClassIRIDNo = int.Parse(clsArray.IRIDNo[i]);
                    clsFSR.ClassTAIDNo = int.Parse(clsArray.TAIDNo[i]);
                    clsFSR.ClassIRNo = clsArray.IRNo[i];
                    clsFSR.ClassMerchant = clsArray.Merchant[i];
                    clsFSR.ClassTID = clsArray.TID[i];
                    clsFSR.ClassMID = clsArray.MID[i];
                    clsFSR.ClassTerminalSN = clsArray.TerminalSN[i];
                    clsFSR.ClassSIMSN = clsArray.SIMSerialNo[i];
                    clsFSR.ClassPowerSN = clsArray.PowerSN[i];
                    clsFSR.ClassDockSN = clsArray.DockSN[i];
                    clsFSR.ClassTimeArrived = clsArray.TimeArrived[i];
                    clsFSR.ClassTimeStart = clsArray.TimeStart[i];
                    clsFSR.ClassFSR = clsArray.FSR[i];
                    clsFSR.ClassFSRDate = clsArray.FSRDate[i];
                    clsFSR.ClassFSRTime = clsArray.FSRTime[i];
                    clsFSR.ClassTimeEnd = clsArray.TimeEnd[i];
                    clsFSR.ClassMerchantContactNo = clsArray.MerchantContactNo[i];
                    clsFSR.ClassMerchantRepresentative = clsArray.MerchantRepresentative[i];
                    clsFSR.ClassFEName = clsArray.FEName[i];
                    clsFSR.ClassSerialNo = clsArray.SerialNo[i];
                    clsFSR.ClassActionMade = clsArray.ActionMade[i];
                    clsFSR.ClassProblemReported = clsArray.ProblemReported[i];
                    clsFSR.ClassProblemReported = clsArray.ActualProblemReported[i];
                    clsFSR.ClassActionTaken = clsArray.ActionTaken[i];
                    clsFSR.ClassServiceTypeDescription = clsArray.ServiceTypeDescription[i];
                    
                    i++;

                }
            }
        }

        public void UpdateFSRDateTime(string sFSRNo, string sServiceNo, string sFSRDate, string sFSRTime, string sTimeArrived, string sTimeStart, string sTimeEnd)
        {
            int iFSRNo = int.Parse((sFSRNo.Length > 0 ? sFSRNo : clsFunction.sZero));
            int iServiceNo = int.Parse((sServiceNo.Length > 0 ? sServiceNo : clsFunction.sZero));
            if (iFSRNo > 0 && iServiceNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sFSRNo + clsFunction.sPipe +
                                                    sServiceNo + clsFunction.sPipe +
                                                    sFSRDate + clsFunction.sPipe +
                                                    sFSRTime + clsFunction.sPipe +
                                                    sTimeArrived + clsFunction.sPipe +
                                                    sTimeStart + clsFunction.sPipe +
                                                    sTimeEnd;

                Debug.WriteLine("UpdateFSRDateTime::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update FSR Date And Time", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateTARemarks(string sTAIDNo, string sRemarks, string sComments)
        {
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            if (iTAIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                                    sRemarks + clsFunction.sPipe +
                                                    sComments;

                Debug.WriteLine("UpdateTARemarks::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update TA Remarks", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void UpdateTAFESP(string sTAIDNo, string sFEID, string sSPID)
        {
            int iTAIDNo = int.Parse((sTAIDNo.Length > 0 ? sTAIDNo : clsFunction.sZero));
            if (iTAIDNo > 0)
            {
                clsSearch.ClassAdvanceSearchValue = sTAIDNo + clsFunction.sPipe +
                                    sFEID + clsFunction.sPipe +
                                    sSPID;

                Debug.WriteLine("UpdateTARemarks::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", "Update TA FE And SP", clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void GetReponseFileName(string StatementType, string SearchBy, string SearchValue, string Action)
        {
            string sTemp = "";

            switch (SearchBy)
            {
                case "Region":
                    sTemp = clsDefines.RESP_REGIONLIST_FILENAME;
                    break;
                case "Province":
                    sTemp = clsDefines.RESP_PROVINCELIST_FILENAME;
                    break;
                case "RegionDetail":
                    sTemp = clsDefines.RESP_REGIONDETAILLIST_FILENAME;
                    break;                
                case "Merchant":
                    sTemp = clsDefines.RESP_MERCHANTLIST_FILENAME;
                    break;
                case "Field Engineer":
                    sTemp = clsDefines.RESP_FELIST_FILENAME;
                    break;
                case "Terminal Type":
                    sTemp = clsDefines.RESP_TERMINALTYPELIST_FILENAME;
                    break;
                case "Terminal Model":
                    sTemp = clsDefines.RESP_TERMINALMODELLIST_FILENAME;
                    break;
                case "Terminal Brand":
                    sTemp = clsDefines.RESP_TERMINALBRANDLIST_FILENAME;
                    break;
                case "Status List":
                    sTemp = clsDefines.RESP_TERMINALSTATUSLIST_FILENAME;
                    break;
                case "Status":
                    sTemp = clsDefines.RESP_STATUS_FILENAME;
                    break;
                case "Service Type Active":
                    sTemp = clsDefines.RESP_SERVICETYPELIST_FILENAME;
                    break;
                case "Service Status Active":
                    sTemp = clsDefines.RESP_SERVICESTATUSLIST_FILENAME;
                    break;
                case "Particular List":
                    if (SearchValue.Equals("2|0|0|0|0"))
                        sTemp = clsDefines.RESP_CLIENTLIST_FILENAME; // Client

                    if (SearchValue.Equals("5|0|0|0|0")) 
                        sTemp = clsDefines.RESP_SPLIST_FILENAME; // SP

                    if (SearchValue.Equals("1|0|0|0|0"))
                        sTemp = clsDefines.RESP_MERCHANTLIST_FILENAME; // Merchant

                    if (SearchValue.Equals("3|0|0|0|0"))
                        sTemp = clsDefines.RESP_FELIST_FILENAME; // FE

                    if (SearchValue.Equals("6|0|0|0|0"))
                        sTemp = clsDefines.RESP_EMPLIST_FILENAME; // Employee

                    break;
                case "Reason":
                    //if (SearchValue.Equals("0|0|REASON"))
                        sTemp = clsDefines.RESP_REASON_FILENAME;

                    //if (SearchValue.Equals("0|0|RESOLUTION"))
                    //    sTemp = clsDefines.RESP_RESOLUTION_FILENAME;

                    break;
                case "Department":
                    sTemp = clsDefines.RESP_DEPARTMENTLIST_FILENAME;
                    break;
                case "Position":
                    sTemp = clsDefines.RESP_POSITIONLIST_FILENAME;
                    break;
                case "LeaveType":
                    sTemp = clsDefines.RESP_LEAVETYPELIST_FILENAME;
                    break;
                case "WorkType":
                    sTemp = clsDefines.RESP_WORKTYPELIST_FILENAME;
                    break;
                case "Country":
                    sTemp = clsDefines.RESP_COUNTRYLIST_FILENAME;
                    break;
                case "Location":
                    sTemp = clsDefines.RESP_LOCATIONLIST_FILENAME;
                    break;
                case "Asset Type":
                    sTemp = clsDefines.RESP_ASSETTYPELIST_FILENAME;
                    break;
                case "Carrier":
                    sTemp = clsDefines.RESP_CARRIERLIST_FILENAME;
                    break;
                case "Setup":
                    sTemp = clsDefines.RESP_SETUPLIST_FILENAME;
                    break;
                case "Particular":
                    sTemp = clsDefines.RESP_PARTICULARLIST_FILENAME;
                    break;
                case "Type List":
                    sTemp = clsDefines.RESP_TYPELIST_FILENAME;
                    break;
                case "Rental Fee List":
                    sTemp = clsDefines.RESP_RENTALFEELIST_FILENAME;
                    break;
                case "All Type":
                    sTemp = clsDefines.RESP_ALLTYPE_FILENAME;
                    break;

            }

            if (sTemp.Length > 0)
            {
                clsSearch.ClassResponseFileName = sTemp;
                Debug.WriteLine("GetReponseFileName, clsSearch.ClassResponseFileName=" + clsSearch.ClassResponseFileName);
            }            
        }

        public void GetServiceInfo()
        {
            int i = 0;

            ExecuteAPI("GET", "View", "ServiceNo", clsSearch.ClassAdvanceSearchValue, "Servicing Detail", "", "ViewServicingDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (!isNoRecordFound())
            {
                while (clsArray.ServiceNo.Length > i)
                {

                    clsServicingDetail.ServiceNo = int.Parse(clsArray.ServiceNo[i]);
                    clsServicingDetail.ClassIRIDNo = int.Parse(clsArray.IRIDNo[i]);
                    clsServicingDetail.ClassTAIDNo = int.Parse(clsArray.TAIDNo[i]);
                    clsServicingDetail.ClassClientID = int.Parse(clsArray.ClientID[i]);
                    clsServicingDetail.ClassMerchantID = int.Parse(clsArray.MerchantID[i]);
                    clsServicingDetail.ClassRequestNo = clsArray.RequestNo[i];
                    clsServicingDetail.ClassIRNo = clsArray.IRNo[i];
                    clsServicingDetail.ClassServiceDateTime = clsArray.ServiceDateTime[i];
                    clsServicingDetail.ClassServiceDate = clsArray.ServiceDate[i];
                    clsServicingDetail.ClassServiceTime = clsArray.ServiceTime[i];
                    clsServicingDetail.ClassServiceReqDate = clsArray.ServiceReqDate[i];
                    clsServicingDetail.ClassServiceReqTime = clsArray.ServiceReqTime[i];
                    clsServicingDetail.ClassReferenceNo = clsArray.ReferenceNo[i];
                    clsServicingDetail.ClassCustomerName = clsArray.CustomerName[i];
                    clsServicingDetail.ClassCustomerContactNo = clsArray.CustomerContactNo[i];
                    clsServicingDetail.ClassRemarks = clsArray.Remarks[i];
                    clsServicingDetail.AppVersion = clsArray.AppVersion[i];
                    clsServicingDetail.AppCRC = clsArray.AppCRC[i];

                    i++;

                }
            }
        }

        public void GenerateResponseFile(ucStatus control)
        {
            clsSearch._isWriteResponse = true;

            Cursor.Current = Cursors.WaitCursor;

            control.iState = 3;           

            control.sMessage = "Creating service type.";
            control.AnimateStatus();
            GetServiceTypeList("View", "Service Type Active", "", "Service Type");

            control.sMessage = "Creating service status.";
            control.AnimateStatus();
            GetServiceTypeList("View", "Service Status Active", "", "Service Type");

            control.sMessage = "Creating particular client.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsGlobalVariables.iClient_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassSearchValue, "Particular", "", "ViewAdvanceParticular");

            control.sMessage = "Creating particular supplier.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsGlobalVariables.iSP_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassSearchValue, "Particular", "", "ViewAdvanceParticular");

            control.sMessage = "Creating particular FE file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsGlobalVariables.iFE_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassSearchValue, "Particular", "", "ViewAdvanceParticular");

            control.sMessage = "Creating particular EMP file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsGlobalVariables.iEMP_Type.ToString() + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Particular List", clsSearch.ClassSearchValue, "Particular", "", "ViewAdvanceParticular");

            control.sMessage = "Creating terminal type file.";
            control.AnimateStatus();
            GetTerminalTypeList("View", "Terminal Type", "", "Terminal Type");

            control.sMessage = "Creating terminal model file.";
            control.AnimateStatus();
            GetTerminalModelList("View", "Terminal Model", "", "Terminal Model");

            control.sMessage = "Creating terminal brand file.";
            control.AnimateStatus();
            GetTerminalBrandList("View", "Terminal Brand", "", "Terminal Brand");

            control.sMessage = "Creating terminal status file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + clsGlobalVariables.iTerminal_Type + clsFunction.sPipe + clsFunction.sNull;
            GetTerminalStatusList("View", "Status List", clsSearch.ClassSearchValue, "Terminal Status");

            //control.sMessage = "Creating reason file.";
            //control.AnimateStatus();
            //clsSearch.ClassSearchValue = clsFunction.sPadZero + clsFunction.sPipe + clsFunction.sNull + clsFunction.sPipe + clsGlobalVariables.REASON_TYPE;
            //ExecuteAPI("GET", "View", "Reason", clsSearch.ClassSearchValue, "Reason", "", "ViewReason");

            control.sMessage = "Creating resolution file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.sPadZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Reason", clsSearch.ClassSearchValue, "Reason", "", "ViewReason");

            control.sMessage = "Creating department file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe;
            ExecuteAPI("GET", "View", "Department", clsSearch.ClassSearchValue, "Department", "", "ViewDepartment");

            control.sMessage = "Creating position file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe + clsFunction.sZero;
            ExecuteAPI("GET", "View", "Position", clsSearch.ClassSearchValue, "Position", "", "ViewPosition");

            control.sMessage = "Creating leave type file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe;
            ExecuteAPI("GET", "View", "LeaveType", clsSearch.ClassSearchValue, "LeaveType", "", "ViewLeaveType");

            control.sMessage = "Creating work type file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe;
            dbAPI.ExecuteAPI("GET", "View", "WorkType", "", "Type", "", "ViewType");

            control.sMessage = "Creating country file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Country", "", "Advance Detail", "", "ViewAdvanceDetail");

            control.sMessage = "Creating location file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Location", "", "Advance Detail", "", "ViewAdvanceDetail");

            control.sMessage = "Creating asset type file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Asset Type", "", "Advance Detail", "", "ViewAdvanceDetail");

            control.sMessage = "Creating carrier file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Carrier", "", "Advance Detail", "", "ViewAdvanceDetail");

            control.sMessage = "Creating setup file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Setup", "", "Advance Detail", "", "ViewAdvanceDetail");
            
            control.sMessage = "Creating type list file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Type List", "", "Type", "", "ViewType");

            control.sMessage = "Creating rental fee file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Rental Fee List", "", "Type", "", "ViewType");

            control.sMessage = "Creating region list file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "Region", "", "Region", "", "ViewRegion");

            control.sMessage = "Creating province list file.";
            control.AnimateStatus();
            clsSearch.ClassSearchValue = clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero + clsFunction.sPipe + clsFunction.sZero;
            dbAPI.ExecuteAPI("GET", "View", "Province", clsSearch.ClassSearchValue, "Province", "", "ViewRegionDetail");
            
            control.sMessage = "Creating type file.";
            control.AnimateStatus();
            dbAPI.ExecuteAPI("GET", "View", "All Type", "", "Type", "", "ViewType");

            Cursor.Current = Cursors.Default;

            clsSearch._isWriteResponse = false;

            control.sMessage = "Complete.";
        }

        public void PreviewFSR(string pIRNo, string pRequestNo, string pMerchantName, string pTID, string pMID, string pServiceType, int pServiceNo, int pFSRNo, int pIRIDNo, bool isConfrm)
        {
            Debug.WriteLine("--PreviewFSR--");
            Debug.WriteLine("pIRNo="+ pIRNo);
            Debug.WriteLine("pRequestNo=" + pRequestNo);
            Debug.WriteLine("pMerchantName=" + pMerchantName);
            Debug.WriteLine("pTID=" + pTID);
            Debug.WriteLine("pMID=" + pMID);
            Debug.WriteLine("pServiceNo=" + pServiceNo);
            Debug.WriteLine("pFSRNo=" + pFSRNo);
            Debug.WriteLine("pIRIDNo=" + pIRIDNo);

            if (isConfrm)
            {
                if (MessageBox.Show("Preview Field Service Report(FSR) details below:\n" +
                "\nJob Type: " + pServiceType +
                "\nRequest ID: " + pIRNo +
                "\nRequest No: " + pRequestNo +
                "\n" +
                "\nMerchant Name: " + pMerchantName +
                "\nTID: " + pTID +
                "\nMID:" + pMID +
                "\n\n" + "Preview report?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    return;
            }
            

            dbFunction.PreviewTA(pIRNo, pServiceNo, pFSRNo, pIRIDNo, pRequestNo);
        }

        public void FillComboBoxLeaveType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsFunction.isActive + clsFunction.sPipe;
            ExecuteAPI("GET", "View", "LeaveType", clsSearch.ClassSearchValue, "LeaveType", "", "ViewLeaveType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.LeaveTypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.LeaveTypeDesc[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxDepartment(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe;
            ExecuteAPI("GET", "View", "Department", clsSearch.ClassSearchValue, "Department", "", "ViewDepartment");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.DepartmentID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.DepartmentDesc[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxPosition(ComboBox obj, string pSearchBy)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsFunction.isActive.ToString() + clsFunction.sPipe + clsSearch.ClassDepartmentID;
            ExecuteAPI("GET", "View", pSearchBy, clsSearch.ClassSearchValue, "Position", "", "ViewPosition");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.PositionID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.PositionDesc[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxCountry(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            ExecuteAPI("GET", "View", "Country", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.Country[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxDateType(ComboBox obj)
        {
            obj.Items.Clear();

            obj.Items.Add(clsFunction.sDefaultSelect);
            obj.Items.Add("WHOLEDAY");
            obj.Items.Add("HALFDAY");
        }

        public void FillComboBoxEmploymentStatus(ComboBox obj)
        {
            obj.Items.Clear();

            obj.Items.Add(clsFunction.sDefaultSelect);
            obj.Items.Add("REGULAR");
            obj.Items.Add("PROBATIONARY");
            obj.Items.Add("CONTRACTUAL");
            obj.Items.Add("OTHERS");

            obj.SelectedIndex = 0;
        }

        public void FillComboBoxWorkType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsFunction.isActive + clsFunction.sPipe;
            ExecuteAPI("GET", "View", "WorkType", clsSearch.ClassSearchValue, "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.WorkTypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.Description[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string GetJobTypeDescriptionByJobType(JobType jobType)
        {
            Debug.WriteLine("--GetJobTypeDescriptionByJobType--");
            Debug.WriteLine("jobType=" + jobType);

            string sJobTypeDesc = "";
            switch (jobType)
            {
                case JobType.iInstallation:
                    sJobTypeDesc = clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC;
                    break;
                case JobType.iServicing:
                    sJobTypeDesc = clsGlobalVariables.JOB_TYPE_SERVICING_DESC;
                    break;
                case JobType.iPullOut:
                    sJobTypeDesc = clsGlobalVariables.JOB_TYPE_PULLOUT_DESC;
                    break;
                case JobType.iReplacement:
                    sJobTypeDesc = clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC;
                    break;
                case JobType.iReprogramming:
                    sJobTypeDesc = clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC;
                    break;
                default:
                    sJobTypeDesc = "UNDEFINED";
                    break;
            }

            Debug.WriteLine("sJobTypeDesc="+ sJobTypeDesc);

            return sJobTypeDesc;
        }

        public void UpdateLastSNAllocated(string pSearchBy, string pID, string pParticluarID, string pIRIDNo, string pClientID)
        {
            Debug.WriteLine("--UpdateLastSNAllocated--");
            Debug.WriteLine("pID="+ pID);
            Debug.WriteLine("pParticluarID=" + pParticluarID);
            Debug.WriteLine("pIRIDNo=" + pIRIDNo);
            Debug.WriteLine("pClientID=" + pClientID);

            if (pSearchBy.Length > 0 && pID.Length > 0)
            {
                clsSearch.ClassAdvanceSearchValue = pID + clsFunction.sPipe +
                                    pParticluarID + clsFunction.sPipe +
                                    pIRIDNo + clsFunction.sPipe +
                                    pClientID;

                Debug.WriteLine("UpdateLastSNAllocated::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

                ExecuteAPI("PUT", "Update", pSearchBy, clsSearch.ClassAdvanceSearchValue, "", "", "UpdateCollectionDetail");
            }
        }

        public void ProcessTimeSheet()
        {
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassParticularID + clsFunction.sPipe +
                                                     clsSearch.ClassMobileTerminalID + clsFunction.sPipe +
                                                     clsSearch.ClassDepartmentID + clsFunction.sPipe +
                                                     clsSearch.ClassDateFrom + clsFunction.sPipe +
                                                     clsSearch.ClassDateTo + clsFunction.sPipe +
                                                     clsSearch.ClassMissingTimeSheet;

            Debug.WriteLine("ProcessTimeSheet::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("POST", "Process", "TimeSheet", clsSearch.ClassAdvanceSearchValue, "TimeSheet", "", "ProcessTimeSheet");
            
        }

        public void GenerateTerminalSummary()
        {
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassLocationID + clsFunction.sPipe +
                                                     clsSearch.ClassTerminalTypeID + clsFunction.sPipe +
                                                     clsSearch.ClassTerminalModelID + clsFunction.sPipe +
                                                     clsSearch.ClassClientID;

            Debug.WriteLine("ProcessTerminalInventorySummary::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("POST", "Generate", "Inventory Summary", clsSearch.ClassAdvanceSearchValue, "Inventory Summary", "", "GenerateTerminalSummary");

        }

        public void GenerateSIMSummary()
        {
            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassLocationID + clsFunction.sPipe +
                                                     clsSearch.ClassSIMCarrier;

            Debug.WriteLine("GenerateSIMSummary::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("POST", "Generate", "Inventory Summary", clsSearch.ClassAdvanceSearchValue, "Inventory Summary", "", "GenerateSIMSummary");

        }

        public void GetHeader()
        {
            int i = 0;

            ExecuteAPI("GET", "View", "", "", "Header", "", "ViewHeader");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsHeader.ClassRecordFound)
            {
                while (clsArray.HeaderID.Length > i)
                {
                    clsHeader.ClassHeaderID = int.Parse(clsArray.HeaderID[i].ToString());
                    clsHeader.ClassName = clsArray.Name[i];
                    clsHeader.ClassHeader1 = clsArray.Header1[i];
                    clsHeader.ClassHeader2 = clsArray.Header2[i];
                    clsHeader.ClassHeader3 = clsArray.Header3[i];
                    clsHeader.ClassHeader4 = clsArray.Header4[i];
                    clsHeader.ClassHeader5 = clsArray.Header5[i];

                    i++;
                }
            }
        }

        public string[] GetMDRType()
        {
            string[] ret = { clsFunction.sDefaultSelect, "MDR TYPE A(3.5%)", "MDR TYPE(4.0%)" };

            return ret;
        }

        public void FillComboBoxMDRType(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetMDRType().Length > i)
            {
                obj.Items.Add(GetMDRType()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public bool isValidUserAccess(UserFunctionType userFunctionType, int pUserID, int pFunctionID)
        {
            Debug.WriteLine("--isValidUserAccess--");
            Debug.WriteLine("userFunctionType="+ userFunctionType);
            Debug.WriteLine("pUserID=" + pUserID + ",ClassCurrentUserID="+ clsSearch.ClassCurrentUserID);
            Debug.WriteLine("pFunctionID=" + pFunctionID);

            bool isValid = false;
            string sTemp = clsFunction.sNull;
            string sForm = clsFunction.sNull;
            string sDescription = clsFunction.sNull;

            pUserID = clsSearch.ClassCurrentUserID; // override

            switch (userFunctionType)
            {
                case UserFunctionType.isAdd:
                    sTemp = "Add/New";                                     
                    break;
                case UserFunctionType.isDelete:
                    sTemp = "Delete";
                    break;
                case UserFunctionType.isUpdate:
                    sTemp = "Save/Update";
                    break;
                case UserFunctionType.isView:
                    sTemp = "View/Access";
                    break;
                case UserFunctionType.isPrint:
                    sTemp = "Print/Report";
                    break;
            }

            clsSearch.ClassSearchValue = pUserID + clsFunction.sPipe + pFunctionID + clsFunction.sPipe + sTemp;
            ExecuteAPI("GET", "Search", "User Privacy Detail", clsSearch.ClassSearchValue, "CheckRecordExist", "", "CheckRecordExist");
            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
                isValid = false;
            else
                isValid = true;

            if (!isValid)
            {
                // Privacy info
                dbAPI.ExecuteAPI("GET", "Search", "Privacy Info", pFunctionID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    sForm = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    sDescription = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                }

                dbFunction.SetMessageBox("[ " + sTemp + " ]" + " rights and privacy denied." + "\n\n" +
                                        "Function detail:" + "\n" +
                                        "     >" + sForm + "\n" +
                                        "     >" + sDescription, "Permission", clsFunction.IconType.iError);
            }

            return isValid;
        }

        public string[] GetLocation()
        {
            Debug.WriteLine("--GetLocation--");

            string[] ret = { clsFunction.sDefaultSelect };

            ExecuteAPI("GET", "View", "Location", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (clsGlobalVariables.isAPIResponseOK)
            {
                ret = clsArray.Description;
            }

            return ret;
        }

        public void FillComboBoxLocation(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            GetLocation();

            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {
                    obj.Items.Add(clsArray.Description[i]);

                    i++;
                }
            }
            
            if (i > 0)
                obj.SelectedIndex = 0;

        }

        public void FillComboBoxRentalType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsGlobalVariables.RENTAL_TYPE.ToString();
            ExecuteAPI("GET", "View", "Type", clsSearch.ClassSearchValue, "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();

            while (clsArray.TypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.TypeDescription[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxRentalTerm(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            clsSearch.ClassSearchValue = clsGlobalVariables.RENTAL_TERM_TYPE.ToString();
            ExecuteAPI("GET", "View", "Type", clsSearch.ClassSearchValue, "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();

            while (clsArray.TypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.TypeDescription[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxPOSType(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            ExecuteAPI("GET", "View", "Rental Fee List", "", "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();

            while (clsArray.TypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.TypeDescription[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string[] GetAssetType()
        {
            string[] ret = { clsFunction.sDefaultSelect };

            ExecuteAPI("GET", "View", "Asset Type", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (clsGlobalVariables.isAPIResponseOK)
            {
                ret = clsArray.Description;
            }

            return ret;
        }
        public void FillComboBoxAssetType(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            GetAssetType();

            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {
                    obj.Items.Add(clsArray.Description[i]);

                    i++;
                }
            }
            
            if (i > 0)
                obj.SelectedIndex = 0;

        }

        public void FillComboBoxServiceResultLocation(ComboBox obj, string pSearchValue)
        {
            int i = 0;

            obj.Items.Clear();

            string[] ret = { clsFunction.sDefaultSelect };

            ExecuteAPI("GET", "View", "Service Result Location", pSearchValue, "Advance Detail", "", "ViewAdvanceDetail");
            
            if (clsArray.ID.Length > 0)
            {
                while (clsArray.ID.Length > i)
                {
                    obj.Items.Add(clsArray.Description[i]);

                    i++;
                }
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }

        public void FillComboBoxTypeByGroup(ComboBox obj, int groupType)
        {
            int i = 0;
            bool fSelect = false;
           

            ExecuteAPI("GET", "View", "Type", groupType.ToString(), "Type", "", "ViewType");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();

            while (clsArray.TypeID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsArray.TypeDescription[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void GenerateID(bool isAutoGen, TextBox objID, TextBox objNo, string pSearchBy, string pPrefix)
        {
            int iControlNo = 0;

            // Get Service Request ID
            if (isAutoGen)
            {
                iControlNo = GetControlID(pSearchBy);
            }
            else
            {
                iControlNo = int.Parse(dbFunction.CheckAndSetNumericValue(objNo.Text));
            }

            objNo.Text = iControlNo.ToString();
            objID.Text = iControlNo.ToString();
            objID.Text = dbFunction.GenerateControlNo(iControlNo, pPrefix, true);
        }
        
        public JobType GetJobType(string pJobTypeDescription)
        {
            JobType jobType = JobType.iServicing;

            if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
            {
                jobType = JobType.iInstalled;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC))
            {
                jobType = JobType.iServicing;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
            {
                jobType = JobType.iPullOut;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                jobType = JobType.iReplacement;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
            {
                jobType = JobType.iReprogramming;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC))
            {
                jobType = JobType.iServicing;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_DISPATCH_DESC))
            {
                jobType = JobType.iDispatch;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_NEGATIVE_DESC))
            {
                jobType = JobType.iNegative;
            }

            return jobType;
        }

        public void GenerateInstallationSummary()
        {
            
            Debug.WriteLine("ProcessTerminalInventorySummary::clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            ExecuteAPI("POST", "Generate", "Installation Request Summary", clsSearch.ClassAdvanceSearchValue, "Installation Request, Summary", "", "GenerateInstallationSummary");

        }

        public string GetValueFromJSONString(string jsonString, string pFieldTag)
        {
            string sValue = clsFunction.sNull;

            try
            {
                sValue = JObject.Parse(jsonString)[pFieldTag].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error message: " + ex.Message + "\n\n" + "Tag Field: " + "["+pFieldTag+"]"+ "\n" + "Data: " + "["+jsonString+"]", "JSON parse error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return sValue;
        }

        public string GetBeautifyJSON(string jsonString)
        {
            string pReturn = clsFunction.sNull;
            Debug.WriteLine("--GetBeautifyJSON--");

            if (jsonString.Length > 0)
            {
                JToken parsedJson = JToken.Parse(jsonString);
                var beautified = parsedJson.ToString(Formatting.Indented);
                var minified = parsedJson.ToString(Formatting.None);

                pReturn = minified;

                Debug.WriteLine(">Formatting.Indented[BEAUTIFIED]");
                Debug.WriteLine(beautified);
            }
            else
            {
                Debug.WriteLine("Unable to beautify JSON string....");
            }
          
            return pReturn;

        }

        public bool isValidTerminalTypeModel(string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", "Search", "Terminal Type/Model Check", SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool isValidSIMCarrier(string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", "Search", "SIM Carrier Check", SearchValue, "CheckRecordExist", "", "CheckRecordExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public string[] getServiceStatus()
        {
            //string[] ret = { clsFunction.sDefaultSelect, "PENDING", "PROCESSING", "PENDING/PROCESSING","COMPLETED" };
            string[] ret = { clsFunction.sDefaultSelect, "PENDING", "PROCESSING", "OVERALL PENDING", "COMPLETED" };

            return ret;
        }

        public void FillComboBoxServiceStatus(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (getServiceStatus().Length > i)
            {
                obj.Items.Add(getServiceStatus()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public int getServiceJobType(string pJobTypeDescription)
        {
            int pValue = 0;

            Debug.WriteLine("--getServiceJobType--");
            Debug.WriteLine("pJobTypeDescription="+ pJobTypeDescription);
            
            if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_INSTALLATION_DESC))
            {
                pValue = clsGlobalVariables.JOB_TYPE_INSTALLATION;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_SERVICING_DESC) || pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_DIAGNOSTIC_DESC))
            {
                pValue = clsGlobalVariables.JOB_TYPE_SERVICING;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_PULLOUT_DESC))
            {
                pValue = clsGlobalVariables.JOB_TYPE_PULLOUT;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_REPLACEMENT_DESC))
            {
                pValue = clsGlobalVariables.JOB_TYPE_REPLACEMENT;
            }
            else if (pJobTypeDescription.Equals(clsGlobalVariables.JOB_TYPE_REPROGRAMMING_DESC))
            {
                pValue = clsGlobalVariables.JOB_TYPE_REPROGRAMMING;
            }

            Debug.WriteLine("getServiceJobType, pValue=" + pValue);

            return pValue;
        }

        public string getPrimaryIRNo(int pIRIDNo)
        {
            string pOutput = "";
            string pSearchValue = clsGlobalVariables.JOB_TYPE_INSTALLATION.ToString() + clsDefines.gPipe + pIRIDNo.ToString();
            dbAPI.ExecuteAPI("GET", "Search", "Primary IRNo", pSearchValue, "Get Info Detail", "", "GetInfoDetail");
            Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);
            if (clsSearch.ClassOutParamValue.Length > 0)
            {
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                if (!dbAPI.isNoRecordFound())
                {
                    pOutput = dbAPI.GetValueFromJSONString(clsSearch.ClassOutParamValue, clsDefines.TAG_IRNO);
                }
            }

            return pOutput;
        }

        public string[] getYesNoSelection()
        {
            string[] ret = { clsFunction.sDefaultSelect, "YES", "NO" };

            return ret;
        }

        public string[] getSurverySelection()
        {
            string[] ret = { clsFunction.sDefaultSelect, "1", "2", "3", "4", "5" };

            return ret;
        }

        public void FillComboBoxYesNo(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (getYesNoSelection().Length > i)
            {
                obj.Items.Add(getYesNoSelection()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }
        
        public void FillComboBoxSurvey(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (getSurverySelection().Length > i)
            {
                obj.Items.Add(getSurverySelection()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string getInfoDetailJSON(string pStatementType, string pSearchBy, string pSearchValue)
        {
            string pOutput = "";           
            ExecuteAPI("GET", pStatementType, pSearchBy, pSearchValue, "Get Info Detail", "", "GetInfoDetail");
            //Debug.WriteLine("clsSearch.ClassOutParamValue=" + clsSearch.ClassOutParamValue);
            if (clsSearch.ClassOutParamValue.Length > 0)
            {
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                if (!isNoRecordFound())
                {
                    pOutput = clsSearch.ClassOutParamValue;

                }
            }

            return pOutput;
        }

        public bool isValidCancelService(string pServiceNo, string pFSRNo)
        {
            bool isValid = true;

            if (dbFunction.isValidID(pServiceNo) && dbFunction.isValidID(pFSRNo))
                isValid = false;

            if (!isValid)
            {
                dbFunction.SetMessageBox("Unable to cancel the service; it has already been completed.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;
        }

        public bool isValidDiagnotic(string pServiceNo, string pFSRNo)
        {
            bool isValid = true;

            if (!isRecordExist("Search", "Diagnostic Detail", dbFunction.CheckAndSetNumericValue(pServiceNo) + clsDefines.gPipe + dbFunction.CheckAndSetNumericValue(pFSRNo)))
            {
                isValid = false;
            }

            if (!isValid)
            {
                dbFunction.SetMessageBox("Unable to process, diagnostic not found", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;

        }

        public void FillComboBoxPositionType(ComboBox obj, int pPositionType)
        {
            int i = 0;
            bool fSelect = false;

            ExecuteAPI("GET", "View", "Particular By Position Type List", pPositionType.ToString(), "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ID.Length > i)
            {
                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }
                
                obj.Items.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FullName));

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string[] getFSRModeSelection()
        {     
            string[] ret = { clsFunction.sDefaultSelect, "DIGITAL FSR", "MANUAL FSR" };

            return ret;
        }

        public void FillComboBoxFSRMode(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (getFSRModeSelection().Length > i)
            {
                obj.Items.Add(getFSRModeSelection()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void getTATInfo(int pClientID, int pIRIDNo, int pServiceNo)
        {
            if (dbFunction.isValidID(pClientID.ToString()) && dbFunction.isValidID(pIRIDNo.ToString()) && dbFunction.isValidID(pServiceNo.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "TAT Info", pClientID.ToString() + clsDefines.gPipe + pIRIDNo.ToString() + clsDefines.gPipe + pServiceNo.ToString());
                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                clsSearch.ClassSLA = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SLA);
                clsSearch.ClassNetworkDays = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_NetworkDays);
                clsSearch.ClassDaysOverDue = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DaysOverDue);
                clsSearch.ClassTATStatus = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TATStatus);
                
            }
            
        }

        public void saveResponseToFile(string pRequest, string pResponse, string pContentBody)
        {
            string pTemp = (clsSearch.ClassSearchBy.Length > 0 ? clsSearch.ClassSearchBy : clsSearch.ClassMaintenanceType);
            string pFileName = clsSearch.ClassAPIMethod + clsDefines.gUnderScore +
                               clsSearch.ClassAction + clsDefines.gUnderScore +
                               pTemp.Replace(clsDefines.gSpace, clsDefines.gUnderScore);

            Debug.WriteLine("--saveResponseToFile--");
            Debug.WriteLine("StatementType=" + clsSearch.ClassStatementType);
            Debug.WriteLine("SearchBy=" + clsSearch.ClassSearchBy);
            Debug.WriteLine("SearchValue=" + clsSearch.ClassSearchValue);
            Debug.WriteLine("MaintenanceType=" + clsSearch.ClassMaintenanceType);
            Debug.WriteLine("Action=" + clsSearch.ClassAction);
            Debug.WriteLine("ContentBody=" + pContentBody);
            Debug.WriteLine("pFileName=" + pFileName);

            // Define the file path where you want to save the content
            string filePath = dbDump.sDumpFullPath + pFileName + clsDefines.FILE_EXT_JSON;

            if (dbDump.FileExist(filePath))
                dbDump.DeleteFile(filePath);

            string pHeader = "Date/Time: " + dbFunction.getCurrentDateTime() + "\n" +
                             "Method: " + clsSearch.ClassAPIMethod + "\n" +
                             "StatementType: " + clsSearch.ClassStatementType + "\n" +
                             "MaintenanceType: " + clsSearch.ClassMaintenanceType + "\n" +
                             "SearchBy: " + clsSearch.ClassSearchBy + "\n" +
                             "SearchValue: " + clsSearch.ClassSearchValue + "\n" +
                             "ContentBody: " + pContentBody + "\n" +
                             "EndPoint: " + pRequest + "\n" +
                             dbFunction.Separator(true) + "\n";

            Debug.WriteLine("pHeader=" + pHeader);

            try
            {
                File.WriteAllText(filePath, pHeader + dbFunction.BeautifyJson(pResponse));

            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public bool isDuplicateSN(int pID, string pSN, int pIRIDNo, string pSearch)
        {
            bool isValid = true;

            int i = 0;
            int iLineNo = 0;
            string pTemp = "";

            dbFunction = new clsFunction();
            
            if (dbFunction.isValidID(pID.ToString()))
            {
                ExecuteAPI("GET", "View", "Duplicate SN Merchant List", pID + clsFunction.sPipe + pIRIDNo + clsFunction.sPipe + pSearch, "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return false;

                if (isNoRecordFound() == false)
                {
                    isValid = false;

                    while (clsArray.ID.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        pTemp += "Line# : " + iLineNo.ToString() + Environment.NewLine +
                                 "Name : " + GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MERCHANTNAME) + Environment.NewLine +
                                 "TID : " + GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TID) + Environment.NewLine +
                                 "MID : " + GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MID) + Environment.NewLine + 
                                 dbFunction.Separator(true) + Environment.NewLine;

                        i++;
                    }

                }
            }

            if (!isValid)
            {
                dbFunction.SetMessageBox(pSearch + " " + dbFunction.AddBracketStartEnd(pSN) + "\n\n" +
                                         pTemp + "\n" +
                                         "Multiple serial number assignment found." + "\n\n" +
                                         clsDefines.CONTACT_ADMIN_MESSAGE, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;
        }

        public void FillListViewFSRList(ListView obj)
        {
            int i = 0;
            int iLineNo = 0;
            bool isDiagnostic = false;
            bool isExist = false;
            string pFileName = "";

            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Generate FSR List", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.FSRNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.FSRNo[i].ToString());
                    item.SubItems.Add(clsArray.ServiceNo[i].ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i].ToString());
                    item.SubItems.Add(clsArray.MerchantName[i].ToString());
                    item.SubItems.Add(clsArray.ServiceJobTypeDescription[i].ToString());
                    item.SubItems.Add(clsArray.FEName[i].ToString());
                    item.SubItems.Add(clsArray.FSRDate[i].ToString());
                    item.SubItems.Add(clsArray.MerchantEmail[i].ToString());
                    item.SubItems.Add(clsArray.FEEmail[i].ToString());
                    
                    // Diagnostic
                    if (isRecordExist("Search", "Diagnostic Detail", clsArray.ServiceNo[i] + clsDefines.gPipe + clsArray.FSRNo[i]))
                        isDiagnostic = true;
                    else
                        isDiagnostic = false;

                    item.SubItems.Add(dbFunction.setIntegerToYesNoString(isDiagnostic ? 1 : 0));
                    
                    // Merchant Signature
                    pFileName = clsArray.ServiceNo[i] + "_" + dbFunction.padLeftChar(clsDefines.MERCHANT_SIGNATURE_INDEX.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
                    if (isFileExist("Search", "Check Upload File", pFileName))
                        isExist = true;
                    else
                        isExist = false;

                    item.SubItems.Add(dbFunction.setIntegerToYesNoString(isExist ? 1 : 0));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void FillListViewFailedServiceList(ListView obj)
        {
            int i = 0;
            int iLineNo = 0;
            
            dbFunction = new clsFunction();

            obj.Items.Clear();

            ExecuteAPI("GET", "View", "Failed Service List", clsFunction.sZero, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());                    
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRIDNO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MerchantID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRNO));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MERCHANTNAME));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_JobTypeDescription));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_CustomerName));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_DispatchBy));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FEName));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_CreatedDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_RequestDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ScheduleDate));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_DispatchDate));

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public bool isFileExist(string StatementType, string SearchBy, string SearchValue)
        {
            bool isExist = true;

            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckFileExist", "", "CheckFileExist");

            if (clsGlobalVariables.sAPIResponseCode.CompareTo(clsGlobalVariables.NO_RECORD_FOUND) == 0)
            {
                isExist = false;
            }

            return isExist;
        }

        public bool isValidSystemVersion()
        {
            bool isValid = false;

            GetSystemInfo();

            isValid = dbFunction.isValidVersion();
            
            return isValid;
        }

        public void FillListViewStockDetail(ListView obj, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            obj.Items.Clear();

            GetTerminalSNList("View", "Stock Detail List", SearchValue, "Stock Detail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound() == false)
            {
                obj.Items.Clear();
                while (clsArray.TerminalID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());

                    item.ForeColor = dbFunction.GetColorByStatus(int.Parse(clsArray.TerminalStatus[i]), clsArray.TerminalStatusDescription[i]); // set forecolor per status

                    item.SubItems.Add(clsArray.TerminalID[i].ToString());
                    item.SubItems.Add(clsArray.TerminalSN[i]);
                    item.SubItems.Add(clsArray.TerminalType[i]);
                    item.SubItems.Add(clsArray.TerminalModel[i]);
                    item.SubItems.Add(clsArray.TerminalBrand[i]);
                    item.SubItems.Add(clsArray.TerminalStatus[i].ToString());
                    item.SubItems.Add(clsArray.TerminalStatusDescription[i]);
                    item.SubItems.Add(clsArray.ClientName[i]);
                    item.SubItems.Add(clsArray.MerchantName[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.IRNo[i]);
                    item.SubItems.Add(clsArray.Location[i]);
                    item.SubItems.Add(clsArray.Allocation[i]);
                    item.SubItems.Add(clsArray.AssetType[i]);
                    item.SubItems.Add(clsArray.DeliveryDate[i]);
                    item.SubItems.Add(clsArray.ReceiveDate[i]);
                    item.SubItems.Add(clsArray.ReleaseDate[i]);

                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

        }

        public void BulkUpdateStockMovementDetail(ListView lvw, int pClientID, bool isDispatch)
        {
            string sRowSQL = "";
            string sSQL = "";
            int iItemID = 0;
            string sSerialNo = "";
            int pStatus = 0;
            int pLocationID = 0;
            string pRemarks = "-";

            Debug.WriteLine("--BulkUpdateStockMovementDetail--");
            
            if (lvw.Items.Count > 0)
            {
                foreach (ListViewItem i in lvw.Items)
                {
                    sSQL = "";

                    foreach (ListViewItem x in lvw.Items)
                    {
                        iItemID = int.Parse(x.SubItems[1].Text);
                        sSerialNo = x.SubItems[6].Text;

                        pLocationID = 0;
                        dbFunction.GetIDFromFile("Location", x.SubItems[7].Text);
                        pLocationID = clsSearch.ClassOutFileID;

                        pStatus = 0;
                        if (isDispatch)
                            pStatus = clsGlobalVariables.STATUS_DISPATCH;

                        dbFunction.GetIDFromFile("Status List", x.SubItems[8].Text);
                        pStatus = clsSearch.ClassOutFileID;

                        // Update
                        sRowSQL = "";
                        sRowSQL = iItemID + "~" +
                        sRowSQL + sRowSQL + "" + pStatus + "~" +
                        sRowSQL + sRowSQL + "" + pClientID + "~" +
                        sRowSQL + sRowSQL + "" + pLocationID + "~" +
                        sRowSQL + sRowSQL + "" + clsSearch.ClassCurrentUserID + "~" +
                        sRowSQL + sRowSQL + "" + dbFunction.getCurrentDateTime() + "~" +
                        sRowSQL + sRowSQL + "" + pRemarks;

                        Debug.WriteLine("sRowSQL=" + sRowSQL + "\n");

                        dbFunction.parseDelimitedString(sRowSQL, clsDefines.gTilde, 1);

                        if (sSQL.Length > 0)
                            sSQL = sSQL + "|" + sRowSQL;
                        else
                            sSQL = sSQL + sRowSQL;

                    }

                    sSQL = lvw.Items.Count.ToString() + "^" + sSQL;

                    Debug.WriteLine("sSQL=" + sSQL);

                    dbFunction.parseDelimitedString(sSQL, clsDefines.gCaret, 1);

                    dbAPI.ExecuteAPI("PUT", "Update", "Multiple Stock Movement Detail", sSQL, "", "", "UpdateBulkCollectionDetail");
                }
            }
            
        }

        public string getStockkMovementDetail(string SearchBy, string SearchValue)
        {
            int i = 0;
            int iLineNo = 0;
            string pOutput = "";
            
            ExecuteAPI("GET", "View", SearchBy, SearchValue, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return pOutput;

            if (isNoRecordFound() == false)
            {   
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;                    
                    pOutput += dbFunction.padLeftChar(iLineNo.ToString(), "0", 2) + "." +
                                                 dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalModel) + " " + clsFunction.sPipe +
                                                 dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SerialNo) + clsDefines.gComma;

                    i++;

                }                
            }
            
            return pOutput;
        }

        public bool isPromptAdminLogIn()
        {
            bool isValid = false;

            frmLogInOverride frm = new frmLogInOverride();
            frm.ShowDialog();

            if (frmLogInOverride.fSelected)
            {
                dbFunction.SetMessageBox("You may now continue the process you want.!", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iInformation);
                isValid = true;
            }

            return isValid;
        }

        public void loadMultiMerchantInfo(ListView obj, int pIRIDNo)
        {
            int lineNo = 0;
            int i = 0;
            int counter = 5;


            dbFunction.ClearListViewItems(obj);
            if (dbFunction.isValidID(pIRIDNo.ToString()))
            {
                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Multi Merchant Info", pIRIDNo.ToString());
                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                if (dbFunction.isValidDescription(pJSONString))
                {
                    do
                    {
                        lineNo++;
                        ListViewItem item = new ListViewItem(lineNo.ToString());

                        string pName = dbAPI.GetValueFromJSONString(pJSONString, "MM_Name_" + lineNo);
                        string pMID = dbAPI.GetValueFromJSONString(pJSONString, "MM_MID_" + lineNo);
                        string pTID = dbAPI.GetValueFromJSONString(pJSONString, "MM_TID_" + lineNo);
                        item.SubItems.Add(pName.Equals("NULL") || pName.Equals("-") ? "-" : pName.Trim());
                        item.SubItems.Add(pTID.Equals("NULL") || pTID.Equals("-")  ? "-" : dbFunction.padLeftChar(pTID, clsFunction.sZero, clsFunction.TID_LENGTH));
                        item.SubItems.Add(pMID.Equals("NULL") || pMID.Equals("-")  ? "-" : dbFunction.padLeftChar(pMID, clsFunction.sZero, clsFunction.MID_LENGTH));
                        
                        obj.Items.Add(item);
                        i++;

                    }
                    while (i < counter);

                    dbFunction.ListViewAlternateBackColor(obj);

                }
            }
        }

        public string[] GetGroupType()
        {
            string[] ret = { clsFunction.sDefaultSelect, "SIM TYPE", "POS TYPE", "BILLING TYPE", "APPS TYPE", "ASSET TYPE", "POS SETUP TYPE", "PLAN TYPE", "TERM TYPE", "BILL TYPE", "SOURCE TYPE", "CATEGORY TYPE", "SUB CATEGORY TYPE" , "MSP-CATEGORY", "MSP-NATURE OF BUSINESS", "MSP-BUSINESS TYPE", "MSP-REFERRAL TYPE", "MSP-SCHEME", "MSP-STATUS", "MSP-ACQUIRER", "MSP-MDR CREDIT", "MSP-MDR DEBIT", "MSP-MDR INST", "MSP-RESULT", "DEPEDENCY", "STATUS REASON"};

            return ret;
        }

        public void FillComboBoxGroupType(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetGroupType().Length > i)
            {
                obj.Items.Add(GetGroupType()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public string[] GetReasonType()
        {
            string[] ret = { clsFunction.sDefaultSelect, "REASON", "HARDWARE", "SOFTWARE", "RESOLUTION", "NEGATIVE" };

            return ret;
        }

        public void FillComboBoxReasonType(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetReasonType().Length > i)
            {
                obj.Items.Add(GetReasonType()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxDepedency(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.Depedency);

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        if (!fSelect)
                        {
                            obj.Items.Add(clsFunction.sDefaultSelect);
                            fSelect = true;
                        }

                        obj.Items.Add(itemData.Description);

                    }

                    if (i > 0)
                        obj.SelectedIndex = 0;

                }
            }
        }

        public void FillComboBoxStatusReason(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TypeController> mList = null;

            obj.Items.Clear();
            mList = _mTypeController.getDetailList(clsFunction.sZero + clsFunction.sPipe + (int)OtherType.StatusReason);

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        if (!fSelect)
                        {
                            obj.Items.Add(clsFunction.sDefaultSelect);
                            fSelect = true;
                        }

                        obj.Items.Add(itemData.Description);

                    }

                    if (i > 0)
                        obj.SelectedIndex = 0;

                }
            }
        }

        public void FillComboBoxServiceStatusCategory(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;

            GetServiceTypeList("View", "Service Type Status Category", "", "Service Type");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (isNoRecordFound()) return;

            obj.Items.Clear();
            while (clsArray.ServiceTypeID.Length > i)
            {
                clsServiceType.ClassDescription = clsArray.ServiceJobTypeDescription[i];

                if (!fSelect)
                {
                    obj.Items.Add(clsFunction.sDefaultSelect);
                    fSelect = true;
                }

                obj.Items.Add(clsServiceType.ClassDescription);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

        public void FillComboBoxMobileTerminal(ComboBox obj)
        {
            int i = 0;
            bool fSelect = false;
            List<TerminalController> mList = null;

            obj.Items.Clear();
            mList = _mTerminalController.getDetailList(clsFunction.sNull);

            if (mList != null)
            {
                if (mList != null)
                {
                    foreach (var itemData in mList)
                    {
                        if (!fSelect)
                        {
                            obj.Items.Add(clsFunction.sDefaultSelect);
                            fSelect = true;
                        }

                        obj.Items.Add(itemData.MobileTerminalID);

                    }

                    if (i > 0)
                        obj.SelectedIndex = 0;

                }
            }
        }

        public void insertReportStatus(string pReportDesc, int processType, string pStatus, string pProcesseedAt, string pProcessedBy)
        {
            string sSQL = "";
            
            var data = new
            {
                ReportDesc = pReportDesc,
                StatusID = processType,
                Status = pStatus,
                ProcessedAt = pProcesseedAt,
                ProcessedBy = pProcessedBy

            };

            sSQL = IFormat.Insert(data);

            Debug.WriteLine("insertReportStatus" + sSQL);

            dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Report Status", sSQL, "InsertMaintenanceMaster");
        }

        public Dictionary<string, string> getDeployedLocation()
        {
            return new Dictionary<string, string>
            {
                { "0", "[NOT SPECIFIED]" },
                { "18", "PRODUCTION" }
            };
        }

        public Dictionary<string, string> getReturnLocation()
        {
            return new Dictionary<string, string>
            {
                { "0", "[NOT SPECIFIED]" },
                { "1", "STOCK ROOM" },
                { "2", "CITAS F.E" },
                { "4", "DAVAO" },
                { "5", "CEBU" },
                { "6", "ILOILO" },
                { "9", "NCR MT" },
                { "10", "THIRD PARTY" },
                { "20", "NORTH LUZON MT" },
                { "21", "LEYTE" },
                { "22", "SOUTH LUZON MT" },
                { "23", "VISAYAS MT" },
                { "24", "MINDANAO MT" },
                { "25", "CDO" }
            };
        }

        public void LoadDeployedLocation(ComboBox combo)
        {
            combo.DataSource = new BindingSource(getDeployedLocation(), null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
        }

        public void LoadReturnedLocation(ComboBox combo)
        {
            combo.DataSource = new BindingSource(getReturnLocation(), null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
        }

        public static Dictionary<string, string> getDeployedStatus()
        {
            return new Dictionary<string, string>
            {
                { "0", "[NOT SPECIFIED]" },
                { "7", "INSTALLED" }
            };
        }

        public static Dictionary<string, string> getReturntatus()
        {
            return new Dictionary<string, string>
            {
                { "0", "[NOT SPECIFIED]" },
                { "1", "AVAILABLE" },
                { "4", "DEFECTIVE" },
                { "5", "LOST" },                
                { "6", "LOAN/BORROW" },
                { "3", "FOR REPAIR" }
            };
        }

        public void LoadDeployedStatus(ComboBox combo)
        {
            combo.DataSource = new BindingSource(getDeployedStatus(), null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
        }

        public void LoadReturnedStatus(ComboBox combo)
        {
            combo.DataSource = new BindingSource(getReturntatus(), null);
            combo.DisplayMember = "Value";
            combo.ValueMember = "Key";
        }

        public void viewTerminalHistory(ListView lvw, string pID)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();
            
            lvw.Items.Clear();

            if (dbFunction.isValidID(pID))
            {
                dbAPI.ExecuteAPI("GET", "View", "Service TerminalSN History List", dbFunction.CheckAndSetNumericValue(pID), "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (dbAPI.isNoRecordFound() == false)
                {
                    while (clsArray.ServiceNo.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FSRNO));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MERCHANTNAME));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MID));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ServiceJobTypeDescription));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRNO));

                        // Servicing Date Info
                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Servicing Date Info", dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TicketDate));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ActionMade));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SIMSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceTerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceSIMSN));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_JobTypeStatusDescription));

                        lvw.Items.Add(item);

                        i++;
                    }

                    dbFunction.ListViewAlternateBackColor(lvw);
                }
            }

        }

        public void viewSIMHistory(ListView lvw, string pID)
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();
            
            lvw.Items.Clear();

            if (dbFunction.isValidID(pID))
            {
                dbAPI.ExecuteAPI("GET", "View", "Service SIMSN History List", dbFunction.CheckAndSetNumericValue(pID), "Advance Detail", "", "ViewAdvanceDetail");

                if (!clsGlobalVariables.isAPIResponseOK) return;

                if (dbAPI.isNoRecordFound() == false)
                {
                    //lvwHistoryList.Items.Clear();
                    while (clsArray.ServiceNo.Length > i)
                    {
                        // Add to List
                        iLineNo++;
                        ListViewItem item = new ListViewItem(iLineNo.ToString());

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FSRNO));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MERCHANTNAME));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TID));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_MID));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ServiceJobTypeDescription));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_IRNO));

                        // Servicing Date Info
                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Servicing Date Info", dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SERVICENO));
                        dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_CreatedDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatchDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceDate));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TicketDate));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ActionMade));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SIMSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceTerminalSN));
                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_ReplaceSIMSN));

                        item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_JobTypeStatusDescription));

                        lvw.Items.Add(item);

                        i++;
                    }

                    dbFunction.ListViewAlternateBackColor(lvw);
                }
            }

        }

        public string getAPISSLEnable()
        {
            bool isEnable = false;
            string output = "";
            
            isEnable = isSSLEnable();

            if (isEnable)
                output = "https://";
            else
                output = "http://";

            return output;
        }

        public bool isSSLEnable()
        {
            bool isSSL = false;

            if (clsGlobalVariables.strAPISSLEnable > 0)
                isSSL = true;
            else
                isSSL = false;

            return isSSL;
        }

        public void updateSignature(int pServiceNo, int pFSRNo)
        {
            // generate filename                                  
            string mFileName = $"{pServiceNo.ToString()}_{dbFunction.padLeftChar("5", clsFunction.sZero, 2)}.png";
            string vFileName = $"{pServiceNo.ToString()}_{dbFunction.padLeftChar("6", clsFunction.sZero, 2)}.png";

            string pSearchValue = pFSRNo.ToString() + "|" + pServiceNo.ToString() + "|" + (mFileName) + "|" + (vFileName);

            // update signature1/signature2 using filename
            Debug.WriteLine("Signature, pSearchValue=" + pSearchValue);
            ExecuteAPI("PUT", "Update", "Update Signature", pSearchValue, "", "", "UpdateCollectionDetail");
        }

        public string deleteFileInfo(string StatementType, string SearchBy, string SearchValue)
        {
            string pOutput = "";
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "DeleteFileInfo", "", "DeleteFileInfo");           
            if (clsSearch.ClassOutParamValue.Length > 0)
            {
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                if (!isNoRecordFound())
                {
                    pOutput = clsSearch.ClassOutParamValue;

                }
            }

            return pOutput;
        }

        public string checkFileInfo(string StatementType, string SearchBy, string SearchValue)
        {
            string pOutput = "";
            ExecuteAPI("GET", StatementType, SearchBy, SearchValue, "CheckFileInfo", "", "CheckFileInfo");
            if (clsSearch.ClassOutParamValue.Length > 0)
            {
                jsonObj obj = JsonConvert.DeserializeObject<jsonObj>(clsSearch.ClassOutParamValue);

                if (!isNoRecordFound())
                {
                    pOutput = clsSearch.ClassOutParamValue;

                }
            }

            return pOutput;
        }

        public string[] getReportStatusSelection()
        {
            string[] ret = { clsFunction.sDefaultSelect, "INCLUDED", "EXCLUDED" };

            return ret;
        }

        public void FillComboBoxReportStatus(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (getReportStatusSelection().Length > i)
            {
                obj.Items.Add(getReportStatusSelection()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;
        }

    }

}
