using System;

namespace SlackNet
{
    public class SlackException : Exception
    {
        public string SlackError { get; }

        public SlackException(string slackError, string message = null) 
            : base(message ?? $"Slack returned an error response: {slackError}.")
        {
            SlackError = slackError;
        }
    }
}