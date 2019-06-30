using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace FrOnDaL_AIO.Common.Utilities
{
    public static class spellMisc
    {
        public static AIHeroClient Gamer => ObjectManager.Player;
        public static Spell Q { get; set; } = default(Spell);
        public static Spell Q2 { get; set; } = default(Spell);
        public static Spell Q3 { get; set; } = default(Spell);
        public static Spell W { get; set; } = default(Spell);
        public static Spell W2 { get; set; } = default(Spell);
        public static Spell E { get; set; } = default(Spell);
        public static Spell E2 { get; set; } = default(Spell);
        public static Spell R { get; set; } = default(Spell);
        public static Spell R2 { get; set; } = default(Spell);
        public static Menu Main { get; set; } = default(Menu);
        public static bool SpellShield(AIBaseClient shield) { return shield.HasBuffOfType(BuffType.SpellShield) || shield.HasBuffOfType(BuffType.SpellImmunity); }
    }
}
