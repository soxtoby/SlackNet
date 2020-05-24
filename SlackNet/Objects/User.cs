using System;
using Newtonsoft.Json;

namespace SlackNet
{
    public class User
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Color { get; set; }
        public string RealName { get; set; }
        public string Tz { get; set; }
        public string TzLabel { get; set; }
        public int TzOffset { get; set; }
        public UserProfile Profile { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPrimaryOwner { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsUltraRestricted { get; set; }
        public bool IsBot { get; set; }
        public bool IsStranger { get; set; }
        public int Updated { get; set; }
        public bool IsAppUser { get; set; }
        public bool IsInvitedUser { get; set; }
        [JsonIgnore]
        public DateTime UpdateTime => Updated.ToDateTime().GetValueOrDefault();
        [JsonProperty("has_2fa")]
        public bool Has2FA { get; set; }
        public string TwoFactorType { get; set; }
        public string Locale { get; set; }
        public EnterpriseUser EnterpriseUser { get; set; }

        public override string ToString() => Link.User(Id).ToString();
    }
}