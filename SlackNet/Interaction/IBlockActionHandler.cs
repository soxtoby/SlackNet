using System.Threading.Tasks;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public interface IBlockActionHandler { }

    public interface IBlockActionHandler<in TAction> : IBlockActionHandler 
        where TAction : BlockAction
    {
        Task Handle(TAction action, BlockActionRequest request);
    }
}