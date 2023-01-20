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
        string api_key = "5848512175:AAH5WZlB-1uBxqoUDQJysu1HPOIeztzOCRc";
        string src_link_example = "https://t.me/mersin_community_adpool/2";
        string src_chat_id = "@mersin_community_adpool";
        string src_message_id = "2";

        string dst_link_example = "https://t.me/mersin_community_test/";
        string dst_chat_id = "@mersin_community_test";


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
            tgRootCopyMessage tmpRootCopyMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<tgRootCopyMessage>(tgBot.LastAnswer);
            if (tmpRootCopyMessage != null)
                tmp = tgBot.pinChatMessage(dst_chat_id, tmpRootCopyMessage.result.message_id);

            return tmp;
        }
    }

        
}
