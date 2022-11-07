/*
 * ArrayToObjectConverter<T>
 * Found at https://stackoverflow.com/a/47558161/992239
 * Thanks to Brian Rogers (https://stackoverflow.com/users/10263/brian-rogers)
 * 
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace JsonConverters
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class JsonArrayIndexAttribute : Attribute
    {
        public int Index { get; private set; }

        public JsonArrayIndexAttribute(int index)
        {
            Index = index;
        }
    }

    public sealed class ArrayToObjectConverter<T> : JsonConverter where T : class, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);

            var propsByIndex = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<JsonArrayIndexAttribute>() != null)
                .ToDictionary(p => p.GetCustomAttribute<JsonArrayIndexAttribute>().Index);

            JObject obj = new JObject(array
                .Select((jt, i) =>
                {
                    return propsByIndex.TryGetValue(i, out PropertyInfo prop) ? new JProperty(prop.Name, jt) : null;
                })
                .Where(jp => jp != null)
            );

            T target = new T();
            serializer.Populate(obj.CreateReader(), target);

            return target;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
