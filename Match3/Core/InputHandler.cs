using Match3.Utils;

namespace Match3.Core
{
    public class InputHandler
    {
        private readonly Game _game;

        private Vector2<int> _cellSize;
        private Vector2<int> _gridOffset;

        public InputHandler(Game game, Vector2<int> cellSize, Vector2<int> gridOffset)
        {
            _game = game;
            _cellSize = cellSize;
            _gridOffset = gridOffset;
        }

        public void HandleMouseClick(Vector2<int> position)
        {
            position -= _gridOffset;

            if (position < Vector2<int>.Zero)
            {
                _game.ResetCellSelection();
            }
            else
            {
                int x = position.X / _cellSize.X;
                int y = position.Y / _cellSize.Y;
                _game.SelectCell(x, y);
            }
        }
    }
}
