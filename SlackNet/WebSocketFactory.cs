using System.Security.Authentication;
using WebSocket4Net;

namespace SlackNet
{
    public interface IWebSocketFactory
    {
        IWebSocket Create(string uri);
    }

    class WebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri) => new WebSocketWrapper(new WebSocket(uri, sslProtocols: SslProtocols.Tls12));
    }
}