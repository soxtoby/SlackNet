using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Determines when a<see cref="PlainTextInput"/> element will return a <see cref="BlockAction"/> interaction payload.
/// </summary>
public class DispatchActionConfig
{
    /// <summary>
    /// An array of <see cref="InteractionType"/> that you would like to receive a <see cref="BlockAction"/> payload for.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<InteractionType> TriggerActionsOn { get; set; } = new List<InteractionType>();
}

public enum InteractionType
{
    OnEnterPressed,
    OnCharacterEntered
}