using System;
using NUnit.Framework;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class SlackServiceConfigurationTests : SlackHandlerConfigurationBaseTests<SlackServiceConfiguration>
    {
        protected override ISlackServiceFactory Configure(Action<SlackServiceConfiguration> configure)
        {
            var config = new SlackServiceConfiguration();
            configure(config);
            return config;
        }
    }
}