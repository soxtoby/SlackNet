#nullable enable
namespace SlackNet
{
    public enum LogCategory
    {
        /// <summary>
        /// Everything sent and received to and from Slack.
        /// </summary>
        Data,

        /// <summary>
        /// Low-level logging from the serializer.
        /// </summary>
        Serialization,

        /// <summary>
        /// Low-level logging of internal operations.
        /// </summary>
        Internal,

        /// <summary>
        /// High-level logging for request handling.
        /// </summary>
        Request,

        /// <summary>
        /// Error logging.
        /// </summary>
        Error
    }
}