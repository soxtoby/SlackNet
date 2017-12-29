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
                .RegisterActionHandler<Counter>(Counter.ActionName)
                .RegisterActionHandler<ColorSelector>(ColorSelector.ActionName)
                .RegisterOptionProvider<ColorSelector>(ColorSelector.ActionName));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSlackNet(c => c.VerifyWith("MpBGX2olqofbOsVusecbibr6"));

            app.UseMvc();
        }
    }
}
