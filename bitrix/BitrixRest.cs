using System;
using System.Collections.Generic;
using System.Text;

namespace bitrix
{   
    public class aAnswer
    {        
        public string Path { get; protected set; }
        public string Parameters { get; protected set; }

        public aAnswer(string path, Dictionary<string, string> parameters)
        {
            this.Path = path;
            this.Parameters = mywebcore.core.RawPostData(parameters);
        }
    }
    public class BitrixRest
    {
        public string Path { get { return "/rest/"; } }
        public string ImRecentGet { get { return "im.recent.get"; } }
        public string ImRecentPin { get { return "im.recent.pin"; } }
        public string ImRecentHide { get { return "im.recent.hide"; } }
    }

    public class BitrixRestIm : BitrixRest
    {
        new public string Path { get { return base.Path + "im"; } }
    }

    public class BitrixRestImRecent:BitrixRestIm
    {
        new public string Path { get { return base.Path + ".recent"; } }
        private Dictionary<string, string> MyParameters;

        public BitrixRestImRecent() { }         

        public aAnswer Get( bool JSON = true,
                            bool SKIP_OPENLINES = false,
                            bool SKIP_CHAT = false,
                            bool SKIP_DIALOG = false,
                            bool ONLY_OPENLINES = false,
                            string LAST_UPDATE = "19700101",
                            string LAST_SYNC_DATE = "19700101")
        {
            
            MyParameters = new Dictionary<string, string>();

            MyParameters.Add("SKIP_OPENLINES", SKIP_OPENLINES ? "Y" : "N");
            MyParameters.Add("SKIP_CHAT", SKIP_CHAT ? "Y" : "N");
            MyParameters.Add("SKIP_DIALOG", SKIP_DIALOG ? "Y" : "N");
            MyParameters.Add("ONLY_OPENLINES", ONLY_OPENLINES ? "Y" : "N");

            aAnswer result = new aAnswer(Path + ".get" + ((JSON) ? ".json" : ".xml"), MyParameters);

            return result;
        }

        public aAnswer Pin(string DIALOG_ID, bool JSON = true)
        {
            MyParameters = new Dictionary<string, string>();
            
            MyParameters.Add("DIALOG_ID", DIALOG_ID);

            return new aAnswer(Path + ".pin" + ((JSON) ? ".json" : ".xml"), MyParameters);
        }

    }

    public class BitrixRestImDialog : BitrixRestIm
    {
        new public string Path { get { return base.Path + ".dialog"; } }
    }
    public class BitrixRestImDialogMessages : BitrixRestImDialog
    {
        new public string Path { get { return base.Path + ".messages"; } }
        private Dictionary<string, string> MyParameters;
        public aAnswer MessagesGet( string DIALOG_ID,
                                    bool JSON = true,
                                    long LAST_ID = -1,
                                    long FIRST_ID = -1,
                                    int LIMIT = 20)
        {
            MyParameters = new Dictionary<string, string>();

            MyParameters.Add("DIALOG_ID", DIALOG_ID);
            if (LAST_ID > 0)
                MyParameters.Add("LAST_ID", LAST_ID.ToString());
            if (FIRST_ID >= 0)
                MyParameters.Add("FIRST_ID", FIRST_ID.ToString());
            if (LIMIT > 0)
                MyParameters.Add("LIMIT", LIMIT.ToString());

            return new aAnswer(Path + ".get" + ((JSON) ? ".json" : ".xml"), MyParameters);
        }

        public aAnswer MessagesGet(string DIALOG_ID,
                                    bool JSON = true)
        {
            return MessagesGet(DIALOG_ID, JSON, -1, -1, -1);
        }

        public aAnswer MessagesGet(string DIALOG_ID,
                                    int LIMIT = 20,
                                    bool JSON = true)
        {
            return MessagesGet(DIALOG_ID, JSON, -1, -1, LIMIT);
        }
    }
    
}
