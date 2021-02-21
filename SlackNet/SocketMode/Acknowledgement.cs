namespace SlackNet.SocketMode
{
    public class Acknowledgement
    {
        public string EnvelopeId { get; set; }
    }

    public class Acknowledgement<T> : Acknowledgement
    {
        public T Payload { get; set; }
    }
}