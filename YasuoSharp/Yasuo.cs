using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using YasuoSharp;

namespace Yasuo_Sharpino
{
    class Yasuo
    {
        internal class YasDash
        {
            public Vector3 from = new Vector3(-1,-1,-1);
            public Vector3 to = new Vector3(-1,-1,-1);

            public YasDash()
            {
                from = new Vector3(-1,-1,-1);
                to = new Vector3(-1,-1,-1);
            }

            public YasDash(Vector3 fromV, Vector3 toV)
            {
                from = fromV;
                to = toV;
            }

            public YasDash(YasDash dash)
            {
                from = dash.from;
                to = dash.to;
            }

        }

        internal  class YasWall
        {
            public Obj_SpellLineMissile pointL;
            public Obj_SpellLineMissile pointR;
            public float endtime = 0;
            public YasWall()
            {

            }

            public YasWall(Obj_SpellLineMissile L, Obj_SpellLineMissile R)
            {
                pointL = L;
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setR(Obj_SpellLineMissile R)
            {
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setL(Obj_SpellLineMissile L)
            {
                pointL = L;
                endtime = Game.Time + 4;
            }
        }

        public static List<YasDash> dashes = new List<YasDash>(); 

        public static YasDash lastDash = new YasDash();

        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Vector3 test = new Vector3();

        public static Spellbook sBook = Player.Spellbook;

        public static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public static Spell Q = new Spell(SpellSlot.Q, 475);
        public static Spell QEmp = new Spell(SpellSlot.Q, 900);
        public static Spell QCir = new Spell(SpellSlot.Q, 320);
        public static Spell W = new Spell(SpellSlot.W, 400);
        public static Spell E = new Spell(SpellSlot.E, 475);
        public static Spell R = new Spell(SpellSlot.R, 1200);
        //Much Skillshot                    1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8
        public static Spell[] levelUpSeq = {Q,E,W,Q,Q,R,Q,E,Q,E,R,E,W,E,W,R,W,W};

        //Much NotSoMuch                    1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8
        public static Spell[] levelUpSeq2 = {Q,E,Q,W,Q,R,Q,E,Q,E,R,E,W,E,W,R,W,W};

        //Ignore these spells with W
        public static List<string> WIgnore;

        public static Vector3 point1 = new Vector3();
        public static Vector3 point2 = new Vector3();

        public static Vector3 castFrom;
        public static bool isDashigPro = false;
        public static float startDash = 0;
        public static float time = 0;

        public static YasWall wall = new YasWall();

        public static JungleTimers jTimers;

#region WallDashing

        public static void setDashes()
        {
            #region WallDashingValues
            dashes.Add(new YasDash(new Vector3(5997.00f, 5065.00f, 51.67f), new Vector3(6447.35f, 5216.45f, 56.11f)));
            dashes.Add(new YasDash(new Vector3(6897.00f, 5665.00f, 55.66f), new Vector3(6659.32f, 5285.89f, 58.84f)));
            dashes.Add(new YasDash(new Vector3(3847.00f, 5965.00f, 55.13f), new Vector3(3477.00f, 6263.00f, 55.61f)));
            dashes.Add(new YasDash(new Vector3(3197.00f, 6815.00f, 53.86f), new Vector3(3328.71f, 6366.87f, 55.61f)));
            dashes.Add(new YasDash(new Vector3(6615.00f, 5197.00f, 56.40f), new Vector3(6885.00f, 5761.00f, 55.60f)));
            dashes.Add(new YasDash(new Vector3(3435.00f, 6267.00f, 55.61f), new Vector3(4003.00f, 6007.00f, 54.55f)));
            dashes.Add(new YasDash(new Vector3(3353.00f, 6319.00f, 55.61f), new Vector3(3141.00f, 6745.00f, 53.93f)));
            dashes.Add(new YasDash(new Vector3(6511.00f, 5233.00f, 57.02f), new Vector3(5972.25f, 5084.35f, 51.67f)));
            dashes.Add(new YasDash(new Vector3(5997.00f, 5065.00f, 51.67f), new Vector3(6447.35f, 5216.45f, 56.11f)));
            dashes.Add(new YasDash(new Vector3(6897.00f, 5665.00f, 55.66f), new Vector3(6659.32f, 5285.89f, 58.84f)));
            dashes.Add(new YasDash(new Vector3(3847.00f, 5965.00f, 55.13f), new Vector3(3477.00f, 6263.00f, 55.61f)));
            dashes.Add(new YasDash(new Vector3(3197.00f, 6815.00f, 53.86f), new Vector3(3328.71f, 6366.87f, 55.61f)));
            dashes.Add(new YasDash(new Vector3(6615.00f, 5197.00f, 56.40f), new Vector3(6885.00f, 5761.00f, 55.60f)));
            dashes.Add(new YasDash(new Vector3(3435.00f, 6267.00f, 55.61f), new Vector3(4003.00f, 6007.00f, 54.55f)));
            dashes.Add(new YasDash(new Vector3(3353.00f, 6319.00f, 55.61f), new Vector3(3141.00f, 6745.00f, 53.93f)));
            dashes.Add(new YasDash(new Vector3(6511.00f, 5233.00f, 57.02f), new Vector3(5972.25f, 5084.35f, 51.67f)));
            dashes.Add(new YasDash(new Vector3(7319.51f, 7394.79f, 54.01f), new Vector3(7709.69f, 7649.68f, 53.35f)));
            dashes.Add(new YasDash(new Vector3(3423.00f, 7719.00f, 55.06f), new Vector3(3541.61f, 7204.61f, 54.48f)));
            dashes.Add(new YasDash(new Vector3(3469.00f, 7739.00f, 55.01f), new Vector3(3979.00f, 8101.00f, 51.94f)));
            dashes.Add(new YasDash(new Vector3(3579.00f, 7217.00f, 54.39f), new Vector3(3452.67f, 7586.14f, 55.04f)));
            dashes.Add(new YasDash(new Vector3(4027.00f, 7829.00f, 51.99f), new Vector3(3617.00f, 7589.00f, 54.40f)));
            dashes.Add(new YasDash(new Vector3(1789.00f, 8209.00f, 54.92f), new Vector3(1241.82f, 8198.85f, 50.63f)));
            dashes.Add(new YasDash(new Vector3(1297.00f, 8113.00f, 50.86f), new Vector3(1759.00f, 8223.00f, 54.92f)));
            dashes.Add(new YasDash(new Vector3(7901.00f, 2423.00f, 54.28f), new Vector3(7982.06f, 2994.11f, 55.34f)));
            dashes.Add(new YasDash(new Vector3(8213.00f, 2429.00f, 54.28f), new Vector3(8221.72f, 2912.02f, 57.89f)));
            dashes.Add(new YasDash(new Vector3(8245.00f, 2415.00f, 54.28f), new Vector3(8110.77f, 2953.00f, 56.58f)));
            dashes.Add(new YasDash(new Vector3(7259.00f, 3821.00f, 57.42f), new Vector3(7213.00f, 3965.00f, 57.29f)));
            dashes.Add(new YasDash(new Vector3(7795.00f, 2915.00f, 54.70f), new Vector3(7936.29f, 2465.55f, 54.28f)));
            dashes.Add(new YasDash(new Vector3(7423.00f, 3763.00f, 56.91f), new Vector3(7751.41f, 3288.25f, 54.77f)));
            dashes.Add(new YasDash(new Vector3(7293.00f, 3893.00f, 57.30f), new Vector3(6671.00f, 3883.00f, 51.67f)));
            dashes.Add(new YasDash(new Vector3(12311.00f, 6151.00f, 54.84f), new Vector3(12444.78f, 6731.78f, 54.85f)));
            dashes.Add(new YasDash(new Vector3(12233.00f, 6235.00f, 54.86f), new Vector3(12782.00f, 6383.88f, 56.40f)));
            dashes.Add(new YasDash(new Vector3(12267.00f, 6313.00f, 54.83f), new Vector3(12775.00f, 5951.00f, 58.53f)));
            dashes.Add(new YasDash(new Vector3(10445.00f, 7263.00f, 55.32f), new Vector3(10384.54f, 6816.62f, 54.87f)));
            dashes.Add(new YasDash(new Vector3(10701.00f, 8105.00f, 63.34f), new Vector3(10678.48f, 7568.38f, 55.35f)));
            dashes.Add(new YasDash(new Vector3(10529.00f, 8083.00f, 65.67f), new Vector3(9949.00f, 8319.00f, 64.18f)));
            dashes.Add(new YasDash(new Vector3(10595.00f, 6615.00f, 54.87f), new Vector3(9977.00f, 6511.00f, 55.13f)));
            dashes.Add(new YasDash(new Vector3(10645.00f, 7563.00f, 55.35f), new Vector3(10702.72f, 8030.58f, 64.66f)));
            dashes.Add(new YasDash(new Vector3(12745.00f, 6215.00f, 57.56f), new Vector3(12277.06f, 6270.45f, 54.84f)));
            dashes.Add(new YasDash(new Vector3(10545.00f, 6765.00f, 54.87f), new Vector3(10861.00f, 7303.00f, 55.35f)));
            dashes.Add(new YasDash(new Vector3(10695.00f, 7263.00f, 55.35f), new Vector3(10577.00f, 6803.00f, 54.87f)));
            dashes.Add(new YasDash(new Vector3(7411.00f, 9301.00f, 55.46f), new Vector3(7003.00f, 8827.00f, 56.02f)));
            dashes.Add(new YasDash(new Vector3(7535.00f, 9207.00f, 55.51f), new Vector3(7987.00f, 9637.00f, 53.53f)));
            #endregion
            jTimers = new JungleTimers();
        }

        public static YasDash getClosestDash(float dist = 350)
        {
            YasDash closestWall = dashes[0];
            for(int i=1;i<dashes.Count;i++)
            {
                closestWall = closestDashToMouse(closestWall, dashes[i]);
            }
            if (closestWall.to.Distance(Game.CursorPos) < dist)
                return closestWall;
            return null;
        }

        public static YasDash closestDashToMouse(YasDash w1, YasDash w2)
        {
            return Vector3.DistanceSquared(w1.to, Game.CursorPos) > Vector3.DistanceSquared(w2.to, Game.CursorPos) ? w2 : w1;
        }

        public static void saveLastDash()
        {
            if(lastDash.from.X != -1 && lastDash.from.Y != -1)
                dashes.Add(new YasDash(lastDash));
            lastDash = new YasDash();
        }

        public static void fleeToMouse()
        {
            try
            {
                YasDash closeDash = getClosestDash();
                if (closeDash != null)
                {
                    List<Obj_AI_Base> jumps = canGoThrough(closeDash);
                    if (jumps.Count > 0 || ((W.IsReady() || (Yasuo.wall != null && (Yasuo.wall.endtime-Game.Time)>3f))  && jTimers.closestJCUp(closeDash.to)))
                    {
                        var distToDash = Player.Distance(closeDash.from);

                        if (W.IsReady() && distToDash < 136f && jumps.Count == 0)
                        {
                            W.Cast(closeDash.to);
                        }

                        if (distToDash > 5f)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, closeDash.from);
                            return;
                        }

                        if (distToDash < 6f && jumps.Count > 0)
                        {
                            E.Cast(jumps.First());
                        }
                        return;
                    }
                }
                if (getClosestDash(400)==null)
                   Yasuo.gapCloseE(Game.CursorPos.To2D());
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static List<Obj_AI_Base> canGoThrough(YasDash dash)
        {
            List<Obj_AI_Base> jumps = ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy) && enemy.IsValidTarget(550, true, dash.to)).ToList();
            Console.WriteLine(jumps.Count);
            List<Obj_AI_Base> canBejump = new List<Obj_AI_Base>();
            foreach (var jumpe in jumps)
            {
                if (YasMath.interCir(dash.from.To2D(), dash.to.To2D(), jumpe.Position.To2D(), 25) /*&& jumpe.Distance(dash.to) < Player.Distance(dash.to)*/)
                {
                    canBejump.Add(jumpe);
                }
            }
            return canBejump.OrderBy(jum => Player.Distance(jum)).ToList();
        }


