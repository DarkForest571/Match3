namespace Match3.Core.Gems
{
    public class BombGem : Gem
    {
        private readonly int _explosionRadius;

        private int _lastFrame;

        public BombGem(int colorID,
                       int explosionRadius,
                       int framesBeforeExpired) : base(colorID, framesBeforeExpired)
        {
            _explosionRadius = explosionRadius;
        }

        public BombGem(IReadOnlyGem gem, int explosionRadius, int framesBeforeExpired)
            : this(gem.ColorID, explosionRadius, framesBeforeExpired) { }

        public int ExplosionRadius => _explosionRadius;

        public float NormalizedTimer => (_endFrame - _lastFrame) / (float)_framesBeforeExpired;

        public override BombGem Clone() => new BombGem(ColorID, _explosionRadius, _framesBeforeExpired);

        public override void Update(int frame)
        {
            if (!IsActive)
                return;

            _lastFrame = frame;
        }
    }
}
