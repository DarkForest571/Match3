using Match3.Core;
using Match3.Core.Gems;
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
        private List<Bitmap> _bombTextures;
        private List<Bitmap> _horArrowsTextures;
        private List<Bitmap> _verArrowsTextures;
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

            _gemsTextures = new List<Bitmap>();
            _bombTextures = new List<Bitmap>();
            _horArrowsTextures = new List<Bitmap>();
            _verArrowsTextures = new List<Bitmap>();
            LoadTextures();
        }

        private void LoadTextures()
        {
            Bitmap gemsTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            gemsTexture = new Bitmap(gemsTexture, 400, 400);

            Point[] gemPositionsInAtlas = {
                new (1,0), // Red
                new (2,0), // Green
                new (0,1), // Blue
                new (2,1), // Yellow
                new (3,2)  // Orange
            };

            LoadFromAtlas(_cellSize, gemPositionsInAtlas, _gemsTextures, gemsTexture);

            Bitmap bonusesTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_arrow_atlas.png");
            bonusesTexture = new Bitmap(bonusesTexture, 400, 400);

            Point[] bombPositionsInAtlas = {
                new (1,0), // Red
                new (0,0), // Green
                new (0,1), // Blue
                new (2,0), // Yellow
                new (3,0)  // Orange
            };

            LoadFromAtlas(_cellSize, bombPositionsInAtlas, _bombTextures, bonusesTexture);

            Point[] horArrowsPositionsInAtlas = {
                new (2,1), // Red left
                new (0,3), // Green left
                new (4,3), // Blue left
                new (0,2), // Yellow left
                new (4,2), // Orange left
                new (3,1), // Red right
                new (1,3), // Green right
                new (5,3), // Blue right
                new (1,2), // Yellow right
                new (5,2)  // Orange right
            };

            LoadFromAtlas(new Size(_cellSize.Width / 2, _cellSize.Height),
                          horArrowsPositionsInAtlas,
                          _horArrowsTextures,
                          bonusesTexture);

            Point[] verArrowsPositionsInAtlas = {
                new (2,2), // Red up
                new (1,6), // Green up
                new (3,6), // Blue up
                new (0,4), // Yellow up
                new (3,4), // Orange up
                new (2,3), // Red down
                new (1,7), // Green down
                new (3,7), // Blue down
                new (0,5), // Yellow down
                new (3,5)  // Orange down
            };

            LoadFromAtlas(new Size(_cellSize.Width, _cellSize.Height / 2),
                          verArrowsPositionsInAtlas,
                          _verArrowsTextures,
                          bonusesTexture);

            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridImage = new Bitmap(_gridImage, _cellSize);
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
            _gridHighlightedImage = new Bitmap(_gridHighlightedImage, _cellSize);
        }

        private void LoadFromAtlas(Size imageSize, Point[] points, List<Bitmap> images, Bitmap atlas)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Point position = new(imageSize.Width * points[i].X, imageSize.Height * points[i].Y);
                Rectangle rectangle = new(position, imageSize);
                images.Add(atlas.Clone(rectangle, atlas.PixelFormat));
            }

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
                        switch (cell.Gem)
                        {
                            case BombGem:
                                _graphics.DrawImage(_bombTextures[cell.Gem.ColorID], drawRect);
                                break;
                            case LineGem lineGem:
                                _graphics.DrawImage(_gemsTextures[cell.Gem.ColorID], drawRect);
                                if (lineGem.Type == LineGemType.Vertical)
                                {
                                    Rectangle halfRect = new(drawRect.Location, new(drawRect.Width, drawRect.Height / 2));
                                    _graphics.DrawImage(_verArrowsTextures[cell.Gem.ColorID], halfRect);
                                    halfRect.Y += halfRect.Height;
                                    _graphics.DrawImage(_verArrowsTextures[cell.Gem.ColorID + _verArrowsTextures.Count / 2], halfRect);
                                }
                                else
                                {
                                    Rectangle halfRect = new(drawRect.Location, new(drawRect.Width / 2, drawRect.Height));
                                    _graphics.DrawImage(_horArrowsTextures[cell.Gem.ColorID], halfRect);
                                    halfRect.X += halfRect.Width;
                                    _graphics.DrawImage(_horArrowsTextures[cell.Gem.ColorID + _horArrowsTextures.Count / 2], halfRect);
                                }
                                break;
                            default:
                                _graphics.DrawImage(_gemsTextures[cell.Gem.ColorID], drawRect);
                                break;
                        }
                    }
                }
            }
        }
    }
}
