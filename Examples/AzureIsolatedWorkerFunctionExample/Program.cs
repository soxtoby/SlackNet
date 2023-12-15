﻿using System;
using AzureIsolatedWorkerFunctionExample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlackNet.AspNetCore;
using SlackNet.AzureFunctions;
using SlackNet.Events;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication( app =>
		{
			app.UseSlackNet();
		})
	.ConfigureServices(services => {
			var apiToken = Environment.GetEnvironmentVariable("SlackApiToken", EnvironmentVariableTarget.Process);
			var signingSecret = Environment.GetEnvironmentVariable("SlackSigningSecret", EnvironmentVariableTarget.Process);

			AzureFunctionExtensions.AddSlackNet( services, c => c
				// Configure the token used to authenticate with Slack
				.UseApiToken(apiToken)
            
				// Register your Slack handlers here
				.RegisterEventHandler<MessageEvent, PingDemo>()
			);

			// This is roughly equivalent to the .UseSlackNet() call in ASP.NET Core
			services.AddSingleton(new SlackEndpointConfiguration()
				// The signing secret ensures that SlackNet only handles requests from Slack
				.UseSigningSecret(signingSecret));
		})
	.Build();

host.Run();