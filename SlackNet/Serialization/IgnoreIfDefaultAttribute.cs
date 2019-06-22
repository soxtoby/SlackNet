using System;
using System.Reflection;

namespace SlackNet
{
    public class IgnoreIfDefaultAttribute : ShouldSerializeAttribute
    {
        public override bool ShouldSerialize(object value)
        {
            if (ReferenceEquals(value, null))
                return false;

            var type = value.GetType();
            return !type.GetTypeInfo().IsValueType
                || !Equals(value, Activator.CreateInstance(type));
        }
    }
}