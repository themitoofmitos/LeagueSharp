using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

namespace Yasuo_Sharpino
{
    internal class YasuoSharp
    {

        public const string CharName = "Yasuo";
        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;

        public static Obj_AI_Base target_db;

        public static Menu Config;

        public YasuoSharp()
        {
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;
            Drawing.OnDraw += onDraw;
            Game.OnGameUpdate += OnGameUpdate;

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
        }

        private static void onLoad(EventArgs args)
        {
            Yasuo.setSkillShots();

            Game.PrintChat("Yasuo - SharpSword by DeTuKs");

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
            //Debug
            Config.AddSubMenu(new Menu("Debug", "debug"));
            Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('T', KeyBindType.Press, false));

            Config.AddToMainMenu();
        }

        private static void OnGameUpdate(EventArgs args)
        {
            
            if (Orbwalker.ActiveMode.ToString() == "Combo")
            {
                //Yasuo.gapCloseE(Game.CursorPos.To2D());
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doCombo(target);
            }

            if (Config.Item("db_targ").GetValue<KeyBind>().Active)
            {
                if(Yasuo.E.IsReady())
                    Yasuo.gapCloseE(Game.CursorPos.To2D());
            }
          

        }

        private static void onDraw(EventArgs args)
        {
            if (Yasuo.isDashing())
                Drawing.DrawCircle(Yasuo.getDashEndPos(), 50, Color.Purple);

            Drawing.DrawCircle(Yasuo.Player.Position, 475, Color.Blue);
            Drawing.DrawCircle(Yasuo.test, 47, Color.Blue);

            Drawing.DrawCircle(Game.CursorPos, 47, Color.Blue);
            foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => Yasuo.enemyIsJumpable(enemy)))
            {
                Drawing.DrawCircle(enemy.Position, 60, Color.Green);
            }

        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
           // if (sender.Name.Contains("missile") || sender.Name.Contains("Minion"))
            //    return;

            //Obj_AI_Base objis = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>(sender.NetworkId);
            //Console.WriteLine(sender.Name+" - "+objis.SkinName);
            if (sender is Obj_SpellMissile)
            {
                Obj_SpellMissile missle = (Obj_SpellMissile)sender;
                Console.WriteLine(missle.Name);
                if (Yasuo.Player.Distance(missle.Position) > 420)
                {
                    Yasuo.W.Cast(missle.Position, true);
                }

            }

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
        }

        public static void OnProcessSpell(LeagueSharp.Obj_AI_Base obj, LeagueSharp.GameObjectProcessSpellCastEventArgs arg)
        {
            //if (obj.Name.Contains("Turret") || obj.Name.Contains("Minion"))
              //  return;
          
            //Console.WriteLine(obj.BasicAttack.Name + " -:- " + arg.ToString());
        }


      

    }
}
