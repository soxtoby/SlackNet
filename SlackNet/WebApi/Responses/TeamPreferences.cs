#nullable disable
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class TeamPreferences
{
    public bool AllowMessageDeletion { get; set; }
    public bool DisplayRealNames { get; set; }
    public string DisableFileUploads { get; set; }
    [JsonProperty("msg_edit_window_mins")]
    public int MsgEditWindowMin { get; set; }
    public string WhoCanPostGeneral { get; set; }
}