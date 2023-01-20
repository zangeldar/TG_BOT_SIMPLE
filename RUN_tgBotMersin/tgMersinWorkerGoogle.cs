using gSheetsLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgBotMersin
{
    public partial class tgMersinWorker
    {
        public static string GetUrlFromGoogleSheet(GoogleSpreadSheet sheet, DateTime askDate)
        {
            string result = "";
            int targetRow = 1;
            int targetCol = 1;
            string targetSheet = "";

            string tmp;

            targetSheet = GetSheetNameOfWeek(askDate);
            targetCol = GetColForDate(sheet, askDate);
            targetRow = GetRowForTime(sheet, askDate);

            if (targetCol == 1)
                //can't found data in table
                return result;
            if (targetRow == 1)
                //can't found current time slot (less than first timeSlot in table)
                return result;


            tmp = sheet.GetValues(targetRow, targetCol, targetSheet);
            if (tmp != null)
            {
                gValues tmpValues = Newtonsoft.Json.JsonConvert.DeserializeObject<gValues>(tmp);
                if (tmpValues != null)
                    if (tmpValues.values.Count > 0)
                        if (tmpValues.values[0].Count > 0)
                            result = tmpValues.values[0][0];
            }

            return result;
        }

        /// <summary>
        /// Get number of week as integer
        /// </summary>
        /// <param name="askDate"></param>
        /// <returns></returns>
        public static int GetNumberOfWeek(DateTime askDate)
        {
            var cal = new GregorianCalendar();
            return cal.GetWeekOfYear(askDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Get number of week as string
        /// </summary>
        /// <param name="askDate"></param>
        /// <returns></returns>
        public static string GetSheetNameOfWeek(DateTime askDate)
        {
            return GetNumberOfWeek(askDate).ToString();
        }

        public static List<string> GetTimeSlots(GoogleSpreadSheet sheet, string sheetName)
        {
            List<string> result = new List<string>();

            string tmp = sheet.GetValues(1, 1, 3600, 1, sheetName);
            if (tmp != null)
            {
                gValues tmpValues = Newtonsoft.Json.JsonConvert.DeserializeObject<gValues>(tmp);

                foreach (List<string> itemList in tmpValues.values)
                    foreach (string item in itemList)
                        //if (item!="")
                        result.Add(item);
            }

            return result;
        }

        public static List<string> GetDateSlots(GoogleSpreadSheet sheet, string sheetName)
        {
            List<string> result = new List<string>();

            string tmp = sheet.GetValues(1, 1, 1, 33, sheetName);
            if (tmp != null)
            {
                gValues tmpValues = Newtonsoft.Json.JsonConvert.DeserializeObject<gValues>(tmp);

                if (tmpValues != null)
                    if (tmpValues.values.Count > 0)
                        foreach (string item in tmpValues.values[0])
                            //if (item!="")
                            result.Add(item);
            }

            return result;
        }

        public static int GetColForDate(GoogleSpreadSheet sheet, DateTime askDate)
        {
            int result = 0;

            List<string> DateSlots = GetDateSlots(sheet, GetSheetNameOfWeek(askDate));

            foreach (string item in DateSlots)
            {
                result++;
                DateOnly tmpDateOnly = ParseStringToDate(item);
                if (tmpDateOnly.CompareTo(DateOnly.FromDateTime(askDate)) == 0)
                {
                    //result--;
                    break;
                }
            }

            return result;
        }

        public static int GetRowForTime(GoogleSpreadSheet sheet, DateTime askDate)
        {
            int result = 0;
            TimeOnly prevTimeSlot;

            List<string> TimeSlots = GetTimeSlots(sheet, GetSheetNameOfWeek(askDate));

            foreach (string item in TimeSlots)
            {
                result++;
                TimeOnly tmpTimeOnly = ParseStringToTime(item);
                int curInd = tmpTimeOnly.CompareTo(TimeOnly.FromDateTime(askDate));
                int prevInd = prevTimeSlot.CompareTo(TimeOnly.FromDateTime(askDate));
                if (curInd != prevInd)
                {
                    return result;
                    break;
                }
                prevTimeSlot = tmpTimeOnly;
            }
            // in case of last time slot in a day
            result++;

            return result;
        }

        public static DateOnly ParseStringToDate(string incString)
        {
            DateOnly result;

            if (!DateOnly.TryParse(incString, out result))
            {
                int year = 0, month = 0, day = 0;
                string[] tmpArr = incString.Split('/');
                if (tmpArr.Length == 3)
                {
                    if (int.TryParse(tmpArr[2], out year) && int.TryParse(tmpArr[0], out month) && int.TryParse(tmpArr[1], out day))
                        result = new DateOnly(year, month, day);
                }
            }

            return result;
        }

        public static TimeOnly ParseStringToTime(string incString)
        {
            TimeOnly result;

            if (!TimeOnly.TryParse(incString, out result))
            {
                int hour = 0, minute = 0;
                string[] tmpArr = incString.Split('/');
                if (tmpArr.Length == 2)
                {
                    if (int.TryParse(tmpArr[1], out minute) && int.TryParse(tmpArr[0], out hour))
                        result = new TimeOnly(hour, minute);
                }
            }

            return result;
        }
    }





}
