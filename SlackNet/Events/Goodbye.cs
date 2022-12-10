namespace SlackNet.Events;

/// <summary>
/// Sent by a server that expects it will close the connection after an unspecified amount of time.
/// </summary>
public class Goodbye : Event { }