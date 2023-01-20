using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    // actual date: 2021/06/25 
    public class tgChat
    {
        // required:
        public long id { get; set; }
        public string type { get; set; }
        
        // optional:
        public string title { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }        
        //public tgChatPhoto photo { get; private set; }        
        public string bio { get; set; }
        public string description { get; set; }
        public string invite_link { get; set; }
        public tgMessage pinned_message { get; set; }
        //public tgChatPermissions permissions { get; private set; }
        public int slow_mode_delay { get; set; }
        public int message_auto_delete_time { get; set; }
        public string sticker_set_name { get; set; }
        public bool can_set_sticker_set { get; set; }
        public long linked_chat_id { get; set; }
        //public tgChatLocation location { get; private set; }        

        // Constructors:
        public tgChat() { }
        /// <summary>
        /// This object represents a chat.
        /// </summary>
        /// <param name="id">Unique identifier for this chat. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.</param>
        /// <param name="type">Type of chat, can be either “private”, “group”, “supergroup” or “channel”</param>
        public tgChat(long id, string type)
        {
            this.id = id;
            this.type = type;
        }
    }
}
