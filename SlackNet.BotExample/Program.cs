using SlackNet.Bot;
using System;
using System.Threading.Tasks;

namespace SlackNet.BotExample
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
            using (var bot = new SlackBot(token))
            {
                bot.AddHandler(new PingHandler());

                await bot.Connect().ConfigureAwait(false);
                Console.WriteLine("Connected");

                await WaitForKeyPress().ConfigureAwait(false);
            }
        }

        private static async Task WaitForKeyPress()
        {
            Console.WriteLine("Press any key to disconnect...");
            while (!Console.KeyAvailable)
                await Task.Delay(250).ConfigureAwait(false);
            Console.ReadKey();
        }
    }
}
