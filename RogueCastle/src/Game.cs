using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpriteSystem;
using InputSystem;
using Tweener;
using Tweener.Ease;
using DS2DEngine;
using System.IO;
using System.Globalization;
using System.Threading;

namespace RogueCastle
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        //Generic textures used for multiple objects.
        public static Texture2D GenericTexture;
        public static Effect MaskEffect;
        public static Effect BWMaskEffect;
        public static Effect ShadowEffect;
        public static Effect ParallaxEffect;
        public static Effect RippleEffect;
        public static GaussianBlur GaussianBlur;
        public static Effect HSVEffect;
        public static Effect InvertShader;
        public static Effect ColourSwapShader;

        public static AreaStruct[] Area1List;

        public GraphicsDeviceManager graphics;
        public static RCScreenManager ScreenManager { get; internal set; }
        SaveGameManager m_saveGameManager;
        PhysicsManager m_physicsManager;

        public static EquipmentSystem EquipmentSystem;
        //public static TraitSystem TraitSystem;

        public static PlayerStats PlayerStats = new PlayerStats();
        public static SpriteFont PixelArtFont;
        public static SpriteFont PixelArtFontBold;
        public static SpriteFont JunicodeFont;
        public static SpriteFont EnemyLevelFont;
        public static SpriteFont PlayerLevelFont;
        public static SpriteFont GoldFont;
        public static SpriteFont HerzogFont;
        public static SpriteFont JunicodeLargeFont;
        public static SpriteFont CinzelFont;
        public static SpriteFont BitFont;
        public static SpriteFont NotoSansSCFont; // Noto Sans Simplified Chinese
        public static SpriteFont RobotoSlabFont;

        public static Cue LineageSongCue;

        public static InputMap GlobalInput;
        public static SettingStruct GameConfig;
        //Song tempSong;

        public static List<string> NameArray;
        public static List<string> FemaleNameArray;

        private string m_commandLineFilePath = "";

        private GameTime m_forcedGameTime1, m_forcedGameTime2;
        private float m_frameLimit = 1 / 40f;
        private bool m_frameLimitSwap;
        public static float TotalGameTime = 0;

        public static float HoursPlayedSinceLastSave { get; set; }

        private bool m_contentLoaded = false;
        private bool m_gameLoaded = false;

        public Game(string filePath = "")
        {
            // Make sure to remove reference from LocaleBuilder's text refresh list when a TextObj is disposed
            TextObj.disposeMethod = LocaleBuilder.RemoveFromTextRefreshList;

            if (filePath.Contains("-t"))
            {
                LevelEV.TESTROOM_LEVELTYPE = GameTypes.LevelType.TOWER;
                filePath = filePath.Replace("-t", "");
            }
            else if (filePath.Contains("-d"))
            {
                LevelEV.TESTROOM_LEVELTYPE = GameTypes.LevelType.DUNGEON;
                filePath = filePath.Replace("-d", "");
            }
            else if (filePath.Contains("-g"))
            {
                LevelEV.TESTROOM_LEVELTYPE = GameTypes.LevelType.GARDEN;
                filePath = filePath.Replace("-g", "");    
            }

            /* flibit didn't like this
            if (Thread.CurrentThread.CurrentCulture.Name != "en-US")
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
            }
            */
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            m_commandLineFilePath = filePath;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //this.graphics.PreferredBackBufferWidth = 1360;// GlobalEV.ScreenWidth;
            //this.graphics.PreferredBackBufferHeight = 768;//GlobalEV.ScreenHeight;

            EngineEV.ScreenWidth = GlobalEV.ScreenWidth; // Very important. Tells the engine if the game is running at a fixed resolution (which it is).
            EngineEV.ScreenHeight = GlobalEV.ScreenHeight;

            //this.graphics.IsFullScreen = true;
            this.Window.Title = "Rogue Legacy";
            ScreenManager = new RCScreenManager(this);
            m_saveGameManager = new SaveGameManager(this);

            // Set first to false and last to true for targetelapsedtime to work.
            this.IsFixedTimeStep = false; // Sets game to slow down instead of frame skip if set to false.
            this.graphics.SynchronizeWithVerticalRetrace = !LevelEV.SHOW_FPS; // Disables setting the FPS to your screen's refresh rate.
            // WARNING, if you turn off frame limiting, if the framerate goes over 1000 then the elapsed time will be too small a number for a float to carry and things will break.
            //this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f);// Sets the frame rate to 30 fps.
            this.Window.AllowUserResizing = false;
            
            if (LevelEV.ENABLE_OFFSCREEN_CONTROL == false)
                InactiveSleepTime = new TimeSpan(); // Overrides sleep time, which disables the lag when losing focus.

            m_physicsManager = new PhysicsManager();
            EquipmentSystem = new RogueCastle.EquipmentSystem();
            EquipmentSystem.InitializeEquipmentData();
            EquipmentSystem.InitializeAbilityCosts();

            //TraitSystem = new RogueCastle.TraitSystem();
            //TraitSystem.Initialize();

            GameConfig = new SettingStruct();

            GraphicsDeviceManager.PreparingDeviceSettings += ChangeGraphicsSettings;

            SleepUtil.DisableScreensaver();
        }

        protected void ChangeGraphicsSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat = DepthFormat.None;
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here.
            Tween.Initialize(7000);
            InputManager.Initialize();
            InputManager.InitializeDXManager(this.Services, this.Window);
            Buttons[] buttonList = new Buttons[]
            {
                Buttons.X,
                Buttons.A,
                Buttons.B,
                Buttons.Y,
                Buttons.LeftShoulder,
                Buttons.RightShoulder,
                Buttons.LeftTrigger,
                Buttons.RightTrigger,
                Buttons.Back,
                Buttons.Start,
                Buttons.LeftStick,
                Buttons.RightStick,
            };
            InputManager.RemapDXPad(buttonList);

            SpriteLibrary.Init();
            DialogueManager.Initialize();

            // Default to english language
            LocaleBuilder.languageType = LanguageType.English;
#if false
            // Remove the comment tags for these language documents creating a release build.
            //TxtToBinConverter.Convert("Content\\Languages\\Diary_En.txt");
            //TxtToBinConverter.Convert("Content\\Languages\\Text_En.txt");
            
            // Comment out these language documents when creating a release build.
            // Don't forget to copy paste the created bin files to your project's language folder!
            if (LevelEV.CREATE_RETAIL_VERSION == false)
            {
                DialogueManager.LoadLanguageDocument(Content, "Languages\\Text_En");
                DialogueManager.LoadLanguageDocument(Content, "Languages\\Diary_En");
            }
            else
            {
                DialogueManager.LoadLanguageBinFile("Content\\Languages\\Text_En.bin");
                DialogueManager.LoadLanguageBinFile("Content\\Languages\\Diary_En.bin");
            }
            DialogueManager.SetLanguage("English");
