using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using SlackNet.Events;
using SlackNet.Handlers;

namespace SlackNet.AzureFunctions;

public interface ISlackRequestHandler
{
	Task<SlackResult> HandleEventRequest(HttpRequestData request, SlackEndpointConfiguration config);
	Task<SlackResult> HandleActionRequest(HttpRequestData request, SlackEndpointConfiguration config);
	Task<SlackResult> HandleOptionsRequest(HttpRequestData request, SlackEndpointConfiguration config);
	Task<SlackResult> HandleSlashCommandRequest(HttpRequestData request, SlackEndpointConfiguration config);
}

public class SlackRequestHandler : ISlackRequestHandler
{
	private readonly IEnumerable<ISlackRequestListener> _requestListeners;
	private readonly ISlackHandlerFactory _handlerFactory;
	private readonly SlackJsonSettings _jsonSettings;
	private readonly ILogger _log;

	public SlackRequestHandler(
		IEnumerable<ISlackRequestListener> requestListeners,
		ISlackHandlerFactory handlerFactory,
		SlackJsonSettings jsonSettings,
		ILogger logger)
	{
		_requestListeners = requestListeners;
		_handlerFactory = handlerFactory;
		_jsonSettings = jsonSettings;
		_log = logger.ForSource<SlackRequestHandler>();
	}

	public Task<SlackResult> HandleEventRequest( HttpRequestData request, SlackEndpointConfiguration config ) =>
		InRequestContext(request,
			async requestContext =>
				{
					if (request.Method != "POST")
					{
						_log.Internal("Request method {RequestMethod} blocked - only POST is allowed", request.Method);
						return new SlackResult( request.FunctionContext, HttpStatusCode.MethodNotAllowed);
					}

					if (request.ContentType != "application/json")
					{
						_log.Internal("Request Content-Type {ContentType} blocked - only application/json is allowed", request.ContentType);
						return new SlackResult( request.FunctionContext, HttpStatusCode.UnsupportedMediaType);
					}

					var requestBody = await ReadString(request).ConfigureAwait(false);
                    
					_log.WithContext("RequestBody", requestBody)
						.Data("Received event request");
                    
					var eventRequest = DeserializeEventRequest(requestBody);

					if (eventRequest is null)
					{
						_log.Internal("Unrecognized event request content");
						return new SlackResult( request.FunctionContext, HttpStatusCode.BadRequest);
					}

					return EventRequestValidation(requestBody, request.Headers, eventRequest, config)
					       ?? eventRequest switch
						       {
							       EventCallback eventCallback => HandleEvent(requestContext, eventCallback),
							       UrlVerification urlVerification => HandleUrlVerification(urlVerification),
							       _ => UnrecognizedEventRequestType(eventRequest)
						       };
				});
	
	private SlackResult HandleEvent(SlackRequestContext requestContext, EventCallback eventCallback) =>
		new SlackResult(HttpStatusCode.OK)
			.OnCompleted(() =>
				{
					var handler = _handlerFactory.CreateEventHandler(requestContext);
					_log.RequestHandler(handler, eventCallback, "Handling {EventType} event", eventCallback.Event.Type);
					return handler.Handle(eventCallback);
				});

	private SlackResult HandleUrlVerification(UrlVerification urlVerification)
	{
		_log.Internal("Responding to URL verification challenge");
		return new SlackResult(HttpStatusCode.OK, urlVerification.Challenge);
	}

	private SlackResult UnrecognizedEventRequestType(EventRequest eventRequest)
	{
		_log.Request("Unrecognized event request type {RequestType}", eventRequest.Type);
		return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
	}

	public Task<SlackResult> HandleActionRequest( HttpRequestData request, SlackEndpointConfiguration config )
	{
		throw new NotImplementedException( );
	}

	public Task<SlackResult> HandleOptionsRequest( HttpRequestData request, SlackEndpointConfiguration config )
	{
		throw new NotImplementedException( );
	}

	public Task<SlackResult> HandleSlashCommandRequest( HttpRequestData request, SlackEndpointConfiguration config )
	{
		throw new NotImplementedException( );
	}
	
	private async Task<SlackResult> InRequestContext(HttpRequestData request, Func<SlackRequestContext, Task<SlackResult>> handleRequest)
	{
		var requestContext = SlackRequestContext.Current = new SlackRequestContext
			{
				[nameof(SlackRequestContext.RequestId)] = request.FunctionContext.InvocationId
			};
		var requestScope = requestContext.BeginRequest(_requestListeners);

		try
		{
			return (await handleRequest(requestContext).ConfigureAwait(false))
				.OnCompleted(() => requestScope.DisposeAsync().AsTask());
		}
		catch (Exception e)
		{
			_log.Error(e, "Error handling request");
			throw;
		}
	}
	
	private EventRequest? DeserializeEventRequest(string requestBody)
	{
		using var stringReader = new StringReader(requestBody);
		using var jsonTextReader = new JsonTextReader(stringReader);
		return JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<EventRequest>(jsonTextReader);
	}
}