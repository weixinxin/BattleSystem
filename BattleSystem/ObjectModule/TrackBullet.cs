using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public  class TrackBullet : BulletBase
    {
        UnitBase Target;
        private Vector3 targetPos;
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
                //爆炸发伤害
                return true;
            }
            return false;
        }
    }
}
