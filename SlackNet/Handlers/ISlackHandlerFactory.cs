using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

public interface ISlackHandlerFactory
{
    Experimental.IEventHandler<Event> CreateEventHandler(SlackRequestContext context);
    IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context);
    IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context);
    IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context);
    IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context);
    IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context);
    IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context);

    IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context);
    IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context);
    IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context);
}
