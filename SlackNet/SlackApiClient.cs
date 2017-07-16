using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet
{
    public interface ISlackApiClient
    {
        IApiApi Api { get; }
        IAuthApi Auth { get; }
        IBotsApi Bots { get; }
        IChannelsApi Channels { get; }
        IChatApi Chat { get; }
        IDndApi Dnd { get; }
        IEmojiApi Emoji { get; }
        IFileCommentsApi FileComments { get; }
        IFilesApi Files { get; }
        IGroupsApi Groups { get; }
        IImApi Im { get; }
        IMpimApi Mpim { get; }
        IOAuthApi OAuth { get; }
        IPinsApi Pins { get; }
        IReactionsApi Reactions { get; }
        IRemindersApi Reminder { get; }
        IRtmApi Rtm { get; }
        ISearchApi Search { get; }
        IStarsApi Stars { get; }
        ITeamApi Team { get; }
        ITeamProfileApi TeamProfile { get; }
        IUserGroupsApi UserGroups { get; }
        IUserGroupUsersApi UserGroupUsers { get; }
        IUsersApi Users { get; }
        IUserProfileApi UserProfile { get; }

        /// <summary>
        /// Calls a Slack API method.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        Task Get(string apiMethod, Args args, CancellationToken? cancellationToken);

        /// <summary>
        /// Calls a Slack API method.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        Task<T> Get<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class;

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="bodyName">Parameter name for POST body content.</param>
        /// <param name="bodyContent">POST body content.</param>
        /// <param name="cancellationToken"></param>
        Task Post(string apiMethod, Args args, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="bodyName">Parameter name for POST body content.</param>
        /// <param name="bodyContent">POST body content.</param>
        /// <param name="cancellationToken"></param>
        Task<T> Post<T>(string apiMethod, Args args, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken) where T : class;
    }

    public class SlackApiClient : ISlackApiClient
    {
        private readonly IHttp _http;
        private readonly ISlackUrlBuilder _urlBuilder;
        private readonly string _token;
        private readonly JsonSerializerSettings _serializerSettings;

        public SlackApiClient(string token)
        {
            _serializerSettings = Default.SerializerSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            _http = Default.Http(_serializerSettings);
            _urlBuilder = Default.UrlBuilder(_serializerSettings);
            _token = token;
        }

        public SlackApiClient(IHttp http, ISlackUrlBuilder urlBuilder, JsonSerializerSettings serializerSettings, string token)
        {
            _http = http;
            _urlBuilder = urlBuilder;
            _serializerSettings = serializerSettings;
            _token = token;
        }

        public IApiApi Api => new ApiApi(this);
        public IAuthApi Auth => new AuthApi(this);
        public IBotsApi Bots => new BotsApi(this);
        public IChannelsApi Channels => new ChannelsApi(this);
        public IChatApi Chat => new ChatApi(this);
        public IDndApi Dnd => new DndApi(this);
        public IEmojiApi Emoji => new EmojiApi(this);
        public IFileCommentsApi FileComments => new FileCommentsApi(this);
        public IFilesApi Files => new FilesApi(this);
        public IGroupsApi Groups => new GroupsApi(this);
        public IImApi Im => new ImApi(this);
        public IMpimApi Mpim => new MpimApi(this);
        public IOAuthApi OAuth => new OAuthApi(this);
        public IPinsApi Pins => new PinsApi(this);
        public IReactionsApi Reactions => new ReactionsApi(this);
        public IRemindersApi Reminder => new RemindersApi(this);
        public IRtmApi Rtm => new RtmApi(this);
        public ISearchApi Search => new SearchApi(this);
        public IStarsApi Stars => new StarsApi(this);
        public ITeamApi Team => new TeamApi(this);
        public ITeamProfileApi TeamProfile => new TeamProfileApi(this);
        public IUserGroupsApi UserGroups => new UserGroupsApi(this);
        public IUserGroupUsersApi UserGroupUsers => new UserGroupUsersApi(this);
        public IUsersApi Users => new UsersApi(this);
        public IUserProfileApi UserProfile => new UserProfileApi(this);

        /// <summary>
        /// Calls a Slack API method.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        public Task Get(string apiMethod, Args args, CancellationToken? cancellationToken) =>
            Get<object>(apiMethod, args, cancellationToken);

        /// <summary>
        /// Calls a Slack API method.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        public async Task<T> Get<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class =>
            Deserialize<T>(await _http.Get<WebApiResponse>(Url(apiMethod, args), cancellationToken ?? CancellationToken.None).ConfigureAwait(false));

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="bodyName">Parameter name for POST body content.</param>
        /// <param name="bodyContent">POST body content.</param>
        /// <param name="cancellationToken"></param>
        public Task Post(string apiMethod, Args args, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken) =>
            Post<object>(apiMethod, args, bodyName, bodyContent, cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="bodyName">Parameter name for POST body content.</param>
        /// <param name="bodyContent">POST body content.</param>
        /// <param name="cancellationToken"></param>
        public async Task<T> Post<T>(string apiMethod, Args args, string bodyName, HttpContent bodyContent, CancellationToken? cancellationToken) where T : class =>
            Deserialize<T>(await _http.Post<WebApiResponse>(Url(apiMethod, args), bodyName, bodyContent, cancellationToken ?? CancellationToken.None).ConfigureAwait(false));

        private string Url(string apiMethod, Args args)
        {
            if (!args.ContainsKey("token"))
                args["token"] = _token;
            return _urlBuilder.Url(apiMethod, args);
        }

        private T Deserialize<T>(WebApiResponse response) where T : class =>
            response.Ok
                ? response.Data?.ToObject<T>(JsonSerializer.Create(_serializerSettings))
                : throw new SlackException(response.Error);
    }
}