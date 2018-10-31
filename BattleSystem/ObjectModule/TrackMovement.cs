using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public class TrackMovement : MovementBase
    {
        IMovable Target;
        public TrackMovement(IMovable owner, IMovable target)
        {
            Owner = owner;
            Target = target;
        }
        public override bool Update(float dt)
        {
            var dis = Target.position - Owner.position;
            if (Owner.acceleration != 0)
                Owner.speed += Owner.acceleration * dt;
            var shift_len = Owner.speed * dt;

            var sqr_length = dis.x * dis.x + dis.y * dis.y;
            if (sqr_length <= shift_len * shift_len)
            {
                Owner.position = Target.position;
                return true;
            }
            else
            {
                var arg = shift_len / (float)Math.Sqrt(sqr_length);
                Owner.position += dis * arg;
                return false;
            }
        }
    }
}
