using Match3.Utils;

namespace Match3.Core
{
    public class CellSwapper
    {
        private Cell _firstCell;
        private Cell _secondCell;
        private Vector2<int> _firstPosition;
        private Vector2<int> _secondPosition;
        private Vector2<float> _delta;

        private readonly GameTimer _timer;

        private bool _isReverseSwapping;

        public CellSwapper(int framesForSwap)
        {
            _isReverseSwapping = false;
            _timer = new(framesForSwap);
        }

        public Vector2<int> FirstPosition => _firstPosition;

        public Vector2<int> SecondPosition => _secondPosition;

        public bool IsActive(int frame) => _timer.IsActivated(frame);

        public bool IsSwapped(int frame) => _timer.IsExpired(frame);

        public void InitSwap(Cell first, Cell second, Vector2<int> firstPosition, Vector2<int> secondPosition, int frame)
        {
            (_firstCell, _secondCell) = (first, second);
            (_firstPosition, _secondPosition) = (firstPosition, secondPosition);
            _delta = (secondPosition - firstPosition).ConvertTo<float>();
            _timer.StartTimer(frame);
            _isReverseSwapping = false;
        }

        public void Finish(bool makeRow, int frame)
        {
            if (_isReverseSwapping = !makeRow)
            {
                (_firstCell, _secondCell) = (_secondCell, _firstCell);
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
            _firstCell.SetOffset(offset);
            _firstCell.SetOffset(-offset);

            if (_timer.IsExpired(frame))
            {
                _firstCell.SwapGems(_secondCell);
                _firstCell.ResetOffset();
                _secondCell.ResetOffset();

                if (_isReverseSwapping)
                    _timer.ResetTimer();
            }
        }
    }
}
