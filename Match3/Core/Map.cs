using Match3.Core.GameObjects;
using Match3.Utils;
using System.Numerics;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Vector2<int> Size { get; }

        public IReadOnlyCell CellAt(int x, int y);

        public IReadOnlyCollection<IReadOnlyGem> Gems { get; }

        public IReadOnlyCollection<IReadOnlyDestroyer> Destroyers { get; }

        public bool SwapInProgress(int frame);
    }

    public class Map : IReadOnlyMap
    {
        private readonly float _gravity;

        private readonly Vector2<int> _size;

        private readonly List<Gem> _gems;
        private readonly List<Destroyer> _destroyers;
        private readonly Cell[,] _cellMatrix;

        private readonly Vector2<int>[] _spawnCellPositions;
        private List<Gem> _gemsForSpawn;

        private readonly CellSwapper _cellSwapper;

        private readonly int _framesForBomb;
        private readonly int _framesForLine;
        private readonly float _destroyersAcceleration;

        public Map(int x,
                   int y,
                   float gravity,
                   int framesForSwap,
                   int framesForBomb,
                   int framesForLine)
        {
            _gravity = gravity;
            _size = new(x, y);
            _gems = [];
            _destroyers = [];
            _cellMatrix = new Cell[x, y];
            _spawnCellPositions = [];
            _gemsForSpawn = [];
            _cellSwapper = new CellSwapper(framesForSwap);
            _framesForBomb = framesForBomb;
            _framesForLine = framesForLine;
        }

        public Map(Vector2<int> size,
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

        public Vector2<int> Size => _size;

        public IReadOnlyCell CellAt(int x, int y) => _cellMatrix[x, y];

        public IReadOnlyCollection<IReadOnlyGem> Gems => _gems;

        public IReadOnlyCollection<IReadOnlyDestroyer> Destroyers => _destroyers;

        public bool SwapInProgress(int frame) => _cellSwapper.IsActive(frame);

        public void StartSwappingGems(Vector2<int> first, Vector2<int> second, int frame)
        {
            _cellSwapper.InitSwap(_cellMatrix[first.X, first.Y],
                                 _cellMatrix[second.X, second.Y],
                                 first,
                                 second,
                                 frame);
        }

        public virtual void InitMap()
        {
            for (int y = 0; y < _size.Y; ++y)
            {
                for (int x = 0; x < _size.X; ++x)
                {
                    _cellMatrix[x, y] = new Cell();
                }
            }
            for (int x = 0; x < _size.X; ++x)
                _spawnCellPositions[x] = new(x, 0);
        }

        public virtual void InitGems()
        {
            for (int y = 0; y < _size.Y; ++y)
            {
                for (int x = 0; x < _size.X; ++x)
                {
                    int choice;
                    Gem gem;
                    do
                    {
                        choice = Random.Shared.Next(_gemsForSpawn.Count);
                        gem = _gemsForSpawn[choice].Clone();
                        _cellMatrix[x, y].SetGem(gem);
                    } while (CheckRowAt(x, y, true));
                    _cellMatrix[x, y].ResetOffset();
                    gem.Position = new(x, y);
                    _gems.Add(gem);
                }
            }
        }

        public void SetListOfGems(List<Gem> gems)
        {
            _gemsForSpawn = gems;
        }

        public void Update(int frame) // Need to change function call ordering!!!
        {
            ApplyGravityToGems(frame); // To end
            SpawnGems();

            _cellSwapper.Update(frame);
            if (_cellSwapper.IsSwapped(frame))
            {
                bool first = CheckRowAt(_cellSwapper.FirstPosition);
                bool second = CheckRowAt(_cellSwapper.SecondPosition);

                _cellSwapper.Finish(first || second, frame);

                if (first)
                    ActivateRowAt(_cellSwapper.FirstPosition, frame);
                if (second)
                    ActivateRowAt(_cellSwapper.SecondPosition, frame);
            }

            UpdateGems(frame);

            DestroyExpiredGems(frame);
        }

        private void SpawnGems()
        {
            foreach (var position in _spawnCellPositions)
            {
                if (_cellMatrix[position.X, position.Y].Gem is null)
                {
                    int choice;
                    choice = Random.Shared.Next(_gemsForSpawn.Count);
                    Gem gem = _gemsForSpawn[choice].Clone();
                    Vector2<float> offset = new(0.0f, -0.5f); // Instead add line above map and set new gems in it
                    _cellMatrix[position.X, position.Y].SetGem(gem);
                    _cellMatrix[position.X, position.X].SetOffset(offset);
                    gem.Position = position.ConvertTo<float>() + offset;
                    _gems.Add(gem);
                }
            }
        }

        private void UpdateGems(int frame)
        {
            foreach (var gem in _gems)
            {
                gem.Update(frame);
            }
        }

        private void DestroyExpiredGems(int frame)
        {

            foreach (var gem in _gems)
            {
                if (!gem.IsExpired(frame))
                    continue;
                switch (gem)
                {
                    case BombGem bombGem:
                        Vector2<int> delta = new(bombGem.ExplosionRadius, bombGem.ExplosionRadius);
                        ActivateArea(bombGem.Position - delta, bombGem.Position + delta, frame);
                        break;
                    case LineGem lineGem:
                        if (lineGem.Type == LineGemType.Vertical || lineGem.Type == LineGemType.Both)
                        {
                            _destroyers.Add(new Destroyer(lineGem, Direction.Up, _destroyersAcceleration));
                            _destroyers.Add(new Destroyer(lineGem, Direction.Down, _destroyersAcceleration));
                        }
                        if (lineGem.Type == LineGemType.Horizontal || lineGem.Type == LineGemType.Both)
                        {
                            _destroyers.Add(new Destroyer(lineGem, Direction.Left, _destroyersAcceleration));
                            _destroyers.Add(new Destroyer(lineGem, Direction.Right, _destroyersAcceleration));
                        }
                        break;
                    default:
                        break;
                }
                _cellMatrix[x, y].DestroyGem();
            }
        }

        #region Check and activate row

        public bool CheckRowAt(Vector2<int> position, bool checkDynamic = false) =>
            CheckRowAt(position.X, position.Y, checkDynamic);

        public bool CheckRowAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem;
            if (gem is null)
                return false;

            Vector2<int> rowSize = RowSizeAt(x, y, checkDynamic);

            return rowSize.X >= 3 || rowSize.Y >= 3;
        }

        private Vector2<int> RowSizeAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem ?? throw new InvalidOperationException();
            Vector2<int> rowSize = Vector2<int>.One;

            foreach (var delta in Vector2<int>.AllDirections)
            {
                Vector2<int> observer = new(x, y);

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        gem.Equals(_cellMatrix[observer.X, observer.Y].Gem) &&
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
            return rowSize;
        }

        public void ActivateRowAt(Vector2<int> position, int frame) =>
            ActivateRowAt(position.X, position.Y, frame);

        public void ActivateRowAt(int x, int y, int frame)
        {
            IReadOnlyGem? gem = _cellMatrix[x, y].Gem ?? throw new InvalidOperationException();
            Vector2<int> rowSize = RowSizeAt(x, y);

            foreach (var delta in Vector2<int>.AllDirections)
            {
                if (rowSize.X < 3 && delta.X != 0 ||
                    rowSize.Y < 3 && delta.Y != 0)
                    continue;
                Vector2<int> observer = new(x, y);

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        _cellMatrix[observer.X, observer.Y].IsStatic &&
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

        private Gem? CalcBonus(IReadOnlyGem gem, Vector2<int> rowSize)
        {
            if (rowSize.X == 5 || rowSize.Y == 5 ||
                (rowSize.X >= 3 && rowSize.Y >= 3))
                return new BombGem(gem, 1, _framesForBomb);
            if (rowSize.X == 4)
                return new LineGem(gem, _framesForLine, LineGemType.Vertical);
            if (rowSize.Y == 4)
                return new LineGem(gem, _framesForLine, LineGemType.Horizontal);
            return null;
        }

        private void ActivateArea(Vector2<int> upperLeft, Vector2<int> bottomRight, int frame)
        { // TODO Bug Bomb spawn other bonus, that is die immediately
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

        private void ApplyGravityToGems(int frame)
        {
            foreach (var gem in _gems)
            {
                if (gem.IsActive(frame))
                    return;

                int bottomX = y + 1;
                if (bottomX == _size.Y || !_cellMatrix[x, bottomX].GemIsFalling)
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
        }

        public bool InBounds(int x, int y) =>
            x >= 0 && y >= 0 &&
            x < _size.X && y < _size.Y;

        public bool InBounds(Vector2<int> point) => InBounds(point.X, point.Y);
    }
}
