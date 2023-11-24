using Match3.Core.GameObjects;
using Match3.Core.UI;
using Match3.Utils;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Match3.Core
{
    public class Renderer
    {
        private readonly Game _game;

        private BufferedGraphics _bufferedGraphics;

        private Vector2<int> _cellSize;
        private Vector2<int> _gridOffset;

        private List<Bitmap> _gemsTextures;
        private List<Bitmap> _bombTextures;
        private List<Bitmap> _upArrowsTextures;
        private List<Bitmap> _downArrowsTextures;
        private List<Bitmap> _leftArrowsTextures;
        private List<Bitmap> _rightArrowsTextures;
        private Bitmap _gridTexture;
        private Bitmap _selectedGridTexture;

        public Renderer(Game game)
        {
            _game = game;

            _cellSize = new(100, 100);
            _gridOffset = new(100, 0);

            _gemsTextures = [];
            _bombTextures = [];
            _upArrowsTextures = [];
            _downArrowsTextures = [];
            _leftArrowsTextures = [];
            _rightArrowsTextures = [];

            LoadTextures();
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

            UIFrame frame = _game.CurrentUIFrame;
            if (frame.Elements is not null)
            {
                foreach (var element in frame.Elements)
                {
                    Point position = new(element.Position.X, element.Position.Y);
                    if (element is MenuButton button)
                        _bufferedGraphics.Graphics.DrawImage(button.Image, position);
                    Point textPosition = new(position.X + element.Size.X / 2,
                                             position.Y + element.Size.Y / 2);
                    Font font = new Font(FontFamily.GenericSerif, element.TextSize);
                    Brush brush = new SolidBrush(Color.FromArgb(255, 255, 255));
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    _bufferedGraphics.Graphics.DrawString(element.Text, font, brush, textPosition, stringFormat);
                }
            }

            if (frame.Title == "Game scene")
            {
                DrawGrid();
                DrawGems(_game.CurrentFrame);
                DrawDestroyers();
            }

            _bufferedGraphics.Render();
        }

        private void LoadTextures()
        {
            XmlReader reader = XmlReader.Create("..\\..\\..\\..\\config\\config.xml");

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            XElement imagesInfo = XElement.Load(reader);
            imagesInfo = imagesInfo.Elements().ElementAt(0);

            Dictionary<string, List<Bitmap>> arraysOfImages = [];
            foreach (var elementOfType in imagesInfo.Elements())
            {
                var attributes = elementOfType.Attributes();
                string tag = elementOfType.Name.LocalName;

                string atlasPath = attributes.ElementAt(0).Value;
                Vector2<int> atlasSize = new(Convert.ToInt32(attributes.ElementAt(1).Value),
                                             Convert.ToInt32(attributes.ElementAt(2).Value));

                List<Vector2<int>> positions = [];
                foreach (var atlasPosition in elementOfType.Elements())
                {
                    attributes = atlasPosition.Attributes();
                    positions.Add(new(Convert.ToInt32(attributes.ElementAt(0).Value),
                                      Convert.ToInt32(attributes.ElementAt(1).Value)));
                }
                List<Bitmap> bitmaps = [];
                Vector2<int> imageSize = tag switch
                {
                    "arrowUpImage" or "arrowDownImage" => new(_cellSize.X, _cellSize.Y / 2),
                    "arrowLeftImage" or "arrowRightImage" => new(_cellSize.X / 2, _cellSize.Y),
                    _ => new(_cellSize.X, _cellSize.Y)
                };
                LoadFromAtlas("..\\..\\..\\..\\" + atlasPath, atlasSize, imageSize, positions, bitmaps);
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

        private void LoadFromAtlas(string atlasPath,
                                   Vector2<int> atlasSize,
                                   Vector2<int> imageSize,
                                   List<Vector2<int>> positions,
                                   List<Bitmap> images)
        {
            Bitmap atlas = new(atlasPath);

            Size newAtlasSize = new(imageSize.X * atlasSize.X, imageSize.Y * atlasSize.Y);
            atlas = new Bitmap(atlas, newAtlasSize);
            for (int i = 0; i < positions.Count; i++)
            {
                Point position = new(imageSize.X * positions[i].X, imageSize.Y * positions[i].Y);
                Rectangle rectangle = new(position.X, position.Y, imageSize.X, imageSize.Y);
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
            switch (direction)
            {
                case Direction.Up:
                    _bufferedGraphics.Graphics.DrawImage(_upArrowsTextures[colorID], drawRect);
                    break;
                case Direction.Down:
                    drawRect.Y += _cellSize.Y / 2;
                    _bufferedGraphics.Graphics.DrawImage(_downArrowsTextures[colorID], drawRect);
                    break;
                case Direction.Left:
                    _bufferedGraphics.Graphics.DrawImage(_leftArrowsTextures[colorID], drawRect);
                    break;
                case Direction.Right:
                    drawRect.X += _cellSize.X / 2;
                    _bufferedGraphics.Graphics.DrawImage(_rightArrowsTextures[colorID], drawRect);
                    break;
            }
        }

        private void DrawBombGem(IReadOnlyBombGem bombGem, int frame) =>
            DrawBombGem(bombGem.ColorID, bombGem.Position, bombGem.NormalizedTimer(frame));

        private void DrawBombGem(int colorID, Vector2<float> position, float normalizedTimer)
        {
            normalizedTimer -= 1;
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
