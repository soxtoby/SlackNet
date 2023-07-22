using System;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;

namespace SlackNet;

class SerializationLogger : ITraceWriter
{
    private readonly ILogger _log;
    public SerializationLogger(ILogger logging) => _log = logging.ForSource<SerializationLogger>();

    public void Trace(TraceLevel level, string message, Exception ex) =>
        _log.WithContext("TraceLevel", level)
            .Serialization(ex, "{Message}", message);

    public TraceLevel LevelFilter => TraceLevel.Verbose;
}