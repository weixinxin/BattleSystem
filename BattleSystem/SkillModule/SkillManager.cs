using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.SkillModule
{
    public static class SkillManager
    {
        public static SkillAction LoadSkillAction(int templateID)
        {
            SkillAction[] acts = new SkillAction[3];
            acts[0] = new WaitSecondsAction(1);
                SkillAction[] pacts = new SkillAction[3];
                pacts[0] = new WaitSecondsAction(0.5f);
                pacts[1] = new InstantAction(ActionType.kAoeField, new string[0]);
                pacts[2] = new InstantAction(ActionType.kPlayEffect, new string[0]);
                acts[1] = new ParallelAction(pacts);
            acts[2] = new InstantAction(ActionType.kAddBuff,new string[0]);
            SkillAction root = new SequenceAction(acts);
            return root;
        }

        public static List<BuffEffect> LoadBuffEffect(int templateID)
        {
            return new List<BuffEffect>();
        }
    }
}
