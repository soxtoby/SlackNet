using System.Runtime.Serialization;

namespace SlackNet.WebApi.Responses
{
    public enum DisabledReason
    {
        /// <summary>
        /// Manually disabled by user.
        /// </summary>
        User,
        /// <summary>
        /// Rate limits exceeded.
        /// </summary>
        RateLimits,
        /// <summary>
        /// Disabled by Slack.
        /// </summary>
        Slack,
        /// <summary>
        /// Too many errors.
        /// </summary>
        Errors,
        /// <summary>
        /// A system change (i.e. channel archived)
        /// </summary>
        System,
        /// <summary>
        /// Admin (i.e. user deleted).
        /// </summary>
        Admin,
        /// <summary>
        /// User declined the API TOS
        /// </summary>
        ApiDecline,
        /// <summary>
        /// Service deauthorized.
        /// </summary>
        [EnumMember(Value = "deauth")] DeAuth
    }
}