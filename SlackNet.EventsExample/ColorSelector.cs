using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SlackNet.EventsExample
{
    public class ColorSelector : IActionHandler, IOptionProvider
    {
        public static readonly string ActionName = "color_select";

        public async Task<MessageResponse> Handle(InteractiveMessage message)
        {
            message.OriginalAttachment.Color = message.Action.SelectedValue;
            message.OriginalAttachment.Actions[0].SelectedOptions = new List<Option>
                {
                    GetOptions(string.Empty)
                        .FirstOrDefault(o => o.Value == message.Action.SelectedValue)
                    ?? new Option { Text = message.Action.SelectedValue, Value = message.Action.SelectedValue }
                };

            return new MessageResponse
                {
                    ReplaceOriginal = true,
                    Message = message.OriginalMessage
                };
        }

        public async Task<OptionsResponse> GetOptions(OptionsRequest request) => new OptionsResponse { Options = GetOptions(request.Value) };

        private static List<Option> GetOptions(string search) =>
            new ColorConverter().GetStandardValues()
                .Cast<Color>()
                .Where(c => c.Name.ToUpperInvariant().Contains(search.ToUpperInvariant()))
                .Select(c => new Option { Text = c.Name, Value = $"#{c.R:X2}{c.G:X2}{c.B:X2}" })
                .ToList();

        public static IList<Action> Actions => new List<Action>
            {
                new Action
                    {
                        Type = ActionType.Select,
                        Name = ActionName,
                        Text = "Select a color",
                        DataSource = DataSource.External
                    }
            };
    }
}