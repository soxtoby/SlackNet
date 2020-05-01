namespace SlackNet.Interaction
{
    public class MessageUpdateResponse : MessageResponseWrapper
    {
        private readonly MessageResponse _updateResponse;
        public MessageUpdateResponse(MessageResponse response) : base(response) => _updateResponse = response;

        public ResponseType ResponseType => _updateResponse.ResponseType;
        public bool ReplaceOriginal => _updateResponse.ReplaceOriginal;
        public bool DeleteOriginal => _updateResponse.DeleteOriginal;
    }
}