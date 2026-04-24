using System;
using System.Diagnostics;
using MIS.AppConnection;
using MIS.AppData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static MIS.AppData.ConstData.Api;
using static MIS.AppData.JsonData;
using static MIS.AppData.SettingsData;
using static MIS.Function.AppUtilities;

namespace MIS.AppMainActivity
{
    class ApiControl
    {  

        private ApiConn Conn;
        private ApiSettings api;
        private RespMsg Resp;

        private string BaseUrl { get; set; }

        public ApiControl()
        {
            api = new ApiSettings();
            //Api = new IniFileFunc().ReadSettings<ApiSettings>();

            // Base Url
            BaseUrl = $"http://{api.Host}{api.Path}{ConstData.Url.ApiKey}{api.Keys}{ConstData.Url.Action}";
        }

        private void VerifyResponse(RespMsg res)
        {
            if (res != null)
            {
                switch (res.Code)
                {
                    case RespCode.SUCCESS:

                        Debug.WriteLine(sDisplay.Separator);
                        Debug.WriteLine($"Code     : {res.Code}");
                        Debug.WriteLine($"Record   : {res.Record}");
                        Debug.WriteLine($"Message  : {res.Message}");
                        if (res.Data != null) { Debug.WriteLine("Data     : " + res.Data.ToString(Formatting.None)); }
                        Debug.WriteLine(sDisplay.Separator + "\n");

                        break;

                    case RespCode.BAD_REQUEST:
                        Prompt.Error("Bad Request Error", res.Message);
                        return;

                    case RespCode.REQUEST_METHOD_NOT_VALID:
                        Prompt.Error("Response Error", $"Request method not valid: {res.Message}");
                        return;

                    case RespCode.REQUEST_CONTENTTYPE_NOT_VALID:
                        Prompt.Error("Response Error", $"Request content type not valid: {res.Message}");
                        return;

                    case RespCode.REQUEST_NOT_VALID:
                        Prompt.Error("Response Error", $"Request not valid: {res.Message}");
                        return;

                    case RespCode.VALIDATE_PARAMETER_REQUIRED:
                        Prompt.Error("Response Error", $"Validation parameter required: {res.Message}");
                        return;

                    case RespCode.VALIDATE_PARAMETER_DATATYPE:
                        Prompt.Error("Response Error", $"Validation parameter datatype: {res.Message}");
                        return;

                    case RespCode.API_NAME_REQUIRED:
                        Prompt.Error("Response Error", $"Name required: {res.Message}");
                        return;

                    case RespCode.API_PARAM_REQUIRED:
                        Prompt.Error("Response Error", $"Parameter required: {res.Message}");
                        return;

                    case RespCode.API_KEY_REQUIRED:
                        Prompt.Error("Response Error", $"Api Key required: {res.Message}");
                        return;

                    case RespCode.INVALID_AUTH_USER_PW:
                        Prompt.Error("Invalid Credentials", $"Invalid Api Auth Credentials: {res.Message}");
                        return;

                    case RespCode.INVALID_METHOD:
                        Prompt.Error("Invalid Method Error", $"Invalid Method: {res.Message}");
                        return;

                    case RespCode.API_SCRIPT_ERROR:
                        Prompt.Error("Api Script Error", $"Api Script Error: {res.Message}");
                        return;

                    case RespCode.STORED_PROCEDURE_ERROR:
                        Prompt.Error("Stored Procedure Error", $"An error occured: {res.Message}");
                        return;

                    default:

                        Prompt.Error("Server Response Error", $"An error occurred: \n{res.Message}");

                        Debug.WriteLine($"{sDisplay.Separator}");
                        Debug.WriteLine($"Code     : {res.Code}");
                        Debug.WriteLine($"Record   : {res.Record}");
                        Debug.WriteLine($"Message  : {res.Message}");
                        if (res.Data != null) { Debug.WriteLine("Data     : " + res.Data.ToString(Formatting.None)); }
                        Debug.WriteLine(sDisplay.Separator);

                        return;
                }
            }

            return;
        }

