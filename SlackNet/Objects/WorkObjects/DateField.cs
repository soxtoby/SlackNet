using System;

namespace SlackNet;

[SlackType("slack#/types/date")]
public class DateField() : EntityField("slack#/types/date")
{
    public DateTime Value { get; set; }
    
    /// <summary>
    /// The field's content will be hyperlinked with the URL specified here.
    /// </summary>
    public string Link { get; set; }
}