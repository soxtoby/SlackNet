using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet
{
    public class SlackRequestContext
    {
        private static readonly AsyncLocal<SlackRequestContext> CurrentContext = new();
        private readonly Dictionary<string, object> _items = new();
        private readonly Stack<Func<Task>> _onCompleteCallbacks = new();

        /// <summary>
        /// Returns true if there is a current request context available.
        /// </summary>
        public static bool IsAvailable => CurrentContext.Value is not null;

        /// <summary>
        /// Context for the current Slack request.
        /// </summary>
        /// <exception cref="InvalidOperationException">Not inside a request context</exception>
        public static SlackRequestContext Current
        {
            get => CurrentContext.Value ?? throw new InvalidOperationException("Not inside a request context");
            set => CurrentContext.Value = value;
        }

        /// <summary>
        /// Gets or sets the value with the associated key.
        /// Returns null if no value has been set for the key.
        /// </summary>
        public object this[string key]
        {
            get => _items.TryGetValue(key, out var value) ? value : default;
            set => _items[key] = value;
        }

        public bool Remove(string key) => _items.Remove(key);
        public bool ContainsKey(string key) => _items.ContainsKey(key);
        public bool TryGetValue(string key, out object value) => _items.TryGetValue(key, out value);

        public string RequestId => (string)_items[nameof(RequestId)];
        public ISlackServiceProvider ServiceProvider => (ISlackServiceProvider)_items[nameof(ServiceProvider)];
        public IEnumerable<IEventHandler> EventHandlers => (IEnumerable<IEventHandler>)_items[nameof(EventHandlers)];
        public IEnumerable<IAsyncBlockActionHandler> BlockActionHandlers => (IEnumerable<IAsyncBlockActionHandler>)_items[nameof(BlockActionHandlers)];
        public IHandlerIndex<IBlockOptionProvider> BlockOptionProviders => (IHandlerIndex<IBlockOptionProvider>)_items[nameof(BlockOptionProviders)];
        public IEnumerable<IAsyncMessageShortcutHandler> MessageShortcutHandlers => (IEnumerable<IAsyncMessageShortcutHandler>)_items[nameof(MessageShortcutHandlers)];
        public IEnumerable<IAsyncGlobalShortcutHandler> GlobalShortcutHandlers => (IEnumerable<IAsyncGlobalShortcutHandler>)_items[nameof(GlobalShortcutHandlers)];
        public IHandlerIndex<IAsyncViewSubmissionHandler> ViewSubmissionHandlers => (IHandlerIndex<IAsyncViewSubmissionHandler>)_items[nameof(ViewSubmissionHandlers)];
        public IHandlerIndex<IAsyncSlashCommandHandler> SlashCommandHandlers => (IHandlerIndex<IAsyncSlashCommandHandler>)_items[nameof(SlashCommandHandlers)];
        public IEnumerable<IAsyncWorkflowStepEditHandler> WorkflowStepEditHandlers => (IEnumerable<IAsyncWorkflowStepEditHandler>)_items[nameof(WorkflowStepEditHandlers)];
        public IHandlerIndex<IInteractiveMessageHandler> LegacyInteractiveMessageHandlers => (IHandlerIndex<IInteractiveMessageHandler>)_items[nameof(LegacyInteractiveMessageHandlers)];
        public IHandlerIndex<IOptionProvider> LegacyOptionProviders => (IHandlerIndex<IOptionProvider>)_items[nameof(LegacyOptionProviders)];
        public IHandlerIndex<IDialogSubmissionHandler> LegacyDialogSubmissionHandlers => (IHandlerIndex<IDialogSubmissionHandler>)_items[nameof(LegacyDialogSubmissionHandlers)];

        /// <summary>
        /// Registers a callback to be called when the request ends.
        /// Callbacks will be called in first-in-last-out order.
        /// </summary>
        public void OnComplete(Func<Task> onComplete) =>
            _onCompleteCallbacks.Push(onComplete);

        public IAsyncDisposable BeginRequest(IEnumerable<ISlackRequestListener> requestListeners)
        {
            foreach (var listener in requestListeners)
                listener.OnRequestBegin(this);

            return new SlackRequestContextScope(this);
        }

        class SlackRequestContextScope : IAsyncDisposable
        {
            private readonly SlackRequestContext _requestContext;
            public SlackRequestContextScope(SlackRequestContext requestContext) => _requestContext = requestContext;

            public async ValueTask DisposeAsync()
            {
                foreach (var callback in _requestContext._onCompleteCallbacks)
                    await callback().ConfigureAwait(false);
            }
        }
    }
}