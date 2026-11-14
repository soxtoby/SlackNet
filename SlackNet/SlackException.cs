using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.WebApi;

namespace SlackNet;

public class SlackException(ErrorResponse errorResponse)
    : Exception(errorResponse == null
        ? "Slack returned an unknown error response type"
        : string.Join(Environment.NewLine,
            new[] { $"Slack returned an error response: {errorResponse.Error}." }
                .Concat(errorResponse.ResponseMetadata?.Messages ?? [])
                .Concat(errorResponse.Fields.Where(f => !IsShownAsErrorDetails(f)).Select(f => $"{f.Key}: {FieldText(f.Value)}"))
                .Concat(errorResponse.Errors.Select(e => e.Pointer is null ? e.Message : $"{e.Pointer}: {e.Message}"))))
{
    private readonly ErrorResponse _errorResponse = errorResponse;

    public string ErrorCode { get; } = errorResponse?.Error ?? "unknown";
    public IReadOnlyList<string> ErrorMessages { get; } = errorResponse?.ResponseMetadata?.Messages.ToList() ?? [];
    public IReadOnlyDictionary<string, string> ErrorInfo { get; } = errorResponse?.Info ?? new Dictionary<string, string>();

    /// <summary>
    /// Issues found in the request, such as invalid elements in a Block Kit payload.
    /// </summary>
    public IReadOnlyList<ErrorDetail> ErrorDetails { get; } = errorResponse?.Errors.ToList() ?? [];

    /// <summary>
    /// Fields specific to the error, as returned by Slack, such as the items in a request that Slack couldn't process.
    /// </summary>
    public IReadOnlyDictionary<string, JToken> ErrorFields { get; } = errorResponse?.Fields ?? new Dictionary<string, JToken>();

    /// <summary>
    /// Reads one of the <see cref="ErrorFields"/> using SlackNet's JSON conventions.
    /// </summary>
    /// <param name="fieldName">Name of the field, as returned by Slack.</param>
    /// <returns>The field's value, or the default value if Slack didn't return the field.</returns>
    public T ErrorField<T>(string fieldName) =>
        _errorResponse is null
            ? default
            : _errorResponse.Field<T>(fieldName);

    private static bool IsShownAsErrorDetails(KeyValuePair<string, JToken> field) =>
        field is { Key: "errors", Value: JArray { Count: > 0 } errors }
        && errors.All(ErrorResponse.IsErrorDetail);

    private static string FieldText(JToken value) =>
        value.Type == JTokenType.String
            ? value.ToObject<string>()
            : value.ToString(Formatting.None);
}