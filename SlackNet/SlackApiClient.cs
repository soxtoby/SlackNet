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

namespace SlackNet;

public interface ISlackApiClient
{
    IAgentsSessionsApi AgentsSessions { get; }
    IApiApi Api { get; }
    IAppsConnectionsApi AppsConnectionsApi { get; }
    IAppsEventAuthorizationsApi AppsEventAuthorizations { get; }
    IAssistantSearchApi AssistantSearch { get; }
    IAssistantThreadsApi AssistantThreads { get; }
    IAuthApi Auth { get; }
    IBookmarksApi Bookmarks { get; }
    IBotsApi Bots { get; }
    ICallParticipantsApi CallParticipants { get; }
    ICallsApi Calls { get; }
    ICanvasesApi Canvases { get; }
    IChatApi Chat { get; }
    IConversationsApi Conversations { get; }
    IDialogApi Dialog { get; }
    IDndApi Dnd { get; }
    IEmojiApi Emoji { get; }
    IEntityApi Entity { get; }
    IExternalTeamsApi ExternalTeams { get; }
    IFileCommentsApi FileComments { get; }
    IFilesApi Files { get; }
    IFunctionsApi Functions { get; }
    IFunctionDistributionPermissionsApi FunctionDistributionPermissions { get; }
    IFunctionWorkflowStepsApi FunctionWorkflowSteps { get; }
    IListApi List { get; }
    IListDownloadApi ListDownload { get; }
    IListItemsApi ListItems { get; }
    IListAccessApi ListAccess { get; }
    IMigrationApi Migration { get; }
    IOAuthApi OAuth { get; }
    IOAuthV2Api OAuthV2 { get; }
    IOpenIdApi OpenIdApi { get; }
    IPinsApi Pins { get; }
    IReactionsApi Reactions { get; }
    IRemindersApi Reminders { get; }
    IRemoteFilesApi RemoteFiles { get; }
    IRtmApi Rtm { get; }
    IScheduledMessagesApi ScheduledMessages { get; }
    ISearchApi Search { get; }
    ITeamApi Team { get; }
    ITeamBillingApi TeamBilling { get; }
    ITeamPreferencesApi TeamPreferences { get; }
    ITeamProfileApi TeamProfile { get; }
    IToolingApi Tooling { get; }
    IUserGroupsApi UserGroups { get; }
    IUserGroupUsersApi UserGroupUsers { get; }
    IUsersApi Users { get; }
    IUserProfileApi UserProfile { get; }
    IViewsApi Views { get; }
    IWorkflowsFeaturedApi WorkflowsFeatured { get; }
    IWorkflowTriggerPermissionsApi WorkflowTriggerPermissions { get; }

    /// <summary>
    /// Calls a Slack API method.
    /// </summary>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
    /// <param name="cancellationToken"></param>
    Task Get(string apiMethod, Args args, CancellationToken cancellationToken);

    /// <summary>
    /// Calls a Slack API method.
    /// </summary>
    /// <typeparam name="T">Type of response expected.</typeparam>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
    /// <param name="cancellationToken"></param>
    Task<T> Get<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Calls a Slack API that requires POST content.
    /// </summary>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. Authorization headers will be added automatically.</param>
    /// <param name="cancellationToken"></param>
    Task Post(string apiMethod, Args args, CancellationToken cancellationToken);

    /// <summary>
    /// Calls a Slack API that requires POST content.
    /// </summary>
    /// <typeparam name="T">Type of response expected.</typeparam>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. Authorization headers will be added automatically.</param>
    /// <param name="cancellationToken"></param>
    Task<T> Post<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Calls a Slack API that requires POST content.
    /// </summary>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
    /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
    /// <param name="cancellationToken"></param>
    Task Post(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken);

