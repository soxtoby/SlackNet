using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using SlackNet.Events;

namespace SlackNet.EventsExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSlackNet(c => c
                .UseApiToken(Configuration["Slack:ApiToken"])
                .RegisterEventHandler<MessageEvent, MessageHandler>()
                .RegisterInteractiveMessageHandler<Counter>(Counter.ActionName)
                .RegisterInteractiveMessageHandler<ColorSelector>(ColorSelector.ActionName)
                .RegisterOptionProvider<ColorSelector>(ColorSelector.ActionName)
                .RegisterInteractiveMessageHandler<DialogDemo>(DialogDemo.EchoDialog)
                .RegisterInteractiveMessageHandler<DialogDemo>(DialogDemo.ErrorDialog)
                .RegisterDialogSubmissionHandler<DialogDemo>());
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSlackNet(c => c.VerifyWith(Configuration["Slack:VerificationToken"]));

            app.UseMvc();
        }
    }
}
