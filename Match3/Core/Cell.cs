using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyCell
    {
        public float YOffset { get; }

        public float XOffset { get; }

        public Gem? Gem { get; }

        public bool IsStatic { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private bool _isStatic;
        protected float _xOffset; // (-0.5, 0.5]
        protected float _yOffset; // (-0.5, 0.5]
        private float _xVelocity;
        private float _yVelocity;
        protected Gem? _gem;

        public Cell()
        {
            _isStatic = false;
            _xOffset = 0.0f;
            _yOffset = 0.0f;
            _yVelocity = 0.0f;
            _gem = null;
        }

        public float XOffset => _xOffset;

        public float YOffset => _yOffset;

        public Gem? Gem => _gem;

        public bool IsStatic => _isStatic;

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

        public void ClearGem()
        {
            _gem = null;
            _isStatic = false;
        }

        public void MoveGemTo(Cell cell, Direction direction)
        {
            if (cell._gem is not null)
                throw new InvalidOperationException();

            _isStatic = false;
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

        public void SetStatic()
        {
            _isStatic = true;
            _xVelocity = 0.0f;
            _yVelocity = 0.0f;
            _xOffset = 0.0f;
            _yOffset = 0.0f;
        }
    }
}
