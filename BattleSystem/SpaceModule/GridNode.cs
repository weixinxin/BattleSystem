using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSystem.SpaceModule
{
    internal class GridNode
    {
        private List<BaseObject> mList;

        public bool isEmpty { get; private set; }

        public GridNode()
        {
            mList = new List<BaseObject>(4);
            isEmpty = true;
        }
        public void Remove(BaseObject obj)
        {
            if (mList.Contains(obj))
            {
                mList.Remove(obj);
            }
            isEmpty = mList.Count == 0;
        }
        public void Add(BaseObject obj)
        {
            if (!mList.Contains(obj))
            {
                mList.Add(obj);
            }
            isEmpty = mList.Count == 0;
        }

        public void Select(List<BaseObject> resultNodes, Predicate<BaseObject> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                if (match(mList[i]))
                {
                    resultNodes.Add(mList[i]);
                }
            }
        }

        public void SelectRect(float left, float bottom, float right, float top, List<BaseObject> resultNodes, Predicate<BaseObject> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                var obj = mList[i];
                if (obj.position.x >= left && obj.position.x <= right && obj.position.y >= bottom && obj.position.y <= top)
                {
                    if (match(mList[i]))
                    {
                        resultNodes.Add(mList[i]);
                    }
                }
            }
        }


        public void SelectCircle(float x, float y, float sqr_raduis, List<BaseObject> resultNodes, Predicate<BaseObject> match)
        {
            for (int i = 0; i < mList.Count; ++i)
            {
                var obj = mList[i];
                var dis = (obj.position.x - x) * (obj.position.x - x) + (obj.position.y - y) * (obj.position.y - y);
                if (dis <= sqr_raduis)
                {
                    if (match(mList[i]))
                    {
                        resultNodes.Add(mList[i]);
                    }
                }
            }
        }
    }
}
