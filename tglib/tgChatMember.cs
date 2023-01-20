using System;
using System.Collections.Generic;
using System.Text;

namespace tglib
{
    public class ChatMember
    {
        public string status;
        public tgUser user;
    }

    public class ChatMemberUpdated
    {
        public tgChat chat;
        public tgUser from;
        public int date;
        public ChatMember old_chat_member;
        public ChatMember new_chat_member;
        public tgChatInviteLink invite_link;

        public ChatMemberUpdated() { }
        public ChatMemberUpdated(tgChat chat, tgUser from, int date, ChatMember old_chat_member, ChatMember new_chat_member, tgChatInviteLink invite_link)
        {
            this.chat = chat;
            this.from = from;
            this.date = date;
            this.old_chat_member = old_chat_member;
            this.new_chat_member = new_chat_member;
            this.invite_link = invite_link;
        }
        public ChatMemberUpdated(tgChat chat, tgUser from, int date, ChatMember old_chat_member, ChatMember new_chat_member)
        {
            this.chat = chat;
            this.from = from;
            this.date = date;
            this.old_chat_member = old_chat_member;
            this.new_chat_member = new_chat_member;
        }
    }

    public class tgChatInviteLink
    {
        public string invite_link;
        public tgUser creator;
        public bool is_primary;
        public bool is_revoked;
        public int expire_date;
        public int member_limit;
    }

}
