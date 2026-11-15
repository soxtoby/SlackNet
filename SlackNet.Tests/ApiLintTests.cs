using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.AppManifests;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.Tests;

public class ApiLintTests
{
    [TestCaseSource(nameof(ApiClasses))]
    public void Lint(Type api)
    {
        var client = new FakeClient();
        var instance = CreateApiInstance(api, client);

        var apiInterface = GetApiInterface(api);

        ApiClientHasAPropertyForApi(api, apiInterface);

        var apiMethods = api.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

        AllPublicMethodsAreExposedOnInterface(apiInterface, apiMethods);

        string slackMethodGroup = null;

        foreach (var method in apiMethods)
        {
            client.Reset();
            Invoke(method, instance);

            LastParamShouldBeOptionalCancellationToken(method);
            AllMethodsShouldUseSameSlackMethodGroup(client, method, ref slackMethodGroup);
            AllArgsShouldBeSnakeCase(client.Args, method);
        }
    }

    internal static HashSet<string> ImplementedSlackMethods() =>
        ImplementedSlackMethodCalls().Select(m => m.SlackMethod).ToHashSet();

    internal static IEnumerable<SlackMethodCall> ImplementedSlackMethodCalls()
    {
        var client = new FakeClient();
        foreach (var api in ApiClasses)
        {
            var instance = CreateApiInstance(api, client);
            foreach (var method in api.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                client.Reset();
                Invoke(method, instance);
                yield return new SlackMethodCall(client.SlackMethod, client.Args?.Keys.ToArray() ?? [], client.ResponseType);
            }
        }
    }

    private static void Invoke(MethodInfo method, object instance)
    {
        var closedMethod = WithDummyTypeArguments(method);
        var result = closedMethod.Invoke(instance, closedMethod.GetParameters().Select(DummyValue).ToArray());
        if (AsyncEnumerableItemType(closedMethod.ReturnType) is { } itemType)
            StartEnumeratingMethod.MakeGenericMethod(itemType).Invoke(null, [result]);
    }

