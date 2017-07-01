using System.Collections.Generic;

namespace SlackNet.WebApi.Responses
{
    class MembershipResponse
    {
        public List<string> Users { get; } = new List<string>();
    }
}