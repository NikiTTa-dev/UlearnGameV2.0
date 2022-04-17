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

    class RayPart
    {
        public RayPart PrevRayPart { get; set; }
        public PointF Position { get; set; }
        public RayPart(RayPart prevPart)
        {
            PrevRayPart = prevPart;
        }
    }

    public class Ray : IGameObject
    {
        public List<PointF> RayParts { get; set; }
        public int Radius { get; set; } = 5;
        public PointF Position { get; set; }
        public PointF MotionVector { get; set; }
        public Color ObjectColor { get; set; }
        public int Opacity { get; set; } = 255;
        public int Speed { get; set; }

        public Ray(int speed, PointF pos, PointF motionV, Color color)
        {
            Speed = speed;
            Position = pos;
            MotionVector = motionV;
            ObjectColor = color;
            RayParts = new List<PointF> { pos, pos };
        }

        public void LengthenRay()
        {
            var newPos = new PointF(Position.X + MotionVector.X, Position.Y + MotionVector.Y);
            Position = newPos;
            RayParts[RayParts.Count - 1] = newPos;
        }

        public void RefreshRay(List<Wall> Walls)
        {
            foreach (var wall in Walls)
                if (Geometry.GetDistanceToSegment(wall, Position) <= Radius &&
                    Geometry.GetVectorDirection(wall, this) == RayDirection.ToWall)
                {
                    MotionVector = Geometry.GetCollisionVector(wall, this);
                    RayParts.Add(Position);
                }
            LengthenRay();
        }
    }

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

        public RayCircle(PointF position, Game game, float feetAngle, bool isFeetFlipped)
        {
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
