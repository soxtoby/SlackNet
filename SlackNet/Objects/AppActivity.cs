#nullable enable
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// A log event for a <a href="https://docs.slack.dev/workflows/">workflow app</a>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.activities.list/">Slack documentation</a> for more information.</remarks>
public class AppActivity
{
    /// <summary>
    /// The log level of the log event.
    /// </summary>
    public AppActivityLogLevel Level { get; set; }

    /// <summary>
    /// The event type of the log event, such as <c>function_execution_started</c> or <c>workflow_execution_result</c>.
    /// </summary>
    public required string EventType { get; set; }

    /// <summary>
    /// The source of the log event.
    /// </summary>
    public AppActivitySource Source { get; set; }

    /// <summary>
    /// The component type of the log event.
    /// </summary>
    public AppActivityComponentType ComponentType { get; set; }

    /// <summary>
    /// The component ID of the log event. Will be <c>FnXXXXXX</c> for functions, and <c>WfXXXXXX</c> for workflows.
    /// </summary>
    public string? ComponentId { get; set; }

    /// <summary>
    /// The content of the log event, which depends on the <see cref="EventType"/>.
    /// </summary>
    public JObject Payload { get; set; } = new();

    /// <summary>
    /// When the log event was created, in epoch microseconds.
    /// </summary>
    public long Created { get; set; }

    [JsonIgnore]
    public DateTime CreatedDate => (Created / 1_000_000m).ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The trace ID of the log event.
    /// </summary>
    public string? TraceId { get; set; }
}

/// <summary>
/// The log level of an <see cref="AppActivity"/>, in order of relative importance from smallest to largest.
/// </summary>
public enum AppActivityLogLevel
{
    Trace,
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

/// <summary>
/// The source of an <see cref="AppActivity"/>.
/// </summary>
public enum AppActivitySource
{
    Slack,
    Developer
}

/// <summary>
/// The component type of an <see cref="AppActivity"/>.
/// </summary>
public enum AppActivityComponentType
{
    EventsApi,
    Workflows,
    Functions,
    Tables
}
