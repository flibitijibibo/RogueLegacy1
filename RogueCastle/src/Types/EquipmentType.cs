using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EquipmentCategoryType
    {
        public const int Sword = 0;
        public const int Helm = 1;
        public const int Chest = 2;
        public const int Limbs = 3;
        public const int Cape = 4;

        public const int Total = 5;

        // English-only string needed by BlacksmithScreen for sprite name generation
        public static string ToStringEN(int equipmentType)
        {
            switch (equipmentType)
            {
                case (Sword):
                    return "Sword";
                case (Cape):
                    return "Cape";
                case (Limbs):
                    return "Limbs";
                case (Helm):
                    return "Helm";
                case (Chest):
                    return "Chest";
            }
            return "None";
        }

        public static string ToStringID(int equipmentType)
        {
            switch (equipmentType)
            {
                case (Sword):
                    return "LOC_ID_EQUIPMENT_CAT_1";
                case (Cape):
                    return "LOC_ID_EQUIPMENT_CAT_2";
                case (Limbs):
                    return "LOC_ID_EQUIPMENT_CAT_3";
                case (Helm):
                    return "LOC_ID_EQUIPMENT_CAT_4";
                case (Chest):
                    return "LOC_ID_EQUIPMENT_CAT_5";
            }
            return "LOC_ID_EQUIPMENT_CAT_6";
        }

        public static string ToStringID2(int equipmentType) // Sigh, my horrid architecture caused this.
        {
            switch (equipmentType)
            {
                case (Sword):
                    return "LOC_ID_EQUIPMENT_CAT2_1";
                case (Cape):
                    return "LOC_ID_EQUIPMENT_CAT2_2";
                case (Limbs):
                    return "LOC_ID_EQUIPMENT_CAT2_3";
                case (Helm):
                    return "LOC_ID_EQUIPMENT_CAT2_4";
                case (Chest):
                    return "LOC_ID_EQUIPMENT_CAT2_5";
            }
            return "LOC_ID_EQUIPMENT_CAT2_6";
        }
    }

    public class EquipmentState
    {
        public const int NotFound = 0;
        public const int FoundButNotSeen = 1;
        public const int FoundAndSeen = 2;
        public const int Purchased = 3;
        //public const int PurchasedAndEquipped = 4;
    }

    public class EquipmentAbilityType
    {
        public const int DoubleJump = 0;
        public const int Dash = 1;
        public const int Vampirism = 2;
        public const int Flight = 3;
        public const int ManaGain = 4;
        public const int DamageReturn = 5;
        public const int GoldGain = 6;
        public const int MovementSpeed = 7;
        public const int RoomLevelUp = 8;
        public const int RoomLevelDown = 9;
        public const int ManaHPGain = 10;

        public const int Total = 11;

        // Special rune gained only through locking the castle.
        public const int ArchitectFee = 20;

        // Special rune gained only through beating the castle at least once.
        public const int NewGamePlusGoldBonus = 21;

        //public static string ToString(int type)
        //{
        //    return "";
        //}

        public static string ToStringID(int type)
        {
            switch (type)
            {
                case (DoubleJump):
                    return "LOC_ID_EQUIPMENT_ABILITY_1";
                case (Dash):
                    return "LOC_ID_EQUIPMENT_ABILITY_2";
                case (Vampirism):
                    return "LOC_ID_EQUIPMENT_ABILITY_3";
                case (Flight):
                    return "LOC_ID_EQUIPMENT_ABILITY_4";
                case (ManaGain):
                    return "LOC_ID_EQUIPMENT_ABILITY_5";
                case (ManaHPGain):
                    return "LOC_ID_EQUIPMENT_ABILITY_6";
                case (DamageReturn):
                    return "LOC_ID_EQUIPMENT_ABILITY_7";
                case (GoldGain):
                    return "LOC_ID_EQUIPMENT_ABILITY_8";
                case (MovementSpeed):
                    return "LOC_ID_EQUIPMENT_ABILITY_9";
                case (RoomLevelUp):
                    return "LOC_ID_EQUIPMENT_ABILITY_10";
                case (RoomLevelDown):
                    return "LOC_ID_EQUIPMENT_ABILITY_11";
                case (ArchitectFee):
                    return "LOC_ID_EQUIPMENT_ABILITY_12";
                case(NewGamePlusGoldBonus):
                    return "LOC_ID_EQUIPMENT_ABILITY_13";
            }

            return "";
        }

        public static string ToStringID2(int type)
        {
            switch (type)
            {
                case (DoubleJump):
                    return "LOC_ID_EQUIPMENT_ABILITY2_1";
                case (Dash):
                    return "LOC_ID_EQUIPMENT_ABILITY2_2";
                case (Vampirism):
                    return "LOC_ID_EQUIPMENT_ABILITY2_3";
                case (Flight):
                    return "LOC_ID_EQUIPMENT_ABILITY2_4";
                case (ManaGain):
                    return "LOC_ID_EQUIPMENT_ABILITY2_5";
                case (ManaHPGain):
                    return "LOC_ID_EQUIPMENT_ABILITY2_6";
                case (DamageReturn):
                    return "LOC_ID_EQUIPMENT_ABILITY2_7";
                case (GoldGain):
                    return "LOC_ID_EQUIPMENT_ABILITY2_8";
                case (MovementSpeed):
                    return "LOC_ID_EQUIPMENT_ABILITY2_9";
                case (RoomLevelUp):
                    return "LOC_ID_EQUIPMENT_ABILITY2_10";
                case (RoomLevelDown):
                    return "LOC_ID_EQUIPMENT_ABILITY2_11";
                case (ArchitectFee):
                    return "LOC_ID_EQUIPMENT_ABILITY2_12";
                case (NewGamePlusGoldBonus):
                    return "LOC_ID_EQUIPMENT_ABILITY2_13";
            }

            return "";
        }

        public static string DescriptionID(int type)
        {
            switch (type)
            {
                case (DoubleJump):
                    return "LOC_ID_EQUIPMENT_DESC_1";
                case (Dash):
                    return "LOC_ID_EQUIPMENT_DESC_2";
                case (Vampirism):
                    return "LOC_ID_EQUIPMENT_DESC_3";
                case (Flight):
                    return "LOC_ID_EQUIPMENT_DESC_4";
                case (ManaGain):
                    return "LOC_ID_EQUIPMENT_DESC_5";
                case (ManaHPGain):
                    return "LOC_ID_EQUIPMENT_DESC_6";
                case (DamageReturn):
                    return "LOC_ID_EQUIPMENT_DESC_7";
                case (GoldGain):
                    return "LOC_ID_EQUIPMENT_DESC_8";
                case (MovementSpeed):
                    return "LOC_ID_EQUIPMENT_DESC_9";
                case (RoomLevelUp):
                    return "LOC_ID_EQUIPMENT_DESC_10";
                case (RoomLevelDown):
                    return "LOC_ID_EQUIPMENT_DESC_11";
            }

            return "";
        }

        // Returns localized string. This is safe because calling function LoadBackCardStats()
        // is refreshed on language change.
        public static string ShortDescription(int type, float amount)
        {
            switch (type)
            {
                case (DoubleJump):
                    if (amount > 1)
                        return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1_NEW_A"), amount);
                        //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1b");
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1_NEW_B"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_1c");
                case (Dash):
                    if (amount > 1)
                        return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2_NEW_A"), amount);
                        //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2b");
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2_NEW_B"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_2c");
                case (Vampirism):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_3_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_3") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_3b");
                case (Flight):
                    if (amount > 1)
                        return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4_NEW_A"), amount);
                        //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4b");
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4_NEW_B"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_4c");
                case (ManaGain):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_5_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_5") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_5b");
                case (ManaHPGain):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_6");
                case (DamageReturn):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_7_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_7") + " " + amount + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_7b");
                case (GoldGain):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_8_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_8") + " " + amount + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_8b");
                case (MovementSpeed):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_9_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_9") + " " + amount + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_9b");
                case (ArchitectFee):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_10");
                case (NewGamePlusGoldBonus):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_11_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_11") + " " + amount + "%";
                case (RoomLevelUp):
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_12_NEW"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_12") + " " + (int)((amount / LevelEV.ROOM_LEVEL_MOD) * LevelEV.ENEMY_LEVEL_FAKE_MULTIPLIER) + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_12b");
                case (RoomLevelDown):
                    if (amount > 1)
                        return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13_NEW_A"), amount);
                        //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13b");
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13_NEW_B"), amount);
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13") + " " + amount + " " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_SHORT_13c");
            }

            return "";
        }

        // Returns localized string. This is safe because calling function UpdateEquipmentDataText()
        // is refreshed on language change.
        public static string Instructions(int type)
        {
            switch (type)
            {
                case (DoubleJump):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_1_NEW");
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_1") + " [Input:" + InputMapType.PLAYER_JUMP1 + "] " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_1b");
                    //return "[Input:" + InputMapType.PLAYER_JUMP1 + "] " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_1b");
                case (Dash):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_2_NEW");
                    //return "[Input:" + InputMapType.PLAYER_DASHLEFT + "] " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_2") + " [Input:" + InputMapType.PLAYER_DASHRIGHT + "] " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_2b");
                    //return "[Input:" + InputMapType.PLAYER_DASHLEFT + "] or [Input:" + InputMapType.PLAYER_DASHRIGHT + "] to blah blah blah";
                case (Vampirism):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_3");
                case (Flight):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_4_NEW");
                    //return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_4") + " [Input:" + InputMapType.PLAYER_JUMP1 + "] " + LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_4b");
                case (ManaGain):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_5");
                case (ManaHPGain):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_6");
                case (DamageReturn):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_7");
                case (GoldGain):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_8");
                case (MovementSpeed):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_9");
                case (RoomLevelUp):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_10");
                case (RoomLevelDown):
                    return LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_INST_11");
            }

            return "";
        }

        public static string Icon(int type)
        {
            switch (type)
            {
                case (DoubleJump):
                    return "EnchantressUI_DoubleJumpIcon_Sprite";
                case (Dash):
                    return "EnchantressUI_DashIcon_Sprite";
                case (Vampirism):
                    return "EnchantressUI_VampirismIcon_Sprite";
                case (Flight):
                    return "EnchantressUI_FlightIcon_Sprite";
                case (ManaGain):
                    return "EnchantressUI_ManaGainIcon_Sprite";
                case (ManaHPGain):
                    return "EnchantressUI_BalanceIcon_Sprite";
                case (DamageReturn):
                    return "EnchantressUI_DamageReturnIcon_Sprite";
                case (GoldGain):
                    return "Icon_Gold_Gain_Up_Sprite";
                case (MovementSpeed):
                    return "EnchantressUI_SpeedUpIcon_Sprite";
                case (RoomLevelUp):
                    return "EnchantressUI_CurseIcon_Sprite";
                case (RoomLevelDown):
                    return "EnchantressUI_BlessingIcon_Sprite";
            }

            return "";
        }

    }

    public class EquipmentBaseType
    {
        public const int Bronze = 0;
        public const int Silver = 1;
        public const int Gold = 2;
        public const int Imperial = 3;
        public const int Royal = 4;

        public const int Knight = 5;
        public const int Earthen = 6;
        public const int Sky = 7;
        public const int Dragon = 8;
        public const int Eternal = 9;

        public const int Blood = 10;
        public const int Amethyst = 11;
        public const int Spike = 12;
        public const int Holy = 13;
        public const int Dark = 14;

        public const int Total = 15;

        public static string ToStringID(int equipmentBaseType)
        {
            switch (equipmentBaseType)
            {
                case (Bronze):
                    return "LOC_ID_EQUIPMENT_BASE_1";
                case (Silver):
                    return "LOC_ID_EQUIPMENT_BASE_2";
                case (Gold):
                    return "LOC_ID_EQUIPMENT_BASE_3";
                case (Imperial):
                    return "LOC_ID_EQUIPMENT_BASE_4";
                case (Royal):
                    return "LOC_ID_EQUIPMENT_BASE_5";
                case (Knight):
                    return "LOC_ID_EQUIPMENT_BASE_6";
                case (Earthen):
                    return "LOC_ID_EQUIPMENT_BASE_7";
                case (Sky):
                    return "LOC_ID_EQUIPMENT_BASE_8";
                case (Dragon):
                    return "LOC_ID_EQUIPMENT_BASE_9";
                case (Eternal):
                    return "LOC_ID_EQUIPMENT_BASE_10";
                case (Amethyst):
                    return "LOC_ID_EQUIPMENT_BASE_11";
                case (Blood):
                    return "LOC_ID_EQUIPMENT_BASE_12";
                case (Spike):
                    return "LOC_ID_EQUIPMENT_BASE_13";
                case (Holy):
                    return "LOC_ID_EQUIPMENT_BASE_14";
                case (Dark):
                    return "LOC_ID_EQUIPMENT_BASE_15";
            }
            return "";
        }
    }

    public class EquipmentData
    {
        public int BonusDamage;
        public int BonusMagic;
        public int Weight;
        public int BonusMana;
        public int BonusHealth;
        public int BonusArmor;
        public int Cost = 9999;
        public Color FirstColour = Color.White;
        public Color SecondColour = Color.White;
        public Vector2[] SecondaryAttribute = null;
        public byte ChestColourRequirement = 0;
        public byte LevelRequirement = 0;

        public void Dispose()
        {
            if (SecondaryAttribute != null)
                Array.Clear(SecondaryAttribute, 0, SecondaryAttribute.Length);
            SecondaryAttribute = null;
        }
    }

    public class EquipmentSecondaryDataType
    {
        public const int None = 0;
        public const int CritChance = 1;
        public const int CritDamage = 2;
        public const int GoldBonus = 3;
        public const int DamageReturn = 4;
        public const int XpBonus = 5;
        // Anything above this line will be displayed as a percent.
        public const int AirAttack = 6;
        public const int Vampirism = 7;
        public const int ManaDrain = 8;
        public const int DoubleJump = 9;
        public const int MoveSpeed = 10;
        public const int AirDash = 11;
        public const int Block = 12;
        public const int Float = 13;
        public const int AttackProjectiles = 14;
        public const int Flight = 15;

        public const int Total = 16;

        public static string ToStringID(int equipmentSecondaryDataType)
        {
            switch (equipmentSecondaryDataType)
            {
                case (CritChance):
                    return "LOC_ID_EQUIPMENT_SEC_1";
                case (CritDamage):
                    return "LOC_ID_EQUIPMENT_SEC_2";
                case (Vampirism):
                    return "LOC_ID_EQUIPMENT_SEC_3";
                case (GoldBonus):
                    return "LOC_ID_EQUIPMENT_SEC_4";
                case (ManaDrain):
                    return "LOC_ID_EQUIPMENT_SEC_5";
                case (XpBonus):
                    return "LOC_ID_EQUIPMENT_SEC_6";
                case (AirAttack):
                    return "LOC_ID_EQUIPMENT_SEC_7";
                case (DoubleJump):
                    return "LOC_ID_EQUIPMENT_SEC_8";
                case (DamageReturn):
                    return "LOC_ID_EQUIPMENT_SEC_9";
                case (AirDash):
                    return "LOC_ID_EQUIPMENT_SEC_10";
                case (Block):
                    return "LOC_ID_EQUIPMENT_SEC_11";
                case (Float):
                    return "LOC_ID_EQUIPMENT_SEC_12";
                case (AttackProjectiles):
                    return "LOC_ID_EQUIPMENT_SEC_13";
                case (Flight):
                    return "LOC_ID_EQUIPMENT_SEC_14";
                case (MoveSpeed):
                    return "LOC_ID_EQUIPMENT_SEC_15";
            }
            return "LOC_ID_EQUIPMENT_SEC_16";
        }
    }
}