    private static Type AsyncEnumerableItemType(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            ? type.GetGenericArguments()[0]
            : null;

    private static readonly MethodInfo StartEnumeratingMethod = typeof(ApiLintTests).GetMethod(nameof(StartEnumerating), BindingFlags.NonPublic | BindingFlags.Static);

    /// <summary>
    /// Async enumerables, such as those for file downloads, only call the API once enumeration starts.
    /// </summary>
    private static void StartEnumerating<T>(IAsyncEnumerable<T> enumerable)
    {
        var enumerator = enumerable.GetAsyncEnumerator();
        enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult();
        enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Generic methods, such as those taking app-defined function outputs, can only be invoked or compared once their type arguments are filled in.
    /// </summary>
    private static MethodInfo WithDummyTypeArguments(MethodInfo method) =>
        method.IsGenericMethodDefinition
            ? method.MakeGenericMethod(method.GetGenericArguments().Select(_ => typeof(JObject)).ToArray())
            : method;

    private static object CreateApiInstance(Type api, FakeClient client)
    {
        return GetConstructor<SlackJsonSettings>(api) is not null ? Activator.CreateInstance(api, client, Default.JsonSettings())
            : GetConstructor<IHttp>(api) is not null ? Activator.CreateInstance(api, client, new FakeHttp())
            : Activator.CreateInstance(api, client);
    }

    private static ConstructorInfo GetConstructor<TExtraParam>(Type api) =>
        api.GetConstructor(BindingFlags.Public | BindingFlags.Instance, [typeof(ISlackApiClient), typeof(TExtraParam)]);

    private static Type GetApiInterface(Type api)
    {
        api.GetInterfaces().ShouldBeSingular($"{api.Name} doesn't implement an interface");
        return api.GetInterfaces().Single();
    }

    private static void ApiClientHasAPropertyForApi(Type api, Type apiInterface)
    {
        typeof(ISlackApiClient).GetProperties()
            .SingleOrDefault(p => p.PropertyType == apiInterface)
            .ShouldNotBeNull($"{api.Name} doesn't have a property on {nameof(ISlackApiClient)}").And(property =>
                {
                    if (!ApiPropertyNameExceptions.Contains(property.Name))
                        property.Name.ShouldNotContain("Api");
                });
    }

    private static readonly string[] ApiPropertyNameExceptions =
        [
            "Api",

            // These are wrong, but they've already been shipped :'(
            "AppsConnectionsApi",
            "OpenIdApi"
        ];

    private static void AllPublicMethodsAreExposedOnInterface(Type apiInterface, MethodInfo[] apiMethods)
    {
        apiMethods.ShouldNotBeEmpty();

        var interfaceMethods = apiInterface.GetMethods();

        apiMethods.ShouldOnlyContain(interfaceMethods, MethodsEqual);
    }

    private static bool MethodsEqual(MethodInfo a, MethodInfo b) =>
        a.Name == b.Name
        && ParameterTypes(WithDummyTypeArguments(a)).SequenceEqual(ParameterTypes(WithDummyTypeArguments(b)));

    private static IEnumerable<Type> ParameterTypes(MethodInfo method)
    {
        return method.GetParameters().Select(p => p.ParameterType);
    }

    private static void LastParamShouldBeOptionalCancellationToken(MethodInfo method)
    {
        var lastParam = method.GetParameters().Last();
        lastParam.ParameterType.ShouldBe(typeof(CancellationToken), $"{method.DeclaringType!.Name}.{method.Name} is missing CancellationToken param");
        lastParam.DefaultValue.ShouldBeNull($"{method.DeclaringType.Name}.{method.Name} CancellationToken param isn't null by default");
    }

    private static void AllMethodsShouldUseSameSlackMethodGroup(FakeClient client, MethodInfo method, ref string slackMethodGroup)
    {
        if (MethodGroupExceptions.TryGetValue(method.DeclaringType!, out var exceptions) && exceptions.Contains(client.SlackMethod))
            return;

        var methodGroup = string.Join('.', client.SlackMethod.Split('.').SkipLast(1));
        if (slackMethodGroup == null)
            slackMethodGroup = methodGroup;
        else
            methodGroup.ShouldBe(slackMethodGroup, $"{method.DeclaringType!.Name}.{method.Name} uses a different API method group than other methods");
    }

    private static readonly Dictionary<Type, string[]> MethodGroupExceptions = new()
        {
            [typeof(AuthApi)] =
                [
                    "auth.teams.list"
                ],
            [typeof(CanvasesApi)] =
                [
                    "canvases.access.delete",
                    "canvases.sections.lookup",
                    "canvases.access.set"
                ],
            [typeof(ConversationsApi)] =
                [
                    "conversations.canvases.create",
                    "conversations.externalInvitePermissions.set",
                    "conversations.requestSharedInvite.approve",
                    "conversations.requestSharedInvite.deny",
                    "conversations.requestSharedInvite.list"
                ],
            [typeof(FunctionWorkflowStepsApi)] = ["functions.workflows.steps.responses.export"],
            [typeof(OAuthV2Api)] = ["oauth.v2.user.access"],
            [typeof(UsersApi)] =
                [
                    "users.discoverableContacts.lookup"
                ]
        };

    private static void AllArgsShouldBeSnakeCase(Args args, MethodInfo method) =>
        args.Keys.AllItemsSatisfy(arg => arg.ShouldMatch("[a-z_]", $"{method.DeclaringType!.Name}.{method.Name} has incorrect casing for argument {arg}"));

    private static object DummyValue(ParameterInfo param) => ArgumentFactories.TryGetValue(param.ParameterType, out var factory)
        ? factory(param)
        : throw new AssertionException($"Unexpected param type {param.ParameterType.Name} in {param.Member.DeclaringType!.Name}.{param.Member.Name}");

    private static readonly Dictionary<Type, Func<ParameterInfo, object>> ArgumentFactories = new()
        {
            { typeof(string), p => p.Name == "contentType" ? "text/png" : "foo" },
            { typeof(int), _ => 0 },
            { typeof(int?), _ => null },
            { typeof(long), _ => 0L },
            { typeof(bool), _ => false },
            { typeof(bool?), _ => null },
            { typeof(object), _ => null },
            { typeof(IEnumerable<string>), _ => Enumerable.Empty<string>() },
            { typeof(Message), _ => new Message() },
            { typeof(Dialog), _ => new Dialog() },
            { typeof(MessageUpdate), _ => new MessageUpdate() },
            { typeof(DateTime), _ => DateTime.Now },
            { typeof(DateTime?), _ => DateTime.Now },
            { typeof(TimeSpan), _ => TimeSpan.Zero },
            { typeof(TimeSpan?), _ => TimeSpan.Zero },
            { typeof(SortBy), _ => SortBy.Score },
            { typeof(SortDirection), _ => SortDirection.Ascending },
            { typeof(SortDirection?), _ => null },
            { typeof(AppActivityLogLevel?), _ => null },
            { typeof(AppActivitySource?), _ => null },
            { typeof(AppActivityComponentType?), _ => null },
            { typeof(ManagedAppPermissionType), _ => ManagedAppPermissionType.Everyone },
            { typeof(AppPermissionType), _ => AppPermissionType.Everyone },
            { typeof(ChannelRestrictionMode?), _ => null },
            { typeof(WorkflowAuthStrategy?), _ => null },
            { typeof(RichLinkPreviewType?), _ => null },
            { typeof(IEnumerable<RichLinkPreviewType>), _ => Enumerable.Empty<RichLinkPreviewType>() },
            { typeof(AppDomainRestrictions), _ => null },
            { typeof(IEnumerable<long>), _ => Enumerable.Empty<long>() },
            { typeof(AuthPolicyEntityType), _ => AuthPolicyEntityType.User },
            { typeof(AuthPolicyEntityType?), _ => null },
            { typeof(IEnumerable<RestrictedSubject>), _ => Enumerable.Empty<RestrictedSubject>() },
            { typeof(IEnumerable<ConversationSearchType>), _ => Enumerable.Empty<ConversationSearchType>() },
            { typeof(ConversationSearchSort?), _ => null },
            { typeof(ConversationPermission), _ => null },
            { typeof(UserConnectionStatus), _ => UserConnectionStatus.Connected },
            { typeof(ProfileFieldVisibility), _ => ProfileFieldVisibility.All },
            { typeof(Presence), _ => Presence.Active },
            { typeof(RequestPresence), _ => RequestPresence.Auto },
            { typeof(BookmarkType), _ => BookmarkType.Link },
            { typeof(TriggerPermissionType), _ => TriggerPermissionType.Everyone },
            { typeof(TriggerPermissionType?), _ => TriggerPermissionType.Everyone },
            { typeof(FunctionPermissionType?), _ => FunctionPermissionType.Everyone },
            { typeof(IEnumerable<FunctionPermissionSetting>), _ => Enumerable.Empty<FunctionPermissionSetting>() },
            { typeof(PermissionVisibility?), _ => null },
            { typeof(IEnumerable<TriggerTypePermissionSetting>), _ => Enumerable.Empty<TriggerTypePermissionSetting>() },
            { typeof(WorkflowSource?), _ => null },
            { typeof(WorkflowSearchSort?), _ => null },
            { typeof(PublishStatus?), _ => null },
            { typeof(ChangeType?), _ => null },
            { typeof(LinkSource), _ => LinkSource.Composer },
            { typeof(AccessLevel), _ => AccessLevel.Read },
            { typeof(AccessLevel?), _ => null },
            { typeof(CanvasContentType?), _ => null },
            { typeof(PermissionAction), _ => PermissionAction.Upgrade },
            { typeof(IEnumerable<ConversationType>), _ => Enumerable.Empty<ConversationType>() },
            { typeof(IEnumerable<FileType>), _ => Enumerable.Empty<FileType>() },
            { typeof(IEnumerable<Block>), _ => Enumerable.Empty<Block>() },
            { typeof(IEnumerable<ContentType>), _ => Enumerable.Empty<ContentType>() },
            { typeof(IEnumerable<RichTextBlock>), _ => Enumerable.Empty<RichTextBlock>() },
            { typeof(UserProfile), _ => new UserProfile() },
            { typeof(ViewDefinition), _ => new ModalViewDefinition() },
            { typeof(HomeViewDefinition), _ => new HomeViewDefinition() },
            { typeof(IEnumerable<CallUser>), _ => Enumerable.Empty<CallUser>() },
            { typeof(byte[]), _ => Array.Empty<byte>() },
            { typeof(Stream), _ => Stream.Null },
            { typeof(Args), _ => new Args() },
            { typeof(IDictionary<string, Attachment>), _ => new Dictionary<string, Attachment>() },
            { typeof(IDictionary<string, string>), _ => new Dictionary<string, string>() },
            { typeof(IDictionary<string, object>), _ => new Dictionary<string, object>() },
            { typeof(IEnumerable<ExternalFileReference>), _ => Enumerable.Empty<ExternalFileReference>() },
            { typeof(FileUpload), _ => new FileUpload("foo", string.Empty) },
            { typeof(IEnumerable<FileUpload>), _ => Enumerable.Empty<FileUpload>() },
            { typeof(IEnumerable<AssistantPrompt>), _ => Enumerable.Empty<AssistantPrompt>() },
            { typeof(IEnumerable<CanvasChange>), _ => Enumerable.Empty<CanvasChange>() },
            { typeof(IEnumerable<SlackConnectPref>), _ => Enumerable.Empty<SlackConnectPref>() },
            { typeof(IEnumerable<ColumnSchema>), _ => Enumerable.Empty<ColumnSchema>() },
            { typeof(IEnumerable<ListItemField>), _ => Enumerable.Empty<ListItemField>() },
            { typeof(IEnumerable<ListItemFieldUpdate>), _ => Enumerable.Empty<ListItemFieldUpdate>() },
            { typeof(DocumentContent), _ => new DocumentContent { Markdown = string.Empty } },
            { typeof(LookupCriteria), _ => new LookupCriteria() },
            { typeof(ApprovalMessage), _ => new ApprovalMessage() },
            { typeof(MessageMetadata), _ => null },
            { typeof(UnfurlMetadata), _ => null },
            { typeof(WorkObjectEntity), _ => null },
            { typeof(EntityError), _ => null },
            { typeof(IEnumerable<EntityComment>), _ => Enumerable.Empty<EntityComment>() },
            { typeof(EntityComment), _ => null },
            { typeof(IEnumerable<StreamChunk>), _ => Enumerable.Empty<StreamChunk>() },
            { typeof(AgentSessionStatus), _ => AgentSessionStatus.Active },
            { typeof(AgentSessionStatus?), _ => null },
            { typeof(TaskDisplayMode?), _ => null },
            { typeof(SkillListFilter?), _ => null },
            { typeof(SkillAutoEnabledStatus?), _ => null },
            { typeof(SkillSort?), _ => null },
            { typeof(SkillAutoEnabledPermissionType), _ => SkillAutoEnabledPermissionType.Everyone },
            { typeof(TeamDiscoverability), _ => TeamDiscoverability.Open },
            { typeof(TeamDiscoverability?), _ => null },
            { typeof(IEnumerable<McpServer>), _ => Enumerable.Empty<McpServer>() },
            { typeof(IEnumerable<AgentCommand>), _ => Enumerable.Empty<AgentCommand>() },
            { typeof(CodeChannelProperties), _ => null },
            { typeof(AgentResource), _ => null },
            { typeof(AppManifest), _ => null },
            { typeof(JObject), _ => new JObject() },
            { typeof(IEnumerable<JObject>), _ => Enumerable.Empty<JObject>() },
            { typeof(CancellationToken), _ => null }
        };

    private static IEnumerable<Type> ApiClasses => typeof(ApiApi).Assembly
        .GetExportedTypes()
        .Where(t => t.IsClass && t.Name.EndsWith("Api"));

    private class FakeClient : ISlackApiClient
    {
        public string SlackMethod { get; private set; }
        public Args Args { get; private set; }
        public Type ResponseType { get; private set; }

        public void Reset()
        {
            SlackMethod = null;
            Args = null;
            ResponseType = null;
        }

        public Task Get(string apiMethod, Args args, CancellationToken cancellationToken)
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(0);
        }

        public Task<T> Get<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            ResponseType = typeof(T);
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task<Stream> GetStream(string apiMethod, Args args, CancellationToken cancellationToken)
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(Stream.Null);
        }

        public Task Post(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken)
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(0);
        }

