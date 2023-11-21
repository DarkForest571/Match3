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

        public LineGem(int colorID, LineGemType type) : base(colorID)
        {
            Type = type;
        }

        public LineGem(IReadOnlyGem gem, LineGemType type) : this(gem.ColorID, type) { }
    }
}
