using System.Collections.Generic;
using System.Diagnostics;
using BattleSystem.ObjectModule;
namespace BattleSystem.SkillModule
{
    /// <summary>
    /// buff效果
    /// </summary>
    public class BuffEffect
    {
        /// <summary>
        /// 效果类型
        /// </summary>
        protected BuffEffectType Type;

        /// <summary>
        /// 是否永久效果
        /// </summary>
        protected bool isPermanent;

        /// <summary>
        /// 基础值变化
        /// </summary>
        protected int BaseDelta;

        /// <summary>
        /// 当前值变化
        /// </summary>
        protected int CurDelta;

        /// <summary>
        /// 当前值的百分比改变
        /// </summary>
        protected float CurPercent;

        /// <summary>
        /// 基础值的百分比改变
        /// </summary>
        protected float BasePercent;


        public BuffEffect(int templateID)
        {
            //根据模板ID获取参数

        }

        private void ModifyAttribute(Attribute attribute,bool nagative)
        {
            int sign = nagative ? -1 : 1;
            attribute.BaseModify(BaseDelta * sign);
            attribute.CurModify(CurDelta * sign);
            attribute.BasePercentageModify(BasePercent * sign);
            attribute.CurPercentageModify(CurPercent * sign);
        }

        private void ClearModifyAttribute(Attribute attribute, bool nagative)
        {
            if (!isPermanent)
            {
                int sign = nagative ? -1 : 1;
                attribute.RemoveBase(BaseDelta * sign);
                attribute.RemoveCur(CurDelta * sign);
                attribute.RemoveBasePercentage(BasePercent * sign);
                attribute.RemoveCurPercentage(CurPercent * sign);
            }
        }

