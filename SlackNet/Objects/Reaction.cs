namespace SlackNet.Objects
{
    public class Reaction
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string[] Users { get; set; }
    }
}