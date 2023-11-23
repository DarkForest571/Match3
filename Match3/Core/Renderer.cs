using Match3.Core.GameObjects;
using Match3.Utils;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Xml;

namespace Match3.Core
{
    public class Renderer
    {
        private readonly Game _game;

        private BufferedGraphics _bufferedGraphics;

        private Vector2<int> _cellSize;
        private Vector2<int> _gridOffset;

        private  List<Bitmap> _gemsTextures;
        private  List<Bitmap> _bombTextures;
        private  List<Bitmap> _upArrowsTextures;
        private  List<Bitmap> _downArrowsTextures;
        private  List<Bitmap> _leftArrowsTextures;
        private  List<Bitmap> _rightArrowsTextures;
        private Bitmap _gridTexture;
        private Bitmap _selectedGridTexture;

        public Renderer(Game game, Vector2<int> cellSize, Vector2<int> gridOffset)
        {
            _game = game;

            _cellSize = cellSize;
            _gridOffset = gridOffset;

            _gemsTextures = [];
            _bombTextures = [];
            _upArrowsTextures = [];
            _downArrowsTextures = [];
            _leftArrowsTextures = [];
            _rightArrowsTextures = [];
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
            XmlReader reader = XmlReader.Create("..\\..\\..\\..\\config\\config.xml");

            reader.MoveToContent();
            reader.ReadStartElement();
            reader.ReadToNextSibling("images");

            Dictionary<string, List<Bitmap>> arraysOfImages = [];
            for (int i = 0; i < 8; ++i)
            {
                reader.ReadStartElement();
                reader.Read();
                string tag = reader.Name;

                // atlas parameters
                reader.MoveToFirstAttribute();
                string atlasPath = reader.Value;
                reader.MoveToNextAttribute();
                int xSize = Convert.ToInt32(reader.Value);
                reader.MoveToNextAttribute();
                int ySize = Convert.ToInt32(reader.Value);
                Vector2<int> atlasSize = new(xSize, ySize);

                reader.MoveToContent();
                reader.ReadStartElement();
                reader.Read();

                List<Vector2<int>> positions = [];
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    reader.MoveToFirstAttribute();
                    int x = Convert.ToInt32(reader.Value);
                    reader.MoveToNextAttribute();
                    int y = Convert.ToInt32(reader.Value);
                    positions.Add(new(x, y));
                    reader.MoveToElement();
                    reader.Read();
                    reader.Read();
                }
                List<Bitmap> bitmaps = [];
                LoadFromAtlas("..\\..\\..\\..\\" + atlasPath, atlasSize, positions, bitmaps);
                arraysOfImages.Add(tag, bitmaps);
            }
            reader.Dispose();

            _gridTexture = arraysOfImages["gridImage"][0];
            _selectedGridTexture = arraysOfImages["selectedGridImage"][0];
            _gemsTextures = arraysOfImages["gemImage"];
            _bombTextures = arraysOfImages["bombGemImage"];
            _upArrowsTextures = arraysOfImages["arrowUpImage"];
            _downArrowsTextures = arraysOfImages["arrowDownImage"];
            _leftArrowsTextures = arraysOfImages["arrowLeftImage"];
            _rightArrowsTextures = arraysOfImages["arrowRightImage"];
        }

        private void LoadFromAtlas(string atlasPath, Vector2<int> atlasSize, List<Vector2<int>> positions, List<Bitmap> images)
        {
            Bitmap atlas = new(atlasPath);
            Size newAtlasSize = new(_cellSize.X * atlasSize.X, _cellSize.Y * atlasSize.X);
            atlas = new Bitmap(atlas, newAtlasSize);
            for (int i = 0; i < positions.Count; i++)
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
                _bufferedGraphics.Graphics.DrawImage(_upArrowsTextures[colorID], drawRect);
            else
                _bufferedGraphics.Graphics.DrawImage(_downArrowsTextures[colorID], drawRect);
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
