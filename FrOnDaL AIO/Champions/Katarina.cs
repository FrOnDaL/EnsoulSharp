using System;
using SharpDX;
using EnsoulSharp;
using System.Linq;
using EnsoulSharp.SDK;
using System.Windows.Forms;
using EnsoulSharp.SDK.Utility;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using EnsoulSharp.SDK.MenuUI.Values;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using static FrOnDaL_AIO.Common.Utilities.spellMisc;
using static FrOnDaL_AIO.Common.Utilities.Extensions;

namespace FrOnDaL_AIO.Champions
{
    public class Katarina
    {
        public static void GameOn()
        {
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 725);
            R = new Spell(SpellSlot.R, 550);
            /*TextFont = new Font(Drawing.Direct3DDevice, new FontDescription
            {
                FaceName = "mahomet",
                Height = 14, Weight = FontWeight.Normal,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });*/
            Main = new Menu("Index", "FrOnDaL AIO", true);
            
            var katarina = new Menu(Gamer.CharacterName, Gamer.CharacterName);
            {
                var combo = new Menu("combo", "Combo");
                {
                    combo.Add(new MenuList("cMode", "Combo mode :", new[] { "Q - E", "E - Q" }) { Index = 1 });
                    combo.Add(new MenuBool("q", "Use combo Q"));
                    combo.Add(new MenuBool("w", "Use combo W"));
                    combo.Add(new MenuSeparator("0", "E Settings"));
                    combo.Add(new MenuBool("e", "Use combo E"));
                    combo.Add(new MenuBool("eTur", "Dont E under the turret", false));
                    combo.Add(new MenuBool("edag", "Save E if no daggers", false));
                    combo.Add(new MenuList ("eMode", "E mode :", new[] { "Infront", "Behind", "Logic" }) { Index = 0 });
                    combo.Add(new MenuSeparator("1", "R Settings"));
                    combo.Add(new MenuList ("rMode", "R mode :", new[] { "If x health", "Only if killable", "Never" }) { Index = 1 });
                    combo.Add(new MenuSlider("rHealt", "Enemies health percentage % to use R (-R mode 'If x health' is active-)", 40, 1));
                    combo.Add(new MenuBool("rCancel", "Cancel R if no Enemies"));
                }
                katarina.Add(combo);

                var harass = new Menu("harass", "Harass");
                {
                    harass.Add(new MenuList ("hMode", "Harass mode :", new[] { "Q - E", "E - Q" }) { Index = 1 });
                    harass.Add(new MenuBool("harq", "Use harass Q"));
                    harass.Add(new MenuBool("harqAuto", "Use harass Q auto", false));
                    harass.Add(new MenuBool("harw", "Use harass W", false));
                    harass.Add(new MenuSeparator("0", "E Settings"));
                    harass.Add(new MenuBool("hare", "Use harass E", false));
                    harass.Add(new MenuBool("hareTur", "Dont E under the turret", false));
                    harass.Add(new MenuBool("haredag", "Save E if no daggers", false));
                    harass.Add(new MenuList ("hareMode", "Harass E mode :", new[] { "Infront", "Behind", "Logic" }) { Index = 0 });
                }
                katarina.Add(harass);

                var killsteal = new Menu("killsteal", "Killsteal");
                {
                    killsteal.Add(new MenuBool("ksQ", "Use killsteal Q"));
                    killsteal.Add(new MenuBool("ksE", "Use killsteal E"));
                    killsteal.Add(new MenuBool("ksdag", "Use killsteal E, in enemy dagger range"));
                    killsteal.Add(new MenuBool("ksegap", "Approach with E to kill with Q"));

                }
                katarina.Add(killsteal);

                var laneClear = new Menu("laneClear", "LaneClear");
                {
                    laneClear.Add(new MenuKeyBind("clearOn", "Lane clear (On/Off)", Keys.G, KeyBindType.Toggle,"false"));
                    laneClear.Add(new MenuBool("clearQ", "Use lane clear Q"));
                    laneClear.Add(new MenuBool("clearQlastq", "Use lane clear Q Last Hit"));
                    laneClear.Add(new MenuBool("clearQlastaa", "Q Don't Last Hit in AA Range"));
                    laneClear.Add(new MenuBool("clearW", "Use lane clear W", false));
                    laneClear.Add(new MenuSlider("clearCountW", "W hit x units minions >= x", 3, 1, 6));
                    laneClear.Add(new MenuBool("clearE", "Use lane clear E", false));
                    laneClear.Add(new MenuSlider("clearCountE", "Dagger hit x units minions >= x", 3, 1, 6));
                    laneClear.Add(new MenuBool("cleartur", "Dont E under the turret"));
                }
                katarina.Add(laneClear);

                var jungClear = new Menu("jungClear", "JungClear");
                {
                    jungClear.Add(new MenuBool("jungClearQ", "Use jung clear Q"));
                    jungClear.Add(new MenuBool("jungClearW", "Use jung clear W"));
                    jungClear.Add(new MenuBool("jungClearE", "Use jung clear E"));
                }
                katarina.Add(jungClear);

                var lastHit = new Menu("lasthit", "LastHit");
                {
                    lastHit.Add(new MenuBool("lastQ", "Use last hit Q"));
                    lastHit.Add(new MenuBool("lastaa", "Q Don't Last Hit in AA Range"));
                }
                katarina.Add(lastHit);

                var drawings = new Menu("drawings", "Drawings");
                {
                    drawings.Add(new MenuBool("qdraw", "Draw Q"));
                    drawings.Add(new MenuBool("edraw", "Draw E"));
                    drawings.Add(new MenuBool("rdraw", "Draw R"));
                    drawings.Add(new MenuBool("cleardraw", "Lane clear status draw"));
                    drawings.Add(new MenuBool("dagdraw", "Draw Daggers"));
                }
                katarina.Add(drawings);
            }

