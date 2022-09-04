using SlackNet.AspNetCore;
using SlackNet.Events;
using SlackNet.EventsAspNetCoreExample.Handlers;
using SlackNet.EventsAspNetCoreExample.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = builder.Configuration.GetSection("Slack").Get<Settings>();

var accessToken = Environment.GetEnvironmentVariable("SlackAccessToken") ?? settings.SlackAccessToken;
var signingSecret = Environment.GetEnvironmentVariable("SlackSigningSecret") ?? settings.SlackSigningSecret;


#if DEBUG
builder.Services.AddSingleton(new SlackEndpointConfiguration());
#else
builder.Services.AddSingleton(new SlackEndpointConfiguration().UseSigningSecret(signingSecret));
#endif

builder.Services.AddSlackNet(c => c
.UseApiToken(accessToken)
.RegisterEventHandler<MessageEvent, PingHandler>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

