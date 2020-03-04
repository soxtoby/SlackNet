using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackNet.Interaction;
using ActionElement = SlackNet.Interaction.ActionElement;
using Button = SlackNet.Interaction.Button;

namespace SlackNet.EventsExample
{
    public class LegacyCounter : IInteractiveMessageHandler
    {
        public static readonly string ActionName = "add";

        private static readonly Regex _counterPattern = new Regex("Counter: (\\d+)");

        public Task<MessageResponse> Handle(InteractiveMessage message)
        {
            var counterText = _counterPattern.Match(message.OriginalAttachment.Text);
            if (counterText.Success)
            {
                var count = int.Parse(counterText.Groups[1].Value);
                var increment = int.Parse(message.Action.Value);
                message.OriginalAttachment.Text = $"Counter: {count + increment}";
                message.OriginalAttachment.Actions = Actions;
                return Task.FromResult(new MessageResponse
                    {
                        ReplaceOriginal = true,
                        Message = message.OriginalMessage
                    });
            }

            return Task.FromResult<MessageResponse>(null);
        }

        public static IList<ActionElement> Actions => new List<ActionElement>
            {
                new Button
                    {
                        Name = ActionName,
                        Value = "1",
                        Text = "Add 1"
                    },
                new Button
                    {
                        Name = ActionName,
                        Value = "5",
                        Text = "Add 5"
                    },
                new Button
                    {
                        Name = ActionName,
                        Value = "10",
                        Text = "Add 10"
                    }
            };
    }
}