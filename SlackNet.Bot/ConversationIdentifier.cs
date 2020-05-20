using System.Threading.Tasks;

namespace SlackNet.Bot
{
    public abstract class ConversationIdentifier
    {
        public abstract Task<string> ConversationId(SlackBot bot);
        
        public static implicit operator ConversationIdentifier(string name) => 
              name?[0] == '@' ? new ImByName(name)
            : name?[0] == '#' ? (ConversationIdentifier)new ChannelByName(name)
            : new GroupByName(name);

        public static implicit operator ConversationIdentifier(Conversation conversation) => new ConversationByRef(conversation);
    }

    public class ConversationByRef : ConversationIdentifier
    {
        private readonly Conversation _conversation;
        public ConversationByRef(Conversation conversation) => _conversation = conversation;

        public override Task<string> ConversationId(SlackBot bot) => Task.FromResult(_conversation.Id);
    }

    public class ChannelByName : ConversationIdentifier
    {
        private readonly string _name;
        public ChannelByName(string name) => _name = name;
        
        public override Task<string> ConversationId(SlackBot bot) => Task.FromResult(_name);
    }

    public class GroupByName : ConversationIdentifier
    {
        private readonly string _name;
        public GroupByName(string name) => _name = name;
        
        public override Task<string> ConversationId(SlackBot bot) => Task.FromResult(_name);
    }

    public class ImByName : ConversationIdentifier
    {
        private readonly string _name;
        public ImByName(string name) => _name = name;

        public override async Task<string> ConversationId(SlackBot bot) => (await bot.GetImByName(_name.Substring(1)).ConfigureAwait(false)).Id;
    }
}
