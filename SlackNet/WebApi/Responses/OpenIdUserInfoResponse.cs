using Newtonsoft.Json;

namespace SlackNet.WebApi
{
    public class OpenIdUserInfoResponse
    {
        string Ok { get; set; }
        string Sub { get; set; }

        [JsonProperty("https://slack.com/user_id")]
        string UserId { get; set; }

        [JsonProperty("https://slack.com/team_id")]
        string TeamId { get; set; }

        string Email { get; set; }
        string EmailVerified { get; set; }
        string DateEmailVerified { get; set; }
        string Name { get; set; }
        string Picture { get; set; }
        string GivenName { get; set; }
        string FamilyName { get; set; }
        string Locale { get; set; }

        [JsonProperty("https://slack.com/team_name")]
        string TeamName { get; set; }
        [JsonProperty("https://slack.com/team_domain")]
        string TeamDomain { get; set; }

        [JsonProperty("https://slack.com/user_image_24")]
        string UserImage24 { get; set; }
        [JsonProperty("https://slack.com/user_image_32")]
        string UserImage32 { get; set; }
        [JsonProperty("https://slack.com/user_image_48")]
        string UserImage48 { get; set; }
        [JsonProperty("https://slack.com/user_image_72")]
        string UserImage72 { get; set; }
        [JsonProperty("https://slack.com/user_image_192")]
        string UserImage192 { get; set; }
        [JsonProperty("https://slack.com/user_image_512")]
        string UserImage512 { get; set; }

        [JsonProperty("https://slack.com/team_image_34")]
        string TeamImage34 { get; set; }
        [JsonProperty("https://slack.com/team_image_44")]
        string TeamImage44 { get; set; }
        [JsonProperty("https://slack.com/team_image_68")]
        string TeamImage68 { get; set; }
        [JsonProperty("https://slack.com/team_image_88")]
        string TeamImage88 { get; set; }
        [JsonProperty("https://slack.com/team_image_102")]
        string TeamImage102 { get; set; }
        [JsonProperty("https://slack.com/team_image_132")]
        string TeamImage132 { get; set; }
        [JsonProperty("https://slack.com/team_image_230")]
        string TeamImage230 { get; set; }
        [JsonProperty("https://slack.com/team_image_default")]
        string TeamImageDefault { get; set; }
    }
}
