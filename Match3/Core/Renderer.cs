using Match3.Core.GameObjects;
using Match3.Utils;
using System.Drawing;
using System.Drawing.Imaging;

namespace Match3.Core
{
    public class Renderer
    {
        private readonly Game _game;

        private readonly BufferedGraphics _bufferedGraphics;
        private readonly Graphics _graphics;

        private Size _cellSize;
        private Vector2<int> _gridOffset;

        private List<Bitmap> _gemsTextures;
        private List<Bitmap> _bombTextures;
        private List<Bitmap> _horArrowsTextures;
        private List<Bitmap> _verArrowsTextures;
        private Bitmap _gridImage;
        private Bitmap _gridHighlightedImage;

        public Renderer(Game game, BufferedGraphics bufferedGraphics)
        {
            _game = game;

            _bufferedGraphics = bufferedGraphics;
            _graphics = _bufferedGraphics.Graphics;

            _cellSize = new(100, 100);
            _gridOffset = new(100, 0);

            _gemsTextures = [];
            _bombTextures = [];
            _horArrowsTextures = [];
            _verArrowsTextures = [];
            LoadTextures();
        }

        public void Draw()
        {
            _graphics.Clear(Color.MidnightBlue);

            DrawGrid();
            DrawGems(_game.CurrentFrame);
            DrawDestroyers();

            _bufferedGraphics.Render();
        }

        private void LoadTextures()
        {
            Bitmap gemsTexture = new("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            gemsTexture = new(gemsTexture, 400, 400);

            Point[] gemPositionsInAtlas = [
                new(1, 0), // Red
                new(2, 0), // Green
                new(0, 1), // Blue
                new(2, 1), // Yellow
                new(3, 2)  // Orange
            ];

            LoadFromAtlas(_cellSize, gemPositionsInAtlas, _gemsTextures, gemsTexture);

            Bitmap bonusesTexture = new("..\\..\\..\\..\\img\\sprite_arrow_atlas.png");
            bonusesTexture = new(bonusesTexture, 400, 400);

            Point[] bombPositionsInAtlas = [
                new(1, 0), // Red
                new(0, 0), // Green
                new(0, 1), // Blue
                new(2, 0), // Yellow
                new(3, 0)  // Orange
            ];

            LoadFromAtlas(_cellSize, bombPositionsInAtlas, _bombTextures, bonusesTexture);

            Point[] horArrowsPositionsInAtlas = [
                new(2, 1), // Red left
                new(0, 3), // Green left
                new(4, 3), // Blue left
                new(0, 2), // Yellow left
                new(4, 2), // Orange left
                new(3, 1), // Red right
                new(1, 3), // Green right
                new(5, 3), // Blue right
                new(1, 2), // Yellow right
                new(5, 2)  // Orange right
            ];

            LoadFromAtlas(new Size(_cellSize.Width / 2, _cellSize.Height),
                          horArrowsPositionsInAtlas,
                          _horArrowsTextures,
                          bonusesTexture);

            Point[] verArrowsPositionsInAtlas = [
                new(2, 2), // Red up
                new(1, 6), // Green up
                new(3, 6), // Blue up
                new(0, 4), // Yellow up
                new(3, 4), // Orange up
                new(2, 3), // Red down
                new(1, 7), // Green down
                new(3, 7), // Blue down
                new(0, 5), // Yellow down
                new(3, 5)  // Orange down
            ];

            LoadFromAtlas(new Size(_cellSize.Width, _cellSize.Height / 2),
                          verArrowsPositionsInAtlas,
                          _verArrowsTextures,
                          bonusesTexture);

            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridImage = new Bitmap(_gridImage, _cellSize);
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
            _gridHighlightedImage = new Bitmap(_gridHighlightedImage, _cellSize);
        }

        private static void LoadFromAtlas(Size imageSize, Point[] points, List<Bitmap> images, Bitmap atlas)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Point position = new(imageSize.Width * points[i].X, imageSize.Height * points[i].Y);
                Rectangle rectangle = new(position, imageSize);
                images.Add(atlas.Clone(rectangle, atlas.PixelFormat));
            }

        }


