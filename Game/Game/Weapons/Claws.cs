using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Claws : IWeapon
    {
        public string Name { get; } = "Claws";
        public float Damage { get; set; }
        public static event Action<Creature> Attack;
        public RectangleF HitBox { get { return GetHitBox(); } }
        public Creature ParentCreature { get; set; }
        public int LightAttackCoolDown { get; set; } = 1000;
        public bool InAction { get; set; }
        public int AnimationFrameTimerInTicks { get; set; } = 1;
        public Queue<int> AnimationQueue { get; set; }
        public Claws(Creature creat, float damage)
        {
            Damage = damage;
            ParentCreature = creat;
            AnimationQueue = new Queue<int>();
        }
        public Claws() { }


        private RectangleF GetHitBox()
        {
            var size = new SizeF(ParentCreature.HitBox.Width * 2, ParentCreature.HitBox.Height * 1f);
            var loc = new PointF();
            switch (ParentCreature.DirectionOfView)
            {
                case MapElement.Direction.Up:
                    loc = new PointF(ParentCreature.HitBox.Left - ParentCreature.HitBox.Width / 2,
                   ParentCreature.HitBox.Top - size.Height);
                    return new RectangleF(loc, size);

                case MapElement.Direction.Down:
                    loc = new PointF(ParentCreature.HitBox.Left - ParentCreature.HitBox.Width / 2,
                   ParentCreature.HitBox.Bottom);
                    return new RectangleF(loc, size);

                case MapElement.Direction.Left:
                    loc = new PointF(ParentCreature.HitBox.Left - size.Height,
                    ParentCreature.HitBox.Top - size.Width / 4);
                    return new RectangleF(loc, new SizeF(size.Height, size.Width));

                case MapElement.Direction.Right:
                    loc = new PointF(ParentCreature.HitBox.Right,
                    ParentCreature.HitBox.Top - size.Width / 4);
                    return new RectangleF(loc, new SizeF(size.Height, size.Width));
            }
            return new RectangleF();
        }

        public async void LightAttack()
        {
            InAction = true;
            AnimationQueue = new Queue<int>(new int[] { 0, 1, 2 });
            Attack(ParentCreature);
            await Task.Delay(LightAttackCoolDown);
            InAction = false;
        }
    }

}
