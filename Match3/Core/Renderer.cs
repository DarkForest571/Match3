using Match3.Core.GameObjects;
using Match3.Utils;
using System.Drawing.Imaging;

namespace Match3.Core
{
    public class Renderer
    {
        private readonly Game _game;

        private BufferedGraphics _bufferedGraphics;

        private Vector2<int> _cellSize;
        private Vector2<int> _gridOffset;

        private readonly List<Bitmap> _gemsTextures;
        private readonly List<Bitmap> _bombTextures;
        private readonly List<Bitmap> _horArrowsTextures;
        private readonly List<Bitmap> _verArrowsTextures;
        private Bitmap _gridTexture;
        private Bitmap _selectedGridTexture;

        public Renderer(Game game, Vector2<int> cellSize, Vector2<int> gridOffset)
        {
            _game = game;

            _cellSize = cellSize;
            _gridOffset = gridOffset;

            _gemsTextures = [];
            _bombTextures = [];
            _horArrowsTextures = [];
            _verArrowsTextures = [];
        }

        public void SetBufferedGraphics(BufferedGraphics bufferedGraphics)
        {
            _bufferedGraphics = bufferedGraphics;
        }

        public void Draw()
        {
            if (_bufferedGraphics is null)
                return;
            _bufferedGraphics.Graphics.Clear(Color.MidnightBlue);

            DrawGrid();
            DrawGems(_game.CurrentFrame);
            DrawDestroyers();

            _bufferedGraphics.Render();
        }

        private void LoadTextures()
        {
            Bitmap gemsTexture = new("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            gemsTexture = new(gemsTexture, 400, 400);

            LoadFromAtlas(_cellSize, gemPositionsInAtlas, _gemsTextures, gemsTexture);

            Bitmap bonusesTexture = new("..\\..\\..\\..\\img\\sprite_arrow_atlas.png");
            bonusesTexture = new(bonusesTexture, 400, 400);

            LoadFromAtlas(_cellSize, bombPositionsInAtlas, _bombTextures, bonusesTexture);

            LoadFromAtlas(new Size(_cellSize.X / 2, _cellSize.Y),
                          horArrowsPositionsInAtlas,
                          _horArrowsTextures,
                          bonusesTexture);

            LoadFromAtlas(new Size(_cellSize.X, _cellSize.Y / 2),
                          verArrowsPositionsInAtlas,
                          _verArrowsTextures,
                          bonusesTexture);

            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridImage = new Bitmap(_gridImage, _cellSize);
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
            _gridHighlightedImage = new Bitmap(_gridHighlightedImage, _cellSize);
        }

        private void LoadFromAtlas(Bitmap atlas, Vector2<int> atlasSize, Vector2<int>[] positions, List<Bitmap> images)
        {
            Size newAtlasSize = new (_cellSize.X * atlasSize.X, _cellSize.Y * atlasSize.X);
            atlas = new Bitmap(atlas, newAtlasSize);
            for (int i = 0; i < positions.Length; i++)
            {
                Point position = new(_cellSize.X * positions[i].X, _cellSize.Y * positions[i].Y);
                Rectangle rectangle = new(position.X, position.Y, _cellSize.X, _cellSize.Y);
                images.Add(atlas.Clone(rectangle, atlas.PixelFormat));
            }
        }


        private void DrawGrid()
        {
            //IReadOnlyMap map = _game.Map; // Can be used to draw obstacle cells
            Rectangle drawRect = new(0, 0, _cellSize.X, _cellSize.Y);
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    // Obstacle check here
                    drawRect.Location = new(x * _cellSize.X + _gridOffset.X,
                                            y * _cellSize.Y + _gridOffset.Y);
                    _bufferedGraphics.Graphics.DrawImage(_gridTexture, drawRect);
                }
            }

            if (_game.SelectedCell is not null)
            {
                Vector2<int>? position = _game.SelectedCell;
                if (position is not null)
                {
                    drawRect.Location = new(position.Value.X * _cellSize.X + _gridOffset.X,
                                            position.Value.Y * _cellSize.Y + _gridOffset.Y);
                    _bufferedGraphics.Graphics.DrawImage(_selectedGridTexture, drawRect);
                }
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
            Rectangle drawRect = new((int)(position.X * _cellSize.X) + _gridOffset.X,
                                     (int)(position.Y * _cellSize.Y) + _gridOffset.Y,
                                     _cellSize.X,
                                     _cellSize.Y);
            _bufferedGraphics.Graphics.DrawImage(_gemsTextures[colorID], drawRect);
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
                ? new((int)(position.X * _cellSize.X) + _gridOffset.X,
                      (int)(position.Y * _cellSize.Y) + _gridOffset.Y,
                      _cellSize.X,
                      _cellSize.Y / 2)
                 : new((int)(position.X * _cellSize.X) + _gridOffset.X,
                       (int)(position.Y * _cellSize.Y) + _gridOffset.Y,
                       _cellSize.X / 2,
                       _cellSize.Y);

            if (isVertical)
                _bufferedGraphics.Graphics.DrawImage(_verArrowsTextures[colorID], drawRect);
            else
                _bufferedGraphics.Graphics.DrawImage(_horArrowsTextures[colorID], drawRect);
        }

        private void DrawBombGem(IReadOnlyBombGem bombGem, int frame) =>
            DrawBombGem(bombGem.ColorID, bombGem.Position, bombGem.NormalizedTimer(frame));

        private void DrawBombGem(int colorID, Vector2<float> position, float normalizedTimer)
        {
            Rectangle drawRect = new((int)(position.X * _cellSize.X) + _gridOffset.X,
                                     (int)(position.Y * _cellSize.Y) + _gridOffset.Y,
                                     _cellSize.X,
                                     _cellSize.Y);
            _bufferedGraphics.Graphics.DrawImage(_bombTextures[colorID],
                                                 drawRect,
                                                 0,
                                                 0,
                                                 _cellSize.X,
                                                 _cellSize.Y,
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
