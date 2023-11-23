using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyGameObject
    {
        public int ColorID { get; }

        public Vector2<float> Position { get; }

        public Vector2<float> Velocity { get; }

        public bool IsStatic { get; }
    }

    public abstract class GameObject
    {
        private readonly int _colorID;

        private Vector2<float> _position;
        private Vector2<float> _velocity;

        public GameObject(int colorID, Vector2<float> position = default)
        {
            _colorID = colorID;
            _position = position;
            _velocity = Vector2<float>.Zero;
        }

        public int ColorID => _colorID;

        public Vector2<float> Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2<float> Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public bool IsStatic => _velocity == Vector2<float>.Zero;

        public abstract GameObject Clone();

        public virtual void Update(int frame)
        {
            _position += _velocity;
        }

        public void SetStatic()
        {
            _velocity = Vector2<float>.Zero;
        }
    }
}
