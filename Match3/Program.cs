using Match3.Core;

namespace Match3
{
    internal static class Program
    {
        private static Map _map;

        private static Gem _red;
        private static Gem _green;
        private static Gem _blue;
        private static Gem _yellow;
        private static Gem _orange;

        //private static Gem _freeSpace;

        private static Point SelectedCell;

        static Program()
        {
            _map = new Map(8, 8);
            _map.Init();

            _red = new Gem(1, new(1, 0));
            _green = new Gem(2, new(2, 0));
            _blue = new Gem(3, new(0, 1));
            _yellow = new Gem(4, new(2, 1));
            _orange = new Gem(5, new(3, 2));

            //_freeSpace = new Gem(0, new(0, 2));
        }

        public static IReadOnlyMap Map => _map;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        public static Point GetAtlasPosition(int ID)
        {
            return new Point();
        }
    }
}