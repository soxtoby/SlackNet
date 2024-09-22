#nullable enable
using System.Collections.Generic;
using NUnit.Framework;

namespace SlackNet.Tests;

class TestLogger : ILogger
{
    public List<ILogEvent> Events { get; } = new();
        
    public void Log(ILogEvent logEvent)
    {
        Events.Add(logEvent);
        TestContext.WriteLine($"[{logEvent.Category}] {logEvent.FullMessage()}");
    }
}