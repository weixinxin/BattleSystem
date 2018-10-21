using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.SkillModule
{
    public abstract class SkillAction
    {
        protected Skill mSkill;
        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>是否结束动作</returns>
        public abstract bool Execute(float dt);

        public SkillAction Create(Skill skill,string name,string[] args)
        {
            SkillAction res = null;
            ActionType type = (ActionType)Enum.Parse(typeof(ActionType), name);
            switch(type)
            {
                case ActionType.kWaitSeconds:
                    float dt;
                    if(float.TryParse(args[0],out dt))
                    {
                        res = new WaitSecondsAction(dt);
                        res.mSkill = skill;
                    }
                    break;
                default:
                    res = new InstantAction(type, args);
                    res.mSkill = skill;
                    break;
            }
            return res;
        }

        public abstract void Reset();
    }

    /// <summary>
    /// 瞬时动作
    /// </summary>
    public class InstantAction : SkillAction
    {
        private string[] Args;
        private ActionType Type;
        public InstantAction(ActionType t,string[] args)
        {
            Type = t;
            Args = args;
        }
        public override bool Execute(float dt)
        {
            switch(Type)
            {
                case ActionType.kSelectTargets:

                    break;
                case ActionType.kAddBuff:
                    break;
                case ActionType.kAoeField:
                    break;
                case ActionType.kPlayAnimation:
                    break;
                case ActionType.kPlayEffect:
                    break;
                case ActionType.kPlaySound:
                    break;
                case ActionType.kShootBullet:
                    break;
                case ActionType.kSummon:
                    break;
                default:
                    Trace.Assert(false, "未实现的技能行为" + Type.ToString());
                    break;
            }
            Debug.Log("Execute " + Type.ToString());
            return true;
        }
        public override void Reset()
        {

        }

    }
    
    /// <summary>
    /// 等待动作
    /// </summary>
    public class WaitSecondsAction : SkillAction
    {
        private float mElapseTime = 0;
        private float mDuration;

        public WaitSecondsAction(float t)
        {
            mDuration = t;
            mElapseTime = 0;
        }
        public override bool Execute(float dt)
        {
            mElapseTime += dt;
            Debug.Log("Execute WaitSecondsAction");
            return mElapseTime >= mDuration;
        }
        public override void Reset()
        {
            mElapseTime = 0;
        }
    }
     
    /// <summary>
    /// 序列动作
    /// </summary>
    public class SequenceAction : SkillAction
    {
        private SkillAction[] mActions;
        private int mIndex = 0;
        public SequenceAction(SkillAction[] actions)
        {
            mActions = actions;
            mIndex = 0;
        }
        public override bool Execute(float dt)
        {
            while(mIndex < mActions.Length)
            {
                Debug.Log("Execute SequenceAction " + mIndex);
                if(mActions[mIndex].Execute(dt))
                {
                    mIndex++;
                }
                else
                {
                    break;
                }
            }
            return mIndex >= mActions.Length;
        }
        public override void Reset()
        {
            mIndex = 0;
            for(int i = 0; i < mActions.Length;++i)
            {
                mActions[i].Reset();
            }
        }

    }

    /// <summary>
    /// 平行动作
    /// </summary>
    public class ParallelAction : SkillAction
    {
        private SkillAction[] mActions;
        private List<SkillAction> mList = null;
        public ParallelAction(SkillAction[] actions)
        {
            mActions = actions;
            mList = new List<SkillAction>(actions.Length);
            
            for(int i = actions.Length - 1; i >= 0 ; --i)
            {
                mList.Add(actions[actions.Length - 1 - i]);
            }
        }
        public override bool Execute(float dt)
        {
            Debug.Log("Execute ParallelAction");
            for (int i = mList.Count - 1; i >= 0; --i)
            {
                if (mList[i].Execute(dt))
                {
                    mList.RemoveAt(i);
                }
            }
            return mList.Count == 0;
        }
        public override void Reset()
        {
            mList.Clear();
            for (int i = mActions.Length - 1; i >= 0; --i)
            {
                var action = mActions[mActions.Length - 1 - i];
                action.Reset();
                mList.Add(action);
            }
        }
    }
}
