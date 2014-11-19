using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace HypaJungle
{
    class MasterYi : Jungler
    {

        public bool startedMedi = false;

        public MasterYi()
        {
            setUpSpells();
            setUpItems();
            levelUpSeq = new Spell[] { Q, W, E, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W };
            buffPriority = 10;
        }

        public override void setUpSpells()
        {
            recall = new Spell(SpellSlot.Recall);
            Q = new Spell(SpellSlot.Q, 600);
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
                    itemIds = new List<int>{1039,2003}
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
                    goldReach = 1800,
                    itemsMustHave = new List<int>{3144},
                    itemIds = new List<int>{3153}
                },
                new ItemToShop()
                {
                    goldReach = 1337,
                    itemsMustHave = new List<int>{3153},
                    itemIds = new List<int>{3134}
                },
                new ItemToShop()
                {
                    goldReach = 1363,
                    itemsMustHave = new List<int>{3134},
                    itemIds = new List<int>{3142}
                },
                new ItemToShop()
                {
                    goldReach = 1100,
                    itemsMustHave = new List<int>{3086},
                    itemIds = new List<int>{}
                },
                new ItemToShop()
                {
                    goldReach = 1400,
                    itemsMustHave = new List<int>{3087},
                    itemIds = new List<int>{}
                },
            };
            #endregion

            checkItems();
        }

        public override void UseQ(Obj_AI_Minion minion)
        {
            if (Q.IsReady() && minion.Health > Q.GetDamage(minion))
                Q.Cast(minion);
        }

        public override void UseW(Obj_AI_Minion minion)
        {

        }

        public override void UseE(Obj_AI_Minion minion)
        {
            if (E.IsReady() && minion.Health/getDPS(minion)>4)
                E.Cast();
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
            UseR(minion);
        }

        public override void castWhenNear(JungleCamp camp)
        {

        }

        public override void doWhileRunningIdlin()
        {
            if (W.IsReady() && player.Health < player.MaxHealth*0.7f)
            {
                startedMedi = true;
                W.Cast();
            }
        }

        public override float getDPS(Obj_AI_Minion minion)
        {
            float dps = 0;
            dps += Q.GetDamage(minion)/Qdata.Cooldown;
            dps +=(float) player.GetAutoAttackDamage(minion)*player.AttackSpeedMod;
            dpsFix = dps;
            return dps;
        }

        public override bool canMove()
        {
            if (player.HasBuff("Meditate") && player.Health != player.MaxHealth)
            {
                startedMedi = false;
                return false;
            }

            if (startedMedi)
                return false;

           
            return true;
        }
    }
}
