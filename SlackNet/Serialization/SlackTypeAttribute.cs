using System;

namespace SlackNet;

/// <summary>
/// Specifies the string that Slack uses to identify this type, usually in the "type" property of an object.
/// Use this to override the default snake_case conversion used to match Slack types to .NET types.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class SlackTypeAttribute(string typeIdentifier) : Attribute
{
    public string TypeIdentifier { get; } = typeIdentifier;
}