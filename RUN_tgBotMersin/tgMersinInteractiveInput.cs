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
        public const string defaultSettingsFileName = "settings.ini";
        public static string settingsFileName = "";

        public static tgMersinSettings settings = null;
        public static bool InteractiveRun = true;
        public static bool Debug = false;

        public static bool NoPin = false;

        /// <summary>
        /// Интерактивное заполнение настроек в консоли
        /// </summary>
        /// <returns></returns>
        public static bool FillSettingsInteractive(bool goNeed = true, bool tgNeed = true)
        {
            if (!InteractiveRun)
            {
                Console.WriteLine("This operation cant be doing in AUTOMATIC mode. Plz, remove key AUTO or SCHEDULE and check it INTERACTIVE!");
                return false;
            }
            if (settings == null)
                settings = new tgMersinSettings();

            string readStr = "";

            TelegramSettings setTg = settings.SettingsTelegram;
            GoogleSettings setGo = settings.SettingsGoogle;

            while (readStr == "" | readStr == null)
            {
                bool goFirst = false;
                bool tgFirst = false;
                if (goNeed & tgNeed)
                {
                    Console.WriteLine("Select which settings you will fill first: Google('g') or Telegram('t')?");
                    readStr = Console.ReadLine();
                    if (readStr == "exit")
                    {
                        Console.WriteLine("Canceled by user.");
                        return false;
                    }
                    else if (readStr == "g" | readStr == "google")
                    {
                        goFirst = true;
                    }
                    else if (readStr == "t" | readStr == "telegram")
                    {
                        goFirst = false;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input!");
                        readStr = "";
                        continue;
                    }
                }

                if (goFirst)
                {
                    readStr = "g";
                    if (goNeed)
                    {
                        // заполнять bx
                        setGo = FillSettingGoogle();
                    }
                    if (setGo != null | !goNeed)
                    {
                        if (tgNeed)
                        {
                            // заполнять tg
                            setTg = FillSettingTelegram();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with Google connection");
                        readStr = "";
                    }
                }
                else if (!goFirst)
                {
                    readStr = "t";
                    if (tgNeed)
                    {
                        // заполнять tg
                        setTg = FillSettingTelegram();
                    }
                    if (setTg != null | !tgNeed)
                    {
                        if (goNeed)
                        {
                            // заполнять bx
                            setGo = FillSettingGoogle();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with Telegram connection");
                        readStr = "";
                    }
                }
            }


            if (goNeed)
            {
                while (setGo == null)
                {
                    readStr = "";

                    while (readStr == "" | readStr == null)
                    {
                        Console.WriteLine("Google fail. Try again? (y/n)");
                        readStr = Console.ReadLine();
                        if (readStr == "y")
                            setGo = FillSettingGoogle();
                        else if (readStr == "n")
                            break;
                        else
                            readStr = "";
                    }
                    if (readStr == "n")
                        break;
                }

            }

            if (tgNeed)
            {
                while (setTg == null)
                {
                    readStr = "";

                    while (readStr == "" | readStr == null)
                    {
                        Console.WriteLine("Telegram fail. Try again? (y/n)");
                        readStr = Console.ReadLine();
                        if (readStr == "y")
                            setTg = FillSettingTelegram();
                        else if (readStr == "n")
                            break;
                        else
                            readStr = "";
                    }
                    if (readStr == "n")
                        break;
                }

            }

            if (goNeed)
            {
                if (setGo == null)
                {
                    Console.WriteLine("Bitrix settings fail. Exit now.");
                    return false;
                }
                else
                {
                    settings.SettingsGoogle = setGo;
                }
            }

            if (tgNeed)
            {
                if (setTg == null)
                {
                    Console.WriteLine("Telegram settings fail. Exit now.");
                    return false;
                }
                else
                {
                    settings.SettingsTelegram = setTg;
                }
            }

            return true;

        }

        /// <summary>
        /// Интерактивное заполнение настроек ТГ
        /// </summary>
        /// <returns></returns>
        public static TelegramSettings FillSettingTelegram()
        {
            TelegramSettings result = new TelegramSettings();

            string readStr = "";

            //TelegramBot testTg = null;

            Console.WriteLine("TELEGRAM settings:");

            readStr = "";
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input TG API-key:");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    //settings = new Settings();
                    return null;
                }
                else if (readStr.Length > 0)
                {
                    result.API_KEY = readStr;
                    break;
                }
                else
                    readStr = "";
            }

            readStr = "";
            
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input TG destination chat URL:");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    return null;
                }
                else if (readStr == "" | readStr == null)
                {
                    //testTg = new TelegramBot(result.API_KEY);
                    //result.ServerUrl = testTg.serverUrl;
                    break;
                }
                else if (readStr.StartsWith("http://t.me/") | readStr.StartsWith("https://t.me/"))
                {
                    result.UrlToDstChat = readStr;
                    //testTg = new TelegramBot(result.API_KEY, result.ServerUrl);
                    break;
                }
                else
                    readStr = "";
            }          


            Console.WriteLine("Checking TG settings ...");
            if (!CheckSettingsTelegram(result))
            {
                Console.WriteLine("Incorrect data to create telegram connection! Check ur API_KEY and SERVER and try again.");
                return null;
            }

            Console.WriteLine("Success! Telegram is ready!");
            return result;
        }

        /// <summary>
        /// Проверка корректности настроек ТГ
        /// </summary>
        /// <param name="checkSet"></param>
        /// <returns></returns>
        public static bool CheckSettingsTelegram(TelegramSettings checkSet)
        {
            TelegramBot testTg = null;
            if (checkSet.API_KEY != null)
            {
                if (checkSet.API_KEY.Length > 0)
                {
                    if (checkSet.UrlToDstChat != null)
                        if (checkSet.UrlToDstChat.Length > 0)
                            if (checkSet.UrlToDstChat.StartsWith("https://t.me/") | checkSet.UrlToDstChat.StartsWith("http://t.me/"))                                
                                testTg = new TelegramBot(checkSet.API_KEY);
                    
                    // если не указан serverUrl тогда создаем Телеграм только по токену
                    if (testTg != null)
                        // Проверяем Телеграм
                        if (testTg.getMe() != null)
                        {
                            //checkSet.ServerUrl = testTg.serverUrl;                        
                            return true;
                        }
                }
            }

            if (testTg != null)
            {
                if (testTg.LastError != null)
                {
                    string fullErrorMessage = "";
                    Exception curError = testTg.LastError;
                    while (curError != null)
                    {
                        fullErrorMessage += "+-" + curError.Message;
                        curError = curError.InnerException;
                    }
                    Console.WriteLine("TG has error: " + fullErrorMessage);
                }
                else
                    Console.WriteLine("TG Last Answer: " + testTg.LastAnswer);
            }

            return false;
        }

        /// <summary>
        /// Интерактивное заполнение настроек Битрикса в консоли
        /// </summary>
        /// <returns></returns>
        public static GoogleSettings FillSettingGoogle()
        {
            GoogleSettings result = new GoogleSettings();

            string readStr = "";

            //bitrix.bitrix testBitrix;

            Console.WriteLine("Google settings:");


            readStr = "";
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input URL to Google spreadsheet (must include http:// or https://):");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    return null;
                }
                else if (readStr.StartsWith("http://") | readStr.StartsWith("https://"))
                {
                    result.UrlToTable = readStr;
                    break;
                }
                else
                    readStr = "";
            }

            readStr = "";
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input GoogleApiKey:");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    return null;
                }
                else if (readStr.Length > 0)
                {
                    result.GoogleApiKey = readStr;
                    //readStr = "";
                    break;
                }
                else
                    readStr = "";
            }

            //testBitrix = new bitrix.bitrix(result.ServerUrl, result.Login, result.Password);

            Console.WriteLine("Checking Google settings ...");
            if (!CheckSettingsGoogle(result))
            {
                Console.WriteLine("Incorrect data to create Google connection! Check ur data and try again.");
                return null;
            }

            Console.WriteLine("Success! Google is ready!");
            return result;
        }

        /// <summary>
        /// Проверка корректноси настроек Битрикс
        /// </summary>
        /// <param name="checkSet"></param>
        /// <returns></returns>
        public static bool CheckSettingsGoogle(GoogleSettings checkSet)
        {
            GoogleSpreadSheet testGoogleSheet = null;
            
            if (checkSet.UrlToTable != null & checkSet.GoogleApiKey != null)
            {
                if (checkSet.UrlToTable.Length > 0 & checkSet.GoogleApiKey.Length > 0)
                {
                    if (checkSet.UrlToTable.StartsWith("https://") | checkSet.UrlToTable.StartsWith("http://"))
                    {
                        if (!checkSet.UrlToTable.EndsWith("/"))
                            checkSet.UrlToTable = checkSet.UrlToTable + "/";
                        testGoogleSheet = new GoogleSpreadSheet(checkSet.UrlToTable, checkSet.GoogleApiKey, true);                            
                    }


                    if (testGoogleSheet != null) // если все заполнено
                        if (testGoogleSheet.GetValues(1,1) != null) // если есть ответ от сервера
                            return true;
                }
            }
            return false;
        }

    }
}
