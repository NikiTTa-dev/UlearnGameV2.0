using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

//TODO:
//stop in front of wall
//SceneManagement
//Tests
//Tutorial
//Menu
//Pause

namespace UlearnGame
{
    public class Game
    {
        public Queue<RayCircle> CharacterRayCircles { get; set; }
        public List<Wall> Walls { get; set; }
        public PointF MouseLocation { get; set; }
        public bool IsMouseDown { get; set; }
        public int StepLength { get; set; } = 125;
        public PointF LastCirclePosition { get; set; }

        public Game()
        {
            new MakeTestScene(this);
            CharacterRayCircles = new Queue<RayCircle>();
            MouseLocation = new PointF();
        }

        public PointF EnqueueNewRayCircle(PointF center, PointF offset, bool feetFlip)
        {
            if (LastCirclePosition.IsEmpty)
            {
                LastCirclePosition = center;
                CharacterRayCircles.Enqueue(new RayCircle(center, this, 0, feetFlip) { Feet = "feetSmall2" });
                return offset;
            }

            double length = Math.Sqrt((MouseLocation.X - center.X) * (MouseLocation.X - center.X) +
                (MouseLocation.Y - center.Y) * (MouseLocation.Y - center.Y));
            double angle = Math.Atan2(MouseLocation.Y - center.Y, MouseLocation.X - center.X);
            PointF diff;
            if (length >= StepLength)
            {
                diff = new PointF((float)(StepLength * Math.Cos(angle)),
                    (float)(StepLength * Math.Sin(angle)));
            }
            else
            {
                diff = MouseLocation.PDiff(center);
            }

            var position = LastCirclePosition.PSumm(diff);
            foreach (var wall in Walls)
                if (Geometry.IsLineCrossed(wall, LastCirclePosition, position) || Geometry.GetDistanceToSegment(wall, position) < 20)
                {
                    return offset;
                }

                    

            var resOff = offset.PDiff(diff);
            var angleInDegrees = (float)(angle / Math.PI * 180) + 90;
            var newRayCircle = new RayCircle(position, this, angleInDegrees, feetFlip);
            LastCirclePosition = newRayCircle.Position;
            CharacterRayCircles.Enqueue(newRayCircle);
            return resOff;
        }

        public void Refresh()
        {
            foreach (var rayCircle in CharacterRayCircles)
            {
                foreach (var ray in rayCircle.Rays)
                {
                    foreach (var wall in Walls)
                        if (Geometry.GetDistanceToSegment(wall, ray.Position) <= ray.Radius &&
                            Geometry.GetVectorDirection(wall, ray) == RayDirection.ToWall)
                        {
                            ray.MotionVector = Geometry.GetCollisionVector(wall, ray);
                            ray.RayParts.Add(new PointF(ray.Position.X, ray.Position.Y));
                        }
                    ray.LengthenRay();
                    ray.Opacity = (int)(ray.Opacity * 0.975);
                }
            }
        }
    }
}
