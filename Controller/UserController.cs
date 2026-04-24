using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MIS.Controller
{
    public class UserController
    {
        private static clsFunction dbFunction = new clsFunction();
        private static clsAPI dbAPI = new clsAPI();

        public int UserID { get; set; }
        public int ParticularID { get; set; }
        public int MobileID { get; set; }
        public int isAppVersion { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MD5Password { get; set; }
        public string UserType { get; set; }
        public string MobileTerminalID { get; set; }
        public string MobileTerminalName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        private UserController setInitValue()
        {
            UserController model = new UserController();

            model.UserID = UserID;
            model.ParticularID = ParticularID;
            model.MobileID = MobileID;
            model.isAppVersion = isAppVersion;
            model.FullName = FullName;
            model.UserName = UserName;
            model.Password = Password;
            model.MD5Password = MD5Password;
            model.UserType = UserType;
            model.MobileTerminalID = MobileTerminalID;
            model.MobileTerminalName = MobileTerminalName;
            model.Mobile = Mobile;
            model.Email = Email;

            return model;
        }

        public UserController getInfo(int pID)
        {
            UserController model = new UserController();

            if (dbFunction.isValidID(pID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "User Info", pID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                try
                {
                    dbFunction.parseDelimitedString(clsSearch.ClassOutParamValue, clsDefines.gPipe, 0);

                    if (clsSearch.ClassOutParamValue.Length > 0)
                    {
                        model.UserID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0));
                        model.ParticularID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1));
                        model.UserName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                        model.Password = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                        model.FullName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                        model.UserType = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                        model.Mobile = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                        model.Email = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                        model.MobileID = int.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8));
                        model.MobileTerminalID = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                        model.MobileTerminalName = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                        model.MD5Password = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);

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
    }
}
