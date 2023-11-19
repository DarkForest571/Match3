namespace Match3
{
    public partial class MainForm : Form
    {
        private Graphics _graphics;

        private Size _cellSize;

        private Size _ImageSize;
        private Bitmap _gemsTexture;

        public MainForm()
        {
            InitializeComponent();

            _graphics = CreateGraphics();
            _cellSize = new Size(100, 100);

            _ImageSize = new Size(512, 512);
            _gemsTexture = new Bitmap("..\\..\\..\\..\\img\\sprite_fruit_face_atlas_01.png");
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _graphics.Clear(Color.Pink);
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            Rectangle imageRectangle = new(new(_ImageSize.Width * Program._yellow.AtlasPosition.X, _ImageSize.Height * Program._yellow.AtlasPosition.Y), _ImageSize);
            _graphics.DrawImage(_gemsTexture, new Rectangle(new(e.X, e.Y), _cellSize), imageRectangle, GraphicsUnit.Pixel);
        }
    }
}
