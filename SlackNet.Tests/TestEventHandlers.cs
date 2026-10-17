using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Events;
using Experimental = SlackNet.Handlers.Experimental;

namespace SlackNet.Tests;

class ContextEventHandler<TEvent>(Func<Experimental.IEventContext<TEvent>, Task> handle) : Experimental.IEventHandler<TEvent> where TEvent : Event
{
    public Task Handle(Experimental.IEventContext<TEvent> context) => handle(context);
}

class RecordingContextEventHandler<TEvent> : Experimental.IEventHandler<TEvent> where TEvent : Event
{
    public List<Experimental.IEventContext<TEvent>> Contexts { get; } = new();

    public Task Handle(Experimental.IEventContext<TEvent> context)
    {
        Contexts.Add(context);
        return Task.CompletedTask;
    }
}

class RecordingEventHandler : IEventHandler
{
    public List<EventCallback> Callbacks { get; } = new();

    public Task Handle(EventCallback callback)
    {
        Callbacks.Add(callback);
        return Task.CompletedTask;
    }
}

class RecordingEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : Event
{
    public List<TEvent> Events { get; } = new();

    public Task Handle(TEvent slackEvent)
    {
        Events.Add(slackEvent);
        return Task.CompletedTask;
    }
}
