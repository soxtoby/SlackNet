using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SlackNet.AzureFunctions;

public class SlackResult : HttpResponseData
{
	private readonly IList<Func<Task>> _requestCompletedCallbacks = new List<Func<Task>>( );

	public SlackResult( FunctionContext functionContext, HttpStatusCode statusCode )
		: base( functionContext )
	{
		StatusCode = statusCode;
	}

	public override HttpStatusCode StatusCode { get; set; }
	public override HttpHeadersCollection Headers { get; set; }
	public override Stream Body { get; set; }
	public override HttpCookies Cookies { get; }

	public SlackResult OnCompleted( Func<Task> callback )
	{
		_requestCompletedCallbacks.Add( callback );
		return this;
	}
}