            Main.Add(katarina);
            Common.DamageIndicator.DamageIndicator.Attach(Main);
            Main.Attach();
            
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += SpellDraw;
        }

        private static readonly IEnumerable<AIBaseClient> Daggers = ObjectManager.Get<AIBaseClient>().Where(x => x.Name == "HiddenMinion" && x.IsValid && !x.IsDead);
        public static double DaggersPassive(AIBaseClient target)
        {
            double dmg = 0;
            double returnDmg = 0;
            foreach (var daggers in GameObjects.AllGameObjects)
            {
                if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                {
                    if (daggers.Distance(target) < 450)
                    {
                        if (Gamer.Level >= 1 && Gamer.Level < 6)
                        {
                            dmg = 0.55;
                        }
                        if (Gamer.Level >= 6 && Gamer.Level < 11)
                        {
                            dmg = 0.7;
                        }
                        if (Gamer.Level >= 11 && Gamer.Level < 16)
                        {
                            dmg = 0.85;
                        }
                        if (Gamer.Level >= 16)
                        {
                            dmg = 1;
                        }
                        var damage = Gamer.CalculateDamage(target, DamageType.Magical, ((Gamer.TotalMagicalDamage * dmg) + (35 + (Gamer.Level * 12)) + (Gamer.TotalAttackDamage - Gamer.BaseAttackDamage)));
                        returnDmg = damage;
                    }

                }
            }
            return returnDmg;
        }

        private static double RDamage(AIBaseClient target)
        {
            double dmg = 0;
            if (Gamer.Spellbook.GetSpell(SpellSlot.R).Level == 1)
            {
                dmg = 25;
            }
            if (Gamer.Spellbook.GetSpell(SpellSlot.R).Level == 2)
            {
                dmg = 37.5;
            }
            if (Gamer.Spellbook.GetSpell(SpellSlot.R).Level == 3)
            {
                dmg = 50;
            }
            var damage = Gamer.CalculateDamage(target, DamageType.Magical, ((Gamer.TotalMagicalDamage * 0.19) + ((Gamer.TotalAttackDamage - Gamer.BaseAttackDamage) * 0.22) + dmg));
            return damage;

        }


