namespace SlackNet.Interaction
{
    public class SlashCommandMessageResponse : MessageResponseWrapper
    {
        private readonly SlashCommandResponse _commandResponse;
        public SlashCommandMessageResponse(SlashCommandResponse response) : base(response) => _commandResponse = response;

        public ResponseType ResponseType => _commandResponse.ResponseType;
    }
}