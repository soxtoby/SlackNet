using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using Button = SlackNet.Blocks.Button;

namespace SlackNet.EventsExample
{
    public class AppHome : IEventHandler<AppHomeOpened>, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
    {
        private const string OpenModal = "open_modal";
        private const string InputBlockId = "input_block";
        private const string InputActionId = "text_input";
        public static readonly string ModalCallbackId = "home_modal";

        private readonly ISlackApiClient _slack;
        public AppHome(ISlackApiClient slack) => _slack = slack;

        public async Task Handle(AppHomeOpened slackEvent)
        {
            if (slackEvent.Tab == AppHomeTab.Home)
                await _slack.Views.Publish(slackEvent.User, new HomeViewDefinition
                    {
                        Blocks =
                            {
                                new SectionBlock { Text = "Welcome to the SlackNet example home" },
                                new ActionsBlock
                                    {
                                        Elements =
                                            {
                                                new Button
                                                    {
                                                        Text = "Open modal",
                                                        ActionId = OpenModal
                                                    }
                                            }
                                    }
                            }
                    }, slackEvent.View?.Hash).ConfigureAwait(false);
        }

        public async Task Handle(ButtonAction action, BlockActionRequest request)
        {
            if (action.ActionId == OpenModal)
                await _slack.Views.Open(request.TriggerId, new ModalViewDefinition
                    {
                        Title = "Example Modal",
                        CallbackId = ModalCallbackId,
                        Blocks =
                            {
                                new InputBlock
                                    {
                                        Label = "Input",
                                        BlockId = InputBlockId,
                                        Element = new PlainTextInput
                                            {
                                                ActionId = InputActionId,
                                                Placeholder = "Enter some text"
                                            }
                                    }
                            },
                        Submit = "Submit",
                        NotifyOnClose = true
                    }).ConfigureAwait(false);
        }

        public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
        {
            await _slack.Chat.PostMessage(new Message
                {
                    Channel = await UserIm(viewSubmission.User).ConfigureAwait(false),
                    Text = $"You entered: {viewSubmission.View.State.Values[InputBlockId][InputActionId].Value}"
                }).ConfigureAwait(false);

            return ViewSubmissionResponse.Null;
        }

        public async Task HandleClose(ViewClosed viewClosed)
        {
            await _slack.Chat.PostMessage(new Message
                {
                    Channel = await UserIm(viewClosed.User).ConfigureAwait(false),
                    Text = "You cancelled the modal"
                }).ConfigureAwait(false);
        }

        private Task<string> UserIm(User user)
        {
            return _slack.Conversations.Open(new[] { user.Id });
        }
    }
}