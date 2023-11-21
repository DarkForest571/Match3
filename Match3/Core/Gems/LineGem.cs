namespace Match3.Core.Gems
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
                       int framesBeforeExpired) : base(colorID, framesBeforeExpired)
        {
            Type = type;
        }

        public LineGem(IReadOnlyGem gem,
                       LineGemType type,
                       int framesBeforeExpired) : this(gem.ColorID, type, framesBeforeExpired) { }
    }
}
