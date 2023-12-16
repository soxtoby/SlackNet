#nullable enable
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore;

public class AspNetSlackServiceConfiguration : AspNetSlackServiceConfiguration<AspNetSlackServiceConfiguration>
{
    internal AspNetSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection) { }
}

public abstract class AspNetSlackServiceConfiguration<TConfig> : ServiceCollectionSlackServiceConfiguration<TConfig>
    where TConfig : AspNetSlackServiceConfiguration<TConfig>
{
    // TODO: Move request validation properties into this class
    private readonly SlackEndpointConfiguration _endpointConfiguration = new();
    
    protected internal AspNetSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection)
    {
        UseLogger<MicrosoftLoggerAdaptor>();
    }

    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    protected internal new void ConfigureServices()
    {
        ServiceCollection.TryAddSingleton<SlackEndpointConfiguration>(_endpointConfiguration);
        ServiceCollection.TryAddSingleton<ISlackRequestValidationConfiguration>(_endpointConfiguration);
        ServiceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
        ServiceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ServiceCollection.TryAddSingleton<IRequestServiceProviderAccessor, HttpContextServiceProviderAccessor>();
        ServiceCollection.TryAddSingleton<IServiceProviderSlackRequestListener, AspNetCoreServiceProviderSlackRequestListener>();
        
        base.ConfigureServices();
    }
    /// <summary>
    /// Use a signing secret to verify that requests are coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    public TConfig UseSigningSecret(string signingSecret) => Chain(() => _endpointConfiguration.UseSigningSecret(signingSecret));

    /// <summary>
    /// Verify the signing secret or token of Slack's URL verification requests. Enabled by default.
    /// </summary>
    /// <remarks>
    /// You should only disable this temporarily and in certain circumstances (see <a href="https://github.com/soxtoby/SlackNet/pull/57">SlackNet pull request #57</a> for more information).
    /// </remarks>
    public TConfig UseEventUrlVerification(bool verifyEventUrl) => Chain(() => _endpointConfiguration.UseEventUrlVerification(verifyEventUrl));

    /// <summary>
    /// Use a token to verify that requests are actually coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    public TConfig VerifyWith(string verificationToken) => Chain(() => _endpointConfiguration.VerifyWith(verificationToken));
}