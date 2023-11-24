using Match3.Utils;
using System.Collections.ObjectModel;

namespace Match3.Core.UI
{
    public class UIFrame
    {
        public readonly string Title;
        private List<UIElement> _elements;

        public UIFrame(string title)
        {
            Title = title;
        }

        public UIFrame? ClickOnMenu(Vector2<int> position)
        {
            foreach (var element in _elements)
            {
                if (element is MenuButton button &&
                    position > element.Position &&
                    position <= element.LowwerRightCorner)
                    return button.NextMenu;
            }
            return null;
        }

        public IReadOnlyCollection<UIElement> Elements => _elements;

        public void SetElements(List<UIElement> elements)
        {
            _elements = elements;
        }
    }
}
