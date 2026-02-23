namespace SlackNet;

/// <summary>
/// Context about a user interaction within a function.
/// </summary>
public class Interactivity
{
    public string InteractivityPointer { get; set; }
    public UserContext Interactor { get; set; }
}
