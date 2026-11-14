#nullable enable
using System;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// A custom emoji in an Enterprise organization.
/// </summary>
public class CustomEmoji
{
    /// <summary>
    /// The URL of the emoji's image.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// The time the emoji was added.
    /// </summary>
    public int DateCreated { get; set; }

    /// <summary>
    /// The time the emoji was added.
    /// </summary>
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The <see cref="User.Id"/> of the user who uploaded the emoji.
    /// </summary>
    public required string UploadedBy { get; set; }
}
