using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet
{
    public class UserProfile
    {
        public string AvatarHash { get; set; }
        public string StatusEmoji { get; set; }
        public string StatusText { get; set; }
        public long StatusExpiration { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RealName { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Phone { get; set; }
        public bool AlwaysActive { get; set; }
        public string Image24 { get; set; }
        public string Image32 { get; set; }
        public string Image48 { get; set; }
        public string Image72 { get; set; }
        public string Image192 { get; set; }
        public string Image512 { get; set; }
        [JsonConverter(typeof(IgnoreArrayConverter))] // Slack returns an empty array instead of an object if profile has been modified, but no custom fields have been set
        public IDictionary<string, UserProfileField> Fields { get; set; } = new Dictionary<string, UserProfileField>();
    }
}