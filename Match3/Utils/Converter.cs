namespace Match3.Utils
{
    public static class Converter
    {
        public static Point Up => new Point(0, -1);
        public static Point Down => new Point(0, 1);
        public static Point Left => new Point(-1, 0);
        public static Point Right => new Point(1, 0);
        public static Point Zero => new Point(0, 0);

        public static Point DirectionToPoint(Direction direction) =>
            direction switch
            {
                Direction.Up => Up,
                Direction.Down => Down,
                Direction.Left => Left,
                Direction.Right => Right,
                Direction.None => Zero,
                _ => throw new NotImplementedException()
            };

        public static Direction PointToDirection(Point point)
        {
            if (point == Up) return Direction.Up;
            if (point == Down) return Direction.Down;
            if (point == Left) return Direction.Left;
            if (point == Right) return Direction.Right;
            if (point == Zero) return Direction.None;
            throw new NotImplementedException();
        }
    }
}
