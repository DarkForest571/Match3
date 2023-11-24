using Match3.Core.UI;
using Match3.Utils;

namespace Match3.Core
{
    public class InputHandler
    {
        private Game _game;

        private Vector2<int> _cellSize;
        private Vector2<int> _gridOffset;

        public InputHandler(Game game)
        {
            _game = game;
            _cellSize = new(100, 100);
            _gridOffset = new(100, 0);
        }

        public void HandleMouseClick(Vector2<int> position)
        {
            if (_game.IsGameScene)
            {
                position -= _gridOffset;

                if (position < Vector2<int>.Zero)
                {
                    _game.ResetCellSelection();
                }
                else
                {
                    _game.SelectCell(new(position.X / _cellSize.X, position.Y / _cellSize.Y));
                }
            }
            else
            {
                UIFrame frame = _game.CurrentUIFrame;
                UIFrame? newFrame = frame.ClickOnMenu(position);
                if (newFrame is not null)
                    _game.CurrentUIFrame = newFrame;
            }
        }
    }
}
