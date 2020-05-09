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
        public SlackServiceConfiguration(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

        public SlackServiceConfiguration UseApiToken(string token)
        {
            ApiToken = token;
            return this;
        }

        #region Events

        /// <summary>
        /// Take over all event handling with your own handler.
        /// Handlers registered with <c>RegisterEventHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceEventHandling(Func<IServiceProvider, IEventHandler> handlerFactory) => 
            RegisterReplacementHandler<IEventHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterEventHandler<THandler>()
            where THandler : class, IEventHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterEventHandler(IEventHandler handler) => 
            RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, s => handler));

        public SlackServiceConfiguration RegisterEventHandler(Func<IServiceProvider, IEventHandler> handlerFactory) => 
            RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler(p, handlerFactory));

        public SlackServiceConfiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : Event
            where THandler : class, IEventHandler<TEvent>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : Event =>
            RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, s => handler));

        public SlackServiceConfiguration RegisterEventHandler<TEvent>(Func<IServiceProvider, IEventHandler<TEvent>> handlerFactory) 
            where TEvent : Event =>
            RegisterCompositeItem<IEventHandler>(p => new ResolvedEventHandler<TEvent>(p, handlerFactory));

        #endregion Events

        #region Block actions

        /// <summary>
        /// Take over all block action handling with your own handler.
        /// Handlers registered with <c>RegisterBlockActionHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceBlockActionHandling(Func<IServiceProvider, IBlockActionHandler> handlerFactory) => 
            RegisterReplacementHandler<IBlockActionHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterBlockActionHandler<THandler>()
            where THandler : class, IBlockActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler(IBlockActionHandler handler) => 
            RegisterCompositeItem<IBlockActionHandler>(p => handler);

        public SlackServiceConfiguration RegisterBlockActionHandler(Func<IServiceProvider, IBlockActionHandler> handlerFactory) => 
            RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler(p, handlerFactory));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => handler));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, handlerFactory));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, IBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => handler)));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, handlerFactory)));

        #endregion Block actions

        #region Message shortcuts

        /// <summary>
        /// Take over all message shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterMessageShortcutHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceMessageShortcutHandling(Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) => 
            RegisterReplacementHandler<IMessageShortcutHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>()
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(IMessageShortcutHandler handler) => 
            RegisterCompositeItem<IMessageShortcutHandler>(c => handler);

        public SlackServiceConfiguration RegisterMessageShortcutHandler(Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) => 
            RegisterCompositeItem<IMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, handlerFactory));

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler) => 
            RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, handler));

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) => 
            RegisterCompositeItem<IMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, handlerFactory)));

        #endregion Message shortcuts

        #region Global shortcuts

        /// <summary>
        /// Take over all global shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterGlobalShortcutHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceGlobalShortcutHandling(Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterReplacementHandler<IGlobalShortcutHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterGlobalShortcutHandler<THandler>()
            where THandler : class, IGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(IGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IGlobalShortcutHandler>(c => handler);

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, handlerFactory));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, s => s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(string callbackId, IGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, handler));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(string callbackId, Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, handlerFactory)));

        #endregion Global shortcuts

        #region Block option providers

        /// <summary>
        /// Take over all block option providing with your own handler.
        /// Providers registered with <c>RegisterBlockOptionProvider</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceBlockOptionProviding(Func<IServiceProvider, IBlockOptionProvider> providerFactory) => 
            RegisterReplacementHandler<IBlockOptionProvider>(providerFactory);

        public SlackServiceConfiguration RegisterBlockOptionProvider<TProvider>(string actionId)
            where TProvider : class, IBlockOptionProvider
        {
            _serviceCollection.TryAddScoped<TProvider>();
            return RegisterKeyedItem<IBlockOptionProvider>(actionId, p => new ResolvedBlockOptionProvider(p, s => s.GetRequiredService<TProvider>()));
        }

        public SlackServiceConfiguration RegisterBlockOptionProvider(string actionId, IBlockOptionProvider optionProvider) => 
            RegisterKeyedItem<IBlockOptionProvider>(actionId, p => new ResolvedBlockOptionProvider(p, s => optionProvider));

        public SlackServiceConfiguration RegisterBlockOptionProvider(string actionId, Func<IServiceProvider, IBlockOptionProvider> providerFactory) => 
            RegisterKeyedItem<IBlockOptionProvider>(actionId,  p => new ResolvedBlockOptionProvider(p, providerFactory));

        #endregion Block option providers

        #region View submission

        /// <summary>
        /// Take over all view submission handling with your own handler.
        /// Handlers registered with <c>RegisterViewSubmissionHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceViewSubmissionHandling(Func<IServiceProvider, IViewSubmissionHandler> handlerFactory) => 
            RegisterReplacementHandler<IViewSubmissionHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, IViewSubmissionHandler handler) => 
            RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => handler));

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, Func<IServiceProvider, IViewSubmissionHandler> handlerFactory) => 
            RegisterKeyedItem<IViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, handlerFactory));

        #endregion View submission

        #region Slash commands

        /// <summary>
        /// Take over all slash command handling with your own handler.
        /// Handlers registered with <c>RegisterSlashCommandHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceSlashCommandHandling(Func<IServiceProvider, ISlashCommandHandler> handlerFactory) => 
            RegisterReplacementHandler<ISlashCommandHandler>(handlerFactory);

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

        /// <summary>
        /// Take over all interactive message handling with your own handler.
        /// Handlers registered with <c>RegisterInteractiveMessageHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceLegacyInteractiveMessageHandling(Func<IServiceProvider, IInteractiveMessageHandler> handlerFactory) => 
            RegisterReplacementHandler<IInteractiveMessageHandler>(handlerFactory);

        public SlackServiceConfiguration RegisterInteractiveMessageHandler<THandler>(string actionName)
            where THandler : class, IInteractiveMessageHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IInteractiveMessageHandler>(actionName, p => new ResolvedInteractiveMessageHandler(p, s => s.GetRequiredService<THandler>()));
        }

        /// <summary>
        /// Take over all legacy option providing with your own provider.
        /// Providers registered with <c>RegisterOptionProvider</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceLegacyOptionProviding(Func<IServiceProvider, IOptionProvider> providerFactory) => 
            RegisterReplacementHandler<IOptionProvider>(providerFactory);

        public SlackServiceConfiguration RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider
        {
            _serviceCollection.TryAddScoped<TProvider>();
            return RegisterKeyedItem<IOptionProvider>(actionName, p => new ResolvedOptionProvider(p, s => s.GetRequiredService<TProvider>()));
        }

        /// <summary>
        /// Take over all legacy dialog submission handling with your own handler.
        /// Handlers registered with <c>RegisterDialogSubmissionHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceLegacyDialogSubmissionHandling(Func<IServiceProvider, IDialogSubmissionHandler> handlerFactory) => 
            RegisterReplacementHandler<IDialogSubmissionHandler>(handlerFactory);

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

        private SlackServiceConfiguration RegisterReplacementHandler<T>(Func<IServiceProvider, T> handlerFactory) 
            where T : class
        {
            _serviceCollection.AddSingleton<T>(handlerFactory);
            return this;
        }

        public string ApiToken { get; private set; }
    }
}