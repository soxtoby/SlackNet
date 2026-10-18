using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.SocketMode;
using Experimental = SlackNet.Handlers.Experimental;

namespace SlackNet.Tests;

public class SlackSocketModeClientTests
{
    private RecordingSocket _socket;
    private Experimental.IEventHandler<Event> _handler;
    private SlackSocketModeClient _sut;
    private TaskCompletionSource<Experimental.IEventContext<Event>> _received;
    private TaskCompletionSource<Acknowledgement> _response;
    private TaskCompletionSource _cleanedUp;
    private TaskCompletionSource<Exception> _error;
    private Exception _sendError;
    private int _responseCount;
    private int _responseSocketId;
    private int _cleanupCount;

    [SetUp]
    public void SetUp()
    {
        ResetRequestTracking();
        _socket = new RecordingSocket(RecordResponse);
        _sut = CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
        _socket.Dispose();
    }

    [Test]
    public async Task HandleRequest_HandlerAcknowledgesLater_WaitsToRespond()
    {
        var allowAcknowledgement = Signal();
        HandleEventsWith(async context =>
            {
                await allowAcknowledgement.Task;
                await context.Acknowledge();
            });

        ReceiveEvent();
        await WaitForHandler();
        _response.Task.IsCompleted.ShouldBe(false);

        allowAcknowledgement.SetResult();
        var result = await WaitForResponse();
        await WaitForCleanup();

        result.EnvelopeId.ShouldBe("envelope");
        _responseSocketId.ShouldBe(7);
        _responseCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleRequest_HandlerContinuesAfterAcknowledgement_WaitsToCleanUp()
    {
        var finishHandling = Signal();
        HandleEventsWith(async context =>
            {
                await context.Acknowledge();
                await finishHandling.Task;
            });

        ReceiveEvent();
        await WaitForResponse();
        _cleanupCount.ShouldBe(0);

        finishHandling.SetResult();
        await WaitForCleanup();

        _cleanupCount.ShouldBe(1);
        _responseCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleRequest_HandlerCompletesWithoutAcknowledgement_SendsResponse()
    {
        HandleEventsWith(_ => Task.CompletedTask);

        ReceiveEvent();
        var context = await WaitForHandler();
        var result = await WaitForResponse();
        await WaitForCleanup();

        result.EnvelopeId.ShouldBe("envelope");
        context.IsAcknowledged.ShouldBe(true);
        _responseCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleRequest_HandlerFailsBeforeAcknowledgement_LogsFailureAndAcknowledges()
    {
        var error = new InvalidOperationException("Handler failed");
        HandleEventsWith(_ => Task.FromException(error));

        ReceiveEvent();
        var logged = await WaitForError();
        var result = await WaitForResponse();

        logged.ShouldReferTo(error);
        result.EnvelopeId.ShouldBe("envelope");
        _responseCount.ShouldBe(1);
        _cleanupCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleRequest_HandlerFailsAfterAcknowledgement_LogsFailureWithoutAnotherResponse()
    {
        var error = new InvalidOperationException("Handler failed");
        HandleEventsWith(async context =>
            {
                await context.Acknowledge();
                throw error;
            });

        ReceiveEvent();
        var result = await WaitForError();

        result.ShouldReferTo(error);
        _responseCount.ShouldBe(1);
        _cleanupCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleRequest_AcknowledgementFails_SkipsLegacyHandler()
    {
        var error = new InvalidOperationException("Acknowledgement failed");
        _sendError = error;
        var legacy = new RecordingEventHandler();
        HandleLegacyEventsWith(legacy);

        ReceiveEvent();
        var result = await WaitForError();

        result.ShouldReferTo(error);
        legacy.Callbacks.ShouldBeEmpty();
        _responseCount.ShouldBe(1);
        _cleanupCount.ShouldBe(1);
    }

    private void HandleEventsWith(Func<Experimental.IEventContext<Event>, Task> handle) =>
        _handler = new ContextEventHandler<Event>(context =>
            {
                _received.SetResult(context);
                return handle(context);
            });

    private void HandleLegacyEventsWith(IEventHandler legacy) =>
        _handler = legacy.ToEventHandler();

    private void ReceiveEvent() => _socket.Incoming.OnNext(new EventEnvelope
        {
            SocketId = 7,
            EnvelopeId = "envelope",
            Payload = new EventCallback { EventId = "event", Event = new MessageEvent { Text = "message" } }
        });

    private Task<Experimental.IEventContext<Event>> WaitForHandler() => _received.Task.WaitAsync(TimeSpan.FromSeconds(5));
    private Task<Acknowledgement> WaitForResponse() => _response.Task.WaitAsync(TimeSpan.FromSeconds(5));
    private Task WaitForCleanup() => _cleanedUp.Task.WaitAsync(TimeSpan.FromSeconds(5));
    private Task<Exception> WaitForError() => _error.Task.WaitAsync(TimeSpan.FromSeconds(5));

    private void ResetRequestTracking()
    {
        _responseCount = _responseSocketId = _cleanupCount = 0;
        _sendError = null;
        _received = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _response = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _cleanedUp = Signal();
        _error = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private Task RecordResponse(int socketId, Acknowledgement acknowledgement)
    {
        _responseCount++;
        _responseSocketId = socketId;
        _response.TrySetResult(acknowledgement);
        return _sendError is null ? Task.CompletedTask : Task.FromException(_sendError);
    }

    private SlackSocketModeClient CreateClient()
    {
        var services = new SlackServiceBuilder().ReplaceEventHandling(_ => _handler);
        return new SlackSocketModeClient(_socket, services.GetJsonSettings(), new[] { CreateCompletionListener() }, services.GetHandlerFactory(), new ErrorLogger(_error));
    }

    private RequestCompletionListener CreateCompletionListener() => new(() =>
        {
            _cleanupCount++;
            _cleanedUp.SetResult();
        });

    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    class RecordingSocket(Func<int, Acknowledgement, Task> send) : ICoreSocketModeClient
    {
        public Subject<SocketMessage> Incoming { get; } = new();
        public IObservable<SocketMessage> Messages => Incoming;
        public IObservable<RawSocketMessage> RawSocketMessages => throw new NotSupportedException();
        public bool Connected => true;

        public Task Connect(SocketModeConnectionOptions connectionOptions = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Disconnect() { }
        public Task DisconnectAsync() => Task.CompletedTask;
        public Task Send(int socketId, Acknowledgement acknowledgement) => send(socketId, acknowledgement);
        public void Dispose() => Incoming.Dispose();

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }

    class ErrorLogger(TaskCompletionSource<Exception> error) : ILogger
    {
        public void Log(ILogEvent logEvent)
        {
            if (logEvent.Category == LogCategory.Error)
                error.TrySetResult(logEvent.Exception);
        }
    }
}
