using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace UlearnGame
{
    public class Game
    {
        public int CurLevel { get; set; } = 0;
        private List<Level> Levels;
        public Queue<RayCircle> CharacterRayCircles { get; set; }
        public WinningScuare winningScuare { get; set; }
        public List<Wall> Walls { get; set; }
        public bool IsGameWon { get; set; }
        public bool IsLevelsEnded { get; set; }
        public int StepLength { get; set; } = 130;
        public PointF LastCirclePosition { get; set; }
        private int CloseToWallDistance { get; set; } = 30;

        public Game()
        {
            Levels = JsonConvert.DeserializeObject<List<Level>>(File.ReadAllText("Resources/Levels.json"));
            Walls = Levels[CurLevel].Walls;
            winningScuare = new WinningScuare(Levels[CurLevel].WinningSquarePositon);
            CharacterRayCircles = new Queue<RayCircle>();
        }

        public PointF EnqueueNewRayCircle(PointF location, PointF center,
            PointF offset, bool feetFlip, out bool isEnqueued, string feet = "feetSmall")
        {
            isEnqueued = true;
            if (LastCirclePosition.IsEmpty)
            {
                LastCirclePosition = center;
                CharacterRayCircles.Enqueue(new RayCircle(center, this, 0, feetFlip, "feetSmall2"));
                return offset;
            }

            double length = Math.Sqrt((location.X - center.X) * (location.X - center.X) +
                (location.Y - center.Y) * (location.Y - center.Y));
            double angle = Math.Atan2(location.Y - center.Y, location.X - center.X);
            PointF diff;
            if (length >= StepLength)
                diff = new PointF((float)(StepLength * Math.Cos(angle)),
                    (float)(StepLength * Math.Sin(angle)));
            else
                diff = location.PDiff(center);

            var position = LastCirclePosition.PSumm(diff);
            foreach (var wall in winningScuare.Walls)
                if (Geometry.IsLineCrossed(wall, LastCirclePosition, position))
                    WinLevel();
                
            foreach (var wall in Walls)
                if (Geometry.IsLineCrossed(wall, LastCirclePosition, position) ||
                    Geometry.GetDistanceToSegment(wall, position) < CloseToWallDistance)
                {
                    isEnqueued = false;
                    return offset;
                }
                    

            var resOff = offset.PDiff(diff);
            var angleInDegrees = (float)(angle / Math.PI * 180) + 90;
            var newRayCircle = new RayCircle(position, this, angleInDegrees, feetFlip, feet);
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
                    foreach (var rayPart in ray.RayParts)
                        rayPart.Opacity = (int)(rayPart.Opacity * 0.985);
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
