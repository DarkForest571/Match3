﻿using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyGem : IReadOnlyGameObject
    {
        public bool IsActive(int frame);

        public bool IsExpired(int frame);

        public float NormalizedTimer(int frame);

        public int Score { get; }

        public bool Equals(IReadOnlyGem? second) => second is not null && ColorID == second.ColorID;
    }

    public class Gem : GameObject, IReadOnlyGem
    {
        private readonly GameTimer _timer;

        private int _score;
        private Gem? _newGem;

        public Gem(int colorID,
                   int framesBeforeExpired,
                   int score,
                   Vector2<float> position = default) : base(colorID,
                                                             position)
        {
            _timer = new(framesBeforeExpired);
            _score = score;
            _newGem = null;
        }

        protected int FramesBeforeExpired => _timer.FramesPerTick;

        public bool IsActive(int frame) => _timer.IsActivated(frame);

        public bool IsExpired(int frame) => _timer.IsExpired(frame);

        public float NormalizedTimer(int frame) => _timer.Normalized(frame);

        public int Score => _score;

        public Gem? NewGem => _newGem;

        public override Gem Clone() => new(ColorID,
                                           _timer.FramesPerTick,
                                           _score,
                                           Position);

        public virtual int Activate(int frame, Gem? newGem = null)
        {
            if (IsActive(frame))
                return 0;
            _timer.StartTimer(frame);
            _newGem = newGem;
            return _score;
        }
    }
}
