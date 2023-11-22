using Match3.Core.GameObjects;
using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyCell
    {
        public Vector2<float> Offset { get; }

        public IReadOnlyGem? Gem { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private Vector2<float> _offset;
        private Gem? _gem;

        public Cell()
        {
            ResetOffset();
            _gem = null;
        }

        public Vector2<float> Offset => _offset;

        public IReadOnlyGem? Gem => _gem;

        public void SetGem(Gem gem)
        {
            _gem = gem;
        }

        public void SwapGems(Cell cell)
        {
            (_gem, cell._gem) = (cell._gem, _gem);
        }

        public void ActivateGem(int frame, Gem? newGem = null)
        {
            if (_gem is null)
                return;
            _gem.Activate(frame, newGem);
        }

        public void SetOffset(Vector2<float> offset)
        {
            _offset = offset;
        }

        public void ResetOffset()
        {
            _offset = Vector2<float>.Zero;
        }
    }
}
