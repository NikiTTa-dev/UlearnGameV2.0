using System;
using System.Drawing;
using System.Windows.Forms;

namespace UlearnGame
{
    public partial class GameForm : Form
    {
        PointF Center { get; set; }
        PointF Offset { get; set; }
        PointF MouseLocation { get; set; }
        Game GameModel { get; }
        PictureBox GamePictureBox { get; set; }
        Label WinningLabel { get; set; }
        Label TutorialLabel { get; set; }
        Button StartButton { get; set; }
        Button ContinueButton { get; set; }
        Button NextLevelButton { get; set; }
        Button ExitButton { get; set; }
        Timer PaintTimer { get; set; }
        Timer ClickTimer { get; set; }
        Timer ClickIntervalTimer { get; set; }
        Bitmap SmallFeet { get; }
        Bitmap SmallFeet2 { get; }
        bool IsFeetFlipped { get; set; }
        bool IsMouseDown { get; set; }
        bool IsStepped { get; set; }
        bool IsGameStarted { get; set; }
        bool IsGamePaused { get; set; }
        bool IsWallsVisualized { get; set; }

        public GameForm(Game gameModel)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1920, 1080);
            BackColor = Color.Black;
            this.GameModel = gameModel;
            StartPosition = FormStartPosition.CenterScreen;
            Center = new PointF(Width / 2, Height / 2);
            Offset = new PointF(0, 0);

            SmallFeet = (Bitmap)Image.FromFile(@"Resources/Pictures/feetsmall.png");
            SmallFeet2 = (Bitmap)Image.FromFile(@"Resources/Pictures/feetsmall2.png");
            SetTimers();
            SetLabels();
            SetButtons();

            Controls.Add(StartButton);
            Controls.Add(ExitButton);

