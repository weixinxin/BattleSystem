using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.ObjectModule
{
    public delegate void FloatDelegate(float f);
    public class UnitBase :BaseObject
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 血量
        /// </summary>
        public int HP { get; private set; }

        /// <summary>
        /// 最大血量
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; private set; }

        /// <summary>
        /// 攻击力
        /// </summary>
        public Attribute ATK { get; set; }

        /// <summary>
        /// 移动速度
        /// </summary>
        public Attribute MoveSpeed { get; set; }

        /// <summary>
        /// 攻击间隔
        /// </summary>
        public AttackDuration AttackDuration { get; set; }

        /// <summary>
        /// 攻击距离
        /// </summary>
        public Attribute AttackRange { get; set; }
        
        /// <summary>
        /// 视野
        /// </summary>
        public Attribute VisualRange { get; set; }

        /// <summary>
        /// 阵营ID
        /// </summary>
        public int CampID { get; set; }


        private int mAttackMissCount = 0;
        private int mMagicDamageImmunityCount = 0;
        private int mNotargetCount = 0;
        private int mPhysicalDamageImmunityCount = 0;
        private int mUnableAttackCount = 0;
        private int mUnableCastCount = 0;
        private int mUnmovableCount = 0;
        private int mDeathlessCount = 0;
        private int mNegativeEffectImmunityCount = 0;
        

        /// <summary>
        /// 是否处于攻击失效状态
        /// </summary>
        public bool isAttackMiss { get { return mAttackMissCount > 0; } }

        /// <summary>
        /// 是否处于魔法免疫状态
        /// </summary>
        public bool isMagicDamageImmunity { get { return mMagicDamageImmunityCount > 0; } }

        /// <summary>
        /// 是否处于不被选中状态
        /// </summary>
        public bool isNotarget { get { return mNotargetCount > 0; } }

        /// <summary>
        /// 是否处于物理免疫状态
        /// </summary>
        public bool isPhysicalDamageImmunity { get { return mPhysicalDamageImmunityCount > 0; } }

        /// <summary>
        /// 是否处于无法攻击状态
        /// </summary>
        public bool isUnableAttack { get { return mUnableAttackCount > 0; } }

        /// <summary>
        /// 是否处于无法施法状态
        /// </summary>
        public bool isUnableCast { get { return mUnableCastCount > 0; } }

        /// <summary>
        /// 是否处于无法移动状态
        /// </summary>
        public bool isUnmovable { get { return mUnmovableCount > 0; } }

        /// <summary>
        /// 是否处于不死状态
        /// </summary>
        public bool isDeathless { get { return mDeathlessCount > 0; } }

        
        /// <summary>
        /// 是否免疫负面效果
        /// </summary>
        public bool isNegativeEffectImmunity { get { return mNegativeEffectImmunityCount > 0; } }


        internal List<Buff> Buffs = new List<Buff>();


        /// <summary>
        /// 加状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal void AddState(BuffEffectType state)
        {
            switch (state)
            {
                case BuffEffectType.kAttackMiss:
                    mAttackMissCount++;
                    break;
                case BuffEffectType.kMagicDamageImmunity:
                    mMagicDamageImmunityCount++;
                    break;
                case BuffEffectType.kNotarget:
                    mNotargetCount++;
                    break;
                case BuffEffectType.kPhysicalDamageImmunity:
                    mPhysicalDamageImmunityCount++;
                    break;
                case BuffEffectType.kUnableAttack:
                    mUnableAttackCount++;
                    break;
                case BuffEffectType.kUnableCast:
                    mUnableCastCount++;
                    break;
                case BuffEffectType.kUnmovable:
                    mUnmovableCount++;
                    break;
                case BuffEffectType.kDeathless:
                    mDeathlessCount++;
                    break;
                case BuffEffectType.kNegativeEffectImmunity:
                    mNegativeEffectImmunityCount++;
                    break;
            }
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        internal void RemoveState(BuffEffectType state)
        {
            switch (state)
            {
                case BuffEffectType.kAttackMiss:
                    Trace.Assert(mAttackMissCount > 0);
                    mAttackMissCount--;
                    break;
                case BuffEffectType.kMagicDamageImmunity:
                    Trace.Assert(mMagicDamageImmunityCount > 0);
                    mMagicDamageImmunityCount--;
                    break;
                case BuffEffectType.kNotarget:
                    Trace.Assert(mNotargetCount > 0);
                    mNotargetCount--;
                    break;
                case BuffEffectType.kPhysicalDamageImmunity:
                    Trace.Assert(mPhysicalDamageImmunityCount > 0);
                    mPhysicalDamageImmunityCount--;
                    break;
                case BuffEffectType.kUnableAttack:
                    Trace.Assert(mUnableAttackCount > 0);
                    mUnableAttackCount--;
                    break;
                case BuffEffectType.kUnableCast:
                    Trace.Assert(mUnableCastCount > 0);
                    mUnableCastCount--;
                    break;
                case BuffEffectType.kUnmovable:
                    Trace.Assert(mUnmovableCount > 0);
                    mUnmovableCount--;
                    break;
                case BuffEffectType.kDeathless:
                    Trace.Assert(mDeathlessCount > 0);
                    mDeathlessCount--;
                    break;
                case BuffEffectType.kNegativeEffectImmunity:
                    Trace.Assert(mNegativeEffectImmunityCount > 0);
                    mNegativeEffectImmunityCount--;
                    break;
            }
        }
        public UnitBase(){}
        public UnitBase(int id, int templateID, int campID)
        {
            ID = id;
            CampID = campID;
            HP = 100;
            MaxHP = 100;
            IsDead = false;
            ATK = new Attribute(100, null);
            MoveSpeed = new Attribute(100, null);
            AttackDuration = new AttackDuration(100, null);
            AttackRange = new Attribute(100, null);
            VisualRange = new Attribute(100, null);

        }
        public void AddHP(int hp,float curPercent,float maxPercent,float lostPercent)
        {
            Trace.Assert(hp >= 0 && curPercent >= 0 &&maxPercent >= 0 &&lostPercent >= 0);
            float delta = hp + HP * curPercent + MaxHP * maxPercent + (MaxHP - HP) * lostPercent;

            HP = HP + (int)delta;
            if (HP > MaxHP) HP = MaxHP;
        }

        public void LostHP(int hp, float curPercent, float maxPercent, float lostPercent)
        {
            Trace.Assert(hp >= 0 && curPercent >= 0 && maxPercent >= 0 && lostPercent >= 0);
            float delta = hp + HP * curPercent + MaxHP * maxPercent + (MaxHP - HP) * lostPercent;

            HP = HP - (int)delta;
            if (HP < 0)
            {
                if(isDeathless)
                {
                    HP = 1;
                }
                else
                {
                    HP = 0;

                }
            }
        }
        public virtual void Update(float dt)
        {
            //更新并移除已经结束的buff
            for(int i = Buffs.Count - 1;i >= 0; --i)
            {
                if(Buffs[i].Update(dt))
                {
                    Buffs.RemoveAt(i);
                }
            }
        }
    }
}
