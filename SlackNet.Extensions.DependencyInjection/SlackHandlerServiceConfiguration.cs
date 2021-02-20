using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    public class SlackHandlerServiceConfiguration : FactorySlackHandlerConfigurationWithDependencyResolver<SlackHandlerServiceConfiguration, IServiceProvider>
    {
        private readonly IServiceCollection _serviceCollection;
        public SlackHandlerServiceConfiguration(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

        protected override void ReplaceClientService<TService>(Func<TService> serviceFactory) =>
            _serviceCollection.AddSingleton(sp => serviceFactory());

        protected override void ReplaceClientService<TService>(Func<IServiceProvider, TService> serviceFactory) =>
            _serviceCollection.AddSingleton(serviceFactory);

        protected override void ReplaceClientService<TService, TImplementation>() =>
            _serviceCollection.AddSingleton<TService, TImplementation>();

        protected override void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<THandler> handlerFactory) =>
            _serviceCollection.AddScoped(sp => handlerFactory(sp.GetRequiredService<IEnumerable<CollectionItem<THandler>>>().Select(h => h.Item)));

        protected override void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<THandler> handlerFactory) =>
            _serviceCollection.AddScoped(sp => handlerFactory(new KeyedHandlerIndex<THandler>(sp.GetRequiredService<IEnumerable<KeyedItem<THandler>>>())));

        protected override void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<IServiceProvider, THandler> handlerFactory) =>
            _serviceCollection.AddScoped(sp => handlerFactory(sp, sp.GetRequiredService<IEnumerable<CollectionItem<THandler>>>().Select(h => h.Item)));

        protected override void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<IServiceProvider, THandler> handlerFactory) =>
            _serviceCollection.AddScoped(sp => handlerFactory(sp, new KeyedHandlerIndex<THandler>(sp.GetRequiredService<IEnumerable<KeyedItem<THandler>>>())));

        protected override void ReplaceCollectionHandling<THandler, TImplementation>() =>
            _serviceCollection.AddScoped<THandler, TImplementation>();

        protected override void ReplaceKeyedHandler<THandler, TImplementation>() =>
            _serviceCollection.AddScoped<THandler, TImplementation>();

        protected override void AddCollectionHandler<THandler>(THandler handler) =>
            _serviceCollection.AddSingleton(new CollectionItem<THandler>(handler));

        protected override void AddKeyedHandler<THandler>(string key, THandler handler) =>
            _serviceCollection.AddSingleton(new KeyedItem<THandler>(handler, key));

        protected override void AddCollectionHandler<TInnerHandler, TOuterHandler>(Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _serviceCollection.AddScoped<TInnerHandler>();
            _serviceCollection.AddScoped(sp => new CollectionItem<TOuterHandler>(adaptor(sp.GetRequiredService<TInnerHandler>())));
        }

        protected override void AddKeyedHandler<TInnerHandler, TOuterHandler>(string key, Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _serviceCollection.AddScoped<TInnerHandler>();
            _serviceCollection.AddScoped(sp => new KeyedItem<TOuterHandler>(adaptor(sp.GetRequiredService<TInnerHandler>()), key));
        }

        protected override void AddCollectionHandler<THandler>(Func<IServiceProvider, THandler> handlerFactory)
        {
            _serviceCollection.AddScoped(sp => new CollectionItem<THandler>(handlerFactory(sp)));
        }

        protected override void AddKeyedHandler<THandler>(string key, Func<IServiceProvider, THandler> handlerFactory)
        {
            _serviceCollection.AddScoped(sp => new KeyedItem<THandler>(handlerFactory(sp), key));
        }
    }
}