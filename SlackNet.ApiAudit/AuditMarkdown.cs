using System.Text;

namespace SlackNet.ApiAudit;

public static class AuditMarkdown
{
    public static string Render(AuditReport report)
    {
        var text = new StringBuilder("# Slack API audit\n\n");
        text.AppendLine($"Generated {report.GeneratedAt:O}. Scope: users.*, conversations.*, chat.*.");
        text.AppendLine("\nFindings are candidates, not confirmed API contracts. Live probes sample a single page; missing fields never imply removal. SDK and changelog snapshots are change signals for investigation, not parsed schemas.\n");
        text.AppendLine("## Findings\n\n| ID | Method | Finding | Path | Model | Evidence | Disposition |\n| --- | --- | --- | --- | --- | --- | --- |");
        foreach (var finding in report.Findings)
            text.AppendLine($"| {finding.Id} | {finding.Method} | {finding.Kind} | {finding.Path} | {finding.Model} | {finding.Evidence}: {finding.Source} | {finding.Disposition}{(finding.Reason == null ? "" : $": {finding.Reason}")} |");
        if (report.Findings.Count == 0)
            text.AppendLine("\nNo discrepancies found in the covered sources and scenarios.");

        text.AppendLine("\n## Coverage\n\n| Method | Evidence | Status | Detail |\n| --- | --- | --- | --- |");
        foreach (var observation in report.Observations)
            text.AppendLine($"| {observation.Method} | {observation.Evidence} | {observation.Status} | {observation.Detail} |");
        text.AppendLine("\n## Sources\n\n| Source | Status | Change | SHA-256 |\n| --- | --- | --- | --- |");
        foreach (var source in report.Sources)
            text.AppendLine($"| {source.Url} | {source.Status} | {source.Change} | {source.Sha256} |");

        text.AppendLine("\n## Candidate PR handoff\n\nChoose one related group of review findings. Inspect cached docs and SDK types; corroborate undocumented behavior with dev-workspace probes. Use existing types; preserve public-interface compatibility. A parameter being accepted does not prove it has an effect. Include concise Unreleased notes and behavior tests where existing convention coverage is insufficient. The PR must list finding IDs, source hashes/links, live scenarios and unverified behavior. Reuse an existing PR with those IDs. Never merge automatically.\n");
        return text.ToString();
    }
}
