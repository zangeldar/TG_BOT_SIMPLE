using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace gSheetsLib
{
    public class GoogleSpreadSheet
    {
        //public const string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
        //private string api_key = "AIzaSyAPklyQKUd1Il0WwabqJf5FUssY0fDTc4Q";
        public string spreadsheetId { get; private set; }
        public string LastRange { get; private set; }
        public GoogleSpreadSheet()
        {
            //this.spreadsheetId = makeNewGoogleSpreadSheet();
            serverUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
        }

        public GoogleSpreadSheet(string spreadsheetId)
        {
            serverUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
            this.spreadsheetId = spreadsheetId;
            //SetApiKey("AIzaSyAPklyQKUd1Il0WwabqJf5FUssY0fDTc4Q");
        }

        public GoogleSpreadSheet(string spreadsheetId, string api_key)
        {
            serverUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
            this.spreadsheetId = spreadsheetId;            
            SetApiKey(api_key);
        }

        public GoogleSpreadSheet(string spreadsheetUrl, string api_key, bool byUrl=true)
        {
            serverUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
            this.spreadsheetId = GetSpreadSheetIdFromUrl(spreadsheetUrl);
            SetApiKey(api_key);
        }

        public bool SetApiKey(string api_key)
        {
            this.token = "?key=" + api_key;
            HasApiKey = true;
            return HasApiKey;
        }
        public static string GetSpreadSheetIdFromUrl(string urlToSpreadSheet)
        {
            string result = "";

            //urlToSpreadSheet = "https://docs.google.com/spreadsheets/d/1tO5HIjEKPeZdITzzdO7OyuD_iPcuyVF-z9-ITIA0Jw4/edit#gid=2137331829";
            string[] tmpStr = urlToSpreadSheet.Split('/');
            if (tmpStr.Length > 5)
                result = tmpStr[5];

            return result;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            return UnixTimeStampToDateTime((double)unixTimeStamp);
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public Exception LastError { get; private set; }
        public string LastErrorResponse { get; private set; }
        public string LastRequest { get; private set; }
        public string LastAnswer { get; private set; }
        private string token;
        public bool HasApiKey { get; private set; }
        public string serverUrl { get; private set; }

        public string postBase { get => serverUrl + spreadsheetId + "/"; }


        private string sendRequest(string postData)
        {
            /*
            ServicePointManager.Expect100Continue = true;
            */
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            /*                   | SecurityProtocolType.Tls11
                               | SecurityProtocolType.Tls12
                               | SecurityProtocolType.Ssl3;
                               */
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            string result = "";

            LastRequest = postData.Replace(token, "#api_token_replace#");
            var req = (HttpWebRequest)WebRequest.Create(postData);

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)req.GetResponse();
                LastError = null;
                LastErrorResponse = null;
            }
            catch (Exception e)
            {
                LastError = e;
                //return null; // вернем после обработки исключения для получения расширенного ответа от сервера
                //throw;
            }


            if (LastError != null)
            {
                if (LastError is WebException)
                {
                    LastErrorResponse = new StreamReader((LastError as WebException).Response.GetResponseStream()).ReadToEnd();
                }
                return null;
            }

            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            LastAnswer = result;

            return result;
        }

        // Here is public API methods:

        public string GetValues(string Range)
        {
            LastRange = Range;
            return sendRequest(postBase + "values/" + Range + token);
        }

        public string GetValues(int row, int col, string sheetName="")
        {
            return GetValues(MakeRange(MakeCell(row, col),"",sheetName));
        }

        public string GetValues(int startRow, int startCol, int endRow, int endCol, string sheetName = "")
        {
            return GetValues(MakeRange(MakeCell(startRow, startCol),MakeCell(endRow, endCol), sheetName));
        }

        public string MakeRange(string cellStart, string cellEnd = "", string sheetName = "")
        {
            string result = cellStart;

            if (cellEnd != "")
                result += ":" + cellEnd;
            if (sheetName != "")
                result = sheetName + "!" + result;

            return result;
        }

        public string MakeCell(int row, int col)
        {
            return "R" + row.ToString() + "C" + col.ToString();
        }
    }

    public class gSpreadSheet
    {
        public string spreadsheetId { get; set; }
        public gProperties properties { get; set; }
        public List<gSheet> sheets { get; set; }
        public string spreadsheetUrl { get; set; }
    }

    public class gSheet
    {
        public gProperties properties { get; set; }
    }

    public class gProperties
    {
        public string title { get; set; }
        public string locale { get; set; }
        public string autoRecalc { get; set; }
        public string timeZone { get; set; }
        /*
        public DefaultFormat defaultFormat { get; set; }
        public SpreadsheetTheme spreadsheetTheme { get; set; }
        */
        public int sheetId { get; set; }
        public int index { get; set; }
        public string sheetType { get; set; }
        /*
        public GridProperties gridProperties { get; set; }
        public TabColor tabColor { get; set; }
        public TabColorStyle tabColorStyle { get; set; }
        */
    }

    public class gValues
    {
        public string range { get; set; }
        public string majorDimension { get; set; }
        public List<List<string>> values { get; set; }
    }
}
