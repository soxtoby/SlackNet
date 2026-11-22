using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// A group of settings corresponding to the <b>Features</b> section of an app's configuration pages.
/// </summary>
public class Features
{
    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/surfaces/app-home/">App Home</a> configuration.
    /// </summary>
    public AppHome? AppHome { get; set; }

    /// <summary>
    /// Settings related to the agent view for <a href="https://docs.slack.dev/ai/agents/">apps using AI features</a>.
    /// Powers the Agent messaging experience, in which agent conversations happen in the app's Messages tab,
    /// with threads shown in a timeline above the composer.
    /// </summary>
    public AgentView? AgentView { get; set; }

    /// <summary>
    /// Settings related to assistant view for <a href="https://docs.slack.dev/ai/">apps using AI features</a>.
    /// Powers the older Assistant messaging experience, which uses separate Chat and History tabs.
    /// New apps can only use <see cref="AgentView"/>.
    /// </summary>
    public AssistantView? AssistantView { get; set; }

    /// <summary>
    /// Settings that describe bot user configuration.
    /// </summary>
    public BotUser? BotUser { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/ai/slack-code/">code channels</a> configuration.
    /// </summary>
    public CodeChannels? CodeChannels { get; set; }

    /// <summary>
    /// Settings that describe rich previews configuration.
    /// </summary>
    public RichPreviews? RichPreviews { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/enterprise-search/">enterprise search</a> configuration.
    /// </summary>
    public Search? Search { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/interactivity/implementing-shortcuts/#shortcut-types">shortcuts</a> configuration.
    /// </summary>
    public IList<Shortcut>? Shortcuts { get; set; }

    /// <summary>
    /// Settings that describe <a href="https://docs.slack.dev/interactivity/implementing-slash-commands/">slash commands</a> configuration.
    /// </summary>
    public IList<SlashCommand>? SlashCommands { get; set; }

    /// <summary>
    /// Valid <a href="https://docs.slack.dev/messaging/unfurling-links-in-messages/#configuring_domains">unfurl domains</a> to register.
    /// </summary>
    public IList<string>? UnfurlDomains { get; set; }

    /// <summary>
    /// <b>Legacy feature</b>: settings that describe workflow steps configuration. This feature has been deprecated.
    /// </summary>
    public IList<WorkflowStep>? WorkflowSteps { get; set; }
}
