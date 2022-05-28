using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace UlearnGame
{
    [TestFixture]
    public class SomeTests
    {
        Wall[] Walls;
        PointF[] Points;
        Ray[] rays;

        [SetUp]
        public void SetUp()
        {
            Walls = new Wall[] {
                new Wall (new PointF(0, 100), new PointF(100, 0))
            };
            rays = new Ray[] {
                new Ray(5, new PointF(1, 1), new PointF(5, 5), Color.White),
                new Ray(5, new PointF(6, 6), new PointF(-5, -5), Color.White),
                new Ray(5, new PointF(200, 200), new PointF(10,1), Color.White)
            };
            Points = new PointF[] {
                new PointF(0, 0),
                new PointF(300, 300),
                new PointF(20, 20),
                new PointF(0, 99),
                new PointF(99, 0)
            };
        }

        [Test]
        public void GetCollisionVectorTests()
        {
            var testRes = Geometry.GetCollisionVector(Walls[0], rays[0]);
            Assert.AreEqual(-3.535534, testRes.X, 10e-5);
            Assert.AreEqual(-3.535534, testRes.Y, 10e-5);
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        [TestCase(true, 2)]
        public void GetVectorDirectionTests(bool testExpected, int rayIndex)
        {
            Assert.AreEqual(testExpected, Geometry.GetVectorDirection(Walls[0], rays[rayIndex]) == RayDirection.FromWall);
        }

        [TestCase(true, 0)]
        [TestCase(false, 1)]
        [TestCase(false, 2)]
        public void GetDistanceTests(bool testExpected, int rayIndex)
        {
            var d1 = Geometry.GetDistanceToSegment(Walls[0], rays[rayIndex].Position);
            var d2 = Geometry.GetDistanceToSegment(Walls[0], rays[rayIndex].Position.PSumm(rays[rayIndex].MotionVector));
            Assert.AreEqual(testExpected, d1 > d2);
        }

        [TestCase(true, 0, 1)]
        [TestCase(false, 0, 2)]
        [TestCase(false, 3, 4)]
        public void LineCrossTest(bool testExpected, int firstPIndex, int secondePIndex)
        {
            Assert.AreEqual(testExpected, Geometry.IsLineCrossed(Walls[0], Points[firstPIndex], Points[secondePIndex]));
        }
    }
}
