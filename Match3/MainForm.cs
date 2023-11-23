using Match3.Core;
using Match3.Core.GameObjects;
using Match3.Utils;
using System.Drawing;
using System.Drawing.Imaging;

namespace Match3
{
    public partial class MainForm : Form
    {
        private Renderer _renderer;

        public MainForm(Game game)
        {
            InitializeComponent();
            BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), DisplayRectangle);
            _renderer = new (game, bufferedGraphics);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _renderer.Draw();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            Vector2<int> point = new(e.Location.X, e.Location.Y);

            point -= _gridOffset;

            if (point < Vector2<int>.Zero)
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

    }
}
