using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Vector2 Size { get; }

        public IReadOnlyCell? CellAt(int x, int y);

        public bool SwapInProgress { get; }
    }

    public class Map : IReadOnlyMap
    {
        private float _gravity;

        private int _xSize;
        private int _ySize;

        private Cell[,] _cellMatrix;
        private Cell[] _spawnCell;

        private List<Gem> _gems;

        private CellSwapper _cellSwapper;

        public Map(int x, int y, float gravity, float swapSpeed)
        {
            _gravity = gravity;
            _xSize = x;
            _ySize = y;
            _cellMatrix = new Cell[x, y];
            _spawnCell = new Cell[x];
            _gems = new List<Gem>();
            _cellSwapper = new CellSwapper(swapSpeed);
        }

        public Map(Vector2 size, float gravity, float swapSpeed) : this(size.X, size.Y, gravity, swapSpeed) { }

        public Vector2 Size => new Vector2(_xSize, _ySize);

        public IReadOnlyCell? CellAt(int x, int y) => _cellMatrix[x, y];

        public bool SwapInProgress => _cellSwapper.SwapInProgress;

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
                    } while (CheckRowAt(x, y, true));
                }
            }
        }

        public void SetListOfGems(List<Gem> gems)
        {
            _gems = gems;
        }

        public void Update()
        {
            ApplyGravity();
            SpawnGems();
            _cellSwapper.Update();
        }

        public void SwapGems(Vector2 first, Vector2 second)
        {
            _cellSwapper.InitSwap(_cellMatrix[first.X, first.Y],
                                 _cellMatrix[second.X, second.Y],
                                 second - first);
        }

        public bool CheckRowAt(int x, int y, bool checkDynamic)
        {
            Gem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                return false;

            Vector2[] directionsAndDeltas =
            {
                Vector2.Up,
                Vector2.Down,
                Vector2.Left,
                Vector2.Right
            };
            Vector2 position = new Vector2(x, y);
            Vector2 rowSize = Vector2.One;

            foreach (var delta in directionsAndDeltas)
            {
                Vector2 observer = position;

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        _cellMatrix[observer.X, observer.Y].Gem == gem &&
                        (_cellMatrix[observer.X, observer.Y].IsStatic || checkDynamic))
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

        private bool SpawnGems()
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
                    } while (CheckRowAt(x, 0, false));
                }
            }
            return isStatic;
        }

        private bool ApplyGravity()
        {
            bool isStatic = false;
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    isStatic &= ApplyGravityToCell(x, y);
                }
            }
            return isStatic;
        }

        private bool ApplyGravityToCell(int xPosition, int yPosition)
        {
            Cell cell = _cellMatrix[xPosition, yPosition];
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
                    throw new InvalidOperationException();
            }


            return cell.IsStatic;
        }

        public bool InBounds(int x, int y) =>
            x >= 0 && y >= 0 &&
            x < _xSize && y < _ySize;

        public bool InBounds(Vector2 point) => InBounds(point.X, point.Y);
    }
}
