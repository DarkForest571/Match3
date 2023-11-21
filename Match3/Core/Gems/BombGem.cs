namespace Match3.Core.Gems
{
    public class BombGem : Gem
    {
        private readonly int _explosionRadius;

        private float _timer;
        private float _deltaT;

        public BombGem(int colorID, int explosionRadius, float deltaT) : base(colorID)
        {
            _explosionRadius = explosionRadius;
            _timer = 0.0f;
            _deltaT = deltaT;
        }
        public override BombGem Clone() => new BombGem(ColorID, _explosionRadius, _deltaT);

        public override void Activate()
        {
            _state = GemState.Active;
        }

        public override void Update()
        {
            if (_state != GemState.Active)
                return;

            _timer += _deltaT;
            if (_timer >= _explosionRadius)
                _state = GemState.ReadyToDestroy;
        }
    }
}
