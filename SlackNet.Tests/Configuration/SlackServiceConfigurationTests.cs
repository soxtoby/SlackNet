using System;
using NUnit.Framework;

namespace SlackNet.Tests.Configuration;

[TestFixture]
public class SlackServiceConfigurationTests : SlackServiceConfigurationBaseTests<SlackServiceBuilder>
{
    protected override ISlackServiceProvider Configure(Action<SlackServiceBuilder> configure)
    {
        var config = new SlackServiceBuilder();
        configure(config);
        return config;
    }
}