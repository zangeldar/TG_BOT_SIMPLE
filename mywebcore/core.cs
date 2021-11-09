using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace mywebcore
{
    public class core
    {
        public string serverURL { get; protected set; }
        //public string pathURL { get; protected set; }

        public core() { }
        public core(string serverUrl)
        {
            this.serverURL = serverUrl;
        }
        public Dictionary<string, string> parameters { get; protected set; }

        /// <summary>
        /// Функция для получения тела запроса из параметров поиска
        /// </summary>
        /// <returns>Возвращает тело запроса в виде строки</returns>
        static public string RawPostData(Dictionary<string, string> curParameters, bool skipBlank = true)
        {
            string result = "";
            bool first = true;
            foreach (KeyValuePair<string, string> item in curParameters)
            {
                if (skipBlank & item.Value != "")
                {
                    if (first)
                    {
                        //result += "?";
                        result += "";
                        first = false;
                    }
                    else
                        result += "&";
                    result += item.Key + "=" + item.Value;
                }
            }

            return result;
        }
        /*
        static public string RawPostData(Dictionary<string, long> curParameters, bool skipBlank = true)
        {
            string result = "";
            bool first = true;
            foreach (KeyValuePair<string, long> item in curParameters)
            {
                if (skipBlank & item.Value != 0)
                {
                    if (first)
                    {
                        //result += "?";
                        result += "";
                        first = false;
                    }
                    else
                        result += "&";
                    result += item.Key + "=" + item.Value;
                }
            }

            return result;
        }
        */

        static public object ParseJson(string rawJson)
        {            
            return JsonConvert.DeserializeObject<object>(rawJson);
        }

        protected Dictionary<string, string> GetTokenFromJsonByName(string rawJson, List<string> nameToken)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string item in nameToken)
                result.Add(item, "");

            Newtonsoft.Json.Linq.JObject jsonObject = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(rawJson);
            foreach (Newtonsoft.Json.Linq.JToken item in jsonObject.Children())
            {
                if (item.Type == Newtonsoft.Json.Linq.JTokenType.Property)
                    if (nameToken.Contains(((Newtonsoft.Json.Linq.JProperty)item).Name))
                    {
                        result[((Newtonsoft.Json.Linq.JProperty)item).Name] = ((Newtonsoft.Json.Linq.JProperty)item).Value.ToString();
                    }
            }
            return result;
        }

        protected string GetTokenFromJsonByName(string rawJson, string nameToken)
        {
            return GetTokenFromJsonByName(rawJson, new List<string>() { nameToken })[nameToken];
        }
    }
}
