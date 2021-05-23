using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public static class GameControls
    {
        public static event Action<LevelCreator.LevelDelta> LevelChanged;
        public static event Action DebugEnabled;
        public static HashSet<Keys> pressedKeys = new HashSet<Keys>();

        
        public static void ConverKeysToActions(Model level)
        {
            int dx = 0;
            int dy = 0;
            if (pressedKeys.Contains(Keys.W))
            {
                dy -= 1;
                level.Player.DirectionOfView = MapElement.Direction.Up;
            }
            if (pressedKeys.Contains(Keys.S))
            {
                dy += 1;
                level.Player.DirectionOfView = MapElement.Direction.Down;
            }
            if (pressedKeys.Contains(Keys.D))
            {
                dx += 1;
                level.Player.DirectionOfView = MapElement.Direction.Right;
            }
            if (pressedKeys.Contains(Keys.A))
            {
                dx -= 1;
                level.Player.DirectionOfView = MapElement.Direction.Left;
            }
            if (pressedKeys.Contains(Keys.Right)) LevelChanged(LevelCreator.LevelDelta.Next);
            if (pressedKeys.Contains(Keys.Left)) LevelChanged(LevelCreator.LevelDelta.Preious);
            
            level.Player.Move(dx, dy);
            foreach (var creat in level.Creatures)
            {
                if (creat is Monster monster && monster.NeedToMove)
                {
                    monster.Logic();
                }
            }
            level.RemoveDeadCreatures();
        }


        public static void AddPressedKeyWhenDown(Keys key,Player player)
        {
            pressedKeys.Add(key);
        }

        public static void RemovePressedKeyWhenUp(Keys key, Player player)
        {
            var lvl = player.BelongsToLevel;
            if (pressedKeys.Contains(Keys.J)) DebugEnabled();
            if (pressedKeys.Contains(Keys.L))
            {
                if (player.ActiveWeapon is Sword sword
                    && !sword.InAction) 
                    sword.LightAttack();
            }

            if (pressedKeys.Contains(Keys.K))
            {
                player.Health = 0;
                Form1.GameState = GameStates.Over;
            }

            pressedKeys.Remove(key);
        }
    }
}
