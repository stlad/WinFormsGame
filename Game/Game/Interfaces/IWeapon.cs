using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public interface IWeapon
    {
        float Damage { get; set; }
        RectangleF HitBox { get;}
        bool InAction { get; set; }
        int LightAttackCoolDown { get; set; }
        Creature ParentCreature { get; set; }
        Queue<int> AnimationQueue { get; set; }
        
        int AnimationFrameTimerInTicks { get; set; }
    }

    
}
