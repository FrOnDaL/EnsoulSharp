using System;
using SharpDX;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using System.Windows.Forms;
using EnsoulSharp.SDK.Utility;
using System.Collections.Generic;
using EnsoulSharp.SDK.Prediction;
using Color = System.Drawing.Color;
using EnsoulSharp.SDK.MenuUI.Values;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;
using FrOnDaL_AIO.Common.Utilities.SPrediction;
using FrOnDaL_AIO.Common.Utilities.SpellBlocking;
using static FrOnDaL_AIO.Common.Utilities.spellMisc;
using static FrOnDaL_AIO.Common.Utilities.Extensions;
using MenuSlider = EnsoulSharp.SDK.MenuUI.Values.MenuSlider;

namespace FrOnDaL_AIO.Champions
{
    public class Orianna
    {

        private static Vector3 _ballPos;
        public static void GameOn()
        {

            Q = new Spell(SpellSlot.Q, 825f);
            W = new Spell(SpellSlot.W, 255);
            E = new Spell(SpellSlot.E, 1100f);
            R = new Spell(SpellSlot.R, 410);


            Q.SetSkillshot(0.1f, 125f, 1300f, false, SkillshotType.Circle);
            W.SetSkillshot(0.15f, 225f, float.MaxValue, false, SkillshotType.Circle);
            E.SetSkillshot(0.25f, 80f, float.MaxValue, false, SkillshotType.Line);
            R.SetSkillshot(0.6f, 400f, 100f, false, SkillshotType.Circle);



            Main = new Menu("Index", "FrOnDaL AIO", true);

            var orianna = new Menu(Gamer.CharacterName, Gamer.CharacterName);
            {
                var combo = new Menu("combo", "Combo");
                {
                    combo.Add(new MenuBool("q", "Use combo Q"));
                    combo.Add(new MenuBool("w", "Use combo W"));
                    combo.Add(new MenuBool("e", "Use combo E"));
                    combo.Add(new MenuBool("distE", "Use E to reduce distance to enemy(ball)"));
                    combo.Add(new MenuBool("r", "Use combo R"));
                    combo.Add(new MenuList("rMode", "R mode :", new[] { "Enemy count R", "Only if killable", "Never" }) { Index = 1 });
                    combo.Add(new MenuSlider("rHits", "Ball hit x units enemies >= x to use R (-R mode 'Enemy count R' is active-)", 2, 1, 5));
                }
                orianna.Add(combo);

                /*var autoshield = new Menu("autoshield", "Auto Shield E");
                {
                    autoshield.Add(new MenuBool("shieldE", "Auto Shield (On/Off)"));
                    autoshield.Add(new MenuSlider("eHealt", "Ally health percentage % to use E", 30, 1));
                    autoshield.Add(new MenuSlider("eMana", "Min mana percentage % to use E", 25, 1));
                    autoshield.Add(new MenuBool("shieldAae", "Enemy AA Auto Shield"));
                    autoshield.Add(new MenuSeparator("0", "Ally White List Settings"));
                    var whiteListE = new Menu("whiteListE", "E Shield white list");
                    {
                        foreach (var enemies in GameObjects.AllyHeroes)
                        {
                            whiteListE.Add(new MenuBool("eWhiteList" + enemies.CharacterName.ToLower(), enemies.CharacterName));
                        }
                    }
                    autoshield.Add(whiteListE);
                    autoshield.Add(new MenuSeparator("1", "Enemy Skill Shots Settings"));
                    var whiteListSs = new Menu("whiteListSs", "Enemy Skill Shots");
                    {
                        whiteListSs.Add(new MenuBool("ssOn", "Auto Shield Skill Shots (On/Off)"));
                        foreach (var hero in GameObjects.EnemyHeroes.Where(i => SpellDatabase.Spells.Any(a => a.ChampionName == i.CharacterName)))
                        {
                            foreach (var spell in SpellDatabase.Spells.Where(i => GameObjects.EnemyHeroes.Any(a => a.CharacterName == i.ChampionName)))
                            {
                                if (string.Equals(spell.ChampionName, hero.CharacterName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (DamageBoostDatabase.Spells.Any(x => x.Spell == spell.SpellName))
                                    {
                                        var skill = new Menu("skill" + hero.CharacterName.ToLower(), hero.CharacterName);
                                        {
                                            skill.Add(new MenuBool("skillSpell" + spell.SpellName.ToLower(), hero.CharacterName + " | " + spell.SpellName + " " + " | " + spell.Slot));
                                        }
                                        whiteListSs.Add(skill);
                                    }
                                }
                            }
                        }
                        whiteListSs.Add(new MenuSeparator("2", "- If the enemy spell use is within range E -"));
                    }
                    autoshield.Add(whiteListSs);
                }
                orianna.Add(autoshield);*/

                var harass = new Menu("harass", "Harass");
                {
                    harass.Add(new MenuBool("harq", "Use harass Q"));
                    harass.Add(new MenuBool("harw", "Use harass W"));
                    harass.Add(new MenuBool("hare", "Use harass E"));
                    harass.Add(new MenuSeparator("3", "Auto Harass Settings"));
                    harass.Add(new MenuKeyBind("autoharOn", "Auto harass (On/Off)", Keys.H, KeyBindType.Toggle, "false"));
                    harass.Add(new MenuBool("autoharq", "Use harass Q auto"));
                    harass.Add(new MenuBool("autoharw", "Use harass W auto"));
                    harass.Add(new MenuSlider("autoharMana", "Min mana percentage % to use harass", 25, 1));
                }
                orianna.Add(harass);

                var killsteal = new Menu("killsteal", "Killsteal");
                {
                    killsteal.Add(new MenuBool("ksq", "Use killsteal Q"));
                    killsteal.Add(new MenuBool("ksw", "Use killsteal W"));
                    killsteal.Add(new MenuBool("ksr", "Use killsteal R"));
                }
                orianna.Add(killsteal);

                var laneClear = new Menu("laneClear", "LaneClear");
                {
                    laneClear.Add(new MenuKeyBind("clearOn", "Lane clear (On/Off)", Keys.G, KeyBindType.Toggle, "false"));
                    laneClear.Add(new MenuBool("clearQ", "Use lane clear Q"));
                    laneClear.Add(new MenuSlider("clearCountQ", "Q hit x units minions >= x", 3, 1, 6));
                    laneClear.Add(new MenuBool("clearW", "Use lane clear W"));
                    laneClear.Add(new MenuSlider("clearCountW", "W hit x units minions >= x", 3, 1, 6));
                    laneClear.Add(new MenuBool("clearE", "Use lane clear E", false));
                    laneClear.Add(new MenuSlider("clearM", "Min mana percentage % to use clear", 40, 1));
                }
                orianna.Add(laneClear);

                var jungClear = new Menu("jungClear", "JungClear");
                {
                    jungClear.Add(new MenuBool("jungClearQ", "Use jung clear Q"));
                    jungClear.Add(new MenuBool("jungClearW", "Use jung clear W"));
                    jungClear.Add(new MenuBool("jungClearE", "Use jung clear E"));
                }
                orianna.Add(jungClear);

                var misc = new Menu("misc", "Misc");
                {
                    var gapcloser = new Menu("gapcloser", "Anti-gapcloser");
                    {
                        gapcloser.Add(new MenuBool("gapE", "Use E anti gap closer",false));
                    }
                    misc.Add(gapcloser);
                    var interrupt = new Menu("interrupt", "Interrupt Dangerous Spells");
                    {
                        interrupt.Add(new MenuBool("interruptR", "Use R interrupt", false));
                    }
                    misc.Add(interrupt);
                }
                orianna.Add(misc);

                var drawings = new Menu("drawings", "Drawings");
                {
                    drawings.Add(new MenuBool("qdraw", "Draw Q"));
                    drawings.Add(new MenuBool("wdraw", "Draw W"));
                    drawings.Add(new MenuBool("edraw", "Draw E"));
                    drawings.Add(new MenuBool("rdraw", "Draw R"));
                    drawings.Add(new MenuBool("cleardraw", "Lane clear status draw"));
                    drawings.Add(new MenuBool("harasdraw", "Auto harass status draw"));
                }
                orianna.Add(drawings);
            }

            Main.Add(orianna);
            Common.DamageIndicator.DamageIndicator.Attach(Main);
            Main.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += SpellDraw;
            GameObject.OnCreate += CreateBall;
            //AIBaseClient.OnProcessSpellCast += AutoAttackShield;
            AIBaseClient.OnProcessSpellCast += AutoShield;
            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterrupterSpell += DangerousSpellsInterupt;
        }

