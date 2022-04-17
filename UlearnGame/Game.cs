using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

//TODO:
//SceneManagement
//Tests
//Tutorial

namespace UlearnGame
{
    public class Game
    {
        public Queue<RayCircle> CharacterRayCircles { get; set; }
        public WinningScuare winningScuare { get; set; }
        public List<Wall> Walls { get; set; }
        public PointF MouseLocation { get; set; }
        public bool IsMouseDown { get; set; }
        public int StepLength { get; set; } = 130;
        public PointF LastCirclePosition { get; set; }
        private int closeToWallDistance { get; set; } = 30;

        public Game()
        {
            new MakeTestScene(this);
            CharacterRayCircles = new Queue<RayCircle>();
            winningScuare = new WinningScuare(new PointF(300, 400));
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
                diff = new PointF((float)(StepLength * Math.Cos(angle)),
                    (float)(StepLength * Math.Sin(angle)));
            else
                diff = MouseLocation.PDiff(center);

            var position = LastCirclePosition.PSumm(diff);
            foreach (var wall in Walls)
                if (Geometry.IsLineCrossed(wall, LastCirclePosition, position) ||
                    Geometry.GetDistanceToSegment(wall, position) < closeToWallDistance)
                    return offset;

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
                    ray.RefreshRay(Walls);
                    ray.Opacity = (int)(ray.Opacity * 0.985);
                }
            }
            foreach (var ray in winningScuare.Rays)
            {
                ray.RefreshRay(winningScuare.Walls);
                ray.Opacity = (int)(ray.Opacity);
            }
        }
    }
}
