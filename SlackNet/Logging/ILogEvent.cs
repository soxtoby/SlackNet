#nullable enable
using System;
using System.Collections.Generic;

namespace SlackNet
{
    public interface ILogEvent
    {
        /// <summary>
        /// Type of log event, useful for determining log level.
        /// </summary>
        LogCategory Category { get; }

        /// <summary>
        /// Log message template with placeholders for properties.
        /// </summary>
        /// <example><c>"Message with a {Property} placeholder"</c></example>
        string MessageTemplate { get; }

        /// <summary>
        /// Properties associated with the log event.
        /// May contain more properties than are used in the <see cref="MessageTemplate"/>.
        /// </summary>
        IReadOnlyDictionary<string, object?> Properties { get; }

        /// <summary>
        /// Exception object related to log event. Can be included with any log category.
        /// </summary>
        Exception? Exception { get; }

        /// <summary>
        /// When the log event was created, in UTC.
        /// </summary>
        DateTimeOffset Timestamp { get; }
    }
}