namespace Match3.Core.Gems
{
    public interface IReadOnlyGem
    {
        public int ColorID { get; }

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : IReadOnlyGem
    {
        private int _colorID;

        protected int _startFrame;
        protected int _endFrame;
        protected readonly int _framesBeforeExpired;

        public Gem(int colorID, int framesBeforeExpired)
        {
            _colorID = colorID;
            _framesBeforeExpired = framesBeforeExpired;
            _startFrame = -1;
        }

        public int ColorID => _colorID;

        public bool IsActive => _startFrame > -1;

        public bool IsExpired(int frame) => _startFrame > 1 && frame >= _endFrame;

        public virtual Gem Clone() => new Gem(_colorID, _framesBeforeExpired);

        public void Activate(int frame)
        {
            _startFrame = frame;
            _endFrame = frame + _framesBeforeExpired;
        }
        public virtual void Update(int frame) { }
    }
}
