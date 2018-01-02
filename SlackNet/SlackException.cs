using System;
using System.Collections.Generic;
using System.Linq;
using SlackNet.WebApi;

namespace SlackNet
{
    public class SlackException : Exception
    {
        public string ErrorCode { get; }
        public IReadOnlyList<string> ErrorMessages { get; }

        public SlackException(ErrorResponse errorResponse) 
            : base($"Slack returned an error response: {errorResponse.Error}.")
        {
            ErrorCode = errorResponse.Error;
            ErrorMessages = errorResponse.ResponseMetadata.Messages.ToList();
        }
    }
}