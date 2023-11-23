using Match3.Core.GameObjects;
using Match3.Utils;

namespace Match3.Core
{
    public readonly struct GameSettings(int framesPerSecond,
                                        float gravity,
                                        int framesForSwap,
                                        int gemExpireFrames,
                                        int bombGemExpireFrames,
                                        int lineGemExpireFrames,
                                        float destroyerAcceleration)
    {
        public readonly int FramesPerSecond = framesPerSecond;
        public readonly float Gravity = gravity;
        public readonly int FramesForSwap = framesForSwap;
        public readonly int GemExpireFrames = gemExpireFrames;
        public readonly int BombGemExpireFrames = bombGemExpireFrames;
        public readonly int LineGemExpireFrames = lineGemExpireFrames;
        public readonly float DestroyerAcceleration = destroyerAcceleration;
    }

    public class Game
    {
        private readonly Map _map;

        private Vector2<int>? _selectedCell;

        private int _currentFrame;

        public Game(int xSize, int ySize, GameSettings settings)
        {
            _map = new Map(xSize, ySize, settings);

            _map.SetListOfGems([
                new Gem(0, settings.GemExpireFrames), // Red
                new Gem(1, settings.GemExpireFrames), // Green
                new Gem(2, settings.GemExpireFrames), // Blue
                new Gem(3, settings.GemExpireFrames), // Yellow
                new Gem(4, settings.GemExpireFrames)  // Orange
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
