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
        public RectangleF AreaOfVision => GetAriaOFVision();
        public Monster(PointF loc, SizeF tileSize) : base(loc,tileSize) 
        {
            Health = 30;
            Symbol = 'm';
            Speed = 2;
        }

        public void Logic()
        {
            if (AreaOfVision.IntersectsWith(BelongsToLevel.Player.HitBox) && ActiveWeapon != null
                && !ActiveWeapon.InAction)
            {
                ActiveWeapon.LightAttack();
            }
                ChangeMonsterPosition();
        }

        private void ChangeMonsterPosition()
        {
            var rnd = new Random();
            var previousDelta = this.Moves.Dequeue();
            var newDelta = new PointF();
            if (!this.Move(previousDelta))
            {
                newDelta = previousDelta;
            }
            else
            {
                newDelta = MapElement.PossibleDeltas[rnd.Next() %8];
            }
            this.DirectionOfView = ConvertDeltaToDirection(newDelta);
            this.Moves.Enqueue(newDelta);
        }

        private RectangleF GetAriaOFVision()
        {

            var size = new SizeF(Size.Width * 3, Size.Height * 3);
            var loc = new PointF(HitBox.Left - Size.Width, Location.Y);
            switch (DirectionOfView)
            {
                case MapElement.Direction.Up:
                    loc = new PointF(HitBox.Left - HitBox.Width,
                   HitBox.Top - HitBox.Height*2);
                    return new RectangleF(loc, size);

                case MapElement.Direction.Down:
                    loc = new PointF(HitBox.Left - HitBox.Width,
                   HitBox.Top);
                    return new RectangleF(loc, size);

                case MapElement.Direction.Left:
                    loc = new PointF(HitBox.Left-HitBox.Width*2,
                    HitBox.Top - HitBox.Height);
                    return new RectangleF(loc, size);

                case MapElement.Direction.Right:
                    loc = new PointF(HitBox.Left,
                    HitBox.Top - HitBox.Height);
                    return new RectangleF(loc, size);
            }
            return new RectangleF();
        }
    }
}
