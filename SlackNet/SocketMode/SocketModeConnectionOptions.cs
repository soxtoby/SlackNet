using System;

namespace SlackNet.SocketMode;

public class SocketModeConnectionOptions
{
    /// <summary>
    /// Number of connections to create.
    /// </summary>
    public int NumberOfConnections
    {
        get;
        set => field = value >= 1
            ? value
            : throw new ArgumentOutOfRangeException(nameof(NumberOfConnections), "Must have at least 1 connection");
    } = 2;

    /// <summary>
    /// Delay between creating connections to avoid connections expiring at the same time.
    /// </summary>
    public TimeSpan ConnectionDelay
    {
        get;
        set => field = value >= TimeSpan.Zero
            ? value
            : throw new ArgumentOutOfRangeException(nameof(ConnectionDelay), "Delay cannot be negative");
    } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Makes the connection time significantly shorter (360 seconds), for testing and debugging reconnects without waiting around.
    /// </summary>
    public bool DebugReconnects { get; set; }
}