        private void DrawGrid()
        {
            //IReadOnlyMap map = _game.Map; // Can be used to draw obstacle cells
            Rectangle drawRect = new(new(0, 0), _cellSize);
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    // Obstacle check here
                    drawRect.Location = new(x * _cellSize.Width + _gridOffset.X,
                                            y * _cellSize.Height + _gridOffset.Y);
                    _graphics.DrawImage(_gridImage, drawRect);
                }
            }

            if (_game.SelectedCell is not null)
            {
                Vector2<int> position = _game.SelectedCell.Value;
                drawRect.Location = new(position.X * _cellSize.Width + _gridOffset.X,
                                        position.Y * _cellSize.Height + _gridOffset.Y);
                _graphics.DrawImage(_gridHighlightedImage, drawRect);
            }
        }

        private void DrawGems(int frame)
        {
            foreach (var gem in _game.Map.Gems)
            {
                DrawGemByGemType(gem, frame);
            }
        }

        private void DrawDestroyers()
        {
            foreach (var destroyer in _game.Map.Destroyers)
            {
                DrawDestroyer(destroyer);
            }
        }

        private void DrawGemByGemType(IReadOnlyGem gem, int frame)
        {
            switch (gem)
            {
                case IReadOnlyBombGem bombGem:
                    DrawBombGem(bombGem, frame);
                    break;
                case IReadOnlyLineGem lineGem:
                    DrawLineGem(lineGem);
                    break;
                default:
                    DrawGem(gem);
                    break;
            }
        }

        private void DrawGem(IReadOnlyGem gem) => DrawGem(gem.ColorID, gem.Position);

        private void DrawGem(int colorID, Vector2<float> position)
        {
            Rectangle drawRect = new((int)(position.X * _cellSize.Width) + _gridOffset.X,
                                     (int)(position.Y * _cellSize.Height) + _gridOffset.Y,
                                     _cellSize.Width,
                                     _cellSize.Height);
            _graphics.DrawImage(_gemsTextures[colorID], drawRect);
        }

        private void DrawLineGem(IReadOnlyLineGem lineGem) =>
            DrawLineGem(lineGem.ColorID, lineGem.Position, lineGem.Type);

        private void DrawLineGem(int colorID, Vector2<float> position, LineGemType type)
        {
            DrawGem(colorID, position);
            if (type == LineGemType.Vertical || type == LineGemType.Both)
            {
                DrawArrow(colorID, position, Direction.Up);
                DrawArrow(colorID, position, Direction.Down);
            }
            if (type == LineGemType.Horizontal || type == LineGemType.Both)
            {
                DrawArrow(colorID, position, Direction.Left);
                DrawArrow(colorID, position, Direction.Right);
            }
        }

        private void DrawDestroyer(IReadOnlyDestroyer destroyer) =>
            DrawArrow(destroyer.ColorID, destroyer.Position, destroyer.Direction);

        private void DrawArrow(int colorID, Vector2<float> position, Direction direction)
        {
            bool isVertical =
                direction == Direction.Up ||
                direction == Direction.Down;
            Rectangle drawRect = isVertical
                ? new((int)(position.X * _cellSize.Width) + _gridOffset.X,
                      (int)(position.Y * _cellSize.Height) + _gridOffset.Y,
                      _cellSize.Width,
                      _cellSize.Height / 2)
                 : new((int)(position.X * _cellSize.Width) + _gridOffset.X,
                       (int)(position.Y * _cellSize.Height) + _gridOffset.Y,
                       _cellSize.Width / 2,
                       _cellSize.Height);

            if (isVertical)
                _graphics.DrawImage(_verArrowsTextures[colorID], drawRect);
            else
                _graphics.DrawImage(_horArrowsTextures[colorID], drawRect);
        }

        private void DrawBombGem(IReadOnlyBombGem bombGem, int frame) =>
            DrawBombGem(bombGem.ColorID, bombGem.Position, bombGem.NormalizedTimer(frame));

        private void DrawBombGem(int colorID, Vector2<float> position, float normalizedTimer)
        {
            Rectangle drawRect = new((int)(position.X * _cellSize.Width) + _gridOffset.X,
                                     (int)(position.Y * _cellSize.Height) + _gridOffset.Y,
                                     _cellSize.Width,
                                     _cellSize.Height);
            _graphics.DrawImage(_bombTextures[colorID],
                                drawRect,
                                0,
                                0,
                                _cellSize.Width,
                                _cellSize.Height,
                                GraphicsUnit.Pixel,
                                GetTransparentAttributes(1.0f - normalizedTimer));
        }

        private static ImageAttributes GetTransparentAttributes(float opacity)
        {
            ColorMatrix matrix = new()
            {
                Matrix33 = opacity
            };

            ImageAttributes attributes = new();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            return attributes;
        }
    }
}
