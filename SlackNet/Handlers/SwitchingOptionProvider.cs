using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers;

public class SwitchingOptionProvider : IOptionProvider, IComposedHandler<OptionsRequest>
{
    private readonly IHandlerIndex<IOptionProvider> _providers;
    public SwitchingOptionProvider(IHandlerIndex<IOptionProvider> providers) => _providers = providers;

    public Task<OptionsResponse> GetOptions(OptionsRequest request) =>
        _providers.TryGetHandler(request.Name, out var provider)
            ? provider.GetOptions(request)
            : Task.FromResult(new OptionsResponse());

    IEnumerable<object> IComposedHandler<OptionsRequest>.InnerHandlers(OptionsRequest request) =>
        _providers.TryGetHandler(request.Name, out var provider)
            ? provider.InnerHandlers(request)
            : Enumerable.Empty<object>();
}