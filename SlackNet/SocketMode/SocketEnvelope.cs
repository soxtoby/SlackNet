namespace SlackNet.SocketMode
{
    public abstract class SocketEnvelope : SocketMessage
    {
        public string EnvelopeId { get; set; }
        public bool AcceptsResponsePayload { get; set; }
        public int? RetryAttempt { get; set; }
        public string RetryReason { get; set; }
    }

    public abstract class SocketEnvelope<T> : SocketEnvelope
    {
        public T Payload { get; set; }
    }
}