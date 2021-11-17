using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerFrmwrk
{
    public class Message
    {
        public bool? toTg { get; set; }
        public bool? toBx {
            get { return !toTg; }
            set {
                if (value != null)
                    toTg = !value;
                else
                    toTg = value;
            }
        }
        public bool? fromTg
        {
            get { return toBx; }
            set { toBx = value; }
        }
        public bool? fromBx
        {
            get { return toTg; }
            set { toTg = value; }
        }
        public string id { get; set; }
        public string chat_id { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string date { get; set; }
        public string text { get; set; }
    }
}
