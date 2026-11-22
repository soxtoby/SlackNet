#nullable disable
namespace SlackNet.WebApi;

record ConversationPrefsResponse
{
    public ConversationPrefs Prefs { get; init; } = new();
}
