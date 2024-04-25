using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.Tests;

public class ApiLintTest
{
    [TestCaseSource(nameof(ApiClasses))]
    public void Lint(Type api)
    {
        var client = new FakeClient();
        var instance = GetConstructor<SlackJsonSettings>(api) is not null ? Activator.CreateInstance(api, client, Default.JsonSettings())
            : GetConstructor<IHttp>(api) is not null ? Activator.CreateInstance(api, client, new FakeHttp())
            : Activator.CreateInstance(api, client);

        var apiInterface = GetApiInterface(api);

        ApiClientHasAPropertyForApi(api, apiInterface);

        var apiMethods = api.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

        AllPublicMethodsAreExposedOnInterface(apiInterface, apiMethods);

        string slackMethodGroup = null;

        foreach (var method in apiMethods)
        {
            client.Reset();
            method.Invoke(instance, method.GetParameters().Select(DummyValue).ToArray());

            LastParamShouldBeOptionalCancellationToken(method);
            AllMethodsShouldUseSameSlackMethodGroup(client, method, ref slackMethodGroup);
            AllArgsShouldBeSnakeCase(client.Args, method);
        }
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
            .Any(p => p.PropertyType == apiInterface)
            .ShouldBe(true, $"{api.Name} doesn't have a property on {nameof(ISlackApiClient)}");
    }

    private static void AllPublicMethodsAreExposedOnInterface(Type apiInterface, MethodInfo[] apiMethods)
    {
        apiMethods.ShouldNotBeEmpty();

        var interfaceMethods = apiInterface.GetMethods();

        apiMethods.ShouldOnlyContain(interfaceMethods, MethodsEqual);
    }

    private static bool MethodsEqual(MethodInfo a, MethodInfo b) =>
        a.Name == b.Name
        && ParameterTypes(a).SequenceEqual(ParameterTypes(b));

    private static IEnumerable<Type> ParameterTypes(MethodInfo method)
    {
        return method.GetParameters().Select(p => p.ParameterType);
    }

    private static void LastParamShouldBeOptionalCancellationToken(MethodInfo method)
    {
        var lastParam = method.GetParameters().Last();
        lastParam.ParameterType.ShouldBe(typeof(CancellationToken?), $"{method.DeclaringType!.Name}.{method.Name} is missing CancellationToken param");
        lastParam.DefaultValue.ShouldBeNull($"{method.DeclaringType.Name}.{method.Name} CancellationToken param isn't null by default");
    }

    private static void AllMethodsShouldUseSameSlackMethodGroup(FakeClient client, MethodInfo method, ref string slackMethodGroup)
    {
        var methodGroup = string.Join('.', client.SlackMethod.Split('.').SkipLast(1));
        if (slackMethodGroup == null)
            slackMethodGroup = methodGroup;
        else
            methodGroup.ShouldBe(slackMethodGroup, $"{method.DeclaringType!.Name}.{method.Name} uses a different API method group than other methods");
    }

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
            { typeof(bool), _ => false },
            { typeof(bool?), _ => null },
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
            { typeof(ProfileFieldVisibility), _ => ProfileFieldVisibility.All },
            { typeof(Presence), _ => Presence.Active },
            { typeof(RequestPresence), _ => RequestPresence.Auto },
            { typeof(BookmarkType), _ => BookmarkType.Link },
            { typeof(ChangeType?), _ => null },
            { typeof(LinkSource), _ => LinkSource.Composer },
            { typeof(IEnumerable<ConversationType>), _ => Enumerable.Empty<ConversationType>() },
            { typeof(IEnumerable<FileType>), _ => Enumerable.Empty<FileType>() },
            { typeof(IEnumerable<Block>), _ => Enumerable.Empty<Block>() },
            { typeof(UserProfile), _ => new UserProfile() },
            { typeof(ViewDefinition), _ => new ModalViewDefinition() },
            { typeof(HomeViewDefinition), _ => new HomeViewDefinition() },
            { typeof(IEnumerable<CallUser>), _ => Enumerable.Empty<CallUser>() },
            { typeof(byte[]), _ => Array.Empty<byte>() },
            { typeof(Stream), _ => Stream.Null },
            { typeof(Args), _ => new Args() },
            { typeof(IDictionary<string, Attachment>), _ => new Dictionary<string, Attachment>() },
            { typeof(IDictionary<string, WorkflowInput>), _ => new Dictionary<string, WorkflowInput>() },
            { typeof(IDictionary<string, string>), _ => new Dictionary<string, string>() },
            { typeof(IEnumerable<WorkflowOutput>), _ => Enumerable.Empty<WorkflowOutput>() },
            { typeof(WorkflowError), _ => new WorkflowError() },
            { typeof(IEnumerable<ExternalFileReference>), _ => Enumerable.Empty<ExternalFileReference>() },
            { typeof(FileUpload), _ => new FileUpload(string.Empty, string.Empty) },
            { typeof(IEnumerable<FileUpload>), _ => Enumerable.Empty<FileUpload>() },
            { typeof(CancellationToken?), _ => null },
        };

    private static IEnumerable<Type> ApiClasses => typeof(ApiApi).Assembly
        .GetExportedTypes()
        .Where(t => t.IsClass && t.Name.EndsWith("Api"));

    private class FakeClient : ISlackApiClient
    {
        public string SlackMethod { get; private set; }
        public Args Args { get; private set; }

        public void Reset()
        {
            SlackMethod = null;
            Args = null;
        }

        public Task Get(string apiMethod, Args args, CancellationToken? cancellationToken)
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(0);
        }

        public Task<T> Get<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task Post(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken)
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(0);
        }

        public Task Post(string apiMethod, Args args, CancellationToken? cancellationToken) =>
            Post<object>(apiMethod, args, cancellationToken);

        public Task<T> Post<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken) where T : class
        {
            SlackMethod = apiMethod;
            Args = args;
            return Task.FromResult(Activator.CreateInstance<T>());
        }

        public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken? cancellationToken) => throw new NotImplementedException();

        public Task PostToWebhook(string webhookUrl, Message message, CancellationToken? cancellationToken) => throw new NotImplementedException();

        public ISlackApiClient WithAccessToken(string accessToken) => this;

        public IApiApi Api { get; }
        public IAppsConnectionsApi AppsConnectionsApi { get; }
        public IAppsEventAuthorizationsApi AppsEventAuthorizations { get; }
        public IAuthApi Auth { get; }
        public IBookmarksApi Bookmarks { get; }
        public IBotsApi Bots { get; }
        public ICallParticipantsApi CallParticipants { get; }
        public ICallsApi Calls { get; }
        public IChatApi Chat { get; }
        public IConversationsApi Conversations { get; }
        public IDialogApi Dialog { get; }
        public IDndApi Dnd { get; }
        public IEmojiApi Emoji { get; }
        public IFileCommentsApi FileComments { get; }
        public IFilesApi Files { get; }
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
        public ITeamApi Team { get; }
        public ITeamBillingApi TeamBilling { get; }
        public ITeamPreferencesApi TeamPreferences { get; }
        public ITeamProfileApi TeamProfile { get; }
        public IUserGroupsApi UserGroups { get; }
        public IUserGroupUsersApi UserGroupUsers { get; }
        public IUsersApi Users { get; }
        public IUserProfileApi UserProfile { get; }
        public IViewsApi Views { get; }
        public IWorkflowsApi Workflows { get; }
    }

    class FakeHttp : IHttp
    {
        public Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken? cancellationToken = null) => Task.FromResult(Activator.CreateInstance<T>());
    }
}