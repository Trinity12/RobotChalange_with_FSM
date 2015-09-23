using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BilykMariia.RoboChalange;
using Robot.Common;
using System.Collections.Generic;

namespace BilykMariia.RoboChalange.Test1
{
    [TestClass]
    public class MoveCommandTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var algorithm = new BilykMariiaAngoritm();
            var map = new Map();
            var stationPositon = new Position(1, 1);
            map.Stations.Add(new EnergyStation()
            {
                Energy = 1000,
                Position = stationPositon,
                RecoveryRate = 2
            });
            var robots = new List<Robot.Common.Robot>(){new Robot.Common.Robot() {Energy = 200, Position = new Position(2,3)}};

            var command = algorithm.DoStep(robots, 0, map);

            Assert.IsTrue(command is MoveCommand);
            Assert.AreEqual(((MoveCommand)command).NewPosition, stationPositon);
        }

    
    }
}
