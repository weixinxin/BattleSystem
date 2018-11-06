using BattleSystem.BehaviorTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public enum UnitState
    {
        kIdel = 1,
        kAttack = 2,
        kMove = 3,
    }
    public class UnitController
    {
        public UnitBase Unit { get; protected set;}

        public UnitController() { }

        TrackMovement mTrackMovement;
        NormalMovement mNormalMovement;
        UnitState mState = 0;
        const float IdleDuration = 0.1f;

        private float mIdleTime = 0;

        private float mAttackCoolDwon = 0;
        public UnitController(UnitBase unit)
        {
            this.Unit = unit;
            this.TargetUnit = null;
            mTrackMovement = new TrackMovement(unit);
            mNormalMovement = new NormalMovement(unit);
            EnterIdle();
        }

        protected UnitBase TargetUnit { get; protected set; }

        public virtual void GetWithinAttackDistance(UnitBase target)
        {
            this.TargetUnit = target;
            this.mTrackMovement.Retarget(target);
        }
        
        internal BehaviorResult WaitForAttackCD()
        {
            return BehaviorResult.success;
        }

        internal void EnterAttack()
        {
            if(mState != UnitState.kAttack)
            {
                mState = UnitState.kAttack;
                //开始攻击逻辑
            }
            mAttackCoolDwon   = Unit.AttackDuration.value + BattleInterface.Instance.GameTimeElapsed;
        }

        internal BehaviorResult Attack()
        {
            return BehaviorResult.running;
        }
        internal void ExitAttack(BehaviorResult result)
        {
            mState = UnitState.kIdel;
        }


        internal void EnterApproachToAttackTarget()
        {
            mState = UnitState.kMove;
        }

        internal BehaviorResult ApproachToAttackTarget()
        {
            return BehaviorResult.running;
        }
        internal void ExitApproachToAttackTarget(BehaviorResult result)
        {

        }
        internal BehaviorResult SearchEnemy()
        {
            return BehaviorResult.running;
        }


        internal void EnterIdle()
        {
            if(mState != UnitState.kIdel)
            {
                //开始待机
                mState = UnitState.kIdel;
            }
            mIdleTime = IdleDuration + BattleInterface.Instance.GameTimeElapsed;
        }

        internal BehaviorResult Idle()
        {
            if (BattleInterface.Instance.GameTimeElapsed < mIdleTime)
                return BehaviorResult.running;
            else
                return BehaviorResult.success;
        }
        
    }
}
