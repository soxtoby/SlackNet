using System.Collections.Generic;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class SlackRequestContext
    {
        private readonly Dictionary<string, object> _items = new();

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

        public ISlackServiceFactory ServiceFactory => (ISlackServiceFactory)_items[nameof(ServiceFactory)];
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
    }
}