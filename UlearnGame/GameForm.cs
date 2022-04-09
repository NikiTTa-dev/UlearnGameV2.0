using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UlearnGame
{
    public partial class GameForm : Form
    {
        public PointF Center { get; set; }
        public PointF Offset { get; set; }
        Game game { get; }
        PictureBox PictureBox { get; }
        Timer PaintTimer { get; }
        Timer ClickTimer { get; set; }
        Timer ClickIntervalTimer { get; set; }
        Bitmap SmallFeet { get; } = Resources.feetsmall;
        Bitmap SmallFeet2 { get; } = Resources.feetsmall2;
        bool IsFeetFlipped { get; set; }
        bool IsMouseDown { get; set; }
        bool IsStepped { get; set; }


        public GameForm(Game game)
        {
            BackColor = Color.Black;
            Width = 1080;
            Height = 720;
            this.game = game;
            StartPosition = FormStartPosition.CenterScreen;
            Center = new PointF(Width / 2, Height / 2);
            Offset = new PointF(0, 0);

            PaintTimer = new Timer();
            PaintTimer.Interval = 20;
            PaintTimer.Tick += PaintTimer_Tick;
            PaintTimer.Start();

            ClickIntervalTimer = new Timer();
            ClickIntervalTimer.Interval = 600;
            ClickIntervalTimer.Tick += ClickIntervalTimer_Tick;

            ClickTimer = new Timer();
            ClickTimer.Interval = 650;
            ClickTimer.Tick += ClickTimer_Tick;

            PictureBox = new PictureBox
            {
                Location = new Point(0, 0),
                Width = this.Width,
                Height = this.Height
            };
            PictureBox.Paint += PB_OnPaint;
            PictureBox.MouseDown += PB_MouseDown;
            PictureBox.MouseUp += PB_MouseUp;
            PictureBox.MouseMove += PB_MouseMove;
            Controls.Add(PictureBox);
            
            Offset = game.EnqueueNewRayCircle(Center, Offset, IsFeetFlipped);
        }

        private void PB_OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (var rayCircle in game.CharacterRayCircles)
            {
                DrawRays(g, rayCircle);
                DrawFeet(g, rayCircle);
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

        private void DrawRays(Graphics g, RayCircle rayCircle)
        {
            foreach (var ray in rayCircle.Rays)
            {
                for (int i = 1; i < ray.RayParts.Count; i++)
                    g.DrawLine(new Pen(Color.FromArgb(ray.Opacity, ray.Color), ray.Radius * 2),
                        ray.RayParts[i - 1].PSumm(Offset), ray.RayParts[i].PSumm(Offset));
                g.FillEllipse(new SolidBrush(Color.FromArgb(ray.Opacity, ray.Color)),
                    ray.Position.X - ray.Radius + Offset.X, ray.Position.Y - ray.Radius + Offset.Y, 2 * ray.Radius, 2 * ray.Radius);
            }
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            game.Refresh();
            PictureBox.Invalidate();
        }

        private void ClickIntervalTimer_Tick(object sender, EventArgs e)
        {
            IsStepped = false;
            ClickIntervalTimer.Stop();
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            EnqueRCircle();
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
            ClickTimer.Stop();
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            game.MouseLocation = new PointF(e.Location.X, e.Location.Y);
            EnqueRCircle();

            ClickTimer.Start();
            IsMouseDown = true;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
                game.MouseLocation = new PointF(e.Location.X, e.Location.Y);
        }

        private void EnqueRCircle()
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
