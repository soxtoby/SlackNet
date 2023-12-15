using Microsoft.Extensions.DependencyInjection;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.AzureFunctions;

class IsolatedWorkerServiceProviderSlackRequestListener : IServiceProviderSlackRequestListener
{
	private readonly IFunctionContextAccessor _contextAccessor;
	private readonly IServiceProvider _serviceProvider;

	public IsolatedWorkerServiceProviderSlackRequestListener( IFunctionContextAccessor contextAccessor, IServiceProvider serviceProvider )
	{
		_contextAccessor = contextAccessor;
		_serviceProvider = serviceProvider;
	}

	public void OnRequestBegin( SlackRequestContext context )
	{
		if ( context.ContainsKey( "Envelope" ) ) // Socket mode
		{
			var scope = _serviceProvider.CreateScope( );
			context.SetServiceProvider( scope.ServiceProvider );
			context.OnComplete( ( ) =>
				{
					scope.Dispose( );
					return Task.CompletedTask;
				} );
		}
		else
		{
			context.SetServiceProvider( _contextAccessor.FunctionContext?.InstanceServices );
		}
	}
}