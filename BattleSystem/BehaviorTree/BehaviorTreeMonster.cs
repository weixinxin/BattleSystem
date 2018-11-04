using System.Collections;
using System.Collections.Generic;

namespace BattleSystem.BehaviorTree
{
    class BehaviorTreeMonster : BehaviorTreeBase
    {
        public MonsterController m_Controller { get; private set; }
        public Monster m_Agent { get; private set; }

        public BehaviorTreeMonster(BattleCampaign bc, Monster monster)
            : base(bc)
        {
            m_Controller = monster.m_Controller as MonsterController;
            m_Agent = monster;

            Blackboard.AddVariable<string>("UnitCommand", "");
            Blackboard.AddVariable<Vector3>("MoveTargetLocation", Vector3.zero);
            Blackboard.AddVariable<bool>("MoveByFindPath", true);//是否寻路
            Blackboard.AddVariable<AICommandCallbackEvent>("MoveCompleteEvent", null);
            Blackboard.AddVariable<bool>("isPlayingAttackAnimation", false);
            Start();
        }

        public override void NotifyMoveTo(Vector3 targetLocation,AICommandCallbackEvent callback)
        {
            Blackboard.SetValue<string>("UnitCommand", AICommand.Moving);
            Blackboard.SetValue<Vector3>("MoveTargetLocation", targetLocation);
            Blackboard.SetValue<AICommandCallbackEvent>("MoveCompleteEvent", callback);
        }

        protected bool IsMatchedCommand(string cmd)
        {
            string cur = Blackboard.GetValue<string>("UnitCommand");
            return cur == cmd;
        }
        protected void ResetMatchedCommand(string cmd)
        {
            if (Blackboard.GetValue<string>("UnitCommand") == cmd)
                Blackboard.SetValue<string>("UnitCommand", "");
        }

        public override void NotifyFireSkill(Skill skill, AICommandCallbackEvent callback = null)
        {

            Blackboard.SetValue<string>("UnitCommand", AICommand.Skill);
            Blackboard.SetValue<Skill>("PlayerFiredSkill", skill);

        }

