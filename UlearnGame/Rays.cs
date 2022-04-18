using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace UlearnGame
{
    public enum RayDirection
    {
        ToWall,
        FromWall
    }

    public class RayPart
    {
        public int Opacity { get; set; }
        public RayPart PrevRayPart { get; set; }
        public PointF Position { get; set; }
        public RayPart(RayPart prevPart, PointF position, int opacity)
        {
            Opacity = opacity;
            PrevRayPart = prevPart;
            Position = position;
        }
    }

    public class Ray : IGameObject
    {
        public RayPart LastRayPart {get; set; }
        public Queue<RayPart> RayParts { get; set; }
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
            RayParts = new Queue<RayPart>();
            LastRayPart = new RayPart(null, pos, Opacity);
            RayParts.Enqueue(LastRayPart);
            
        }

        public void LengthenRay()
        {
            Position = Position.PSumm(MotionVector);
        }

        public void RefreshRay(List<Wall> walls)
        {
            foreach (var wall in walls)
                if (Geometry.GetDistanceToSegment(wall, Position) <= Radius &&
                    Geometry.GetVectorDirection(wall, this) == RayDirection.ToWall)
                {
                    MotionVector = Geometry.GetCollisionVector(wall, this);
                    LastRayPart = new RayPart(LastRayPart, Position, Opacity);
                    RayParts.Enqueue(LastRayPart);
                }
            LengthenRay();
        }

        public void RefreshWinningRays(List<Wall> walls)
        {
            RefreshRay(walls);
            if (RayParts.Count > 5)
            {
                RayParts.Dequeue();
                foreach (var rayPart in RayParts)
                    rayPart.Opacity = (int)(rayPart.Opacity * 0.7);
            }
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
