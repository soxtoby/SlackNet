using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Describes a Slack Web API call so a token provider can select the appropriate credentials.
/// </summary>
/// <param name="RequestUrl">The URL the request will be sent to.</param>
/// <param name="ApiMethod">The Slack Web API method being called.</param>
/// <param name="Arguments">The arguments supplied to the method.</param>
public record SlackApiCallContext(string RequestUrl, string ApiMethod, IReadOnlyDictionary<string, object> Arguments)
{
    /// <summary>
    /// The incoming Slack request that caused this API call, or <see langword="null"/> when the call was made independently.
    /// </summary>
    public SlackRequestContext OriginatingRequest { get; init; }
}
