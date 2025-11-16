using System;
using Newtonsoft.Json;

namespace SlackNet;

[SlackType("slack#/types/timestamp")]
public class TimestampField() : EntityField("slack#/types/timestamp")
{
    public long Value { get; set; }
    
    [JsonIgnore]
    public DateTime? DateTime
    {
        get => Value.ToDateTime();
        set => Value = value?.ToTimestampNumber() ?? 0;
    }
    
    /// <summary>
    /// The field's content will be hyperlinked with the URL specified here.
    /// </summary>
    public string Link { get; set; }
}