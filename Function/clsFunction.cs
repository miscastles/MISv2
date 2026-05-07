using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using static MIS.Function.AppUtilities;
using System.Drawing.Drawing2D;
using System.Net.Http;

namespace MIS
{
    public class clsFunction
    {
        public const string allowedCharacters = " 1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$^&*()_-+=[]{}|<>/.";
        public const int _ALIGN_CENTER = 0;
        public const int _ALIGN_LEFT = 1;
        public const int _ALIGN_RIGHT = 2;
        public const int _ALIGN_LEFT_RIGHT = 3;
        public const string sPadSpace = " ";
        public const string sPadZero = "0";
        public const string sPipe = "|";
        public const string sDummy = "";
        public const string sNull = "";
        public const string sDash = "-";
        public const string sComma = ",";
        public const string sZero = "0";
        public const string sOne = "1";
        public const string sAsterisk = "*";
        public const string sCaret = "^";
        public const string sSingleQuote = "'";
        public const int inCharPerLine = 30;
        public const int iZero = 0;
        public const string sNone = "99";
        public const string sDateFormat = "0000-00-00";
        public const string sDateFormatMDY = "00-00=0000";
        public const string sValueDateFormat = "yyyy-MM-dd";
        public const string sTimeFormat = "00:00:00";
        public const string sDateDefault = "00-00-0000";
        public const string sTimeDefault = "00:00:00 00";
        public const string sInvalidTime = "00:00:00";
        public const string sDefaultAmount = "0.00";
        public const string sDateDefaultFormat = "ddd, MMM-dd-yyyy";
        public const string sTimeDefaultFormat = "hh:mm tt";
        public const string sStandardDateDefault = "MMM-dd-yyyy";
        public const int iDefaultID = 0;
        public const string sYes = "Y";
        public const string sNo = "N";
        public const string sBlank = "Blank";
        public const string sLineSeparator = "=======================================";
        public const string sSingleLineSeparator = "---------------------------------------";
        public const string slvSeparator = "-----------";
        public const string sBackSlash = "/";
        public const string sForwardSlash = "\\";
        public const string sQuestionMark = "?";
        public const string sEqualSign = "=";
        public const string sAnd = "&";
        public const string sAndString = "AND";
        public const string sPeriod = ".";
        public const string sColon = ":";
        public const string sNotApplicable = "99";
        public const string sPosted = "POSTED";

        public const string sDevicePC = "-";
        public const string sPC = "PC";

        public const string sDeviceMobile = "XXXXXXXX";
        public const string sMobile = "MOBILE";

        public const string sDeviceEntry = "x";
        public const string sEntry = "ENTRY";


        public const string sNonePNG = "none.png";
        public const string sX = "x";
        public const char cPipe = '|';

        int iProgressBarMax = 0;
        public const int iOneRecordOnly = 1;
        public const int TID_LENGTH = 8;
        public const int MID_LENGTH = 15;

        public const int REQUESTID_LENGTH = 23;
        public const int REFERENCE_LENGTH = 23;

        public const int iLimit = 25;
        //public const int iOffSetTo = 500;
        public const int iOffSetTo = 50;
        public const int iOffSetToSerialNo = 5;
        public static readonly Color SearchBackColor = Color.LightGreen;
        public static readonly Color PKBackColor = Color.FromArgb(255, 255, 230);
        public static readonly Color MKBackColor = Color.FromArgb(230, 255, 255);
        public static readonly Color EntryBackColor = Color.White;
        public static readonly Color DisableBackColor = Color.FromArgb(242, 242, 242);
        public static readonly Color ListViewBackColor = Color.Azure;
        public static readonly Color GridViewBackColor = Color.Azure;
        public static readonly Color DateBackColor = Color.Lavender;
        public static readonly Color NewBackColor = Color.SpringGreen;
        public static readonly Color AlternateBackColor1 = Color.Snow;
        public static readonly Color AlternateBackColor2 = Color.AliceBlue;
        //public static readonly Color AlternateBackColor1 = Color.FromArgb(255, 255, 255);
        //public static readonly Color AlternateBackColor2 = Color.FromArgb(250, 250, 250);
        public static readonly Color InvalidBackColor = Color.Silver;
        public static readonly Color CountBackColor = Color.Lime;
        public static readonly Color BackColorAzure = Color.MintCream;
        public static readonly Color StatusBackColor = Color.FromArgb(230, 245, 255);

        public static readonly Color AddButtonBackColor = Color.FromArgb(0, 102, 255);
        public static readonly Color SaveButtonBackColor = Color.FromArgb(0, 153, 51);
        public static readonly Color ClearButtonBackColor = Color.FromArgb(230, 230, 0);
        public static readonly Color DeleteButtonBackColor = Color.FromArgb(230, 57, 0);
        public static readonly Color PrintButtonBackColor = Color.FromArgb(0, 128, 0);

        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        public const string initVector = "1234567890123456";
        // This constant is used to determine the keysize of the encryption algorithm
        public const int keysize = 256;
        // System Password
        public const string SystemPassword = "XNX951";
        public const string sDefaultSelect = "[NOT SPECIFIED]";
        public const string sUndefineStatus = "[UNDFINED STATUS]";

        // Expiry Date
        public const string userRoot = "HKEY_CURRENT_USER";
        public const string subkey = "WD2537_091180";
        public const string keyName = userRoot + "\\" + subkey;
        public const string MasterKey = "Skullbucks091180";
        public const int MasterKeySize = 256;

        public const int Numeric_Input = 0;
        public const int Alpha_Input = 2;
        public const int AlphaNumeric_Input = 3;

        public static int isNotActive = 0;
        public static int isActive = 1;

        private clsFile dbFile = new clsFile();

        private Dictionary<int, TabPage> hiddenTabs = new Dictionary<int, TabPage>();

        public enum CheckType
        {
            iClientID, iClientName,
            iMerchantID, iMerchantName, iMerchantAddress,
            iFEID, iFEName,          
            iSPID, iSPName,
            iEMPID,
            iParticularID,
            iSupplierID, iSupplierName,
            iIRIDNo, iIRNo, iServiceNo, iTAIDNo,
            iComments, iRemarks, iRequestNo, iReferenceNo, iRequestID,
            iCustomerName, iRequestBy,
            iMerchantRepresentative, iMerchantRepPosition, iMerchantContactNo, iMerchantEmail,
            iContactNo,
            iTerminalID, iTerminalSN, iTerminalStatus,
            iSIMID, iSIMSN, iSIMStatus,
            iDockID, iDockSN, iDockStatus,
            iRepTerminalID, iRepTerminalSN, iRepTerminalStatus,
            iRepSIMID, iRepSIMSN, iRepSIMStatus,
            iRepDockID, iRepDockSN, iRepDockStatus,
            iCurTerminalID, iCurTerminalSN, iCurTerminalStatus,
            iCurSIMID, iCurSIMSN, iCurSIMStatus,
            iCurDockID, iCurDockSN, iCurDockStatus,
            iDate, iTime,
            iTID, iMID,
            iServiceType,
            iRegion, iRegionType, iRegionID,
            iReason,
            iLeaveType,
            iDepartment,
            iPosition,
            iDateType,
            iEmploymentStatus,
            iCreditLimit,
            iActionMade,
            iActionTaken,
            iAppVersion,
            iAppCRC,
            iCountry,
            iMDRType,
            iLocation, iAllocation, iAssetType, iCarrier,
            iPrefix, iPadLen, iStartIndex, iEndIndex,
            iTerminalType, iTerminalModel, iTerminalBrand,
            iName, iCity, iProvince, iMobileNo, iEmail,
            iSpecialInstruction,
            iFromLocation, iToLocation,
            iItemList, iBatchNo, iService,
            iUserName, iPassword, iOldPassword, iNewPassword, iConfirmPassword,
            iInvoceNo, iAcntNo, iCustomerNo, iRentalType, iRentalTerms, iPOSType, iFromInvoice, iToInvoice,
            iRequestIDFrom, iRequestIDTo,
            iCurTerminalLocation, iCurSIMLocation,
            iRepTerminalLocation, iRepSIMLocation,
            iProfileInfo,
            iTIDLength, iMIDLength,
            iRequestIDLength, iReferenceNoLength,
            iDescription, iType,
            iStatus,
            iDispatchID, iiDispatcher,
            iVendor, iRequestor

        }

        private class DataGridViewRowData
        {
            public Dictionary<string, object> Values { get; set; }
        }

        private class ListViewRowData
        {
            public Dictionary<string, object> Values { get; set; }
        }

        public static string Left(string param, int length)
        {
            string result = param.Substring(0, length);
            return result;
        }
        public static string Right(string param, int length)
        {
            string result = param.Substring(param.Length - length, length);
            return result;
        }
        public static string Mid(string param, int startIndex, int length)
        {
            string result = param.Substring(startIndex, length);
            return result;
        }
        public string padLeftChar(string StringInput, string strPadChar, int padLen)
        {
            string StrValue = "";
            string StrPadded = "";
            int inLen, inCtr = 0;

            inLen = padLen - StringInput.Length;

            if (inLen > 0)
            {
                while (inLen > inCtr)
                {
                    StrValue += strPadChar;
                    inCtr++;
                }
            }
            else
            {
                StrPadded = StringInput;
            }

            StrPadded = StrValue + StringInput;
            return StrPadded;
        }
        public string padRightChar(string StringInput, string strPadChar, int padLen)
        {
            string StrValue = "";
            string StrPadded = "";
            int inLen, inCtr = 0;

            inLen = padLen;

            while (inLen > inCtr)
            {
                StrValue += strPadChar;
                inCtr++;
            }

            StrPadded = StringInput + StrValue;
            return StrPadded;
        }
        public string padLeftRight(string StringInput, string strPadChar, int padLen, int PadType)
        {
            string StrValue = "";
            string StrPadded = "";
            int inLen, inCtr = 0;

            inLen = padLen;

            switch (PadType)
            {
                case _ALIGN_LEFT: // Left                    
                    while (inLen > inCtr)
                    {
                        StrValue += strPadChar;
                        inCtr++;
                    }
                    break;
                case _ALIGN_RIGHT: // Right                    
                    while (inLen > inCtr)
                    {
                        StrValue += strPadChar;
                        inCtr++;
                    }
                    break;
            }

            StrPadded = StrValue;

            return StrPadded;
        }
        public string DisplayLeftRight(int Alignment, string sPrintLeft, string sPrintRight, ref string sToPrint)
        {
            int inLen = 0;
            int inSpaceLeft = 0;
            int inSpaceRight = 0;
            string sPaddedLeft = "";
            string sPaddedRight = "";
            string sPrintText = "";

            inLen = sPrintLeft.Length;

            switch (Alignment)
            {
                case _ALIGN_CENTER: // Center
                    inSpaceLeft = (inCharPerLine - inLen) / 2;
                    sPaddedLeft = padLeftRight(sPrintLeft, sPadSpace, inSpaceLeft, _ALIGN_LEFT);
                    sPaddedRight = padLeftRight(sPrintLeft, sPadSpace, inSpaceLeft, _ALIGN_RIGHT);
                    sPrintText = sPaddedLeft + sPrintLeft + sPaddedRight;
                    break;
                case _ALIGN_LEFT: // Left
                    inSpaceLeft = (inCharPerLine - inLen);
                    sPaddedLeft = padLeftRight(sPrintLeft, sPadSpace, inSpaceLeft, _ALIGN_LEFT);
                    sPrintText = sPaddedLeft + sPrintLeft;
                    break;
                case _ALIGN_RIGHT: // Right                    
                    inSpaceRight = (inCharPerLine - inLen);
                    sPaddedRight = padLeftRight(sPrintLeft, sPadSpace, inSpaceRight, _ALIGN_RIGHT);
                    sPrintText = sPrintLeft + sPaddedRight;
                    break;
                case _ALIGN_LEFT_RIGHT: // LeftRight                    
                    string sTemp = "";
                    int inLeft = sPrintLeft.Length;
                    int inRight = sPrintRight.Length;
                    int inLenPad = 0;

                    inLenPad = (inCharPerLine - inLeft - inRight);
                    sTemp = padLeftRight("", sPadSpace, inLenPad, _ALIGN_LEFT);
                    sPrintText = sPrintLeft + sTemp + sPrintRight;

                    break;
            }

            sToPrint = sPrintText;

            return sPrintText;
        }

        public string Separator(bool fDoubleLiner)
        {
            string sSepararator = "";

            if (fDoubleLiner == true)
            {
                sSepararator = padLeftRight("", "=", inCharPerLine, _ALIGN_LEFT);
            }
            else
            {
                sSepararator = padLeftRight("", "-", inCharPerLine, _ALIGN_LEFT);

            }
            return (sSepararator);
        }

        //Encrypt
        public string EncryptString(string plainText, string passPhrase)
        {
            Debug.WriteLine("--EncryptString--");
            Debug.WriteLine("plainText=" + plainText);
            Debug.WriteLine("passPhrase=" + passPhrase);

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();

            string pResult = Convert.ToBase64String(cipherTextBytes);

            Debug.WriteLine("EncryptString, pResult=" + pResult);

            return pResult;
        }

        //Decrypt
        public string DecryptString(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        //Open file in to a filestream and read data in a byte array.
        public byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes 
            //to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);

            return data;
        }

        public bool isMapExist(string MapFrom)
        {
            int i = 0;
            string sMapFrom = "";
            bool fFound = false;

            for (i = 0; i < clsArray.MapID.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i].ToString();

                if (sMapFrom.CompareTo(MapFrom) == 0)
                {
                    fFound = true;
                    break;
                }
            }

            return fFound;
        }

        public string GetValidMap()
        {
            int i = 0;
            string sMapFrom = "";
            string sTemp = "";

            for (i = 0; i < clsArray.MapID.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i].ToString();
                sTemp += sMapFrom + "\n";
            }

