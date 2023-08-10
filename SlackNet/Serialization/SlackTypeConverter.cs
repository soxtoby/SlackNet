using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.Rtm;

namespace SlackNet;

// Adapted from https://github.com/manuc66/JsonSubTypes
class SlackTypeConverter : JsonConverter
{
    private readonly ISlackTypeResolver _slackTypeResolver;
    private readonly ILogger _log;
    [ThreadStatic] private static bool _isInsideRead;
    [ThreadStatic] private static JsonReader _reader;

    public SlackTypeConverter(ISlackTypeResolver slackTypeResolver, ILogger logger)
    {
        _slackTypeResolver = slackTypeResolver;
        _log = logger.ForSource<SlackTypeConverter>();
    }

    public override bool CanRead => !_isInsideRead || !string.IsNullOrEmpty(_reader.Path);

    public sealed override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => ReadJson(reader, objectType, serializer);

    private object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
    {
        while (reader.TokenType == JsonToken.Comment)
            reader.Read();

        return reader.TokenType switch
            {
                JsonToken.Null => null,
                JsonToken.StartObject => ReadObject(reader, objectType, serializer),
                JsonToken.StartArray => ReadArray(reader, objectType, serializer),
                _ => ReadInner(CreateAnotherReader(JToken.Load(reader), reader), objectType, serializer)
            };
    }

    private IList ReadArray(JsonReader reader, Type targetType, JsonSerializer serializer)
    {
        var elementType = GetElementType(targetType);
        var list = CreateCompatibleList(targetType, elementType);

        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            list.Add(ReadJson(reader, elementType, serializer));

        if (!targetType.IsArray) 
            return list;
            
        var array = Array.CreateInstance(targetType.GetElementType()!, list.Count);
        list.CopyTo(array, 0);

        return array;
    }

    private static IList CreateCompatibleList(Type targetContainerType, Type elementType) =>
        targetContainerType.IsArray || targetContainerType.GetTypeInfo().IsAbstract
            ? (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))
            : (IList)Activator.CreateInstance(targetContainerType);

    private static Type GetElementType(Type arrayOrGenericContainer) =>
        arrayOrGenericContainer.IsArray
            ? arrayOrGenericContainer.GetElementType()
            : arrayOrGenericContainer.GenericTypeArguments[0];

    private object ReadObject(JsonReader reader, Type objectType, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var targetType = GetType(jObject, objectType);
        return ReadInner(CreateAnotherReader(jObject, reader), targetType, serializer);
    }

    private static JsonReader CreateAnotherReader(JToken jObject, JsonReader reader)
    {
        var jObjectReader = jObject.CreateReader();
        jObjectReader.Culture = reader.Culture;
        jObjectReader.CloseInput = reader.CloseInput;
        jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
        jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
        jObjectReader.FloatParseHandling = reader.FloatParseHandling;
        jObjectReader.DateFormatString = reader.DateFormatString;
        jObjectReader.DateParseHandling = reader.DateParseHandling;
        return jObjectReader;
    }

    private Type GetType(JToken jObject, Type parentType)
    {
        if (jObject.Value<uint>("reply_to") > 0)
            return typeof(Reply);

        var type = GetType(jObject, "type", parentType);
        return GetType(jObject, "subtype", type);
    }

    private Type GetType(JToken jObject, string typeProperty, Type baseType)
    {
        var slackType = jObject.Value<string>(typeProperty);
        return slackType == null 
            ? baseType 
            : _slackTypeResolver.FindType(baseType, slackType);
    }

    private object ReadInner(JsonReader reader, Type objectType, JsonSerializer serializer)
    {
        _reader = reader;
        _isInsideRead = true;
        try
        {
            return serializer.Deserialize(reader, objectType);
        }
        catch (Exception ex)
        { 
            _log.Serialization(ex, "Error deserializing to {ObjectType}, falling back to default value", objectType.FullName);
            return DefaultValue(objectType);
        }
        finally
        {
            _isInsideRead = false;
        }
    }

    private static object DefaultValue(Type type) =>
        type.GetTypeInfo().IsValueType
            ? Activator.CreateInstance(type)
            : null;
}