using Match3.Utils;

namespace Match3.Core.UI
{
    public class UIElement
    {
        public string Text;
        public readonly int TextSize;
        public readonly Vector2<int> Position;
        public readonly Vector2<int> Size;

        public UIElement(string text, int textSize, Vector2<int> position, Vector2<int> size)
        {
            Text = text;
            TextSize = textSize;
            Position = position;
            Size = size;
        }

        public Vector2<int> LowwerRightCorner => Position + Size;
    }
}
