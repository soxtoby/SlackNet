using System;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

class SlackHandlerFactory : ISlackHandlerFactory
{
    private const string RequestHandler = nameof(RequestHandler);
    private readonly Func<SlackRequestContext, IEventHandler> _eventHandler;
    private readonly Func<SlackRequestContext, IAsyncBlockActionHandler> _blockActionHandler;
    private readonly Func<SlackRequestContext, IBlockOptionProvider> _blockOptionProvider;
    private readonly Func<SlackRequestContext, IAsyncMessageShortcutHandler> _messageShortcutHandler;
    private readonly Func<SlackRequestContext, IAsyncGlobalShortcutHandler> _globalShortcutHandler;
    private readonly Func<SlackRequestContext, IAsyncViewSubmissionHandler> _viewSubmissionHandler;
    private readonly Func<SlackRequestContext, IAsyncSlashCommandHandler> _slashCommandHandler;
    private readonly Func<SlackRequestContext, IInteractiveMessageHandler> _legacyInteractiveMessageHandler;
    private readonly Func<SlackRequestContext, IOptionProvider> _legacyOptionProvider;
    private readonly Func<SlackRequestContext, IDialogSubmissionHandler> _legacyDialogSubmissionHandler;

    public SlackHandlerFactory(
        Func<SlackRequestContext, IEventHandler> eventHandler,
        Func<SlackRequestContext, IAsyncBlockActionHandler> blockActionHandler,
        Func<SlackRequestContext, IBlockOptionProvider> blockOptionProvider,
        Func<SlackRequestContext, IAsyncMessageShortcutHandler> messageShortcutHandler,
        Func<SlackRequestContext, IAsyncGlobalShortcutHandler> globalShortcutHandler,
        Func<SlackRequestContext, IAsyncViewSubmissionHandler> viewSubmissionHandler,
        Func<SlackRequestContext, IAsyncSlashCommandHandler> slashCommandHandler,
        Func<SlackRequestContext, IInteractiveMessageHandler> legacyInteractiveMessageHandler,
        Func<SlackRequestContext, IOptionProvider> legacyOptionProvider,
        Func<SlackRequestContext, IDialogSubmissionHandler> legacyDialogSubmissionHandler)
    {
        _eventHandler = eventHandler;
        _blockActionHandler = blockActionHandler;
        _blockOptionProvider = blockOptionProvider;
        _messageShortcutHandler = messageShortcutHandler;
        _globalShortcutHandler = globalShortcutHandler;
        _viewSubmissionHandler = viewSubmissionHandler;
        _slashCommandHandler = slashCommandHandler;
        _legacyInteractiveMessageHandler = legacyInteractiveMessageHandler;
        _legacyOptionProvider = legacyOptionProvider;
        _legacyDialogSubmissionHandler = legacyDialogSubmissionHandler;
    }

    public IEventHandler CreateEventHandler(SlackRequestContext context) => CreateHandler(context, _eventHandler);
    public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => CreateHandler(context, _blockActionHandler);
    public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => CreateHandler(context, _blockOptionProvider);
    public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => CreateHandler(context, _messageShortcutHandler);
    public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => CreateHandler(context, _globalShortcutHandler);
    public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => CreateHandler(context, _viewSubmissionHandler);
    public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => CreateHandler(context, _slashCommandHandler);
    public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => CreateHandler(context, _legacyInteractiveMessageHandler);
    public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => CreateHandler(context, _legacyOptionProvider);
    public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => CreateHandler(context, _legacyDialogSubmissionHandler);

    private static T CreateHandler<T>(SlackRequestContext context, Func<SlackRequestContext, T> handlerFactory)
    {
        if (context.ContainsKey(RequestHandler))
            throw new InvalidOperationException("Handler already created for this request");

        var handler = handlerFactory(context);
        context[RequestHandler] = handler;
        return handler;
    }
}