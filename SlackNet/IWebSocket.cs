using System;
using System.Reactive;
using System.Reactive.Linq;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace SlackNet
{
    public interface IWebSocket : IDisposable
    {
        void Open();
        void Send(string message);
        WebSocketState State { get; }
        IObservable<Unit> Opened { get; }
        IObservable<Unit> Closed { get; }
        IObservable<Exception> Errors { get; }
        IObservable<string> Messages { get; }
    }

    public class WebSocketWrapper : IWebSocket
    {
        private readonly WebSocket _webSocket;

        public WebSocketWrapper(WebSocket webSocket)
        {
            _webSocket = webSocket;
        }

        public void Open() => _webSocket.Open();

        public void Send(string message) => _webSocket.Send(message);

        public WebSocketState State => _webSocket.State;

        public IObservable<Unit> Opened => Observable.FromEventPattern(h => _webSocket.Opened += h, h => _webSocket.Opened -= h)
            .Select(_ => Unit.Default);

        public IObservable<Unit> Closed => Observable.FromEventPattern(h => _webSocket.Closed += h, h => _webSocket.Closed -= h)
            .Select(_ => Unit.Default);

        public IObservable<Exception> Errors => Observable.FromEventPattern<ErrorEventArgs>(h => _webSocket.Error += h, h => _webSocket.Error -= h)
            .Select(e => e.EventArgs.Exception);

        public IObservable<string> Messages => Observable.FromEventPattern<MessageReceivedEventArgs>(h => _webSocket.MessageReceived += h, h => _webSocket.MessageReceived -= h)
            .Select(e => e.EventArgs.Message);

        public void Dispose() => _webSocket.Dispose();
    }
}