using Newtonsoft.Json;
using SlackNet.ApiAudit;

var options = args.Chunk(2).ToDictionary(pair => pair[0], pair => pair.Length == 2 ? pair[1] : throw new ArgumentException("Options require values."));
var allowed = new[] { "--output", "--offline", "--live", "--token-file", "--channel", "--user", "--team", "--suppressions" };
if (options.Keys.Except(allowed).Any())
    throw new ArgumentException($"Supported options: {string.Join(", ", allowed)}. Boolean options take true/false.");

var output = Path.GetFullPath(Option("--output", "artifacts/api-audit"));
var offline = bool.Parse(Option("--offline", "false"));
var live = bool.Parse(Option("--live", "false"));
if (live && offline)
    throw new ArgumentException("Offline replay cannot make live requests.");
var token = !live ? null : Option("--token-file") is { } tokenFile
    ? (await File.ReadAllTextAsync(tokenFile)).Trim()
    : Environment.GetEnvironmentVariable("SLACK_API_AUDIT_TOKEN");
if (live && string.IsNullOrWhiteSpace(token))
    throw new ArgumentException("Live probes require SLACK_API_AUDIT_TOKEN or --token-file. Never put a token in command-line arguments.");
var suppressionsFile = Option("--suppressions", "docs/api-audit-suppressions.json");
var suppressions = JsonConvert.DeserializeObject<Suppression[]>(await File.ReadAllTextAsync(suppressionsFile))
    ?? throw new FormatException("Suppressions must be an array.");
if (suppressions.Any(s => string.IsNullOrWhiteSpace(s.Id) || string.IsNullOrWhiteSpace(s.Reason)) || suppressions.DistinctBy(s => s.Id).Count() != suppressions.Length)
    throw new FormatException("Each suppression needs a unique finding ID and a reason.");

using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(45) };
http.DefaultRequestHeaders.UserAgent.ParseAdd("SlackNet-ApiAudit/1.0");
var report = await new AuditRunner(http, output, offline).Run(token, Option("--channel"), Option("--user"), Option("--team"), suppressions);
await File.WriteAllTextAsync(Path.Combine(output, "report.json"), JsonConvert.SerializeObject(report, Formatting.Indented));
await File.WriteAllTextAsync(Path.Combine(output, "report.md"), AuditMarkdown.Render(report));
Console.WriteLine($"{report.Findings.Count(f => f.Disposition == "Review")} findings; report: {Path.Combine(output, "report.md")}");
Console.WriteLine($"Live coverage: {report.Observations.Count(o => o.Evidence == "Live" && o.Status == "Checked")} checked; {report.Observations.Count(o => o.Evidence == "Live" && o.Status != "Checked")} incomplete.");
return report.Sources.Any(s => s.Status == "Failed") || report.Observations.Any(o => o.Status == "Failed") ? 1 : 0;

string Option(string name, string fallback = null) => options.GetValueOrDefault(name) is { Length: > 0 } value ? value : fallback;
