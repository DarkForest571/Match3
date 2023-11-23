using Match3.Core.GameObjects;
using Match3.Utils;

namespace Match3.Core
{
    public class GemSwapper
    {
        private Gem _firstGem;
        private Gem _secondGem;
        private Vector2<int> _firstPosition;
        private Vector2<int> _secondPosition;
        private Vector2<float> _delta;

        private readonly GameTimer _timer;

        private bool _isReverseSwapping;

        public GemSwapper(int framesForSwap)
        {
            _isReverseSwapping = false;
            _timer = new(framesForSwap);
        }

        public Vector2<int> FirstPosition => _firstPosition;

        public Vector2<int> SecondPosition => _secondPosition;

        public bool IsActive(int frame) => _timer.IsActivated(frame);

        public bool IsSwapped(int frame) => _timer.IsExpired(frame);

        public void InitSwap(Gem first, Gem second, Vector2<int> firstPosition, Vector2<int> secondPosition, int frame)
        {
            (_firstGem, _secondGem) = (first, second);
            (_firstPosition, _secondPosition) = (firstPosition, secondPosition);
            _delta = (secondPosition - firstPosition).ConvertTo<float>();
            _timer.StartTimer(frame);
            _isReverseSwapping = false;
        }

        public void Finish(bool makeRow, int frame)
        {
            if (_isReverseSwapping = !makeRow)
            {
                (_firstPosition, _secondPosition) = (_secondPosition, _firstPosition);
                _delta = -_delta;
                _timer.StartTimer(frame);
            }
            else
                _timer.ResetTimer();
        }

        public void Update(int frame)
        {
            if (!IsActive(frame))
                return;

            float slerp = (float)(-Math.Cos(_timer.Normalized(frame) * Math.PI) + 1.0) / 2;
            Vector2<float> offset = _delta * slerp;
            _firstGem.Position = _firstPosition.ConvertTo<float>() + offset;
            _secondGem.Position = _secondPosition.ConvertTo<float>() - offset;

            if (_timer.IsExpired(frame))
            {
                if (_isReverseSwapping)
                    _timer.ResetTimer();
            }
        }
    }
}
