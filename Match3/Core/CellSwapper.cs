using Match3.Utils;

namespace Match3.Core
{
    public class CellSwapper
    {
        private Cell _from;
        private Cell _to;
        private float _deltaX;
        private float _deltaY;
        private float _tParam;
        private float _tParamDelta;

        private bool _swapInProgress;

        public CellSwapper(float tParamDelta)
        {
            _tParamDelta = tParamDelta;
            _swapInProgress = false;
        }

        public bool SwapInProgress => _swapInProgress;

        public void InitSwap(Cell from, Cell to, Vector2 delta)
        {
            _from = from;
            _to = to;
            _deltaX = delta.X;
            _deltaY = delta.Y;
            _tParam = 0.0f;
            _swapInProgress = true;
        }

        public void Update()
        {
            if (!_swapInProgress)
                return;

            _tParam += _tParamDelta;
            float slerp = (float)Math.Sin(_tParam * Math.PI / 2);

            _from.SetOffset(_deltaX * slerp, _deltaY * slerp);
            _to.SetOffset(-_deltaX * slerp, -_deltaY * slerp);
            
            if (_tParam >= 1.0f)
            {
                _from.SwapGems(_to);
                _from.SetStatic();
                _to.SetStatic();
                _swapInProgress = false;
            }
        }
    }
}
