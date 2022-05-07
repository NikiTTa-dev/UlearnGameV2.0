using System.Drawing;
using System.Collections.Generic;

namespace UlearnGame
{
    public class Ray : IGameObject
    {
        public RayPart LastRayPart { get; set; }
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
}
