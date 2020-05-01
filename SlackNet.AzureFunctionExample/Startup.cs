using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using SlackNet.AzureFunctionExample;
using SlackNet.Events;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SlackNet.AzureFunctionExample
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var accessToken = Environment.GetEnvironmentVariable("SlackAccessToken", EnvironmentVariableTarget.Process);
            var signingSecret = Environment.GetEnvironmentVariable("SlackSigningSecret", EnvironmentVariableTarget.Process);

            builder.Services.AddSlackNet(c => c
                .UseApiToken(accessToken)
                .RegisterEventHandler<MessageEvent, PingHandler>());

            builder.Services.AddSingleton(new SlackEndpointConfiguration()
                .UseSigningSecret(signingSecret));
        }
    }
}