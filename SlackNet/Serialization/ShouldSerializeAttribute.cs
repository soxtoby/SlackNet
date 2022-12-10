using System;

namespace SlackNet;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class ShouldSerializeAttribute : Attribute
{
    public abstract bool ShouldSerialize(object value);
}