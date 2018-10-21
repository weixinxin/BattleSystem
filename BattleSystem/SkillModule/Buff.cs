
using BattleSystem.ObjectModule;
using System.Collections.Generic;
namespace BattleSystem.SkillModule
{
    /// <summary>
    /// 附加状态
    /// </summary>
    public class Buff
    {

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get;private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 首次延迟
        /// </summary>
        public float Delay { get; private set; }
        private float mDelay = 0;

        /// <summary>
        /// 循环标记
        /// </summary>
        public bool isLoop { get; private set; }

        /// <summary>
        /// 间隔
        /// </summary>
        public float Duration { get; private set; }
        private float mElapseTime = 0;

        /// <summary>
        /// 生命周期不受父节点影响
        /// </summary>
        public bool isIndividual { get; private set; }
        
        /// <summary>
        /// 是否可移除
        /// </summary>
        public bool isClearable { get; private set; }

        /// <summary>
        /// 是否是负面影响
        /// </summary>
        public bool isNegative { get; private set; }

        /// <summary>
        /// buff持有者
        /// </summary>
        public UnitBase Owner { get; private set; }

        /// <summary>
        /// buff施法者
        /// </summary>
        public UnitBase Caster { get; private set; }


        private List<BuffEffect> mEffects;

        private List<BuffEffect> mRunningEffects = new List<BuffEffect>();
        private int mPauseCount = 0;
        public Buff(int id,int templateID,UnitBase Owner,UnitBase caster)
        {
            this.Owner = Owner;
            this.Caster = caster;
            this.ID = id;
            mPauseCount = 0;
            //根据templateID 读取配置
            mEffects = SkillManager.LoadBuffEffect(templateID);
            this.Delay = 0;

            mDelay = Delay;
            if (mDelay == 0)
                Apply();
            mElapseTime = 0;
        }
        protected void Apply()
        {
            for (int i = 0; i < mEffects.Count; ++i)
            {
                if(mEffects[i].Apply(Owner))
                {
                    //需要结束后移除的效果才添加进队列 
                    mRunningEffects.Add(mEffects[i]);
                }
            }
        }
        
        /// <summary>
        /// 更新buff
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>是否结束</returns>
        public bool Update(float dt)
        {
            //检查buff效果是否结束
            if(mDelay > 0)
            {
                mDelay -= dt;
                if(mDelay <= 0)
                {
                    Apply();
                }
            }
            else if (isLoop)
            {
                mElapseTime += dt;
                if(mElapseTime >= Duration)
                {
                    Clear();
                    //循环buff
                    mElapseTime = 0;
                    Apply();
                }
            }
            else if(mElapseTime < Duration)
            {
                mElapseTime += dt;
                if (mElapseTime >= Duration)
                {
                    Clear();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 清除buff
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < mRunningEffects.Count; ++i)
            {
                mRunningEffects [i].Clear(Owner);
            }
            mRunningEffects.Clear();
        }
        

        /// <summary>
        /// 暂停buff效果(例如沉默，计数器是针对多个沉默先后施加的情况)
        /// </summary>
        public void Pause()
        {
            if (mPauseCount == 0)
            {
                Clear();
            }
            mPauseCount++;
        }

        /// <summary>
        /// 恢复buff效果(例如沉默效果结束)
        /// </summary>
        public void Resume()
        {
            if (mPauseCount > 0)
            {
                mPauseCount--;
                if (mPauseCount == 0)
                {
                    Apply();
                }
            }
            else
            {
                Debug.LogError("buff is not paused!");
            }
        }
    }
}