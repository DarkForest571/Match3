namespace Match3.Core
{
    public class Game
    {
        private const int _xSize = 8;
        private const int _ySize = 8;
        private Map _map;

        private Gem _red;
        private Gem _green;
        private Gem _blue;
        private Gem _yellow;
        private Gem _orange;

        private Point _selectedCell;

        public Game(int physicalFrames)
        {
            _map = new Map(_xSize, _ySize, 1f / physicalFrames);

            _red = new Gem(0);
            _green = new Gem(1);
            _blue = new Gem(2);
            _yellow = new Gem(3);
            _orange = new Gem(4);
        }

        public IReadOnlyMap Map => _map;

        public Point? SelectedCell => _selectedCell.X > -1 ? _selectedCell : null;

        public void Init()
        {
            ResetCellSelection();
            _map.InitMap();
            _map.SetListOfGems(new List<Gem>{
                _red,
                _green,
                _blue,
                _yellow,
                _orange
            });
        }

        public void Update()
        {
            _map.Update();
        }

        public void SelectCell(int x, int y)
        {
            if (x >= 0 && y >= 0 &&
                x < _xSize && y < _ySize &&
                _map.CellAt(x, y) is not null &&
                _map.CellAt(x, y).IsStatic)
                _selectedCell = new Point(x, y);
            else
                ResetCellSelection();
        }

        public void ResetCellSelection()
        {
            _selectedCell = new Point(-1);
        }
    }
}
