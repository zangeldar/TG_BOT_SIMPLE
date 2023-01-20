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
        /// Get message from srcChat by urlToSrcMessage
        /// </summary>
        /// <param name="urlToSrcMessage"></param>
        /// <returns></returns>
        public static string ForwardMessageToChat(TelegramBot tgBot, string urlToSrcMessage, string urlToDstChat, bool pin=false)
        {
            string result = "";

            string dst_chat_id = "";
            string src_chat_id = "";
            long src_message_id = 0;

            List<string> tmp = GetChatIdFromUrl(urlToDstChat);
            if (tmp.Count > 0)
            {
                dst_chat_id = "@" + tmp[0];

                tmp = GetChatIdFromUrl(urlToSrcMessage);
                if (tmp.Count > 1)
                {
                    src_chat_id = "@" + tmp[0];
                    if (long.TryParse(tmp[1], out src_message_id))
                    {
                        result = tgBot.copyMessage(dst_chat_id, src_chat_id, src_message_id);                        

                        tgRootCopyMessage tmpRootCopyMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootCopyMessage>(tgBot.LastAnswer);
                        if (tmpRootCopyMessage != null)
                        {
                            settings.LastForwardedMessageId = tmpRootCopyMessage.result.message_id;
                            settings.LastChatId = dst_chat_id;
                            if (pin)
                                result = tgBot.pinChatMessage(dst_chat_id, tmpRootCopyMessage.result.message_id);
                        }
                            
                    }
                }
            }

            return result;
        }

        public static string RemoveUnPinMessage(TelegramBot tgBot, string dst_chat_id, long MessageId, bool remove=false)
        {
            string result = "";

            result = tgBot.unpinChatMessage(dst_chat_id, MessageId);
            if (result != null)
            {
                result = tgBot.deleteMessage(dst_chat_id, MessageId + 1); // remove notification about pinned message
                if (remove)
                    result = tgBot.deleteMessage(dst_chat_id, MessageId); // remove message if needed
            }

            return result;
        }
                
        /// <summary>
        /// Example: https://t.me/mersin_community_adpool/2
        /// </summary>
        /// <param name="inputUrl"></param>
        /// <returns></returns>
        public static List<string> GetChatIdFromUrl(string inputUrl)
        {
            List<string> result = new List<string>();

            int k = 0;
            foreach (string item in inputUrl.Split('/'))
            {
                if (k > 2)
                    result.Add(item);
                k++;       
            }

            return result;
        }

    }
}
