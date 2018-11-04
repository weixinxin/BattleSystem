using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    /// <summary>
    /// 跟踪子弹
    /// </summary>
    public  class TrackBullet : BulletBase
    {
        UnitBase Target;
        private Vector3 targetPos;

        TrackMovement Movement;
        public TrackBullet(UnitBase shooter, UnitBase target)
        {
            ID = id;
            Shooter = shooter;
            Target = target;
            Movement = new TrackMovement(this, target);
        }


        public override bool Update(float dt)
        {
            if(Movement.Update(dt))
            {
                //普攻伤害
                if (damage > 0)
                    Target.LostHP(damage, Shooter, damageType, isAttack);
                //给目标上buff
                if (buffs != null)
                {
                    for(int i = 0;i< buffs.Length;++i)
                    {
                        Target.AddBuff(buffs[i], Shooter);
                    }
                }
                //AOE
                if (radius > 0)
                {
                    AoeRegion region = new CircleRegion(BattleInterface.Instance.world,position.x,position.y,radius);
                    AoeField aoe = new AoeField(Shooter, region, duration, interval, emitters);
                    BattleInterface.Instance.AddAoeField(aoe);
                }
                return true;
            }
            return false;
        }
    }
}
