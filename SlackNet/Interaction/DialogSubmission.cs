using System.Collections.Generic;

namespace SlackNet.Interaction
{
    public class DialogSubmission : InteractionRequest
    {
        public IDictionary<string, string> Submission { get; set; } = new Dictionary<string, string>();
        public string State { get; set; }
    }
}