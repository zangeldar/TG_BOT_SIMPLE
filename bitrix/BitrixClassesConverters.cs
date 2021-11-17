using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace bitrix
{
    public class MyBoolToNullConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }
                
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // У 1С странные представления о типизации. В объекте типа DateTime можно обнаружить:
            //  - DateTime
            //  - Bool (= false)
            //  - Object (= null)
            // Поэтому, такой подход не всегда работает корректно.
            
            if (reader.TokenType == JsonToken.Boolean)
                if ((bool)reader.Value == false)
                    return null;

            object result;
            try
            {
                result = serializer.Deserialize(reader, objectType);
            }
            catch (Exception e)
            {
                result = null;
                //throw;
            }

            return result;
            

            // Попытка 2 обыграть логику 1С Битрикс
            // Все, что не DateTime - будет null
            // ломает десериализация для объектов типа Phone
            // Пробуем добавить фильтр в предыдущий способ
            /*
            if (reader.TokenType == JsonToken.Date)
                return serializer.Deserialize(reader, objectType);            
            return null;
            */
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
