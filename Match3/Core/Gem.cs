using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3
{
    public class Gem
    {
        public readonly int ID;
        public readonly Point AtlasPosition;

        public Gem(int id, Point atlasPosition)
        {
            ID = id;
            AtlasPosition = atlasPosition;
        }
    }
}
