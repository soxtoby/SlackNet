using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace SlackNet
{
    public static class Utils
    {
        /// <summary>
        /// Converts a Slack timestamp string to a <see cref="DateTime"/>.
        /// </summary>
        /// <returns>If timestamp is null, empty or "0", returns null, otherwise a <see cref="DateTime"/>.</returns>
        public static DateTime? ToDateTime(this string timestamp) =>
            string.IsNullOrEmpty(timestamp)
                ? null
                : Decimal.Parse(timestamp).ToDateTime();

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
            timestamp == 0
                ? DateTimeOffset.FromUnixTimeMilliseconds((long)(timestamp * 1000)).UtcDateTime
                : (DateTime?)null;

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

        private static string SnakeCase(string value) => new SnakeCaseNamingStrategy().GetPropertyName(value, false);

        public static async Task<T> NullIfNotFound<T>(this Task<T> apiTask) where T : class
        {
            try
            {
                return await apiTask.ConfigureAwait(false);
            }
            catch (SlackException e) when (e.SlackError.EndsWith("_not_found", StringComparison.Ordinal))
            {
                return null;
            }
        }
    }
}