using System.Linq;

namespace SlackNet.Bot
{
    static class ConversationConversion
    {
        public static Hub ToHub(this Conversation conversation) =>
            conversation.IsIm
                ? (Hub)conversation.ToIm()
                : conversation.ToChannel();
        
        public static Channel ToChannel(this Conversation conversation) =>
            CopyHubProperties(conversation, new Channel
                {
                    Name = conversation.Name,
                    Creator = conversation.Creator,
                    IsArchived = conversation.IsArchived,
                    Topic = conversation.Topic,
                    Purpose = conversation.Topic
                });

        public static Im ToIm(this Conversation conversation) =>
            CopyHubProperties(conversation, new Im
                {
                    User = conversation.User,
                    IsUserDeleted = conversation.IsUserDeleted
                });

        private static THub CopyHubProperties<THub>(Conversation conversation, THub hub)
            where THub : Hub
        {
            hub.Id = conversation.Id;
            hub.IsIm = conversation.IsIm;
            hub.IsChannel = conversation.IsChannel;
            hub.IsGroup = conversation.IsGroup;
            hub.IsMpim = conversation.IsMpim;
            hub.IsOrgShared = conversation.IsOrgShared;
            hub.Created = conversation.Created;
            hub.Members = conversation.Members.ToArray();
            hub.LastRead = conversation.LastRead;
            hub.Latest = conversation.Latest;
            hub.UnreadCount = conversation.UnreadCount;
            hub.UnreadCountDisplay = conversation.UnreadCountDisplay;
            hub.IsGeneral = conversation.IsGeneral;
            hub.IsMember = conversation.IsMember;
            return hub;
        }
    }
}