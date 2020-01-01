using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet
{
    public class File
    {
        public string Id { get; set; }
        public int Created { get; set; }
        public int Timestamp { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Mimetype { get; set; }
        public string Filetype { get; set; }
        public string PrettyType { get; set; }
        public string User { get; set; }
        public string Mode { get; set; }
        public bool Editable { get; set; }
        public bool IsExternal { get; set; }
        public string ExternalType { get; set; }
        public string Username { get; set; }
        public int Size { get; set; }
        public string UrlPrivate { get; set; }
        public string UrlPrivateDownload { get; set; }
        public string Thumb64 { get; set; }
        public string Thumb80 { get; set; }
        public string Thumb360 { get; set; }
        public string Thumb360Gif { get; set; }
        public int Thumb360W { get; set; }
        public int Thumb360H { get; set; }
        public string Thumb480 { get; set; }
        public int Thumb480W { get; set; }
        public int Thumb480H { get; set; }
        public string Thumb160 { get; set; }
        public string Permalink { get; set; }
        public string PermalinkPublic { get; set; }
        public string EditLink { get; set; }
        public string Preview { get; set; }
        public string PreviewHighlight { get; set; }
        public int Lines { get; set; }
        public int LinesMore { get; set; }
        public bool IsPublic { get; set; }
        public bool PublicUrlShared { get; set; }
        public bool DisplayAsBot { get; set; }
        public string[] Channels { get; set; }
        public string[] Groups { get; set; }
        public string[] Ims { get; set; }
        public FileComment InitialComment { get; set; }
        public int NumStars { get; set; }
        public bool IsStarred { get; set; }
        public string[] PinnedTo { get; set; }
        public FileShares Shares { get; set; }
        public IList<Reaction> Reactions { get; set; } = new List<Reaction>();
        public int CommentsCount { get; set; }
        public string ExternalId { get; set; }
        public string ExternalUrl { get; set; }
        public bool HasRichPreview { get; set; }
    }

    public class FileShares
    {
        public IDictionary<string, FileShare> Public { get; set; }
        public IDictionary<string, FileShare> Private { get; set; }
    }

    public class FileShare
    {
        public string[] ReplyUsers { get; set; }
        public int ReplyUsersCount { get; set; }
        public int ReplyCount { get; set; }
        public string Ts { get; set; }
        public string ThreadTs { get; set; }
        /// <summary>
        /// Id of latest message reply.
        /// </summary>
        public string LatestReply { get; set; }
        public string ChannelName { get; set; }
        public string TeamId { get; set; }
    }

}