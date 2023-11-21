using Match3.Core.Gems;
using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Vector2 Size { get; }

        public IReadOnlyCell CellAt(int x, int y);

        public bool SwapInProgress(int frame);
    }

    public class Map : IReadOnlyMap
    {
        private readonly float _gravity;

        private readonly int _xSize;
        private readonly int _ySize;

        private Cell[,] _cellMatrix;
        private Cell[] _spawnCell;

        private List<Gem> _gems;

        private CellSwapper _cellSwapper;

        private readonly int _framesForBomb;
        private readonly int _framesForLine;

        public Map(int x,
                   int y,
                   float gravity,
                   int framesForSwap,
                   int framesForBomb,
                   int framesForLine)
        {
            _gravity = gravity;
            _xSize = x;
            _ySize = y;
            _cellMatrix = new Cell[x, y];
            _spawnCell = new Cell[x];
            _gems = new List<Gem>();
            _cellSwapper = new CellSwapper(framesForSwap);
            _framesForBomb = framesForBomb;
            _framesForLine = framesForLine;
        }

        public Map(Vector2 size,
                   float gravity,
                   int framesForSwap,
                   int framesForBomb,
                   int framesForLine) : this(size.X,
                                             size.Y,
                                             gravity,
                                             framesForSwap,
                                             framesForBomb,
                                             framesForLine)
        { }

        public Vector2 Size => new Vector2(_xSize, _ySize);

        public IReadOnlyCell CellAt(int x, int y) => _cellMatrix[x, y];

        public bool SwapInProgress(int frame) => _cellSwapper.SwapInProgress(frame);

        public void SwapGems(Vector2 first, Vector2 second, int frame)
        {
            _cellSwapper.InitSwap(_cellMatrix[first.X, first.Y],
                                 _cellMatrix[second.X, second.Y],
                                 first,
                                 second,
                                 frame);
        }

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

        public void Update(int frame)
        {
            ApplyGravity(frame);
            SpawnGems();

            if (_cellSwapper.Update(frame))
            {
                bool first = CheckRowAt(_cellSwapper.FirstPosition);
                bool second = CheckRowAt(_cellSwapper.SecondPosition);

                _cellSwapper.Finish(first || second, frame);

                if (first)
                    ActivateRowAt(_cellSwapper.FirstPosition, frame);
                if (second)
                    ActivateRowAt(_cellSwapper.SecondPosition, frame);
            }

            UpdateAllCells(frame);

            DestroyExpiredGems(frame);
        }

        private void UpdateAllCells(int frame)
        {
            for (int y = 0; y < _xSize; ++y)
            {
                for (int x = 0; x < _xSize; ++x)
                {
                    _cellMatrix[x, y].UpdateGem(frame);
                }
            }
        }

        private void DestroyExpiredGems(int frame)
        {
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    if (_cellMatrix[x, y].IsExpiredGem(frame))
                    {
                        IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
                        switch (gem)
                        {
                            case BombGem bombGem: // TODO Bug: Bomb spawn other bonus, that is die immediately
                                Vector2 delta = new Vector2(bombGem.ExplosionRadius, bombGem.ExplosionRadius);
                                ActivateArea(new Vector2(x, y) - delta, new Vector2(x, y) + delta, frame);
                                break;
                            case LineGem lineGem:
                                if(lineGem.Type == LineGemType.Vertical || lineGem.Type == LineGemType.Both)
                                {

                                }
                                if(lineGem.Type == LineGemType.Horizontal || lineGem.Type == LineGemType.Both)
                                {

                                }
                                break;
                            default:
                                break;
                        }
                        _cellMatrix[x, y].DestroyGem();
                    }
                }
            }
        }

        #region Check and activate row

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
                        (!_cellMatrix[observer.X, observer.Y].GemIsFalling || checkDynamic))
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

        public void ActivateRowAt(Vector2 position, int frame) =>
            ActivateRowAt(position.X, position.Y, frame);

        public void ActivateRowAt(int x, int y, int frame)
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
                        !_cellMatrix[observer.X, observer.Y].GemIsFalling &&
                        gem.Equals(_cellMatrix[observer.X, observer.Y].Gem))
                    {
                        _cellMatrix[observer.X, observer.Y].ActivateGem(frame);
                    }
                    else
                        break;
                }
            }
            _cellMatrix[x, y].ActivateGem(frame, CalcBonus(gem, rowSize));
        }

        #endregion

        private Gem? CalcBonus(IReadOnlyGem gem, Vector2 rowSize)
        {
            if (rowSize.X == 5 || rowSize.Y == 5 ||
                (rowSize.X >= 3 && rowSize.Y >= 3))
                return new BombGem(gem, 1, _framesForBomb);
            if (rowSize.X == 4)
                return new LineGem(gem, LineGemType.Vertical, _framesForLine);
            if (rowSize.Y == 4)
                return new LineGem(gem, LineGemType.Horizontal, _framesForLine);
            return null;
        }

        private void ActivateArea(Vector2 upperLeft, Vector2 bottomRight, int frame)
        {
            if (upperLeft > bottomRight)
                throw new InvalidOperationException();

            for (int y = upperLeft.Y; y <= bottomRight.Y; ++y)
            {
                for (int x = upperLeft.X; x <= bottomRight.X; ++x)
                {
                    if (InBounds(x, y))
                        _cellMatrix[x, y].ActivateGem(frame);
                }
            }
        }

        private void ApplyGravity(int frame)
        {
            for (int y = _xSize - 1; y >= 0; --y)
            {
                for (int x = _xSize - 1; x >= 0; --x)
                {
                    ApplyGravityToCell(x, y, frame);
                }
            }
        }

        private void ApplyGravityToCell(int x, int y, int frame)
        {
            Cell cell = _cellMatrix[x, y];
            if (cell.Gem is null || cell.Gem.IsActive)
                return;

            int bottomX = y + 1;
            if (bottomX == _ySize || !_cellMatrix[x, bottomX].GemIsFalling)
            {
                if (!cell.GemIsFalling)
                    return;

                cell.ApplyGravity(_gravity);
                if (cell.YOffset >= 0.0f)
                {
                    cell.ResetVelocity();
                    cell.ResetOffset();
                    if (CheckRowAt(x, y))
                        ActivateRowAt(x, y, frame);
                }
                return;
            }

            Cell bottomCell = _cellMatrix[x, y + 1];
            cell.ApplyGravity(_gravity);
            if (cell.YOffset > 0.5f)
            {
                if (bottomCell.Gem is null)
                    cell.MoveGemTo(bottomCell, Direction.Down);
                else
                {
                    cell.ResetVelocity();
                    if (!bottomCell.GemIsFalling)
                        cell.ResetOffset();
                }
            }
        }

        public bool InBounds(int x, int y) =>
            x >= 0 && y >= 0 &&
            x < _xSize && y < _ySize;

        public bool InBounds(Vector2 point) => InBounds(point.X, point.Y);
    }
}
