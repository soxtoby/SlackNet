namespace SlackNet.WebApi
{
    public class OauthV2AccessResponse
    {
        public string AccessToken { get; set; }
        #nullable enable
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
        #nullable disable
        public string Scope { get; set; }

        public Space Team { get; set; }
        public Space Enterprise { get; set; }
        public AuthedUser AuthedUser { get; set; }
    }

    public class Space
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AuthedUser
    {
        public string Id { get; set; }
        public string Scope { get; set; }
        public string AccessToken { get; set; }
        #nullable enable
        public int? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        #nullable disable
        public string TokenType { get; set; }
    }
}
