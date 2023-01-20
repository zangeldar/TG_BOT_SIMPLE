using System;
using System.Collections.Generic;
using System.IO;
using System.Net;


namespace tglib
{
    public class TelegramBot
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            return UnixTimeStampToDateTime((double)unixTimeStamp);
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public Exception LastError { get; private set; }        
        public string LastErrorResponse { get; private set; }
        public string LastRequest { get; private set; }
        public string LastAnswer { get; private set; }
        private string token;
        public string serverUrl { get; private set; }

        public TelegramBot(string token)
        {
            this.token = token;
            this.serverUrl = "https://api.telegram.org/";
        }
        public TelegramBot(string token, string serverUrl)
        {
            this.token = token;
            if (!serverUrl.EndsWith("/"))
                serverUrl = serverUrl + "/";
            this.serverUrl = serverUrl;
        }

        public string postBase { get => serverUrl + "bot" + token + "/"; }


        private string sendRequest(string postData)
        {
            /*
            ServicePointManager.Expect100Continue = true;
            */
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
/*                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;
                   */
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            string result = "";

            LastRequest = postData.Replace(token, "#api_token_replace#");
            var req = (HttpWebRequest)WebRequest.Create(postData);            

            HttpWebResponse response = null;
            
            try
            {
                response = (HttpWebResponse)req.GetResponse();
                LastError = null;
                LastErrorResponse = null;
            }
            catch (Exception e)
            {
                LastError = e;
                //return null; // вернем после обработки исключения для получения расширенного ответа от сервера
                //throw;
            }

            
            if (LastError != null)
            {    
                if (LastError is WebException)
                {
                    LastErrorResponse = new StreamReader((LastError as WebException).Response.GetResponseStream()).ReadToEnd();
                }                
                return null;
            }              

            result = new StreamReader(response.GetResponseStream()).ReadToEnd();

            LastAnswer = result;

            return result;
        }

        // Here is public API methods:
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getMe()
        {
            return sendRequest(postBase + "getMe");
            // result must be PRIVEDEN to USER object-type
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string logOut()
        {
            return sendRequest(postBase + "logOut");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string close()
        {
            return sendRequest(postBase + "close");
        }

        public string sendMessage(string chat_id, string text, string parse_mode = null, tgMessageEntity[] entities=null, bool disable_web_page_preview=false, bool disable_notification=false, int reply_to_message_id=0, bool allow_sending_without_reply=false, string reply_markup=null)
        {
            tgMessage result = null;

            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);
            MyParameters.Add("text", text);

            return sendRequest(postBase + "sendMessage?" + mywebcore.core.RawPostData(MyParameters));
        }

        public string forwardMessage(string chat_id, string from_chat_id, long message_id, long message_thread_id=0,bool disable_notification=false, bool protect_content=false)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);
            MyParameters.Add("from_chat_id", from_chat_id);
            MyParameters.Add("message_id", message_id.ToString());

            return sendRequest(postBase + "forwardMessage?" + mywebcore.core.RawPostData(MyParameters));
        }

        public string copyMessage(string chat_id, string from_chat_id, long message_id, long message_thread_id=-1, string caption=null, string parse_mode=null, tgMessageEntity[] caption_entities=null, bool disable_notification=false, bool protect_content=false, long reply_to_message_id=-1, bool allow_sending_without_reply=true, string reply_markup = null)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);
            MyParameters.Add("from_chat_id", from_chat_id);
            MyParameters.Add("message_id", message_id.ToString());

            return sendRequest(postBase + "copyMessage?" + mywebcore.core.RawPostData(MyParameters));                       
        }

        public string pinChatMessage(string chat_id, long message_id, bool disable_notification = false)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);            
            MyParameters.Add("message_id", message_id.ToString());            

            return sendRequest(postBase + "pinChatMessage?" + mywebcore.core.RawPostData(MyParameters));
        }

        public string unpinChatMessage(string chat_id, long message_id = -1)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);
            if (message_id>=0)
                MyParameters.Add("message_id", message_id.ToString());

            return sendRequest(postBase + "unpinChatMessage?" + mywebcore.core.RawPostData(MyParameters));
        }

        public string deleteMessage(string chat_id, long message_id)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);
            MyParameters.Add("message_id", message_id.ToString());

            return sendRequest(postBase + "deleteMessage?" + mywebcore.core.RawPostData(MyParameters));
        }

        public string getUpdates()
        {
            return sendRequest(postBase + "getUpdates");
        }

        public string getChat(string chat_id)
        {
            Dictionary<string, string> MyParameters = new Dictionary<string, string>();

            MyParameters.Add("chat_id", chat_id);            

            return sendRequest(postBase + "getChat?" + mywebcore.core.RawPostData(MyParameters));
        }

        /*
        public string getChat(long chat_id)
        {
            Dictionary<string, long> MyParameters = new Dictionary<string, long>();

            MyParameters.Add("chat_id", chat_id);

            return sendRequest(postBase + "getChat?" + mywebcore.core.RawPostData(MyParameters));
        }
        */


    }
}
