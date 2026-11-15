#nullable enable

namespace SlackNet.WebApi;

/// <summary>
/// An issue found in a request, such as an invalid element in a Block Kit payload.
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// The error code (e.g. failed_constraint).
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// A human-readable description of the issue.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// A JSON pointer path to the invalid element (e.g. /0/text/type).
    /// </summary>
    public string? Pointer { get; set; }

    /// <summary>
    /// Details about what was expected.
    /// </summary>
    public JsonData? Constraint { get; set; }
}
