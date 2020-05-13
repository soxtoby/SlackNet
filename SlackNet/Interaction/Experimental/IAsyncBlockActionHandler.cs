using System;
using System.Threading.Tasks;
using SlackNet.Blocks;

namespace SlackNet.Interaction.Experimental
{
    [Obsolete(Warning.Experimental)]
    public interface IAsyncBlockActionHandler
    {
        Task Handle(BlockActionRequest request, Responder respond);
    }

    [Obsolete(Warning.Experimental)]
    public interface IAsyncBlockActionHandler<in TAction>
        where TAction : BlockAction
    {
        Task Handle(TAction action, BlockActionRequest request, Responder respond);
    }
}