            return sTemp;
        }

        public string GetMapTo(string MapFrom)
        {
            int i = 0;
            string sMapFrom = "";
            string sMapTo = "";

            for (i = 0; i < clsArray.MapID.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i].ToString();

                if (sMapFrom.CompareTo(MapFrom) == 0)
                {
                    sMapTo = clsArray.MapTo[i].ToString();
                    break;
                }
            }

            if (sMapTo.Length > 0)
                return sMapTo;
            else
                return "UNDEFINED";
        }
        public int GetMapColumnIndex(string MapFrom)
        {
            int i = 0;
            string sMapFrom = "";
            int iMapColumnIndex = 0;

            //Debug.WriteLine("--GetMapColumnIndex--");
            //Debug.WriteLine("MapFrom="+ MapFrom);
            //Debug.WriteLine("clsArray.MapID.Length="+ clsArray.MapID.Length);

            for (i = 0; i < clsArray.MapID.Length; i++)
            {
                sMapFrom = clsArray.MapFrom[i].ToString();

                if (sMapFrom.CompareTo(MapFrom) == 0)
                {
                    iMapColumnIndex = int.Parse(clsArray.MapColumnIndex[i].ToString());
                    break;
                }
            }

            //Debug.WriteLine("GetMapColumnIndex, MapFrom=" + MapFrom + ",iMapColumnIndex=" + iMapColumnIndex);

            return iMapColumnIndex;
        }

        public string RemoveUnwantedChar(string input)
        {

            StringBuilder builder = new StringBuilder(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]) || allowedCharacters.Contains(input[i]))
                {
                    builder.Append(input[i]);
                }
            }
            return builder.ToString();
        }

        public void GetPublishVersion(Label obj)
        {
            string sVersion = "v";
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                obj.Text = sVersion + ad.CurrentVersion.ToString();
                clsSystemSetting.ClassSystemLocalPublishVersion = obj.Text;
            }
        }

        public string GetURLDetail()
        {
            string sTemp = "";

            sTemp = "*API DETAILS*" + "\n" +
                    "URL: " + clsGlobalVariables.strAPIURL + "\n" +
                    "PATH: " + clsGlobalVariables.strAPIPath + "\n" +
                    //"KEY: " + clsGlobalVariables.strAPIKeys + "\n" +
                    "CONTENT TYPE: " + clsGlobalVariables.strAPIContentType + "\n" +
                    "BANK: " + clsGlobalVariables.strAPIBank;
            //"AUTH USERNAME: " + clsGlobalVariables.strAPIAuthUser + "\n" +
            //"AUTH PASSWORD: " + clsGlobalVariables.strAPIAuthPassword;

            return sTemp;
        }

        public string GetComputerDetail()
        {
            string sTemp = "";

            sTemp = "*COMPUTER DETAILS*" + "\n" +
                    "NAME: " + clsGlobalVariables.strComputerName + "\n" +
                    "LOCAL IP: " + clsGlobalVariables.strLocalIP;

            return sTemp;
        }

        public enum ImportType
        {
            iCity, iProvince, iMerchant, iIRDetail, iIRImportDetail, iSerialNo, iFSR, iDummyFSR, iSIM, iERM, iDTR, iImportTerminal, iImportSIM, iImportStock, iRegion, iRegionProvince
        }
        public string GetImportFileName(ImportType iType, int iIndex)
        {
            DateTime WriteDateTime = DateTime.Now;
            string sWriteDateTime = "";
            string sTemp = "";
            string sFileName = "";

            sWriteDateTime = WriteDateTime.ToString("yyyyMMdd");

            switch (iType)
            {
                case ImportType.iCity:
                    sTemp = "city";
                    break;
                case ImportType.iProvince:
                    sTemp = "prov";
                    break;
                case ImportType.iMerchant:
                    sTemp = "merc";
                    break;
                case ImportType.iIRDetail:
                    sTemp = "irdt";
                    break;
                case ImportType.iIRImportDetail:
                    sTemp = "irmt";
                    break;
                case ImportType.iSerialNo:
                    sTemp = "sern";
                    break;
                case ImportType.iFSR:
                    sTemp = "ifsr";
                    break;
                case ImportType.iDummyFSR:
                    sTemp = "dfsr";
                    break;
                case ImportType.iSIM:
                    sTemp = "simc";
                    break;
                case ImportType.iERM:
                    sTemp = "ierm";
                    break;
                case ImportType.iDTR:
                    sTemp = "idtr";
                    break;
                case ImportType.iImportTerminal:
                    sTemp = "iterm";
                    break;
                case ImportType.iImportSIM:
                    sTemp = "isim";
                    break;
                case ImportType.iImportStock:
                    sTemp = "istock";
                    break;
                case ImportType.iRegion:
                    sTemp = "region";
                    break;
                case ImportType.iRegionProvince:
                    sTemp = "regionprov";
                    break;

            }

            if (iIndex > 0)
            {
                sFileName = "im_" + clsSearch.ClassBankCode + "_" + sTemp + "_" + sWriteDateTime + "_" + padLeftChar(clsUser.ClassUserID.ToString(), sPadZero, 6) + "_" + padLeftChar(iIndex.ToString(), sPadZero, 3) + ".csv";
            }
            else
            {
                sFileName = "im_" + clsSearch.ClassBankCode + "_" + sTemp + "_" + sWriteDateTime + "_" + padLeftChar(clsUser.ClassUserID.ToString(), sPadZero, 6) + ".csv";
            }


            return sFileName;
        }
        public enum ExportType
        {
            iMerchant, iIRDetail, iIRImportDetail, iSerialNo, iFSR, iSIM, iTA
        }
        public string GetExportFileName(ExportType iType)
        {
            DateTime WriteDateTime = DateTime.Now;
            string sWriteDateTime = "";
            string sTemp = "";
            string sFileName = "";

            sWriteDateTime = WriteDateTime.ToString("yyyyMMdd");

            switch (iType)
            {
                case ExportType.iMerchant:
                    sTemp = "merc";
                    break;
                case ExportType.iIRDetail:
                    sTemp = "irdt";
                    break;
                case ExportType.iIRImportDetail:
                    sTemp = "IR Report";
                    break;
                case ExportType.iSerialNo:
                    sTemp = "SerialNo Report";
                    break;
                case ExportType.iFSR:
                    sTemp = "FSR Report";
                    break;
                case ExportType.iSIM:
                    sTemp = "SIM Report";
                    break;
                case ExportType.iTA:
                    sTemp = "TA Report";
                    break;

            }

            sFileName = sTemp + "_" + sWriteDateTime + ".xls";

            return sFileName;
        }
        public string GetRequestTime(string sFunction)
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDateTime = "";

            sProcessDateTime = ProcessDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            Debug.WriteLine("Request Time:" + sProcessDateTime + " > " + sFunction);

            clsSearch.ClassRequestTime = sProcessDateTime;

            return sProcessDateTime;
        }

        public string GetResponseTime(string sFunction)
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDateTime = "";

            sProcessDateTime = ProcessDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            Debug.WriteLine("Response Time:" + sProcessDateTime + " > " + sFunction);

            clsSearch.ClassResponseTime = sProcessDateTime;

            //if (clsSearch.ClassRequestTime.Length > 0 && clsSearch.ClassResponseTime.Length > 0)
            //   ComputeResReqTime(sFunction);

            return sProcessDateTime;
        }

        public void ComputeResReqTime(string sFunction)
        {
            DateTime dteRequest = DateTime.Parse(clsSearch.ClassRequestTime);
            DateTime dteResponse = DateTime.Parse(clsSearch.ClassResponseTime);

            TimeSpan span = (dteResponse - dteRequest);

            string sTimeInterval = span.Hours + "h" + ":" +
                        span.Minutes.ToString() + "m" + ":" +
                        span.Seconds.ToString() + "s" + ":" +
                        span.Milliseconds.ToString() + "ms";

            Debug.WriteLine("TIme Interval(Request/Response) " + "[" + sFunction + "]" + ":" + sTimeInterval);
        }

        public string SetNumericValue(string sData)
        {
            string sValue = "";

            if (sValue.Length > 0)
                sValue = (sData.Length > 0 ? sData : sPadZero);
            else
                sValue = sPadZero;

            return sValue;
        }

        public void InitProgressBar(ProgressBar obj, int iMax)
        {
            iProgressBarMax = iMax;
            obj.Minimum = 0;
            obj.Maximum = iMax;
            obj.Value = 0;
        }

        public void UpdateProgressBar(ProgressBar obj, Label lObj, int iValue)
        {
            //Debug.WriteLine("iProgressBarMax=" + iProgressBarMax.ToString() + "- " + "iValue=" + iValue.ToString());

            if (iProgressBarMax >= iValue)
            {
                obj.Increment(iValue);
                //Thread.Sleep(1);
            }

        }
        public void UpdateProgressStatus(RichTextBox obj, int iStateType, string sMessage, int iLineNo, int iMaxLine)
        {
            string sTemp = "";

            switch (iStateType)
            {
                case 0:
                    sTemp = sMessage + "[ " + iLineNo.ToString() + " / " + iMaxLine.ToString() + " ]";
                    break;
                case 1:
                    sTemp = sMessage;
                    break;
                case 2:
                    sTemp = "[ " + iLineNo.ToString() + " / " + iMaxLine.ToString() + " ]";
                    break;
                case 3:
                    sTemp = sMessage;
                    break;
                default:
                    break;
            }

            obj.SelectionStart = obj.Text.Length;
            obj.AppendText(sTemp + Environment.NewLine);
            obj.ScrollToCaret();

            //Thread.Sleep(10);
        }

        public void UpdateAnimate(Bunifu.Framework.UI.BunifuCircleProgressbar obj, int iDir)
        {
            if (obj.Value == 80)
            {
                iDir = -1;
                obj.animationIterval = 4;
            }
            else if (obj.Value == 20)
            {
                iDir += 1;
                obj.animationIterval = 1;
            }
            else
            {
                obj.Value = iDir;
            }
        }

        public void UpdateAnimateHeader(RichTextBox objLabel1, string sHeader1, RichTextBox objLabel2, string sHeader2)
        {
            // Header 1 
            objLabel1.Text = "";
            objLabel1.SelectionStart = objLabel1.Text.Length;
            objLabel1.AppendText(sHeader1);
            objLabel1.ScrollToCaret();

            // Header 1         
            objLabel2.Text = "";
            objLabel2.SelectionStart = objLabel2.Text.Length;
            objLabel2.AppendText(sHeader2);
            objLabel2.ScrollToCaret();
        }

        public void SetMessageBox(string sMessage, string sTitle, IconType iIcon)
        {
            if (Application.OpenForms.Count > 0)
            {
                Form mainForm = Application.OpenForms[0];

                if (mainForm.InvokeRequired)
                {
                    mainForm.Invoke(new Action(() => ShowMessage(sMessage, sTitle, iIcon)));
                }
                else
                {
                    ShowMessage(sMessage, sTitle, iIcon);
                }
            }
            else
            {
                // fallback if no form (rare case)
                ShowMessage(sMessage, sTitle, iIcon);
            }
        }

        private void ShowMessage(string sMessage, string sTitle, IconType iIcon)
        {
            MessageBoxIcon icon = MessageBoxIcon.None;

            switch (iIcon)
            {
                case IconType.iAsterisk:
                    icon = MessageBoxIcon.Asterisk; break;
                case IconType.iError:
                    icon = MessageBoxIcon.Error; break;
                case IconType.iExclamation:
                    icon = MessageBoxIcon.Exclamation; break;
                case IconType.iHand:
                    icon = MessageBoxIcon.Hand; break;
                case IconType.iInformation:
                    icon = MessageBoxIcon.Information; break;
                case IconType.iQuestion:
                    icon = MessageBoxIcon.Question; break;
                case IconType.iWarning:
                    icon = MessageBoxIcon.Warning; break;
                case IconType.iNone:
                default:
                    icon = MessageBoxIcon.None; break;
            }

            MessageBox.Show(
                sMessage,
                sTitle,
                MessageBoxButtons.OK,
                icon,
                MessageBoxDefaultButton.Button1
            );
        }

        public enum IconType
        {
            iAsterisk, iError, iExclamation, iHand, iInformation, iNone, iQuestion, iStop, iWarning,
            iAdd_On, iAdd_Off,
            iFind_On, iFind_On2, iFind_Off,
            iFolder_On, iFolder_Off,
            iRemove_On, iRemove_Off,
            iSearch_On, iSearch_Off
        }

        public int SetValueToZero(string sValue)
        {
            int iValue = 0;
            bool isNumeric = false;

            isNumeric = int.TryParse(sValue, out iValue);

            if (!isNumeric)
                iValue = 0;
            else
                iValue = int.Parse(sValue);

            return iValue;
        }

        public string SetZeroIfNull(string sValue)
        {
            string sTemp = "";

            if (sValue.Length > 0)
            {
                sTemp = sValue;
            }
            else
            {
                sTemp = sPadZero;
            }

            return sTemp;
        }

        public int GetImportCSVRowIndex(DataGridView obj, ImportType iType)
        {
            int iColCount = 3;
            int iRowCount = obj.RowCount;
            string sHeader = "";
            int iRowIndex = 0;

            for (int i = 0; i < iRowCount; i++)
            {
                sHeader = "";
                for (int x = 0; x < iColCount; x++)
                {
                    string sCellValue = obj.Rows[i].Cells[x].Value.ToString();
                    sCellValue = sCellValue.Replace("\n", "");
                    sHeader = sHeader + (sCellValue.Length > 0 ? sCellValue : clsFunction.sDash) + ",";
                    Debug.WriteLine("i=" + i.ToString() + "-" + "x=" + x.ToString() + "-" + "sHeader=" + sHeader);
                }

                if (iType == ImportType.iFSR)
                {
                    if (sHeader.ToUpper().CompareTo("NO,MERCHANT,MID,".ToUpper()) == 0)
                    {
                        iRowIndex = i;
                        break;
                    }
                }

                if (iType == ImportType.iERM)
                {
                    if (sHeader.ToUpper().CompareTo("NO,MERCHANT,MID,".ToUpper()) == 0)
                    {
                        iRowIndex = i;
                        break;
                    }
                }

                if (iType == ImportType.iIRDetail || iType == ImportType.iIRImportDetail)
                {
                    if (sHeader.CompareTo("NO,MCC INSTALLATION REQUEST ID,DATE REQUEST,") == 0)
                    {
                        iRowIndex = i;
                        break;
                    }
                }

                if (iType == ImportType.iSerialNo)
                {
                    if (sHeader.CompareTo("NO,SERIAL NO,TYPE,") == 0)
                    {
                        iRowIndex = i;
                        break;
                    }
                }
            }

            Debug.WriteLine("iRowIndex=" + iRowIndex.ToString());

            return iRowIndex;
        }

        public int GetImportCSVColumnCount(DataGridView obj, ImportType iType)
        {
            int iColCount = obj.ColumnCount;
            int iReturnColumnCount = 0;
            int iRowIndex = 0;
            string sHeader = "";

            iRowIndex = GetImportCSVRowIndex(obj, iType);

            for (int i = 0; i < iColCount; i++)
            {
                sHeader = "";
                string sCellValue = obj.Rows[iRowIndex].Cells[i].Value.ToString();
                sCellValue = sCellValue.Replace("\n", "");
                sHeader = sCellValue;
                Debug.WriteLine("i=" + i.ToString() + "-" + "iRowIndex=" + iRowIndex.ToString() + "-" + "sCellValue=" + sCellValue);

                if (iType == ImportType.iFSR)
                {
                    if (sHeader.ToUpper().CompareTo("Additional Information".ToUpper()) == 0)
                    {
                        iReturnColumnCount = i;
                        break;
                    }
                }

                if (iType == ImportType.iIRDetail || iType == ImportType.iIRImportDetail)
                {
                    if (sHeader.CompareTo("REPORT DATE(Date Forwarded the feedback to MCC)") == 0)
                    {
                        iReturnColumnCount = i;
                        break;
                    }
                }

                if (iType == ImportType.iSerialNo)
                {
                    if (sHeader.CompareTo("RECEIVED DATE") == 0)
                    {
                        iReturnColumnCount = i;
                        break;
                    }
                }
            }

            iReturnColumnCount = iReturnColumnCount + 1;

            return iReturnColumnCount;
        }

        public int GetImportCSVRowCount(DataGridView obj, ImportType iType)
        {
            int iRowCount = obj.RowCount;
            int iReturnRowCount = 0;
            int iRowIndex = 0;

            iRowIndex = GetImportCSVRowIndex(obj, iType);

            iReturnRowCount = iRowCount - iRowIndex;

            return iRowCount;
        }

        public void ListViewAlternateBackColor(ListView obj)
        {
            if (obj.Items.Count > 0)
            {
                for (int i = 0; i <= obj.Items.Count - 1; i++)
                {
                    if (i % 2 == 0)
                        obj.Items[i].BackColor = clsFunction.AlternateBackColor1;
                    else
                        obj.Items[i].BackColor = clsFunction.AlternateBackColor2;
                }
            }
        }

        public void FocusFirstItemInListView(ListView obj, int pIndex)
        {
            // Focus first item
            if (obj.Items.Count > 0)
            {
                obj.FocusedItem = obj.Items[pIndex];
                obj.Items[pIndex].Selected = true;
                obj.Select();
            }
        }

        public void UpdateListViewLineNo(ListView obj)
        {
            int LineNo = 1;

            foreach (ListViewItem i in obj.Items)
            {
                if (obj.Items.Count > 0)
                {
                    i.SubItems[0].Text = LineNo.ToString();
                    LineNo++;
                }
            }
        }

        public void ListViewRowFocus(ListView obj, int iRowIndex)
        {
            if (obj.Items.Count > 0)
            {
                if (iRowIndex >= obj.Items.Count)
                {
                    obj.Items[iRowIndex - 1].Selected = false;
                    iRowIndex = 0;
                }

                obj.FocusedItem = obj.Items[iRowIndex];

                if (iRowIndex > 0)
                {
                    obj.Items[iRowIndex - 1].Selected = false;
                    obj.Items[iRowIndex].Selected = true;
                }
                else
                {
                    obj.Items[iRowIndex].Selected = true;
                }

                obj.Select();
            }
        }
        public void DataGridViewAlternateBackColor(DataGridView obj)
        {
            foreach (DataGridViewRow row in obj.Rows)
            {
                if (row.Index % 2 == 0)
                    row.DefaultCellStyle.BackColor = clsFunction.AlternateBackColor1;
                else
                    row.DefaultCellStyle.BackColor = clsFunction.AlternateBackColor2;
            }
        }

        public void SetDataGridViewBackColor(DataGridView obj, int iRow, Color cColor)
        {
            foreach (DataGridViewRow row in obj.Rows)
            {
                if (row.Index == iRow)
                {
                    row.DefaultCellStyle.BackColor = cColor;
                }
            }
        }

        public string UpdateTime(string sTime)
        {
            IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            DateTime time_24;
            bool isNumeric = false;
            int iValue;

            isNumeric = int.TryParse(sTime, out iValue);

            if (sTime.Length > 0 && isNumeric)
            {
                time_24 = DateTime.ParseExact(sTime, "HHmmss", format);
                sTime = time_24.ToString("hh:mm:ss tt");
            }
            else
            {
                sTime = sDash;
            }

            return sTime;
        }
        public void ImportToDummyDataGrid(DataGridView obj, string sSheetName, string sPathFileName)
        {
            try
            {
                FileInfo file = new FileInfo(sPathFileName);
                if (!file.Exists)
                    throw new FileNotFoundException("File does not exist: " + sPathFileName);

                if (file.Extension.ToLower() != ".xlsx")
                    throw new NotSupportedException("Only .xlsx files are supported with EPPlus.");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set license context

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sSheetName];
                    if (worksheet == null)
                        throw new Exception($"Sheet '{sSheetName}' not found.");

                    DataTable dt = new DataTable();

                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;

                    // Add columns
                    for (int col = 1; col <= colCount; col++)
                    {
                        string colName = worksheet.Cells[1, col].Text;
                        if (string.IsNullOrWhiteSpace(colName))
                            colName = $"Column{col}";
                        dt.Columns.Add(colName);
                    }

                    // Add rows
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int col = 1; col <= colCount; col++)
                        {
                            string rawValue = worksheet.Cells[row, col].Text?.Trim() ?? "";

                            //dr[col - 1] = worksheet.Cells[row, col].Text;
                            dr[col - 1] = StrClean(rawValue);
                        }
                        dt.Rows.Add(dr);
                    }

                    obj.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception error " + ex.Message);
                SetMessageBox($"Excel Import Error:\n\n{ex.Message}", "Error", IconType.iError);
            }
        }


        public string GetDateFromParse(string sDateTime, string sDateFormat, string sOutDateFormat)
        {
            //IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            DateTime dDate;
            string sTemp = "";
            CultureInfo culture = new CultureInfo("en-US");
            bool isDateTime = false;

            if (sDateTime.Equals(sDash) || sDateTime.Equals(sDateFormat))
                sDateTime = clsDefines.DEV_DATE;

            if (sDateTime.Length > 0)
            {
                //    try
                //    {
                //        dDate = DateTime.ParseExact(sDateTime, sDateFormat, culture);
                //        sTemp = dDate.ToString(sOutDateFormat);
                //    }
                //    catch (FormatException)
                //    {
                //        sTemp = sDash;
                //    }

                //}
                //else
                //{
                //    sTemp = sDash;
                //}

                // ROCKY - FUNCTION ISSUE: FIX FOR FUNCTION NO DATE DISPLAY
                isDateTime = DateTime.TryParse(sDateTime, culture, DateTimeStyles.NoCurrentDateDefault, out dDate);
                sTemp = dDate.ToString(sOutDateFormat);

                if (!isDateTime)
                {
                    sTemp = sDash;
                }

            }

            return sTemp;
        }

        public int GetDataGridHeaderColumnIndex(DataGridView obj, string sHeader)
        {
            int iColIndex = 0;

            for (int i = 0; i < obj.ColumnCount; i++)
            {
                string cellParam = obj.Columns[i].Name;

                if (sHeader.CompareTo(cellParam) == 0)
                {
                    iColIndex = i;
                    break;
                }
            }

            return iColIndex;
        }

        public void GetMapMustAndFormat(string pHeader, ref bool outIsMust, ref string outFormat, ref int outColIndex)
        {
            outIsMust = false;
            outFormat = sNull;
            outColIndex = iZero;

            Debug.WriteLine("--GetMapMustAndFormat--");
            Debug.WriteLine("clsArray.MapID.Length=" + clsArray.MapID.Length);

            for (int i = 0; i < clsArray.MapID.Length; i++)
            {
                string sMapFrom = clsArray.MapFrom[i].ToString();

                if (sMapFrom.CompareTo(pHeader) == 0)
                {
                    outIsMust = (int.Parse(clsArray.isMust[i]) > 0 ? true : false);
                    outFormat = clsArray.Format[i];
                    outColIndex = int.Parse(clsArray.MapColumnIndex[i]);
                    break;
                }
            }

            Debug.WriteLine("pHeader=" + pHeader);
            Debug.WriteLine("outIsMust=" + outIsMust);
            Debug.WriteLine("outFormat=" + outFormat);
            Debug.WriteLine("outColIndex=" + outColIndex);

        }

        public string GetDataGridColumnValue(int iRowIndex, int iColIndex, DataGridView obj)
        {
            string sColumnValue = obj.Rows[iRowIndex].Cells[iColIndex].Value.ToString();

            return sColumnValue;
        }

        public void ClearTextBox(Control obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(obj.Controls);
        }

        public void ClearComboBox(Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        (control as ComboBox).Items.Clear();
                        (control as ComboBox).Text = sNull;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(obj.Controls);

        }
        public void TextBoxUnLock(bool isLock, Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                    {
                        //(control as TextBox).Enabled = isLock;
                        (control as TextBox).ReadOnly = !isLock;

                        if (isLock)
                            (control as TextBox).BackColor = clsFunction.EntryBackColor;
                        else
                            (control as TextBox).BackColor = clsFunction.DisableBackColor;
                    }
                    else
                        func(control.Controls);
            };

            func(obj.Controls);
        }

        public void InitTextBoxCharacterCasing(CharacterCasing casing, Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox tb)
                {
                    tb.CharacterCasing = casing;
                }

                // Recurse for nested containers (e.g., Panel, GroupBox)
                if (ctrl.HasChildren)
                {
                    InitTextBoxCharacterCasing(casing, ctrl);
                }
            }
        }


        public void ComBoBoxUnLock(bool isLock, Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        (control as ComboBox).Enabled = isLock;
                        (control as ComboBox).Text = sDefaultSelect;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(obj.Controls);
        }

        public void ComBoBoxDropDownStyle(ComboBoxStyle style, Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        (control as ComboBox).DropDownStyle = style;
                        (control as ComboBox).Text = sDefaultSelect;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(obj.Controls);
        }

        public void ComboBoxCaps(bool isCaps, Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is ComboBox comboBox)
                {
                    var updatedItems = new List<string>
            {
                sDefaultSelect // 1st item
            };

                    foreach (var item in comboBox.Items)
                    {
                        string text = item.ToString();
                        if (!string.IsNullOrWhiteSpace(text) && !text.Equals(sDefaultSelect, StringComparison.OrdinalIgnoreCase))
                        {
                            updatedItems.Add(isCaps ? text.ToUpper() : text);
                        }
                    }

                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(updatedItems.ToArray());

                    // Optional: set default selection to "[NOT SPECIFIED]"
                    comboBox.SelectedIndex = 0;
                }

                if (ctrl.HasChildren)
                {
                    ComboBoxCaps(isCaps, ctrl);
                }
            }
        }


        public void DatePickerUnlock(bool isLock, Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is DateTimePicker)
                    {
                        (control as DateTimePicker).Enabled = isLock;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(obj.Controls);
        }
        
        public void InitDateTimePicker(string format, Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is DateTimePicker dtp)
                {
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = format;
                }

                // Recursive check for containers (like Panels, GroupBoxes, etc.)
                if (ctrl.HasChildren)
                {
                    InitDateTimePicker(format, ctrl);
                }

            }
        }


        public void LabelForecolor(Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is Label)
                        (control as Label).ForeColor = Color.Black;
                    else
                        func(control.Controls);
            };

            func(obj.Controls);
        }

        public void ClearDataGrid(DataGridView obj)
        {
            try
            {
                obj.DataSource = null;
                obj.Columns.Clear();
                obj.Rows.Clear();
                obj.Refresh();
            }
            catch (Exception ex)
            {
                SetMessageBox($"Exceptional error {ex.Message}", "ClearDataGrid", IconType.iError);
            }
            

            //if (obj.RowCount > 0)
            //{
            //    if (obj.DataSource != null)
            //    {
            //        obj.DataSource = null;
            //        obj.Rows.Clear();
            //    }
            //    else
            //    {
            //        obj.Rows.Clear();
            //    }
            //    obj.Refresh();

            //    obj.Rows.Clear();
            //    //obj.ColumnCount = 0;
            //    //obj.RowCount = 0;
            //}            
        }

        public void ClearListView(ListView obj)
        {
            obj.Clear();
            obj.Items.Clear();
        }

        public void ClearListViewItems(ListView obj)
        {
            obj.Items.Clear();
        }

        public void InitDataGridView(DataGridView obj)
        {
            obj.AlternatingRowsDefaultCellStyle.Font = new Font("Courier New", 8);
            obj.ColumnHeadersDefaultCellStyle.Font = new Font("Courier New", 8);
            obj.DefaultCellStyle.Font = new Font("Courier New", 8);
            obj.RowHeadersDefaultCellStyle.Font = new Font("Courier New", 8);
            obj.RowsDefaultCellStyle.Font = new Font("Courier New", 8);
        }

        public void ProcessReport(int ReportID)
        {
            Debug.WriteLine("--ProcessReport--");
            Debug.WriteLine("ReportID=" + ReportID);
            Debug.WriteLine("clsReport.ClassReportDesc=" + clsReport.ClassReportDesc);
            Debug.WriteLine("clsSearch.ClassIsExportToPDF=" + clsSearch.ClassIsExportToPDF);

            frmReportViewer.sReportToView = clsReport.ClassReportDesc;
            frmReportViewer.sReportID = clsSearch.ClassReportID;
            frmReportViewer frm = new frmReportViewer();

            // Do not show preview when auto export to pdf file
            if (clsSearch.ClassIsExportToPDF)
            {
                frm.Size = new Size(0, 0);
                frm.WindowState = FormWindowState.Normal;
            }

            frm.Show();
        }

        public string FormatDate(string sFromFormat, string sToFormat, string sDate)
        {
            IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            DateTime dDate;
            string sTemp = "";

            if (sDate.Length > 0)
            {
                try
                {
                    dDate = DateTime.ParseExact(sDate, sFromFormat, format);
                    sTemp = dDate.ToString(sToFormat);
                }
                catch (FormatException)
                {
                    sTemp = sDash;
                }

            }
            else
            {
                sTemp = sDash;
            }

            return sTemp;
        }

        public void SetDateCustomFormat(DateTimePicker obj)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = "MM-dd-yyyy H:mm:ss tt";
        }
        public void SetTimeCustomFormat(DateTimePicker obj)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = "h:mm:ss tt";
            obj.ShowUpDown = true;
        }

        public void SetDateFormatWithWeekDay(DateTimePicker obj)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = "ddd, MM-dd-yyyy";
        }

        public void SetDateFormat(DateTimePicker obj, string pFormat)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = pFormat;
        }

        public void SetTimeFormat(DateTimePicker obj, string pFormat)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = pFormat;
            obj.ShowUpDown = true;
        }

        public void PreviewTA(string sRequestID, int iServiceNo, int iFSRNo, int iIRIDNo, string sReferenceNo)
        {
            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = "FSR REPORT";
            clsSearch.ClassIRNo = sRequestID;
            clsSearch.ClassServiceNo = iServiceNo;
            clsSearch.ClassFSRNo = iFSRNo;
            clsSearch.ClassIRIDNo = iIRIDNo;
            clsSearch.ClassServiceRequestID = sReferenceNo;

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassIRNo + clsFunction.sPipe + clsSearch.ClassServiceNo + clsFunction.sPipe + clsSearch.ClassFSRNo + clsFunction.sPipe + clsSearch.ClassIRIDNo;

            Debug.WriteLine("PreviewTA::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            try
            {
                clsSearch.ClassReportID = 5;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "FSR";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceTA";
                ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public bool isValidSystemVersion(string sLocalPublishVersion, string sNewPublishVersion)
        {
            bool fValid = false;

            if (sLocalPublishVersion.CompareTo(sNewPublishVersion) == 0)
                fValid = true;


            if (!fValid)
            {
                MessageBox.Show("Application version is not updated. Check details below:" + "\n\n" +
                                "Current Version Detail: " + "\n" +
                                "  Publish Version: " + sLocalPublishVersion + "\n" +
                                "\n\n" +
                                "New Version Detail: " + "\n" +
                                "  Publish Version: " + sNewPublishVersion + "\n\n" +
                                "Application will be close and execute MIS UPDATE to download latest application version."
                                , "Version Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return fValid;
        }

        public bool fSavingConfirm(bool fUpdate)
        {
            bool fConfirm = true;

            if (MessageBox.Show("Are you sure you want to " + (fUpdate == true ? "update" : "save") + " " + "record?" +
                                    "\n\n",
                                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        public bool CheckTimeFromTo(DateTimePicker objFrom, DateTimePicker objTo, string sTitle, bool isStartEndTime)
        {
            bool fValid = true;
            int iResult;

            string sTimeFrom = GetDateFromParse(objFrom.Text, "h:mm tt", "HH:mm:ss");
            string sTimeTo = GetDateFromParse(objTo.Text, "h:mm tt", "HH:mm:ss");

            iResult = DateTime.Compare(DateTime.Parse(sTimeTo), DateTime.Parse(sTimeFrom));

            if (iResult >= 0)
                fValid = true;
            else if (iResult < 0)
                fValid = false;
            else
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("Incorrect time interval.\n\n[Time From] must not greater than or equal [Time To]" +
                                       "\n\n" +
                                       (isStartEndTime ? "Time End: " : "Receipt Time: ") + sTimeFrom +
                                       "\n" +
                                       (isStartEndTime ? "Time Start: " : "Time Arrived: ") + sTimeTo, "Time Check" + "[" + sTitle + "]", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        public string FormatNumber(int iValue)
        {
            string sFormat = "";
            string sTemp = "";

            sTemp = iValue.ToString("#,#", CultureInfo.InvariantCulture);

            if (sTemp.Length > 0)
                sFormat = sTemp;
            else
                sFormat = clsFunction.sDash;

            return sFormat;
        }

        public string GenerateControlNo(int ControlID, string sPrefix, bool isDate)
        {

            string sTemp = "";
            DateTime dDateTime = DateTime.Now;
            string sDateTime = "";

            if (isDate)
                sDateTime = dDateTime.ToString("yyMMdd");

            sTemp = sPrefix + sDateTime + sDash + padLeftChar(ControlID.ToString(), sZero, 6);

            return sTemp;

        }
        public bool isNumeric(string sValue)
        {
            int iValue = 0;
            bool fNumber = false;

            fNumber = int.TryParse(sValue, out iValue);

            //Debug.WriteLine("isNumeric|sValue="+ sValue+ "|fNumber="+ fNumber);

            return fNumber;
        }

        public string CheckAndSetStringValue(string sValue)
        {
            string sTemp = "";

            if (sValue != null)
            {
                sTemp = sValue;
            }
            else
            {
                sTemp = sDash;
            }

            sTemp = sTemp.Replace("\n", "");

            return sTemp;
        }
        public string CheckAndSetNumericValue(string sValue)
        {
            string sTemp = "";

            if (sValue.Length > 0)
                sTemp = sValue;
            else
                sTemp = sZero;

            return sTemp;
        }

        public int CheckAndSetBooleanValue(bool isBool)
        {
            int iValue = 0;

            if (isBool)
                iValue = 1;
            else
                iValue = 0;

            return iValue;
        }

        public string AddSingleQuote(string sValue)
        {
            string sTemp = "";

            if (sValue.Length > 0)
                sTemp = sValue;
            else
                sTemp = sDash;

            sTemp = "'" + sTemp + "'";

            return sTemp;
        }
        
        public string CheckAndSetDatePickerValueToDate(DateTimePicker obj)
        {
            string sTemp = sDateFormat;
            DateTime stDate = obj.Value;

            sTemp = stDate.ToString("yyyy-MM-dd");

            return sTemp;
        }

        public string CheckAndSetDatePickerValueToTime(DateTimePicker obj)
        {
            string sTemp = sTimeFormat;
            DateTime stDate = obj.Value;

            sTemp = stDate.ToString("HH:mm:ss");

            return sTemp;
        }


        public int CheckAndSetYesNoValue(string pYesNo)
        {
            int iValue = 0;

            if (pYesNo.Equals(sNo))
                iValue = 0;
            else if (pYesNo.Equals(sYes))
                iValue = 1;
            else
                iValue = 0;

            return iValue;
        }

        public string CheckDefaultSelectValue(string sValue)
        {
            string sTemp = "";

            if (sValue != null)
            {
                if (sValue.Equals(sDefaultSelect))
                    sTemp = sDefaultSelect;
                else
                    sTemp = sValue;
            }
            else
            {
                sTemp = sDash;
            }

            return sTemp;
        }

        public bool isValidComboBoxValue(string sValue)
        {
            bool fValid = false;

            if (sValue.Length > 0)
            {
                if (sValue.CompareTo(sDefaultSelect) != 0)
                    fValid = true;
            }

            return fValid;
        }
        public static void WaitWindow(bool fShow, Form frmWait)
        {
            if (fShow)
            {
                Cursor.Current = Cursors.WaitCursor; // Waiting / Hour Glass
                frmWait.Show();
            }
            else
            {
                Cursor.Current = Cursors.Default; // Normal
                frmWait.Close();
            }

        }
        public string[] GetSkinColor()
        {
            string[] ret = { clsFunction.sDefaultSelect, "BLACK", "DIMGRAY", "DARKGRAY", "SADDLEBROWN", "MAROON", "DARKRED", "DARKOLIVEGREEN", "FORESTGREEN", "DARKCYAN", "DARKSLATEGRAY", "STEELBLUE", "DODGERBLUE", "ROYALBLUE", "NAVY", "DARKBLUE", "INDIGO", "DARKMAGENTA", "CRIMSON" };

            return ret;
        }
        public void FillComboBoxSkinColor(ComboBox obj)
        {
            int i = 0;

            obj.Items.Clear();
            while (GetSkinColor().Length > i)
            {
                obj.Items.Add(GetSkinColor()[i]);

                i++;
            }

            if (i > 0)
                obj.SelectedIndex = 0;

        }

        public string ParseString(string sData, int iIndex)
        {
            // Split IP And Port
            string[] sTemp = sData.Split('|');
            int iCount = sData.Length;
            string sParse = "";

            for (int x = 0; x < iCount; x++)
            {
                if (x == iIndex)
                {
                    sParse = sTemp[x].ToString();
                    break;
                }
            }

            return sParse;
        }

        public bool isValidID(string sValue)
        {
            bool fValid = true;

            if (sValue.Length <= 0)
                fValid = false;

            if (sValue.CompareTo(sZero) == 0 || sValue.CompareTo(sNull) == 0 || sValue.CompareTo(sDash) == 0)
                fValid = false;

            return fValid;

        }

        public bool isValidDescription(string sValue)
        {
            bool fValid = true;

            try
            {
                if (sValue.Length <= 0 || sValue is null)
                    fValid = false;

                if (sValue.CompareTo(sDash) == 0 || sValue.CompareTo(sDefaultSelect) == 0 || sValue.CompareTo(sZero) == 0)
                    fValid = false;

            }
            catch (Exception ex)
            {
                fValid = false;
                Debug.WriteLine("Error " + ex.Message);
            }

            return fValid;
        }

        public bool isValidCount(int iValue)
        {
            bool fValid = true;

            if (iValue <= 0)
                fValid = false;

            return fValid;
        }

        public bool isValidLength(string sValue, int length)
        {
            bool fValid = true;

            if (sValue.Length <= 0)
                fValid = false;

            if (sValue.Length != length)
                fValid = false;

            return fValid;

        }

        public bool isValidEntry(CheckType iCheckType, string sValue)
        {
            bool fValid = false;
            string sTemp = "";

            switch (iCheckType)
            {
                case CheckType.iParticularID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Name must not be blank.";
                    break;
                case CheckType.iClientID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Client must not be blank.";
                    break;
                case CheckType.iClientName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Client must not be blank.";
                    break;
                case CheckType.iMerchantID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant must not be blank.";
                    break;
                case CheckType.iMerchantName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant name must not be blank.";
                    break;
                case CheckType.iMerchantAddress:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant address must not be blank.";
                    break;
                case CheckType.iFEID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Vendor field engineer must not be blank.";
                    break;
                case CheckType.iFEName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Vendor field engineer must not be blank.";
                    break;
                case CheckType.iSPID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service provider must not be blank.";
                    break;
                case CheckType.iSPName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service provider must not be blank.";
                    break;
                case CheckType.iSupplierID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Supplier must not be blank.";
                    break;
                case CheckType.iSupplierName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Supplier must not be blank.";
                    break;
                case CheckType.iIRIDNo:
                    break;
                case CheckType.iIRNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Request ID must not be blank.";
                    break;
                case CheckType.iServiceNo:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Request ID must not be blank.";
                    break;
                case CheckType.iRequestNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service Request ID must not be blank.";
                    break;
                case CheckType.iReferenceNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Reference No. must not be blank.";
                    break;
                case CheckType.iTAIDNo:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "TAIDNo must not be blank.";
                    break;
                case CheckType.iComments:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Comments must not be blank.";
                    break;
                case CheckType.iRemarks:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Remarks must not be blank.";
                    break;
                case CheckType.iCustomerName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Customer Name must not be blank.";
                    break;
                case CheckType.iRequestBy:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Requested By must not be blank.";
                    break;
                case CheckType.iContactNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Contact Number must not be blank.";
                    break;
                case CheckType.iTerminalID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Serial number must not be blank.";

                    break;
                case CheckType.iTerminalSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Serial number must not be blank.";
                    break;
                case CheckType.iTerminalStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Status must not be blank.";
                    break;
                case CheckType.iSIMID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "SIM Serial Number must not be blank.";
                    break;
                case CheckType.iSIMSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "SIM Serial Number must not be blank.";
                    break;
                case CheckType.iSIMStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "SIM status must be available.";
                    break;
                case CheckType.iDockID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Dock Serial Number must not be blank.";
                    break;
                case CheckType.iDockSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Dock Serial Number must not be blank.";
                    break;
                case CheckType.iDockStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Dock Serial Number must be available.";
                    break;
                case CheckType.iTime:
                    if (sValue.CompareTo(sInvalidTime) != 0)
                    {
                        fValid = true;
                    }
                    sTemp = "Time must not be value of " + sInvalidTime;
                    break;
                case CheckType.iTID:
                    if (isValidDescription(sValue) && sValue.Length >= TID_LENGTH)
                    {
                        fValid = true;
                    }
                    sTemp = "TID must not be blank and length must be " + TID_LENGTH;
                    break;
                case CheckType.iMID:
                    if (isValidDescription(sValue) && sValue.Length >= MID_LENGTH)
                    {
                        fValid = true;
                    }
                    sTemp = "MID must not be blank and length must be " + MID_LENGTH;
                    break;
                case CheckType.iServiceType:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service Type must not be blank.";
                    break;
                case CheckType.iRegion:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Region must not be blank.";
                    break;
                case CheckType.iMerchantRepresentative:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant representative name must not be blank.";
                    break;
                case CheckType.iMerchantContactNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant representative contact no. must not be blank.";
                    break;
                case CheckType.iMerchantEmail:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant representative email must not be blank.";
                    break;
                case CheckType.iMerchantRepPosition:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Merchant representative position must not be blank.";
                    break;
                case CheckType.iReason:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Reason must not be blank.";
                    break;
                case CheckType.iLeaveType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Leave type must not be blank.";
                    break;
                case CheckType.iDepartment:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Department must not be blank.";
                    break;
                case CheckType.iPosition:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Position must not be blank.";
                    break;
                case CheckType.iEmploymentStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Employment status must not be blank.";
                    break;
                case CheckType.iDateType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Date type must not be blank.";
                    break;
                case CheckType.iEMPID:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Employee must not be blank.";
                    break;
                case CheckType.iCreditLimit:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Credit limit must not be blank.";
                    break;
                case CheckType.iActionMade:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service result must not be blank.";
                    break;
                case CheckType.iActionTaken:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Action made must not be blank.";
                    break;
                case CheckType.iCurTerminalStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for current Terminal.";
                    break;
                case CheckType.iCurSIMStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for current SIM.";
                    break;
                case CheckType.iCurDockStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for current Dock.";
                    break;
                case CheckType.iRepTerminalStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for replaced Terminal.";
                    break;
                case CheckType.iRepSIMStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for replaced SIM.";
                    break;
                case CheckType.iRepDockStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's status for replaced Dock.";
                    break;

                case CheckType.iAppVersion:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Application version must not be blank.";
                    break;
                case CheckType.iAppCRC:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Application CRC must not be blank.";
                    break;
                case CheckType.iCountry:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Country must not be blank.";
                    break;
                case CheckType.iMDRType:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "MDR type must not be blank.";
                    break;
                case CheckType.iLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Location must not be blank.";
                    break;
                case CheckType.iAllocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Allocation must not be blank.";
                    break;
                case CheckType.iAssetType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Asset type must not be blank.";
                    break;
                case CheckType.iCarrier:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Carrier must not be blank.";
                    break;
                case CheckType.iPrefix:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Prefix must not be blank.";
                    break;
                case CheckType.iPadLen:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Pad length must not be blank.";
                    break;
                case CheckType.iStartIndex:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Start index must not be blank.";
                    break;
                case CheckType.iEndIndex:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "End index must not be blank.";
                    break;
                case CheckType.iTerminalType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Type must not be blank.";
                    break;
                case CheckType.iTerminalModel:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Model must not be blank.";
                    break;
                case CheckType.iTerminalBrand:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Brand must not be blank.";
                    break;
                case CheckType.iCurTerminalID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Current terminal SN must not be blank.";
                    break;
                case CheckType.iCurTerminalSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Current terminal SN must not be blank.";
                    break;
                case CheckType.iCurSIMID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Current SIM SN must not be blank.";
                    break;
                case CheckType.iCurSIMSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Current SIM SN must not be blank.";
                    break;
                case CheckType.iRepTerminalID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Replace terminal SN must not be blank.";
                    break;
                case CheckType.iRepTerminalSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Replace terminal SN must not be blank.";
                    break;
                case CheckType.iRepSIMID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Replace SIM SN must not be blank.";
                    break;
                case CheckType.iRepSIMSN:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Replace SIM SN must not be blank.";
                    break;
                case CheckType.iName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Name must not be blank.";
                    break;
                case CheckType.iCity:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "City must not be blank.";
                    break;
                case CheckType.iProvince:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Province must not be blank.";
                    break;
                case CheckType.iMobileNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Mobile No. must not be blank.";
                    break;
                case CheckType.iEmail:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Email must not be blank.";
                    break;
                case CheckType.iSpecialInstruction:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Special instruction must not be blank.";
                    break;
                case CheckType.iFromLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Location [From] must not be blank.";
                    break;
                case CheckType.iToLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Location [To] must not be blank.";
                    break;
                case CheckType.iItemList:
                    sValue = (sValue.Length <= 0 ? sZero : sValue);
                    if (isValidCount(int.Parse(sValue)))
                    {
                        fValid = true;
                    }
                    sTemp = "Item(s) on list must not be blank.";
                    break;
                case CheckType.iBatchNo:
                    sValue = (sValue.Length <= 0 ? sZero : sValue);
                    if (isValidCount(int.Parse(sValue)))
                    {
                        fValid = true;
                    }
                    sTemp = "Batch No. must not be blank.";
                    break;
                case CheckType.iService:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Service must not be blank.";
                    break;
                case CheckType.iUserName:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Username must not be blank.";
                    break;
                case CheckType.iPassword:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Password must not be blank.";
                    break;
                case CheckType.iOldPassword:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Old password must not be blank.";
                    break;
                case CheckType.iNewPassword:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "New password must not be blank.";
                    break;
                case CheckType.iConfirmPassword:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Confirm password must not be blank.";
                    break;
                case CheckType.iInvoceNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Invoice No. must not be blank.";
                    break;
                case CheckType.iAcntNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Account No. must not be blank.";
                    break;
                case CheckType.iCustomerNo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Customer No. must not be blank.";
                    break;
                case CheckType.iRentalType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Rental Type must not be blank.";
                    break;
                case CheckType.iRentalTerms:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Rental Terms must not be blank.";
                    break;
                case CheckType.iFromInvoice:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Invoice No. [From] must not be blank.";
                    break;
                case CheckType.iToInvoice:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Invoice No. [To] must not be blank.";
                    break;
                case CheckType.iRequestIDFrom:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "[FROM] Request ID must not be blank.";
                    break;
                case CheckType.iRequestIDTo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "[TO] Request ID must not be blank.";
                    break;
                case CheckType.iCurTerminalLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's Location for current Terminal.";
                    break;
                case CheckType.iCurSIMLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's Location for current SIM.";
                    break;
                case CheckType.iRepTerminalLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's Location for replace Terminal.";
                    break;
                case CheckType.iRepSIMLocation:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select SN's Location for replace SIM.";
                    break;
                case CheckType.iProfileInfo:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Please select valid profile.";
                    break;
                case CheckType.iRequestID:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Request ID must not be blank.";
                    break;
                case CheckType.iTIDLength:
                    if (isValidLength(sValue, TID_LENGTH))
                    {
                        fValid = true;
                    }
                    sTemp = "Input value is " + AddBracketStartEnd(sValue) + AddBracketStartEnd(sValue.Length.ToString()) + "\n\n" + "TID length must be " + TID_LENGTH + ".";
                    break;
                case CheckType.iMIDLength:
                    if (isValidLength(sValue, MID_LENGTH))
                    {
                        fValid = true;
                    }
                    sTemp = "Input value is " + AddBracketStartEnd(sValue) + AddBracketStartEnd(sValue.Length.ToString()) + "\n\n" + "MID length must be " + MID_LENGTH + ".";
                    break;
                case CheckType.iRegionType:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Region must not be blank.";
                    break;
                case CheckType.iRegionID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Province must not be blank.";
                    break;
                case CheckType.iRequestIDLength:
                    if (isValidID(sValue) && sValue.Length <= clsSystemSetting.ClassSystemRequestIDMaxLimit)
                    {
                        fValid = true;
                    }
                    sTemp = "Input value is " + AddBracketStartEnd(sValue) + AddBracketStartEnd(sValue.Length.ToString()) + "\n\n" + "Request ID length must be greater than " + clsSystemSetting.ClassSystemRequestIDMaxLimit + ".";
                    break;
                case CheckType.iReferenceNoLength:
                    if (isValidID(sValue) && sValue.Length <= clsSystemSetting.ClassSystemRequestIDMaxLimit)
                    {
                        fValid = true;
                    }
                    sTemp = "Input value is " + AddBracketStartEnd(sValue) + AddBracketStartEnd(sValue.Length.ToString()) + "\n\n" + "Referene No. length must be greater than " + clsSystemSetting.ClassSystemRequestIDMaxLimit + ".";
                    break;
                case CheckType.iPOSType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "POS Type must not be blank.";
                    break;
                case CheckType.iDescription:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Description must not be blank.";
                    break;
                case CheckType.iType:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Type must not be blank.";
                    break;
                case CheckType.iStatus:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Status must not be blank.";
                    break;
                case CheckType.iDispatchID:
                    if (isValidID(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Vendor dispatcher must not be blank.";
                    break;
                case CheckType.iiDispatcher:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Vendor dispatcher must not be blank.";
                    break;
                case CheckType.iVendor:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Vendor must not be blank.";
                    break;
                case CheckType.iRequestor:
                    if (isValidDescription(sValue))
                    {
                        fValid = true;
                    }
                    sTemp = "Requestor must not be blank.";
                    break;


            }

            if (!fValid)
            {
                SetMessageBox(sTemp, "Warning", IconType.iExclamation);
            }

            return fValid;
        }

        public string FormatAmount(double dAmount)
        {
            string sAmount = "";

            sAmount = string.Format("{0:#.##}", dAmount);

            return sAmount;
        }

        public string FormatAmountWithComma(double dAmount)
        {
            string sAmount = "";

            sAmount = string.Format("#,}", dAmount);

            return sAmount;
        }

        public string FormatCountWithComma(int dCount)
        {
            string sAmount = "";

            sAmount = string.Format("{0:#,0}", dCount);

            return sAmount;
        }

        public bool isValidAmount(string sAmount)
        {
            bool isValid = false;

            if (sAmount.Length > 0)
            {
                if (sAmount.CompareTo(sDefaultAmount) == 0)
                    isValid = false;
                else
                    isValid = true;
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        public void GetCurrentDateTime()
        {
            DateTime CurrentDateTime = DateTime.Now;

            clsSearch.ClassCurrentDateTime = CurrentDateTime.ToString("yyyy-MM-dd H:mm:ss");
            clsSearch.ClassCurrentDate = CurrentDateTime.ToString("yyyy-MM-dd");
            clsSearch.ClassCurrentTime = CurrentDateTime.ToString("H:mm:ss");
        }

        public string getCurrentYear()
        {
            string pOutput = "0000";
            DateTime CurrentDateTime = DateTime.Now;

            pOutput = CurrentDateTime.ToString("yyyy");

            return pOutput;
        }

        public void ShowToolTip(Control obj, string sLineNo, string sTitle, string sMessage)
        {
            ToolTip tp = new ToolTip();
            tp.ToolTipTitle = sTitle;
            tp.ToolTipIcon = ToolTipIcon.Info;
            tp.UseFading = true;
            tp.UseAnimation = true;
            tp.ShowAlways = true;
            tp.IsBalloon = false;
            tp.AutoPopDelay = 10000;
            tp.InitialDelay = 500;
            tp.ReshowDelay = 500;
            tp.SetToolTip(obj, sMessage);
        }

        public string GetListViewSelectedRow(ListView obj, int iRow)
        {
            int iLineNo = 0;
            string sTemp = "";
            int iColCount = obj.Columns.Count;

            List<string> SearchKeyCol = new List<String>();
            List<string> SearchValueCol = new List<String>();

            for (int i = 0; i < iColCount; i++)
            {
                iLineNo++;
                string pSearchKey = obj.Columns[i].Text; // Param
                string pSearchValue = obj.SelectedItems[0].SubItems[i].Text; // Value

                sTemp = sTemp +
                       "[" + i.ToString() + "]" +
                       pSearchKey +
                       " > " + "{" + pSearchValue + "}" + Environment.NewLine;

                // Set Search Array
                SearchKeyCol.Add(pSearchKey);
                SearchValueCol.Add(pSearchValue);
            }

            clsArray.SearchKey = SearchKeyCol.ToArray();
            clsArray.SearchValue = SearchValueCol.ToArray();

            parseDelimitedString(sTemp, clsDefines.gNewLine, 0);

            return sTemp;
        }

        public void DisplayListViewSelectedRow(ListView objSource, ListView objDest, int iRow)
        {
            int iLineNo = 0;
            int iColCount = objSource.Columns.Count;

            for (int i = 0; i < iColCount; i++)
            {
                iLineNo++;
                string cellParam = objSource.Columns[i].Text; // Param
                string cellValue = objSource.SelectedItems[0].SubItems[i].Text; // Value

                if (cellParam.Substring(0, 1).CompareTo(clsFunction.sAsterisk) != 0)
                {
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(cellParam);
                    item.SubItems.Add(cellValue);
                    objDest.Items.Add(item);
                }

            }

        }

        public string[] GetInvalidChar()
        {
            string[] ret = { "\'", "\"", "\\", "`", "~", "%" };

            return ret;
        }
        public bool isValidateEntry(string sValue)
        {
            int iCharLength = GetInvalidChar().Length;
            int i = 0;
            bool fValid = true;

            while (i < iCharLength)
            {
                string sInvalid = GetInvalidChar()[i];

                if (sValue.CompareTo(sInvalid) == 0)
                {
                    fValid = false;
                }

                if (!fValid) break;

                i++;
            }

            return fValid;
        }

        public void AppDoEvents(bool fEvents)
        {
            fEvents = false;

            if (fEvents)
                Application.DoEvents();
        }

        public int ID_Width()
        {
            int iWidth = 0;

            iWidth = (clsSystemSetting.ClassSystemDeveloperMode > 0 ? 90 : 0);

            return iWidth;
        }

        public int GetPageLimit()
        {
            int iLimit = 0;

            iLimit = (clsSystemSetting.ClassSystemPageLimit);

            return iLimit;
        }

        public bool WriteToRegistry(string sKeyName, string sKeyValue)
        {
            //sKeyValue = dbFunction.EncryptString(sKeyValue, clsFunction.initVector);

            Registry.SetValue(keyName, sKeyName, sKeyValue);

            return (true);
        }

        public bool ReadFromRegistry(string KeyName, ref string sKeyValue)
        {
            string sValue;
            bool fValid = false;
            sKeyValue = "0";

            try
            {
                sValue = (string)Registry.GetValue(keyName, KeyName, "0"); // convert to string

                if (sValue.CompareTo("0") != 0)
                {
                    fValid = true;
                    sKeyValue = sValue;
                    //sKeyValue = dbFunction.DecryptString(sValue, clsFunction.initVector);                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error " + ex.Message);
                sValue = "0";
                sKeyValue = sValue;
            }

            return fValid;
        }

        public bool IsDateTime(string sDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(sDate, out tempDate);
        }

        // *************************************************************************************************************************************
        // Encryption
        // *************************************************************************************************************************************
        public string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(MasterKey);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(MasterKeySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        // *************************************************************************************************************************************
        // Decryption
        // *************************************************************************************************************************************        
        public string Decrypt(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(MasterKey);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(MasterKeySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public void ListViewChangeBackColorByRow(ListView obj, int pRowIndex, Color forecolor)
        {
            if (obj.Items.Count > 0)
            {
                for (int i = 0; i <= obj.Items.Count - 1; i++)
                {
                    if (i == pRowIndex)
                    {
                        obj.Items[i].BackColor = forecolor;
                        break;
                    }
                }
            }
        }

        public bool isHeaderExist(DataGridView gridView, string pHeader)
        {
            //Debug.WriteLine("--isHeaderExist--");

            bool isExist = false;
            string sHeader = "";

            for (int i = 0; i < gridView.ColumnCount; i++)
            {
                isExist = false;
                sHeader = gridView.Columns[i].Name.Replace("_", " ");

                Debug.WriteLine("i=" + i + ",sHeader=" + sHeader);

                if (sHeader.Equals(pHeader))
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        public void SetDate(DateTimePicker obj, string sDate)
        {
            Debug.WriteLine("--SetDate--");
            Debug.WriteLine("obj.Name=" + obj.Name);
            Debug.WriteLine("sDate=" + sDate);

            // ROCKY -- FIX FUNCTION: ADD HANDLE FOR NULL VALUE
            if (!string.IsNullOrWhiteSpace(sDate) || sDate.Equals(sDateFormat) || sDate.Equals(sDateFormatMDY))
            {
                DateTime dateTime = DateTime.ParseExact(sDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                obj.Value = dateTime;
            }
            else
            {
                obj.Value = DateTime.Now;
            }

        }

        public void SetTime(DateTimePicker obj, string sTime)
        {
            DateTime dateTime = DateTime.Parse(sTime);

            obj.Value = dateTime;
            obj.CustomFormat = "hh:mm tt";
            obj.Format = DateTimePickerFormat.Custom;
        }

        public void SetDateCustomFormatForFilter(DateTimePicker obj)
        {
            // Set the Format type and the CustomFormat string.
            obj.Format = DateTimePickerFormat.Custom;
            obj.CustomFormat = "MM-dd-yyyy";
        }

        public void GetProcessedByAndDateTime()
        {
            DateTime dteDateTime = DateTime.Now;
            string sDateTime = "";
            string sDate = "";
            string sTime = "";

            sDateTime = dteDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDate = dteDateTime.ToString("yyyy-MM-dd");
            sTime = dteDateTime.ToString("H:mm:ss");

            clsUser.ClassProcessedDate = sDate;
            clsUser.ClassProcessedTime = sTime;
            clsUser.ClassProcessedDateTime = sDateTime;
            clsUser.ClassProcessedBy = clsUser.ClassUserFullName;
        }

        public void GetModifiedByAndDateTime()
        {
            DateTime dteDateTime = DateTime.Now;
            string sDateTime = "";
            string sDate = "";
            string sTime = "";

            sDateTime = dteDateTime.ToString("yyyy-MM-dd H:mm:ss");
            sDate = dteDateTime.ToString("yyyy-MM-dd");
            sTime = dteDateTime.ToString("H:mm:ss");

            clsUser.ClassModifiedDate = sDate;
            clsUser.ClassModifiedTime = sTime;
            clsUser.ClassModifiedDateTime = sDateTime;
            clsUser.ClassModifiedBy = clsUser.ClassUserFullName;
        }

        public void GetIDFromFile(string pSearchBy, string pSearchValue)
        {
            //Debug.WriteLine("--GetIDFromFile--");
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);

            string pFullPathFileName = "";
            int i = 0;
            string json = "";
            int ID = 0;
            string Description = "";

            clsSearch.ClassOutFileID = 0;

            List<string> IDCol = new List<String>();
            List<string> DescriptionCol = new List<String>();
            List<string> DescriptionCol2 = new List<String>();

            // ----------------------------------------------------------------------------
            // FileName Path
            // ----------------------------------------------------------------------------           
            switch (pSearchBy)
            {
                case "Region":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_REGIONLIST_FILENAME;
                    break;
                case "Province":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_PROVINCELIST_FILENAME;
                    break;
                case "RegionDetail":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_REGIONDETAILLIST_FILENAME;
                    break;
                case "Terminal Type":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TERMINALTYPELIST_FILENAME;
                    break;
                case "Terminal Model":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TERMINALMODELLIST_FILENAME;
                    break;
                case "Terminal Brand":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TERMINALBRANDLIST_FILENAME;
                    break;
                case "Status List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TERMINALSTATUSLIST_FILENAME;
                    break;
                case "Service Type Active":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_SERVICETYPELIST_FILENAME;
                    break;
                case "Service Status Active":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_SERVICESTATUSLIST_FILENAME;
                    break;
                case "Client List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_CLIENTLIST_FILENAME;
                    break;
                case "SP List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_SPLIST_FILENAME;
                    break;
                case "FE List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_FELIST_FILENAME;
                    break;
                case "Reason":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_REASON_FILENAME;
                    break;
                //case "Resolution":
                //    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_RESOLUTION_FILENAME;
                //    break;
                case "Department":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_DEPARTMENTLIST_FILENAME;
                    break;
                case "Position":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_POSITIONLIST_FILENAME;
                    break;
                case "LeaveType":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_LEAVETYPELIST_FILENAME;
                    break;
                case "WorkType":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_WORKTYPELIST_FILENAME;
                    break;
                case "Country":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_COUNTRYLIST_FILENAME;
                    break;
                case "Location":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_LOCATIONLIST_FILENAME;
                    break;
                case "Asset Type":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_ASSETTYPELIST_FILENAME;
                    break;
                case "Carrier":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_CARRIERLIST_FILENAME;
                    break;
                case "Setup":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_SETUPLIST_FILENAME;
                    break;
                case "Particular":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_PARTICULARLIST_FILENAME;
                    break;
                case "Type List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TYPELIST_FILENAME;
                    break;
                case "Rental Fee List":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_RENTALFEELIST_FILENAME;
                    break;
                case "All Type":
                    pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_ALLTYPE_FILENAME;
                    break;
            }
            //Debug.WriteLine("pFullPathFileName=" + pFullPathFileName);
            // ----------------------------------------------------------------------------
            // FileName Path
            // ----------------------------------------------------------------------------

            using (StreamReader r = new StreamReader(pFullPathFileName))
            {
                json = r.ReadToEnd(); // issue value is None  cannot be parse
            }

            CollectionDataDetailOnline colData = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(json); // Parse JSON Format
            foreach (var element in colData.data)
            {
                switch (pSearchBy)
                {
                    case "Region":
                        IDCol.Add(element.RegionID.ToString());
                        DescriptionCol.Add(element.Region);
                        break;
                    case "Province":
                        IDCol.Add(element.RegionID.ToString());
                        DescriptionCol.Add(element.Region);
                        break;
                    case "RegionDetail":
                        IDCol.Add(element.RegionID.ToString());
                        DescriptionCol.Add(element.Province);
                        break;
                    case "Terminal Type":
                        IDCol.Add(element.TerminalTypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Terminal Model":
                        IDCol.Add(element.TerminalModelID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Terminal Brand":
                        IDCol.Add(element.TerminalBrandID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Status List":
                        IDCol.Add(element.TerminalStatusID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Status":
                        IDCol.Add(element.TerminalStatusID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Service Type Active":
                        IDCol.Add(element.ServiceTypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Service Status Active":
                        IDCol.Add(element.ServiceStatus.ToString());
                        DescriptionCol.Add(element.ServiceStatusDescription);
                        break;
                    case "Particular List":
                    case "Client List":
                    case "SP List":
                    case "FE List":
                        IDCol.Add(element.ParticularID.ToString());
                        DescriptionCol.Add(element.Name);
                        //DescriptionCol2.Add(element.Address);
                        break;
                    case "Reason":
                        //case "Resolution":
                        IDCol.Add(element.RegionID.ToString());
                        DescriptionCol.Add(element.Region);
                        break;
                    case "Department":
                        IDCol.Add(element.DepartmentID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Position":
                        IDCol.Add(element.PositionID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "LeaveType":
                        IDCol.Add(element.LeaveTypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "WorkType":
                        IDCol.Add(element.WorkTypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Country":
                        IDCol.Add(element.ID.ToString());
                        DescriptionCol.Add(element.Country);
                        break;
                    case "Location":
                    case "Asset Type":
                    case "Carrier":
                    case "Setup":
                        IDCol.Add(element.ID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Particular":
                        IDCol.Add(element.ID.ToString());
                        DescriptionCol.Add(element.ParticularName);
                        break;
                    case "Type List":
                    case "Type":
                    case "All Type":
                        IDCol.Add(element.TypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                    case "Rental Fee List":
                        IDCol.Add(element.TypeID.ToString());
                        DescriptionCol.Add(element.Description);
                        break;
                }
            }

            clsArray.ID = IDCol.ToArray();
            clsArray.Description = DescriptionCol.ToArray();

            //Debug.WriteLine("GetIDFromFile, clsArray.ID=" + clsArray.ID.Length);

            // Loop to search
            while (clsArray.ID.Length > i)
            {
                Debug.WriteLine(pSearchValue + " is equal to " + clsArray.Description[i]);
                if (pSearchValue.Equals(clsArray.ID[i]) || pSearchValue.Equals(clsArray.Description[i]))
                {
                    // found
                    ID = int.Parse(clsArray.ID[i]);
                    Description = clsArray.Description[i];
                    //Description2 = clsArray.Address[i];
                    break;
                }

                i++;
            }

            clsSearch.ClassOutFileID = ID;
            clsSearch.ClassOutFileDescription = Description;
            //clsSearch.ClassOutFileDescription2 = Description2; // Address

            //Debug.WriteLine("Search result...");
            //Debug.WriteLine("clsSearch.ClassOutFileID=" + clsSearch.ClassOutFileID);
            //Debug.WriteLine("clsSearch.ClassOutFileDescription=" + clsSearch.ClassOutFileDescription);
            //Debug.WriteLine("clsSearch.ClassOutFileDescription2=" + clsSearch.ClassOutFileDescription2);

        }

        public void GetServiceTypeInfoFromFile(string pSearchBy, string pSearchValue,
                                                out int outServiceTypeID, out string outDescription,
                                                out string outServiceCode, out int outServiceStatus,
                                                out string outServiceStatusDescription,
                                                out int outJobType,
                                                out string outJobTypeDescription,
                                                out string outServiceJobTypeDescription)
        {
            //Debug.WriteLine("--GetServiceTypeInfoFromFile--");            
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);

            int i = 0;
            string json = "";

            outServiceTypeID = 0;
            outDescription = "";
            outServiceCode = "";
            outServiceStatus = 0;
            outServiceStatusDescription = "";
            outJobType = 0;
            outJobTypeDescription = "";
            outServiceJobTypeDescription = "";

            List<string> ServiceTypeIDCol = new List<String>();
            List<string> DescriptionCol = new List<String>();
            List<string> ServiceCodeCol = new List<String>();
            List<string> ServiceStatusCol = new List<String>();
            List<string> ServiceStatusDescriptionCol = new List<String>();
            List<string> JobTypeCol = new List<String>();
            List<string> JobTypeDescriptionCol = new List<String>();
            List<string> ServiceJobTypeDescriptionCol = new List<String>();

            string[] arrServiceTypeID;
            string[] arrDescription;
            string[] arrServiceCode;
            string[] arrServiceStatus;
            string[] arrServiceStatusDescription;
            string[] arrJobType;
            string[] arrJobTypeDescription;
            string[] arrServiceJobTypeDescription;

            string pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_SERVICETYPELIST_FILENAME;
            //Debug.WriteLine("pFullPathFileName=" + pFullPathFileName);

            using (StreamReader r = new StreamReader(pFullPathFileName))
            {
                json = r.ReadToEnd();
            }

            CollectionDataDetailOnline colData = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(json); // Parse JSON Format
            foreach (var element in colData.data)
            {
                ServiceTypeIDCol.Add(element.ServiceTypeID.ToString());
                DescriptionCol.Add(element.Description);
                ServiceCodeCol.Add(element.Code);
                ServiceStatusCol.Add(element.ServiceStatus.ToString());
                ServiceStatusDescriptionCol.Add(element.ServiceStatusDescription);
                JobTypeCol.Add(element.JobType.ToString());
                JobTypeDescriptionCol.Add(element.JobTypeDescription);
                ServiceJobTypeDescriptionCol.Add(element.ServiceJobTypeDescription);

            }

            arrServiceTypeID = ServiceTypeIDCol.ToArray();
            arrDescription = DescriptionCol.ToArray();
            arrServiceCode = ServiceCodeCol.ToArray();
            arrServiceStatus = ServiceStatusCol.ToArray();
            arrServiceStatusDescription = ServiceStatusDescriptionCol.ToArray();
            arrJobType = JobTypeCol.ToArray();
            arrJobTypeDescription = JobTypeDescriptionCol.ToArray();
            arrServiceJobTypeDescription = ServiceJobTypeDescriptionCol.ToArray();

            //Debug.WriteLine("arrServiceTypeID=" + arrServiceTypeID.Length);

            // Loop to search
            while (arrServiceTypeID.Length > i)
            {
                //Debug.WriteLine("i=" + i + ", arrServiceTypeID=" + arrServiceTypeID[i] + ", arrDescription=" + arrDescription[i] + ",arrServiceCode=" + arrServiceCode[i] + ",arrServiceStatus=" + arrServiceStatus[i] + ",arrServiceStatusDescription=" + arrServiceStatusDescription[i] + ",arrJobType=" + arrJobType[i] + ",arrJobTypeDescription=" + arrJobTypeDescription[i] + ",arrServiceJobTypeDescription=" + arrServiceJobTypeDescription[i]);

                if (pSearchValue.Equals(arrServiceTypeID[i]) ||
                    pSearchValue.Equals(arrDescription[i]) ||
                    pSearchValue.Equals(arrServiceCode[i]) ||
                    pSearchValue.Equals(arrServiceStatus[i]) ||
                    pSearchValue.Equals(arrServiceStatusDescription[i]) ||
                    pSearchValue.Equals(arrJobType[i]) ||
                    pSearchValue.Equals(arrJobTypeDescription[i]) ||
                    pSearchValue.Equals(arrServiceJobTypeDescription[i])
                    )
                {
                    // found                    
                    outServiceTypeID = int.Parse(arrServiceTypeID[i]);
                    outDescription = arrDescription[i];
                    outServiceCode = arrServiceCode[i];
                    outServiceStatus = int.Parse(arrServiceStatus[i]);
                    outServiceStatusDescription = arrServiceStatusDescription[i];
                    outJobType = int.Parse(arrJobType[i]);
                    outJobTypeDescription = arrJobTypeDescription[i];
                    outServiceJobTypeDescription = arrServiceJobTypeDescription[i];

                    break;
                }

                i++;
            }

            Debug.WriteLine("Search result...");
            Debug.WriteLine("outServiceTypeID=" + outServiceTypeID);
            Debug.WriteLine("outDescription=" + outDescription);
            Debug.WriteLine("outServiceCode=" + outServiceCode);
            Debug.WriteLine("outServiceStatus=" + outServiceStatus);
            Debug.WriteLine("outServiceStatusDescription=" + outServiceStatusDescription);
            Debug.WriteLine("outJobType=" + outJobType);
            Debug.WriteLine("outJobTypeDescription=" + outJobTypeDescription);
            Debug.WriteLine("outServiceJobTypeDescription=" + outServiceJobTypeDescription);

        }

        public void GetTerminalStatusInfoFromFile(string pSearchBy, string pSearchValue, out int outTerminalStatusID, out string outDescription, out int outTerminalStatusType)
        {
            //Debug.WriteLine("--GetTerminalStatusInfoFromFile--");
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);

            int i = 0;
            string json = "";

            outTerminalStatusID = 0;
            outDescription = "";
            outTerminalStatusType = 0;

            List<string> TerminalStatusIDCol = new List<String>();
            List<string> DescriptionCol = new List<String>();
            List<string> TerminalStatusTypeCol = new List<String>();

            string[] arrTerminalStatusID;
            string[] arrDescription;
            string[] arrTerminalStatusType;

            string pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_TERMINALSTATUSLIST_FILENAME;
            //Debug.WriteLine("pFullPathFileName=" + pFullPathFileName);

            using (StreamReader r = new StreamReader(pFullPathFileName))
            {
                json = r.ReadToEnd();
            }

            CollectionDataDetailOnline colData = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(json); // Parse JSON Format
            foreach (var element in colData.data)
            {
                TerminalStatusIDCol.Add(element.TerminalStatusID.ToString());
                DescriptionCol.Add(element.Description);
                TerminalStatusTypeCol.Add(element.TerminalStatusType.ToString());

            }

            arrTerminalStatusID = TerminalStatusIDCol.ToArray();
            arrDescription = DescriptionCol.ToArray();
            arrTerminalStatusType = TerminalStatusTypeCol.ToArray();

            //Debug.WriteLine("arrTerminalStatusID=" + arrTerminalStatusID.Length);

            // Loop to search
            while (arrTerminalStatusID.Length > i)
            {
                //Debug.WriteLine("i="+i+ ", arrTerminalStatusID="+ arrTerminalStatusID[i]+ ", arrDescription="+ arrDescription[i]+ ",arrTerminalStatusType="+ arrTerminalStatusType[i]);

                if (pSearchValue.Equals(arrTerminalStatusID[i]) ||
                    pSearchValue.Equals(arrDescription[i]) ||
                    pSearchValue.Equals(arrTerminalStatusType[i])
                    )
                {
                    // found
                    outTerminalStatusID = int.Parse(arrTerminalStatusID[i]);
                    outDescription = arrDescription[i];
                    outTerminalStatusType = int.Parse(arrTerminalStatusType[i]);

                    break;
                }

                i++;
            }

            Debug.WriteLine("Search result...");
            Debug.WriteLine("outTerminalStatusID=" + outTerminalStatusID);
            Debug.WriteLine("outDescription=" + outDescription);
            Debug.WriteLine("outTerminalStatusType=" + outTerminalStatusType);
        }

        public void GetLeaveTypeInfoFromFile(string pSearchBy, string pSearchValue, out int outLeaveTypeID, out string outCode, out string outDescription, out double outCreditLimit)
        {
            //Debug.WriteLine("--GetLeaveTypeInfoFromFile--");
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);

            int i = 0;
            string json = "";

            outLeaveTypeID = 0;
            outCode = "";
            outDescription = "";
            outCreditLimit = 0.00;

            List<string> LeaveTypeIDCol = new List<String>();
            List<string> CodeCol = new List<String>();
            List<string> DescriptionCol = new List<String>();
            List<string> CreditLimitCol = new List<String>();

            string[] arrLeaveTypeID;
            string[] arrCode;
            string[] arrDescription;
            string[] arrCreditLimit;

            string pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_LEAVETYPELIST_FILENAME;
            //Debug.WriteLine("pFullPathFileName=" + pFullPathFileName);

            using (StreamReader r = new StreamReader(pFullPathFileName))
            {
                json = r.ReadToEnd();
            }

            CollectionDataDetailOnline colData = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(json); // Parse JSON Format
            foreach (var element in colData.data)
            {
                LeaveTypeIDCol.Add(element.LeaveTypeID.ToString());
                CodeCol.Add(element.Code);
                DescriptionCol.Add(element.Description);
                CreditLimitCol.Add(element.CreditLimit.ToString());

            }

            arrLeaveTypeID = LeaveTypeIDCol.ToArray();
            arrCode = CodeCol.ToArray();
            arrDescription = DescriptionCol.ToArray();
            arrCreditLimit = CreditLimitCol.ToArray();

            //Debug.WriteLine("arrLeaveTypeID=" + arrLeaveTypeID.Length);

            // Loop to search
            while (arrLeaveTypeID.Length > i)
            {
                //Debug.WriteLine("i=" + i + ", arrLeaveTypeID=" + arrLeaveTypeID[i] + ", arrCode=" + arrCode[i] + ", arrDescription=" + arrDescription[i] + ",arrCreditLimit=" + arrCreditLimit[i]);

                if (pSearchValue.Equals(arrLeaveTypeID[i]) ||
                    pSearchValue.Equals(arrCode[i]) ||
                    pSearchValue.Equals(arrDescription[i])
                    )
                {
                    // found
                    outLeaveTypeID = int.Parse(arrLeaveTypeID[i]);
                    outCode = arrCode[i];
                    outDescription = arrDescription[i];
                    outCreditLimit = int.Parse(arrCreditLimit[i]);

                    break;
                }

                i++;
            }

            Debug.WriteLine("Search result...");
            Debug.WriteLine("outLeaveTypeID=" + outLeaveTypeID);
            Debug.WriteLine("outCode=" + outCode);
            Debug.WriteLine("outDescription=" + outDescription);
            Debug.WriteLine("outCreditLimit=" + outCreditLimit);
        }

        public void GetListViewHeaderColumnFromFile(string pSearchBy, string pSearchValue, out string outField, out int outWidth,
                                                                                     out string outTitle, out HorizontalAlignment outAlign,
                                                                                     out bool outVisible, out bool outAutoWidth,
                                                                                     out string outFormat)
        {
            //Debug.WriteLine("--GetLeaveTypeInfoFromFile--");
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);

            int i = 0;
            string json = "";

            outField = "UNDEFINED";
            outWidth = 0;
            outTitle = "UNDEFINED";
            outAlign = 0; // (Left = 0, Right = 1, Center = 2)
            outVisible = false;
            outAutoWidth = false;
            outFormat = "";

            List<string> FieldCol = new List<String>();
            List<string> WidthCol = new List<String>();
            List<string> TitleCol = new List<String>();
            List<string> AlignCol = new List<String>();
            List<string> VisibleCol = new List<String>();
            List<string> AutoWidthCol = new List<String>();
            List<string> FormatCol = new List<String>();

            string[] arrField;
            string[] arrWidth;
            string[] arrTitle;
            string[] arrAlign;
            string[] arrVisible;
            string[] arrAutoWidth;
            string[] arrFormat;

            try
            {
                string pFullPathFileName = dbFile.sRespFullPath + clsDefines.RESP_LISTVIEWHEADERCOLUMN_FILENAME;
                //Debug.WriteLine("pFullPathFileName=" + pFullPathFileName);

                using (StreamReader r = new StreamReader(pFullPathFileName))
                {
                    json = r.ReadToEnd();
                }

                CollectionDataDetailOnline colData = JsonConvert.DeserializeObject<CollectionDataDetailOnline>(json); // Parse JSON Format
                foreach (var element in colData.data)
                {
                    FieldCol.Add(element.Field.ToString());
                    WidthCol.Add(element.Width.ToString());
                    TitleCol.Add(element.Title.ToString());
                    AlignCol.Add(element.Align.ToString());
                    VisibleCol.Add(element.Visible.ToString());
                    AutoWidthCol.Add(element.AutoWidth.ToString());
                    FormatCol.Add(element.Format.ToString());

                }

                arrField = FieldCol.ToArray();
                arrWidth = WidthCol.ToArray();
                arrTitle = TitleCol.ToArray();
                arrAlign = AlignCol.ToArray();
                arrVisible = VisibleCol.ToArray();
                arrAutoWidth = AutoWidthCol.ToArray();
                arrFormat = FormatCol.ToArray();

                //Debug.WriteLine("arrField=" + arrField.Length);

                // Loop to search
                while (arrField.Length > i)
                {
                    //Debug.WriteLine("i=" + i + ", arrField=" + arrField[i] + ", arrWidth=" + arrWidth[i] + ", arrTitle=" + arrTitle[i] + ",arrAlign=" + arrAlign[i]);

                    string sField = arrField[i].ToUpper();
                    string sTitle = arrTitle[i].ToUpper();

                    //Debug.WriteLine(sField + "->" + pSearchValue.ToUpper());
                    //Debug.WriteLine(sTitle + "->" + pSearchValue.ToUpper());

                    if (pSearchValue.ToUpper().Equals(sField) || pSearchValue.ToUpper().Equals(sTitle))
                    {
                        //Debug.WriteLine("Field found!!!->"+pSearchValue);

                        // found                    
                        outField = arrField[i].ToUpper();
                        outWidth = int.Parse(arrWidth[i]);
                        outTitle = arrTitle[i].ToUpper();

                        if (arrAlign[i].ToUpper().Equals("LEFT"))
                            outAlign = HorizontalAlignment.Left;
                        if (arrAlign[i].ToUpper().Equals("RIGHT"))
                            outAlign = HorizontalAlignment.Right;
                        if (arrAlign[i].ToUpper().Equals("CENTER"))
                            outAlign = HorizontalAlignment.Center;

                        outVisible = (arrVisible[i].ToUpper().Equals("TRUE") ? true : false);
                        outAutoWidth = (arrAutoWidth[i].ToUpper().Equals("TRUE") ? true : false);
                        outFormat = arrFormat[i];

                        break;
                    }

                    i++;
                }
            }
            catch (Exception ex)
            {
                SetMessageBox("Exceptional error " + ex.Message, "GetListViewHeaderColumnFromFile", IconType.iError);
            }

            //Debug.WriteLine("Search result...");
            //Debug.WriteLine("pSearchBy=" + pSearchBy);
            //Debug.WriteLine("pSearchValue=" + pSearchValue);
            //Debug.WriteLine("outField=" + outField);
            //Debug.WriteLine("outWidth=" + outWidth);
            //Debug.WriteLine("outTitle=" + outTitle);
            //Debug.WriteLine("outAlign=" + outAlign);
            //Debug.WriteLine("outVisible=" + outVisible);
            //Debug.WriteLine("outAutoWidth=" + outAutoWidth);
            //Debug.WriteLine("outFormat=" + outFormat);
        }

        public string getReformatDate(string pDate)
        {
            string sFormatted = "0000-00-00";
            string sYear = "0000";
            string sMos = "00";
            string sDay = "00";

            if (pDate.Length > 0)
            {
                sYear = pDate.Substring(pDate.Length - 4, 4);
                sMos = pDate.Substring(0, 2);
                sDay = pDate.Substring(3, 2);
            }

            sFormatted = sYear + sDash + sMos + sDash + sDay;

            return sFormatted;
        }

        public bool isWeekEnd(DateTime pDate)
        {
            bool isValid = false;

            DayOfWeek today = pDate.DayOfWeek;

            if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
            {
                isValid = true;
            }

            return isValid;
        }

        public string getDelimitedString(string pString, char pDelimeter, int pIndex)
        {
            string sTemp = sNull;
            string[] arrString = pString.Split(pDelimeter);
            int iCount = arrString.Length;

            //Debug.WriteLine("getDelimitedString->pString=" + pString);
            //Debug.WriteLine("getDelimitedString->pDelimeter=" + pDelimeter);
            //Debug.WriteLine("getDelimitedString->iCount=" + iCount + ",pIndex=" + pIndex);

            if (iCount > 0)
            {
                for (int i = 0; i < iCount; i++)
                {
                    if (i == pIndex)
                    {
                        sTemp = arrString[i].ToString();
                        //Debug.WriteLine("getDelimitedString->i=" + i+",sTemp="+ sTemp);

                        // ROCKY - FUNCTION ISSUE: FIX FOR FUNCTION ERROR IN DATE PARSE FORMATS - 0000-00-00
                        if (sTemp == "0000-00-00")
                        {
                            sTemp = DateTime.Now.ToString("yyyy-MM-dd");
                        }

                        return sTemp;
                    }
                }
            }
            else
            {
                sTemp = sNull;
            }

            return sTemp;

        }

        public string parseDelimitedString(string pString, char pDelimeter, int startIndex)
        {
            string sTemp = sNull;
            string[] arrString = pString.Split(pDelimeter);
            int iCount = arrString.Length;
            int iLineNo = startIndex;

            if (iCount > 0)
            {
                sTemp = Environment.NewLine;

                for (int i = 0; i < iCount; i++)
                {
                    sTemp = sTemp +
                           "[" + iLineNo + "]" +
                           " > " + "{" + arrString[i].ToString() + "}" + Environment.NewLine;

                    iLineNo++;
                }

                Debug.WriteLine("--parseDelimitedString--");
                Debug.WriteLine("Len=" + pString.Length + ",pString=" + pString);
                Debug.WriteLine("pDelimeter=" + pDelimeter);
                Debug.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Debug.WriteLine(sTemp);
                Debug.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }

            return sTemp;

        }

        public string setIntegerToYesNo(int pValue)
        {
            string sTemp = clsFunction.sNull;

            if (pValue > 0)
                sTemp = sYes;
            else
                sTemp = sNo;

            return sTemp;
        }

        public bool setYesNoToBoolean(string pValue)
        {
            bool isBool = false;

            if (pValue.Equals(sYes))
                isBool = true;
            else
                isBool = false;

            return isBool;
        }

        public string setBooleanToYesNo(bool pValue)
        {
            string sTemp = clsFunction.sNull;

            if (pValue)
                sTemp = sYes;
            else
                sTemp = sNo;

            return sTemp;
        }

        public Color GetColorByStatus(int pStatus, string pDescription)
        {
            //Debug.WriteLine("--GetColorByStatus--");
            //Debug.WriteLine("pStatus="+ pStatus);
            //Debug.WriteLine("pDescription=" + pDescription);

            Color color = Color.Black;

            /*
            if (pStatus.Equals(clsGlobalVariables.STATUS_INSTALLED))
                color = Color.FromArgb(0, 179, 60);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_ALLOCATED))
                color = Color.FromArgb(255, 128, 0);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_DAMAGE))
                color = Color.FromArgb(255, 51, 0);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_PULLED_OUT))
                color = Color.FromArgb(255, 51, 0);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_LOSS))
                color = Color.FromArgb(102, 0, 255);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_DISPATCH))
                color = Color.FromArgb(0, 0, 255);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_AVAILABLE))
                color = Color.Black;
            else if (pStatus.Equals(clsGlobalVariables.STATUS_BORROWED))
                color = Color.FromArgb(51, 204, 204);
            else if (pStatus.Equals(clsGlobalVariables.STATUS_NEGATIVE))
                color = Color.FromArgb(255, 51, 0);
            else
                color = Color.Black;
             */

            if (pDescription.Equals(clsGlobalVariables.STATUS_INSTALLED_DESC) ||
                pDescription.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                color = Color.FromArgb(0, 179, 60);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_ALLOCATED_DESC))
                color = Color.FromArgb(255, 128, 0);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_DAMAGE_DESC))
                color = Color.FromArgb(255, 51, 0);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_PULLEDOUT_DESC))
                color = Color.FromArgb(255, 51, 0);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_LOSS_DESC))
                color = Color.FromArgb(102, 0, 255);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_DISPATCH_DESC))
                color = Color.FromArgb(0, 0, 255);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_AVAILABLE_DESC))
                color = Color.FromArgb(0, 0, 0);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_BORROWED_DESC))
                color = Color.FromArgb(51, 204, 204);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_NEGATIVE_DESC) ||
                pDescription.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE))
                color = Color.FromArgb(255, 51, 0);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_HOLD_DESC))
                color = Color.FromArgb(77, 77, 77);
            else if (pDescription.Equals(clsGlobalVariables.STATUS_CANCEL_DESC))
                color = Color.FromArgb(128, 0, 0);  
            else
                color = Color.FromArgb(0, 0, 0);

            return color;
        }

        public void FormButtoBackcolor(Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is Button)
                    {
                        control.ForeColor = Color.Black;
                        control.Font = new Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        control.BackColor = Color.WhiteSmoke;

                        if (control.Name.Equals("btnAdd"))
                            control.BackColor = clsFunction.AddButtonBackColor;
                        if (control.Name.Equals("btnSave"))
                            control.BackColor = clsFunction.SaveButtonBackColor;
                        if (control.Name.Equals("btnClear"))
                            control.BackColor = clsFunction.ClearButtonBackColor;
                        if (control.Name.Equals("btnDelete"))
                            control.BackColor = clsFunction.DeleteButtonBackColor;
                        if (control.Name.Equals("btnPrint"))
                            control.BackColor = clsFunction.PrintButtonBackColor;
                        if (control.Name.Equals("btnDispactJO"))
                            control.BackColor = clsFunction.SaveButtonBackColor;
                        if (control.Name.Equals("btnCancelJO"))
                            control.BackColor = clsFunction.DeleteButtonBackColor;
                    }
                    else
                        func(control.Controls);
            };

            func(obj.Controls);
        }

        public void SetIconImage(Bunifu.Framework.UI.BunifuImageButton btn, IconType iconType)
        {
            switch (iconType)
            {
                case IconType.iAdd_On:
                    btn.Image = Properties.Resources.add_on;
                    break;
                case IconType.iAdd_Off:
                    btn.Image = Properties.Resources.add_off;
                    break;
                case IconType.iFind_On:
                    btn.Image = Properties.Resources.find_on;
                    break;
                case IconType.iFind_On2:
                    btn.Image = Properties.Resources.find_on_2;
                    break;
                case IconType.iFind_Off:
                    btn.Image = Properties.Resources.find_off;
                    break;
                case IconType.iFolder_On:
                    btn.Image = Properties.Resources.folder_on;
                    break;
                case IconType.iFolder_Off:
                    btn.Image = Properties.Resources.folder_off;
                    break;
                case IconType.iRemove_On:
                    btn.Image = Properties.Resources.remove_on;
                    break;
                case IconType.iRemove_Off:
                    btn.Image = Properties.Resources.remove_off;
                    break;
                case IconType.iSearch_On:
                    btn.Image = Properties.Resources.search_on;
                    break;
                case IconType.iSearch_Off:
                    btn.Image = Properties.Resources.search_off;
                    break;
            }
        }

        public bool isValidCarrier()
        {
            bool isValid = false;

            return isValid;
        }

        public bool isSNChanged(int pOldID, string pOldSN, int pNewID, string pNewSN)
        {
            bool isChanged = false;

            Debug.WriteLine("--isTerminalChanged--");

            Debug.WriteLine("Old Terminal ID=" + pOldID, ",SN=" + pOldSN);
            Debug.WriteLine("New Terminal ID=" + pNewID, ",SN=" + pNewSN);

            if (pOldID > 0)
            {
                if (pOldSN.CompareTo(pNewSN) == 0)
                    isChanged = false;
                else
                    isChanged = true;
            }
            else
            {
                isChanged = false;
            }

            Debug.WriteLine("isChanged=" + isChanged);

            return isChanged;
        }

        public bool isValidDataGridValue(ImportType importType, clsAPI dbAPI, DataGridView dataGridView, string pFileName, int checkIndex, bool isBypass)
        {
            bool isValid = true;
            bool outIsMust = false;
            string outFormat = sNull;
            int outColIndex = iZero;
            int i = 0;
            int x = 0;
            string sFieldCheck = sNull;
            string sHeader = sNull;
            string sMessage = "";
            string sTemp = "";
            bool isExitLoop = false;
            int iLineNo = 0;
            string sHint = "";
            bool isOutput = false;

            Debug.WriteLine("--isValidDataGridValue--");
            Debug.WriteLine("pFileName=" + pFileName);
            Debug.WriteLine("checkIndex=" + checkIndex);
            Debug.WriteLine("dataGridView.RowCount=" + dataGridView.RowCount);
            Debug.WriteLine("dataGridView.ColumnCount=" + dataGridView.ColumnCount);

            Cursor.Current = Cursors.WaitCursor;

            switch (checkIndex)
            {
                case 0:
                    switch (importType)
                    {
                        case ImportType.iIRImportDetail:
                            for (i = 0; i < dataGridView.RowCount; i++)
                            {
                                iLineNo++;
                                // get column index
                                int iMerchantName = GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
                                int iIRNo = GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
                                int iRequestDate = GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
                                int iRequestFor = GetMapColumnIndex(clsDefines.IR_REQUESTOR);
                                int iTID = GetMapColumnIndex(clsDefines.IR_TID);
                                int iMID = GetMapColumnIndex(clsDefines.IR_MID);
                                int iAddress = GetMapColumnIndex(clsDefines.IR_ADDRESS);
                                int iRegion = GetMapColumnIndex(clsDefines.IR_AREA1);
                                int iProvince = GetMapColumnIndex(clsDefines.IR_CITY);
                                int iContactPerson = GetMapColumnIndex(clsDefines.IR_CONTACT_PERSON);
                                int iContactNumber = GetMapColumnIndex(clsDefines.IR_CONTACT_NUMBER);

                                // get column value
                                string sMerchantName = dataGridView.Rows[i].Cells[iMerchantName].Value.ToString();
                                string sIRNo = dataGridView.Rows[i].Cells[iIRNo].Value.ToString();
                                string sRequestDate = dataGridView.Rows[i].Cells[iRequestDate].Value.ToString();
                                string sRequestFor = dataGridView.Rows[i].Cells[iRequestFor].Value.ToString();
                                string sTID = dataGridView.Rows[i].Cells[iTID].Value.ToString();
                                string sMID = dataGridView.Rows[i].Cells[iMID].Value.ToString();
                                string sAddress = dataGridView.Rows[i].Cells[iAddress].Value.ToString();
                                string sRegion = dataGridView.Rows[i].Cells[iRegion].Value.ToString();
                                string sProvince = dataGridView.Rows[i].Cells[iProvince].Value.ToString();
                                string sContactPerson = dataGridView.Rows[i].Cells[iContactPerson].Value.ToString();
                                string sContactNumber = dataGridView.Rows[i].Cells[iContactNumber].Value.ToString();

                                if (isValidDescription(sMerchantName) &&
                                        isValidDescription(sIRNo) &&
                                        isValidDescription(sRequestDate) &&
                                        isValidDescription(sRequestFor) &&
                                        isValidDescription(sTID) &&
                                        isValidDescription(sMID) &&
                                        isValidDescription(sAddress) &&
                                        isValidDescription(sRegion) &&
                                        isValidDescription(sProvince) &&
                                        isValidDescription(sContactPerson) &&
                                        isValidDescription(sContactNumber))
                                {
                                    // do nothing
                                }
                                else
                                {
                                    isValid = false;
                                    sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                             "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                             "> Request ID : " + sIRNo + Environment.NewLine +
                                             "> Request Date : " + sRequestDate + Environment.NewLine +
                                             "> Requestor : " + sRequestFor + Environment.NewLine +
                                             "> TID : " + sTID + Environment.NewLine +
                                             "> MID : " + sMID + Environment.NewLine +
                                             "> Address : " + sAddress + Environment.NewLine +
                                             "> Region : " + sRegion + Environment.NewLine +
                                             "> Province : " + sProvince + Environment.NewLine +
                                             "> Contact Person : " + sContactPerson + Environment.NewLine +
                                             "> Contact Number : " + sContactNumber + Environment.NewLine +
                                             sSingleLineSeparator + Environment.NewLine;
                                }
                            }

                            sHint = "Mandatory fields must not be blank." + Environment.NewLine +
                                    " > Request ID" + Environment.NewLine +
                                    " > Request Date" + Environment.NewLine +
                                    " > Requestor" + Environment.NewLine +
                                    " > Merchant Name" + Environment.NewLine +
                                    " > TID" + Environment.NewLine +
                                    " > MID" + Environment.NewLine +
                                    " > Address" + Environment.NewLine +
                                    " > Region" + Environment.NewLine +
                                    " > Province" + Environment.NewLine +
                                    " > Contact Person" + Environment.NewLine +
                                    " > Contact Number";
                            break;

                        case ImportType.iImportTerminal:
                            for (i = 0; i < dataGridView.RowCount; i++)
                            {
                                iLineNo++;
                                string pSerialNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_SerialNo)].Value.ToString();
                                string pType = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Type)].Value.ToString();
                                string pModel = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Model)].Value.ToString();
                                string pBrand = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Brand)].Value.ToString();
                                string pLocation = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Location)].Value.ToString();
                                string pStatus = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Status)].Value.ToString();

                                if (isValidDescription(pSerialNo) &&
                                isValidDescription(pType) &&
                                isValidDescription(pModel) &&
                                isValidDescription(pBrand) &&
                                isValidDescription(pLocation) &&
                                isValidDescription(pStatus))
                                {
                                    // do nothing
                                }
                                else
                                {
                                    isValid = false;
                                    sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                             "> Serial No. : " + pSerialNo + Environment.NewLine +
                                             "> Type : " + pType + Environment.NewLine +
                                             "> Model : " + pModel + Environment.NewLine +
                                             "> Brand : " + pBrand + Environment.NewLine +
                                             "> Status : " + pStatus + Environment.NewLine +
                                             sSingleLineSeparator + Environment.NewLine;
                                }
                            }

                            sHint = "Mandatory fields must not be blank." + Environment.NewLine +
                                    " > Serial No." + Environment.NewLine +
                                    " > Type" + Environment.NewLine +
                                    " > Model" + Environment.NewLine +
                                    " > Brand" + Environment.NewLine +
                                    " > Status";
                            break;

                        case ImportType.iImportSIM:
                            for (i = 0; i < dataGridView.RowCount; i++)
                            {
                                iLineNo++;
                                string pSerialNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_SerialNo)].Value.ToString();
                                string pCarrier = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Carrier)].Value.ToString();
                                string pLocation = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Location)].Value.ToString();
                                string pStatus = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Status)].Value.ToString();

                                if (isValidDescription(pSerialNo) &&
                                isValidDescription(pCarrier) &&
                                isValidDescription(pLocation) &&
                                isValidDescription(pStatus))
                                {
                                    // do nothing
                                }
                                else
                                {
                                    isValid = false;
                                    sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                             "> Serial No. : " + pSerialNo + Environment.NewLine +
                                             "> Carrier : " + pCarrier + Environment.NewLine +
                                             "> Status : " + pStatus + Environment.NewLine +
                                             sSingleLineSeparator + Environment.NewLine;
                                }
                            }

                            sHint = "Mandatory fields must not be blank." + Environment.NewLine +
                                    " > Serial No." + Environment.NewLine +
                                    " > Carrier" + Environment.NewLine +
                                    " > Status";
                            break;

                        case ImportType.iImportStock:
                            for (i = 0; i < dataGridView.RowCount; i++)
                            {
                                iLineNo++;
                                string pSerialNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_SerialNo)].Value.ToString();
                                string pType = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Type)].Value.ToString();
                                string pModel = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Model)].Value.ToString();
                                string pBrand = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Brand)].Value.ToString();
                                string pLocation = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Location)].Value.ToString();
                                string pStatus = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.HDR_TEMPLATE_Status)].Value.ToString();

                                if (isValidDescription(pSerialNo) &&
                                isValidDescription(pType) &&
                                isValidDescription(pModel) &&
                                isValidDescription(pBrand) &&
                                isValidDescription(pLocation) &&
                                isValidDescription(pStatus))
                                {
                                    // do nothing
                                }
                                else
                                {
                                    isValid = false;
                                    sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                             "> Serial No. : " + pSerialNo + Environment.NewLine +
                                             "> Type : " + pType + Environment.NewLine +
                                             "> Model : " + pModel + Environment.NewLine +
                                             "> Brand : " + pBrand + Environment.NewLine +
                                             "> Status : " + pStatus + Environment.NewLine +
                                             sSingleLineSeparator + Environment.NewLine;
                                }
                            }

                            sHint = "Mandatory fields must not be blank." + Environment.NewLine +
                                " > Serial No." + Environment.NewLine +
                                " > Type" + Environment.NewLine +
                                " > Model" + Environment.NewLine +
                                " > Brand" + Environment.NewLine +
                                " > Status";
                            break;
                    }

                    break;
                case 1:
                    for (i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        sHeader = dataGridView.Columns[i].Name.Replace("_", " ");

                        GetMapMustAndFormat(sHeader, ref outIsMust, ref outFormat, ref outColIndex);

                        Debug.WriteLine("sHeader=" + sHeader + ",outIsMust=" + outIsMust + ",outFormat=" + outFormat + ",outColIndex=" + outColIndex);

                        if (outIsMust)
                        {
                            // Loop
                            for (x = 0; x < dataGridView.RowCount; x++)
                            {
                                iLineNo++;
                                sFieldCheck = dataGridView.Rows[x].Cells[outColIndex].Value.ToString(); // Check is not blank
                                Debug.WriteLine("Checking...X=" + x + ",sFieldCheck=" + sFieldCheck);

                                // check 3 columns has no value exit loop
                                Debug.WriteLine("row=" + x + "=>" + dataGridView.Rows[x].Cells[0].Value.ToString() + ", " + dataGridView.Rows[x].Cells[1].Value.ToString() + ", " + dataGridView.Rows[x].Cells[2].Value.ToString());
                                if (dataGridView.Rows[x].Cells[0].Value.ToString().Length <= 0 && dataGridView.Rows[x].Cells[1].Value.ToString().Length <= 0 && dataGridView.Rows[x].Cells[2].Value.ToString().Length <= 0)
                                {
                                    isExitLoop = true;
                                    break;
                                }
                            }
                        }

                        // exit reading of rows...
                        if (isExitLoop) break;

                    }

                    sHint = "Check import file and use row/column/header as your reference.";

                    break;
                case 2:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;

                        // get column index
                        int iIRNo = GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
                        int iRequestDate = GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
                        int iDBAName = GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);

                        // get column value
                        string sIRNo = dataGridView.Rows[i].Cells[iIRNo].Value.ToString();
                        string sRequestDate = dataGridView.Rows[i].Cells[iRequestDate].Value.ToString();
                        string sMerchantName = dataGridView.Rows[i].Cells[iDBAName].Value.ToString();

                        string sFormattedDate = GetDateFromParse(sRequestDate, "yyyy-MM-dd", "yyyy-MM-dd");
                        Debug.WriteLine("i=" + i + ",sIRNo=" + sIRNo + ",sRequestDate=" + sRequestDate + ",sMerchantName=" + sMerchantName + ",sFormattedDate=" + sFormattedDate);

                        // Try parse original request date
                        DateTime requestDate;
                        string[] formats = {
                            "yyyy-MM-dd",
                            "MM-dd-yyyy H:mm:ss tt",
                            "MM/dd/yyyy",
                            "yyyy/MM/dd"
                        };

                        bool isValidDate = DateTime.TryParseExact(
                            sRequestDate,
                            formats,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out requestDate
                        );

                        if (!isValidDate)
                        {
                            sRequestDate = clsDefines.DEV_DATE;                            
                        }

                        sFormattedDate = requestDate.ToString("yyyy-MM-dd");

                        if (DateTime.Parse(sFormattedDate) >= DateTime.Parse(getCurrentDate()))
                        {
                            // do nothing...
                        }
                        else
                        {
                            isValid = false;
                            string sLogDate = GetDateFromParse(sRequestDate, "yyyy-MM-dd", "yyyy-MM-dd");

                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Request Date : " + sLogDate + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;

                        }

                    }

                    sHint = "Request date is outdated." + Environment.NewLine + "Please check and verify if valid.";

                    break;
                case 3:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;
                        // get column index
                        int iIRNo = GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
                        int iRequestDate = GetMapColumnIndex(clsDefines.IR_REQUEST_DATE);
                        int iDBAName = GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);

                        // get column value
                        string sIRNo = dataGridView.Rows[i].Cells[iIRNo].Value.ToString();
                        string sRequestDate = dataGridView.Rows[i].Cells[iRequestDate].Value.ToString();
                        string sMerchantName = dataGridView.Rows[i].Cells[iDBAName].Value.ToString();

                        if (!dbAPI.isRecordExist("Search", "IRNo Check", sIRNo))
                        {
                            // do nothing...
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                    }

                    sHint = "Request ID is already exist on the server.";

                    break;

                case 4:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;
                        // get column index
                        int iMerchantName = GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
                        int iRegion = GetMapColumnIndex(clsDefines.IR_AREA1); // Region
                        int iProvince = GetMapColumnIndex(clsDefines.IR_CITY);    // Province

                        string sMerchantName = dataGridView.Rows[i].Cells[iMerchantName].Value.ToString();
                        string sRegion = dataGridView.Rows[i].Cells[iRegion].Value.ToString();
                        string sProvince = dataGridView.Rows[i].Cells[iProvince].Value.ToString();

                        if (dbAPI.isRecordExist("Search", "Region", sRegion))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> Region : " + sRegion + Environment.NewLine +                                     
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                    }

                    sHint = "Region must exist on the server.";

                    break;
                case 5:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;

                        // get column index
                        int iIRNo = GetMapColumnIndex(clsDefines.IR_REQUEST_ID);
                        int iMerchantName = GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME);
                        int iTID = GetMapColumnIndex(clsDefines.IR_TID);
                        int iMID = GetMapColumnIndex(clsDefines.IR_MID);

                        // get column value
                        string sIRNo = dataGridView.Rows[i].Cells[iIRNo].Value.ToString();
                        // get column value
                        string sMerchantName = dataGridView.Rows[i].Cells[iMerchantName].Value.ToString();
                        string sTID = dataGridView.Rows[i].Cells[iTID].Value.ToString();
                        string sMID = dataGridView.Rows[i].Cells[iMID].Value.ToString();

                        Debug.WriteLine("i=" + i + ",sIRNo=" + sIRNo);

                        // Validate Request ID
                        if (isValidIRNoPrefix(sIRNo))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;                        
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> MID : " + sMID + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +                                     
                                     sSingleLineSeparator + Environment.NewLine;
                        }
                        
                    }

                    sHint = $"Invalid request id prefix found.{Environment.NewLine}Valid prefix are [{clsSystemSetting.ClassSystemIRNoPrefix}]";
                    break;
                case 6:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;

                        // get column value
                        string sIRNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_REQUEST_ID)].Value.ToString();
                        string sMerchantName = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME)].Value.ToString();
                        string sTID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_TID)].Value.ToString();
                        string sMID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MID)].Value.ToString();
                        string sAddress = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_ADDRESS)].Value.ToString();
                        string sRegion = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_AREA1)].Value.ToString();
                        string sProvince = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_CITY)].Value.ToString();

                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Basic Info 2", $"{sTID}{clsDefines.gPipe}{sMID}{clsDefines.gPipe}{sMerchantName}");
                        parseDelimitedString(pJSONString, clsDefines.gComma, 0);
                        
                        if (isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RegionType)))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> MID : " + sMID + Environment.NewLine +
                                     "> Region : " + sRegion + Environment.NewLine +
                                     "> Province : " + sProvince + Environment.NewLine + Environment.NewLine +
                                     "*Region does not exist!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                        if (isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RegionID)))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine + 
                                     "> MID : " + sMID + Environment.NewLine +
                                     "> Region : " + sRegion + Environment.NewLine +
                                     "> Province : " + sProvince + Environment.NewLine + Environment.NewLine +
                                     "*Province does not exist!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                        if (isValidID(dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ParticularID)))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> MID : " + sMID + Environment.NewLine +
                                     "> Region : " + sRegion + Environment.NewLine +
                                     "> Province : " + sProvince + Environment.NewLine + Environment.NewLine +
                                     "*Merchant name does not exist!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                    }
                    sHint = "Merchant name, region and province must exist on the server.";
                    break;
                case 7:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;

                        // get column value
                        string sIRNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_REQUEST_ID)].Value.ToString();
                        string sMerchantName = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME)].Value.ToString();
                        string sTID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_TID)].Value.ToString();
                        string sMID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MID)].Value.ToString();
                        string sAddress = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_ADDRESS)].Value.ToString();
                        string sRegion = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_AREA1)].Value.ToString();
                        string sProvince = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_CITY)].Value.ToString();

                        string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Basic Info 2", $"{sTID}{clsDefines.gPipe}{sMID}{clsDefines.gPipe}{sMerchantName}");
                        parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                        if (dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME).Equals(sMerchantName))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine + Environment.NewLine +
                                     "*Merchant name mismatch!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                        if (dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REGION).Equals(sRegion))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> Region : " + sRegion + Environment.NewLine + Environment.NewLine +
                                     "*Region mismatch!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                        if (dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Province).Equals(sProvince))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> Province : " + sProvince + Environment.NewLine + Environment.NewLine +
                                     "*Province mismatch!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                        if (dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Address).Equals(sAddress))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;
                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> Address : " + sAddress + Environment.NewLine + Environment.NewLine +
                                     "*Address mismatch!" + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }
                        
                    }
                    sHint = "Merchant name, address, region and province mismatch on the server." + Environment.NewLine +
                            "Update record using import data.";
                    break;
                case 8:
                    for (i = 0; i < dataGridView.RowCount; i++)
                    {
                        iLineNo++;

                        /// get column value
                        string sIRNo = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_REQUEST_ID)].Value.ToString();
                        string sMerchantName = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MERCHANT_NAME)].Value.ToString();
                        string sTID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_TID)].Value.ToString();
                        string sMID = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_MID)].Value.ToString();
                        string sAddress = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_ADDRESS)].Value.ToString();
                        string sRegion = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_AREA1)].Value.ToString();
                        string sProvince = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_CITY)].Value.ToString();
                        string sRequestType = dataGridView.Rows[i].Cells[GetMapColumnIndex(clsDefines.IR_REQUEST_TYPE)].Value.ToString().ToUpper();

                        // Validate Request ID
                        if (isValidRequestType(sRequestType))
                        {
                            // do nothing
                        }
                        else
                        {
                            isValid = false;

                            sTemp += "> Line# : " + iLineNo + Environment.NewLine +
                                     "> Merchant Name : " + sMerchantName + Environment.NewLine +
                                     "> TID : " + sTID + Environment.NewLine +
                                     "> MID : " + sMID + Environment.NewLine +
                                     "> Request ID : " + sIRNo + Environment.NewLine +
                                     "> Request Type : " + sRequestType + Environment.NewLine +
                                     sSingleLineSeparator + Environment.NewLine;
                        }

                    }

                    sHint = $"Invalid request type found.{Environment.NewLine}Valid request type are [{clsSystemSetting.ClassSystemValidRequestType}]";
                    break;
                default:
                    break;
            }

            if (!isValid)
            {
                sMessage = clsDefines.FIELD_CHECK_MSG;
                frmPromptMessage.sMenuHeader = sMessage;
                frmPromptMessage.sMessage = sTemp;

                frmPromptMessage.sHintMessage = "";
                frmPromptMessage.sHint = sHint + Environment.NewLine + Environment.NewLine + "Please check import file.";
                frmPromptMessage frm = new frmPromptMessage();
                frm.ShowDialog();

            }

            if (isBypass)
            {
                isOutput = true;
            }
            else
            {
                if (!isValid)
                    isOutput = false;
                else
                    isOutput = true;
            }

            Debug.WriteLine("isValidDataGridValue, isBypass=" + isBypass + ",isOutput=" + isOutput + ",isValid=" + isValid);

            Cursor.Current = Cursors.Default;

            return isOutput;
        }

        public void SetButtonIconImage(Bunifu.Framework.UI.BunifuImageButton obj)
        {
            string objName = obj.Name;
            bool isEnable = obj.Enabled;

            switch (objName)
            {
                case "btnSearchCurTerminal":
                case "btnSearchCurSIM":
                case "btnSearchRepTerminal":
                case "btnSearchRepSIM":
                case "btnSearchClient":
                case "btnSearchFE":
                case "btnSearchDispatcher":
                case "btnSearchMobile":
                case "btnSearchTerminalSN":
                case "btnSearchSIMSN":
                case "btnSearchStock":
                case "btnSearchReason":
                    obj.Image = (isEnable ? Properties.Resources.search_on : Properties.Resources.search_off);
                    break;
                case "btnRemoveCurTerminal":
                case "btnRemoveCurSIM":
                case "btnRemoveRepTerminal":
                case "btnRemoveRepSIM":
                case "btnRemoveClient":
                case "btnRemoveFE":
                case "btnRemoveDispatcher":
                case "btnRemoveStock":
                    obj.Image = (isEnable ? Properties.Resources.remove_on : Properties.Resources.remove_off);
                    break;
                case "btnSearchMerchant":
                case "btnSearchService":
                case "btnFSRSearch":
                case "btnMerchantSearch":
                case "btnMerchantListSearch":
                case "btnClientSearch":
                case "btnSearchMSP":
                case "btnProvinceSearch":
                case "btnSearchSales":
                case "btnSearch":                
                    obj.Image = (isEnable ? Properties.Resources.find_on : Properties.Resources.find_off);
                    break;
                case "btnNoRequestID":
                case "btnNoReferenceNo":
                    obj.Image = (isEnable ? Properties.Resources.ic_edit_on : Properties.Resources.ic_edit_off);
                    break;
                case "btnUpdateMerchRep":
                    obj.Image = (isEnable ? Properties.Resources.ic_save : Properties.Resources.ic_save);
                    break;
                case "btnSearchAssistNo":
                    obj.Image = (isEnable ? Properties.Resources.find_on_1 : Properties.Resources.find_off);
                    break;
                default:
                    Debug.WriteLine("SetButtonIconImage, Object=" + objName + " not defined.");
                    MessageBox.Show("Object " + objName + " undefined", "SetButtonIconImage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

        }

        public void DataGridViewAddLineNo(object sender, DataGridViewRowPostPaintEventArgs e, Font font)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerformat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, font, SystemBrushes.ControlText, headerBounds, centerformat);
        }

        public string AddBracketStartEnd(string pString)
        {
            string pOutput = "";

            pOutput = "[" + pString + "]";

            return pOutput;
        }

        public string AddBracesStartEnd(string pString)
        {
            string pOutput = "";

            pOutput = "{" + pString + "}";

            return pOutput;
        }

        public string AddParenthesisStartEnd(string pString)
        {
            string pOutput = "";

            pOutput = "(" + pString + ")";

            return pOutput;
        }

        public byte[] streamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        public ListView updateListView(ListView obj, int pColIndex, string pColValue, bool isCheck)
        {

            if (obj.Items.Count > 0)
            {
                if (isCheck)
                {
                    foreach (ListViewItem i in obj.Items)
                    {
                        if (i.Checked)
                        {
                            i.SubItems[pColIndex].Text = pColValue;
                        }
                    }
                }
                else
                {
                    foreach (ListViewItem i in obj.Items)
                    {
                        i.SubItems[pColIndex].Text = pColValue;
                    }
                }

            }

            return obj;
        }

        public bool isValidListViewChecked(ListView obj)
        {
            bool isValid = false;

            if (obj.Items.Count > 0)
            {
                foreach (ListViewItem i in obj.Items)
                {
                    if (i.Checked)
                    {
                        isValid = true;
                        break;
                    }
                }
            }
            else
            {
                isValid = false;
            }

            if (!isValid)
            {
                SetMessageBox("One or more must be checked on the list.", clsDefines.FIELD_CHECK_MSG, IconType.iError);
            }

            return isValid;
        }

        public void updateSytemDateFormat()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\International", true);

            key.SetValue("sShortDate", "MM-dd-yyyy");
            key.SetValue("sLongDate", "dddd, MMMM-dd-yyyy");

        }

        public void checkAllListView(ListView lvwObj, CheckBox chkObj)
        {
            if (lvwObj.Items.Count > 0)
            {
                if (chkObj.Checked)
                {
                    chkObj.Text = "UNCHECK ALL";

                    // Check All
                    foreach (ListViewItem i in lvwObj.Items)
                    {
                        i.Checked = true;
                    }
                }
                else
                {
                    chkObj.Text = "CHECK ALL";

                    // UnCheck All
                    foreach (ListViewItem i in lvwObj.Items)
                    {
                        i.Checked = false;
                    }
                }
            }
        }

        public void removeItemListView(ListView lvw, bool isCheckEnable)
        {
            if (lvw.SelectedItems.Count > 0)
            {
                if (isCheckEnable)
                {
                    ListView.CheckedIndexCollection checkedItems = lvw.CheckedIndices;

                    while (checkedItems.Count > 0)
                    {
                        lvw.Items.RemoveAt(checkedItems[0]);
                    }
                }
                else
                {
                    lvw.SelectedItems[0].Remove();
                }

                UpdateListViewLineNo(lvw); // Update ListView LineNo
            }
        }

        // ROCKY - FUNCTION ENHANCEMENT: ADD FUNCTION FOR REMOVAL OF SPECIAL CHARACTERS
        public static string FormatCharAndDate(string input)
        {
            StringBuilder sb = new StringBuilder(input);

            Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                { "ñ", "n" },
                { "Ñ", "N" },
                { "\t", "" },
                { "\n", "" },
                { "\r", "" },
                { " ", "" },
                { ",", "" },
                { "'", "" },
                { ";", "" },
                { "\"", "" }
            };

            foreach (var replacement in replacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            try
            {
                if (DateTime.TryParse(input, out DateTime tempDate))
                {
                    return tempDate.ToString(sValueDateFormat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occured : {ex}", "Format Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return sb.ToString();
        }

        public bool isValidVersion()
        {
            bool isValid = false;
            bool isDeployed = ApplicationDeployment.IsNetworkDeployed;

            //isDeployed = true;
            //clsSearch.ClassisAppVersion = true;

            Debug.WriteLine("--isValidVersion--");
            Debug.WriteLine("isDeployed="+ isDeployed);
            Debug.WriteLine("isDeployed=" + clsSearch.ClassisAppVersion);

            if (isDeployed && clsSearch.ClassisAppVersion)
            {
                string sLocalVersion = clsSystemSetting.ClassSystemLocalPublishVersion.Replace("v", clsFunction.sNull);

                Debug.WriteLine("Current version=" + sLocalVersion);
                Debug.WriteLine("Latest version=" + clsSystemSetting.ClassSystemPublishVersion);

                dbFile.WriteAPILog(0, "Version checking->" + $"Bank:{clsSearch.ClassBankName}|Current: {sLocalVersion}|Latest:{clsSystemSetting.ClassSystemPublishVersion}");

                if (!sLocalVersion.Equals(clsSystemSetting.ClassSystemPublishVersion))
                {
                    MessageBox.Show("A new version of " + AddBracketStartEnd(clsSystemSetting.ClassApplicationName) + " is now available." + "\n" + "Please update to continue using this application." + 
                        "\n\n" +
                        "Version details below:" + "\n" +
                        " > Bank: " + clsSearch.ClassBankName + "\n" +
                        " > Current: " + sLocalVersion + "\n" +
                        " > Latest: " + clsSystemSetting.ClassSystemPublishVersion + "\n\n" + "Application will be closed after this version checking.", "Version check", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    Application.Exit();
                }
                else
                {
                    isValid = true;
                }
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

        public static void setServiceResultBackColor(TextBox obj, string pStatus)
        {
            if (pStatus.Equals(clsGlobalVariables.ACTION_MADE_SUCCESS))
                obj.BackColor = Color.Green;
            else if (pStatus.Equals(clsGlobalVariables.ACTION_MADE_NEGATIVE))
                obj.BackColor = Color.Red;
            else
                obj.BackColor = Color.Orange;
        }
        
        public static async Task LoadImageFromURLAsync(string pURL, string pRemotePath, PictureBox obj,
                                               int index, string pServiceNo, bool isSignature)
        {
            Debug.WriteLine("--LoadImageFromURLAsync--");

            clsFunction dbFunction = new clsFunction();
            string pFileName = pServiceNo + "_" + dbFunction.padLeftChar(index.ToString(), "0", 2)
                               + (isSignature ? ".png" : ".jpg");

            string sURL = pURL + pRemotePath + pFileName;
            Debug.WriteLine("sURL=" + sURL);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                try
                {
                    byte[] data = null;

                    // Simple retry (2 attempts)
                    for (int attempt = 1; attempt <= 2; attempt++)
                    {
                        try
                        {
                            data = await client.GetByteArrayAsync(sURL);
                            break; // success
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Attempt {attempt} failed for {sURL}: {ex.Message}");
                            if (attempt == 2) throw; // fail after 2nd try
                        }
                    }

                    Image img = null;
                    using (var ms = new MemoryStream(data))
                    using (var tempImg = Image.FromStream(ms))
                    {
                        img = new Bitmap(tempImg); // clone = safe for UI
                    }

                    if (obj.IsHandleCreated && !obj.IsDisposed)
                    {
                        obj.BeginInvoke((MethodInvoker)(() => obj.Image = img));
                    }
                }
                catch (Exception ex)
                {
                    if (obj.IsHandleCreated && !obj.IsDisposed)
                    {
                        obj.BeginInvoke((MethodInvoker)(() => obj.Image = null));
                    }
                    Debug.WriteLine("Error loading image: " + ex.Message);
                }
            }
        }


        public string BeautifyJson(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);

            string formattedJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            return formattedJson;
        }

        public void setDoubleBuffer(Control ctl, bool DoubleBuffered)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, ctl, new object[] { DoubleBuffered });
        }

        public void eDiagnosticReport(int ReportID)
        {

            Debug.WriteLine("--eDiagnosticReport--");

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = clsSearch.ClassReportDescription = "DIAGNOSTIC REPORT";

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassFSRNo + clsFunction.sPipe + clsSearch.ClassServiceNo;

            Debug.WriteLine("eDiagnosticReport::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            try
            {
                clsSearch.ClassReportID = ReportID;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "eFSR Diagnostic";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceDetail";
                ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public void eFSRReport(int ReportID)
        {

            Debug.WriteLine("--eFSRReport--");

            Cursor.Current = Cursors.WaitCursor;  // Waiting / Hour Glass

            clsReport.ClassReportDesc = clsSearch.ClassReportDescription = "FSR REPORT";

            clsSearch.ClassAdvanceSearchValue = clsSearch.ClassIRNo + clsFunction.sPipe + clsSearch.ClassServiceNo + clsFunction.sPipe + clsSearch.ClassFSRNo + clsFunction.sPipe + clsSearch.ClassIRIDNo;

            Debug.WriteLine("eFSRReport::" + "clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);

            try
            {
                clsSearch.ClassReportID = ReportID;
                clsSearch.ClassStatementType = "View";
                clsSearch.ClassSearchBy = "FSR";
                clsSearch.ClassSearchValue = clsSearch.ClassAdvanceSearchValue;
                clsSearch.ClassStoredProcedureName = "spViewAdvanceTA";
                ProcessReport(clsSearch.ClassReportID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Cursor.Current = Cursors.Default;  // Back to normal 
        }

        public string convertIpToHostName(string pIPAddress, bool isSecured)
        {
            string pOutput = "";

            IPHostEntry hostEntry = Dns.GetHostEntry(pIPAddress);

            pOutput = (isSecured ? "https" : "http") + "://" + hostEntry.HostName;

            Debug.WriteLine("convertIpToHostName, pOutput="+ pOutput);

            return pOutput;
        }

        public bool isValidIRNoPrefix(string pSearch)
        {
            //Debug.WriteLine("--isValidIRNoPrefix--");

            bool isValid = false;

            // delimeted string
            string delimitedString = clsSystemSetting.ClassSystemIRNoPrefix;

            // split the string
            //string[] prefixes = { "DR", "MR", "SR", "IR" };
            string[] prefixes = delimitedString.Split(clsDefines.gComma);

            //Debug.WriteLine("pSearch=" + pSearch);

            // Print the result to verify
            /*
            foreach (string value in prefixes)
            {
                Debug.WriteLine("value=" + value);
            }
            */

            string pattern = $"({string.Join("|", prefixes.Select(Regex.Escape))})";

            //Debug.WriteLine("--isValidIRNoPrefix--");
            //Debug.WriteLine("pSearch=" + pSearch + ",pattern=" + pattern);

            if (Regex.IsMatch(pSearch, pattern))
            {
                isValid = true;
                //Console.WriteLine($"The string '{pSearch}' starts with one of the prefixes: {string.Join(", ", prefixes)}.");

            }
            else
            {
                isValid = false;
                //Console.WriteLine($"The string '{pSearch}' does not start with any of the specified prefixes.");

            }

            //Debug.WriteLine("isValid=" + isValid);

            return isValid;
        }

        private ToolTip toolTip;
        public void setToolTip(Control obj, string pMessage)
        {
            toolTip = new ToolTip();
            toolTip.SetToolTip(obj, pMessage);
        }

        public bool fPromptConfirmation(string pMessage)
        {
            bool fConfirm = true;

            if (MessageBox.Show(pMessage +
                                    "\n\n",
                                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        public bool ShowMenuInputBox(string pTitle, string pMessage, string pDescription, int pMaxLimit, int pOptionType, ref string pOutput)
        {
            InputBox.iInputType = AlphaNumeric_Input;
            InputBox.iInputLimitSize = 500;
            InputBoxResult MenuNum = InputBox.Show(pMessage, pTitle, pDescription, 100, 0, pMaxLimit, pOptionType);

            if (MenuNum.ReturnCode == DialogResult.OK)
            {
                pOutput = MenuNum.Text;
                return true;
            }

            if (MenuNum.ReturnCode == DialogResult.No || MenuNum.ReturnCode == DialogResult.Cancel)
            {
                pOutput = "";
                return false;
            }

            return false;
        }

        public string GetSearchValue(string pSearchKey)
        {
            string pValue = sDash;
            int i = 0;

            while (clsArray.SearchKey.Length > i)
            {
                if (pSearchKey.ToUpper().Equals(clsArray.SearchKey[i].ToUpper()))
                {
                    pValue = clsArray.SearchValue[i];
                    break;
                }
                i++;
            }

            return pValue;
        }

        public ListView updateListViewByColRow(ListView obj, int pColIndex, int pRowIndex, string pColValue)
        {
            int x = 0;
            if (obj.Items.Count > 0)
            {
                foreach (ListViewItem i in obj.Items)
                {
                    x++;

                    if (x == pRowIndex)
                    {
                        i.SubItems[pColIndex].Text = pColValue;
                        break;
                    }
                }
            }

            return obj;
        }

        public string EncryptMD5(string pValue)
        {
            string pOutput = "";

            // Create an instance of MD5 hash algorithm
            using (MD5 md5 = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] inputBytes = Encoding.UTF8.GetBytes(pValue);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" means hexadecimal with two digits
                }

                pOutput = sb.ToString().ToUpper();

                Console.WriteLine("Input string: " + pValue);
                Console.WriteLine("MD5 hash: " + pOutput);
            }

            return pOutput;
        }

        public string getCurrentDateTime()
        {
            string pOutput = "0000-00-00 00:00:00";
            DateTime CurrentDateTime = DateTime.Now;

            pOutput = CurrentDateTime.ToString("yyyy-MM-dd H:mm:ss");

            return pOutput;
        }

        public string getCurrentDate()
        {
            string pOutput = "0000-00-00";
            DateTime CurrentDateTime = DateTime.Now;

            pOutput = CurrentDateTime.ToString("yyyy-MM-dd");

            return pOutput;
        }

        public string getCurrentTime()
        {
            string pOutput = "00:00:00";
            DateTime CurrentDateTime = DateTime.Now;

            pOutput = CurrentDateTime.ToString("H:mm:ss");

            return pOutput;
        }

        public bool isValidSheetName(string pValue)
        {
            bool isValid = false;

            if (pValue.Equals(clsDefines.VALID_SHEET_NAME))
                isValid = true;

            if (!isValid)
            {
                SetMessageBox(AddBracketStartEnd(pValue) + " is not valid sheet name." + "\n\n" + "Valid sheet name must be " + AddBracketStartEnd(clsDefines.VALID_SHEET_NAME), "Field checking", IconType.iError);
            }

            return isValid;

        }

        public string getSheetName(string pFile)
        {
            string pOutput = "";

            // Load the Excel workbook using Spire.Xls
            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromFile(pFile);

            try
            {
                // Loop through all sheets in the workbook
                foreach (Spire.Xls.Worksheet sheet in workbook.Worksheets)
                {
                    // Append the name of each sheet to the output string
                    pOutput = sheet.Name;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Dispose of the workbook object to release resources
                workbook.Dispose();
            }

            return pOutput;
        }

        public string setIntegerToYesNoString(int pValue)
        {
            string sTemp = clsFunction.sNull;

            if (pValue > 0)
                sTemp = "YES";
            else
                sTemp = "NO";

            return sTemp;
        }

        public void Sleep(int seconds)
        {
            seconds = seconds * 1000;
            Thread.Sleep(seconds);
        }

        public bool isValidDigitalFSRMode(string pFSRMode, bool isPrompt)
        {
            bool isValid = false;

            if (pFSRMode.Equals(clsDefines.DIGITAL_FSR))
                isValid = true;

            if (!isValid && isPrompt)
            {
                SetMessageBox("FSR Mode is not " + AddBracketStartEnd(clsDefines.DIGITAL_FSR), "Field Check", IconType.iError);
            }

            return isValid;
        }

        public string genJSONFormat(object obj, int pRowSelected, string token = "", string nestedObject = "")
        {
            try
            {
                if (obj is DataGridView dgv)
                {
                    return ConvertDataGridViewToJson(dgv, pRowSelected, token, nestedObject);
                }
                else if (obj is ListView lv)
                {
                    return ConvertListViewToJson(lv, pRowSelected, token, nestedObject);
                }
                else
                {
                    throw new ArgumentException("Unsupported object type. Use DataGridView or ListView.");
                }
            }
            catch (Exception ex)
            {
                // ✅ log (recommended)
                Console.WriteLine("Error in genJSONFormat: " + ex.Message);

                // optional: show message (UI apps)
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // return safe fallback
                return string.Empty;
            }
        }

        public string ConvertDataGridViewToJson(DataGridView dgv, int pRowSelected, string token = "", string nestedObject = "")
        {
            try
            {
                List<Dictionary<string, object>> rowDataList = new List<Dictionary<string, object>>();
                int rowIndex = 0;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    // if specific row selected, reset list
                    if (pRowSelected >= 0 && rowIndex == pRowSelected)
                    {
                        rowDataList.Clear();
                    }

                    Dictionary<string, object> rowData = new Dictionary<string, object>();

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var column = cell.OwningColumn;

                        // ✅ FIX: fallback to Name if HeaderText is empty
                        string columnTag = column.HeaderText;
                        if (string.IsNullOrWhiteSpace(columnTag))
                        {
                            columnTag = column.Name;
                        }

                        columnTag = Regex.Replace(columnTag.Trim(), @"\r\n?|\n", "");

                        // ✅ value handling
                        object cellValue = (cell.Value != null && !cell.Value.Equals("null"))
                            ? cell.Value
                            : "-";

                        string cleanValue = Regex.Replace(cellValue.ToString(), @"\r\n?|\n", "");
                        cleanValue = CheckAndSetStringValue(cleanValue);

                        // ✅ prevent duplicate keys (important)
                        if (rowData.ContainsKey(columnTag))
                        {
                            columnTag = $"{columnTag}_{cell.ColumnIndex}";
                        }

                        rowData.Add(columnTag, cleanValue);

                        //Debug.WriteLine($"Column Header: '{columnTag}'");
                    }

                    rowDataList.Add(rowData);

                    // stop if only one row needed
                    if (pRowSelected >= 0 && rowIndex == pRowSelected)
                    {
                        break;
                    }

                    rowIndex++;
                }

                // ✅ result shaping
                object jsonResult = (rowDataList.Count == 1)
                    ? (object)rowDataList[0]
                    : rowDataList;

                // ✅ wrap with token / nested object if needed
                if (!string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(nestedObject))
                {
                    var jsonObject = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(token))
                    {
                        jsonObject["token"] = token;
                    }

                    if (!string.IsNullOrEmpty(nestedObject))
                    {
                        jsonObject[nestedObject] = jsonResult;
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
                    }

                    string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    //Debug.WriteLine("JSON DataGridView=" + jsonString);
                    return jsonString;
                }

                return JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ConvertDataGridViewToJson ERROR] {ex}");
                return string.Empty;
            }
        }

        public string ConvertListViewToJson(ListView lv, int pRowSelected, string token = "", string nestedObject = "")
        {
            try
            {
                List<Dictionary<string, object>> rowDataList = new List<Dictionary<string, object>>();

                for (int rowIndex = 0; rowIndex < lv.Items.Count; rowIndex++)
                {
                    if (pRowSelected >= 0 && rowIndex != pRowSelected)
                        continue;

                    ListViewItem item = lv.Items[rowIndex];
                    Dictionary<string, object> rowData = new Dictionary<string, object>();

                    for (int i = 0; i < item.SubItems.Count; i++)
                    {
                        // ✅ FIX: fallback if column header is empty
                        string columnName = (i < lv.Columns.Count)
                            ? lv.Columns[i].Text
                            : $"Column{i + 1}";

                        if (string.IsNullOrWhiteSpace(columnName))
                        {
                            columnName = $"Column{i + 1}";
                        }

                        columnName = Regex.Replace(columnName.Trim(), @"\r\n?|\n", "");

                        // ✅ value handling
                        string rawValue = item.SubItems[i].Text;
                        string cellValue = !string.IsNullOrWhiteSpace(rawValue) ? rawValue : "-";

                        cellValue = Regex.Replace(cellValue, @"\r\n?|\n", "");
                        cellValue = CheckAndSetStringValue(cellValue);

                        // ✅ prevent duplicate keys
                        if (rowData.ContainsKey(columnName))
                        {
                            columnName = $"{columnName}_{i}";
                        }

                        rowData.Add(columnName, cellValue);

                        Debug.WriteLine($"Column Header: '{columnName}'");
                    }

                    rowDataList.Add(rowData);

                    if (pRowSelected >= 0)
                        break;
                }

                // ✅ result shaping
                object jsonResult = (rowDataList.Count == 1)
                    ? (object)rowDataList[0]
                    : rowDataList;

                // ✅ wrapper (token / nestedObject)
                if (!string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(nestedObject))
                {
                    var jsonObject = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(token))
                    {
                        jsonObject["token"] = token;
                    }

                    if (!string.IsNullOrEmpty(nestedObject))
                    {
                        jsonObject[nestedObject] = jsonResult;
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
                    }

                    string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    Debug.WriteLine("JSON ListView=" + jsonString);
                    return jsonString;
                }

                return JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ConvertListViewToJson ERROR] {ex}");
                return string.Empty;
            }
        }
        
        public bool isValidLen(string sValue)
        {
            bool fValid = true;

            if (sValue.Length <= 0)
                fValid = false;

            if (sValue.CompareTo(sZero) == 0 || sValue.CompareTo(sNull) == 0 || sValue.CompareTo(sDash) == 0)
                fValid = false;

            return fValid;

        }

        public void initTabSelection(TabControl obj, int index)
        {
            obj.SelectedIndex = index;
        }

        // User Control
        public void ucStatusInit(ucStatus obj, int pState, int pMin, int pMax, int pTextAlignment)
        {
            obj.iState = pState;
            obj.iMin = pMin;
            obj.iMax = pMax;
        }

        public void ucStatusMessage(ucStatus obj, string pMessage)
        {
            obj.sMessage = pMessage;
            obj.AnimateStatus();
        }

        public string getDataGridRowDataInJSON(DataGridView obj, int pRow)
        {
            string pOutput = "";
            int rowIndex = 0;
            Debug.WriteLine("--getDataGridRowDataInJSON--");

            List<DataGridViewRowData> rowDataList = new List<DataGridViewRowData>();

            foreach (DataGridViewRow row in obj.Rows)
            {
                DataGridViewRowData rowData = new DataGridViewRowData();
                rowData.Values = new Dictionary<string, object>();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    string columnName = cell.OwningColumn.Name;
                    object cellValue = ((cell.Value != null || cell.Value.Equals("null")) ? cell.Value : clsFunction.sDash);
                    rowData.Values.Add(columnName, cellValue);
                }

                rowDataList.Add(rowData);

                string jsonString = JsonConvert.SerializeObject(rowData, Formatting.Indented);
                Debug.WriteLine("jsonString=" + jsonString);

                if (rowIndex == pRow)
                {
                    pOutput = jsonString;
                    break;
                }

                rowIndex++;
            }

            return pOutput;

        }

        public void iniComboBoxDefaultSelect(Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is ComboBox)
                    {
                        //(control as ComboBox).Items.Clear();
                        (control as ComboBox).Text = sDefaultSelect;
                    }
                    else
                    {
                        func(control.Controls);
                    }
            };

            func(obj.Controls);

        }

        public string CheckAndSetDate(string pValue)
        {
            string pOutput = clsDefines.DEV_DATE;

            if (!pValue.Equals(sDateFormat) && !pValue.Equals(sDash) && pValue.Length > 0)
                pOutput = pValue;

            return pOutput;
        }

        public void onFormMove(object sender, EventArgs e)
        {
            // Cast the sender to a Form object
            var form = sender as Form;

            // Get the screen where the form is currently located
            var screen = Screen.FromControl(form);

            // If the form is maximized, adjust its bounds to fit the new screen
            if (form.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Normal; // Set to normal before resizing
                form.Bounds = screen.WorkingArea; // Adjust to the new screen's working area
                //this.WindowState = FormWindowState.Maximized; // Re-maximize it on the correct screen
            }
        }

        public void handleForm(Form frm)
        {
            // Check if the form is already open using the form's specific type
            Form openForm = Application.OpenForms.OfType<Form>()
                              .FirstOrDefault(f => f.GetType() == frm.GetType());

            if (openForm != null)
            {
                // If it's already open, bring it to the front
                openForm.BringToFront();
                openForm.WindowState = FormWindowState.Normal; // In case it was minimized

                // Call OnLoad manually to simulate the form loading logic
                openForm.GetType().InvokeMember("OnLoad",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, openForm, new object[] { EventArgs.Empty });

            }
            else
            {
                // If it's not open, show the passed form
                frm.Show();
            }
        }

        public void getCurrentFirstAndLastDate(ref string refFirstdate, ref string refLastdate)
        {
            DateTime CurrentDateTime = DateTime.Now;

            // Get the first day of the current month
            DateTime firstDate = new DateTime(CurrentDateTime.Year, CurrentDateTime.Month, 1);

            // Get the last day of the current month
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);

            refFirstdate = firstDate.ToString("yyyy-MM-dd");
            refLastdate = lastDate.ToString("yyyy-MM-dd");
        }

        public int getFileID(ComboBox obj, string pSearchBy)
        {
            int pOutput = 0;

            GetIDFromFile(pSearchBy, obj.Text);
            pOutput = clsSearch.ClassOutFileID;

            return pOutput;
        }

        public bool isValidDescriptionEntry(string sValue, string sMessage)
        {
            bool fValid = false;
            
            if (isValidDescription(sValue))
            {
                fValid = true;
            }
            
            if (!fValid)
            {
                SetMessageBox(sMessage, "Warning", IconType.iExclamation);
            }

            return fValid;
        }

        public void populateListViewFromDataSet(ListView listView, DataSet dataSet, string excludeFilePath)
        {
            if (dataSet.Tables.Count == 0) return;

            DataTable table = dataSet.Tables[0];

            // Clear existing data
            listView.Items.Clear();
            listView.Columns.Clear();

            // Add line number column
            listView.Columns.Add("Line#");

            // Read and parse the exclusion file
            HashSet<string> excludedColumnsSet = dbFile.loadExcludedColumns(excludeFilePath);

            // Keep track of columns to display
            var visibleColumns = new List<int>();
            
            // Add columns dynamically, skipping those in exclusion lists
            foreach (DataColumn column in table.Columns)
            {
                if (!excludedColumnsSet.Contains(column.ColumnName))
                {
                    listView.Columns.Add(column.ColumnName);
                    visibleColumns.Add(column.Ordinal);
                }
            }

            // Add rows
            int lineNumber = 1;
            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(lineNumber.ToString());

                foreach (int columnIndex in visibleColumns)
                {
                    item.SubItems.Add(row[columnIndex]?.ToString() ?? string.Empty);
                }

                listView.Items.Add(item);

                if (lineNumber >= clsSystemSetting.ClassSystemPageLimit) break;

                lineNumber++;
            }

            // Adjust column widths
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = -2; // Auto-resize to fit content
            }

            listView.View = View.Details;
        }

        public void searchListView(ListView listView, string searchText)
        {
            // Loop through all the items in the ListView
            foreach (ListViewItem item in listView.Items)
            {
                bool isMatch = false;

                // Loop through all the subitems (columns) in the current item
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    // Check if the subitem text contains the search text (case-insensitive)
                    if (subItem.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        isMatch = true;
                        break; // If we found a match, no need to continue searching in other columns
                    }
                }

                // Hide or show the item based on whether it matches the search text
                item.ForeColor = isMatch ? Color.Black : Color.Transparent; // Change the item color for visibility (hide by setting transparent color)
            }
        }

        public string convertToJson(Dictionary<string, string> tagValueMap)
        {
            // Serialize the dictionary to JSON string
            string jsonString = JsonConvert.SerializeObject(tagValueMap);

            // Escape the double quotes inside the JSON string
            //return jsonString.Replace("\"", "\\\"");

            return jsonString;
        }

        public bool checkDateFromTo(DateTime objFrom, DateTime objTo)
        {
            bool fValid = true;
            int iResult;

            Debug.WriteLine("--checkDateFromTo--");
            Debug.WriteLine("objFrom=" + objFrom.ToString());
            Debug.WriteLine("objTo=" + objTo.ToString());

            iResult = DateTime.Compare(DateTime.Parse(objFrom.ToShortDateString()), DateTime.Parse(objTo.ToShortDateString()));

            Debug.WriteLine("iResult=" + iResult);

            if (iResult > 0)
                fValid = false;
            
            return fValid;
        }

        public string getJSONTagValue(string jsonString, string tag, string token = "", string nestedObject = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                    return "Error: JSON string is empty or null";

                //Debug.WriteLine($"--getJSONTagValue--\njsonString={jsonString}\ntag={tag}\ntoken={token}\nnestedObject={nestedObject}");

                JObject jsonObj;
                try
                {
                    jsonObj = JObject.Parse(jsonString);
                }
                catch (JsonException ex)
                {
                    return $"Error: Invalid JSON format - {ex.Message}";
                }

                JToken value = null;

                // If a token is provided, search within the specified token
                if (!string.IsNullOrEmpty(token))
                {
                    if (!jsonObj.ContainsKey(token))
                        return "Token not found";

                    JToken tokenObj = jsonObj[token];

                    if (!string.IsNullOrEmpty(nestedObject) && tokenObj[nestedObject] != null)
                    {
                        value = tokenObj[nestedObject][tag];
                    }
                    else
                    {
                        value = tokenObj[tag];
                    }
                }
                else
                {
                    // No token provided, check root level first
                    if (jsonObj.ContainsKey(tag))
                    {
                        value = jsonObj[tag];
                    }
                    else
                    {
                        // Check inside first nested object if no token is provided
                        JProperty firstProperty = jsonObj.Properties().FirstOrDefault();
                        if (firstProperty != null && firstProperty.Value is JObject innerObj)
                        {
                            if (innerObj.ContainsKey(tag))
                            {
                                value = innerObj[tag];
                            }
                            else
                            {
                                // Case-insensitive search inside the first nested object
                                value = innerObj.Properties()
                                               .FirstOrDefault(p => p.Name.Equals(tag, StringComparison.OrdinalIgnoreCase))
                                               ?.Value;
                            }
                        }
                    }
                }

                return value?.ToString() ?? "Tag not found";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        public void ClearRichTextBox(Form obj)
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is RichTextBox)
                        (control as RichTextBox).Clear();
                    else
                        func(control.Controls);
            };

            func(obj.Controls);
        }

        public void populateListViewFromJsonString(Control control, string jsonString, string rootKey, string nestedObject = "")
        {
            try
            {
                // Parse JSON
                var jsonObject = JObject.Parse(jsonString);
                Dictionary<string, string> values = null;

                // Handle different cases for rootKey and nestedObject
                if (!string.IsNullOrEmpty(rootKey) && jsonObject[rootKey] != null)
                {
                    // If nestedObject exists inside rootKey
                    if (!string.IsNullOrEmpty(nestedObject) && jsonObject[rootKey]?[nestedObject] != null)
                        values = jsonObject[rootKey][nestedObject].ToObject<Dictionary<string, string>>();
                    else
                        values = jsonObject[rootKey].ToObject<Dictionary<string, string>>();
                }
                else if (string.IsNullOrEmpty(rootKey)) // No root key provided
                {
                    if (!string.IsNullOrEmpty(nestedObject) && jsonObject[nestedObject] != null)
                        values = jsonObject[nestedObject].ToObject<Dictionary<string, string>>();
                    else
                        values = jsonObject.ToObject<Dictionary<string, string>>(); // Use entire JSON object
                }

                // If values is still null, show an error
                if (values == null)
                {
                    Debug.WriteLine($"Key is invalid\n\n rootKey:[{rootKey}]\nestedObject:[{nestedObject}]\nJSON String:[{jsonString}]");
                    MessageBox.Show("Key is invalid." + "\n\n" +
                        "rootKey: " + rootKey + "\n" +
                        "nestedObject: " + nestedObject + "\n" +
                        "JSON String: " + jsonString, 
                        "Error: populateListViewFromJsonString", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Handle ListView population
                if (control is ListView lvw)
                {
                    // Configure ListView
                    lvw.View = View.Details;
                    lvw.Columns.Clear();
                    lvw.Columns.Add("LINE#", 60, HorizontalAlignment.Left);
                    lvw.Columns.Add("TAG", 210, HorizontalAlignment.Left);
                    lvw.Columns.Add("VALUE", 600, HorizontalAlignment.Left);

                    lvw.Items.Clear();
                    int lineNumber = 1;

                    // Populate ListView
                    foreach (var kvp in values)
                    {
                        ListViewItem item = new ListViewItem(lineNumber.ToString());
                        item.SubItems.Add(kvp.Key);
                        item.SubItems.Add(kvp.Value);
                        lvw.Items.Add(item);
                        lineNumber++;
                    }

                    //ListViewAlternateBackColor(lvw);
                }
                // Handle DataGridView population
                else if (control is DataGridView dgv)
                {
                    dgv.Columns.Clear();
                    dgv.Rows.Clear();
                    dgv.ColumnCount = 3;

                    dgv.Columns[0].Name = "LINE#";
                    dgv.Columns[0].Width = 60;
                    dgv.Columns[1].Name = "TAG";
                    dgv.Columns[1].Width = 210;
                    dgv.Columns[2].Name = "VALUE";
                    dgv.Columns[2].Width = 600;

                    int lineNumber = 1;

                    foreach (var kvp in values)
                    {
                        dgv.Rows.Add(lineNumber.ToString(), kvp.Key, kvp.Value);
                        lineNumber++;
                    }

                    //DataGridViewAlternateBackColor(dgv);
                }
                else
                {
                    throw new ArgumentException("Unsupported control type. Use ListView or DataGridView.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}" + "\n\n" +
                    "JSON String: " + jsonString, "Error: populateListViewFromJsonString", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public string getFSRMode(int pMobileID, int pFSRNo, int pServiceNo)
        {
            string output = "";
            
            if (pFSRNo > 0)
            {
                output = (pMobileID > 0 ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);
            }
            else
            {
                output = clsDefines.gNull;
            }

            return output;
        }

        public bool isAlreadyCompleted(string pStatus)
        {
            bool isValid = false;

            if (pStatus.Equals(clsDefines.SERVICE_STATUS_COMPLETED))
                isValid = true;

            if (isValid)
            {
                SetMessageBox("This service has already been marked as 'Completed' and cannot be updated.", "Action not allowed", IconType.iError);
            }

            return isValid;
        }

        public bool isValidRequestType(string pSearch)
        {
            Debug.WriteLine("--isValidRequestType--");

            bool isValid = false;

            // delimeted string
            string delimitedString = clsSystemSetting.ClassSystemValidRequestType;

            // split the string
            //string[] prefixes = { "DR", "MR", "SR", "IR" };
            string[] prefixes = delimitedString.Split(clsDefines.gComma);

            Debug.WriteLine("pSearch=" + pSearch);

            // Print the result to verify
            foreach (string value in prefixes)
            {
                Debug.WriteLine("value=" + value);
            }

            string pattern = $"({string.Join("|", prefixes.Select(Regex.Escape))})";

            Debug.WriteLine("--isValidRequestType--");
            Debug.WriteLine("pSearch=" + pSearch + ",pattern=" + pattern);

            if (Regex.IsMatch(pSearch, pattern))
            {
                isValid = true;
                Console.WriteLine($"The string '{pSearch}' starts with one of the prefixes: {string.Join(", ", prefixes)}.");

            }
            else
            {
                isValid = false;
                Console.WriteLine($"The string '{pSearch}' does not start with any of the specified prefixes.");

            }

            Debug.WriteLine("isValid=" + isValid);

            return isValid;
        }

        public void debugDataSet(DataSet dataSet, int rowLimitToCheck)
        {
            if (dataSet == null)
            {
                Debug.WriteLine("dataSet is null");
            }
            else if (dataSet.Tables.Count == 0)
            {
                Debug.WriteLine("dataSet has no tables");
            }
            else
            {
                Debug.WriteLine($"dataSet contains {dataSet.Tables.Count} tables.");

                foreach (DataTable table in dataSet.Tables)
                {
                    Debug.WriteLine($"\nData from Table: {table.TableName}");
                    int rowCount = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (DataColumn col in table.Columns)
                        {
                            Debug.Write($"[{col.ColumnName}: {row[col]}]");
                        }

                        if (rowLimitToCheck > 0)
                            if (++rowCount == rowLimitToCheck) break;
                    }
                }
            }
        }
        
        public void applyOuterColorOverlay(Label label, Color overlayColor, int thickness = 2)
        {
            label.Paint -= Label_PaintWithOverlay;

            void Label_PaintWithOverlay(object sender, PaintEventArgs e)
            {
                Label lbl = (Label)sender;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                string text = lbl.Tag?.ToString() ?? lbl.Text;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddString(text, lbl.Font.FontFamily, (int)lbl.Font.Style,
                                   lbl.Font.SizeInPoints * e.Graphics.DpiY / 72f,
                                   new Rectangle(0, 0, lbl.Width, lbl.Height),
                                   getStringFormatFromContentAlignment(lbl.TextAlign));

                    using (Pen outline = new Pen(overlayColor, thickness))
                    {
                        outline.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                        outline.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;  // <-- solid pen
                        e.Graphics.DrawPath(outline, path);
                    }

                    using (SolidBrush textBrush = new SolidBrush(lbl.ForeColor))
                    {
                        e.Graphics.FillPath(textBrush, path);
                    }
                }
            }

            label.Tag = label.Text;
            label.Text = "";
            label.BackColor = Color.Transparent;
            label.Paint += Label_PaintWithOverlay;
            label.Invalidate();
        }


        public StringFormat getStringFormatFromContentAlignment(ContentAlignment align)
        {
            StringFormat sf = new StringFormat();

            switch (align)
            {
                case ContentAlignment.TopLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
            }

            return sf;
        }

        public void InitLogo(Bunifu.Framework.UI.BunifuImageButton obj)
        {
            Debug.WriteLine("--InitLogo--");

            obj.Image = null;
            obj.Enabled = false;
            obj.Image = Image.FromFile(dbFile.sImagePath + clsGlobalVariables.IMAGE_LOGO);
        }

        public void InitBankLogo(Bunifu.Framework.UI.BunifuImageButton obj)
        {
            Debug.WriteLine("--InitBankLogo--");

            obj.Image = null;
            obj.Enabled = false;
            obj.Image = Image.FromFile(dbFile.sImagePath + clsSearch.ClassBankCode + ".png");

            obj.BackColor = Color.Transparent;
        }

        // Format duration in "X min Y secs"
        public string formatDuration(TimeSpan ts)
        {
            return $"{ts.Minutes} min {ts.Seconds} secs";
        }

        public List<clsBank> loadBankList(string path)
        {
            string json = File.ReadAllText(path);
            BankResponse response = JsonConvert.DeserializeObject<BankResponse>(json);
            return response.Data;

        }

        public bool isValidRequestID(string pJORequestID, string pFSRRequestID)
        {
            bool isValid = false;

            if (pJORequestID.Equals(pFSRRequestID))
                isValid = true;

            if (!isValid)
            {
                SetMessageBox($"Mismatch detected: Job Order [{pJORequestID}] and Manual FSR [{pFSRRequestID}] have different Request IDs.", "Request ID mismatch", IconType.iError);
            }

            return isValid;


        }

        public void downloadSignature(int index, int serviceno)
        {
            string sFile = serviceno.ToString() + "_" + padLeftChar(index.ToString(), "0", 2) + clsDefines.FILE_EXT_PNG;
            string sRemotePath = $"{clsGlobalVariables.strFTPRemoteImagesPath}{clsSearch.ClassBankCode}/";
            string sLocaPath = $"{dbFile.sSignatuPath}";

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.download(sRemotePath + sFile, sLocaPath + sFile);
                ftpClient.disconnect();

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                SetMessageBox("Exceptional error " + ex.Message, clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

        }

        public bool isMultipleListViewChecked(ListView obj)
        {
            if (obj.Items.Count == 0) return false;

            int checkedCount = 0;

            foreach (ListViewItem item in obj.Items)
            {
                if (item.Checked)
                {
                    checkedCount++;
                    if (checkedCount > 1) return true; // more than 1 checked → multiple
                }
            }

            return false; // 0 or 1 item checked → not multiple
        }

        public void GetEnvironment(Label obj)
        {
            string prefix = "ENV: ";
            string env = clsSystemSetting.ClassSystemEnvironment;

            if (string.IsNullOrWhiteSpace(env))
            {
                obj.Text = $"{prefix}[UNKNOWN]";
            }
            else
            {
                obj.Text = $"{prefix}[{env.Trim().ToUpper()}]";
            }
        }

        public void CopyListViewToClipboard(ListView lvw, bool selectedOnly = false)
        {
            if (lvw.Items.Count == 0) return;

            var sb = new StringBuilder();

            var items = selectedOnly ? lvw.SelectedItems.Cast<ListViewItem>() : lvw.Items.Cast<ListViewItem>();

            foreach (var item in items)
            {
                var row = new List<string>();

                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    row.Add(item.SubItems[i].Text);
                }

                sb.AppendLine(string.Join("\t", row)); // tab-separated
            }

            Clipboard.SetText(sb.ToString());
        }
        
        public void CopyGridToClipboard(DataGridView grid, bool includeHeaders = true)
        {
            if (grid == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("No data to copy.", "Warning");
                return;
            }

            StringBuilder sb = new StringBuilder();

            // ✅ headers
            if (includeHeaders)
            {
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    if (!grid.Columns[i].Visible) continue;
                    sb.Append(grid.Columns[i].HeaderText + "\t");
                }
                sb.AppendLine();
            }

            // ✅ rows
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!cell.OwningColumn.Visible) continue;

                    sb.Append((cell.Value?.ToString() ?? "") + "\t");
                }
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        public bool checkFileInUse(string filePath)
        {
            bool isValid = true;

            if (dbFile.IsFileInUse(filePath))
            {
                MessageBox.Show("File is currently open or in use.");
                isValid = false;
            }
            
            return isValid;
        }

        public string formattedTID(string pValue)
        {
            string pOutput = "";

            pOutput = padLeftChar(pValue, sPadZero, TID_LENGTH);

            return pOutput;
        }

        public string formattedMID(string pValue)
        {
            string pOutput = "";

            pOutput = padLeftChar(pValue, sPadZero, MID_LENGTH);

            return pOutput;
        }

        public bool findInListView(ListView lvw, int colIndex, string pSearch)
        {
            bool isFound = false;

            foreach (ListViewItem item in lvw.Items)
            {
                // Assuming NAME is in column index 1
                string name = item.SubItems[colIndex].Text.ToUpper();

                if (name.Contains(pSearch))
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();

                    lvw.Focus();

                    isFound = true;
                    break;
                }
            }

            return isFound;
        }

        public string CleanFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Unknown";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }

            return input.Trim();
        }

    }

}
