﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace HypaJungle
{
    class Shyvana :Jungler
    {
       public Shyvana()
        {
            setUpSpells();
            setUpItems();
            levelUpSeq = new Spell[] {W,Q,E,W,W,R,W,E,W,E,R,E,E,Q,Q,R,Q,Q};
            buffPriority = 5;
            gotMana = false;
        }

        public override void setUpSpells()
        {
            recall = new Spell(SpellSlot.Recall);
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 925);
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
                    itemIds = new List<int>{1039,2003,2003,2003,2003,3340}
                },
                new ItemToShop()
                {
                    goldReach = 485,
                    itemsMustHave = new List<int>{1039},
                    itemIds = new List<int>{3106,2003}
                },
                new ItemToShop()
                {
                    goldReach = 775,
                    itemsMustHave = new List<int>{3106},
                    itemIds = new List<int>{1042,1001}
                },
                new ItemToShop()
                {
                    goldReach = 575,
                    itemsMustHave = new List<int>{1042,1001},
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
            if (Q.IsReady())
                Q.Cast();
        }

        public override void UseW(Obj_AI_Minion minion)
        {
            if (W.IsReady())
                W.Cast();
        }

        public override void UseE(Obj_AI_Minion minion)
        {
            if (E.IsReady())
                E.Cast(minion.Position);
        }

        public override void UseR(Obj_AI_Minion minion)
        {

        }

        public override void attackMinion(Obj_AI_Minion minion)
        {
            player.IssueOrder(GameObjectOrder.AttackUnit, minion);
            UseQ(minion);
            UseW(minion);
            UseE(minion);
        }

        public override void castWhenNear(JungleCamp camp)
        {
            if (JungleClearer.focusedCamp != null && E.IsReady())
            {
                float dist = player.Distance(JungleClearer.focusedCamp.Position);
                if (dist < E.Range * 0.8f && dist >200)
                {
                    E.Cast(camp.Position);
                }
            }
        }

        public override void doWhileRunningIdlin()
        {
            if (JungleClearer.focusedCamp != null && E.IsReady())
            {
                float dist = player.Distance(JungleClearer.focusedCamp.Position);
                if (dist/player.MoveSpeed > 8)
                {
                    UseW(null);
                }
            }
        }

        public override float getDPS(Obj_AI_Minion minion)
        {
            float dps = 0;
            if (Q.Level != 0)
                dps += Q.GetDamage(minion) / Qdata.Cooldown;
            if (W.Level != 0)
                dps += W.GetDamage(minion) / Qdata.Cooldown;
            if(E.Level != 0)
                dps +=E.GetDamage(minion) / Qdata.Cooldown;
            dps += (float)player.GetAutoAttackDamage(minion) * player.AttackSpeedMod;
            dpsFix = dps;
            return (dps == 0) ? 999 : dps;
        }

        public override bool canMove()
        {
            return true;
        }
    }
}
