using Match3.Utils;

namespace Match3.Core
{
    public class CellSwapper
    {
        private Cell _from;
        private Cell _to;
        private Vector2 _delta;
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
            _delta = delta;
            _tParam = 0.0f;
            _swapInProgress = true;
        }

        public void Update()
        {
            if (!_swapInProgress)
                return;

        }
    }
}
