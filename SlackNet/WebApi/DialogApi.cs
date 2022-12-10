using System.Threading;
using System.Threading.Tasks;
using SlackNet.Interaction;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IDialogApi
{
    /// <summary>
    /// Open a dialog with a user.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/dialog.open">Slack documentation</a> for more information.</remarks>
    /// <param name="triggerId">Exchange a trigger to post to the user.</param>
    /// <param name="dialog">The dialog definition.</param>
    /// <param name="cancellationToken"></param>
    Task Open(string triggerId, Dialog dialog, CancellationToken? cancellationToken = null);
}

public class DialogApi : IDialogApi
{
    private readonly ISlackApiClient _client;
    public DialogApi(ISlackApiClient client) => _client = client;

    public Task Open(string triggerId, Dialog dialog, CancellationToken? cancellationToken = null) =>
        _client.Post("dialog.open", new Args
            {
                { "dialog", dialog },
                { "trigger_id", triggerId }
            }, cancellationToken);
}