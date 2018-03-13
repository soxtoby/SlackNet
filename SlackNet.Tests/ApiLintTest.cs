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

            ApiClientHasAPropertyForApi(api);

            var apiMethods = api.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            apiMethods.ShouldNotBeEmpty();

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

        private static void ApiClientHasAPropertyForApi(Type api)
        {
            api.GetInterfaces().ShouldBeSingular($"{api.Name} doesn't implement an interface");
            var apiInterface = api.GetInterfaces().Single();
            typeof(ISlackApiClient).GetProperties()
                .Any(p => p.PropertyType == apiInterface)
                .ShouldBe(true, $"{api.Name} doesn't have a property on {nameof(ISlackApiClient)}");
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
            : param.ParameterType == typeof(byte[]) ? new byte[0]
            : param.ParameterType == typeof(Stream) ? Stream.Null
            : param.ParameterType == typeof(Args) ? new Args()
            : param.ParameterType == typeof(IDictionary<string, Attachment>) ? new Dictionary<string, Attachment>()
            : param.ParameterType == typeof(CancellationToken?) ? (object)null
            : throw new AssertionException($"Unexpected param type {param.ParameterType.Name} in {param.Member.DeclaringType.Name}.{param.Member.Name}");

        static IEnumerable<Type> ApiClasses => typeof(ApiApi).Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Api"));

        class FakeClient : ISlackApiClient
        {
            public string HttpMethod { get; set; }
            public string SlackMethod { get; set; }
            public Args Args { get; set; }
            public CancellationToken? CancellationToken { get; set; }

            public void Reset()
            {
                HttpMethod = null;
                SlackMethod = null;
                Args = null;
            }

            public Task Get(string apiMethod, Args args, CancellationToken? cancellationToken)
            {
                HttpMethod = "GET";
                SlackMethod = apiMethod;
                Args = args;
                CancellationToken = cancellationToken;
                return Task.FromResult(0);
            }

            public Task<T> Get<T>(string apiMethod, Args args, CancellationToken? cancellationToken) where T : class
            {
                HttpMethod = "GET";
                SlackMethod = apiMethod;
                Args = args;
                CancellationToken = cancellationToken;
                return Task.FromResult(Activator.CreateInstance<T>());
            }

            public Task Post(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken)
            {
                HttpMethod = "POST";
                SlackMethod = apiMethod;
                Args = args;
                CancellationToken = cancellationToken;
                return Task.FromResult(0);
            }

            public Task<T> Post<T>(string apiMethod, Args args, HttpContent content, CancellationToken? cancellationToken) where T : class
            {
                HttpMethod = "POST";
                SlackMethod = apiMethod;
                Args = args;
                CancellationToken = cancellationToken;
                return Task.FromResult(Activator.CreateInstance<T>());
            }

            public IApiApi Api { get; }
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
            public IRtmApi Rtm { get; }
            public ISearchApi Search { get; }
            public IStarsApi Stars { get; }
            public ITeamApi Team { get; }
            public ITeamProfileApi TeamProfile { get; }
            public IUserGroupsApi UserGroups { get; }
            public IUserGroupUsersApi UserGroupUsers { get; }
            public IUsersApi Users { get; }
            public IUserProfileApi UserProfile { get; }
        }
    }
}