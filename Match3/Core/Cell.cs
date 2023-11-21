using Match3.Core.Gems;
using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyCell
    {
        public float YOffset { get; }

        public float XOffset { get; }

        public IReadOnlyGem? Gem { get; }

        public bool GemIsFalling { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private float _xOffset; // (-0.5, 0.5]
        private float _yOffset; // (-0.5, 0.5]
        private float _xVelocity;
        private float _yVelocity;
        private bool _isFalling;
        private GameObject? _gameObject;
        private GameObject? _newGameObject;

        public Cell()
        {
            _xOffset = 0.0f;
            _yOffset = 0.0f;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = true;
            _gameObject = null;
            _newGameObject = null;
        }

        public float XOffset => _xOffset;

        public float YOffset => _yOffset;

        public IReadOnlyGem? Gem => _gameObject as Gem;

        public bool GemIsFalling => _gameObject == null || _isFalling;

        public bool IsExpiredGem(int frame) => _gameObject != null && ((Gem)_gameObject).IsExpired(frame);

        public void SpawnGem(Gem gem)
        {
            _xOffset = 0.0f;
            _yOffset = -0.5f;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = true;
            _gameObject = gem;
            _newGameObject = null;
        }

        public void SwapObjects(Cell cell)
        {
            (_gameObject, cell._gameObject) = (cell._gameObject, _gameObject);
            (_newGameObject, cell._newGameObject) = (cell._newGameObject, _newGameObject);
        }

        public void ActivateGem(int frame, Gem? newGem = null)
        {
            if (_gameObject as Gem is null)
                return;
            _newGameObject = newGem;
            ((Gem)_gameObject).Activate(frame);
        }

        public void UpdateGem(int frame)
        {
            (_gameObject as Gem)?.Update(frame);
        }

        public void DestroyGem()
        {
            _gameObject = _newGameObject;
        }

        public void MoveGemTo(Cell cell, Direction direction)
        {
            if (cell._gameObject is not null)
                throw new InvalidOperationException();

            cell._xVelocity = _xVelocity;
            cell._yVelocity = _yVelocity;
            cell._isFalling = _isFalling;
            cell._gameObject = _gameObject;
            cell._newGameObject = _newGameObject;
            _gameObject = null;
            switch (direction)
            {
                case Direction.Up:
                    cell._xOffset = _xOffset;
                    cell._yOffset = _yOffset + 1.0f;
                    break;
                case Direction.Down:
                    cell._xOffset = _xOffset;
                    cell._yOffset = _yOffset - 1.0f;
                    break;
                case Direction.Left:
                    cell._xOffset = _xOffset + 1.0f;
                    cell._yOffset = _yOffset;
                    break;
                case Direction.Right:
                    cell._xOffset = _xOffset - 1.0f;
                    cell._yOffset = _yOffset;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void AddVelocity(float x, float y)
        {
            _xVelocity += x;
            _yVelocity += y;
        }

        public void ApplyVelocity()
        {
            _xOffset += _xVelocity;
            _yOffset += _yVelocity;
        }

        public void ApplyGravity(float gravity)
        {
            _yVelocity += gravity;
            _yOffset += _yVelocity;
            _isFalling = true;
        }

        public void ResetVelocity()
        {
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = false;
        }

        public void SetOffset(float x, float y) // TODO Try without it
        {
            _xOffset = x;
            _yOffset = y;
        }

        public void ResetOffset()
        {
            _xOffset = 0.0f;
            _yOffset = 0.0f;
        }
    }
}
