using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace UlearnGame
{
    public class Wall
    {
        public PointF First { get; set; }
        public PointF Last { get; set; }

        public Wall(PointF first, PointF last)
        {
            First = first;
            Last = last;
        }
    }
}
