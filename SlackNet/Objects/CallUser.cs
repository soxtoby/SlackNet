namespace SlackNet;

public class CallUser
{
    public string SlackId { get; set; }
    public string ExternalId { get; set; }
    public string DisplayName { get; set; }
    public string AvatarUrl { get; set; }
        
    public static implicit operator CallUser(User user) => new() { SlackId = user.Id };
}