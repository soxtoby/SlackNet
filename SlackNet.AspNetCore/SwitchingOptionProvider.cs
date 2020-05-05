using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingOptionProvider : IOptionProvider
    {
        private readonly Dictionary<string, IOptionProvider> _providers;
        public SwitchingOptionProvider(IEnumerable<KeyedItem<IOptionProvider>> providers) => 
            _providers = providers.ToDictionary(p => p.Key, p => p.Item);

        public Task<OptionsResponse> GetOptions(OptionsRequest request) =>
            _providers.TryGetValue(request.Name, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new OptionsResponse());
    }
}