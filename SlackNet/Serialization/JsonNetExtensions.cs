using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace SlackNet;

/// <summary>
/// Converts between <see cref="JsonData"/> and Newtonsoft.Json's <see cref="JToken"/>.
/// </summary>
public static class JsonNetExtensions
{
    /// <summary>
    /// Wraps a <see cref="JToken"/> in a <see cref="JsonData"/>. Changes to the token change the <see cref="JsonData"/>.
    /// </summary>
    [return: NotNullIfNotNull(nameof(token))]
    public static JsonData? ToJsonData(this JToken? token) =>
        token is null ? null : new(token, null);

    /// <summary>
    /// The <see cref="JToken"/> underlying a <see cref="JsonData"/>. Changes to the token change the <see cref="JsonData"/>.
    /// </summary>
    [return: NotNullIfNotNull(nameof(data))]
    public static JToken? ToJToken(this JsonData? data) => data?.Token;
}
