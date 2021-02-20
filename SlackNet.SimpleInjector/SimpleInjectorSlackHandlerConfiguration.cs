using System;
using System.Collections;
using System.Collections.Generic;
using SimpleInjector;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using Container = SimpleInjector.Container;

namespace SlackNet.SimpleInjector
{
    public class SimpleInjectorSlackHandlerConfiguration : FactorySlackHandlerConfigurationWithExternalDependencyResolver<SimpleInjectorSlackHandlerConfiguration>
    {
        private readonly Container _container;
        private readonly Dictionary<Type, IDictionary> _keyedHandlerProviders;

        private readonly Dictionary<string, Func<IBlockOptionProvider>> _blockOptionProviders = new();
        private readonly Dictionary<string, Func<IAsyncViewSubmissionHandler>> _viewSubmissionHandlers = new();
        private readonly Dictionary<string, Func<IAsyncSlashCommandHandler>> _slashCommandHandlers = new();

        private readonly Dictionary<string, Func<IInteractiveMessageHandler>> _legacyInteractiveMessageHandlers = new();
        private readonly Dictionary<string, Func<IOptionProvider>> _legacyOptionProviders = new();
        private readonly Dictionary<string, Func<IDialogSubmissionHandler>> _legacyDialogSubmissionHandlers = new();

        public SimpleInjectorSlackHandlerConfiguration(Container container)
        {
            _container = container;

            _keyedHandlerProviders = new Dictionary<Type, IDictionary>
                {
                    { typeof(IBlockOptionProvider), _blockOptionProviders },
                    { typeof(IAsyncViewSubmissionHandler), _viewSubmissionHandlers },
                    { typeof(IAsyncSlashCommandHandler), _slashCommandHandlers },
                    { typeof(IInteractiveMessageHandler), _legacyInteractiveMessageHandlers },
                    { typeof(IOptionProvider), _legacyOptionProviders },
                    { typeof(IDialogSubmissionHandler), _legacyDialogSubmissionHandlers }
                };
        }

        protected override void ReplaceClientService<TService>(Func<TService> serviceFactory) =>
            _container.RegisterSingleton(serviceFactory);

        protected override void ReplaceClientService<TService, TImplementation>() =>
            _container.RegisterSingleton<TService, TImplementation>();

        protected override void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<THandler> handlerFactory) =>
            _container.Register(() => handlerFactory(_container.GetAllInstances<THandler>()), Lifestyle.Scoped);

        protected override void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<THandler> handlerFactory) =>
            _container.Register(() => handlerFactory(_container.GetInstance<IHandlerIndex<THandler>>()), Lifestyle.Scoped);

        protected override void ReplaceCollectionHandling<THandler, TImplementation>() =>
            _container.Register<THandler, TImplementation>(Lifestyle.Scoped);

        protected override void ReplaceKeyedHandler<THandler, TImplementation>() =>
            _container.Register<THandler, TImplementation>(Lifestyle.Scoped);

        protected override void AddCollectionHandler<THandler>(THandler handler) =>
            _container.Collection.AppendInstance(handler);

        protected override void AddKeyedHandler<THandler>(string key, THandler handler) =>
            HandlerProviders<THandler>().Add(key, () => handler);

        protected override void AddCollectionHandler<TInnerHandler, TOuterHandler>(Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _container.RegisterFallbackType<TInnerHandler, TInnerHandler>(Lifestyle.Scoped);
            _container.Collection.Append(() => adaptor(_container.GetInstance<TInnerHandler>()), Lifestyle.Scoped);
        }

        protected override void AddKeyedHandler<TInnerHandler, TOuterHandler>(string key, Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _container.RegisterFallbackType<TInnerHandler, TInnerHandler>(Lifestyle.Scoped);
            HandlerProviders<TOuterHandler>().Add(key, () => adaptor(_container.GetInstance<TInnerHandler>()));
        }

        protected override void AddCollectionHandler<THandler>(Func<THandler> handlerFactory)
        {
            _container.Collection.Append(handlerFactory, Lifestyle.Scoped);
        }

        protected override void AddKeyedHandler<THandler>(string key, Func<THandler> handlerFactory)
        {
            var instanceProducer = Lifestyle.Scoped.CreateProducer(handlerFactory, _container);
            HandlerProviders<THandler>().Add(key, () => instanceProducer.GetInstance());
        }

        internal IHandlerIndex<THandler> HandlerIndex<THandler>() =>
            new SimpleInjectorHandlerIndex<THandler>(HandlerProviders<THandler>());

        private Dictionary<string, Func<THandler>> HandlerProviders<THandler>() => (Dictionary<string, Func<THandler>>)_keyedHandlerProviders[typeof(THandler)];
    }
}