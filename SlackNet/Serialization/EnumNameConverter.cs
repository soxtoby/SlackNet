using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SlackNet;

class EnumNameConverter : JsonConverter<Enum>
{
    private readonly NamingStrategy _namingStrategy;
    public EnumNameConverter(NamingStrategy namingStrategy) => _namingStrategy = namingStrategy;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(SerializedName((Enum)value));
    }

    protected override Enum ReadJsonValue(JsonReader reader, Type objectType, Enum existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            if (reader.TokenType == JsonToken.String)
                return ParseEnumName(UnderlyingType(objectType), reader.Value.ToString());
            if (reader.TokenType == JsonToken.Integer)
                throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Integer value {0} is not allowed.", reader.Value));
        }
        catch (Exception ex)
        {
            throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Error converting value {0} to type '{1}'.", reader.Value, objectType), ex);
        }

        throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected token {0} when parsing enum.", reader.TokenType));
    }

    private Enum ParseEnumName(Type type, string name) =>
        Enum.GetValues(type)
            .Cast<Enum>()
            .FirstOrDefault(e => SerializedName(e) == name);

    private string SerializedName(Enum enumValue)
    {
        var enumText = enumValue.ToString("G");

        if (char.IsNumber(enumText[0]) || enumText[0] == '-')
            throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Integer value {0} is not allowed.", enumText));

        var explicitName = enumValue.GetType()
            .GetRuntimeField(enumText)?
            .GetCustomAttribute<EnumMemberAttribute>()?
            .Value;

        return _namingStrategy.GetPropertyName(explicitName ?? enumText, explicitName != null);
    }
}