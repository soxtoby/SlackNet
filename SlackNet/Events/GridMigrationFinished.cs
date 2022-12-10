namespace SlackNet.Events;

/// <summary>
/// An enterprise grid migration has finished on this workspace.
/// </summary>
public class GridMigrationFinished : Event
{
    public string EnterpriseId { get; set; }
}