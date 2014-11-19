using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace HypaJungle
{
    abstract class Jungler
    {

        internal class ItemToShop
        {
            public int goldReach;
            public List<int> itemIds;
            public List<int> itemsMustHave;
        }

        private SpellSlot smite = player.GetSpellSlot("SummonerSmite");

        

        public static Obj_AI_Hero player = ObjectManager.Player;

        public static Spellbook sBook = player.Spellbook;

        public SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public Spell Q;//Emp 1470
        public Spell W;
        public Spell E;
        public Spell R;
        public Spell recall;
        public Spell[] levelUpSeq;
        public List<ItemToShop> buyThings;

        public ItemToShop nextItem;

        public float dpsFix = 0;
        public int buffPriority = 7;

        public abstract void setUpSpells();
        public abstract void setUpItems();

        public abstract void UseQ(Obj_AI_Minion minion);
        public abstract void UseW(Obj_AI_Minion minion);
        public abstract void UseE(Obj_AI_Minion minion);
        public abstract void UseR(Obj_AI_Minion minion);

        public abstract void attackMinion(Obj_AI_Minion minion);

        public abstract void doWhileRunningIdlin();


        public abstract float getDPS(Obj_AI_Minion minion);

        public abstract bool canMove();

        public void castSmite(Obj_AI_Base target)
        {
           // smite = player.GetSpellSlot("SummonerSmite");
            if (player.SummonerSpellbook.CanUseSpell(smite) == SpellState.Ready)
                player.SummonerSpellbook.CastSpell(smite, target);
        }

        public static void usePots()
        {
            
        }

        public void levelUp(Obj_AI_Base sender, CustomEvents.Unit.OnLevelUpEventArgs args)
        {
            if (sender.NetworkId == player.NetworkId)
            {
                    sBook.LevelUpSpell(levelUpSeq[args.NewLevel - 1].Slot);
            }
        }

        public bool canKill(JungleCamp camp)
        {
            if (dpsFix == 0 || camp.dps == 0)
                return true;
            float secToKill = camp.health/dpsFix;
            if (secToKill*camp.dps > (player.Health))
                return false;
            return true;
        }

        public void checkItems()
        {
            for (int i = buyThings.Count - 1; i >= 0; i--)
            {
                bool hasThemAll = buyThings[i].itemsMustHave.All(item => Items.HasItem(item));
                if (hasThemAll)
                {
                    nextItem = buyThings[i];
                    return;
                }
            }
        }

        public void buyItems()
        {
            if (inSpwan())
            {
                foreach (var item in nextItem.itemIds)
                {
                    if (!Items.HasItem(item))
                    {
                        Packet.C2S.BuyItem.Encoded(new Packet.C2S.BuyItem.Struct(item, ObjectManager.Player.NetworkId))
                            .Send();
                    }
                }
            }
            checkItems();
        }

        public bool inSpwan()
        {
            Vector3 spawnPos1 = new Vector3(14286f, 14382f, 172f);
            Vector3 spawnPos0 = new Vector3(416f, 468f, 182f);
            if (player.Distance(spawnPos1) < 600 || player.Distance(spawnPos0) < 600)
                return true;
            return false;
        }
        
    }
}
