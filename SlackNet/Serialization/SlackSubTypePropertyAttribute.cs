using System;

namespace SlackNet;

/// <summary>
/// Specifies which property to use to differentiate between multiple Slack subtypes on a single class.
/// If not specified, <c>Subtype</c> will be used.
/// </summary>
/// <param name="propertyName"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SlackSubTypePropertyAttribute(string propertyName): Attribute
{
    public string PropertyName { get; } = propertyName;
}