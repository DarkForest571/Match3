using Match3.Core.GameObjects;
using Match3.Core.UI;
using Match3.Utils;

namespace Match3.Core
{
    public readonly struct GameSettings(int framesPerSecond,
                                        float gravity,
                                        int framesForSwap,
                                        int gemExpireFrames,
                                        int bombGemExpireFrames,
                                        int lineGemExpireFrames,
                                        float destroyerAcceleration,
                                        float roundDuration)
    {
        public readonly int FramesPerSecond = framesPerSecond;
        public readonly float Gravity = gravity;
        public readonly int FramesForSwap = framesForSwap;
        public readonly int GemExpireFrames = gemExpireFrames;
        public readonly int BombGemExpireFrames = bombGemExpireFrames;
        public readonly int LineGemExpireFrames = lineGemExpireFrames;
        public readonly float DestroyerAcceleration = destroyerAcceleration;
        public readonly float RoundDuration = roundDuration;
    }

    public class Game
    {
        private readonly Map _map;

        private Vector2<int>? _selectedCell;

        GameTimer _timer;

        private UIFrame _mainMenu;
        private UIFrame _gameScene;
        private UIFrame _scoreMenu;

        private UIFrame _currentUIFrame;

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

            _timer = new((int)(settings.FramesPerSecond * settings.RoundDuration));

            Bitmap buttonImage = new("..\\..\\..\\..\\img\\button.png");
            _mainMenu = new("Main menu");
            _gameScene = new("Game scene");
            _scoreMenu = new("Scone");
            Vector2<int> position = new(500 - buttonImage.Size.Width / 2, 400 - buttonImage.Size.Height / 2);
            Vector2<int> size = new(buttonImage.Size.Width, buttonImage.Size.Height);
            MenuButton playButton = new(_gameScene, buttonImage, "Play", 100, position, size);
            _mainMenu.SetElements([playButton]);
            MenuButton quitButton = new(_mainMenu, buttonImage, "Quit", 100, position, size);
            UIElement finalScore = new("test", 100, new(position.X, position.Y - size.Y), size);
            _scoreMenu.SetElements([finalScore, quitButton]);
            _currentUIFrame = _mainMenu;
        }

        public bool IsGameScene => _currentUIFrame == _gameScene;

        public IReadOnlyMap Map => _map;

        public UIFrame CurrentUIFrame
        {
            get => _currentUIFrame;
            set
            {
                if (value == _mainMenu)
                {
                    _currentUIFrame = value;
                }
                if (value == _gameScene)
                {
                    _timer.StartTimer(_currentFrame);
                    _map.InitMap();
                    _currentUIFrame = value;
                }
                if (value == _scoreMenu)
                {
                    _scoreMenu.Elements.ElementAt(0).Text = "Score: " + _map.Score;
                    _currentUIFrame = value;
                }
            }
        }

        public Vector2<int>? SelectedCell => _selectedCell;

        public int CurrentFrame => _currentFrame;

        private void Init()
        {
            ResetCellSelection();
            _map.InitMap();
        }

        public void Update()
        {
            _currentFrame++;
            if (_timer.IsActivated(_currentFrame) && IsGameScene)
            {
                _map.Update(_currentFrame);
                if (_timer.IsExpired(_currentFrame))
                    CurrentUIFrame = _scoreMenu;
            }
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
