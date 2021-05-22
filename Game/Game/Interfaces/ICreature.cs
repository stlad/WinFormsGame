using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public  interface ICreature
    {
        float Health { get; set; }
        float Speed { get; set; }
    }
}
