using Match3.Core;
using Match3.Utils;

namespace Match3
{
    public partial class MainForm : Form
    {
        private Game _game;

        private Graphics _graphics;
        private BufferedGraphics _bufferedGraphics;

        private Size _cellSize;
        private Vector2 _gridOffset;

        private List<Bitmap> _gemsTextures;
        private Bitmap _gridImage;
        private Bitmap _gridHighlightedImage;

        public MainForm(Game game)
        {
            _game = game;

            InitializeComponent();
            _bufferedGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            _graphics = _bufferedGraphics.Graphics;

            _cellSize = new Size(100, 100);
            _gridOffset = new Vector2(100, 0);

            LoadTextures();
        }

        private void LoadTextures()
        {
            Bitmap gemsTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            gemsTexture = new Bitmap(gemsTexture, 400, 400);
            _gemsTextures = new List<Bitmap>();
            Point[] positionsOnAtlas = {
                new (1,0),
                new (2,0),
                new (0,1),
                new (2,1),
                new (3,2)
            };
            for (int i = 0; i < positionsOnAtlas.Length; i++)
            {
                Point position = new(_cellSize.Width * positionsOnAtlas[i].X, _cellSize.Height * positionsOnAtlas[i].Y);
                Rectangle rectangle = new(position, _cellSize);
                _gemsTextures.Add(gemsTexture.Clone(rectangle, gemsTexture.PixelFormat));
            }

            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridImage = new Bitmap(_gridImage, _cellSize);
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
            _gridHighlightedImage = new Bitmap(_gridHighlightedImage, _cellSize);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _graphics.Clear(Color.MidnightBlue);

            DrawGrid();
            DrawGems();

            _bufferedGraphics.Render();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            Vector2 point = new(e.Location);

            point -= _gridOffset;

            if (point < Vector2.Zero)
            {
                _game.ResetCellSelection();
            }
            else
            {
                int x = point.X / _cellSize.Width;
                int y = point.Y / _cellSize.Height;
                _game.SelectCell(x, y);
            }

            Invalidate(new Rectangle(0, 0, 1, 1)); // TODO Fix it
        }

        private void DrawGrid()
        {
            IReadOnlyMap map = _game.Map;
            Rectangle drawRect = new(new(0, 0), _cellSize);
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    if (map.CellAt(x, y) is not null)
                    {
                        drawRect.Location = new(x * _cellSize.Width + _gridOffset.X, y * _cellSize.Height + _gridOffset.Y);
                        _graphics.DrawImage(_gridImage, drawRect);
                    }
                }
            }

            if (_game.SelectedCell is not null)
            {
                Vector2 position = _game.SelectedCell.Value;
                drawRect.Location = new(position.X * _cellSize.Width + _gridOffset.X, position.Y * _cellSize.Height + _gridOffset.Y);
                _graphics.DrawImage(_gridHighlightedImage, drawRect);
            }
        }

        private void DrawGems()
        {
            IReadOnlyMap map = _game.Map;
            Rectangle drawRect = new(new(0, 0), _cellSize);
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    IReadOnlyCell? cell = map.CellAt(x, y);
                    if (cell is not null && cell.Gem is not null)
                    {
                        drawRect.Location = new((int)((x + cell.XOffset) * _cellSize.Width) + _gridOffset.X,
                            (int)((y + cell.YOffset) * _cellSize.Height) + _gridOffset.Y);
                        _graphics.DrawImage(_gemsTextures[cell.Gem.ID], drawRect);
                    }
                }
            }
        }
    }
}
