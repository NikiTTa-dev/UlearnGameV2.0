using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace UlearnGame
{
    public enum RayDirection
    {
        ToWall,
        FromWall
    }

    public class Ray : IGameObject
    {
        public List<PointF> RayParts { get; set; }
        public int Radius { get; set; } = 5;
        public PointF Position { get; set; }
        public PointF MotionVector { get; set; }
        public Color Color { get; set; }
        public int Opacity { get; set; } = 255;
        public int Speed { get; set; }

        public Ray(int speed, PointF pos, PointF motionV, Color color)
        {
            Speed = speed;
            Position = pos;
            MotionVector = motionV;
            Color = color;
            RayParts = new List<PointF>();
            RayParts.Add(pos);
            RayParts.Add(pos);
        }

        public void LengthenRay()
        {
            var newPos = new PointF(Position.X + MotionVector.X, Position.Y + MotionVector.Y);
            Position = newPos;
            RayParts[RayParts.Count - 1] = newPos;
        }
    }

    public class RayCircle : IGameObject
    {
        private readonly Timer timer;
        public Color Color { get; set; } = Color.White;
        public int Speed { get; set; } = 5;
        public int Radius { get; set; } = 20;
        public int RaysCount { get; set; } = 24;
        public PointF Position { get; set; }
        public List<Ray> Rays { get; set; }
        public string Feet { get; set; } = "feetSmall";
        public float FeetAngle { get; set; }
        public bool IsFeetFlipped { get; }

        public RayCircle(PointF position, Game game, float feetAngle, bool isFeetFlipped)
        {
            IsFeetFlipped = isFeetFlipped;
            FeetAngle = feetAngle;
            Position = position;
            Rays = new List<Ray>();
            FillRaysList();

            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (sender, e) =>
            {
                game.CharacterRayCircles.Dequeue();
                timer.Stop();
            };
            timer.Start();
        }

        private void FillRaysList()
        {
            for (int i = 0; i < RaysCount; i++)
            {
                double angle = i * Math.PI * 2 / RaysCount;
                PointF position = new PointF((float)(Position.X + Radius * Math.Cos(angle)),
                    (float)(Position.Y + Radius * Math.Sin(angle)));
                PointF motionV = new PointF((float)(Speed * Math.Cos(angle)), (float)(Speed * Math.Sin(angle)));
                Rays.Add(new Ray(Speed, position, motionV, Color));
            }
        }
    }
}
