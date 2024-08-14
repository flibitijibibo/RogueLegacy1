using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    public class ChestConditionType
    {
        public const int None = 0;
        public const int KillAllEnemies = 1;
        public const int HealthBelow15 = 2;
        public const int DontLook = 3;
        public const int NoJumping = 4;
        public const int NoSound = 5;
        public const int NoFloor = 6;
        public const int NoAttackingEnemies = 7;
        public const int ReachIn5Seconds = 8;
        public const int TakeNoDamage = 9;
        public const int InvisibleChest = 10;
    }
}