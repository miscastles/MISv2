using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.AppData
{
    class ConstData
    {
        public struct Url
        {
            // Define Url Paramaters
            public const string sHttps      = "http://";
            public const string Statement   = "&pStatementType=";
            public const string SearchBy    = "&pSearchBy=";
            public const string SearchValue = "&pSearchValue=";
            public const string Sql         = "&pSql=";
            public const string ApiKey      = "?ApiKey=";
            public const string Action      = "&Action=";
            public const string ContentType = "application/json";
        }

        public static class Api
        {
            public const string SECRETE_KEY = "!Rockz";

            public static class RespCode
            {
                public const string REQUEST_METHOD_NOT_VALID = "100";
                public const string REQUEST_CONTENTTYPE_NOT_VALID = "101";
                public const string REQUEST_NOT_VALID = "102";
                public const string VALIDATE_PARAMETER_REQUIRED = "103";
                public const string VALIDATE_PARAMETER_DATATYPE = "104";
                public const string API_NAME_REQUIRED = "105";
                public const string API_PARAM_REQUIRED = "106";
                public const string API_DOES_NOT_EXIST = "107";
                public const string INVALID_USER_PASS = "108";
                public const string USER_NOT_ACTIVE = "109";
                public const string NO_RECORD_FOUND = "110";
                public const string RECORD_FOUND = "111";
                public const string API_KEY_REQUIRED = "112";
                public const string INVALID_AUTH_USER_PW = "113";
                public const string INVALID_METHOD = "114";
                public const string SUCCESS = "200";
                public const string FAILED_RESPONSE = "201";
                public const string BAD_REQUEST = "400";
                public const string AUTHORIZATION_HEADER_NOT_FOUND = "302";
                public const string ACCESS_TOKEN_ERROR = "303";
                public const string STORED_PROCEDURE_ERROR = "401";
                public const string API_SCRIPT_ERROR = "402";

                // Exemption Errors
                public const string REQUEST_FAILED = "500";
                public const string REQUEST_TIMEOUT = "501";
                public const string UNKNOWN_ERROR = "502";
            }

            public static class Messages
            {
                public const string RECORD_FOUND_MSG = "Record found";
                public const string NO_RECORD_FOUND_MSG = "Record not found";
                public const string TABLE_PAGE_MSG = "Table Page";
                public const string EMAIL_SENT_SUCCESS_MSG = "Email sent complete";
                public const string EMAIL_SENT_FAILED_MSG = "Email sent failed";
                public const string SMS_SENT_SUCCESS_MSG = "SMS sent complete";

                // Exemption Errors
                public const string ERROR_REQUEST_FAILED = "Failed to make an HTTP request.";
                public const string ERROR_REQUEST_TIMEOUT = "The request was canceled or timed out";
                public const string ERROR_UNKNOWN = "An unknown error occurred while running Method";

                // ... Add more message constants here
            }
        }
    }
}
