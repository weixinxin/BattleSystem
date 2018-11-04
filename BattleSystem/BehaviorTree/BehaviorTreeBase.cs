using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BattleSystem.BehaviorTree
{

    public class AICommand
    {
        public static string Moving = "Moving";
        public static string Skill = "Skill";
        public static string Attack = "Attack";
        public static string RearrageFormation = "RearrageFormation";       
    }
    public delegate void AICommandCallbackEvent();



    class BehaviorTreeBase:NetworkBehaviorTree
    {
        public override float GameTime
        {
            get
            {
                return BattleInterface.Instance.GameTimeElapsed;
            }
        }

        public virtual void NotifyMoveTo(Vector3 targetLocation, AICommandCallbackEvent callback = null)
        { 
        
        
        }



    }
}
