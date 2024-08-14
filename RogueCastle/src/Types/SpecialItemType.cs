using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    class SpecialItemType
    {
        public const byte None = 0;
        public const byte FreeEntrance = 1; // Done
        public const byte LoseCoins = 2;  // Done
        public const byte Revive = 3; // Done
        public const byte SpikeImmunity = 4; // Done
        public const byte GoldPerKill = 5; // Done
        public const byte Compass = 6; // Done

        public const byte Total = 7;

        public const byte Glasses = 8; // Done. Don't include glasses on the list because it needs to be hard coded in.

        public const byte EyeballToken = 9;
        public const byte SkullToken = 10;
        public const byte FireballToken = 11;
        public const byte BlobToken = 12;
        public const byte LastBossToken = 13;

        public static string ToStringID(byte itemType)
        {
            switch (itemType)
            {
                case (Revive):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_1";
                case (SpikeImmunity):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_2";
                case (LoseCoins):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_3";
                case (FreeEntrance):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_4";
                case (Compass):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_5";
                case (GoldPerKill):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_6";
                case (Glasses):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_7";
                case (EyeballToken):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_8";
                case (SkullToken):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_9";
                case (FireballToken):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_10";
                case (BlobToken):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_11";
                case (LastBossToken):
                    return "LOC_ID_SPECIAL_ITEM_TYPE_12";
            }
            return "";
        }

        public static string SpriteName(byte itemType)
        {
            switch (itemType)
            {
                case (Revive):
                    return "BonusRoomRingIcon_Sprite";
                case (SpikeImmunity):
                    return "BonusRoomBootsIcon_Sprite";
                case (LoseCoins):
                    return "BonusRoomHedgehogIcon_Sprite";
                case(FreeEntrance):
                    return "BonusRoomObolIcon_Sprite";
                case (Compass):
                    return "BonusRoomCompassIcon_Sprite";
                case (GoldPerKill):
                    return "BonusRoomBlessingIcon_Sprite";
                case(Glasses):
                    return "BonusRoomGlassesIcon_Sprite";
                case (EyeballToken):
                    return "ChallengeIcon_Eyeball_Sprite";
                case (SkullToken):
                    return "ChallengeIcon_Skull_Sprite";
                case (FireballToken):
                    return "ChallengeIcon_Fireball_Sprite";
                case (BlobToken):
                    return "ChallengeIcon_Blob_Sprite";
                case (LastBossToken):
                    return "ChallengeIcon_LastBoss_Sprite";
            }
            return "";
        }
    }
}
