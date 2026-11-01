using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackNet.ApiAudit;

public class LiveProbes(HttpClient http, IReadOnlyDictionary<string, ApiMethod> inventory,
    List<Finding> findings, List<Observation> observations)
{
    public async Task Run(string channel, string user, string team = null)
    {
        var auth = await Call("auth.test", []);
        if (auth == null)
            observations.Add(new Observation("Live probes", "Live", "Not covered", "Authentication failed; dependent probes were not run.", []));
        else
        {
            var channelResponse = channel == null ? null
                : await Probe("conversations.info", new() { ["channel"] = channel, ["include_num_members"] = "true", ["include_locale"] = "true" });
            var userId = user ?? auth.Value<string>("user_id");
            var teamId = team ?? channelResponse?["channel"]?.Value<string>("context_team_id") ?? auth.Value<string>("team_id");
            var usersArguments = new Dictionary<string, string> { ["limit"] = "20", ["include_locale"] = "true" };
            var conversationsArguments = new Dictionary<string, string> { ["limit"] = "20", ["exclude_archived"] = "true" };
            if (!string.IsNullOrEmpty(teamId))
            {
                usersArguments["team_id"] = teamId;
                conversationsArguments["team_id"] = teamId;
            }
            await Probe("users.list", usersArguments);
            if (userId != null)
                await Probe("users.info", new() { ["user"] = userId, ["include_locale"] = "true" });
            await Probe("conversations.list", conversationsArguments);

            if (channel != null)
                await Probe("conversations.history", new() { ["channel"] = channel, ["limit"] = "15", ["include_all_metadata"] = "true" });
            else
            {
                observations.Add(new Observation("conversations.info", "Live", "Not covered", "No fixture channel supplied.", []));
                observations.Add(new Observation("conversations.history", "Live", "Not covered", "No fixture channel supplied.", []));
            }
        }
    }

    private async Task<JObject> Probe(string method, Dictionary<string, string> arguments)
    {
        var response = await Call(method, arguments);
        if (response != null)
        {
            var audit = new ResponseAudit();
            audit.Inspect(response, inventory[method].ResponseType, method, "Live", "Dev workspace");
            findings.AddRange(audit.Findings);
            var count = (response["members"] as JArray ?? response["channels"] as JArray ?? response["messages"] as JArray)?.Count;
            var more = response.Value<bool?>("has_more") == true || !string.IsNullOrEmpty(response["response_metadata"]?.Value<string>("next_cursor"));
            observations.Add(new Observation(method, "Live", count == 0 ? "Partial" : "Checked",
                $"One response sampled{(count == null ? "" : $"; {count} items")}{(more ? "; more pages available" : "")}. Values discarded; absent fields do not imply removal.", audit.Shapes));
        }
        return response;
    }

    private async Task<JObject> Call(string method, Dictionary<string, string> arguments)
    {
        var query = string.Join("&", arguments.Select(a => $"{Uri.EscapeDataString(a.Key)}={Uri.EscapeDataString(a.Value)}"));
        try
        {
            using var response = await http.GetAsync($"https://slack.com/api/{method}?{query}");
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                observations.Add(new Observation(method, "Live", "Not covered", "Rate limited; retry in a later run after Slack's Retry-After interval.", []));
                return null;
            }
            response.EnsureSuccessStatusCode();
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (json.Value<bool?>("ok") != true)
            {
                // Error names are restricted; never copy response values or content into reports.
                var error = json.Value<string>("error");
                var detail = error != null && Regex.IsMatch(error, "^[a-z_]+$") ? error : "Slack rejected the probe";
                observations.Add(new Observation(method, "Live", "Not covered", detail, []));
                return null;
            }
            return json;
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException or JsonException)
        {
            observations.Add(new Observation(method, "Live", "Failed", "Transport or JSON failure; response content was discarded.", []));
            return null;
        }
    }
}
