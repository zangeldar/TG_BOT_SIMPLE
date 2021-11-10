using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using bitrix;
using tglib;

namespace Messenger
{
    class Program
    {
        private const string defaultSettingsFileName = "settings.ini";
        private static string settingsFileName = "";
        private static bool InteractiveRun = true;
        private static Settings settings = null;

        static void Main(string[] args)
        {
            string tmpFileName="";
            // проверка, передали ли в качестве аргумента имя файла настроек
            if (args.Length > 0)
                foreach (string arg in args)
                {
                    if (arg.Contains("settings="))
                    {
                        tmpFileName = arg.Replace("settings=", "");
                    }
                    else if (arg == "schedule" | arg == "auto")
                    {
                        InteractiveRun = false;
                    }
                    else
                    {
                        Console.WriteLine("Unknown arg: " + arg);
                    }
                }

                // проверка существует ли файл
                if (System.IO.File.Exists(tmpFileName))
                {
                    // пробуем загрузить настройки из файла. Если успешно, то сохраняем имя файла
                    if (LoadSettings(tmpFileName))
                        settingsFileName = tmpFileName;
                }
                else
                {
                    // файла нет, проверяем возмодность создания файла с таким именем
                    try
                    {
                        System.IO.File.WriteAllText(tmpFileName, "test");
                    }
                    catch (Exception e)
                    {
                        //throw;
                    }
                    
                    // если получилось, то удаляем проверочный файл и сохраняем имя файла настроек
                    if (System.IO.File.Exists(tmpFileName))
                    {
                        System.IO.File.Delete(tmpFileName);
                        settingsFileName = tmpFileName;
                    }                    
                }

            // проверяем, удалось ли сохранить имя файла. Если нет, то используем имя по умолчанию
            if (settingsFileName.Length == 0)
                settingsFileName = defaultSettingsFileName;

            // проверяем, существет ли файл настроек
            bool goodSettings = false;
            if (System.IO.File.Exists(settingsFileName))
                if (LoadSettings(settingsFileName))
                    goodSettings = true;

            if (!goodSettings)
            {
                Console.WriteLine("Settings not found! Looks like it is a first executing!");
                if (FillSettingsInteractive())
                {
                    if (!SaveSettings(settingsFileName))
                        Console.WriteLine("Warning! Setting was not saved!");
                }
                else
                {
                    Console.WriteLine("Breaked bcs has errors");
                    return;
                }
            }
            else
            {
                // Check Uploaded Settings
                SettingsBitrix corSetBx = null;
                SettingsTelegram corSetTg = null;
                bool tgNeed = false;
                bool bxNeed = false;

                if (!CheckSettingsBitrix(settings.SettingsBitrix))
                {
                    goodSettings = false;
                    Console.WriteLine("Incorrect Bitrix Settings in stored settings. Correct interactive!");
                    bxNeed = true;
                    /*
                    corSetBx = FillSettingBitrix();
                    CheckSettingsBitrix(corSetBx);
                    */
                }
                if (!CheckSettingsTelegram(settings.SettingsTelegram))
                {
                    goodSettings = false;
                    Console.WriteLine("Incorrect Telegram Settings in stored settings. Correct interactive!");
                    tgNeed = true;
                    /*
                    corSetTg = FillSettingTelegram();
                    CheckSettingsTelegram(corSetTg);
                    */
                }

                if (!goodSettings)
                {
                    if (!FillSettingsInteractive(bxNeed, tgNeed))
                    {
                        Console.WriteLine("Something worng with stored settings. Cant continue. Exit.");
                        return;
                    }                    
                }
            }

            //////////////////////////////////////////////
            // here must start magic
            //////////////////////////////////////////////////
            ///

            bitrix.bitrix b = new bitrix.bitrix(settings.SettingsBitrix.ServerUrl, settings.SettingsBitrix.Login, settings.SettingsBitrix.Password);

            object tmp = mywebcore.core.ParseJson(b.imUserGet());
            RootGetUser tmpRootUser = Newtonsoft.Json.JsonConvert.DeserializeObject<RootGetUser>(b.lastAnswer);

            /*
            // создаем рабочий список пользователей ..
            List<User> workListUser = new List<User>();
            // .. и копируем в него список из настроек
            workListUser.AddRange(settings.UsersList);
            */
            List<User> workListUser = settings.UsersList;

            // создаем рабочее соответствие пользователя-пересылателя со стороны битрикс...
            User tmpUserObj = new User() { UserIdBitrix = tmpRootUser.result.id, NameFullBitrix = tmpRootUser.result.name };
            

            tmp = mywebcore.core.ParseJson(b.imRecentGet());
            RootRecent tmpRootRecent = Newtonsoft.Json.JsonConvert.DeserializeObject<RootRecent>(b.lastAnswer);

            // создаем ВРЕМЕННЫЙ список чатов .. (сюда заносятся имеющиеся актуальные чаты со стороны БИТРИКСА, не имеющие соответствия в ТГ)
            List<Chat> tmpListChat = new List<Chat>();
            // создаем рабочий список чатов .. (здесь хранятся уже имеющиеся соответствия чатов BX-TG)
            /*
            List<Chat> workListChat = new List<Chat>();
            // .. и копируем в него список из настроек
            workListChat.AddRange(settings.ChatsList);
            */
            List<Chat> workListChat = settings.ChatsList;

            // заполняем ВРЕМЕННЫЙ спиcок чатов теми чатами, которых нет в списке соответствия
            foreach (bitrix.ResultRecent item in tmpRootRecent.result)
                if (item.type == "chat" & item.chat != null)
                {
                    if (settings.GetTgChatIdByBx(item.id.ToString()) == null)
                        tmpListChat.Add(new Chat() { ChatIdBitrix = item.id, NameBitrix = item.title });
                } 
                    

            TelegramBot t = new TelegramBot(settings.SettingsTelegram.API_KEY, settings.SettingsTelegram.ServerUrl);
            tmp = mywebcore.core.ParseJson(t.getMe());
            tgRootGetMe tmpTgRootGetMe = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootGetMe>(t.LastAnswer);
            
            // заполняем рабочее соответствие пользоватлея-пересылателя со стороны телеграмм..
            tmpUserObj.NameFullTelegram = tmpTgRootGetMe.result.username;
            tmpUserObj.UserIdTelegram = tmpTgRootGetMe.result.id.ToString();
            tmpUserObj.Description = "[BX<=>TG] system user";

            // ищем/добавляем соответствие пользователя битрикса пользователю ТГ
            if (!workListUser.Contains(tmpUserObj))
                workListUser.Add(tmpUserObj);

            /*
             * удалить
            if (tmpListUser.Count == 1)
            {
                tmpListUser[0].NameFullTelegram = tmpTgRootGetMe.result.username;
                tmpListUser[0].UserIdTelegram = tmpTgRootGetMe.result.id.ToString();
            } else if (tmpListUser.Count == 0)
                tmpListUser.Add(new User() { NameFullTelegram = tmpTgRootGetMe.result.username, UserIdTelegram = tmpTgRootGetMe.result.id.ToString() });
            else
            {
                Console.WriteLine("WARNING! Excepted only one user stored, but there are " + tmpListUser.Count + " found");
                // попробовать установить соответствие среди имеющихся пользователей.
            }
            */
                
            // получить последние обновления из ТГ
            tmp = mywebcore.core.ParseJson(t.getUpdates());
            tgRootGetUpdates tmpTgUpdates = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootGetUpdates>(t.LastAnswer);
            /*
            // проверка всех tgUpdate в ответе
            foreach (tgUpdate item in tmpTgUpdates.result)
            {
                // проверять на тип обновления (update) : добавление/удаление пользователей, сообщение и т.д.


                // проверять update_id на предмет обработанности и на случай сброса (перескока) последовательности нумерации в отдельной функции, там же заносить последнее значение в настройки
                //settings.SettingsTelegram.LastKnownUpdateId = Math.Max(settings.SettingsTelegram.LastKnownUpdateId, item.update_id);


                // если tgUpdate обработан, то его update_id заносим в последний известный LastKnownUpdateId
                //settings.SettingsTelegram.LastKnownUpdateId = item.update_id;
            }
            */


            List<Message> MessagesToSend = new List<Message>();

            // перебор только новых обновлений ТГ
            foreach (tgUpdate item in tmpTgUpdates.getNewUpdatesOnly(settings.SettingsTelegram.LastKnownUpdateId))
            {
                if (item.message != null)
                {
                    if (item.message.chat != null)
                    {
                        Message newMessage = new Message();
                        // Проверим, есть ли этот чат в списке соответствия чатов?  
                        // надо также проверять в рабочем списке соответствия чатов.
                        string sendToChatId = settings.GetBxChatIdByTg(item.message.chat.id.ToString());
                        if (sendToChatId != null)
                        {
                            // использовать Bx.chat_id для пересылки сообщения 
                            
                        }
                        else
                        {
                            // принимать решение и заносить TgChatId в список, или устанавливать соответствие с имеющимися BxChatId
                            // использовать tmpListChat

                            // здесь надо получить полное описание чата
                            if (t.getChat(item.message.chat.id.ToString()) != null)
                            {
                                tmp = mywebcore.core.ParseJson(t.LastAnswer);
                                tgRoot tgRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRoot>(t.LastAnswer);
                                tgRootGetChat tgRootChat = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootGetChat>(t.LastAnswer);
                                
                                
                                for (int i = tmpListChat.Count - 1; i >= 0; i--)
                                {
                                    // если в описании чата ТГ есть полное название чата БКС, то это признак соответствия
                                    if (tgRootChat.result.description != null)
                                    {
                                        //if (tgRootChat.result.description.Contains(tmpListChat[i].NameBitrix))    // планирую добавить более сложную проверку, например, шифровать имя чата битрикса и просить вставить зашифрованную строку в описание чата ТГ
                                        if (CheckMappedChat(tgRootChat.result.description, tmpListChat[i].NameBitrix))
                                        {
                                            // вносим в рабочий список
                                            workListChat.Add(new Chat()
                                            {
                                                ChatIdBitrix = tmpListChat[i].ChatIdBitrix,
                                                ChatIdTelegram = tgRootChat.result.id.ToString(),
                                                DescriptionBitrix = tmpListChat[i].DescriptionBitrix,
                                                DescriptionTelegram = tgRootChat.result.description,
                                                NameBitrix = tmpListChat[i].NameBitrix,
                                                NameTelegram = tgRootChat.result.title
                                            });
                                            // удаляем из временного списка
                                            tmpListChat.RemoveAt(i);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Error while get ChatDescription for chat_id: " + item.message.chat.id.ToString());
                                Console.WriteLine("Error desc: " + t.LastError.Message);
                            }
                        }

                        // проверим, есть ли пользователь в списке соответствия пользователей?
                        User currentUser = settings.GetUserByTg(item.message.from.id.ToString());
                        /*
                        string sendFromUserId = settings.GetBxUserIdByTg(item.message.from.id.ToString());                        
                        if (sendFromUserId != null)
                        {
                            // использовать Bx.user_id для пересылки сообщения                             
                        }
                        else
                        {
                            // принимать решение и заносить TgUserId в список, или устанавливать соответствие с имеющимися BxUserId
                            // либо использовать tg.user.id + tg.user.first_name для пересылки сообщения. В таком случае, соответствие придется устанавливать вручную, или другим способом
                        }
                        */

                        // проверим, есть ли текст сообщения ?
                        if (item.message.text != null)
                        {
                            // использовать item.message.text для пересылки сообщения
                            newMessage.toBx = true;
                            newMessage.chat_id = item.message.chat.id.ToString();
                            newMessage.sender_id = item.message.from.id.ToString();
                            newMessage.sender_name = item.message.from.first_name;
                            newMessage.text = item.message.text;

                            newMessage.date = item.message.date.ToString();

                            if (sendToChatId != null)
                            {
                                string bxUser = "<unknown>";
                                string tgUser = item.message.from.first_name;
                                if (currentUser != null)
                                    if (currentUser.UserIdBitrix != null & currentUser.UserIdBitrix != "")
                                        bxUser = currentUser.NameFullBitrix;
                                b.SendMessage(sendToChatId,
                                    bxUser + " [" + tgUser + "] wrote in TG:" + Environment.NewLine +
                                    newMessage.text
                                    );
                            }
                        }
                        else
                        {
                            // текста нет, это какое-то системное событие, типа:
                            //  -   смена названия чата
                            //  -   добавление/удаление пользователя
                            //  -   ???
                            //  -   ???
                            //  -   ???
                            //  -   ???
                            //  -   ???
                            //  -   ???
                            //  -   ???
                            //  -   ???
                        }
                    }
                    settings.SettingsTelegram.LastKnownUpdateId = item.update_id;
                }
            }


            // here is place for BX => TG part
            // надо проверять чаты BX из workListChat (он был обновлен при проверке телеграмма)
            //workListChat[0].ChatIdBitrix
            foreach (Chat item in workListChat)
            {
                if (item.ChatIdBitrix != null)
                {
                    if (item.ChatIdBitrix != "")
                    {
                        tmp = mywebcore.core.ParseJson(b.imDialogMessagesGet(item.ChatIdBitrix));
                        RootDialogMessages tmpRootDialogMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<RootDialogMessages>(b.lastAnswer);

                        
                        for (int i = tmpRootDialogMessages.result.messages.Count - 1; i >= 0; i--)
                        //foreach (MessageDialogMessages inItem in tmpRootDialogMessages.result.messages)   // цикл надо развернуть
                        {
                            if (tmpRootDialogMessages.result.messages[i].id <= item.LastMessageIdBitrix)  // пропускаем обработанные сообщения (id менбше сохраненного)
                                continue;
                            else if (tmpRootDialogMessages.result.messages[i].author_id == 0)    // игнорируем системные сообщения
                                continue;
                            else if (tmpRootDialogMessages.result.messages[i].author_id.ToString() == tmpUserObj.UserIdBitrix)  // игнорируем сообщения системного пользователя
                                continue;
                            else
                            {                                
                                // обрабатывем сообщение битрикс для отправки в ТГ
                                /*
                                Message tMess = new Message()
                                {
                                    chat_id = item.ChatIdTelegram,
                                    date = tmpRootDialogMessages.result.messages[i].date.ToString(),
                                    fromBx = true,
                                    id = tmpRootDialogMessages.result.messages[i].id.ToString(),
                                    sender_id = tmpRootDialogMessages.result.messages[i].author_id.ToString(),
                                    // Получить имя пользователя из массива tmpRootDialogMessages.result.users по user_id
                                    sender_name = GetUserAuthor(tmpRootDialogMessages.result.users, tmpRootDialogMessages.result.messages[i].author_id.ToString()).name,
                                    text = tmpRootDialogMessages.result.messages[i].text
                                };
                                */
                                // ищем пользователя в списке соответствия пользователей
                                if (item.ChatIdTelegram != null)
                                {
                                    //if (tmpRootDialogMessages.result.messages[i].text !=)                                
                                    {
                                        User currentUser = settings.GetUserByBx(tmpRootDialogMessages.result.messages[i].id.ToString());
                                        string tgUser = "<unknown>";
                                        string bXUser = "<unknown>";
                                        bitrix.User tmpBxUSer = GetUserAuthor(tmpRootDialogMessages.result.users, tmpRootDialogMessages.result.messages[i].author_id.ToString());
                                        if (tmpBxUSer != null)
                                            bXUser = tmpBxUSer.name;
                                        
                                        if (currentUser != null)
                                            if (currentUser.UserIdTelegram != null & currentUser.UserIdTelegram != "")
                                                tgUser = currentUser.NameFullTelegram;

                                        t.sendMessage(item.ChatIdTelegram,
                                            tgUser + " [" + bXUser + "] wrote in BX:" + Environment.NewLine +
                                            tmpRootDialogMessages.result.messages[i].text
                                            );
                                    }
                                }
                                item.LastMessageIdBitrix = tmpRootDialogMessages.result.messages[i].id;
                            }
                        }
                    }
                }
                    
            }
            

            // exit routines
            SaveSettings(settingsFileName);

            //Console.ReadKey();
            return;

            /*
            Console.WriteLine("Hello World!");
            bitrix.bitrix b = new bitrix.bitrix("https://stels-ug.bitrix24.ru/", "admin@stels-ug.ru", "p@ssw0rd");
            */
            /*
            Console.WriteLine(b.lastAnswer);
            Console.WriteLine();
            */

            string tmpUser;
            string tmpMessage;

            TelegramBot tg = new TelegramBot("1820928079:AAHaJnDpL8OpIj9a-wka7vkNlauNRa1gnR4");
            /*
            Console.WriteLine(tg.getMe());
            tgRootGetMe tmpRootGetMe = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootGetMe>(tg.LastAnswer);
            */
            Console.WriteLine(tg.getUpdates());
            tgRootGetUpdates tmpRootGetUpdates = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootGetUpdates>(tg.LastAnswer);
            //tg.sendMessage("-528947865", "test message from automated application");
            foreach (tgUpdate item in tmpRootGetUpdates.result)
            {
                tmpUser = "";
                tmpMessage = "";
                // здесь должна быть проверка на обработанность обновления
                if (item.message != null)
                    if (item.message.chat != null)
                        // здесь должна быть проверка чата на принадлежность к списку отслеживаемых чатов
                        // if (item.message.chat.id == "<id chata>")
                        if (item.message.chat.title.Contains("STELS"))
                            if (item.message.text != null & item.message.text != "")
                            {
                                tmpMessage = item.message.text;
                                if (item.message.from != null)
                                {
                                    if (item.message.from.first_name != null & item.message.from.first_name != "")
                                        tmpUser += item.message.from.first_name + " ";
                                    if (item.message.from.last_name != null & item.message.from.last_name != "")
                                        tmpUser += item.message.from.last_name + " ";
                                }
                                b.imMessageAdd("1", "Next message get from automatic application", true, true);
                                b.imMessageAdd("1", tmpUser + " написал в ТГ: " + Environment.NewLine + tmpMessage);
                            }                
            }

            /* // work 
            object tmp = mywebcore.core.ParseJson(b.imRecentGet());
            RootRecent tmpRootRecent = Newtonsoft.Json.JsonConvert.DeserializeObject<RootRecent>(b.lastAnswer);

            tmp = mywebcore.core.ParseJson(b.imDialogMessagesGet("chat1"));
            RootDialogMessages tmpRootDialogMessages = Newtonsoft.Json.JsonConvert.DeserializeObject<RootDialogMessages>(b.lastAnswer);
            */
            b.imMessageAdd("1", "Next message get from automatic application",true, true);
            b.imMessageAdd("1", "Test message from REST");
            /*
            Console.WriteLine(b.UpdateState());

            Console.WriteLine(b.GetLastMessages("1"));
            object tmpMess = mywebcore.core.ParseJson(b.lastAnswer);

            Console.WriteLine(b.SendMessage("1", "test message from stdio"));
            */
        }

        /// <summary>
        /// Интерактивное заполнение настроек в консоли
        /// </summary>
        /// <returns></returns>
        public static bool FillSettingsInteractive(bool bxNeed = true, bool tgNeed = true)
        {
            if (!InteractiveRun)
            {
                Console.WriteLine("This operation cant be doing in AUTOMATIC mode. Plz, remove key AUTO or SCHEDULE and check it INTERACTIVE!");
                return false;
            }                
            if (settings == null)
                settings = new Settings();

            string readStr = "";

            SettingsBitrix setBx = settings.SettingsBitrix;
            SettingsTelegram setTg = settings.SettingsTelegram;


            while (readStr == "" | readStr == null)
            {
                bool bxFirst = false;
                bool tgFirst = false;
                if (bxNeed & tgNeed)
                {
                    Console.WriteLine("Select which settings you will fill first: Bitrix('b') or Telegram('t')?");
                    readStr = Console.ReadLine();
                    if (readStr == "exit")
                    {
                        Console.WriteLine("Canceled by user.");
                        return false;
                    }
                    else if (readStr == "b" | readStr == "bitrix")
                    {
                        bxFirst = true;
                    }
                    else if (readStr == "t" | readStr == "telegram")
                    {
                        bxFirst = false;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input!");
                        readStr = "";
                        continue;
                    }
                }

                if (bxFirst)
                {
                    readStr = "b";
                    if (bxNeed)
                    {
                        // заполнять bx
                        setBx = FillSettingBitrix();
                    }
                    if (setBx != null | !bxNeed)
                    {
                        if (tgNeed)
                        {
                            // заполнять tg
                            setTg = FillSettingTelegram();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with Bitrix connection");
                        readStr = "";
                    }
                }
                else if (!bxFirst)
                {
                    readStr = "t";
                    if (tgNeed)
                    {
                        // заполнять tg
                        setTg = FillSettingTelegram();
                    }
                    if (setTg != null | !tgNeed)
                    {
                        if (bxNeed)
                        {
                            // заполнять bx
                            setBx = FillSettingBitrix();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with Telegram connection");
                        readStr = "";
                    }
                }
            

                /*
                Console.WriteLine("Select which settings you will fill first: Bitrix('b') or Telegram('t')?");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    return false;
                }                    
                else if (readStr == "b" | readStr == "bitrix")
                {
                    setBx = FillSettingBitrix();
                    if (setBx != null)
                        setTg = FillSettingTelegram();
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with Bitrix connection");
                        readStr = "";
                    }
                        
                }
                else if (readStr == "t" | readStr == "telegram")
                {
                    setTg = FillSettingTelegram();
                    if (setTg != null)
                        setBx = FillSettingBitrix();
                    else
                    {
                        Console.WriteLine("Breaking bcs has error with telegram connection");
                        readStr = "";
                    }                        
                }
                else
                {
                    Console.WriteLine("Incorrect input!");
                    readStr = "";
                }
                */
            }


            if (bxNeed)
            {
                while (setBx == null)
                {
                    readStr = "";

                    while (readStr == "" | readStr == null)
                    {
                        Console.WriteLine("Bitrix fail. Try again? (y/n)");
                        readStr = Console.ReadLine();
                        if (readStr == "y")
                            setBx = FillSettingBitrix();
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
            
            if (bxNeed)
            {
                if (setBx == null)
                {
                    Console.WriteLine("Bitrix settings fail. Exit now.");
                    return false;
                }
                else
                {
                    settings.SettingsBitrix = setBx;
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
                

            /*
            if (setBx == null)
                Console.WriteLine("Bitrix settings fail. Exit now.");
            else if (setTg == null)
                Console.WriteLine("Telegram settings fail. Exit now.");
            else
            {
                settings.SettingsBitrix = setBx;
                settings.SettingsTelegram = setTg;
                return true;
            }

            return false;
            */
        }

        /// <summary>
        /// Интерактивное заполнение настроек ТГ
        /// </summary>
        /// <returns></returns>
        public static SettingsTelegram FillSettingTelegram()
        {
            SettingsTelegram result = new SettingsTelegram();

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
                Console.WriteLine("Input TG server URL (leave blank to use default):");
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
                else if (readStr.Contains("http://") | readStr.Contains("https://"))
                {
                    result.ServerUrl = readStr;
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
        private static bool CheckSettingsTelegram(SettingsTelegram checkSet)
        {
            TelegramBot testTg = null;
            if (checkSet.API_KEY != null)
            {
                if (checkSet.API_KEY.Length > 0)
                {
                    if (checkSet.ServerUrl.Length > 0)
                        if (checkSet.ServerUrl.StartsWith("https://") | checkSet.ServerUrl.StartsWith("http://"))
                            testTg = new TelegramBot(checkSet.API_KEY, checkSet.ServerUrl);
                    // если не указан serverUrl тогда создаем Телеграм только по токену
                    if (testTg == null)
                        testTg = new TelegramBot(checkSet.API_KEY);

                    // Проверяем Телеграм
                    if (testTg.getMe() != null)
                    {
                        checkSet.ServerUrl = testTg.serverUrl;
                        return true;
                    }
                }
            }            
            return false;
        }

        /// <summary>
        /// Интерактивное заполнение настроек Битрикса в консоли
        /// </summary>
        /// <returns></returns>
        public static SettingsBitrix FillSettingBitrix()
        {
            SettingsBitrix result = new SettingsBitrix();

            string readStr = "";

            //bitrix.bitrix testBitrix;

            Console.WriteLine("BITRIX settings:");


            readStr = "";
            while (readStr == "" | readStr == null)
            {                
                Console.WriteLine("Input BX Server URL(must include http:// or https://):");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");                    
                    return null;
                }
                else if (readStr.StartsWith("http://") | readStr.StartsWith("https://"))
                {
                    result.ServerUrl = readStr;                    
                    break;
                }                    
                else
                    readStr = "";
            }

            readStr = "";
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input BX Login:");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");                    
                    return null;
                }
                else if (readStr.Length > 0)
                {
                    result.Login = readStr;
                    //readStr = "";
                    break;
                }
                else
                    readStr = "";
            }

            readStr = "";
            while (readStr == "" | readStr == null)
            {
                Console.WriteLine("Input BX Password:");
                readStr = Console.ReadLine();
                if (readStr == "exit")
                {
                    Console.WriteLine("Canceled by user.");
                    return null;
                }
                else if (readStr.Length > 0)
                {
                    result.Password = readStr;
                    //readStr = "";
                    break;
                }                    
                else
                    readStr = "";
            }


            //testBitrix = new bitrix.bitrix(result.ServerUrl, result.Login, result.Password);
                       
            Console.WriteLine("Checking BX settings ...");
            if (!CheckSettingsBitrix(result))
            {
                Console.WriteLine("Incorrect data to create bitrix connection! Check ur data and try again.");
                return null;
            }

            Console.WriteLine("Success! Bitrix is ready!");            
            return result;
        }

        /// <summary>
        /// Проверка корректноси настроек Битрикс
        /// </summary>
        /// <param name="checkSet"></param>
        /// <returns></returns>
        private static bool CheckSettingsBitrix(SettingsBitrix checkSet)
        {
            bitrix.bitrix testBx = null;
            if (checkSet.ServerUrl != null & checkSet.Login != null & checkSet.Password != null)
            {
                if (checkSet.ServerUrl.Length > 0 & checkSet.Login.Length > 0 & checkSet.Login.Length > 0)
                {
                    if (checkSet.ServerUrl.StartsWith("https://") | checkSet.ServerUrl.StartsWith("http://"))
                    {
                        if (!checkSet.ServerUrl.EndsWith("/"))
                            checkSet.ServerUrl = checkSet.ServerUrl + "/";
                        testBx = new bitrix.bitrix(checkSet.ServerUrl, checkSet.Login, checkSet.Password);
                    }
                        

                    if (testBx != null) // если все заполнено
                        if (testBx.bitrixSessionId != null) // если есть ответ от сервера
                            return true;
                }
            }            
            return false;
        }

        /// <summary>
        /// Загрузить настройки приложения из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool LoadSettings(string filename)
        {
            object tmpSet = SFileIO.LoadObjectXML(new Settings(), filename);
            if (tmpSet != null)
                if (tmpSet.GetType() == typeof(Settings))
                {
                    settings = (Settings)tmpSet;
                    return true;
                }                    
            return false;
        }

        /// <summary>
        ///  Сохранить настройки приложения в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool SaveSettings(string filename)
        {
            return SFileIO.SaveObjectXML(settings, filename);            
        }

        /// <summary>
        /// Проверка соответствия чата Битрикса чату ТГ. 
        /// Планируется шифровать имя чата битрикса и предлагать установить полученную зашифорованную строку в описание чата ТГ на время установки соответствия.
        ///  
        /// </summary>
        /// <param name="tgChatDesc"></param>
        /// <param name="bxChatName"></param>
        /// <returns></returns>
        public static bool CheckMappedChat(string tgChatDesc, string bxChatName)
        {
            bool result = false;

            result = tgChatDesc.Contains(bxChatName);

            return result;
        }

        private static bitrix.User GetUserAuthor(IEnumerable<bitrix.User>Users, string user_id)
        {
            foreach (bitrix.User item in Users)
            {
                if (item.id == user_id)
                    return item;
            }
            return null;
        }
    }
}
