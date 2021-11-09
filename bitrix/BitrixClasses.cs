using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace bitrix
{
    // REST
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

 

    public class Phones
    {
        public string work_phone { get; set; }
        public string personal_mobile { get; set; }
        public string inner_phone { get; set; }
    }
    public class Avatar
    {
        public string url { get; set; }
        public string color { get; set; }
    }

    public class MessageRecent: MessageBase
    {
        //public int id { get; set; }
        //public string text { get; set; }
        public bool file { get; set; }
        //public int author_id { get; set; }
        public bool attach { get; set; }
        //public DateTime date { get; set; }
        public string status { get; set; }
    }

    public class MessageDialogMessages: MessageBase
    {
        //public int id { get; set; }
        public int chat_id { get; set; }
        //public int author_id { get; set; }
        //public DateTime date { get; set; }
        //public string text { get; set; }
        public bool unread { get; set; }
        public object @params { get; set; }
    }

    public class MessageBase
    {
        public int id { get; set; }
        public string text { get; set; }
        public int author_id { get; set; }
        public DateTime date { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string name { get; set; }
        public int owner { get; set; }
        public bool extranet { get; set; }
        public string avatar { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public string entity_data_1 { get; set; }
        public string entity_data_2 { get; set; }
        public string entity_data_3 { get; set; }
        public List<int> mute_list { get; set; }
        public List<int> manager_list { get; set; }
        public DateTime date_create { get; set; }
        public string message_type { get; set; }
    }

    [JsonObject("Result")]
    public class ResultRecent
    {
        public string id { get; set; }
        public int chat_id { get; set; }
        public string type { get; set; }
        public Avatar avatar { get; set; }
        public string title { get; set; }
        public MessageRecent message { get; set; }
        public int counter { get; set; }
        public bool pinned { get; set; }
        public bool unread { get; set; }
        public DateTime date_update { get; set; }
        public Chat chat { get; set; }
        public User user { get; set; }
        public List<object> options { get; set; }
    }

    [JsonObject("Result")]
    public class ResultDialogMessages
    {
        public int chat_id { get; set; }        
        public List<MessageDialogMessages> messages { get; set; }
        public List<User> users { get; set; }
        public List<File> files { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public bool active { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string work_position { get; set; }
        public string color { get; set; }
        public string avatar { get; set; }
        public string gender { get; set; }
        public string birthday { get; set; }
        public bool extranet { get; set; }
        public bool network { get; set; }
        public bool bot { get; set; }
        public bool connector { get; set; }
        public string external_auth_id { get; set; }
        public string status { get; set; }

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public DateTime idle { get; set; }  // Bool usually, but can be DateTime

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public DateTime last_activity_date { get; set; } // DateTime usually, but can be Bool       

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public DateTime mobile_last_date { get; set; } // DateTime usually, but can be Bool

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public DateTime absent { get; set; } // DateTime usually, but can be Bool
        public List<int> departments { get; set; }

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public Phones phones { get; set; } // was just object type

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public DateTime desktop_last_date { get; set; }
    }

    public class Image
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class ViewerAttrs
    {
        public object viewer { get; set; }
        public string viewerType { get; set; }
        public string src { get; set; }
        public string objectId { get; set; }
        public string viewerGroupBy { get; set; }
        public string title { get; set; }
        public string actions { get; set; }
    }

    public class File
    {
        public int id { get; set; }
        public int chatId { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public int size { get; set; }

        [JsonConverter(typeof(MyBoolToNullConverter))]
        public Image image { get; set; } // Image usually, but can be Bool
        public string status { get; set; }
        public int progress { get; set; }
        public int authorId { get; set; }
        public string authorName { get; set; }
        public string urlPreview { get; set; }
        public string urlShow { get; set; }
        public string urlDownload { get; set; }
        public ViewerAttrs viewerAttrs { get; set; }
    }

    public class Time
    {
        public double start { get; set; }
        public double finish { get; set; }
        public double duration { get; set; }
        public double processing { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_finish { get; set; }
    }

    public class RootRecent
    {
        public List<ResultRecent> result { get; set; }
        public Time time { get; set; }
    }

    public class RootDialogMessages
    {
        public ResultDialogMessages result { get; set; }
        public Time time { get; set; }
    }

    public class RootGetUser
    {
        public User result { get; set; }
        public Time time { get; set; }
    }


    /*  // NON-REST
    class BitrixLastMessages
    {
        public string REVISION { get; set; }
        public string MOBILE_REVISION { get; set; }
        public string CHAT_ID { get; set; }
        public string DISK_FOLDER_ID { get; set; }
        public string USER_ID { get; set; }
        public MESSAGE MESSAGE { get; set; }
        //public USERSMESSAGE USERS_MESSAGE { get; set; }
        public List<object> UNREAD_MESSAGE { get; set; }
        public List<USER> USERS { get; set; }
        //public USERINGROUP USER_IN_GROUP { get; set; }
        public List<object> CHAT { get; set; }
        public List<object> USER_BLOCK_CHAT { get; set; }
        public List<object> USER_IN_CHAT { get; set; }
        public string USER_LOAD { get; set; }
        //public READEDLIST READED_LIST { get; set; }
        public List<object> PHONES { get; set; }
        public List<object> FILES { get; set; }
        public List<object> LINES { get; set; }
        public List<object> OPENLINES { get; set; }
        public string NETWORK_ID { get; set; }
        public string ERROR { get; set; }
    }

    public class USER
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string work_position { get; set; }
        public string color { get; set; }
        public string avatar { get; set; }
        public string avatar_id { get; set; }
        public bool birthday { get; set; }
        public string gender { get; set; }
        public bool phone_device { get; set; }
        public bool phones { get; set; }
        public bool extranet { get; set; }
        public string tz_offset { get; set; }
        public bool network { get; set; }
        public bool bot { get; set; }
        public bool connector { get; set; }
        public string profile { get; set; }
        public string external_auth_id { get; set; }
        public string status { get; set; }
        public bool idle { get; set; }
        public DateTime last_activity_date { get; set; }
        public bool mobile_last_date { get; set; }
        public DateTime desktop_last_date { get; set; }
        public List<string> departments { get; set; }
        public bool absent { get; set; }
        public string services { get; set; }
        public string messageId { get; set; }
        public DateTime date { get; set; }
    }

    public class MESSAGE
    {
        public string id { get; set; }
        public string chatId { get; set; }
        public string senderId { get; set; }
        public string recipientId { get; set; }
        public string system { get; set; }
        public DateTime date { get; set; }
        public string text { get; set; }
        public string textOriginal { get; set; }
        public List<object> @params { get; set; }
    }
    */


}
