using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class PlayerPart
    {
        public const int None = -1;
        public const int Wings = 0;
        public const int Cape = 1;
        public const int Legs = 2;
        public const int ShoulderB = 3;
        public const int Chest = 4;
        public const int Boobs = 5;
        public const int Arms = 6;
        public const int Hair = 7;
        public const int Neck = 8;
        public const int ShoulderA = 9;
        public const int Sword1 = 10;
        public const int Sword2 = 11;
        public const int Head = 12;
        public const int Bowtie = 13;
        public const int Glasses = 14;
        public const int Extra = 15;
        public const int Light = 16;

        public const int NumHeadPieces = 5;
        public const int NumChestPieces = 5;
        public const int NumShoulderPieces = 5;

        public const int DragonHelm = 6;
        public const int IntroHelm = 7;

        public static Vector3 GetPartIndices(int category)
        {
            switch (category)
            {
                case (EquipmentCategoryType.Cape):
                    return new Vector3(PlayerPart.Cape, PlayerPart.Neck, PlayerPart.None);
                case (EquipmentCategoryType.Chest):
                    return new Vector3(PlayerPart.Chest, PlayerPart.ShoulderB, PlayerPart.ShoulderA);
                case (EquipmentCategoryType.Helm):
                    return new Vector3(PlayerPart.Head, PlayerPart.Hair, PlayerPart.None);
                case (EquipmentCategoryType.Limbs):
                    return new Vector3(PlayerPart.Arms, PlayerPart.Legs, PlayerPart.None);
                case (EquipmentCategoryType.Sword):
                    return new Vector3(PlayerPart.Sword1, PlayerPart.Sword2, PlayerPart.None);
            }
            return new Vector3(-1, -1, -1);
        }
    }
}
