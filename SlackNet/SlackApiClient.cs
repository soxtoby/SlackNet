using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlackNet.Interaction;
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
        IConversationsApi Conversations { get; }
        IDialogApi Dialog { get; }
        IDndApi Dnd { get; }
        IEmojiApi Emoji { get; }
        IFileCommentsApi FileComments { get; }
        IFilesApi Files { get; }
        IGroupsApi Groups { get; }
        IImApi Im { get; }
        IMigrationApi Migration { get; }
        IMpimApi Mpim { get; }
        IOAuthApi OAuth { get; }
        IPinsApi Pins { get; }
        IReactionsApi Reactions { get; }
        IRemindersApi Reminders { get; }
        IRemoteFilesApi RemoteFiles { get; }
        IRtmApi Rtm { get; }
        IScheduledMessagesApi ScheduledMessages { get; }
        ISearchApi Search { get; }
        IStarsApi Stars { get; }
        ITeamApi Team { get; }
        ITeamProfileApi TeamProfile { get; }
        IUserGroupsApi UserGroups { get; }
        IUserGroupUsersApi UserGroupUsers { get; }
        IUsersApi Users { get; }
        IUserProfileApi UserProfile { get; }
        IViewsApi Views { get; }

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
        /// <param name="args">Arguments to send to Slack. Authorization headers will be added automatically.</param>
        /// <param name="cancellationToken"></param>
        Task Post(string apiMethod, Args args, CancellationToken? cancellationToken);
        
        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. Authorization headers will be added automatically.</param>
        /// <param name="cancellationToken"></param>
        Task<T> Post<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class;
        
        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
        /// <param name="cancellationToken"></param>
        Task Post(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken);
        
        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
        /// <param name="cancellationToken"></param>
        Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken) where T : class;

        /// <summary>
        /// Posts a message to a response URL provided by e.g. <see cref="InteractionRequest"/> or <see cref="SlashCommand"/>.
        /// </summary>
        /// <param name="responseUrl">A temporary webhook that can be used to send messages in response to interactions.</param>
        /// <param name="message">The message to respond with.</param>
        /// <param name="cancellationToken"></param>
        Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken? cancellationToken);

        /// <summary>
        /// Returns a copy of the client using a different access token.
        /// Useful when you need to run a command as a specific user.
        /// </summary>
        /// <param name="accessToken">New access token.</param>
        ISlackApiClient WithAccessToken(string accessToken);
    }

    public class SlackApiClient : ISlackApiClient
    {
        private readonly IHttp _http;
        private readonly ISlackUrlBuilder _urlBuilder;
        private readonly string _token;
        private readonly SlackJsonSettings _jsonSettings;
        public bool DisableRetryOnRateLimit { get; set; }

        public SlackApiClient(string token)
        {
            _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            _http = Default.Http(_jsonSettings);
            _urlBuilder = Default.UrlBuilder(_jsonSettings);
            _token = token;
        }

        public SlackApiClient(IHttp http, ISlackUrlBuilder urlBuilder, SlackJsonSettings jsonSettings, string token)
        {
            _http = http;
            _urlBuilder = urlBuilder;
            _jsonSettings = jsonSettings;
            _token = token;
        }

        /// <summary>
        /// Returns a copy of the client using a different access token.
        /// Useful when you need to run a command as a specific user.
        /// </summary>
        /// <param name="accessToken">New access token.</param>
        public ISlackApiClient WithAccessToken(string accessToken) => new SlackApiClient(_http, _urlBuilder, _jsonSettings, accessToken);

        public IApiApi Api => new ApiApi(this);
        public IAuthApi Auth => new AuthApi(this);
        public IBotsApi Bots => new BotsApi(this);
        public IChannelsApi Channels => new ChannelsApi(this);
        public IChatApi Chat => new ChatApi(this);
        public IConversationsApi Conversations => new ConversationsApi(this);
        public IDialogApi Dialog => new DialogApi(this);
        public IDndApi Dnd => new DndApi(this);
        public IEmojiApi Emoji => new EmojiApi(this);
        public IFileCommentsApi FileComments => new FileCommentsApi(this);
        public IFilesApi Files => new FilesApi(this);
        public IGroupsApi Groups => new GroupsApi(this);
        public IImApi Im => new ImApi(this);
        public IMigrationApi Migration => new MigrationApi(this);
        public IMpimApi Mpim => new MpimApi(this);
        public IOAuthApi OAuth => new OAuthApi(this);
        public IPinsApi Pins => new PinsApi(this);
        public IReactionsApi Reactions => new ReactionsApi(this);
        public IRemindersApi Reminders => new RemindersApi(this);
        public IRemoteFilesApi RemoteFiles => new RemoteFilesApi(this);
        public IRtmApi Rtm => new RtmApi(this);
        public IScheduledMessagesApi ScheduledMessages => new ScheduledMessagesApi(this);
        public ISearchApi Search => new SearchApi(this);
        public IStarsApi Stars => new StarsApi(this);
        public ITeamApi Team => new TeamApi(this);
        public ITeamProfileApi TeamProfile => new TeamProfileApi(this);
        public IUserGroupsApi UserGroups => new UserGroupsApi(this);
        public IUserGroupUsersApi UserGroupUsers => new UserGroupUsersApi(this);
        public IUsersApi Users => new UsersApi(this);
        public IUserProfileApi UserProfile => new UserProfileApi(this);
        public IViewsApi Views => new ViewsApi(this);

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
        public Task<T> Get<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class => 
            WebApiRequest<T>(() => new HttpRequestMessage(HttpMethod.Get, Url(apiMethod, args)), cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        public Task Post(string apiMethod, Args args, CancellationToken? cancellationToken) =>
            Post<object>(apiMethod, args, cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="cancellationToken"></param>
        public Task<T> Post<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class => 
            Post<T>(Url(apiMethod), (object)StripNullArgs(args), cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
        /// <param name="cancellationToken"></param>
        public Task Post(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken) =>
            Post<object>(apiMethod, args, content, cancellationToken);

        /// <summary>
        /// Calls a Slack API that requires POST content.
        /// </summary>
        /// <typeparam name="T">Type of response expected.</typeparam>
        /// <param name="apiMethod">Name of Slack method.</param>
        /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
        /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
        /// <param name="cancellationToken"></param>
        public Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken) where T : class => 
            WebApiRequest<T>(() => new HttpRequestMessage(HttpMethod.Post, Url(apiMethod, args)) { Content = content }, cancellationToken);

        /// <summary>
        /// Posts a message to a response URL provided by e.g. <see cref="InteractionRequest"/> or <see cref="SlashCommand"/>.
        /// </summary>
        /// <param name="responseUrl">A temporary webhook that can be used to send messages in response to interactions.</param>
        /// <param name="message">The message to respond with.</param>
        /// <param name="cancellationToken"></param>
        public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken? cancellationToken) =>
            Post<object>(responseUrl, message, cancellationToken);

        private Task<T> Post<T>(string requestUri, object body, CancellationToken? cancellationToken) where T : class =>
            WebApiRequest<T>(() =>
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    requestMessage.Content = new StringContent(JsonConvert.SerializeObject(body, _jsonSettings.SerializerSettings), Encoding.UTF8, "application/json");
                    return requestMessage;
                }, cancellationToken);

        private string Url(string apiMethod) =>
            _urlBuilder.Url(apiMethod, new Args());
        
        private string Url(string apiMethod, Args args)
        {
            if (!args.ContainsKey("token"))
                args["token"] = _token;
            return _urlBuilder.Url(apiMethod, args);
        }

        private async Task<T> WebApiRequest<T>(Func<HttpRequestMessage> createRequest, CancellationToken? cancellationToken) where T : class
        {
            while (true)
            {
                try
                {
                    var response = await _http.Execute<WebApiResponse>(createRequest(), cancellationToken ?? CancellationToken.None).ConfigureAwait(false)
                        ?? new WebApiResponse { Ok = true };
                    return Deserialize<T>(response);
                }
                catch (SlackRateLimitException e) when (!DisableRetryOnRateLimit)
                {
                    await Task.Delay(e.RetryAfter ?? TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
            }
        }

        private T Deserialize<T>(WebApiResponse response) where T : class =>
            response.Ok
                ? response.Data?.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings))
                : throw new SlackException(response.Data?.ToObject<ErrorResponse>(JsonSerializer.Create(_jsonSettings.SerializerSettings)));

        private static Args StripNullArgs(Args args) => 
            args.Where(kv => kv.Value != null)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}