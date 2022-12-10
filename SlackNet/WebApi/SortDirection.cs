using System.Runtime.Serialization;

namespace SlackNet.WebApi;

public enum SortDirection
{
    [EnumMember(Value = "asc")] Ascending,
    [EnumMember(Value = "desc")] Descending
}