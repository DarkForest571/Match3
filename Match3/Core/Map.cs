using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Core
{
    public interface IReadOnlyMap
    {
        public Size Size { get; }

        public Cell? CellAt(int x, int y);
    }

    public class Map : IReadOnlyMap
    {
        private Size _size;

        private Cell[,] _cellMatrix;

        public Map(int x, int y)
        {
            _size = new Size(x, y);
            _cellMatrix = new Cell[x, y];
        }

        public Map(Size size) : this(size.Width, size.Height) { }

        public Size Size => _size;

        public Cell? CellAt(int x, int y) => _cellMatrix[x, y];
    }
}
