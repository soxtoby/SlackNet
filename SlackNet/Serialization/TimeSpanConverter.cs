using System;
using System.Globalization;
using Newtonsoft.Json;

namespace SlackNet;

class TimeSpanConverter : JsonConverter<TimeSpan>
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var timeSpan = (TimeSpan?)value;
        if (timeSpan.HasValue)
            writer.WriteValue(timeSpan.Value.ToString("hh\\:mm"));
        else
            writer.WriteNull();
    }

    protected override TimeSpan ReadJsonValue(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            return TimeSpan.Parse(reader.Value.ToString());
        }
        catch (Exception ex)
        {
            throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Error converting value {0} to type '{1}'", reader.Value, objectType), ex);
        }
    }
}