        private string GetBaseUrl(string Procedure, string pStatement, string pSearchBy, string pSearchVal, string pSql)
        {
            string baseUrl = null;

            switch (pStatement)
            {
                case "View":
                    baseUrl = $"{BaseUrl}{Procedure}&pStatement={pStatement}&pSearchBy={pSearchBy}&pSearchVal={pSearchVal}&pSql={pSql}";
                    break;

                case "Update":
                case "Insert":
                case "Delete":
                    baseUrl = $"{BaseUrl}{Procedure}";
                    break;

                case "Imports":
                    baseUrl = $"{BaseUrl}{Procedure}";
                    break;

                default:
                    Prompt.Error("Api Request", "Statement Method Invalid");
                    break;
            }

            return baseUrl;
        }

        private async Task<RespMsg> ParseResponse(ApiConn Conn, string pStatement)
        {
            switch (pStatement)
            {
                case "View":
                    return JFunc.Deserialized<RespMsg>(await Conn.ExeAsyncJsonRequest(HttpMethod.Get));

                case "Update":
                    return JFunc.Deserialized<RespMsg>(await Conn.ExeAsyncJsonRequest(HttpMethod.Put));

                case "Insert":
                    return JFunc.Deserialized<RespMsg>(await Conn.ExeAsyncJsonRequest(HttpMethod.Post));

                case "Delete":
                    return JFunc.Deserialized<RespMsg>(await Conn.ExeAsyncJsonRequest(HttpMethod.Delete));

                case "Imports":
                    return JFunc.Deserialized<RespMsg>(await Conn.ExeAsyncJsonRequest(HttpMethod.Post));

                default:
                    return null;
            }
        }

        private void DebugDetails(string Procedure, string Statement, string SearchBy, string SearchVal, string Sql)
        {
            Debug.WriteLine($"\n{sDisplay.Separator}");
            Debug.WriteLine("Request Details");
            Debug.WriteLine(sDisplay.Separator);
            Debug.WriteLine($"Statement : {Statement}");
            Debug.WriteLine($"SearchBy  : {SearchBy}");

            if (!string.IsNullOrEmpty(SearchVal))
                Debug.WriteLine($"SearchVal : {SearchVal}");

            if (!string.IsNullOrEmpty(Sql))
            {
                if (Sql.Contains("),("))
                {
                    string SqlResult = string.Join("\n", Sql.Split(new[] { "),(" }, StringSplitOptions.None).Select(batch => $" {batch}"));

                    // Display the generated batches in debug output
                    Debug.WriteLine($"Sql:");
                    Debug.WriteLine($"{SqlResult}");
                }
                else
                {
                    // Display the generated batches in debug output
                    Debug.WriteLine($"Sql       : {Sql}");
                }
            }

            Debug.WriteLine(sDisplay.Separator);
            Debug.WriteLine($"Retrieving : Call {Procedure}(\"{Statement}\", \"{SearchBy}\", \"{SearchVal}\", \"{Sql}\");");
            Debug.WriteLine($"{sDisplay.Separator}\n");
        }

        public async Task<JArray> AppMainActiviy(string pStatement, string pSearchBy, string pSearchVal, string pSql)
        {
            Conn = new ApiConn(api.AuthUser, api.AuthPassword);

            Conn.ContentBody = pSql != null ? JFunc.Serialized(new { pStatement, pSearchBy, pSearchVal, pSql }) : null;

            DebugDetails("spMainActivity", pStatement, pSearchBy, pSearchVal, pSql);

            Conn.Url = GetBaseUrl("MainActivity", pStatement, pSearchBy, pSearchVal, pSql);

            Resp = await ParseResponse(Conn, pStatement);

            VerifyResponse(Resp);

            return Resp.Data;
        }

    }
}
