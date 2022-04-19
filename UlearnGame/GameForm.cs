using System;
using System.Drawing;
using System.Windows.Forms;

namespace UlearnGame
{
    public partial class GameForm : Form
    {
        PointF Center { get; set; }
        PointF Offset { get; set; }
        Game game { get; }
        PictureBox PictureBox { get; set; }
        Label WinningTextBox { get; set; }
        Button StartButton { get; set; }
        Button ContinueButton { get; set; }
        Button NextLevelButton { get; set; }
        Button ExitButton { get; set; }
        Timer PaintTimer { get; set; }
        Timer ClickTimer { get; set; }
        Timer ClickIntervalTimer { get; set; }
        Bitmap SmallFeet { get; } = Resources.feetsmall;
        Bitmap SmallFeet2 { get; } = Resources.feetsmall2;
        bool IsFeetFlipped { get; set; }
        bool IsMouseDown { get; set; }
        bool IsStepped { get; set; }
        bool IsGameStarted { get; set; }
        bool IsGamePaused { get; set; }
        bool IsWallsVisualized { get; set; } = true;

        public GameForm(Game game)
        {
            BackColor = Color.Black;
            Width = 1600;
            Height = 900;
            this.game = game;
            StartPosition = FormStartPosition.CenterScreen;
            Center = new PointF(Width / 2, Height / 2);
            Offset = new PointF(0, 0);

            SetTimers();
            SetTextBoxes();
            Setbuttons();

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
            }
        }

        private void Setbuttons()
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

            Controls.Add(StartButton);

            ExitButton = new Button();
            ExitButton.Text = "EXIT!";
            ExitButton.Font = new Font(ExitButton.Font.Name, 16);
            ExitButton.ForeColor = Color.White;
            ExitButton.Size = new Size(100, 50);
            ExitButton.Location = new Point((int)Center.X - ExitButton.Width / 2,
                (int)Center.Y);
            ExitButton.TabStop = false;
            ExitButton.Click += ExitButton_Click;

            Controls.Add(ExitButton);

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

        private void SetTextBoxes()
        {
            WinningTextBox = new Label();
            WinningTextBox.Text = "YOU ENDED THE GAME!\r\nCONGRATULATIONS!";
            WinningTextBox.Font = new Font(WinningTextBox.Font.Name, 30);
            WinningTextBox.ForeColor = Color.White;
            WinningTextBox.Size = new Size(600, 200);
            WinningTextBox.Location = new Point((int)Center.X - WinningTextBox.Width / 2,
                (int)Center.Y - 200);
            WinningTextBox.TabStop = false;
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
            PictureBox = new PictureBox
            {
                Location = new Point(0, 0),
                Width = this.Width,
                Height = this.Height
            };
            PictureBox.Paint += PB_OnPaint;
            PaintTimer.Start();
            PictureBox.MouseDown += PB_MouseDown;
            PictureBox.MouseUp += PB_MouseUp;
            PictureBox.MouseMove += PB_MouseMove;
            Controls.Add(PictureBox);
            game.EnqueueNewRayCircle(Center, Offset, IsFeetFlipped);
            IsGameStarted = true;
        }

        private void UnpauseGame()
        {
            Controls.Clear();
            Controls.Add(PictureBox);
            IsGamePaused = false;
            PaintTimer.Start();
            foreach (var rayCircle in game.CharacterRayCircles)
                rayCircle.DestroyTimer.Start();
        }

        private void PauseGame()
        {
            Controls.Clear();
            PaintTimer.Stop();
            foreach (var rayCircle in game.CharacterRayCircles)
                rayCircle.DestroyTimer.Stop();
            Controls.Add(ExitButton);
            Controls.Add(ContinueButton);
            IsGamePaused = true;
        }

        private void ShowWinningScreen()
        {
            Controls.Clear();
            PaintTimer.Stop();
            Controls.Add(ExitButton);
            if (!game.IsLevelsEnded)
                Controls.Add(NextLevelButton);
            else
            {
                Controls.Add(WinningTextBox);
            }
            
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
            game.IsGameWon = false;
            Controls.Clear();
            Controls.Add(PictureBox);
            PaintTimer.Start();
            game.EnqueueNewRayCircle(Center, Offset, IsFeetFlipped);
        }

        private void PB_OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (IsGameStarted)
            {
                foreach (var rayCircle in game.CharacterRayCircles)
                {
                    foreach (var ray in rayCircle.Rays)
                        DrawRay(g, ray);
                    DrawFeet(g, rayCircle);
                }
                foreach (var ray in game.winningScuare.Rays)
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
            g.TranslateTransform(x, y);
            g.RotateTransform(r.FeetAngle);
            g.TranslateTransform(-x, -y);
            if (r.IsFeetFlipped)
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.DrawImage(image,
                x - image.Width / 2,
                y - image.Height / 2,
                image.Width,
                image.Height);
            if (r.IsFeetFlipped)
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.TranslateTransform(x, y);
            g.RotateTransform(-r.FeetAngle);
            g.TranslateTransform(-x, -y);
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
            foreach (var wall in game.Walls)
                g.DrawLine(new Pen(Color.Red, 2), wall.First.PSumm(Offset), wall.Last.PSumm(Offset));
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            if (!game.IsGameWon)
            {
                game.Refresh();
                PictureBox.Invalidate();
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
            EnqueueRCircle();
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            game.MouseLocation = new PointF(e.Location.X, e.Location.Y);
            EnqueueRCircle();

            ClickTimer.Start();
            IsMouseDown = true;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
                game.MouseLocation = new PointF(e.Location.X, e.Location.Y);
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
            ClickTimer.Stop();
        }

        private void EnqueueRCircle()
        {
            if (!IsStepped)
            {
                Offset = game.EnqueueNewRayCircle(Center, Offset, IsFeetFlipped);
                IsFeetFlipped = !IsFeetFlipped;
                ClickIntervalTimer.Start();
                IsStepped = true;
            }
        }
    }
}