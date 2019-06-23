using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IBlockOptionProvider
    {
        Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request);
    }
}