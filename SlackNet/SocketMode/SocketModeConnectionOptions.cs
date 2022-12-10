using System;

namespace SlackNet.SocketMode;

public class SocketModeConnectionOptions
{
    private int _numberOfConnections = 2;
    private TimeSpan _connectionDelay = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Number of connections to create.
    /// </summary>
    public int NumberOfConnections
    {
        get => _numberOfConnections;
        set => _numberOfConnections = value >= 1 ? value
            : throw new ArgumentOutOfRangeException(nameof(NumberOfConnections), "Must have at least 1 connection");
    }

    /// <summary>
    /// Delay between creating connections, to avoid connections expiring at the same time.
    /// </summary>
    public TimeSpan ConnectionDelay
    {
        get => _connectionDelay;
        set => _connectionDelay = value >= TimeSpan.Zero ? value
            : throw new ArgumentOutOfRangeException(nameof(ConnectionDelay), "Delay cannot be negative");
    }

    /// <summary>
    /// Makes the connection time significantly shorter (360 seconds), for testing and debugging reconnects without waiting around.
    /// </summary>
    public bool DebugReconnects { get; set; }
}