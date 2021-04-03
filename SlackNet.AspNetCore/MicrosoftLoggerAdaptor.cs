using Microsoft.Extensions.Logging;
using SlackNetLogger = SlackNet.ILogger;
using MicrosoftLogger = Microsoft.Extensions.Logging.ILogger;

namespace SlackNet.AspNetCore
{
    class MicrosoftLoggerAdaptor : SlackNetLogger
    {
        private readonly MicrosoftLogger _logger;
        public MicrosoftLoggerAdaptor(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger("SlackNet");

        public void Log(ILogEvent logEvent) =>
            _logger.Log(logEvent.Category switch
                    {
                        LogCategory.Data => LogLevel.Trace,
                        LogCategory.Internal => LogLevel.Debug,
                        LogCategory.Request => LogLevel.Information,
                        LogCategory.Error => LogLevel.Error,
                        _ => LogLevel.Trace
                    },
                logEvent.FullMessageTemplate(),
                logEvent.FullMessagePropertyValues());
    }
}