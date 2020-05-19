using System.Linq;

namespace SlackNet.WebApi
{
    public enum ConversationType
    {
        PublicChannel,
        PrivateChannel,
        Mpim,
        Im
    }

    public static class ConversationTypeExtensions
    {
        public static string ToSnakeCase(this ConversationType type) =>
            string.Concat(type.ToString()
                    .Select((x, i) => i > 0 && char.IsUpper(x) ? $"_{x}" : x.ToString()))
                .ToLowerInvariant();

    }
}