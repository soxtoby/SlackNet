using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet
{
    public class WebApiClient
    {
        private readonly IHttp _http;
        private readonly ISlackUrlBuilder _urlBuilder;
        private readonly string _token;
        private readonly JsonSerializerSettings _serializerSettings;

        public WebApiClient(string token)
        {
            _serializerSettings = Default.SerializerSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            _http = Default.Http(_serializerSettings);
            _urlBuilder = Default.UrlBuilder(_serializerSettings);
            _token = token;
        }

        public WebApiClient(IHttp http, ISlackUrlBuilder urlBuilder, JsonSerializerSettings serializerSettings, string token)
        {
            _http = http;
            _urlBuilder = urlBuilder;
            _serializerSettings = serializerSettings;
            _token = token;
        }

        public ApiApi Api => new ApiApi(this);
        public AuthApi Auth => new AuthApi(this);
        public BotsApi Bots => new BotsApi(this);
        public ChannelsApi Channels => new ChannelsApi(this);
        public ChatApi Chat => new ChatApi(this);
        public DndApi Dnd => new DndApi(this);
        public EmojiApi Emoji => new EmojiApi(this);
        public FileCommentsApi FileComments => new FileCommentsApi(this);
        public FilesApi Files => new FilesApi(this);
        public GroupsApi Groups => new GroupsApi(this);
        public ImApi Im => new ImApi(this);
        public MpimApi Mpim => new MpimApi(this);
        public OAuthApi OAuth => new OAuthApi(this);
        public PinsApi Pins => new PinsApi(this);
        public ReactionsApi Reactions => new ReactionsApi(this);
        public RemindersApi Reminder => new RemindersApi(this);
        public RtmApi Rtm => new RtmApi(this);
        public SearchApi Search => new SearchApi(this);
        public StarsApi Stars => new StarsApi(this);
        public TeamApi Team => new TeamApi(this);
        public TeamProfileApi TeamProfile => new TeamProfileApi(this);
        public UserGroupsApi UserGroups => new UserGroupsApi(this);
        public UserGroupUsersApi UserGroupUsers => new UserGroupUsersApi(this);
        public UsersApi Users => new UsersApi(this);
        public UserProfileApi UserProfile => new UserProfileApi(this);

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