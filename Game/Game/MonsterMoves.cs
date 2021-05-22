using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class MonsterMoves
    {
        public static void GetNextMove(Monster monster, PointF previousDelta)
        {
            var newDelta = new PointF();
            if (!monster.Move(previousDelta))
            {
                newDelta = previousDelta;
            }
            else
            {

            }
            monster.DirectionOfView = ConvertDeltaToDirection(newDelta);
            monster.Moves.Enqueue(newDelta);
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
