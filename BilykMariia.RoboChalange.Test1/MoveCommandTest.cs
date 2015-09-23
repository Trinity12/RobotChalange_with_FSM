using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;
using BilykMariia.RoboChalange;

namespace BilykMariia.RoboChalange.Test1
{
    [TestClass]
    public class MoveCommandTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p1 = new Position(0, 0);
            var p2 = new Position(1, 4);

            Assert.AreEqual(17, DistanceHelper.FindDistance(p1, p2));
        }
    }
}
