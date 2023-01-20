using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    public class tgUpdate
    {
        public long update_id { get; set; }
        //Optional
        public tgMessage message { get; set; }
        public tgMessage edited_message { get; set; }
        public tgMessage channel_post { get; set; }
        public tgMessage edited_channel_post { get; set; }
        /*
        InlineQuery inline_query { get; set; }
        ChosenInlineResult chosen_inline_result { get; set; }
        CallbackQuery callback_query { get; set; }
        PreCheckoutQuery pre_checkout_query { get; set; }
        Poll poll { get; set; }
        PollAnswer poll_answer { get; set; }
        */

        public ChatMemberUpdated my_chat_member { get; set; }
        public ChatMemberUpdated chat_member { get; set; }

        public tgUpdate() { }

        public tgUpdate(long update_id)
        {
            this.update_id = update_id;
        }
    }
}
