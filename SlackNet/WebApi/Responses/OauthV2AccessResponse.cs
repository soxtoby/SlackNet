namespace SlackNet.WebApi
{
    public class OauthV2AccessResponse : OauthAccessResponse
    {
        public string AppId { get; set; }

        public string TokenType { get; set; }

        public string BotUserId { get; set; }

        public bool IsEnterpriseInstall { get; set; }

#nullable enable
        /// <summary>
        /// Only available for OAuth flows with Token Rotation enabled. See more: https://api.slack.com/authentication/rotation
        /// </summary>
        public string? RefreshToken { get; set; }
        /// <summary>
        /// Only available for OAuth flows with Token Rotation enabled. See more: https://api.slack.com/authentication/rotation
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
    }
}
