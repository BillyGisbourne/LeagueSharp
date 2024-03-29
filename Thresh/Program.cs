﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace SFTemplate
{
    class Program
    {
        public static string ChampName = "Thresh";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;

        public static Menu SF;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 450);

            //Prediction
            Q.SetSkillshot(0.5f, 60f, 1200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0f, 0f, 150f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.3f, 110f, 1100f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 0f, 0f, false, SkillshotType.SkillshotCircle);

            //Base menu
            SF = new Menu("SF" + ChampName, ChampName, true);
            //Orbwalker and menu
            SF.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(SF.SubMenu("Orbwalker"));
            //Target selector and menu
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            SF.AddSubMenu(ts);
            //Combo menu
            SF.AddSubMenu(new Menu("Combo", "Combo"));
            SF.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            SF.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            SF.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            SF.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            //Exploits
            SF.AddItem(new MenuItem("NFE", "No-Face Exploit").SetValue(true));
            //Make the menu visible
            SF.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; // Add onDraw
            Game.OnGameUpdate += Game_OnGameUpdate; // adds OnGameUpdate (Same as onTick in bol)

            Game.PrintChat("SF" + ChampName + " loaded! By iSnorflake");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (SF.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.LightBlue);
            Utility.DrawCircle(Player.Position, W.Range, Color.LightGreen);
            Utility.DrawCircle(Player.Position, E.Range, Color.Red);
            Utility.DrawCircle(Player.Position, R.Range, Color.Yellow);
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, SF.Item("NFE").GetValue<bool>());
            }
            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target, SF.Item("NFE").GetValue<bool>(), true);
            }
            if (target.IsValidTarget(R.Range) && R.IsReady())
            {
                R.Cast(target, SF.Item("NFE").GetValue<bool>(), true);
            }
        }
    }
}