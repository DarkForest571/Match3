using Match3.Utils;

namespace Match3.Core.GameObjects
{
    public enum LineGemType
    {
        Horizontal,
        Vertical,
        Both
    }

    public interface IReadOnlyLineGem : IReadOnlyGem
    {
        public LineGemType Type { get; }
    }

    public class LineGem : Gem
    {
        private readonly LineGemType _type;

        public LineGem(int colorID,
                       LineGemType type,
                       int framesBeforeExpired,
                       Vector2<float> position = default) : base(colorID,
                                                                 framesBeforeExpired,
                                                                 position)
        {
            _type = type;
        }

        public LineGem(IReadOnlyGem gem,
                       LineGemType type,
                       int framesBeforeExpired,
                       Vector2<float> position = default) : this(gem.ColorID,
                                                                 type,
                                                                 framesBeforeExpired,
                                                                 position)
        { }

        public LineGemType Type => _type;

        public override LineGem Clone() => new(ColorID,
                                               _type,
                                               _framesBeforeExpired,
                                               Position);
    }
}
