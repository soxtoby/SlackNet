#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.WebApi;

public class AssistantSearchContextResponse
{
    public AssistantSearchContextResults Results { get; set; } = new();

    /// <summary>
    /// The content types that couldn't be searched.
    /// </summary>
    public IList<ContentType> FailedContentTypes { get; set; } = [];

    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

/// <summary>
/// The search results for each of the requested <see cref="ContentType"/>s.
/// </summary>
public class AssistantSearchContextResults
{
    public IList<AssistantSearchMessage> Messages { get; set; } = [];
    public IList<AssistantSearchFile> Files { get; set; } = [];
    public IList<AssistantSearchChannel> Channels { get; set; } = [];
    public IList<AssistantSearchUser> Users { get; set; } = [];
}

/// <summary>
/// A message found by <see cref="IAssistantSearchApi.Context"/>.
/// </summary>
public class AssistantSearchMessage
{
    public required string AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public required string AuthorUserId { get; set; }
    public required string TeamId { get; set; }
    public required string ChannelId { get; set; }
    public required string ChannelName { get; set; }
    public required string MessageTs { get; set; }
    public required string Content { get; set; }
    public bool IsAuthorBot { get; set; }
    public required string Permalink { get; set; }

    /// <summary>
    /// The message's blocks. Only included when requested.
    /// </summary>
    public IList<Block> Blocks { get; set; } = [];

    public int? ReplyCount { get; set; }
}

/// <summary>
/// A file found by <see cref="IAssistantSearchApi.Context"/>.
/// </summary>
public class AssistantSearchFile
{
    public required string AuthorName { get; set; }
    public string? AuthorEmail { get; set; }
    public required string AuthorUserId { get; set; }
    public required string UploaderUserId { get; set; }
    public required string TeamId { get; set; }
    public required string FileId { get; set; }
    public int DateCreated { get; set; }

    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    public int DateUpdated { get; set; }

    [JsonIgnore]
    public DateTime? Updated => DateUpdated.ToDateTime();

    public required string Title { get; set; }
    public required string FileType { get; set; }
    public required string Permalink { get; set; }
    public required string Content { get; set; }
    public int? Size { get; set; }
}

/// <summary>
/// A channel found by <see cref="IAssistantSearchApi.Context"/>.
/// </summary>
public class AssistantSearchChannel
{
    public required string TeamId { get; set; }
    public required string CreatorUserId { get; set; }
    public string? CreatorName { get; set; }
    public int DateCreated { get; set; }

    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();

    public int DateUpdated { get; set; }

    [JsonIgnore]
    public DateTime? Updated => DateUpdated.ToDateTime();

    public required string Name { get; set; }
    public string? Topic { get; set; }
    public string? Purpose { get; set; }
    public required string Permalink { get; set; }
    public bool IsArchived { get; set; }
    public ConversationType ChannelType { get; set; }
}

/// <summary>
/// A user found by <see cref="IAssistantSearchApi.Context"/>.
/// </summary>
public class AssistantSearchUser
{
    public required string UserId { get; set; }
    public required string Permalink { get; set; }
    public string? ProfilePicPermalink { get; set; }
    public required string FullName { get; set; }
    public required string Title { get; set; }
    public required string Timezone { get; set; }
    public required string Email { get; set; }
}
