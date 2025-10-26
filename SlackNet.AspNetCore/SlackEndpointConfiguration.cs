﻿using System;
using SlackNet.SocketMode;

namespace SlackNet.AspNetCore;

public class SlackEndpointConfiguration : ISlackRequestValidationConfiguration
{
    /// <summary>
    /// Sets the path to receive Slack requests on. Defaults to "slack".
    /// Configures the following routes:
    /// <br /><c>/{RoutePrefix}/event</c> - Event subscriptions
    /// <br /><c>/{RoutePrefix}/action</c> - Interactive component requests
    /// <br /><c>/{RoutePrefix}/options</c> - Options loading (for message menus)
    /// <br /><c>/{RoutePrefix}/command</c> - Slash command requests
    /// </summary>
    public SlackEndpointConfiguration MapToPrefix(string routePrefix)
    {
        RoutePrefix = routePrefix;
        return this;
    }

    /// <summary>
    /// Use a token to verify that requests are actually coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    [Obsolete("Configure this in AddSlackNet instead.")]
    public SlackEndpointConfiguration VerifyWith(string verificationToken)
    {
        VerificationToken = verificationToken;
        return this;
    }

    /// <summary>
    /// Use a signing secret to verify that requests are coming from Slack.
    /// You'll find this value in the "App Credentials" section of your app's application management interface.
    /// </summary>
    [Obsolete("Configure this in AddSlackNet instead.")]
    public SlackEndpointConfiguration UseSigningSecret(string signingSecret)
    {
        SigningSecret = signingSecret;
        return this;
    }

    /// <summary>
    /// Verify the signing secret or token of Slack's URL verification requests. Enabled by default.
    /// </summary>
    /// <remarks>
    /// You should only disable this temporarily and in certain circumstances (see <a href="https://github.com/soxtoby/SlackNet/pull/57">SlackNet pull request #57</a> for more information).
    /// </remarks>
    [Obsolete("Configure this in AddSlackNet instead.")]
    public SlackEndpointConfiguration UseEventUrlVerification(bool verifyEventUrl)
    {
        VerifyEventUrl = verifyEventUrl;
        return this;
    }

    /// <summary>
    /// Use a <a href="https://docs.slack.dev/apis/events-api/using-socket-mode/">socket mode</a> client instead of ASP.NET middleware.
    /// Use this for testing your app without needing to host it publicly.
    /// </summary>
    public SlackEndpointConfiguration UseSocketMode(SocketModeConnectionOptions connectionOptions = null) => UseSocketMode(true, connectionOptions);

    /// <summary>
    /// Use a <a href="https://docs.slack.dev/apis/events-api/using-socket-mode/">socket mode</a> client instead of ASP.NET middleware.
    /// Use this for testing your app without needing to host it publicly.
    /// </summary>
    public SlackEndpointConfiguration UseSocketMode(bool useSocketMode = true, SocketModeConnectionOptions connectionOptions = null)
    {
        SocketMode = useSocketMode;
        SocketModeConnectionOptions = connectionOptions;
        return this;
    }

    /// <summary>
    /// Delay responding to Slack until after handlers have completed.
    /// May be required in environments that pause or shut down after a request has been completed.
    /// </summary>
    [Obsolete(Warning.Experimental)]
    public SlackEndpointConfiguration DelayResponse(bool delay = true)
    {
        DelayedResponse = delay;
        return this;
    }

    public string RoutePrefix { get; private set; } = "slack";
    public string VerificationToken { get; private set; }
    public string SigningSecret { get; private set; }
    public bool VerifyEventUrl { get; private set; } = true;
    public bool SocketMode { get; private set; }
    public bool DelayedResponse { get; private set; }
    public SocketModeConnectionOptions SocketModeConnectionOptions { get; [Obsolete("Pass options to UseSocketMode instead.")] set; }
}