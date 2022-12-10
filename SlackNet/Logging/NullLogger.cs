namespace SlackNet;

class NullLogger : ILogger
{
    public void Log(ILogEvent logEvent) { }
}