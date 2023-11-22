using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyBombGem : IReadOnlyGem
    {
        public int ExplosionRadius { get; }
    }

    public class BombGem : Gem, IReadOnlyBombGem
    {
        private readonly int _explosionRadius;

        public BombGem(int colorID,
                       int explosionRadius,
                       int framesBeforeExpired,
                       Vector2<float> position = default) : base(colorID,
                                                                 framesBeforeExpired,
                                                                 position)
        {
            _explosionRadius = explosionRadius;
        }

        public BombGem(IReadOnlyGem gem,
                       int explosionRadius,
                       int framesBeforeExpired,
                       Vector2<float> position = default) : this(gem.ColorID,
                                                                 explosionRadius,
                                                                 framesBeforeExpired,
                                                                 position)
        { }

        public int ExplosionRadius => _explosionRadius;

        public override BombGem Clone() => new (ColorID,
                                                _explosionRadius,
                                                _framesBeforeExpired,
                                                Position);
    }
}
