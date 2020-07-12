using System;
using System.Threading.Tasks;

namespace SlackNet.Bot
{
    [Obsolete("Use ConversationIdentifier instead")]
    public abstract class HubIdentifier
    {
        public abstract Task<string> HubId(ISlackBot bot);
        
        public static implicit operator HubIdentifier(string name) => 
              name?[0] == '@' ? new ImByName(name)
            : name?[0] == '#' ? (HubIdentifier)new ChannelByName(name)
            : new GroupByName(name);

        public static implicit operator HubIdentifier(Hub hub) => new HubByRef(hub);
        
        public static implicit operator HubIdentifier(Conversation conversation) => new HubByConversationIdentifier(conversation);
        public static implicit operator HubIdentifier(ConversationIdentifier conversationIdentifier) => new HubByConversationIdentifier(conversationIdentifier);
    }

    [Obsolete("Use ConversationByRef instead")]
    public class HubByRef : HubIdentifier
    {
        private readonly Hub _hub;
        public HubByRef(Hub hub) => _hub = hub;

        public override Task<string> HubId(ISlackBot bot) => Task.FromResult(_hub.Id);
    }

    [Obsolete("Use ConversationByName instead")]
    public class ChannelByName : HubIdentifier
    {
        private readonly string _name;
        public ChannelByName(string name) => _name = name;
        
        public override Task<string> HubId(ISlackBot bot) => Task.FromResult(_name);
    }

    [Obsolete("Use ConversationByName instead")]
    public class GroupByName : HubIdentifier
    {
        private readonly string _name;
        public GroupByName(string name) => _name = name;
        
        public override Task<string> HubId(ISlackBot bot) => Task.FromResult(_name);
    }

    [Obsolete("Use ConversationByName instead")]
    public class ImByName : HubIdentifier
    {
        private readonly string _name;
        public ImByName(string name) => _name = name;

        public override async Task<string> HubId(ISlackBot bot) => (await bot.GetImByName(_name.Substring(1)).ConfigureAwait(false)).Id;
    }

    class HubByConversationIdentifier : HubIdentifier
    {
        private readonly ConversationIdentifier _conversationIdentifier;
        public HubByConversationIdentifier(ConversationIdentifier conversationIdentifier) => _conversationIdentifier = conversationIdentifier;
        
        public override Task<string> HubId(ISlackBot bot) => _conversationIdentifier.ConversationId(bot);
    }
}
