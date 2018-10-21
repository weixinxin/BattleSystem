namespace BattleSystem
{
    public enum ActionType
    {
        //播放角色动画
        kPlayAnimation = 1,
        //播放特效
        kPlayEffect,
        //播放音效
        kPlaySound,
        //延迟等待
        kWaitSeconds,
        //产生法术场
        kAoeField,
        //为目标施加buff
        kAddBuff,
        //召唤单位
        kSummon,
        //发射子弹
        kShootBullet,
        //选择目标
        kSelectTargets,
    }

    public enum BuffEffectType
    {
        //物理伤害
        kPhysicalDamage = 1,
        //魔法伤害
        kMagicDamage,
        //真实伤害
        kTrueDamage,
        //治疗
        kHeal,
        //移动加速
        kSpeedUp,
        //移动减速
        kSlowDown,
        //减少技能CD
        //kCDReduction,
        //增加攻击力
        kIncreaseATK,
        //减少攻击力
        kDecreaseATK,
        //增加攻击速度
        kIncreaseAttackSpeed,
        //减少攻击速度
        kDecreaseAttackSpeed,
        //增加攻击距离
        kExtendAttackRange,
        //缩小攻击距离
        kReduceAttackRange,
        //增加视野距离
        kExtendVisualRange,
        //缩小视野距离
        kReduceVisualRange,
        //无法移动
        kUnmovable,
        //无法施法
        kUnableCast,
        //物理伤害免疫
        kPhysicalDamageImmunity,
        //魔法伤害免疫
        kMagicDamageImmunity,
        //无法攻击
        kUnableAttack,
        //不被选中
        kNotarget,
        //攻击失效
        kAttackMiss,
        //不死
        kDeathless,
        //负面效果免疫
        kNegativeEffectImmunity,
        //清除负面效果
        kCleanse,
    }
    public enum BuffEvent
    {

    }
    public enum EventCode
    {
        //单位死亡
        UnitDead,
    }
}