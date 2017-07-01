using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class ImListResponse
    {
        public List<Im> Ims { get; } = new List<Im>();
    }
}