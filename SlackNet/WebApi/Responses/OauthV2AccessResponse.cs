namespace SlackNet.WebApi;

public class OauthV2AccessResponse : OauthAccessResponse
{
    public string AppId { get; set; }

    public string TokenType { get; set; }

    public string BotUserId { get; set; }

    public bool IsEnterpriseInstall { get; set; }

#nullable enable
    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See the <a href="https://docs.slack.dev/authentication/using-token-rotation/">Slack documentation</a> for more information.
    /// </summary>
    public string? RefreshToken { get; set; }
    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See the <a href="https://docs.slack.dev/authentication/using-token-rotation/">Slack documentation</a> for more information.
    /// </summary>
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// Only available for with the identity.team scope.
    /// </summary>
    public WorkSpace? Team { get; set; }
    /// <summary>
    /// Only available for enterprise price tier.
    /// </summary>
    public WorkSpace? Enterprise { get; set; }
#nullable disable

    public AuthedUser AuthedUser { get; set; }
    
    public IncomingWebhook IncomingWebhook { get; set; }
}

public class IncomingWebhook
{
    /// <summary>
    /// The name of the channel that the user selected as a destination for webhook messages.
    /// </summary>
    public string Channel { get; set; }
    
    /// <summary>
    /// The ID of the same channel.
    /// </summary>
    public string ChannelId { get; set; }
    
    /// <summary>
    /// A link to the page that configures your app within the workspace it was installed to.
    /// </summary>
    public string ConfigurationUrl { get; set; }
    
    /// <summary>
    /// The Incoming Webhook URL.
    /// </summary>
    public string Url { get; set; }
}