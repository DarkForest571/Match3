using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3
{
    public class Cell
    {
        private float _yPosition;
        private Gem? _gem;

        public Cell()
        {
            _yPosition = 0.0f;
            _gem = null;
        }
    }
}
