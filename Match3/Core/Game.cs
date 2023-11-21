using Match3.Core.Gems;
using Match3.Utils;
using System.Diagnostics.Metrics;

namespace Match3.Core
{
    public class Game
    {
        private Map _map;

        private Vector2? _selectedCell;

        private int _counter;

        public Game(int xSize, int ySize, int physicalFrames)
        {
            float timePerFrame = 1f / physicalFrames;
            int framesForSwap = physicalFrames / 4;
            int framesForBomb = physicalFrames / 4;
            int framesForLine = physicalFrames / 10;
            _map = new Map(xSize, ySize, timePerFrame, framesForSwap, framesForBomb, framesForLine);

            int gemExpireFrames = physicalFrames / 10;
            _map.SetListOfGems([
                new Gem(0, gemExpireFrames), // Red
                new Gem(1, gemExpireFrames), // Green
                new Gem(2, gemExpireFrames), // Blue
                new Gem(3, gemExpireFrames), // Yellow
                new Gem(4, gemExpireFrames)  // Orange
            ]);

            _counter = 0;

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
            _map.Update(_counter++);
        }

        public void SelectCell(int x, int y)
        {
            if (_map.SwapInProgress(_counter) ||
                !_map.InBounds(x, y) ||
                _map.CellAt(x, y) is null ||
                _map.CellAt(x, y).GemIsFalling)
            {
                ResetCellSelection();
                return;
            }

            Vector2 newPosition = new(x, y);
            if (_selectedCell == null)
            {
                _selectedCell = newPosition;
            }
            else
            {
                if (_selectedCell.Value.IsNeighbor(new(x, y)))
                {
                    _map.SwapGems(_selectedCell.Value, newPosition, _counter);
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
