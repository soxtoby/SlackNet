using System;

namespace SlackNet.AspNetCore;

public class SlackEndpointConfiguration
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
    public bool DelayedResponse { get; private set; }
}
