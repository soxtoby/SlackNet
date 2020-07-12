using System.Threading.Tasks;

namespace SlackNet.Bot
{
    public abstract class ConversationIdentifier
    {
        public abstract Task<string> ConversationId(ISlackBot bot);

        public static implicit operator ConversationIdentifier(string name) => new ConversationByName(name);
        public static implicit operator ConversationIdentifier(Conversation conversation) => new ConversationByRef(conversation);
        
        public static implicit operator ConversationIdentifier(Hub hub) => new ConversationByHubIdentifier(hub);
        public static implicit operator ConversationIdentifier(HubIdentifier hubId) => new ConversationByHubIdentifier(hubId);
    }

    public class ConversationByName : ConversationIdentifier
    {
        private readonly string _name;

        /// <param name="name">Channel, group or IM name, with leading # or @ symbol as appropriate.</param>
        public ConversationByName(string name) => _name = name;

        public override async Task<string> ConversationId(ISlackBot bot) =>
            (await bot.GetConversationByName(_name).ConfigureAwait(false)).Id;
    }

    public class ConversationByRef : ConversationIdentifier
    {
        private readonly Conversation _conversation;
        public ConversationByRef(Conversation conversation) => _conversation = conversation;

        public override Task<string> ConversationId(ISlackBot bot) => Task.FromResult(_conversation.Id);
    }

    class ConversationByHubIdentifier : ConversationIdentifier
    {
        private readonly HubIdentifier _hubIdentifier;
        public ConversationByHubIdentifier(HubIdentifier hubIdentifier) => _hubIdentifier = hubIdentifier;
        
        public override Task<string> ConversationId(ISlackBot bot) => _hubIdentifier.HubId(bot);
    }
}