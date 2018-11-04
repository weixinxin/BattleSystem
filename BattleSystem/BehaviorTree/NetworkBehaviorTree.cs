//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Networking;

namespace BattleSystem.BehaviorTree
{
    public class NetworkBehaviorTree :  IBehaviorTree
    {
        BehaviorTreeExecutor m_BehaviorTreeExecutor;

        public NetworkBehaviorTree()
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

        public System.Random RandomMgr
        {
            get { return null; }
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
            m_BehaviorTreeExecutor.Stop();
        }

       // public MonoBehaviour MonoBehaviour { get { return this; } }
       // public NetworkBehaviour NetworkBehaviour { get { return this; } }

        #endregion
    }
}
