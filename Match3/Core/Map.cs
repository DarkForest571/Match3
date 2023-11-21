using Match3.Core.Gems;
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

        public bool SwapInProgress => _cellSwapper.State != SwapperState.Idle;

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
                        _cellMatrix[x, y].SpawnGem(_gems[choice].Clone());
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
            if (_cellSwapper.State == SwapperState.Ready)
            {
                bool first = CheckRowAt(_cellSwapper.FirstPosition);
                bool second = CheckRowAt(_cellSwapper.SecondPosition);

                if (first || second)
                    _cellSwapper.Finish();
                else
                    _cellSwapper.SetReverse();

                if (first)
                    ActivateRowAt(_cellSwapper.FirstPosition);
                if (second)
                    ActivateRowAt(_cellSwapper.SecondPosition);
            }

            UpdateAllCells();

            DestroyExpiredGems();
        }

        public void SwapGems(Vector2 first, Vector2 second)
        {
            _cellSwapper.InitSwap(_cellMatrix[first.X, first.Y],
                                 _cellMatrix[second.X, second.Y],
                                 first,
                                 second);
        }

        public void UpdateAllCells()
        {
            for (int y = 0; y < _xSize; ++y)
            {
                for (int x = 0; x < _xSize; ++x)
                {
                    _cellMatrix[x, y].Update();
                }
            }
        }

        #region Check and activate combo

        public bool CheckRowAt(Vector2 position, bool checkDynamic = false) =>
            CheckRowAt(position.X, position.Y, checkDynamic);

        public bool CheckRowAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                return false;

            Vector2 rowSize = RowSizeAt(x, y, checkDynamic);

            return rowSize.X >= 3 || rowSize.Y >= 3;
        }

        private Vector2 RowSizeAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                throw new InvalidOperationException();
            Vector2 rowSize = Vector2.One;

            foreach (var delta in Vector2.AllDirections)
            {
                Vector2 observer = new(x, y);

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        gem.Equals(_cellMatrix[observer.X, observer.Y].Gem) &&
                        (_cellMatrix[observer.X, observer.Y].IsIdle || checkDynamic))
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
            return rowSize;
        }

        public void ActivateRowAt(Vector2 position) =>
            ActivateRowAt(position.X, position.Y);

        public void ActivateRowAt(int x, int y)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                throw new InvalidOperationException();

            Vector2 rowSize = RowSizeAt(x, y);

            foreach (var delta in Vector2.AllDirections)
            {
                if (rowSize.X < 3 && delta.X != 0 ||
                    rowSize.Y < 3 && delta.Y != 0)
                    continue;
                Vector2 observer = new(x, y);

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        _cellMatrix[observer.X, observer.Y].IsIdle &&
                        gem.Equals(_cellMatrix[observer.X, observer.Y].Gem))
                    {
                        _cellMatrix[observer.X, observer.Y].ActivateGem();
                    }
                    else
                        break;
                }
            }
            _cellMatrix[x, y].ActivateGem(CalcBonus(gem, rowSize));
        }

        #endregion

        private Gem? CalcBonus(IReadOnlyGem gem, Vector2 rowSize)
        {
            if (rowSize.X == 5 || rowSize.Y == 5 ||
                (rowSize.X >= 3 && rowSize.Y >= 3))
                return new BombGem(gem, 1, _gravity);
            if (rowSize.X == 4)
                return new LineGem(gem, LineGemType.Vertical);
            if (rowSize.Y == 4)
                return new LineGem(gem, LineGemType.Horizontal);
            return null;
        }

        private void SpawnGems()
        {
            for (int x = 0; x < _xSize; ++x)
            {
                if (_spawnCell[x].Gem is null)
                {
                    int choice;
                    do
                    {
                        choice = Random.Shared.Next(_gems.Count);
                        _spawnCell[x].SpawnGem(_gems[choice].Clone());
                    } while (CheckRowAt(x, 0));
                }
            }
        }

        private void DestroyExpiredGems()
        {
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    if (_cellMatrix[x, y].IsExpiredGem)
                    {
                        IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
                        _cellMatrix[x, y].DestroyGem();
                        if (gem is BombGem)
                        {
                            BombGem bombGem = (BombGem)gem;
                            Vector2 delta = new Vector2(bombGem.ExplosionRadius, bombGem.ExplosionRadius);
                            ActivateArea(new Vector2(x, y) - delta, new Vector2(x, y) + delta);
                        }
                    }
                }
            }
        }

        private void ActivateArea(Vector2 upperLeft, Vector2 bottomRight)
        {
            if (upperLeft > bottomRight)
                throw new InvalidOperationException();

            for (int y = upperLeft.Y; y <= bottomRight.Y; ++y)
            {
                for (int x = upperLeft.X; x <= bottomRight.X; ++x)
                {
                    if (InBounds(x, y))
                        _cellMatrix[x, y].ActivateGem();
                }
            }
        }

        private void ApplyGravity()
        {
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    ApplyGravityToCell(x, y);
                }
            }
        }

        private bool ApplyGravityToCell(int xPosition, int yPosition)
        {
            Cell cell = _cellMatrix[xPosition, yPosition];
            if (cell.Gem is null)
                return true;

            cell.ApplyGravity(_gravity);
            cell.ApplyFallVelocity();

            int bottomCellY = yPosition + 1;
            if (bottomCellY == _ySize || _cellMatrix[xPosition, bottomCellY].IsIdle)
            {
                if (cell.YOffset >= 0.0f)
                {
                    cell.ResetOffset();
                    cell.ResetVelocity();
                    if (CheckRowAt(xPosition, yPosition))
                        ActivateRowAt(xPosition, yPosition);
                }
                return cell.IsIdle;
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
            return cell.IsIdle;
        }

        public bool InBounds(int x, int y) =>
            x >= 0 && y >= 0 &&
            x < _xSize && y < _ySize;

        public bool InBounds(Vector2 point) => InBounds(point.X, point.Y);
    }
}
