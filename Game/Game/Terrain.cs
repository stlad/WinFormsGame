using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{

    public class Terrain:MapElement
    {
        public Terrain(PointF loc, SizeF tileSize) : base(loc,tileSize) {}
    }

    public class Wall:Terrain
    {
        public Wall(PointF loc, SizeF tileSize) : base(loc, tileSize) { Symbol = 'w'; }
    }


}
