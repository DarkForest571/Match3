namespace Match3.Utils
{
    public class GameTimer
    {
        private int _startFrame;
        private int _endFrame;
        private readonly int _framesPerTick;
        private bool _expired;

        public GameTimer(int framesPerTick)
        {
            _framesPerTick = framesPerTick;
            ResetTimer();
        }

        public int FramesPerTick => _framesPerTick;

        public void StartTimer(int frame)
        {
            _startFrame = frame;
            _endFrame = frame + _framesPerTick;
        }

        public bool IsActivated(int frame) => frame >= _startFrame && frame < _endFrame;

        public bool IsExpired(int frame) => _endFrame != -1 && frame >= _endFrame;

        public float Normalized(int frame) => (frame - _startFrame) / (float)_framesPerTick;

        public void ResetTimer()
        {
            _startFrame = -1;
            _endFrame = -1;
            _expired = false;
        }
    }
}
