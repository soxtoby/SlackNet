using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SlackNet.Objects;
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
            int count = 0;
            var api = new WebApiClient(token);
            using (var rtmClient = new RtmClient(token))
            {
                await rtmClient.Connect().ConfigureAwait(false);
                Console.WriteLine("Connected");

                rtmClient.Messages
                    .Where(m => m.Text.Contains("ping"))
                    .Subscribe(async m =>
                        {
                            User user = (await api.Users.Info(m.User).ConfigureAwait(false));
                            Console.WriteLine($"Received ping from @{user.Name}");

                            await api.Chat.PostMessage(new SlackMessage
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