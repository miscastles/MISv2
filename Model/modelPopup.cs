using System;
using System.Collections.Generic;

namespace MIS.Model
{
    public class modelPopup
    {
        public string Title { get; set; } = "POPUP";
        public string Message { get; set; } = "";

        public string ProceedText { get; set; } = "PROCEED";
        public string CancelText { get; set; } = "CANCEL";

        public List<KeyValuePair<string, string>> Details { get; } = new List<KeyValuePair<string, string>>();

        public modelPopup() { }

        public modelPopup(string pTitle, string pMessage)
        {
            Title = pTitle;
            Message = pMessage;
        }

        public modelPopup AddDetail(string pLabel, object pValue)
        {
            string sValue;

            if (pValue == null || pValue == DBNull.Value)
                sValue = "";
            else if (pValue is DateTime)
                sValue = ((DateTime)pValue).ToString(clsFunction.sStandardDateDefault);
            else
                sValue = pValue.ToString();

            Details.Add(new KeyValuePair<string, string>(pLabel ?? "", sValue));
            return this;
        }

        public void ClearDetails()
        {
            Details.Clear();
        }
    }
}