#endif

            m_saveGameManager.Initialize();

            m_physicsManager.Initialize(ScreenManager.Camera);
            m_physicsManager.TerminalVelocity = 2000;
            //this.IsMouseVisible = true;

            //Components.Add(ScreenManager);
            ScreenManager.Initialize(); // Necessary to manually call screenmanager initialize otherwise its LoadContent() method will be called first.
            InitializeGlobalInput();
            LoadConfig(); // Loads the config file, override language if specified in config file
            InitializeScreenConfig(); // Applies the screen config data.

            // Must be called after config file is loaded so that the correct language name array is loaded.
            InitializeMaleNameArray(false);
            InitializeFemaleNameArray(false);

            if (LevelEV.SHOW_FPS == true)
            {
                FrameRateCounter fpsCounter = new FrameRateCounter(this);
                Components.Add(fpsCounter);
                fpsCounter.Initialize();
            }

            // Code used to handle game chop.
            m_forcedGameTime1 = new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0, (int)(m_frameLimit * 1000)));
            m_forcedGameTime2 = new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0, (int)(m_frameLimit * 1050)));

            // Initializes the global input map.
            //InitializeGlobalInput();
            //LoadConfig(); // Loads the config file.
            //InitializeScreenConfig(); // Applies the screen config data.
            base.Initialize(); // Must be called before the enemylist is created so that their content is loaded.

            // Everything below this line can be disabled for release.

            if (LevelEV.CREATE_RETAIL_VERSION == false)
            {
                //Steps for adding enemies to editor.
                //1. Add a new EnemyEditorData object to enemyList with the name of the enemy class as the constructor (right below).
                //2. In the Builder class, add a case statement for the enemy string in BuildEnemy().
                //3. Press F5 to build and run, which should create the EnemyList.xml file that the map editor reads.
                List<EnemyEditorData> enemyList = new List<EnemyEditorData>();
                enemyList.Add(new EnemyEditorData(EnemyType.Skeleton));
                enemyList.Add(new EnemyEditorData(EnemyType.Knight));
                enemyList.Add(new EnemyEditorData(EnemyType.Fireball));
                enemyList.Add(new EnemyEditorData(EnemyType.Fairy));
                enemyList.Add(new EnemyEditorData(EnemyType.Turret));
                enemyList.Add(new EnemyEditorData(EnemyType.Ninja));
                enemyList.Add(new EnemyEditorData(EnemyType.Horse));
                enemyList.Add(new EnemyEditorData(EnemyType.Zombie));
                enemyList.Add(new EnemyEditorData(EnemyType.Wolf));
                enemyList.Add(new EnemyEditorData(EnemyType.BallAndChain));
                enemyList.Add(new EnemyEditorData(EnemyType.Eyeball));
                enemyList.Add(new EnemyEditorData(EnemyType.Blob));
                enemyList.Add(new EnemyEditorData(EnemyType.SwordKnight));
                enemyList.Add(new EnemyEditorData(EnemyType.Eagle));
                enemyList.Add(new EnemyEditorData(EnemyType.ShieldKnight));
                enemyList.Add(new EnemyEditorData(EnemyType.FireWizard));
                enemyList.Add(new EnemyEditorData(EnemyType.IceWizard));
                enemyList.Add(new EnemyEditorData(EnemyType.EarthWizard));
                enemyList.Add(new EnemyEditorData(EnemyType.BouncySpike));
                enemyList.Add(new EnemyEditorData(EnemyType.SpikeTrap));
                enemyList.Add(new EnemyEditorData(EnemyType.Plant));
                enemyList.Add(new EnemyEditorData(EnemyType.Energon));
                enemyList.Add(new EnemyEditorData(EnemyType.Spark));
                enemyList.Add(new EnemyEditorData(EnemyType.SkeletonArcher));
                enemyList.Add(new EnemyEditorData(EnemyType.Chicken));
                enemyList.Add(new EnemyEditorData(EnemyType.Platform));
                enemyList.Add(new EnemyEditorData(EnemyType.HomingTurret));
                enemyList.Add(new EnemyEditorData(EnemyType.LastBoss));
                enemyList.Add(new EnemyEditorData(EnemyType.Dummy));
                enemyList.Add(new EnemyEditorData(EnemyType.Starburst));
                enemyList.Add(new EnemyEditorData(EnemyType.Portrait));
                enemyList.Add(new EnemyEditorData(EnemyType.Mimic));

                // Take this out when building release version.
                XMLCompiler.CompileEnemies(enemyList, Directory.GetCurrentDirectory());
            }
        }

        public static void InitializeGlobalInput()
        {
            if (GlobalInput != null)
                GlobalInput.ClearAll();
            else
                GlobalInput = new InputMap(PlayerIndex.One, true);

            //////////// KEYBOARD INPUT MAP
            GlobalInput.AddInput(InputMapType.MENU_CONFIRM1, Keys.Enter);
            GlobalInput.AddInput(InputMapType.MENU_CANCEL1, Keys.Escape);
            GlobalInput.AddInput(InputMapType.MENU_CREDITS, Keys.LeftControl);
            GlobalInput.AddInput(InputMapType.MENU_OPTIONS, Keys.Tab);
            GlobalInput.AddInput(InputMapType.MENU_PROFILECARD, Keys.LeftShift);
            GlobalInput.AddInput(InputMapType.MENU_ROGUEMODE, Keys.Back);
            GlobalInput.AddInput(InputMapType.MENU_PAUSE, Keys.Escape);
            GlobalInput.AddInput(InputMapType.MENU_MAP, Keys.Tab);

            GlobalInput.AddInput(InputMapType.PLAYER_JUMP1, Keys.S);
            GlobalInput.AddInput(InputMapType.PLAYER_JUMP2, Keys.Space);
            GlobalInput.AddInput(InputMapType.PLAYER_SPELL1, Keys.W);
            GlobalInput.AddInput(InputMapType.PLAYER_ATTACK, Keys.D);
            GlobalInput.AddInput(InputMapType.PLAYER_BLOCK, Keys.A);
            GlobalInput.AddInput(InputMapType.PLAYER_DASHLEFT, Keys.Q);
            GlobalInput.AddInput(InputMapType.PLAYER_DASHRIGHT, Keys.E);
            GlobalInput.AddInput(InputMapType.PLAYER_UP1, Keys.I);
            GlobalInput.AddInput(InputMapType.PLAYER_UP2, Keys.Up);
            GlobalInput.AddInput(InputMapType.PLAYER_DOWN1, Keys.K);
            GlobalInput.AddInput(InputMapType.PLAYER_DOWN2, Keys.Down);
            GlobalInput.AddInput(InputMapType.PLAYER_LEFT1, Keys.J);
            GlobalInput.AddInput(InputMapType.PLAYER_LEFT2, Keys.Left);
            GlobalInput.AddInput(InputMapType.PLAYER_RIGHT1, Keys.L);
            GlobalInput.AddInput(InputMapType.PLAYER_RIGHT2, Keys.Right);

            //////////// GAMEPAD INPUT MAP

            GlobalInput.AddInput(InputMapType.MENU_CONFIRM1, Buttons.A);
            GlobalInput.AddInput(InputMapType.MENU_CONFIRM2, Buttons.Start);
            GlobalInput.AddInput(InputMapType.MENU_CANCEL1, Buttons.B);
            GlobalInput.AddInput(InputMapType.MENU_CANCEL2, Buttons.Back);
            GlobalInput.AddInput(InputMapType.MENU_CREDITS, Buttons.RightTrigger);
            GlobalInput.AddInput(InputMapType.MENU_OPTIONS, Buttons.Y);
            GlobalInput.AddInput(InputMapType.MENU_PROFILECARD, Buttons.X);
            GlobalInput.AddInput(InputMapType.MENU_ROGUEMODE, Buttons.Back);
            GlobalInput.AddInput(InputMapType.MENU_PAUSE, Buttons.Start);
            GlobalInput.AddInput(InputMapType.MENU_MAP, Buttons.Back);

            GlobalInput.AddInput(InputMapType.PLAYER_JUMP1, Buttons.A);
            GlobalInput.AddInput(InputMapType.PLAYER_ATTACK, Buttons.X);
            GlobalInput.AddInput(InputMapType.PLAYER_BLOCK, Buttons.Y);
            GlobalInput.AddInput(InputMapType.PLAYER_DASHLEFT, Buttons.LeftTrigger);
            GlobalInput.AddInput(InputMapType.PLAYER_DASHRIGHT, Buttons.RightTrigger);
            GlobalInput.AddInput(InputMapType.PLAYER_UP1, Buttons.DPadUp);
            //GlobalInput.AddInput(InputMapType.PLAYER_UP2, Buttons.LeftThumbstickUp);
            GlobalInput.AddInput(InputMapType.PLAYER_UP2, ThumbStick.LeftStick, -90, 30);
            GlobalInput.AddInput(InputMapType.PLAYER_DOWN1, Buttons.DPadDown);
            GlobalInput.AddInput(InputMapType.PLAYER_DOWN2, ThumbStick.LeftStick, 90, 37);
            GlobalInput.AddInput(InputMapType.PLAYER_LEFT1, Buttons.DPadLeft);
            GlobalInput.AddInput(InputMapType.PLAYER_LEFT2, Buttons.LeftThumbstickLeft);
            GlobalInput.AddInput(InputMapType.PLAYER_RIGHT1, Buttons.DPadRight);
            GlobalInput.AddInput(InputMapType.PLAYER_RIGHT2, Buttons.LeftThumbstickRight);
            GlobalInput.AddInput(InputMapType.PLAYER_SPELL1, Buttons.B);

            GlobalInput.AddInput(InputMapType.MENU_PROFILESELECT, Keys.Escape);
            GlobalInput.AddInput(InputMapType.MENU_PROFILESELECT, Buttons.Back);
            GlobalInput.AddInput(InputMapType.MENU_DELETEPROFILE, Keys.Back);
            GlobalInput.AddInput(InputMapType.MENU_DELETEPROFILE, Buttons.Y);

            // Adding mouse confirm/cancel controls
            GlobalInput.AddInput(InputMapType.MENU_CONFIRM3, Keys.F13);
            GlobalInput.AddInput(InputMapType.MENU_CANCEL3, Keys.F14);

            // Special code so that player attack acts as the second confirm and player jump acts as the second cancel.
            GlobalInput.KeyList[InputMapType.MENU_CONFIRM2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_ATTACK];
            GlobalInput.KeyList[InputMapType.MENU_CANCEL2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP1];
        }

        private void InitializeDefaultConfig()
        {
            GameConfig.FullScreen = false;
            GameConfig.ScreenWidth = 1360;
            GameConfig.ScreenHeight = 768;
            GameConfig.MusicVolume = 1;
            GameConfig.SFXVolume = 0.8f;
            GameConfig.EnableDirectInput = true;
            InputManager.Deadzone = 10;
            GameConfig.ProfileSlot = 1;
            GameConfig.EnableSteamCloud = false;
            GameConfig.ReduceQuality = false;

            InitializeGlobalInput();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if (m_contentLoaded == false)
            {
                m_contentLoaded = true;
                LoadAllSpriteFonts();
                LoadAllEffects();
                LoadAllSpritesheets();

                // Initializing Sound Manager.
                SoundManager.Initialize("Content\\Audio\\RogueCastleXACTProj.xgs");
                SoundManager.LoadWaveBank("Content\\Audio\\SFXWaveBank.xwb");
                SoundManager.LoadWaveBank("Content\\Audio\\MusicWaveBank.xwb", true);
                SoundManager.LoadSoundBank("Content\\Audio\\SFXSoundBank.xsb");
                SoundManager.LoadSoundBank("Content\\Audio\\MusicSoundBank.xsb", true);
                SoundManager.GlobalMusicVolume = Game.GameConfig.MusicVolume;
                SoundManager.GlobalSFXVolume = Game.GameConfig.SFXVolume;

                if (InputManager.GamePadIsConnected(PlayerIndex.One))
                    InputManager.SetPadType(PlayerIndex.One, PadTypes.GamePad);

                // Creating a generic texture for use.
                Game.GenericTexture = new Texture2D(GraphicsDevice, 1, 1);
                Game.GenericTexture.SetData(new Color[] { Color.White });

                // This causes massive slowdown on load.
                if (LevelEV.LOAD_SPLASH_SCREEN == false)
                {
                    LevelBuilder2.Initialize();
                    LevelParser.ParseRooms("Map_1x1", this.Content);
                    LevelParser.ParseRooms("Map_1x2", this.Content);
                    LevelParser.ParseRooms("Map_1x3", this.Content);
                    LevelParser.ParseRooms("Map_2x1", this.Content);
                    LevelParser.ParseRooms("Map_2x2", this.Content);
                    LevelParser.ParseRooms("Map_2x3", this.Content);
                    LevelParser.ParseRooms("Map_3x1", this.Content);
                    LevelParser.ParseRooms("Map_3x2", this.Content);
                    LevelParser.ParseRooms("Map_Special", this.Content);
                    LevelParser.ParseRooms("Map_DLC1", this.Content, true);
                    LevelBuilder2.IndexRoomList();
                }

                SkillSystem.Initialize(); // Must be initialized after the sprites are loaded because the MiscSpritesheet is needed.

                AreaStruct CastleZone = new AreaStruct()
                {
                    Name = "The Grand Entrance",
                    LevelType = GameTypes.LevelType.CASTLE,
                    TotalRooms = new Vector2(24, 28),//(17,19),//(20, 22),//(25,35),//(20,25),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(1, 3),//(2, 3),
                    BonusRooms = new Vector2(2, 3),
                    Color = Color.White,
                };

                AreaStruct GardenZone = new AreaStruct()
                {
                    LevelType = GameTypes.LevelType.GARDEN,
                    TotalRooms = new Vector2(23, 27),//(25,29),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(1, 3),
                    BonusRooms = new Vector2(2, 3),
                    Color = Color.Green,
                };

                AreaStruct TowerZone = new AreaStruct()
                {
                    LevelType = GameTypes.LevelType.TOWER,
                    TotalRooms = new Vector2(23, 27),//(27,31),//(25,29),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(1, 3),
                    BonusRooms = new Vector2(2, 3),
                    Color = Color.DarkBlue,
                };

                AreaStruct DungeonZone = new AreaStruct()
                {
                    LevelType = GameTypes.LevelType.DUNGEON,
                    TotalRooms = new Vector2(23, 27),//(29,33),//(25, 29),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(1, 3),
                    BonusRooms = new Vector2(2, 3),
                    Color = Color.Red,
                };

                #region Demo Levels

                AreaStruct CastleZoneDemo = new AreaStruct()
                {
                    Name = "The Grand Entrance",
                    LevelType = GameTypes.LevelType.CASTLE,
                    TotalRooms = new Vector2(24, 27),//(25,35),//(20,25),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(2, 3),
                    BonusRooms = new Vector2(2, 3),
                    Color = Color.White,
                };

                AreaStruct GardenZoneDemo = new AreaStruct()
                {
                    Name = "The Grand Entrance",
                    LevelType = GameTypes.LevelType.GARDEN,
                    TotalRooms = new Vector2(12, 14),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(2, 3),
                    BonusRooms = new Vector2(1, 2),
                    Color = Color.Green,
                };

                AreaStruct DungeonZoneDemo = new AreaStruct()
                {
                    Name = "The Grand Entrance",
                    LevelType = GameTypes.LevelType.DUNGEON,
                    TotalRooms = new Vector2(12, 14),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(2, 3),
                    BonusRooms = new Vector2(1, 2),
                    Color = Color.Red,
                };

                AreaStruct TowerZoneDemo = new AreaStruct()
                {
                    Name = "The Grand Entrance",
                    LevelType = GameTypes.LevelType.TOWER,
                    TotalRooms = new Vector2(12, 14),//(25, 35),//(15, 25),
                    BossInArea = true,
                    SecretRooms = new Vector2(2, 3),
                    BonusRooms = new Vector2(1, 2),
                    Color = Color.DarkBlue,
                };

                #endregion

                Area1List = new AreaStruct[] { CastleZone, GardenZone, TowerZone, DungeonZone }; //DUNGEON IS LAST AREA

                if (LevelEV.RUN_DEMO_VERSION == true)
                    Area1List = new AreaStruct[] { CastleZoneDemo }; //DUNGEON IS LAST AREA
                //Area1List = new AreaStruct[] { CastleZoneDemo, GardenZoneDemo, TowerZoneDemo, DungeonZoneDemo }; //DUNGEON IS LAST AREA

            }

            //ScreenManager.LoadContent(); // What is this doing here?
        }

        public void LoadAllSpriteFonts()
        {
            SpriteFontArray.SpriteFontList.Clear();
            PixelArtFont = Content.Load<SpriteFont>("Fonts\\Arial12");
            SpriteFontArray.SpriteFontList.Add(PixelArtFont);
            PixelArtFontBold = Content.Load<SpriteFont>("Fonts\\PixelArtFontBold");
            SpriteFontArray.SpriteFontList.Add(PixelArtFontBold);
            EnemyLevelFont = Content.Load<SpriteFont>("Fonts\\EnemyLevelFont");
            SpriteFontArray.SpriteFontList.Add(EnemyLevelFont);
            EnemyLevelFont.Spacing = -5;
            PlayerLevelFont = Content.Load<SpriteFont>("Fonts\\PlayerLevelFont");
            SpriteFontArray.SpriteFontList.Add(PlayerLevelFont);
            PlayerLevelFont.Spacing = -7;
            GoldFont = Content.Load<SpriteFont>("Fonts\\GoldFont");
            SpriteFontArray.SpriteFontList.Add(GoldFont);
            GoldFont.Spacing = -5;
            JunicodeFont = Content.Load<SpriteFont>("Fonts\\Junicode");
            SpriteFontArray.SpriteFontList.Add(JunicodeFont);
            //JunicodeFont.Spacing = -1;
            JunicodeLargeFont = Content.Load<SpriteFont>("Fonts\\JunicodeLarge");
            SpriteFontArray.SpriteFontList.Add(JunicodeLargeFont);
            JunicodeLargeFont.Spacing = -1;
            HerzogFont = Content.Load<SpriteFont>("Fonts\\HerzogVonGraf24");
            SpriteFontArray.SpriteFontList.Add(HerzogFont);
            CinzelFont = Content.Load<SpriteFont>("Fonts\\CinzelFont");
            SpriteFontArray.SpriteFontList.Add(CinzelFont);
            BitFont = Content.Load<SpriteFont>("Fonts\\BitFont");
            SpriteFontArray.SpriteFontList.Add(BitFont);
            NotoSansSCFont = Content.Load<SpriteFont>("Fonts\\NotoSansSC");
            SpriteFontArray.SpriteFontList.Add(NotoSansSCFont);
            RobotoSlabFont = Content.Load<SpriteFont>("Fonts\\RobotoSlab");
            SpriteFontArray.SpriteFontList.Add(RobotoSlabFont);
        }

        public void LoadAllSpritesheets()
        {
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\blacksmithUISpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\enemyFinal2Spritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\enemyFinalSpritesheetBig", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\miscSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\traitsCastleSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\castleTerrainSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\playerSpritesheetBig", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\titleScreen3Spritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\mapSpritesheetBig", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\startingRoomSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\towerTerrainSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\dungeonTerrainSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\profileCardSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\portraitSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\gardenTerrainSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\parallaxBGSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\getItemScreenSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\neoTerrainSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\languageSpritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\language2Spritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\language3Spritesheet", false);
            SpriteLibrary.LoadSpritesheet(Content, "GameSpritesheets\\blitworksSpritesheet", false);
        }

        public void LoadAllEffects()
        {
            // Necessary stuff to create a 2D shader.
            MaskEffect = Content.Load<Effect>("Shaders\\AlphaMaskShader");

            ShadowEffect = Content.Load<Effect>("Shaders\\ShadowFX");
            ParallaxEffect = Content.Load<Effect>("Shaders\\ParallaxFX");
            HSVEffect = Content.Load<Effect>("Shaders\\HSVShader");
            InvertShader = Content.Load<Effect>("Shaders\\InvertShader");
            ColourSwapShader = Content.Load<Effect>("Shaders\\ColourSwapShader");
            RippleEffect = Content.Load<Effect>("Shaders\\Shockwave");

            RippleEffect.Parameters["mag"].SetValue(2);

            GaussianBlur = new RogueCastle.GaussianBlur(this, 1320, 720);
            GaussianBlur.Amount = 2f;
            GaussianBlur.Radius = 7;
            GaussianBlur.ComputeKernel();
            GaussianBlur.ComputeOffsets();
            Game.GaussianBlur.InvertMask = true;

            // Necessary stuff to create Black/White mask shader.
            BWMaskEffect = Content.Load<Effect>("Shaders\\BWMaskShader");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        WeakReference gcTracker = new WeakReference(new object());
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        public int graphicsToggle = 0;

        protected override void Update(GameTime gameTime)
        {
            //if (InputManager.JustPressed(Keys.B, null))
            //{
            //    //Game.PlayerStats.TimesCastleBeaten = 0;
            //    //Game.PlayerStats.LastbossBeaten = false;
            //    //Game.PlayerStats.EyeballBossBeaten = true;
            //    //Game.PlayerStats.FairyBossBeaten = true;
            //    //Game.PlayerStats.FireballBossBeaten = true;
            //    //Game.PlayerStats.BlobBossBeaten = true;

            //    //Game.PlayerStats.GetBlueprintArray[0][EquipmentBaseType.Bronze] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[1][EquipmentBaseType.Bronze] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[2][EquipmentBaseType.Bronze] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[3][EquipmentBaseType.Bronze] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[4][EquipmentBaseType.Bronze] = EquipmentState.Purchased;

            //    //Game.PlayerStats.GetBlueprintArray[0][EquipmentBaseType.Knight] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[1][EquipmentBaseType.Knight] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[2][EquipmentBaseType.Knight] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[3][EquipmentBaseType.Knight] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[4][EquipmentBaseType.Knight] = EquipmentState.Purchased;

            //    //Game.PlayerStats.GetBlueprintArray[0][EquipmentBaseType.Blood] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[1][EquipmentBaseType.Blood] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[2][EquipmentBaseType.Blood] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[3][EquipmentBaseType.Blood] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetBlueprintArray[4][EquipmentBaseType.Blood] = EquipmentState.Purchased;

            //    //Game.PlayerStats.GetRuneArray[0][EquipmentAbilityType.DoubleJump] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[1][EquipmentAbilityType.DoubleJump] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[1][EquipmentAbilityType.Flight] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[0][EquipmentAbilityType.ManaHPGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[1][EquipmentAbilityType.ManaHPGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[2][EquipmentAbilityType.ManaHPGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[0][EquipmentAbilityType.Dash] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[0][EquipmentAbilityType.DamageReturn] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[1][EquipmentAbilityType.DamageReturn] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[3][EquipmentAbilityType.ManaGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[3][EquipmentAbilityType.Vampirism] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[2][EquipmentAbilityType.Vampirism] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[2][EquipmentAbilityType.ManaGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[3][EquipmentAbilityType.GoldGain] = EquipmentState.Purchased;
            //    //Game.PlayerStats.GetRuneArray[4][EquipmentAbilityType.GoldGain] = EquipmentState.Purchased;
            //    //ScreenManager.Player.Visible = false;
            //    //ScreenManager.Player.Opacity = 0;
            //    ScreenManager.GetLevelScreen().SetPlayerHUDVisibility(false);
            //    ScreenManager.GetLevelScreen().SetMapDisplayVisibility(false);
            //}

            if (m_gameLoaded == false)
            {
                m_gameLoaded = true;
                if (LevelEV.DELETE_SAVEFILE == true)
                {
                    SaveManager.ClearAllFileTypes(true);
                    SaveManager.ClearAllFileTypes(false);
                }

#if false
                SkillSystem.ResetAllTraits();
                Game.PlayerStats.Dispose();
                Game.PlayerStats = new PlayerStats();
                //(ScreenManager as RCScreenManager).Player.Reset();
                Game.GameConfig.ProfileSlot = (byte)1;
                (ScreenManager.Game as Game).SaveManager.LoadFiles(null, SaveType.PlayerData, SaveType.Lineage, SaveType.UpgradeData);
                // Special circumstance where you should override player's current HP/MP
                //Game.ScreenManager.Player.CurrentHealth = Game.PlayerStats.CurrentHealth;
                //Game.ScreenManager.Player.CurrentMana = Game.PlayerStats.CurrentMana;

                ScreenManager.LoadContent();

                // Ending screen, doesn't work when loaded this way
                //ScreenManager.DisplayScreen(ScreenType.Ending, true, null);

                // GetItemScreen
                /*
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(100,100));
                
                objectList.Add(GetItemType.Rune);
                objectList.Add(new Vector2(EquipmentCategoryType.Helm, EquipmentAbilityType.ManaHPGain)); // (EquipmentCategoryType, EquipmentAbilityType)
                
                //objectList.Add(GetItemType.SpecialItem);
                //objectList.Add(new Vector2(SpecialItemType.BlobToken, 0));
                
                //objectList.Add(GetItemType.StatDrop);
                //objectList.Add(new Vector2(ItemDropType.Stat_MaxHealth, 0));

                //objectList.Add(GetItemType.TripStatDrop);
                //objectList.Add(new Vector2(ItemDropType.Stat_MaxHealth, 0));
                //objectList.Add(new Vector2(ItemDropType.Stat_MaxMana, ItemDropType.Stat_Magic));

                ScreenManager.DisplayScreen(ScreenType.GetItem, true, objectList);
                 */

                // Enchantress (equipment)
                //ScreenManager.DisplayScreen(ScreenType.Enchantress, true, null);

                // Blacksmith
                //ScreenManager.DisplayScreen(ScreenType.Blacksmith, true, null);

                // Skill screen, manor
                //ScreenManager.DisplayScreen(ScreenType.Skill, true, null);

                // Profile select
                //ScreenManager.DisplayScreen(ScreenType.ProfileSelect, true, null);

                // Profile card
                //ScreenManager.DisplayScreen(ScreenType.ProfileCard, true, null);

                // Lineage screen
                //ScreenManager.DisplayScreen(ScreenType.Lineage, true, null);

                // Starting room, i.e. castle entrance with training dummy
                //ScreenManager.DisplayScreen(ScreenType.StartingRoom, true, null);

                // Tutorial, make sure LevelEV.RUN_TESTROOM is false
                //ScreenManager.DisplayScreen(ScreenType.TutorialRoom, true, null);

                // Dialog choice
                /*
                ScreenManager.DialogueScreen.SetDialogue("CarnivalRoom1-Start"); //"Meet Architect", "LineageChoiceWarning"
                ScreenManager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                //ScreenManager.DialogueScreen.SetConfirmEndHandler(this, "StartGame");
                //ScreenManager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                ScreenManager.DisplayScreen(ScreenType.Dialogue, true);
                 */

                // Dialog screen
                /*
                ScreenManager.DialogueScreen.SetDialogue("DiaryEntry10"); // "FinalBossTalk03"
                //ScreenManager.DialogueScreen.SetConfirmEndHandler(this, "Part4");
                ScreenManager.DisplayScreen(ScreenType.Dialogue, true);
                 */

                // Options screen
                /*
                List<object> optionsData = new List<object>();
                optionsData.Add(false);
                ScreenManager.DisplayScreen(ScreenType.Options, false, optionsData);
                 */

                // Title screen
                //ScreenManager.DisplayScreen(ScreenType.Title, true, null);
#else
                if (LevelEV.LOAD_SPLASH_SCREEN == true)
                {
                    if (LevelEV.RUN_DEMO_VERSION == true)
                        ScreenManager.DisplayScreen(ScreenType.DemoStart, true, null);
                    else
                        ScreenManager.DisplayScreen(ScreenType.CDGSplash, true, null);
                }
                else
                {
                    if (LevelEV.LOAD_TITLE_SCREEN == false)
                    {
                        if (LevelEV.RUN_TESTROOM == true)
                            ScreenManager.DisplayScreen(ScreenType.Level, true, null);
                        else
                        {
                            if (LevelEV.RUN_TUTORIAL == true)
                                ScreenManager.DisplayScreen(ScreenType.TutorialRoom, true, null);
                            else
                                //ScreenManager.DisplayScreen(ScreenType.Lineage, true, null); // Just for testing lineages.
                                ScreenManager.DisplayScreen(ScreenType.StartingRoom, true, null);
                            //ScreenManager.DisplayScreen(ScreenType.Ending, true, null);
                            //ScreenManager.DisplayScreen(ScreenType.Credits, true, null);
                        }
                    }
                    else
                    {
                        //if (SaveManager.FileExists(SaveType.PlayerData))
                        //    this.SaveManager.LoadFiles(null, SaveType.PlayerData);

                        //if (Game.PlayerStats.TutorialComplete == false)
                        //    ScreenManager.DisplayScreen(ScreenType.TutorialRoom, true, null);
                        //else
                        ScreenManager.DisplayScreen(ScreenType.Title, true, null);
                    }
                }
#endif
            }

            // This code forces the game to slow down (instead of chop) if it drops below the frame limit.
            TotalGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            GameTime gameTimeToUse = gameTime;
            if (gameTime.ElapsedGameTime.TotalSeconds > m_frameLimit)
            {
                if (m_frameLimitSwap == false)
                {
                    m_frameLimitSwap = true;
                    gameTimeToUse = m_forcedGameTime1;
                }
                else
                {
                    m_frameLimitSwap = false;
                    gameTimeToUse = m_forcedGameTime2;
                }
            }

            //if (!gcTracker.IsAlive)
            //{
            //    Console.WriteLine("A garbage collection occurred!");
            //    gcTracker = new WeakReference(new object());
            //}

            // The screenmanager is updated via the Components.Add call in the game constructor. It is called after this Update() call.
            SoundManager.Update(gameTimeToUse);
            if ((m_previouslyActiveCounter <= 0 && IsActive == true) || LevelEV.ENABLE_OFFSCREEN_CONTROL == true) // Only accept input if you have screen focus.
                InputManager.Update(gameTimeToUse);
            if (LevelEV.ENABLE_DEBUG_INPUT == true)
                HandleDebugInput();
            Tween.Update(gameTimeToUse);
            ScreenManager.Update(gameTimeToUse);
            SoundManager.Update3DSounds(); // Special method to handle 3D sound overrides. Must go after enemy update.
            base.Update(gameTime);

            if (IsActive == false)
                m_previouslyActiveCounter = 0.25f;

            // Prevents mouse from accidentally leaving game while active.
            //if (IsActive == true)
            //    Mouse.SetPosition((int)(GlobalEV.ScreenWidth / 2f), (int)(GlobalEV.ScreenHeight / 2f));

            if (m_previouslyActiveCounter > 0)
                m_previouslyActiveCounter -= 0.016f;
        }

        private float m_previouslyActiveCounter = 0; // This makes sure your very first inputs upon returning after leaving the screen does not register (no accidental inputs happen).

        private void HandleDebugInput()
        {
            int languageType = (int)LocaleBuilder.languageType;

            if (InputManager.JustPressed(Keys.OemQuotes, null))
            {
                languageType++;
                if (languageType >= (int)LanguageType.MAX)
                    languageType = 0;

                LocaleBuilder.languageType = (LanguageType)languageType;
                LocaleBuilder.RefreshAllText();
                Console.WriteLine("Changing to language type: " + (LanguageType)languageType);
            }
            else if (InputManager.JustPressed(Keys.OemSemicolon, null))
            {
                languageType--;
                if (languageType < 0)
                    languageType = (int)LanguageType.MAX - 1;

                LocaleBuilder.languageType = (LanguageType)languageType;
                LocaleBuilder.RefreshAllText();
                Console.WriteLine("Changing to language type: " + (LanguageType)languageType);
            }

            if (InputManager.JustPressed(Keys.OemPipe, null))
            {
                Game.PlayerStats.ForceLanguageGender++;
                if (Game.PlayerStats.ForceLanguageGender > 2)
                    Game.PlayerStats.ForceLanguageGender = 0;
                LocaleBuilder.RefreshAllText();
            }

            if (InputManager.JustPressed(Keys.Z, null))
            {
                graphicsToggle++;
                if (graphicsToggle > 1) graphicsToggle = 0;

                switch (graphicsToggle)
                {
                    case (0):
                        ScreenManager.GetLevelScreen().SetPlayerHUDVisibility(true);
                        ScreenManager.GetLevelScreen().SetMapDisplayVisibility(true);
                        break;
                    case (1):
                        ScreenManager.GetLevelScreen().SetPlayerHUDVisibility(false);
                        ScreenManager.GetLevelScreen().SetMapDisplayVisibility(false);
                        break;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // The screenmanager is drawn via the Components.Add call in the game constructor. It is called after this Draw() call.
            ScreenManager.Draw(gameTime);
            base.Draw(gameTime);
        }

        private bool m_maleChineseNamesLoaded = false;
        public void InitializeMaleNameArray(bool forceCreate)
        {
            // The name list needs to be reloaded every time the language is from Chinese to another language or vice versa.
            if ((m_maleChineseNamesLoaded == false && LocaleBuilder.languageType == LanguageType.Chinese_Simp) ||
                (m_maleChineseNamesLoaded == true && LocaleBuilder.languageType != LanguageType.Chinese_Simp))
                forceCreate = true;

            if (NameArray != null && NameArray.Count > 0 && forceCreate == false)
                return;

            if (NameArray != null)
                NameArray.Clear();
            else
                NameArray = new List<string>();

            // Logographic fonts cannot make use of the name change system, otherwise the bitmap spritesheet
            // generated by the system would have to include every single glyph.
            if (LocaleBuilder.languageType != LanguageType.Chinese_Simp)
            {
                m_maleChineseNamesLoaded = false;

                using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(Path.Combine(Content.RootDirectory, "HeroNames.txt"))))
                {
                    // A test to make sure no special characters are used in the game.
                    SpriteFont junicode = Content.Load<SpriteFont>("Fonts\\Junicode");
                    SpriteFontArray.SpriteFontList.Add(junicode);
                    TextObj specialCharTest = new TextObj(junicode);

                    while (!sr.EndOfStream)
                    {
                        string name = sr.ReadLine();
                        bool hasSpecialChar = false;

                        try
                        {
                            specialCharTest.Text = name;
                        }
                        catch
                        {
                            hasSpecialChar = true;
                        }

                        if (!name.Contains("//") && hasSpecialChar == false)
                            NameArray.Add(name);
                    }

                    specialCharTest.Dispose();
                    SpriteFontArray.SpriteFontList.Remove(junicode);
                }
            }
            else
            {
                m_maleChineseNamesLoaded = true;

                // List of male Chinese names.
                NameArray.Add("马如龙");
                NameArray.Add("常遇春");
                NameArray.Add("胡不归");
                NameArray.Add("何千山");
                NameArray.Add("方日中");
                NameArray.Add("谢南山");
                NameArray.Add("慕江南");
                NameArray.Add("赵寒江");
                NameArray.Add("宋乔木");
                NameArray.Add("应楚山");
                NameArray.Add("江山月");
                NameArray.Add("赵慕寒");
                NameArray.Add("万重山");
                NameArray.Add("郭百鸣");
                NameArray.Add("谢武夫");
                NameArray.Add("关中林");
                NameArray.Add("吴深山");
                NameArray.Add("向春风");
                NameArray.Add("牛始旦");
                NameArray.Add("卫东方");
                NameArray.Add("萧北辰");
                NameArray.Add("黃鹤年");
                NameArray.Add("王石柱");
                NameArray.Add("胡江林");
                NameArray.Add("周宇浩");
                NameArray.Add("程向阳");
                NameArray.Add("魏海风");
                NameArray.Add("龚剑辉");
                NameArray.Add("周宇浩");
                NameArray.Add("何汝平");
            }

            // Ensures the name array is greater than 0.
            if (NameArray.Count < 1)
            {
                NameArray.Add("Lee");
                NameArray.Add("Charles");
                NameArray.Add("Lancelot");
            }
        }

        private bool m_femaleChineseNamesLoaded = false;
        public void InitializeFemaleNameArray(bool forceCreate)
        {
            // The name list needs to be reloaded every time the language is from Chinese to another language or vice versa.
            if ((m_femaleChineseNamesLoaded == false && LocaleBuilder.languageType == LanguageType.Chinese_Simp) ||
                (m_femaleChineseNamesLoaded == true && LocaleBuilder.languageType != LanguageType.Chinese_Simp))
                forceCreate = true;

            if (FemaleNameArray != null && FemaleNameArray.Count > 0 && forceCreate == false)
                return;

            if (FemaleNameArray != null)
                FemaleNameArray.Clear();
            else
                FemaleNameArray = new List<string>();

            // Logographic fonts cannot make use of the name change system, otherwise the bitmap spritesheet
            // generated by the system would have to include every single glyph.
            if (LocaleBuilder.languageType != LanguageType.Chinese_Simp)
            {
                m_femaleChineseNamesLoaded = false;

                using (StreamReader sr = new StreamReader(TitleContainer.OpenStream(Path.Combine(Content.RootDirectory, "HeroineNames.txt"))))
                {
                    // A test to make sure no special characters are used in the game.
                    SpriteFont junicode = Content.Load<SpriteFont>("Fonts\\Junicode");
                    SpriteFontArray.SpriteFontList.Add(junicode);
                    TextObj specialCharTest = new TextObj(junicode);

                    while (!sr.EndOfStream)
                    {
                        string name = sr.ReadLine();
                        bool hasSpecialChar = false;

                        try
                        {
                            specialCharTest.Text = name;
                        }
                        catch
                        {
                            hasSpecialChar = true;
                        }

                        if (!name.Contains("//") && hasSpecialChar == false)
                            FemaleNameArray.Add(name);
                    }

                    specialCharTest.Dispose();
                    SpriteFontArray.SpriteFontList.Remove(junicode);
                }
            }
            else
            {
                m_femaleChineseNamesLoaded = true;

                // List of female Chinese names.
                FemaleNameArray.Add("水一方");
                FemaleNameArray.Add("刘妙音");
                FemaleNameArray.Add("郭釆薇");
                FemaleNameArray.Add("颜如玉");
                FemaleNameArray.Add("陈巧雅");
                FemaleNameArray.Add("萧玉旋");
                FemaleNameArray.Add("花可秀");
                FemaleNameArray.Add("董小婉");
                FemaleNameArray.Add("李诗诗");
                FemaleNameArray.Add("唐秋香");
                FemaleNameArray.Add("方美人");
                FemaleNameArray.Add("金喜儿");
                FemaleNameArray.Add("达莉萍");
                FemaleNameArray.Add("蔡靜语");
                FemaleNameArray.Add("郭玲玲");
                FemaleNameArray.Add("黃晓莺");
                FemaleNameArray.Add("杜秋娘");
                FemaleNameArray.Add("高媛媛");
                FemaleNameArray.Add("林靜妤");
                FemaleNameArray.Add("凤雨婷");
                FemaleNameArray.Add("徐瑶瑶");
                FemaleNameArray.Add("祝台英");
                FemaleNameArray.Add("郭燕秋");
                FemaleNameArray.Add("江小满");
                FemaleNameArray.Add("项月芳");
                FemaleNameArray.Add("郑云云");
                FemaleNameArray.Add("王琼琼");
                FemaleNameArray.Add("李瓶儿");
                FemaleNameArray.Add("周楚红");
                FemaleNameArray.Add("叶秋菊");
            }

            // Ensures the female name array is greater than 0.
            if (FemaleNameArray.Count < 1)
            {
                FemaleNameArray.Add("Jenny");
                FemaleNameArray.Add("Shanoa");
                FemaleNameArray.Add("Chun Li");
            }
        }

        public static void ConvertPlayerNameFormat(ref string playerName, ref string romanNumeral)
        {
            if (playerName.Length < 3)
                return;

            // Remove the Sir or Lady title in the player name.
            if (playerName.Substring(0, 3) == "Sir")
            {
                if (playerName.Length > 3)
                    playerName = playerName.Substring(4); // Removing "Sir ".
            }
            else if (playerName.Length > 3 && playerName.Substring(0, 4) == "Lady")
            {
                if (playerName.Length > 4)
                    playerName = playerName.Substring(5); // Removing "Lady ".
            }

            // Remove the roman numerals in the player name.
            int romanNumeralIndex = playerName.Trim().LastIndexOf(" ");
            if (romanNumeralIndex > 0)
            {
                string romanNumeralString = playerName.Substring(romanNumeralIndex + 1);
                // Can't check them all, so only fix the first 40 name duplicates.
                string[] romanNumeralCheckArray = new string[] { 
                        "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX",
                        "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX", "XXXI", "XXXII", "XXXIII", "XXXIV", "XXXV", "XXXVI", 
                        "XXXVII", "XXXVIII", "XXXIX", "XXXX",
                    };
                for (int i = 0; i < romanNumeralCheckArray.Length; i++)
                {
                    if (romanNumeralString == romanNumeralCheckArray[i])
                    {
                        playerName = playerName.Substring(0, playerName.Length - romanNumeralString.Length).Trim();
                        romanNumeral = romanNumeralString.Trim();
                        break;
                    }
                }
            }
        }

        public static string NameHelper(string playerName, string romanNumerals, bool isFemale, bool forceConversionCheck = false)
        {
            if (Game.PlayerStats.RevisionNumber <= 0 || forceConversionCheck == true)
                ConvertPlayerNameFormat(ref playerName, ref romanNumerals);

            if (isFemale == true)
            {
                if (LocaleBuilder.languageType == LanguageType.Chinese_Simp && (romanNumerals == "" || romanNumerals == null))
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_14_NEW_SINGULAR_ZH"), playerName, "").Trim();
                else
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_14_NEW"), playerName, romanNumerals).Trim();
            }
            else
            {
                if (LocaleBuilder.languageType == LanguageType.Chinese_Simp && (romanNumerals == "" || romanNumerals == null))
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_12_NEW_SINGULAR_ZH"), playerName, "").Trim();
                else
                    return string.Format(LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_12_NEW"), playerName, romanNumerals).Trim();
            }
        }

        public static string NameHelper()
        {
            return NameHelper(Game.PlayerStats.PlayerName, Game.PlayerStats.RomanNumeral, Game.PlayerStats.IsFemale);
        }

        public static bool gameIsCorrupt;
        public void SaveOnExit()
        {
            if (gameIsCorrupt == false)
            {
                // Quick hack to fix bug where save file is deleted on closing during splash screen.
                if (ScreenManager.CurrentScreen is CDGSplashScreen == false && ScreenManager.CurrentScreen is DemoStartScreen == false)
                {
                    ProceduralLevelScreen level = Game.ScreenManager.GetLevelScreen();
                    //Special handling to revert your spell if you are in a carnival room.
                    if (level != null && (level.CurrentRoom is CarnivalShoot1BonusRoom || level.CurrentRoom is CarnivalShoot2BonusRoom))
                    {
                        if (level.CurrentRoom is CarnivalShoot1BonusRoom)
                            (level.CurrentRoom as CarnivalShoot1BonusRoom).UnequipPlayer();
                        if (level.CurrentRoom is CarnivalShoot2BonusRoom)
                            (level.CurrentRoom as CarnivalShoot2BonusRoom).UnequipPlayer();
                    }

                    // A check to make sure challenge rooms do not override player save data.
                    if (level != null)
                    {
                        ChallengeBossRoomObj challengeRoom = level.CurrentRoom as ChallengeBossRoomObj;
                        if (challengeRoom != null)
                        {
                            challengeRoom.LoadPlayerData(); // Make sure this is loaded before upgrade data, otherwise player equipment will be overridden.
                            SaveManager.LoadFiles(level, SaveType.UpgradeData);
                            level.Player.CurrentHealth = challengeRoom.StoredHP;
                            level.Player.CurrentMana = challengeRoom.StoredMP;
                        }
                    }

                    // Special check in case the user closes the program while in the game over screen to reset the traits.
                    if (ScreenManager.CurrentScreen is GameOverScreen)
                        Game.PlayerStats.Traits = Vector2.Zero;

                    if (SaveManager.FileExists(SaveType.PlayerData))
                    {
                        SaveManager.SaveFiles(SaveType.PlayerData, SaveType.UpgradeData);

                        // This code is needed otherwise the lineage data will still be on Revision 0 when the game exits, but player data is Rev1
                        // which results in a mismatch.
                        if (Game.PlayerStats.RevisionNumber <= 0)
                            SaveManager.SaveFiles(SaveType.Lineage);

                        // IMPORTANT!! Only save map data if you are actually in the castle. Not at the title screen, starting room, or anywhere else. Also make sure not to save during the intro scene.
                        if (Game.PlayerStats.TutorialComplete == true && level != null && level.CurrentRoom.Name != "Start" && level.CurrentRoom.Name != "Ending" && level.CurrentRoom.Name != "Tutorial")
                            SaveManager.SaveFiles(SaveType.MapData);
                    }
                }
            }
        }

        //protected override void OnExiting(object sender, EventArgs args)
        //{
        //    // Quick hack to fix bug where save file is deleted on closing during splash screen.
        //    if (ScreenManager.CurrentScreen is CDGSplashScreen == false && ScreenManager.CurrentScreen is DemoStartScreen == false)
        //    {
        //        UpdatePlaySessionLength();

        //        ProceduralLevelScreen level = Game.ScreenManager.GetLevelScreen();
        //        //Special handling to revert your spell if you are in a carnival room.
        //        if (level != null && (level.CurrentRoom is CarnivalShoot1BonusRoom || level.CurrentRoom is CarnivalShoot2BonusRoom))
        //        {
        //            if (level.CurrentRoom is CarnivalShoot1BonusRoom)
        //                (level.CurrentRoom as CarnivalShoot1BonusRoom).UnequipPlayer();
        //            if (level.CurrentRoom is CarnivalShoot2BonusRoom)
        //                (level.CurrentRoom as CarnivalShoot2BonusRoom).UnequipPlayer();
        //        }

        //        // Special check in case the user closes the program while in the game over screen to reset the traits.
        //        if (ScreenManager.CurrentScreen is GameOverScreen)
        //            Game.PlayerStats.Traits = Vector2.Zero;

        //        if (SaveManager.FileExists(SaveType.PlayerData))
        //        {
        //            SaveManager.SaveFiles(SaveType.PlayerData, SaveType.UpgradeData);

        //            // IMPORTANT!! Only save map data if you are actually in the castle. Not at the title screen, starting room, or anywhere else. Also make sure not to save during the intro scene.
        //            if (Game.PlayerStats.TutorialComplete == true && level != null && level.CurrentRoom.Name != "Start" && level.CurrentRoom.Name != "Ending" && level.CurrentRoom.Name != "Tutorial")
        //                SaveManager.SaveFiles(SaveType.MapData);
        //        }
        //    }

        //    SWManager.instance().shutdown();
        //    base.OnExiting(sender, args);
        //}

        public PhysicsManager PhysicsManager
        {
            get { return m_physicsManager; }
        }

        public ContentManager ContentManager
        {
            get { return Content; }
        }

        public SaveGameManager SaveManager
        {
            get { return m_saveGameManager; }
        }

        public List<Vector2> GetSupportedResolutions()
        {
            List<Vector2> list = new List<Vector2>();
            foreach (DisplayMode mode in GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                //if (mode.AspectRatio > 1.7f)
                if (mode.Width < 2000 && mode.Height < 2000) // Restricts the resolution to below 2048 (which is max supported texture size).
                {
                    Vector2 res = new Vector2(mode.Width, mode.Height);
                    if (list.Contains(res) == false)
                        list.Add(new Vector2(mode.Width, mode.Height));
                }
            }

            //list.Sort(delegate(Vector2 obj1, Vector2 obj2) { return obj1.X.CompareTo(obj2.X); }); // Why did I do this? It just screwed up the ordering.

            return list;
        }

        public void SaveConfig()
        {
            Console.WriteLine("Saving Config file");

            if (!Directory.Exists(Program.OSDir))
                Directory.CreateDirectory(Program.OSDir);
            string configFilePath = Path.Combine(Program.OSDir, "GameConfig.ini");

            using (StreamWriter writer = new StreamWriter(configFilePath, false))
            {
                writer.WriteLine("[Screen Resolution]");
                writer.WriteLine("ScreenWidth=" + Game.GameConfig.ScreenWidth);
                writer.WriteLine("ScreenHeight=" + Game.GameConfig.ScreenHeight);
                writer.WriteLine();
                writer.WriteLine("[Fullscreen]");
                writer.WriteLine("Fullscreen=" + Game.GameConfig.FullScreen);
                writer.WriteLine();
                writer.WriteLine("[QuickDrop]");
                writer.WriteLine("QuickDrop=" + Game.GameConfig.QuickDrop);
                writer.WriteLine();
                writer.WriteLine("[Game Volume]");
                writer.WriteLine("MusicVol=" + String.Format("{0:F2}", Game.GameConfig.MusicVolume));
                writer.WriteLine("SFXVol=" + String.Format("{0:F2}", Game.GameConfig.SFXVolume));
                writer.WriteLine();
                writer.WriteLine("[Joystick Dead Zone]");
                writer.WriteLine("DeadZone=" + InputManager.Deadzone);
                writer.WriteLine();
                writer.WriteLine("[Enable DirectInput Gamepads]");
                writer.WriteLine("EnableDirectInput=" + Game.GameConfig.EnableDirectInput);
                writer.WriteLine();
                writer.WriteLine("[Reduce Shader Quality]");
                writer.WriteLine("ReduceQuality=" + Game.GameConfig.ReduceQuality);
                writer.WriteLine();
                //writer.WriteLine("[Enable Steam Cloud]");
                //writer.WriteLine("EnableSteamCloud=" + Game.GameConfig.EnableSteamCloud);
                //writer.WriteLine();
                writer.WriteLine("[Profile]");
                writer.WriteLine("Slot=" + Game.GameConfig.ProfileSlot);
                writer.WriteLine();
                writer.WriteLine("[Keyboard Config]");
                writer.WriteLine("KeyUP=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_UP1]);
                writer.WriteLine("KeyDOWN=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_DOWN1]);
                writer.WriteLine("KeyLEFT=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_LEFT1]);
                writer.WriteLine("KeyRIGHT=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_RIGHT1]);
                writer.WriteLine("KeyATTACK=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_ATTACK]);
                writer.WriteLine("KeyJUMP=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP1]);
                writer.WriteLine("KeySPECIAL=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_BLOCK]);
                writer.WriteLine("KeyDASHLEFT=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_DASHLEFT]);
                writer.WriteLine("KeyDASHRIGHT=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_DASHRIGHT]);
                writer.WriteLine("KeySPELL1=" + Game.GlobalInput.KeyList[InputMapType.PLAYER_SPELL1]);
                writer.WriteLine();
                writer.WriteLine("[Gamepad Config]");
                writer.WriteLine("ButtonUP=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_UP1]);
                writer.WriteLine("ButtonDOWN=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_DOWN1]);
                writer.WriteLine("ButtonLEFT=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_LEFT1]);
                writer.WriteLine("ButtonRIGHT=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_RIGHT1]);
                writer.WriteLine("ButtonATTACK=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_ATTACK]);
                writer.WriteLine("ButtonJUMP=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_JUMP1]);
                writer.WriteLine("ButtonSPECIAL=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_BLOCK]);
                writer.WriteLine("ButtonDASHLEFT=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_DASHLEFT]);
                writer.WriteLine("ButtonDASHRIGHT=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_DASHRIGHT]);
                writer.WriteLine("ButtonSPELL1=" + Game.GlobalInput.ButtonList[InputMapType.PLAYER_SPELL1]);
                writer.WriteLine();
                writer.WriteLine("[Language]");
                writer.WriteLine("Language=" + LocaleBuilder.languageType);
                writer.WriteLine();
                if (Game.GameConfig.UnlockTraitor > 0)
                    writer.WriteLine("UnlockTraitor=" + Game.GameConfig.UnlockTraitor);

                writer.Close();
            }
        }

        public void LoadConfig()
        {
            Console.WriteLine("Loading Config file");
            InitializeDefaultConfig(); // Initialize a default config first in case new config data is added in the future.
            try
            {
                string configFilePath = Path.Combine(Program.OSDir, "GameConfig.ini");

                using (StreamReader reader = new StreamReader(configFilePath))
                {
                    // flibit didn't like this
                    // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    // ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    CultureInfo ci = CultureInfo.InvariantCulture;

                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int titleIndex = line.IndexOf("=");

                        if (titleIndex != -1)
                        {
                            string lineTitle = line.Substring(0, titleIndex);
                            string lineValue = line.Substring(titleIndex + 1);

                            switch (lineTitle)
                            {
                                case ("ScreenWidth"):
                                    Game.GameConfig.ScreenWidth = int.Parse(lineValue, NumberStyles.Any, ci);
                                    break;
                                case ("ScreenHeight"):
                                    Game.GameConfig.ScreenHeight = int.Parse(lineValue, NumberStyles.Any, ci);
                                    break;
                                case ("Fullscreen"):
                                    Game.GameConfig.FullScreen = bool.Parse(lineValue);
                                    break;
                                case("QuickDrop"):
                                    Game.GameConfig.QuickDrop = bool.Parse(lineValue);
                                    break;
                                case ("MusicVol"):
                                    Game.GameConfig.MusicVolume = Single.Parse(lineValue);
                                    break;
                                case ("SFXVol"):
                                    Game.GameConfig.SFXVolume = Single.Parse(lineValue);
                                    break;
                                case ("DeadZone"):
                                    InputManager.Deadzone = int.Parse(lineValue, NumberStyles.Any, ci);
                                    break;
                                case ("EnableDirectInput"):
                                    Game.GameConfig.EnableDirectInput = bool.Parse(lineValue);
                                    break;
                                case ("ReduceQuality"):
                                    Game.GameConfig.ReduceQuality = bool.Parse(lineValue);
                                    LevelEV.SAVE_FRAMES = Game.GameConfig.ReduceQuality;
                                    break;
                                case ("EnableSteamCloud"):
                                    Game.GameConfig.EnableSteamCloud = bool.Parse(lineValue);
                                    break;
                                case ("Slot"):
                                    Game.GameConfig.ProfileSlot = byte.Parse(lineValue, NumberStyles.Any, ci);
                                    break;
                                case ("KeyUP"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_UP1] =  (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyDOWN"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_DOWN1] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyLEFT"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_LEFT1] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyRIGHT"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_RIGHT1] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyATTACK"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_ATTACK] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyJUMP"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP1] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeySPECIAL"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_BLOCK] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyDASHLEFT"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_DASHLEFT] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeyDASHRIGHT"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_DASHRIGHT] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("KeySPELL1"):
                                    Game.GlobalInput.KeyList[InputMapType.PLAYER_SPELL1] = (Keys)Enum.Parse(typeof(Keys), lineValue);
                                    break;
                                case ("ButtonUP"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_UP1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonDOWN"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_DOWN1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonLEFT"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_LEFT1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonRIGHT"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_RIGHT1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonATTACK"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_ATTACK] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonJUMP"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_JUMP1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonSPECIAL"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_BLOCK] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonDASHLEFT"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_DASHLEFT] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonDASHRIGHT"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_DASHRIGHT] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("ButtonSPELL1"):
                                    Game.GlobalInput.ButtonList[InputMapType.PLAYER_SPELL1] = (Buttons)Enum.Parse(typeof(Buttons), lineValue);
                                    break;
                                case ("Language"):
                                    LocaleBuilder.languageType = (LanguageType)Enum.Parse(typeof(LanguageType), lineValue);
                                    break;
                                case("UnlockTraitor"):
                                    Game.GameConfig.UnlockTraitor = byte.Parse(lineValue, NumberStyles.Any, ci);
                                    break;
                            }
                        }
                    }

                    // Special code so that player attack acts as the second confirm and player jump acts as the second cancel.
                    Game.GlobalInput.KeyList[InputMapType.MENU_CONFIRM2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_ATTACK];
                    Game.GlobalInput.KeyList[InputMapType.MENU_CANCEL2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP1];

                    reader.Close();

                    // Game config file was not loaded properly. Throw an exception.
                    if (Game.GameConfig.ScreenHeight <= 0 || Game.GameConfig.ScreenWidth <= 0)
                        throw new Exception("Blank Config File");
                }
            }
            catch
            {
                //If exception occurred, then no file was found and default config must be created.
                Console.WriteLine("Config File Not Found. Creating Default Config File.");
                InitializeDefaultConfig();
                this.SaveConfig();
            }
        }

        public void InitializeScreenConfig()
        {
            if (Environment.GetEnvironmentVariable("SteamTenfoot") == "1" || Environment.GetEnvironmentVariable("SteamDeck") == "1")
            {
                // We are asked to override resolution settings in Big Picture modes
                DisplayMode mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                graphics.PreferredBackBufferWidth = mode.Width;
                graphics.PreferredBackBufferHeight = mode.Height;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            else
            {
                graphics.PreferredBackBufferWidth = Game.GameConfig.ScreenWidth;
                graphics.PreferredBackBufferHeight = Game.GameConfig.ScreenHeight;
                if ((graphics.IsFullScreen == true && Game.GameConfig.FullScreen == false) || (graphics.IsFullScreen == false && Game.GameConfig.FullScreen == true))
                    graphics.ToggleFullScreen();
                else
                    graphics.ApplyChanges();
            }

            // No need to call Graphics.ApplyChanges() since ToggleFullScreen() implicitly calls it.
            Game.ScreenManager.ForceResolutionChangeCheck();
        }

        public static void ChangeBitmapLanguage(SpriteObj sprite, string spriteName)
        {
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.English):
                    sprite.ChangeSprite(spriteName);
                    break;
                case (LanguageType.German):
                    sprite.ChangeSprite(spriteName + "_DE");
                    break;
                case (LanguageType.Russian):
                    sprite.ChangeSprite(spriteName + "_RU");
                    break;
                case (LanguageType.French):
                    sprite.ChangeSprite(spriteName + "_FR");
                    break;
                case (LanguageType.Polish):
                    sprite.ChangeSprite(spriteName + "_PO");
                    break;
                case (LanguageType.Portuguese_Brazil):
                    sprite.ChangeSprite(spriteName + "_BR");
                    break;
                case (LanguageType.Spanish_Spain):
                    sprite.ChangeSprite(spriteName + "_SP");
                    break;
                case (LanguageType.Chinese_Simp):
                    sprite.ChangeSprite(spriteName + "_ZH");
                    break;
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return graphics; }
        }

        public struct SettingStruct
        {
            public int ScreenWidth;
            public int ScreenHeight;
            public bool FullScreen;
            public float MusicVolume;
            public float SFXVolume;
            public bool QuickDrop;
            public bool EnableDirectInput;
            public byte ProfileSlot;
            public bool ReduceQuality;
            public bool EnableSteamCloud;
            public byte UnlockTraitor;
        }
    }
}
