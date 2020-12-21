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
using SlackNet.Interaction;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.Tests
{
    public class ApiLintTest
    {
        [TestCaseSource(nameof(ApiClasses))]
        public void Lint(Type api)
        {
            var client = new FakeClient();
            var instance = Activator.CreateInstance(api, client);

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
            lastParam.ParameterType.ShouldBe(typeof(CancellationToken?), $"{method.DeclaringType.Name}.{method.Name} is missing CancellationToken param");
            lastParam.DefaultValue.ShouldBeNull($"{method.DeclaringType.Name}.{method.Name} CancellationToken param isn't null by default");
        }

        private static void AllMethodsShouldUseSameSlackMethodGroup(FakeClient client, MethodInfo method, ref string slackMethodGroup)
        {
            var methodGroup = string.Join('.', client.SlackMethod.Split('.').SkipLast(1));
            if (slackMethodGroup == null)
                slackMethodGroup = methodGroup;
            else
                methodGroup.ShouldBe(slackMethodGroup, $"{method.DeclaringType.Name}.{method.Name} uses a different API method group than other methods");
        }

        private static void AllArgsShouldBeSnakeCase(Args args, MethodInfo method) =>
            args.Keys.AllItemsSatisfy(arg => arg.ShouldMatch("[a-z_]", $"{method.DeclaringType.Name}.{method.Name} has incorrect casing for argument {arg}"));

        private static object DummyValue(ParameterInfo param) =>
            param.ParameterType == typeof(string) && param.Name == "contentType" ? "text/png"
            : param.ParameterType == typeof(string) ? "foo"
            : param.ParameterType == typeof(int) ? 0
            : param.ParameterType == typeof(int?) ? null
            : param.ParameterType == typeof(bool) ? false
            : param.ParameterType == typeof(IEnumerable<string>) ? Enumerable.Empty<string>()
            : param.ParameterType == typeof(Message) ? new Message()
            : param.ParameterType == typeof(Dialog) ? new Dialog()
            : param.ParameterType == typeof(MessageUpdate) ? new MessageUpdate()
            : param.ParameterType == typeof(DateTime) ? DateTime.Now
            : param.ParameterType == typeof(TimeSpan) ? TimeSpan.Zero
            : param.ParameterType == typeof(SortBy) ? SortBy.Score
            : param.ParameterType == typeof(SortDirection) ? SortDirection.Ascending
            : param.ParameterType == typeof(ProfileFieldVisibility) ? ProfileFieldVisibility.All
            : param.ParameterType == typeof(Presence) ? Presence.Active
            : param.ParameterType == typeof(ChangeType?) ? null
            : param.ParameterType == typeof(IEnumerable<ConversationType>) ? Enumerable.Empty<ConversationType>()
            : param.ParameterType == typeof(IEnumerable<FileType>) ? Enumerable.Empty<FileType>()
            : param.ParameterType == typeof(UserProfile) ? new UserProfile()
            : param.ParameterType == typeof(ViewDefinition) ? new ModalViewDefinition()
            : param.ParameterType == typeof(HomeViewDefinition) ? new HomeViewDefinition()
            : param.ParameterType == typeof(byte[]) ? new byte[0]
            : param.ParameterType == typeof(Stream) ? Stream.Null
            : param.ParameterType == typeof(Args) ? new Args()
            : param.ParameterType == typeof(IDictionary<string, Attachment>) ? new Dictionary<string, Attachment>()
            : param.ParameterType == typeof(IDictionary<string, WorkflowInput>) ? new Dictionary<string, WorkflowInput>()
            : param.ParameterType == typeof(IDictionary<string, string>) ? new Dictionary<string, string>()
            : param.ParameterType == typeof(IEnumerable<WorkflowOutput>) ? Enumerable.Empty<WorkflowOutput>()
            : param.ParameterType == typeof(WorkflowError) ? new WorkflowError()
            : param.ParameterType == typeof(CancellationToken?) ? (object)null
            : throw new AssertionException($"Unexpected param type {param.ParameterType.Name} in {param.Member.DeclaringType.Name}.{param.Member.Name}");

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

            public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken? cancellationToken)
            {
                throw new NotImplementedException();
            }

            public ISlackApiClient WithAccessToken(string accessToken)
            {
                throw new NotImplementedException();
            }

            public IApiApi Api { get; }
            public IAppsEventsAuthorizationsApi AppsEventsAuthorizations { get; }
            public IAuthApi Auth { get; }
            public IBotsApi Bots { get; }
            public IChannelsApi Channels { get; }
            public IChatApi Chat { get; }
            public IConversationsApi Conversations { get; }
            public IDialogApi Dialog { get; }
            public IDndApi Dnd { get; }
            public IEmojiApi Emoji { get; }
            public IFileCommentsApi FileComments { get; }
            public IFilesApi Files { get; }
            public IGroupsApi Groups { get; }
            public IImApi Im { get; }
            public IMigrationApi Migration { get; }
            public IMpimApi Mpim { get; }
            public IOAuthApi OAuth { get; }
            public IPinsApi Pins { get; }
            public IReactionsApi Reactions { get; }
            public IRemindersApi Reminders { get; }
            public IRemoteFilesApi RemoteFiles { get; }
            public IRtmApi Rtm { get; }
            public IScheduledMessagesApi ScheduledMessages { get; }
            public ISearchApi Search { get; }
            public IStarsApi Stars { get; }
            public ITeamApi Team { get; }
            public ITeamProfileApi TeamProfile { get; }
            public IUserGroupsApi UserGroups { get; }
            public IUserGroupUsersApi UserGroupUsers { get; }
            public IUsersApi Users { get; }
            public IUserProfileApi UserProfile { get; }
            public IViewsApi Views { get; }
            public IWorkflowsApi Workflows { get; }
        }
    }
}