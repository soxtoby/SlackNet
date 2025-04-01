using System.Net.WebSockets;
using System.Security.Authentication;

namespace SlackNet;

public interface IWebSocketFactory
{
    IWebSocket Create(string uri);
}

class WebSocketFactory : IWebSocketFactory
{
    public IWebSocket Create(string uri) => new WebSocketWrapper(new ClientWebSocket(), uri);
}