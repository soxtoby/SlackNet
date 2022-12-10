namespace SlackNet;

public class ConnectedTeam
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Icons Icon { get; set; }
    public bool IsVerified { get; set; }
    public string Domain { get; set; }
    public int DateCreated { get; set; }
}