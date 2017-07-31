using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SlackNet.WebApi;

namespace SlackNet.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Please provide a Slack token as a command line argument.");
            else
                Run(args[0]).Wait();
        }

        private static async Task Run(string token)
        {
            var count = 0;
            var api = new SlackApiClient(token);
            using (var rtmClient = new SlackRtmClient(token))
            {
                await rtmClient.Connect().ConfigureAwait(false);
                Console.WriteLine("Connected");

                rtmClient.Messages
                    .Where(m => m.Text.Contains("ping"))
                    .Subscribe(async m =>
                        {
                            var user = (await api.Users.Info(m.User).ConfigureAwait(false));
                            Console.WriteLine($"Received ping from @{user.Name}");

                            await api.Chat.PostMessage(new Message
                                {
                                    Channel = m.Channel,
                                    Text = "pong",
                                    Attachments = { new Attachment { Text = $"Count: {++count}" } }
                                }).ConfigureAwait(false);
                        });

                await rtmClient.Events;
            }
        }
    }
}