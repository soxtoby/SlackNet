#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Extensions.DependencyInjection;
using SlackNet.SocketMode;

namespace SlackNet.AspNetCore;

public class AspNetSlackServiceConfiguration : AspNetSlackServiceConfiguration<AspNetSlackServiceConfiguration>
{
    internal AspNetSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection) { }
}

public abstract class AspNetSlackServiceConfiguration<TConfig> : ServiceCollectionSlackServiceConfiguration<TConfig>
    where TConfig : AspNetSlackServiceConfiguration<TConfig>
{
    private readonly SlackEndpointConfiguration _endpointConfiguration = new();
    private readonly SlackRequestValidationConfiguration _validationConfiguration = new();
    private readonly SlackSocketModeConfiguration _socketModeConfiguration = new();

    protected internal AspNetSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection)
    {
        UseLogger<MicrosoftLoggerAdaptor>();
    }

    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    protected internal new void ConfigureServices()
    {
        ServiceCollection.TryAddSingleton(_endpointConfiguration);
        ServiceCollection.TryAddSingleton(_validationConfiguration);
        ServiceCollection.TryAddSingleton(_socketModeConfiguration);
        ServiceCollection.TryAddSingleton<ISlackSigningSecretProvider, UnconfiguredSigningSecretProvider>();
        ServiceCollection.TryAddSingleton<ISlackVerificationTokenProvider, UnconfiguredVerificationTokenProvider>();
        ServiceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
        ServiceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        ServiceCollection.TryAddSingleton<IRequestServiceProviderAccessor, HttpContextServiceProviderAccessor>();
        ServiceCollection.TryAddSingleton<IServiceProviderSlackRequestListener, AspNetCoreServiceProviderSlackRequestListener>();
        
        if (_socketModeConfiguration.SocketMode)
            ServiceCollection.AddHostedService<SocketModeService>();

        base.ConfigureServices();
    }

    /// <summary>
    /// Use a signing secret to verify that requests are coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    public TConfig UseSigningSecret(string signingSecret) =>
        Chain(() => ServiceCollection.AddSingleton<ISlackSigningSecretProvider>(new ConstantSigningSecretProvider(signingSecret)));

    /// <summary>
    /// Uses the specified provider to choose the signing secret for each incoming Slack request.
    /// </summary>
    public TConfig UseSigningSecretProvider(ISlackSigningSecretProvider signingSecretProvider) =>
        Chain(() => ServiceCollection.AddSingleton(signingSecretProvider));

    /// <summary>
    /// Uses a service resolved from the ASP.NET Core container to choose the signing secret for each incoming Slack request.
    /// </summary>
    public TConfig UseSigningSecretProvider<TProvider>() where TProvider : class, ISlackSigningSecretProvider
    {
        ServiceCollection.TryAddSingleton<TProvider>();
        return UseSigningSecretProvider(sp => sp.GetRequiredService<TProvider>());
    }

    /// <summary>
    /// Uses a service factory to choose the signing-secret provider for incoming Slack requests.
    /// </summary>
    public TConfig UseSigningSecretProvider(Func<IServiceProvider, ISlackSigningSecretProvider> signingSecretProvider) =>
        Chain(() => ServiceCollection.AddSingleton(signingSecretProvider));

    /// <summary>
    /// Verify the signing secret or token of Slack's URL verification requests. Enabled by default.
    /// </summary>
    /// <remarks>
    /// You should only disable this temporarily and in certain circumstances (see <a href="https://github.com/soxtoby/SlackNet/pull/57">SlackNet pull request #57</a> for more information).
    /// </remarks>
    public TConfig UseEventUrlVerification(bool verifyEventUrl) =>
        Chain(() => _validationConfiguration.VerifyEventUrl = verifyEventUrl);

    /// <summary>
    /// Use a token to verify that requests are actually coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    public TConfig VerifyWith(string verificationToken) =>
        Chain(() => ServiceCollection.AddSingleton<ISlackVerificationTokenProvider>(new ConstantVerificationTokenProvider(verificationToken)));

    /// <summary>
    /// Uses the specified provider to choose the legacy verification token for each incoming Slack request.
    /// </summary>
    public TConfig UseVerificationTokenProvider(ISlackVerificationTokenProvider verificationTokenProvider) =>
        Chain(() => ServiceCollection.AddSingleton(verificationTokenProvider));

    /// <summary>
    /// Uses a service resolved from the ASP.NET Core container to choose the legacy verification token for each incoming Slack request.
    /// </summary>
    public TConfig UseVerificationTokenProvider<TProvider>() where TProvider : class, ISlackVerificationTokenProvider
    {
        ServiceCollection.TryAddSingleton<TProvider>();
        return UseVerificationTokenProvider(sp => sp.GetRequiredService<TProvider>());
    }

    /// <summary>
    /// Uses a service factory to choose the verification-token provider for incoming Slack requests.
    /// </summary>
    public TConfig UseVerificationTokenProvider(Func<IServiceProvider, ISlackVerificationTokenProvider> verificationTokenProvider) =>
        Chain(() => ServiceCollection.AddSingleton(verificationTokenProvider));

    /// <summary>
    /// Use a <a href="https://docs.slack.dev/apis/events-api/using-socket-mode/">socket mode</a> client instead of ASP.NET middleware.
    /// Use this for testing your app without needing to host it publicly.
    /// </summary>
    public TConfig UseSocketMode(SocketModeConnectionOptions? connectionOptions = null) =>
        UseSocketMode(true, connectionOptions);

    /// <summary>
    /// Use a <a href="https://docs.slack.dev/apis/events-api/using-socket-mode/">socket mode</a> client instead of ASP.NET middleware.
    /// Use this for testing your app without needing to host it publicly.
    /// </summary>
    public TConfig UseSocketMode(bool useSocketMode = true, SocketModeConnectionOptions? connectionOptions = null) =>
        Chain(() =>
            {
                _socketModeConfiguration.SocketMode = useSocketMode;
                _socketModeConfiguration.ConnectionOptions = connectionOptions;
            });
}

class ConstantVerificationTokenProvider(string verificationToken) : ISlackVerificationTokenProvider
{
    public Task<string> GetVerificationToken(SlackRequestContext context, CancellationToken cancellationToken) =>
        Task.FromResult(verificationToken);
}

class ConstantSigningSecretProvider(string signingSecret) : ISlackSigningSecretProvider
{
    public Task<string> GetSigningSecret(SlackRequestContext context, CancellationToken cancellationToken) =>
        Task.FromResult(signingSecret);
}
