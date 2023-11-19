namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Size Size { get; }

        public IReadOnlyCell? CellAt(int x, int y);
    }

    public class Map : IReadOnlyMap
    {
        private Size _size;

        private Cell[,] _cellMatrix;

        public Map(int x, int y)
        {
            _size = new Size(x, y);
            _cellMatrix = new Cell[x, y];
        }

        public Map(Size size) : this(size.Width, size.Height) { }

        public Size Size => _size;

        public IReadOnlyCell? CellAt(int x, int y) => _cellMatrix[x, y];

        public virtual void Init()
        {
            for (int y = 0; y < _size.Height; ++y)
            {
                for (int x = 0; x < _size.Width; ++x)
                {
                    _cellMatrix[x, y] = new Cell();
                }
            }
        }
    }
}
