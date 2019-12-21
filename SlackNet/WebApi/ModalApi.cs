using System.Threading;
using System.Threading.Tasks;
using SlackNet.Interaction;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IModalApi
    {
        /// <summary>
        /// Open a modal with a user.
        /// </summary>
        /// <param name="triggerId">Exchange a trigger to post to the user.</param>
        /// <param name="modal">The modal definition.</param>
        /// <param name="cancellationToken"></param>
        Task Open(string triggerId, Modal modal, CancellationToken? cancellationToken = null);
    }

    public class ModalApi : IModalApi
    {
        private readonly ISlackApiClient _client;
        public ModalApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Open a modal with a user.
        /// </summary>
        /// <param name="triggerId">Exchange a trigger to post to the user.</param>
        /// <param name="modal">The modal definition.</param>
        /// <param name="cancellationToken"></param>
        public Task Open(string triggerId, Modal modal, CancellationToken? cancellationToken = null) =>
            _client.Post("views.open", new Args
                {
                    { "view", modal },
                    { "trigger_id", triggerId }
                }, cancellationToken);
    }
}