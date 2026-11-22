#nullable disable
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SlackNet.Events;

public class Event
{
    public string Type { get; set; }

    public string EventTs { get; set; }

    /// <summary>
    /// Anything that Slack includes in the event that isn't covered by other properties can be accessed through this property.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// </summary>
    [JsonExtensionData]
    [Obsolete("Use ExtraProperty<T> and SetExtraProperty instead.")]
    public Dictionary<string, object> ExtraProperties { get; set; } = new();

    private SlackJsonSettings _jsonSettings;

    /// <summary>
    /// Reads something Slack included in the event that isn't covered by other properties.
    /// If you find anything here, please raise an issue at https://github.com/soxtoby/SlackNet/issues so we can add it to the library.
    /// </summary>
    /// <param name="propertyName">Name of the property, as sent by Slack.</param>
    /// <param name="jsonSettings">The JSON settings to use. Defaults to the settings the event was read with, or SlackNet's defaults.</param>
    /// <returns>The property's value, or the default value if Slack didn't send the property.</returns>
    public T ExtraProperty<T>(string propertyName, SlackJsonSettings jsonSettings = null) =>
        ExtensionData.Read<T>(ExtraProperties, propertyName, jsonSettings ?? _jsonSettings);

    /// <summary>
    /// Sets a property that isn't covered by other properties, such as to test code that reads it with <see cref="ExtraProperty{T}"/>.
    /// </summary>
    /// <param name="propertyName">Name of the property, as sent by Slack.</param>
    /// <param name="value">The property's value.</param>
    /// <param name="jsonSettings">The JSON settings to use. Defaults to the settings the event was read with, or SlackNet's defaults.</param>
    public void SetExtraProperty(string propertyName, object value, SlackJsonSettings jsonSettings = null) =>
        ExtraProperties[propertyName] = ExtensionData.ToToken(value, jsonSettings ?? _jsonSettings);

    internal void CopyExtraPropertiesFrom(Event source)
    {
        ExtraProperties = source.ExtraProperties;
        _jsonSettings = source._jsonSettings;
    }

    [OnDeserialized]
    private void RememberJsonSettings(StreamingContext context) => _jsonSettings = SlackJsonSettings.FromContext(context);
}