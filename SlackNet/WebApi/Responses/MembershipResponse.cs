using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class MembershipResponse
    {
        public List<string> Users { get; } = new List<string>();
    }
}