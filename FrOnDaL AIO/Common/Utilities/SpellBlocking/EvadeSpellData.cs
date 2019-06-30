using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI.Values;

namespace FrOnDaL_AIO.Common.Utilities.SpellBlocking
{
    public class EvadeSpellData
    {
        public delegate float MoveSpeedAmount();

        public bool CanShieldAllies;
        public string CheckSpellName = "";
        public int Delay;
        public bool IsDash;
        public bool IsInvulnerability;
        public bool IsMovementSpeedBuff;
        public bool IsShield;
        public bool IsSpellShield;
        public float Range;
        public string Name;
        public bool SelfCast;
        public SpellSlot Slot;
        public int Speed;
        public int _dangerLevel;

        public EvadeSpellData() { }

        public EvadeSpellData(string name, SpellSlot slot, float range, int delay, int speed, int dangerLevel,
            bool isSpellShield = false)
        {
            Name = name;
            Slot = slot;
            Range = range;
            Delay = delay;
            Speed = speed;
            _dangerLevel = dangerLevel;
            IsSpellShield = isSpellShield;
        }

        public int DangerLevel
        {
            get
            {
                return EvadeManager.EvadeSpellMenu["DangerLevel_" + Slot] == null
                    ? _dangerLevel
                    : EvadeManager.EvadeSpellMenu["DangerLevel_" + Slot].GetValue<MenuSlider>().Value;
            }
            set { _dangerLevel = value; }
        }

        public bool Enabled => EvadeManager.EvadeSpellMenu["Enabled" + Slot] == null ||
                               EvadeManager.EvadeSpellMenu["Enabled" + Slot];

        public bool Tower => EvadeManager.EvadeSpellMenu["Tower" + Slot] == null ||
                             EvadeManager.EvadeSpellMenu["Tower" + Slot];

        public bool IsReady()
        {
            return (CheckSpellName == "" || ObjectManager.Player.Spellbook.GetSpell(Slot).Name == CheckSpellName) &&
                   ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready;
        }
    }
}
