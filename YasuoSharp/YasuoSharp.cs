using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
/*
 * To DO:
 * Dont use skills on cd
 * Run +q
 * Fix Q far after Dash                  <-- done
 * Run away (not to mouse to safety)
 * Item support
 * Auto level                            <-- done
 * Overkill minions too
 * 
 * 
 * auto R on killables
 * 
 * add smart minion health pred
 * 
 * R when enemy is almost on ground<-- done
 * 
 * 
 * 
 * */
using SharpDX;
using Color = System.Drawing.Color;


namespace Yasuo_Sharpino
{
    internal class YasuoSharp
    {

        public static Map map;

        public const string CharName = "Yasuo";
        //Orbwalker
        public static Orbwalking.Orbwalker Orbwalker;


        public static Menu Config;

        public static List<Obj_SpellMissile> skillShots = new List<Obj_SpellMissile>();

        public static string lastSpell = "";

        public static int afterDash = 0;

        public static bool canSave = true;
        public static bool canExport = true;
        public static bool canDelete = true;

        public YasuoSharp()
        {
            if (ObjectManager.Player.BaseSkinName != CharName) 
                return;

            map = new Map();
            /* CallBAcks */
            CustomEvents.Game.OnGameLoad += onLoad;

        }

        private static void onLoad(EventArgs args)
        {
            Yasuo.setSkillShots();
            Yasuo.setDashes();
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
                //SmartW
                Config.SubMenu("combo").AddItem(new MenuItem("smartW", "Smart W")).SetValue(true);
                //SmartR
                Config.SubMenu("combo").AddItem(new MenuItem("smartR", "Smart R")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useRHit", "Use R if hit")).SetValue(new Slider(3, 5, 1));
                Config.SubMenu("combo").AddItem(new MenuItem("useRHitTime", "Use R when they land")).SetValue(true);



                Config.SubMenu("combo").AddItem(new MenuItem("useEWall", "use E to safe")).SetValue(true);
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
                Config.SubMenu("harass").AddItem(new MenuItem("harassTower", "Harass under tower")).SetValue(false);
                Config.SubMenu("harass").AddItem(new MenuItem("harassOn", "Harass enemies")).SetValue(true);
                Config.SubMenu("harass").AddItem(new MenuItem("harQ3Only", "Use only Q3")).SetValue(false);
                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("djTur", "Dont Jump turrets")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("disDraw", "Dissabel drawing")).SetValue(false);
                List<string> levStrings = new List<string>();
               // levStrings.Add("None");
               // levStrings.Add("Q E W Q start");
               // levStrings.Add("Q E Q W start");
                Config.SubMenu("extra").AddItem(new MenuItem("autoLevel", "Auto Level")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("levUpSeq", "")).SetValue(new StringList(new string[2] { "Q E W Q start", "Q E Q W start" }));

                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("WWLast", "Print last ww blocked")).SetValue(new KeyBind('T', KeyBindType.Press, false));
                Config.SubMenu("debug").AddItem(new MenuItem("saveDash", "saveDashd")).SetValue(new KeyBind('o', KeyBindType.Press, false));
                Config.SubMenu("debug").AddItem(new MenuItem("exportDash", "export dashes")).SetValue(new KeyBind('p', KeyBindType.Press, false));
                Config.SubMenu("debug").AddItem(new MenuItem("deleteDash", "deleteLastDash")).SetValue(new KeyBind('i', KeyBindType.Press, false));
            
                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnGameUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                CustomEvents.Unit.OnLevelUp += OnLevelUp;

                Game.OnGameSendPacket += OnGameSendPacket;
                Game.OnGameProcessPacket += OnGameProcessPacket;

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

            if (Orbwalker.ActiveMode.ToString() == "LastHit")
            {
                Obj_AI_Hero target = SimpleTs.GetTarget(1250, SimpleTs.DamageType.Physical);
                Yasuo.doLastHit(target);
                Yasuo.useQSmart(target);
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
                Yasuo.fleeToMouse();
            }

            if (Config.Item("saveDash").GetValue<KeyBind>().Active && canSave)
            {
                Yasuo.saveLastDash();
                canSave = false;
            }
            else
            {
                canSave = true;
            }

