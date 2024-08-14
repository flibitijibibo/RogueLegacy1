//#define OLD_CONSOLE_CREDITS
//#define SWITCH_CREDITS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using SpriteSystem;
using Microsoft.Xna.Framework.Graphics;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class ProceduralLevelScreen : Screen
    {
        private List<RoomObj> m_roomList; // Make sure to dispose this list.  Even though rooms are taken from LevelBuilder, clones are made so that the level builder collection is not modified.
        private PlayerObj m_player;

        protected int m_leftMostBorder = int.MaxValue;
        protected int m_rightMostBorder = -int.MaxValue;
        protected int m_topMostBorder = int.MaxValue;
        protected int m_bottomMostBorder = -int.MaxValue;

        protected int LeftDoorPercent = 80;
        protected int RightDoorPercent = 80;
        protected int TopDoorPercent = 80;
        protected int BottomDoorPercent = 80;

        protected TextManager m_textManager;
        protected PhysicsManager m_physicsManager;
        protected ProjectileManager m_projectileManager;
        protected ItemDropManager m_itemDropManager;

        protected RoomObj m_currentRoom;
        protected MapObj m_miniMapDisplay;
        private SpriteObj m_mapBG;

        private InputMap m_inputMap;
        private const byte INPUT_TOGGLEMAP = 0;
        private const byte INPUT_TOGGLEZOOM = 1;
        private const byte INPUT_LEFTCONTROL = 2;
        private const byte INPUT_LEFT = 3;
        private const byte INPUT_RIGHT = 4;
        private const byte INPUT_UP = 5;
        private const byte INPUT_DOWN = 6;
        private const byte INPUT_DISPLAYROOMINFO = 7;

        private List<Vector2> m_enemyStartPositions;
        private List<Vector2> m_tempEnemyStartPositions;

        private PlayerHUDObj m_playerHUD;

        private EnemyHUDObj m_enemyHUD;
        private EnemyObj m_lastEnemyHit;
        private float m_enemyHUDDuration = 2.0f;
        private float m_enemyHUDCounter = 0;

        /// Death Animation Variables ///
        public float BackBufferOpacity { get; set; }

        private List<EnemyObj> m_killedEnemyObjList;
        private int m_coinsCollected = 0;
        private int m_bagsCollected = 0;
        private int m_diamondsCollected = 0;
        private int m_blueprintsCollected = 0;
        private int m_bigDiamondsCollected = 0;

        private GameObj m_objKilledPlayer;
        private RenderTarget2D m_roomBWRenderTarget; // A special render target that is created so that rooms can draw their backgrounds.
                                                     // Created in level instead of having each room create their own, to save massive VRAM.

        // Effects for dynamic lighting ////
        SpriteObj m_dungeonLight;
        SpriteObj m_traitAura;

        ////////////////////////////////////

        ImpactEffectPool m_impactEffectPool;

        private TextObj m_roomTitle;
        private TextObj m_roomEnteringTitle;
        public bool CameraLockedToPlayer { get; set; }

        // Black borders for cinematic scenes.
        private SpriteObj m_blackBorder1;
        private SpriteObj m_blackBorder2;
        private int m_borderSize;

        private List<ChestObj> m_chestList;
        public bool LoadGameData = false;
        private ProjectileIconPool m_projectileIconPool;

        // Code needed for spells
        private float m_enemyPauseDuration = 0;
        private bool m_enemiesPaused = false;

        public float ShoutMagnitude { get; set; }

        // Code for objective plate.
        private ObjContainer m_objectivePlate;
        private TweenObject m_objectivePlateTween;

        public SkyObj m_sky;
        private SpriteObj m_whiteBG;

        public bool DisableRoomOnEnter { get; set; } // Sometimes I need to currentRoom.OnEnter().

        // Variables for the credits that appear in the beginning.
        private TextObj m_creditsText;
        private TextObj m_creditsTitleText;
        private string[] m_creditsTextList;
        private string[] m_creditsTextTitleList;
        private int m_creditsIndex = 0;
        private SpriteObj m_filmGrain;

        // Variables for the compass.
        private SpriteObj m_compassBG;
        private SpriteObj m_compass;
        private DoorObj m_compassDoor;
        private bool m_compassDisplayed = false;

        public bool DisableSongUpdating { get; set; }
        public bool DisableRoomTransitioning { get; set; }

        public TextObj DebugTextObj;

        public bool JukeboxEnabled { get; set; }

        private float m_elapsedScreenShake = 0;

        public ProceduralLevelScreen()
        {
            DisableRoomTransitioning = false;
            m_roomList = new List<RoomObj>();
            m_textManager = new TextManager(700); //200 TEDDY RAISING POOL TO 500
            m_projectileManager = new ProjectileManager(this, 700);
            m_enemyStartPositions = new List<Vector2>();
            m_tempEnemyStartPositions = new List<Vector2>();
            
            m_impactEffectPool = new ImpactEffectPool(2000);
            CameraLockedToPlayer = true;

            m_roomTitle = new TextObj();
            m_roomTitle.Font = Game.JunicodeLargeFont;
            //m_roomTitle.Align = Types.TextAlign.Centre;
            m_roomTitle.Align = Types.TextAlign.Right;
            m_roomTitle.Opacity = 0;
            m_roomTitle.FontSize = 40;
            m_roomTitle.Position = new Vector2(1320 - 50, 720 - 150);
            //m_roomTitle.Position = new Vector2(1320 / 2, 720 / 2 - 150);
            m_roomTitle.OutlineWidth = 2;
            //m_roomTitle.DropShadow = new Vector2(4, 4);

            m_roomEnteringTitle = m_roomTitle.Clone() as TextObj;
            m_roomEnteringTitle.Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_1", m_roomEnteringTitle); //"Now Entering"
            m_roomEnteringTitle.FontSize = 24;
            m_roomEnteringTitle.Y -= 50;

            m_inputMap = new InputMap(PlayerIndex.One, false);
            m_inputMap.AddInput(INPUT_TOGGLEMAP, Keys.Y);
            m_inputMap.AddInput(INPUT_TOGGLEZOOM, Keys.U);
            m_inputMap.AddInput(INPUT_LEFTCONTROL, Keys.LeftControl);
            m_inputMap.AddInput(INPUT_LEFT, Keys.Left);
            m_inputMap.AddInput(INPUT_RIGHT, Keys.Right);
            m_inputMap.AddInput(INPUT_UP, Keys.Up);
            m_inputMap.AddInput(INPUT_DOWN, Keys.Down);
            m_inputMap.AddInput(INPUT_DISPLAYROOMINFO, Keys.OemTilde);

            m_chestList = new List<ChestObj>();
            m_miniMapDisplay = new MapObj(true, this); // Must be called before CheckForRoomTransition() since rooms are added to the map during that call.

            m_killedEnemyObjList = new List<EnemyObj>();
        }

        public override void LoadContent()
        {
            DebugTextObj = new TextObj(Game.JunicodeFont);
            DebugTextObj.FontSize = 26;
            DebugTextObj.Align = Types.TextAlign.Centre;
            DebugTextObj.Text = "";
            DebugTextObj.ForceDraw = true;

            m_projectileIconPool = new ProjectileIconPool(200, m_projectileManager, ScreenManager as RCScreenManager);
            m_projectileIconPool.Initialize();

            m_textManager.Initialize();

            m_impactEffectPool.Initialize();

            m_physicsManager = (ScreenManager.Game as Game).PhysicsManager;
            m_physicsManager.SetGravity(0, -GlobalEV.GRAVITY);

            m_projectileManager.Initialize();
            m_physicsManager.Initialize(ScreenManager.Camera);

            m_itemDropManager = new ItemDropManager(600, m_physicsManager);
            m_itemDropManager.Initialize();

            m_playerHUD = new PlayerHUDObj();
            m_playerHUD.SetPosition(new Vector2(20, 40));

            m_enemyHUD = new EnemyHUDObj();
            m_enemyHUD.Position = new Vector2(GlobalEV.ScreenWidth / 2 - m_enemyHUD.Width / 2, 20);

            m_miniMapDisplay.SetPlayer(m_player);
            m_miniMapDisplay.InitializeAlphaMap(new Rectangle(1320 - 250, 50, 200, 100), Camera);

            InitializeAllRooms(true); // Required to initialize all the render targets for each room. Must be called before InitializeEnemies/Chests() so that the room's level is set.
            InitializeEnemies();
            InitializeChests(true);
            InitializeRenderTargets();

            m_mapBG = new SpriteObj("MinimapBG_Sprite");
            m_mapBG.Position = new Vector2(1320 - 250, 50);
            m_mapBG.ForceDraw = true;

            UpdateCamera();

            m_borderSize = 100;
            m_blackBorder1 = new SpriteObj("Blank_Sprite");
            m_blackBorder1.TextureColor = Color.Black;
            m_blackBorder1.Scale = new Vector2(1340f / m_blackBorder1.Width, m_borderSize/m_blackBorder1.Height);
            m_blackBorder2 = new SpriteObj("Blank_Sprite");
            m_blackBorder2.TextureColor = Color.Black;
            m_blackBorder2.Scale = new Vector2(1340f / m_blackBorder2.Width, m_borderSize / m_blackBorder2.Height);
            m_blackBorder1.ForceDraw = true;
            m_blackBorder2.ForceDraw = true;
            m_blackBorder1.Y = -m_borderSize;
            m_blackBorder2.Y = 720;

            m_dungeonLight = new SpriteObj("LightSource_Sprite");
            m_dungeonLight.ForceDraw = true;
            m_dungeonLight.Scale = new Vector2(12, 12);
            m_traitAura = new SpriteObj("LightSource_Sprite");
            m_traitAura.ForceDraw = true;

            // Objective plate
            m_objectivePlate = new ObjContainer("DialogBox_Character");
            m_objectivePlate.ForceDraw = true;
            TextObj objTitle = new TextObj(Game.JunicodeFont);
            objTitle.Position = new Vector2(-400, -60);
            objTitle.OverrideParentScale = true;
            objTitle.FontSize = 10;
            objTitle.Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_2", objTitle); //"Fairy Chest Objective:"
            objTitle.TextureColor = Color.Red;
            objTitle.OutlineWidth = 2;
            m_objectivePlate.AddChild(objTitle);

            TextObj objDescription = new TextObj(Game.JunicodeFont);
            objDescription.OverrideParentScale = true;
            objDescription.Position = new Vector2(objTitle.X, objTitle.Y + 40);
            objDescription.ForceDraw = true;
            objDescription.FontSize = 9;
            objDescription.Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_3", objDescription); //"Reach the chest in 15 seconds:"
            objDescription.WordWrap(250);
            objDescription.OutlineWidth = 2;
            m_objectivePlate.AddChild(objDescription);

            TextObj objProgress = new TextObj(Game.JunicodeFont);
            objProgress.OverrideParentScale = true;
            objProgress.Position = new Vector2(objDescription.X, objDescription.Y + 35);
            objProgress.ForceDraw = true;
            objProgress.FontSize = 9;
            objProgress.Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_4", objProgress); //"Time Remaining:"
            objProgress.WordWrap(250);
            objProgress.OutlineWidth = 2;
            m_objectivePlate.AddChild(objProgress);

            m_objectivePlate.Scale = new Vector2(250f / m_objectivePlate.GetChildAt(0).Width, 130f / m_objectivePlate.GetChildAt(0).Height);
            m_objectivePlate.Position = new Vector2(1170 + 300, 250);

            SpriteObj objectiveLine1 = new SpriteObj("Blank_Sprite");
            objectiveLine1.TextureColor = Color.Red;
            objectiveLine1.Position = new Vector2(objDescription.X, objDescription.Y + 20);
            objectiveLine1.ForceDraw = true;
            objectiveLine1.OverrideParentScale = true;
            objectiveLine1.ScaleY = 0.5f;
            m_objectivePlate.AddChild(objectiveLine1);

            SpriteObj objectiveLine2 = new SpriteObj("Blank_Sprite");
            objectiveLine2.TextureColor = Color.Red;
            objectiveLine2.Position = new Vector2(objDescription.X, objectiveLine1.Y + 35);
            objectiveLine2.ForceDraw = true;
            objectiveLine2.OverrideParentScale = true;
            objectiveLine2.ScaleY = 0.5f;
            m_objectivePlate.AddChild(objectiveLine2);
            base.LoadContent(); // Doesn't do anything.

            m_sky = new SkyObj(this);
            m_sky.LoadContent(Camera);

            m_whiteBG = new SpriteObj("Blank_Sprite");
            m_whiteBG.Opacity = 0;
            m_whiteBG.Scale = new Vector2(1320f / m_whiteBG.Width, 720f/m_whiteBG.Height);

            m_filmGrain = new SpriteObj("FilmGrain_Sprite");
            m_filmGrain.ForceDraw = true;
            m_filmGrain.Scale = new Vector2(2.015f, 2.05f);
            m_filmGrain.X -= 5;
            m_filmGrain.Y -= 5;
            m_filmGrain.PlayAnimation(true);
            m_filmGrain.AnimationDelay = 1 / 30f;

            m_compassBG = new SpriteObj("CompassBG_Sprite");
            m_compassBG.ForceDraw = true;
            m_compassBG.Position = new Vector2(1320 / 2f, 90);
            m_compassBG.Scale = Vector2.Zero;
            m_compass = new SpriteObj("Compass_Sprite");
            m_compass.Position = m_compassBG.Position;
            m_compass.ForceDraw = true;
            m_compass.Scale = Vector2.Zero;

            InitializeCreditsText();
        }

        private void InitializeCreditsText()
        {
            m_creditsTextTitleList = new string[]
                {
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_1",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_2",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_3",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_4",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_5",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_6",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_7",
#if OLD_CONSOLE_CREDITS || SWITCH_CREDITS
                    "LOC_ID_CREDITS_SCREEN_26",
                    //"Japanese Localization & Production By",  // This is not translated
#endif
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_8"
                };

            m_creditsTextList = new string[]
                {
                    "Cellar Door Games",
                    "Teddy Lee",
                    "Kenny Lee", 
#if SWITCH_CREDITS
                    "Ryan Lee",
#else
                    "Marie-Christine Bourdua",
#endif
                    "Glauber Kotaki",
                    "Gordon McGladdery",
                    "Judson Cowan",
#if OLD_CONSOLE_CREDITS
                    "Abstraction Games",
                    //"8-4, Ltd.", // The Japanese Localization text above needs to be translated before this can be uncommented out.
#endif
#if SWITCH_CREDITS
                    "BlitWorks SL",
#endif
                    "Rogue Legacy",
                };

            m_creditsText = new TextObj(Game.JunicodeFont);
            m_creditsText.FontSize = 20;
            m_creditsText.Text = "Cellar Door Games";
            m_creditsText.DropShadow = new Vector2(2, 2);
            m_creditsText.Opacity = 0;

            m_creditsTitleText = m_creditsText.Clone() as TextObj;
            m_creditsTitleText.FontSize = 14;
            m_creditsTitleText.Position = new Vector2(50, 580);

            m_creditsText.Position = m_creditsTitleText.Position;
            m_creditsText.Y += 35;
            m_creditsTitleText.X += 5;
        }

        public void DisplayCreditsText(bool resetIndex)
        {
            if (resetIndex == true)
                m_creditsIndex = 0;

            m_creditsTitleText.Opacity = 0;
            m_creditsText.Opacity = 0;

            if (m_creditsIndex < m_creditsTextList.Length)
            {
                m_creditsTitleText.Opacity = 0;
                m_creditsText.Opacity = 0;

                m_creditsTitleText.Text = LocaleBuilder.getString(m_creditsTextTitleList[m_creditsIndex], m_creditsTitleText);
                m_creditsText.Text = m_creditsTextList[m_creditsIndex];

                // Tween text in.
                Tween.To(m_creditsTitleText, 0.5f, Tween.EaseNone, "Opacity", "1");
                Tween.To(m_creditsText, 0.5f, Tween.EaseNone, "delay", "0.2", "Opacity", "1");
                m_creditsTitleText.Opacity = 1;
                m_creditsText.Opacity = 1;

                // Tween text out.
                Tween.To(m_creditsTitleText, 0.5f, Tween.EaseNone, "delay", "4", "Opacity", "0");
                Tween.To(m_creditsText, 0.5f, Tween.EaseNone, "delay", "4.2", "Opacity", "0");
                m_creditsTitleText.Opacity = 0;
                m_creditsText.Opacity = 0;

                m_creditsIndex++;
                Tween.RunFunction(8, this, "DisplayCreditsText", false);
            }
        }

        public void StopCreditsText()
        {
            m_creditsIndex = 0;
            Tween.StopAllContaining(m_creditsTitleText, false);
            Tween.StopAllContaining(m_creditsText, false);
            Tween.StopAllContaining(this, false);
            m_creditsTitleText.Opacity = 0;
        }

        public override void ReinitializeRTs()
        {
            m_sky.ReinitializeRT(Camera);
            m_miniMapDisplay.InitializeAlphaMap(new Rectangle(1320 - 250, 50, 200, 100), Camera);
            InitializeRenderTargets();
            InitializeAllRooms(false);

            if (CurrentRoom == null || CurrentRoom.Name != "Start")
            {
                if (CurrentRoom.Name == "ChallengeBoss")
                {
                    //m_foregroundSprite.TextureColor = Color.Black;
                    //m_backgroundSprite.TextureColor = Color.Black;
                    m_backgroundSprite.Scale = Vector2.One;
                    m_backgroundSprite.ChangeSprite("NeoBG_Sprite", ScreenManager.Camera);
                    m_backgroundSprite.Scale = new Vector2(2, 2);

                    m_foregroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.ChangeSprite("NeoFG_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                }
                else
                {
                    switch (CurrentRoom.LevelType)
                    {
                        case (GameTypes.LevelType.CASTLE):
                            m_backgroundSprite.Scale = Vector2.One;
                            m_foregroundSprite.Scale = Vector2.One;
                            m_backgroundSprite.ChangeSprite("CastleBG1_Sprite", ScreenManager.Camera);
                            m_foregroundSprite.ChangeSprite("CastleFG1_Sprite", ScreenManager.Camera);
                            m_backgroundSprite.Scale = new Vector2(2, 2);
                            m_foregroundSprite.Scale = new Vector2(2, 2);
                            break;
                        case (GameTypes.LevelType.TOWER):
                            m_backgroundSprite.Scale = Vector2.One;
                            m_foregroundSprite.Scale = Vector2.One;
                            m_backgroundSprite.ChangeSprite("TowerBG2_Sprite", ScreenManager.Camera);
                            m_foregroundSprite.ChangeSprite("TowerFG2_Sprite", ScreenManager.Camera);
                            m_backgroundSprite.Scale = new Vector2(2, 2);
                            m_foregroundSprite.Scale = new Vector2(2, 2);
                            break;
                        case (GameTypes.LevelType.DUNGEON):
                            m_backgroundSprite.Scale = Vector2.One;
                            m_foregroundSprite.Scale = Vector2.One;
                            m_backgroundSprite.ChangeSprite("DungeonBG1_Sprite", ScreenManager.Camera);
                            m_foregroundSprite.ChangeSprite("DungeonFG1_Sprite", ScreenManager.Camera);
                            m_backgroundSprite.Scale = new Vector2(2, 2);
                            m_foregroundSprite.Scale = new Vector2(2, 2);
                            break;
                        case (GameTypes.LevelType.GARDEN):
                            m_backgroundSprite.Scale = Vector2.One;
                            m_foregroundSprite.Scale = Vector2.One;
                            m_backgroundSprite.ChangeSprite("GardenBG_Sprite", ScreenManager.Camera);
                            m_foregroundSprite.ChangeSprite("GardenFG_Sprite", ScreenManager.Camera);
                            m_backgroundSprite.Scale = new Vector2(2, 2);
                            m_foregroundSprite.Scale = new Vector2(2, 2);
                            break;
                    }
                }

                if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
                {
                    m_foregroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.ChangeSprite("NeoFG_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                }
            }

            m_backgroundSprite.Position = CurrentRoom.Position;
            m_foregroundSprite.Position = CurrentRoom.Position;

            base.ReinitializeRTs();
        }

        // Removes the previous room's objects from the physics manager and adds the new room's objects.
        // Not performance heavy because removing and adding objects to physics manager is O(1), with the exception of Clear(), which is O(n).
        private void LoadPhysicsObjects(RoomObj room)
        {
            Rectangle expandedRoomRect = new Rectangle((int)room.X - 100, (int)room.Y - 100, room.Width + 200, room.Height + 200); // An expanded bounds of the room.
            //m_physicsManager.ObjectList.Clear();
            m_physicsManager.RemoveAllObjects();

            foreach (TerrainObj obj in CurrentRoom.TerrainObjList)
                m_physicsManager.AddObject(obj);

            foreach (ProjectileObj obj in m_projectileManager.ActiveProjectileList)
                m_physicsManager.AddObject(obj);

            foreach (GameObj obj in CurrentRoom.GameObjList)
            {
                IPhysicsObj physicsObj = obj as IPhysicsObj;
                if (physicsObj != null && obj.Bounds.Intersects(expandedRoomRect)) // Not sure why we're doing a bounds intersect check.
                {
                    BreakableObj breakable = obj as BreakableObj;
                    if (breakable != null && breakable.Broken == true) // Don't add broken breakables to the list.
                        continue;
                    m_physicsManager.AddObject(physicsObj);
                }
            }

            // This is needed for entering boss doors.
            foreach (DoorObj door in CurrentRoom.DoorList)
                m_physicsManager.AddObject(door);

            foreach (EnemyObj enemy in CurrentRoom.EnemyList)
            {
                m_physicsManager.AddObject(enemy);

                if (enemy is EnemyObj_BallAndChain) // Special handling to add the separate entity ball for the ball and chain dude.
                {
                    if (enemy.IsKilled == false)
                    {
                        m_physicsManager.AddObject((enemy as EnemyObj_BallAndChain).BallAndChain);
                        if (enemy.Difficulty > GameTypes.EnemyDifficulty.BASIC)
                            m_physicsManager.AddObject((enemy as EnemyObj_BallAndChain).BallAndChain2);
                    }
                }
            }

            foreach (EnemyObj enemy in CurrentRoom.TempEnemyList)
                m_physicsManager.AddObject(enemy);

            m_physicsManager.AddObject(m_player);
        }

        public void InitializeEnemies()
        {
            //int enemyLevel = 1;
            //int enemyDifficulty = (int)GameTypes.EnemyDifficulty.BASIC;
            //int levelCounter = 0;

            List<TerrainObj> terrainCollList = new List<TerrainObj>();

            foreach (RoomObj room in m_roomList)
            {
                foreach (EnemyObj enemy in room.EnemyList)
                {
                    enemy.SetPlayerTarget(m_player);
                    enemy.SetLevelScreen(this); // Must be called before enemy.Initialize().

                    int roomLevel = room.Level;
                    // Special handling for boss rooms.
                    if (room.Name == "Boss" && room.LinkedRoom != null)
                    {
                        //int roomLevel = room.LinkedRoom.RoomNumber;
                        roomLevel = room.LinkedRoom.Level;
                        int bossEnemyLevel = (int)(roomLevel / (LevelEV.ROOM_LEVEL_MOD + Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelDown) * GameEV.RUNE_GRACE_ROOM_LEVEL_LOSS));
                        enemy.Level = bossEnemyLevel;
                    }
                    else
                    {
                        int enemyLevel = (int)(roomLevel / (LevelEV.ROOM_LEVEL_MOD + Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelDown) * GameEV.RUNE_GRACE_ROOM_LEVEL_LOSS));
                        if (enemyLevel < 1) enemyLevel = 1;
                        enemy.Level = enemyLevel; // Call this before Initialize(), since Initialie sets their starting health and so on.
                    }

                    int enemyDifficulty = (int)(enemy.Level / LevelEV.ENEMY_LEVEL_DIFFICULTY_MOD);
                    if (enemyDifficulty > (int)GameTypes.EnemyDifficulty.EXPERT)
                        enemyDifficulty = (int)GameTypes.EnemyDifficulty.EXPERT;

                    if (enemy.IsProcedural == true)
                    {
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                            enemy.Level += LevelEV.ENEMY_EXPERT_LEVEL_MOD;
                        if ((int)enemy.Difficulty < enemyDifficulty)
                            enemy.SetDifficulty((GameTypes.EnemyDifficulty)enemyDifficulty, false);
                    }
                    else
                    {
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                        {
                            if (room is ArenaBonusRoom) // Level up arena room enemies by expert level instead of miniboss.
                                enemy.Level += LevelEV.ENEMY_EXPERT_LEVEL_MOD;
                            else
                                enemy.Level += LevelEV.ENEMY_MINIBOSS_LEVEL_MOD;
                        }
                    }

                    //if (enemy.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                    //    enemy.Level += LevelEV.ENEMY_EXPERT_LEVEL_MOD; // If an enemy is already expert, then he is a yellow orb, and should gain these extra level.
                    //else if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                    //    enemy.Level += LevelEV.ENEMY_MINIBOSS_LEVEL_MOD; // Minibosses gain these extra levels.
                    //else if (enemy.IsProcedural == true && (int)enemy.Difficulty < enemyDifficulty) // Only change the difficulty of procedural enemies and if they're not yellows and they're lower difficulty.
                    //    enemy.SetDifficulty((GameTypes.EnemyDifficulty)enemyDifficulty, false);

                    enemy.Initialize();

                    // Positioning each enemy to the ground closest below them. But don't do it if they fly.
                    if (enemy.IsWeighted == true)
                    {
                        float closestGround = float.MaxValue;
                        TerrainObj closestTerrain = null;
                        terrainCollList.Clear();
                        Rectangle enemyBoundsRect = new Rectangle((int)enemy.X, (int)enemy.TerrainBounds.Bottom, 1, 5000);
                        foreach (TerrainObj terrainObj in room.TerrainObjList)
                        {
                            if (terrainObj.Rotation == 0)
                            {
                                if (terrainObj.Bounds.Top >= enemy.TerrainBounds.Bottom && CollisionMath.Intersects(terrainObj.Bounds, enemyBoundsRect))
                                    terrainCollList.Add(terrainObj);
                            }
                            else
                            {
                                if (CollisionMath.RotatedRectIntersects(enemyBoundsRect, 0, Vector2.Zero, terrainObj.TerrainBounds, terrainObj.Rotation, Vector2.Zero))
                                    terrainCollList.Add(terrainObj);
                            }
                        }

                        foreach (TerrainObj terrain in terrainCollList)
                        {
                            bool collides = false;
                            int groundDist = 0;
                            if (terrain.Rotation == 0)
                            {
                                collides = true;
                                groundDist = terrain.TerrainBounds.Top - enemy.TerrainBounds.Bottom;
                            }
                            else
                            {
                                Vector2 pt1, pt2;
                                if (terrain.Width > terrain.Height) // If rotated objects are done correctly.
                                {
                                    pt1 = CollisionMath.UpperLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                    pt2 = CollisionMath.UpperRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                }
                                else // If rotated objects are done Teddy's incorrect way.
                                {
                                    if (terrain.Rotation > 0) // ROTCHECK
                                    {
                                        pt1 = CollisionMath.LowerLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                        pt2 = CollisionMath.UpperLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                    }
                                    else
                                    {
                                        pt1 = CollisionMath.UpperRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                        pt2 = CollisionMath.LowerRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                                    }
                                }

                                // A check to make sure the enemy collides with the correct slope.
                                if (enemy.X > pt1.X && enemy.X < pt2.X)
                                    collides = true;

                                float u = pt2.X - pt1.X;
                                float v = pt2.Y - pt1.Y;
                                float x = pt1.X;
                                float y = pt1.Y;
                                float x1 = enemy.X;

                                groundDist = (int)(y + (x1 - x) * (v / u)) - enemy.TerrainBounds.Bottom;
                            }

                            if (collides == true && groundDist < closestGround && groundDist > 0)
                            {
                                closestGround = groundDist;
                                closestTerrain = terrain;
                            }
                        }

                        //foreach (TerrainObj terrainObj in room.TerrainObjList)
                        //{
                        //    if (terrainObj.Y >= enemy.Y)
                        //    {
                        //        if (terrainObj.Y - enemy.Y < closestGround && CollisionMath.Intersects(terrainObj.Bounds, new Rectangle((int)enemy.X, (int)(enemy.Y + (terrainObj.Y - enemy.Y) + 5), enemy.Width, (int)(enemy.Height / 2))))
                        //        {
                        //            closestGround = terrainObj.Y - enemy.Y;
                        //            closestTerrain = terrainObj;
                        //        }
                        //    }
                        //}

                        if (closestTerrain != null)
                        {
                            enemy.UpdateCollisionBoxes();
                            if (closestTerrain.Rotation == 0)
                                enemy.Y = closestTerrain.Y - (enemy.TerrainBounds.Bottom - enemy.Y);
                            else
                                HookEnemyToSlope(enemy, closestTerrain);
                        }
                    }
                }
            }
        }

        private void HookEnemyToSlope(IPhysicsObj enemy, TerrainObj terrain)
        {
            float y1 = float.MaxValue;
            Vector2 pt1, pt2;
            if (terrain.Width > terrain.Height) // If rotated objects are done correctly.
            {
                pt1 = CollisionMath.UpperLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                pt2 = CollisionMath.UpperRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
            }
            else // If rotated objects are done Teddy's incorrect way.
            {
                if (terrain.Rotation > 0) // ROTCHECK
                {
                    pt1 = CollisionMath.LowerLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                    pt2 = CollisionMath.UpperLeftCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                }
                else
                {
                    pt1 = CollisionMath.UpperRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                    pt2 = CollisionMath.LowerRightCorner(terrain.TerrainBounds, terrain.Rotation, Vector2.Zero);
                }
            }

            //if (enemy.X > pt1.X + 10 && enemy.X < pt2.X - 10)
            {
                float u = pt2.X - pt1.X;
                float v = pt2.Y - pt1.Y;
                float x = pt1.X;
                float y = pt1.Y;
                float x1 = enemy.X;

                y1 = y + (x1 - x) * (v / u);

                enemy.UpdateCollisionBoxes();
                y1 -= (enemy.Bounds.Bottom - enemy.Y) + (5 * (enemy as GameObj).ScaleX);
                enemy.Y = (float)Math.Round(y1, MidpointRounding.ToEven);
            }
        }

        public void InitializeChests(bool resetChests)
        {
            m_chestList.Clear();

            //int chestLevel = 1;
            //int levelCounter = 0; // Every 5 times a room is iterated, the chest's level goes up.

            foreach (RoomObj room in RoomList)
            {
                //if (room.Name != "Secret") // Do not modify chests for secret rooms yet.
                {
                    foreach (GameObj obj in room.GameObjList)
                    {
                        ChestObj chest = obj as ChestObj;
                        if (chest != null && chest.ChestType != ChestType.Fairy)// && room.Name != "Bonus") // Do not modify chests for bonus rooms or fairy chests.
                        {
                            //chest.Level = chestLevel;  // Setting the chest level.
                            chest.Level = (int)(room.Level / (LevelEV.ROOM_LEVEL_MOD + Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelDown) * GameEV.RUNE_GRACE_ROOM_LEVEL_LOSS));

                            if (chest.IsProcedural == true) // Ensures chests loaded from a save file are not overwritten.
                            {
                                // Closes the chests.
                                if (resetChests == true)
                                    chest.ResetChest();


                                // Turning the chest into a brown, silver, or gold chest.

                                int chestRoll = CDGMath.RandomInt(1, 100);
                                int chestType = 0;
                                for (int i = 0; i < GameEV.CHEST_TYPE_CHANCE.Length; i++)
                                {
                                    chestType += GameEV.CHEST_TYPE_CHANCE[i];
                                    if (chestRoll <= chestType)
                                    {
                                        if (i == 0)
                                            chest.ChestType = ChestType.Brown;
                                        else if (i == 1)
                                            chest.ChestType = ChestType.Silver;
                                        else
                                            chest.ChestType = ChestType.Gold;
                                        break;
                                    }
                                }
                                //////////////////////////////////////////////////////////
                            }
                            m_chestList.Add(chest);
                        }
                        else if (chest != null && chest.ChestType == ChestType.Fairy)
                        {
                            FairyChestObj fairyChest = chest as FairyChestObj;
                            if (fairyChest != null)
                            {
                                if (chest.IsProcedural == true)
                                {
                                    if (resetChests == true)
                                        fairyChest.ResetChest();
                                }
                                //fairyChest.SetPlayer(m_player);
                                fairyChest.SetConditionType();
                            }
                        }
                        m_chestList.Add(chest);

                        // Code to properly recentre chests (since their anchor points were recently modified in the spritesheet.
                        if (chest != null)
                        {
                            chest.X += chest.Width / 2;
                            chest.Y += 60; // The height of a tile.
                        }
                    }
                }

                //if (room.Level % LevelEV.ROOM_LEVEL_MOD == 0)
                //    chestLevel++;

                //levelCounter++;
                //if (levelCounter >= LevelEV.ROOM_LEVEL_MOD)
                //{
                //    levelCounter = 0;
                //    chestLevel++;
                //}
            }
        }

        private Texture2D m_castleBorderTexture, m_towerBorderTexture, m_dungeonBorderTexture, m_gardenBorderTexture;
        private Texture2D m_neoBorderTexture;

        public void InitializeAllRooms(bool loadContent)
        {
            m_castleBorderTexture = (new SpriteObj("CastleBorder_Sprite") { Scale = new Vector2(2, 2) }).ConvertToTexture(Camera, true, SamplerState.PointWrap);
            string castleCornerTextureString = "CastleCorner_Sprite";
            string castleCornerLTextureString = "CastleCornerL_Sprite";

            m_towerBorderTexture = (new SpriteObj("TowerBorder2_Sprite") { Scale = new Vector2(2, 2) }).ConvertToTexture(Camera, true, SamplerState.PointWrap);
            string towerCornerTextureString = "TowerCorner_Sprite";
            string towerCornerLTextureString = "TowerCornerL_Sprite";

            m_dungeonBorderTexture = (new SpriteObj("DungeonBorder_Sprite") { Scale = new Vector2(2, 2) }).ConvertToTexture(Camera, true, SamplerState.PointWrap);
            string dungeonCornerTextureString = "DungeonCorner_Sprite";
            string dungeonCornerLTextureString = "DungeonCornerL_Sprite";

            m_gardenBorderTexture = (new SpriteObj("GardenBorder_Sprite") { Scale = new Vector2(2, 2) }).ConvertToTexture(Camera, true, SamplerState.PointWrap);
            string gardenCornerTextureString = "GardenCorner_Sprite";
            string gardenCornerLTextureString = "GardenCornerL_Sprite";

            m_neoBorderTexture = (new SpriteObj("NeoBorder_Sprite") { Scale = new Vector2(2, 2) }).ConvertToTexture(Camera, true, SamplerState.PointWrap);
            string futureCornerTextureString = "NeoCorner_Sprite";
            string futureCornerLTextureString = "NeoCornerL_Sprite";

            if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
            {
                castleCornerLTextureString = dungeonCornerLTextureString = towerCornerLTextureString = gardenCornerLTextureString = futureCornerLTextureString;
                castleCornerTextureString = dungeonCornerTextureString = towerCornerTextureString = gardenCornerTextureString = futureCornerTextureString;
            }
            // These textures need to be stored and released during dispose().

            int roomLevel = 0;
            roomLevel = Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelUp) * GameEV.RUNE_CURSE_ROOM_LEVEL_GAIN;

            if (m_roomBWRenderTarget != null)
                m_roomBWRenderTarget.Dispose();
            m_roomBWRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            foreach (RoomObj room in RoomList)
            {
                int roomLevelMod = 0;
                switch (room.LevelType)
                {
                    case(GameTypes.LevelType.CASTLE):
                        roomLevelMod = LevelEV.CASTLE_ROOM_LEVEL_BOOST;
                        break;
                    case (GameTypes.LevelType.GARDEN):
                        roomLevelMod = LevelEV.GARDEN_ROOM_LEVEL_BOOST - 2; // Subtracting 2 for each subsequent number of Linker rooms there are on the map.
                        break;
                    case (GameTypes.LevelType.TOWER):
                        roomLevelMod = LevelEV.TOWER_ROOM_LEVEL_BOOST - 4;
                        break;
                    case (GameTypes.LevelType.DUNGEON):
                        roomLevelMod = LevelEV.DUNGEON_ROOM_LEVEL_BOOST - 6;
                        break;
                }

                if (Game.PlayerStats.TimesCastleBeaten == 0)
                    room.Level = roomLevel + roomLevelMod;
                else
                    room.Level = roomLevel + roomLevelMod + (LevelEV.NEWGAMEPLUS_LEVEL_BASE + ((Game.PlayerStats.TimesCastleBeaten - 1) * LevelEV.NEWGAMEPLUS_LEVEL_APPRECIATION)); //TEDDY DELETING 1 from TimesCastleBeaten CAUSE APPRECIATION SHOULDNT KICK IN.

                roomLevel++;

                if (loadContent == true)
                    room.LoadContent(Camera.GraphicsDevice);

                room.InitializeRenderTarget(m_roomBWRenderTarget);

                if (room.Name == "ChallengeBoss")
                {
                    foreach (BorderObj border in room.BorderList)
                    {
                        border.SetBorderTextures(m_neoBorderTexture, futureCornerTextureString, futureCornerLTextureString);
                        border.NeoTexture = m_neoBorderTexture;
                    }
                }
                else
                {
                    foreach (BorderObj border in room.BorderList)
                    {
                        switch (room.LevelType)
                        {
                            case (GameTypes.LevelType.TOWER):
                                border.SetBorderTextures(m_towerBorderTexture, towerCornerTextureString, towerCornerLTextureString);
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                border.SetBorderTextures(m_dungeonBorderTexture, dungeonCornerTextureString, dungeonCornerLTextureString);
                                break;
                            case (GameTypes.LevelType.GARDEN):
                                border.SetBorderTextures(m_gardenBorderTexture, gardenCornerTextureString, gardenCornerLTextureString);
                                border.TextureOffset = new Vector2(0, -18);
                                break;
                            case (GameTypes.LevelType.CASTLE):
                            default:
                                border.SetBorderTextures(m_castleBorderTexture, castleCornerTextureString, castleCornerLTextureString);
                                break;
                        }
                        border.NeoTexture = m_neoBorderTexture;
                    }
                }

                bool addTerrainBoxToBreakables = false;
                if (Game.PlayerStats.Traits.X == TraitType.NoFurniture || Game.PlayerStats.Traits.Y == TraitType.NoFurniture)
                    addTerrainBoxToBreakables = true;

                foreach (GameObj obj in room.GameObjList)
                {
                    HazardObj hazard = obj as HazardObj;
                    if (hazard != null)
                        hazard.InitializeTextures(Camera);

                    HoverObj hoverObj = obj as HoverObj;
                    if (hoverObj != null)
                        hoverObj.SetStartingPos(hoverObj.Position);

                    if (addTerrainBoxToBreakables == true)
                    {
                        BreakableObj breakableObj = obj as BreakableObj;

                        if (breakableObj != null && breakableObj.HitBySpellsOnly == false && breakableObj.HasTerrainHitBox == false)
                        {
                            breakableObj.CollisionBoxes.Add(new CollisionBox(breakableObj.RelativeBounds.X, breakableObj.RelativeBounds.Y, breakableObj.Width, breakableObj.Height, Consts.TERRAIN_HITBOX, breakableObj));
                            breakableObj.DisableHitboxUpdating = true;
                            breakableObj.UpdateTerrainBox();
                        }
                    }
                }

                if (LevelEV.RUN_TESTROOM == true && loadContent == true)
                {
                    foreach (GameObj obj in room.GameObjList)
                    {
                        if (obj is PlayerStartObj)
                            m_player.Position = obj.Position;
                    }
                }

                if ((room.Name == "Boss" || room.Name == "ChallengeBoss") && room.LinkedRoom != null)
                {
                    CloseBossDoor(room.LinkedRoom, room.LevelType);
                    //OpenChallengeBossDoor(room.LinkedRoom, room.LevelType); // Extra content added to link challenge boss rooms.
                    //if (Game.PlayerStats.ChallengeLastBossBeaten == false && Game.PlayerStats.ChallengeLastBossUnlocked == true)
                    //    OpenLastBossChallengeDoors();
                }
            }
        }

        public void CloseBossDoor(RoomObj linkedRoom, GameTypes.LevelType levelType)
        {
            bool closeDoor = false;

            switch (levelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    if (Game.PlayerStats.EyeballBossBeaten == true)
                        closeDoor = true;
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    if (Game.PlayerStats.BlobBossBeaten == true)
                        closeDoor = true;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    if (Game.PlayerStats.FairyBossBeaten == true)
                        closeDoor = true;
                    break;
                case (GameTypes.LevelType.TOWER):
                    if (Game.PlayerStats.FireballBossBeaten == true)
                        closeDoor = true;
                    break;
            }

            if (closeDoor == true)
            {
                foreach (DoorObj door in linkedRoom.DoorList)
                {
                    if (door.IsBossDoor == true)
                    {
                        // Change the door graphic to closed.
                        foreach (GameObj obj in linkedRoom.GameObjList)
                        {
                            if (obj.Name == "BossDoor")
                            {
                                obj.ChangeSprite((obj as SpriteObj).SpriteName.Replace("Open", ""));
                                obj.TextureColor = Color.White;
                                obj.Opacity = 1;
                                linkedRoom.LinkedRoom = null;
                                break;
                            }
                        }

                        // Lock the door.
                        door.Locked = true;
                        break;
                    }
                }
            }

            OpenChallengeBossDoor(linkedRoom, levelType); // Extra content added to link challenge boss rooms.
            //if (Game.PlayerStats.ChallengeLastBossBeaten == false && Game.PlayerStats.ChallengeLastBossUnlocked == true)
            if (Game.PlayerStats.ChallengeLastBossUnlocked == true)
                OpenLastBossChallengeDoors();
        }

        public void OpenLastBossChallengeDoors()
        {
            LastBossChallengeRoom lastBossChallengeRoom = null;
            foreach (RoomObj room in RoomList)
            {
                if (room.Name == "ChallengeBoss")
                {
                    if (room is LastBossChallengeRoom)
                    {
                        lastBossChallengeRoom = room as LastBossChallengeRoom;
                        break;
                    }
                }
            }

            foreach (RoomObj room in RoomList)
            {
                if (room.Name == "EntranceBoss")
                {
                    bool linkChallengeBossRoom = false;

                    // Make sure to only link rooms with bosses that are beaten.
                    if (room.LevelType == GameTypes.LevelType.CASTLE && Game.PlayerStats.EyeballBossBeaten == true)
                        linkChallengeBossRoom = true;
                    else if (room.LevelType == GameTypes.LevelType.DUNGEON && Game.PlayerStats.BlobBossBeaten == true)
                        linkChallengeBossRoom = true;
                    else if (room.LevelType == GameTypes.LevelType.GARDEN && Game.PlayerStats.FairyBossBeaten == true)
                        linkChallengeBossRoom = true;
                    else if (room.LevelType == GameTypes.LevelType.TOWER && Game.PlayerStats.FireballBossBeaten == true)
                        linkChallengeBossRoom = true;
                    
                    if (linkChallengeBossRoom == true)
                    {
                        foreach (DoorObj door in room.DoorList)
                        {
                            if (door.IsBossDoor == true)
                            {
                                room.LinkedRoom = lastBossChallengeRoom;

                                foreach (GameObj obj in room.GameObjList)
                                {
                                    if (obj.Name == "BossDoor")
                                    {
                                        // Change the door graphic to close
                                        if (Game.PlayerStats.ChallengeLastBossBeaten == true)
                                        {
                                            if ((obj as SpriteObj).SpriteName.Contains("Open") == true)
                                                obj.ChangeSprite((obj as SpriteObj).SpriteName.Replace("Open", ""));
                                            //obj.TextureColor = new Color(0, 255, 255);
                                            //obj.Opacity = 0.6f;
                                            obj.TextureColor = Color.White;
                                            obj.Opacity = 1;
                                            room.LinkedRoom = null;
                                            door.Locked = true;
                                        }
                                        else
                                        {
                                            // Change the door graphic to open
                                            if ((obj as SpriteObj).SpriteName.Contains("Open") == false)
                                                obj.ChangeSprite((obj as SpriteObj).SpriteName.Replace("_Sprite", "Open_Sprite"));
                                            obj.TextureColor = new Color(0, 255, 255);
                                            obj.Opacity = 0.6f;

                                            // Unlock the door. It now leads to the challenge room.
                                            door.Locked = false;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OpenChallengeBossDoor(RoomObj linkerRoom, GameTypes.LevelType levelType)
        {
            bool openSpecialBossDoor = false;

            switch (levelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    if (Game.PlayerStats.EyeballBossBeaten == true && Game.PlayerStats.ChallengeEyeballBeaten == false && Game.PlayerStats.ChallengeEyeballUnlocked == true)
                        openSpecialBossDoor = true;
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    if (Game.PlayerStats.BlobBossBeaten == true && Game.PlayerStats.ChallengeBlobBeaten == false && Game.PlayerStats.ChallengeBlobUnlocked == true)
                        openSpecialBossDoor = true;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    if (Game.PlayerStats.FairyBossBeaten == true && Game.PlayerStats.ChallengeSkullBeaten == false && Game.PlayerStats.ChallengeSkullUnlocked == true)
                        openSpecialBossDoor = true;
                    break;
                case (GameTypes.LevelType.TOWER):
                    if (Game.PlayerStats.FireballBossBeaten == true && Game.PlayerStats.ChallengeFireballBeaten == false && Game.PlayerStats.ChallengeFireballUnlocked == true)
                        openSpecialBossDoor = true;
                    break;
            }

            if (openSpecialBossDoor == true)
            {
                RoomObj linkedRoom = LevelBuilder2.GetChallengeBossRoomFromRoomList(levelType, m_roomList);
                linkerRoom.LinkedRoom = linkedRoom;

                foreach (DoorObj door in linkerRoom.DoorList)
                {
                    if (door.IsBossDoor == true)
                    {
                        // Change the door graphic to open
                        foreach (GameObj obj in linkerRoom.GameObjList)
                        {
                            if (obj.Name == "BossDoor")
                            {
                                obj.ChangeSprite((obj as SpriteObj).SpriteName.Replace("_Sprite", "Open_Sprite"));
                                obj.TextureColor = new Color(0,255,255);
                                obj.Opacity = 0.6f;
                                break;
                            }
                        }

                        // Unlock the door. It now leads to the challenge room.
                        door.Locked = false;
                        break;
                    }
                }
            }
        }

        public void AddRooms(List<RoomObj> roomsToAdd)
        {
            foreach (RoomObj room in roomsToAdd)
            {
                m_roomList.Add(room);
                if (room.X < m_leftMostBorder)
                    m_leftMostBorder = (int)room.X;
                if (room.X + room.Width > m_rightMostBorder)
                    m_rightMostBorder = (int)room.X + room.Width;
                if (room.Y < m_topMostBorder)
                    m_topMostBorder = (int)room.Y;
                if (room.Y + room.Height > m_bottomMostBorder)
                    m_bottomMostBorder = (int)room.Y + room.Height;
            }
        }

        public void AddRoom(RoomObj room)
        {
            m_roomList.Add(room);
            if (room.X < m_leftMostBorder)
                m_leftMostBorder = (int)room.X;
            if (room.X + room.Width > m_rightMostBorder)
                m_rightMostBorder = (int)room.X + room.Width;
            if (room.Y < m_topMostBorder)
                m_topMostBorder = (int)room.Y;
            if (room.Y + room.Height > m_bottomMostBorder)
                m_bottomMostBorder = (int)room.Y + room.Height;
        }

        private void CheckForRoomTransition()
        {
            if (m_player != null)
            {
                foreach (RoomObj roomObj in m_roomList)
                {
                    if (roomObj != CurrentRoom)
                    {
                        if (roomObj.Bounds.Contains((int)m_player.X, (int)m_player.Y))
                        {
                            // This was moved here. If causing problems, remove this one and uncomment the one lower in this function.
                            ResetEnemyPositions(); // Must be called before the current room is set. Resets the positions of all enemies in the previous room.

                            // Before changing rooms, reset enemy logic.
                            if (CurrentRoom != null)
                            {
                                foreach (EnemyObj enemy in EnemyList)
                                    enemy.ResetState();
                            }

                            if (m_enemiesPaused == true)
                                UnpauseAllEnemies();

                            m_player.RoomTransitionReset();

                            m_miniMapDisplay.AddRoom(roomObj); // Add the room to the map display the moment you enter it.
                            // Save the player data and map data upon transition to new room.
                            if (roomObj.Name != "Start")
                                (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.MapData);

                            // Override texture colour if challenge room
                            if (roomObj.Name == "ChallengeBoss")
                            {
                                //m_foregroundSprite.TextureColor = Color.Black;
                                //m_backgroundSprite.TextureColor = Color.Black;
                                m_backgroundSprite.Scale = Vector2.One;
                                m_backgroundSprite.ChangeSprite("NeoBG_Sprite", ScreenManager.Camera);
                                m_backgroundSprite.Scale = new Vector2(2, 2);

                                m_foregroundSprite.Scale = Vector2.One;
                                m_foregroundSprite.ChangeSprite("NeoFG_Sprite", ScreenManager.Camera);
                                m_foregroundSprite.Scale = new Vector2(2, 2);
                            }
                            //else
                            //{
                            //    m_foregroundSprite.TextureColor = Color.White;
                            //    m_backgroundSprite.TextureColor = Color.White;
                            //}

                            // This code only happens if the level type you are entering is different from the previous one you were in.
                            if ((CurrentRoom == null || CurrentLevelType != roomObj.LevelType || (CurrentRoom != null && CurrentRoom.Name == "ChallengeBoss")) && roomObj.Name != "Start")
                            {
                                if (roomObj.Name != "ChallengeBoss")
                                {
                                    switch (roomObj.LevelType)
                                    {
                                        case (GameTypes.LevelType.CASTLE):
                                            m_backgroundSprite.Scale = Vector2.One;
                                            m_foregroundSprite.Scale = Vector2.One;
                                            m_backgroundSprite.ChangeSprite("CastleBG1_Sprite", ScreenManager.Camera);
                                            m_foregroundSprite.ChangeSprite("CastleFG1_Sprite", ScreenManager.Camera);
                                            m_backgroundSprite.Scale = new Vector2(2, 2);
                                            m_foregroundSprite.Scale = new Vector2(2, 2);
                                            break;
                                        case (GameTypes.LevelType.TOWER):
                                            m_backgroundSprite.Scale = Vector2.One;
                                            m_foregroundSprite.Scale = Vector2.One;
                                            m_backgroundSprite.ChangeSprite("TowerBG2_Sprite", ScreenManager.Camera);
                                            m_foregroundSprite.ChangeSprite("TowerFG2_Sprite", ScreenManager.Camera);
                                            m_backgroundSprite.Scale = new Vector2(2, 2);
                                            m_foregroundSprite.Scale = new Vector2(2, 2);
                                            break;
                                        case (GameTypes.LevelType.DUNGEON):
                                            m_backgroundSprite.Scale = Vector2.One;
                                            m_foregroundSprite.Scale = Vector2.One;
                                            m_backgroundSprite.ChangeSprite("DungeonBG1_Sprite", ScreenManager.Camera);
                                            m_foregroundSprite.ChangeSprite("DungeonFG1_Sprite", ScreenManager.Camera);
                                            m_backgroundSprite.Scale = new Vector2(2, 2);
                                            m_foregroundSprite.Scale = new Vector2(2, 2);
                                            break;
                                        case (GameTypes.LevelType.GARDEN):
                                            m_backgroundSprite.Scale = Vector2.One;
                                            m_foregroundSprite.Scale = Vector2.One;
                                            m_backgroundSprite.ChangeSprite("GardenBG_Sprite", ScreenManager.Camera);
                                            m_foregroundSprite.ChangeSprite("GardenFG_Sprite", ScreenManager.Camera);
                                            m_backgroundSprite.Scale = new Vector2(2, 2);
                                            m_foregroundSprite.Scale = new Vector2(2, 2);
                                            break;
                                    }
                                }

                                if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
                                {
                                    m_foregroundSprite.Scale = Vector2.One;
                                    m_foregroundSprite.ChangeSprite("NeoFG_Sprite", ScreenManager.Camera);
                                    m_foregroundSprite.Scale = new Vector2(2, 2);
                                }

                                // Setting shadow intensity.
                                if (roomObj.LevelType == GameTypes.LevelType.DUNGEON  || Game.PlayerStats.Traits.X == TraitType.Glaucoma || Game.PlayerStats.Traits.Y == TraitType.Glaucoma || roomObj.Name == "Compass")
                                    Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0.7f);
                                else
                                    Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0);

                                // Tower frame parallaxing effect.
                                //if (roomObj.LevelType == GameTypes.LevelType.TOWER)
                                //{
                                //    m_gameObjStartPos.Clear();
                                //    foreach (GameObj obj in roomObj.GameObjList)
                                //        m_gameObjStartPos.Add(obj.Position);
                                //}

                                //m_roomTitle.Text = "Now Entering\n" + WordBuilder.BuildDungeonName(roomObj.LevelType);
                                m_roomTitle.Text = LocaleBuilder.getString(WordBuilder.BuildDungeonNameLocID(roomObj.LevelType), m_roomTitle);
                                if (Game.PlayerStats.Traits.X == TraitType.Dyslexia || Game.PlayerStats.Traits.Y == TraitType.Dyslexia)
                                    m_roomTitle.RandomizeSentence(false);

                                m_roomTitle.Opacity = 0;

                                if (roomObj.Name != "Boss" && roomObj.Name != "Tutorial" && roomObj.Name != "Ending" && roomObj.Name != "ChallengeBoss")// && roomObj.Name != "CastleEntrance")
                                {
                                    Tween.StopAllContaining(m_roomEnteringTitle, false);
                                    Tween.StopAllContaining(m_roomTitle, false);
                                    m_roomTitle.Opacity = 0;
                                    m_roomEnteringTitle.Opacity = 0;

                                    if (m_player.X > roomObj.Bounds.Center.X)
                                    {
                                        m_roomTitle.X = 50;
                                        m_roomTitle.Align = Types.TextAlign.Left;
                                        m_roomEnteringTitle.X = 70;
                                        m_roomEnteringTitle.Align = Types.TextAlign.Left;
                                    }
                                    else
                                    {
                                        m_roomTitle.X = 1320 - 50;
                                        m_roomTitle.Align = Types.TextAlign.Right;
                                        m_roomEnteringTitle.X = 1320 - 70;
                                        m_roomEnteringTitle.Align = Types.TextAlign.Right;
                                    }

                                    Tween.To(m_roomTitle, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "1");
                                    m_roomTitle.Opacity = 1; // This is necessary because the tweener stores the initial value of the property when it is called.
                                    Tween.To(m_roomTitle, 0.5f, Linear.EaseNone, "delay", "2.2", "Opacity", "0");
                                    m_roomTitle.Opacity = 0;

                                    Tween.To(m_roomEnteringTitle, 0.5f, Linear.EaseNone,"Opacity", "1");
                                    m_roomEnteringTitle.Opacity = 1; // This is necessary because the tweener stores the initial value of the property when it is called.
                                    Tween.To(m_roomEnteringTitle, 0.5f, Linear.EaseNone, "delay", "2", "Opacity", "0");
                                    m_roomEnteringTitle.Opacity = 0;
                                }
                                else
                                {
                                    Tween.StopAllContaining(m_roomEnteringTitle, false);
                                    Tween.StopAllContaining(m_roomTitle, false);
                                    m_roomTitle.Opacity = 0;
                                    m_roomEnteringTitle.Opacity = 0;
                                }

                                JukeboxEnabled = false;
                                Console.WriteLine("Now entering " + roomObj.LevelType);
                            }

                            //ResetEnemyPositions(); // Must be called before the current room is set. Resets the positions of all enemies in the previous room.

                            if (m_currentRoom != null)
                                m_currentRoom.OnExit(); // Call on exit if exiting from a room. This also removes all dementia enemies from the room. IMPORTANT.

                            m_currentRoom = roomObj; // Sets to newly entered room to be the current room.

                            // Necessary to keep track of which room the player is in otherwise it won't load in the correct room at start up.
                            //if (m_currentRoom.Name == "Boss" && LevelEV.RUN_TESTROOM == false)
                            //    Game.PlayerStats.RespawnPos = m_currentRoom.LinkedRoom.Position;
                            //(ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData); // Saving player data.

                            //if (m_currentRoom.Name != "Start" && m_currentRoom.Name != "CastleEntrance")
                              //  (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.MapData); // Saving map data.

                            m_backgroundSprite.Position = CurrentRoom.Position;
                            m_foregroundSprite.Position = CurrentRoom.Position;
                            m_gardenParallaxFG.Position = CurrentRoom.Position;

                            if (SoundManager.IsMusicPaused == true)
                                SoundManager.ResumeMusic();

                            if (DisableSongUpdating == false && JukeboxEnabled == false)
                                UpdateLevelSong();

                            if (m_currentRoom.Player == null)
                                m_currentRoom.Player = m_player;
                            //m_currentRoom.OnEnter();

                            if (m_currentRoom.Name != "Start" && m_currentRoom.Name != "Tutorial" && m_currentRoom.Name != "Ending" &&
                                m_currentRoom.Name != "CastleEntrance" && m_currentRoom.Name != "Bonus" && m_currentRoom.Name != "Throne" &&
                                m_currentRoom.Name != "Secret" && m_currentRoom.Name != "Boss" && m_currentRoom.LevelType != GameTypes.LevelType.NONE && m_currentRoom.Name != "ChallengeBoss")
                            {
                                if (Game.PlayerStats.Traits.X == TraitType.Dementia || Game.PlayerStats.Traits.Y == TraitType.Dementia)
                                {
                                    if (CDGMath.RandomFloat(0, 1) < GameEV.TRAIT_DEMENTIA_SPAWN_CHANCE)
                                        SpawnDementiaEnemy();
                                }
                            }

                            if (m_currentRoom.HasFairyChest)// && ScreenManager.GetScreens().Length > 0)
                                m_currentRoom.DisplayFairyChestInfo();

                            m_tempEnemyStartPositions.Clear();
                            m_enemyStartPositions.Clear(); // Clear out the start position array.
                            foreach (EnemyObj enemy in CurrentRoom.EnemyList) // Saves all enemy positions in the new room to an array for when the player exits the room.
                                m_enemyStartPositions.Add(enemy.Position);
                            foreach (EnemyObj enemy in CurrentRoom.TempEnemyList) // Saves all enemy positions in the new room to an array for when the player exits the room.
                                m_tempEnemyStartPositions.Add(enemy.Position);

                            m_projectileManager.DestroyAllProjectiles(false);
                            LoadPhysicsObjects(roomObj);
                            //m_miniMapDisplay.AddRoom(roomObj); // Add the room to the map display.
                            m_itemDropManager.DestroyAllItemDrops();
                            m_projectileIconPool.DestroyAllIcons(); // Destroys all icons for projectiles in the room.

                            m_enemyPauseDuration = 0; // Resets the enemy pause counter. Don't unpause all enemies because they will unpause when Enemy.ResetState() is called.


                            if (LevelEV.SHOW_ENEMY_RADII == true)
                            {
                                foreach (EnemyObj enemy in roomObj.EnemyList)
                                {
                                    enemy.InitializeDebugRadii();
                                }
                            }
                            m_lastEnemyHit = null; // Clear out last enemy hit.

                            foreach (GameObj obj in m_currentRoom.GameObjList)
                            {
                                FairyChestObj chest = obj as FairyChestObj;
                                if (chest != null && chest.IsOpen == false) // Always reset chests.
                                {
                                    //chest.State = ChestConditionChecker.STATE_LOCKED;
                                    //chest.TextureColor = Color.White;
                                    //chest.ResetChest();
                                }

                                //ObjContainer objContainer = obj as ObjContainer;
                                IAnimateableObj objContainer = obj as IAnimateableObj;
                                // This is the code that sets the frame rate and whether to animate objects in the room.
                                if (objContainer != null && objContainer.TotalFrames > 1 && !(objContainer is ChestObj) && !(obj is BreakableObj)) // What's this code for?
                                {
                                    objContainer.AnimationDelay = 1 / 10f;
                                    objContainer.PlayAnimation(true);
                                }
                            }

                            if (DisableRoomOnEnter == false)
                                m_currentRoom.OnEnter(); 
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateLevelSong()
        {
            //if (!(m_currentRoom is StartingRoomObj) && !(m_currentRoom is IntroRoomObj) && SoundManager.IsMusicPlaying == false)
            if (CurrentRoom.Name != "Start" && CurrentRoom.Name != "Tutorial" && CurrentRoom.Name != "Ending" &&  SoundManager.IsMusicPlaying == false)
            {
                if (m_currentRoom is CarnivalShoot1BonusRoom || m_currentRoom is CarnivalShoot2BonusRoom)
                    SoundManager.PlayMusic("PooyanSong", true, 1);
                else
                {
                    switch (m_currentRoom.LevelType)
                    {
                        default:
                        case (GameTypes.LevelType.CASTLE):
                            SoundManager.PlayMusic("CastleSong", true, 1);
                            break;
                        case (GameTypes.LevelType.GARDEN):
                            SoundManager.PlayMusic("GardenSong", true, 1);
                            break;
                        case (GameTypes.LevelType.TOWER):
                            SoundManager.PlayMusic("TowerSong", true, 1);
                            break;
                        case (GameTypes.LevelType.DUNGEON):
                            SoundManager.PlayMusic("DungeonSong", true, 1);
                            break;
                    }
                }
            }
            else if (!(m_currentRoom is StartingRoomObj) && SoundManager.IsMusicPlaying == true)
            {
                if ((m_currentRoom is CarnivalShoot1BonusRoom || m_currentRoom is CarnivalShoot2BonusRoom) && SoundManager.GetCurrentMusicName() != "PooyanSong")
                    SoundManager.PlayMusic("PooyanSong", true, 1);
                else
                {
                    if (m_currentRoom.LevelType == GameTypes.LevelType.CASTLE && SoundManager.GetCurrentMusicName() != "CastleSong")
                        SoundManager.PlayMusic("CastleSong", true, 1);
                    else if (m_currentRoom.LevelType == GameTypes.LevelType.GARDEN && SoundManager.GetCurrentMusicName() != "GardenSong")
                        SoundManager.PlayMusic("GardenSong", true, 1);
                    else if (m_currentRoom.LevelType == GameTypes.LevelType.DUNGEON && SoundManager.GetCurrentMusicName() != "DungeonSong")
                        SoundManager.PlayMusic("DungeonSong", true, 1);
                    else if (m_currentRoom.LevelType == GameTypes.LevelType.TOWER && SoundManager.GetCurrentMusicName() != "TowerSong")
                        SoundManager.PlayMusic("TowerSong", true, 1);
                }
            }
        }

        private float m_fakeElapsedTotalHour = (1f / 60f) / (60f * 60f);
        public override void Update(GameTime gameTime)
        {
            //if (InputManager.JustPressed(Keys.L, PlayerIndex.One))
            //{
            //    if (Camera.Zoom != 2)
            //        Camera.Zoom = 2;
            //    else
            //        Camera.Zoom = 1;
            //}
            //if (InputManager.JustPressed(Keys.K, PlayerIndex.One))
            //{
            //    if (Camera.Zoom != 0.5)
            //        Camera.Zoom = 0.5f;
            //    else
            //        Camera.Zoom = 1;
            //}

            //if (InputManager.JustPressed(Keys.B, null))
            //    this.ResetEnemyPositions();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_projectileIconPool.Update(Camera);

            if (this.IsPaused == false)
            {
                //TotalGameTimeHours = (float)gameTime.TotalGameTime.TotalHours;
                float elapsedTotalHours = (float)gameTime.ElapsedGameTime.TotalHours;
                if (elapsedTotalHours <= 0) // This is a check to ensure total GameTime is always incremented.
                    elapsedTotalHours = m_fakeElapsedTotalHour;
                Game.HoursPlayedSinceLastSave += elapsedTotalHours;

                m_sky.Update(gameTime);

                if (m_enemyPauseDuration > 0)
                {
                    m_enemyPauseDuration -= elapsed;
                    if (m_enemyPauseDuration <= 0)
                        StopTimeStop();
                }

                CurrentRoom.Update(gameTime);

                if (m_player != null)
                    m_player.Update(gameTime);

                m_enemyHUD.Update(gameTime);
                m_playerHUD.Update(m_player);

                m_projectileManager.Update(gameTime);
                m_physicsManager.Update(gameTime);

                // Only check for room transitions if the player steps out of the camera zone.
                if (DisableRoomTransitioning == false && CollisionMath.Intersects(new Rectangle((int)m_player.X, (int)m_player.Y, 1,1), Camera.Bounds) == false)
                    CheckForRoomTransition();

                if ((m_inputMap.Pressed(INPUT_LEFTCONTROL) == false || (m_inputMap.Pressed(INPUT_LEFTCONTROL) == true && (LevelEV.RUN_DEMO_VERSION == true || LevelEV.CREATE_RETAIL_VERSION == true))) && CameraLockedToPlayer == true)
                    UpdateCamera(); // Must be called AFTER the PhysicsManager Update() because the PhysicsManager changes the player's position depending on what he/she is colliding with.

                if (Game.PlayerStats.SpecialItem == SpecialItemType.Compass && CurrentRoom.Name != "Start" && CurrentRoom.Name !="Tutorial" && CurrentRoom.Name != "Boss" && CurrentRoom.Name != "Throne" && CurrentRoom.Name != "ChallengeBoss")
                {
                    if (m_compassDisplayed == false) // Display compass here
                        DisplayCompass();
                    else
                        UpdateCompass();
                }
                else
                {
                    if (m_compassDisplayed == true && CurrentRoom.Name != "Compass")
                        HideCompass();
                }

                // This means the objective plate is displayed. Now we are checking to make sure if any enemy of player collides with it, change its opacity.
                if (m_objectivePlate.X == 1170)
                {
                    bool objectivePlateCollides = false;
                    Rectangle objectivePlateAbsRect = m_objectivePlate.Bounds;
                    objectivePlateAbsRect.X += (int)Camera.TopLeftCorner.X;
                    objectivePlateAbsRect.Y += (int)Camera.TopLeftCorner.Y;

                    if (CollisionMath.Intersects(m_player.Bounds, objectivePlateAbsRect))
                        objectivePlateCollides = true;

                    if (objectivePlateCollides == false)
                    {
                        foreach (EnemyObj enemy in CurrentRoom.EnemyList)
                        {
                            if (CollisionMath.Intersects(enemy.Bounds, objectivePlateAbsRect))
                            {
                                objectivePlateCollides = true;
                                break;
                            }
                        }
                    }

                    if (objectivePlateCollides == true)
                        m_objectivePlate.Opacity = 0.5f;
                    else
                        m_objectivePlate.Opacity = 1;
                }

                if (CurrentRoom != null && (CurrentRoom is BonusRoomObj == false))
                {
                    if (m_elapsedScreenShake > 0)
                    {
                        m_elapsedScreenShake -= elapsed;
                        if (m_elapsedScreenShake <= 0)
                        {
                            if (Game.PlayerStats.Traits.X == (int)TraitType.Clonus || Game.PlayerStats.Traits.Y == (int)TraitType.Clonus)
                            {
                                ShakeScreen(1, true, true);
                                GamePad.SetVibration(PlayerIndex.One, 0.25f, 0.25f);
                                Tween.RunFunction(CDGMath.RandomFloat(1, 1.5f), this, "StopScreenShake");
                                m_elapsedScreenShake = CDGMath.RandomFloat(GameEV.TRAIT_CLONUS_MIN, GameEV.TRAIT_CLONUS_MAX);
                            }
                        }
                    }

                    if (m_shakeScreen == true)
                        UpdateShake();
                }
            }

            base.Update(gameTime); // Necessary to update the ScreenManager.
        }

        public void UpdateCamera()
        {
            if (m_player != null)
            {
                ScreenManager.Camera.X = (int)(m_player.Position.X + GlobalEV.Camera_XOffset);
                ScreenManager.Camera.Y = (int)(m_player.Position.Y + GlobalEV.Camera_YOffset);
            }

            if (m_currentRoom != null)
            {
                //Constrain the X-Axis of the camera to the current room.
                if (ScreenManager.Camera.Width < m_currentRoom.Width)
                {
                    if (ScreenManager.Camera.Bounds.Left < m_currentRoom.Bounds.Left)
                        ScreenManager.Camera.X = (int)(m_currentRoom.Bounds.Left + ScreenManager.Camera.Width * 0.5f);
                    else if (ScreenManager.Camera.Bounds.Right > m_currentRoom.Bounds.Right)
                        ScreenManager.Camera.X = (int)(m_currentRoom.Bounds.Right - ScreenManager.Camera.Width * 0.5f);
                }
                else
                    ScreenManager.Camera.X = (int)(m_currentRoom.X + m_currentRoom.Width * 0.5f);

                //Constrain the Y-Axis of the camera to the current room.
                if (ScreenManager.Camera.Height < m_currentRoom.Height)
                {
                    if (ScreenManager.Camera.Bounds.Top < m_currentRoom.Bounds.Top)
                        ScreenManager.Camera.Y = (int)(m_currentRoom.Bounds.Top + ScreenManager.Camera.Height * 0.5f);
                    else if (ScreenManager.Camera.Bounds.Bottom > m_currentRoom.Bounds.Bottom)
                        ScreenManager.Camera.Y = (int)(m_currentRoom.Bounds.Bottom - ScreenManager.Camera.Height * 0.5f);
                }
                else
                    ScreenManager.Camera.Y = (int)(m_currentRoom.Y + m_currentRoom.Height * 0.5f);
            }
        }

        //HandleInput is called AFTER Update().
        public override void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_PAUSE) && CurrentRoom.Name != "Ending")
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Pause, true);

            if (LevelEV.ENABLE_DEBUG_INPUT == true)
                HandleDebugInput();

            if (m_player != null && (m_inputMap.Pressed(INPUT_LEFTCONTROL) == false || (m_inputMap.Pressed(INPUT_LEFTCONTROL) == true && (LevelEV.RUN_DEMO_VERSION == true || LevelEV.CREATE_RETAIL_VERSION == true))) && m_player.IsKilled == false)
                m_player.HandleInput();

            base.HandleInput();
        }

        private void HandleDebugInput()
        {
            if (InputManager.JustPressed(Keys.RightControl, null))
            {
                if (SoundManager.GetCurrentMusicName() == "CastleSong")
                    SoundManager.PlayMusic("TowerSong", true, 0.5f);
                else if (SoundManager.GetCurrentMusicName() == "TowerSong")
                    SoundManager.PlayMusic("DungeonBoss", true, 0.5f);
                else
                    SoundManager.PlayMusic("CastleSong", true, 0.5f);
            }

            if (m_inputMap.JustPressed(INPUT_TOGGLEMAP))
            {
                m_miniMapDisplay.AddAllRooms(m_roomList);
                //(ScreenManager as RCScreenManager).AddRoomsToMap(m_miniMapDisplay.AddedRoomsList);
                //(ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Map, true, null);
            }

            if (m_inputMap.JustPressed(INPUT_DISPLAYROOMINFO))
                LevelEV.SHOW_DEBUG_TEXT = !LevelEV.SHOW_DEBUG_TEXT;

            if (m_inputMap.JustPressed(INPUT_TOGGLEZOOM))
            {
                //CameraLockedToPlayer = false;
                if (Camera.Zoom < 1)
                    Camera.Zoom = 1;
                else
                    //Tween.To(Camera, 4, Quad.EaseInOut, "Zoom", "0.05");
                    Camera.Zoom = 0.05f;
            }

            float debugCameraSpeed = 2000;
            if (m_inputMap.Pressed(INPUT_LEFTCONTROL) && m_inputMap.Pressed(INPUT_LEFT))
                Camera.X -= debugCameraSpeed * (float)Camera.GameTime.ElapsedGameTime.TotalSeconds;
            else if (m_inputMap.Pressed(INPUT_LEFTCONTROL) && m_inputMap.Pressed(INPUT_RIGHT))
                Camera.X += debugCameraSpeed * (float)Camera.GameTime.ElapsedGameTime.TotalSeconds;

            if (m_inputMap.Pressed(INPUT_LEFTCONTROL) && m_inputMap.Pressed(INPUT_UP))
                Camera.Y -= debugCameraSpeed * (float)Camera.GameTime.ElapsedGameTime.TotalSeconds;
            else if (m_inputMap.Pressed(INPUT_LEFTCONTROL) && m_inputMap.Pressed(INPUT_DOWN))
                Camera.Y += debugCameraSpeed * (float)Camera.GameTime.ElapsedGameTime.TotalSeconds;

            if (InputManager.JustPressed(Keys.C, null))
                ToggleMagentaBG();
         
            //if (InputManager.JustPressed(Keys.H, null))
            //    ZoomOutAllObjects();
        }

        private void UpdateCompass()
        {
            if (m_compassDoor == null && CurrentRoom.Name != "Ending" && CurrentRoom.Name != "Boss" && CurrentRoom.Name != "Start" && CurrentRoom.Name != "Tutorial" && CurrentRoom.Name !=" ChallengeBoss")
            {
                Console.WriteLine("Creating new bonus room for compass");
                RoomObj roomToLink = null;
                EnemyObj enemyToLink = null;

                List<RoomObj> acceptableRooms = new List<RoomObj>();
                foreach (RoomObj room in m_roomList)
                {
                    bool hasEnemies = false;
                    foreach (EnemyObj enemy in room.EnemyList)
                    {
                        if (enemy.IsWeighted == true)
                        {
                            hasEnemies = true;
                            break;
                        }
                    }

                    // No need to check for CastleEntrance or linker because they have no enemies in them.
                    if (room.Name != "Ending" && room.Name != "Tutorial" && room.Name != "Boss" && room.Name != "Secret" && room.Name != "Bonus" && hasEnemies == true && room.Name != "ChallengeBoss")
                        acceptableRooms.Add(room);
                }

                if (acceptableRooms.Count > 0)
                {
                    roomToLink = acceptableRooms[CDGMath.RandomInt(0, acceptableRooms.Count - 1)];
                    int counter = 0;
                    while (enemyToLink == null || enemyToLink.IsWeighted == false)
                    {
                        enemyToLink = roomToLink.EnemyList[counter];
                        counter++;
                    }

                    DoorObj door = new DoorObj(roomToLink, 120, 180, GameTypes.DoorType.OPEN);
                    door.Position = enemyToLink.Position;
                    door.IsBossDoor = true;
                    door.DoorPosition = "None";
                    door.AddCollisionBox(0, 0, door.Width, door.Height, Consts.TERRAIN_HITBOX); // This adds the terrain collision box for terrain objects.
                    door.AddCollisionBox(0, 0, door.Width, door.Height, Consts.BODY_HITBOX); // This adds a body collision box to terrain objects.

                    float closestGround = float.MaxValue;
                    TerrainObj closestTerrain = null;
                    foreach (TerrainObj terrainObj in roomToLink.TerrainObjList)
                    {
                        if (terrainObj.Y >= door.Y)
                        {
                            if (terrainObj.Y - door.Y < closestGround && CollisionMath.Intersects(terrainObj.Bounds, new Rectangle((int)door.X, (int)(door.Y + (terrainObj.Y - door.Y) + 5), door.Width, (int)(door.Height / 2))))
                            {
                                closestGround = terrainObj.Y - door.Y;
                                closestTerrain = terrainObj;
                            }
                        }
                    }

                    if (closestTerrain != null)
                    {
                        door.UpdateCollisionBoxes();
                        if (closestTerrain.Rotation == 0)
                            door.Y = closestTerrain.Y - (door.TerrainBounds.Bottom - door.Y);
                        else
                            HookEnemyToSlope(door, closestTerrain);
                    }

                    roomToLink.DoorList.Add(door);

                    roomToLink.LinkedRoom = m_roomList[m_roomList.Count - 1]; // The last room is always the compass room.
                    roomToLink.LinkedRoom.LinkedRoom = roomToLink;
                    roomToLink.LinkedRoom.LevelType = roomToLink.LevelType;

                    string castleCornerTextureString = "CastleCorner_Sprite";
                    string castleCornerLTextureString = "CastleCornerL_Sprite";
                    string towerCornerTextureString = "TowerCorner_Sprite";
                    string towerCornerLTextureString = "TowerCornerL_Sprite";
                    string dungeonCornerTextureString = "DungeonCorner_Sprite";
                    string dungeonCornerLTextureString = "DungeonCornerL_Sprite";
                    string gardenCornerTextureString = "GardenCorner_Sprite";
                    string gardenCornerLTextureString = "GardenCornerL_Sprite";

                    if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
                    {
                        string futureCornerTextureString = "NeoCorner_Sprite";
                        string futureCornerLTextureString = "NeoCornerL_Sprite";
                        castleCornerLTextureString = dungeonCornerLTextureString = towerCornerLTextureString = gardenCornerLTextureString = futureCornerLTextureString;
                        castleCornerTextureString = dungeonCornerTextureString = towerCornerTextureString = gardenCornerTextureString = futureCornerTextureString;
                    }

                    foreach (BorderObj border in roomToLink.LinkedRoom.BorderList)
                    {
                        switch (roomToLink.LinkedRoom.LevelType)
                        {
                            case (GameTypes.LevelType.TOWER):
                                border.SetBorderTextures(m_towerBorderTexture, towerCornerTextureString, towerCornerLTextureString);
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                border.SetBorderTextures(m_dungeonBorderTexture, dungeonCornerTextureString, dungeonCornerLTextureString);
                                break;
                            case (GameTypes.LevelType.GARDEN):
                                border.SetBorderTextures(m_gardenBorderTexture, gardenCornerTextureString, gardenCornerLTextureString);
                                border.TextureOffset = new Vector2(0, -18);
                                break;
                            case (GameTypes.LevelType.CASTLE):
                            default:
                                border.SetBorderTextures(m_castleBorderTexture, castleCornerTextureString, castleCornerLTextureString);
                                break;
                        }
                    }

                    m_compassDoor = door;
                }
            }

            if (m_compassDoor != null)
                m_compass.Rotation = CDGMath.AngleBetweenPts(m_player.Position, new Vector2(m_compassDoor.Bounds.Center.X, m_compassDoor.Bounds.Center.Y));
        }

        public void RemoveCompassDoor()
        {
            if (m_compassDoor != null)
            {
                m_compassDoor.Room.DoorList.Remove(m_compassDoor);
                m_compassDoor.Dispose();
                m_compassDoor = null;
            }
        }

        private void DisplayCompass()
        {
            Tween.StopAllContaining(m_compassBG, false);
            Tween.StopAllContaining(m_compass, false);
            Tween.To(m_compassBG, 0.5f, Back.EaseOutLarge, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_compass, 0.5f, Back.EaseOutLarge, "ScaleX", "1", "ScaleY", "1");
            m_compassDisplayed = true;
        }

        private void HideCompass()
        {
            Tween.StopAllContaining(m_compassBG, false);
            Tween.StopAllContaining(m_compass, false);
            Tween.To(m_compassBG, 0.5f, Back.EaseInLarge, "ScaleX", "0", "ScaleY", "0");
            Tween.To(m_compass, 0.5f, Back.EaseInLarge, "ScaleX", "0", "ScaleY", "0");
            m_compassDisplayed = false;
            RemoveCompassDoor();
        }

        private RenderTarget2D m_finalRenderTarget; // The final render target that is drawn.
        private RenderTarget2D m_fgRenderTarget; // The render target that the foreground is drawn on.
        private RenderTarget2D m_bgRenderTarget; // The render target that the background is drawn on.
        private RenderTarget2D m_skyRenderTarget; // The sky is drawn on this render target.

        // Special pixel shader render targets.
        private RenderTarget2D m_shadowRenderTarget; // The render target used to drawn the shadows in the dungeon.
        private RenderTarget2D m_lightSourceRenderTarget; // Also used to calculate shadows.  Maybe these can be merged somehow.
        private RenderTarget2D m_traitAuraRenderTarget; // A render target used to draw trait effects like near sighted.

        private BackgroundObj m_foregroundSprite, m_backgroundSprite, m_backgroundParallaxSprite, m_gardenParallaxFG;

        private const SurfaceFormat fgTargetFormat = SurfaceFormat.Color;
        private const SurfaceFormat effectTargetFormat = SurfaceFormat.Color;

        public void InitializeRenderTargets()
        {
            int screenWidth = GlobalEV.ScreenWidth;
            int screenHeight = GlobalEV.ScreenHeight;

            if (LevelEV.SAVE_FRAMES == true)
            {
                screenWidth /= 2;
                screenHeight /= 2;
            }

            // Initializing foreground render target.
            if (m_fgRenderTarget != null)
                m_fgRenderTarget.Dispose();
            m_fgRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, screenWidth, screenHeight, false, fgTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (m_shadowRenderTarget != null) m_shadowRenderTarget.Dispose();
            m_shadowRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, screenWidth, screenHeight, false, effectTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Camera.Begin();
            Camera.GraphicsDevice.SetRenderTarget(m_shadowRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black); // Requires no wrap.
            Camera.End();

            if (m_lightSourceRenderTarget != null) m_lightSourceRenderTarget.Dispose();
            m_lightSourceRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, screenWidth, screenHeight, false, effectTargetFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (m_finalRenderTarget != null) m_finalRenderTarget.Dispose();            
            m_finalRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (m_skyRenderTarget != null) m_skyRenderTarget.Dispose();
            m_skyRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (m_bgRenderTarget != null) m_bgRenderTarget.Dispose();
            m_bgRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            if (m_traitAuraRenderTarget != null) m_traitAuraRenderTarget.Dispose();
            m_traitAuraRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            InitializeBackgroundObjs();
        }

        public void InitializeBackgroundObjs()
        {
            if (m_foregroundSprite != null)
                m_foregroundSprite.Dispose();

            m_foregroundSprite = new BackgroundObj("CastleFG1_Sprite");
            m_foregroundSprite.SetRepeated(true, true, Camera, SamplerState.PointWrap);
            m_foregroundSprite.Scale = new Vector2(2, 2);

            /////////////////////////////////////////////////////////

            // Initializing background render target.
            if (m_backgroundSprite != null)
                m_backgroundSprite.Dispose();

            m_backgroundSprite = new BackgroundObj("CastleBG1_Sprite");
            m_backgroundSprite.SetRepeated(true, true, Camera, SamplerState.PointWrap); // Must be called before anything else.
            m_backgroundSprite.Scale = new Vector2(2f, 2f);

            if (m_backgroundParallaxSprite != null)
                m_backgroundParallaxSprite.Dispose();

            m_backgroundParallaxSprite = new BackgroundObj("TowerBGFrame_Sprite");
            m_backgroundParallaxSprite.SetRepeated(true, true, Camera, SamplerState.PointWrap);
            m_backgroundParallaxSprite.Scale = new Vector2(2, 2);

            /////////////////////////////////////////////////////////
            // Initializing the parallaxing background render target.
            if (m_gardenParallaxFG != null)
                m_gardenParallaxFG.Dispose();

            m_gardenParallaxFG = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_gardenParallaxFG.SetRepeated(true, true, Camera, SamplerState.LinearWrap);
            m_gardenParallaxFG.TextureColor = Color.White;
            //m_gardenParallaxFG.ForceDraw = true;
            m_gardenParallaxFG.Scale = new Vector2(3, 3);
            m_gardenParallaxFG.Opacity = 0.7f;
            m_gardenParallaxFG.ParallaxSpeed = new Vector2(0.3f, 0);
        }

        // All objects that need to be drawn on a render target before being drawn the back buffer go here.
        public void DrawRenderTargets()
        {
            // This happens if graphics virtualization fails two times in a row.  Buggy XNA RenderTarget2Ds.
            if (m_backgroundSprite.Texture.IsContentLost == true)
                this.ReinitializeRTs();    

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.GetTransformation());
            // Drawing the B/W outline of the room to wallpaper on the FG and BG later.
            if (CurrentRoom != null)
                CurrentRoom.DrawRenderTargets(Camera); // Requires LinearWrap.
            Camera.End();

            ///////// ALL DRAW CALLS THAT REQUIRE A MATRIX TRANSFORMATION GO HERE /////////////////
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.GetTransformation());

            // Drawing the tiled foreground onto m_fgRenderTarget.
            Camera.GraphicsDevice.SetRenderTarget(m_fgRenderTarget);
            m_foregroundSprite.Draw(Camera); // Requires PointWrap.

            // Setting sampler state to Linear Wrap since most RTs below require it.
            //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Drawing the trait aura onto m_traitAuraRenderTarget (used for trait effects).
            if (m_enemiesPaused == false)
            {
                if (Game.PlayerStats.Traits.X == TraitType.NearSighted || Game.PlayerStats.Traits.Y == TraitType.NearSighted)
                    m_traitAura.Scale = new Vector2(15, 15);
                else if (Game.PlayerStats.Traits.X == TraitType.FarSighted || Game.PlayerStats.Traits.Y == TraitType.FarSighted)
                    m_traitAura.Scale = new Vector2(8, 8);
                else
                    m_traitAura.Scale = new Vector2(10, 10);
            }
            Camera.GraphicsDevice.SetRenderTarget(m_traitAuraRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Transparent);
            if (CurrentRoom != null)
            {
                m_traitAura.Position = m_player.Position;
                m_traitAura.Draw(Camera); // Requires LinearWrap.
            }

            // Drawing a light source onto a transparent m_lightSourceRenderTarget (used for dungeon lighting).
            Camera.GraphicsDevice.SetRenderTarget(m_lightSourceRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Transparent);
            if (CurrentRoom != null)
            {
                m_dungeonLight.Position = m_player.Position;
                m_dungeonLight.Draw(Camera); // Requires LinearWrap.
            }

            // Drawing a completely black RT onto m_shadowRenderTarget for the shadows in the dungeon.
            //Camera.GraphicsDevice.SetRenderTarget(m_shadowRenderTarget);
            //Camera.GraphicsDevice.Clear(Color.Black); // Requires no wrap.
            Camera.End();

            // Separated the mini map draw calls to deferred to speed up performance. Had to separate them from the sky render target.
            ///////// ALL DRAW CALLS THAT DO NOT REQUIRE A MATRIX TRANSFORMATION GO HERE /////////////////
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            // Making the map render targets.
            m_miniMapDisplay.DrawRenderTargets(Camera); // Requires PointClamp
            Camera.End();

            // Drawing the sky parallax background to m_skyRenderTarget.
            Camera.GraphicsDevice.SetRenderTarget(m_skyRenderTarget);
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null);
            //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            m_sky.Draw(Camera); // Requires PointWrap.
            Camera.End();

            // Setting the render target back to the main render target.
            //Camera.GraphicsDevice.SetRenderTarget(m_finalRenderTarget);
        }

        static Vector2 MoveInCircle(GameTime gameTime, float speed)
        {
            double time = Game.TotalGameTime * speed;

            float x = (float)Math.Cos(time);
            float y = (float)Math.Sin(time);

            return new Vector2(x, y);
        }

        private bool m_toggleMagentaBG = false;

        private void ToggleMagentaBG()
        {
            m_toggleMagentaBG = !m_toggleMagentaBG;
        }

        public override void Draw(GameTime gameTime)
        {
            //Camera.Zoom = 2;
            //BackBufferOpacity = 1; //TEDDY - ADDED FOR THE BLACKENING OF THE BG FOR SNAPSHOTS
            DrawRenderTargets();

            Camera.GraphicsDevice.SetRenderTarget(m_bgRenderTarget);
            ///////// DRAWING BACKGROUND /////////////////////////
            // If the foreground and background effect are merged into one effect, this draw call can be removed.
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.GetTransformation());
            m_backgroundSprite.Draw(Camera);

            if (CurrentRoom != null && Camera.Zoom == 1 && (m_inputMap.Pressed(INPUT_LEFTCONTROL) == false || (m_inputMap.Pressed(INPUT_LEFTCONTROL) == true && (LevelEV.RUN_DEMO_VERSION == true || LevelEV.CREATE_RETAIL_VERSION == true))))
            {
                CurrentRoom.DrawBGObjs(Camera);
                // This line isn't being drawn anyway for some reason.
                //if (CurrentRoom.LevelType == GameTypes.LevelType.TOWER)
                //    m_backgroundParallaxSprite.Draw(Camera);
            }
            else
            {
                // Debug drawing. Holding control allows you to zoom around.
                foreach (RoomObj room in m_roomList)
                    room.DrawBGObjs(Camera);
            }
            Camera.End();

            Camera.GraphicsDevice.SetRenderTarget(m_finalRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black);
            if (m_enemiesPaused == true)
                Camera.GraphicsDevice.Clear(Color.White);
            Camera.GraphicsDevice.Textures[1] = m_skyRenderTarget;
            Camera.GraphicsDevice.Textures[1].GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.ParallaxEffect); // Parallax Effect has been disabled in favour of ripple effect for now.
            if (m_enemiesPaused == false)
                Camera.Draw(m_bgRenderTarget, Vector2.Zero, Color.White);
            Camera.End();
            //////////////////////////////////////////////////////

            //////// DRAWING FOREGROUND///////////
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, Game.BWMaskEffect, Camera.GetTransformation());
            Camera.GraphicsDevice.Textures[1] = m_fgRenderTarget;
            Camera.GraphicsDevice.Textures[1].GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Camera.Draw(CurrentRoom.BGRender, Camera.TopLeftCorner, Color.White);
            Camera.End();
            ///////////////////////////////////////////

            //////// IMPORTANT!!!! //////////////////
            // At this point in time, m_fgRenderTarget, m_bgRenderTarget, and m_skyRenderTarget are no longer needed.
            // They can now be (and should be) re-used for whatever rendertarget processes you need.
            // This will cut down immensely on the render targets needed for the game.
            ////////////////////////////////////////

            ////// DRAWING ACTUAL LEVEL ///////////////////////////////////
            if (LevelEV.SHOW_ENEMY_RADII == false)
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.GetTransformation()); // Set SpriteSortMode to immediate to allow instant changes to samplerstates.
            else
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, Camera.GetTransformation());
            // IT is currently necessary to draw all rooms for debug purposes.         
            //if (CurrentRoom != null && Camera.Zoom == 1 && (m_inputMap.Pressed(INPUT_LEFTCONTROL) == false || (m_inputMap.Pressed(INPUT_LEFTCONTROL) == true && LevelEV.RUN_DEMO_VERSION == true)))
                CurrentRoom.Draw(Camera);
            //else
            //{
            //    foreach (RoomObj room in m_roomList)
            //        room.Draw(Camera);
            //}
             

            if (LevelEV.SHOW_ENEMY_RADII == true)
            {
                foreach (EnemyObj enemy in m_currentRoom.EnemyList)
                {
                    enemy.DrawDetectionRadii(Camera);
                }
            }

            m_projectileManager.Draw(Camera);

            if (m_enemiesPaused == true)
            {
                Camera.End();
                //Camera.GraphicsDevice.SetRenderTarget(m_invertRenderTarget); // Removing m_invertRenderTarget by re-using m_bgRenderTarget.
                Camera.GraphicsDevice.SetRenderTarget(m_bgRenderTarget);
                Camera.GraphicsDevice.Textures[1] = m_traitAuraRenderTarget;
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.InvertShader);
                Camera.Draw(m_finalRenderTarget, Vector2.Zero, Color.White);
                Camera.End();

                Game.HSVEffect.Parameters["Saturation"].SetValue(0);
                Game.HSVEffect.Parameters["UseMask"].SetValue(true);
                Camera.GraphicsDevice.SetRenderTarget(m_finalRenderTarget);
                Camera.GraphicsDevice.Textures[1] = m_traitAuraRenderTarget;
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.HSVEffect);
                Camera.Draw(m_bgRenderTarget, Vector2.Zero, Color.White);
                //Camera.Draw(m_invertRenderTarget, Vector2.Zero, Color.White); // Removing m_invertRenderTarget by re-using m_bgRenderTarget.
                //Camera.End();
            }

            Camera.End();

            if (m_toggleMagentaBG == true)
                Camera.GraphicsDevice.Clear(Color.Magenta);
            // SpriteSortMode changed to deferred.
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformation());
            
            //////  Death animation sprites.
            Camera.Draw(Game.GenericTexture, new Rectangle((int)Camera.TopLeftCorner.X, (int)Camera.TopLeftCorner.Y, 1320, 720), Color.Black * BackBufferOpacity);

            // Player
            //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            if (m_player.IsKilled == false)
                m_player.Draw(Camera);

            if (LevelEV.CREATE_RETAIL_VERSION == false)
            {
                DebugTextObj.Position = new Vector2(Camera.X, Camera.Y - 300);
                DebugTextObj.Draw(Camera);
            }

            m_itemDropManager.Draw(Camera);
            m_impactEffectPool.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.GetTransformation());
            //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            m_textManager.Draw(Camera);

            //// Special code for parallaxing the Garden FG.
            if (CurrentRoom.LevelType == GameTypes.LevelType.TOWER)
            {
                //m_gardenParallaxFG.Position = CurrentRoom.Position - Camera.Position;
                m_gardenParallaxFG.Draw(Camera);
            }

            //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_whiteBG.Draw(Camera);

            //ScreenManager.Camera.Draw_CameraBox();
            //m_physicsManager.DrawAllCollisionBoxes(Camera, Game.GenericTexture, Consts.TERRAIN_HITBOX);
            //m_physicsManager.DrawAllCollisionBoxes(ScreenManager.Camera, Game.GenericTexture, Consts.WEAPON_HITBOX);
            //m_physicsManager.DrawAllCollisionBoxes(ScreenManager.Camera, Game.GenericTexture, Consts.BODY_HITBOX);

            Camera.End();

            /////////// DRAWING THE SHADOWS & LIGHTING //////////////////////////////
            if ((CurrentLevelType == GameTypes.LevelType.DUNGEON || Game.PlayerStats.Traits.X == TraitType.Glaucoma || Game.PlayerStats.Traits.Y == TraitType.Glaucoma) 
                && (Game.PlayerStats.Class != ClassType.Banker2 || (Game.PlayerStats.Class == ClassType.Banker2 && Player.LightOn == false)))
            {
                // Can't do this because switching from a rendertarget and back is a bug in XNA that causes a purple screen.  Might work with Monogame.
                //Camera.GraphicsDevice.SetRenderTarget(m_bgRenderTarget);
                //Camera.GraphicsDevice.Clear(Color.Black);
                //Camera.GraphicsDevice.SetRenderTarget(m_finalRenderTarget);
                Camera.GraphicsDevice.Textures[1] = m_lightSourceRenderTarget;
                Camera.GraphicsDevice.Textures[1].GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.ShadowEffect);
                if (LevelEV.SAVE_FRAMES == true)
                    Camera.Draw(m_shadowRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 1);
                else
                    Camera.Draw(m_shadowRenderTarget, Vector2.Zero, Color.White);
                Camera.End();
            }

            // Myopia effect.
            if (CurrentRoom.Name != "Ending")
            {
                if ((Game.PlayerStats.Traits.X == TraitType.NearSighted || Game.PlayerStats.Traits.Y == TraitType.NearSighted) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                {
                    Game.GaussianBlur.InvertMask = true;
                    Game.GaussianBlur.Draw(m_finalRenderTarget, Camera, m_traitAuraRenderTarget);
                }
                // Hyperopia effect.
                else if ((Game.PlayerStats.Traits.X == TraitType.FarSighted || Game.PlayerStats.Traits.Y == TraitType.FarSighted) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                {
                    Game.GaussianBlur.InvertMask = false;
                    Game.GaussianBlur.Draw(m_finalRenderTarget, Camera, m_traitAuraRenderTarget);
                }
            }

            /////////// DRAWING MINIMAP & ENEMY HUD//////////////////////////////////
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);

            m_projectileIconPool.Draw(Camera);

            m_playerHUD.Draw(Camera);

            if (m_lastEnemyHit != null && m_enemyHUDCounter > 0)
                m_enemyHUD.Draw(Camera);

            if (m_enemyHUDCounter > 0)
                m_enemyHUDCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentRoom.Name != "Start" && CurrentRoom.Name != "Boss" && CurrentRoom.Name != "ChallengeBoss" && m_miniMapDisplay.Visible == true)
            {
                m_mapBG.Draw(Camera);
                m_miniMapDisplay.Draw(Camera);
            }

            if (CurrentRoom.Name != "Boss" && CurrentRoom.Name != "Ending")
            {
                m_compassBG.Draw(Camera);
                m_compass.Draw(Camera);
            }

            m_objectivePlate.Draw(Camera);
            m_roomEnteringTitle.Draw(Camera);
            m_roomTitle.Draw(Camera);

            if (CurrentRoom.Name != "Ending")
            {
                if ((Game.PlayerStats.TutorialComplete == false || Game.PlayerStats.Traits.X == TraitType.Nostalgic || Game.PlayerStats.Traits.Y == TraitType.Nostalgic) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                    m_filmGrain.Draw(Camera);
            }
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            m_blackBorder1.Draw(Camera);
            m_blackBorder2.Draw(Camera);

            Camera.End();
            //////////////////////////////////////////////////////////////

            // This is where you apply all the rendertarget effects.

            // Applying the Fus Ro Dah ripple effect, applying it to m_finalRenderTarget, and saving it to another RT.
            //Camera.GraphicsDevice.SetRenderTarget(m_rippleRenderTarget);
            Camera.GraphicsDevice.SetRenderTarget(m_bgRenderTarget);
            Game.RippleEffect.Parameters["width"].SetValue(ShoutMagnitude);

            Vector2 playerPos = m_player.Position - Camera.TopLeftCorner;
            if (Game.PlayerStats.Class == ClassType.Barbarian || Game.PlayerStats.Class == ClassType.Barbarian2)
            {
                Game.RippleEffect.Parameters["xcenter"].SetValue(playerPos.X / 1320f);
                Game.RippleEffect.Parameters["ycenter"].SetValue(playerPos.Y / 720f);
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.RippleEffect);
            }
            else
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);

            Camera.Draw(m_finalRenderTarget, Vector2.Zero, Color.White);
            Camera.End();

            // Changing to the final Screen manager RenderTarget. This is where the final drawing goes.
            Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);

            if (CurrentRoom.Name != "Ending")
            {
                // Colour blind effect.
                if ((Game.PlayerStats.Traits.X == TraitType.ColorBlind || Game.PlayerStats.Traits.Y == TraitType.ColorBlind) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                {
                    Game.HSVEffect.Parameters["Saturation"].SetValue(0);
                    Game.HSVEffect.Parameters["Brightness"].SetValue(0);
                    Game.HSVEffect.Parameters["Contrast"].SetValue(0);
                    Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.HSVEffect);
                }
                else if ((Game.PlayerStats.TutorialComplete == false || Game.PlayerStats.Traits.X == TraitType.Nostalgic || Game.PlayerStats.Traits.Y == TraitType.Nostalgic) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                {
                    Camera.GraphicsDevice.SetRenderTarget(m_finalRenderTarget);
                    Game.HSVEffect.Parameters["Saturation"].SetValue(0.2f);
                    Game.HSVEffect.Parameters["Brightness"].SetValue(0.1f);
                    Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.HSVEffect);
                    Camera.Draw(m_bgRenderTarget, Vector2.Zero, Color.White);
                    Camera.End();

                    Camera.GraphicsDevice.SetRenderTarget(m_bgRenderTarget);
                    Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
                    Color sepia = new Color(180, 150, 80);
                    Camera.Draw(m_finalRenderTarget, Vector2.Zero, sepia);
                    m_creditsText.Draw(Camera);
                    m_creditsTitleText.Draw(Camera);
                    Camera.End();

                    Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);
                    Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
                }
                else
                    Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
            }
            else
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);

            Camera.Draw(m_bgRenderTarget, Vector2.Zero, Color.White);

            Camera.End();

            base.Draw(gameTime); // Doesn't do anything.
        }

        public void RunWhiteSlashEffect()
        {
            m_whiteBG.Position = CurrentRoom.Position;
            m_whiteBG.Scale = Vector2.One;
            m_whiteBG.Scale = new Vector2(CurrentRoom.Width / m_whiteBG.Width, m_currentRoom.Height / m_whiteBG.Height);
            m_whiteBG.Opacity = 1;
            Tween.To(m_whiteBG, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.RunFunction(0.2f, this, "RunWhiteSlash2");
        }

        public void RunWhiteSlash2()
        {
            m_whiteBG.Position = CurrentRoom.Position;
            m_whiteBG.Scale = Vector2.One;
            m_whiteBG.Scale = new Vector2(CurrentRoom.Width / m_whiteBG.Width, m_currentRoom.Height / m_whiteBG.Height);
            m_whiteBG.Opacity = 1;
            Tween.To(m_whiteBG, 0.2f, Tween.EaseNone, "Opacity", "0");
        }

        // Same as RunWhiteSlash but calls a different sfx.
        public void LightningEffectTwice()
        {
            m_whiteBG.Position = CurrentRoom.Position;
            m_whiteBG.Scale = Vector2.One;
            m_whiteBG.Scale = new Vector2(CurrentRoom.Width / m_whiteBG.Width, m_currentRoom.Height / m_whiteBG.Height);
            m_whiteBG.Opacity = 1;
            Tween.To(m_whiteBG, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.RunFunction(0.2f, this, "LightningEffectOnce");
        }

        public void LightningEffectOnce()
        {
            m_whiteBG.Position = CurrentRoom.Position;
            m_whiteBG.Scale = Vector2.One;
            m_whiteBG.Scale = new Vector2(CurrentRoom.Width / m_whiteBG.Width, m_currentRoom.Height / m_whiteBG.Height);
            m_whiteBG.Opacity = 1;
            Tween.To(m_whiteBG, 1, Tween.EaseNone, "Opacity", "0");
            SoundManager.PlaySound("LightningClap1", "LightningClap2");
        }

        public void SpawnDementiaEnemy()
        {
            List<EnemyObj> enemyObjList = new List<EnemyObj>();

            foreach (EnemyObj enemy in m_currentRoom.EnemyList)
            {
                if (enemy.Type !=  EnemyType.Turret && enemy.Type != EnemyType.SpikeTrap && enemy.Type != EnemyType.Platform && 
                    enemy.Type != EnemyType.Portrait && enemy.Type != EnemyType.Eyeball && enemy.Type != EnemyType.Starburst)
                    enemyObjList.Add(enemy);
            }

            if (enemyObjList.Count > 0)
            {
                EnemyObj enemy = enemyObjList[CDGMath.RandomInt(0, enemyObjList.Count - 1)];
                byte[] enemyList = null;

                if (enemy.IsWeighted == true)
                    enemyList = LevelEV.DEMENTIA_GROUND_LIST;
                else
                    enemyList = LevelEV.DEMENTIA_FLIGHT_LIST;

                EnemyObj newEnemy = EnemyBuilder.BuildEnemy(enemyList[CDGMath.RandomInt(0, enemyList.Length - 1)], null, null, null, GameTypes.EnemyDifficulty.BASIC, true);
                newEnemy.Position = enemy.Position; // Make sure this is set before calling AddEnemyToCurrentRoom()
                newEnemy.SaveToFile = false;
                newEnemy.IsDemented = true;
                newEnemy.NonKillable = true;
                newEnemy.GivesLichHealth = false;
                AddEnemyToCurrentRoom(newEnemy);
            }
        }

        public void AddEnemyToCurrentRoom(EnemyObj enemy)
        {
            //m_currentRoom.EnemyList.Add(enemy);
            m_currentRoom.TempEnemyList.Add(enemy); // Add enemy to the temp list instead of the real one.
            m_physicsManager.AddObject(enemy);
            //m_enemyStartPositions.Add(enemy.Position);
            m_tempEnemyStartPositions.Add(enemy.Position);
            enemy.SetPlayerTarget(m_player);
            enemy.SetLevelScreen(this);
            enemy.Initialize();
        }

        public void RemoveEnemyFromCurrentRoom(EnemyObj enemy, Vector2 startingPos)
        {
            m_currentRoom.TempEnemyList.Remove(enemy);
            m_physicsManager.RemoveObject(enemy);
            m_tempEnemyStartPositions.Remove(startingPos);
        }

        public void RemoveEnemyFromRoom(EnemyObj enemy, RoomObj room, Vector2 startingPos)
        {
            room.TempEnemyList.Remove(enemy);
            m_physicsManager.RemoveObject(enemy);
            m_tempEnemyStartPositions.Remove(startingPos);
        }

        public void RemoveEnemyFromRoom(EnemyObj enemy, RoomObj room)
        {
            int enemyIndex = room.TempEnemyList.IndexOf(enemy);
            if (enemyIndex != -1)
            {
                room.TempEnemyList.RemoveAt(enemyIndex);
                m_physicsManager.RemoveObject(enemy);
                m_tempEnemyStartPositions.RemoveAt(enemyIndex);
            }
        }

        public void ResetEnemyPositions()
        {
            for (int i = 0; i < m_enemyStartPositions.Count; i++)
                CurrentRoom.EnemyList[i].Position = m_enemyStartPositions[i];

            for (int i = 0; i < m_tempEnemyStartPositions.Count; i++)
                CurrentRoom.TempEnemyList[i].Position = m_tempEnemyStartPositions[i];
        }

        public override void PauseScreen()
        {
            if (this.IsPaused == false)
            {
                Tweener.Tween.PauseAll();
                CurrentRoom.PauseRoom();
                ItemDropManager.PauseAllAnimations();
                m_impactEffectPool.PauseAllAnimations();
                if (m_enemiesPaused == false) // Only pause the projectiles if they aren't already paused via time stop.
                    m_projectileManager.PauseAllProjectiles(true);
                SoundManager.PauseAllSounds("Pauseable");

                m_player.PauseAnimation();
                GamePad.SetVibration(PlayerIndex.One, 0, 0);

                base.PauseScreen();
            }
        }

        public override void UnpauseScreen()
        {
            if (this.IsPaused == true)
            {
                Tweener.Tween.ResumeAll();
                CurrentRoom.UnpauseRoom();
                ItemDropManager.ResumeAllAnimations();
                m_impactEffectPool.ResumeAllAnimations();
                if (m_enemiesPaused == false) // Only unpause all projectiles if enemies are paused.
                    m_projectileManager.UnpauseAllProjectiles();
                SoundManager.ResumeAllSounds("Pauseable");

                m_player.ResumeAnimation();
                base.UnpauseScreen();
            }
        }

        public void RunGameOver()
        {
            m_player.Opacity = 1;
            m_killedEnemyObjList.Clear();
            List<Vector2> enemiesKilledInRun = Game.PlayerStats.EnemiesKilledInRun;

            int roomSize = m_roomList.Count;
            for (int i = 0; i < enemiesKilledInRun.Count; i++)
            {
                if (enemiesKilledInRun[i].X != -1 && enemiesKilledInRun[i].Y != -1)
                {
                    if ((int)enemiesKilledInRun[i].X < roomSize)
                    {
                        RoomObj room = m_roomList[(int)enemiesKilledInRun[i].X];
                        int numEnemies = room.EnemyList.Count;
                        if ((int)enemiesKilledInRun[i].Y < numEnemies)
                        {
                            EnemyObj enemy = m_roomList[(int)enemiesKilledInRun[i].X].EnemyList[(int)enemiesKilledInRun[i].Y];
                            m_killedEnemyObjList.Add(enemy);
                        }
                    }
                }

                //EnemyObj enemy = m_roomList[(int)enemiesKilledInRun[i].X].EnemyList[(int)enemiesKilledInRun[i].Y];
                //m_killedEnemyObjList.Add(enemy);
            }

            List<object> dataList = new List<object>();
            dataList.Add(m_player);
            dataList.Add(m_killedEnemyObjList);
            dataList.Add(m_coinsCollected);
            dataList.Add(m_bagsCollected);
            dataList.Add(m_diamondsCollected);
            dataList.Add(m_bigDiamondsCollected);
            dataList.Add(m_objKilledPlayer);

            Tween.RunFunction(0, ScreenManager, "DisplayScreen", ScreenType.GameOver, true, dataList);
        }

        public void RunCinematicBorders(float duration)
        {
            StopCinematicBorders();
            m_blackBorder1.Opacity = 1;
            m_blackBorder2.Opacity = 1;
            m_blackBorder1.Y = 0;
            m_blackBorder2.Y = 720 - m_borderSize;
            float fadeSpeed = 1f;
            Tween.By(m_blackBorder1, fadeSpeed, Quad.EaseInOut, "delay", (duration - fadeSpeed).ToString(), "Y", (-m_borderSize).ToString());
            Tween.By(m_blackBorder2, fadeSpeed, Quad.EaseInOut, "delay", (duration - fadeSpeed).ToString(),"Y", m_borderSize.ToString());
            Tween.To(m_blackBorder1, fadeSpeed, Linear.EaseNone, "delay", (duration - fadeSpeed + 0.2f).ToString(), "Opacity", "0");
            Tween.To(m_blackBorder2, fadeSpeed, Linear.EaseNone, "delay", (duration - fadeSpeed + 0.2f).ToString(), "Opacity", "0");
        }

        public void StopCinematicBorders()
        {
            Tween.StopAllContaining(m_blackBorder1, false);
            Tween.StopAllContaining(m_blackBorder2, false);
        }

        public void DisplayMap(bool isTeleporterScreen)
        {
            //m_miniMapDisplay.AddAllRooms(m_roomList);
            (ScreenManager as RCScreenManager).AddRoomsToMap(m_miniMapDisplay.AddedRoomsList);
            if (isTeleporterScreen == true)
                (ScreenManager as RCScreenManager).ActivateMapScreenTeleporter();
            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Map, true);
        }

        public void PauseAllEnemies()
        {
            m_enemiesPaused = true;
            CurrentRoom.PauseRoom();
            foreach (EnemyObj enemy in CurrentRoom.EnemyList)
                enemy.PauseEnemy();

            foreach (EnemyObj enemy in CurrentRoom.TempEnemyList)
                enemy.PauseEnemy();

            m_projectileManager.PauseAllProjectiles(false);
        }

        public void CastTimeStop(float duration)
        {
            SoundManager.PlaySound("Cast_TimeStart");
            SoundManager.PauseMusic();
            m_enemyPauseDuration = duration;
            PauseAllEnemies();
            Tween.To(m_traitAura, 0.2f, Tween.EaseNone, "ScaleX", "100", "ScaleY", "100");
        }

        public void StopTimeStop()
        {
            SoundManager.PlaySound("Cast_TimeStop");
            SoundManager.ResumeMusic();
            Tween.To(m_traitAura, 0.2f, Tween.EaseNone, "ScaleX", "0", "ScaleY", "0");
            Tween.AddEndHandlerToLastTween(this, "UnpauseAllEnemies");
        }

        public void UnpauseAllEnemies()
        {
            Game.HSVEffect.Parameters["UseMask"].SetValue(false);
            m_enemiesPaused = false;

            CurrentRoom.UnpauseRoom();

            foreach (EnemyObj enemy in CurrentRoom.EnemyList)
                enemy.UnpauseEnemy();

            foreach (EnemyObj enemy in CurrentRoom.TempEnemyList)
                enemy.UnpauseEnemy();

            m_projectileManager.UnpauseAllProjectiles();
        }

        public void DamageAllEnemies(int damage)
        {
            // Do temp enemies first otherwise one of them will get hit twice.
            List<EnemyObj> tempEnemyList = new List<EnemyObj>(); // Necessary because TempEnemyList is a list that is continually modified.
            tempEnemyList.AddRange(CurrentRoom.TempEnemyList);
            foreach (EnemyObj enemy in tempEnemyList)
            {
                if (enemy.IsDemented == false && enemy.IsKilled == false)
                    enemy.HitEnemy(damage, enemy.Position, true);
            }

            tempEnemyList.Clear();
            tempEnemyList = null;

            foreach (EnemyObj enemy in CurrentRoom.EnemyList)
            {
                if (enemy.IsDemented == false && enemy.IsKilled == false)
                    enemy.HitEnemy(damage, enemy.Position, true);
            }
        }

        public virtual void Reset()
        {
            BackBufferOpacity = 0;

            m_killedEnemyObjList.Clear();

            m_bigDiamondsCollected = 0;
            m_diamondsCollected = 0;
            m_coinsCollected = 0;
            m_bagsCollected = 0;
            m_blueprintsCollected = 0;

            if (m_player != null)
            {
                m_player.Reset();
                m_player.ResetLevels();
                m_player.Position = new Vector2(200, 200);
                //UpdatePlayerHUDHP();
                //UpdatePlayerHUDMP();
            }

            ResetEnemyPositions();

            foreach (RoomObj room in m_roomList)
                room.Reset();

            InitializeChests(false);


            foreach (RoomObj room in RoomList)
            {
                foreach (GameObj obj in room.GameObjList)
                {
                    BreakableObj breakableObj = obj as BreakableObj;
                    if (breakableObj != null)
                        breakableObj.Reset();
                }
            }

            m_projectileManager.DestroyAllProjectiles(true);
            Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0);
        }

        public override void DisposeRTs()
        {
            m_fgRenderTarget.Dispose();
            m_fgRenderTarget = null;
            m_bgRenderTarget.Dispose();
            m_bgRenderTarget = null;
            m_skyRenderTarget.Dispose();
            m_skyRenderTarget = null;
            m_finalRenderTarget.Dispose();
            m_finalRenderTarget = null;

            m_shadowRenderTarget.Dispose();
            m_shadowRenderTarget = null;
            m_lightSourceRenderTarget.Dispose();
            m_lightSourceRenderTarget = null;
            m_traitAuraRenderTarget.Dispose();
            m_traitAuraRenderTarget = null;

            m_foregroundSprite.Dispose();
            m_foregroundSprite = null;
            m_backgroundSprite.Dispose();
            m_backgroundSprite = null;
            m_backgroundParallaxSprite.Dispose();
            m_backgroundParallaxSprite = null;
            m_gardenParallaxFG.Dispose();
            m_gardenParallaxFG = null;

            m_roomBWRenderTarget.Dispose();
            m_roomBWRenderTarget = null;

            m_miniMapDisplay.DisposeRTs();
            base.DisposeRTs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Procedural Level Screen");

                Tween.StopAll(false);

                m_currentRoom = null;
                DisposeRTs();
                foreach (RoomObj room in m_roomList)
                    room.Dispose();
                m_roomList.Clear();
                m_roomList = null;
                m_enemyStartPositions.Clear();
                m_enemyStartPositions = null;
                m_tempEnemyStartPositions.Clear();
                m_tempEnemyStartPositions = null;
                m_textManager.Dispose();
                m_textManager = null;
                //m_physicsManager.Dispose(); // Don't dispose the Physics manager since it's created in Game.cs and needs to remain persistent.
                m_physicsManager = null;
                m_projectileManager.Dispose();
                m_projectileManager = null;
                m_itemDropManager.Dispose();
                m_itemDropManager = null;
                m_currentRoom = null;
                m_miniMapDisplay.Dispose();
                m_miniMapDisplay = null;
                m_mapBG.Dispose();
                m_mapBG = null;
                m_inputMap.Dispose();
                m_inputMap = null;
                m_lastEnemyHit = null;
                m_playerHUD.Dispose();
                m_playerHUD = null;
                m_player = null;
                m_enemyHUD.Dispose();
                m_enemyHUD = null;
                m_impactEffectPool.Dispose();
                m_impactEffectPool = null;

                m_blackBorder1.Dispose();
                m_blackBorder1 = null;
                m_blackBorder2.Dispose();
                m_blackBorder2 = null;

                m_chestList.Clear();
                m_chestList = null;

                m_projectileIconPool.Dispose();
                m_projectileIconPool = null;

                m_objKilledPlayer = null;

                m_dungeonLight.Dispose();
                m_dungeonLight = null;
                m_traitAura.Dispose();
                m_traitAura = null;

                m_killedEnemyObjList.Clear();
                m_killedEnemyObjList = null;

                m_roomEnteringTitle.Dispose();
                m_roomEnteringTitle = null;
                m_roomTitle.Dispose();
                m_roomTitle = null;

                m_creditsText.Dispose();
                m_creditsText = null;
                m_creditsTitleText.Dispose();
                m_creditsTitleText = null;
                Array.Clear(m_creditsTextTitleList, 0, m_creditsTextTitleList.Length);
                Array.Clear(m_creditsTextList, 0, m_creditsTextList.Length);
                m_creditsTextTitleList = null;
                m_creditsTextList = null;
                m_filmGrain.Dispose();
                m_filmGrain = null;

                m_objectivePlate.Dispose();
                m_objectivePlate = null;
                m_objectivePlateTween = null;

                m_sky.Dispose();
                m_sky = null;
                m_whiteBG.Dispose();
                m_whiteBG = null;

                m_compassBG.Dispose();
                m_compassBG = null;
                m_compass.Dispose();
                m_compass = null;

                if (m_compassDoor != null)
                    m_compassDoor.Dispose();
                m_compassDoor = null;

                m_castleBorderTexture.Dispose();
                m_gardenBorderTexture.Dispose();
                m_towerBorderTexture.Dispose();
                m_dungeonBorderTexture.Dispose();
                m_neoBorderTexture.Dispose();

                m_castleBorderTexture = null;
                m_gardenBorderTexture = null;
                m_towerBorderTexture = null;
                m_dungeonBorderTexture = null;

                DebugTextObj.Dispose();
                DebugTextObj = null;

                base.Dispose(); // Sets the IsDisposed flag to true.
            }
        }

        public void SetLastEnemyHit(EnemyObj enemy)
        {
            m_lastEnemyHit = enemy;
            m_enemyHUDCounter = m_enemyHUDDuration;
            //m_enemyHUD.UpdateEnemyInfo(m_lastEnemyHit.Name, m_lastEnemyHit.Level, m_lastEnemyHit.CurrentHealth / (float)m_lastEnemyHit.MaxHealth);
            m_enemyHUD.UpdateEnemyInfo(m_lastEnemyHit.LocStringID, m_lastEnemyHit.Level, m_lastEnemyHit.CurrentHealth / (float)m_lastEnemyHit.MaxHealth);
        }

        public void KillEnemy(EnemyObj enemy)
        {
            if (enemy.SaveToFile == true)
            {
                Vector2 killedEnemy = new Vector2(m_roomList.IndexOf(CurrentRoom), CurrentRoom.EnemyList.IndexOf(enemy));

                if (killedEnemy.X < 0 || killedEnemy.Y < 0)
                    throw new Exception("Could not find killed enemy in either CurrentRoom or CurrentRoom.EnemyList. This may be because the enemy was a blob");
                Game.PlayerStats.EnemiesKilledInRun.Add(killedEnemy);
            }
        }

        public void ItemDropCollected(int itemDropType)
        {
            switch (itemDropType)
            {
                case (ItemDropType.Coin):
                    m_coinsCollected++;
                    break;
                case (ItemDropType.MoneyBag):
                    m_bagsCollected++;
                    break;
                case (ItemDropType.Diamond):
                    m_diamondsCollected++;
                    break;
                case(ItemDropType.BigDiamond):
                    m_bigDiamondsCollected++;
                    break;
                case (ItemDropType.Blueprint):
                case (ItemDropType.Redprint):
                    m_blueprintsCollected++;
                    break;
            }
        }

        public void RefreshMapChestIcons()
        {
            m_miniMapDisplay.RefreshChestIcons(CurrentRoom);
            (ScreenManager as RCScreenManager).RefreshMapScreenChestIcons(CurrentRoom);
        }

        public void DisplayObjective(string objectiveTitleID, string objectiveDescriptionID, string objectiveProgressID, bool tween)
        {
            //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"FairyChest_Start");
            // Objective Lines.
            (m_objectivePlate.GetChildAt(4) as SpriteObj).ScaleX = 0;
            (m_objectivePlate.GetChildAt(5) as SpriteObj).ScaleX = 0;

            m_objectivePlate.GetChildAt(2).Opacity = 1f;
            m_objectivePlate.GetChildAt(3).Opacity = 1f;
            m_objectivePlate.X = 1170 + 300;

            if (m_objectivePlateTween != null && m_objectivePlateTween.TweenedObject == m_objectivePlate && m_objectivePlateTween.Active == true)
                m_objectivePlateTween.StopTween(false);

            (m_objectivePlate.GetChildAt(1) as TextObj).Text = LocaleBuilder.getString(objectiveTitleID, m_objectivePlate.GetChildAt(1) as TextObj);
            (m_objectivePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getString(objectiveDescriptionID, m_objectivePlate.GetChildAt(2) as TextObj);
            (m_objectivePlate.GetChildAt(3) as TextObj).Text = LocaleBuilder.getString(objectiveProgressID, m_objectivePlate.GetChildAt(3) as TextObj);

            if (tween == true)
                m_objectivePlateTween = Tween.By(m_objectivePlate, 0.5f, Back.EaseOut, "X", "-300");
            else
                m_objectivePlate.X -= 300;
        }

        public void ResetObjectivePlate(bool tween)
        {
            if (m_objectivePlate != null)
            {
                m_objectivePlate.X = 1170;

                if (m_objectivePlateTween != null && m_objectivePlateTween.TweenedObject == m_objectivePlate && m_objectivePlateTween.Active == true)
                    m_objectivePlateTween.StopTween(false);

                if (tween == true)
                    Tween.By(m_objectivePlate, 0.5f, Back.EaseIn, "X", "300");
                else
                    m_objectivePlate.X += 300;
            }
        }

        // progress parameter is actual display string
        public void UpdateObjectiveProgress(string progress)
        {
            (m_objectivePlate.GetChildAt(3) as TextObj).Text = progress;
        }

        public void ObjectiveFailed()
        {
            (m_objectivePlate.GetChildAt(1) as TextObj).Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_6", m_objectivePlate.GetChildAt(1) as TextObj); //"Objective Failed"
            m_objectivePlate.GetChildAt(2).Opacity = 0.3f;
            m_objectivePlate.GetChildAt(3).Opacity = 0.3f;
        }

        public void ObjectiveComplete()
        {
            // objective lines
            //Tween.By(m_objectivePlate.GetChildAt(4), 0.3f, Tween.EaseNone, "ScaleX", (m_objectivePlate.GetChildAt(2).Width / 5).ToString());
            //if ((m_objectivePlate.GetChildAt(3) as TextObj).Text != "")
            //    Tween.By(m_objectivePlate.GetChildAt(5), 0.3f, Tween.EaseNone, "delay", "0.2", "ScaleX", (m_objectivePlate.GetChildAt(3).Width / 5).ToString());

            m_objectivePlate.GetChildAt(2).Opacity = 0.3f;
            m_objectivePlate.GetChildAt(3).Opacity = 0.3f;

            m_objectivePlate.X = 1170;

            if (m_objectivePlateTween != null && m_objectivePlateTween.TweenedObject == m_objectivePlate && m_objectivePlateTween.Active == true)
                m_objectivePlateTween.StopTween(false);

            (m_objectivePlate.GetChildAt(1) as TextObj).Text = LocaleBuilder.getString("LOC_ID_LEVEL_SCREEN_5", m_objectivePlate.GetChildAt(1) as TextObj); //"Objective Complete!"
            //m_objectivePlateTween = Tween.By(m_objectivePlate, 0.5f, Back.EaseIn, "delay", "1", "X", "300");
        }

        public override void OnEnter()
        {
            (ScreenManager.Game as Game).SaveManager.ResetAutosave();
            m_player.DisableAllWeight = false; // Fixes bug where you translocate right before enter the castle, resulting in screwed up gravity.
            m_player.StopAllSpells();
            StopScreenShake();

            ShoutMagnitude = 3;

            // Setting up player.
            if (Game.PlayerStats.Traits.X == TraitType.Gigantism || Game.PlayerStats.Traits.Y == TraitType.Gigantism)
                m_player.Scale = new Vector2(GameEV.TRAIT_GIGANTISM, GameEV.TRAIT_GIGANTISM);//(3.5f, 3.5f);
            else if (Game.PlayerStats.Traits.X == TraitType.Dwarfism || Game.PlayerStats.Traits.Y == TraitType.Dwarfism)
                m_player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);
            else
                m_player.Scale = new Vector2(2, 2);

            // Modifying the player's scale based on traits.
            if (Game.PlayerStats.Traits.X == TraitType.Ectomorph || Game.PlayerStats.Traits.Y == TraitType.Ectomorph)
            {
                m_player.ScaleX *= 0.825f;
                m_player.ScaleY *= 1.15f;
                //m_player.Scale = new Vector2(1.8f, 2.2f);
            }
            else if (Game.PlayerStats.Traits.X == TraitType.Endomorph || Game.PlayerStats.Traits.Y == TraitType.Endomorph)
            {
                m_player.ScaleX *= 1.25f;
                m_player.ScaleY *= 1.175f;
                //m_player.Scale = new Vector2(2.5f, 2f);
            }

            if (Game.PlayerStats.Traits.X == TraitType.Clonus || Game.PlayerStats.Traits.Y == TraitType.Clonus)
            {
                m_elapsedScreenShake = CDGMath.RandomFloat(GameEV.TRAIT_CLONUS_MIN, GameEV.TRAIT_CLONUS_MAX);
            }

            m_player.CurrentHealth = Game.PlayerStats.CurrentHealth;
            m_player.CurrentMana = Game.PlayerStats.CurrentMana;

            if (LevelEV.RUN_TESTROOM == true)
            {
                Game.ScreenManager.Player.CurrentHealth = Game.ScreenManager.Player.MaxHealth;
                Game.ScreenManager.Player.CurrentMana = Game.ScreenManager.Player.MaxMana;
            }

            m_player.UpdateInternalScale();

            CheckForRoomTransition();
            UpdateCamera();
            UpdatePlayerHUDAbilities();
            m_player.UpdateEquipmentColours();
            m_player.StopAllSpells();

            // Adding treasure chest icons to map for Spelunker.
            if (Game.PlayerStats.Class == ClassType.Banker2)
            {
                m_miniMapDisplay.AddAllIcons(this.RoomList);
                (ScreenManager as RCScreenManager).AddIconsToMap(this.RoomList);
            }

            //// Adding teleporters to all bosses already beaten.
            //if (Game.PlayerStats.EyeballBossBeaten)
            //    m_miniMapDisplay.AddLinkerRoom(GameTypes.LevelType.CASTLE, this.RoomList);
            //if (Game.PlayerStats.FairyBossBeaten)
            //    m_miniMapDisplay.AddLinkerRoom(GameTypes.LevelType.GARDEN, this.RoomList);
            //if (Game.PlayerStats.FireballBossBeaten)
            //    m_miniMapDisplay.AddLinkerRoom(GameTypes.LevelType.TOWER, this.RoomList);
            //if (Game.PlayerStats.BlobBossBeaten)
            //    m_miniMapDisplay.AddLinkerRoom(GameTypes.LevelType.DUNGEON, this.RoomList);

            if (Game.PlayerStats.EyeballBossBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_EYES");
            if (Game.PlayerStats.FairyBossBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_GHOSTS");
            if (Game.PlayerStats.BlobBossBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_SLIME");
            if (Game.PlayerStats.FireballBossBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_FIRE");
            if (Game.PlayerStats.LastbossBeaten == true || Game.PlayerStats.TimesCastleBeaten > 0)
                GameUtil.UnlockAchievement("FEAR_OF_FATHERS");
            if (Game.PlayerStats.TimesCastleBeaten > 1)
                GameUtil.UnlockAchievement("FEAR_OF_TWINS");

            if (Game.PlayerStats.ChallengeEyeballBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_BLINDNESS");
            if (Game.PlayerStats.ChallengeSkullBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_BONES");
            if (Game.PlayerStats.ChallengeFireballBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_CHEMICALS");
            if (Game.PlayerStats.ChallengeBlobBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_SPACE");
            if (Game.PlayerStats.ChallengeLastBossBeaten == true)
                GameUtil.UnlockAchievement("FEAR_OF_RELATIVES");

            bool skeletonMBKilled = false;
            bool plantMBKilled = false;
            bool paintingMBKilled = false;
            bool knightMBKilled = false;
            bool wizardMBKilled = false;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Skeleton].W > 0)
                skeletonMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Plant].W > 0)
                plantMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Portrait].W > 0)
                paintingMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Knight].W > 0)
                knightMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.EarthWizard].W > 0)
                wizardMBKilled = true;

            if (skeletonMBKilled && plantMBKilled && paintingMBKilled && knightMBKilled && wizardMBKilled)
                GameUtil.UnlockAchievement("FEAR_OF_ANIMALS");

            if ((Game.PlayerStats.TotalHoursPlayed + Game.HoursPlayedSinceLastSave) >= 20)
                GameUtil.UnlockAchievement("FEAR_OF_SLEEP");

            if (Game.PlayerStats.TotalRunesFound > 10)
                GameUtil.UnlockAchievement("LOVE_OF_MAGIC");

            base.OnEnter();
        }

        public override void OnExit()
        {
            StopScreenShake();
            if (m_currentRoom != null)
                m_currentRoom.OnExit(); // Call on exit if exiting from a room.

            SoundManager.StopAllSounds("Default");
            SoundManager.StopAllSounds("Pauseable");
            base.OnExit();
        }

        public void RevealMorning()
        {
            m_sky.MorningOpacity = 0;
            Tween.To(m_sky, 2, Tween.EaseNone, "MorningOpacity", "1");
        }

        public void ZoomOutAllObjects()
        {
            Vector2 centrePt = new Vector2(CurrentRoom.Bounds.Center.X, CurrentRoom.Bounds.Center.Y);
            List<Vector2> objPositions = new List<Vector2>();
            float delay = 0;

            foreach (GameObj obj in CurrentRoom.GameObjList)
            {
                int zoomXAmount = 0;
                int zoomYAmount = 0;

                if (obj.Y < centrePt.Y)
                    zoomYAmount = CurrentRoom.Bounds.Top - (int)(obj.Bounds.Top + obj.Bounds.Height);
                else
                    zoomYAmount = CurrentRoom.Bounds.Bottom - (int)(obj.Bounds.Top);

                if (obj.X < centrePt.X)
                    zoomXAmount = (int)(CurrentRoom.Bounds.Left - (obj.Bounds.Left + obj.Bounds.Width));
                else
                    zoomXAmount = (int)(CurrentRoom.Bounds.Right - (obj.Bounds.Left));

                if (Math.Abs(zoomXAmount) > Math.Abs(zoomYAmount))
                {
                    objPositions.Add(new Vector2(0, zoomYAmount));
                    Tween.By(obj, 0.5f, Back.EaseIn, "delay", delay.ToString(), "Y", zoomYAmount.ToString());
                }
                else
                {
                    objPositions.Add(new Vector2(zoomXAmount, 0));
                    Tween.By(obj, 0.5f, Back.EaseIn, "delay", delay.ToString(), "X", zoomXAmount.ToString());
                }
                delay += 0.05f;
            }
            Tween.RunFunction(delay + 0.5f, this, "ZoomInAllObjects", objPositions);
            //Tween.AddEndHandlerToLastTween(this, "ZoomInAllObjects", objPositions);
        }

        public void ZoomInAllObjects(List<Vector2> objPositions)
        {
            int counter = 0;
            float delay = 1;
            foreach (GameObj obj in CurrentRoom.GameObjList)
            {
                Tween.By(obj, 0.5f, Back.EaseOut,"delay", delay.ToString(), "X", (-objPositions[counter].X).ToString(), "Y", (-objPositions[counter].Y).ToString());
                counter++;
                delay += 0.05f;
            }
        }

        public void UpdateLevel(GameTypes.LevelType levelType)
        {
            switch (levelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    m_backgroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.Scale = Vector2.One;
                    m_backgroundSprite.ChangeSprite("CastleBG1_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.ChangeSprite("CastleFG1_Sprite", ScreenManager.Camera);
                    m_backgroundSprite.Scale = new Vector2(2, 2);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                    break;
                case (GameTypes.LevelType.TOWER):
                    m_backgroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.Scale = Vector2.One;
                    m_backgroundSprite.ChangeSprite("TowerBG2_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.ChangeSprite("TowerFG2_Sprite", ScreenManager.Camera);
                    m_backgroundSprite.Scale = new Vector2(2, 2);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    m_backgroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.Scale = Vector2.One;
                    m_backgroundSprite.ChangeSprite("DungeonBG1_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.ChangeSprite("DungeonFG1_Sprite", ScreenManager.Camera);
                    m_backgroundSprite.Scale = new Vector2(2, 2);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                    break;
                case (GameTypes.LevelType.GARDEN):
                    m_backgroundSprite.Scale = Vector2.One;
                    m_foregroundSprite.Scale = Vector2.One;
                    m_backgroundSprite.ChangeSprite("GardenBG_Sprite", ScreenManager.Camera);
                    m_foregroundSprite.ChangeSprite("GardenFG_Sprite", ScreenManager.Camera);
                    m_backgroundSprite.Scale = new Vector2(2, 2);
                    m_foregroundSprite.Scale = new Vector2(2, 2);
                    break;
            }

            // Setting shadow intensity.
            if (levelType == GameTypes.LevelType.DUNGEON)
                Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0.7f);
            else
                Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0);
        }

        private bool m_horizontalShake;
        private bool m_verticalShake;
        private bool m_shakeScreen;
        private float m_screenShakeMagnitude;

        public void ShakeScreen(float magnitude, bool horizontalShake = true, bool verticalShake = true)
        {
            m_screenShakeMagnitude = magnitude;
            m_horizontalShake = horizontalShake;
            m_verticalShake = verticalShake;
            m_shakeScreen = true;
        }

        // This shake is specific to the Clonus trait.
        public void UpdateShake()
        {
            if (m_horizontalShake == true)
                Player.AttachedLevel.Camera.X += CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);

            if (m_verticalShake == true)
                Player.AttachedLevel.Camera.Y += CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);
        }

        public void StopScreenShake()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            m_shakeScreen = false;
        }

        public void RefreshPlayerHUDPos()
        {
            m_playerHUD.SetPosition(new Vector2(20, 40));
        }

        public void UpdatePlayerHUD()
        {
            m_playerHUD.Update(m_player);
        }

        public void ForcePlayerHUDLevel(int level)
        {
            m_playerHUD.forcedPlayerLevel = level;
        }

        public void UpdatePlayerHUDAbilities()
        {
            m_playerHUD.UpdateAbilityIcons();
        }

        public void UpdatePlayerHUDSpecialItem()
        {
            m_playerHUD.UpdateSpecialItemIcon();
        }

        public void UpdatePlayerSpellIcon()
        {
            m_playerHUD.UpdateSpellIcon();
        }

        public void SetMapDisplayVisibility(bool visible)
        {
            m_miniMapDisplay.Visible = visible;
        }

        public void SetPlayerHUDVisibility(bool visible)
        {
            m_playerHUD.Visible = visible;
        }

        public List<RoomObj> MapRoomsUnveiled
        {
            get { return m_miniMapDisplay.AddedRoomsList; }
            set 
            {
                m_miniMapDisplay.ClearRoomsAdded();
                m_miniMapDisplay.AddAllRooms(value);
                //(ScreenManager as RCScreenManager).AddRoomsToMap(value);
            }
        }

        public List<RoomObj> MapRoomsAdded
        {
            get { return m_miniMapDisplay.AddedRoomsList; }
        }

        public PlayerObj Player
        {
            get { return m_player; }
            set { m_player = value; }
        }

        public List<RoomObj> RoomList
        {
            get { return m_roomList; }
        }

        public PhysicsManager PhysicsManager
        {
            get { return m_physicsManager; }
        }

        public RoomObj CurrentRoom
        {
            get { return m_currentRoom; }
        }

        public ProjectileManager ProjectileManager
        {
            get { return m_projectileManager; }
        }

        public List<EnemyObj> EnemyList
        {
            get { return CurrentRoom.EnemyList; }
        }

        public List<ChestObj> ChestList
        {
            get { return m_chestList; }
        }

        public TextManager TextManager
        {
            get { return m_textManager; }
        }

        public ImpactEffectPool ImpactEffectPool
        {
            get { return m_impactEffectPool; }
        }

        public ItemDropManager ItemDropManager
        {
            get { return m_itemDropManager; }
        }

        public GameTypes.LevelType CurrentLevelType
        {
            get { return m_currentRoom.LevelType; }
        }

        public int LeftBorder
        {
            get { return m_leftMostBorder; }
        }

        public int RightBorder
        {
            get { return m_rightMostBorder; }
        }

        public int TopBorder
        {
            get { return m_topMostBorder; }
        }

        public int BottomBorder
        {
            get { return m_bottomMostBorder; }
        }

        public void SetObjectKilledPlayer(GameObj obj)
        {
            m_objKilledPlayer = obj;
        }

        public RenderTarget2D RenderTarget
        {
            get { return m_finalRenderTarget; }
        }

        public bool EnemiesPaused
        {
            get { return m_enemiesPaused; }
        }

        public override void RefreshTextObjs()
        {
            foreach (RoomObj room in m_roomList)
                room.RefreshTextObjs();

            m_playerHUD.RefreshTextObjs();
            m_enemyHUD.RefreshTextObjs();
            base.RefreshTextObjs();
        }
    }
}
