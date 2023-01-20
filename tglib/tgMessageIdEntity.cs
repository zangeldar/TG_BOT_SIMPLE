using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    public class tgMessageId
    {
        public int message_id { get; set; }
        public tgMessageId(int message_id)
        {
            this.message_id = message_id;
        }
    }

    public class tgMessageEntity
    {
        public string type { get; set; }
        public int offset { get; set; }
        public int length { get; set; }
        // optional
        public string url { get; set; }
        public tgUser user { get; set; }
        public string language { get; set; }

        //constructors:
        public tgMessageEntity() { }
        public tgMessageEntity(string type, int offset, int length)
        {
            this.type = type;
            this.offset = offset;
            this.length = length;
        }
    }
}