        private static void CreateBall(GameObject ball, EventArgs args)
        {
            if (ball.IsValid && ball.IsAlly && ball.Name == "TheDoomBall")
            {
                _ballPos = ball.Position;
            }
            foreach (var ballpos in GameObjects.AllyHeroes)
            {
                if (ballpos.HasBuff("orianaghost"))
                {
                    _ballPos = ballpos.Position;
                }
                else if (Gamer.HasBuff("orianaghostself"))
                {
                    _ballPos = Gamer.Position;
                }
            }
        }

        private static void SpellDraw(EventArgs args)
        {
            if (Gamer.IsDead) return;
            
            if (Main[Gamer.CharacterName]["drawings"]["qdraw"] && Q.IsReady())
            {
                Render.Circle.DrawCircle(Gamer.Position, Q.Range, Color.Green, 2);
            }
            if (Main[Gamer.CharacterName]["drawings"]["wdraw"] && W.IsReady())
            {
                foreach (var ballpos in GameObjects.AllyHeroes)
                {
                    if (ballpos.HasBuff("orianaghost"))
                    {
                        Render.Circle.DrawCircle(ballpos.Position, W.Range, Color.LawnGreen, 2);
                    }
                    else if (Gamer.HasBuff("orianaghostself"))
                    {
                        Render.Circle.DrawCircle(Gamer.Position, W.Range, Color.LawnGreen, 2);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(_ballPos, W.Range, Color.LawnGreen, 2);
                    }
                }
            }
            if (Main[Gamer.CharacterName]["drawings"]["edraw"] && E.IsReady())
            {
                Render.Circle.DrawCircle(Gamer.Position, E.Range, Color.Green, 2);
            }
            if (Main[Gamer.CharacterName]["drawings"]["rdraw"] && R.IsReady())
            {
                foreach (var ballpos in GameObjects.AllyHeroes)
                {
                    if (ballpos.HasBuff("orianaghost"))
                    {
                        Render.Circle.DrawCircle(ballpos.Position, R.Range, Color.LawnGreen, 2);
                    }
                    else if (Gamer.HasBuff("orianaghostself"))
                    {
                        Render.Circle.DrawCircle(Gamer.Position, R.Range, Color.LawnGreen, 2);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(_ballPos, R.Range, Color.LawnGreen, 2);
                    }
                }
            }
            if (Main[Gamer.CharacterName]["drawings"]["cleardraw"] && Gamer.IsVisibleOnScreen)
            {
                if (Main[Gamer.CharacterName]["laneClear"]["clearOn"].GetValue<MenuKeyBind>().Active)
                {
                    RenderDrawText("LaneClear: On", Gamer.HPBarPosition + new Vector2(-35, 160), Color.LawnGreen, 15);
                }
                else
                {
                    RenderDrawText("LaneClear: Off", Gamer.HPBarPosition + new Vector2(-35, 160), Color.Red, 15);
                }
            }
            if (Main[Gamer.CharacterName]["drawings"]["harasdraw"] && Gamer.IsVisibleOnScreen)
            {
                if (Main[Gamer.CharacterName]["harass"]["autoharOn"].GetValue<MenuKeyBind>().Active)
                {
                    RenderDrawText("Auto harass: On", Gamer.HPBarPosition + new Vector2(-35, 180), Color.LawnGreen, 15);
                }
                else
                {
                    RenderDrawText("Auto harass: Off", Gamer.HPBarPosition + new Vector2(-35, 180), Color.Red, 15);
                }
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
                    LaneClear();
                    JungleClear();
                    break;
            }

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo || Orbwalker.ActiveMode != OrbwalkerMode.Harass)
            {
                if (Main[Gamer.CharacterName]["harass"]["autoharOn"].GetValue<MenuKeyBind>().Active && Gamer.ManaPercent >= Main[Gamer.CharacterName]["harass"]["autoharMana"].GetValue<MenuSlider>().Value)
                {
                    if (Q.IsReady() && Main[Gamer.CharacterName]["harass"]["autoharq"])
                    {
                        var target = GetBestEnemyHeroTargetInRange(Q.Range);
                        if (target != null)
                        {
                            foreach (var ballpos in GameObjects.AllyHeroes)
                            {
                                if (ballpos.HasBuff("orianaghost"))
                                {
                                    Q.UpdateSourcePosition(_ballPos, _ballPos);
                                    var qPred = Q.GetPrediction(target, true);
                                    if (qPred.Hitchance >= HitChance.High)
                                    {
                                        Q.Cast(qPred.CastPosition);
                                    }
                                }
                                else if (Gamer.HasBuff("orianaghostself"))
                                {
                                    Q.UpdateSourcePosition(Gamer.Position, Gamer.Position);
                                    var qPred = Q.GetPrediction(target, true);
                                    if (qPred.Hitchance >= HitChance.High)
                                    {
                                        Q.Cast(qPred.CastPosition);
                                    }
                                }
                                else
                                {
                                    Q.UpdateSourcePosition(_ballPos, _ballPos);
                                    var qPred = Q.GetPrediction(target, true);
                                    if (qPred.Hitchance >= HitChance.High)
                                    {
                                        Q.Cast(qPred.CastPosition);
                                    }
                                }
                            }
                        }
                    }

                    if (W.IsReady() && Main[Gamer.CharacterName]["harass"]["autoharw"])
                    {
                        var target = GetBestEnemyHeroTargetInRange(E.Range);
                        if (target != null)
                        {
                            foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => _ballPos.Distance(x.Position) < 225))
                            {
                                if (ballDistance.IsValidTarget())
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }

                if (Q.IsReady() && Main[Gamer.CharacterName]["killsteal"]["ksq"])
                {
                    var target = GetBestEnemyHeroTargetInRange(Q.Range);
                    if (target != null && target.Health < Q.GetDamage(target))
                    {
                        foreach (var ballpos in GameObjects.AllyHeroes)
                        {
                            if (ballpos.HasBuff("orianaghost"))
                            {
                                Q.UpdateSourcePosition(_ballPos, _ballPos);
                                var qPred = Q.GetPrediction(target, true);
                                if (qPred.Hitchance >= HitChance.High)
                                {
                                    Q.Cast(qPred.CastPosition);
                                }
                            }
                            else if (Gamer.HasBuff("orianaghostself"))
                            {
                                Q.UpdateSourcePosition(Gamer.Position, Gamer.Position);
                                var qPred = Q.GetPrediction(target, true);
                                if (qPred.Hitchance >= HitChance.High)
                                {
                                    Q.Cast(qPred.CastPosition);
                                }
                            }
                            else
                            {
                                Q.UpdateSourcePosition(_ballPos, _ballPos);
                                var qPred = Q.GetPrediction(target, true);
                                if (qPred.Hitchance >= HitChance.High)
                                {
                                    Q.Cast(qPred.CastPosition);
                                }
                            }
                        }
                    }
                }

                if (W.IsReady() && Main[Gamer.CharacterName]["killsteal"]["ksw"])
                {
                    var target = GetBestEnemyHeroTargetInRange(E.Range);
                    if (target != null && target.Health < W.GetDamage(target))
                    {
                        foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => _ballPos.Distance(x.Position) < 250))
                        {
                            if (ballDistance.IsValidTarget())
                            {
                                W.Cast();
                            }
                        }
                    }
                }
                
