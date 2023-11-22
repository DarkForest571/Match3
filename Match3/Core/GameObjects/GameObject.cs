using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyGameObject
    {
        public int ColorID { get; }

        public float PositionX { get; }

        public float PositionY { get; }
    }

    public class GameObject
    {
        private int _colorID;

        private float _positionX;
        private float _positionY;
        private float _velocityX;
        private float _velocityY;
        private float _accelerationX;
        private float _accelerationY;

        public GameObject(int colorID, Vector2 position = default)
        {
            _colorID = colorID;
            _positionX = position.X;
            _positionY = position.Y;
            _velocityX = 0.0f;
            _velocityY = 0.0f;
            _accelerationX = 0.0f;
            _accelerationY = 0.0f;
        }

        public int ColorID => _colorID;

        public float PositionX => _positionX;

        public float PositionY => _positionY;
    }
}
