using System;
using System.Collections.Generic;
using System.Linq;
using SlackNet.WebApi;

namespace SlackNet;

public class SlackException : Exception
{
    public string ErrorCode { get; }
    public IReadOnlyList<string> ErrorMessages { get; }

    public SlackException(ErrorResponse errorResponse)
        : base(errorResponse == null
            ? "Slack returned an unknown error response type"
            : string.Join(Environment.NewLine,
                new[] { $"Slack returned an error response: {errorResponse.Error}." }
                    .Concat(errorResponse.ResponseMetadata?.Messages ?? [])))
    {
        ErrorCode = errorResponse?.Error ?? "unknown";
        ErrorMessages = errorResponse?.ResponseMetadata?.Messages.ToList() ?? [];
    }
}