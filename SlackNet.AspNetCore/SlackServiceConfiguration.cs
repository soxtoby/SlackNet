using Microsoft.Extensions.DependencyInjection;
using SlackNet.Events;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public class SlackServiceConfiguration
    {
        private readonly IServiceCollection _serviceCollection;

        public SlackServiceConfiguration(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public SlackServiceConfiguration UseApiToken(string token)
        {
            ApiToken = token;
            return this;
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : Event
            where THandler : class, IEventHandler<TEvent>
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<IEventHandler, ResolvedEventHandler<TEvent, THandler>>();
            return this;
        }

        public SlackServiceConfiguration RegisterActionHandler<THandler>(string actionName)
            where THandler : class, IActionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<ResolvedActionHandler>(c => new ResolvedActionHandler<THandler>(c, actionName));
            return this;
        }

        public SlackServiceConfiguration RegisterMessageActionHandler<THandler>(string callbackId)
            where THandler : class, IMessageActionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<ResolvedMessageActionHandler>(c => new ResolvedMessageActionHandler<THandler>(c, callbackId));
            return this;
        }

        public SlackServiceConfiguration RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider
        {
            _serviceCollection.AddTransient<TProvider>();
            _serviceCollection.AddSingleton<ResolvedOptionProvider>(c => new ResolvedOptionProvider<TProvider>(c, actionName));
            return this;
        }

        public SlackServiceConfiguration RegisterDialogSubmissionHandler<THandler>()
            where THandler : IDialogSubmissionHandler
        {
            _serviceCollection.AddSingleton<IDialogSubmissionHandler>(c => new ResolvedDialogSubmissionHandler<THandler>(c));
            return this;
        }

        public string ApiToken { get; private set; }
    }
}