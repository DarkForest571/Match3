namespace Match3
{
    public partial class MainForm : Form
    {
        private Graphics _graphics;
        private BufferedGraphics _bufferedGraphics;

        private Size _cellSize;
        private Point _gridOffset;

        private Rectangle _ImageRect;
        private Bitmap _gemsTexture;
        private Bitmap _gridImage;
        private Bitmap _gridHighlightedImage;


        public MainForm()
        {
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

            _bufferedGraphics.Render();

        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            //Rectangle imageRectangle = new(new(_ImageSize.Width * Program._yellow.AtlasPosition.X, _ImageSize.Height * Program._yellow.AtlasPosition.Y), _ImageSize);
            //_graphics.DrawImage(_gemsTexture, new Rectangle(new(e.X, e.Y), _cellSize), imageRectangle, GraphicsUnit.Pixel);
        }

        private void DrawGrid()
        {
            Rectangle rectangle = new(new(0,0), _cellSize);
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    rectangle.Location = new(x * _cellSize.Width + _gridOffset.X, y * _cellSize.Height + _gridOffset.Y);
                    _graphics.DrawImage(_gridImage, rectangle, _ImageRect, GraphicsUnit.Pixel);
                }
            }
        }
    }
}
