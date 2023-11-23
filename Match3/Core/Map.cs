using Match3.Core.GameObjects;
using Match3.Utils;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Vector2<int> Size { get; }

        public IReadOnlyGem? GemAt(Vector2<int> position);

        public IReadOnlyGem? GemAt(int x, int y);

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
        private readonly Gem?[,] _gemMatrix;

        private readonly Vector2<int>[] _spawnPositions;
        private List<Gem> _gemsForSpawn;

        private readonly GemSwapper _gemSwapper;

        private readonly int _bombGemExpiredFrames;
        private readonly int _lineGemExpiredFrames;
        private readonly float _destroyersAcceleration;

        public Map(int x, int y, GameSettings settings)
        {
            _gravity = settings.Gravity;
            _size = new(x, y);
            _gems = [];
            _destroyers = [];
            _gemMatrix = new Gem[x, y];
            _spawnPositions = new Vector2<int>[x];
            _gemsForSpawn = [];
            _gemSwapper = new GemSwapper(settings.FramesForSwap);
            _bombGemExpiredFrames = settings.BombGemExpireFrames;
            _lineGemExpiredFrames = settings.LineGemExpireFrames;
            _destroyersAcceleration = settings.DestroyerAcceleration;
        }

        public Map(Vector2<int> size, GameSettings settings) : this(size.X, size.Y, settings) { }

        public Vector2<int> Size => _size;

        public IReadOnlyGem? GemAt(Vector2<int> position) => _gemMatrix[position.X, position.Y];

        public IReadOnlyGem? GemAt(int x, int y) => _gemMatrix[x, y];

        public IReadOnlyCollection<IReadOnlyGem> Gems => _gems;

        public IReadOnlyCollection<IReadOnlyDestroyer> Destroyers => _destroyers;

        public bool SwapInProgress(int frame) => _gemSwapper.IsActive(frame);

        public void StartSwappingGems(Vector2<int> first, Vector2<int> second, int frame) =>
            _gemSwapper.InitSwap(_gemMatrix[first.X, first.Y],
                                 _gemMatrix[second.X, second.Y],
                                 first,
                                 second,
                                 frame);

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
                        _gemMatrix[x, y] = gem;
                    } while (CheckRowAt(x, y, true));
                    gem.Position = new(x, y);
                    _gems.Add(gem);
                }
            }
            for (int x = 0; x < _size.X; x++)
                _spawnPositions[x] = new(x, 0);
        }

        public void SetListOfGems(List<Gem> gems)
        {
            _gemsForSpawn = gems;
        }

        public void Update(int frame)
        {
            UpdateGemMatrix();
            AddGravityToGems(frame);
            SpawnGems();

            _gemSwapper.Update(frame);
            if (_gemSwapper.IsSwapped(frame))
            {
                bool first = CheckRowAt(_gemSwapper.FirstPosition);
                bool second = CheckRowAt(_gemSwapper.SecondPosition);

                _gemSwapper.Finish(first || second, frame);

                if (first)
                    ActivateRowAt(_gemSwapper.FirstPosition, frame);
                if (second)
                    ActivateRowAt(_gemSwapper.SecondPosition, frame);
            }

            UpdateGems(frame);
            UpdateDestroyers(frame);

            DestroyExpiredGems(frame);
            DestroyDestroyersOuterBound();
        }

        private void UpdateGemMatrix()
        {
            for (int y = 0; y < _size.Y; ++y)
                for (int x = 0; x < _size.X; ++x)
                    _gemMatrix[x, y] = null;

            foreach (var gem in _gems)
            {
                Vector2<int> position = GetMatrixPosition(gem.Position);
                if (_gemMatrix[position.X, position.Y] is null)
                    _gemMatrix[position.X, position.Y] = gem;
                else
                {
                    position = GetMatrixPosition(gem.Position - Vector2<float>.One * 0.001f);
                    if (_gemMatrix[position.X, position.Y] is null)
                        _gemMatrix[position.X, position.Y] = gem;
                    else
                        throw new Exception();
                }
            }
        }

        private void SpawnGems()
        {
            foreach (var position in _spawnPositions)
            {
                if (_gemMatrix[position.X, position.Y] is null)
                {
                    int choice;
                    choice = Random.Shared.Next(_gemsForSpawn.Count);
                    Gem gem = _gemsForSpawn[choice].Clone();
                    Vector2<float> offset = new(0.0f, -0.5f); // Instead add line above map and set new gems in it
                    _gemMatrix[position.X, position.Y] = gem;
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

        private void UpdateDestroyers(int frame)
        {
            foreach (var destroyer in _destroyers)
            {
                destroyer.Update(frame);
                Vector2<int> position = GetMatrixPosition(destroyer.Position);
                if (InBounds(position) &&
                    _gemMatrix[position.X, position.Y] is not null)
                    _gemMatrix[position.X, position.Y]?.Activate(frame);
            }
        }

        private void DestroyExpiredGems(int frame)
        {
            List<Gem> newGems = [];
            for (int i = 0; i < _gems.Count; i++)
            {
                Gem gem = _gems[i];
                if (!gem.IsExpired(frame))
                    continue;
                switch (gem)
                {
                    case BombGem bombGem:
                        Vector2<int> position = GetMatrixPosition(bombGem.Position);
                        Vector2<int> delta = new(bombGem.ExplosionRadius, bombGem.ExplosionRadius);
                        ActivateArea(position - delta, position + delta, frame);
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
                if (gem.NewGem is not null)
                    newGems.Add(gem.NewGem);
                _gems.Remove(gem);
                i--;
            }
            _gems.AddRange(newGems);
        }

        private void DestroyDestroyersOuterBound()
        {
            _destroyers.RemoveAll((destroyer) =>
            !(destroyer.Position >= -Vector2<float>.One &&
            destroyer.Position < _size.ConvertTo<float>() + Vector2<float>.One));
        }

        #region Check and activate row

        private bool CheckRowAt(Vector2<int> position, bool checkDynamic = false) =>
            CheckRowAt(position.X, position.Y, checkDynamic);

        private bool CheckRowAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _gemMatrix[x, y];
            if (gem is null)
                return false;

            Vector2<int> rowSize = RowSizeAt(x, y, checkDynamic);

            return rowSize.X >= 3 || rowSize.Y >= 3;
        }

        private Vector2<int> RowSizeAt(int x, int y, bool checkDynamic = false)
        {
            IReadOnlyGem? gem = _gemMatrix[x, y] ?? throw new InvalidOperationException();
            Vector2<int> rowSize = Vector2<int>.One;

            foreach (var delta in Vector2<int>.AllDirections)
            {
                Vector2<int> observer = new(x, y);

                for (int i = 0; i < 2; ++i)
                {
                    observer += delta;
                    if (InBounds(observer) &&
                        gem.Equals(_gemMatrix[observer.X, observer.Y]) &&
                        _gemMatrix[observer.X, observer.Y] is not null &&
                        (_gemMatrix[observer.X, observer.Y].IsStatic || checkDynamic))
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

        private void ActivateRowAt(Vector2<int> position, int frame) =>
            ActivateRowAt(position.X, position.Y, frame);

        private void ActivateRowAt(int x, int y, int frame)
        {
            IReadOnlyGem? gem = _gemMatrix[x, y] ?? throw new InvalidOperationException();
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
                        _gemMatrix[observer.X, observer.Y] is not null &&
                        _gemMatrix[observer.X, observer.Y].IsStatic &&
                        gem.Equals(_gemMatrix[observer.X, observer.Y]))
                    {
                        _gemMatrix[observer.X, observer.Y].Activate(frame);
                    }
                    else
                        break;
                }
            }
            _gemMatrix[x, y].Activate(frame, CalcBonus(gem, rowSize));
        }

        private void ActivateArea(Vector2<int> upperLeft, Vector2<int> bottomRight, int frame)
        {
            if (upperLeft > bottomRight)
                throw new InvalidOperationException();

            for (int y = upperLeft.Y; y <= bottomRight.Y; ++y)
            {
                for (int x = upperLeft.X; x <= bottomRight.X; ++x)
                {
                    if (InBounds(x, y) && _gemMatrix[x, y] is not null)
                        _gemMatrix[x, y].Activate(frame);
                }
            }
        }

        #endregion

        private Gem? CalcBonus(IReadOnlyGem gem, Vector2<int> rowSize)
        {
            if (rowSize.X == 5 || rowSize.Y == 5 ||
                (rowSize.X >= 3 && rowSize.Y >= 3))
                return new BombGem(gem, _bombGemExpiredFrames, 1);
            if (rowSize.X == 4)
                return new LineGem(gem, _lineGemExpiredFrames, LineGemType.Vertical);
            if (rowSize.Y == 4)
                return new LineGem(gem, _lineGemExpiredFrames, LineGemType.Horizontal);
            return null;
        }

        private void AddGravityToGems(int frame)
        {
            foreach (var gem in _gems)
            {
                Vector2<int> position = GetMatrixPosition(gem.Position);
                if (gem.IsActive(frame) ||
                    (_gemSwapper.IsActive(frame) &&
                    (position == _gemSwapper.FirstPosition ||
                    position == _gemSwapper.SecondPosition)))
                    continue;

                int bottomY = position.Y + 1;
                if (bottomY == _size.Y) // Last row
                {
                    if (gem.Position.Y < _size.Y - 1)
                    {
                        gem.Velocity += new Vector2<float>(0.0f, _gravity);
                        if (gem.Velocity.Y > 0.4f)
                            gem.Velocity -= new Vector2<float>(0.0f, gem.Velocity.Y - 0.4f);
                        continue;
                    }
                    else
                    {
                        gem.Position = position.ConvertTo<float>();
                        gem.SetStatic();
                        if (CheckRowAt(position))
                            ActivateRowAt(position, frame);
                        continue;
                    }
                }

                if (_gemMatrix[position.X, bottomY] is null ||
                    (_gemMatrix[position.X, bottomY].IsStatic &&
                    gem.Position.Y - position.Y < 0.0f))
                {
                    gem.Velocity += new Vector2<float>(0.0f, _gravity);
                    if (gem.Velocity.Y > 0.4f)
                        gem.Velocity -= new Vector2<float>(0.0f, gem.Velocity.Y - 0.4f);
                }
                else if (gem.Position.Y - position.Y > 0.0f)
                {
                    gem.Position = position.ConvertTo<float>();
                    gem.SetStatic();
                    if (CheckRowAt(position))
                        ActivateRowAt(position, frame);
                }
            }
        }

        private static Vector2<int> GetMatrixPosition(Vector2<float> position)
        {
            double x = Math.Floor(position.X);
            if (position.X - x >= 0.5)
                x = Math.Ceiling(position.X);
            double y = Math.Floor(position.Y);
            if (position.Y - y >= 0.5)
                y = Math.Ceiling(position.Y);
            return new((int)x, (int)y);
        }

        public bool InBounds(int x, int y) => InBounds(new(x, y));

        public bool InBounds(Vector2<int> position) => position >= Vector2<int>.Zero && position < _size;
    }
}
