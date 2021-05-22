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
        public RectangleF HitBox => new RectangleF(new PointF(Location.X - Size.Width / 2, Location.Y - Size.Height / 2), Size);

        public SizeF Size;
        public static char Symbol { get; set; }

        public readonly static PointF[] PossibleDeltas = new PointF[8]
        {
            new PointF(0,1),
            new PointF(1,1),
            new PointF(1,0),
            new PointF(1,-1),
            new PointF(0,-1),
            new PointF(-1,-1),
            new PointF(-1,0),
            new PointF(-1,1)
        };

        public MapElement()
        { }
        public MapElement(PointF loc, SizeF tileSize)
        {
            Size = tileSize;
            Location = loc;
        }

        public static MapElement.Direction ConvertDeltaToDirection(PointF delta)
        {
            if (delta.Y > 0) return MapElement.Direction.Down;
            if (delta.Y < 0) return MapElement.Direction.Up;
            if (delta.X > 0) return MapElement.Direction.Right;
            if (delta.X < 0) return MapElement.Direction.Left;
            return MapElement.Direction.None;
        }
    }
}
