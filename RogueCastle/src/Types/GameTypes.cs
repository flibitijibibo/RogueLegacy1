using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    public class GameTypes
    {
        public enum EnemyDifficulty
        {
            BASIC,
            ADVANCED,
            EXPERT,
            MINIBOSS
        }

        public enum DoorType
        {
            NULL,
            OPEN,
            LOCKED,
            BLOCKED
        }

        public enum LevelType
        {
            NONE,
            CASTLE,
            GARDEN,
            DUNGEON,
            TOWER
        }

        public enum WeaponType
        {
            NONE,
            DAGGER,
            SWORD,
            SPEAR,
            AXE,
        }

        public enum ArmorType
        {
            NONE,
            HEAD,
            BODY,
            RING,
            FOOT,
            HAND,
            ALL
        }

        public enum EquipmentType
        {
            NONE,
            WEAPON,
            ARMOR
        }

        public enum StatType
        {
            STRENGTH = 0,
            HEALTH = 1,
            ENDURANCE = 2,
            EQUIPLOAD = 3,
        }

        public enum SkillType
        {
            STRENGTH = 0,
            HEALTH,
            DEFENSE,
        }

        // CollisionType represents the type of object you are colliding with.
        public const int CollisionType_NULL = 0;
        public const int CollisionType_WALL = 1;
        public const int CollisionType_PLAYER= 2;
        public const int CollisionType_ENEMY = 3;
        public const int CollisionType_ENEMYWALL = 4; // An enemy that you cannot walk through while invincible.
        public const int CollisionType_WALL_FOR_PLAYER = 5;
        public const int CollisionType_WALL_FOR_ENEMY = 6;
        public const int CollisionType_PLAYER_TRIGGER = 7;
        public const int CollisionType_ENEMY_TRIGGER = 8;
        public const int CollisionType_GLOBAL_TRIGGER = 9;
        public const int CollisionType_GLOBAL_DAMAGE_WALL = 10;

        public const int LogicSetType_NULL = 0;
        public const int LogicSetType_NONATTACK = 1;
        public const int LogicSetType_ATTACK = 2;
        public const int LogicSetType_CD = 3;
    }
}
