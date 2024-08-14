using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    class SpellType
    {
        public const byte None = 0;
        public const byte Dagger = 1; // DONE
        public const byte Axe = 2; // DONE
        public const byte TimeBomb = 3; //
        public const byte TimeStop = 4; // DONE - Just needs art.
        public const byte Nuke = 5; // DONE - Needs art.
        public const byte Translocator = 6; // DONE - Just needs effect.
        public const byte Displacer = 7; // DONE - But buggy.
        public const byte Boomerang = 8; // DONE
        public const byte DualBlades = 9; // DONE
        public const byte Close = 10; // DONE
        public const byte DamageShield = 11; // DONE
        public const byte Bounce = 12; // DONE
        public const byte DragonFire = 13; // Special spell for the dragon.
        public const byte RapidDagger = 14;
        public const byte DragonFireNeo = 15;
        public const byte Total = 16;

        public const byte Laser = 100; // DONE - Needs art. // Disabled for now.
        public const byte Shout = 20; // Special spell for the barbarian.

        public static string ToStringID(byte spellType)
        {
            switch (spellType)
            {
                case (Dagger):
                    return "LOC_ID_SPELL_TYPE_1";
                case (Axe):
                    return "LOC_ID_SPELL_TYPE_2";
                case (TimeBomb):
                    return "LOC_ID_SPELL_TYPE_3";
                case (TimeStop):
                    return "LOC_ID_SPELL_TYPE_4";
                case (Nuke):
                    return "LOC_ID_SPELL_TYPE_5";
                case (Translocator):
                    return "LOC_ID_SPELL_TYPE_6";
                case (Displacer):
                    return "LOC_ID_SPELL_TYPE_7";
                case (Boomerang):
                    return "LOC_ID_SPELL_TYPE_8";
                case (DualBlades):
                    return "LOC_ID_SPELL_TYPE_9";
                case (Close):
                    return "LOC_ID_SPELL_TYPE_10";
                case (DamageShield):
                    return "LOC_ID_SPELL_TYPE_11";
                case (Bounce):
                    return "LOC_ID_SPELL_TYPE_12";
                case (Laser):
                    return "LOC_ID_SPELL_TYPE_13";
                case (DragonFire):
                case (DragonFireNeo):
                    return "LOC_ID_SPELL_TYPE_14";
                case (RapidDagger):
                    return "LOC_ID_SPELL_TYPE_15";
            }
            return "";
        }

        public static string DescriptionID(byte spellType)
        {
            switch (spellType)
            {
                case (Dagger):
                    return "LOC_ID_SPELL_DESC_1";
                case (Axe):
                    return "LOC_ID_SPELL_DESC_2";
                case (TimeBomb):
                    return "LOC_ID_SPELL_DESC_3";
                case (TimeStop):
                    return "LOC_ID_SPELL_DESC_4";
                case (Nuke):
                    return "LOC_ID_SPELL_DESC_5";
                case (Translocator):
                    return "LOC_ID_SPELL_DESC_6";
                case (Displacer):
                    return "LOC_ID_SPELL_DESC_7";
                case (Boomerang):
                    return "LOC_ID_SPELL_DESC_8";
                case (DualBlades):
                    return "LOC_ID_SPELL_DESC_9";
                case (Close):
                    return "LOC_ID_SPELL_DESC_10";
                case (DamageShield):
                    return "LOC_ID_SPELL_DESC_11";
                case (Bounce):
                    return "LOC_ID_SPELL_DESC_12";
                case (Laser):
                    return "LOC_ID_SPELL_DESC_13";
                case (DragonFire):
                case (DragonFireNeo):
                    return "LOC_ID_SPELL_DESC_14";
                case (RapidDagger):
                    return "LOC_ID_SPELL_DESC_15";
            }
            return "";
        }

        public static string Icon(byte spellType)
        {
            switch (spellType)
            {
                case (Dagger):
                    return "DaggerIcon_Sprite";
                case (Axe):
                    return "AxeIcon_Sprite";
                case (TimeBomb):
                    return "TimeBombIcon_Sprite";
                case (TimeStop):
                    return "TimeStopIcon_Sprite";
                case (Nuke): 
                    return "NukeIcon_Sprite";
                case (Translocator):
                    return "TranslocatorIcon_Sprite";
                case (Displacer):
                    return "DisplacerIcon_Sprite";
                case (Boomerang):
                    return "BoomerangIcon_Sprite";
                case (DualBlades):
                    return "DualBladesIcon_Sprite";
                case (Close):
                    return "CloseIcon_Sprite";
                case (DamageShield):
                    return "DamageShieldIcon_Sprite";
                case (Bounce):
                    return "BounceIcon_Sprite";
                case (Laser):
                    return "DaggerIcon_Sprite";
                case (DragonFire):
                case (DragonFireNeo):
                    return "DragonFireIcon_Sprite";
                case (RapidDagger):
                    return "RapidDaggerIcon_Sprite";
            }
            return "DaggerIcon_Sprite";
        }

        public static Vector3 GetNext3Spells()
        {
            byte[] spellArray = ClassType.GetSpellList(ClassType.Wizard2);
            List<byte> spellList = new List<byte>();
            foreach (byte spell in spellArray)
                spellList.Add(spell);
            int spellIndex = spellList.IndexOf(Game.PlayerStats.Spell);
            spellList.Clear();
            spellList = null;

            byte[] wizardSpells = new byte[3];
            for (int i = 0; i < 3; i++)
            {
                wizardSpells[i] = spellArray[spellIndex];
                spellIndex++;
                if (spellIndex >= spellArray.Length)
                    spellIndex = 0;
            }

            return new Vector3(wizardSpells[0], wizardSpells[1], wizardSpells[2]);
        }
    }
}
