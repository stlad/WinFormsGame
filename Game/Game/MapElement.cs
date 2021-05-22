using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class MapElement
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            None
        }
        public Model BelongsToLevel { get; set; }
        public PointF Location { get; set; }
        public RectangleF HitBox => new RectangleF(new PointF(Location.X - Size.Width / 2, Location.Y - Size.Height / 2),Size);

        public SizeF Size;
        public static char Symbol { get; set; }
        public MapElement()
        { }
        public MapElement(PointF loc, SizeF tileSize)
        {
            Size = tileSize;
            Location = loc;
        }
    }
}
