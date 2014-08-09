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
 * Overkill minions too
 * 
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

        public static string lastSpell = "";

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
                //SmaetR
                Config.SubMenu("combo").AddItem(new MenuItem("smartR", "Smart R")).SetValue(true);
                Config.SubMenu("lClear").AddItem(new MenuItem("useRHit", "Use R if hit")).SetValue(new Slider(3, 5, 1));
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
                Config.SubMenu("debug").AddItem(new MenuItem("WWLast", "Print last ww blocked")).SetValue(new KeyBind('T', KeyBindType.Press, false));

            
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
            if (Orbwalker.ActiveMode.ToString() == "Combo")
            {
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doCombo(target);
            }

            if (Orbwalker.ActiveMode.ToString() == "Mixed")
            {
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

            if (Config.Item("WWLast").GetValue<KeyBind>().Active)
            {
                Console.WriteLine("Last WW skill blocked: " + lastSpell);
                Game.PrintChat("Last WW skill blocked: " + lastSpell);
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
            Drawing.DrawCircle(Yasuo.Player.Position, 475, Color.Blue);

            foreach (Obj_SpellMissile mis in skillShots)
            {
                Drawing.DrawCircle(mis.Position, 47, Color.Orange);
                Drawing.DrawCircle(mis.EndPosition, 100, Color.BlueViolet);
               Drawing.DrawCircle(mis.SpellCaster.Position, Yasuo.Player.BoundingRadius + mis.SData.LineWidth, Color.DarkSalmon);
                Drawing.DrawCircle(mis.StartPosition, 70, Color.Green);
            }

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender is Obj_SpellMissile && sender.IsEnemy)
            {
                Obj_SpellMissile missle = (Obj_SpellMissile)sender;
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
        }

    }
}
