using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UlearnGame
{
    interface IGameObject
    {
        PointF Position { get; set; }
        Color Color { get; set; }
        int Speed { get; set; }
    }
}
