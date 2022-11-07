/*
 * TolerantObjectConverter<T>
 * Found at https://stackoverflow.com/a/52752393/992239
 * Thanks to Brian Rogers (https://stackoverflow.com/users/10263/brian-rogers)
 * 
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JsonConverters
{
    public class TolerantObjectConverter<T> : JsonConverter where T : new()
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            object result = new T();
            if (token.Type == JTokenType.Object)
            {
                serializer.Populate(token.CreateReader(), result);
            }
            return result;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
