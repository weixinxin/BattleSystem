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
        public UnitBase Unit { get; protected set; }

        public UnitController() { }

        TrackMovement mTrackMovement;
        NormalMovement mNormalMovement;
        UnitState mState = 0;
        const float IdleDuration = 0.1f;

        private float mIdleTime = 0;

        private float mAttackCoolDwon = 0;
        private float mAttackDuration = 0;
        public UnitController(UnitBase unit)
        {
            this.Unit = unit;
            this.TargetUnit = null;
            mTrackMovement = new TrackMovement(unit);
            mNormalMovement = new NormalMovement(unit);
            EnterIdle();
        }

        protected float getAnimationDuration(string name)
        {
            return 1;
        }
        public UnitBase TargetUnit { get; protected set; }

        public virtual void GetWithinAttackDistance(UnitBase target)
        {
            this.TargetUnit = target;
            this.mTrackMovement.Retarget(target);
        }

        internal BehaviorResult WaitForAttackCD()
        {
            if (mAttackCoolDwon > BattleInterface.Instance.GameTimeElapsed)
            {
                return BehaviorResult.running;
            }
            else
                return BehaviorResult.success;
        }

        internal void EnterAttack()
        {
            if (mState != UnitState.kAttack)
            {
                mState = UnitState.kAttack;
                //开始攻击逻辑
#if DEBUG
                Debug.Log(Unit.ID + " Attack");

#endif
            }
            mAttackDuration = getAnimationDuration("Attack") + BattleInterface.Instance.GameTimeElapsed;
            mAttackCoolDwon = Unit.AttackDuration.value + BattleInterface.Instance.GameTimeElapsed;
        }

        internal BehaviorResult Attack()
        {
            if (BattleInterface.Instance.GameTimeElapsed < mAttackDuration)
                return BehaviorResult.running;
            else
                return BehaviorResult.success;
        }
        internal void ExitAttack(BehaviorResult result)
        {
            mState = UnitState.kIdel;

#if DEBUG
            Debug.Log(Unit.ID + " ExitAttack");
#endif
        }


        internal void EnterApproachToAttackTarget()
        {
            mState = UnitState.kMove;
            mTrackMovement.Retarget(Unit.AttackTarget);
#if DEBUG
                Debug.Log(Unit.ID + " ApproachToAttackTarget");
#endif
        }

        internal BehaviorResult ApproachToAttackTarget()
        {
            if (mTrackMovement.Update(BattleInterface.Instance.deltaTime))
            {
                return BehaviorResult.success;
            }
            else
                return BehaviorResult.running;
        }
        internal void ExitApproachToAttackTarget(BehaviorResult result)
        {

#if DEBUG
            Debug.Log(Unit.ID + " ExitApproachToAttackTarget");
#endif
        }
        internal BehaviorResult SearchEnemy()
        {

#if DEBUG
            Debug.Log(Unit.ID + " SearchEnemy");
#endif
            List<UnitBase> set = new List<UnitBase>();
            BattleInterface.Instance.world.SelectCircle(Unit.position.x, Unit.position.y, Unit.VisualRange.value, set, (obj) => obj.CampID != Unit.CampID);
            float sqr_dis = 0;
            UnitBase unit = null;
            for (int i = 0; i < set.Count; ++i)
            {

                float dx = set[i].position.x - Unit.position.x;
                float dy = set[i].position.y - Unit.position.y;
                var _sqr_dis = dx * dx + dy * dy;
                if (unit == null || _sqr_dis < sqr_dis)
                {
                    unit = set[i];
                    sqr_dis = _sqr_dis;
                }
            }
            if (unit != null)
            {
                this.Unit.AttackTarget = unit;
                return BehaviorResult.success;
            }
            else
                return BehaviorResult.failed;
            
        }


        internal void EnterIdle()
        {
            if (mState != UnitState.kIdel)
            {
                //开始待机
                mState = UnitState.kIdel;
#if DEBUG
                Debug.Log(Unit.ID + " Idle");
#endif
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
