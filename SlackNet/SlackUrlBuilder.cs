using System;
using System.Collections;
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
        private readonly SlackJsonSettings _jsonSettings;
        public SlackUrlBuilder(SlackJsonSettings jsonSettings) => _jsonSettings = jsonSettings;

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
            : value is IEnumerable enumerable ? SerializeEnumerable(enumerable)
            : JsonConvert.SerializeObject(value, _jsonSettings.SerializerSettings);

        private string SerializeEnumerable(IEnumerable enumerable) =>
            string.Join(",", enumerable.Cast<object>()
                .Select(o => JsonConvert.SerializeObject(o, _jsonSettings.SerializerSettings))
                .Select(val => val.Trim('"')));
    }
}