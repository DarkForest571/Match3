using Match3.Core.Gems;
using Match3.Utils;

namespace Match3.Core
{
    public class Game
    {
        private int _xSize;
        private int _ySize;
        private Map _map;

        private Vector2? _selectedCell;

        public Game(int xSize, int ySize, int physicalFrames)
        {
            _xSize = xSize;
            _ySize = ySize;
            float timePerFrame = 1f / physicalFrames;
            _map = new Map(_xSize, _ySize, timePerFrame, timePerFrame * 5);

            _map.SetListOfGems([
                new Gem(0), // Red
                new Gem(1), // Green
                new Gem(2), // Blue
                new Gem(3), // Yellow
                new Gem(4)  // Orange
            ]);

            _selectedCell = null;
        }

        public IReadOnlyMap Map => _map;

        public Vector2? SelectedCell => _selectedCell;

        public void Init()
        {
            ResetCellSelection();
            _map.InitMap();

            _map.InitGems();
        }

        public void Update()
        {
            _map.Update();
        }

        public void SelectCell(int x, int y)
        {
            if (_map.SwapInProgress ||
                !_map.InBounds(x, y) ||
                _map.CellAt(x, y) is null ||
                !_map.CellAt(x, y).IsIdle)
            {
                ResetCellSelection();
                return;
            }

            Vector2 newPosition = new Vector2(x, y);
            if (_selectedCell == null)
            {
                _selectedCell = newPosition;
            }
            else
            {
                if (_selectedCell.Value.IsNeighbor(new(x, y)))
                {
                    _map.SwapGems(_selectedCell.Value, newPosition);
                }
                ResetCellSelection();
            }
        }

        public void ResetCellSelection()
        {
            _selectedCell = null;
        }
    }
}
