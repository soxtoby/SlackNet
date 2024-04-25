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
                .Concat(errorResponse.ResponseMetadata?.Messages ?? [])))
{
    public string ErrorCode { get; } = errorResponse?.Error ?? "unknown";
    public IReadOnlyList<string> ErrorMessages { get; } = errorResponse?.ResponseMetadata?.Messages.ToList() ?? [];
}