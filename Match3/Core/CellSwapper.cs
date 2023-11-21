using Match3.Utils;

namespace Match3.Core
{
    public enum SwapperState
    {
        Idle,
        Swapping,
        Ready,
        ReverseSwapping
    }

    public class CellSwapper
    {
        private Cell _firstCell;
        private Cell _secondCell;
        private Vector2 _firstPosition;
        private Vector2 _secondPosition;
        private float _deltaX;
        private float _deltaY;
        private float _tParam;
        private readonly float _tParamDelta;

        private SwapperState _state;

        public CellSwapper(float tParamDelta)
        {
            _tParamDelta = tParamDelta;
            _state = SwapperState.Idle;
        }

        public Vector2 FirstPosition => _firstPosition;
        public Vector2 SecondPosition => _secondPosition;

        public SwapperState State => _state;

        public void InitSwap(Cell first, Cell second, Vector2 firstPosition, Vector2 secondPosition)
        {
            (_firstCell, _secondCell) = (first, second);
            (_firstPosition, _secondPosition) = (firstPosition, secondPosition);
            Vector2 delta = secondPosition - firstPosition;
            (_deltaX, _deltaY) = (delta.X, delta.Y);
            _tParam = 0.0f;
            _state = SwapperState.Swapping;
        }

        public void SetReverse()
        {
            _state = SwapperState.ReverseSwapping;
        }

        public void Finish()
        {
            _state = SwapperState.Idle;
        }

        public void Update()
        {
            if (_state == SwapperState.Idle)
                return;

            _tParam += _tParamDelta;
            float slerp = (float)Math.Sin(_tParam * Math.PI / 2);

            _firstCell.SetOffset(_deltaX * slerp, _deltaY * slerp);
            _secondCell.SetOffset(-_deltaX * slerp, -_deltaY * slerp);

            if (_tParam >= 1.0f)
            {
                _firstCell.SwapGems(_secondCell);
                _firstCell.SetStatic();
                _secondCell.SetStatic();

                (_firstCell, _secondCell) = (_secondCell, _firstCell);
                (_deltaX, _deltaY) = (-_deltaX, -_deltaY);
                _tParam = 0.0f;

                if (_state == SwapperState.Swapping)
                    _state = SwapperState.Ready;
                else
                    _state = SwapperState.Idle;
            }
        }
    }
}
