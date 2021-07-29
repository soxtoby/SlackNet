namespace SlackNet.WebApi
{
    public class OauthV2AccessResponse
    {
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? ExpiredIn { get; set; }
        public string Scope { get; set; }

        public Space Team { get; set; }
        public Space Enterprise { get; set; }
        public User User { get; set; }
    }

    public class Space
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Scope { get; set; }
        public string AccessToken { get; set; }
        public int? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public string TokenType { get; set; }
    }
}
