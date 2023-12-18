using AzureFunctionsExample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SlackNet.AzureFunctions;
using SlackNet.Events;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
        {
            builder.UseSlackNet();
        })
    .ConfigureServices((hostContext, services) =>
        {
            services.AddSlackNet(c => c
                // Configure the token used to authenticate with Slack
                .UseApiToken(hostContext.Configuration["SlackApiToken"]!)
                
                // The signing secret ensures that SlackNet only handles requests from Slack
                .UseSigningSecret(hostContext.Configuration["SlackSigningSecret"]!)
            
                // Register your Slack handlers here
                .RegisterEventHandler<MessageEvent, PingDemo>()
            );
        })
    .Build();

host.Run();