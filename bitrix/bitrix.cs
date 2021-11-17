using mywebcore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace bitrix
{
    public class bitrix:core
    {
        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public Exception lastError { get; private set; }
        public string lastAnswer { get; private set; }

        public string bitrixSessionId { get; private set; }
        
        public string login { get; private set; }
        public string password { get; private set; }

        public CookieContainer Cookies { get; private set; }

        public bitrix(string serverURL, string login, string password):base()
        {
            this.serverURL = serverURL;
            this.login = login;
            this.password = password;

            this.Rest = new BitrixRest();

            initialize();
        }

        const string authPath = "desktop_app/login/?mode=2w";
        const string commandGetLastMessagesFromChat = "desktop_app/im.ajax.php?LOAD_LAST_MESSAGE&";
        const string commandSendMessage = "desktop_app/im.ajax.php?MESSAGE_SEND&";
        const string commandUpdateState = "desktop_app/im.ajax.php?UPDATE_STATE&";

        

        private void initialize()
        {
            Cookies = new CookieContainer();

            parameters = new Dictionary<string, string>();
            parameters.Add("action", "login");
            parameters.Add("login", login);
            parameters.Add("password", password);
            
            sendRequest(serverURL, authPath, RawPostData(parameters));
            if (lastAnswer != null)
            {
                //base.ParseJson(lastAnswer);
                bitrixSessionId = GetTokenFromJsonByName(lastAnswer, "bitrixSessionId");
            }            
        }

        public BitrixRest Rest;
        private string sendRequest(string serverURL, string pathURL, string postData="")
        {
            /*
            ServicePointManager.Expect100Continue = true;
            */
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
              /*     | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;
                   */
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            pathURL = pathURL.Replace("+", "%2B");
            pathURL = pathURL.Replace(" ", "+");
            if (pathURL.StartsWith("/") & serverURL.EndsWith("/"))
                serverURL = serverURL.Remove(serverURL.Length - 1);
            var request = (HttpWebRequest)WebRequest.Create(serverURL + pathURL);

            request.CookieContainer = Cookies;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //request.ContentLength = data.Length;            
            request.SendChunked = true;

            //request.Headers.Add("Accept", "*/*");
            request.Accept = "*/*";
            //request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //request.Headers.Add("X-MicrosoftAjax", "Delta=true");
            //request.Headers.Add("Cache-Control", "no-cache");
            //request.Headers.Add("Referer",              "https://lot-online.ru/");
            //request.Referer = serverURL;
            request.Headers.Add("Accept-Language", "ru-RU");    //Accept-Language:ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7
            //request.Headers.Add("User-Agent",           "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");            

            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            /*
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
             */

            

            HttpWebResponse response;

            try
            {
                //using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), new UTF8Encoding()))
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    //writer.WriteLine(postData);
                    writer.Write(postData);
                }
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                lastError = e;
                return null;
                //throw;
            }

            lastAnswer = new StreamReader(response.GetResponseStream()).ReadToEnd();    // put result in lastAnswer to cache   

            response.Dispose();

            return lastAnswer;
            // login
            // "{success: true, desktopRevision: 5, userId: '160', sessionId: 'n6mPXHk3OEnTEA45HceOnN6x2kd2qqIk', bitrixSessionId: '12f9df7804aa5ce42ca56d1d4b037b49'}"

            // UPDATE_STATE
            // "{"REVISION":130,"MOBILE_REVISION":19,"DISK_REVISION":"6","RECENT":[],"LINES_LIST":[],"COUNTERS":{"**":0,"crm_all":1,"crm_all_no_orders":1,"crm_company_all":0,"crm_contact_all":0,"crm_contact_overdue":0,"crm_contact_pending":0,"crm_deal_all":1,"crm_deal_c0_all":1,"crm_deal_c0_idle":1,"crm_deal_c0_overdue":0,"crm_deal_c0_pending":0,"crm_deal_idle":1,"crm_deal_overdue":0,"crm_deal_pending":0,"crm_lead_all":0,"crm_order_all":0,"crm_order_idle":0,"crm_order_overdue":0,"crm_order_pending":0,"tasks_accomplices":0,"tasks_auditor":1,"tasks_effective":100,"tasks_my":0,"tasks_originator":1,"tasks_total":0,"shop_all":0},"CHAT_COUNTERS":{"type":{"all":0,"notify":0,"dialog":0,"chat":0,"lines":0,"mail":0},"dialog":[],"dialogUnread":[],"chat":[],"chatMuted":[],"chatUnread":[],"lines":[]},"NOTIFY_LAST_ID":147106,"ONLINE":{"1":{"id":"1","status":"online","color":"#df532d","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":false,"absent":false},"10":{"id":"10","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T15:44:50+03:00","mobile_last_date":"2021-10-11T13:54:02+03:00","absent":false},"12":{"id":"12","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":false,"absent":false},"16":{"id":"16","status":"online","color":"#64a513","idle":false,"last_activity_date":"2020-08-10T10:31:07+03:00","mobile_last_date":false,"absent":false},"18":{"id":"18","status":"online","color":"#728f7a","idle":false,"last_activity_date":"2020-07-31T14:20:23+03:00","mobile_last_date":"2020-07-31T14:20:23+03:00","absent":false},"20":{"id":"20","status":"online","color":"#ab7761","idle":"2021-10-21T15:49:21+03:00","last_activity_date":"2021-10-21T16:03:01+03:00","mobile_last_date":false,"absent":false},"22":{"id":"22","status":"online","color":"#64a513","idle":"2021-10-21T15:42:28+03:00","last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-04T11:23:43+03:00","absent":false},"24":{"id":"24","status":"online","color":"#29619b","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-18T15:31:43+03:00","absent":false},"26":{"id":"26","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-21T15:33:08+03:00","mobile_last_date":"2021-10-14T15:50:58+03:00","absent":false},"32":{"id":"32","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T15:54:10+03:00","mobile_last_date":"2021-10-21T15:54:10+03:00","absent":false},"40":{"id":"40","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T16:05:14+03:00","mobile_last_date":false,"absent":false},"44":{"id":"44","status":"online","color":"#4ba5c3","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-20T11:55:34+03:00","absent":false},"54":{"id":"54","status":"online","color":"#4ba5c3","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-20T17:30:39+03:00","absent":false},"66":{"id":"66","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-21T09:25:08+03:00","mobile_last_date":"2021-10-21T09:25:08+03:00","absent":false},"70":{"id":"70","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T15:44:50+03:00","mobile_last_date":"2021-10-21T15:44:50+03:00","absent":false},"78":{"id":"78","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-06-04T16:31:32+03:00","mobile_last_date":"2019-02-07T10:13:43+03:00","absent":false},"84":{"id":"84","status":"online","color":"#4ba5c3","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-08-25T09:15:19+03:00","absent":false},"86":{"id":"86","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2020-04-06T16:41:18+03:00","absent":false},"88":{"id":"88","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-10-21T16:04:31+03:00","mobile_last_date":false,"absent":false},"92":{"id":"92","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T15:40:13+03:00","mobile_last_date":"2021-10-21T15:40:13+03:00","absent":false},"112":{"id":"112","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-19T21:32:50+03:00","absent":false},"116":{"id":"116","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-21T15:27:00+03:00","mobile_last_date":"2021-10-21T15:27:00+03:00","absent":false},"118":{"id":"118","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-10-21T15:56:31+03:00","mobile_last_date":"2021-10-21T15:56:31+03:00","absent":false},"120":{"id":"120","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T14:00:09+03:00","mobile_last_date":"2021-10-21T13:58:41+03:00","absent":false},"126":{"id":"126","status":"online","color":"#df532d","idle":false,"last_activity_date":"2021-10-20T22:24:49+03:00","mobile_last_date":"2021-10-20T22:24:49+03:00","absent":false},"130":{"id":"130","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":false,"absent":false},"136":{"id":"136","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-21T15:35:54+03:00","mobile_last_date":"2021-10-21T15:35:54+03:00","absent":false},"142":{"id":"142","status":"online","color":"#64a513","idle":"2021-10-21T14:27:03+03:00","last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":false,"absent":false},"150":{"id":"150","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T12:41:30+03:00","mobile_last_date":"2021-10-21T12:41:30+03:00","absent":false},"156":{"id":"156","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-08-31T11:18:20+03:00","mobile_last_date":"2021-08-31T11:18:20+03:00","absent":false},"158":{"id":"158","status":"online","color":"#728f7a","idle":false,"last_activity_date":"2021-04-02T16:58:23+03:00","mobile_last_date":"2021-03-01T12:24:28+03:00","absent":false},"160":{"id":"160","status":"online","color":"#ab7761","idle":false,"last_activity_date":"2021-10-21T16:06:04+03:00","mobile_last_date":"2021-10-21T12:02:58+03:00","absent":false},"162":{"id":"162","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T16:04:59+03:00","mobile_last_date":false,"absent":false},"166":{"id":"166","status":"online","color":"#8474c8","idle":false,"last_activity_date":"2021-10-15T16:28:34+03:00","mobile_last_date":false,"absent":false},"168":{"id":"168","status":"online","color":"#728f7a","idle":false,"last_activity_date":"2021-02-25T08:23:21+03:00","mobile_last_date":"2021-02-23T13:50:12+03:00","absent":false},"178":{"id":"178","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-10-21T13:02:53+03:00","mobile_last_date":"2021-05-14T12:42:41+03:00","absent":false},"188":{"id":"188","status":"online","color":"#f76187","idle":false,"last_activity_date":"2021-10-20T12:27:21+03:00","mobile_last_date":"2021-10-20T12:27:21+03:00","absent":false},"192":{"id":"192","status":"online","color":"#64a513","idle":false,"last_activity_date":"2021-10-21T16:02:43+03:00","mobile_last_date":"2021-10-21T15:33:37+03:00","absent":false}},"XMPP_STATUS":"N","DESKTOP_STATUS":"Y","INTRANET_USTAT_ONLINE_DATA":[],"SERVER_TIME":1634821564,"LAST_UPDATE":"2021-10-21T16:06:04+03:00","ERROR":""}"
            
            // GET_LAST_MESSAGES
            // "{'REVISION':'130','MOBILE_REVISION':'19','CHAT_ID':'3588','DISK_FOLDER_ID':'0','USER_ID':'1','MESSAGE':{'145552':{'id':'145552','chatId':'3588','senderId':'160','recipientId':'1','system':'N','date':'2021-10-04T12:50:19+03:00','text':'I tried to &nbsp;send this message a few days later and.. check how it works','textOriginal':'I tried to  send this message a few days later and.. check how it works','params':[]},'145454':{'id':'145454','chatId':'3588','senderId':'1','recipientId':'160','system':'N','date':'2021-10-01T16:01:38+03:00','text':'from user... ueah, i\'m edit this message and remove &quot;answer&quot;','textOriginal':'from user... ueah, i\'m edit this message and remove \"answer\"','params':{'IS_EDITED':'Y'}},'145452':{'id':'145452','chatId':'3588','senderId':'160','recipientId':'1','system':'N','date':'2021-10-01T16:00:43+03:00','text':'my second <br />test message from request','textOriginal':'my second \ntest message from request','params':[]},'145450':{'id':'145450','chatId':'3588','senderId':'160','recipientId':'1','system':'N','date':'2021-10-01T16:00:23+03:00','text':'my first test message from request','textOriginal':'my first test message from request','params':[]},'145432':{'id':'145432','chatId':'3588','senderId':'160','recipientId':'1','system':'N','date':'2021-10-01T15:27:41+03:00','text':'test','textOriginal':'test','params':[]},'145430':{'id':'145430','chatId':'3588','senderId':'160','recipientId':'1','system':'N','date':'2021-10-01T15:27:37+03:00','text':'тест','textOriginal':'тест','params':[]}},'USERS_MESSAGE':{'1':['145552','145454','145452','145450','145432','145430']},'UNREAD_MESSAGE':[],'USERS':{'1':{'id':'1','name':'Администратор','active':true,'first_name':'Администратор','last_name':'','work_position':'','color':'#df532d','avatar':'https://bitrix2.cdnvideo.ru/b9448547/resize_cache/3586/ff58db95aecdfa09ae61b51b5fd8f63f/main/800/8006a8d9c1534821aa40a198e36ab389/45.jpg?h=stels-ug.bitrix24.ru','avatar_id':'3586','birthday':false,'gender':'M','phone_device':false,'phones':false,'extranet':false,'tz_offset':'0','network':false,'bot':false,'connector':false,'profile':'/company/personal/user/1/','external_auth_id':'socservices','status':'online','idle':false,'last_activity_date':'2021-10-05T11:37:34+03:00','mobile_last_date':false,'desktop_last_date':'2020-07-02T11:39:12+03:00','departments':['9'],'absent':false,'services':''},'160':{'id':'160','name':'Эльдар','active':true,'first_name':'Эльдар','last_name':'','work_position':'','color':'#ab7761','avatar':'https://bitrix2.cdnvideo.ru/b9448547/resize_cache/3442/ff58db95aecdfa09ae61b51b5fd8f63f/main/60c/60ce7e0f5bb78c097ae27961192d7161/avatar.png?h=stels-ug.bitrix24.ru','avatar_id':'3442','birthday':false,'gender':'M','phone_device':false,'phones':false,'extranet':false,'tz_offset':'0','network':false,'bot':false,'connector':false,'profile':'/company/personal/user/160/','external_auth_id':'socservices','status':'online','idle':false,'last_activity_date':'2021-10-18T16:52:22+03:00','mobile_last_date':'2021-10-15T13:58:41+03:00','desktop_last_date':'2021-10-18T16:47:28+03:00','departments':['9'],'absent':false,'services':''}},'USER_IN_GROUP':{'9':{'id':'9','users':['1','160']}},'CHAT':[],'USER_BLOCK_CHAT':[],'USER_IN_CHAT':[],'USER_LOAD':'Y','READED_LIST':{'1':{'messageId':'145454','date':'2021-10-01T16:01:38+03:00'}},'PHONES':[],'FILES':[],'LINES':[],'OPENLINES':[],'NETWORK_ID':'','ERROR':''}"

            // MESSAGE_SEND
            // "{'TMP_ID':'temp0','ID':'147644','CHAT_ID':'3588','SEND_DATE':'2021-10-21T16:46:49+03:00','SEND_MESSAGE':'test message from stdio','SEND_MESSAGE_PARAMS':[],'SEND_MESSAGE_FILES':[],'SENDER_ID':'160','RECIPIENT_ID':'1','OL_SILENT':'N','ERROR':''}"
        }

        public string GetLastMessages(string dialogId)
        {
            Dictionary<string, string> pathPar = new Dictionary<string, string>();
            pathPar.Add("D", dialogId);
            pathPar.Add("V", "130");

            Dictionary<string, string> postPar = new Dictionary<string, string>();
            postPar.Add("IM_LOAD_LAST_MESSAGE", "Y");
            if (dialogId.Contains("chat"))
                postPar.Add("CHAT", "Y");
            else
                postPar.Add("CHAT", "N");
            postPar.Add("USER_ID", dialogId);
            postPar.Add("USER_LOAD", "Y");
            postPar.Add("IM_AJAX_CALL", "Y");
            postPar.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, commandGetLastMessagesFromChat+RawPostData(pathPar), RawPostData(postPar));
        }

        public string UpdateState()
        {
            Dictionary<string, string> pathPar = new Dictionary<string, string>();            
            pathPar.Add("V", "130");

            Dictionary<string, string> postPar = new Dictionary<string, string>();
            postPar.Add("IM_UPDATE_STATE", "Y");            
            postPar.Add("IM_AJAX_CALL", "Y");
            postPar.Add("IS_OPERATOR", "N");
            postPar.Add("IS_DESKTOP", "Y");
            //postPar.Add("RECENT_LAST_UPDATE", "2021-10-21T14:34:10+03:00");
            postPar.Add("LINES_LAST_UPDATE", "N");
            postPar.Add("TAB", "150");
            postPar.Add("SITE_ID", "s1");
            postPar.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, commandUpdateState + RawPostData(pathPar), RawPostData(postPar));
        }

        public string SendMessage(string dialogId, string message)
        {
            Dictionary<string, string> pathPar = new Dictionary<string, string>();
            pathPar.Add("V", "130");
            pathPar.Add("logTag", "in.message.add");
            pathPar.Add("timType", "private");
            pathPar.Add("timDevice", "bitrixDesktop");
            

            Dictionary<string, string> postPar = new Dictionary<string, string>();
            postPar.Add("IM_SEND_MESSAGE", "Y");
            if (dialogId.Contains("chat"))
                postPar.Add("CHAT", "Y");
            else
                postPar.Add("CHAT", "N");
            postPar.Add("ID", "temp0");
            postPar.Add("RECIPIENT_ID", dialogId);            
            postPar.Add("MESSAGE", message);
            postPar.Add("OL_SILENT", "N");
            postPar.Add("TAB", "1");
            postPar.Add("USER_TZ_OFFSET", "0");
            postPar.Add("IM_AJAX_CALL", "Y");
            postPar.Add("FOCUS", "N");
            postPar.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, commandUpdateState + RawPostData(pathPar), RawPostData(postPar));
        }

        public string imRecentGet(bool JSON = true,
                            bool SKIP_OPENLINES = false,
                            bool SKIP_CHAT = false,
                            bool SKIP_DIALOG = false,
                            bool ONLY_OPENLINES = false,
                            string LAST_UPDATE = "19700101",
                            string LAST_SYNC_DATE = "19700101")
        {
            Dictionary<string,string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("SKIP_OPENLINES", SKIP_OPENLINES ? "Y" : "N");
            MyParameters.Add("SKIP_CHAT", SKIP_CHAT ? "Y" : "N");
            MyParameters.Add("SKIP_DIALOG", SKIP_DIALOG ? "Y" : "N");
            MyParameters.Add("ONLY_OPENLINES", ONLY_OPENLINES ? "Y" : "N");

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.recent.get" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));

            /*
            BitrixRestImRecent tmp = new BitrixRestImRecent();
            aAnswer ans = tmp.Get();
            
            return sendRequest(serverURL, ans.Path, ans.Parameters);
            */


        }

        /*
        public string imDialogMessagesGet(string DIALOG_ID, 
                                    bool JSON=true,
                                    string LAST_ID="",
                                    string FIRST_ID="",
                                    string LIMIT = "")
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("DIALOG_ID", DIALOG_ID);
            MyParameters.Add("LAST_ID", LAST_ID);
            MyParameters.Add("FIRST_ID", FIRST_ID);
            MyParameters.Add("LIMIT", LIMIT);

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.dialog.messages.get" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }
        */

        public string imMessageAdd(string DIALOG_ID,
                                string MESSAGE,
                                bool JSON = true,
                                bool SYSTEM = false,
                                string ATTACH = "",
                                bool URL_PREVIEW = true,
                                string KEYBOARD = "",
                                string MENU = "")
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("DIALOG_ID", DIALOG_ID);
            MyParameters.Add("MESSAGE", MESSAGE);
            MyParameters.Add("SYSTEM", SYSTEM ? "Y" : "N");
            MyParameters.Add("ATTACH", ATTACH);
            MyParameters.Add("URL_PREVIEW", URL_PREVIEW ? "Y" : "N");
            MyParameters.Add("KEYBOARD", KEYBOARD);
            MyParameters.Add("MENU", MENU);

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.message.add" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }

        public string imDiskFolderGet(string CHAT_ID, bool JSON = true)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("CHAT_ID", CHAT_ID);

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.disk.folder.get" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }

        /// <summary>
        /// Файл должен быть предварительно загружен через метод disk.folder.uploadfile.
        /// Для успешного вызова API нужно указать CHAT_ID и одно из двух полей – UPLOAD_ID или DISK_ID
        /// </summary>
        /// <param name="CHAT_ID"></param>
        /// <param name="UPLOAD_ID">
        /// Если UPLOAD = TRUE, то Идентификатор загруженного файла через методы модуля DISK
        /// Иначе Идентификатор файла, доступного из локального диска
        /// </param>
        /// <param name="UPLOAD">Селектор между UPLOAD_ID и DISK_ID</param>
        /// <param name="MESSAGE">Описание файла будет опубликовано в чате</param>
        /// <param name="SILENT_MODE">Параметр для чата Открытых линий, обозначает, будет ли отправлена информация о файле клиенту или нет</param>
        /// <param name="JSON"></param>
        /// <returns></returns>
        public string imDiskFileCommit(string CHAT_ID, string UPLOAD_ID, bool UPLOAD=true, string MESSAGE="", bool SILENT_MODE=false, bool JSON = true)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("CHAT_ID", CHAT_ID);
            MyParameters.Add(((UPLOAD) ? "UPLOAD_ID" : "DISK_ID"), UPLOAD_ID);
            MyParameters.Add("MESSAGE", MESSAGE);
            MyParameters.Add("SILENT_MODE", SILENT_MODE ? "Y" : "N");
            
            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.disk.file.commit" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }

        /// <summary>
        /// НЕ РЕАЛИЗОВАНО
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fileContent"></param>
        /// <param name="data"></param>
        /// <param name="JSON"></param>
        /// <returns></returns>
        public string diskFolderUploadFile(string id, string fileContent, string data, bool JSON=true)
        {
            throw new NotImplementedException();

            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("ID", id);            
            MyParameters.Add("DATA", data);
            MyParameters.Add("FILE_CONTENT", fileContent);

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/disk.folder.uploadfile" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }

        public string imUserGet(string ID="", bool AVATAR_HR = false, bool JSON = true)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            if (ID != null & ID.Length > 0)
                MyParameters.Add("ID", ID);        
            if (AVATAR_HR)
                MyParameters.Add("AVATAR_HR", AVATAR_HR ? "Y" : "N");

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest(serverURL, "/rest/im.user.get" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));
        }

        public string imDialogMessagesGet(string DIALOG_ID,
                                    bool JSON = true,
                                    long LAST_ID = -1,
                                    long FIRST_ID = -1,
                                    int LIMIT = 20)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("DIALOG_ID", DIALOG_ID);
            if (LAST_ID > 0)
                MyParameters.Add("LAST_ID", LAST_ID.ToString());
            if (FIRST_ID >= 0)
                MyParameters.Add("FIRST_ID", FIRST_ID.ToString());
            if (LIMIT > 0)
                MyParameters.Add("LIMIT", LIMIT.ToString());

            MyParameters.Add("sessid", bitrixSessionId);

            return sendRequest( serverURL, "/rest/im.dialog.messages.get" + ((JSON) ? ".json" : ".xml"), RawPostData(MyParameters));            
        }
    }
}
