namespace SlackNet.Events;

/// <summary>
/// An enterprise grid migration has started on this workspace.
/// </summary>
public class GridMigrationStarted : Event
{
    public string EnterpriseId { get; set; }
}