using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

//TODO:
//Tutorial
//RedSquare

namespace UlearnGame
{
    public class Game
    {
        private int CurLevel = 0;
        private List<Level> Levels;
        public Queue<RayCircle> CharacterRayCircles { get; set; }
        public WinningScuare winningScuare { get; set; }
        public List<Wall> Walls { get; set; }
        public PointF MouseLocation { get; set; }
        public bool IsGameWon { get; set; }
        public bool IsLevelsEnded { get; set; }
        public int StepLength { get; set; } = 130;
        public PointF LastCirclePosition { get; set; }
        private int closeToWallDistance { get; set; } = 30;

        public Game()
        {
            Levels = JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Levels.json"));
            Walls = Levels[CurLevel].Walls;
            winningScuare = new WinningScuare(Levels[CurLevel].WinningSquarePositon);
            CharacterRayCircles = new Queue<RayCircle>();
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

            foreach (var wall in winningScuare.Walls)
            {
                if (Geometry.IsLineCrossed(wall, LastCirclePosition, position))
                {
                    WinLevel();
                }
            }

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

        private void WinLevel()
        {
            IsGameWon = true;
            LastCirclePosition = PointF.Empty;
            CurLevel++;
            if (CurLevel == Levels.Count)
            {
                CurLevel = 0;
                IsLevelsEnded = true;
            } 
            Walls = Levels[CurLevel].Walls;
            winningScuare = new WinningScuare(Levels[CurLevel].WinningSquarePositon);
            foreach (var rayCircle in CharacterRayCircles)
                rayCircle.DestroyTimer.Stop();
            CharacterRayCircles.Clear();
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
                ray.RefreshWinningRays(winningScuare.Walls);
            }
        }
    }
}
