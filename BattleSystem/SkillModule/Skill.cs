
using System.Collections.Generic;
namespace BattleSystem.SkillModule
{
    public enum SkillStatus
    {
        kReady,
        kRunning,
        kCoolDown,
    }
    public class Skill
    {

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public int TemplateID { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 技能消耗值
        /// </summary>
        public int Cost { get; private set; }

        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public float CD { get; private set; }
        protected float mCDTime = 0; 




        /// <summary>
        /// 几段技能
        /// </summary>
        public int Paragraph { get; private set; }

        private int CurParagraph = 0;

        /// <summary>
        /// 技能持续时间
        /// </summary>
        public float[] Duration { get; private set; }


        /// <summary>
        /// 技能等级
        /// </summary>
        public int Level { get;private set; }


        /// <summary>
        /// 是否自动释放(被动技能)
        /// </summary>
        public bool AutoCast { get; private set; }

        /// <summary>
        /// 技能状态
        /// </summary>
        public SkillStatus Status { get; private set; }

        /// <summary>
        /// 构造技能
        /// </summary>
        /// <param name="templateID">技能模板ID</param>
        /// <param name="id">技能ID</param>
        /// <param name="level">技能等级</param>
        public Skill(int templateID,int id,int level = 1)
        {
            TemplateID = templateID;
            ID = id;
            Level = level;
            //通过模板ID获取配置数据
            mAction = SkillManager.LoadSkillAction(templateID);

            var cdScale = 0;
            //初始CD
            mCDTime = CD * cdScale;
            Status = mCDTime == 0 ? SkillStatus.kReady : SkillStatus.kCoolDown;
        }

        /// <summary>
        /// 输入的技能目标参数
        /// </summary>
        private int mInputID = -1;

        /// <summary>
        /// 输入的技能坐标参数X
        /// </summary>
        private float mInputX = 0;
        /// <summary>
        /// 输入的技能坐标参数Y
        /// </summary>
        private float mInputY = 0;


        
        //internal List<>

        /// <summary>
        /// 技能行为
        /// </summary>
        private SkillAction mAction = null;

        /// <summary>
        /// 释放技能
        /// </summary>
        public void Cast()
        {
            if(Status == SkillStatus.kReady)
            {
                mAction.Reset();
                Status = SkillStatus.kRunning;
            }
        }

        /// <summary>
        /// 对单位释放技能
        /// </summary>
        /// <param name="targetID"></param>
        public void Cast(int targetID)
        {
            mInputID = targetID;
            Cast();
        }
        /// <summary>
        /// 对坐标区域释放技能
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Cast(float x,float y)
        {
            mInputX = x;
            mInputY = y;
            Cast();
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        public void Abort()
        {
            if (Status == SkillStatus.kRunning)
            {
                mCDTime = CD;
                Status = SkillStatus.kCoolDown;
            }
        }

        public void Update(float dt)
        {
            switch(Status)
            {
                case SkillStatus.kCoolDown:
                    {
                        mCDTime -= dt;
                        if(mCDTime <= 0)
                        {
                            Status = SkillStatus.kReady;
                        }
                    }
                    break;
                case SkillStatus.kRunning:
                    {
                        if(mAction.Execute(dt))
                        {
                            mCDTime = CD;
                            Status = SkillStatus.kCoolDown;
                        }
                    }
                    break;
            }
        }

    }
}