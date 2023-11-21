namespace Match3.Core.Gems
{
    public class BombGem : Gem
    {
        public readonly int ExplosionRadius;

        private float _timer;
        private float _deltaT;

        public BombGem(int colorID, int explosionRadius, float deltaT) : base(colorID)
        {
            ExplosionRadius = explosionRadius;
            _timer = 0.0f;
            _deltaT = deltaT;
        }

        public BombGem(IReadOnlyGem gem, int explosionRadius, float deltaT)
            : this(gem.ColorID, explosionRadius, deltaT) { }

        public float NormalizedTimer => _timer / ExplosionRadius;

        public override BombGem Clone() => new BombGem(ColorID, ExplosionRadius, _deltaT);

        public override void Activate()
        {
            _state = GemState.Active;
        }

        public override void Update(int frame)
        {
            if (_state != GemState.Active)
                return;

            _timer += _deltaT;
            if (_timer >= ExplosionRadius)
                _state = GemState.Expired;
        }
    }
}
