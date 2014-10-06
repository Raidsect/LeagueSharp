using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace SimpleYasuo
{
    class Program
    {
        public static string ChampName = "Yasuo";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, Q2, W, E, R;

        public static Menu RS;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 465);
            //Q.SetSkillshot(SimpleYasuo.getNewQSpeed(), 15f, float.MaxValue, false, SkillshotType.SkillshotLine);

            Q2 = new Spell(SpellSlot.Q, 1150);
            Q2.SetSkillshot(0.5f, 50f, 1450f, false, SkillshotType.SkillshotLine);

            /*
            QCir = new Spell(SpellSlot.Q, 305);
            QCir.SetSkillshot(0f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            */

            /*
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1200);
            */
            
            
            //Base menu
            RS = new Menu("RS" + ChampName, ChampName, true);

            //Orbwalker and menu
            //RS.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            //Orbwalker = new Orbwalking.Orbwalker(RS.SubMenu("Orbwalker"));

            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            RS.AddSubMenu(ts);

            // Auto Q menu
            //RS.AddItem(new MenuItem("Disable All Q", "Disable All Q").SetValue(true));
            RS.AddItem(new MenuItem("Auto Q", "Auto Q").SetValue(true));
            RS.AddItem(new MenuItem("Auto Emp Q", "Auto Emp Q").SetValue(true));

            //Combo menu
            //RS.AddSubMenu(new Menu("Combo", "Combo"));
            //RS.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            //RS.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Exploits
            RS.AddItem(new MenuItem("Packets", "Packets").SetValue(true));

            //Drawings
            //RS.AddItem(new MenuItem("Disable All", "Disable All").SetValue(false));
            RS.AddItem(new MenuItem("Draw Q", "Draw Q").SetValue(true));
            RS.AddItem(new MenuItem("Draw Emp Q", "Draw Emp Q").SetValue(true));

            //Make the menu visible
            RS.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat("Simple" + ChampName + " by Raidsect loaded");
        }

        public static bool isQEmpowered()
        {
            return Player.HasBuff("yasuoq3w", true);
        }

        public static float getNewQSpeed()
        {
            float a = 0.5f;//s
            float b = 1 / a * Player.AttackSpeedMod;
            return 1 / b;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            /*
            if (RS.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            */
            Q.SetSkillshot(getNewQSpeed(), 15f, float.MaxValue, false, SkillshotType.SkillshotLine);

            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            var Qpred = Q.GetPrediction(target).Hitchance;
            var Q2pred = Q2.GetPrediction(target).Hitchance;
            //if (Program.RS.Item("Disable All Q").GetValue<bool>())
            //    return;

            if (Program.RS.Item("Auto Q").GetValue<bool>())                
                if (target.IsValidTarget(Q.Range) && Q.IsReady() && !isQEmpowered() && Qpred >= HitChance.High)
                {
                    Q.Cast(target, RS.Item("Packets").GetValue<bool>());
                }

            if (Program.RS.Item("Auto Emp Q").GetValue<bool>())
                if (target.IsValidTarget(Q2.Range) && Q2.IsReady() && isQEmpowered() && Q2pred >= HitChance.High)
                {
                    Q2.Cast(target, RS.Item("Packets").GetValue<bool>());
                }

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            //if (Program.RS.Item("Disable All").GetValue<bool>())
            //    return;

            if (Program.RS.Item("Draw Q").GetValue<bool>())
                if (Q.Level > 0 && !isQEmpowered())
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Aqua);

            if (Program.RS.Item("Draw Emp Q").GetValue<bool>())
                if (Q.Level > 0 && isQEmpowered())
                    Utility.DrawCircle(Player.Position, Q2.Range, Color.Aqua);
        }

        

        /*
        public static void Combo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && !isQEmpowered())
            {
                Q.Cast(target, RS.Item("NFE").GetValue<bool>());
            }

            if (target.IsValidTarget(Q2.Range) && Q2.IsReady() && isQEmpowered())
            {
                Q2.Cast(target, RS.Item("NFE").GetValue<bool>());
            }

            /*
            if (target.IsValidTarget(QCir.Range) && QCir.IsReady())
            {
                QCir.Cast(target, RS.Item("NFE").GetValue<bool>());
            }
            /
        }
        */
    }
}
