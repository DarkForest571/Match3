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
            float secondsPerFrame = 1f / physicalFrames;
            GameSettings settings = new(physicalFrames,
                                        secondsPerFrame,
                                        physicalFrames / 4,
                                        physicalFrames / 10,
                                        physicalFrames / 4,
                                        physicalFrames / 10,
                                        secondsPerFrame,
                                        10f);

            _game = new(8, 8, settings);

            ApplicationConfiguration.Initialize();

            System.Windows.Forms.Timer timer = new();
            timer.Tick += new EventHandler(Update);
            timer.Interval = 1000 / physicalFrames;
            timer.Start();

            Renderer renderer = new (_game);
            InputHandler inputHandler = new (_game);

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