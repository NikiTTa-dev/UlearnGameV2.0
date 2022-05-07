using System.Drawing;

namespace UlearnGame
{
    public class RayPart : IGameObject
    {
        public int Opacity { get; set; }
        public RayPart PrevRayPart { get; set; }
        public PointF Position { get; set; }
        public Color ObjectColor { get; set; }
        public int Speed { get; set; }

        public RayPart(RayPart prevPart, PointF position, int opacity)
        {
            Opacity = opacity;
            PrevRayPart = prevPart;
            Position = position;
        }
    }
}
