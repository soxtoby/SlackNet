using System;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Blocks;
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
            return RegisterEventHandler(c => new ResolvedEventHandler<TEvent, THandler>(c));
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : Event
        {
            return RegisterEventHandler(c => handler);
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(Func<IServiceProvider, IEventHandler<TEvent>> handlerFactory) 
            where TEvent : Event
        {
            _serviceCollection.AddSingleton<IEventHandler>(handlerFactory);
            return this;
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.AddTransient<THandler>();
            return RegisterBlockActionHandler(c => new ResolvedBlockActionHandler<TAction, THandler>(c));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.AddTransient<THandler>();
            return RegisterBlockActionHandler(c => new SpecificBlockActionHandler<TAction>(actionId, new ResolvedBlockActionHandler<TAction, THandler>(c)));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler) 
            where TAction : BlockAction
        {
            return RegisterBlockActionHandler(c => handler);
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory) 
            where TAction : BlockAction
        {
            _serviceCollection.AddSingleton<IBlockActionHandler>(handlerFactory);
            return this;
        }

        public SlackServiceConfiguration RegisterInteractiveMessageHandler<THandler>(string actionName)
            where THandler : class, IInteractiveMessageHandler
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<ResolvedInteractiveMessageHandler>(c => new ResolvedInteractiveMessageHandler<THandler>(c, actionName));
            return this;
        }

        public SlackServiceConfiguration RegisterMessageActionHandler<THandler>()
            where THandler : class, IMessageActionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            return RegisterMessageActionHandler(c => new ResolvedMessageActionHandler<THandler>(c));
        }

        public SlackServiceConfiguration RegisterMessageActionHandler<THandler>(string callbackId)
            where THandler : class, IMessageActionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            return RegisterMessageActionHandler(c => new SpecificMessageActionHandler(callbackId, new ResolvedMessageActionHandler<THandler>(c)));
        }

        public SlackServiceConfiguration RegisterMessageActionHandler(IMessageActionHandler handler)
        {
            return RegisterMessageActionHandler(c => handler);
        }

        public SlackServiceConfiguration RegisterMessageActionHandler(Func<IServiceProvider, IMessageActionHandler> handlerFactory)
        {
            _serviceCollection.AddSingleton(handlerFactory);
            return this;
        }

        public SlackServiceConfiguration RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider
        {
            _serviceCollection.AddTransient<TProvider>();
            _serviceCollection.AddSingleton<ResolvedOptionProvider>(c => new ResolvedOptionProvider<TProvider>(c, actionName));
            return this;
        }

        public SlackServiceConfiguration RegisterBlockOptionProvider<TProvider>(string actionId)
            where TProvider : class, IBlockOptionProvider
        {
            _serviceCollection.AddTransient<TProvider>();
            _serviceCollection.AddSingleton<ResolvedBlockOptionProvider>(c => new ResolvedBlockOptionProvider<TProvider>(c, actionId));
            return this;
        }

        public SlackServiceConfiguration RegisterDialogSubmissionHandler<THandler>()
            where THandler : class, IDialogSubmissionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<IDialogSubmissionHandler>(c => new ResolvedDialogSubmissionHandler<THandler>(c));
            return this;
        }

        public SlackServiceConfiguration RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler
        {
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<ResolvedViewSubmissionHandler>(c => new ResolvedViewSubmissionHandler<THandler>(c, callbackId));
            return this;
        }

        public SlackServiceConfiguration RegisterSlashCommandHandler<THandler>(string command)
            where THandler : class, ISlashCommandHandler
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));
            _serviceCollection.AddTransient<THandler>();
            _serviceCollection.AddSingleton<ResolvedSlashCommandHandler>(c => new ResolvedSlashCommandHandler<THandler>(c, command));
            return this;
        }

        public string ApiToken { get; private set; }
    }
}