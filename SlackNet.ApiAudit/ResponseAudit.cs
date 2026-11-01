using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SlackNet.ApiAudit;

public record Finding(string Id, string Kind, string Method, string Path, string Model, string Evidence, string Source)
{
    public string Disposition { get; set; } = "Review";
    public string Reason { get; set; }

    public static Finding Create(string kind, string method, string path, string model, string evidence, string source) =>
        new(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{kind}|{method}|{path}|{model}")))[..16].ToLowerInvariant(),
            kind, method, path, model, evidence, source);
}

public record Shape(string Path, string Kind);

public class ResponseAudit
{
    private readonly JsonSerializer _serializer = JsonSerializer.Create(Default.JsonSettings().SerializerSettings);
    private readonly List<Finding> _findings = [];
    private readonly HashSet<Shape> _shapes = [];
    private static readonly HashSet<string> EnvelopeProperties = ["ok", "error", "warning", "warnings", "needed", "provided"];

    public IReadOnlyList<Finding> Findings => _findings.DistinctBy(f => f.Id).ToArray();
    public IReadOnlyList<Shape> Shapes => _shapes.OrderBy(s => s.Path).ThenBy(s => s.Kind).ToArray();

    public void Inspect(JToken json, Type type, string method, string evidence, string source, string path = "$")
    {
        _shapes.Add(new Shape(path, json.Type.ToString()));
        if (json.Type == JTokenType.Null || type == typeof(object) || typeof(JToken).IsAssignableFrom(type))
            return;

        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (underlyingType.IsEnum)
        {
            var known = Enum.GetValues(underlyingType).Cast<object>().Select(e => JToken.FromObject(e, _serializer));
            if (!known.Any(e => JToken.DeepEquals(e, json)))
                AddFinding("Unknown enum value", type);
        }
        else
        {
            object value = null;
            try
            {
                value = json.ToObject(type, _serializer);
            }
            catch (Exception e) when (e is JsonException or ArgumentException or FormatException or InvalidOperationException or OverflowException)
            {
                AddFinding("Incompatible value", type);
            }

            var contract = _serializer.ContractResolver.ResolveContract(value?.GetType() ?? type);
            switch (json, contract)
            {
                case (JObject obj, JsonObjectContract objectContract):
                    foreach (var property in obj.Properties())
                    {
                        var member = objectContract.Properties.GetClosestMatchProperty(property.Name);
                        if (path == "$" && EnvelopeProperties.Contains(property.Name))
                        {
                            // SlackApiClient consumes these before returning the typed result.
                        }
                        else if (objectContract.UnderlyingType == typeof(Icons) && Regex.IsMatch(property.Name, @"^image_\d+$"))
                            _shapes.Add(new Shape($"{path}.image_*", property.Value.Type.ToString()));
                        else if (member == null || member.Ignored)
                        {
                            _shapes.Add(new Shape($"{path}.{property.Name}", property.Value.Type.ToString()));
                            _findings.Add(Finding.Create("Unmodeled property", method, $"{path}.{property.Name}",
                                objectContract.UnderlyingType.FullName, evidence, source));
                        }
                        else if (member.Converter != null && property.Value is JArray { Count: 0 })
                        {
                            _shapes.Add(new Shape($"{path}.{property.Name}", "Array"));
                            try
                            {
                                using var reader = property.Value.CreateReader();
                                reader.Read();
                                member.Converter.ReadJson(reader, member.PropertyType, null, _serializer);
                            }
                            catch (Exception e) when (e is JsonException or ArgumentException or FormatException or InvalidOperationException)
                            {
                                _findings.Add(Finding.Create("Incompatible value", method, $"{path}.{property.Name}", member.PropertyType.FullName, evidence, source));
                            }
                        }
                        else
                            Inspect(property.Value, member.PropertyType, method, evidence, source, $"{path}.{property.Name}");
                    }
                    break;
                case (JArray array, JsonArrayContract arrayContract):
                    foreach (var item in array)
                        Inspect(item, arrayContract.CollectionItemType ?? typeof(object), method, evidence, source, $"{path}[]");
                    break;
                case (JObject obj, JsonDictionaryContract dictionaryContract):
                    foreach (var property in obj.Properties())
                        Inspect(property.Value, dictionaryContract.DictionaryValueType ?? typeof(object), method, evidence, source, $"{path}.*");
                    break;
                case (JObject or JArray, _):
                    AddFinding("Incompatible shape", type);
                    break;
            }
        }

        void AddFinding(string kind, Type model) =>
            _findings.Add(Finding.Create(kind, method, path, model.FullName, evidence, source));
    }
}
