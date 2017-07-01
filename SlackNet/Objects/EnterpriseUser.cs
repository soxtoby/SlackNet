using System.Collections.Generic;

namespace SlackNet.Objects
{
    public class EnterpriseUser
    {
        public string Id { get; set; }
        public string EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public IList<string> Teams { get; set; }
    }
}