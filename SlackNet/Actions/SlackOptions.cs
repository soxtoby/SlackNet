using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet
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

    public interface IOptionProvider
    {
        Task<OptionsResponse> GetOptions(OptionsRequest request);
    }

    public class OptionsResponse
    {
        public IList<Option> Options { get; set; }
        public IList<OptionGroup> OptionGroups { get; set; }
    }
}