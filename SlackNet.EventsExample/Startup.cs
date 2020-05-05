using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
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

                .RegisterBlockActionHandler<ButtonAction, BlockCounter>(BlockCounter.Add1)
                .RegisterBlockActionHandler<ButtonAction, BlockCounter>(BlockCounter.Add5)
                .RegisterBlockActionHandler<ButtonAction, BlockCounter>(BlockCounter.Add10)
                .RegisterBlockActionHandler<ExternalSelectAction, BlockColorSelector>(BlockColorSelector.ActionId)
                .RegisterBlockOptionProvider<BlockColorSelector>(BlockColorSelector.ActionId)
                .RegisterBlockActionHandler<ButtonAction, BlockDialogDemo>(DialogDemoBase.EchoDialog)
                .RegisterBlockActionHandler<ButtonAction, BlockDialogDemo>(DialogDemoBase.ErrorDialog)

                .RegisterInteractiveMessageHandler<LegacyCounter>(LegacyCounter.ActionName)
                .RegisterInteractiveMessageHandler<LegacyColorSelector>(LegacyColorSelector.ActionName)
                .RegisterOptionProvider<LegacyColorSelector>(LegacyColorSelector.ActionName)
                .RegisterInteractiveMessageHandler<LegacyDialogDemo>(DialogDemoBase.EchoDialog)
                .RegisterInteractiveMessageHandler<LegacyDialogDemo>(DialogDemoBase.ErrorDialog)

                .RegisterDialogSubmissionHandler<EchoDialogHandler>(DialogDemoBase.EchoDialog)
                .RegisterDialogSubmissionHandler<ErrorDialogHandler>(DialogDemoBase.ErrorDialog)

                .RegisterEventHandler<AppHomeOpened, AppHome>()
                .RegisterBlockActionHandler<ButtonAction, AppHome>()
                .RegisterViewSubmissionHandler<AppHome>(AppHome.ModalCallbackId)
            
                .RegisterSlashCommandHandler<EchoCommand>("/echo")
            );
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSlackNet(c => c.UseSigningSecret(Configuration["Slack:SigningSecret"]));
            
            app.UseMvc();
        }
    }
}
