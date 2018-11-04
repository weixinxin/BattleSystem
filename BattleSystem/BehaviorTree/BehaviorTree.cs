//using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    /// <summary>
    /// 行为树
    /// </summary>
    public abstract class BehaviorTree :  IBehaviorTree
    {
        BehaviorTreeExecutor m_BehaviorTreeExecutor;

        public BehaviorTree()
        {
            m_BehaviorTreeExecutor = new BehaviorTreeExecutor(this);
        }

        #region IBehavior Tree

        public float TickDuration
        {
            get { return m_BehaviorTreeExecutor.TickDuration; }
            set { m_BehaviorTreeExecutor.TickDuration = value; }
        }

        public virtual float GameTime
        {
            get { return 0; }
        }

        public abstract System.Random RandomMgr
        {
            get;
        }

        public BehaviorRootNode Root { get { return m_BehaviorTreeExecutor.Root; } }
        public BehaviorBlackboard Blackboard { get { return m_BehaviorTreeExecutor.Blackboard; } }
        public bool Running { get { return m_BehaviorTreeExecutor.Running; } }

        public void SetPaused(bool pause)
        {
            m_BehaviorTreeExecutor.Paused = pause;
        }

        public void Exec()
        {
           
            m_BehaviorTreeExecutor.Exec();
        }

        public bool ExecImmediately()
        {
            return m_BehaviorTreeExecutor.ExecImmediately();
        }

        public void Stop()
        {
            SetPaused(true);
            m_BehaviorTreeExecutor.Stop();
            
        }

      //  public MonoBehaviour MonoBehaviour { get { return this; } }
      //  public NetworkBehaviour NetworkBehaviour { get { return null; } }

        #endregion
    }
}