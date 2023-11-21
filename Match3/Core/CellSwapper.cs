using Match3.Utils;

namespace Match3.Core
{
    public class CellSwapper
    {
        private Cell _firstCell;
        private Cell _secondCell;
        private Vector2 _firstPosition;
        private Vector2 _secondPosition;
        private Vector2 _delta;

        private int _startFrame;
        private int _endFrame;
        private readonly int _framesForSwap;

        private bool _isReverseSwapping;

        public CellSwapper(int framesForSwap)
        {
            _framesForSwap = framesForSwap;
            _isReverseSwapping = false;
            _startFrame = -1;
        }

        public Vector2 FirstPosition => _firstPosition;
        public Vector2 SecondPosition => _secondPosition;

        private bool IsActive => _startFrame > -1;

        private void StopSwapper() => _startFrame = -1;

        public void InitSwap(Cell first, Cell second, Vector2 firstPosition, Vector2 secondPosition, int frame)
        {
            (_firstCell, _secondCell) = (first, second);
            (_firstPosition, _secondPosition) = (firstPosition, secondPosition);
            _delta = secondPosition - firstPosition;
            _startFrame = frame;
            _endFrame = frame + _framesForSwap;
            _isReverseSwapping = false;
        }

        public bool SwapInProgress(int frame) => frame < _endFrame;

        public void Finish(bool makeRow, int frame)
        {
            if (_isReverseSwapping = !makeRow)
            {
                (_firstCell, _secondCell) = (_secondCell, _firstCell);
                _delta = -_delta;
                _startFrame = frame;
                _endFrame = _startFrame + _framesForSwap;
            }
            else
                StopSwapper();
        }

        public bool Update(int frame)
        {
            if (!IsActive)
                return false;

            float tParam = (frame - _startFrame) / (float)_framesForSwap;
            float slerp = (float)(-Math.Cos(tParam * Math.PI) + 1.0) / 2;
            float dX = _delta.X * slerp;
            float dY = _delta.Y * slerp;
            _firstCell.SetOffset(dX, dY);
            _secondCell.SetOffset(-dX, -dY);

            if (!SwapInProgress(frame))
            {
                _firstCell.SwapObjects(_secondCell);
                //_firstCell.ResetVelocity();
                //_secondCell.ResetVelocity();
                _firstCell.ResetOffset();
                _secondCell.ResetOffset();

                if (_isReverseSwapping)
                    StopSwapper();
                return !_isReverseSwapping;
            }
            return false;
        }
    }
}
