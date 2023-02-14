// See https://aka.ms/new-console-template for more information
using tgBotMersin;
using tglib;



string tmpFileName = "";
// проверка, передали ли в качестве аргумента имя файла настроек
if (args.Length > 0)
    foreach (string arg in args)
    {
        if (arg.Contains("settings="))
        {
            Console.WriteLine("Key " + arg + "found");
            tmpFileName = arg.Replace("settings=", "");
        }
        else if (arg == "schedule" | arg == "auto")
        {
            Console.WriteLine("Key " + arg + "found");
            tgMersinWorker.InteractiveRun = false;
        }
        else if (arg == "debug")
        {
            Console.WriteLine("Key " + arg + "found");
            tgMersinWorker.Debug = true;            
        }
        else if (arg == "nopin")
        {
            Console.WriteLine("Key " + arg + "found");
            tgMersinWorker.NoPin = true;
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
    if (tgMersinSettings.LoadSettings(tmpFileName))
        tgMersinWorker.settingsFileName = tmpFileName;
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
        tgMersinWorker.settingsFileName = tmpFileName;
    }
}

// проверяем, удалось ли сохранить имя файла. Если нет, то используем имя по умолчанию
if (tgMersinWorker.settingsFileName.Length == 0)
    tgMersinWorker.settingsFileName = tgMersinWorker.defaultSettingsFileName;

// проверяем, существет ли файл настроек
bool goodSettings = false;
if (System.IO.File.Exists(tgMersinWorker.settingsFileName))
    if (tgMersinSettings.LoadSettings(tgMersinWorker.settingsFileName))
        goodSettings = true;

if (!goodSettings)
{
    Console.WriteLine("Settings not found! Looks like it is a first executing!");
    if (tgMersinWorker.FillSettingsInteractive())
    {
        if (!tgMersinSettings.SaveSettings(tgMersinWorker.settingsFileName))
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
    // С файлом настроек все в порядке, если нужно - здесь проверяем работоспособность самих настроек
    // Используем флаг goodSettings при проверке
    goodSettings = tgMersinWorker.CheckSettingsTelegram(tgMersinWorker.settings.SettingsTelegram) 
        & tgMersinWorker.CheckSettingsGoogle(tgMersinWorker.settings.SettingsGoogle);        

    // если после всех проверок что-то пошло не так, то предлагаем заполнить настройки интерактивно
    if (!goodSettings)
    {
        if (!tgMersinWorker.FillSettingsInteractive())
        {
            Console.WriteLine("Something worng with stored settings. Cant continue. Exit.");
            return;
        }
    }
}



Console.WriteLine("Starting tgBotMersin..");

string prevRange = tgMersinWorker.settings.LastUsedRangeGoogleSheet;
Console.WriteLine("Last used range: " + prevRange);

string src_msg;
src_msg = tgMersinWorker.GetLinkFromTable(tgMersinWorker.settings.SettingsGoogle.UrlToTable, DateTime.Now.AddHours(0));
//src_msg = tgMersinWorker.GetLinkFromTable(tgMersinWorker.settings.SettingsGoogle.UrlToTable, DateTime.Parse("2023-01-24T02:22:01.0000000+03:00"));
Console.WriteLine("Get data from GoogleSheet range [" + tgMersinWorker.settings.LastUsedRangeGoogleSheet + "]: " + src_msg);

if (prevRange != tgMersinWorker.settings.LastUsedRangeGoogleSheet)
{
    Console.WriteLine("Something new, let work with TG..");
    if (src_msg.StartsWith("https://t.me/") |
        src_msg == "clear" |
        src_msg == "unpin")
    {
        TelegramBot tgBot = new TelegramBot(
            tgMersinWorker.settings.SettingsTelegram.API_KEY);
        //dynamic trr = tgBot.getUpdates();        

        //Console.WriteLine("Get \"" + src_msg + "\" command to proceed with previous message..");
        Console.WriteLine("Proceed with previous message..");
        if (tgMersinWorker.settings.LastChatId != null &
            tgMersinWorker.settings.LastForwardedMessageId != null)
            if (tgMersinWorker.settings.LastChatId != "" &
                tgMersinWorker.settings.LastForwardedMessageId != 0)
                Console.WriteLine(
                    tgMersinWorker.RemoveUnPinMessage(
                        tgBot,
                        tgMersinWorker.settings.LastChatId,
                        tgMersinWorker.settings.LastForwardedMessageId,
                        src_msg == "clear")
                    );

        if (src_msg.StartsWith("https://t.me/"))
        {
            Console.WriteLine("Forwarding new message..");
            Console.WriteLine(
                tgMersinWorker.ForwardMessageToChat(
                    tgBot, src_msg,
                    tgMersinWorker.settings.SettingsTelegram.UrlToDstChat,
                    !tgMersinWorker.NoPin)
                );
        }
    }

}

// exit routines
tgMersinSettings.SaveSettings(tgMersinWorker.settingsFileName);

Console.WriteLine("Gule-gule");




/// string srcLink = GetLinkFromTable("link_to_googlesheet");
/// if (srcLink=="clear")
/// {
///     unpinMessage(dstChat, Settings.LastMessageId);
///     removeMessage(dstChat, Settings.LastMessageId);
///     Settings.LastMessageId = -1;
/// } else {
///     dyn res = tryParse(srcLink);
///     if (res.result == true)
///     {
///         LastMessageId = CopyMessage(res.srcChat, res.srcMsg, dstChat);
///         PinMessage(dstChat, LastMessageId);
///         Settings.LastMessageId = LastMessageId;
///     } else {
///         // do nothing
///     }
/// }
///     

