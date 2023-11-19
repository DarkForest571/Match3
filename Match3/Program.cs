using Match3.Core;

namespace Match3
{
    internal static class Program
    {
        private static Game _game;
        private static MainForm _mainForm;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _game = new();
            _game.Init();

            ApplicationConfiguration.Initialize();

            System.Windows.Forms.Timer timer = new();
            timer.Tick += new EventHandler(Update);
            timer.Interval = 20;
            timer.Start();

            _mainForm = new MainForm(_game);
            Application.Run(_mainForm);
        }

        private static void Update(object? sender, EventArgs e)
        {
            _game.Update();
            _mainForm.Invalidate(new Rectangle(0, 0, 1, 1));
        }
    }
}