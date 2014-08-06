using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

/*
 * To DO:
 * Dont use skills on cd
 * Run +q
 * Fix Q far after Dash                  <-- done
 * Run away (not to mouse to safety)
 * Item support
 * Auto level
 * 
 * 
 * 
 * */


namespace Yasuo_Sharpino
{
    internal class YasuoSharp
    {

        public const string CharName = "Yasuo";
        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;

        public static Obj_AI_Base target_db;

        public static Menu Config;

        public static List<Obj_SpellMissile> skillShots = new List<Obj_SpellMissile>();

        public YasuoSharp()
        {
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;
          
        }

        private static void onLoad(EventArgs args)
        {
            Yasuo.setSkillShots();
            Yasuo.point1 = Yasuo.Player.Position;
            Game.PrintChat("Yasuo - SharpSword by DeTuKs");

            try
            {

                Config = new Menu("Yasuo - SharpSwrod", "Yasuo", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                SimpleTs.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
                //SmaetW
                Config.SubMenu("combo").AddItem(new MenuItem("smartW", "Smart W")).SetValue(true);
                //Flee away
                Config.SubMenu("combo").AddItem(new MenuItem("flee", "E away")).SetValue(new KeyBind('X', KeyBindType.Press, false));

                //LastHit
                Config.AddSubMenu(new Menu("LastHit Sharp", "lHit"));
                Config.SubMenu("lHit").AddItem(new MenuItem("useQlh", "Use Q")).SetValue(true);
                Config.SubMenu("lHit").AddItem(new MenuItem("useElh", "Use E")).SetValue(true);
                //LaneClear
                Config.AddSubMenu(new Menu("LaneClear Sharp", "lClear"));
                Config.SubMenu("lClear").AddItem(new MenuItem("useQlc", "Use Q")).SetValue(true);
                Config.SubMenu("lClear").AddItem(new MenuItem("useEmpQHit", "Emp Q Min hit")).SetValue(new Slider(3,6,1));
                Config.SubMenu("lClear").AddItem(new MenuItem("useElc", "Use E")).SetValue(true);
                //Harass
                Config.AddSubMenu(new Menu("Harass Sharp", "harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("harassOn", "Harass enemies")).SetValue(true);
                Config.SubMenu("harass").AddItem(new MenuItem("harQ3Only", "Use only Q3")).SetValue(false);
                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("djTur", "Dont Jump turrets")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("disDraw", "Dissabel drawing")).SetValue(false);

                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('T', KeyBindType.Press, false));

            
                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            }
            catch
            {
                Game.PrintChat("Oops. Something went wrong with Yasuo- Sharpino");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {
          /*  Console.Clear();
            Console.WriteLine("AA; "+Yasuo.Player.IsAutoAttacking);
            Console.WriteLine("Root: "+Yasuo.Player.IsRooted);
            Console.WriteLine("chan: " + Yasuo.Player.IsChanneling);
            if (Config.Item("db_targ").GetValue<KeyBind>().Active)
            {
                Console.Clear();
                Console.WriteLine(Yasuo.isDashing());
            }*/
            
           
            
            if (Orbwalker.ActiveMode.ToString() == "Combo")
            {
                //Console.WriteLine(YasMath.DistanceFromPointToLine(Yasuo.point1.To2D(), Yasuo.point2.To2D(), Yasuo.Player.Position.To2D()));

                //Yasuo.gapCloseE(Game.CursorPos.To2D());
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doCombo(target);
            }

            if (Orbwalker.ActiveMode.ToString() == "Mixed")
            {
               // Console.WriteLine("wdawad");
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doLastHit(target);
                Yasuo.useQSmart(target);
            }

            if (Orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doLaneClear(target);
            }

            if (Config.Item("flee").GetValue<KeyBind>().Active)
            {
                Yasuo.gapCloseE(Game.CursorPos.To2D());
            }
            if (Config.Item("harassOn").GetValue<bool>() && Orbwalker.ActiveMode.ToString() == "None")
            {
                Obj_AI_Hero target = SimpleTs.GetTarget(1000, SimpleTs.DamageType.Physical);
                Yasuo.useQSmart(target, Config.Item("harQ3Only").GetValue<bool>());
            }

            if (Config.Item("smartW").GetValue<bool>())
                foreach (Obj_SpellMissile mis in skillShots)
                {
                    if(mis.IsValid)
                        Yasuo.useWSmart(mis);
                }
        }

        private static void onDraw(EventArgs args)
        {
            if (Config.Item("disDraw").GetValue<bool>())
                return; 
                                if (Yasuo.isDashing())
                Drawing.DrawCircle(Yasuo.getDashEndPos(), 50, Color.Purple);

            Drawing.DrawCircle(Yasuo.Player.Position, 475, Color.Blue);

         //   Drawing.DrawCircle(Yasuo.point1, 66, Color.Orange);
         //   Drawing.DrawCircle(Yasuo.point2, 66, Color.Orange);
        //    Drawing.DrawLine(Yasuo.point1.X, Yasuo.point1.Y, Yasuo.point2.X, Yasuo.point2.Y, 10f, Color.Blue);

           /* foreach (Obj_AI_Turret tur in ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsEnemy && tur.Health > 0))
            {
                Drawing.DrawCircle(tur.Position, tur.CastRange, Color.Blue);
            }

            Drawing.DrawCircle(Yasuo.test, 47, Color.Blue);

            Drawing.DrawCircle(Game.CursorPos, 47, Color.Blue);

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Yasuo.Q.Range + 50);
            foreach (var minion in minions.Where(minion => minion.IsValidTarget(Yasuo.Q.Range)))
            {
                Drawing.DrawCircle(minion.Position, 47, Color.White);
            }

            */foreach (Obj_SpellMissile mis in skillShots)
            {
              

                Drawing.DrawCircle(mis.Position, 47, Color.Orange);
                Drawing.DrawCircle(mis.EndPosition, 100, Color.BlueViolet);
               Drawing.DrawCircle(mis.SpellCaster.Position, Yasuo.Player.BoundingRadius + mis.SData.LineWidth, Color.DarkSalmon);
                Drawing.DrawCircle(mis.StartPosition, 70, Color.Green);
               // Drawing.DrawCircle(mis.SpellCaster.Position, 100, Color.BlueViolet);
                //if(YasMath.interact(mis.EndPosition.To2D(), mis.Position.To2D(), Yasuo.Player.Position.To2D(), Yasuo.Player.BoundingRadius + mis.SData.LineWidth))

               // Drawing.DrawCircle(mis.EndPosition, 47, Color.Orange);
            }

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
           // if (sender.Name.Contains("missile") || sender.Name.Contains("Minion"))
            //    return;
           // if (args is GameObjectProcessSpellCastEventArgs)
              //  Console.WriteLine("itrsdasd");
            //Obj_AI_Base objis = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(sender.NetworkId);
            //Console.WriteLine(sender.Name+" - "+objis.SkinName);


            if (sender is Obj_SpellMissile && sender.IsEnemy)
            {
                Obj_SpellMissile missle = (Obj_SpellMissile)sender;
               // Console.WriteLine(missle.SData.Name);
                    skillShots.Add(missle);
            }



        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            int i = 0;
            foreach (var lho in skillShots)
            {
                if (lho.NetworkId == sender.NetworkId)
                {
                    skillShots.RemoveAt(i);
                    return;
                }
                i++;
            }
        }

        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base obj, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {
            

            if (obj.Name.Contains("Turret") || obj.Name.Contains("Minion"))
                return;

            //Missle mis = new Missle(arg, obj);
            //Console.WriteLine(obj.Name + " - " + arg.SData.Name);
            
          
            //Console.WriteLine(obj.BasicAttack.Name + " -:- " + arg.ToString());
        }


      

    }
}
