using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tglib;

namespace TEST_tgBotMersin
{
    public class testTG:test
    {
        string api_key = "PLACE_HERE_YOUR_TG_BOT_API_KEY";
        string src_link_example = "https://t.me/src_chat_id/31337";         // direct link to src message in src chat "https://t.me/src_chat_id/31337"
        string src_chat_id = "PLACE_HERE_SRC_TG_CHAT_ID";                   // src chat id "@src_chat_id"
        string src_message_id = "31337";                                    // message id in current chat "31337"

        string dst_link_example = "https://t.me/dst_chat_id/";    // direct link to dst chat "https://t.me/dst_chat_id"
        string dst_chat_id = "@dst_chat_id";                      // dst chat id "@dst_chat_id"


        //string src_message_id = "2";

        public string DoTest()
        {
            TelegramBot tgBot = new TelegramBot(api_key);

            dynamic tmp;

            //tmp = tgBot.getMe();

            //tmp = tgBot.getUpdates();

            //tmp = tgBot.getChat(src_chat_id);

            tmp = tgBot.copyMessage(dst_chat_id, src_chat_id, 2);

            //tmp = mywebcore.core.ParseJson(tgBot.LastAnswer);

            //tgRoot tgRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRoot>(tgBot.LastAnswer);
            tgRootCopyMessage tmpRootCopyMessage = null;
            if (tgBot.LastAnswer != null)
                tmpRootCopyMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootCopyMessage>(tgBot.LastAnswer);
            if (tmpRootCopyMessage != null)
                tmp = tgBot.pinChatMessage(dst_chat_id, tmpRootCopyMessage.result.message_id);

            return tmp;
        }
    }

        
}
