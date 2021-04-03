#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace SlackNet
{
    class LogEvent : ILogEvent
    {
        internal static readonly Regex PropertyPattern = new(@"\{(.*?)\}", RegexOptions.Compiled);
        private readonly Dictionary<string, object?> _properties = new();

        public LogEvent(LogCategory category, [StructuredMessageTemplate] string messageTemplate, params object?[] values)
            : this(category,
                messageTemplate,
                PropertyPattern.Matches(messageTemplate)
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .Zip(values, (prop, value) => new KeyValuePair<string, object?>(prop, value))) { }

        public LogEvent(ILogEvent baseEvent, IEnumerable<KeyValuePair<string, object?>> context)
            : this(baseEvent.Category, baseEvent.MessageTemplate, context.Concat(baseEvent.Properties))
        {
            Exception = baseEvent.Exception;
        }

        private LogEvent(LogCategory category, [StructuredMessageTemplate] string messageTemplate, IEnumerable<KeyValuePair<string, object?>> properties)
        {
            Category = category;
            MessageTemplate = messageTemplate;

            if (SlackRequestContext.IsAvailable)
                _properties[nameof(SlackRequestContext.RequestId)] = SlackRequestContext.Current.RequestId;

            foreach (var p in properties)
                _properties[p.Key] = p.Value;
        }

        public LogCategory Category { get; }
        public string MessageTemplate { get; }

        public IReadOnlyDictionary<string, object?> Properties => _properties;

        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
        public Exception? Exception { get; set; }
    }
}