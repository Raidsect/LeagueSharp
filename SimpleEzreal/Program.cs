using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace SimpleEzreal
{
    class Program
    {
        public static string ChampName = "Ezreal";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q;

        public static Menu RS;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 1050);
            Q.SetSkillshot(0.5f, 70f, 1850f, true, SkillshotType.SkillshotLine);

            //Base menu
            RS = new Menu("Simple" + ChampName, ChampName, true);

            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            RS.AddSubMenu(ts);

            //Hook Keybind
            RS.AddItem(new MenuItem("Mystic", "Mystic Shot").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Exploits
            RS.AddItem(new MenuItem("Packets", "Packets").SetValue(true));

            //Drawings
            RS.AddItem(new MenuItem("Draw Q", "Draw Q").SetValue(true));

            //Make the menu visible
            RS.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat("Simple" + ChampName + " by Raidsect loaded");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {

            if (RS.Item("Mystic").GetValue<KeyBind>().Active)
            {
                MysticShot();
            }

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Program.RS.Item("Draw Q").GetValue<bool>())
                if (Q.Level > 0 && Q.IsReady())
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
        }

        public static void MysticShot()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            var Qpred = Q.GetPrediction(target).Hitchance;

            if (target.IsValidTarget(Q.Range) && Q.IsReady() && Qpred >= HitChance.High)
            {
                Q.Cast(target, RS.Item("Packets").GetValue<bool>());
            }
        }

    }
}
