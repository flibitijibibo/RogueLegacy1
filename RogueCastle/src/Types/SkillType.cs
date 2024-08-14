using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    public enum SkillType
    {
        Null = 0,
        Filler,
        Health_Up, // Done
        Invuln_Time_Up, // Done
        Death_Dodge, // Done

        Attack_Up, // Done
        Down_Strike_Up, // Done
        Crit_Chance_Up, // Done
        Crit_Damage_Up, // Done

        Magic_Damage_Up, // Done
        Mana_Up, // Done
        Mana_Cost_Down, // Done

        Smithy, // Done
        Enchanter, // Done
        Architect, //Done

        Equip_Up, // Done
        Armor_Up, // Done

        Gold_Gain_Up, // Done
        Prices_Down,

        Potion_Up, // Done
        Randomize_Children,

        Lich_Unlock,
        Banker_Unlock,
        Spellsword_Unlock,
        Ninja_Unlock,

        Knight_Up,
        Mage_Up,
        Assassin_Up,
        Banker_Up,
        Barbarian_Up,
        Lich_Up,
        Ninja_Up,
        SpellSword_Up,

        SuperSecret,


        DIVIDER, // NO LONGER USED // This separates the traits from the skills.
        Attack_Speed_Up, // Done
        Invuln_Attack_Up, // Done
        Health_Up_Final,
        Equip_Up_Final,
        Damage_Up_Final,
        Mana_Up_Final,
        XP_Gain_Up, // Done
        Gold_Flat_Bonus, // Done
        Mana_Regen_Up,
        Run,
        Block,
        Cartographer,
        Env_Damage_Down,
        Gold_Loss_Down,
        Vampire_Up,

        Stout_Heart, // Done
        Quick_of_Breath, // Done
        Born_to_Run, // Done
        Out_the_Gate, // Done HP portion only since MP isn't implemented yet.
        Perfectionist, // Done

        Guru,
        Iron_Lung,
        Sword_Master,
        Tank,
        Vampire,
        Second_Chance,
        Peace_of_Mind,
        Cartography_Ninja,
        Strong_Man,
        Suicidalist,
        Crit_Barbarian,
        Magician,
        Keymaster,
        One_Time_Only,
        Cutting_Out_Early,
        Quaffer,

        Spell_Sword,
        Sorcerer,

        Well_Endowed,
        Treasure_Hunter,
        Mortar_Master,
        Explosive_Expert,
        Icicle,
        ENDER,
    }

    public class TraitState
    {
        public const byte Invisible = 0;
        public const byte Purchasable = 1;
        public const byte Purchased = 2;
        public const byte MaxedOut = 3;
    }

    class TraitStatType
    {
        public const int PlayerMaxHealth = 0; //  A dummy variable used for property in TraitObj called StatType that is no longer used.

        public static float GetTraitStat(SkillType traitType)
        {
            switch (traitType)
            {
                case (SkillType.Health_Up_Final):
                case (SkillType.Health_Up):
                    return Game.ScreenManager.Player.MaxHealth;
                case (SkillType.Invuln_Time_Up):
                    return Game.ScreenManager.Player.InvincibilityTime;
                case (SkillType.Death_Dodge):
                    return SkillSystem.GetSkill(SkillType.Death_Dodge).ModifierAmount * 100;
                case (SkillType.Damage_Up_Final):
                case (SkillType.Attack_Up):
                    return Game.ScreenManager.Player.Damage;
                case (SkillType.Crit_Chance_Up):
                    return Game.ScreenManager.Player.TotalCritChance;
                case (SkillType.Crit_Damage_Up):
                    return Game.ScreenManager.Player.TotalCriticalDamage * 100;
                case (SkillType.Mana_Up_Final):
                case (SkillType.Mana_Up):
                    return Game.ScreenManager.Player.MaxMana;
                case (SkillType.Mana_Regen_Up):
                    return Game.ScreenManager.Player.ManaGain;
                case (SkillType.Equip_Up_Final):
                case (SkillType.Equip_Up):
                    return Game.ScreenManager.Player.MaxWeight;
                case (SkillType.Armor_Up):
                    return Game.ScreenManager.Player.TotalArmor;
                case (SkillType.Gold_Gain_Up):
                    return Game.ScreenManager.Player.TotalGoldBonus;
                case (SkillType.XP_Gain_Up):
                    return Game.ScreenManager.Player.TotalXPBonus;
                case (SkillType.Mana_Cost_Down):
                    return SkillSystem.GetSkill(SkillType.Mana_Cost_Down).ModifierAmount * 100; //Game.ScreenManager.Player.TotalXPBonus;
                case (SkillType.Attack_Speed_Up):
                    return SkillSystem.GetSkill(SkillType.Attack_Speed_Up).ModifierAmount * 10;
                case (SkillType.Magic_Damage_Up):
                    return Game.ScreenManager.Player.TotalMagicDamage;
                case (SkillType.Potion_Up):
                    return (GameEV.ITEM_HEALTHDROP_AMOUNT + SkillSystem.GetSkill(SkillType.Potion_Up).ModifierAmount) * 100;
                case (SkillType.Prices_Down):
                    return SkillSystem.GetSkill(SkillType.Prices_Down).ModifierAmount * 100;
                case (SkillType.Down_Strike_Up):
                    return SkillSystem.GetSkill(SkillType.Down_Strike_Up).ModifierAmount * 100;
            }

            return -1;
        }
    }
}
