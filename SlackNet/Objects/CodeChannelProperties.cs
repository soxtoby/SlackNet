using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SlackNet;

/// <summary>
/// Code-specific state for a code channel.
/// Only the properties that are set are sent, so leave a property <c>null</c> to keep its current value.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/ai/slack-code/code-channel-properties/">Slack documentation</a> for more information.</remarks>
public class CodeChannelProperties
{
    /// <summary>
    /// Git hosting provider (e.g. github.com).
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Repository name (e.g. org/repo).
    /// </summary>
    public string? Repo { get; set; }

    /// <summary>
    /// Working branch name.
    /// </summary>
    public string? Branch { get; set; }

    /// <summary>
    /// Base branch the working branch was created from.
    /// </summary>
    public string? BaseBranch { get; set; }

    /// <summary>
    /// Current commit SHA.
    /// </summary>
    public string? CommitSha { get; set; }

    /// <summary>
    /// Pull request number.
    /// </summary>
    public int? PrNumber { get; set; }

    /// <summary>
    /// URL of the pull request.
    /// </summary>
    public string? PrUrl { get; set; }

    /// <summary>
    /// Title of the pull request.
    /// </summary>
    public string? PrTitle { get; set; }

    /// <summary>
    /// The pull request status displayed for the channel as a whole.
    /// </summary>
    public PullRequestStatus? PrStatus { get; set; }

    /// <summary>
    /// URL to the CI pipeline run.
    /// </summary>
    public string? CiUrl { get; set; }

    /// <summary>
    /// State of the CI pipeline (e.g. pending, success, failure).
    /// </summary>
    public string? CiState { get; set; }

    /// <summary>
    /// List of file paths relevant to this session.
    /// </summary>
    public IList<string>? FilePaths { get; set; }

    /// <summary>
    /// Primary programming language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Upstream repository URL.
    /// </summary>
    public string? UpstreamUrl { get; set; }

    /// <summary>
    /// URL of the working branch.
    /// </summary>
    public string? BranchUrl { get; set; }

    /// <summary>
    /// Items to display in the channel context bar, such as the repo, branch, PR, or CI status.
    /// The list you send replaces the current set, so to add or update a single item, resend all the items you want to keep.
    /// </summary>
    public IList<ContextBarItem>? ContextBarItems { get; set; }

    /// <summary>
    /// Points to the current summary message for this code channel. Must reference a message within this channel.
    /// When the channel is archived with a summary message, this property is also set to record where the summary lives so clients can fetch it.
    /// </summary>
    public SummaryMessage? SummaryMessage { get; set; }
}

/// <summary>
/// The pull request status displayed for a code channel as a whole.
/// </summary>
public enum PullRequestStatus
{
    /// <summary>
    /// The pull request is a draft. No status is displayed.
    /// </summary>
    [EnumMember(Value = "")] Draft,

    /// <summary>
    /// The pull request is ready for review.
    /// </summary>
    Open,

    /// <summary>
    /// A reviewer requested changes, checks are failing, or the pull request has merge conflicts.
    /// </summary>
    Blocked,

    /// <summary>
    /// The pull request is merged.
    /// </summary>
    Merged,

    /// <summary>
    /// The pull request is closed without being merged.
    /// </summary>
    Closed
}

/// <summary>
/// Records which message in a code channel represents the current session summary.
/// </summary>
public class SummaryMessage
{
    /// <summary>
    /// Timestamp of the summary message in the code channel.
    /// </summary>
    public required string MessageTs { get; set; }

    /// <summary>
    /// Thread timestamp if the summary message is a thread reply.
    /// Clients need this to fetch the message via <see cref="WebApi.IConversationsApi.Replies"/>.
    /// </summary>
    public string? ThreadTs { get; set; }
}
