using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Function
{
    public class ErrorModel
    {
        public string ErrorCode { get; set; }
        public string FieldName { get; set; }
        public string Description { get; set; }
        public string Resolution { get; set; }
    }

    public class ErrorHelper
    {
        private static readonly Dictionary<string, (string Description, string Resolution)> ErrorMappings =
            new Dictionary<string, (string Description, string Resolution)>
            {
            { "ERR001", ("Invalid input data", "Check the input and try again.") },
            { "ERR002", ("Connection timeout", "Ensure network connectivity and retry.") },
            { "ERR003", ("Unauthorized access", "Verify credentials and try again.") },
            { "ERR004", ("File not found", "Ensure the file exists and has the correct path.") }
            };

        public static ErrorModel GetErrorModel(string errorCode, string fieldName)
        {
            if (ErrorMappings.TryGetValue(errorCode, out var errorInfo))
            {
                return new ErrorModel
                {
                    ErrorCode = errorCode,
                    FieldName = fieldName,
                    Description = errorInfo.Description,
                    Resolution = errorInfo.Resolution
                };
            }

            return null; 
        }
    }
}
