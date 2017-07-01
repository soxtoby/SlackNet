using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet
{
    public interface ISlackUrlBuilder
    {
        string Url(string apiMethod, Args args);
    }

    class SlackUrlBuilder : ISlackUrlBuilder
    {
        private readonly JsonSerializerSettings _serializerSettings;
        public SlackUrlBuilder(JsonSerializerSettings serializerSettings) => _serializerSettings = serializerSettings;

        public string Url(string apiMethod, Args args) =>
            $"https://slack.com/api/{apiMethod}{Query(args)}";

        private string Query(Args args) => 
            args.Any()
                ? "?" + string.Join("&", ArgValues(args))
                : string.Empty;

        private IEnumerable<string> ArgValues(Args args) => 
            args.Where(a => a.Value != null)
                .Select(a => $"{a.Key}={Uri.EscapeDataString(ArgValue(a.Value))}");

        private string ArgValue(object value) =>
              value is string stringValue ? stringValue
            : value is IEnumerable<string> stringList ? string.Join(",", stringList)
            : JsonConvert.SerializeObject(value, _serializerSettings);
    }
}