using System;
using System.Threading;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctions;

// Adapted from https://gist.github.com/dolphinspired/796d26ebe1237b78ee04a3bff0620ea0
class FunctionContextServiceProviderAccessor : IRequestServiceProviderAccessor
{
    private static readonly AsyncLocal<ServiceProviderRedirect> CurrentServiceProvider = new();

    public IServiceProvider? ServiceProvider
    {
        get => CurrentServiceProvider.Value?.Provider;
        set
        {
            var holder = CurrentServiceProvider.Value;
            if (holder != null)
            {
                // Clear current context trapped in the AsyncLocals, as its done.
                holder.Provider = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the context in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when it's cleared.
                CurrentServiceProvider.Value = new ServiceProviderRedirect(value);
            }
        }
    }

    class ServiceProviderRedirect(IServiceProvider provider)
    {
        public IServiceProvider? Provider { get; set; } = provider;
    }
}