        public static float getLengthTillPos(Vector3 pos)
        {
            return 0;
        }

#endregion

        public static void setSkillShots()
        {
            Q.SetSkillshot(getNewQSpeed(), 50f, float.MaxValue, false, SkillshotType.SkillshotLine);
            QEmp.SetSkillshot(0.1f, 50f, 1200f, false, SkillshotType.SkillshotLine);
            QCir.SetSkillshot(0f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public static float getNewQSpeed()
        {
            float ds = 0.3f;//s
            float a = 1 / ds * Yasuo.Player.AttackSpeedMod;
            return 1 / a;
        }

        public static void doCombo(Obj_AI_Hero target)
        {

            Q.SetSkillshot(getNewQSpeed(), 50f, float.MaxValue, false, SkillshotType.SkillshotLine);
            if (target == null) return;
            useHydra(target);

            if (YasuoSharp.Config.Item("smartR").GetValue<bool>() && R.IsReady())
                useRSmart();
            if (YasuoSharp.Config.Item("smartW").GetValue<bool>()) 
                putWallBehind(target);
            if (YasuoSharp.Config.Item("useEWall").GetValue<bool>()) 
                eBehindWall(target);

            if (!useESmart(target))
            {
                //Console.WriteLine("test");
                List<Obj_AI_Hero> ignore = new List<Obj_AI_Hero>();
                ignore.Add(target);
                gapCloseE(target.Position.To2D(), ignore);
            }

            useQSmart(target);
        }

        public static Vector2 getNextPos(Obj_AI_Hero target)
        {
            Vector2 dashPos = target.Position.To2D();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.To2D();
                Vector2 path = target.Path[0].To2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            return dashPos;
        }

        public static void putWallBehind(Obj_AI_Hero target)
        {
            if (!W.IsReady() || !E.IsReady())
                return;
            Vector2 dashPos = getNextPos(target);
            PredictionOutput po = Prediction.GetPrediction(target, 0.5f);

            float dist = Player.Distance(po.UnitPosition);
            if (!target.IsMoving || Player.Distance(dashPos) <= dist+40)
                if (dist < 330 && dist > 100 && W.IsReady())
                    W.Cast(po.UnitPosition);
        }

        public static void eBehindWall(Obj_AI_Hero target)
        {
            if (!E.IsReady() || !enemyIsJumpable(target) || target.IsMelee())
                return;
            float dist = Player.Distance(target);
            var pPos = Player.Position.To2D();
            Vector2 dashPos = target.Position.To2D();
            if (!target.IsMoving || Player.Distance(dashPos) <= dist)
            {
                foreach ( Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy)))
                {
                    Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.To2D() - pPos) * E.Range);
                    if ((target.Distance(posAfterE) < dist 
                        || target.Distance(posAfterE) < Orbwalking.GetRealAutoAttackRange(target)+100)
                        && goesThroughWall(target.Position, posAfterE.To3D()))
                    {
                        useENormal(enemy);
                    }
                }
            }
            
        }

        public static bool goesThroughWall(Vector3 vec1, Vector3 vec2)
        {
            if (wall.endtime < Game.Time)
                return false;
            Vector2 inter = YasMath.LineIntersectionPoint(vec1.To2D(), vec2.To2D(), wall.pointL.Position.To2D(), wall.pointR.Position.To2D());
            float wallW = (300 + 50 * W.Level);
            if (wall.pointL.Position.To2D().Distance(inter) > wallW ||
                wall.pointR.Position.To2D().Distance(inter) > wallW)
                return false;
            var dist = vec1.Distance(vec2);
            if (vec1.To2D().Distance(inter) + vec2.To2D().Distance(inter)-30 > dist)
                return false;

            return true;
        }

        public static void doLastHit(Obj_AI_Hero target)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + 50);
            foreach ( var minion in minions.Where( minion => minion.IsValidTarget(Q.Range)))
            {
                if (Player.Distance(minion) < Orbwalking.GetRealAutoAttackRange(minion) && minion.Health < DamageLib.CalcPhysicalMinionDmg((double)(Player.BaseAttackDamage + Player.FlatPhysicalDamageMod), (Obj_AI_Minion)minion, true))
                    return;
                if (YasuoSharp.Config.Item("useElh").GetValue<bool>() && minion.Health < DamageLib.getDmg(minion, DamageLib.SpellType.E))
                    useENormal(minion);

                if (YasuoSharp.Config.Item("useQlh").GetValue<bool>() && !isQEmpovered() && HealthPrediction.LaneClearHealthPrediction(minion,(int)(getNewQSpeed()*1000))  < DamageLib.getDmg(minion, DamageLib.SpellType.Q))
                    if (!(target != null && isQEmpovered() && Player.Distance(target) < 1050))
                    {
                        if (canCastFarQ())
                            Q.Cast(minion);
                    }
            }
        }

        public static void doLaneClear(Obj_AI_Hero target)
        {
            List<Obj_AI_Base> minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 800,MinionTypes.All,MinionTeam.NotAlly);
            if (YasuoSharp.Config.Item("useElc").GetValue<bool>())
                foreach (var minion in minions.Where(minion => minion.IsValidTarget(E.Range)))
                {
                    if (minion.Health < DamageLib.getDmg(minion, DamageLib.SpellType.E)
                        || Q.IsReady() && minion.Health < (DamageLib.getDmg(minion, DamageLib.SpellType.E) + DamageLib.getDmg(minion, DamageLib.SpellType.Q)))
                        
                    {
                        useENormal(minion);
                    }
                }

            if (Q.IsReady()  && YasuoSharp.Config.Item("useQlc").GetValue<bool>())
            {
                if (isQEmpovered() && !(target != null && Player.Distance(target) < 1050))
                {
                    if (canCastFarQ())
                    {
                        List<Vector2> minionPs = MinionManager.GetMinionsPredictedPositions(minions, 0.25f, 50f, 1200f, Player.ServerPosition, 900f, false, SkillshotType.SkillshotLine);
                        MinionManager.FarmLocation farm = QEmp.GetLineFarmLocation(minionPs); //MinionManager.GetBestLineFarmLocation(minionPs, 50f, 900f);
                        if (farm.MinionsHit >= YasuoSharp.Config.Item("useEmpQHit").GetValue<Slider>().Value)
                        {
                           // Console.WriteLine("Cast q simp Emp");
                            QEmp.Cast(farm.Position, false);
                            return;
                        }
                    }
                    else
                    {
                        if (minions.Count(min => min.IsValid && min.Distance(getDashEndPos()) < 250) >= YasuoSharp.Config.Item("useEmpQHit").GetValue<Slider>().Value)
                        {
                              QCir.Cast(Player.Position, false);
                        }
                    }
                }
                else
                {
                    if (canCastFarQ())
                    {
                        List<Vector2> minionPs = MinionManager.GetMinionsPredictedPositions(minions, 0.25f, 30f, 1800f, Player.ServerPosition, 475, false, SkillshotType.SkillshotLine);
                        Vector2 clos = Geometry.Closest(Player.ServerPosition.To2D(), minionPs);
                        if (Player.Distance(clos) < 475)
                        {
                           // Console.WriteLine("Cast q simp");
                            
                            Q.Cast(clos, false);
                            return;
                        }
                    }
                    else
                    {
                        if (minions.Count(min => !min.IsDead && min.Distance(getDashEndPos())<250) > 1)
                        {
                            QCir.Cast(Player.Position, false);
                           // Console.WriteLine("Cast q circ simp");
                            return;
                        }
                    }
                }
            }
            
        }

        public static void doHarass(Obj_AI_Hero target)
        {
            if (!inTowerRange(Player.ServerPosition.To2D()) || YasuoSharp.Config.Item("harassTower").GetValue<bool>())
             useQSmart(target);
        }


        public static void doFastGetTo()
        {
            List<Obj_AI_Base> jumpies = ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy)).ToList();
            
        }

        public static void useHydra(Obj_AI_Base target)
        {
            
            if ((Items.CanUseItem(3074) || Items.CanUseItem(3074)) && target.Distance(Player.ServerPosition) < (400 + target.BoundingRadius - 20))
            {
                Items.UseItem(3074, target);
                Items.UseItem(3077, target);
            }
        }


        public static bool inTowerRange(Vector2 pos)
        {
          //  if (!YasuoSharp.Config.Item("djTur").GetValue<bool>())
         //      return false;
            foreach (Obj_AI_Turret tur in ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsEnemy && tur.Health > 0))
            {
                if (pos.Distance(tur.Position.To2D()) < (850+Player.BoundingRadius))
                    return true;
            }
            return false;
        }

        public static Vector3 getDashEndPos()
        {
            Vector2 dashPos2 = Player.GetDashInfo().EndPos;
            return new Vector3(dashPos2, Player.ServerPosition.Z);
        }

        public static bool isQEmpovered()
        {
            return Player.HasBuff("yasuoq3w", true);
        }

        public static bool isDashing()
        {
            return isDashigPro;
        }

        public static bool canCastFarQ()
        {
            return !isDashigPro;
        }

        public static bool canCastCircQ()
        {
            return isDashigPro;
        }


        public static List<Obj_AI_Hero> getKockUpEnemies(ref float lessKnockTime)
        {
            List<Obj_AI_Hero> enemKonck = new List<Obj_AI_Hero>();
            foreach (Obj_AI_Hero enem in ObjectManager.Get<Obj_AI_Hero>().Where(enem => enem.IsEnemy))
            {
                foreach(BuffInstance buff in enem.Buffs)
                {
                    if (buff.Type == BuffType.Knockback || buff.Type == BuffType.Knockup)
                    {
                        if (buff.Type == BuffType.Knockup)
                            lessKnockTime = (buff.EndTime - Game.Time) < lessKnockTime
                                ? (buff.EndTime - Game.Time)
                                : lessKnockTime;
                        enemKonck.Add(enem);
                        break;
                    }
                }
            }
            if (!YasuoSharp.Config.Item("useRHitTime").GetValue<bool>())
                lessKnockTime = 0;
            return enemKonck;
        }


        public static void setUpWall()
        {
            if (wall == null)
                return;

        }

        public static void useQSmart(Obj_AI_Hero target,bool onlyEmp = false)
        {
            if (!Q.IsReady())
                return;
            if (isQEmpovered())
            {
                if (canCastFarQ())
                {
                    PredictionOutput po = QEmp.GetPrediction(target); //QEmp.GetPrediction(target, true);
                    if (po.Hitchance >= HitChance.Medium && Player.Distance(po.CastPosition) < 900)
                    {
                        QEmp.Cast(po.CastPosition);
                    }
                }
                else//dashing
                {
                    float trueRange = QCir.Range-10;
                    Vector3 endPos = getDashEndPos();
                    if (Player.Distance(endPos) < 10 && target.Distance(endPos) < 270)
                    {
                        QCir.Cast(target.Position);
                    }
                } 
            }
            else if (!onlyEmp)
            {
                if (canCastFarQ() )
                {
                    PredictionOutput po = Q.GetPrediction(target);
                    if (po.Hitchance >= HitChance.Medium)
                        Q.Cast(po.CastPosition);

                }
                else//dashing
                {
                    float trueRange = QCir.Range-10;
                    Vector3 endPos = getDashEndPos();
                    if (Player.Distance(endPos) < 5 && target.Distance(endPos) < 270)
                    {
                        QCir.Cast(target.Position);
                    }
                }
            }
        }

        public static void useWSmart(Obj_SpellMissile missle)
        {
            if (!W.IsReady())
                return;
            if (missle.SpellCaster is Obj_AI_Hero && missle.IsEnemy)
            {
                Obj_AI_Base enemHero = missle.SpellCaster;
                float dmg = (enemHero.BaseAttackDamage + enemHero.FlatPhysicalDamageMod);
                if (missle.SData.Name.Contains("Crit"))
                    dmg *= 2;
                if (!missle.SData.Name.Contains("Attack") || (enemHero.CombatType == GameObjectCombatType.Ranged && dmg > Player.MaxHealth / 8))
                {
                    if (missle.Target.IsMe || (YasMath.DistanceFromPointToLine(missle.SpellCaster.Position.To2D(), missle.EndPosition.To2D(), Yasuo.Player.ServerPosition.To2D()) < (Player.BoundingRadius + missle.SData.LineWidth)))
                    {
                        Vector3 blockWhere = missle.Position;//Player.ServerPosition + Vector3.Normalize(missle.Position - Player.ServerPosition)*30; // missle.Position; 
                        if (Player.Distance(missle.Position) < 420)
                        {
                            if (missle.Target.IsMe || isMissileCommingAtMe(missle))
                            {
                                Console.WriteLine(missle.BoundingRadius);
                                Console.WriteLine(missle.SData.LineWidth);
                                YasuoSharp.lastSpell = missle.SData.Name;
                                W.Cast(blockWhere, true);
                                YasuoSharp.skillShots.Clear();
                            }
                        }
                    }
                }
            }
        }


        public static void useENormal(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            //Console.WriteLine("gapcloseer?");
            Vector2 pPos = Player.ServerPosition.To2D();
            Vector2 posAfterE = pPos + (Vector2.Normalize(target.Position.To2D() - pPos) * E.Range);
            if ((!inTowerRange(posAfterE) || !YasuoSharp.Config.Item("djTur").GetValue<bool>()))
                E.Cast(target, false);
        }

        public static bool useESmart(Obj_AI_Hero target,List<Obj_AI_Hero> ignore = null)
        {
            if (!E.IsReady())
                return false;
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
                  //  Console.WriteLine("test2");
                    useENormal(target);
                    return true;
                }
            }
            return false;
        }

        public static void gapCloseE(Vector2 pos, List<Obj_AI_Hero> ignore = null)
        {
            if (!E.IsReady())
                return;

           // Console.WriteLine("gapcloseer?");
           //Player.IssueOrder(GameObjectOrder.MoveTo, pos.To3D());
            Vector2 pPos = Player.ServerPosition.To2D();
            Obj_AI_Base bestEnem = null;
            //Check if can go through wall
           // foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy, ignore)))
           // {
//
            //}

            
            float distToPos = Player.Distance(pos);
            if (((distToPos < Q.Range )) &&
                goesThroughWall(pos.To3D(), Player.Position))
                return;
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
                useENormal(bestEnem);

        }


        public static void useRSmart()
        {
            float timeToLand = float.MaxValue;
            List<Obj_AI_Hero> enemInAir = getKockUpEnemies(ref timeToLand);
            foreach (Obj_AI_Hero enem in enemInAir)
            {
                int aroundAir = 0;
                foreach (Obj_AI_Hero enem2 in enemInAir)
                {
                    if (Vector3.DistanceSquared(enem.ServerPosition, enem2.ServerPosition) < 400 * 400)
                        aroundAir++;

                }
                if (aroundAir >= YasuoSharp.Config.Item("useRHit").GetValue<Slider>().Value && timeToLand<0.1f)
                    R.Cast(enem);
            }
        }


        public static void setWIgnore()
        {
            WIgnore.Add("LuxPrismaticWaveMissile");

        }



        public static bool isMissileCommingAtMe(Obj_SpellMissile missle)
        {
            Vector3 step =missle.StartPosition+ Vector3.Normalize(missle.StartPosition - missle.EndPosition) * 10;
            return (!(Player.Distance(step) < Player.Distance(missle.StartPosition)));
        }

        public static bool enemyIsJumpable(Obj_AI_Base enemy, List<Obj_AI_Hero> ignore = null)
        {
            //Console.WriteLine("gapcloseerawfawfaw23125?");
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
