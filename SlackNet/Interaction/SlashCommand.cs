namespace SlackNet.Interaction
{
    public class SlashCommand
    {
        /// <summary>
        /// This is a verification token, a deprecated feature that you shouldn't use any more.
        /// It was used to verify that requests were legitimately being sent by Slack to your app,
        /// but you should use the signed secrets functionality to do this instead.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The command that was typed in to trigger this request.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// This is the part of the Slash Command after the command itself,
        /// and it can contain absolutely anything that the user might decide to type.
        /// It is common to use this text parameter to provide extra context for the command. 
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// A temporary webhook URL that you can use to generate messages responses.
        /// </summary>
        public string ResponseUrl { get; set; }

        /// <summary>
        /// A short-lived ID that will let your app open a modal.
        /// </summary>
        public string TriggerId { get; set; }

        /// <summary>
        /// The ID of the user who triggered the command.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The plain text name of the user who triggered the command.
        /// Do not rely on this field as it is being phased out, use the <see cref="UserId"/> instead.
        /// </summary>
        public string UserName { get; set; }

        public string TeamId { get; set; }
        public string TeamName { get; set; }

        public string EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
    }
}