using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Game
{
    public partial class Form1 : Form
    {
        public List<Model> Levels { get; set; }
        public PointF ClientCursor { get; set; }
        public bool HitBoxes { get; set; } = false;
        public bool PlayerView { get; set; } = false;
        public TableLayoutPanel DebugPanel { get; set; }
        public Model Level = new Model();
        public static bool IsOver { get; set; } = false;
        public int GlobalTime;
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!IsOver)
            {
                e.Graphics.TranslateTransform(View.MapZeroPoint.X, View.MapZeroPoint.Y);
                View.DrawMap(e, Level);
                View.DrawCreatures(e, Level, GlobalTime);
                View.DrawTerrain(e, Level);
                View.DrawHitAnimation(e, Level, GlobalTime);
                if (PlayerView) View.DrawLineFromPlayer(e, Level, ClientCursor);
                if (HitBoxes) View.DrawHitBoxes(e, Level);
                //e.Graphics.DrawString(Level.LevelTime.ToString(), new Font("Arial", 10), Brushes.Red, new PointF(400, 400));
            }
            else
            {
                e.Graphics.DrawString("GAME OVER, bye", new Font("Arial", 30), Brushes.Black, 
                    new Point(ClientSize.Width / 2, ClientSize.Height / 2));
            }
        }


        public Form1()
        {
            DoubleBuffered = true;
            InitializeComponent();

            GetGameControllsEvents();
            GetLevels();
            GetImages();
            GetModelEvents();


            
                //if (Level.Creatures[1] is Monster monster)
                //    monster.GetPathToPoint(Level.Player.Location);
            
            ClientSize = new Size((int)Level.MapSize.Width+100, (int)Level.MapSize.Height + 150);

            var timer = new Timer();
            timer.Interval = 15;//1000/600; 
            timer.Tick += (sender, args) =>
            {
                GameControls.ConverKeysToActions(Level);
                GlobalTime = int.MaxValue == GlobalTime? 0: GlobalTime+1;
                Invalidate();
            };
            timer.Start();
        }


        private void GetGameControllsEvents()
        {
            GameControls.LevelChanged += (levelDelta) =>
            {
                var indexDelta = levelDelta == LevelCreator.LevelDelta.Next ? 1 : -1;
                var currentIndex = Levels.IndexOf(Level);
                if (currentIndex == 0 && indexDelta == -1) indexDelta = 0;
                if (currentIndex == Levels.Count - 1 && indexDelta == 1) indexDelta = 0;
                Level = Levels[currentIndex + indexDelta];
                ClientSize = new Size((int)Level.MapSize.Width + 100, (int)Level.MapSize.Height + 150);
            };
            GameControls.DebugEnabled += () => { HitBoxes = !HitBoxes; PlayerView = !PlayerView; };

            KeyUp += (sender, EventArgs) => GameControls.RemovePressedKeyWhenUp(EventArgs.KeyCode, Level.Player);
            KeyDown += (sender, EventArgs) => GameControls.AddPressedKeyWhenDown(EventArgs.KeyCode, Level.Player);
            MouseMove += (sender, arg) => ClientCursor = (PointF)arg.Location;
        }
        private void GetLevels()
        {
            Levels = new List<Model>()
            {
                LevelCreator.CreateLevelFromStringPattern(LevelCreator.MapPattern1),
                LevelCreator.CreateLevelFromStringPattern(LevelCreator.MapPattern2),
            };
            Level = Levels.First();
        }
        private void GetImages()
        {
            var imagesDirectory = new DirectoryInfo("Assets");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                View.bitmaps[e.Name] = Image.FromFile(e.FullName);
        }
        private void GetModelEvents()
        {
            Model.GameOver += () => IsOver = true;
        }

        private void GetGameOverButtons()
        {
            var exitButton = new Button()
            {
                Location = new Point(ClientSize.Width / 2, ClientSize.Height / 2),
                Size = new Size(50, 50),
                Text = "EXIT"
            };
        }

        private void GetButtons()
        {
            var hitBoxButton = new Button()
            {
                Text = $"Show Hit Boxes  ({HitBoxes})",
                Location = new Point((int)View.MapZeroPoint.X, (int)(View.MapZeroPoint.Y + Level.MapSizeInTiles.Height)),
                Size = new Size(200, 100)
            };
            hitBoxButton.Click += (sender, args) =>
            {
                HitBoxes = !HitBoxes;
                hitBoxButton.Text = $"Show Hit Boxes({HitBoxes})";
            };
            Controls.Add(hitBoxButton);

            var playerViewButton = new Button()
            {
                Text = $"Show Player View Line ({PlayerView})",
                Location = new Point((int)View.MapZeroPoint.X, hitBoxButton.Bottom),
                Size = new Size(200, 100)
            };
            playerViewButton.Click += (sender, args) =>
            {
                PlayerView = !PlayerView;
                playerViewButton.Text = $"Show Player View Line({PlayerView})";
            };
            Controls.Add(playerViewButton);
        }
        private TableLayoutPanel GetDebugPanel()
        {
            var panel = new TableLayoutPanel();
            panel.Location = new Point((int)View.MapZeroPoint.X, (int)(View.MapZeroPoint.Y + Level.MapSizeInTiles.Height));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var hitBoxButton = new Button()
            {
                Text = $"Show Hit Boxes  ({HitBoxes})",
                Size = new Size(200,100)
            };
            hitBoxButton.Click += (sender, args) => 
            {
                HitBoxes = !HitBoxes;
                hitBoxButton.Text = $"Show Hit Boxes({HitBoxes})";
            };
            panel.Controls.Add(hitBoxButton, 0, 0);
            
            var playerViewButton = new Button()
            {
                Text = $"Show Player View Line ({PlayerView})",
                Size = new Size(200, 100)
            };
            playerViewButton.Click += (sender, args) =>
            {
                PlayerView = !PlayerView;
                playerViewButton.Text = $"Show Player View Line({PlayerView})";
            };
            panel.Controls.Add(playerViewButton, 0, 1);

            
            return panel;
        }

    }
}
