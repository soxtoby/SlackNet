using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using ActionElement = SlackNet.Interaction.ActionElement;

namespace SlackNet.EventsExample
{
    public class LegacyColorSelector : IInteractiveMessageHandler, IOptionProvider
    {
        public static readonly string ActionName = "color_select";

        public async Task<MessageResponse> Handle(InteractiveMessage message)
        {
            var menu = (Menu)message.Action;
            message.OriginalAttachment.Color = menu.SelectedValue;
            ((Menu)message.OriginalAttachment.Actions[0]).SelectedOptions = new List<Option>
                {
                    GetOptions(string.Empty)
                        .FirstOrDefault(o => o.Value == menu.SelectedValue)
                    ?? new Option { Text = menu.SelectedValue, Value = menu.SelectedValue }
                };

            return new MessageResponse
                {
                    ReplaceOriginal = true,
                    Message = message.OriginalMessage
                };
        }

        public async Task<OptionsResponse> GetOptions(OptionsRequest request) => new() { Options = GetOptions(request.Value) };

        private static List<Option> GetOptions(string search) =>
            FindColors(search)
                .Select(c => new Option { Text = c.Name, Value = $"#{c.R:X2}{c.G:X2}{c.B:X2}" })
                .ToList();

        private static IEnumerable<Color> FindColors(string search) =>
            new ColorConverter().GetStandardValues()
                .Cast<Color>()
                .Where(c => c.Name.ToUpperInvariant().Contains(search.ToUpperInvariant()));

        public static IList<ActionElement> Actions => new List<ActionElement>
            {
                new Menu
                    {
                        Name = ActionName,
                        Text = "Select a color",
                        DataSource = DataSource.External
                    }
            };
    }
}