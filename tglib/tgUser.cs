using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    public class tgUser : atgBase
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
        public bool can_join_groups { get; set; }
        public bool can_read_all_group_messages { get; set; }
        public bool supports_inline_queries { get; set; }

        public tgUser() { }
        public tgUser(long id, bool is_bot, string first_name)
        {
            this.id = id;
            this.is_bot = is_bot;
            this.first_name = first_name;
        }
    }
}
