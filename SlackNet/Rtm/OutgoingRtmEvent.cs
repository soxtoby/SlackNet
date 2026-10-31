using System.Reflection;

namespace SlackNet.Rtm;

public abstract class OutgoingRtmEvent : SlackObject
{
    public string Type => GetType().GetTypeInfo().SlackType();
}