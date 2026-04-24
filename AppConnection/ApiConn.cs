using MIS.AppData;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static MIS.AppData.JsonData;
using static MIS.Function.AppUtilities;

namespace MIS.AppConnection
{
    class ApiConn
    {
        private HttpClient httpClient;
        private HttpResponseMessage HttpRespMsg;

        // Credentials
        public string Url { get; set; }
        private string AuthUser { get; set; }
        private string AuthPass { get; set; }

        // Paramaters
        public string ContentBody { get; set; }
        private string DebugResp { get; set; }


        public ApiConn(string user, string pass)
        {
            AuthUser = user;
            AuthPass = pass;
        }

        public async Task<string> ExeAsyncJsonRequest(HttpMethod Method)
        {
            // Request
            HttpRespMsg = await SendAsyncRequest(Method);

            if (HttpRespMsg != null)
            {
                // Check Details
                DebugDetails(Method.ToString(), HttpRespMsg.IsSuccessStatusCode ? $"Response : OK  {Url}" : $"Response : Failed  {Url}");

                return HttpRespMsg.IsSuccessStatusCode ? await HttpRespMsg.Content.ReadAsStringAsync() : ErrorResponse("Invalid Url");
            }

            // Return Json Error
            return ErrorResponse("The server could not be reached.");

        }

        private async Task<HttpResponseMessage> SendAsyncRequest(HttpMethod Method)
        {
            httpClient = new HttpClient();

            try
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{AuthUser}:{AuthPass}"));

                using (var request = new HttpRequestMessage(Method, Url))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    if (Method != HttpMethod.Get && ContentBody != null)
                    {
                        var content = new StringContent(ContentBody, Encoding.UTF8, ConstData.Url.ContentType);
                        request.Content = content;
                    }

                    return await httpClient.SendAsync(request);
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"An error occured: \n{httpEx.Message}", "HttpRequestException SendRequest");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occured: \n{ex.Message}", "Exception SendRequest");
                return null;
            }
        }

        private void DebugDetails(string Method, string Result)
        {
            Debug.WriteLine($"\n{sDisplay.Separator}");
            Debug.WriteLine("Response Details");
            Debug.WriteLine(sDisplay.Separator);
            Debug.WriteLine($"Request  : {Method} {Url}");
            Debug.WriteLine(Result);
        }

        private string ErrorResponse(string Message)
        {
            var RespError = new RespMsg
            {
                Code = "201",
                Message = Message,
                Record = "0",
                Data = null
            };

            return JsonConvert.SerializeObject(RespError);

        }
    }
}
