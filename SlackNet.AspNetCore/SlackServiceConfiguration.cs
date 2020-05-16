using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

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
        /// Handlers registered with <c>RegisterAsyncBlockActionHandler</c> will be ignored.
        /// </summary>
        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration ReplaceAsyncBlockActionHandling(Func<IServiceProvider, IAsyncBlockActionHandler> handlerFactory) => 
            RegisterReplacementHandler<IAsyncBlockActionHandler>(handlerFactory);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<THandler>()
            where THandler : class, IAsyncBlockActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler(IAsyncBlockActionHandler handler) => 
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => handler);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler(Func<IServiceProvider, IAsyncBlockActionHandler> handlerFactory) => 
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler(p, handlerFactory));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction>(IAsyncBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => handler));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction>(Func<IServiceProvider, IAsyncBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, handlerFactory));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => s.GetRequiredService<THandler>())));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction>(string actionId, IAsyncBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => handler)));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<IServiceProvider, IAsyncBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, handlerFactory)));

        /// <summary>
        /// Take over all block action handling with your own handler.
        /// Handlers registered with <c>RegisterBlockActionHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceBlockActionHandling(Func<IServiceProvider, IBlockActionHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncBlockActionHandler>(p => new BlockActionHandlerAsyncWrapper(handlerFactory(p)));

        public SlackServiceConfiguration RegisterBlockActionHandler<THandler>()
            where THandler : class, IBlockActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler(p, s => new BlockActionHandlerAsyncWrapper(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler(IBlockActionHandler handler) =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new BlockActionHandlerAsyncWrapper(handler));

        public SlackServiceConfiguration RegisterBlockActionHandler(Func<IServiceProvider, IBlockActionHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler(p, s => new BlockActionHandlerAsyncWrapper(handlerFactory(s))));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(handler)));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(handlerFactory(s))));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction>
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(s.GetRequiredService<THandler>()))));
        }

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, IBlockActionHandler<TAction> handler)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(handler))));

        public SlackServiceConfiguration RegisterBlockActionHandler<TAction>(string actionId, Func<IServiceProvider, IBlockActionHandler<TAction>> handlerFactory)
            where TAction : BlockAction =>
            RegisterCompositeItem<IAsyncBlockActionHandler>(p => new SpecificBlockActionHandler(actionId, new ResolvedBlockActionHandler<TAction>(p, s => new BlockActionHandlerAsyncWrapper<TAction>(handlerFactory(s)))));

        #endregion Block actions

        #region Message shortcuts

        /// <summary>
        /// Take over all message shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterMessageShortcutHandler</c> will be ignored.
        /// </summary>
        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration ReplaceAsyncMessageShortcutHandling(Func<IServiceProvider, IAsyncMessageShortcutHandler> handlerFactory) => 
            RegisterReplacementHandler<IAsyncMessageShortcutHandler>(handlerFactory);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler<THandler>()
            where THandler : class, IAsyncMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler(IAsyncMessageShortcutHandler handler) => 
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(c => handler);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler(Func<IServiceProvider, IAsyncMessageShortcutHandler> handlerFactory) => 
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, handlerFactory));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => s.GetRequiredService<THandler>())));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler(string callbackId, IAsyncMessageShortcutHandler handler) => 
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, handler));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncMessageShortcutHandler(string callbackId, Func<IServiceProvider, IAsyncMessageShortcutHandler> handlerFactory) => 
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, handlerFactory)));

        /// <summary>
        /// Take over all message shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterMessageShortcutHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceMessageShortcutHandling(Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncMessageShortcutHandler>(p => new MessageShortcutHandlerAsyncWrapper(handlerFactory(p)));

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>()
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(IMessageShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(c => new MessageShortcutHandlerAsyncWrapper(handler));

        public SlackServiceConfiguration RegisterMessageShortcutHandler(Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(handlerFactory(s))));

        public SlackServiceConfiguration RegisterMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IMessageShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(s.GetRequiredService<THandler>()))));
        }

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new MessageShortcutHandlerAsyncWrapper(handler)));

        public SlackServiceConfiguration RegisterMessageShortcutHandler(string callbackId, Func<IServiceProvider, IMessageShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(handlerFactory(s)))));

        [Obsolete("Use RegisterMessageShortcutHandler instead")]
        public SlackServiceConfiguration RegisterMessageActionHandler<THandler>()
            where THandler : class, IMessageActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(new MessageActionAdapter(s.GetRequiredService<THandler>()))));
        }

        [Obsolete("Use RegisterMessageShortcutHandler instead")]
        public SlackServiceConfiguration RegisterMessageActionHandler(IMessageActionHandler handler) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(c => new MessageShortcutHandlerAsyncWrapper(new MessageActionAdapter(handler)));

        [Obsolete("Use RegisterMessageShortcutHandler instead")]
        public SlackServiceConfiguration RegisterMessageActionHandler(Func<IServiceProvider, IMessageActionHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(new MessageActionAdapter(handlerFactory(s)))));

        [Obsolete("Use RegisterMessageShortcutHandler instead")]
        public SlackServiceConfiguration RegisterMessageActionHandler<THandler>(string callbackId)
            where THandler : class, IMessageActionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncMessageShortcutHandler>(p => new SpecificMessageShortcutHandler(callbackId, new ResolvedMessageShortcutHandler(p, s => new MessageShortcutHandlerAsyncWrapper(new MessageActionAdapter(s.GetRequiredService<THandler>())))));
        }

        #endregion Message shortcuts

        #region Global shortcuts

        /// <summary>
        /// Take over all global shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterGlobalShortcutHandler</c> will be ignored.
        /// </summary>
        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration ReplaceAsyncGlobalShortcutHandling(Func<IServiceProvider, IAsyncGlobalShortcutHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncGlobalShortcutHandler>(handlerFactory);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler<THandler>()
            where THandler : class, IAsyncGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(c => handler);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler(Func<IServiceProvider, IAsyncGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, handlerFactory));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, s => s.GetRequiredService<THandler>())));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler(string callbackId, IAsyncGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, handler));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncGlobalShortcutHandler(string callbackId, Func<IServiceProvider, IAsyncGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, handlerFactory)));
        
        /// <summary>
        /// Take over all global shortcut handling with your own handler.
        /// Handlers registered with <c>RegisterGlobalShortcutHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceGlobalShortcutHandling(Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncGlobalShortcutHandler>(p => new GlobalShortcutHandlerAsyncWrapper(handlerFactory(p)));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler<THandler>()
            where THandler : class, IGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, s => new GlobalShortcutHandlerAsyncWrapper(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(IGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(c => new GlobalShortcutHandlerAsyncWrapper(handler));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new ResolvedGlobalShortcutHandler(p, s => new GlobalShortcutHandlerAsyncWrapper(handlerFactory(s))));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IGlobalShortcutHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, s => new GlobalShortcutHandlerAsyncWrapper(s.GetRequiredService<THandler>()))));
        }

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(string callbackId, IGlobalShortcutHandler handler) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new GlobalShortcutHandlerAsyncWrapper(handler)));

        public SlackServiceConfiguration RegisterGlobalShortcutHandler(string callbackId, Func<IServiceProvider, IGlobalShortcutHandler> handlerFactory) =>
            RegisterCompositeItem<IAsyncGlobalShortcutHandler>(p => new SpecificGlobalShortcutHandler(callbackId, new ResolvedGlobalShortcutHandler(p, s => new GlobalShortcutHandlerAsyncWrapper(handlerFactory(s)))));

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
        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration ReplaceAsyncViewSubmissionHandling(Func<IServiceProvider, IAsyncViewSubmissionHandler> handlerFactory) => 
            RegisterReplacementHandler<IAsyncViewSubmissionHandler>(handlerFactory);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IAsyncViewSubmissionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler) =>
            RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => handler));

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncViewSubmissionHandler(string callbackId, Func<IServiceProvider, IAsyncViewSubmissionHandler> handlerFactory) =>
            RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, handlerFactory));

        /// <summary>
        /// Take over all view submission handling with your own handler.
        /// Handlers registered with <c>RegisterViewSubmissionHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceViewSubmissionHandling(Func<IServiceProvider, IViewSubmissionHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncViewSubmissionHandler>(s => new ViewSubmissionHandlerAsyncWrapper(handlerFactory(s)));

        public SlackServiceConfiguration RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler
        {
            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => new ViewSubmissionHandlerAsyncWrapper(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, IViewSubmissionHandler handler) => 
            RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => new ViewSubmissionHandlerAsyncWrapper(handler)));

        public SlackServiceConfiguration RegisterViewSubmissionHandler(string callbackId, Func<IServiceProvider, IViewSubmissionHandler> handlerFactory) => 
            RegisterKeyedItem<IAsyncViewSubmissionHandler>(callbackId, p => new ResolvedViewSubmissionHandler(p, s => new ViewSubmissionHandlerAsyncWrapper(handlerFactory(s))));

        #endregion View submission

        #region Slash commands

        /// <summary>
        /// Take over all slash command handling with your own handler.
        /// Handlers registered with <c>RegisterSlashCommandHandler</c> will be ignored.
        /// </summary>
        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration ReplaceAsyncSlashCommandHandling(Func<IServiceProvider, IAsyncSlashCommandHandler> handlerFactory) =>
            RegisterReplacementHandler<IAsyncSlashCommandHandler>(handlerFactory);

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncSlashCommandHandler<THandler>(string command)
            where THandler : class, IAsyncSlashCommandHandler
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => s.GetRequiredService<THandler>()));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncSlashCommandHandler(string command, IAsyncSlashCommandHandler handler)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => handler));
        }

        [Obsolete(Warning.Experimental)]
        public SlackServiceConfiguration RegisterAsyncSlashCommandHandler(string command, Func<IServiceProvider, IAsyncSlashCommandHandler> handlerFactory)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, handlerFactory));
        }

        /// <summary>
        /// Take over all slash command handling with your own handler.
        /// Handlers registered with <c>RegisterSlashCommandHandler</c> will be ignored.
        /// </summary>
        public SlackServiceConfiguration ReplaceSlashCommandHandling(Func<IServiceProvider, ISlashCommandHandler> handlerFactory) => 
            RegisterReplacementHandler<IAsyncSlashCommandHandler>(p => new SlashCommandHandlerAsyncWrapper(handlerFactory(p)));

        public SlackServiceConfiguration RegisterSlashCommandHandler<THandler>(string command)
            where THandler : class, ISlashCommandHandler
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            _serviceCollection.TryAddScoped<THandler>();
            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => new SlashCommandHandlerAsyncWrapper(s.GetRequiredService<THandler>())));
        }

        public SlackServiceConfiguration RegisterSlashCommandHandler(string command, ISlashCommandHandler handler)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => new SlashCommandHandlerAsyncWrapper(handler)));
        }

        public SlackServiceConfiguration RegisterSlashCommandHandler(string command, Func<IServiceProvider, ISlashCommandHandler> handlerFactory)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));

            return RegisterKeyedItem<IAsyncSlashCommandHandler>(command, p => new ResolvedSlashCommandHandler(p, s => new SlashCommandHandlerAsyncWrapper(handlerFactory(s))));
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