namespace SlackNet.AspNetCore
{
    public class SlackEndpointConfiguration
    {
        public SlackEndpointConfiguration MapToPrefix(string routePrefix)
        {
            RoutePrefix = routePrefix;
            return this;
        }

        public SlackEndpointConfiguration VerifyWith(string verificationToken)
        {
            VerificationToken = verificationToken;
            return this;
        }

        public SlackEndpointConfiguration UseSigningSecret(string signingSecret)
        {
            SigningSecret = signingSecret;
            return this;
        }

        /// <summary>
        /// Path to receive Slack requests on. Defaults to "slack".
        /// Configures the following routes:
        /// <br /><c>/{RoutePrefix}/event</c> - Event subscriptions
        /// <br /><c>/{RoutePrefix}/action</c> - Interactive components requests
        /// <br /><c>/{RoutePrefix}/options</c> - Options loading (for message menus)
        /// </summary>
        public string RoutePrefix { get; private set; } = "slack";

        /// <summary>
        /// Use this token to verify that requests are actually coming from Slack.
        /// You'll find this value in the "App Credentials" section of your app's application management interface.
        /// </summary>
        public string VerificationToken { get; private set; }
        
        /// <summary>
        /// Use this signing secret to verify that requests are coming from Slack.
        /// You'll find this value in the "App Credentials" section of your app's application management interface.
        /// </summary>
        public string SigningSecret { get; private set; }
    }
}