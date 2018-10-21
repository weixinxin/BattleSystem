using BattleSystem.SpaceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSystem.ObjectModule
{
    public abstract class BaseObject
    {
        public Vector3 position;

        private WorldSpace mWorldSpace = null;

        internal GridNode mGridNode = null;
        public BaseObject() { }
        public BaseObject(WorldSpace ws){mWorldSpace = ws;}
        public void UpdatePosition(float x,float y,float z)
        {
            if(float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z) || float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(z))
            {
                Debug.LogErrorFormat("error: {0},{1},{2}",x,y,z);
                return;
            }
            position.x = x;
            position.y = y;
            position.z = z;
            mWorldSpace.UpdateNode(this);
        }
    }
}
