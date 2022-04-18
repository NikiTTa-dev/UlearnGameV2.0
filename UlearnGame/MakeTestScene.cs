using System.Collections.Generic;
using System.Drawing;

namespace UlearnGame
{
    public class MakeTestScene
    {
        public MakeTestScene(Game game)
        {
            game.Walls = new List<Wall>
            {
                new Wall(new PointF(0, 100), new PointF(1080, 100)),
                new Wall(new PointF(1080, 100), new PointF(1080, 300)),
                new Wall(new PointF(0, 100), new PointF(0, 500)),
                new Wall(new PointF(0, 500), new PointF(540, 720)),
                new Wall(new PointF(540, 720), new PointF(1080, 720)),
                new Wall(new PointF(1080, 720), new PointF(1080, 550)),
                new Wall(new PointF(1080, 550), new PointF(1330, 550)),
                new Wall(new PointF(1330, 550), new PointF(1330, 300)),
                new Wall(new PointF(1330, 300), new PointF(1080, 300)),

            };
            game.winningScuare = new WinningScuare(new PointF(1080, 300));
        }
    }
}
