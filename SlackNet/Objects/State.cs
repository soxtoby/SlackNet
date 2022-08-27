using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet
{
	public class State
	{
		/// <summary>
		/// Keyed with the <see cref="Block.BlockId"/>s of any user-modified <see cref="InputBlock"/> blocks from the modal view.
		/// For each <see cref="Block.BlockId"/> is another dictionary keyed by the <see cref="IActionElement.ActionId"/>
		/// of the child <see cref="IInputBlockElement"/> of the input block.
		/// This final child object will contain the type and submitted value of the input block element.
		/// </summary>
		[JsonConverter(typeof(IgnoreArrayConverter))] // Slack returns an empty array instead of an object if no values have been filled in
		public Dictionary<string, Dictionary<string, ElementValue>> Values { get; set; } = new();

		/// <summary>
		/// Get an element value by the <see cref="Block.BlockId"/> of the block it is in, and its <see cref="IActionElement.ActionId"/>.
		/// </summary>
		/// <returns>Element value if it can be found and is the specified type, otherwise null.</returns>
		public TValue GetValue<TValue>(string blockId, string actionId) where TValue : ElementValue
		{
			return Values.TryGetValue(blockId, out var blockValues)
			       && blockValues.TryGetValue(actionId, out var elementValue)
				? elementValue as TValue
				: null;
		}

		/// <summary>
		/// Get an element value by its <see cref="IActionElement.ActionId"/>. Assumes action ID is unique in the view.
		/// </summary>
		/// <returns>Element value if it can be found and is the specified type, otherwise null.</returns>
		public TValue GetValue<TValue>(string actionId) where TValue : ElementValue
		{
			return Values.Values
				.SelectMany(b => b)
				.Where(v => v.Key == actionId)
				.Select(v => v.Value)
				.FirstOrDefault() as TValue;
		}
	}
}