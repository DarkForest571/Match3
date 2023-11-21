using Match3.Utils;

namespace Match3.Core
{
    public class Destroyer
    {
        public readonly int ColorID;

        public readonly Direction Direction;

        public Destroyer(int colorID, Direction direction)
        {
            ColorID = colorID;
            Direction = direction;
        }
    }
}
