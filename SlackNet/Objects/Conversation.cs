using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Events;

namespace SlackNet
{
    public class Conversation
    {
        public string Id { get; set; }

        /// <summary>
        /// The name of the channel-like thing, without a leading hash sign. Don't get too attached to that name. It might change.
        /// Don't bother storing it even. When thinking about channel-like things, think about their IDs and their type and the team/workspace they belong to.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether a conversation is a public channel. Everything said in a public channel can be read by anyone else belonging to a workspace.
        /// <see cref="IsPrivate"/> will be False. Check both just to be sure, why not?
        /// </summary>
        public bool IsChannel { get; set; }

        /// <summary>
        /// Means the channel is a private channel. <see cref="IsPrivate"/> will also be true. Check yourself before you wreck yourself.
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Means the conversation is a direct message between two distinguished individuals or a user and a bot. Yes, it's an <see cref="IsPrivate"/> conversation.
        /// </summary>
        public bool IsIm { get; set; }

        /// <summary>
        /// Unix timestamp of when the conversation was created.
        /// </summary>
        public int Created { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate => Created.ToDateTime().GetValueOrDefault();

        /// <summary>
        /// The user ID of the member that created this channel.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Indicates the "host" of a shared channel. The value may contain a workspace's ID (beginning with T) or an enterprise grid organization's ID (beginning with E).
        /// </summary>
        public string ConversationHostId { get; set; }
        
        /// <summary>
        /// Indicates a conversation is archived. Frozen in time.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Means the channel is the workspace's "general" discussion channel (even if it's not named #general but it might be anyway).
        /// That might be important to your app because almost every user is a member.
        /// </summary>
        public bool IsGeneral { get; set; }

        public int Unlinked { get; set; }

        public string NameNormalized { get; set; }

        /// <summary>
        /// Means the conversation is in some way shared between multiple workspaces. Look for <see cref="IsExtShared"/> and <see cref="IsOrgShared"/>
        /// to learn which kind it is, and if that matters, act accordingly. Have we mentioned how great <see cref="IsPrivate"/> is yet?
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Represents this conversation as being part of a Shared Channel with a remote organization.
        /// Your app should make sure the data it shares in such a channel is appropriate for both workspaces.
        /// <see cref="IsShared"/> will also be True.
        /// </summary>
        public bool IsExtShared { get; set; }

        /// <summary>
        /// Explains whether this shared channel is shared between Enterprise Grid workspaces within the same organization.
        /// It's a little different from (externally) Shared Channels. Yet, <see cref="IsShared"/> will be True.
        /// </summary>
        public bool IsOrgShared { get; set; }

        public object[] PendingShared { get; set; }

        /// <summary>
        /// Is intriguing. It means the conversation is ready to become an <see cref="IsExtShared"/> channel but isn't quite ready yet
        /// and needs some kind of approval or sign off. Best to treat it as if it were a shared channel, even if it traverses only one workspace.
        /// </summary>
        public bool IsPendingExtShared { get; set; }

        /// <summary>
        /// Indicates the user or bot user or Slack app associated with the token making the API call is itself a member of the conversation.
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// Means the conversation is privileged between two or more members. Meet their privacy expectations.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Represents an unnamed private conversation between multiple users. It's an <see cref="IsPrivate"/> kind of thing.
        /// </summary>
        public bool IsMpim { get; set; }

        /// <summary>
        /// The timestamp for the last message the calling user has read in this channel.
        /// </summary>
        public string LastRead { get; set; }

        /// <summary>
        /// The latest message in the channel.
        /// </summary>
        public MessageEvent Latest { get; set; }

        /// <summary>
        /// Full count of visible messages that the calling user has yet to read.
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// Count of messages that the calling user has yet to read that matter to them (this means it excludes things like join/leave messages).
        /// </summary>
        public int UnreadCountDisplay { get; set; }

        public Topic Topic { get; set; }

        public Purpose Purpose { get; set; }

        public string[] PreviousNames { get; set; }

        public int Priority { get; set; }

        /// <summary>
        /// Means the conversation can't be written to by typical users. Admins may have the ability.
        /// </summary>
        public bool IsReadOnly { get; set; }

        public int NumMembers { get; set; }

        public string Locale { get; set; }

        /// <summary>
        /// A list of user IDs for all users in this channel. This includes any disabled accounts that were in this channel when they were disabled.
        /// </summary>
        public IList<string> Members { get; set; } = new List<string>();

        /// <summary>
        /// The other user in an IM.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Has the other user in an IM been deleted.
        /// </summary>
        public bool IsUserDeleted { get; set; }

        public override string ToString() => IsIm
            ? Link.User(User).ToString()
            : Link.Conversation(Id).ToString();
    }
}