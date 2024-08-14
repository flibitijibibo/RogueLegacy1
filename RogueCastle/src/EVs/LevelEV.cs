using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    public class LevelEV
    {
        public const int ENEMY_LEVEL_DIFFICULTY_MOD = 32;//30; //40;//30;//15;//6;//12; // The number of levels an enemy needs to be before he goes to the next difficulty.
        public const float ENEMY_LEVEL_FAKE_MULTIPLIER = 2.75f;//3.0f;//2.75f;//2.5f;//2.25f;//This multiplies the enemy level by a fake amount in order to keep it more in par with how close we want the player to be when engaging.
        public const int ROOM_LEVEL_MOD = 4;//6; //The number of rooms that have to be generated before the room raises a difficulty level (raising enemy lvl).
        public const byte TOTAL_JOURNAL_ENTRIES = 25;//5;//19;//15; // This needs to be kept up-to-date.

        public const int ENEMY_EXPERT_LEVEL_MOD = 4; //3; //The number of bonus levels an expert enemy gains over the current room level mod.
        public const int ENEMY_MINIBOSS_LEVEL_MOD = 7;//8;//5; //The number of bonus levels the BOSSES GAIN over the current room level mod.

        public const int LAST_BOSS_MODE1_LEVEL_MOD = 8; //ADDS DIRECTLY TO THE LEVEL, IGNORING ROOM_LEVEL_MOD.
        public const int LAST_BOSS_MODE2_LEVEL_MOD = 10; //ADDS DIRECTLY TO THE LEVEL, IGNORING ROOM_LEVEL_MOD.

        //////////////////////////////////////////////////////////////

        // EVs related to new game +

        // These are room levels prior to dividing by ROOM_LEVEL_MOD.
        public const int CASTLE_ROOM_LEVEL_BOOST = 0;
        public const int GARDEN_ROOM_LEVEL_BOOST = 2;//3;//5;//10;
        public const int TOWER_ROOM_LEVEL_BOOST = 4;//6;//10;//20;
        public const int DUNGEON_ROOM_LEVEL_BOOST = 6;//9;//15;//30;

        public const int NEWGAMEPLUS_LEVEL_BASE = 128;//120;//120;//10;
        public const int NEWGAMEPLUS_LEVEL_APPRECIATION = 128;//80; //60;
        public const int NEWGAMEPLUS_MINIBOSS_LEVEL_BASE = 0;
        public const int NEWGAMEPLUS_MINIBOSS_LEVEL_APPRECIATION = 0;

        //////////////////////////////////////////////////////////////

        public const bool LINK_TO_CASTLE_ONLY = true;
        public const byte CASTLE_BOSS_ROOM = BossRoomType.EyeballBossRoom;
        public const byte TOWER_BOSS_ROOM = BossRoomType.FireballBossRoom;
        public const byte DUNGEON_BOSS_ROOM = BossRoomType.BlobBossRoom;
        public const byte GARDEN_BOSS_ROOM = BossRoomType.FairyBossRoom;
        public const byte LAST_BOSS_ROOM = BossRoomType.LastBossRoom;

        ///////////////////////////////////////////////////////////////
        //The list of enemies that each area will randomly add enemies to.


        public static byte[] DEMENTIA_FLIGHT_LIST = new byte[] { EnemyType.FireWizard, EnemyType.IceWizard, EnemyType.Eyeball, EnemyType.Fairy, EnemyType.BouncySpike, EnemyType.Fireball, EnemyType.Starburst };
        public static byte[] DEMENTIA_GROUND_LIST = new byte[] { EnemyType.Skeleton, EnemyType.Knight, EnemyType.Blob, EnemyType.BallAndChain, EnemyType.SwordKnight, EnemyType.Zombie, EnemyType.Ninja, EnemyType.Plant, EnemyType.HomingTurret, EnemyType.Horse, };

        public static byte[] CASTLE_ENEMY_LIST = new byte[] { EnemyType.Skeleton, EnemyType.Knight, EnemyType.FireWizard, EnemyType.IceWizard, EnemyType.Eyeball, EnemyType.BouncySpike, EnemyType.SwordKnight, EnemyType.Zombie, EnemyType.Fireball, EnemyType.Portrait, EnemyType.Starburst, EnemyType.HomingTurret, };
        public static byte[] GARDEN_ENEMY_LIST = new byte[] { EnemyType.Skeleton, EnemyType.Blob, EnemyType.BallAndChain, EnemyType.EarthWizard, EnemyType.FireWizard, EnemyType.Eyeball, EnemyType.Fairy, EnemyType.ShieldKnight, EnemyType.BouncySpike, EnemyType.Wolf, EnemyType.Plant, EnemyType.SkeletonArcher, EnemyType.Starburst, EnemyType.Horse, };
        public static byte[] TOWER_ENEMY_LIST = new byte[] { EnemyType.Knight, EnemyType.BallAndChain, EnemyType.IceWizard, EnemyType.Eyeball, EnemyType.Fairy, EnemyType.ShieldKnight, EnemyType.BouncySpike, EnemyType.Wolf, EnemyType.Ninja, EnemyType.Plant, EnemyType.Fireball, EnemyType.SkeletonArcher, EnemyType.Portrait, EnemyType.Starburst, EnemyType.HomingTurret, EnemyType.Mimic, };
        public static byte[] DUNGEON_ENEMY_LIST = new byte[] { EnemyType.Skeleton, EnemyType.Knight, EnemyType.Blob, EnemyType.BallAndChain, EnemyType.EarthWizard, EnemyType.FireWizard, EnemyType.IceWizard, EnemyType.Eyeball, EnemyType.Fairy, EnemyType.BouncySpike, EnemyType.SwordKnight, EnemyType.Zombie, EnemyType.Ninja, EnemyType.Plant, EnemyType.Fireball, EnemyType.Starburst, EnemyType.HomingTurret, EnemyType.Horse, };

        public static byte[] CASTLE_ENEMY_DIFFICULTY_LIST = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static byte[] GARDEN_ENEMY_DIFFICULTY_LIST = new byte[] { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static byte[] TOWER_ENEMY_DIFFICULTY_LIST = new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, };
        public static byte[] DUNGEON_ENEMY_DIFFICULTY_LIST = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, };


        //EVERY CURRENT ENEMY
        //{ EnemyType.BallAndChain, EnemyType.Blob, EnemyType.BouncySpike, EnemyType.Eagle, EnemyType.EarthWizard, EnemyType.Energon, EnemyType.Eyeball, EnemyType.Fairy, EnemyType.Fireball, EnemyType.FireWizard, EnemyType.HomingTurret, EnemyType.Horse, EnemyType.IceWizard, EnemyType.Knight, EnemyType.Ninja, EnemyType.Plant, EnemyType.ShieldKnight, EnemyType.Skeleton, EnemyType.SkeletonArcher, EnemyType.Spark, EnemyType.SpikeTrap, EnemyType.SwordKnight, EnemyType.Wolf, EnemyType.Zombie };  

        public static string[] CASTLE_ASSETSWAP_LIST = new string[] { "BreakableBarrel1_Character", "BreakableBarrel2_Character", "CastleAssetKnightStatue_Character", "CastleAssetWindow1_Sprite", "CastleAssetWindow2_Sprite", "CastleBGPillar_Character", "CastleAssetWeb1_Sprite", "CastleAssetWeb2_Sprite", "CastleAssetBackTorch_Character", "CastleAssetSideTorch_Character", "CastleAssetChandelier1_Character", "CastleAssetChandelier2_Character", "CastleAssetCandle1_Character", "CastleAssetCandle2_Character", "CastleAssetFireplace_Character", "CastleAssetBookcase_Sprite", "CastleAssetBookCase2_Sprite", "CastleAssetBookCase3_Sprite", "CastleAssetUrn1_Character", "CastleAssetUrn2_Character", "BreakableChair1_Character", "BreakableChair2_Character", "CastleAssetTable1_Character", "CastleAssetTable2_Character", "CastleDoorOpen_Sprite", "CastleAssetFrame_Sprite" };
        public static string[] DUNGEON_ASSETSWAP_LIST = new string[] { "BreakableCrate1_Character", "BreakableBarrel1_Character", "CastleAssetDemonStatue_Character", "DungeonSewerGrate1_Sprite", "DungeonSewerGrate2_Sprite", "", "CastleAssetWeb1_Sprite", "CastleAssetWeb2_Sprite", "DungeonChainRANDOM2_Character", "", "DungeonHangingCell1_Character", "DungeonHangingCell2_Character", "DungeonTorch1_Character", "DungeonTorch2_Character", "DungeonMaidenRANDOM3_Character", "DungeonPrison1_Sprite", "DungeonPrison2_Sprite", "DungeonPrison3_Sprite", "", "", "DungeonBucket1_Character", "DungeonBucket2_Character", "DungeonTable1_Character", "DungeonTable2_Character", "DungeonDoorOpen_Sprite",""};
        public static string[] TOWER_ASSETSWAP_LIST = new string[] { "BreakableCrate1_Character", "BreakableCrate2_Character", "CastleAssetAngelStatue_Character", "TowerHoleRANDOM9_Sprite", "TowerHoleRANDOM9_Sprite", "", "TowerLever1_Sprite", "TowerLever2_Sprite", "CastleAssetBackTorchUnlit_Character", "CastleAssetSideTorchUnlit_Character", "DungeonChain1_Character", "DungeonChain2_Character", "TowerTorch_Character", "TowerPedestal2_Character", "CastleAssetFireplaceNoFire_Character", "BrokenBookcase1_Sprite", "BrokenBookcase2_Sprite", "", "TowerBust1_Character", "TowerBust2_Character", "TowerChair1_Character", "TowerChair2_Character", "TowerTable1_Character", "TowerTable2_Character", "TowerDoorOpen_Sprite", "CastleAssetFrame_Sprite" };
        public static string[] GARDEN_ASSETSWAP_LIST = new string[] { "GardenUrn1_Character", "GardenUrn2_Character", "CherubStatue_Character", "GardenFloatingRockRANDOM5_Sprite", "GardenFloatingRockRANDOM5_Sprite", "GardenPillar_Character", "", "", "GardenFairy_Character", "", "GardenVine1_Character", "GardenVine2_Character", "GardenLampPost1_Character", "GardenLampPost2_Character", "GardenFountain_Character", "GardenBush1_Sprite", "GardenBush2_Sprite", "", "", "", "GardenMushroom1_Character", "GardenMushroom2_Character", "GardenTrunk1_Character", "GardenTrunk2_Character", "GardenDoorOpen_Sprite", "" };

        ///////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////
        //Percent chance the door direction will be open or closed when procedurally generating a room.
        public const int LEVEL_CASTLE_LEFTDOOR = 90;//70;
        public const int LEVEL_CASTLE_RIGHTDOOR = 90;//70;
        public const int LEVEL_CASTLE_TOPDOOR = 90;//70;
        public const int LEVEL_CASTLE_BOTTOMDOOR = 90;//70;

        public const int LEVEL_GARDEN_LEFTDOOR = 70;//80;
        public const int LEVEL_GARDEN_RIGHTDOOR = 100;//80;
        public const int LEVEL_GARDEN_TOPDOOR = 45;//85;//40;
        public const int LEVEL_GARDEN_BOTTOMDOOR = 45;//85;//40;

        public const int LEVEL_TOWER_LEFTDOOR = 45;//65;
        public const int LEVEL_TOWER_RIGHTDOOR = 45;//65;
        public const int LEVEL_TOWER_TOPDOOR = 100; //90;
        public const int LEVEL_TOWER_BOTTOMDOOR = 60;//40;

        public const int LEVEL_DUNGEON_LEFTDOOR = 55;//50;
        public const int LEVEL_DUNGEON_RIGHTDOOR = 55; //50;
        public const int LEVEL_DUNGEON_TOPDOOR = 45; //40;
        public const int LEVEL_DUNGEON_BOTTOMDOOR = 100;//75; //100;
        ////////////////////////////////////////////////////////////////

        //DEBUG EVS
        public static bool SHOW_ENEMY_RADII = false;
        public static bool ENABLE_DEBUG_INPUT = true;
        public static bool UNLOCK_ALL_ABILITIES = false;
        public static bool UNLOCK_ALL_DIARY_ENTRIES = false;

        public static GameTypes.LevelType TESTROOM_LEVELTYPE = GameTypes.LevelType.CASTLE;
        public static bool TESTROOM_REVERSE = false;
        public static bool RUN_TESTROOM = false;
        public static bool SHOW_DEBUG_TEXT = false;
        public static bool LOAD_TITLE_SCREEN = true;
        public static bool LOAD_SPLASH_SCREEN = false;
        public static bool SHOW_SAVELOAD_DEBUG_TEXT = false;
        public static bool DELETE_SAVEFILE = false;
        //public static bool CLOSE_TESTROOM_DOORS = true;
        public static bool CLOSE_TESTROOM_DOORS = false;
        public static bool RUN_TUTORIAL = false;
        public static bool RUN_DEMO_VERSION = false;
        public static bool DISABLE_SAVING = false;
        public static bool RUN_CRASH_LOGS = false;
        public static bool WEAKEN_BOSSES = false;
        public static bool ENABLE_OFFSCREEN_CONTROL = false;
        public static bool ENABLE_BACKUP_SAVING = true;
        public const string GAME_VERSION = "v1.4.1";
        public static bool ENABLE_BLITWORKS_SPLASH = false;
        public static bool CREATE_RETAIL_VERSION = true; // This EV overrides all the other Level EVs to create a retail build of the game.

        public static bool SHOW_FPS = false; // Setting this true also turns vsync off (so that you can get an FPS greater than 60).
        public static bool SAVE_FRAMES = false;
        public static int SAVEFILE_REVISION_NUMBER = 1;
    }
}
