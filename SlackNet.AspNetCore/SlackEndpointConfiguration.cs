namespace SlackNet.AspNetCore
{
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
        /// Use a token to verify that requests are actually coming from Slack.
        /// You'll find this value in the "App Credentials" section of your app's application management interface.
        /// </summary>
        public SlackEndpointConfiguration VerifyWith(string verificationToken)
        {
            VerificationToken = verificationToken;
            return this;
        }

        /// <summary>
        /// Use a signing secret to verify that requests are coming from Slack.
        /// You'll find this value in the "App Credentials" section of your app's application management interface.
        /// </summary>
        public SlackEndpointConfiguration UseSigningSecret(string signingSecret)
        {
            SigningSecret = signingSecret;
            return this;
        }
        
        public SlackEndpointConfiguration UseEventUrlVerification(bool verifyEventUrl)
        {
            VerifyEventUrl = verifyEventUrl;
            return this;
        }

        public string RoutePrefix { get; private set; } = "slack";
        public string VerificationToken { get; private set; }
        public string SigningSecret { get; private set; }
        public bool VerifyEventUrl { get; private set; } = true;
    }
}