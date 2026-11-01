using System.Reflection;
using SlackNet.WebApi;

namespace SlackNet.ApiAudit;

public record ApiMethod(string Name, IReadOnlySet<string> Parameters, Type ResponseType)
{
    public IReadOnlyList<Type> ResponseTypes { get; init; } = [ResponseType];
}

public static class ApiInventory
{
    public static async Task<IReadOnlyDictionary<string, ApiMethod>> Read()
    {
        var client = DispatchProxy.Create<ISlackApiClient, RecordingClient>();
        var recorder = (RecordingClient)client;
        var methods = new List<ApiMethod>();
        object[] apis = [new UsersApi(client), new UserProfileApi(client), new ConversationsApi(client),
            new ChatApi(client, Default.JsonSettings()), new ScheduledMessagesApi(client)];

        foreach (var api in apis)
        {
            foreach (var method in api.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                recorder.LastCall = null;
                var task = (Task)method.Invoke(api, method.GetParameters().Select(DummyValue).ToArray());
                try
                {
                    await task;
                }
                catch (RequestRecordedException)
                {
                    // Stop wrappers before they try to unwrap an invented response.
                }
                methods.Add(recorder.LastCall ?? throw new InvalidOperationException($"No Slack request recorded for {method.Name}."));
            }
        }

        return methods.GroupBy(m => m.Name).ToDictionary(g => g.Key,
            g => new ApiMethod(g.Key, g.SelectMany(m => m.Parameters).ToHashSet(), g.First().ResponseType)
                { ResponseTypes = g.Select(m => m.ResponseType).Distinct().ToArray() });
    }

    private static object DummyValue(ParameterInfo parameter) => DummyValue(parameter.ParameterType, parameter.Name);

    private static object DummyValue(Type type, string name) =>
        type == typeof(string) ? name == "contentType" ? "image/png" : "audit"
        : type.IsArray ? Array.CreateInstance(type.GetElementType()!, 0)
        : type == typeof(Stream) ? new MemoryStream()
        : type == typeof(DateTime) ? DateTime.UnixEpoch
        : type.IsValueType ? Activator.CreateInstance(type)
        : type.GetConstructor(Type.EmptyTypes) != null ? Activator.CreateInstance(type)
        : null;

    public class RecordingClient : DispatchProxy
    {
        public ApiMethod LastCall { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var responseType = targetMethod.IsGenericMethod ? targetMethod.GetGenericArguments()[0] : typeof(object);
            var parameters = ((Dictionary<string, object>)args[1]).Keys.ToHashSet();
            foreach (var multipart in args.OfType<MultipartFormDataContent>())
            {
                parameters.UnionWith(multipart.Select(part => part.Headers.ContentDisposition!.Name!.Trim('"')));
                multipart.Dispose();
            }
            LastCall = new ApiMethod((string)args[0], parameters, responseType);

            return targetMethod.IsGenericMethod
                ? typeof(Task).GetMethods().Single(m => m.Name == nameof(Task.FromException) && m.IsGenericMethod)
                    .MakeGenericMethod(responseType).Invoke(null, [new RequestRecordedException()])
                : Task.FromException(new RequestRecordedException());
        }
    }

    private class RequestRecordedException : Exception;
}
