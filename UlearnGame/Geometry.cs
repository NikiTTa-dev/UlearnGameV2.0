using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UlearnGame
{
    public static class Geometry
    {
        static readonly Func<double, double, double> GetDistance = (diffX, diffY) =>
            Math.Sqrt(diffX * diffX + diffY * diffY);
        static readonly Func<double, double, double, bool> IsObtuse = (double line, double a, double b) =>
            (a * a + b * b - line * line) / (2 * a * b) < 0;

        public static PointF GetCollisionVector(Wall wall, Ray ray)
        {
            var wallInclination = Math.Atan2(wall.Last.Y - wall.First.Y, wall.Last.X - wall.First.X);
            var direction = Math.Atan2(ray.MotionVector.Y, ray.MotionVector.X);
            var ansAngle = 2 * wallInclination - direction;

            return new PointF((float)(ray.Speed * Math.Cos(ansAngle)), (float)(ray.Speed * Math.Sin(ansAngle))); ;
        }

        public static double GetDistanceToSegment(Wall wall, PointF point)
        {
            double ab = GetDistance(wall.Last.X - wall.First.X, wall.Last.Y - wall.First.Y);
            double ac = GetDistance(point.X - wall.First.X, point.Y - wall.First.Y);
            double bc = GetDistance(point.X - wall.Last.X, point.Y - wall.Last.Y);
            if (IsObtuse(ac, bc, ab)) return bc;
            if (IsObtuse(bc, ac, ab)) return ac;

            double p = (ac + bc + ab) / 2;
            return 2 * Math.Sqrt(p * (p - ab) * (p - bc) * (p - ac)) / ab;
        }

        public static RayDirection GetVectorDirection(Wall wall, Ray ray)
        {
            double ans;
            if (wall.Last.Y == wall.First.Y)
                ans = (wall.First.Y - ray.Position.Y) / ray.MotionVector.Y;
            else if (wall.Last.X == wall.First.X)
                ans = (wall.First.X - ray.Position.X) / ray.MotionVector.X;
            else
                ans = ((ray.Position.Y - wall.Last.Y) / (wall.First.Y - wall.Last.Y) -
                    (ray.Position.X - wall.Last.X) / (wall.First.X - wall.Last.X)) /
                    (ray.MotionVector.X / (wall.First.X - wall.Last.X) -
                    ray.MotionVector.Y / (wall.First.Y - wall.Last.Y));

            return ans >= 0 ? RayDirection.ToWall : RayDirection.FromWall;
        }

        public static bool IsLineCrossed(Wall wall, PointF p1, PointF p2)
        {
            var denominator = (wall.Last.Y - wall.First.Y) * (p1.X - p2.X) - (wall.Last.X - wall.First.X) * (p1.Y - p2.Y);
            if (denominator == 0)
            {
                    return false;
            }
            else
            {
                var numerator_a = (wall.Last.X - p2.X) * (wall.Last.Y - wall.First.Y) - (wall.Last.X - wall.First.X) * (wall.Last.Y - p2.Y);
                var numerator_b = (p1.X - p2.X) * (wall.Last.Y - p2.Y) - (wall.Last.X - p2.X) * (p1.Y - p2.Y);
                var Ua = numerator_a / denominator;
                var Ub = numerator_b / denominator;

                if (Ua >= 0 && Ua <= 1 && Ub >= 0 && Ub <= 1)
                    return true;
                return false;
            }
        }

        public static PointF GetNewPointOfWallCrossing(Wall wall, PointF lastPos, PointF newPos)
        {
            return newPos;
        }
    }
}
