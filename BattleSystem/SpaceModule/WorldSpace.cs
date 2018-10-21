using BattleSystem.ObjectModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSystem.SpaceModule
{
    public class WorldSpace
    {
        private float mWidth;
        private float mHeight;
        private float mGridSize;

        private GridNode[,] mNodes;

        private int mRowCount;
        private int mColCount;
        private int mExpandCount;


        public bool Init(float width, float height, float gridSize, float safeExpand)
        {
            mWidth = width;
            mHeight = height;
            mGridSize = gridSize;
            


            mColCount = (int)Math.Ceiling(width / gridSize);
            mRowCount= (int)Math.Ceiling(height / gridSize);
            mExpandCount = (int)Math.Ceiling(safeExpand / gridSize);
            mNodes = new GridNode[mColCount + mExpandCount * 2, mRowCount + mExpandCount * 2];
            GridNode empty = new GridNode();
            for (int x = 0; x < mColCount + mExpandCount * 2; ++x)
            {
                for (int y = 0; y < mRowCount + mExpandCount * 2; ++y)
                {

                    if (x < mExpandCount || x >= mColCount + mExpandCount || y < mExpandCount || y >= mRowCount + mExpandCount)
                    {
                        mNodes[x, y] = empty;
                    }
                    else
                    {
                        mNodes[x, y] = new GridNode();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 更新物体的坐标和所属格子
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void UpdateNode(BaseObject obj)
        {
            var cur = GetGridNodeByPosition(obj.position.x, obj.position.y);
            if (obj.mGridNode != null && obj.mGridNode != cur)
            {
                obj.mGridNode.Remove(obj);
                cur.Add(obj);
            }
            else
            {
                cur.Add(obj);
            }
            obj.mGridNode = cur;
        }

        /// <summary>
        /// 通过坐标获取所属格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private GridNode GetGridNodeByPosition(float x,float y)
        {
            x = Mathf.Clamp(x, 0, mWidth);
            y = Mathf.Clamp(y, 0, mHeight);

            int ix = (int)Math.Floor(x / mGridSize);
            int iy = (int)Math.Floor(y / mGridSize);

            return mNodes[ix + mExpandCount, iy + mExpandCount];
        }

        //public Stopwatch stopwatch1 = new Stopwatch();

        //public Stopwatch stopwatch2 = new Stopwatch();

        //public Stopwatch stopwatch3 = new Stopwatch();

        //public Stopwatch stopwatch4 = new Stopwatch();
        public void Select(float x, float y, float width, float height, List<BaseObject> resultNodes, Predicate<BaseObject> match)
        {
            float _r = x + width;
            float _t = y + height;
            int left = (int)Math.Floor(x / mGridSize) + 1;
            int right = (int)Math.Floor(_r / mGridSize) - 1;
            int bottom = (int)Math.Floor(y / mGridSize) + 1;
            int top = (int)Math.Floor(_t / mGridSize) - 1;
            //外侧
            if (bottom != top + 2)
            {
                for (int ix = left - 1; ix <= right + 1;++ix)
                {
                    var node = mNodes[ix + mExpandCount, bottom - 1 + mExpandCount];
                    if(!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                    node = mNodes[ix + mExpandCount, top + 1 + mExpandCount];
                    if (!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                }
            }
            else
            {
                for (int ix = left - 1; ix <= right + 1; ++ix)
                {
                    var node = mNodes[ix + mExpandCount, bottom - 1 + mExpandCount];
                    if (!node.isEmpty)
                        node.SelectRect(x, y, _r, _t, resultNodes, match);
                }
            }
            if(bottom <= top)
            {
                if (left != right + 2)
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[left - 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                        node = mNodes[right + 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                    }
                }
                else
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[left - 1 + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectRect(x, y, _r, _t, resultNodes, match);
                    }

                }
            }

            //内部不检测越界
            left = Mathf.Clamp(left, 0, mColCount - 1);
            right = Mathf.Clamp(right, 0, mColCount - 1);
            bottom = Mathf.Clamp(bottom, 0, mRowCount - 1);
            top = Mathf.Clamp(top, 0, mRowCount - 1);
            if (left <= right && bottom <= top)
            {
                for (int ix = left; ix <= right; ++ix)
                {
                    for (int iy = bottom; iy <= top; ++iy)
                    {
                        var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.Select(resultNodes, match);
                    }
                }
            }
        }
        public void Select(float x, float y, float radius, List<BaseObject> resultNodes, Predicate<BaseObject> match)
        {
            float sqr_raduis = radius * radius;
            int left = (int)Math.Floor((x - radius) / mGridSize);
            int right = (int)Math.Floor((x + radius) / mGridSize);
            int bottom = (int)Math.Floor((y - radius) / mGridSize);
            int top = (int)Math.Floor((y + radius) / mGridSize);
            float in_radius = radius * 0.7071f;
            int in_left = (int)Math.Floor((x - in_radius) / mGridSize) + 1;
            int in_right = (int)Math.Floor((x + in_radius) / mGridSize) - 1;
            int in_bottom = (int)Math.Floor((y - in_radius) / mGridSize) + 1;
            int in_top = (int)Math.Floor((y + in_radius) / mGridSize) - 1;
            if(in_left > in_right || in_bottom > in_top)
            {
                //不存在内部区域

                left = Mathf.Clamp(left, 0, mColCount - 1);
                right = Mathf.Clamp(right, 0, mColCount - 1);
                bottom = Mathf.Clamp(bottom, 0, mRowCount - 1);
                top = Mathf.Clamp(top, 0, mRowCount - 1);
                for(int mx = left; mx <= right;++mx)
                {
                    for(int my = bottom;my <= top;++my)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                            node.SelectCircle(x, y, sqr_raduis,resultNodes, match);
                    }
                }

            }
            else
            {
                //存在内部区域

                for (int mx = left; mx <= right; ++mx)
                {
                    for (int my = bottom; my < in_bottom; ++my)
                    {

                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }

                    for (int my = in_top + 1; my <= top; ++my)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }
                }
                for (int my = in_bottom; my <= in_top; ++my)
                {
                    for (int mx = left; mx < in_left; ++mx)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }

                    for (int mx = in_right + 1; mx <= right; ++mx)
                    {
                        var node = mNodes[mx + mExpandCount, my + mExpandCount];
                        if (!node.isEmpty)
                        {
                            node.SelectCircle(x, y, sqr_raduis, resultNodes, match);
                        }
                    }
                }
                //内部不检测越界
                in_left = Mathf.Clamp(in_left, 0, mColCount - 1);
                in_right = Mathf.Clamp(in_right, 0, mColCount - 1);
                in_bottom = Mathf.Clamp(in_bottom, 0, mRowCount - 1);
                in_top = Mathf.Clamp(in_top, 0, mRowCount - 1);
                for (int ix = in_left; ix <= in_right; ++ix)
                {
                    for (int iy = in_bottom; iy <= in_top; ++iy)
                    {
                        var node = mNodes[ix + mExpandCount, iy + mExpandCount];
                        if (!node.isEmpty)
                            node.Select(resultNodes, match);
                    }
                }
            }
        }
    }
}
