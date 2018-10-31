using BattleSystem.SkillModule;
using BattleSystem.SpaceModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.ObjectModule
{
    public delegate void FloatDelegate(float f);
    public class UnitBase : IMovable
    {


        private WorldSpace mWorldSpace = null;

        internal GridNode mGridNode = null;
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 position { get; protected set; }
        /// <summary>
        /// 护盾
        /// </summary>
        public Shield Shield = new Shield();

        /// <summary>
        /// 血量
        /// </summary>
        public int HP { get; private set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 最大血量
        /// </summary>
        public int MaxHP { get; set; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; private set; }

        private float mDestroyCountdown = 5.0f;

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
        /// 尝试往单位身上加buff
        /// </summary>
        /// <param name="templateID">模板id</param>
        /// <param name="caster">施法者</param>
        /// <returns>是否成功</returns>
   
        internal bool AddBuff(int templateID,UnitBase caster)
        {
            if (IsDead) return false;
            int Groups = 1;//读取配置获取所属组
            for(int i = Buffs.Count - 1; i >=0;--i)
            {
                if (Buffs[i].MutexCheck(Groups))
                    return false;
            }
            for (int i = Buffs.Count - 1; i >= 0; --i)
            {
                if (Buffs[i].OverlayCheck(templateID))
                    return Buffs[i].OverlayTactics == OverlayTactics.kAddTime;
            }
            var buff = new Buff(templateID, this, caster);
            Buffs.Add(buff);
            return true;
        }

        /// <summary>
        /// 加状态
        /// </summary>
        /// <param name="state">状态</param>
        internal void AddState(BuffEffectType state)
        {
            if (IsDead) return;
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
        /// <param name="state">状态</param>
        internal void RemoveState(BuffEffectType state)
        {
            if (IsDead) return;
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

        private static int s_id = 1;
        private static int id
        {
            get
            {
                return s_id++;
            }
        }
        public UnitBase(WorldSpace ws,int templateID, int campID, int level)
        {
            ID = id;
            CampID = campID;
            HP = 100;
            MaxHP = 100; 
            mWorldSpace = ws; 
            IsDead = false;
            ATK = new Attribute(100, null);
            MoveSpeed = new Attribute(100, null);
            AttackDuration = new AttackDuration(100, null);
            AttackRange = new Attribute(100, null);
            VisualRange = new Attribute(100, null);

        }
        /// <summary>
        /// 单位受到治疗
        /// </summary>
        /// <param name="delta">治疗量</param>
        /// <param name="healer">治疗者</param>
        public void AddHP(int delta,UnitBase healer)
        {
            if (IsDead) return;
            var offset = BuffManager.OnUnitWillHeal(this, healer, delta);
            delta += offset;
            if(delta > 0)
            {
                var origin = HP;
                HP = HP + delta;
                if (HP > MaxHP) HP = MaxHP;
                BuffManager.OnUnitBeHealed(this, healer, HP - origin);
            }
        }
        /// <summary>
        /// 单位受到伤害
        /// </summary>
        /// <param name="delta">伤血值</param>
        /// <param name="assailant">攻击者</param>
        /// <param name="dt">伤害类型</param>
        /// <param name="isAttack">是否普攻</param>
        public void LostHP(int delta, UnitBase assailant, DamageType dt,bool isAttack)
        {
            if (IsDead) return;
            var offset = BuffManager.OnUnitWillHurt(this, assailant, delta, dt, isAttack);
            delta += offset;
            if (delta > 0)
            {
                var _delta = Shield.Consume(delta);
                HP -= _delta;
                BuffManager.OnUnitBeHurted(this, assailant, delta, dt, isAttack);
                if (HP < 0)
                {
                    if (isDeathless)
                    {
                        HP = 1;
                    }
                    else if (BuffManager.OnUnitWillDie(this, assailant))
                    {
                        HP = 0;
                        OnDead();
                        BuffManager.OnUnitBeSlayed(this, assailant);
                    }
                    else
                    {
                        if (HP <= 0) HP = 1;
                    }
                }
            }
        }

        public virtual void OnDead()
        {
            IsDead = true;
            mDestroyCountdown = 5.0f;
        }

        /// <summary>
        /// 更新单位逻辑
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns>是否可以销毁单位</returns>
        public virtual bool Update(float dt)
        {
            if(IsDead)
            {
                mDestroyCountdown -= dt;
            }
            else
            {
                //更新并移除已经结束的buff
                for (int i = Buffs.Count - 1; i >= 0; --i)
                {
                    if (Buffs[i].Update(dt))
                    {
                        Buffs[i].Destroy();
                        Buffs.RemoveAt(i);
                    }
                }
            }
            return mDestroyCountdown <= 0;
        }


        public void UpdatePosition(float x, float y, float z)
        {
            if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) || float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z))
            {
                Debug.LogErrorFormat("error: {0},{1},{2}", x, y, z);
                return;
            }
            position = new Vector3(x, y, z);
            mWorldSpace.UpdateNode(this);
        }

        public float speed
        {
            get { return MoveSpeed.value; }
            set { }
        }


        public float acceleration
        {
            get { return 0; }
        }
    }
}
