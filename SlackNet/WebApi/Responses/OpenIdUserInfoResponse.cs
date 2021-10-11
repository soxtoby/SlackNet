using Newtonsoft.Json;

namespace SlackNet.WebApi
{
    public class OpenIdUserInfoResponse
    {
        public string Ok { get; set; }
        public string Sub { get; set; }

        [JsonProperty("https://slack.com/user_id")]
        public string UserId { get; set; }

        [JsonProperty("https://slack.com/team_id")]
        public string TeamId { get; set; }

        public string Email { get; set; }
        public string EmailVerified { get; set; }
        public string DateEmailVerified { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Locale { get; set; }

        [JsonProperty("https://slack.com/team_name")]
        public string TeamName { get; set; }
        [JsonProperty("https://slack.com/team_domain")]
        public string TeamDomain { get; set; }

        [JsonProperty("https://slack.com/user_image_24")]
        public string UserImage24 { get; set; }
        [JsonProperty("https://slack.com/user_image_32")]
        public string UserImage32 { get; set; }
        [JsonProperty("https://slack.com/user_image_48")]
        public string UserImage48 { get; set; }
        [JsonProperty("https://slack.com/user_image_72")]
        public string UserImage72 { get; set; }
        [JsonProperty("https://slack.com/user_image_192")]
        public string UserImage192 { get; set; }
        [JsonProperty("https://slack.com/user_image_512")]
        public string UserImage512 { get; set; }

        [JsonProperty("https://slack.com/team_image_34")]
        public string TeamImage34 { get; set; }
        [JsonProperty("https://slack.com/team_image_44")]
        public string TeamImage44 { get; set; }
        [JsonProperty("https://slack.com/team_image_68")]
        public string TeamImage68 { get; set; }
        [JsonProperty("https://slack.com/team_image_88")]
        public string TeamImage88 { get; set; }
        [JsonProperty("https://slack.com/team_image_102")]
        public string TeamImage102 { get; set; }
        [JsonProperty("https://slack.com/team_image_132")]
        public string TeamImage132 { get; set; }
        [JsonProperty("https://slack.com/team_image_230")]
        public string TeamImage230 { get; set; }
        [JsonProperty("https://slack.com/team_image_default")]
        public string TeamImageDefault { get; set; }
    }
}
