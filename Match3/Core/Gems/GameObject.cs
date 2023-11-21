namespace Match3.Core.Gems
{
    public interface IReadOnlyGameObject
    {
        public int ColorID { get; }
    }

    public class GameObject
    {
        private int _colorID;

        public GameObject(int colorID)
        {
            _colorID = colorID;
        }

        public int ColorID => _colorID;
    }
}
