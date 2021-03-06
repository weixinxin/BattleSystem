﻿using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public class ReturnBullet : BulletBase
    {
        NormalMovement Movement;

        float damageScale = 1.0f;
        bool arrive = false;

        /// <summary>
        /// 伤害衰减比例
        /// </summary>
        float decayScale = 0;

        /// <summary>
        /// 穿透子弹
        /// </summary>
        /// <param name="shooter">射手</param>
        /// <param name="width">弹道宽度</param>
        /// <param name="decayScale">伤害衰减系数</param>
        /// <param name="target">终点</param>
        public ReturnBullet(UnitBase shooter, float decayScale, Vector3 target)
        {
            this.arrive = false;
            this.Shooter = shooter;
            this.decayScale = decayScale;
            this.damageScale = 1.0f;
            Movement = new NormalMovement(this);
            Movement.Retarget(target);
        }
        public override bool Update(float dt)
        {

            float x = position.x;
            float y = position.y;
            if (Movement.Update(dt))
            {
                if (!this.arrive)
                {
                    Movement.Retarget(Shooter.position);
                    this.arrive = true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var shift = speed * dt;

                List<UnitBase> res = new List<UnitBase>();
                BattleInterface.Instance.world.SelectRect(x, y, Movement.shift.x, Movement.shift.y, this.radius * 2, shift, res, (obj) => obj.CampID != Shooter.CampID);

                if (res.Count > 0)
                {

                    if(this.decayScale > 0)
                    {
                        res.Sort((a, b) =>
                        {
                            float da_x = a.position.x - x;
                            float da_y = a.position.y - y;
                            float db_x = b.position.x - x;
                            float db_y = b.position.y - y;
                            float da = da_x * da_x + da_y * da_y;
                            float db = db_x * db_x + db_y * db_y;
                            return da.CompareTo(db);
                        });
                        for (int i = 0; i < res.Count; ++i)
                        {
                            //普攻伤害
                            var rd = (int)(damage * this.damageScale);
                            if (rd > 0)
                                res[i].LostHP(rd, Shooter, damageType, isAttack);
                            //给目标上buff
                            if (buffs != null)
                            {
                                for (int n = 0; n < buffs.Length; ++n)
                                {
                                    res[i].AddBuff(buffs[n], Shooter);
                                }
                            }
                            this.damageScale *= (1 - this.decayScale);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < res.Count; ++i)
                        {
                            //普攻伤害
                            if (damage > 0)
                                res[i].LostHP(damage, Shooter, damageType, isAttack);
                            //给目标上buff
                            if (buffs != null)
                            {
                                for (int n = 0; n < buffs.Length; ++n)
                                {
                                    res[i].AddBuff(buffs[n], Shooter);
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
