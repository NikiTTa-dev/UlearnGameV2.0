using System;
using System.Collections.Generic;
using System.Drawing;


namespace UlearnGame
{
    public class WinningScuare : IGameObject
    {
        public PointF Position { get; set; }
        public Color ObjectColor { get; set; }
        public int Speed { get; set; }
        public List<Ray> Rays { get; set; }
        private int RaysCount { get; set; } = 10;
        private Size SquareSize { get; set; }
        public List<Wall> Walls { get; set; }  

        public WinningScuare(PointF position)
        {
            SquareSize = new Size(250, 250);
            Speed = 5;
            Position = position;
            Walls = new List<Wall>
            {
                new Wall(position, position.PSumm(new PointF(SquareSize.Width, 0))),
                new Wall(position.PSumm(new PointF(SquareSize.Width, 0)), position.PSumm(new PointF(SquareSize.Width, SquareSize.Height))),
                new Wall(position.PSumm(new PointF(SquareSize.Width, SquareSize.Height)), position.PSumm(new PointF(0, SquareSize.Height))),
                new Wall(position.PSumm(new PointF(0, SquareSize.Height)), position)
            };
            ObjectColor = Color.White;
            InitRays();
        }

        private void InitRays()
        {
            Random random = new Random();
            Rays = new List<Ray>();
            for (int i = 0; i < RaysCount; i++)
            {
                double angle = random.Next(-180, 180) * Math.PI / 180;
                PointF motionV = new PointF((float)(Speed * Math.Cos(angle)), (float)(Speed * Math.Sin(angle)));
                var newRay = new Ray(Speed, Position.PSumm(new PointF(SquareSize.Width / 2, SquareSize.Height / 2)), motionV, ObjectColor);
                newRay.Opacity = (int)(newRay.Opacity * 0.5);
                Rays.Add(newRay);
            }
        }
    }
}
