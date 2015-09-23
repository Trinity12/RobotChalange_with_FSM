using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robot.Common;

namespace BilykMariia.RoboChalange
{
    public class BilykMariiaAngoritm : IRobotAlgorithm
    {
        public string Author
        {
            get { return "Mariia Bilyk"; }
        }
        private int roundCounter;

        public BilykMariiaAngoritm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
            StrategyState.ActiveState = StrategyState.States.Borning;
        }

        void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            if (++roundCounter == 35 || roundCounter == 40)
                StrategyState.NextState();
            //else if (roundCounter == 35)
            //    StrategyState.NextState(true);
                // підрахунок кількості раундів
            //if (++roundCounter%6 == 0)
            //    StrategyState.NextState();  // підрахунок кількості раундів

        }
        public string Description
        {
            get { return "d"; }
        }

        private RobotCommand Borning(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            int MinEnergyToCreateNewRobot = 200 + 100 + 40;
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            if ((movingRobot.Energy > MinEnergyToCreateNewRobot) && (robots.Count < map.Stations.Count))
            {
                return new CreateNewRobotCommand();
            }

            if (AmIOnStation(movingRobot, map) > 10) // хай та функція певертає суму енергії всіх доступних станцій, якщо воа = 0 - скаутінг
                return new CollectEnergyCommand();

            Position stationPosition = FindNearestFreeStation(robots[robotToMoveIndex], map, robots);

            if (stationPosition == null)
                return null;


            Position newPosition = stationPosition;//FindNearestEatingPosition(stationPosition, movingRobot, robots);

            if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                return new MoveCommand() { NewPosition = newPosition };
            else
            {
                int dx = stationPosition.X - movingRobot.Position.X;
                int dy = stationPosition.Y - movingRobot.Position.Y;
                if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                else
                {
                    dx = Math.Sign(dx);
                    dy = Math.Sign(dy);
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                }
            }        
            
        }

        private RobotCommand Fighting(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            var stations = map.GetNearbyResources(movingRobot.Position, 2);

            if (stations.Count == 0)
                return Scouting(robots, robotToMoveIndex, map);

            Position nearestEnemy = new Position(movingRobot.Position.X, movingRobot.Position.Y);
            foreach (var s in stations)
            {
                var enemies = IsStationOnlyMy(s, movingRobot, robots, map);
                if (enemies.Count == 0) return  new CollectEnergyCommand();
                foreach (var robot in enemies)
                {
                    if (DistanceHelper.FindDistance(movingRobot.Position, nearestEnemy) > DistanceHelper.FindDistance(robot.Position, movingRobot.Position) &&
                         DistanceHelper.FindDistance(robot.Position, movingRobot.Position) + (30 + robot.Energy * 0.3) + 20 < movingRobot.Energy)
                        nearestEnemy = robot.Position;
                } 
                
            }

            if (nearestEnemy != movingRobot.Position)
                return new MoveCommand() { NewPosition = nearestEnemy };
            return new CollectEnergyCommand();

        }

        private RobotCommand Scouting(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            int MinEnergyToCreateNewRobot = 200 + 100 + 40;
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            //if ((movingRobot.Energy > MinEnergyToCreateNewRobot) && (robots.Count < map.Stations.Count))
            //{
            //    return new CreateNewRobotCommand();
            //}

            //if (AmIOnStation(movingRobot, map))
            //    return new CollectEnergyCommand();

            Position stationPosition = FindNearestFreeStation(robots[robotToMoveIndex], map, robots);

            if (stationPosition == null)
                return null;


            Position newPosition = FindNearestEatingPosition(stationPosition, movingRobot, robots);

            if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                return new MoveCommand() { NewPosition = newPosition };
            else
            {
                int dx = stationPosition.X - movingRobot.Position.X;
                int dy = stationPosition.Y - movingRobot.Position.Y;
                if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                else
                {
                    dx = Math.Sign(dx);
                    dy = Math.Sign(dy);
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                }
            }        
            
        }

        private RobotCommand Eating(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
           // int MinEnergyToCreateNewRobot = 200 + 100 + 40;
            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            //if ((movingRobot.Energy > MinEnergyToCreateNewRobot) && (robots.Count < map.Stations.Count))
            //{
            //    return new CreateNewRobotCommand();
            //}
            if (AmIOnStation(movingRobot, map) > 10) // хай та функція певертає суму енергії всіх доступних станцій, якщо воа = 0 - скаутінг
                return new CollectEnergyCommand();

            //if (AmIOnStation(movingRobot, map))
            //    return new CollectEnergyCommand();

            Position stationPosition = FindNearestFreeStation(robots[robotToMoveIndex], map, robots);

            if (stationPosition == null)
                return null;


            Position newPosition = FindNearestEatingPosition(stationPosition, movingRobot, robots);

            if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                return new MoveCommand() { NewPosition = newPosition };
            else
            {
                int dx = stationPosition.X - movingRobot.Position.X;
                int dy = stationPosition.Y - movingRobot.Position.Y;
                if (DistanceHelper.FindDistance(newPosition, movingRobot.Position) < movingRobot.Energy - 40)
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                else
                {
                    dx = Math.Sign(dx);
                    dy = Math.Sign(dy);
                    return Move(robots, movingRobot, map, new Position(movingRobot.Position.X + dx, movingRobot.Position.Y + dy));
                }
            }        
            
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            switch (StrategyState.ActiveState)
            {
                case StrategyState.States.Borning:
                    return Borning(robots, robotToMoveIndex, map);
                case StrategyState.States.Scouting:
                    return Scouting(robots, robotToMoveIndex, map);
                case StrategyState.States.Fighting:
                    return Fighting(robots, robotToMoveIndex, map);
                case StrategyState.States.Eating:
                    return Eating(robots, robotToMoveIndex, map);
                default:
                    throw new Exception("Invalid value for StrategyState");
            }
            return null;
        }

        private RobotCommand Move(IList<Robot.Common.Robot> robots, Robot.Common.Robot movingRobot, Map map, Position nextPosition)
        {
            if (IsCellFree(nextPosition,movingRobot,robots))
                return new MoveCommand() { NewPosition = nextPosition };
            nextPosition = map.FindFreeCell(nextPosition, robots);
            return new MoveCommand() { NewPosition = nextPosition };
            
        }

        public int AmIOnStation(Robot.Common.Robot movingRobot, Map map)
        {
            int Sum = 0;
            var resourses = map.GetNearbyResources(movingRobot.Position, 2);
            if (resourses.Count > 0)
                foreach (var s in resourses)
                    Sum += s.Energy;

            return Sum;
        }

        public Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            List<EnergyStation> nearest = new List<EnergyStation>();

            var stations = from S in map.Stations
                          where S.Energy > 10
                          select S;
            int minDistance = int.MaxValue;
            foreach (var station in stations)
            {
                int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);

                if (d < minDistance)
                {
                    minDistance = d;
                    nearest.Add(station);
                }
            }

            EnergyStation mostFree = null;
            nearest.Reverse();
            minDistance = 100;
            int dis = 0;

            for (int i = 0; i < Math.Min(nearest.Count, 5); i++) // 5 - це перших 4 станцій, тобто серед 4 найближчих станцій вибираємо найвільнішу.
            {
                dis = IsStationMoreFree(nearest.ElementAt(i), movingRobot, robots, map);
                if ((dis < minDistance))
                {
                    minDistance = dis;
                    mostFree = nearest.ElementAt(i);
                }
            }

            return mostFree == null ? null : mostFree.Position;
        }

        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            return IsCellFree(station.Position, movingRobot, robots);
        }

        private int IsStationMoreFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            int robotsOnStation = 0;

            foreach (var robot in robots)
            {
                var list = map.GetNearbyResources(robot.Position, 2);
                foreach (var s in list)
                    if (s == station) robotsOnStation++;
            }
            return robotsOnStation - 1;
        }

        private List<Robot.Common.Robot> IsStationOnlyMy(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots, Map map)
        {
            List<EnergyStation> list = new List<EnergyStation>();
            List<Robot.Common.Robot> enemies = new List<Robot.Common.Robot>();

            try
            {
                foreach (var robot in robots)
                {
                    if (robot.Owner != movingRobot.Owner)
                    {
                        list = map.GetNearbyResources(robot.Position, 2);
                        foreach (var s in list)
                            if (s == station) enemies.Add(robot);
                    }
                }
                return enemies;
            }
            catch
            {
                return enemies;
            }
            
        }

        public bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    if (robot.Position == cell)
                        return false;
                }
            }
            return true;
        }

        private Position FindNearestEatingPosition(Position stationP, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int dx = stationP.X - movingRobot.Position.X - 2;
            int dy = stationP.Y - movingRobot.Position.Y - 2;

            if (IsCellFree(new Position(dx, dy), movingRobot, robots))
                return new Position(dx, dy);

            else if (IsCellFree(new Position(++dx, ++dy), movingRobot, robots))
                return new Position(dx, dy);
            else return stationP;
        }
    }
}
