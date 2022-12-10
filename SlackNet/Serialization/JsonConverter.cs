using System;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace SlackNet;

abstract class JsonConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType) =>
        typeof(T).GetTypeInfo().IsAssignableFrom(UnderlyingType(objectType).GetTypeInfo());

    protected static Type UnderlyingType(Type objectType) =>
        IsNullable(objectType)
            ? Nullable.GetUnderlyingType(objectType)
            : objectType;

    protected static bool IsNullable(Type objectType)
    {
        var typeInfo = objectType.GetTypeInfo();
        return typeInfo.IsGenericType
            && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return IsNullable(objectType)
                ? (object)null
                : throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Cannot convert null value to {0}.", objectType));

        return ReadJsonValue(reader, objectType, existingValue == null ? default : (T)existingValue, existingValue != null, serializer);
    }

    protected abstract T ReadJsonValue(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer);
}