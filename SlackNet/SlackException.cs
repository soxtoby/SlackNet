using System;
using System.Collections.Generic;
using System.Linq;
using SlackNet.WebApi;

namespace SlackNet;

public class SlackException(ErrorResponse errorResponse)
    : Exception(errorResponse == null
        ? "Slack returned an unknown error response type"
        : string.Join(Environment.NewLine,
            new[] { $"Slack returned an error response: {errorResponse.Error}." }
                .Concat(errorResponse.ResponseMetadata.Messages ?? [])
                .Concat(errorResponse.FieldDescriptions)
                .Concat(errorResponse.Errors.Select(e => e.Pointer is null ? e.Message : $"{e.Pointer}: {e.Message}"))))
{
    public string ErrorCode { get; } = errorResponse?.Error ?? "unknown";
    public IReadOnlyList<string> ErrorMessages { get; } = errorResponse?.ResponseMetadata.Messages.ToList() ?? [];
    public IReadOnlyDictionary<string, string> ErrorInfo { get; } = errorResponse?.Info ?? new Dictionary<string, string>();

    /// <summary>
    /// Issues found in the request, such as invalid elements in a Block Kit payload.
    /// </summary>
    public IReadOnlyList<ErrorDetail> ErrorDetails { get; } = errorResponse?.Errors.ToList() ?? [];

    /// <summary>
    /// Reads a field specific to the error, as returned by Slack, such as the items in a request that Slack couldn't process.
    /// </summary>
    /// <param name="fieldName">Name of the field, as returned by Slack.</param>
    /// <returns>The field's value, or the default value if Slack didn't return the field.</returns>
    public T ErrorField<T>(string fieldName) =>
        errorResponse is null
            ? default
            : errorResponse.Field<T>(fieldName);
}