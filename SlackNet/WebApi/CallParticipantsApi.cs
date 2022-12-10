using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ICallParticipantsApi
{
    /// <summary>
    /// Registers new participants added to a Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.participants.add">Slack documentation</a> for more information.</remarks>
    /// <param name="id"><see cref="Call.Id"/> returned by the <see cref="Add"/> method.</param>
    /// <param name="users">The list of users to add as participants in the Call.</param>
    /// <param name="cancellationToken"></param>
    Task Add(string id, IEnumerable<CallUser> users, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Registers participants removed from a Call.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/calls.participants.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="id"><see cref="Call.Id"/> returned by the <see cref="Add"/> method.</param>
    /// <param name="users">The list of users to remove as participants in the Call.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(string id, IEnumerable<CallUser> users, CancellationToken? cancellationToken = null);
}

public class CallParticipantsApi : ICallParticipantsApi
{
    private readonly ISlackApiClient _client;
    public CallParticipantsApi(ISlackApiClient client) => _client = client;

    public Task Add(string id, IEnumerable<CallUser> users, CancellationToken? cancellationToken = null) =>
        _client.Post("calls.participants.add", new Args
            {
                { "id", id },
                { "users", users }
            }, cancellationToken);

    public Task Remove(string id, IEnumerable<CallUser> users, CancellationToken? cancellationToken = null) =>
        _client.Post("calls.participants.remove", new Args
            {
                { "id", id },
                { "users", users }
            }, cancellationToken);
}