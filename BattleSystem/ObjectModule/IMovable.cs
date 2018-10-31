using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public interface IMovable
    {
        Vector3 position
        {
            get;
            set;
        }
        float speed { get; set; }

        float acceleration { get; }
        void UpdatePosition(float x, float y, float z);
    }
}
