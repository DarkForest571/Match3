namespace Match3
{
    internal static class Program
    {
        public static Cell[,] _cellMatrix;

        public static Gem _red;
        public static Gem _green;
        public static Gem _blue;
        public static Gem _yellow;
        public static Gem _orange;

        public static Gem _freeSpace;


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _red = new Gem(1, new(1,0));
            _green = new Gem(2, new(2,0));
            _blue = new Gem(3, new(0,1));
            _yellow = new Gem(4, new(2,1));
            _orange = new Gem(5, new(3,2));
            _freeSpace = new Gem(0, new(0,2));

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}