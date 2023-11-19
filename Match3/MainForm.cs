namespace Match3
{
    public partial class MainForm : Form
    {
        private Graphics _graphics;
        private Size _cellSize;
        private Point _gridOffset;

        private Size _ImageSize;
        private Bitmap _gemsTexture;
        private Bitmap _gridImage;
        private Bitmap _gridHighlightedImage;

        public MainForm()
        {
            InitializeComponent();

            _graphics = CreateGraphics();
            _cellSize = new Size(100, 100);
            _gridOffset = new Point(100, 0);

            _ImageSize = new Size(512, 512);
            _gemsTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
            _gridImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid.png");
            _gridHighlightedImage = new Bitmap("..\\..\\..\\..\\img\\ingame_grid_highlighted.png");
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _graphics.Clear(Color.Pink);
            DrawGrid();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            //Rectangle imageRectangle = new(new(_ImageSize.Width * Program._yellow.AtlasPosition.X, _ImageSize.Height * Program._yellow.AtlasPosition.Y), _ImageSize);
            //_graphics.DrawImage(_gemsTexture, new Rectangle(new(e.X, e.Y), _cellSize), imageRectangle, GraphicsUnit.Pixel);
        }

        private void DrawGrid()
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _graphics.DrawImage(_gridImage, i * _cellSize.Width, j * _cellSize.Height, _cellSize.Width, _cellSize.Height);

                }
            }
        }
    }
}
