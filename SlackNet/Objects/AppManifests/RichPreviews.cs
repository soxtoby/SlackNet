#nullable enable
using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// Settings that describe rich previews configuration.
/// </summary>
public class RichPreviews
{
    /// <summary>
    /// Whether or not Work Object embeds render with the allow-same-origin sandbox token.
    /// </summary>
    public bool? AllowSameOrigin { get; set; }

    /// <summary>
    /// Domains allowed for Work Object embeds.
    /// </summary>
    public IList<string>? DomainAllowlist { get; set; }

    /// <summary>
    /// Entity types for rich previews.
    /// </summary>
    public IList<string>? EntityTypes { get; set; }

    /// <summary>
    /// Whether or not rich previews are enabled.
    /// </summary>
    public bool? IsActive { get; set; }

    public RichPreviewSlug? Slug { get; set; }
}

public class RichPreviewSlug
{
    /// <summary>
    /// Whether or not a task's slug leaves out its display ID.
    /// </summary>
    public bool? HideDisplayId { get; set; }
}
