namespace Match3.Utils
{
    public struct Vector2
    {
        public int X, Y;

        public Vector2(int x = 0, int y = 0)
        {
            (X, Y) = (x, y);
        }

        public Vector2(Point point)
        {
            (X, Y) = (point.X, point.Y);
        }

        public readonly bool IsNeighbor(Vector2 vector)
        {
            Vector2 delta = this - vector;
            if (delta == Up || delta == Down || delta == Left || delta == Right)
                return true;
            return false;
        }

        private static Vector2 _zero = new (0, 0);
        private static Vector2 _one = new (1, 1);

        private static Vector2 _up = new (0, -1);
        private static Vector2 _down = new (0, 1);
        private static Vector2 _left = new (-1, 0);
        private static Vector2 _right = new (1, 0);

        public static Vector2 Zero => _zero;
        public static Vector2 One => _one;

        public static Vector2 Up => _up;
        public static Vector2 Down => _down;
        public static Vector2 Left => _left;
        public static Vector2 Right => _right;

        public static Vector2[] AllDirections => new Vector2[] { Up, Down, Left, Right };

        public static Vector2 operator -(Vector2 vector) => new(-vector.X, -vector.Y);

        public static bool operator <(Vector2 left, Vector2 right) => left.X < right.X && left.Y < right.Y;

        public static bool operator >(Vector2 left, Vector2 right) => left.X > right.X && left.Y > right.Y;

        public static bool operator <=(Vector2 left, Vector2 right) => left.X <= right.X && left.Y <= right.Y;

        public static bool operator >=(Vector2 left, Vector2 right) => left.X >= right.X && left.Y >= right.Y;

        public static bool operator ==(Vector2 left, Vector2 right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);


        public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.X - right.X, left.Y - right.Y);

        public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

        public static Vector2 operator +(Vector2 left, Direction right) =>
            left + FromDirection(right);


        public static Vector2 operator *(Vector2 left, int right) => new(left.X * right, left.Y * right);

        public static Vector2 operator *(int left, Vector2 right) => right * left;

        public static Vector2 operator /(Vector2 left, int right) => new(left.X / right, left.Y / right);


        public static Vector2 operator *(Vector2 left, float right) => new((int)(left.X * right), (int)(left.Y * right));

        public static Vector2 operator *(float left, Vector2 right) => right * left;

        public static Vector2 operator /(Vector2 left, float right) => new((int)(left.X / right), (int)(left.Y / right));

        public static Vector2 FromDirection(Direction direction) =>
            direction switch
            {
                Direction.Up => Up,
                Direction.Right => Right,
                Direction.Down => Down,
                Direction.Left => Left,
                Direction.None => Zero,
                _ => throw new NotImplementedException()
            };

        public void Deconstruct(out int X, out int Y)
        {
            throw new NotImplementedException();
        }
    }
}
