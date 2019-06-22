using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.WebApi;

namespace SlackNet.Interaction
{
    public class MessageUpdateResponse : IReadOnlyMessage
    {
        private readonly MessageResponse _response;
        public MessageUpdateResponse(MessageResponse response) => _response = response;

        public ResponseType ResponseType => _response.ResponseType;
        public bool ReplaceOriginal => _response.ReplaceOriginal;
        public bool DeleteOriginal => _response.DeleteOriginal;
        public string Channel => _response.Message.Channel;
        public string Text => _response.Message.Text;
        public ParseMode Parse => _response.Message.Parse;
        public bool LinkNames => _response.Message.LinkNames;
        public IList<Attachment> Attachments => _response.Message.Attachments;
        public IList<Block> Blocks => _response.Message.Blocks;
        public bool UnfurlLinks => _response.Message.UnfurlLinks;
        public bool UnfurlMedia => _response.Message.UnfurlMedia;
        public string Username => _response.Message.Username;
        public bool AsUser => _response.Message.AsUser;
        public string IconUrl => _response.Message.IconUrl;
        public string IconEmoji => _response.Message.IconEmoji;
        public string ThreadTs => _response.Message.ThreadTs;
        public bool ReplyBroadcast => _response.Message.ReplyBroadcast;
    }
}