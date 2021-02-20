using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public static class HandlerAdaptor
    {
        public static IEventHandler ToEventHandler<TEvent>(this IEventHandler<TEvent> handler) where TEvent : Event =>
            new TypedEventHandler<TEvent>(handler);

        public static IAsyncBlockActionHandler ToBlockActionHandler<TAction>(this IBlockActionHandler<TAction> handler, string actionId) where TAction : BlockAction =>
            new SpecificBlockActionHandler(actionId, new TypedBlockActionHandler<TAction>(handler).ToBlockActionHandler());

        public static IAsyncBlockActionHandler ToBlockActionHandler<TAction>(this IBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            new TypedBlockActionHandler<TAction>(handler).ToBlockActionHandler();

        public static IAsyncBlockActionHandler ToBlockActionHandler(this IBlockActionHandler handler) =>
            new BlockActionHandlerAsyncWrapper(handler);

        public static IAsyncBlockActionHandler ToBlockActionHandler<TAction>(this IAsyncBlockActionHandler<TAction> handler, string actionId) where TAction : BlockAction =>
            new SpecificBlockActionHandler(actionId, new TypedAsyncBlockActionHandler<TAction>(handler));

        public static IAsyncBlockActionHandler ToBlockActionHandler<TAction>(this IAsyncBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            new TypedAsyncBlockActionHandler<TAction>(handler);

        public static IAsyncMessageShortcutHandler ToMessageShortcutHandler(this IMessageShortcutHandler handler) =>
            new MessageShortcutHandlerAsyncWrapper(handler);

        public static IAsyncMessageShortcutHandler ToMessageShortcutHandler(this IMessageShortcutHandler handler, string callbackId) =>
            new MessageShortcutHandlerAsyncWrapper(handler).ToMessageShortcutHandler(callbackId);

        public static IAsyncMessageShortcutHandler ToMessageShortcutHandler(this IAsyncMessageShortcutHandler handler, string callbackId) =>
            new SpecificMessageShortcutHandler(callbackId, handler);

        public static IAsyncGlobalShortcutHandler ToGlobalShortcutHandler(this IGlobalShortcutHandler handler) =>
            new GlobalShortcutHandlerAsyncWrapper(handler);

        public static IAsyncGlobalShortcutHandler ToGlobalShortcutHandler(this IGlobalShortcutHandler handler, string callbackId) =>
            new GlobalShortcutHandlerAsyncWrapper(handler).ToGlobalShortcutHandler(callbackId);

        public static IAsyncGlobalShortcutHandler ToGlobalShortcutHandler(this IAsyncGlobalShortcutHandler handler, string callbackId) =>
            new SpecificGlobalShortcutHandler(callbackId, handler);

        public static IAsyncViewSubmissionHandler ToViewSubmissionHandler(this IViewSubmissionHandler handler) =>
            new ViewSubmissionHandlerAsyncWrapper(handler);

        public static IAsyncSlashCommandHandler ToSlashCommandHandler(this ISlashCommandHandler handler) =>
            new SlashCommandHandlerAsyncWrapper(handler);

        public static IAsyncWorkflowStepEditHandler ToWorkflowStepEditHandler(this IWorkflowStepEditHandler handler) =>
            new WorkflowStepEditHandlerAsyncWrapper(handler);

        public static IAsyncWorkflowStepEditHandler ToWorkflowStepEditHandler(this IWorkflowStepEditHandler handler, string callbackId) =>
            new WorkflowStepEditHandlerAsyncWrapper(handler).ToWorkflowStepEditHandler(callbackId);

        public static IAsyncWorkflowStepEditHandler ToWorkflowStepEditHandler(this IAsyncWorkflowStepEditHandler handler, string callbackId) =>
            new SpecificWorkflowStepEditHandler(callbackId, handler);
    }
}