using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Creature : MapElement, ICreature
    {
        public static event Func<Creature,RectangleF, Model, RectangleF> CreatureTryingToMove;
        public float Health { get; set; }
        public bool IsAlive => this.Health > 0;
        public float Speed { get; set; }=10;
        public IWeapon ActiveWeapon { get; set; }
        public Direction DirectionOfView { get; set; }
        public Creature(PointF loc, SizeF tileSize) : base(loc,tileSize) { }

        public bool Move(float dx, float dy)
        {
            bool touched = false;
            var nextLoc = new PointF(Location.X, Location.Y);
            for (int count = 0; count < Speed; count++)
            {

                var ghostHitBox = new RectangleF(new PointF(nextLoc.X+dx - HitBox.Width / 2, nextLoc.Y+dy - HitBox.Height / 2),
                    Size);
                var rectOfIntersect = CreatureTryingToMove(this,ghostHitBox, BelongsToLevel);
                if (rectOfIntersect.Width != 0)
                {
                    touched = true;
                    dx = 0;
                }
                if (rectOfIntersect.Height != 0)
                {
                    touched = true;
                    dy = 0;
                }
                nextLoc.X += dx;
                nextLoc.Y += dy;
            }

            Location = nextLoc;
            return touched;
        }
        public bool Move(PointF delta) => Move(delta.X, delta.Y);


    }

    public class Player : Creature
    {
        public Player(PointF loc, SizeF tileSize) : base(loc, tileSize) 
        {
            Health = 100;
            Symbol = 'p';
        }
    }

    public class Monster : Creature
    {
        public bool NeedToMove => Moves.Count != 0;
        public Queue<PointF> Moves = new Queue<PointF>();
        public Monster(PointF loc, SizeF tileSize) : base(loc,tileSize) 
        {
            Health = 20;
            Symbol = 'm';
            Speed = 2;
        }

        public void GetPathToPoint(PointF pt)
        {
            var task = new Task<List<PointF>>(() => PathFinder.FindPaths(this.BelongsToLevel, this, pt));
            task.Start();
            task.Wait();
            var list = task.Result;
            var moveTask = new Task(() => ConvertPointsToMoves(list));
            moveTask.Start(); 
        }

        private void ConvertPointsToMoves(List<PointF> pts)
        {
            var currentDelta = new PointF(0, 0);
            var previous = pts[0];
            foreach (var pt in pts)
            {
                currentDelta = new PointF(pt.X - previous.X, pt.Y - pt.Y);
                //MovesQueue.Enqueue(() => Move(currentDelta.X, currentDelta.Y));
                Moves.Enqueue(currentDelta);
                previous = pt;
            }
        }

    }
}
