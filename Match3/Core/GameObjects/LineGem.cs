using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public enum LineGemType
    {
        Horizontal,
        Vertical,
        Both
    }

    public class LineGem : Gem
    {
        public readonly LineGemType Type;

        public LineGem(int colorID,
                       LineGemType type,
                       int framesBeforeExpired,
                       Vector2 position = default) : base(colorID,
                                                          framesBeforeExpired,
                                                          position)
        {
            Type = type;
        }

        public LineGem(IReadOnlyGem gem,
                       LineGemType type,
                       int framesBeforeExpired,
                       Vector2 position = default) : this(gem.ColorID,
                                                          type,
                                                          framesBeforeExpired,
                                                          position) { }
    }
}
