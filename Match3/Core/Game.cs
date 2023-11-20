using Match3.Utils;

namespace Match3.Core
{
    public class Game
    {
        private int _xSize;
        private int _ySize;
        private Map _map;

        private Gem _red;
        private Gem _green;
        private Gem _blue;
        private Gem _yellow;
        private Gem _orange;

        private Vector2? _selectedCell;

        public Game(int xSize, int ySize, int physicalFrames)
        {
            _xSize = xSize;
            _ySize = ySize;
            _map = new Map(_xSize, _ySize, 1f / physicalFrames);

            _red = new Gem(0);
            _green = new Gem(1);
            _blue = new Gem(2);
            _yellow = new Gem(3);
            _orange = new Gem(4);

            _selectedCell = null;
        }

        public IReadOnlyMap Map => _map;

        public Vector2? SelectedCell => _selectedCell;

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
            _map.InitGems();
        }

        public void Update()
        {
            _map.Update();
        }

        public void SelectCell(int x, int y)
        {
            if (_map.SwapInProgress ||
                !_map.InBounds(x, y) ||
                _map.CellAt(x, y) is null ||
                !_map.CellAt(x, y).IsStatic)
            {
                ResetCellSelection();
                return;
            }

            Vector2 newPosition = new Vector2(x, y);
            if (_selectedCell == null)
            {
                _selectedCell = newPosition;
            }
            else
            {
                if (_selectedCell.Value.IsNeighbor(new(x, y)))
                {
                    _map.SwapGems(_selectedCell.Value, newPosition);
                }
                ResetCellSelection();
            }
        }

        public void ResetCellSelection()
        {
            _selectedCell = null;
        }
    }
}
