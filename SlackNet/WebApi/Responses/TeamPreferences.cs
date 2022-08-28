namespace SlackNet.WebApi
{
    public class TeamPreferences
    {
        public bool AllowMessageDeletion { get; set; }
        public bool DisplayRealNames { get; set; }
        public string DisableFileUploads { get; set; }
        public int MsgEditWindowMin { get; set; }
        public string WhoCanPostGeneral { get; set; }
    }
}