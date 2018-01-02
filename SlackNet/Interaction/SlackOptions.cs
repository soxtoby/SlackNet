using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackOptions
    {
        Task<OptionsResponse> Handle(OptionsRequest request);
        void SetProvider(string actionName, IOptionProvider handler);
    }

    public class SlackOptions : ISlackOptions
    {
        private readonly Dictionary<string, IOptionProvider> _providers = new Dictionary<string, IOptionProvider>();

        public Task<OptionsResponse> Handle(OptionsRequest request) =>
            _providers.TryGetValue(request.Name, out var provider)
                ? provider.GetOptions(request)
                : Task.FromResult(new OptionsResponse());

        public void SetProvider(string actionName, IOptionProvider provider) => _providers[actionName] = provider;
    }
}