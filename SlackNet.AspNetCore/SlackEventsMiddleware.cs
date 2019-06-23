using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SlackNet.Events;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackEventsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SlackEndpointConfiguration _configuration;
        private readonly ISlackEvents _slackEvents;
        private readonly ISlackBlockActions _slackBlockActions;
        private readonly ISlackBlockOptions _slackBlockOptions;
        private readonly ISlackInteractiveMessages _slackInteractiveMessages;
        private readonly ISlackMessageActions _slackMessageActions;
        private readonly ISlackOptions _slackOptions;
        private readonly IDialogSubmissionHandler _dialogSubmissionHandler;
        private readonly SlackJsonSettings _jsonSettings;

        public SlackEventsMiddleware(
            RequestDelegate next,
            SlackEndpointConfiguration configuration,
            ISlackEvents slackEvents,
            ISlackBlockActions slackBlockActions,
            ISlackBlockOptions slackBlockOptions,
            ISlackInteractiveMessages slackInteractiveMessages,
            ISlackMessageActions slackMessageActions,
            ISlackOptions slackOptions, 
            IDialogSubmissionHandler dialogSubmissionHandler,
            SlackJsonSettings jsonSettings)
        {
            _next = next;
            _configuration = configuration;
            _slackEvents = slackEvents;
            _slackBlockActions = slackBlockActions;
            _slackBlockOptions = slackBlockOptions;
            _slackInteractiveMessages = slackInteractiveMessages;
            _slackMessageActions = slackMessageActions;
            _slackOptions = slackOptions;
            _dialogSubmissionHandler = dialogSubmissionHandler;
            _jsonSettings = jsonSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == $"/{_configuration.RoutePrefix}/event")
                await HandleSlackEvent(context).ConfigureAwait(false);
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/action")
                await HandleSlackAction(context).ConfigureAwait(false);
            else if (context.Request.Path == $"/{_configuration.RoutePrefix}/options")
                await HandleSlackOptions(context).ConfigureAwait(false);
            else
                await _next(context).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleSlackEvent(HttpContext context)
        {
            if (context.Request.Method != "POST")
                return await context.Respond(HttpStatusCode.MethodNotAllowed).ConfigureAwait(false);

            if (context.Request.ContentType != "application/json")
                return await context.Respond(HttpStatusCode.UnsupportedMediaType).ConfigureAwait(false);

            var body = DeserializeRequestBody(context);

            if (body is UrlVerification urlVerification && IsValidToken(urlVerification.Token))
                return await context.Respond(HttpStatusCode.OK, "application/x-www-form-urlencoded", urlVerification.Challenge).ConfigureAwait(false);

            if (body is EventCallback eventCallback && IsValidToken(eventCallback.Token))
            {
                var response = context.Respond(HttpStatusCode.OK).ConfigureAwait(false);
                _slackEvents.Handle(eventCallback);
                return await response;
            }

            return await context.Respond(HttpStatusCode.BadRequest, body: "Invalid token or unrecognized content").ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleSlackAction(HttpContext context)
        {
            if (context.Request.Method != "POST")
                return await context.Respond(HttpStatusCode.MethodNotAllowed).ConfigureAwait(false);

            var interactionRequest = await DeserializePayload<InteractionRequest>(context).ConfigureAwait(false);

            if (interactionRequest != null && IsValidToken(interactionRequest.Token))
            {
                switch (interactionRequest)
                {
                    case BlockActionRequest blockActions:
                        return await HandleBlockActions(context, blockActions).ConfigureAwait(false);
                    case InteractiveMessage interactiveMessage:
                        return await HandleInteractiveMessage(context, interactiveMessage).ConfigureAwait(false);
                    case DialogSubmission dialogSubmission:
                        return await HandleDialogSubmission(context, dialogSubmission).ConfigureAwait(false);
                    case DialogCancellation dialogCancellation:
                        return await HandleDialogCancellation(context, dialogCancellation).ConfigureAwait(false);
                    case MessageAction messageAction:
                        return await HandleMessageAction(context, messageAction).ConfigureAwait(false);
                }
            }

            return await context.Respond(HttpStatusCode.BadRequest, body: "Invalid token or unrecognized content").ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleBlockActions(HttpContext context, BlockActionRequest blockActionRequest)
        {
            await _slackBlockActions.Handle(blockActionRequest).ConfigureAwait(false);
            return await context.Respond(HttpStatusCode.OK).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleInteractiveMessage(HttpContext context, InteractiveMessage interactiveMessage)
        {
            var response = await _slackInteractiveMessages.Handle(interactiveMessage).ConfigureAwait(false);

            var responseJson = response == null ? null
                : interactiveMessage.IsAppUnfurl ? Serialize(new AttachmentUpdateResponse(response))
                : Serialize(new MessageUpdateResponse(response));

            return await context.Respond(HttpStatusCode.OK, "application/json", responseJson).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleDialogSubmission(HttpContext context, DialogSubmission dialog)
        {
            var errors = (await _dialogSubmissionHandler.Handle(dialog).ConfigureAwait(false))?.ToList()
                ?? new List<DialogError>();

            return errors.Any()
                ? await context.Respond(HttpStatusCode.OK, "application/json", Serialize(new DialogErrorResponse { Errors = errors })).ConfigureAwait(false)
                : await context.Respond(HttpStatusCode.OK).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleDialogCancellation(HttpContext context, DialogCancellation dialogCancellation)
        {
            await _dialogSubmissionHandler.HandleCancel(dialogCancellation).ConfigureAwait(false);
            return await context.Respond(HttpStatusCode.OK).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleMessageAction(HttpContext context, MessageAction messageAction)
        {
            await _slackMessageActions.Handle(messageAction).ConfigureAwait(false);
            return await context.Respond(HttpStatusCode.OK).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleSlackOptions(HttpContext context)
        {
            if (context.Request.Method != "POST")
                return await context.Respond(HttpStatusCode.MethodNotAllowed).ConfigureAwait(false);

            var optionsRequest = await DeserializePayload<OptionsRequestBase>(context).ConfigureAwait(false);

            if (optionsRequest != null && IsValidToken(optionsRequest.Token))
            {
                switch (optionsRequest)
                {
                    case OptionsRequest legacyOptionsRequest:
                        return await HandleLegacyOptionsRequest(context, legacyOptionsRequest).ConfigureAwait(false);
                    case BlockOptionsRequest blockOptionsRequest:
                        return await HandleBlockOptionsRequest(context, blockOptionsRequest).ConfigureAwait(false);
                }
            }

            return await context.Respond(HttpStatusCode.BadRequest, body: "Invalid token or unrecognized content").ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleLegacyOptionsRequest(HttpContext context, OptionsRequest optionsRequest)
        {
            var response = await _slackOptions.Handle(optionsRequest).ConfigureAwait(false);
            return await context.Respond(HttpStatusCode.OK, "application/json", Serialize(response)).ConfigureAwait(false);
        }

        private async Task<HttpResponse> HandleBlockOptionsRequest(HttpContext context, BlockOptionsRequest blockOptionsRequest)
        {
            var response = await _slackBlockOptions.Handle(blockOptionsRequest).ConfigureAwait(false);
            return await context.Respond(HttpStatusCode.OK, "application/json", Serialize(response)).ConfigureAwait(false);
        }

        private async Task<T> DeserializePayload<T>(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync().ConfigureAwait(false);

            return form["payload"]
                .Select(p => JsonConvert.DeserializeObject<T>(p, _jsonSettings.SerializerSettings))
                .FirstOrDefault();
        }

        private bool IsValidToken(string token) => string.IsNullOrEmpty(_configuration.VerificationToken) || token == _configuration.VerificationToken;

        private string Serialize(object value) => JsonConvert.SerializeObject(value, _jsonSettings.SerializerSettings);

        private Event DeserializeRequestBody(HttpContext context) =>
            JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<Event>(new JsonTextReader(new StreamReader(context.Request.Body)));
    }
}