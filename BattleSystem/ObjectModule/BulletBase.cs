using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public abstract class BulletBase : IMovable
    {
        private static int s_id = 1;
        protected static int id
        {
            get
            {
                return s_id++;
            }
        }

        protected MovementBase Movement;
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get;protected set; }
        /// <summary>
        /// 发射者
        /// </summary>
        public UnitBase Shooter { get; protected set; }

        /// <summary>
        /// 坐标
        /// </summary>
        public Vector3 position { get; protected set; }


        /// <summary>
        /// 移动速度
        /// </summary>
        public float speed { get; set; }
        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration { get;  set; }

        /// <summary>
        /// 更新子弹状态
        /// </summary>
        /// <param name="dt">帧间隔时间</param>
        /// <returns>是否需要移除子弹</returns>
        public abstract bool Update(float dt);


        /// <summary>
        /// 更新坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public virtual void UpdatePosition(float x, float y, float z)
        {

        }

    }
}
