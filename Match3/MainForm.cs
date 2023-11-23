using Match3.Core;
using Match3.Utils;

namespace Match3
{
    public partial class MainForm : Form
    {
        private readonly Renderer _renderer;
        private readonly InputHandler _inputHandler;

        public MainForm(Renderer renderer, InputHandler inputHandler)
        {
            InitializeComponent();
            BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            renderer.SetBufferedGraphics(bufferedGraphics);
            _renderer = renderer;
            _inputHandler = inputHandler;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _renderer.Draw();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            Vector2<int> position = new(e.Location.X, e.Location.Y);
            _inputHandler.HandleMouseClick(position);
        }
    }
}
