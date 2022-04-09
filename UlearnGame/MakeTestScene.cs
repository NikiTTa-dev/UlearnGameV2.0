using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                new Wall(new PointF(100, 0), new PointF(100, 720)),
                new Wall(new PointF(980, 0), new PointF(980, 260)),
                new Wall(new PointF(980, 460), new PointF(980, 720)),
                new Wall(new PointF(0, 620), new PointF(1080, 620)),
                new Wall(new PointF(0, 360), new PointF(540, 720))

            };
        }
    }
}
