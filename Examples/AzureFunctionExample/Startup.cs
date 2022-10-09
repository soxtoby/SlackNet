using System;
using AzureFunctionExample;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using SlackNet.Events;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureFunctionExample;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var apiToken = Environment.GetEnvironmentVariable("SlackApiToken", EnvironmentVariableTarget.Process);
        var signingSecret = Environment.GetEnvironmentVariable("SlackSigningSecret", EnvironmentVariableTarget.Process);

        builder.Services.AddSlackNet(c => c
            // Configure the token used to authenticate with Slack
            .UseApiToken(apiToken)
            
            // Register your Slack handlers here
            .RegisterEventHandler<MessageEvent, PingDemo>()
        );

        // This is roughly equivalent to the .UseSlackNet() call in ASP.NET Core
        builder.Services.AddSingleton(new SlackEndpointConfiguration()
            // The signing secret ensures that SlackNet only handles requests from Slack
            .UseSigningSecret(signingSecret));
    }
}