using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IUsersApi
    {
        /// <summary>
        /// Allows the user to delete their profile image. It will clear whatever image is currently set.
        /// To upload a new profile image, use the companion method <see cref="SetPhoto"/>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task DeletePhoto(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lets you find out information about a user's presence.
        /// </summary>
        /// <param name="userId">User to get presence info on. Defaults to the authed user.</param>
        /// <param name="cancellationToken"></param>
        Task<Presence> GetPresence(string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// After your Slack app is awarded an identity token through Sign in with Slack, use this method to retrieve a user's identity.
        /// The returned fields depend on any additional authorization scopes you've requested.
        /// This method may only be used by tokens with the identity.basic scope, as provided in the Sign in with Slack process.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<IdentityResponse> Identity(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns information about a team member.
        /// </summary>
        /// <param name="userId">User to get info on.</param>
        /// <param name="cancellationToken"></param>
        Task<User> Info(string userId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all users in the team.
        /// This includes deleted/deactivated users.
        /// </summary>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="UserListResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cancellationToken"></param>
        Task<UserListResponse> List(string cursor = null, int limit = 0, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Find a user with an email address.
        /// </summary>
        /// <param name="email">An email address belonging to a user in the workspace.</param>
        /// <param name="cancellationToken"></param>
        Task<User> LookupByEmail(string email, CancellationToken? cancellationToken = null);

        /// <summary>
        /// This method allows the user to set their profile image.
        /// Providing a "crop box" with <see cref="cropX"/>, <see cref="cropY"/>, and <see cref="cropW"/> is optional. Otherwise, the whole image will be used.
        /// If cropping instructions are not specified and the source image is not square, the image will be letterboxed, just like your favorite old laserdiscs.
        /// Please limit your images to a maximum size of 1024 by 1024 pixels. 512x512 pixels is the minimum.
        /// To remove a profile image, use the companion method <see cref="UsersApi.DeletePhoto"/>.
        /// </summary>
        /// <param name="imageContent">Image file contents.</param>
        /// <param name="contentType">MIME type of image file, such as image/gif, image/jpeg, image/png, etc.</param>
        /// <param name="fileName">Image file name.</param>
        /// <param name="cropW">Width/height of crop box(always square).</param>
        /// <param name="cropX">X coordinate of top-left corner of crop box.</param>
        /// <param name="cropY">Y coordinate of top-left corner of crop box.</param>
        /// <param name="cancellationToken"></param>
        Task SetPhoto(byte[] imageContent, string contentType, string fileName = "photo", int? cropW = null, int? cropX = null, int? cropY = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// This method allows the user to set their profile image.
        /// Providing a "crop box" with <see cref="cropX"/>, <see cref="cropY"/>, and <see cref="cropW"/> is optional. Otherwise, the whole image will be used.
        /// If cropping instructions are not specified and the source image is not square, the image will be letterboxed, just like your favorite old laserdiscs.
        /// Please limit your images to a maximum size of 1024 by 1024 pixels. 512x512 pixels is the minimum.
        /// To remove a profile image, use the companion method <see cref="UsersApi.DeletePhoto"/>.
        /// </summary>
        /// <param name="image">Image file.</param>
        /// <param name="contentType">MIME type of image file, such as image/gif, image/jpeg, image/png, etc.</param>
        /// <param name="fileName">Image file name.</param>
        /// <param name="cropW">Width/height of crop box(always square).</param>
        /// <param name="cropX">X coordinate of top-left corner of crop box.</param>
        /// <param name="cropY">Y coordinate of top-left corner of crop box.</param>
        /// <param name="cancellationToken"></param>
        Task SetPhoto(Stream image, string contentType, string fileName = "photo", int? cropW = null, int? cropX = null, int? cropY = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lets you set the calling user's manual presence.
        /// </summary>
        /// <param name="presence">User's presence.</param>
        /// <param name="cancellationToken"></param>
        Task SetPresence(Presence presence, CancellationToken? cancellationToken = null);
    }

    public class UsersApi : IUsersApi
    {
        private readonly ISlackApiClient _client;
        public UsersApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Allows the user to delete their profile image. It will clear whatever image is currently set.
        /// To upload a new profile image, use the companion method <see cref="SetPhoto"/>.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task DeletePhoto(CancellationToken? cancellationToken = null) =>
            _client.Get("users.deletePhoto", new Args(), cancellationToken);

        /// <summary>
        /// Lets you find out information about a user's presence.
        /// </summary>
        /// <param name="userId">User to get presence info on. Defaults to the authed user.</param>
        /// <param name="cancellationToken"></param>
        public async Task<Presence> GetPresence(string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<PresenceResponse>("users.getPresence", new Args { { "user", userId } }, cancellationToken).ConfigureAwait(false)).Presence;

        /// <summary>
        /// After your Slack app is awarded an identity token through Sign in with Slack, use this method to retrieve a user's identity.
        /// The returned fields depend on any additional authorization scopes you've requested.
        /// This method may only be used by tokens with the identity.basic scope, as provided in the Sign in with Slack process.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task<IdentityResponse> Identity(CancellationToken? cancellationToken = null) =>
            _client.Get<IdentityResponse>("users.identity", new Args(), cancellationToken);

        /// <summary>
        /// Returns information about a team member.
        /// </summary>
        /// <param name="userId">User to get info on.</param>
        /// <param name="cancellationToken"></param>
        public async Task<User> Info(string userId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<UserResponse>("users.info", new Args { { "user", userId } }, cancellationToken).ConfigureAwait(false)).User;

        /// <summary>
        /// Returns a list of all users in the team.
        /// This includes deleted/deactivated users.
        /// </summary>
        /// <param name="cursor">
        /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
        /// returned by a previous request's <see cref="UserListResponse.ResponseMetadata"/>.
        /// Default value fetches the first "page" of the collection.
        /// </param>
        /// <param name="limit">The maximum number of items to return. Fewer than the requested number of items may be returned, even if the end of the users list hasn't been reached.</param>
        /// <param name="cancellationToken"></param>
        public Task<UserListResponse> List(string cursor = null, int limit = 0, CancellationToken? cancellationToken = null) =>
            _client.Get<UserListResponse>("users.list", new Args
                {
                    { "cursor", cursor },
                    { "limit", limit }
                }, cancellationToken);

        /// <summary>
        /// Find a user with an email address.
        /// </summary>
        /// <param name="email">An email address belonging to a user in the workspace.</param>
        /// <param name="cancellationToken"></param>
        public async Task<User> LookupByEmail(string email, CancellationToken? cancellationToken = null) =>
            (await _client.Get<UserResponse>("users.lookupByEmail", new Args { { "email", email } }, cancellationToken).ConfigureAwait(false)).User;

        /// <summary>
        /// This method allows the user to set their profile image.
        /// Providing a "crop box" with <see cref="cropX"/>, <see cref="cropY"/>, and <see cref="cropW"/> is optional. Otherwise, the whole image will be used.
        /// If cropping instructions are not specified and the source image is not square, the image will be letterboxed, just like your favorite old laserdiscs.
        /// Please limit your images to a maximum size of 1024 by 1024 pixels. 512x512 pixels is the minimum.
        /// To remove a profile image, use the companion method <see cref="DeletePhoto"/>.
        /// </summary>
        /// <param name="imageContent">Image file contents.</param>
        /// <param name="contentType">MIME type of image file, such as image/gif, image/jpeg, image/png, etc.</param>
        /// <param name="fileName">Image file name.</param>
        /// <param name="cropW">Width/height of crop box(always square).</param>
        /// <param name="cropX">X coordinate of top-left corner of crop box.</param>
        /// <param name="cropY">Y coordinate of top-left corner of crop box.</param>
        /// <param name="cancellationToken"></param>
        public Task SetPhoto(byte[] imageContent, string contentType, string fileName = "photo", int? cropW = null, int? cropX = null, int? cropY = null, CancellationToken? cancellationToken = null) =>
            _client.Post("users.setPhoto", new Args
                    {
                        { "crop_w", cropW },
                        { "crop_x", cropX },
                        { "crop_y", cropY }
                    },
                new MultipartFormDataContent
                    {
                        {
                            new ByteArrayContent(imageContent) { Headers = { ContentType = MediaTypeHeaderValue.Parse(contentType) } },
                            "image",
                            fileName
                        }
                    },
                cancellationToken);

        /// <summary>
        /// This method allows the user to set their profile image.
        /// Providing a "crop box" with <see cref="cropX"/>, <see cref="cropY"/>, and <see cref="cropW"/> is optional. Otherwise, the whole image will be used.
        /// If cropping instructions are not specified and the source image is not square, the image will be letterboxed, just like your favorite old laserdiscs.
        /// Please limit your images to a maximum size of 1024 by 1024 pixels. 512x512 pixels is the minimum.
        /// To remove a profile image, use the companion method <see cref="DeletePhoto"/>.
        /// </summary>
        /// <param name="image">Image file.</param>
        /// <param name="contentType">MIME type of image file, such as image/gif, image/jpeg, image/png, etc.</param>
        /// <param name="fileName">Image file name.</param>
        /// <param name="cropW">Width/height of crop box(always square).</param>
        /// <param name="cropX">X coordinate of top-left corner of crop box.</param>
        /// <param name="cropY">Y coordinate of top-left corner of crop box.</param>
        /// <param name="cancellationToken"></param>
        public Task SetPhoto(Stream image, string contentType, string fileName = "photo", int? cropW = null, int? cropX = null, int? cropY = null, CancellationToken? cancellationToken = null) =>
            _client.Post("users.setPhoto", new Args
                    {
                        { "crop_w", cropW },
                        { "crop_x", cropX },
                        { "crop_y", cropY }
                    },
                new MultipartFormDataContent
                    {
                        {
                            new StreamContent(image) { Headers = { ContentType = MediaTypeHeaderValue.Parse(contentType) } },
                            "image",
                            fileName
                        }
                    },
                cancellationToken);

        /// <summary>
        /// Lets you set the calling user's manual presence.
        /// </summary>
        /// <param name="presence">User's presence.</param>
        /// <param name="cancellationToken"></param>
        public Task SetPresence(Presence presence, CancellationToken? cancellationToken = null) =>
            _client.Get("users.setPresence", new Args { { "presence", presence } }, cancellationToken);
    }
}
