using System.Reflection;

namespace SlackNet.Rtm
{
    public abstract class OutgoingRtmEvent
    {
        public string Type => GetType().GetTypeInfo().SlackType();
    }
}