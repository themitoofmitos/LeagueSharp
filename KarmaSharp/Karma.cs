using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace KarmaSharp
{
    class Karma
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spellbook sBook = Player.Spellbook;

        public static Orbwalking.Orbwalker orbwalker;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 950);
        public static Spell W = new Spell(SpellSlot.W, 675);
        public static Spell E = new Spell(SpellSlot.E, 800);
        public static Spell R = new Spell(SpellSlot.R, 0);

        public static void setSkillShots()
        {
            Q.SetSkillshot(0.5f, 90f, 1800f, true, Prediction.SkillshotType.SkillshotLine);
        }


        public static void doCombo(Obj_AI_Hero target)
        {
            if (KarmaSharp.Config.Item("useQ").GetValue<bool>())
                useQSmart(target, R.IsReady());
            if (KarmaSharp.Config.Item("useW").GetValue<bool>())
                useWSmart(target);
            if (KarmaSharp.Config.Item("useE").GetValue<bool>())
                useESmart();
        }

        public static void doHarass(Obj_AI_Hero target)
        {
            if (KarmaSharp.Config.Item("useQHar").GetValue<bool>())
                useQSmart(target, R.IsReady());
        }

        public static void useQSmart(Obj_AI_Hero target, bool usedR = false)
        {
            if (usedR)
                Q.Range += 210;
            if (!Q.IsReady())
                return;
            Prediction.PredictionOutput predict = Q.GetPrediction(target);
            if (predict.HitChance == Prediction.HitChance.Collision)
            {
               // Console.WriteLine("minions");
                Obj_AI_Base fistCol = predict.CollisionUnitsList.OrderBy(unit => unit.Distance(Player.ServerPosition)).First();
                if (fistCol.Distance(predict.Position) < (210 - fistCol.BoundingRadius / 2))
                {
                   // Console.WriteLine("Casted in minions");
                    useRSmart();
                    Q.Cast(predict.CastPosition);
                    return;
                }
            }
            else if(predict.HitChance != Prediction.HitChance.CantHit && predict.CollisionUnitsList.Count == 0)
            {
               // Console.WriteLine("Casted " + predict.HitChance);
                useRSmart(); 
                Q.Cast(predict.CastPosition);
                return;
            }
            Q.Range = 950;
        }

        public static void useWSmart(Obj_AI_Hero target)
        {
            if (!W.IsReady())
                return;
            if (Player.Distance(target.ServerPosition) < 675)
                W.Cast(target);
        }

        public static void useESmart()
        {
            if (!E.IsReady())
                return;
            E.Cast(Player);
           // foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsAlly && hero.Distance(Player.ServerPosition)< W.Range))
           // {

           // }
        }

        public static bool useRSmart()
        {
            if (!R.IsReady() || !KarmaSharp.Config.Item("useR").GetValue<bool>())
                return false;
            R.Cast();
            return true;
        }

        public static List<Vector2> entitiesAroundTarget(Obj_AI_Hero target)
        {
            List<Obj_AI_Base> minionsAround = MinionManager.GetMinions(target.ServerPosition, 450, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None);
            List<Vector2> entities = MinionManager.GetMinionsPredictedPositions(minionsAround, 0.5f, 90f, 1800f, Player.ServerPosition, 250, true, Prediction.SkillshotType.SkillshotLine, target.ServerPosition);
            return entities;
        }

    }
}
