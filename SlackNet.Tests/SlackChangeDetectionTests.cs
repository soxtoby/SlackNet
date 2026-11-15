using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.Events;
using SlackNet.Rtm;

namespace SlackNet.Tests;

/// <summary>
/// Compares SlackNet against Slack's documentation, to find out about changes to Slack's API.
/// </summary>
[Explicit]
public class SlackChangeDetectionTests
{
    [Test]
    public async Task MissingApis()
    {
        using var http = new HttpClient();
        var methodList = await http.GetFromJsonAsync<NamedItem[]>("https://docs.slack.dev/reference/methods.json");
        var allApiMethods = methodList.Select(m => m.Name).ToHashSet();
        allApiMethods.ShouldNotBeEmpty();

        var missingApiMethods = allApiMethods
            .Where(m => MissingApiMethodExceptions.All(e => !m.StartsWith(e)))
            .Except(ApiLintTests.ImplementedSlackMethods());
        missingApiMethods.ShouldBeEmpty();
    }

    private static readonly string[] MissingApiMethodExceptions = [
            "apps.activities.list", // Only for Slack-hosted apps
            "apps.auth.external.", // Only for Slack-hosted apps
            "apps.datastore.", // Only for Slack-hosted apps
            "stars.", // Deprecated
            "users.setActive", // Deprecated
        ];

    [Test]
    public async Task MissingEvents()
    {
        using var http = new HttpClient();
        var eventList = await http.GetFromJsonAsync<NamedItem[]>("https://docs.slack.dev/reference/events.json");
        var allEvents = eventList
            .Select(e => e.Name.Split('.')[0]) // Message subscriptions like message.channels are all message events
            .ToHashSet();
        allEvents.ShouldNotBeEmpty();

        var implementedEvents = typeof(Event).Assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(typeof(Event))
                || t.IsAssignableTo(typeof(EventRequest)) // e.g. url_verification
                || t.IsAssignableTo(typeof(OutgoingRtmEvent))) // e.g. presence_sub
            .Select(t => t.GetTypeInfo().SlackType());

        allEvents
            .Except(MissingEventExceptions)
            .Except(implementedEvents)
            .ShouldBeEmpty();
    }

    private static readonly string[] MissingEventExceptions = [
            "star_added", // Deprecated with stars API
            "star_removed", // Deprecated with stars API
        ];

    /// <summary>
    /// Snapshots Slack's documented args and output for every implemented method into <see cref="ApiSpecSnapshotFile"/>.
    /// Fails when the documentation has changed since the snapshot was committed, leaving the updated snapshot for git to diff.
    /// </summary>
    [Test]
    public async Task ApiSpecChanges()
    {
        using var http = new HttpClient();
        var methods = ApiLintTests.ImplementedSlackMethods().Order().ToList();
        var specs = new string[methods.Count][];
        await Parallel.ForEachAsync(Enumerable.Range(0, methods.Count), new ParallelOptions { MaxDegreeOfParallelism = 8 }, async (i, ct) =>
            {
                var response = await http.GetAsync($"https://docs.slack.dev/reference/methods/{methods[i]}.json", ct);
                specs[i] = response.IsSuccessStatusCode
                    ? DescribeSpec(methods[i], JObject.Parse(await response.Content.ReadAsStringAsync(ct))).ToArray()
                    : [$"{methods[i]} undocumented ({(int)response.StatusCode})"];
            });
        var actual = specs.SelectMany(s => s).ToList();

        var expected = System.IO.File.Exists(ApiSpecSnapshotFile) 
            ? await System.IO.File.ReadAllLinesAsync(ApiSpecSnapshotFile) 
            : [];
        await System.IO.File.WriteAllLinesAsync(ApiSpecSnapshotFile, actual);

        actual.ShouldMatch(expected, $"Slack API docs have changed. Diff {ApiSpecSnapshotFile} and commit it once handled.");
    }

    private static IEnumerable<string> DescribeSpec(string method, JObject spec)
    {
        if (spec.Value<bool?>("deprecated") == true)
            yield return $"{method} deprecated";

        foreach (var line in DescribeSchema(method, "arg", WithoutIgnoredKeys(spec["args"])))
            yield return line;
        foreach (var line in DescribeSchema(method, "output", WithoutIgnoredKeys(spec["output"])))
            yield return line;
    }

    private static IEnumerable<string> DescribeSchema(string method, string kind, JToken schema)
    {
        if (schema is JObject && schema["properties"] is JObject properties)
        {
            var required = schema["required"]?.Values<string>().ToHashSet() ?? [];
            foreach (var property in properties.Properties())
            {
                var name = required.Contains(property.Name) ? property.Name + " (required)" : property.Name;
                yield return $"{method} {kind} {name} {property.Value.ToString(Formatting.None)}";
            }
        }
        else
        {
            yield return $"{method} {kind} {schema?.ToString(Formatting.None) ?? "null"}";
        }
    }

    /// <summary>
    /// Removes documentation and limits, which could change often without affecting the shape of the API.
    /// </summary>
    private static JToken WithoutIgnoredKeys(JToken token)
    {
        if (token is not JContainer container)
            return token;
        var clone = (JContainer)container.DeepClone();
        clone.Descendants()
            .OfType<JProperty>()
            .Where(p => IgnoredSpecKeys.Contains(p.Name) && p.Parent?.Parent is not JProperty { Name: "properties" })
            .ToList()
            .ForEach(p => p.Remove());
        return clone;
    }

    private static readonly HashSet<string> IgnoredSpecKeys = ["desc", "description", "example", "minLength", "maxLength", "minimum", "maximum", "minItems", "maxItems"];

    internal static string ApiSpecSnapshotFile => Path.Combine(Path.GetDirectoryName(SourceFilePath())!, "ApiSpecSnapshot.txt");

    private static string SourceFilePath([CallerFilePath] string path = null) => path;

    record NamedItem(string Name);
}
