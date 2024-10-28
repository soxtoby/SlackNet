#nullable enable
using System;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace SlackNet;

public static class Utils
{
    /// <summary>
    /// Converts a Slack timestamp string to a <see cref="DateTime"/>.
    /// </summary>
    /// <returns>If timestamp is null, empty or "0", returns null, otherwise a <see cref="DateTime"/>.</returns>
    public static DateTime? ToDateTime(this string? timestamp) =>
        string.IsNullOrEmpty(timestamp)
            ? null
            : decimal.TryParse(timestamp, out var ts)
                ? ts.ToDateTime()
                : null;

    /// <summary>
    /// Converts a Slack timestamp number to a <see cref="DateTime"/>.
    /// </summary>
    /// <returns>If timestamp is 0, returns null, otherwise a DateTime.</returns>
    public static DateTime? ToDateTime(this int timestamp) => ToDateTime((decimal)timestamp);

    /// <summary>
    /// Converts a Slack timestamp number to a <see cref="DateTime"/>.
    /// </summary>
    /// <returns>If timestamp is 0, returns null, otherwise a DateTime.</returns>
    public static DateTime? ToDateTime(this decimal timestamp) =>
        timestamp != 0
            ? DateTimeOffset.FromUnixTimeMilliseconds((long)(timestamp * 1000))
                .AddTicks((long)(timestamp * 1_000_000 % 1000 * 10)) // 10 ticks per microsecond
                .UtcDateTime
            : null;

    /// <summary>
    /// Converts a <see cref="DateTime"/> to a Slack timestamp.
    /// Do not use this for identifying messages; always use the message's Ts string.
    /// </summary>
    public static string ToTimestamp(this DateTime dateTime) =>
        new DateTimeOffset(dateTime).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a Slack time offset, up to a specified limit.
    /// </summary>
    public static string ToOffset(this TimeSpan timeSpan, TimeSpan limit) =>
        timeSpan <= limit
            ? timeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture)
            : throw new InvalidOperationException($"Offset must be within {limit}.");

    /// <summary>
    /// Returns the Slack type identifier for a given type.
    /// For types that require type and subtype identifiers, this method will return only the subtype;
    /// the type identifier must be retrieved from the base type.
    /// </summary>
    public static string SlackType(this TypeInfo type) =>
        type.GetCustomAttribute<SlackTypeAttribute>()?.TypeIdentifier
        ?? SnakeCase(type.Name);

    /// <summary>
    /// Format a <see cref="DateTime"/> in users' own locales.
    /// See https://api.slack.com/reference/surfaces/formatting#date-formatting for more information.
    /// </summary>
    public static string FormatForMessage(this DateTime dateTime, string formatTokenString, string? fallbackText = null, string? linkUrl = null)
        => $"<!date^{dateTime.ToTimestamp()}^{formatTokenString}{(linkUrl != null ? "^" + linkUrl : string.Empty)}|{fallbackText ?? dateTime.ToString("R")}>";

    private static string SnakeCase(string value) => new SnakeCaseNamingStrategy().GetPropertyName(value, false);

    public static async Task<T?> NullIfNotFound<T>(this Task<T> apiTask) where T : class
    {
        try
        {
            return await apiTask.ConfigureAwait(false);
        }
        catch (SlackException e) when (e.ErrorCode.EndsWith("_not_found", StringComparison.Ordinal))
        {
            return null;
        }
    }

    /// <summary>
    /// On error, resubscribes to <paramref name="source"/> after a delay,
    /// starting with <paramref name="initialDelay"/> and then increasing by
    /// <paramref name="delayIncrease"/> each time, up to <paramref name="maxDelay"/>.
    /// Resets back to <paramref name="initialDelay"/> on next value.
    /// </summary>
    [Obsolete("Will be removed in a future version.")]
    public static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, TimeSpan initialDelay, TimeSpan delayIncrease, TimeSpan maxDelay, IScheduler? scheduler = null, Action<Exception, TimeSpan>? onError = null)
    {
        return source.RetryWithDelay(_ => true, initialDelay, delayIncrease, maxDelay, scheduler, onError);
    }

    /// <summary>
    /// On error, resubscribes to <paramref name="source"/> after a delay,
    /// starting with <paramref name="initialDelay"/> and then increasing by
    /// <paramref name="delayIncrease"/> each time, up to <paramref name="maxDelay"/>.
    /// Resets back to <paramref name="initialDelay"/> on next value.
    /// </summary>
    internal static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, Func<Exception, bool> shouldRetry, TimeSpan initialDelay, TimeSpan delayIncrease, TimeSpan maxDelay, IScheduler? scheduler = null, Action<Exception, TimeSpan>? onError = null)
    {
        var currentDelay = initialDelay;
        return source
            .Do(_ => currentDelay = initialDelay)
            .RetryWhen(es => es.SelectMany(e =>
                {
                    if (shouldRetry(e))
                    {
                        onError?.Invoke(e, currentDelay);
                        var delay = Observable.Timer(currentDelay, scheduler ?? Scheduler.Default);
                        currentDelay += delayIncrease;
                        if (currentDelay > maxDelay)
                            currentDelay = maxDelay;
                        return delay;
                    }
                    return Observable.Throw<long>(e);
                }));
    }

    internal static IObservable<T> WhereNotNull<T>(this IObservable<T?> source) => source.Where(v => v is not null)!;
}