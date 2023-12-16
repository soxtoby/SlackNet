using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctions;

// Adapted from https://gist.github.com/dolphinspired/796d26ebe1237b78ee04a3bff0620ea0
class FunctionContextServiceProviderMiddleware(FunctionContextServiceProviderAccessor accessor) : IFunctionsWorkerMiddleware
{
    public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        if (accessor.ServiceProvider != null)
        {
            // This should never happen because the context should be localized to the current Task chain.
            // But if it does happen (perhaps the implementation is bugged), then we need to know immediately so it can be fixed.
            throw new InvalidOperationException($"Unable to initialize {nameof(IRequestServiceProviderAccessor)}: service provider has already been initialized.");
        }
        
        accessor.ServiceProvider = context.InstanceServices;
        
        return next(context);
    }
}