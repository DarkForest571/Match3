namespace Match3
{
    public interface IReadOnlyCell
    {
        public float YOffset { get; }

        public Gem? Gem { get; }
    }

    public class Cell : IReadOnlyCell
    {
        private float _yOffset;
        private Gem? _gem;

        public Cell()
        {
            _yOffset = 0.0f;
            _gem = null;
        }

        public float YOffset => _yOffset;

        public Gem? Gem => _gem;
    }
}
