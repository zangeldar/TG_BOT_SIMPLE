using gSheetsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tglib;

namespace tgBotMersin
{    
    public partial class tgMersinWorker
    {
        /// <summary>
        /// Get Link from Table according to technical task.
        /// To get Link function need to know:
        ///     1.  link to google sheet
        ///     2.  current time & date
        /// </summary>
        /// <returns></returns>
        public static string GetLinkFromTable(string urlToGoogleSheet, DateTime askDate)
        {
            string result = "";
            GoogleSpreadSheet sheet = new GoogleSpreadSheet(urlToGoogleSheet, tgMersinWorker.settings.SettingsGoogle.GoogleApiKey, true);
            //result = GetUrlFromGoogleSheet(sheet, DateTime.Now);
            result = GetUrlFromGoogleSheet(sheet, askDate);
            tgMersinWorker.settings.LastUsedRangeGoogleSheet = sheet.LastRange;
            return result;
        }




    }
}
