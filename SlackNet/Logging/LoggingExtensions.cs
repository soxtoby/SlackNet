#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SlackNet.Handlers;

namespace SlackNet
{
    public static class LoggingExtensions
    {
        private const string ExceptionProperty = "Exception";

        /// <summary>
        /// Returns the message template with indexed placeholders instead of named placeholders.
        /// </summary>
        /// <example><c>"Message with a {0} placeholder here and {1} here"</c></example>
        public static string IndexedMessageTemplate(this ILogEvent logEvent) =>
            IndexedMessageTemplate(logEvent.MessageTemplate);

        /// <summary>
        /// Includes extra properties and exception on separate lines, with indexed placeholders instead of named placeholders.
        /// </summary>
        /// <example><code>
        /// @"Message with a {0} placeholder here and {1} here
        /// ExtraProperty: {2}
        /// Exception: {3}"
        /// </code></example>
        public static string IndexedFullMessageTemplate(this ILogEvent logEvent) =>
            IndexedMessageTemplate(logEvent.FullMessageTemplate());

        private static string IndexedMessageTemplate(string messageTemplate)
        {
            var i = 0;
            return LogEvent.PropertyPattern.Replace(messageTemplate, _ => $"{{{i++}}}");
        }

        /// <summary>
        /// Includes extra properties and exception on separate lines.
        /// </summary>
        /// <example><code>
        /// @"Message with a {Property} placeholder here and {OtherProperty} here
        ///     ExtraProperty: {ExtraProperty}
        ///     Exception: {Exception}"
        /// </code></example>
        public static string FullMessageTemplate(this ILogEvent logEvent) =>
            string.Join(Environment.NewLine,
                new[] { logEvent.MessageTemplate }
                    .Concat(logEvent.ExtraPropertyNames().Select(p => $"  {p}: {{{p}}}"))
                    .Concat(logEvent.Exception is not null
                        ? new[] { $"  Exception: {{{ExceptionProperty}}}" }
                        : Enumerable.Empty<object?>()));

        /// <summary>
        /// All the values for the properties in the <see cref="FullMessageTemplate"/>,
        /// in the order that they're used.
        /// </summary>
        public static object?[] FullMessagePropertyValues(this ILogEvent logEvent) =>
            logEvent.MessagePropertyValues()
                .Concat(logEvent.ExtraPropertyNames()
                    .Select(p => logEvent.Properties[p]))
                .Concat(logEvent.Exception is not null
                    ? new[] { logEvent.Exception }
                    : Enumerable.Empty<object?>())
                .ToArray();

        /// <summary>
        /// The values for the properties in the <see cref="ILogEvent.MessageTemplate"/>,
        /// in the order that they're used.
        /// </summary>
        public static object?[] MessagePropertyValues(this ILogEvent logEvent) =>
            logEvent.MessagePropertyNames()
                .Select(prop => logEvent.Properties.TryGetValue(prop, out var value) ? value : null)
                .ToArray();

        /// <summary>
        /// The distinct property names used in the <see cref="ILogEvent.MessageTemplate"/>,
        /// in the order that they're used.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        public static IEnumerable<string> MessagePropertyNames(this ILogEvent logEvent) =>
            LogEvent.PropertyPattern.Matches(logEvent.MessageTemplate)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct();

        /// <summary>
        /// Available properties that aren't used in the <see cref="ILogEvent.MessageTemplate"/>,
        /// ordered alphabetically.
        /// </summary>
        public static IEnumerable<string> ExtraPropertyNames(this ILogEvent logEvent) =>
            logEvent.Properties.Keys
                .Except(logEvent.MessagePropertyNames())
                .OrderBy(p => p);

        /// <summary>
        /// The <see cref="FullMessageTemplate"/>, with property values filled in.
        /// </summary>
        public static string FullMessage(this ILogEvent logEvent) =>
            string.Format(logEvent.IndexedFullMessageTemplate(), logEvent.FullMessagePropertyValues().Select(FormatLogValue).ToArray<object>());

        private static string FormatLogValue(object? value) =>
            value switch
                {
                    IEnumerable enumerable and not string => FormatEnumerable(enumerable),
                    _ => Convert.ToString(value)
                };

        private static string FormatEnumerable(IEnumerable enumerable)
        {
            var value = string.Join(", ", enumerable.Cast<object>().Select(FormatLogValue));
            return string.IsNullOrEmpty(value)
                ? "<none>"
                : value;
        }

        /// <summary>
        /// Creates a logger that adds a Source property with the specified type's <see cref="Type.FullName"/> to any log events.
        /// </summary>
        public static ILogger ForSource<TSource>(this ILogger logger) =>
            logger.WithContext("Source", typeof(TSource).FullName); // Logging as string, so implementations can filter on source

        /// <summary>
        /// Creates a logger that adds an extra property to any log events.
        /// </summary>
        public static ILogger WithContext(this ILogger logger, string property, object? value) =>
            new ContextLogger(logger, property, value);

        /// <summary>
        /// Logs an event with the <see cref="LogCategory.Data"/> category.
        /// </summary>
        public static void Data(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, params object?[] values) =>
            logger.Log(new LogEvent(LogCategory.Data, messageTemplate, values));

        /// <summary>
        /// Logs an event with the <see cref="LogCategory.Internal"/> category.
        /// </summary>
        public static void Internal(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, params object?[] values) =>
            logger.Log(new LogEvent(LogCategory.Internal, messageTemplate, values));

        /// <summary>
        /// Logs an event with the <see cref="LogCategory.Internal"/> category.
        /// </summary>
        public static void Internal(this ILogger logger, Exception exception, [StructuredMessageTemplate] string messageTemplate, params object?[] values) =>
            logger.Log(new LogEvent(LogCategory.Internal, messageTemplate, values) { Exception = exception });

        /// <summary>
        /// Logs a <see cref="LogCategory.Request"/> event with the inner handlers used to handle a request.
        /// </summary>
        public static void RequestHandler<THandler, TRequest>(this ILogger logger, THandler handler, TRequest request, [StructuredMessageTemplate] string messageTemplate, params object?[] values) where THandler : class =>
            logger.Request(messageTemplate + " with {Handler}", values.Concat(new[] { handler.InnerHandlers(request) }).ToArray());

        /// <summary>
        /// Logs an event with the <see cref="LogCategory.Request"/> category.
        /// </summary>
        public static void Request(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, params object?[] values) =>
            logger.Log(new LogEvent(LogCategory.Request, messageTemplate, values));

        /// <summary>
        /// Logs an event with the <see cref="LogCategory.Error"/> category.
        /// </summary>
        public static void Error(this ILogger logger, Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[] values) =>
            logger.Log(new LogEvent(LogCategory.Error, messageTemplate, values) { Exception = exception });

        class ContextLogger : ILogger
        {
            private readonly ILogger _inner;
            private readonly string _propertyName;
            private readonly object? _value;

            public ContextLogger(ILogger inner, string propertyName, object? value)
            {
                _inner = inner;
                _propertyName = propertyName;
                _value = value;
            }

            public void Log(ILogEvent logEvent) => _inner.Log(new LogEvent(logEvent, new[] { new KeyValuePair<string, object?>(_propertyName, _value) }));
        }
    }
}