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
                       int framesBeforeExpired,
                       LineGemType type,
                       Vector2<float> position = default) : base(colorID,
                                                                 framesBeforeExpired,
                                                                 position)
        {
            _type = type;
        }

        public LineGem(IReadOnlyGem parentGem,
                       int framesBeforeExpired,
                       LineGemType type) : this(parentGem.ColorID,
                                                framesBeforeExpired,
                                                type,
                                                parentGem.Position)
        { }

        public LineGemType Type => _type;

        public override LineGem Clone() => new(ColorID,
                                               _framesBeforeExpired,
                                               _type,
                                               Position);
    }
}
