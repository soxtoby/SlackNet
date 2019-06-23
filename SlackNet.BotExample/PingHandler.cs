using System;
using System.Threading.Tasks;
using SlackNet.Bot;

namespace SlackNet.BotExample
{
    internal class PingHandler : IMessageHandler
    {
        private int _count;

        public async Task HandleMessage(IMessage message)
        {
            if (message.Text.Contains("ping"))
            {
                Console.WriteLine($"Received ping from @{message.User.Name}");

                await message.ReplyWith(new BotMessage
                    {
                        Text = "pong",
                        Attachments = { new Attachment { Text = $"Count: {++_count}" } }
                    }).ConfigureAwait(false);
            }
        }
    }
}