        public Task Post(string apiMethod, Args args, CancellationToken cancellationToken) =>
            Post<object>(apiMethod, args, cancellationToken);

        public Task<T> Post<T>(string apiMethod, Args args, CancellationToken cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            ResponseType = typeof(T);
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            ResponseType = typeof(T);
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task PostToWebhook(string webhookUrl, Message message, CancellationToken cancellationToken) => throw new NotImplementedException();

        public ISlackApiClient WithAccessToken(string accessToken) => this;

        public IAdminAnalyticsApi AdminAnalytics { get; }
        public IAdminAnalyticsMessagesApi AdminAnalyticsMessages { get; }
        public IAdminAnalyticsSlackbotApi AdminAnalyticsSlackbot { get; }
        public IAdminAppsApi AdminApps { get; }
        public IAdminAppsActivitiesApi AdminAppsActivities { get; }
        public IAdminAppsApprovedApi AdminAppsApproved { get; }
        public IAdminAppsConfigApi AdminAppsConfig { get; }
        public IAdminAppsMcpServersApi AdminAppsMcpServers { get; }
        public IAdminAppsMcpServersPermissionsApi AdminAppsMcpServersPermissions { get; }
        public IAdminAppsPermissionsApi AdminAppsPermissions { get; }
        public IAdminAppsRequestsApi AdminAppsRequests { get; }
        public IAdminAppsRestrictedApi AdminAppsRestricted { get; }
        public IAdminAuditAnomalyAllowApi AdminAuditAnomalyAllow { get; }
        public IAdminAuthPolicyApi AdminAuthPolicy { get; }
        public IAdminBarriersApi AdminBarriers { get; }
        public IAdminConversationsApi AdminConversations { get; }
        public IAdminConversationsEkmApi AdminConversationsEkm { get; }
        public IAdminConversationsRestrictAccessApi AdminConversationsRestrictAccess { get; }
        public IAdminEmojiApi AdminEmoji { get; }
        public IAdminFunctionsApi AdminFunctions { get; }
        public IAdminFunctionsPermissionsApi AdminFunctionsPermissions { get; }
        public IAdminInviteRequestsApi AdminInviteRequests { get; }
        public IAdminInviteRequestsApprovedApi AdminInviteRequestsApproved { get; }
        public IAdminInviteRequestsDeniedApi AdminInviteRequestsDenied { get; }
        public IAdminRolesApi AdminRoles { get; }
        public IAdminSlackbotSkillsApi AdminSlackbotSkills { get; }
        public IAdminTeamsApi AdminTeams { get; }
        public IAdminTeamsAdminsApi AdminTeamsAdmins { get; }
        public IAdminTeamsOwnersApi AdminTeamsOwners { get; }
        public IAdminTeamsSettingsApi AdminTeamsSettings { get; }
        public IAdminUserGroupsApi AdminUserGroups { get; }
        public IAdminUsersApi AdminUsers { get; }
        public IAdminUsersAccessApi AdminUsersAccess { get; }
        public IAdminUsersSessionApi AdminUsersSession { get; }
        public IAdminUsersUnsupportedVersionsApi AdminUsersUnsupportedVersions { get; }
        public IAdminWorkflowsApi AdminWorkflows { get; }
        public IAdminWorkflowsCollaboratorsApi AdminWorkflowsCollaborators { get; }
        public IAdminWorkflowsPermissionsApi AdminWorkflowsPermissions { get; }
        public IAdminWorkflowsTriggersTypesPermissionsApi AdminWorkflowsTriggersTypesPermissions { get; }
        public IAgentsConversationsApi AgentsConversations { get; }
        public IAgentsSessionsApi AgentsSessions { get; }
        public IApiApi Api { get; }
        public IAppsApi Apps { get; }
        public IAppsConnectionsApi AppsConnections { get; }
        public IAppsEventAuthorizationsApi AppsEventAuthorizations { get; }
        public IAppsIconApi AppsIcon { get; }
        public IAppsManagedPermissionsApi AppsManagedPermissions { get; }
        public IAppsManifestApi AppsManifest { get; }
        public IAppsUserConnectionApi AppsUserConnection { get; }
        public IAssistantSearchApi AssistantSearch { get; }
        public IAssistantThreadsApi AssistantThreads { get; }
        public IAuthApi Auth { get; }
        public IBlocksApi Blocks { get; }
        public IBookmarksApi Bookmarks { get; }
        public IBotsApi Bots { get; }
        public ICallParticipantsApi CallParticipants { get; }
        public ICallsApi Calls { get; }
        public ICanvasesApi Canvases { get; }
        public IChatApi Chat { get; }
        public IConversationsApi Conversations { get; }
        public IDialogApi Dialog { get; }
        public IDndApi Dnd { get; }
        public IEmojiApi Emoji { get; }
        public IEntityApi Entity { get; }
        public IExternalTeamsApi ExternalTeams { get; }
        public IFileCommentsApi FileComments { get; }
        public IFilesApi Files { get; }
        public IFunctionsApi Functions { get; }
        public IFunctionDistributionPermissionsApi FunctionDistributionPermissions { get; }
        public IFunctionWorkflowStepsApi FunctionWorkflowSteps { get; }
        public IListApi List { get; }
        public IListDownloadApi ListDownload { get; }
        public IListItemsApi ListItems { get; }
        public IListAccessApi ListAccess { get; }
        public IMigrationApi Migration { get; }
        public IOAuthApi OAuth { get; }
        public IOAuthV2Api OAuthV2 { get; }
        public IOpenIdApi OpenIdApi { get; }
        public IPinsApi Pins { get; }
        public IReactionsApi Reactions { get; }
        public IRemindersApi Reminders { get; }
        public IRemoteFilesApi RemoteFiles { get; }
        public IRtmApi Rtm { get; }
        public IScheduledMessagesApi ScheduledMessages { get; }
        public ISearchApi Search { get; }
        public ISlackbotSkillsApi SlackbotSkills { get; }
        public ISlackbotSkillsLibraryApi SlackbotSkillsLibrary { get; }
        public ITeamApi Team { get; }
        public ITeamBillingApi TeamBilling { get; }
        public ITeamPreferencesApi TeamPreferences { get; }
        public ITeamProfileApi TeamProfile { get; }
        public IToolingApi Tooling { get; }
        public IUserGroupsApi UserGroups { get; }
        public IUserGroupUsersApi UserGroupUsers { get; }
        public IUsersApi Users { get; }
        public IUserProfileApi UserProfile { get; }
        public IViewsApi Views { get; }
        public IWorkflowsFeaturedApi WorkflowsFeatured { get; }
        public IWorkflowTriggerPermissionsApi WorkflowTriggerPermissions { get; }
    }

    class FakeHttp : IHttp
    {
        public Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) => Task.FromResult(Activator.CreateInstance<T>());
        public Task<HttpResponseMessage> ExecuteStreaming(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) => Task.FromResult(new HttpResponseMessage());
    }
}

/// <summary>
/// A call an API method makes: the Slack method, the names of the args it can send,
/// and the type its response is deserialized into, or null if the response is ignored.
/// </summary>
internal record SlackMethodCall(string SlackMethod, IReadOnlyCollection<string> ArgNames, Type ResponseType);
