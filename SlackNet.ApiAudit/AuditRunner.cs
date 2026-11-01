using System.Net.Http.Headers;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.ApiAudit;

public record SourceCheck(string Url, string File, string Status, string Sha256 = null, string Change = null);
public record Observation(string Method, string Evidence, string Status, string Detail, IReadOnlyList<Shape> Shapes);
public record Suppression(string Id, string Reason);
public record AuditReport(DateTime GeneratedAt, IReadOnlyList<SourceCheck> Sources, IReadOnlyList<Observation> Observations, IReadOnlyList<Finding> Findings);

public class AuditRunner(HttpClient publicHttp, string output, bool offline)
{
    private readonly List<SourceCheck> _sources = [];
    private readonly List<Observation> _observations = [];
    private readonly List<Finding> _findings = [];
    private static readonly string[] Families = ["users.", "conversations.", "chat."];

    public async Task<AuditReport> Run(string token, string channel, string user, string team, IReadOnlyList<Suppression> suppressions)
    {
        Directory.CreateDirectory(Path.Combine(output, "sources"));
        var inventory = await ApiInventory.Read();
        var index = await Fetch("https://docs.slack.dev/reference/methods.json", "methods.json");
        var names = MethodNames(index);

        foreach (var name in names)
        {
            Console.WriteLine($"Checking {name}");
            await CheckMethod(name, inventory);
        }

        await Fetch("https://docs.slack.dev/changelog/rss.xml", "changelog.xml");
        foreach (var file in new[] { "request/users.ts", "request/conversations.ts", "request/chat.ts", "request/common.ts",
                     "response/UsersInfoResponse.ts", "response/ConversationsInfoResponse.ts", "response/ConversationsHistoryResponse.ts" })
            await Fetch($"https://raw.githubusercontent.com/slackapi/node-slack-sdk/main/packages/web-api/src/types/{file}",
                $"sdk-{file.Replace('/', '-')}");

        if (token != null)
        {
            using var liveHttp = new HttpClient { Timeout = TimeSpan.FromSeconds(45) };
            liveHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await new LiveProbes(liveHttp, inventory, _findings, _observations).Run(channel, user, team);
        }
        else
            _observations.Add(new Observation("Live probes", "Live", "Not run", "No live run requested. Workspace behavior is unverified.", []));

        var findings = _findings.DistinctBy(f => (f.Id, f.Evidence, f.Source)).OrderBy(f => f.Method).ThenBy(f => f.Path).ToArray();
        foreach (var finding in findings)
        {
            var suppression = suppressions.SingleOrDefault(s => s.Id == finding.Id);
            if (suppression != null)
            {
                finding.Disposition = "Suppressed";
                finding.Reason = suppression.Reason;
            }
        }

        return new AuditReport(DateTime.UtcNow, _sources, _observations, findings);
    }

    private string[] MethodNames(string index)
    {
        if (index == null)
            return [];

        try
        {
            var names = JArray.Parse(index).Select(m => m.Value<string>("name"))
                .Where(n => n != null && Families.Any(n.StartsWith)).Order().ToArray();
            if (names.Length == 0)
                throw new FormatException();
            return names;
        }
        catch (Exception e) when (e is JsonException or FormatException or InvalidOperationException)
        {
            _observations.Add(new Observation("Method index", "Documentation", "Failed", "Unrecognized method index; no method coverage established.", []));
            return [];
        }
    }

    private async Task CheckMethod(string name, IReadOnlyDictionary<string, ApiMethod> inventory)
    {
        var url = $"https://docs.slack.dev/reference/methods/{name}.md";
        var markdown = await Fetch(url, $"{name}.md");
        if (!inventory.TryGetValue(name, out var implemented))
            _findings.Add(Finding.Create("Missing method", name, "$", "", "Documentation", url));

        if (markdown != null && implemented != null)
        {
            try
            {
                var documentation = SlackDocumentation.Parse(markdown);
                foreach (var parameter in documentation.Parameters.Except(implemented.Parameters).Order())
                    _findings.Add(Finding.Create("Missing parameter", name, parameter, "", "Documentation", url));

                foreach (var example in documentation.Examples)
                {
                    var audits = implemented.ResponseTypes.Select(type =>
                    {
                        var audit = new ResponseAudit();
                        audit.Inspect(example, type, name, "Documentation example", url);
                        return audit;
                    }).ToArray();
                    var best = audits.OrderBy(a => a.Findings.Count).First();
                    _findings.AddRange(best.Findings);
                }
                _observations.Add(new Observation(name, "Documentation example", documentation.Examples.Count == 0 ? "Not covered" : "Checked",
                    $"{documentation.Parameters.Count} documented parameters; {documentation.Examples.Count} success examples; {documentation.UnparseableExamples} unparseable examples.", []));
            }
            catch (Exception e) when (e is FormatException or JsonException)
            {
                _observations.Add(new Observation(name, "Documentation", "Failed", "Unrecognized documentation format; review the cached source.", []));
            }
        }
    }

    private async Task<string> Fetch(string url, string file)
    {
        var path = Path.Combine(output, "sources", file);
        var previous = System.IO.File.Exists(path) ? await System.IO.File.ReadAllTextAsync(path) : null;
        try
        {
            var content = offline
                ? previous ?? throw new FileNotFoundException()
                : await publicHttp.GetStringAsync(url);
            if (!offline)
                await System.IO.File.WriteAllTextAsync(path, content);
            _sources.Add(new SourceCheck(url, $"sources/{file}", offline ? "Cached" : "Fetched", Hash(content),
                previous == null ? "First seen" : previous == content ? "Unchanged" : "Changed"));
            return content;
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException or FileNotFoundException)
        {
            _sources.Add(new SourceCheck(url, $"sources/{file}", "Failed"));
            return null;
        }
    }

    private static string Hash(string content) => Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
}
