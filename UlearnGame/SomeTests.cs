using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Drawing;
using System.Diagnostics;

namespace UlearnGame
{
    [TestFixture]
    public class SomeTests
    {
        Wall wall;
        Ray ray;
        Ray ray2;
        PointF p1;
        PointF p2;
        PointF p3;
        PointF p4;
        PointF p5;

        public SomeTests()
        {
            wall = new Wall(new PointF(0, 100), new PointF(100, 0));
            ray = new Ray(5, new PointF(1, 1), new PointF(5, 5), Color.White);
            ray2 = new Ray(5, new PointF(6, 6), new PointF(-5, -5), Color.White);
            p1 = new PointF(0, 0);
            p2 = new PointF(300, 300);
            p3 = new PointF(20, 20);
            p4 = new PointF(0, 99);
            p5 = new PointF(99, 0);
        }

        [Test]
        public void First()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var d1 = Geometry.GetDistanceToSegment(wall, ray.Position);
                var d2 = Geometry.GetDistanceToSegment(wall, ray.Position.PSumm(ray.MotionVector));
                Assert.IsTrue(d2 < d1);
                d1 = Geometry.GetDistanceToSegment(wall, ray2.Position);
                d2 = Geometry.GetDistanceToSegment(wall, ray2.Position.PSumm(ray2.MotionVector));
                Assert.IsTrue(d2 > d1);
            }

            sw.Stop();
            var a = sw.ElapsedMilliseconds;
        }

        [Test]
        public void Second()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var yo = ray.Position.Y;
                var xo = ray.Position.X;
                var yA = wall.First.Y;
                var xA = wall.First.X;
                var yB = wall.Last.Y;
                var xB = wall.Last.X;
                var xa = ray.MotionVector.X;
                var ya = ray.MotionVector.Y;

                var L = ((yo - yA) / (yB - yA) - (xo - xA) / (xB - xA)) / ((xa) / (xB - xA) - (ya) / (yB - yA));

                Assert.IsTrue(L > 0);

                yo = ray2.Position.Y;
                xo = ray2.Position.X;
                xa = ray2.MotionVector.X;
                ya = ray2.MotionVector.Y;

                L = ((yo - yA) / (yB - yA) - (xo - xA) / (xB - xA)) / ((xa) / (xB - xA) - (ya) / (yB - yA));

                Assert.IsTrue(L < 0);
            }

            sw.Stop();
            var a = sw.ElapsedMilliseconds;
        }

        [Test]
        public void LineCrossTest()
        {
            Assert.IsTrue(Geometry.IsLineCrossed(wall, p1, p2));
            Assert.IsFalse(Geometry.IsLineCrossed(wall, p1, p3));
            Assert.IsFalse(Geometry.IsLineCrossed(wall, p4, p5));
        }
    }
}