            if (Config.Item("deleteDash").GetValue<KeyBind>().Active && canDelete)
            {
                if(Yasuo.dashes.Count>0)
                    Yasuo.dashes.RemoveAt(Yasuo.dashes.Count - 1);
                canDelete = false;
            }
            else
            {
                canDelete = true;
            }
            if (Config.Item("exportDash").GetValue<KeyBind>().Active && canExport)
            {
                using (var file = new System.IO.StreamWriter(@"C:\YasuoDashes.txt"))
                {
                    
                    foreach (var dash in Yasuo.dashes)
                    {
                        string dashS = "dashes.Add(new YasDash(new Vector3(" + dash.from.X.ToString("0.00").Replace(',', '.') + "f," + dash.from.Y.ToString("0.00").Replace(',', '.') + "f," + dash.from.Z.ToString("0.00").Replace(',', '.') +
                            "f),new Vector3(" + dash.to.X.ToString("0.00").Replace(',', '.') + "f," + dash.to.Y.ToString("0.00").Replace(',', '.') + "f," + dash.to.Z.ToString("0.00").Replace(',', '.') + "f)));";
                        //new YasDash(new Vector3(X,Y,Z),new Vector3(X,Y,Z))

                        file.WriteLine(dashS);
                    }
                    file.Close();
                }

                canExport = false;
            }
            else
            {
                canExport = true;
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


            Utility.DrawCircle(Yasuo.Player.Position, 475, Color.Blue);
            Utility.DrawCircle(Yasuo.Player.Position, 1200, Color.Blue);


            Utility.DrawCircle(Game.CursorPos, 350, Color.Cyan);

            Utility.DrawCircle(Yasuo.lastDash.from, 60, Color.BlueViolet);
            Utility.DrawCircle(Yasuo.lastDash.to, 60, Color.BlueViolet);

            foreach (Yasuo.YasDash dash in Yasuo.dashes)
            {
                Utility.DrawCircle(dash.from, 60, Color.LawnGreen,3,4);
                Utility.DrawCircle(dash.to, 60, Color.LawnGreen,3,4);
            }

         /*   if ((int)NavMesh.GetCollisionFlags(Game.CursorPos) == 2 || (int)NavMesh.GetCollisionFlags(Game.CursorPos) == 64)
                Drawing.DrawCircle(Game.CursorPos, 70, Color.Green);
            if (map.isWall(Game.CursorPos.To2D()))
                Drawing.DrawCircle(Game.CursorPos, 100, Color.Red);

            foreach (Polygon pol in map.poligs)
            {
                pol.Draw(Color.BlueViolet, 3);
            }

            foreach(Obj_AI_Base jun in MinionManager.GetMinions(Yasuo.Player.ServerPosition,700,MinionTypes.All,MinionTeam.Neutral))
            {
                Drawing.DrawCircle(jun.Position, 70, Color.Green);
                 SharpDX.Vector2 proj = map.getClosestPolygonProj(jun.ServerPosition.To2D());
                 SharpDX.Vector2 posAfterE = jun.ServerPosition.To2D() + (SharpDX.Vector2.Normalize(proj - jun.ServerPosition.To2D() ) * 475);
                 Drawing.DrawCircle(posAfterE.To3D(), 50, Color.Violet);
            }
            
            foreach (Obj_SpellMissile mis in skillShots)
            {
                Drawing.DrawCircle(mis.Position, 47, Color.Orange);
                //Drawing.DrawCircle(mis.EndPosition, 100, Color.BlueViolet);
               //Drawing.DrawCircle(mis.SpellCaster.Position, Yasuo.Player.BoundingRadius + mis.SData.LineWidth, Color.DarkSalmon);
                //Drawing.DrawCircle(mis.StartPosition, 70, Color.Green);
            }*/

        }
        /*
         * 
         * 
         */
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            //wall
            if (sender is Obj_SpellLineMissile)
            {
                Obj_SpellLineMissile missle = (Obj_SpellLineMissile)sender;
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    Yasuo.wall.setL(missle);
                }

                if (missle.SData.Name == "yasuowmovingwallmisr")
                {
                    Yasuo.wall.setR(missle);

                }
                Console.WriteLine(missle.SData.Name);
            }
            if (sender is Obj_SpellMissile && sender.IsEnemy)
            {
                
                Obj_SpellMissile missle = (Obj_SpellMissile)sender;
               // if(Yasuo.WIgnore.Contains(missle.SData.Name))
               //     return;
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

        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {
                if (arg.SData.Name == "YasuoDashWrapper")//start dash
                {
                    Yasuo.lastDash.from = Yasuo.Player.Position;
                    Yasuo.isDashigPro = true;
                    Yasuo.castFrom = Yasuo.Player.Position;
                    Yasuo.startDash = Game.Time;
                }
            }
        }

        public static void OnLevelUp(LeagueSharp.Obj_AI_Base sender, LeagueSharp.Common.CustomEvents.Unit.OnLevelUpEventArgs args)
        {
            if (sender.NetworkId == Yasuo.Player.NetworkId)
            {
                if (!Config.Item("autoLevel").GetValue<bool>())
                    return;
                if (Config.Item("levUpSeq").GetValue<StringList>().SelectedIndex == 0)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq[args.NewLevel-1].Slot);
                else if (Config.Item("levUpSeq").GetValue<StringList>().SelectedIndex == 1)
                    Yasuo.sBook.LevelUpSpell(Yasuo.levelUpSeq2[args.NewLevel - 1].Slot);
            }
        }

       

        private static void OnGameProcessPacket(GamePacketEventArgs args)
        {//28 16 176 ??184
            if (args.PacketData[0] == 41)//135no 100no 183no 34no 101 133 56yesss? 127 41yess
            {
                GamePacket gp = new GamePacket(args.PacketData);

                gp.Position = 1;
                if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                {
                    Yasuo.lastDash.to = Yasuo.Player.Position;
                    Yasuo.isDashigPro = false;
                    Yasuo.time = Game.Time - Yasuo.startDash;
                }
               /* for (int i = 1; i < gp.Size() - 4; i++)
                {
                    gp.Position = i;
                    if (gp.ReadInteger() == Yasuo.Player.NetworkId)
                    {
                        Console.WriteLine("Found: "+i);
                    }
                }

                Console.WriteLine("End dash");
                Yasuo.Q.Cast(Yasuo.Player.Position);*/
            }
        }

        private static void OnGameSendPacket(GamePacketEventArgs args)
        {

        }


    }
}
