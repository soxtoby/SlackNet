namespace SlackNet
{
    public class Invite
    {
        public string Id { get; set; }
        public int DateCreated { get; set; }
        public int DateInvalid { get; set; }
        public ConnectedTeam InvitingTeam { get; set; }
        public User InvitingUser { get; set; }
        public string RecipientUserId { get; set; }
        public string Link { get; set; }
    }
}