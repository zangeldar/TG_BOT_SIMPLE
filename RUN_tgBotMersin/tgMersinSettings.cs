using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgBotMersin
{
    [Serializable]
    public class tgMersinSettings
    {
        public TelegramSettings SettingsTelegram { get; set; }
        public GoogleSettings SettingsGoogle { get; set; }
        public string LastUsedUrl { get; set; }
        public string LastUsedRangeGoogleSheet { get; set; }
        public string LastChatId { get; set; }
        public long LastForwardedMessageId { get; set; }


        /// <summary>
        /// Загрузить настройки приложения из файла
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool LoadSettings(string filename)
        {
            object tmpSet = SFileIO.LoadObjectXML(new tgMersinSettings(), filename);
            if (tmpSet != null)
                if (tmpSet.GetType() == typeof(tgMersinSettings))
                {
                    tgMersinWorker.settings = (tgMersinSettings)tmpSet;
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
            return SFileIO.SaveObjectXML(tgMersinWorker.settings, filename);
        }

    }

    public class TelegramSettings
    {
        //public string UrlToSrcChat { get; set; }
        public string UrlToDstChat { get; set; }
        public string API_KEY { get; set; }
    }

    public class GoogleSettings
    {
        public string UrlToTable { get; set; }
        public string GoogleApiKey { get; set; }
    }


}