    /// <summary>
    /// Calls a Slack API that requires POST content.
    /// </summary>
    /// <typeparam name="T">Type of response expected.</typeparam>
    /// <param name="apiMethod">Name of Slack method.</param>
    /// <param name="args">Arguments to send to Slack. The "token" parameter will be filled in automatically.</param>
    /// <param name="content">POST body content. Should be either <see cref="FormUrlEncodedContent"/> or <see cref="MultipartFormDataContent"/>.</param>
    /// <param name="cancellationToken"></param>
    Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Posts a message to a response URL provided by e.g. <see cref="InteractionRequest"/> or <see cref="SlashCommand"/>.
    /// </summary>
    /// <param name="responseUrl">A temporary webhook that can be used to send messages in response to interactions.</param>
    /// <param name="message">The message to respond with.</param>
    /// <param name="cancellationToken"></param>
    Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken cancellationToken);

    /// <summary>
    /// Posts a message to an incoming webhook.
    /// </summary>
    /// <param name="webhookUrl">URL for the incoming webhook.</param>
    /// <param name="message">The message to send to the incoming webhook.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>See the <a href="https://docs.slack.dev/messaging/sending-messages-using-incoming-webhooks/">Slack documentation</a> for more information.</remarks>
    Task PostToWebhook(string webhookUrl, Message message, CancellationToken cancellationToken = default);

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
    private readonly ISlackApiTokenProvider _apiTokenProvider;
    private readonly ISlackAppLevelTokenProvider _appLevelTokenProvider;
    private readonly SlackJsonSettings _jsonSettings;
    public bool DisableRetryOnRateLimit { get; set; }
    public bool WarningsAsErrors { get; set; }

    public SlackApiClient(string token)
    {
        _jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
        _http = Default.Http(_jsonSettings);
        _urlBuilder = Default.UrlBuilder(_jsonSettings);
        _apiTokenProvider = new ConstantApiTokenProvider(token);
        _appLevelTokenProvider = new ConstantAppLevelTokenProvider(token);
    }

    [Obsolete("Use constructor with IHttp, ISlackUrlBuilder, SlackJsonSettings, ISlackApiTokenProvider, ISlackAppLevelTokenProvider")]
    public SlackApiClient(IHttp http, ISlackUrlBuilder urlBuilder, SlackJsonSettings jsonSettings, string token)
        : this(http, urlBuilder, jsonSettings, new ConstantApiTokenProvider(token), new ConstantAppLevelTokenProvider(token)) { }

    /// <summary>
    /// Creates a Slack Web API client whose API and app-level credentials can be selected for each request.
    /// </summary>
    public SlackApiClient(
        IHttp http,
        ISlackUrlBuilder urlBuilder,
        SlackJsonSettings jsonSettings,
        ISlackApiTokenProvider apiTokenProvider,
        ISlackAppLevelTokenProvider appLevelTokenProvider)
    {
        _http = http;
        _urlBuilder = urlBuilder;
        _jsonSettings = jsonSettings;
        _apiTokenProvider = apiTokenProvider;
        _appLevelTokenProvider = appLevelTokenProvider;
    }

    public ISlackApiClient WithAccessToken(string accessToken) =>
        new SlackApiClient(_http, _urlBuilder, _jsonSettings, new ConstantApiTokenProvider(accessToken), new ConstantAppLevelTokenProvider(accessToken))
            {
                DisableRetryOnRateLimit = DisableRetryOnRateLimit,
                WarningsAsErrors = WarningsAsErrors
            };

    public IAgentsSessionsApi AgentsSessions => new AgentsSessionsApi(this);
    public IApiApi Api => new ApiApi(this);
    public IAppsConnectionsApi AppsConnectionsApi => new AppsConnectionsApi(this);
    public IAppsEventAuthorizationsApi AppsEventAuthorizations => new AppsEventAuthorizationsApi(this);
    public IAssistantSearchApi AssistantSearch => new AssistantSearchApi(this);
    public IAssistantThreadsApi AssistantThreads => new AssistantThreadsApi(this);
    public IAuthApi Auth => new AuthApi(this);
    public IBookmarksApi Bookmarks => new BookmarksApi(this);
    public IBotsApi Bots => new BotsApi(this);
    public ICallParticipantsApi CallParticipants => new CallParticipantsApi(this);
    public ICallsApi Calls => new CallsApi(this);
    public ICanvasesApi Canvases => new CanvasesApi(this);
    public IChatApi Chat => new ChatApi(this, _jsonSettings);
    public IConversationsApi Conversations => new ConversationsApi(this);
    public IDialogApi Dialog => new DialogApi(this);
    public IDndApi Dnd => new DndApi(this);
    public IEmojiApi Emoji => new EmojiApi(this);
    public IEntityApi Entity => new EntityApi(this);
    public IExternalTeamsApi ExternalTeams => new ExternalTeamsApi(this);
    public IFileCommentsApi FileComments => new FileCommentsApi(this);
    public IFilesApi Files => new FilesApi(this, _http);
    public IFunctionsApi Functions => new FunctionsApi(this);
    public IFunctionDistributionPermissionsApi FunctionDistributionPermissions => new FunctionDistributionPermissionsApi(this);
    public IFunctionWorkflowStepsApi FunctionWorkflowSteps => new FunctionWorkflowStepsApi(this);
    public IListApi List => new ListApi(this);
    public IListDownloadApi ListDownload => new ListDownloadApi(this);
    public IListItemsApi ListItems => new ListItemsApi(this);
    public IListAccessApi ListAccess => new ListAccessApi(this);
    public IMigrationApi Migration => new MigrationApi(this);
    public IOAuthApi OAuth => new OAuthApi(this);
    public IOAuthV2Api OAuthV2 => new OAuthV2Api(this);
    public IOpenIdApi OpenIdApi => new OpenIdApi(this);
    public IPinsApi Pins => new PinsApi(this);
    public IReactionsApi Reactions => new ReactionsApi(this);
    public IRemindersApi Reminders => new RemindersApi(this);
    public IRemoteFilesApi RemoteFiles => new RemoteFilesApi(this);
    public IRtmApi Rtm => new RtmApi(this);
    public IScheduledMessagesApi ScheduledMessages => new ScheduledMessagesApi(this);
    public ISearchApi Search => new SearchApi(this);
    public ITeamApi Team => new TeamApi(this);
    public ITeamBillingApi TeamBilling => new TeamBillingApi(this);
    public ITeamPreferencesApi TeamPreferences => new TeamPreferencesApi(this);
    public ITeamProfileApi TeamProfile => new TeamProfileApi(this);
    public IToolingApi Tooling => new ToolingApi(this);
    public IUserGroupsApi UserGroups => new UserGroupsApi(this);
    public IUserGroupUsersApi UserGroupUsers => new UserGroupUsersApi(this);
    public IUsersApi Users => new UsersApi(this);
    public IUserProfileApi UserProfile => new UserProfileApi(this);
    public IViewsApi Views => new ViewsApi(this);
    public IWorkflowsFeaturedApi WorkflowsFeatured => new WorkflowsFeaturedApi(this);
    public IWorkflowTriggerPermissionsApi WorkflowTriggerPermissions => new WorkflowTriggerPermissionsApi(this);

    public Task Get(string apiMethod, Args args, CancellationToken cancellationToken) =>
        Get<object>(apiMethod, args, cancellationToken);

    public Task<T> Get<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class
    {
        var requestUrl = Url(apiMethod, args);
        return WebApiRequest<T>(
            () => new HttpRequestMessage(HttpMethod.Get, requestUrl),
            ApiCallContext(requestUrl, apiMethod, args),
            cancellationToken);
    }

    public Task Post(string apiMethod, Args args, CancellationToken cancellationToken) =>
        Post<object>(apiMethod, args, cancellationToken);

    public Task<T> Post<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class =>
        Post<T>(Url(apiMethod), apiMethod, args, StripNullArgs(args), cancellationToken);

    public Task Post(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken) =>
        Post<object>(apiMethod, args, content, cancellationToken);

    public Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken) where T : class
    {
        var requestUrl = Url(apiMethod, args);
        return WebApiRequest<T>(
            () => new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content },
            ApiCallContext(requestUrl, apiMethod, args),
            cancellationToken);
    }

    public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken cancellationToken) =>
        Post<object>(responseUrl, string.Empty, new Args(), message, cancellationToken);

    public Task PostToWebhook(string webhookUrl, Message message, CancellationToken cancellationToken = default) =>
        WithAccessToken(string.Empty).Respond(webhookUrl, message, cancellationToken);

    private Task<T> Post<T>(string requestUrl, string apiMethod, Args args, object body, CancellationToken cancellationToken) where T : class =>
        WebApiRequest<T>(() => new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(body, _jsonSettings.SerializerSettings), Encoding.UTF8, "application/json")
                },
            ApiCallContext(requestUrl, apiMethod, args),
            cancellationToken);

    private string Url(string apiMethod, Args args = null) =>
        _urlBuilder.Url(apiMethod, args ?? new Args());

    private async Task<T> WebApiRequest<T>(Func<HttpRequestMessage> createRequest, SlackApiCallContext callContext, CancellationToken cancellationToken) where T : class
    {
        while (true)
        {
            try
            {
                var request = createRequest();
                var token = await ResolveToken(callContext, cancellationToken).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(token)) // Token is cleared by methods that don't require authentication (e.g. OAuthV2Api.Access)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.Execute<WebApiResponse>(request, cancellationToken).ConfigureAwait(false)
                    ?? new WebApiResponse { Ok = true };

                return Deserialize<T>(response);
            }
            catch (SlackRateLimitException e) when (!DisableRetryOnRateLimit)
            {
                await Task.Delay(e.RetryAfter ?? TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private T Deserialize<T>(WebApiResponse response) where T : class =>
        response.Ok
            ? WarningsAsErrors && ReadErrorResponse(response) is { Warning.Length: > 0 } warning
                ? throw new SlackException(warning)
                : response.Data?.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings))
            : throw new SlackException(ReadErrorResponse(response));

    private ErrorResponse ReadErrorResponse(WebApiResponse response) =>
        response.Data?.ToObject<ErrorResponse>(JsonSerializer.Create(_jsonSettings.SerializerSettings));

    private static Args StripNullArgs(Args args) =>
        args.Where(kv => kv.Value != null)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

    private static SlackApiCallContext ApiCallContext(string requestUrl, string apiMethod, Args arguments) =>
        new(requestUrl, apiMethod, arguments) { OriginatingRequest = SlackRequestContext.IsAvailable ? SlackRequestContext.Current : null };

    private Task<string> ResolveToken(SlackApiCallContext callContext, CancellationToken cancellationToken) =>
        callContext.ApiMethod == WebApi.AppsConnectionsApi.OpenMethod
            ? _appLevelTokenProvider.GetAppLevelToken(callContext, cancellationToken)
            : _apiTokenProvider.GetApiToken(callContext, cancellationToken);
}
