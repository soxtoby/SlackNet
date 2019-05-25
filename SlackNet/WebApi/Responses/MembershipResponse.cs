using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class MembershipResponse
    {
        public List<string> Users { get; set; } = new List<string>();
    }
}