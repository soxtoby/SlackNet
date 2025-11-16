namespace SlackNet;

public class EntityActionButton
{
    /// <summary>
    /// Human readable text that will be displayed in the button.
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
    /// </summary>
    public string ActionId { get; set; }
    
    /// <summary>
    /// The value to send along with the interaction payload.
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// Decorates the button with either a green or red background.
    /// </summary>
    public Style Style { get; set; }
    
    /// <summary>
    /// Clicking the button will open the URL in the user's browser.
    /// </summary>
    public string Url { get; set; }
    
    /// <summary>
    /// A label for longer descriptive text about a button element. This label will be read out by screen readers instead of the button's text parameter.
    /// </summary>
    public string AccessibilityLabel { get; set; }
    
    public EntityActionProcessingState ProcessingState { get; set; }
}

public class EntityActionProcessingState
{
    public bool Enabled { get; set; }
}