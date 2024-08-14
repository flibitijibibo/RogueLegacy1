using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    class ClassType
    {
        // These four classes start unlocked.
        public const byte Knight = 0;
        public const byte Wizard = 1;
        public const byte Barbarian = 2;
        public const byte Assassin = 3;
        
        // These four classes need to be unlocked via skill tree.
        public const byte Ninja = 4;
        public const byte Banker = 5;
        public const byte SpellSword = 6;
        public const byte Lich = 7;

        public const byte Knight2 = 8;
        public const byte Wizard2 = 9;
        public const byte Barbarian2 = 10;
        public const byte Assassin2 = 11;

        public const byte Ninja2 = 12;
        public const byte Banker2 = 13;
        public const byte SpellSword2 = 14;
        public const byte Lich2 = 15;

        public const byte TotalUniques = 8;
        public const byte Total = 16;

        public const byte Dragon = 16;
        public const byte Traitor = 17;

        public static string ToStringID(byte classType, bool isFemale)
        {
            switch (classType)
            {
                case (Knight):
                    return !isFemale ? "LOC_ID_CLASS_NAME_1_MALE" : "LOC_ID_CLASS_NAME_1_FEMALE"; // "Knight"
                case (Knight2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_2_MALE" : "LOC_ID_CLASS_NAME_2_FEMALE"; // "Paladin"
                case (Assassin):
                    return !isFemale ? "LOC_ID_CLASS_NAME_3_MALE" : "LOC_ID_CLASS_NAME_3_FEMALE"; // "Knave"
                case (Assassin2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_4_MALE" : "LOC_ID_CLASS_NAME_4_FEMALE"; // "Assassin";
                case (Banker):
                    return !isFemale ? "LOC_ID_CLASS_NAME_5_MALE" : "LOC_ID_CLASS_NAME_5_FEMALE"; // "Miner"
                case (Banker2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_6_MALE" : "LOC_ID_CLASS_NAME_6_FEMALE"; // "Spelunker", "Spelunkette"
                case (Wizard):
                    return !isFemale ? "LOC_ID_CLASS_NAME_7_MALE" : "LOC_ID_CLASS_NAME_7_FEMALE"; // "Mage"
                case (Wizard2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_8_MALE" : "LOC_ID_CLASS_NAME_8_FEMALE"; // "Archmage"
                case (Barbarian):
                    return !isFemale ? "LOC_ID_CLASS_NAME_9_MALE" : "LOC_ID_CLASS_NAME_9_FEMALE"; // "Barbarian"
                case (Barbarian2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_10_MALE" : "LOC_ID_CLASS_NAME_10_FEMALE"; // "Barbarian King", "Barbarian Queen"
                case (Ninja):
                    return "LOC_ID_CLASS_NAME_11"; // "Shinobi"
                case (Ninja2):
                    return "LOC_ID_CLASS_NAME_12"; // "Hokage"
                case (SpellSword):
                    return !isFemale ? "LOC_ID_CLASS_NAME_13_MALE" : "LOC_ID_CLASS_NAME_13_FEMALE"; // "Spellthief"
                case (SpellSword2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_14_MALE" : "LOC_ID_CLASS_NAME_14_FEMALE"; // "Spellsword"
                case (Lich):
                    return "LOC_ID_CLASS_NAME_15"; // "Lich"
                case (Lich2):
                    return !isFemale ? "LOC_ID_CLASS_NAME_16_MALE" : "LOC_ID_CLASS_NAME_16_FEMALE"; // "Lich King", "Lich Queen"
                case (Dragon):
                    return !isFemale ? "LOC_ID_CLASS_NAME_17_MALE" : "LOC_ID_CLASS_NAME_17_FEMALE"; // "Dragon"
                case (Traitor):
                    return !isFemale ? "LOC_ID_CLASS_NAME_18_MALE" : "LOC_ID_CLASS_NAME_18_FEMALE"; // "Traitor"
            }

            return "";
        }

        public static string DescriptionID(byte classType)
        {
            switch (classType)
            {
                case (Knight):
                    return "LOC_ID_CLASS_DESC_1"; //"Your standard hero. Pretty good at everything.";//"Your average knight. No outstanding abilities or weaknesses.";
                case (Knight2):
                    return "LOC_ID_CLASS_DESC_2"; //"Your standard hero. Pretty good at everything.\nSPECIAL: Guardian's Shield.";//"Your standard hero. Pretty good at everything. [Input:" + InputMapType.PLAYER_BLOCK + "] to block all incoming damage.";
                case (Assassin):
                    return "LOC_ID_CLASS_DESC_3"; //"A risky hero. Low stats but can land devastating critical strikes.";//"75% HP, 25% Crit Chance and Crit Damage";//"35% Health and Manaa\n250% Strength and Intelligence";//"He is an assassin";
                case (Assassin2):
                    return "LOC_ID_CLASS_DESC_4"; //"A risky hero. Low stats but can land devastating critical strikes.\nSPECIAL: Mist Form."; //"A risky hero. He has low stats but can land devestating critical strikes. [Input:" + InputMapType.PLAYER_BLOCK + "] turns you into mist.";
                case (Banker):
                    return "LOC_ID_CLASS_DESC_5"; //"A hero for hoarders. Very weak, but has a huge bonus to gold.";//"Super rich banker";
                case (Banker2):
                    return "LOC_ID_CLASS_DESC_6"; //"A hero for hoarders. Very weak, but has a huge bonus to gold.\nSPECIAL: Ordinary Headlamp.";//"Super rich banker";//"A hero for hoarders. Very weak, but has a huge bonus to gold. [Input:" + InputMapType.PLAYER_BLOCK + "] to toggle your headlamp.";//"Super rich banker";
                case (Wizard):
                    return "LOC_ID_CLASS_DESC_7"; //"A powerful spellcaster. Every kill gives you mana."; //"Everyone loves Magical Trevor";
                case (Wizard2):
                    return "LOC_ID_CLASS_DESC_8"; //"A powerful spellcaster. Every kill gives you mana.\nSPECIAL: Spell Cycle.";//"Very weak, but very intelligent, and every kill gives you mana. [Input:" + InputMapType.PLAYER_BLOCK + "] cycles spells.";
                case (Barbarian):
                    return "LOC_ID_CLASS_DESC_9"; //"A walking tank. This hero can take a beating.";//"Conan the babarian";
                case (Barbarian2):
                    return "LOC_ID_CLASS_DESC_10"; //"A walking tank. This hero can take a beating.\nSPECIAL: Barbarian Shout.";
                case (Ninja):
                    return "LOC_ID_CLASS_DESC_11"; //"A fast hero. Deal massive damage, but you cannot crit.";//I am a rogue";
                case (Ninja2):
                    return "LOC_ID_CLASS_DESC_12"; //"A fast hero. Deal massive damage, but you cannot crit.\nSPECIAL: Replacement Technique.";
                case (SpellSword):
                    return "LOC_ID_CLASS_DESC_13"; //"A hero for experts. Hit enemies to restore mana.";
                case (SpellSword2):
                    return "LOC_ID_CLASS_DESC_14"; //"A hero for experts. Hit enemies to restore mana.\nSPECIAL: Empowered Spell.";
                case (Lich):
                    return "LOC_ID_CLASS_DESC_15"; //"Feed off the dead. Gain permanent life for every kill up to a cap. Extremely intelligent.";
                case (Lich2):
                    return "LOC_ID_CLASS_DESC_16"; //"Feed off the dead. Gain permanent life for every kill up to a cap. Extremely intelligent.\nSPECIAL: HP Conversion.";
                case (Dragon):
                    return "LOC_ID_CLASS_DESC_17"; //"You are a man-dragon";
                case (Traitor):
                    return "LOC_ID_CLASS_DESC_18"; //"Fountain text here";
            }
            return "";
        }

        // Can't return Loc IDs because of all the inline values, just return string and assume calling function/screen will call this on language change refresh
        public static string ProfileCardDescription(byte classType)
        {
            switch (classType)
            {
                case (Knight):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_1");
                case (Knight2):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_2");
                case (Assassin):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_3_NEW"), (PlayerEV.ASSASSIN_CRITCHANCE_MOD * 100), (PlayerEV.ASSASSIN_CRITDAMAGE_MOD * 100));
                    //return "+" + (PlayerEV.ASSASSIN_CRITCHANCE_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_3") + ", +" + (PlayerEV.ASSASSIN_CRITDAMAGE_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_3b");
                case (Assassin2):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_4_NEW"), (PlayerEV.ASSASSIN_CRITCHANCE_MOD * 100), (PlayerEV.ASSASSIN_CRITDAMAGE_MOD * 100));
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_4") + "\n+" + (PlayerEV.ASSASSIN_CRITCHANCE_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_4b") + ", +" + (PlayerEV.ASSASSIN_CRITDAMAGE_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_4c");
                case (Banker):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_5_NEW"), (PlayerEV.BANKER_GOLDGAIN_MOD * 100));
                    //return "+" + (PlayerEV.BANKER_GOLDGAIN_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_5");
                case (Banker2):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_6_NEW"), (PlayerEV.BANKER_GOLDGAIN_MOD * 100));
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_6") + "\n+" + (PlayerEV.BANKER_GOLDGAIN_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_6b");
                case (Wizard):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_7_NEW"), GameEV.MAGE_MANA_GAIN);
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_7") + " " + GameEV.MAGE_MANA_GAIN + " " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_7b");
                case (Wizard2):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_8_NEW"), GameEV.MAGE_MANA_GAIN);
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_8") + " " + GameEV.MAGE_MANA_GAIN + " " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_8b");
                case (Barbarian):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_9");
                case (Barbarian2):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_10");
                case (Ninja):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_11_NEW"), (PlayerEV.NINJA_MOVESPEED_MOD * 100));
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_11") + "\n +" + (PlayerEV.NINJA_MOVESPEED_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_11b");
                case (Ninja2):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_12_NEW"), (PlayerEV.NINJA_MOVESPEED_MOD * 100));
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_12") + "\n +" + (PlayerEV.NINJA_MOVESPEED_MOD * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_12b");
                case (SpellSword):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_13_NEW"), (GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION * 100));
                    //return (GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_13");
                case (SpellSword2):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_14_NEW"), (GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION * 100));
                    //return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_14") + "\n" + (GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION * 100) + LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_14b");
                case (Lich):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_15");
                case (Lich2):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_16");
                case (Dragon):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_17");
                case (Traitor):
                    return LocaleBuilder.getResourceString("LOC_ID_PROFILE_DESC_18");
            }
            return "";
        }

        public static byte GetRandomClass()
        {
            List<byte> randomClassList = new List<byte>();

            // Adding the classes that start unlocked.
            randomClassList.Add(ClassType.Knight);
            randomClassList.Add(ClassType.Wizard);
            randomClassList.Add(ClassType.Barbarian);
            randomClassList.Add(ClassType.Assassin);

            // Adding the classes that have been unlocked via skill tree.
            if (SkillSystem.GetSkill(SkillType.Ninja_Unlock).ModifierAmount > 0)
                randomClassList.Add(ClassType.Ninja);
            if (SkillSystem.GetSkill(SkillType.Banker_Unlock).ModifierAmount > 0)
                randomClassList.Add(ClassType.Banker);
            if (SkillSystem.GetSkill(SkillType.Spellsword_Unlock).ModifierAmount > 0)
                randomClassList.Add(ClassType.SpellSword);
            if (SkillSystem.GetSkill(SkillType.Lich_Unlock).ModifierAmount > 0)
                randomClassList.Add(ClassType.Lich);
            if (SkillSystem.GetSkill(SkillType.SuperSecret).ModifierAmount > 0)
                randomClassList.Add(ClassType.Dragon);

            if (Game.PlayerStats.ChallengeLastBossBeaten == true || Game.GameConfig.UnlockTraitor == 2)
                randomClassList.Add(ClassType.Traitor);

            // After the list is made, randomly select a class from that list.
            byte randClass = randomClassList[CDGMath.RandomInt(0, randomClassList.Count - 1)];

            // Check to see if the upgrade for the class has been unlocked. If so, return the upgraded version of the class.
            if (Upgraded(randClass) == true)
                randClass += 8;

            return randClass;
        }

        public static bool Upgraded(byte classType)
        {
            switch (classType)
            {
                case (ClassType.Knight):
                    return SkillSystem.GetSkill(SkillType.Knight_Up).ModifierAmount > 0;
                case (ClassType.Wizard):
                    return SkillSystem.GetSkill(SkillType.Mage_Up).ModifierAmount > 0;
                case (ClassType.Barbarian):
                    return SkillSystem.GetSkill(SkillType.Barbarian_Up).ModifierAmount > 0;
                case (ClassType.Ninja):
                    return SkillSystem.GetSkill(SkillType.Ninja_Up).ModifierAmount > 0;
                case (ClassType.Assassin):
                    return SkillSystem.GetSkill(SkillType.Assassin_Up).ModifierAmount > 0;
                case (ClassType.Banker):
                    return SkillSystem.GetSkill(SkillType.Banker_Up).ModifierAmount > 0;
                case (ClassType.SpellSword):
                    return SkillSystem.GetSkill(SkillType.SpellSword_Up).ModifierAmount > 0;
                case (ClassType.Lich):
                    return SkillSystem.GetSkill(SkillType.Lich_Up).ModifierAmount > 0;
            }

            return false;
        }

        public static byte[] GetSpellList(byte classType)
        {
            switch (classType)
            {
                case (ClassType.Knight):
                case (ClassType.Knight2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Boomerang, SpellType.DualBlades, SpellType.Close, SpellType.Bounce, };
                case (ClassType.Barbarian):
                case (ClassType.Barbarian2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Boomerang, SpellType.DualBlades, SpellType.Close, };
                case (ClassType.Assassin):
                case (ClassType.Assassin2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Translocator, SpellType.Boomerang, SpellType.DualBlades, SpellType.Bounce, };
                case (ClassType.Banker):
                case (ClassType.Banker2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Boomerang, SpellType.DualBlades, SpellType.DamageShield, SpellType.Bounce, };
                case (ClassType.Lich):
                case (ClassType.Lich2):
                    return new byte[] { SpellType.Nuke, SpellType.DamageShield, SpellType.Bounce, };
                case (ClassType.SpellSword):
                case (ClassType.SpellSword2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Boomerang, SpellType.DualBlades, SpellType.Close, SpellType.DamageShield, };
                case (ClassType.Wizard):
                case (ClassType.Wizard2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.TimeStop, SpellType.Boomerang, SpellType.DualBlades, SpellType.Close, SpellType.DamageShield, SpellType.Bounce, };
                case (ClassType.Ninja):
                case (ClassType.Ninja2):
                    return new byte[] { SpellType.Axe, SpellType.Dagger, SpellType.Translocator, SpellType.Boomerang, SpellType.DualBlades, SpellType.Close, SpellType.Bounce, };
                case (ClassType.Dragon):
                    return new byte[] { SpellType.DragonFire, };
                case (ClassType.Traitor):
                    return new byte[] { SpellType.RapidDagger };
            }

            return null;
        }
    }

}
