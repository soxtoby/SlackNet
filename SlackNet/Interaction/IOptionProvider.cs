using System.Threading.Tasks;

namespace SlackNet.Interaction;

public interface IOptionProvider
{
    Task<OptionsResponse> GetOptions(OptionsRequest request);
}