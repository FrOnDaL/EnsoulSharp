using System;
using EnsoulSharp;
using System.Linq;
using EnsoulSharp.SDK;
using System.Windows.Forms;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.Prediction;
using Color = System.Drawing.Color;
using EnsoulSharp.SDK.MenuUI.Values;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using FrOnDaL_AIO.Common.Utilities.SPrediction;
using static FrOnDaL_AIO.Common.Utilities.spellMisc;
using static FrOnDaL_AIO.Common.Utilities.Extensions;

namespace FrOnDaL_AIO.Champions
{
    public class Ashe
    {
        public static void GameOn()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 1250f);
            W2 = new Spell(SpellSlot.W, 1250f);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(250, 50, 1500, true, SkillshotType.Line);
            W2.SetSkillshot(0.70f, 125f, 1500, true, SkillshotType.Circle);
            R.SetSkillshot(250, 90, 1600, true, SkillshotType.Line);

            Main = new Menu("Index", "FrOnDaL AIO", true);

            var ashe = new Menu(Gamer.CharacterName, Gamer.CharacterName);
            {
                var combo = new Menu("combo", "Combo");
                {
                    combo.Add(new MenuList ("qBa", "Q attack", new[] { "After attack use combo Q", "Before attack use combo Q", "Normal use combo Q" }) { Index = 1 });
                    combo.Add(new MenuBool("w", "Use combo W"));
                    combo.Add(new MenuSeparator("0", "R Settings"));
                    var whiteListR = new Menu("whiteListR", "R white list");
                    {
                        foreach (var enemies in GameObjects.EnemyHeroes)
                        {
                            whiteListR.Add(new MenuBool("rWhiteList" + enemies.CharacterName.ToLower(), enemies.CharacterName));
                        }
                    }
                    combo.Add(whiteListR);
                    combo.Add(new MenuKeyBind("keyR", "Semi-manual cast R key", Keys.T, KeyBindType.Press));
                    combo.Add(new MenuList ("rHit", "R HitChance", new[] { "High", "Medium", "Low" }) { Index = 2 });
                }
                ashe.Add(combo);
                var harass = new Menu("harass", "Harass");
                {
                    harass.Add(new MenuBool("harassW", "Use Harass W (Key C)"));
                    harass.Add(new MenuBool("autoHarassW", "Use auto harass W", false));
                    harass.Add(new MenuSlider("harassM", "Min mana percentage % to use W", 60, 1));
                }
                ashe.Add(harass);

                var laneClear = new Menu("laneClear", "LaneClear");
                {
                    laneClear.Add(new MenuBool("clearQ", "Use lane clear Q"));
                    laneClear.Add(new MenuSlider("clearCountQ", "Q hit x units minions >= x", 3, 1, 6));
                    laneClear.Add(new MenuBool("clearW", "Use lane clear W"));
                    laneClear.Add(new MenuSlider("clearCountW", "W hit x units minions >= x", 3, 1, 3));
                    laneClear.Add(new MenuSlider("clearM", "Min mana percentage % to use Q and W", 60, 1));
                    laneClear.Add(new MenuSliderButton("clearLvl", "Lane clear open level >= x", 8, 1, 18));
                }
                ashe.Add(laneClear);

                var jungClear = new Menu("jungClear", "JungClear");
                {
                    jungClear.Add(new MenuBool("jungClearQ", "Use jung clear Q"));
                    jungClear.Add(new MenuBool("jungClearW", "Use jung clear Clear W"));
                    jungClear.Add(new MenuSlider("jungClearM", "Min mana percentage % to use Q and W", 20, 1));
                }
                ashe.Add(jungClear);

                var gapcloser = new Menu("gapcloser", "Anti-gapcloser");
                {
                    gapcloser.Add(new MenuBool("gapR", "Use R anti gap closer"));
                }
                ashe.Add(gapcloser);

                var interrupt = new Menu("interrupt", "Interrupt Dangerous Spells");
                {
                    interrupt.Add(new MenuBool("interruptR", "Use R interrupt"));
                }
                ashe.Add(interrupt);

                var drawings = new Menu("drawings", "Drawings");
                {
                    drawings.Add(new MenuBool("wdraw", "Draw W"));
                }
                ashe.Add(drawings);
            }
            Main.Add(ashe);
            Common.DamageIndicator.DamageIndicator.Attach(Main);
            Main.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += SpellDraw;
            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterrupterSpell += DangerousSpellsInterupt;
        }
        
        private static void SpellDraw(EventArgs args)
        {
            if (Gamer.IsDead) return;
            if (Main[Gamer.CharacterName]["drawings"]["wdraw"] && W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Green, 2);
            }
        }

        
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Gamer.IsDead || MenuGUI.IsChatOpen || Gamer.IsRecalling()) return;
            switch (Orbwalker.ActiveMode)
            {

                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    if (Main[Gamer.CharacterName]["laneClear"]["clearLvl"].GetValue<MenuSliderButton>().Enabled)
                    {
                        if (Gamer.Level >= Main[Gamer.CharacterName]["laneClear"]["clearLvl"].GetValue<MenuSliderButton>().Value)
                        {
                            LaneClear();
                        }
                    }
                    else
                    {
                        LaneClear();
                    }
                    JungleClear();
                    break;
            }

            if (Main[Gamer.CharacterName]["harass"]["autoHarassW"] && W.IsReady() && Gamer.ManaPercent >= Main[Gamer.CharacterName]["harass"]["harassM"].GetValue<MenuSlider>().Value && GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(W.Range - 50)))
            {
                var target = GetBestEnemyHeroTargetInRange(W.Range - 50);
                var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                if (wPred.CollisionObjects.Count == 0 && wPred.Hitchance >= HitChance.Medium)
                {
                    W.Cast(target.Position);
                }
            }

            if (Main[Gamer.CharacterName]["combo"]["keyR"].GetValue<MenuKeyBind>().Active && R.IsReady() && GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(R.Range)))
            {
                var target = GetBestEnemyHeroTargetInRange(R.Range);

                
                var rPred = R.GetPrediction(target, false, 0, CollisionObjects.Heroes | CollisionObjects.YasuoWall);
                if (Main[Gamer.CharacterName]["combo"]["whiteListR"]["rWhiteList" + target.CharacterName.ToLower()].GetValue<MenuBool>().Enabled && rPred.Hitchance >= RHitChance() && rPred.CollisionObjects.Count == 0)
                {
                    R.Cast(rPred.CastPosition);
                }
            }
        }

        private static void Combo()
        {
            if (Q.IsReady())
            {
                if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("qBa").Index == 0 || Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("qBa").Index == 1)
                {
                    Orbwalker.OnAction += AttackAction;
                }
                else
                {
                    var target = GetBestEnemyHeroTargetInRange(Gamer.GetRealAutoAttackRange() - 40);
                    if (!target.IsValidTarget(Gamer.GetRealAutoAttackRange() - 40)) return;
                    if (target == null) return;
                    Q.Cast();
                }
                
            }
            
            if (Main[Gamer.CharacterName]["combo"]["w"] && W.IsReady() && Gamer.Mana - 50 > (R.IsReady() ? 150 : 50) && GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(W.Range - 50)))
            {
                var target = GetBestEnemyHeroTargetInRange(W.Range - 50);
                var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                if (wPred.CollisionObjects.Count == 0)
                {
                    W.Cast(target.Position);
                }
            }
        }

        private static void AttackAction(object sender, OrbwalkerActionArgs attack)
        {
            var target = GetBestEnemyHeroTargetInRange(Gamer.GetRealAutoAttackRange() - 40);
            if (!target.IsValidTarget(Gamer.GetRealAutoAttackRange() - 40)) return;
            if (target == null) return;
            if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("qBa").Index == 0 && attack.Type == OrbwalkerType.AfterAttack)
            {
                Q.Cast();
            }
            else if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("qBa").Index == 1 && attack.Type == OrbwalkerType.BeforeAttack)
            {
                Q.Cast();
            }


        }
        private static void Harass()
        {
            if (Main[Gamer.CharacterName]["harass"]["harassW"] && W.IsReady() && Gamer.ManaPercent >= Main[Gamer.CharacterName]["harass"]["harassM"].GetValue<MenuSlider>().Value && GameObjects.EnemyHeroes.Any(x => x.IsValidTarget(W.Range - 50)))
            {
                var target = GetBestEnemyHeroTargetInRange(W.Range - 50);
                var wPred = W.GetPrediction(target, false, 0, CollisionObjects.Minions | CollisionObjects.YasuoWall);
                if (wPred.CollisionObjects.Count == 0 && wPred.Hitchance >= HitChance.Low)
                {
                    W.Cast(target.Position);
                }
            }
        }
        private static void LaneClear()
        {
            if (W.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearW"] && Gamer.ManaPercent >= Main[Gamer.CharacterName]["laneClear"]["clearM"].GetValue<MenuSlider>().Value)
            {
                var wPredMinion = W2.GetCircularFarmLocation(MinionManager.GetMinions(Gamer.Position, W2.Range), 170);

                if (wPredMinion.MinionsHit == 0)
                {
                    foreach (var target in GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range)))
                    {
                        if (target != null)
                        {
                            if (GameObjects.EnemyMinions.Count(x => x.IsValidTarget(170, x.IsLaneMinion, W.GetPrediction(target).CastPosition)) >= Main[Gamer.CharacterName]["laneClear"]["clearCountW"].GetValue<MenuSlider>().Value)
                            {
                                W.Cast(W.GetPrediction(target).CastPosition);
                            }
                        }
                    }
                }
                else
                {
                    if (wPredMinion.MinionsHit >= Main[Gamer.CharacterName]["laneClear"]["clearCountW"].GetValue<MenuSlider>().Value)
                    {
                        W.Cast(wPredMinion.Position);
                    }
                }
            }
            if (Q.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearQ"] && Gamer.ManaPercent >= Main[Gamer.CharacterName]["laneClear"]["clearM"].GetValue<MenuSlider>().Value)
            {
                var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(900)).ToList();
                if (!minions.Any()) return;
                if (minions.Count >= Main[Gamer.CharacterName]["laneClear"]["clearCountQ"].GetValue<MenuSlider>().Value)
                {
                    Q.Cast();
                }
            }
        }
        private static void JungleClear()
        {
            var monster = GameObjects.Jungle.FirstOrDefault(x => x.IsValidTarget(900));

            if (monster != null &&
                Gamer.ManaPercent >=
                Main[Gamer.CharacterName]["jungClear"]["jungClearM"].GetValue<MenuSlider>().Value &&
                Main[Gamer.CharacterName]["jungClear"]["jungClearW"] && W.IsReady())
            {
                W.Cast(monster.Position);
            }
            if (monster != null && monster.IsValidTarget(Gamer.GetRealAutoAttackRange() + 50) && Gamer.ManaPercent >= Main[Gamer.CharacterName]["jungClear"]["jungClearM"].GetValue<MenuSlider>().Value && Main[Gamer.CharacterName]["jungClear"]["jungClearQ"] && Q.IsReady())
            {
                Q.Cast();
            }
        }
        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Main[Gamer.CharacterName]["gapcloser"]["gapR"] && R.IsReady() && args.EndPosition.DistanceToPlayer() < 250)
            {
                R.Cast(sender.Position);
            }
        }

        private static void DangerousSpellsInterupt(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (Main[Gamer.CharacterName]["interrupt"]["interruptR"] && R.IsReady() && args.DangerLevel >= Interrupter.DangerLevel.Medium && sender.DistanceToPlayer() < 1500 && sender.IsEnemy)
            {
                R.Cast(sender.Position);
            }
        }
        private static HitChance RHitChance()
        {
            var rHit = HitChance.Medium;
            switch (Main[Gamer.CharacterName]["combo"].GetValue<MenuList >("rHit").Index)
            {
                case 0:
                    rHit = HitChance.High;
                    break;
                case 1:
                    rHit = HitChance.Medium;
                    break;
                case 2:
                    rHit = HitChance.Low;
                    break;
            }
            return rHit;
        }
    }
}
