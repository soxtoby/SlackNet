using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace SlackNet;

class IgnoreArrayConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            reader.Skip();
            return existingValue;
        }

        return serializer.Deserialize(reader, objectType);
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(IDictionary<,>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
    }
}