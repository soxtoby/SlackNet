using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.ApiAudit;

public record MethodDocumentation(IReadOnlySet<string> Parameters, IReadOnlyList<JObject> Examples, int UnparseableExamples);

public static class SlackDocumentation
{
    public static MethodDocumentation Parse(string markdown)
    {
        var arguments = Section(markdown, "Arguments");
        var parameters = Regex.Matches(arguments, @"^\*\*`(?<name>[a-zA-Z0-9_]+)`\*\*", RegexOptions.Multiline)
            .Select(m => m.Groups["name"].Value)
            .Where(n => n != "token")
            .ToHashSet();
        if (parameters.Count == 0 && !arguments.Contains("**`token`**"))
            throw new FormatException("No recognizable argument definitions.");

        var response = Section(markdown, "Response", required: false);
        var examples = new List<JObject>();
        var unparseable = 0;
        foreach (Match code in Regex.Matches(response, @"^```(?:json)?\s*\r?\n(?<json>[\s\S]*?)^```", RegexOptions.Multiline))
        {
            if (code.Groups["json"].Value.TrimStart().StartsWith('{'))
            {
                try
                {
                    var example = JObject.Parse(code.Groups["json"].Value);
                    if (example.Value<bool?>("ok") == true)
                        examples.Add(example);
                }
                catch (JsonException)
                {
                    unparseable++;
                }
            }
        }

        return new MethodDocumentation(parameters, examples, unparseable);
    }

    private static string Section(string markdown, string heading, bool required = true)
    {
        var match = Regex.Match(markdown, $@"^## {heading}(?:[ \t][^\r\n]*)?\r?\n(?<body>[\s\S]*?)(?=^## |\z)", RegexOptions.Multiline);
        if (required && !match.Success)
            throw new FormatException($"Missing {heading} section.");
        return match.Groups["body"].Value;
    }
}
