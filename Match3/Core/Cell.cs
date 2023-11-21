using Match3.Core.Gems;
using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyCell
    {
        public float YOffset { get; }

        public float XOffset { get; }

        public IReadOnlyGem? Gem { get; }

        public bool IsIdle { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private float _xOffset; // (-0.5, 0.5]
        private float _yOffset; // (-0.5, 0.5]
        private float _xVelocity;
        private float _yVelocity;
        private Gem? _gem;
        private Gem? _newGem;

        public Cell()
        {
            _xOffset = 0.0f;
            _yOffset = 0.0f;
            _yVelocity = 0.0f;
            _gem = null;
        }

        public float XOffset => _xOffset;

        public float YOffset => _yOffset;

        public IReadOnlyGem? Gem => _gem;

        public bool IsIdle => _gem != null && (_gem.State == GemState.Idle || _gem.State == GemState.Active);

        public bool IsExpiredGem => _gem != null && _gem.State == GemState.Expired;

        public void SpawnGem(Gem gem)
        {
            _gem = gem;
            _xOffset = 0.0f;
            _yOffset = -0.5f;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
        }

        public void SwapGems(Cell cell)
        {
            (_gem, cell._gem) = (cell._gem, _gem);
        }

        public void ActivateGem(Gem? newGem = null)
        {
            if (_gem is null)
                return;
            _newGem = newGem;
            _gem.Activate();
        }

        public void Update()
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
            cell._gem = _gem;
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

        public void ApplyGravity(float gravity)
        {
            _yVelocity += gravity;
        }

        public void ApplyFallVelocity()
        {
            _yOffset += _yVelocity;
        }

        public void SetOffset(float x, float y)
        {
            _xOffset = x;
            _yOffset = y;
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
        }
    }
}
