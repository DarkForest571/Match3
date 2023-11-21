namespace Match3.Core.Gems
{
    public enum GemState
    {
        Idle,
        Active,
        ReadyToDestroy
    }

    public interface IReadOnlyGem
    {
        public int ColorID {  get; }

        public bool ReadyToDestroy { get; }

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

        public bool ReadyToDestroy => _state == GemState.ReadyToDestroy;

        public virtual Gem Clone() => new Gem(_colorID);

        public virtual void Activate()
        {
            _state = GemState.ReadyToDestroy;
        }
        public virtual void Update() { }
    }
}
