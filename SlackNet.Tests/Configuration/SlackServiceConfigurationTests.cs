using System;
using NUnit.Framework;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class SlackServiceConfigurationTests : SlackHandlerConfigurationBaseTests<SlackServiceFactory>
    {
        protected override ISlackServiceFactory Configure(Action<SlackServiceFactory> configure)
        {
            var config = new SlackServiceFactory();
            configure(config);
            return config;
        }
    }
}