                if (R.IsReady() && Main[Gamer.CharacterName]["killsteal"]["ksr"])
                {
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && _ballPos.Distance(x.Position) < R.Width))
                    {
                        if (target.Health < R.GetDamage(target))
                        {
                            var targetQ = GetBestEnemyHeroTargetInRange(Q.Range);
                            if (targetQ != null)
                            {
                                foreach (var ballpos in GameObjects.AllyHeroes)
                                {
                                    if (ballpos.HasBuff("orianaghost"))
                                    {
                                        Q.UpdateSourcePosition(_ballPos, _ballPos);
                                        var qPred = Q.GetPrediction(targetQ, true);
                                        if (qPred.Hitchance >= HitChance.High)
                                        {
                                            Q.Cast(qPred.CastPosition);
                                        }
                                    }
                                    else if (Gamer.HasBuff("orianaghostself"))
                                    {
                                        Q.UpdateSourcePosition(Gamer.Position, Gamer.Position);
                                        var qPred = Q.GetPrediction(targetQ, true);
                                        if (qPred.Hitchance >= HitChance.High)
                                        {
                                            Q.Cast(qPred.CastPosition);
                                        }
                                    }
                                    else
                                    {
                                        Q.UpdateSourcePosition(_ballPos, _ballPos);
                                        var qPred = Q.GetPrediction(targetQ, true);
                                        if (qPred.Hitchance >= HitChance.High)
                                        {
                                            Q.Cast(qPred.CastPosition);
                                        }
                                    }
                                }
                            }

                            R.Cast();
                        }
                    }
                }
                
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && Main[Gamer.CharacterName]["combo"]["q"])
            {
                var target = GetBestEnemyHeroTargetInRange(Q.Range);
                if (target != null)
                {
                    foreach (var ballpos in GameObjects.AllyHeroes)
                    {
                        if (ballpos.HasBuff("orianaghost"))
                        {
                            Q.UpdateSourcePosition(_ballPos, _ballPos);
                            var qPred = Q.GetPrediction(target, true);
                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition);
                            }
                        }
                        else if (Gamer.HasBuff("orianaghostself"))
                        {
                            Q.UpdateSourcePosition(Gamer.Position, Gamer.Position);
                            var qPred = Q.GetPrediction(target, true);
                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition);
                            }
                        }
                        else
                        {
                            Q.UpdateSourcePosition(_ballPos, _ballPos);
                            var qPred = Q.GetPrediction(target, true);

                            foreach (var ballDistance in GameObjects.EnemyHeroes)
                            {
                                if (_ballPos.Distance(ballDistance.Position) > 1400 && Main[Gamer.CharacterName]["combo"]["distE"] && E.IsReady())
                                {
                                    E.CastOnUnit(Gamer);
                                    Q.Cast(qPred.CastPosition);
                                }
                                else
                                {
                                    if (qPred.Hitchance >= HitChance.High)
                                    {
                                        Q.Cast(qPred.CastPosition);
                                    }
                                }
                            }
                            
                            
                        }
                    }
                }
            }

            if (W.IsReady() && Main[Gamer.CharacterName]["combo"]["w"])
            {
                 var target = GetBestEnemyHeroTargetInRange(E.Range);
                 if (target != null)
                 {
                     foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => _ballPos.Distance(x.Position) < 250))
                     {
                         if (ballDistance.IsValidTarget())
                         {
                             W.Cast();
                         }
                     }
                 }
            }

            if (E.IsReady() && Main[Gamer.CharacterName]["combo"]["e"])
            {
                var target = GetBestEnemyHeroTargetInRange(E.Range);
                if (target != null)
                {
                    var collision = E.GetCollisions(_ballPos.ToVector2() - 50).Units.Where(x => (x is AIHeroClient)).Count(x => x.IsEnemy);
                    if (collision >= 1 && target.Health > R.GetDamage(target) + W.GetDamage(target))
                    {
                        foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget()))
                        {
                            if (_ballPos.Distance(ballDistance.Position) < 200)
                            {
                                E.CastOnUnit(Gamer);
                            }
                        }
                    }
                }
            }

            if (R.IsReady() && Main[Gamer.CharacterName]["combo"]["r"])
            {
                if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList>("rMode").Index == 2) return;
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && _ballPos.Distance(x.Position) < R.Width))
                {
                    if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList>("rMode").Index == 0)
                    {
                        if (target.CountEnemyHeroesInRange(R.Width) >= Main[Gamer.CharacterName]["combo"]["rHits"].GetValue<MenuSlider>().Value)
                        {
                            R.Cast();
                        }
                    }
                    if (Main[Gamer.CharacterName]["combo"].GetValue<MenuList>("rMode").Index != 1) continue;
                    if (target.Health < R.GetDamage(target) && target.CountEnemyHeroesInRange(350) >= 1)
                    {
                        R.Cast();
                    }
                }
            }
        }
        
        private static void AutoShield(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var enemy = sender as AIHeroClient;
            var ally = args.Target as AIHeroClient;
            if (enemy != null && enemy.IsEnemy && DamageBoostDatabase.Spells.Any(x => x.Spell == args.SData.Name) && ally.CountEnemyHeroesInRange(E.Range + enemy.GetRealAutoAttackRange()) > 0 && Main[Gamer.CharacterName]["autoshield"]["whiteListSs"]["ssOn"] && Gamer.ManaPercent >= Main[Gamer.CharacterName]["autoshield"]["eMana"].GetValue<MenuSlider>().Value && Main[Gamer.CharacterName]["autoshield"]["shieldE"] && E.IsReady() && !Gamer.IsRecalling())
            {
                foreach (var allyList in GameObjects.EnemyHeroes)
                {
                    if (enemy.IsEnemy && DamageBoostDatabase.Spells.Any(x => x.Spell == args.SData.Name) && ally.CountEnemyHeroesInRange(E.Range + enemy.GetRealAutoAttackRange()) > 0)
                    {
                        try
                        {
                            if (!Main[Gamer.CharacterName]["autoshield"]["whiteListSs"]["skill" + allyList.CharacterName.ToLower()]["skillSpell" + args.SData.Name.ToLower()].GetValue<MenuBool>().Enabled)
                            {
                                return;
                            }
                            if (ally != null)
                            {
                                if (ally.HealthPercent <= Main[Gamer.CharacterName]["autoshield"]["eHealt"].GetValue<MenuSlider>().Value)
                                {
                                    E.CastOnUnit(ally);
                                }
                            }
                        }
                        catch
                        {
                            //Console.WriteLine();
                        }
                    }
                }
            }
        }

       /* private static void AutoAttackShield(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (Gamer.ManaPercent >= Main[Gamer.CharacterName]["autoshield"]["eMana"].GetValue<MenuSlider>().Value && Main[Gamer.CharacterName]["autoshield"]["shieldE"] && Main[Gamer.CharacterName]["autoshield"]["shieldAae"] && E.IsReady() && !Gamer.IsRecalling())
            {
                foreach (var allyList in GameObjects.AllyHeroes)
                {
                    if (!Main[Gamer.CharacterName]["autoshield"]["whiteListE"]["eWhiteList" + allyList.CharacterName.ToLower()].GetValue<MenuBool>().Enabled)
                    {
                        return;
                    }
                }
                var enemy = sender as AIHeroClient;
                var ally = args.Target as AIHeroClient;
                if (enemy != null && enemy.IsEnemy)
                {
                    foreach (var enemyIn in GameObjects.EnemyHeroes.Where(x => x.Distance(Gamer) <= E.Range + x.GetRealAutoAttackRange() && x.IsEnemy))
                    {
                        if (ally != null && args.SData.Name.Contains("BasicAttack") && enemy.Distance(Gamer) < E.Range + enemyIn.GetRealAutoAttackRange() && !enemy.IsDead && enemy == enemyIn)
                        {
                            if (ally.HealthPercent <= Main[Gamer.CharacterName]["autoshield"]["eHealt"].GetValue<MenuSlider>().Value)
                            {
                                E.CastOnUnit(ally);
                            }
                        }
                    }
                }
            }
        }*/

        private static void Harass()
        {
            if (Q.IsReady() && Main[Gamer.CharacterName]["harass"]["harq"])
            {
                var target = GetBestEnemyHeroTargetInRange(Q.Range);
                if (target != null)
                {
                    foreach (var ballpos in GameObjects.AllyHeroes)
                    {
                        if (ballpos.HasBuff("orianaghost"))
                        {
                            Q.UpdateSourcePosition(_ballPos, _ballPos);
                            var qPred = Q.GetPrediction(target, true);
                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition);
                            }
                        }
                        else if (Gamer.HasBuff("orianaghostself"))
                        {
                            Q.UpdateSourcePosition(Gamer.Position, Gamer.Position);
                            var qPred = Q.GetPrediction(target, true);
                            if (qPred.Hitchance >= HitChance.High)
                            {
                                Q.Cast(qPred.CastPosition);
                            }
                        }
                        else
                        {
                            Q.UpdateSourcePosition(_ballPos, _ballPos);
                            var qPred = Q.GetPrediction(target, true);

                            foreach (var ballDistance in GameObjects.EnemyHeroes)
                            {
                                if (_ballPos.Distance(ballDistance.Position) > 1600)
                                {
                                    E.CastOnUnit(Gamer);
                                    Q.Cast(qPred.CastPosition);
                                }
                                else
                                {
                                    if (qPred.Hitchance >= HitChance.High)
                                    {
                                        Q.Cast(qPred.CastPosition);
                                    }
                                }
                            }


                        }
                    }
                }
            }

            if (W.IsReady() && Main[Gamer.CharacterName]["harass"]["harw"])
            {
                var target = GetBestEnemyHeroTargetInRange(E.Range);
                if (target != null)
                {
                    foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => _ballPos.Distance(x.Position) < 250))
                    {
                        if (ballDistance.IsValidTarget())
                        {
                            W.Cast();
                        }
                    }
                }
            }

            if (E.IsReady() && Main[Gamer.CharacterName]["harass"]["hare"])
            {
                var target = GetBestEnemyHeroTargetInRange(E.Range);
                if (target != null)
                {
                    var collision = E.GetCollisions(_ballPos.ToVector2() - 50).Units.Where(x => (x is AIHeroClient)).Count(x => x.IsEnemy);
                    if (collision >= 1)
                    {
                        foreach (var ballDistance in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget()))
                        {
                            if (_ballPos.Distance(ballDistance.Position) < 200)
                            {
                                E.CastOnUnit(Gamer);
                            }
                        }
                    }
                }
            }
        }
        private static void LaneClear()
        {
            if (Main[Gamer.CharacterName]["laneClear"]["clearOn"].GetValue<MenuKeyBind>().Active && Gamer.ManaPercent >= Main[Gamer.CharacterName]["laneClear"]["clearM"].GetValue<MenuSlider>().Value)
            {
                if (Q.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearQ"])
                {
                    var qPredMinion = Q.GetCircularFarmLocation(new List<AIBaseClient>(GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion()).Cast<AIBaseClient>().ToList()), 170);
                    if (qPredMinion.MinionsHit == 0)
                    {
                        foreach (var target in GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)))
                        {
                            if (target != null)
                            {
                                if (GameObjects.EnemyMinions.Count(x => x.IsValidTarget(170,x.IsLaneMinion,Q.GetPrediction(target).CastPosition)) >= Main[Gamer.CharacterName]["laneClear"]["clearCountQ"].GetValue<MenuSlider>().Value)
                                {
                                    Q.Cast(Q.GetPrediction(target).CastPosition);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (qPredMinion.MinionsHit >= Main[Gamer.CharacterName]["laneClear"]["clearCountQ"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(qPredMinion.Position);
                        }
                    }
                }
                if (W.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearW"])
                {
                    foreach (var minion in GetEnemyLaneMinionsTargetsInRange(E.Range))
                    {
                        if (minion.IsValidTarget(E.Range) && minion != null)
                        {
                            if (GameObjects.EnemyMinions.Count(x => x.IsValidTarget(250, false, _ballPos)) >= Main[Gamer.CharacterName]["laneClear"]["clearCountW"].GetValue<MenuSlider>().Value)
                            {
                                W.Cast();
                            }
                        }
                    }
                }
                if (E.IsReady() && Main[Gamer.CharacterName]["laneClear"]["clearE"])
                {
                    var ePredMinion = E.GetLineFarmLocation(new List<AIBaseClient>(GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range) && x.IsMinion()).Cast<AIBaseClient>().ToList()), E.Width);
                    if (!W.IsReady() && !Q.IsReady() && ePredMinion.MinionsHit >= 2)
                    {
                        E.CastOnUnit(Gamer);
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
                    Q.Cast(monster.Position);
                }
            }
            if (W.IsReady() && Main[Gamer.CharacterName]["jungClear"]["jungClearW"] && monster.IsValidTarget(E.Range))
            {
                if (monster != null)
                {
                    foreach (var jungle in GameObjects.Jungle)
                    {
                        if (jungle.IsValidTarget(E.Range) && jungle != null)
                        {
                            if (GameObjects.Jungle.Count(x => x.IsValidTarget(250, false, _ballPos)) >= 1)
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
            if (monster != null && Main[Gamer.CharacterName]["jungClear"]["jungClearE"] && E.IsReady())
            {
                var junglePred = E.GetLineFarmLocation(new List<AIBaseClient>(GameObjects.Jungle.Where(x => x.IsValidTarget(E.Range) && x.IsJungle()).Cast<AIBaseClient>().ToList()), E.Width);
                if (!W.IsReady() && !Q.IsReady() && junglePred.MinionsHit >= 1)
                {
                    E.CastOnUnit(Gamer);
                }
            }
        }
        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Main[Gamer.CharacterName]["misc"]["gapcloser"]["gapE"])
            {
                if (E.IsReady() && Gamer.Position.Extend(Game.CursorPosRaw, E.Range).CountEnemyHeroesInRange(400) < 3)
                {
                    if (sender.IsValidTarget(E.Range))
                    {
                        E.Cast(Gamer.Position.Extend(Game.CursorPosRaw, E.Range), true);
                    }
                }
            }
        }
        private static void DangerousSpellsInterupt(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (Main[Gamer.CharacterName]["misc"]["interrupt"]["interruptR"] && R.IsReady() && args.DangerLevel >= Interrupter.DangerLevel.Medium && sender.DistanceToPlayer() < Q.Range && sender.IsEnemy)
            {
                Q.Cast(sender.Position);
                if (_ballPos.Distance(sender.Position) < R.Width)
                {
                    R.Cast();
                }
            }
        }
    }
}
