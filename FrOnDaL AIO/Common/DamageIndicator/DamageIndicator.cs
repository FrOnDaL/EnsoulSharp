using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EnsoulSharp.SDK.Utility;
using static FrOnDaL_AIO.Common.Utilities.spellMisc;
using Color = System.Drawing.Color;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace FrOnDaL_AIO.Common.DamageIndicator
{
    public static class DamageIndicator
    {
        public static Menu DrawDamage;
        private const int Width = 103, Height = 11, XOffset = -45, YOffset = -24;

        public static void Attach(Menu mainMenu)
        {
            DrawDamage = new Menu("drawDamage", "Damage Indicator");
            {
                DrawDamage.Add(new MenuBool("enabled", "Enabled"));
                DrawDamage.Add(new MenuBool("qDmg", "Draw Q damage"));
                DrawDamage.Add(new MenuBool("wDmg", "Draw W damage"));
                DrawDamage.Add(new MenuBool("eDmg", "Draw E damage"));
                DrawDamage.Add(new MenuBool("rDmg", "Draw R damage"));
            }
            mainMenu.Add(DrawDamage);
            Drawing.OnEndScene += Render_OnPresent;
        }

        private static void Render_OnPresent(EventArgs args)
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsVisible))
            {
                if (DrawDamage["enabled"] && target.IsHPBarRendered && Render.OnScreen(Drawing.WorldToScreen(target.Position)))
                {
                    var barPos = target.HPBarPosition;
                    float qdmgDraw = 0, wdmgDraw = 0, edmgDraw = 0, rdmgDraw = 0;
                    var qRdy = Q.IsReady();
                    var wRdy = W.IsReady();
                    var eRdy = E.IsReady();
                    var rRdy = R.IsReady();
                    float qDmg = 0;
                    float wDmg = 0;
                    float eDmg = 0;
                    float rDmg = 0;

                    if (qRdy && DrawDamage["qDmg"])
                    {
                        qDmg = (float)Gamer.GetSpellDamage(target, SpellSlot.Q);
                    }

                    if (wRdy && DrawDamage["wDmg"])
                    {
                        wDmg = (float)Gamer.GetSpellDamage(target, SpellSlot.W);
                    }

                    if (eRdy && DrawDamage["eDmg"])
                    {
                        eDmg = (float)Gamer.GetSpellDamage(target, SpellSlot.E);
                    }

                    if (rRdy && DrawDamage["rDmg"])
                    {
                        rDmg = (float)Gamer.GetSpellDamage(target, SpellSlot.R);
                    }

                    switch (Gamer.CharacterName)
                    {
                        case "Ahri":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 3;
                            }
                            break;
                        case "Jhin":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 4;
                            }
                            break;
                        case "Kennen":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 6;
                            }
                            break;
                        case "Lucian":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 10;
                            }
                            break;
                        case "FiddleSticks":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 5;
                            }
                            break;
                        case "MissFortune":
                            if (eRdy && DrawDamage["eDmg"])
                            {
                                eDmg *= 2;
                            }
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 12;
                            }
                            break;
                        case "Gangplank":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 12;
                            }
                            break;
                        case "Swain":
                            if (qRdy && DrawDamage["qDmg"])
                            {
                                qDmg *= 3;
                            }
                            if (eRdy && DrawDamage["eDmg"])
                            {
                                eDmg *= 4;
                            }
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 3;
                            }
                            break;
                        case "Twitch":
                            if (eRdy && DrawDamage["eDmg"])
                            {
                                eDmg += (float)Gamer.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
                            }
                            break;
                        case "Katarina":
                            if (rRdy && DrawDamage["rDmg"])
                            {
                                rDmg *= 16;
                            }
                            break;
                    }

                    var damage = qDmg + wDmg + eDmg + rDmg;

                    if (qRdy)
                    {
                        qdmgDraw = (qDmg / damage);
                    }

                    if (wRdy && Gamer.CharacterName != "Kalista")
                    {
                        wdmgDraw = (wDmg / damage);
                    }

                    if (eRdy)
                    {
                        edmgDraw = (eDmg / damage);
                    }

                    if (rRdy)
                    {
                        rdmgDraw = (rDmg / damage);
                    }

                    var percentHealthAfterDamage = Math.Max(0, target.Health - damage) / target.MaxHealth;
                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width * target.Health / target.MaxHealth;
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + XOffset + (105 * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHp; i++)
                    {
                        if (Q.IsReady() && i < qdmgDraw * differenceInHp)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.FromArgb(0, 240, 240));
                        }
                        else if (W.IsReady() && i < (qdmgDraw + wdmgDraw) * differenceInHp)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.FromArgb(240, 150, 10));
                        }
                        else if (E.IsReady() && i < (qdmgDraw + wdmgDraw + edmgDraw) * differenceInHp)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.FromArgb(240, 240, 0));
                        }
                        else if (R.IsReady() && i < (qdmgDraw + wdmgDraw + edmgDraw + rdmgDraw) * differenceInHp)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, Color.FromArgb(195, 30, 180));
                        }
                    }
                }
            }
        }

    }
}
