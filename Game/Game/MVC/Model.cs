using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public enum GameStates
    {
        Over,
        LevelChanging,
        InGame,
        MainMenu
    }

    public class Model
    {
        public static event Action LevelComplete;
        public static event Action GameOver;
        public int LevelId { get; set; }
        public bool IsOver { get; set; } = false;
        public Player Player { get; set; }
        public static SizeF TileSize = new SizeF(64,64);
        public SizeF MapSizeInTiles { get; set; }
        public SizeF MapSize => new SizeF(MapSizeInTiles.Width * TileSize.Width, MapSizeInTiles.Height * TileSize.Height);
        public List<Creature> Creatures { get; set; }
        public List<Terrain> Terrains { get; set; }
        public int LevelTime { get; private set; }
        private Timer levelTimer { get; set; }
        public Model()
        {
            Creature.CreatureTryingToMove += (creature,nextPosition,level) =>  
                GetIntersectionWithWalls(creature,nextPosition, level);

            Sword.Attack += (Creature) =>
                AttackIntersection(Creature);
            Claws.Attack += (Creature) =>
                AttackIntersection(Creature);

            Creatures = new List<Creature>();
            Terrains = new List<Terrain>();

            levelTimer = new Timer();
            levelTimer.Interval = 1;
            levelTimer.Tick+=(s,args)=> LevelTime = int.MaxValue == LevelTime ? 0 : LevelTime + 1;
            var task = new Task(() => levelTimer.Start());
            task.Start();
            levelTimer.Start();
        }

        public static RectangleF GetIntersectionWithWalls(Creature creature, RectangleF hitBox, Model level)
        {
            foreach(var terr in level.Terrains)
            {
                var rect = RectangleF.Intersect(hitBox, terr.HitBox);
                if (rect != RectangleF.Empty) return rect; 
            }
            return RectangleF.Empty;
        }

        public static void AttackIntersection(Creature creature)
        {
            var level = creature.BelongsToLevel;
            foreach (var conflictCreature in level.Creatures)
            {
                if (!creature.ActiveWeapon.InAction) break;
                if (creature == conflictCreature) continue;
                if (creature is Monster && conflictCreature is Monster) continue;
                var rect = RectangleF.Intersect(creature.ActiveWeapon.HitBox, conflictCreature.HitBox);
                if (rect != RectangleF.Empty)
                { 
                    conflictCreature.Health -= creature.ActiveWeapon.Damage;
                    if (conflictCreature is Player && !conflictCreature.IsAlive)
                        GameOver();
                    if (conflictCreature is Monster && !conflictCreature.IsAlive && level.Creatures.Count == 1)
                        LevelComplete();
                }
            }
            //level.RemoveDeadCreatures();
        }

        public void RemoveDeadCreatures() => this.Creatures.RemoveAll(n => n.IsAlive == false);
        
    }
}
