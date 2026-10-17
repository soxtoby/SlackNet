using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EasyAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.AspNetCore;
using SlackNet.Events;
using Experimental = SlackNet.Handlers.Experimental;

namespace SlackNet.Tests;

public class SlackRequestHandlerTests
{
    private ServiceProvider _services;
    private Func<Experimental.IEventContext<Event>, Task> _handleEvent;
    private TaskCompletionSource<Experimental.IEventContext<Event>> _received;
    private TestLogger _logger;
    private ISlackRequestHandler _sut;
    private int _cleanupCount;

    [SetUp]
    public void SetUp()
    {
        _cleanupCount = 0;
        _received = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _handleEvent = _ => Task.CompletedTask;
        _logger = new TestLogger();
        var listener = new RequestCompletionListener(() => _cleanupCount++);
        var services = new ServiceCollection();
        services.AddSlackNet(c => c
            .RegisterEventHandler(new ContextEventHandler<Event>(HandleEvent))
            .UseLogger(_ => _logger)
            .UseRequestListener(_ => listener));
        _services = services.BuildServiceProvider();
        _sut = _services.GetRequiredService<ISlackRequestHandler>();
    }

    [TearDown]
    public void TearDown() => _services.Dispose();

    [Test]
    public async Task HandleEventRequest_HandlerAcknowledgesLater_WaitsToRespond()
    {
        var allowAcknowledgement = Signal();
        var handlerContinued = false;
        HandleEventsWith(async context =>
            {
                await allowAcknowledgement.Task;
                await context.Acknowledge();
                handlerContinued = true;
            });

        var response = ReceiveEvent();
        await WaitForHandler();
        response.IsCompleted.ShouldBe(false);

        allowAcknowledgement.SetResult();
        var result = await response;
        result.Status.ShouldBe(HttpStatusCode.OK);
        handlerContinued.ShouldBe(false);

        await CompleteResponse(result);
        handlerContinued.ShouldBe(true);
        _cleanupCount.ShouldBe(1);
        LoggedErrors.ShouldBeEmpty();
    }

    [Test]
    public async Task HandleEventRequest_HandlerContinuesAfterAcknowledgement_WaitsToCleanUp()
    {
        var finishHandling = Signal();
        HandleEventsWith(async context =>
            {
                await context.Acknowledge();
                await finishHandling.Task;
            });

        var result = await ReceiveEvent();
        var completion = CompleteResponse(result);
        completion.IsCompleted.ShouldBe(false);
        _cleanupCount.ShouldBe(0);

        finishHandling.SetResult();
        await completion;
        _cleanupCount.ShouldBe(1);
    }

    [Test]
    public async Task HandleEventRequest_HandlerCompletesWithoutAcknowledgement_ReturnsResponse()
    {
        HandleEventsWith(_ => Task.CompletedTask);

        var result = await ReceiveEvent();
        var context = await WaitForHandler();

        result.Status.ShouldBe(HttpStatusCode.OK);
        context.IsAcknowledged.ShouldBe(true);

        await CompleteResponse(result);

        _cleanupCount.ShouldBe(1);
    }

    [Test]
    public void HandleEventRequest_HandlerFailsBeforeAcknowledgement_ReportsFailureAndCleansUp()
    {
        var error = new InvalidOperationException("Handler failed");
        HandleEventsWith(_ => Task.FromException(error));

        ReceiveEvent().ShouldFail().And.ShouldReferTo(error);

        _cleanupCount.ShouldBe(1);
        LoggedErrors.ShouldMatchReferences(new[] { error });
    }

    [Test]
    public async Task HandleEventRequest_HandlerFailsAfterAcknowledgement_ReportsFailureAndCleansUp()
    {
        var error = new InvalidOperationException("Handler failed");
        HandleEventsWith(async context =>
            {
                await context.Acknowledge();
                throw error;
            });

        var result = await ReceiveEvent();
        CompleteResponse(result).ShouldFail().And.ShouldReferTo(error);

        _cleanupCount.ShouldBe(1);
        LoggedErrors.ShouldMatchReferences(new[] { error });
    }

    private void HandleEventsWith(Func<Experimental.IEventContext<Event>, Task> handle) => _handleEvent = handle;

    private Task HandleEvent(Experimental.IEventContext<Event> context)
    {
        _received.SetResult(context);
        return _handleEvent(context);
    }

    private Task<SlackResult> ReceiveEvent()
    {
        var eventCallback = new EventCallback
            {
                Type = "event_callback",
                EventId = "event",
                Event = new MessageEvent { Type = "message", Text = "message" }
            };
        var body = JsonConvert.SerializeObject(eventCallback, Default.JsonSettings().SerializerSettings);

        var http = new DefaultHttpContext
            {
                RequestServices = _services,
                Request =
                    {
                        Method = "POST",
                        ContentType = "application/json",
                        Body = new MemoryStream(Encoding.UTF8.GetBytes(body))
                    }
            };
        _services.GetRequiredService<IHttpContextAccessor>().HttpContext = http;
        return _sut.HandleEventRequest(http.Request).WaitAsync(TimeSpan.FromSeconds(5));
    }

    private Task<Experimental.IEventContext<Event>> WaitForHandler() => _received.Task.WaitAsync(TimeSpan.FromSeconds(5));

    private static async Task CompleteResponse(SlackResult result)
    {
        foreach (var callback in result.RequestCompletedCallbacks)
            await callback().WaitAsync(TimeSpan.FromSeconds(5));
    }

    private IEnumerable<Exception> LoggedErrors => _logger.Events.Where(e => e.Category == LogCategory.Error).Select(e => e.Exception);

    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);
}
