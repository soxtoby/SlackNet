namespace SlackNet.Rtm;

public class Error : SlackObject
{
    public int Code { get; set; }
    public string Msg { get; set; }
}