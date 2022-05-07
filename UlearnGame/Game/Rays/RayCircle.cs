using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UlearnGame
{
    public class RayCircle : IGameObject
    {
        public readonly Timer DestroyTimer;
        public Color ObjectColor { get; set; } = Color.White;
        public int Speed { get; set; } = 5;
        public int Radius { get; set; } = 20;
        public int RaysCount { get; set; } = 24;
        public PointF Position { get; set; }
        public List<Ray> Rays { get; set; }
        public string Feet { get; set; } = "feetSmall";
        public float FeetAngle { get; set; }
        public bool IsFeetFlipped { get; }

        public RayCircle(PointF position, Game game, float feetAngle, bool isFeetFlipped, string feet)
        {
            Feet = feet;
            IsFeetFlipped = isFeetFlipped;
            FeetAngle = feetAngle;
            Position = position;
            Rays = new List<Ray>();
            FillRaysList();

            DestroyTimer = new Timer();
            DestroyTimer.Interval = 2000;
            DestroyTimer.Tick += (sender, e) =>
            {
                game.CharacterRayCircles.Dequeue();
                DestroyTimer.Stop();
            };
            DestroyTimer.Start();
        }

        private void FillRaysList()
        {
            for (int i = 0; i < RaysCount; i++)
            {
                double angle = i * Math.PI * 2 / RaysCount;
                PointF position = new PointF((float)(Position.X + Radius * Math.Cos(angle)),
                    (float)(Position.Y + Radius * Math.Sin(angle)));
                PointF motionV = new PointF((float)(Speed * Math.Cos(angle)), (float)(Speed * Math.Sin(angle)));
                Rays.Add(new Ray(Speed, position, motionV, ObjectColor));
            }
        }
    }
}
