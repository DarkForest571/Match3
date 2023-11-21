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
        private Gem? _gem;
        private Gem? _newGem;

        public Cell()
        {
            _xOffset = 0.0f;
            _yOffset = 0.0f;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = true;
            _gem = null;
            _newGem = null;
        }

        public float XOffset => _xOffset;

        public float YOffset => _yOffset;

        public IReadOnlyGem? Gem => _gem;

        public bool GemIsFalling => _gem == null || _isFalling;

        public bool IsExpiredGem => _gem != null && _gem.State == GemState.Expired;

        public void SpawnGem(Gem gem)
        {
            _xOffset = 0.0f;
            _yOffset = -0.5f;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = true;
            _gem = gem;
            _newGem = null;
        }

        public void SwapGems(Cell cell)
        {
            (_gem, cell._gem) = (cell._gem, _gem);
            (_newGem, cell._newGem) = (cell._newGem, _newGem);
        }

        public void ActivateGem(Gem? newGem = null)
        {
            if (_gem is null)
                return;
            _newGem = newGem;
            _gem.Activate();
        }

        public void UpdateGem()
        {
            _gem?.Update();
        }

        public void DestroyGem()
        {
            _gem = _newGem;
        }

        public void MoveGemTo(Cell cell, Direction direction)
        {
            if (cell._gem is not null)
                throw new InvalidOperationException();

            cell._xVelocity = _xVelocity;
            cell._yVelocity = _yVelocity;
            cell._isFalling = _isFalling;
            cell._gem = _gem;
            cell._newGem = _newGem;
            _gem = null;
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

        public void ResetOffset()
        {
            _xOffset = 0.0f;
            _yOffset = 0.0f;
        }

        public void ResetVelocity()
        {
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _isFalling = false;
        }
    }
}
