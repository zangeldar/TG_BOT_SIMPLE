using System;
using System.Collections.Generic;
using System.Text;

namespace Messenger
{
    [Serializable]
    public class Settings
    {
        public SettingsBitrix SettingsBitrix { get; set; }
        public SettingsTelegram SettingsTelegram { get; set; }
        public List<User> UsersList { get; set; }
        public List<Chat> ChatsList { get; set; }

        public Settings()
        {
            Initialize();
        }

        public void Initialize()
        {
            SettingsBitrix = new SettingsBitrix();
            SettingsTelegram = new SettingsTelegram();
            UsersList = new List<User>();
            ChatsList = new List<Chat>();
        }

        public User GetUserByBx(string bxId)
        {
            foreach (User item in UsersList)
                if (item.UserIdBitrix == bxId)
                    return item;
            return null;
        }
        public User GetUserByTg(string tgId)
        {
            foreach (User item in UsersList)
                if (item.UserIdTelegram == tgId)
                    return item;
            return null;
        }

        public string GetTgUserIdByBx(string bxId)
        {
            foreach (User item in UsersList)
                if (item.UserIdBitrix == bxId)
                    return item.UserIdTelegram;
            return null;
        }
        public string GetBxUserIdByTg(string tgId)
        {
            foreach (User item in UsersList)
                if (item.UserIdTelegram == tgId)
                    return item.UserIdBitrix;
            return null;
        }

        public string GetTgChatIdByBx(string bxId)
        {
            foreach (Chat item in ChatsList)
                if (item.ChatIdBitrix == bxId)
                    return item.ChatIdTelegram;
            return null;
        }
        public string GetBxChatIdByTg(string tgId)
        {
            foreach (Chat item in ChatsList)
                if (item.ChatIdTelegram == tgId)
                    return item.ChatIdBitrix;
            return null;
        }
    }

    [Serializable]
    public class SettingsTelegram
    {
        public string ServerUrl { get; set; }
        public string API_KEY { get; set; }
        public long LastKnownUpdateId { get; set; }

        public SettingsTelegram()
        {
           
        }
    }

    [Serializable]
    public class SettingsBitrix
    {        
        public string ServerUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class User
    {
        public string NameFullBitrix { get; set; }
        public string NameFullTelegram { get; set; }

        public string UserIdBitrix { get; set; }
        public string UserIdTelegram { get; set; }

        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(User))
                return ( (obj as User).NameFullBitrix == NameFullBitrix
                    & (obj as User).NameFullTelegram == NameFullTelegram
                    & (obj as User).UserIdBitrix == UserIdBitrix
                    & (obj as User).UserIdTelegram == UserIdTelegram
                    & (obj as User).Description == Description
                    );

            return false;
            //return base.Equals(obj);
        }
    }

    [Serializable]
    public class Chat
    {
        public string NameBitrix { get; set; }
        public string NameTelegram { get; set; }

        public string DescriptionBitrix { get; set; }
        public string DescriptionTelegram { get; set; }

        public string ChatIdBitrix { get; set; }
        public string ChatIdTelegram { get; set; }
    }
}
