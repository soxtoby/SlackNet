using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.WebApi;

public class MessageSearchResult
{
    public string Iid { get; set; }
    public string Team { get; set; }
    public double Score { get; set; }
    public Conversation Channel { get; set; }
    public string Type { get; set; }
    public string User { get; set; }
    public string Username { get; set; }
    public string Ts { get; set; }
    public IList<Attachment> Attachments { get; set; } = [];
    public IList<Block> Blocks { get; set; } = [];
    public string Text { get; set; }
    public string Permalink { get; set; }
    public bool NoReactions { get; set; }

    /// <summary>
    /// Anything that Slack includes in the search result that isn't covered by other properties can be accessed through this property.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// </summary>
    [JsonExtensionData]
    [Obsolete("Use ExtraProperty<T> and SetExtraProperty instead.")]
    public Dictionary<string, object> ExtraProperties { get; set; } = new();

    private SlackJsonSettings _jsonSettings;

    /// <summary>
    /// Reads something Slack included in the search result that isn't covered by other properties.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// </summary>
    /// <param name="propertyName">Name of the property, as sent by Slack.</param>
    /// <param name="jsonSettings">The JSON settings to use. Defaults to the settings the search result was read with, or SlackNet's defaults.</param>
    /// <returns>The property's value, or the default value if Slack didn't send the property.</returns>
    public T ExtraProperty<T>(string propertyName, SlackJsonSettings jsonSettings = null) =>
        ExtensionData.Read<T>(ExtraProperties, propertyName, jsonSettings ?? _jsonSettings);

    /// <summary>
    /// Sets a property that isn't covered by other properties, such as to test code that reads it with <see cref="ExtraProperty{T}"/>.
    /// </summary>
    /// <param name="propertyName">Name of the property, as sent by Slack.</param>
    /// <param name="value">The property's value.</param>
    /// <param name="jsonSettings">The JSON settings to use. Defaults to the settings the search result was read with, or SlackNet's defaults.</param>
    public void SetExtraProperty(string propertyName, object value, SlackJsonSettings jsonSettings = null) =>
        ExtraProperties[propertyName] = ExtensionData.ToToken(value, jsonSettings ?? _jsonSettings);

    [OnDeserialized]
    private void RememberJsonSettings(StreamingContext context) => _jsonSettings = SlackJsonSettings.FromContext(context);
}