        private static void SpellDraw(EventArgs args)
        {
            if (Gamer.IsDead) return;
            if (Main[Gamer.CharacterName]["drawings"]["qdraw"] && Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Green, 2);
            }
            if (Main[Gamer.CharacterName]["drawings"]["edraw"] && E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Green, 2);
            }
            if (Main[Gamer.CharacterName]["drawings"]["rdraw"] && R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Green, 2);
            }
            if (Main[Gamer.CharacterName]["drawings"]["cleardraw"] && Gamer.IsVisibleOnScreen)
            {
                if (Main[Gamer.CharacterName]["laneClear"]["clearOn"].GetValue<MenuKeyBind>().Active)
                {
                    //RenderFontText(TextFont, "LaneClear: On", Drawing.Width * 0.44f, Drawing.Height * 0.54f, Color.LimeGreen);
                    RenderDrawText("LaneClear: On", Gamer.HPBarPosition + new Vector2(-35, 160), Color.LawnGreen, 15);
                }
                else
                {
                    //RenderFontText(TextFont, "LaneClear: Off", Drawing.Width * 0.44f, Drawing.Height * 0.54f, Color.Red);
                    RenderDrawText("LaneClear: Off", Gamer.HPBarPosition + new Vector2(-35, 160), Color.Red, 15);
                }
            }
            if (Main[Gamer.CharacterName]["drawings"]["dagdraw"])
            {
                foreach (var daggers in GameObjects.AllGameObjects)
                {
                    if (daggers.Name == "HiddenMinion" && daggers.IsValid && !daggers.IsDead && Gamer.IsVisibleOnScreen)
                    {
                        Render.Circle.DrawCircle(daggers.Position, 450, Color.Coral, 2);
                        Render.Circle.DrawCircle(daggers.Position, 150, Color.Coral, 2);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Gamer.IsDead || MenuGUI.IsChatOpen || Gamer.IsRecalling()) return;
            if (Gamer.HasBuff("katarinarsound"))
            {
                Orbwalker.MovementState = false;
                Orbwalker.AttackState = false;
            }
            else
            {
                Orbwalker.MovementState = true;
                Orbwalker.AttackState = true;
            }
            
            switch (Orbwalker.ActiveMode)
            {

                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case OrbwalkerMode.LastHit:
                    LastHit();
                    break;
            }

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo || Orbwalker.ActiveMode != OrbwalkerMode.Harass)
            {
                if (Q.IsReady() && Main[Gamer.CharacterName]["harass"]["harqAuto"])
                {
                    var target = GetBestEnemyHeroTargetInRange(Q.Range);
                    if (!target.IsValidTarget(Q.Range)) return;
                    if (target != null)
                    {
                        Q.CastOnUnit(target);
                    }
                }

                if (Main[Gamer.CharacterName]["killsteal"]["ksdag"])
                {
                    foreach (var daggers in GameObjects.AllGameObjects)
                    {
                        if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                        {
                            var target = GetBestKillableHeroEQ(DamageType.Magical);

                            if (target != null && E.IsReady() && target.Distance(daggers) < 450 && DaggersPassive(target) >= target.Health && target.IsValidTarget(E.Range))
                            {
                                if (Player.HasBuff("katarinarsound"))
                                {
                                    Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                                }
                                E.Cast(target.Position.Extend(daggers.Position, 200));
                            }
                        }
                    }
                }
                if (Q.IsReady() && Main[Gamer.CharacterName]["killsteal"]["ksQ"])
                {
                    var target = GetBestKillableHero(Q, DamageType.Magical);
                    if (target != null && target.IsValidTarget(Q.Range) && Gamer.GetSpellDamage(target, SpellSlot.Q) >= target.Health)
                    {
                        if (Player.HasBuff("katarinarsound"))
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                        }
                        Q.CastOnUnit(target);
                    }
                }
                if (E.IsReady() && Main[Gamer.CharacterName]["killsteal"]["ksE"])
                {
                    var target = GetBestKillableHero(E, DamageType.Magical);
                    if (target != null &&
                        Gamer.GetSpellDamage(target, SpellSlot.E) >= target.Health && target.IsValidTarget(E.Range))
                    {
                        if (Player.HasBuff("katarinarsound"))
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                        }
                        E.CastOnUnit(target);
                    }
                }

                if (!Q.IsReady() || !Main[Gamer.CharacterName]["killsteal"]["ksegap"]) return;
                {
                    var target = GetBestKillableHeroEQ(DamageType.Magical);
                    if (target == null || !(target.Distance(Gamer) > Q.Range) || !(target.Health <= Gamer.GetSpellDamage(target, SpellSlot.Q))) return;
                    foreach (var x in ObjectManager.Get<AIBaseClient>())
                    {
                        if (x.IsDead || !(x.Distance(target) < Q.Range) || !(x.Distance(Gamer) < E.Range)) continue;
                        if (Player.HasBuff("katarinarsoudn"))
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                        }
                        E.Cast(x.Position);
                    }
                }
            }
            

        }

        private static void Combo()
        {
            var target = GetBestEnemyHeroTargetInRange(E.Range);

            if (Gamer.HasBuff("katarinarsound"))
            {
                if (Main[Gamer.CharacterName]["combo"]["rCancel"])
                {
                    if (Gamer.CountEnemyHeroesInRange(R.Range) == 0)
                    {
                        Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                    }
                }

                if (target != null && Gamer.GetSpellDamage(target, SpellSlot.Q) + Gamer.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                {
                    foreach (var daggers in GameObjects.AllGameObjects)
                    {
                        if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid && E.IsReady())
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                            if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range) && E.IsReady())
                            {
                                E.Cast(daggers.Position.Extend(target.Position, 200));
                            }

                            if (daggers.Distance(Gamer) > E.Range)
                            {
                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                            }
                            if (daggers.Distance(target) > 450)
                            {

                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                            }
                        }
                        if (!Daggers.Any() && E.IsReady())
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                            E.Cast(target.Position.Extend(Gamer.Position, -50));
                        }
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                        {
                            Gamer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPosRaw);
                            Q.CastOnUnit(target);
                        }
                    }
                }
            }

            if (!target.IsValidTarget())
            {
                return;
            }

            switch (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("cMode").Index)
            {
                case 0:

                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    if (Q.IsReady() && Main[Gamer.CharacterName]["combo"]["q"] && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            Q.CastOnUnit(target);
                        }
                    }

                    if (E.IsReady() && Main[Gamer.CharacterName]["combo"]["e"] && target.IsValidTarget(E.Range) &&
                        !Q.IsReady())
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            if (Main[Gamer.CharacterName]["combo"]["eTur"] && target.IsUnderEnemyTurret())
                            {
                                return;
                            }
                            foreach (var daggers in GameObjects.AllGameObjects)
                            {
                                if (!Main[Gamer.CharacterName]["combo"]["edag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(Gamer) > E.Range)
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if(Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(target) > 450)
                                        {

                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                    if (!Daggers.Any())
                                    {
                                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, -50));
                                        }
                                        else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, 50));
                                        }
                                        else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                        {
                                            E.Cast(R.IsReady()
                                                ? target.Position.Extend(Gamer.Position, -50)
                                                : target.Position.Extend(Gamer.Position, 50));
                                        }
                                    }
                                }
                                if (Main[Gamer.CharacterName]["combo"]["edag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (W.IsReady() && Main[Gamer.CharacterName]["combo"]["w"] && target.IsValidTarget(W.Range))
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            W.Cast();
                        }
                    }

                    if (R.IsReady() && Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index != 2)
                    {
                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index == 0)
                        {
                            if (target.IsValidTarget(R.Range - 150))
                            {
                                if (target != null && target.HealthPercent <= Main[Gamer.CharacterName]["combo"]["rHealt"].GetValue<MenuSlider>().Value)
                                {
                                    R.Cast();
                                }
                            }
                        }
                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index == 1)
                        {
                            if (target.IsValidTarget(R.Range - 150))
                            {
                                if (target != null && target.Health <= Gamer.GetSpellDamage(target, SpellSlot.Q) + Gamer.GetSpellDamage(target, SpellSlot.E) + DaggersPassive(target) + RDamage(target) * 10)
                                {
                                    if (!Q.IsReady())
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 1:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (E.IsReady() && Main[Gamer.CharacterName]["combo"]["e"] && target.IsValidTarget(E.Range))
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            if (Main[Gamer.CharacterName]["combo"]["eTur"] && target.IsUnderEnemyTurret())
                            {
                                return;
                            }
                            foreach (var daggers in GameObjects.AllGameObjects)
                            {
                                if (!Main[Gamer.CharacterName]["combo"]["edag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(Gamer) > E.Range)
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(target) > 450)
                                        {

                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                    if (!Daggers.Any())
                                    {
                                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, -50));
                                        }
                                        else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, 50));
                                        }
                                        else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                        {
                                            E.Cast(R.IsReady()
                                                ? target.Position.Extend(Gamer.Position, -50)
                                                : target.Position.Extend(Gamer.Position, 50));
                                        }
                                    }
                                }
                                if (Main[Gamer.CharacterName]["combo"]["edag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("eMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (W.IsReady() && Main[Gamer.CharacterName]["combo"]["w"] && target.IsValidTarget(W.Range))
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            W.Cast();
                        }
                    }
                    if (Q.IsReady() && Main[Gamer.CharacterName]["combo"]["q"] && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            if (Gamer.HasBuff("katarinarsound")) return;
                            Q.CastOnUnit(target);
                        }
                    }
                    if (R.IsReady() && Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index != 2)
                    {
                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index == 0)
                        {
                            if (target.IsValidTarget(R.Range - 150))
                            {
                                if (target != null && target.HealthPercent <= Main[Gamer.CharacterName]["combo"]["rHealt"].GetValue<MenuSlider>().Value)
                                {
                                    R.Cast();
                                }
                            }
                        }
                        if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rMode").Index == 1)
                        {
                            if (target.IsValidTarget(R.Range - 150))
                            {
                                if (target != null && target.Health <= Gamer.GetSpellDamage(target, SpellSlot.Q) + Gamer.GetSpellDamage(target, SpellSlot.E) + DaggersPassive(target) + RDamage(target) * 10)
                                {
                                    if (!Q.IsReady())
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }
                    }
                    break;

            }
        }
        private static void Harass()
        {
            var target = GetBestEnemyHeroTargetInRange(E.Range);
            if (!target.IsValidTarget())
            {
                return;
            }

            switch (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hMode").Index)
            {
                case 0:

                    if (!target.IsValidTarget())
                    {
                        return;
                    }

                    if (Q.IsReady() && Main[Gamer.CharacterName]["harass"]["harq"] && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            Q.CastOnUnit(target);
                        }
                    }

                    if (E.IsReady() && Main[Gamer.CharacterName]["harass"]["hare"] && target.IsValidTarget(E.Range) &&
                        !Q.IsReady())
                    {
                        if (target != null)
                        {
                            if (Main[Gamer.CharacterName]["harass"]["hareTur"] && target.IsUnderEnemyTurret())
                            {
                                return;
                            }
                            foreach (var daggers in GameObjects.AllGameObjects)
                            {
                                if (!Main[Gamer.CharacterName]["harass"]["haredag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(Gamer) > E.Range)
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(target) > 450)
                                        {

                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                    if (!Daggers.Any())
                                    {
                                        if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, -50));
                                        }
                                        else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, 50));
                                        }
                                        else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                        {
                                            E.Cast(R.IsReady()
                                                ? target.Position.Extend(Gamer.Position, -50)
                                                : target.Position.Extend(Gamer.Position, 50));
                                        }
                                    }
                                }
                                if (Main[Gamer.CharacterName]["harass"]["haredag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (W.IsReady() && Main[Gamer.CharacterName]["harass"]["harw"] && target.IsValidTarget(W.Range))
                    {
                        if (target != null)
                        {
                            W.Cast();
                        }
                    }
                    break;

                case 1:
                    if (!target.IsValidTarget())
                    {
                        return;
                    }
                    if (E.IsReady() && Main[Gamer.CharacterName]["harass"]["hare"] && target.IsValidTarget(E.Range))
                    {
                        if (target != null)
                        {
                            if (Main[Gamer.CharacterName]["harass"]["hareTur"] && target.IsUnderEnemyTurret())
                            {
                                return;
                            }
                            foreach (var daggers in GameObjects.AllGameObjects)
                            {
                                if (!Main[Gamer.CharacterName]["harass"]["haredag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(Gamer) > E.Range)
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                        if (daggers.Distance(target) > 450)
                                        {

                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                    if (!Daggers.Any())
                                    {
                                        if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, -50));
                                        }
                                        else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                        {
                                            E.Cast(target.Position.Extend(Gamer.Position, 50));
                                        }
                                        else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                        {
                                            E.Cast(R.IsReady()
                                                ? target.Position.Extend(Gamer.Position, -50)
                                                : target.Position.Extend(Gamer.Position, 50));
                                        }
                                    }
                                }
                                if (Main[Gamer.CharacterName]["harass"]["haredag"])
                                {
                                    if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                                    {
                                        if (target.Distance(daggers) < 450 && target.IsValidTarget(E.Range))
                                        {
                                            E.Cast(daggers.Position.Extend(target.Position, 200));
                                        }
                                        else
                                        {
                                            if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 0) //In-front
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, -50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 1)//Behind
                                            {
                                                E.Cast(target.Position.Extend(Gamer.Position, 50));
                                            }
                                            else if (Main[Gamer.CharacterName]["harass"].GetValue<MenuList >("hareMode").Index == 2)//Logic
                                            {
                                                E.Cast(R.IsReady()
                                                    ? target.Position.Extend(Gamer.Position, -50)
                                                    : target.Position.Extend(Gamer.Position, 50));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (W.IsReady() && Main[Gamer.CharacterName]["harass"]["harw"] && target.IsValidTarget(W.Range))
                    {
                        if (target != null)
                        {
                            W.Cast();
                        }
                    }
                    if (Q.IsReady() && Main[Gamer.CharacterName]["harass"]["harq"] && target.IsValidTarget(Q.Range))
                    {
                        if (target != null)
                        {
                            Q.CastOnUnit(target);
                        }
                    }
                    break;

            }
        }
        private static void LaneClear()
        {
            if (!Main[Gamer.CharacterName]["laneClear"]["clearOn"].GetValue<MenuKeyBind>().Active) return;
            if (Q.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearQ"])
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                {
                    if (minion.IsValidTarget(Q.Range) && minion != null && !Main[Gamer.CharacterName]["laneClear"]["clearQlastq"])
                    {
                        Q.CastOnUnit(minion);
                    }
                }
            }
            if (Q.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearQlastq"])
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                {
                    if (minion.Health <= Gamer.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        if (Main[Gamer.CharacterName]["laneClear"]["clearQlastaa"])
                        {
                            if (minion.Distance(Gamer) > 250)
                            {
                                Q.CastOnUnit(minion);
                            }
                        }
                        if (!Main[Gamer.CharacterName]["laneClear"]["clearQlastaa"])
                        {
                            Q.CastOnUnit(minion);
                        }

                    }
                }
            }

            if (E.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearE"])
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
                {
                    if (minion.IsValidTarget(E.Range) && minion != null)
                    {
                        foreach (var daggers in GameObjects.AllGameObjects)
                        {
                            if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                            {
                                if (GameObjects.EnemyMinions.Count(x => x.IsValidTarget(350, false, daggers.Position)) >= Main[Gamer.CharacterName]["laneClear"]["clearCountE"].GetValue<MenuSlider>().Value)
                                {
                                    if (Main[Gamer.CharacterName]["laneClear"]["cleartur"])
                                    {
                                        if (!daggers.Position.IsUnderEnemyTurret())
                                        {
                                            E.Cast(daggers.Position);
                                        }
                                    }
                                    if (!Main[Gamer.CharacterName]["laneClear"]["cleartur"])
                                    {
                                        E.Cast(daggers.Position);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (W.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearW"])
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(300))
                {
                    if (minion.IsValidTarget(W.Range) && minion != null && GetEnemyLaneMinionsTargetsInRange(300).Count >= Main[Gamer.CharacterName]["laneClear"]["clearCountW"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast();
                    }
                }
            }
        }
        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.FirstOrDefault(x => x.IsValidTarget(E.Range));

            if (monster != null && Main[Gamer.CharacterName]["jungClear"]["jungClearQ"] && Q.IsReady())
            {
                if (monster.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(monster);
                }
            }

            if (E.IsReady() && Main[Gamer.CharacterName]["jungClear"]["jungClearE"] && monster.IsValidTarget(E.Range) && !Q.IsReady())
            {
                if (monster != null)
                {
                    foreach (var daggers in GameObjects.AllGameObjects)
                    {
                        if (daggers.Name == "HiddenMinion" && !daggers.IsDead && daggers.IsValid)
                        {
                            if (monster.Distance(daggers) < 450 && monster.IsValidTarget(E.Range))
                            {
                                E.Cast(daggers.Position.Extend(monster.Position, 200));
                            }
                        }
                    }
                }
            }


            if (monster != null && Main[Gamer.CharacterName]["jungClear"]["jungClearW"] && W.IsReady())
            {
                if (monster.IsValidTarget(250f))
                {
                    W.Cast();
                }
            }
        }
        private static void LastHit()
        {
            if (Q.IsReady() && Main[Gamer.CharacterName]["lasthit"]["lastQ"])
            {
                foreach (var minion in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                {
                    if (minion.Health <= Gamer.GetSpellDamage(minion, SpellSlot.Q))
                    {
                        if (Main[Gamer.CharacterName]["lasthit"]["lastaa"])
                        {
                            if (minion.Distance(Gamer) > 250)
                            {
                                Q.CastOnUnit(minion);
                            }
                        }
                        if (!Main[Gamer.CharacterName]["lasthit"]["lastaa"])
                        {
                            Q.CastOnUnit(minion);
                        }

                    }
                }
            }
        }
    }
}
