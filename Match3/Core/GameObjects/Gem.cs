using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyGem : IReadOnlyGameObject
    {
        public bool IsActive { get; }

        public bool IsExpired { get; }

        public float NormalizedTimer { get; }

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : GameObject, IReadOnlyGem
    {
        protected int _startFrame;
        protected int _endFrame;
        protected int _lastFrame;
        protected readonly int _framesBeforeExpired;

        private Gem? _newGem;

        public Gem(int colorID,
                   int framesBeforeExpired,
                   Vector2<float> position = default) : base(colorID,
                                                             position)
        {
            _framesBeforeExpired = framesBeforeExpired;
            _startFrame = -1;
        }

        public bool IsActive => _startFrame > -1;

        public bool IsExpired => IsActive && _lastFrame >= _endFrame;

        public float NormalizedTimer => (_lastFrame - _startFrame) / (float)_framesBeforeExpired;

        public override Gem Clone() => new(ColorID,
                                           _framesBeforeExpired,
                                           Position);

        public virtual void Activate(int frame, Gem? newGem = null)
        {
            _startFrame = frame;
            _endFrame = frame + _framesBeforeExpired;
            _newGem = newGem;
        }

        public override void Update(int frame)
        {
            base.Update(frame);
            if (!IsActive)
                return;
            _lastFrame = frame;
        }

        public Gem? Destroy()
        {
            return _newGem;
        }
    }
}
