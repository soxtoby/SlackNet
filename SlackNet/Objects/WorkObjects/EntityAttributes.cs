using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet;

public class EntityAttributes
{
    /// <summary>
    /// Title of the Work Object entity.
    /// </summary>
    public EntityTitle Title { get; set; } = new(); // NOCOMMIT was just { Text }, so test if different types work 
    
    /// <summary>
    /// User-friendly string ID to display on the entity.
    /// </summary>
    public string DisplayId { get; set; }
    
    /// <summary>
    /// String that represents the resource being unfurled. Default is <see cref="EntityDisplayType.File"/> for a file entity or <see cref="EntityDisplayType.Task"/> for a task entity.
    /// </summary>    
    public EntityDisplayType? DisplayType { get; set; }
    
    /// <summary>
    /// Name of the product that will be displayed in the Work Objects header. Default is the app name.
    /// </summary>
    public string ProductName { get; set; }
    
    /// <summary>
    /// A product icon to display in the Work Object header. Default is the app icon.
    /// If you choose to provide a custom icon, use a different image than your app icon to avoid
    /// displaying duplicate icons in the UI.
    /// Must be a publicly-accessible URL or a <see cref="ImageFileReference"/> object.
    /// </summary>
    public EntityIcon ProductIcon { get; set; }
    
    /// <summary>
    /// If your entity supports a visual preview within Slack, you can provide a thumbnail imagein the unfurl card
    /// and a full size image or PDF preview to be displayed in the full size preview modal.
    /// </summary>
    public FullSizePreview FullSizePreview { get; set; }
    
    /// <summary>
    /// This field indicates when the metadata about this item was last modified.
    /// Slack uses this to determine whether they should request new unfurl metadata via the <see cref="LinkShared"/> event.
    /// This will often be the same value as <see cref="EntityField.DateUpdated"/>, but can be controlled independently
    /// to offer less/more frequent requests to update unfurl metadata.
    /// </summary>
    public long MetadataLastModified { get; set; }

    /// <summary>
    /// This field indicates when the metadata about this item was last modified.
    /// Slack uses this to determine whether they should request new unfurl metadata via the <see cref="LinkShared"/> event.
    /// This will often be the same value as <see cref="EntityField.DateUpdated"/>, but can be controlled independently
    /// to offer less/more frequent requests to update unfurl metadata.
    /// </summary>
    [JsonIgnore]
    public DateTime? MetadataLastModifiedDateTime
    {
        get => MetadataLastModified.ToDateTime();
        set => MetadataLastModified = value?.ToTimestampNumber() ?? 0;
    }
}

public class EntityTitle
{
    public string Text { get; set; }
    public EntityEditSupport Edit { get; set; }
}

public enum EntityDisplayType
{
    [EnumMember(Value = "File")]
    File,
    [EnumMember(Value = "Task")]
    Task,
    [EnumMember(Value = "Document")]
    Document
}

public class FullSizePreview
{
    /// <summary>
    /// Lets Slack know that a full preview is supported.
    /// </summary>
    public bool IsSupported { get; set; }
    /// <summary>
    /// Publicly-accessible URL to an image.
    /// </summary>
    public string PreviewUrl { get; set; }
    /// <summary>
    /// Mime type for the preview.
    /// </summary>
    public string MimeType { get; set; }
    /// <summary>
    /// Error message to display if the full-size preview is not available.
    /// </summary>
    public FullSizePreviewError Error { get; set; }
}

public class FullSizePreviewError
{
    public FullSizePreviewErrorCode Code { get; set; }
    public string Message { get; set; }
}

public enum FullSizePreviewErrorCode
{
    FileNotSupported,
    FileSizeExceeded,
    Custom
}