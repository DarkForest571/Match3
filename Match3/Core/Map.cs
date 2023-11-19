using System.Drawing;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Size Size { get; }

        public IReadOnlyCell? CellAt(int x, int y);
    }

    public class Map : IReadOnlyMap
    {
        private float _gravity;

        private int _xSize;
        private int _ySize;

        private Cell[,] _cellMatrix;
        private SpawnCell[] _spawnCell;

        private List<Gem> _gems;

        public Map(int x, int y, float gravity = 0.01f)
        {
            _gravity = gravity;
            _xSize = x;
            _ySize = y;
            _cellMatrix = new Cell[x, y];
            _spawnCell = new SpawnCell[x];
            _gems = new List<Gem>();
        }

        public Map(Size size) : this(size.Width, size.Height) { }

        public Size Size => new Size(_xSize, _ySize);

        public IReadOnlyCell? CellAt(int x, int y) => _cellMatrix[x, y];

        public virtual void InitMap()
        {
            int lastRow = _ySize - 1;
            for (int y = lastRow; y >= 1; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    _cellMatrix[x, y] = new Cell();
                    if (y < lastRow)
                        _cellMatrix[x, y].SetBottomCell(_cellMatrix[x, y + 1]);
                }
            }
            for (int x = 0; x < _xSize; ++x)
            {
                _spawnCell[x] = new SpawnCell();
                _cellMatrix[x, 0] = _spawnCell[x];
                _cellMatrix[x, 0].SetBottomCell(_cellMatrix[x, 1]);
            }
        }

        public void SetListOfGems(List<Gem> gems)
        {
            _gems = gems;
        }

        public void Update()
        {
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    _cellMatrix[x, y].Update(_gravity);
                }
            }
            TrySpawnGems();
        }

        private void TrySpawnGems()
        {
            for (int x = 0; x < _xSize; ++x)
            {
                if (_spawnCell[x].Gem is null)
                {
                    int choice = Random.Shared.Next(_gems.Count);
                    _spawnCell[x].AddGem(_gems[choice]);
                }
            }
        }
    }
}
