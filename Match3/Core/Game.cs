using Match3.Core.GameObjects;
using Match3.Utils;
using System.Diagnostics.Metrics;

namespace Match3.Core
{
    public class Game
    {
        private Map _map;

        private Vector2<int>? _selectedCell;

        private int _currentFrame;

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

            _currentFrame = 0;

            _selectedCell = null;
        }

        public IReadOnlyMap Map => _map;

        public Vector2<int>? SelectedCell => _selectedCell;

        public int CurrentFrame => _currentFrame;

        public void Init()
        {
            ResetCellSelection();
            _map.InitGems();
        }

        public void Update()
        {
            _map.Update(_currentFrame++);
        }

        public void SelectCell(Vector2<int> position)
        {
            if (_map.SwapInProgress(_currentFrame) ||
                !_map.InBounds(position) ||
                _map.GemAt(position) is null ||
                !_map.GemAt(position).IsStatic)
            {
                ResetCellSelection();
                return;
            }

            if (_selectedCell == null)
            {
                _selectedCell = position;
            }
            else
            {
                if (_selectedCell.Value.IsNeighbor(position))
                {
                    _map.StartSwappingGems(_selectedCell.Value, position, _currentFrame);
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
