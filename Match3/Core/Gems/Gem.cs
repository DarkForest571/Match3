namespace Match3.Core.Gems
{
    public interface IReadOnlyGem : IReadOnlyGameObject
    {
        public bool IsActive { get; }

        public bool IsExpired(int frame);

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : GameObject, IReadOnlyGem
    {
        protected int _startFrame;
        protected int _endFrame;
        protected readonly int _framesBeforeExpired;

        public Gem(int colorID, int framesBeforeExpired) : base(colorID)
        {
            _framesBeforeExpired = framesBeforeExpired;
            _startFrame = -1;
        }

        public bool IsActive => _startFrame > -1;

        public bool IsExpired(int frame) => _startFrame > 1 && frame >= _endFrame;

        public virtual Gem Clone() => new Gem(ColorID, _framesBeforeExpired);

        public void Activate(int frame)
        {
            _startFrame = frame;
            _endFrame = frame + _framesBeforeExpired;
        }
        public virtual void Update(int frame) { }
    }
}
