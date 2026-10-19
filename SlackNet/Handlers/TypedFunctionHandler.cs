using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.Events;
using SlackNet.Handlers.Experimental;

namespace SlackNet.Handlers;

class TypedFunctionHandler<TInputs>(Experimental.IEventHandler<FunctionExecuted<TInputs>> handler, SlackJsonSettings jsonSettings)
    : Experimental.IEventHandler<FunctionExecuted>, IComposedHandler<EventCallback>
{
    public Task Handle(IEventContext<FunctionExecuted> eventContext) =>
        handler.Handle(new EventContext<FunctionExecuted<TInputs>>(eventContext, new FunctionExecuted<TInputs>
            {
                Type = eventContext.Event.Type,
                EventTs = eventContext.Event.EventTs,
                ExtraProperties = eventContext.Event.ExtraProperties,
                Function = eventContext.Event.Function,
                FunctionExecutionId = eventContext.Event.FunctionExecutionId,
                WorkflowExecutionId = eventContext.Event.WorkflowExecutionId,
                BotAccessToken = eventContext.Event.BotAccessToken,
                Inputs = eventContext.Event.Inputs is not null
                    ? eventContext.Event.Inputs.ToObject<TInputs>(JsonSerializer.Create(jsonSettings.SerializerSettings))
                    : default
            }));

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        handler.InnerHandlers(eventCallback);
}
