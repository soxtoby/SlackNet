#nullable enable
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

class JsonDataConverter : Newtonsoft.Json.JsonConverter<JsonData>
{
    public override void WriteJson(JsonWriter writer, JsonData? value, JsonSerializer serializer) =>
        (value.ToJToken() ?? JValue.CreateNull()).WriteTo(writer);

    public override JsonData? ReadJson(JsonReader reader, Type objectType, JsonData? existingValue, bool hasExistingValue, JsonSerializer serializer) =>
        reader.TokenType == JsonToken.Null
            ? null
            : new JsonData(JToken.Load(reader), serializer);
}
