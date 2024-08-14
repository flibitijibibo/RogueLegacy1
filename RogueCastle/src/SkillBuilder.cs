using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    class SkillBuilder
    {

        public static SkillObj BuildSkill(SkillType skillType, SkillObj trait = null)
        {
            if (trait == null)
                trait = new SkillObj("Icon_SwordLocked_Sprite");

            switch (skillType)
            {
                case (SkillType.Null):
                    break;
                case (SkillType.Filler):
                    trait.Name = "Filler"; // not localized
                    //trait.Description = "This is a filler trait used to link to other traits. This text should never be visible."; // not localized
                    trait.NameLocID = "";
                    trait.DescLocID = "";
                    break;

                #region Skills

                case (SkillType.Health_Up):
                    trait.Name = "Health Up";
                    //trait.Description = "Improve your cardio workout. A better heart means better health.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_1";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_1";
                    trait.PerLevelModifier = 10f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 40;
                    trait.MaxLevel = 75;
                    trait.IconName = "Icon_Health_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Invuln_Time_Up):
                    trait.Name = "Invuln Time Up";
                    //trait.Description = "Strengthen your adrenal glands and be invulnerable  like Bane. Let the games begin!";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_2";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_2";
                    trait.PerLevelModifier = 0.1f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 1700;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_InvulnTimeUpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " sec";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_21" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Death_Dodge):
                    trait.Name = "Death Defy";
                    //trait.Description = "Release your inner cat, and avoid death. Sometimes.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_3";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_3";
                    trait.PerLevelModifier = 0.015f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 1500;
                    trait.MaxLevel = 10;
                    trait.IconName = "Icon_DeathDefyLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case(LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Attack_Up):
                    trait.Name = "Attack Up";
                    //trait.Description = "A proper gym will allow you to really  strengthen your arms and butt muscles.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_4";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_4";
                    trait.PerLevelModifier = 2f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 85;
                    trait.MaxLevel = 75;
                    trait.IconName = "Icon_SwordLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " str";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_16" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Down_Strike_Up):
                    trait.Name = "Down Strike Up";
                    //trait.Description = "A pogo practice room has its benefits. Deal more damage with consecutive down strikes.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_5";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_5";
                    trait.PerLevelModifier = 0.05f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 1500;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_Attack_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Crit_Chance_Up):
                    trait.Name = "Crit Chance Up";
                    //trait.Description = "Teaching yourself about the weaknesses of enemies allows you to strike with deadly efficiency.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_6";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_6";
                    trait.PerLevelModifier = 0.02f;
                    trait.BaseCost = 150;
                    trait.Appreciation = 125;
                    trait.MaxLevel = 25;
                    trait.IconName = "Icon_Crit_Chance_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Crit_Damage_Up):
                    trait.Name = "Crit Damage Up";
                    //trait.Description = "Practice the deadly strikes to be even deadlier. Enemies will be so dead.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_7";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_7";
                    trait.PerLevelModifier = 0.05f;
                    trait.BaseCost = 150;
                    trait.Appreciation = 125;
                    trait.MaxLevel = 25;
                    trait.IconName = "Icon_Crit_Damage_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Magic_Damage_Up):
                    trait.Name = "Magic Damage Up";
                    //trait.Description = "Learn the secrets of the universe, so you can use it to kill with spells better.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_8";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_8";
                    trait.PerLevelModifier = 2f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 85;
                    trait.MaxLevel = 75;
                    trait.IconName = "Icon_MagicDmgUpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " int";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_17" };
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Mana_Up):
                    trait.Name = "Mana Up";
                    //trait.Description = "Increase your mental fortitude in order to increase your mana pool. ";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_9";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_9";
                    trait.PerLevelModifier = 10f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 40;
                    trait.MaxLevel = 75;
                    trait.IconName = "Icon_ManaUpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " mp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_15" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Smithy):
                    trait.Name = "Smithy";
                    //trait.Description = "Unlock the smithy and gain access to phat loot.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_10";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_10";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_SmithyLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "0";
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Enchanter):
                    trait.Name = "Enchantress";
                    //trait.Description = "Unlock the enchantress and gain access to her magical runes and powers.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_11";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_11";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_EnchanterLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "0";
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Architect):
                    trait.Name = "Architect";
                    //trait.Description = "Unlock the architect and gain the powers to lock down the castle.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_12";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_12";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_ArchitectLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "0";
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Equip_Up):
                    trait.Name = "Equip Up";
                    //trait.Description = "Upgrading your carry capacity will allow you to wear better and heavier armor.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_13";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_13";
                    trait.PerLevelModifier = 10f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 40;
                    trait.MaxLevel = 50;
                    trait.IconName = "Icon_Equip_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " weight";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_13" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Armor_Up):
                    trait.Name = "Armor Up";
                    //trait.Description = "Strengthen your innards through natural means to reduce incoming damage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_14";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_14";
                    trait.PerLevelModifier = 4f;
                    trait.BaseCost = 125;
                    trait.Appreciation = 105;
                    trait.MaxLevel = 50;
                    trait.IconName = "Icon_ShieldLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " armor";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_20" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Gold_Gain_Up):
                    trait.Name = "Gold Gain Up";
                    //trait.Description = "Improve your looting skills, and get more bang for your buck.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_15";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_15";
                    trait.PerLevelModifier = 0.1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 2150;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_Gold_Gain_UpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Prices_Down):
                    trait.Name = "Haggle";
                    //trait.Description = "Lower Charon's toll by learning how to barter with death itself.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_16";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_16";
                    trait.PerLevelModifier = 0.1f;
                    trait.BaseCost = 500;
                    trait.Appreciation = 1000;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_HaggleLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Potion_Up):
                    trait.Name = "Potion Up";
                    //trait.Description = "Gut cleansing leads to noticable improvements from both potions and meat.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_17";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_17";
                    trait.PerLevelModifier = 0.01f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 1750;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_PotionUpLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "% hp/mp";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " % ", "LOC_ID_SKILL_SCREEN_18" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "% ", "LOC_ID_SKILL_SCREEN_18" };
                            break;
                    }
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Randomize_Children):
                    trait.Name = "Randomize Children";
                    //trait.Description = "Use the power of science to make a whole new batch of babies. Just... don't ask.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_18";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_18";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 5000;
                    trait.Appreciation = 5000;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_RandomizeChildrenLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.MENU_MAP + "] to randomize your children";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.MENU_MAP + "] ", "LOC_ID_SKILL_INPUT_2" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_18";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = false;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Lich_Unlock):
                    trait.Name = "Unlock Lich";
                    //trait.Description = "Release the power of the Lich! A being of massive potential.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_19";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_19";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 850;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_LichUnlockLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Banker_Unlock):
                    trait.Name = "Unlock Miner";
                    //trait.Description = "Unlock the skills of the Miner and raise your family fortune.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_20";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_20";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 400;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_SpelunkerUnlockLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Spellsword_Unlock):
                    trait.Name = "Unlock Spell Thief";
                    //trait.Description = "Unlock the Spellthief, and become a martial  mage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_21";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_21";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 850;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_SpellswordUnlockLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Ninja_Unlock):
                    trait.Name = "Unlock Shinobi";
                    //trait.Description = "Unlock the Shinobi, the fleetest of fighters.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_22";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_22";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 400;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_NinjaUnlockLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Mana_Cost_Down):
                    trait.Name = "Mana Cost Down";
                    //trait.Description = "Practice your basics to reduce mana costs when casting spells.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_23";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_23";
                    trait.PerLevelModifier = 0.05f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 1700;
                    trait.MaxLevel = 5;
                    trait.IconName = "Icon_ManaCostDownLocked_Sprite";
                    //trait.InputDescription = " ";
                    //trait.InputDescLocIDs = new string[] { "", " ", "" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_SPACE";
                    trait.UnitOfMeasurement = "%";
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.French):
                            trait.UnitLocIDs = new string[] { " %", "" };
                            break;
                        default:
                            trait.UnitLocIDs = new string[] { "%", "" };
                            break;
                    }
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Knight_Up):
                    trait.Name = "Upgrade Knight";
                    //trait.Description = "Turn your knights into Paladins. A ferocious forefront fighter.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_24";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_24";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 50;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_KnightUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to block all incoming damage.";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_3" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_24";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Mage_Up):
                    trait.Name = "Upgrade Mage";
                    //trait.Description = "Unlock the latent powers of the Mage and transform them into the all powerful Archmage";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_25";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_25";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 300;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_WizardUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to switch spells";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_4" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_25";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Assassin_Up):
                    trait.Name = "Upgrade Knave";
                    //trait.Description = "Learn the dark arts, and turn the Knave into an Assassin";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_26";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_26";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 300;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_AssassinUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to turn to mist";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_5" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_26";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Banker_Up):
                    trait.Name = "Upgrade Miner";
                    //trait.Description = "Earn your geology degree and go from Miner to Spelunker. Spiffy.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_27";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_27";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1750;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_SpelunkerUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to turn on your headlamp";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_6" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_27";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Barbarian_Up):
                    trait.Name = "Upgrade Barbarian";
                    //trait.Description = "Become a Barbarian King.  The king of freemen. That makes no sense.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_28";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_28";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 300;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_BarbarianUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to cast an epic shout that knocks virtually everything away.";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_7" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_28";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Lich_Up):
                    trait.Name = "Upgrade Lich";
                    //trait.Description = "Royalize your all-powerful Liches, and turn them into Lich Kings.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_29";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_29";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1500;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_LichUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to convert max hp into max mp";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_8" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_29";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.Ninja_Up):
                    trait.Name = "Upgrade Shinobi";
                    //trait.Description = "Become the leader of your village, and turn your Shinobi into a Hokage. Believe it!";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_30";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_30";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 750;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_NinjaUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to flash";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_9" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_30";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                case (SkillType.SpellSword_Up):
                    trait.Name = "Upgrade Spell Thief";
                    //trait.Description = "Ride the vortexes of magic, and turn your Spellthiefs into Spellswords.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_31";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_31";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1500;
                    trait.Appreciation = 0;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_SpellswordUpLocked_Sprite";
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_BLOCK + "] to cast empowered spells";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_BLOCK + "] ", "LOC_ID_SKILL_INPUT_10" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_31";
                    trait.UnitOfMeasurement = " hp";
                    trait.UnitLocIDs = new string[] { " ", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    trait.StatType = TraitStatType.PlayerMaxHealth;
                    break;

                # endregion	























































                case (SkillType.SuperSecret):
                    trait.Name = "Beastiality";
                    //trait.Description = "Half man, half ******, all awesome.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_32";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_32";
                    trait.PerLevelModifier = 10f;
                    trait.BaseCost = 5000;//7500;//50;
                    trait.Appreciation = 30;
                    trait.MaxLevel = 1;
                    trait.IconName = "Icon_Display_Boss_RoomsLocked_Sprite"; //Icon_SuperSecretLocked_Sprite
                    //trait.InputDescription = "Press [Input:" + InputMapType.PLAYER_JUMP1 + "] to awesome.";
                    //trait.InputDescLocIDs = new string[] { "LOC_ID_SKILL_INPUT_1", " [Input:" + InputMapType.PLAYER_JUMP1 + "] ", "LOC_ID_SKILL_INPUT_11" };
                    trait.InputDescLocID = "LOC_ID_SKILL_INPUT_32";
                    trait.UnitOfMeasurement = "hp";
                    trait.UnitLocIDs = new string[] { "", "LOC_ID_SKILL_SCREEN_14" };
                    trait.DisplayStat = true;
                    break;










                case (SkillType.Quick_of_Breath):
                    trait.Name = "Quick of Breath";
                    //trait.Description = "QUICK OF BREATH \nYou're a heavy breather.  Bad for stalking, good for walking! \n\nIncrease your natural endurance regeneration.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_33";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_33";
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 500;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;


                case (SkillType.Born_to_Run):
                    trait.Name = "Born to Run";
                    //trait.Description = "You were infused with tiger blood at a young age.  You have now been infused with the power to release tiger blood when stabbed. \nRunning drains less endurance.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_34";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_34";
                    trait.Position = new Vector2(50, 100);
                    trait.PerLevelModifier = 0.01f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Stout_Heart):
                    trait.Name = "Stout Heart";
                    //trait.Description = "Your have viking ancestry.  \n\nIncrease your starting endurance.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_35";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_35";
                    trait.PerLevelModifier = 20f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 500;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;




                case (SkillType.Out_the_Gate):
                    trait.Name = "Out the Gate";
                    //trait.Description = "You're an early waker. If leveling was like waking up.\n Gain bonus HP and MP every time you level.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_36";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_36";
                    trait.PerLevelModifier = 35f;
                    trait.BaseCost = 2500;
                    trait.Appreciation = 5000;
                    trait.MaxLevel = 2;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;










                case (SkillType.Spell_Sword):
                    trait.Name = "Spellsword";
                    //trait.Description = "You were born with absolute power in your fingertips. \nAll spells deal more damage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_37";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_37";
                    trait.Position = new Vector2(100, 200);
                    trait.PerLevelModifier = 5f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Sorcerer):
                    trait.Name = "Sorcerer";
                    //trait.Description = "You were born with arcane energy coarsing through your veins.  Ow. \nSpells cost less to cast.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_38";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_38";
                    trait.Position = new Vector2(100, 250);
                    trait.PerLevelModifier = 5f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;




                case (SkillType.Perfectionist):
                    trait.Name = "Perfectionist";
                    //trait.Description = "OCD finally comes in handy. \nGain more gold.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_39";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_39";
                    trait.Position = new Vector2(150, 50);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 5;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Well_Endowed):
                    trait.Name = "Well Endowed";
                    //trait.Description = "By law, you are now the best man. \nGive birth to more children.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_40";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_40";
                    trait.Position = new Vector2(150, 100);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 2;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;







                case (SkillType.Treasure_Hunter):
                    trait.Name = "Treasure Hunter";
                    //trait.Description = "Your parents said learning how to sift for gold was useless for a farmer.  Whose laughing now? \n Display treasure rooms at the start of the game.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_41";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_41";
                    trait.Position = new Vector2(150, 250);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 2;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Mortar_Master):
                    trait.Name = "Mortar Master";
                    //trait.Description = "War is hell.  Luckily you were never in one. \n Fire more mortars.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_42";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_42";
                    trait.Position = new Vector2(150, 300);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 3;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Explosive_Expert):
                    trait.Name = "Explosive Expert";
                    //trait.Description = "As a child, you showed an affinity for blowing things up. \n Bombs have a larger radius.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_43";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_43";
                    trait.Position = new Vector2(200, 50);
                    trait.PerLevelModifier = 5f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 3;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Icicle):
                    trait.Name = "Icicle";
                    //trait.Description = "You're great grandfather was a snowman.  He taught you nothing. \n Icicles pierce through more enemies.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_44";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_44";
                    trait.Position = new Vector2(200, 100);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 3;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Guru):
                    trait.Name = "Guru";
                    //trait.Description = "You are Zen-like. \n Regain endurance faster while still.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_45";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_45";
                    trait.Position = new Vector2(50, 50);
                    trait.PerLevelModifier = 5f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Iron_Lung):
                    trait.Name = "Iron Lung";
                    //trait.Description = "Generic SKILL.  Increase total Endurance.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_46";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_46";
                    trait.Position = new Vector2(50, 200);
                    trait.PerLevelModifier = 25f;
                    trait.BaseCost = 500;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 50;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Sword_Master):
                    trait.Name = "Sword Master";
                    //trait.Description = "You fight with finesse \n Attacks Drain X% less endurance.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_47";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_47";
                    trait.Position = new Vector2(50, 150);
                    trait.PerLevelModifier = 0.1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 2;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Tank):
                    trait.Name = "Tank";
                    //trait.Description = "Generic SKILL.  Increase Health";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_48";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_48";
                    trait.Position = new Vector2(50, 200);
                    trait.PerLevelModifier = 25f;
                    trait.BaseCost = 500;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 50;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Vampire):
                    trait.Name = "Vampire";
                    //trait.Description = "You suck... Blood. \n Restore a small amount of life with every hit.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_49";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_49";
                    trait.Position = new Vector2(50, 250);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Second_Chance):
                    trait.Name = "Second Chance";
                    //trait.Description = "Come back to life, just like Jesus. But you're still not jesus. \n Revive once after dying.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_50";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_50";
                    trait.Position = new Vector2(50, 300);
                    trait.PerLevelModifier = 0.25f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Peace_of_Mind):
                    trait.Name = "Peace of Mind";
                    //trait.Description = "Clearing a room is like clearing your mind.  I don't know how. \nRegain helath for every room fully cleared.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_51";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_51";
                    trait.Position = new Vector2(50, 250);
                    trait.PerLevelModifier = 10f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Cartography_Ninja):
                    trait.Name = "Cartography Ninja";
                    //trait.Description = "Cartography /n Each percentage of map revealed adds 0.1 damage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_52";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_52";
                    trait.Position = new Vector2(100, 50);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Strong_Man):
                    trait.Name = "Strong Man";
                    //trait.Description = "Generic SKILL.  Increase Attack Damage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_53";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_53";
                    trait.Position = new Vector2(100, 50);
                    trait.PerLevelModifier = 2f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 50;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Suicidalist):
                    trait.Name = "Suicidalist";
                    //trait.Description = "You're a very, very sore loser. \n Deal massive damage to all enemies on screen upon death.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_54";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_54";
                    trait.Position = new Vector2(100, 100);
                    trait.PerLevelModifier = 100f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Crit_Barbarian):
                    trait.Name = "Crit Barbarian";
                    //trait.Description = "You have learned that hitting the balls deals massive damage. \n Crits deal more damage.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_55";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_55";
                    trait.Position = new Vector2(100, 150);
                    trait.PerLevelModifier = 0.1f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Magician):
                    trait.Name = "Magician";
                    //trait.Description = "GENERIC SKILL.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_56";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_56";
                    trait.Position = new Vector2(100, 250);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 700;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 50;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Keymaster):
                    trait.Name = "Keymaster";
                    //trait.Description = "Oh. They were in my back pocket. \nGain 2 extra keys.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_57";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_57";
                    trait.Position = new Vector2(100, 300);
                    trait.PerLevelModifier = 2f;
                    trait.BaseCost = 2000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.One_Time_Only):
                    trait.Name = "One Time Only";
                    //trait.Description = "Like a pheonix you are reborn from your crappy ashes. \n Regain all HP and MP.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_58";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_58";
                    trait.Position = new Vector2(150, 100);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Cutting_Out_Early):
                    trait.Name = "Cutting Out Early";
                    //trait.Description = "Retire, and invest your money wisely.  End your game early, and gain a bonus to gold found.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_59";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_59";
                    trait.Position = new Vector2(150, 100);
                    trait.PerLevelModifier = 0.25f;
                    trait.BaseCost = 100;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;

                case (SkillType.Quaffer):
                    trait.Name = "Quaffer";
                    //trait.Description = "CHUG CHUG CHUG! \n Drink potions instantly.";
                    trait.NameLocID = "LOC_ID_SKILL_NAME_60";
                    trait.DescLocID = "LOC_ID_SKILL_DESC_60";
                    trait.Position = new Vector2(150, 150);
                    trait.PerLevelModifier = 1f;
                    trait.BaseCost = 1000;
                    trait.Appreciation = 100;
                    trait.MaxLevel = 1;
                    trait.IconName = "IconBootLocked_Sprite";
                    break;
            }

            trait.TraitType = skillType;
            return trait;
        }

        public static SkillLinker GetSkillLinker(int xIndex, int yIndex)
        {
            SkillLinker link = new SkillLinker();

            if (xIndex == 5 && yIndex == 9)
            {
                link.TopLink = new Vector2(5, 8);
                link.BottomLink = new Vector2(6, 9);
                link.LeftLink = new Vector2(4, 5);
                link.RightLink = new Vector2(6, 5);
            }
            else if (xIndex == 8 && yIndex == 8)
            {
                link.TopLink = new Vector2(5, 9);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(2, 8);
                link.RightLink = new Vector2(8, 7);
            }
            else if (xIndex == 3 && yIndex == 4)
            {
                link.TopLink = new Vector2(2, 3);
                link.BottomLink = new Vector2(3, 6);
                link.LeftLink = new Vector2(2, 4);
                link.RightLink = new Vector2(5, 6);
            }
            else if (xIndex == 4 && yIndex == 6)
            {
                link.TopLink = new Vector2(3, 6);
                link.BottomLink = new Vector2(4, 5);
                link.LeftLink = new Vector2(1, 5);
                link.RightLink = new Vector2(5, 8);
            }
            else if (xIndex == 2 && yIndex == 8)
            {
                link.TopLink = new Vector2(4, 5);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(2, 7);
                link.RightLink = new Vector2(6, 9);
            }
            else if (xIndex == 2 && yIndex == 5)
            {
                link.TopLink = new Vector2(2, 4);
                link.BottomLink = new Vector2(2, 6);
                link.LeftLink = new Vector2(0, 5);
                link.RightLink = new Vector2(3, 6);
            }
            else if (xIndex == 1 && yIndex == 5)
            {
                link.TopLink = new Vector2(0, 5);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(0, 6);
                link.RightLink = new Vector2(4, 6);
            }
            else if (xIndex == 8 && yIndex == 5)
            {
                link.TopLink = new Vector2(8, 4);
                link.BottomLink = new Vector2(7, 6);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 2 && yIndex == 6)
            {
                link.TopLink = new Vector2(2, 5);
                link.BottomLink = new Vector2(2, 7);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(4, 5);
            }
            else if (xIndex == 5 && yIndex == 8)
            {
                link.TopLink = new Vector2(5, 7);
                link.BottomLink = new Vector2(5, 9);
                link.LeftLink = new Vector2(4, 6);
                link.RightLink = new Vector2(6, 6);
            }
            else if (xIndex == 5 && yIndex == 6)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(5, 7);
                link.LeftLink = new Vector2(3, 4);
                link.RightLink = new Vector2(8, 3);
            }
            else if (xIndex == 3 && yIndex == 6)
            {
                link.TopLink = new Vector2(3, 4);
                link.BottomLink = new Vector2(4, 6);
                link.LeftLink = new Vector2(2, 5);
                link.RightLink = new Vector2(5, 7);
            }
            else if (xIndex == 7 && yIndex == 6)
            {
                link.TopLink = new Vector2(8, 5);
                link.BottomLink = new Vector2(6, 6);
                link.LeftLink = new Vector2(5, 7);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 6 && yIndex == 6)
            {
                link.TopLink = new Vector2(7, 6);
                link.BottomLink = new Vector2(6, 5);
                link.LeftLink = new Vector2(5, 8);
                link.RightLink = new Vector2(8, 6);
            }
            else if (xIndex == 8 && yIndex == 6)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(8, 7);
                link.LeftLink = new Vector2(6, 6);
                link.RightLink = new Vector2(9, 6);
            }
            else if (xIndex == 8 && yIndex == 4)
            {
                link.TopLink = new Vector2(8, 1);
                link.BottomLink = new Vector2(8, 5);
                link.LeftLink = new Vector2(8, 3);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 8 && yIndex == 7)
            {
                link.TopLink = new Vector2(8, 6);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(6, 9);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 2 && yIndex == 7)
            {
                link.TopLink = new Vector2(2, 6);
                link.BottomLink = new Vector2(2, 7);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(2, 8);
            }
            else if (xIndex == 8 && yIndex == 3)
            {
                link.TopLink = new Vector2(8, 2);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(5, 6);
                link.RightLink = new Vector2(8, 4);
            }
            else if (xIndex == 8 && yIndex == 2)
            {
                link.TopLink = new Vector2(7, 2);
                link.BottomLink = new Vector2(8, 3);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(8, 1);
            }
            else if (xIndex == 9 && yIndex == 6)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(8, 6);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 2 && yIndex == 4)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(2, 5);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(3, 4);
            }
            else if (xIndex == 5 && yIndex == 7)
            {
                link.TopLink = new Vector2(5, 6);
                link.BottomLink = new Vector2(5, 8);
                link.LeftLink = new Vector2(3, 6);
                link.RightLink = new Vector2(7, 6);
            }
            else if (xIndex == 4 && yIndex == 5)
            {
                link.TopLink = new Vector2(4, 6);
                link.BottomLink = new Vector2(2, 8);
                link.LeftLink = new Vector2(2, 6);
                link.RightLink = new Vector2(5, 9);
            }
            else if (xIndex == 6 && yIndex == 5)
            {
                link.TopLink = new Vector2(6, 6);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(5, 9);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 2 && yIndex == 3)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(3, 4);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 7 && yIndex == 2)
            {
                link.TopLink = new Vector2(7, 1);
                link.BottomLink = new Vector2(8, 2);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 7 && yIndex == 1)
            {
                link.BottomLink = new Vector2(7, 2);
                link.TopLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(-1, -1);
            }
            else if (xIndex == 0 && yIndex == 5)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(1, 5);
                link.LeftLink = new Vector2(0, 6);
                link.RightLink = new Vector2(2, 5);
            }

            else if (xIndex == 0 && yIndex == 6)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(-11, -1);
                link.LeftLink = new Vector2(-1, -1);
                link.RightLink = new Vector2(0, 5);
            }
            else if (xIndex == 6 && yIndex == 9)
            {
                link.TopLink = new Vector2(5, 9);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(2, 8);
                link.RightLink = new Vector2(8, 7);
            }
            else if (xIndex == 8 && yIndex == 1)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(8, 4);
                link.LeftLink = new Vector2(8, 2);
                link.RightLink = new Vector2(8, 0);
            }
            else if (xIndex == 8 && yIndex == 0)
            {
                link.TopLink = new Vector2(-1, -1);
                link.BottomLink = new Vector2(-1, -1);
                link.LeftLink = new Vector2(8, 1);
                link.RightLink = new Vector2(-1, -1);
            }
            return link;
        }
    }
}
