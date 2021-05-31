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
        public static GameStates GameState { get; set; } = GameStates.MainMenu;
        public int GlobalTime;

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (GameState)
            {
                case GameStates.InGame:
                    e.Graphics.TranslateTransform(View.MapZeroPoint.X, View.MapZeroPoint.Y);
                    View.DrawMap(e, Level);
                    View.DrawCreatures(e, Level, GlobalTime);
                    View.DrawTerrain(e, Level);
                    View.DrawHitAnimation(e, Level, GlobalTime);
                    if (PlayerView) View.DrawLineFromPlayer(e, Level, ClientCursor);
                    if (HitBoxes) View.DrawHitBoxes(e, Level);
                    View.DrawUserInterface(e, Level);
                    //e.Graphics.DrawString(Level.LevelTime.ToString(), new Font("Arial", 10), Brushes.Red, new PointF(400, 400));
                    break;
                case GameStates.Over:
                    e.Graphics.DrawString("GAME OVER", new Font("Arial", 30), Brushes.Black,
                        new Point(ClientSize.Width / 2, ClientSize.Height / 2));
                    
                    GameState = GameStates.MainMenu;
                    break;
                case GameStates.MainMenu:
                    break;
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
            var menuPanel = GetMainMenu();

            ClientSize = new Size((int)Level.MapSize.Width+100, (int)Level.MapSize.Height + 150);

            var timer = new Timer();
            timer.Interval = 15;//1000/600; 
            timer.Tick += (sender, args) =>
            {
                GameControls.ConverKeysToActions(Level);
                ClientSize = new Size((int)Level.MapSize.Width + 100, (int)Level.MapSize.Height + 150);
                GlobalTime = int.MaxValue == GlobalTime? 0: GlobalTime+1;
                if (GameState != GameStates.MainMenu)
                {
                    Controls.Remove(menuPanel);
                    menuPanel.Enabled = false;
                }
                else
                {
                    Controls.Add(menuPanel);
                    menuPanel.Enabled = true;
                }
                Invalidate();
            };
            timer.Start();
            Controls.Add(menuPanel);
            //Controls.Add(menuPanel);
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

            KeyUp += (sender, EventArgs) =>
            {
                EventArgs.SuppressKeyPress = true;
                GameControls.RemovePressedKeyWhenUp(EventArgs.KeyCode, Level.Player);
            };
            KeyDown += (sender, EventArgs) =>
            {
                EventArgs.SuppressKeyPress = true;
                GameControls.AddPressedKeyWhenDown(EventArgs.KeyCode, Level.Player);
            };
            MouseMove += (sender, arg) => ClientCursor = (PointF)arg.Location;
        }
        private void GetLevels()
        {
            Levels = new List<Model>()
            {
                LevelCreator.CreateLevelFromStringPattern(LevelCreator.MapPattern1),
                LevelCreator.CreateLevelFromStringPattern(LevelCreator.MapPattern2),
                LevelCreator.CreateLevelFromStringPattern(LevelCreator.MapPattern3),
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
            Model.GameOver += () => GameState = GameStates.Over;
            Model.LevelComplete += () =>
            {
                var index = Levels.IndexOf(Level) + 1;
                if (index < Levels.Count)
                    Level = Levels[index];
                else GameState = GameStates.MainMenu;
            };
        }
        private TableLayoutPanel GetMainMenu()
        {
            var panel = new TableLayoutPanel();
            panel.Location = new Point(ClientSize.Width/2,ClientSize.Height/2);
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var newGameButton = new Button()
            {
                Size = new Size(300, 200),
                Text = "New Game"
            };
            newGameButton.Click += (s, arg) =>
            {
                Levels.Clear();

                GetLevels();
                GameState = GameStates.InGame;
            };
            panel.Controls.Add(newGameButton, 0, 0);

            var exitButton = new Button()
            {
                Size = new Size(300, 200),
                Text = "EXIT"
            };
            exitButton.Click += (s, args) => Application.Exit();
            panel.Controls.Add(exitButton, 0, 1);
            return panel;
        }
    }
}
