using System.Security.Authentication;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using SuperSocket.ClientEngine.Proxy;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SlackNet;
using Utils;

namespace SlackNetx
{

    public class WebSocketFactory : IWebSocketFactory
    {
        private string proxyurl = "yourproxy:port";



        public IWebSocket Create(string uri)
        {

           var _socket = new WebSocket(uri, sslProtocols: SslProtocols.Tls12); //"ws://echo.websocket.org", origin: "http://example.com");

            //todo: need to add authentication to the proxy call
            var proxy = new HttpConnectProxy(IPUtils.Parse(proxyurl));
            _socket.Proxy = (SuperSocket.ClientEngine.IProxyConnector)proxy;

            _socket.Opened += new EventHandler(onsocketopened);
            _socket.Error += new EventHandler<ErrorEventArgs>(onsocketerror);
            _socket.Closed += new EventHandler(onsocketclosed);
            _socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(onsocketmessagereceived);

            var _wrapper = new WebSocketWrapper(_socket);

            return _wrapper;
        }

        private void onsocketopened(object sender, EventArgs e)
        {
            System.Console.WriteLine("WS Socket Open");
        }

        private void onsocketclosed(object sender, EventArgs e)
        {
            System.Console.WriteLine("WS socket closed");
            //EventHandler<EventArgs> handler = WSLog;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}
        }

        private void onsocketerror(object sender, EventArgs e)
        {
            System.Console.WriteLine("WS socket Error");
            //EventHandler<EventArgs> handler = WSLog;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}
        }

        private void onsocketmessagereceived(object sender, MessageReceivedEventArgs e)
        {
            //if (_logger != null)
            //{
                System.Console.WriteLine("WS Msg Received: " + e.Message);
            //}
        }

        public event EventHandler<EventArgs> WSLog;

    }

    public class WSEventArgs : EventArgs
    {
        public string message { get; set; }

    }
}