using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IAppsEventsAuthorizationsApi
    {
        /// <summary>
        /// Get a list of authorizations for the given event context. Should always be used with app token type.
        /// </summary>
        /// <param name="eventContext">The <see cref="EventCallback.EventContext"/> from an event.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to the <see cref="AppsEventsAuthorizationsListResponse.CursorNext"/> property
        /// returned by a previous request. Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<AppsEventsAuthorizationsListResponse> List(
            string eventContext,
            int limit = 100,
            string cursor = null,
            CancellationToken? cancellationToken = null);
    }

    public class AppsEventsAuthorizationsApi : IAppsEventsAuthorizationsApi
    {
        private readonly ISlackApiClient _client;
        public AppsEventsAuthorizationsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Get a list of authorizations for the given event context. Should always be used with app token type.
        /// </summary>
        /// <param name="eventContext">The <see cref="EventCallback.EventContext"/> from an event.</param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to the <see cref="AppsEventsAuthorizationsListResponse.CursorNext"/> property
        /// returned by a previous request. Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<AppsEventsAuthorizationsListResponse> List(
            string eventContext,
            int limit = 100,
            string cursor = null,
            CancellationToken? cancellationToken = null
        ) =>
            _client.Post<AppsEventsAuthorizationsListResponse>("apps.events.authorizations.list", new Args
                    {
                        { "event_context", eventContext },
                        { "cursor", cursor },
                        { "limit", limit }
                    },
                cancellationToken);
    }
}