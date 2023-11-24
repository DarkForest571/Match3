using Match3.Utils;

namespace Match3.Core.UI
{
    public class MenuButton : UIElement
    {
        public readonly Bitmap Image;
        public readonly UIFrame NextMenu;

        public MenuButton(UIFrame nextMenu,
                          Bitmap image,
                          string text,
                          int textSize,
                          Vector2<int> position,
                          Vector2<int> size) : base(text,
                                                    textSize,
                                                    position,
                                                    size)
        {
            NextMenu = nextMenu;
            Image = image;
        }
    }
}
