using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    class SkillUnlockType
    {
        public const byte None = 0;
        public const byte Blacksmith = 1;
        public const byte Enchantress = 2;
        public const byte Architect = 3;
        public const byte Ninja = 4;
        public const byte Banker = 5;
        public const byte SpellSword = 6;
        public const byte Lich = 7;

        public const byte KnightUp = 8;
        public const byte WizardUp = 9;
        public const byte BarbarianUp = 10;
        public const byte NinjaUp = 11;
        public const byte AssassinUp = 12;
        public const byte BankerUp = 13;
        public const byte SpellSwordUp = 14;
        public const byte LichUp = 15;

        public const byte Dragon = 16;
        public const byte Traitor = 17;

        public static string DescriptionID(byte unlockType)
        {
            switch (unlockType)
            {
                case (Blacksmith):
                    return "LOC_ID_SKILL_UNLOCK_1";
                case (Enchantress):
                    return "LOC_ID_SKILL_UNLOCK_2";
                case (Architect):
                    return "LOC_ID_SKILL_UNLOCK_3";
                case (Ninja):
                    return "LOC_ID_SKILL_UNLOCK_4";
                case (Banker):
                    return "LOC_ID_SKILL_UNLOCK_5";
                case (SpellSword):
                    return "LOC_ID_SKILL_UNLOCK_6";
                case (Lich):
                    return "LOC_ID_SKILL_UNLOCK_7";
                case (KnightUp):
                    return "LOC_ID_SKILL_UNLOCK_8";
                case (WizardUp):
                    return "LOC_ID_SKILL_UNLOCK_9";
                case (BarbarianUp):
                    return "LOC_ID_SKILL_UNLOCK_10";
                case (NinjaUp):
                    return "LOC_ID_SKILL_UNLOCK_11";
                case (AssassinUp):
                    return "LOC_ID_SKILL_UNLOCK_12";
                case (BankerUp):
                    return "LOC_ID_SKILL_UNLOCK_13";
                case (SpellSwordUp):
                    return "LOC_ID_SKILL_UNLOCK_14";
                case (LichUp):
                    return "LOC_ID_SKILL_UNLOCK_15";
                case(Dragon):
                    return "LOC_ID_SKILL_UNLOCK_16";
                case (Traitor):
                    return "LOC_ID_SKILL_UNLOCK_17";
            }

            return "";
        }
    }

}
