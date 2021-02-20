using System.Threading.Tasks;
using SlackNet.Handlers;

namespace SlackNet
{
    public interface ISlackRequestListener
    {
        Task OnRequestBegin(SlackRequestContext context);
        Task OnRequestEnd(SlackRequestContext context);
    }

    class NullRequestListener : ISlackRequestListener
    {
        private NullRequestListener() { }
        public static ISlackRequestListener Instance = new NullRequestListener();

        public Task OnRequestBegin(SlackRequestContext context) => Task.CompletedTask;
        public Task OnRequestEnd(SlackRequestContext context) => Task.CompletedTask;
    }
}