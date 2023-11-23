using Match3.Core;
using System.Numerics;

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
            int physicalFrames = 50;
            _game = new(8, 8, physicalFrames);
            _game.Init();

            ApplicationConfiguration.Initialize();

            System.Windows.Forms.Timer timer = new();
            timer.Tick += new EventHandler(Update);
            timer.Interval = 1000 / physicalFrames;
            timer.Start();


            Renderer renderer = new (_game, new(100,100), new(100,0));
            InputHandler inputHandler = new (_game, new(100, 100), new(100, 0));

            _mainForm = new MainForm(renderer, inputHandler);
            Application.Run(_mainForm);
        }

        private static void Update(object? sender, EventArgs e)
        {
            _game.Update();
            _mainForm.Invalidate(new Rectangle(0, 0, 1, 1)); // TODO Fix it
        }
    }
}