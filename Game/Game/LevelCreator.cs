using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class LevelCreator
    {
        public enum LevelDelta
        {
            Next,
            Preious
        }

        public static string[] MapPattern1 = new string[]
            {
                "wwwwwwwwwwwwwwwwww",
                "ww               w",
                "w               pw",
                "w                w",
                "w                w",
                "w  m   m         w",
                "w                w",
                "wwwwwwwwwwwwwwwwww"
            };

        public static string[] MapPattern2 = new string[]
            {
                "wwwwwwwwwwwwwwwwwwww",
                "w               wwww",
                "w  p           m   w",
                "w        ww        w",
                "w        ww    m   w",
                "w        ww  m     w",
                "wwwwwwwwwwwwwwwwwwww"
            };

        public static string[] MapPattern3 = new string[]
            {
                "wwwwwwwwwwwwwwwwwwwww",
                "w     w  m w        w",
                "w  p  w    w    m   w",
                "w     w    w        w",
                "w                 www",
                "w                   w",
                "wwwwwwwwwwwwwwwwwwwww"
            };


        public static Model CreateLevelFromStringPattern(string[] map)
        {
            var model = new Model();
            model.MapSizeInTiles = new SizeF(map[0].Length, map.Length);

            for(int y=0;y<map.Length;y++)
            {
                for (int x = 0; x < map[0].Length; x++)
                {
                    var loc = new PointF(Model.TileSize.Width * x + Model.TileSize.Width / 2,
                        Model.TileSize.Height * y + Model.TileSize.Height / 2);
                    switch (map[y][x])
                    {
                        case 'w':
                            model.Terrains.Add(new Wall(loc, Model.TileSize) { BelongsToLevel = model });
                            break;
                        case 'p':
                            var player = new Player(loc,Model.TileSize) {BelongsToLevel = model};
                            player.ActiveWeapon = new Sword(player, 5);
                            model.Player = player;
                            model.Creatures.Add(player);
                            break;
                        case 'm':
                            var monster = new Monster(loc, Model.TileSize) {BelongsToLevel = model};
                            monster.ActiveWeapon = new Claws(monster, 10);
                            monster.Moves.Enqueue(new PointF(1, 0));
                            model.Creatures.Add(monster);
                            break;
                        case ' ':
                            continue;
                    }
                }
            }
            return model;
        }
    }
}
