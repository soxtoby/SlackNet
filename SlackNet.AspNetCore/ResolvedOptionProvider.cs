using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    abstract class ResolvedOptionProvider : IOptionProvider
    {
        protected ResolvedOptionProvider(string actionName) => ActionName = actionName;

        public string ActionName { get; }

        public abstract Task<OptionsResponse> GetOptions(OptionsRequest request);
    }

    class ResolvedOptionProvider<T> : ResolvedOptionProvider
        where T : IOptionProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ResolvedOptionProvider(IServiceProvider serviceProvider, string actionName)
            : base(actionName)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task<OptionsResponse> GetOptions(OptionsRequest request)
        {
            var handler = _serviceProvider.GetRequiredService<T>();
            return handler.GetOptions(request);
        }
    }
}