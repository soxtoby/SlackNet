namespace SlackNet.Blocks
{
    public abstract class SelectMenuBase : ActionElement
    {
        protected SelectMenuBase(string type) : base(type) { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the menu. 
        /// </summary>
        public PlainText Placeholder { get; set; }
    }
}