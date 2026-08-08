using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A block that displays content in a card layout.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/card-block/">Slack documentation</a> for more information.</remarks>
[SlackType("card")]
public class CardBlock() : Block("card")
{
    /// <summary>
    /// The image used at the top of the card.
    /// </summary>
    public Image HeroImage { get; set; }

    /// <summary>
    /// The small image used next to the card's title and subtitle.
    /// </summary>
    public Image Icon { get; set; }

    /// <summary>
    /// A Slack icon rendered next to the card's title and subtitle.
    /// </summary>
    public SlackIcon SlackIcon { get; set; }

    /// <summary>
    /// The title of the card, using plain-text or mrkdwn formatting.
    /// </summary>
    public TextObject Title { get; set; }

    /// <summary>
    /// The subtitle of the card, using plain-text or mrkdwn formatting.
    /// </summary>
    public TextObject Subtitle { get; set; }

    /// <summary>
    /// The content of the card, using plain-text or mrkdwn formatting.
    /// </summary>
    public TextObject Body { get; set; }

    /// <summary>
    /// The subtext rendered below the card's body, using plain-text or mrkdwn formatting.
    /// </summary>
    public TextObject Subtext { get; set; }

    /// <summary>
    /// The action buttons shown at the bottom of the card.
    /// Buttons with <see cref="ButtonStyle.Danger"/> are left-aligned. Buttons with <see cref="ButtonStyle.Primary"/>
    /// or <see cref="ButtonStyle.Default"/> are right-aligned, with primary buttons furthest to the right.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<Button> Actions { get; set; } = [];
}
