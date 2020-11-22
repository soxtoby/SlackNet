using SlackNet.Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.BotExample
{
    public class Program
    {
        static string botToken = "xoxb-1111176898965-1478263177395-RRpDKHeSeo8niRuknzVH06so"; 
        static string appToken = "xapp-1-A01E1NQ5X18-1483935432724-ab68e1b51682ebfceffb1684eb5a1672d184de59df979c830ce42a25e397bba7";
        private static string proxy = "http://vzproxy.verizon.com:9290";
        static bool useSocketMode = true;

        //Not used yet but will be soon
        //string signingSecret = "";
        //bool twoWayTLS = false;
        //bool validateRequests = false;
        //string clientSerial = "";

        public static async Task Main(string[] args)
        {
            if (args.Length == 0 && botToken.Length < 10)
            {
                Console.WriteLine("Please provide a Slack token as a command line argument.");
                System.Console.ReadKey();
            }
            else
            {
                await Run(botToken, appToken).ConfigureAwait(false);
            }
        }

        private static async Task Run(string token, string apptoken=null)
        {
            //////code forklifted from our code....

            SlackBot SlackBotRTM;
            SlackApiClient SlackAPI;
            ISlackRtmClient SlackRTM;

            SlackJsonSettings _js = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));

            HttpClientHandler handler = getProxy();

            HttpClient _hc = new HttpClient(handler);

            //ProxySupportingWebSocketFactory would be a copy of WebSocketFactory that configures the WebSockets with the proxy.
            var ProxySupportingWebSocketFactory = new SlackNetx.WebSocketFactory();

            //ProxySupportingHttp can probably be a copy of Http with its own HttpClient that's been configured with the proxy.
            var ProxSypportingHTTP = new SlackNetx.Http(_hc, _js);

            ISlackApiClient _api =  SlackAPI = new SlackApiClient(ProxSypportingHTTP, Default.UrlBuilder(_js), _js, token, apptoken);

            ISlackRtmClient _rtm = SlackRTM = new SlackRtmClient(_api, ProxySupportingWebSocketFactory, _js, null);
            
            // Connect to the server now 
            try
            {

                SlackBotRTM = new SlackBot(_rtm, _api);
                SlackBotRTM.AddHandler(new PingHandler());
                SlackRTM.Events.Subscribe(
                    (async mm =>
                    {
                        //Send acknowledgement, THIS SHOULD BE IN THE LOWER LEVEL API
                        if (mm.Envelope_ID != null && mm.Envelope_ID.Length > 1)
                        {
                            SlackRTM.SendAck(mm.Envelope_ID);
                        }
                    }
                        ));

                System.Console.WriteLine("Trying to connect");
                SlackBotRTM.Connect(null, useSocketMode).Wait();
            }
            catch (Exception x)
            {
                System.Console.WriteLine("[connect]"+x.Message);
                System.Console.WriteLine("[connect]" + x.StackTrace.ToString());
            }



            string pathSource= @"C:\temp\399230356334.pdf";

            using (FileStream fsSource = new FileStream(pathSource, FileMode.Open, FileAccess.Read))
            {
                // Read the source file into a byte array.
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                numBytesToRead = bytes.Length;



                FilesApi fa = new FilesApi(SlackAPI);
                await fa.Upload(bytes, "pdf", "399230356334.pdf", "Return Label 399230356334",
                    "This is a return label", null, new List<string> { "C01E27DR0H1" });
            }


            await WaitForKeyPress().ConfigureAwait(false);
        }

        private static async Task WaitForKeyPress()
        {
            Console.WriteLine("Press any key to disconnect...");
            while (!Console.KeyAvailable)
                await Task.Delay(250).ConfigureAwait(false);
            Console.ReadKey();
        }


        //TODO: this shoud be generic, proxy base URL probably should be a config value
        private static HttpClientHandler getProxy()
        {
            HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    Proxy = new WebProxy(proxy),
                    UseProxy = true,
                };

            return handler;
        }

    }
}