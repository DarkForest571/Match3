using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyGem : IReadOnlyGameObject
    {
        public bool IsActive(int frame);

        public bool IsExpired(int frame);

        public float NormalizedTimer(int frame);

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : GameObject, IReadOnlyGem
    {
        private readonly GameTimer _timer;

        private Gem? _newGem;

        public Gem(int colorID,
                   int framesBeforeExpired,
                   Vector2<float> position = default) : base(colorID,
                                                             position)
        {
            _timer = new (framesBeforeExpired);
            _newGem = null;
        }

        protected int FramesBeforeExpired => _timer.FramesPerTick;

        public bool IsActive(int frame) => _timer.IsActivated(frame);

        public bool IsExpired(int frame) => _timer.IsExpired(frame);

        public float NormalizedTimer(int frame) => _timer.Normalized(frame);

        public override Gem Clone() => new(ColorID,
                                           _timer.FramesPerTick,
                                           Position);

        public virtual void Activate(int frame, Gem? newGem = null)
        {
            _timer.StartTimer(frame);
            _newGem = newGem;
        }

        public Gem? Destroy()
        {
            return _newGem;
        }
    }
}