        private void DoDamage(UnitBase Owner)
        {
            Owner.LostHP(CurDelta, CurPercent, BasePercent, BaseDelta * 0.01f);
        }
        private void DoHeal(UnitBase Owner)
        {
            Owner.AddHP(CurDelta, CurPercent, BasePercent, BaseDelta * 0.01f);
        }
        /// <summary>
        /// 应用效果
        /// </summary>
        /// <returns>是否需要移除</returns>
        public bool Apply(UnitBase Owner)
        {
            //永久效果不用移除
            bool res = !isPermanent;
            switch(Type)
            {
                case BuffEffectType.kAttackMiss:
                case BuffEffectType.kMagicDamageImmunity:
                case BuffEffectType.kNotarget:
                case BuffEffectType.kPhysicalDamageImmunity:
                case BuffEffectType.kUnableAttack:
                case BuffEffectType.kUnableCast:
                case BuffEffectType.kUnmovable:
                case BuffEffectType.kDeathless:
                    Owner.AddState(Type);
                    break;
                case BuffEffectType.kPhysicalDamage:
                    if (!Owner.isPhysicalDamageImmunity)
                    {
                        //扣血
                        DoDamage(Owner);
                    }
                    else
                    {
                        res = false;
                    }
                    break;
                case BuffEffectType.kMagicDamage:
                    if (!Owner.isMagicDamageImmunity)
                    {
                        //扣血
                        DoDamage(Owner);
                    }
                    else
                    {
                        res = false;
                    }
                    break;
                case BuffEffectType.kTrueDamage:
                    //直接扣血
                    DoDamage(Owner);
                    break;
                case BuffEffectType.kHeal:
                    //回血
                    DoHeal(Owner);
                    break;
                case BuffEffectType.kSpeedUp:
                    //移动加速
                    ModifyAttribute(Owner.MoveSpeed,false);
                    break;
                case BuffEffectType.kSlowDown:
                    //移动减速
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.MoveSpeed, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kIncreaseATK:
                    //增加攻击力
                    ModifyAttribute(Owner.ATK, false);
                    break;
                case BuffEffectType.kDecreaseATK:
                    //减少攻击力
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.ATK, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kIncreaseAttackSpeed:
                    //增加攻击速度
                    ModifyAttribute(Owner.AttackDuration, false);
                    break;
                case BuffEffectType.kDecreaseAttackSpeed:
                    //减少攻击速度
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.AttackDuration, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kExtendAttackRange:
                    //增加攻击距离
                    ModifyAttribute(Owner.AttackRange, false);
                    break;
                case BuffEffectType.kReduceAttackRange:
                    //缩小攻击距离
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.AttackRange, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kExtendVisualRange:
                    //增加视野距离
                    ModifyAttribute(Owner.VisualRange, false);
                    break;
                case BuffEffectType.kReduceVisualRange:
                    //缩小视野距离
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.VisualRange, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kCleanse:
                    //清除负面效果
                    if (isPermanent)
                    {
                        //永久效果直接移除目标身上的效果
                        for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                        {
                            Buff buff = Owner.Buffs[i];
                            if (buff.isClearable && buff.isNegative)
                            {
                                buff.Clear();
                                Owner.Buffs.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        //非永久效果则暂时让对方负面效果失效
                        for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                        {
                            Buff buff = Owner.Buffs[i];
                            if (buff.isClearable && buff.isNegative)
                            {
                                buff.Pause();
                            }
                        }
                    }
                    break;
                default:
                    Trace.Assert(false, "未实现的BuffEffect" + Type.ToString());
                    break;

            }
            return res;
        }
        
        /// <summary>
        /// 清除效果
        /// </summary>
        public void Clear(UnitBase Owner)
        {
            
            switch(Type)
            {
                case BuffEffectType.kAttackMiss:
                case BuffEffectType.kMagicDamageImmunity:
                case BuffEffectType.kNotarget:
                case BuffEffectType.kPhysicalDamageImmunity:
                case BuffEffectType.kUnableAttack:
                case BuffEffectType.kUnableCast:
                case BuffEffectType.kUnmovable:
                case BuffEffectType.kDeathless:
                    Owner.RemoveState(Type);
                    break;
                case BuffEffectType.kPhysicalDamage:
                case BuffEffectType.kMagicDamage: 
                case BuffEffectType.kTrueDamage:
                    //伤害返还
                    DoHeal(Owner);
                    break;
                case BuffEffectType.kHeal:
                    //加血返还
                    DoDamage(Owner);
                    break;
                case BuffEffectType.kSpeedUp:
                    //移动加速
                    ClearModifyAttribute(Owner.MoveSpeed, false);
                    break;
                case BuffEffectType.kSlowDown:
                    //移动减速
                    ClearModifyAttribute(Owner.MoveSpeed, true);
                    break;
                case BuffEffectType.kIncreaseATK:
                    //增加攻击力
                    ClearModifyAttribute(Owner.ATK, false);
                    break;
                case BuffEffectType.kDecreaseATK:
                    //减少攻击力
                    ClearModifyAttribute(Owner.ATK, true);
                    break;
                case BuffEffectType.kIncreaseAttackSpeed:
                    //增加攻击速度
                    ClearModifyAttribute(Owner.AttackDuration, false);
                    break;
                case BuffEffectType.kDecreaseAttackSpeed:
                    //减少攻击速度
                    ClearModifyAttribute(Owner.AttackDuration, true);
                    break;
                case BuffEffectType.kExtendAttackRange:
                    //增加攻击距离
                    ClearModifyAttribute(Owner.AttackRange, false);
                    break;
                case BuffEffectType.kReduceAttackRange:
                    //缩小攻击距离
                    ClearModifyAttribute(Owner.AttackRange, true);
                    break;
                case BuffEffectType.kExtendVisualRange:
                    //增加视野距离
                    ClearModifyAttribute(Owner.VisualRange, false);
                    break;
                case BuffEffectType.kReduceVisualRange:
                    //缩小视野距离
                    ClearModifyAttribute(Owner.VisualRange, true);
                    break;
                case BuffEffectType.kCleanse:
                    for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                    {
                        Buff buff = Owner.Buffs[i];
                        if (buff.isClearable && buff.isNegative)
                        {
                            buff.Resume();
                        }
                    }
                    break;
                default:
                    Trace.Assert(false, "未实现的BuffEffect" + Type.ToString());
                    break;

            }

        }
    }
}
