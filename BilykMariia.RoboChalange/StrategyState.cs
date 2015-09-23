using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BilykMariia.RoboChalange
{
    public class StrategyState
    {
        public enum States { Start, Borning, Scouting, Fighting, Eating };
        private static States state;

        public static States ActiveState
        {
            get { return state; }
            set { state = value; }
        }

        public static void NextState()
        {
                switch (ActiveState)
                {
                    case StrategyState.States.Borning:
                        ActiveState = States.Scouting;
                        break;
                    case StrategyState.States.Scouting:
                        ActiveState = States.Fighting;
                        break;
                    case StrategyState.States.Eating:
                        ActiveState = States.Eating;
                        break;
                    case StrategyState.States.Fighting:
                        ActiveState = States.Eating;
                        break;
                    default:
                        throw new Exception("Invalid value for GameState");
                }
            
        }
    }
}
