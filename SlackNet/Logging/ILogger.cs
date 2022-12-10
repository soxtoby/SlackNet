#nullable enable
namespace SlackNet;

public interface ILogger
{
    public void Log(ILogEvent logEvent);
}