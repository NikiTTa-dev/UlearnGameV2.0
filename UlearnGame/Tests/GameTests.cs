using System;
using NUnit.Framework;
using System.Drawing;

namespace UlearnGame.Tests
{
    [TestFixture]
    public class GameTests
    {
        Game gameModel;
        bool IsEnqueued;

        [SetUp]
        public void SetUp()
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("UlearnGame.exe", "") + "Resources/Levels.json";
            gameModel = new Game(path);
            gameModel.EnqueueNewRayCircle(new PointF(0, 0), new PointF(1, 1),
                new PointF(0, 0), true, out IsEnqueued);
            IsEnqueued = false;
            gameModel.winningScuare = new WinningScuare(new PointF(10, 10));
        }

        [Test]
        public void GameWinTest()
        {
            SetUp();
            gameModel.EnqueueNewRayCircle(new PointF(20, 20), new PointF(1, 1),
                new PointF(0, 0), true, out IsEnqueued);
            Assert.IsTrue(gameModel.IsGameWon);
        }

        [Test]
        public void GameNotWinTest()
        {
            SetUp();
            gameModel.EnqueueNewRayCircle(new PointF(-2, -2), new PointF(1, 1),
                new PointF(0, 0), true, out IsEnqueued);
            Assert.IsFalse(gameModel.IsGameWon);
        }

        [Test]
        public void GameStatusChangesOnWinTest()
        {
            SetUp();
            var nextLevel = gameModel.CurLevel + 1;
            gameModel.EnqueueNewRayCircle(new PointF(20, 21), new PointF(1, 1),
                new PointF(0, 0), true, out IsEnqueued);
            Assert.IsTrue(IsEnqueued);
            Assert.IsTrue(gameModel.IsGameWon);
            Assert.IsTrue(gameModel.LastCirclePosition == PointF.Empty);
            if (!gameModel.IsLevelsEnded)
                Assert.IsTrue(gameModel.CurLevel == nextLevel);
            else
                Assert.IsTrue(gameModel.CurLevel == 0);
        }
    }
}
