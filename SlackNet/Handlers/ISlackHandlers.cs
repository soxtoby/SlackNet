using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public interface ISlackHandlers
    {
        IEventHandler EventHandler { get; }
        IAsyncBlockActionHandler BlockActionHandler { get; }
        IBlockOptionProvider BlockOptionProvider { get; }
        IInteractiveMessageHandler InteractiveMessageHandler { get; }
        IAsyncMessageShortcutHandler MessageShortcutHandler { get; }
        IAsyncGlobalShortcutHandler GlobalShortcutHandler { get; }
        IOptionProvider OptionProvider { get; }
        IDialogSubmissionHandler DialogSubmissionHandler { get; }
        IAsyncViewSubmissionHandler ViewSubmissionHandler { get; }
        IAsyncSlashCommandHandler SlashCommandHandler { get; }
        IAsyncWorkflowStepEditHandler WorkflowStepEditHandler { get; }
    }
}