using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackOptionsService : ISlackOptions
    {
        private readonly ISlackOptions _options = new SlackOptions();

        public SlackOptionsService(IEnumerable<ResolvedOptionProvider> providers)
        {
            foreach (var provider in providers)
                _options.SetProvider(provider.ActionName, provider);
        }

        public Task<OptionsResponse> Handle(OptionsRequest request) => _options.Handle(request);
        public void SetProvider(string actionName, IOptionProvider handler) => _options.SetProvider(actionName, handler);
    }
}