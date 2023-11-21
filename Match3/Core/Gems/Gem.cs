namespace Match3.Core.Gems
{
    public enum GemState
    {
        Idle,
        Falling,
        Active,
        Expired
    }

    public interface IReadOnlyGem
    {
        public int ColorID { get; }

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : IReadOnlyGem
    {
        private int _colorID;
        protected GemState _state;

        public Gem(int colorID)
        {
            _colorID = colorID;
            _state = GemState.Idle;
        }

        public int ColorID => _colorID;

        public GemState State => _state;

        public virtual Gem Clone() => new Gem(_colorID);

        public virtual void Activate()
        {
            _state = GemState.Active;
        }
        public virtual void Update()
        {
            if (_state == GemState.Active)
                _state = GemState.Expired;
        }
    }
}
