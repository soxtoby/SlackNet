using System;
using EasyAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    public abstract class SlackServiceConfigurationBaseTests<TConfig> where TConfig : SlackServiceConfigurationBase<TConfig>
    {
        [Test]
        public void DefaultServices()
        {
            var sut = Configure(_ => { });

            ShouldBeSingleInstance(sut, f => f.GetHttp(), sut.GetHttp());
            ShouldBeSingleInstance(sut, f => f.GetJsonSettings(), sut.GetJsonSettings());
            ShouldBeSingleInstance(sut, f => f.GetTypeResolver(), sut.GetTypeResolver());
            ShouldBeSingleInstance(sut, f => f.GetUrlBuilder(), sut.GetUrlBuilder());
            ShouldBeSingleInstance(sut, f => f.GetWebSocketFactory(), sut.GetWebSocketFactory());
            ShouldBeSingleInstance(sut, f => f.GetRequestListener(), sut.GetRequestListener());
            ShouldBeSingleInstance(sut, f => f.GetHandlerFactory(), sut.GetHandlerFactory());
            ShouldBeSingleInstance(sut, f => f.GetApiClient(), sut.GetApiClient());
            ShouldBeSingleInstance(sut, f => f.GetSocketModeClient(), sut.GetSocketModeClient());
        }

        [Test]
        public void UseHttp()
        {
            UseService(
                Substitute.For<IHttp>(),
                (c, f) => c.UseHttp(f),
                s => s.GetHttp());
        }

        [Test]
        public void UseJsonSettings()
        {
            UseService(
                new SlackJsonSettings(new JsonSerializerSettings()),
                (c, f) => c.UseJsonSettings(f),
                s => s.GetJsonSettings());
        }

        [Test]
        public void UseTypeResolver()
        {
            UseService(
                Substitute.For<ISlackTypeResolver>(),
                (c, f) => c.UseTypeResolver(f),
                s => s.GetTypeResolver());
        }

        [Test]
        public void UseUrlBuilder()
        {
            UseService(
                Substitute.For<ISlackUrlBuilder>(),
                (c, f) => c.UseUrlBuilder(f),
                s => s.GetUrlBuilder());
        }

        [Test]
        public void UseWebSocketFactory()
        {
            UseService(
                Substitute.For<IWebSocketFactory>(),
                (c, f) => c.UseWebSocketFactory(f),
                s => s.GetWebSocketFactory());
        }

        [Test]
        public void UseRequestListener()
        {
            // Using a real request listener from a separate config to test request behaviour
            UseService(
                DefaultServiceFactory().GetRequestListener(),
                (c, f) => c.UseRequestListener(f),
                s => s.GetRequestListener());
        }

        [Test]
        public void UseHandlerFactory()
        {
            UseService(
                Substitute.For<ISlackHandlerFactory>(),
                (c, f) => c.UseHandlerFactory(f),
                s => s.GetHandlerFactory());
        }

        [Test]
        public void UseApiClient()
        {
            UseService(
                Substitute.For<ISlackApiClient>(),
                (c, f) => c.UseApiClient(f),
                s => s.GetApiClient());
        }

        [Test]
        public void UseSocketModeClient()
        {
            UseService(
                Substitute.For<ISlackSocketModeClient>(),
                (c, f) => c.UseSocketModeClient(f),
                s => s.GetSocketModeClient());
        }

        private void UseService<TService>(TService service, Action<TConfig, Func<TService>> registerFactory, Func<ISlackServiceFactory, TService> getService) where TService : class
        {
            var serviceFactory = Substitute.For<Func<TService>>();
            serviceFactory().Returns(service);

            var sut = Configure(c => registerFactory(c, serviceFactory));

            ShouldBeSingleInstance(sut, getService, service);

            serviceFactory.Received(1)(); // Service should only be created once
        }

        private static void ShouldBeSingleInstance<TService>(ISlackServiceFactory sut, Func<ISlackServiceFactory, TService> getService, TService service) where TService : class
        {
            getService(sut).ShouldBe(service);
            getService(sut).ShouldBe(service, "Should be same instance");

            DuringRequest(sut, _ => getService(sut).ShouldBe(service, "Should be same instance during request"));
        }

        protected static void DuringRequest(ISlackServiceFactory services, Action<SlackRequestContext> duringRequest)
        {
            var requestListener = services.GetRequestListener();
            var requestContext = new SlackRequestContext();
            requestListener.OnRequestBegin(requestContext);

            duringRequest(requestContext);

            requestListener.OnRequestEnd(requestContext);
        }

        protected virtual ISlackServiceFactory DefaultServiceFactory() => Configure(_ => { });

        protected abstract ISlackServiceFactory Configure(Action<TConfig> configure);
    }
}