            this.KeyDown += GameForm_KeyDown;
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.O:
                    IsWallsVisualized = !IsWallsVisualized;
                    break;
                case Keys.Escape:
                    if (IsGameStarted && !IsGamePaused)
                        PauseGame();
                    else if (IsGameStarted)
                        UnpauseGame();
                    break;
                case Keys.Space:
                    EnqueueRCircle(Center, "feetSmall2", Sounds.ClapSound);
                    break;
            }
        }

        private void SetButtons()
        {
            StartButton = new Button();
            StartButton.Text = "START!";
            StartButton.Font = new Font(StartButton.Font.Name, 16);
            StartButton.ForeColor = Color.White;
            StartButton.Size = new Size(100, 50);
            StartButton.Location = new Point((int)Center.X - StartButton.Width / 2,
                (int)Center.Y - 100);
            StartButton.TabStop = false;
            StartButton.Click += StartGame;

            ExitButton = new Button();
            ExitButton.Text = "EXIT!";
            ExitButton.Font = new Font(ExitButton.Font.Name, 16);
            ExitButton.ForeColor = Color.White;
            ExitButton.Size = new Size(100, 50);
            ExitButton.Location = new Point((int)Center.X - ExitButton.Width / 2,
                (int)Center.Y);
            ExitButton.TabStop = false;
            ExitButton.Click += ExitButton_Click;

            ContinueButton = new Button();
            ContinueButton.Text = "CONTINUE!";
            ContinueButton.Font = new Font(ContinueButton.Font.Name, 16);
            ContinueButton.ForeColor = Color.White;
            ContinueButton.Size = new Size(170, 50);
            ContinueButton.Location = new Point((int)Center.X - ContinueButton.Width / 2,
                (int)Center.Y - 100);
            ContinueButton.TabStop = false;
            ContinueButton.Click += ContinueButton_Click;

            NextLevelButton = new Button();
            NextLevelButton.Text = "NEXT LEVEL!";
            NextLevelButton.Font = new Font(NextLevelButton.Font.Name, 16);
            NextLevelButton.ForeColor = Color.White;
            NextLevelButton.Size = new Size(170, 50);
            NextLevelButton.Location = new Point((int)Center.X - NextLevelButton.Width / 2,
                (int)Center.Y - 100);
            NextLevelButton.TabStop = false;
            NextLevelButton.Click += NextLevelButton_Click;
        }

        private void SetLabels()
        {
            WinningLabel = new Label();
            WinningLabel.Text =
                "YOU ENDED THE GAME!\r\n" +
                "   CONGRATULATIONS!";
            WinningLabel.Font = new Font(WinningLabel.Font.Name, 30);
            WinningLabel.ForeColor = Color.White;
            WinningLabel.Size = new Size(600, 200);
            WinningLabel.Location = new Point((int)Center.X - WinningLabel.Width / 2 + 50,
                (int)Center.Y - 120);

            TutorialLabel = new Label();
            TutorialLabel.Text =
                " You are in dark cave! Find the exit!\r\n" +
                "                 Walk on LMB.\r\n" +
                "If you get stuck, press the spacebar\r\n" +
                "           and character will clap.";
            TutorialLabel.Font = new Font(TutorialLabel.Font.Name, 30);
            TutorialLabel.ForeColor = Color.White;
            TutorialLabel.AutoSize = true;
            TutorialLabel.Location = new Point((int)Center.X, (int)(Center.Y - 300));
        }

        private void SetTimers()
        {
            PaintTimer = new Timer();
            PaintTimer.Interval = 10;
            PaintTimer.Tick += PaintTimer_Tick;

            ClickIntervalTimer = new Timer();
            ClickIntervalTimer.Interval = 600;
            ClickIntervalTimer.Tick += ClickIntervalTimer_Tick;

            ClickTimer = new Timer();
            ClickTimer.Interval = 650;
            ClickTimer.Tick += ClickTimer_Tick;
        }

        private void StartGame(object sender, EventArgs e)
        {
            Controls.Clear();
            GamePictureBox = new PictureBox
            {
                Location = new Point(0, 0),
                Width = this.Width,
                Height = this.Height
            };
            GamePictureBox.Paint += PB_OnPaint;
            PaintTimer.Start();
            GamePictureBox.MouseDown += PB_MouseDown;
            GamePictureBox.MouseUp += PB_MouseUp;
            GamePictureBox.MouseMove += PB_MouseMove;
            Controls.Add(TutorialLabel);
            Controls.Add(GamePictureBox);
            GameModel.EnqueueNewRayCircle(Center, Center, Offset, IsFeetFlipped, out bool isEnququed);
            Sound.Play(Sounds.ClapSound);
            IsGameStarted = true;
        }

        private void PauseGame()
        {
            Controls.Clear();
            PaintTimer.Stop();
            foreach (var rayCircle in GameModel.CharacterRayCircles)
                rayCircle.DestroyTimer.Stop();
            Controls.Add(ExitButton);
            Controls.Add(ContinueButton);
            IsGamePaused = true;
        }

        private void UnpauseGame()
        {
            Controls.Clear();
            Controls.Add(GamePictureBox);
            IsGamePaused = false;
            PaintTimer.Start();
            foreach (var rayCircle in GameModel.CharacterRayCircles)
                rayCircle.DestroyTimer.Start();
        }

        private void ShowWinningScreen()
        {
            Controls.Clear();
            PaintTimer.Stop();
            Controls.Add(ExitButton);
            if (!GameModel.IsLevelsEnded)
                Controls.Add(NextLevelButton);
            else
                Controls.Add(WinningLabel);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ContinueButton_Click(object sender, EventArgs e)
        {
            UnpauseGame();
        }

        private void NextLevelButton_Click(object sender, EventArgs e)
        {
            GameModel.IsGameWon = false;
            Controls.Clear();
            Controls.Add(GamePictureBox);
            PaintTimer.Start();
            GameModel.EnqueueNewRayCircle(Center, Center, Offset, IsFeetFlipped, out bool isEnququed);
            Sound.Play(Sounds.ClapSound);
        }

        private void PB_OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (IsGameStarted)
            {
                foreach (var rayCircle in GameModel.CharacterRayCircles)
                {
                    foreach (var ray in rayCircle.Rays)
                        DrawRay(g, ray);
                    DrawFeet(g, rayCircle);
                }
                foreach (var ray in GameModel.winningScuare.Rays)
                    DrawRay(g, ray);
                if (IsWallsVisualized)
                    VisualizeWalls(g);
            }
        }

        private void DrawFeet(Graphics g, RayCircle r)
        {
            Image image;
            if (r.Feet == "feetSmall")
                image = SmallFeet;
            else
                image = SmallFeet2;

            float x = r.Position.X + Offset.X;
            float y = r.Position.Y + Offset.Y;
            if (image == SmallFeet)
            {
                g.TranslateTransform(x, y);
                g.RotateTransform(r.FeetAngle);
                g.TranslateTransform(-x, -y);
                if (r.IsFeetFlipped)
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            g.DrawImage(image,
                x - image.Width / 2,
                y - image.Height / 2,
                image.Width,
                image.Height);
            if (image == SmallFeet)
            {
                if (r.IsFeetFlipped)
                    image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                g.TranslateTransform(x, y);
                g.RotateTransform(-r.FeetAngle);
                g.TranslateTransform(-x, -y);
            }
        }

        private void DrawRay(Graphics g, Ray ray)
        {
            foreach (var rayPart in ray.RayParts)
                if (rayPart.PrevRayPart != null)
                    DrawRayPart(g, ray, rayPart.PrevRayPart.Position.PSumm(Offset), rayPart.Position.PSumm(Offset), rayPart.Opacity);
            DrawRayPart(g, ray, ray.LastRayPart.Position.PSumm(Offset), ray.Position.PSumm(Offset), ray.Opacity);
            g.FillEllipse(new SolidBrush(Color.FromArgb(ray.Opacity, ray.ObjectColor)),
                ray.Position.X - ray.Radius + Offset.X, ray.Position.Y - ray.Radius + Offset.Y, 2 * ray.Radius, 2 * ray.Radius);
        }

        private void DrawRayPart(Graphics g, Ray ray, PointF first, PointF second, int opacity)
        {
            g.DrawLine(
                new Pen(Color.FromArgb(opacity, ray.ObjectColor), ray.Radius * 2),
                first,
                second);
        }

        private void VisualizeWalls(Graphics g)
        {
            foreach (var wall in GameModel.Walls)
                g.DrawLine(new Pen(Color.Red, 2), wall.First.PSumm(Offset), wall.Last.PSumm(Offset));
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            if (!GameModel.IsGameWon)
            {
                GameModel.Refresh();
                GamePictureBox.Invalidate();
            }
            else
            {
                Offset = new PointF(0, 0);
                IsFeetFlipped = false;
                ShowWinningScreen();
            }
        }

        private void ClickIntervalTimer_Tick(object sender, EventArgs e)
        {
            IsStepped = false;
            ClickIntervalTimer.Stop();
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            EnqueueRCircle(MouseLocation);
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            MouseLocation = new PointF(e.Location.X, e.Location.Y);
            EnqueueRCircle(MouseLocation);

            ClickTimer.Start();
            IsMouseDown = true;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
                MouseLocation = new PointF(e.Location.X, e.Location.Y);
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
            ClickTimer.Stop();
        }

        private void EnqueueRCircle(PointF location, string feet = "feetSmall", Sounds sound = Sounds.StepSound)
        {
            if (!IsStepped)
            {
                Offset = GameModel.EnqueueNewRayCircle(location, Center, Offset, IsFeetFlipped, out bool isEnqueued, feet);
                if (isEnqueued)
                {
                    if (GameModel.CurLevel == 0)
                        TutorialLabel.Location = new Point((int)(Center.X + Offset.X), (int)(Center.Y - 300 + Offset.Y));
                    Sound.Play(sound);
                    IsFeetFlipped = !IsFeetFlipped;
                    ClickIntervalTimer.Start();
                    IsStepped = true;
                }
            }
        }
    }
}