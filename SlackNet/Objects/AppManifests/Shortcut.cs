#nullable enable
namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe a <a href="https://docs.slack.dev/interactivity/implementing-shortcuts/#shortcut-types">shortcut</a>.
/// </summary>
public class Shortcut
{
    /// <summary>
    /// The name of the shortcut.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The <c>callback_id</c> of this shortcut.
    /// </summary>
    public required string CallbackId { get; set; }

    /// <summary>
    /// A short description of this shortcut.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Specifies which <a href="https://docs.slack.dev/interactivity/implementing-shortcuts/">type of shortcut</a> is being described.
    /// </summary>
    public ShortcutType? Type { get; set; }
}

public enum ShortcutType
{
    /// <summary>
    /// Handled by <see cref="Interaction.MessageShortcut"/> handlers.
    /// </summary>
    Message,

    /// <summary>
    /// Handled by <see cref="Interaction.GlobalShortcut"/> handlers.
    /// </summary>
    Global
}
