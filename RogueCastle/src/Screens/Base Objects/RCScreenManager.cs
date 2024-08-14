using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class RCScreenManager : ScreenManager
    {
        private GameOverScreen m_gameOverScreen;
        private SkillScreen m_traitScreen;
        private EnchantressScreen m_enchantressScreen;
        private BlacksmithScreen m_blacksmithScreen;
        private GetItemScreen m_getItemScreen;
        private DialogueScreen m_dialogueScreen;
        private MapScreen m_mapScreen;
        private PauseScreen m_pauseScreen;
        private OptionsScreen m_optionsScreen;
        private ProfileCardScreen m_profileCardScreen;
        private CreditsScreen m_creditsScreen;
        private SkillUnlockScreen m_skillUnlockScreen;
        private DiaryEntryScreen m_diaryEntryScreen;
        private DeathDefiedScreen m_deathDefyScreen;
        private DiaryFlashbackScreen m_flashbackScreen;
        private TextScreen m_textScreen;
        private GameOverBossScreen m_gameOverBossScreen;

        private ProfileSelectScreen m_profileSelectScreen;

        private VirtualScreen m_virtualScreen;

        private bool m_isTransitioning = false;

        private bool m_inventoryVisible = false;

        private PlayerObj m_player;
        private SpriteObj m_blackTransitionIn, m_blackScreen, m_blackTransitionOut;
        private bool m_isWipeTransitioning = false;

        private int m_currentScreenType;
        public int currentScreenType
        { get { return m_currentScreenType; } }

        public RCScreenManager(Game Game)
            : base(Game)
        {
        }

        public override void Initialize()
        {
            InitializeScreens();
            base.Initialize(); // Camera gets initialized here.

            m_virtualScreen = new VirtualScreen(GlobalEV.ScreenWidth, GlobalEV.ScreenHeight, Camera.GraphicsDevice);
            Game.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            Game.Deactivated += new EventHandler<EventArgs>(PauseGame);

            // Setting up the event handling for window dragging.
            // THIS HAS BEEN DISABLED BECAUSE WE ADDED MOUSE BUTTONS AS BINDABLE
            //System.Windows.Forms.Form winForm = System.Windows.Forms.Control.FromHandle(Game.Window.Handle) as System.Windows.Forms.Form;
            //if (winForm != null)
            //    winForm.MouseCaptureChanged += new EventHandler(PauseGame);

            //Camera.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(ReinitializeContent);
            //Window.AllowUserResizing = true;
        }

        public void PauseGame(object sender, EventArgs e)
        {
            ///Microsoft.Xna.Framework.WindowsGameForm
            //Console.WriteLine(sender is Microsoft.Xna.Framework.GameWindow);
            //Console.WriteLine(sender);
            ProceduralLevelScreen level = CurrentScreen as ProceduralLevelScreen;
            if (level != null && (level.CurrentRoom is EndingRoomObj == false))// && RogueCastle.Game.PlayerStats.TutorialComplete == true)
                DisplayScreen(ScreenType.Pause, true);
        }

        public void ReinitializeContent(object sender, EventArgs e)
        {
            m_virtualScreen.ReinitializeRTs(Game.GraphicsDevice);

            foreach (Screen screen in m_screenArray)
                screen.DisposeRTs();

            foreach (Screen screen in m_screenArray)
                screen.ReinitializeRTs();
        }

        public void ReinitializeCamera(GraphicsDevice graphicsDevice)
        {
            m_camera.Dispose();
            m_camera = new Camera2D(graphicsDevice, EngineEV.ScreenWidth, EngineEV.ScreenHeight);
        }

        private List<Screen> m_screenCleanupList = new List<Screen>();
        public void InitializeScreens()
        {
            if (m_gameOverScreen != null)
                m_screenCleanupList.Add(m_gameOverScreen);
            m_gameOverScreen = new GameOverScreen();

            if (m_traitScreen != null)
                m_screenCleanupList.Add(m_traitScreen);
            m_traitScreen = new SkillScreen();

            if (m_blacksmithScreen != null)
                m_screenCleanupList.Add(m_blacksmithScreen);
            m_blacksmithScreen = new BlacksmithScreen();

            if (m_getItemScreen != null)
                m_screenCleanupList.Add(m_getItemScreen);
            m_getItemScreen = new GetItemScreen();

            if (m_enchantressScreen != null)
                m_screenCleanupList.Add(m_enchantressScreen);
            m_enchantressScreen = new EnchantressScreen();

            if (m_dialogueScreen != null)
                m_screenCleanupList.Add(m_dialogueScreen);
            m_dialogueScreen = new DialogueScreen();

            if (m_pauseScreen != null)
                m_screenCleanupList.Add(m_pauseScreen);
            m_pauseScreen = new PauseScreen();

            if (m_optionsScreen != null)
                m_screenCleanupList.Add(m_optionsScreen);
            m_optionsScreen = new OptionsScreen();

            if (m_profileCardScreen != null)
                m_screenCleanupList.Add(m_profileCardScreen);
            m_profileCardScreen = new ProfileCardScreen();

            if (m_creditsScreen != null)
                m_screenCleanupList.Add(m_creditsScreen);
            m_creditsScreen = new CreditsScreen();

            if (m_skillUnlockScreen != null)
                m_screenCleanupList.Add(m_skillUnlockScreen);
            m_skillUnlockScreen = new SkillUnlockScreen();

            if (m_diaryEntryScreen != null)
                m_screenCleanupList.Add(m_diaryEntryScreen);
            m_diaryEntryScreen = new DiaryEntryScreen();

            if (m_deathDefyScreen != null)
                m_screenCleanupList.Add(m_deathDefyScreen);
            m_deathDefyScreen = new DeathDefiedScreen();

            if (m_textScreen != null)
                m_screenCleanupList.Add(m_textScreen);
            m_textScreen = new TextScreen();

            if (m_flashbackScreen != null)
                m_screenCleanupList.Add(m_flashbackScreen);
            m_flashbackScreen = new DiaryFlashbackScreen();

            if (m_gameOverBossScreen != null)
                m_screenCleanupList.Add(m_gameOverBossScreen);
            m_gameOverBossScreen = new GameOverBossScreen();

            if (m_profileSelectScreen != null)
                m_screenCleanupList.Add(m_profileSelectScreen);
            m_profileSelectScreen = new ProfileSelectScreen();
        }

        public override void LoadContent()
        {
            m_gameOverScreen.LoadContent();
            m_traitScreen.LoadContent();
            m_blacksmithScreen.LoadContent();
            m_getItemScreen.LoadContent();
            m_enchantressScreen.LoadContent();
            m_dialogueScreen.LoadContent();
            m_pauseScreen.LoadContent();
            m_optionsScreen.LoadContent();
            m_profileCardScreen.LoadContent();
            m_creditsScreen.LoadContent();
            m_skillUnlockScreen.LoadContent();
            m_diaryEntryScreen.LoadContent();
            m_deathDefyScreen.LoadContent();
            m_textScreen.LoadContent();
            m_flashbackScreen.LoadContent();
            m_gameOverBossScreen.LoadContent();
            m_profileSelectScreen.LoadContent();

            if (IsContentLoaded == false)
            {
                m_blackTransitionIn = new SpriteObj("Blank_Sprite");
                m_blackTransitionIn.Rotation = 15;
                m_blackTransitionIn.Scale = new Vector2(1320 / m_blackTransitionIn.Width, 2000 / m_blackTransitionIn.Height);
                m_blackTransitionIn.TextureColor = Color.Black;
                m_blackTransitionIn.ForceDraw = true;

                m_blackScreen = new SpriteObj("Blank_Sprite");
                m_blackScreen.Scale = new Vector2(1320 / m_blackScreen.Width, 720 / m_blackScreen.Height);
                m_blackScreen.TextureColor = Color.Black;
                m_blackScreen.ForceDraw = true;

                m_blackTransitionOut = new SpriteObj("Blank_Sprite");
                m_blackTransitionOut.Rotation = 15;
                m_blackTransitionOut.Scale = new Vector2(1320 / m_blackTransitionOut.Width, 2000 / m_blackTransitionOut.Height);
                m_blackTransitionOut.TextureColor = Color.Black;
                m_blackTransitionOut.ForceDraw = true;

                m_blackTransitionIn.X = 0;
                m_blackTransitionIn.X = 1320 - m_blackTransitionIn.Bounds.Left;
                m_blackScreen.X = m_blackTransitionIn.X;
                m_blackTransitionOut.X = m_blackScreen.X + m_blackScreen.Width;
                m_blackTransitionIn.Visible = false;
                m_blackScreen.Visible = false;
                m_blackTransitionOut.Visible = false;

                LoadPlayer();
            }

            base.LoadContent();
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            m_virtualScreen.PhysicalResolutionChanged();
            EngineEV.RefreshEngine(Camera.GraphicsDevice);
            Console.WriteLine("resolution changed");
        }

        private void LoadPlayer()
        {
            if (m_player == null)
            {
                m_player = new PlayerObj("PlayerIdle_Character", PlayerIndex.One, (Game as Game).PhysicsManager, null, (Game as Game));
                m_player.Position = new Vector2(200, 200);
                m_player.Initialize();
            }
        }

        public void DisplayScreen(int screenType, bool pauseOtherScreens, List<object> objList = null)
        {
            LoadPlayer();

            if (pauseOtherScreens == true)
            {
                // This needs to be changed so that the ScreenManager holds a reference to the ProceduralLevelScreen.
                foreach (Screen screen in GetScreens())
                {
                    if (screen == CurrentScreen)
                    {
                        screen.PauseScreen();
                        break;
                    }
                }
            }

            m_currentScreenType = screenType;
            //if (CurrentScreen != null && !(CurrentScreen is ProceduralLevelScreen))
            //    RemoveScreen(CurrentScreen, false);

            // This currently has no checks to see if the screen is already in the screenmanager's screen list.
            switch (screenType)
            {
                case (ScreenType.CDGSplash):
                case(ScreenType.BlitWorks):
                case (ScreenType.Title):
                case(ScreenType.TitleWhite):
                case (ScreenType.StartingRoom):
                case (ScreenType.DemoStart):
                case(ScreenType.DemoEnd):
                case (ScreenType.Lineage):
                    this.LoadScreen((byte)screenType, true);
                    break;
                case (ScreenType.Level):
                    if (RogueCastle.Game.PlayerStats.LockCastle == true || (this.CurrentScreen is ProceduralLevelScreen == false))
                        this.LoadScreen((byte)screenType, true);
                    else
                        this.LoadScreen((byte)screenType, false);
                    break;
                case (ScreenType.Skill):
                    this.AddScreen(m_traitScreen, null);
                    break;
                case(ScreenType.GameOver):
                    m_gameOverScreen.PassInData(objList);
                    this.AddScreen(m_gameOverScreen, null);
                    break;
                case (ScreenType.GameOverBoss):
                    m_gameOverBossScreen.PassInData(objList);
                    this.AddScreen(m_gameOverBossScreen, null);
                    break;
                case (ScreenType.Blacksmith):
                    m_blacksmithScreen.Player = m_player;
                    this.AddScreen(m_blacksmithScreen, null);
                    break;
                case (ScreenType.GetItem):
                    m_getItemScreen.PassInData(objList);
                    this.AddScreen(m_getItemScreen, null);
                    break;
                case (ScreenType.Enchantress):
                    m_enchantressScreen.Player = m_player;
                    this.AddScreen(m_enchantressScreen, null);
                    break;
                case (ScreenType.Dialogue):
                    this.AddScreen(m_dialogueScreen, null);
                    break;
                case (ScreenType.Map):
                    m_mapScreen.SetPlayer(m_player);
                    this.AddScreen(m_mapScreen, null);
                    break;
                case (ScreenType.Pause):
                    GetLevelScreen().CurrentRoom.DarkenRoom();
                    this.AddScreen(m_pauseScreen, null);
                    break;
                case (ScreenType.Options):
                    m_optionsScreen.PassInData(objList);
                    this.AddScreen(m_optionsScreen, null);
                    break;
                case(ScreenType.ProfileCard):
                    this.AddScreen(m_profileCardScreen, null);
                    break;
                case (ScreenType.Credits):
                    this.LoadScreen(ScreenType.Credits, true);
                    break;
                case (ScreenType.SkillUnlock):
                    m_skillUnlockScreen.PassInData(objList);
                    this.AddScreen(m_skillUnlockScreen, null);
                    break;
                case (ScreenType.DiaryEntry):
                    this.AddScreen(m_diaryEntryScreen, null);
                    break;
                case (ScreenType.DeathDefy):
                    this.AddScreen(m_deathDefyScreen, null);
                    break;
                case (ScreenType.Text):
                    m_textScreen.PassInData(objList);
                    this.AddScreen(m_textScreen, null);
                    break;
                case (ScreenType.TutorialRoom):
                    this.LoadScreen(ScreenType.TutorialRoom, true);
                    break;
                case (ScreenType.Ending):
                    GetLevelScreen().CameraLockedToPlayer = false;
                    GetLevelScreen().DisableRoomTransitioning = true;
                    Player.Position = new Vector2(100, 100); //HHHAACCK
                    this.LoadScreen(ScreenType.Ending, true);
                    break;
                case (ScreenType.DiaryFlashback):
                    this.AddScreen(m_flashbackScreen, null);
                    break;
                case (ScreenType.ProfileSelect):
                    this.AddScreen(m_profileSelectScreen, null);
                    break;
            }

            if (m_isWipeTransitioning == true)
                EndWipeTransition();
        }

        public void AddRoomsToMap(List<RoomObj> roomList)
        {
            m_mapScreen.AddRooms(roomList);
        }

        public void AddIconsToMap(List<RoomObj> roomList)
        {
            m_mapScreen.AddAllIcons(roomList);
        }

        public void RefreshMapScreenChestIcons(RoomObj room)
        {
            m_mapScreen.RefreshMapChestIcons(room);
        }

        public void ActivateMapScreenTeleporter()
        {
            m_mapScreen.IsTeleporter = true;
        }

        public void HideCurrentScreen()
        {
            this.RemoveScreen(this.CurrentScreen, false);
            
            ProceduralLevelScreen level = CurrentScreen as ProceduralLevelScreen;
            if (level != null)
                level.UnpauseScreen();

            if (m_isWipeTransitioning == true)
                EndWipeTransition();
        }

        public void ForceResolutionChangeCheck()
        {
            m_virtualScreen.PhysicalResolutionChanged();
            EngineEV.RefreshEngine(this.Game.GraphicsDevice);
        }

        // This is overridden so that a custom LoadScreen can be passed in.
        private void LoadScreen(byte screenType, bool wipeTransition)
        {
            m_currentScreenType = ScreenType.Loading;
            foreach (Screen screen in m_screenArray)
            {
                screen.DrawIfCovered = true;
                ProceduralLevelScreen levelScreen = screen as ProceduralLevelScreen;
                if (levelScreen != null)
                {
                    m_player.AttachLevel(levelScreen);
                    levelScreen.Player = m_player;

                    AttachMap(levelScreen);
                }
            }

            // Double check this.  This doesn't seem right.
            if (m_gameOverScreen != null)
            {
                this.InitializeScreens();
                this.LoadContent(); // Since all screens are disposed, their content needs to be loaded again. Hacked.
            }

            // Create and activate the loading screen.
            LoadingScreen loadingScreen = new LoadingScreen(screenType, wipeTransition);
            m_isTransitioning = true;

            this.AddScreen(loadingScreen, PlayerIndex.One);

            // This has been moved to PerformCleanUp().
            //GC.Collect(); // Should this be called? Most people say don't, but I want to ensure that collection occurs during the loading screen, not some random moment later.
        }

        public void PerformCleanUp()
        {
            foreach (Screen screen in m_screenCleanupList)
            {
                if (screen.IsDisposed == false && screen.IsContentLoaded == true)
                    screen.Dispose();
            }
            m_screenCleanupList.Clear();

            GC.Collect(); // Should this be called? Most people say don't, but I want to ensure that collection occurs during the loading screen, not some random moment later.
        }

        public void LoadingComplete(int screenType)
        {
            m_currentScreenType = screenType;
        }

        public override void RemoveScreen(Screen screen, bool disposeScreen)
        {
            if (screen is LoadingScreen)
                m_isTransitioning = false;
            base.RemoveScreen(screen, disposeScreen);
        }

        public void AttachMap(ProceduralLevelScreen level)
        {
            if (m_mapScreen != null)
                m_mapScreen.Dispose();
            m_mapScreen = new MapScreen(level);
        }

        public override void Update(GameTime gameTime)
        {
            m_virtualScreen.Update();
            if (m_isTransitioning == false)
                base.Update(gameTime);
            else
            {
                Camera.GameTime = gameTime;
                if (CurrentScreen != null)
                {
                    CurrentScreen.Update(gameTime);
                    CurrentScreen.HandleInput();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            m_virtualScreen.BeginCapture();
            Camera.GraphicsDevice.Clear(Color.Black);

            // Must be called after BeginCapture(), in case graphics virtualization fails and the device is hard reset.
            if (Camera.GameTime == null)
                Camera.GameTime = gameTime;

            base.Draw(gameTime);

            m_virtualScreen.EndCapture();
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            m_virtualScreen.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_blackTransitionIn.Draw(Camera);
            m_blackTransitionOut.Draw(Camera);
            m_blackScreen.Draw(Camera);
            Camera.End();

            if (LevelEV.ENABLE_DEBUG_INPUT == true)
            {
                Camera.Begin();
                string forcedGenderString = "None";
                switch (RogueCastle.Game.PlayerStats.ForceLanguageGender)
                {
                    case (1):
                        forcedGenderString = "Male";
                        break;
                    case (2):
                        forcedGenderString = "Female";
                        break;
                }

                string godModeString = "Off";
                if (RogueCastle.Game.PlayerStats.GodMode == true)
                    godModeString = "On";
                Camera.DrawString(RogueCastle.Game.PixelArtFont, "Forced Gender Language: " + forcedGenderString, new Vector2(10, 10), Color.White);
                Camera.DrawString(RogueCastle.Game.PixelArtFont, "God Mode: " + godModeString, new Vector2(10, 30), Color.White);
                Camera.End();
            }
        }

        public void StartWipeTransition()
        {
            m_isWipeTransitioning = true;
            m_blackTransitionIn.Visible = true;
            m_blackScreen.Visible = true;
            m_blackTransitionOut.Visible = true;

            m_blackTransitionIn.X = 0;
            m_blackTransitionOut.Y = -500;
            m_blackTransitionIn.X = 1320 - m_blackTransitionIn.Bounds.Left;
            m_blackScreen.X = m_blackTransitionIn.X;
            m_blackTransitionOut.X = m_blackScreen.X + m_blackScreen.Width;

            Tween.By(m_blackTransitionIn, 0.15f, Quad.EaseInOut, "X", (-m_blackTransitionIn.X).ToString());
            Tween.By(m_blackScreen, 0.15f, Quad.EaseInOut, "X", (-m_blackTransitionIn.X).ToString());
            Tween.By(m_blackTransitionOut, 0.15f, Quad.EaseInOut, "X", (-m_blackTransitionIn.X).ToString());
            //Tween.AddEndHandlerToLastTween(this, "EndWipeTransition");
        }

        public void EndWipeTransition()
        {
            m_isWipeTransitioning = false;

            m_blackTransitionOut.Y = -500;
            Tween.By(m_blackTransitionIn, 0.25f, Quad.EaseInOut, "X", "-3000");
            Tween.By(m_blackScreen, 0.25f, Quad.EaseInOut, "X", "-3000");
            Tween.By(m_blackTransitionOut, 0.25f, Quad.EaseInOut, "X", "-3000");
        }

        public void UpdatePauseScreenIcons()
        {
            m_pauseScreen.UpdatePauseScreenIcons();
        }

        public bool InventoryVisible
        {
            get { return m_inventoryVisible; }
        }

        public ProceduralLevelScreen GetLevelScreen()
        {
            foreach (Screen screen in GetScreens())
            {
                ProceduralLevelScreen level = screen as ProceduralLevelScreen;
                if (level != null)
                    return level;
            }
            return null;
        }

        public RenderTarget2D RenderTarget
        {
            get { return m_virtualScreen.RenderTarget; }
        }

        public DialogueScreen DialogueScreen
        {
            get { return m_dialogueScreen; }
        }

        public SkillScreen SkillScreen
        {
            get { return m_traitScreen; }
        }

        public PlayerObj Player
        {
            get { return m_player; }
        }

        public bool IsTransitioning
        {
            get { return m_isTransitioning; }
        }
    }
}
