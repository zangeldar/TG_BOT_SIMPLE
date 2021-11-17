using System;
using System.Collections.Generic;
using System.Text;
using bitrix;
using tglib;

namespace MessengerCore
{
    public class Deserializator
    {

    }

    public static class DeserializatorBitrix // : Deserializator
    {
        //RootRecent tmpRootRecent = Newtonsoft.Json.JsonConvert.DeserializeObject<RootRecent>(b.lastAnswer);

        public static bitrix.RootRecent RootRecent(bitrix.bitrix b)
        {
            b.imRecentGet();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<bitrix.RootRecent>(b.lastAnswer);
        }
    }

    public class DeserializatorTelegram // : Deserializator
    {

    }
}
