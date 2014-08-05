using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;

namespace Yasuo_Sharpino
{
    class Yasuo
    {



        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Vector3 test = new Vector3();

        public static Spellbook sBook = Player.Spellbook;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 475);
        public static Spell QEmp = new Spell(SpellSlot.Q, 900);
        public static Spell QCir = new Spell(SpellSlot.Q, 375);
        public static Spell W = new Spell(SpellSlot.W, 400);
        public static Spell E = new Spell(SpellSlot.E, 475);
        public static Spell R = new Spell(SpellSlot.R, 1200);

        public static void setSkillShots()
        {
            Q.SetSkillshot(0.25f, 50f, 1800f, false, Prediction.SkillshotType.SkillshotLine);
            QEmp.SetSkillshot(0.25f, 50f, 1200f, false, Prediction.SkillshotType.SkillshotLine);
            //QCir.SetSkillshot(0.25f, 50f, 1200f, false, Prediction.SkillshotType.SkillshotCircle);
        }

        public static void doCombo(Obj_AI_Hero target)
        {
            if (target == null) return;

            useQSmart(target);
            if (!useESmart(target))
            {
                List<Obj_AI_Hero> ign = new List<Obj_AI_Hero>();
                ign.Add(target);
                gapCloseE(target.Position.To2D(), ign);
            }
        }

       

        public static Vector3 getDashEndPos()
        {
            Vector2 dashPos2 = Player.GetDashInfo().EndPos;
            return new Vector3(dashPos2, Player.Position.Z);
        }

        public static bool isQEmpovered()
        {
            return Player.HasBuff("yasuoq3w", true);
        }

        public static bool isDashing()
        {
            return Player.IsDashing();
        }

        public static bool canCastFarQ()
        {
            return !Player.IsDashing();
        }

        public static bool canCastCircQ()
        {
            return Player.IsDashing();
        }

        private static bool faceMe(Obj_AI_Hero target)
        {
            float angleB = Geometry.AngleBetween(Player.Orientation.To2D(), target.Path[0].To2D());
            //Console.WriteLine("Play: " + Player.Path[0].ToString());
            //Console.WriteLine("Targ: " + target.Path[0].ToString());
            //Drawing.DrawCircle(Player.Path[0], 50, System.Drawing.Color.Blue);
           // Drawing.DrawCircle(target.Path[0], 50, System.Drawing.Color.Red);
           // Console.WriteLine(angleB);

            return (Math.Abs(angleB) > Math.PI / 2);
        }


        public static void useQSmart(Obj_AI_Hero target)
        {
            if (!Q.IsReady())
                return;

            if (isQEmpovered())
            {
                if (canCastFarQ())
                {
                    Prediction.PredictionOutput po = Prediction.GetBestAOEPosition(target, 0.25f, 50f, 1800f, Player.Position, 900f, false, Prediction.SkillshotType.SkillshotLine); //QEmp.GetPrediction(target, true);
                    if (po.HitChance >= Prediction.HitChance.HighHitchance && Player.Distance(po.CastPosition) < 900)
                    {
                        Q.Cast(po.CastPosition, true);
                    }

                }
                else//dashing
                {
                    float trueRange = QCir.Range-10;
                    Vector3 endPos = getDashEndPos();
                    if (Player.Distance(endPos) < 10 && target.Distance(endPos) < trueRange)
                    {
                        QCir.Cast(target.Position, true);
                    }
                }
            }
            else
            {
                if (canCastFarQ())
                {
                    Q.CastIfWillHit(target, 1, true);
                }
                else//dashing
                {
                    float trueRange = QCir.Range-10;
                    Vector3 endPos = getDashEndPos();
                    if (Player.Distance(endPos) < 5 && target.Distance(endPos) < trueRange)
                    {
                        QCir.Cast(target.Position, true);
                    }
                }
            }
        }

        public static bool useESmart(Obj_AI_Hero target,List<Obj_AI_Hero> ignore = null)
        {
            float trueAARange = Player.AttackRange + target.BoundingRadius;
            float trueERange = target.BoundingRadius + E.Range;

            float dist = Player.Distance(target);
            Vector2 dashPos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D() ;
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(dashPos) > dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed - targ_ms) == 0 ? 0.0001f : (Player.MoveSpeed - targ_ms);
            float timeToReach = (dist-trueAARange) / msDif;
            //Console.WriteLine(timeToReach);
            if (dist > trueAARange && dist < trueERange)
            {
                if (timeToReach > 1.7f || timeToReach<0.0f)
                {
                    E.Cast(target, true);
                    return true;
                }
            }
            return false;
        }

        public static void gapCloseE(Vector2 pos, List<Obj_AI_Hero> ignore = null)
        {
            Vector2 pPos = Player.Position.To2D();
            Obj_AI_Base bestEnem = null;
            Vector2 bestLoc = pPos + (Vector2.Normalize(pos - pPos) * (Player.MoveSpeed * 0.35f));
            float bestDist = pos.Distance(bestLoc);
            foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy, ignore)))
            {
                float trueRange = E.Range + enemy.BoundingRadius;
                float distToEnem = Player.Distance(enemy);
                if (distToEnem < trueRange && distToEnem>15)
                {
                    Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.To2D() - pPos) * E.Range);
                    float distE = pos.Distance(posAfterE);
                    if (distE < bestDist)
                    {
                        bestLoc = posAfterE;
                        bestDist = distE;
                        bestEnem = enemy;
                    }
                }
            }
            if (bestEnem != null)
                E.Cast(bestEnem, true);

        }

        public static bool enemyIsJumpable(Obj_AI_Base enemy, List<Obj_AI_Hero> ignore = null)
        {
            if(enemy.IsValid && enemy.IsEnemy && !enemy.IsInvulnerable && !enemy.MagicImmune && !enemy.IsDead)
            {
            
                 if (ignore != null )
                     foreach (Obj_AI_Hero ign in ignore)
                     {
                         if(ign.NetworkId == enemy.NetworkId)
                             return false;
                     }
                   

                 foreach(BuffInstance buff in enemy.Buffs)
                 {
                     if (buff.Name == "YasuoDashWrapper")
                         return false;
                 }
                 return true;
            }
            return false;
        }

        public static float getSpellCastTime(Spell spell)
        {
            return sBook.GetSpell(spell.Slot).SData.SpellCastTime;
        }

        public static float getSpellCastTime(SpellSlot slot)
        {
            return sBook.GetSpell(slot).SData.SpellCastTime;
        }
    }
}
