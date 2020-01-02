using System.Collections.Generic;

namespace SlackNet.Blocks
{
    public abstract class StaticSelectMenuBase : SelectMenuBase
    {
        protected StaticSelectMenuBase(string type) : base(type) { }

        /// <summary>
        /// An array of <see cref="Option"/> objects. If <see cref="OptionGroups"/> is specified, this field should not be.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<Option> Options { get; set; } = new List<Option>();

        /// <summary>
        /// An array of <see cref="OptionGroup"/> objects. If <see cref="Options"/> is specified, this field should not be.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<OptionGroup> OptionGroups { get; set; } = new List<OptionGroup>();
    }
}