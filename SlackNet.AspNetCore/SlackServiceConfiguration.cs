using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        #region Events

        public SlackServiceConfiguration RegisterEventHandler<THandler>()
            where THandler : class, IEventHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterEventHandler(IEventHandler handler)
        {
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, s => handler));
        }

        public SlackServiceConfiguration RegisterEventHandler(Func<IServiceProvider, IEventHandler> handlerFactory)
        {
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, handlerFactory));
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : Event
            where THandler : class, IEventHandler<TEvent>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : Event
        {
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, s => handler));
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(Func<IServiceProvider, IEventHandler<TEvent>> handlerFactory) 
            where TEvent : Event
        {
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, handlerFactory));
        }

        #endregion Events

        #region Block actions

        public SlackServiceConfiguration RegisterBlockActionHandler<THandler>()
            where THandler : class, IBlockActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler(IBlockActionHandler handler)
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => handler);
        }

        public SlackServiceConfiguration RegisterBlockActionHandler(Func<IServiceProvider, IBlockActionHandler> handlerFactory)
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler(p, handlerFactory));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler)
            where TAction : BlockAction
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => handler));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, handlerFactory));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, IBlockActionHandler<TAction> handler)
            where TAction : BlockAction
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => handler)));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction
        {
            return RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, handlerFactory)));
        }

        #endregion Block actions

        #region Message shortcuts

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>()
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(IMessageShortcutHandler handler)
        {
            return RegisterCompositeItem<IMessageShortcutHandler>(c => handler);
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(Func<IServiceProvider, IMessageShortcutHandler> handlerFactory)
        {
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, handlerFactory));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler)
        {
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, handler));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, Func<IServiceProvider, IMessageShortcutHandler> handlerFactory)
        {
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, handlerFactory)));
        }

        #endregion Message shortcuts

        #region Block option providers

        public SlackServiceConfiguration RegisterBlockOptionProvider<TProvider>(string actionId)
            where TProvider : class, IBlockOptionProvider
        {
            _serviceCollection.TryAddScoped<TProvider>();
            return RegisterKeyedItem<IBlockOptionProvider>(actionId, p => new ResolvedBlockOptionProvider(p, s => s.GetRequiredService<TProvider>()));
        }

        public SlackServiceConfiguration RegisterBlockOptionProvider(string actionId, IBlockOptionProvider optionProvider)
        {
            return RegisterKeyedItem<IBlockOptionProvider>(actionId, p => new ResolvedBlockOptionProvider(p, s => optionProvider));
        }

        public SlackServiceConfiguration RegisterBlockOptionProvider(string actionId, Func<IServiceProvider, IBlockOptionProvider> providerFactory)
        {
            return RegisterKeyedItem<IBlockOptionProvider>(actionId,  p => new ResolvedBlockOptionProvider(p, providerFactory));
        }

        #endregion Block option providers

        #region View submission

        public SlackServiceConfiguration RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, IViewSubmissionHandler handler)
        {
            return RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => handler));
        }

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, Func<IServiceProvider, IViewSubmissionHandler> handlerFactory)
        {
            return RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, handlerFactory));
        }

        #endregion View submission

        #region Slash commands

        public SlackServiceConfiguration RegisterSlashCommandHandler<THandler>(string command)
            where THandler : class, ISlashCommandHandler
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<ISlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterSlashCommandHandler(string command, ISlashCommandHandler handler)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<ISlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => handler));
        }

        public SlackServiceConfiguration RegisterSlashCommandHandler(string command, Func<IServiceProvider, ISlashCommandHandler> handlerFactory)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<ISlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, handlerFactory));
        }

        #endregion Slash commands

        public SlackServiceConfiguration RegisterInteractiveMessageHandler<THandler>(string actionName)
            where THandler : class, IInteractiveMessageHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IInteractiveMessageHandler>(actionName, p => new ResolvedInteractiveMessageHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider
        {
            _serviceCollection.TryAddScoped<TProvider>();
            return RegisterKeyedItem<IOptionProvider>(actionName, p => new ResolvedOptionProvider(p, s => s.GetRequiredService<TProvider>()));
        }

        public SlackServiceConfiguration RegisterDialogSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IDialogSubmissionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IDialogSubmissionHandler>(callbackId, p => new ResolvedDialogSubmissionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        private SlackServiceConfiguration RegisterCompositeItem<T>(Func<IServiceProvider, T> handlerFactory)
        {
            _serviceCollection.AddSingleton<CompositeItem<T>>(p => new CompositeItem<T>(handlerFactory(p)));
            return this;
        }

        private SlackServiceConfiguration RegisterKeyedItem<T>(string key, Func<IServiceProvider, T> handlerFactory)
        {
            _serviceCollection.AddSingleton<KeyedItem<T>>(p => new KeyedItem<T>(handlerFactory(p), key));
            return this;
        }

        public string ApiToken { get; private set; }
    }
}