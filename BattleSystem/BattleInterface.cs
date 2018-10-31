
using BattleSystem.ObjectModule;
using BattleSystem.SpaceModule;
using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public class BattleInterface
    {
        public static BattleInterface Instance { get; private set; }


        public BattleInterface()
        {
            if (Instance != null)
                Instance.Destroy();

            Instance = this;
        }
        
        private List<UnitBase> mUnits = new List<UnitBase>();

        private List<UnitBase>[] mClampGroup;


        private List<AoeField> mAoeFields = new List<AoeField>();
        public WorldSpace world { get; private set; }

        public bool InitBattle(int campCount)
        {
            var n_world = new WorldSpace();
            n_world.Init(100, 100, 5, 60);

            return InitBattle(campCount,n_world);
        }
        public bool InitBattle(int campCount,WorldSpace world)
        {
            this.world = world;
            mClampGroup = new List<UnitBase>[campCount];
            for (int i = 0; i < campCount;++i)
            {
                mClampGroup[i] = new List<UnitBase>();
            }
            return true;
        }

        public void Update(float dt)
        {

            for (int i = mAoeFields.Count - 1; i >= 0; i--)
            {
                if(mAoeFields[i].Update(dt))
                {
                    mAoeFields.RemoveAt(i);
                }
            }
            for(int i = mUnits.Count - 1;i >=0;i--)
            {
                if(mUnits[i].Update(dt))
                {
                    mClampGroup[mUnits[i].CampID].Remove(mUnits[i]);
                    mUnits.RemoveAt(i);
                }
            }
        }

        public UnitBase AddUnit(int templateID, int campID, int level)
        {
            UnitBase unit = new UnitBase(world,templateID, campID, level);
            mUnits.Add(unit);
            mClampGroup[campID].Add(unit);
            return unit;
        }

        public void AddUnit(UnitBase unit)
        {
            mUnits.Add(unit);
            if (!mClampGroup[unit.CampID].Contains(unit))
                mClampGroup[unit.CampID].Add(unit);
        }

        public void AddAoeField(AoeField aoe)
        {
            mAoeFields.Add(aoe);
        }

        /// <summary>
        /// 找到血量最低的我方单位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="campID"></param>
        /// <returns></returns>
        public UnitBase GetLowestHPAlly(int id,int campID)
        {
            UnitBase unit = null;
            var Group = mClampGroup[campID];
            for (int i = 0; i < Group.Count; ++i)
            {
                if (unit == null || unit.HP > Group[i].HP)
                {
                    unit = Group[i];
                }
            }
            return unit;
        }

        public UnitBase GetLowestHPEnemy(int campID)
        {
            UnitBase unit = null;
            for (int n = 0; n < mClampGroup.Length;++n)
            {
                if(n != campID)
                {
                    var Group = mClampGroup[n];
                    for (int i = 0; i < Group.Count; ++i)
                    {
                        if (unit == null || unit.HP > Group[i].HP)
                        {
                            unit = Group[i];
                        }
                    }
                }
            }
            return unit;
        }
        public UnitBase GetNearestAlly(float x, float y, int id, int campID)
        {

            UnitBase unit = null;
            float sqr_dis = 0;
            var Group = mClampGroup[campID];
            for (int i = 0; i < Group.Count; ++i)
            {
                if (id != Group[i].ID)
                {
                    float dx = Group[i].position.x - x;
                    float dy = Group[i].position.y - y;
                    var _sqr_dis = dx * dx + dy * dy;
                    if (unit == null || _sqr_dis < sqr_dis)
                    {
                        unit = Group[i];
                        sqr_dis = _sqr_dis;
                    }
                }
            }
            return unit;
        }

        public UnitBase GetNearestEnemy(float x, float y, int id, int campID)
        {

            UnitBase unit = null;
            float sqr_dis = 0;
            for (int n = 0; n < mClampGroup.Length; ++n)
            {
                if (n != campID)
                {
                    var Group = mClampGroup[n];
                    for (int i = 0; i < Group.Count; ++i)
                    {
                        if (id != Group[i].ID)
                        {
                            float dx = Group[i].position.x - x;
                            float dy = Group[i].position.y - y;
                            var _sqr_dis = dx * dx + dy * dy;
                            if (unit == null || _sqr_dis < sqr_dis)
                            {
                                unit = Group[i];
                                sqr_dis = _sqr_dis;
                            }
                        }
                    }
                }
            }
            return unit;
        }

        public void Destroy()
        {
            if (world != null)
            {
                world.Destroy();
                world = null;
            }

            if (Instance == this)
                Instance = null;
        }

    }
}
