using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;



namespace SlackNet.WebApi
{
    public class DeleteScheduledMessageResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
    }
}