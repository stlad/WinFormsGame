using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Game
{
    public static class View
    {
        public static PointF MapZeroPoint = new PointF(50,50);
        public static PointF OffSet(PointF pt) =>
            new PointF(pt.X + MapZeroPoint.X, pt.Y + MapZeroPoint.Y);

        public static Dictionary<string, Image> bitmaps = new Dictionary<string, Image>();

        
        public static void DrawMap(PaintEventArgs e, Model model)
        {
            var g = e.Graphics;
            var image = new Bitmap(bitmaps["Floor3.png"]);
            for (int i =0; i<model.MapSizeInTiles.Width;i++)
            {
                for (int j = 0; j < model.MapSizeInTiles.Height;j++)
                {
                    var rect = new RectangleF(new PointF(i * Model.TileSize.Width, j * Model.TileSize.Height), Model.TileSize);
                    g.DrawImage(image, rect);
                    
                }
            }
            //var playRegion = new RectangleF(new PointF(0,0), model.MapSize);
            //g.FillRectangle(Brushes.Green, playRegion);
        }

        public static void DrawCreatures(PaintEventArgs e, Model model, int time)
        {
            var g = e.Graphics;
            foreach (var creature in model.Creatures)
            {
                var rect = creature.HitBox;
                var image = new Bitmap(bitmaps["MissingTexture.png"]);
                if (creature is Player)
                {
                    image = new Bitmap(bitmaps[$"Player{(time / 40) % 2}.png"]);
                    image.RotateFlip(ConverDirectionToRotation(creature.DirectionOfView));
                    g.DrawImage(image, rect);
                }
                if (creature is Monster)
                {

                    image = new Bitmap(bitmaps[$"Monster{(time / 40) %1}.png"]);
                    image.RotateFlip(ConverDirectionToRotation(creature.DirectionOfView));
                    g.DrawImage(image, rect);

                }
                
            }
        }

        public static void DrawTerrain(PaintEventArgs e, Model model)
        {
            var g = e.Graphics;
            var image = new Bitmap(bitmaps["MissingTexture.png"]);
            foreach (var structure in model.Terrains)
            {

                var rect = structure.HitBox;
                if (structure is Wall)
                {
                    image = new Bitmap(bitmaps[$"Wall.png"]);
                    g.DrawImage(image, rect);
                }
            }
        }

        public static void DrawLineFromPlayer(PaintEventArgs e, Model model, PointF cursorPos)
        {
            var g = e.Graphics;

            var mouse = new PointF(cursorPos.X - MapZeroPoint.X, cursorPos.Y - MapZeroPoint.Y);
            g.DrawLine(new Pen(Brushes.Black),model.Player.Location, mouse);
        }

        public static void DrawHitBoxes(PaintEventArgs e, Model model)
        {
            var g = e.Graphics;
            foreach(var creat in model.Creatures)
            {
                g.DrawRectangle(new Pen(Brushes.Black), creat.HitBox.Location.X, creat.HitBox.Location.Y,
                    creat.HitBox.Width,creat.HitBox.Height);
                if (creat is Player player)
                {
                    var pen = player.ActiveWeapon.InAction ? new Pen(Brushes.Red) : new Pen(Brushes.Blue);
                    g.DrawRectangles(pen, new RectangleF[1] { player.ActiveWeapon.HitBox });
                }
                if (creat is Monster monster)
                {
                    var pen = monster.ActiveWeapon.InAction ? new Pen(Brushes.Red) : new Pen(Brushes.Blue);
                    g.DrawRectangles(pen, new RectangleF[1] { monster.ActiveWeapon.HitBox });
                    g.DrawRectangles(new Pen(Brushes.AliceBlue), new RectangleF[1] { monster.AreaOfVision });
                }
                g.DrawString(creat.Health.ToString(), new Font("Arial", 10), Brushes.Red, creat.Location);
            }
            foreach (var creat in model.Terrains)
            { 
                g.DrawRectangle(new Pen(Brushes.Black), creat.HitBox.Location.X, creat.HitBox.Location.Y,
                    creat.HitBox.Width, creat.HitBox.Height);
            }


        }

        public static void DrawHitAnimation(PaintEventArgs e,Model model,int time)
        {
            var g = e.Graphics;
            var image = new Bitmap(bitmaps["MissingTexture.png"]);
            
            //image = new Bitmap(bitmaps[$"sword{(time / 40) % 2}.png"]);
            //image.RotateFlip(ConverDirectionToRotation(creature.DirectionOfView));

            foreach(var creature in model.Creatures)
            {
                if(creature.ActiveWeapon!=null&&
                    creature.ActiveWeapon.AnimationQueue.Count!=0)
                {
                    var rect = creature.ActiveWeapon.HitBox;
                    var frameNumber = creature.ActiveWeapon.AnimationQueue.Peek();
                    if (model.LevelTime % creature.ActiveWeapon.AnimationFrameTimerInTicks == 0) creature.ActiveWeapon.AnimationQueue.Dequeue();
                    image = new Bitmap(bitmaps[$"Sword{frameNumber}.png"]);
                    image.RotateFlip(ConverDirectionToRotation(creature.DirectionOfView));
                    g.DrawImage(image, rect);
                }    
            }
        }

        public static RotateFlipType ConverDirectionToRotation(MapElement.Direction dir)
        {
            if (dir == MapElement.Direction.Up) return RotateFlipType.Rotate180FlipNone;
            if (dir == MapElement.Direction.Down) return RotateFlipType.RotateNoneFlipNone;
            if (dir == MapElement.Direction.Left) return RotateFlipType.Rotate90FlipNone;
            if (dir == MapElement.Direction.Right) return RotateFlipType.Rotate270FlipNone;
            return RotateFlipType.RotateNoneFlipNone;
        }
        //public static FlipSprite
    }
}
