namespace SlackNet.Blocks
{
    public abstract class ExternalSelectMenuBase : SelectMenuBase
    {
        protected ExternalSelectMenuBase(string type) : base(type) { }

        /// <summary>
        /// When the typeahead field is used, a request will be sent on every character change.
        /// If you prefer fewer requests or more fully ideated queries, use this to tell Slack the fewest number of typed characters required before dispatch.
        /// </summary>
        public int? MinQueryLength { get; set; }
    }
}