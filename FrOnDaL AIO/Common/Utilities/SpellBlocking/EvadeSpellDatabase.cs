using EnsoulSharp;

using System.Collections.Generic;

namespace FrOnDaL_AIO.Common.Utilities.SpellBlocking
{
    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.CharacterName == "Janna")
            {
                //Spells.Add(new EvadeSpellData
                //{
                //    Name = "FioraQ",
                //    Slot = SpellSlot.Q,
                //    Range = 340,
                //    Delay = 50,
                //    Speed = 1100,
                //    _dangerLevel = 2
                //});

                Spells.Add(new EvadeSpellData
                {
                    Name = "EyeOfTheStorm",
                    Slot = SpellSlot.E,
                    Range = 800,
                    Delay = 200,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Rakan")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "RakanE",
                    Slot = SpellSlot.E,
                    Range = 550,
                    Delay = 200,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Sona")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "sonaw",
                    Slot = SpellSlot.W,
                    Range = 400,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Lulu")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "LuluE",
                    Slot = SpellSlot.E,
                    Range = 650,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Ivern")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "IvernE",
                    Slot = SpellSlot.E,
                    Range = 650,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }

        }
    }
}
