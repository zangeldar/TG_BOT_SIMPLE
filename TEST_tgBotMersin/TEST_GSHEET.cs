using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gSheetsLib;

namespace TEST_tgBotMersin
{
    public class testGSheet:test
    {
        private string api_key = "AIzaSyAPklyQKUd1Il0WwabqJf5FUssY0fDTc4Q";
        private string gSheetId = "1tO5HIjEKPeZdITzzdO7OyuD_iPcuyVF-z9-ITIA0Jw4";
        GoogleSpreadSheet spreadsheet;// = new GoogleSpreadSheet(gSheetId, api_key);

        public testGSheet()
        {
            spreadsheet = new GoogleSpreadSheet(gSheetId, api_key);
        }

        public string DoTest()
        {
            string result = "";

            result = spreadsheet.GetValues(1, 1, 10, 1, "10");

            gValues tmpValues = Newtonsoft.Json.JsonConvert.DeserializeObject<gValues>(result);

            return result;
        }        

        public string ProceedDate()
        {
            string result = "";

            //string weekNum = GetNumberOfWeek(DateTime.UtcNow).ToString();

            //GetTimeSlots(spreadsheet, weekNum);

            //result = GetUrlFromGoogleSheet(spreadsheet, DateTime.Now);
            result = GetUrlFromGoogleSheet(spreadsheet, DateTime.Now.AddHours(8));

            return result;
        }

        public int GetNumberOfWeek(DateTime askDate)
        {            
            var cal = new GregorianCalendar();
            return cal.GetWeekOfYear(askDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        public string GetSheetNameOfWeek(DateTime askDate)
        {
            return GetNumberOfWeek(askDate).ToString();
        }

        public List<string> GetTimeSlots(GoogleSpreadSheet sheet, string sheetName)
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

        public List<string> GetDateSlots(GoogleSpreadSheet sheet, string sheetName)
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

        public int GetColForDate(GoogleSpreadSheet sheet, DateTime askDate)
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

        public int GetRowForTime(GoogleSpreadSheet sheet, DateTime askDate)
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
                    break;
                }
                prevTimeSlot = tmpTimeOnly;
            }

            return result;                
        }

        public DateOnly ParseStringToDate(string incString)
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

        public TimeOnly ParseStringToTime(string incString)
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

        public string GetUrlFromGoogleSheet(GoogleSpreadSheet sheet, DateTime askDate)
        {
            string result = "";
            int targetRow = 1;
            int targetCol = 1;
            string targetSheet = "";
            


            string tmp;


            targetRow = GetColForDate(sheet, askDate);
            targetCol = GetRowForTime(sheet, askDate);
            targetSheet = GetSheetNameOfWeek(askDate);
                       

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
    }
}
