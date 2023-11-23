using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public interface IReadOnlyDestroyer : IReadOnlyGameObject
    {
        public Direction Direction { get; }
    }

    public class Destroyer : GameObject, IReadOnlyDestroyer
    {
        private readonly Direction _direction;
        private Vector2<float> _acceleration;

        public Destroyer(int colorID,
                         Direction direction,
                         float accelerationPerFrame,
                         Vector2<float> position = default) : base(colorID,
                                                                   position)
        {
            _direction = direction;
            _acceleration = Vector2<float>.FromDirection(direction) * accelerationPerFrame;
        }

        public Destroyer(IReadOnlyGem parentGem,
                         Direction direction,
                         float accelerationPerFrame) : this(parentGem.ColorID,
                                                            direction,
                                                            accelerationPerFrame,
                                                            parentGem.Position)
        { }

        public Direction Direction => _direction;

        public override Destroyer Clone()
        {
            float acceleration =
                _acceleration.X != 0.0f
                ? Math.Abs(_acceleration.X)
                : Math.Abs(_acceleration.Y);
            return new(ColorID, _direction, acceleration, Position);
        }


        public override void Update(int frame)
        {
            AddVelocity(_acceleration);
            base.Update(frame);
        }
    }
}
