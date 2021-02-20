using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class SwitchingOptionProvider : IOptionProvider
    {
        private readonly IHandlerIndex<IOptionProvider> _providers;
        public SwitchingOptionProvider(IHandlerIndex<IOptionProvider> providers) => _providers = providers;

        public Task<OptionsResponse> GetOptions(OptionsRequest request) =>
            _providers.TryGetValue(request.Name, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new OptionsResponse());
    }
}