using Microsoft.Extensions.Logging;
using SlackNetLogger = SlackNet.ILogger;

namespace SlackNet.AspNetCore;

class MicrosoftLoggerAdaptor : SlackNetLogger
{
    private readonly ILoggerFactory _loggerFactory;
    public MicrosoftLoggerAdaptor(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

    public void Log(ILogEvent logEvent) =>
        _loggerFactory
            .CreateLogger($"SlackNet.{logEvent.Category}")
            .Log(logEvent.Category switch
                {
                    LogCategory.Data => LogLevel.Trace,
                    LogCategory.Serialization => LogLevel.Trace,
                    LogCategory.Internal => LogLevel.Debug,
                    LogCategory.Request => LogLevel.Information,
                    LogCategory.Error => LogLevel.Error,
                    _ => LogLevel.Trace
                },
            logEvent.FullMessageTemplate(),
            logEvent.FullMessagePropertyValues());
}