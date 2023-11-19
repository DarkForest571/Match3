using Match3.Core;

namespace Match3
{
    public partial class MainForm : Form
    {
        private Game _game;

        private Graphics _graphics;
        private BufferedGraphics _bufferedGraphics;

        private Size _cellSize;
        private Point _gridOffset;

        private Rectangle _ImageRect;
        private Bitmap _gemsTexture;
        private Bitmap _gridImage;
        private Bitmap _gridHighlightedImage;


        public MainForm(Game game)
        {
            _game = game;

            InitializeComponent();
            _bufferedGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            _graphics = _bufferedGraphics.Graphics;

            _cellSize = new Size(100, 100);
            _gridOffset = new Point(100, 0);

            _ImageRect = new Rectangle(0, 0, 512, 512);
            _gemsTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
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
            Point point = e.Location;

            point.X -= _gridOffset.X;
            point.Y -= _gridOffset.Y;

            if (point.X < 0 || point.Y < 0)
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
                        _graphics.DrawImage(_gridImage, drawRect, _ImageRect, GraphicsUnit.Pixel);
                    }
                }
            }

            if (_game.SelectedCell is not null)
            {
                Point position = _game.SelectedCell.Value;
                drawRect.Location = new(position.X * _cellSize.Width + _gridOffset.X, position.Y * _cellSize.Height + _gridOffset.Y);
                _graphics.DrawImage(_gridHighlightedImage, drawRect, _ImageRect, GraphicsUnit.Pixel);
            }
        }

        private void DrawGems()
        {
            IReadOnlyMap map = _game.Map;
            Rectangle drawRect = new(new(0, 0), _cellSize);
            Rectangle atlasRect = new(new(0, 0), _ImageRect.Size);
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    IReadOnlyCell? cell = map.CellAt(x, y);
                    if (cell is not null && cell.Gem is not null)
                    {
                        drawRect.Location = new(x * _cellSize.Width + _gridOffset.X,
                            (int)((y + cell.YOffset) * _cellSize.Height) + _gridOffset.Y);
                        atlasRect.Location = new(cell.Gem.AtlasPosition.X * _ImageRect.Width, cell.Gem.AtlasPosition.Y * _ImageRect.Height);
                        _graphics.DrawImage(_gemsTexture, drawRect, atlasRect, GraphicsUnit.Pixel);
                    }
                }
            }
        }
    }
}
