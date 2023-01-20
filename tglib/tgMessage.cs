using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    public class tgMessage
    {
        //required:
        public long message_id { get; set; }
        public int date { get; set; }
        public tgChat chat { get; set; }

        //optional:
        public tgUser from { get; set; }
        public tgChat sender_chat { get; set; }
        public tgUser forward_from { get; set; }
        public tgChat forward_from_chat { get; set; }
        public long forward_from_message_id { get; set; }
        public string forward_signature { get; set; }
        public string forward_sender_name { get; set; }
        public int forward_date { get; set; }
        public tgMessage reply_to_message { get; set; }
        public tgUser via_bot { get; set; }
        public int edit_date { get; set; }
        public string media_group_id { get; set; }
        public string author_signature { get; set; }
        public string text { get; set; }
        public tgMessageEntity[] entities { get; set; }
        /*
        public tgAnimation animation { get; set; }
        public tgAudio audio { get; set; }
        public tgDocument document { get; set; }
        public tgPhotoSize[] photo { get; set; }
        public tgSticker sticker { get; set; }
        public tgVideo video { get; set; }
        public tgVideoNote video_note { get; set; }
        public tgVoice voice { get; set; }
        */
        public string caption { get; set; }
        public tgMessageEntity[] caption_entities { get; set; }
        /*
        public tgContact contact { get; set; }
        public tgDice dice { get; set; }
        public tgGame game { get; set; }
        public tgPoll poll { get; set; }
        public tgVenue venue { get; set; }
        public tgLocation location { get; set; }
        */
        public tgUser[] new_chat_members { get; set; }
        public tgUser left_chat_member { get; set; }
        public string new_chat_title { get; set; }
        //public tgPhotoSize new_chat_photo { get; set; }
        public bool delete_chat_photo { get; set; }
        public bool group_chat_created { get; set; }
        public bool supergroup_chat_created { get; set; }
        public bool channel_chat_created { get; set; }
        //public tgMessageAutoDeleteTimerChanged message_auto_delete_timer_changed { get; set; }
        public long migrate_to_chat_id { get; set; }
        public long migrate_from_chat_id { get; set; }
        public tgMessage pinned_message { get; set; }
        /*
        public tgInvoice invoice { get; set; }
        public tgSuccessfulPayment successful_payment { get; set; }
        */
        public string connected_website { get; set; }
        /*
        public tgPassportData passport_data { get; set; }
        public tgProximityAlertTriggered proximity_alert_triggered { get; set; }
        public tgVoiceChatScheduled voice_chat_scheduled { get; set; }
        public tgVoiceChatStarted voice_chat_started { get; set; }
        public tgVoiceChatEnded voice_chat_ended { get; set; }
        public tgVoiceChatParticipantsInvited voice_chat_participants_invited { get; set; }
        public tgInlineKeyboardMarkup reply_markup { get; set; }
        */
        //constructors:
        public tgMessage() { }
        public tgMessage(long message_id, int date, tgChat chat)
        {
            this.message_id = message_id;
            this.date = date;
            this.chat = chat;
        }
    }
}
