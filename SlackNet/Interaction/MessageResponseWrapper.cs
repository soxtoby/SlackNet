using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.WebApi;

namespace SlackNet.Interaction;

public class MessageResponseWrapper(IMessageResponse response) : IReadOnlyMessage
{
    public string Channel => response.Message?.Channel;
    public string Text => response.Message?.Text;
    public ParseMode Parse => response.Message?.Parse ?? default;
    public bool LinkNames => response.Message?.LinkNames ?? false;
    public IList<Attachment> Attachments => response.Message?.Attachments;
    public IList<Block> Blocks => response.Message?.Blocks;
    public bool UnfurlLinks => response.Message?.UnfurlLinks ?? false;
    public bool UnfurlMedia => response.Message?.UnfurlMedia ?? false;
    public string Username => response.Message?.Username;
    public bool? AsUser => response.Message?.AsUser;
    public string IconUrl => response.Message?.IconUrl;
    public string IconEmoji => response.Message?.IconEmoji;
    public string ThreadTs => response.Message?.ThreadTs;
    public bool ReplyBroadcast => response.Message?.ReplyBroadcast ?? false;
    public MessageMetadata MetadataJson => response.Message?.MetadataJson;
    public object MetadataObject => response.Message?.MetadataObject;
    public UnfurlMetadata UnfurlMetadata => response.Message?.UnfurlMetadata;
}