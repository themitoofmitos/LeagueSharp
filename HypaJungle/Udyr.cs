﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace HypaJungle
{
    class Udyr :Jungler
    {
        public Udyr()
        {
            setUpSpells();
            setUpItems();
            levelUpSeq = new Spell[] { Q, W, Q, E, Q,R,Q,E,Q,E,R,W,E,W,W,R,W,W};
            buffPriority = 5;
        }

        public override void setUpSpells()
        {
            recall = new Spell(SpellSlot.Recall);
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 0);
            R = new Spell(SpellSlot.R, 0);
        }

        public override void setUpItems()
        {
            #region itemsToBuyList
            buyThings = new List<ItemToShop>
            {
                 new ItemToShop()
                {
                    goldReach = 475,
                    itemsMustHave = new List<int>{},
                    itemIds = new List<int>{1039,2003,2003}
                },
                new ItemToShop()
                {
                    goldReach = 450,
                    itemsMustHave = new List<int>{1039},
                    itemIds = new List<int>{3106}
                },
                new ItemToShop()
                {
                    goldReach = 700,
                    itemsMustHave = new List<int>{3106},
                    itemIds = new List<int>{1080}
                },
                new ItemToShop()
                {
                    goldReach = 350,
                    itemsMustHave = new List<int>{1080},
                    itemIds = new List<int>{1001}
                },
                new ItemToShop()
                {
                    goldReach = 1025,
                    itemsMustHave = new List<int>{1001},
                    itemIds = new List<int>{3154}
                },
                new ItemToShop()
                {
                    goldReach = 800,
                    itemsMustHave = new List<int>{3154},
                    itemIds = new List<int>{1053}
                },
                new ItemToShop()
                {
                    goldReach = 600,
                    itemsMustHave = new List<int>{1053},
                    itemIds = new List<int>{3144}
                },
                new ItemToShop()
                {
                    goldReach = 9999999,
                    itemsMustHave = new List<int>{3144},
                    itemIds = new List<int>{}
                }
            };
            #endregion

            checkItems();
        }

        public override void UseQ(Obj_AI_Minion minion)
        {
            if (Q.IsReady() && ((minion.Health / getDPS(minion) > 1.6f) || player.Level==1))
                Q.Cast();
        }

        public override void UseW(Obj_AI_Minion minion)
        {
            if (W.IsReady() && player.Health<player.MaxHealth*0.6f)
                W.Cast();
        }

        public override void UseE(Obj_AI_Minion minion)
        {
           // if (E.IsReady())
           //    E.Cast();
        }

        public override void UseR(Obj_AI_Minion minion)
        {
            if (R.IsReady() && (minion.Health / getDPS(minion) > 1.6f))
                R.Cast();
        }

        public override void attackMinion(Obj_AI_Minion minion)
        {

            UseQ(minion);
            player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            UseW(minion);
            UseE(minion);
            UseR(minion);
        }

        public override void castWhenNear(JungleCamp camp)
        {

        }

        public override void doWhileRunningIdlin()
        {
            if (player.MaxMana*0.5f < player.Mana && E.IsReady() && player.IsMoving)
                E.Cast();
        }

        public override float getDPS(Obj_AI_Minion minion)
        {
            float dps = 0;
            dps += Q.GetDamage(minion) / Qdata.Cooldown;
            dps += (float)player.GetAutoAttackDamage(minion) * player.AttackSpeedMod;
            dpsFix = dps;
            return (dps==0)?999:dps;
        }

        public override bool canMove()
        {
            return true;
        }
    }
}
