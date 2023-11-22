using Match3.Utils;

namespace Match3.Core.GameObjects
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

        public Gem(int colorID,
                   int framesBeforeExpired,
                   Vector2 position = default) : base(colorID,
                                                      position)
        {
            _framesBeforeExpired = framesBeforeExpired;
            _startFrame = -1;
        }

        public bool IsActive => _startFrame > -1;

        public bool IsExpired(int frame) => _startFrame > 1 && frame >= _endFrame;

        public virtual Gem Clone() => new (ColorID,
                                           _framesBeforeExpired,
                                           new(PositionX, PositionY));

        public void Activate(int frame)
        {
            _startFrame = frame;
            _endFrame = frame + _framesBeforeExpired;
        }
        public virtual void Update(int frame) { }
    }
}
