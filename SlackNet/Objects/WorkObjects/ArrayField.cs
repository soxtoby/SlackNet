using System.Collections.Generic;

namespace SlackNet;

[SlackType("array")]
[SlackSubTypeProperty(nameof(ItemType))]
public class ArrayField<TItem>(string itemType) : EntityField("array") where TItem : EntityField
{
    public string ItemType { get; set; } = itemType;
    public IList<TItem> Value { get; set; } = [];
}

[SlackType("string")]
public class StringArray() : ArrayField<StringField>("string");

[SlackType("integer")]
public class IntegerArray() : ArrayField<IntegerField>("integer");

[SlackType("slack#/types/channel_id")]
public class ChannelIdArray() : ArrayField<ChannelIdField>("slack#/types/channel_id");

[SlackType("slack#/types/user")]
public class UserArray() : ArrayField<UserField>("slack#/types/user");