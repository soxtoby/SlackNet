using System.Threading.Tasks;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public interface IBlockActionHandler
    {
        Task Handle(BlockActionRequest request);
    }

    public interface IBlockActionHandler<in TAction>
        where TAction : BlockAction
    {
        Task Handle(TAction action, BlockActionRequest request);
    }
}