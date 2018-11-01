using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    /// <summary>
    /// 坐标定位子弹
    /// </summary>
    public  class CoordBullet : BulletBase
    {

        NormalMovement Movement;

        public CoordBullet(UnitBase shooter,Vector3 target)
        {
            Movement = new NormalMovement(this, target);
        }



        public override bool Update(float dt)
        {
            if (Movement.Update(dt))
            {
                //AOE
                if (radius > 0)
                {
                    AoeRegion region = new CircleRegion(BattleInterface.Instance.world, position.x, position.y, radius);
                    AoeField aoe = new AoeField(Shooter, region, duration, interval, emitters);
                    BattleInterface.Instance.AddAoeField(aoe);
                }
                return true;
            }
            return false;
        }
    }
}
