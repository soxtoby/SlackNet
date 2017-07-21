using System.Threading.Tasks;

namespace SlackNet.Bot
{
    public abstract class HubIdentifier
    {
        public abstract Task<string> HubId(SlackBot bot);
        
        public static implicit operator HubIdentifier(string name) => 
              name?[0] == '@' ? new ImByName(name)
            : name?[0] == '#' ? (HubIdentifier)new ChannelByName(name)
            : new GroupByName(name);

        public static implicit operator HubIdentifier(Hub hub) => new HubByRef(hub);
    }

    public class HubByRef : HubIdentifier
    {
        private readonly Hub _hub;
        public HubByRef(Hub hub) => _hub = hub;

        public override Task<string> HubId(SlackBot bot) => Task.FromResult(_hub.Id);
    }

    public class ChannelByName : HubIdentifier
    {
        private readonly string _name;
        public ChannelByName(string name) => _name = name;
        
        public override Task<string> HubId(SlackBot bot) => Task.FromResult(_name);
    }

    public class GroupByName : HubIdentifier
    {
        private readonly string _name;
        public GroupByName(string name) => _name = name;
        
        public override Task<string> HubId(SlackBot bot) => Task.FromResult(_name);
    }

    public class ImByName : HubIdentifier
    {
        private readonly string _name;
        public ImByName(string name) => _name = name;

        public override async Task<string> HubId(SlackBot bot) => (await bot.GetImByName(_name.Substring(1))).Id;
    }
}
