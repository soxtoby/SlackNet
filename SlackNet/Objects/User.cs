using System;
using Newtonsoft.Json;

namespace SlackNet.Objects
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Color { get; set; }
        public UserProfile Profile { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPrimaryOwner { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsUltraRestricted { get; set; }
        public int Updated { get; set; }
        public DateTime UpdateTime => Updated.ToDateTime().GetValueOrDefault();
        [JsonProperty("has_2fa")]
        public bool Has2FA { get; set; }
        public string TwoFactorType { get; set; }
        public EnterpriseUser EnterpriseUser { get; set; }
    }
}