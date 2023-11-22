using System.Numerics;

namespace Match3.Utils
{
    public struct Vector2<T> where T : INumber<T>
    {
        public T X, Y;

        public Vector2(T? x = default, T? y = default)
        {
            if (x is null || y is null)
                throw new ArgumentNullException();
            X = x;
            Y = y;
        }

        public readonly bool IsNeighbor(Vector2<T> vector)
        {
            Vector2<T> delta = this - vector;
            if (delta == Up || delta == Down || delta == Left || delta == Right)
                return true;
            return false;
        }

        private static Vector2<T> _zero = new(T.Zero, T.Zero);
        private static Vector2<T> _one = new(T.One, T.One);

        private static Vector2<T> _up = new(T.Zero, -T.One);
        private static Vector2<T> _down = new(T.Zero, T.One);
        private static Vector2<T> _left = new(-T.One, T.Zero);
        private static Vector2<T> _right = new(T.One, T.Zero);

        public static Vector2<T> Zero => _zero;
        public static Vector2<T> One => _one;

        public static Vector2<T> Up => _up;
        public static Vector2<T> Down => _down;
        public static Vector2<T> Left => _left;
        public static Vector2<T> Right => _right;

        public static Vector2<T>[] AllDirections => new Vector2<T>[] { Up, Down, Left, Right };

        public static Vector2<T> operator -(Vector2<T> vector) => new(-vector.X, -vector.Y);

        public static bool operator <(Vector2<T> left, Vector2<T> right) => left.X < right.X && left.Y < right.Y;

        public static bool operator >(Vector2<T> left, Vector2<T> right) => left.X > right.X && left.Y > right.Y;

        public static bool operator <=(Vector2<T> left, Vector2<T> right) => left.X <= right.X && left.Y <= right.Y;

        public static bool operator >=(Vector2<T> left, Vector2<T> right) => left.X >= right.X && left.Y >= right.Y;

        public static bool operator ==(Vector2<T> left, Vector2<T> right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Vector2<T> left, Vector2<T> right) => !(left == right);


        public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right) => new(left.X - right.X, left.Y - right.Y);

        public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right) => new(left.X + right.X, left.Y + right.Y);

        public static Vector2<T> operator +(Vector2<T> left, Direction right) => left + FromDirection(right);


        public static Vector2<T> operator *(Vector2<T> left, T right) => new(left.X * right, left.Y * right);

        public static Vector2<T> operator *(T left, Vector2<T> right) => right * left;

        public static Vector2<T> operator /(Vector2<T> left, T right) => new(left.X / right, left.Y / right);


        public static Vector2<T> FromDirection(Direction direction) =>
            direction switch
            {
                Direction.Up => Up,
                Direction.Right => Right,
                Direction.Down => Down,
                Direction.Left => Left,
                Direction.None => Zero,
                _ => throw new NotImplementedException()
            };

        public readonly Vector2<U> ConvertTo<U>() where U : INumber<U> =>
            new((U)Convert.ChangeType(X, typeof(U)),
                (U)Convert.ChangeType(Y, typeof(U)));

        public readonly void Deconstruct(out T X, out T Y) => (X, Y) = (this.X, this.Y);

        public override readonly bool Equals(object? obj) => obj is Vector2<T> vector && this == vector;

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
