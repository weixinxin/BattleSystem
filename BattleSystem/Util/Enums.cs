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
        kSelectTarget,
    }

    public enum BuffEffectType
    {
        //物理伤害
        kPhysicalDamage = 0,
        //魔法伤害
        kMagicDamage = 1,
        //真实伤害
        kTrueDamage =2,
        //治疗
        kHeal =3,
        //移动加速
        kSpeedUp = 4,
        //移动减速
        kSlowDown = 5,
        //减少技能CD
        //kCDReduction,
        //增加攻击力
        kIncreaseATK = 6,
        //减少攻击力
        kDecreaseATK = 7,
        //增加攻击速度
        kIncreaseAttackSpeed = 8,
        //减少攻击速度
        kDecreaseAttackSpeed = 9,
        //增加攻击距离
        kExtendAttackRange =10,
        //缩小攻击距离
        kReduceAttackRange = 11,
        //增加视野距离
        kExtendVisualRange = 12,
        //缩小视野距离
        kReduceVisualRange =13,
        //无法移动
        kUnmovable = 14,
        //无法施法
        kUnableCast = 15,
        //物理伤害免疫
        kPhysicalDamageImmunity = 16,
        //魔法伤害免疫
        kMagicDamageImmunity = 17,
        //无法攻击
        kUnableAttack = 18,
        //不被选中
        kNotarget = 19,
        //攻击失效
        kAttackMiss = 20,
        //不死
        kDeathless = 21,
        //负面效果免疫
        kNegativeEffectImmunity = 22,
        //清除负面效果
        kCleanse = 23,
    }

    public enum DamageType
    {
        //物理伤害  
        kPhysical = 0,
        //魔法伤害
        kMagic = 1,
        //真实伤害
        kTrue = 2,
    }
    public enum EventCode
    {
        //单位死亡
        UnitDead,
        //即将造成伤害

    }

    public enum TargetFilter
    {
        kSelf = 0,//自己
        kNearestEnemy = 1,//最近的敌人
        kNearestAlly = 2,//最近的盟友
        kLowestHPEnemy = 3,//血量最低的敌人
        kLowestHPAlly = 4,//血量最低的盟友
    }

    public enum TargetRange
    {
        kBattlefield = 0,//整个战场
        kCirclefield = 1,//半径范围内
    }

    public enum RegionType
    {
        kCircle = 0,//圆形区域
        kRect = 1,//矩形区域
        kSector = 2,//扇形区域
    }

    public enum AoeFilter
    {
        kEnemy = 0,
        kAlly = 1,
        kAll = 2,
    }


    public enum BulletType
    {
        kCoordBullet = 0,
        kLineBullet = 1,
        kPenteralBullet = 2,
        kReturnBullet = 3,
        kTrackBullet = 4,
    }
}

