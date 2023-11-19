namespace Match3
{
    public interface IReadOnlyCell
    {
        public float YOffset { get; }

        public Gem? Gem { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private bool _isStatic;
        protected float _yOffset; // (-1.0, 0.0]
        private float _fallVelocity;
        protected Gem? _gem;
        private Cell _bottomCell;

        public Cell()
        {
            _isStatic = false;
            _yOffset = 0.0f;
            _fallVelocity = 0.0f;
            _gem = null;
            _bottomCell = null;
        }

        public float YOffset => _yOffset;

        public Gem? Gem => _gem;

        public void SetBottomCell(Cell cell)
        {
            _bottomCell = cell;
        }

        public void Update(float gravity)
        {
            if (_gem is null)
                return;
            if (!_isStatic)
            {
                _fallVelocity += gravity;
                _yOffset += _fallVelocity;
                if (_bottomCell is null)
                {
                    if (_yOffset > 0.0f)
                    {
                        _yOffset = 0.0f;
                        _isStatic = true;
                    }
                }
                else
                {
                    if (_yOffset > 0.0f)
                    {
                        if (_bottomCell._isStatic)
                        {
                            _yOffset = 0.0f;
                            _isStatic = true;
                        }
                        else
                        {
                            _bottomCell._gem = _gem;
                            _bottomCell._yOffset = _yOffset - 1.0f;
                            _yOffset = 0.0f;
                            _bottomCell._fallVelocity = _fallVelocity;
                            _fallVelocity = 0.0f;
                            _gem = null;
                        }
                    }
                }
            }
            else
            {
                _fallVelocity = 0.0f;
            }
        }
    }

    public class SpawnCell : Cell
    {
        public void AddGem(Gem gem)
        {
            _gem = gem;
            _yOffset = -1.0f;
        }
    }
}