        protected virtual void Start()
        {
            Root.TargetNode = new BehaviorSelectorLoop()


                 .Add(new BehaviorWithPreconditonNode() { OnPrecondition = m_Agent.IsFrightened }
                    .setActionNode(new BehaviorSequenceNode()
                        .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterFrightened, OnAction = m_Controller.ActionFrightened })
                    )
                )
                .Add(new BehaviorWithPreconditonNode() { OnPrecondition = m_Agent.IsCanNotMovable }
                    .setActionNode(new BehaviorSelectorNode()
                        {
                            OnEnter = delegate()
                            {
                                m_Agent.BattleCampaign.EventHistory.OnCampaignUnitFreeze(m_Agent.EntityID, true);
                            },
                            OnExit = delegate(BehaviorResult result)
                            {
                                m_Agent.BattleCampaign.EventHistory.OnCampaignUnitFreeze(m_Agent.EntityID, false);
                                m_Agent.m_AttackCDElapse = 0;
                            }
                        }
                        .Add(new BehaviorActionNode() { OnPrecondition = m_Agent.IsInStateBuffVertigo, OnAction = m_Controller.ActionStateBuffVertigo })
                        .Add(new BehaviorActionNode() { OnPrecondition = m_Agent.IsInStateBuffBlowFly, OnAction = m_Controller.ActionStateBuffBlowFly })
                    )
                )
                 .Add(new BehaviorWithPreconditonNode() { Desc = "释放主动技能", OnPrecondition = delegate() { return IsMatchedCommand(AICommand.Skill); } }
                    .setActionNode(new BehaviorSequenceNode() { OnExit = delegate(BehaviorResult result) { ResetMatchedCommand(AICommand.Skill); } }
                        .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterActiveSkill, OnAction = m_Controller.ActionActiveSkill, OnExit = m_Controller.ExitSkill })
                    )
                )
                .Add(new BehaviorWithPreconditonNode() 
                    { 
                        Desc = "移动指令",
                        OnPrecondition = delegate() 
                        { 
                            return IsMatchedCommand(AICommand.Moving); 
                        } 
                    }
                    .setActionNode(new BehaviorSequenceNode() { OnExit = delegate(BehaviorResult result) { ResetMatchedCommand(AICommand.Moving); } }
                        .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterMoveToTargetLocation, OnAction = m_Controller.ActionMoveToTargetLocation, OnExit = m_Controller.ExitMove })
                    )
                )
                .Add(new BehaviorWithPreconditonNode() 
                    { 
                        OnPrecondition = delegate() 
                        { 
                            return true; 
                        }
                    }
                    .setActionNode(new BehaviorLoopNode()
                        .setTargetNode(new BehaviorSelectorNode()
                            .Add(new BehaviorSequenceNode() 
                                { 
                                    Desc = "攻击",
                                    OnPrecondition = delegate()
                                    {
                                        return m_Agent.AttackTarget != null && !m_Agent.AttackTarget.isDead &&
                                            (IsPlayingAttackAnimation() || m_Agent.IsTargetEntityInAttackRange(m_Agent.AttackTarget));
                                    }
                                }
                                .Add(new BehaviorActionNode() { OnAction = m_Controller.ActionWaitForAttackCDOnly })
                                .Add(new BehaviorSelectorNode()
                                   // .Add(new BehaviorActionNode() { OnAction = m_Controller.ActionSkill, OnExit = m_Controller.ExitSkill })
                                    .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterActionNormalAttack, OnAction = m_Controller.ActionNormalAttack, OnExit = m_Controller.ExitAttack })
                                )
                                .Add(new BehaviorSequenceNode() 
                                    { 
                                        Desc = "如果攻击目标是主角", 
                                        OnPrecondition = delegate()
                                        { 
                                            return m_Agent.AttackTarget != null && m_Agent.AttackTarget is Hero; 
                                        }
                                    }
                                    .Add(new BehaviorActionNode() { Desc = "搜索视野中有空闲吸引的敌军", OnAction = m_Controller.ActionSearchNearestAttractableEnemy })
                                    .Add(new BehaviorAssignmentNode() 
                                        { 
                                            Desc = "将发现的目标设置为攻击目标",
                                            OnAction = delegate()
                                            { 
                                                m_Agent.AttackTarget = m_Agent.VisionTarget;
                                            }
                                        }
                                    )
                                    .Add(new BehaviorActionNode() { Desc = "通知队伍发现敌军", OnAction = m_Controller.ActionNotifyUnitGroupOnFindEnemy })
                                )
                            )
                            .Add(new BehaviorSequenceNode()
                                {
                                    Desc = "接近攻击目标", OnPrecondition = delegate()
                                    { 
                                        return m_Agent.AttackTarget != null && !m_Agent.AttackTarget.isDead && m_Agent.IsTargetEntityInVisionRange(m_Agent.AttackTarget); 
                                    } 
                                }
                                .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterApproachToAttackTarget, OnAction = m_Controller.ActionApproachToAttackTarget, OnExit = m_Controller.ExitMove })
                            )
                            .Add(new BehaviorSequenceNode() 
                                { 
                                    Desc = "接近强制攻击目标",
                                    OnPrecondition = delegate() 
                                    { 
                                        return m_Agent.ForceAttackTarget != null && !m_Agent.ForceAttackTarget.isDead; 
                                    } 
                                }
                                .Add(new BehaviorActionNode() { OnEnter = m_Controller.EnterApproachToForceAttackTarget, OnAction = m_Controller.ActionApproachToForceAttackTarget, OnExit = m_Controller.ExitMove })
                                .Add(new BehaviorAssignmentNode() 
                                    { 
                                        Desc = "接近强制攻击目标后设置为攻击目标",
                                        OnAction = delegate() 
                                        {
                                            m_Agent.AttackTarget = m_Agent.ForceAttackTarget; m_Agent.ForceAttackTarget = null; 
                                        }
                                    }
                                )
                            )
                            .Add(new BehaviorSequenceNode() { Desc = "发现敌军" }
                                .Add(new BehaviorActionNode() { OnAction = m_Controller.ActionSearchNearestEnemy })
                                .Add(new BehaviorAssignmentNode()
                                    {
                                        Desc = "将发现的目标设置为攻击目标", 
                                        OnAction = delegate()
                                        { 
                                            m_Agent.AttackTarget = m_Agent.VisionTarget;
                                        }
                                    }
                                )
                            )
                            .Add(new BehaviorSequenceNode() 
                                { 
                                    Desc = "队伍视角内发现的敌军",
                                    OnPrecondition = delegate() 
                                    { 
                                        return IsOwnerUnitGroupInFighting();
                                    } 
                                }
                                .Add(new BehaviorActionNode() { Desc = "搜索队伍视角内发现的离自己最近的敌军1", OnAction = m_Controller.ActionSearchUnitGroupFindEnemy })
                                .Add(new BehaviorAssignmentNode() 
                                    { Desc = "将发现的目标设置为攻击目标",
                                        OnAction = delegate() 
                                        { 
                                            m_Agent.ForceAttackTarget = m_Agent.VisionTarget; 
                                        } 
                                    }
                                )
                            )
                            //.add(new behaviorsequencenode() { desc = "设置主角为攻击目标", onprecondition = delegate() { return m_agent.m_ownergroup != null && m_agent.m_ownergroup.isinfighting && m_battlecampaign.playerhero != null && !battlecampaign.s_instance.playerhero.isdead; } }
                            //    .add(new behaviorassignmentnode() { onaction = delegate() { m_agent.forceattacktarget = m_battlecampaign.playerhero; } })
                            //)
                            .Add(new BehaviorParallelNode() { Desc = "待机并侦察", ParallelPolicy = ParallelPolicyType.SuccessedIfOneSuccesseds }
                                .Add(new BehaviorActionNode() { Desc = "待机", OnEnter = m_Controller.EnterIdle, OnAction = m_Controller.ActionIdle })
                                .Add(new BehaviorUntilSuccessNode() { Desc = "直到视野内发现敌军" }
                                    .setTargetNode(new BehaviorTimeDelayNode() { Interval = 0.5f }
                                        .setTargetNode(new BehaviorSelectorNode()
                                            .Add(new BehaviorSequenceNode() 
                                                { 
                                                    Desc = "队伍视角内发现的敌军",
                                                    OnPrecondition = delegate() 
                                                    { 
                                                        return IsOwnerUnitGroupInFighting();
                                                    } 
                                                }
                                                .Add(new BehaviorActionNode() { Desc = "搜索队伍视角内发现的离自己最近的敌军2", OnAction = m_Controller.ActionSearchUnitGroupFindEnemy })
                                                .Add(new BehaviorAssignmentNode() 
                                                    { 
                                                        Desc = "将发现的目标设置为攻击目标", 
                                                        OnAction = delegate() 
                                                        {
                                                            m_Agent.ForceAttackTarget = m_Agent.VisionTarget;
                                                        }
                                                    }
                                                )
                                            )
                                            .Add(new BehaviorSequenceNode() { Desc = "检测到敌军后，转换到接近目标状态" }
                                                .Add(new BehaviorActionNode() { Desc = "搜索视野内离自己最近的敌军", OnAction = m_Controller.ActionSearchNearestEnemy })
                                                .Add(new BehaviorAssignmentNode() 
                                                    { 
                                                        Desc = "将发现的目标设置为攻击目标",
                                                        OnAction = delegate() 
                                                        { 
                                                            m_Agent.AttackTarget = m_Agent.VisionTarget;
                                                        }
                                                    }
                                                )
                                                .Add(new BehaviorActionNode() { Desc = "通知队伍发现敌军", OnAction = m_Controller.ActionNotifyUnitGroupOnFindEnemy })
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                );
            //Exec();
        }

        protected bool IsOwnerUnitGroupInFighting()
        {
            if (m_Agent.m_OwnerGroup != null)
            {
                return m_Agent.m_OwnerGroup.IsInFighting;
            }
            return false;
        }
    }
}
