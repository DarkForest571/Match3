using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public class BombGem : Gem
    {
        private readonly int _explosionRadius;

        private int _lastFrame;

        public BombGem(int colorID,
                       int explosionRadius,
                       int framesBeforeExpired,
                       Vector2 position = default) : base(colorID,
                                                          framesBeforeExpired,
                                                          position)
        {
            _explosionRadius = explosionRadius;
        }

        public BombGem(IReadOnlyGem gem,
                       int explosionRadius,
                       int framesBeforeExpired,
                       Vector2 position = default) : this(gem.ColorID,
                                                          explosionRadius,
                                                          framesBeforeExpired,
                                                          position) { }

        public int ExplosionRadius => _explosionRadius;

        public float NormalizedTimer => (_lastFrame - _startFrame) / (float)_framesBeforeExpired;

        public override BombGem Clone() => new BombGem(ColorID, _explosionRadius, _framesBeforeExpired, new(PositionX, PositionY));

        public override void Update(int frame)
        {
            if (!IsActive)
                return;

            _lastFrame = frame;
        }
    }
}
