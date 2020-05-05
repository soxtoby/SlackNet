using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedOptionProvider : ResolvedHandler<IOptionProvider>, IOptionProvider
    {
        public ResolvedOptionProvider(IServiceProvider serviceProvider, Func<IServiceProvider, IOptionProvider> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task<OptionsResponse> GetOptions(OptionsRequest request) => ResolvedHandle(h => h.GetOptions(request));
    }
}