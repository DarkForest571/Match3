using Match3.Utils;

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
        private Cell[] _spawnCell;

        private List<Gem> _gems;

        public Map(int x, int y, float gravity)
        {
            _gravity = gravity;
            _xSize = x;
            _ySize = y;
            _cellMatrix = new Cell[x, y];
            _spawnCell = new Cell[x];
            _gems = new List<Gem>();
        }

        public Map(Size size, float gravity) : this(size.Width, size.Height, gravity) { }

        public Size Size => new Size(_xSize, _ySize);

        public IReadOnlyCell? CellAt(int x, int y) => _cellMatrix[x, y];

        public virtual void InitMap()
        {
            for (int y = 0; y < _ySize; ++y)
            {
                for (int x = 0; x < _xSize; ++x)
                {
                    _cellMatrix[x, y] = new Cell();
                }
            }
            for (int x = 0; x < _xSize; ++x)
            {
                _spawnCell[x] = _cellMatrix[x, 0];
            }
        }

        public virtual void InitGems()
        {
            for (int y = 0; y < _ySize; ++y)
            {
                for (int x = 0; x < _xSize; ++x)
                {
                    int choice;
                    do
                    {
                        choice = Random.Shared.Next(_gems.Count);
                        _cellMatrix[x, y].SpawnGem(_gems[choice]);
                    } while (CheckRowAt(x, y));
                    if (CheckRowAt(x, y))
                        throw new Exception();
                }
            }
        }

        public void SetListOfGems(List<Gem> gems)
        {
            _gems = gems;
        }

        public bool Update()
        {
            bool isStatic = true;

            isStatic |= ApplyGravityToMap();
            isStatic |= TrySpawnGems();

            return isStatic;
        }

        public bool CheckRowAt(int x, int y)
        {
            Gem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                return false;

            Point[] directionsAndDeltas =
            {
                new Point(0, -1),
                new Point(0, 1),
                new Point(-1, 0),
                new Point(1, 0)
            };
            Point position = new Point(x, y);
            Point rowSize = new Point(1, 1);

            foreach (var delta in directionsAndDeltas)
            {
                Point observer = position;

                for (int i = 0; i < 2; ++i)
                {
                    observer.Offset(delta);
                    if (observer.X >= 0 && observer.Y >= 0 &&
                        observer.X < _xSize && observer.Y < _ySize &&
                        _cellMatrix[observer.X, observer.Y].Gem == gem)
                    {
                        if (delta.X != 0)
                            rowSize.X++;
                        else
                            rowSize.Y++;
                    }
                    else
                        break;
                }
            }

            return rowSize.X >= 3 || rowSize.Y >= 3;
        }

        private bool TrySpawnGems()
        {
            bool isStatic = true;
            for (int x = 0; x < _xSize; ++x)
            {
                if (_spawnCell[x].Gem is null)
                {
                    int choice;
                    do
                    {
                        choice = Random.Shared.Next(_gems.Count);
                        _spawnCell[x].SpawnGem(_gems[choice]);
                        isStatic = false;
                    } while (CheckRowAt(x, 0));
                }
            }
            return isStatic;
        }

        private bool ApplyGravityToMap()
        {
            bool isStatic = false;
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    isStatic &= ApplyGravityToCell(_cellMatrix[x, y], x, y);
                }
            }
            return isStatic;
        }

        private bool ApplyGravityToCell(Cell cell, int xPosition, int yPosition)
        {
            if (cell.Gem is null || cell.IsStatic)
                return true;

            cell.ApplyGravity(_gravity);
            cell.ApplyFallVelocity();

            int bottomCellY = yPosition + 1;
            if (bottomCellY == _ySize || _cellMatrix[xPosition, bottomCellY].IsStatic)
            {
                if (cell.YOffset >= 0.0f)
                    cell.SetStatic();
                return cell.IsStatic;
            }

            if (cell.YOffset > 0.5f)
            {
                Cell bottomCell = _cellMatrix[xPosition, bottomCellY];
                if (bottomCell.Gem is null)
                {
                    cell.MoveGemTo(bottomCell, Direction.Down);
                }
                else
                    throw new Exception();
            }


            return cell.IsStatic;
        }
    }
}
