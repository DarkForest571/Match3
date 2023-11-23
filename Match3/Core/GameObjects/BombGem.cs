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
                       int framesBeforeExpired,
                       int explosionRadius,
                       Vector2<float> position = default) : base(colorID,
                                                                 framesBeforeExpired,
                                                                 position)
        {
            _explosionRadius = explosionRadius;
        }

        public BombGem(IReadOnlyGem parentGem,
                       int framesBeforeExpired,
                       int explosionRadius) : this(parentGem.ColorID,
                                                   framesBeforeExpired,
                                                   explosionRadius,
                                                   parentGem.Position)
        { }

        public int ExplosionRadius => _explosionRadius;

        public override BombGem Clone() => new (ColorID,
                                                FramesBeforeExpired,
                                                _explosionRadius,
                                                Position);
    }
}
