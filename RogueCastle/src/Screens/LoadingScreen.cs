using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class LoadingScreen : Screen
    {
        private TextObj m_loadingText;

        private byte m_screenTypeToLoad = 0;
        private bool m_isLoading = false;
        private bool m_loadingComplete = false;
        private Screen m_levelToLoad = null;

        private ImpactEffectPool m_effectPool;

        private ObjContainer m_gateSprite;

        private SpriteObj m_blackTransitionIn, m_blackScreen, m_blackTransitionOut;
        private bool m_wipeTransition = false;
        private bool m_gameCrashed = false;

        // Special transition code purely for the introduction.
        public float BackBufferOpacity { get; set; }

        public LoadingScreen(byte screenType, bool wipeTransition)
        {
            m_screenTypeToLoad = screenType;
            m_effectPool = new ImpactEffectPool(50);
            m_wipeTransition = wipeTransition;
        }

        public override void LoadContent()
        {
            m_loadingText = new TextObj();
            m_loadingText.Font = Game.JunicodeLargeFont;
            m_loadingText.Text = LocaleBuilder.getString("LOC_ID_LOADING_SCREEN_1", m_loadingText); //"Building"
            m_loadingText.Align = Types.TextAlign.Centre;
            m_loadingText.FontSize = 40;
            m_loadingText.OutlineWidth = 4;
            m_loadingText.ForceDraw = true;

            m_gateSprite = new ObjContainer("LoadingScreenGate_Character");
            m_gateSprite.ForceDraw = true;
            m_gateSprite.Scale = new Vector2(2, 2);
            m_gateSprite.Y -= m_gateSprite.Height;

            m_effectPool.Initialize();

            m_blackTransitionIn = new SpriteObj("Blank_Sprite");
            m_blackTransitionIn.Rotation = 15;
            m_blackTransitionIn.Scale = new Vector2(1320 / m_blackTransitionIn.Width, 2000 / m_blackTransitionIn.Height);
            m_blackTransitionIn.TextureColor = Color.Black;
            m_blackTransitionIn.ForceDraw = true;
            //m_blackTransitionIn.Visible = false;

            m_blackScreen = new SpriteObj("Blank_Sprite");
            m_blackScreen.Scale = new Vector2(1320 / m_blackScreen.Width, 720 / m_blackScreen.Height);
            m_blackScreen.TextureColor = Color.Black;
            m_blackScreen.ForceDraw = true;
            //m_blackScreen.Visible = false;

            m_blackTransitionOut = new SpriteObj("Blank_Sprite");
            m_blackTransitionOut.Rotation = 15;
            m_blackTransitionOut.Scale = new Vector2(1320 / m_blackTransitionOut.Width, 2000 / m_blackTransitionOut.Height);
            m_blackTransitionOut.TextureColor = Color.Black;
            m_blackTransitionOut.ForceDraw = true;
            //m_blackTransitionOut.Visible = false;


            base.LoadContent();
        }

        public override void OnEnter()
        {
            BackBufferOpacity = 0;
            m_gameCrashed = false;

            if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
                m_loadingText.Text = LocaleBuilder.getString("LOC_ID_LOADING_SCREEN_4", m_loadingText); //"Jacking In"
            else if (Game.PlayerStats.Traits.X == TraitType.Nostalgic || Game.PlayerStats.Traits.Y == TraitType.Nostalgic)
                m_loadingText.Text = LocaleBuilder.getString("LOC_ID_LOADING_SCREEN_3", m_loadingText); //"Reminiscing"
            else if (Game.PlayerStats.Traits.X == TraitType.Baldness || Game.PlayerStats.Traits.Y == TraitType.Baldness)
                m_loadingText.Text = LocaleBuilder.getString("LOC_ID_LOADING_SCREEN_2", m_loadingText); //"Balding"
            else
                m_loadingText.Text = LocaleBuilder.getString("LOC_ID_LOADING_SCREEN_1", m_loadingText); //"Building"

            if (m_loadingComplete == false)
            {
                // A special transition for the intro sequence.
                if (m_screenTypeToLoad == ScreenType.TitleWhite)
                {
                    Tween.To(this, 0.05f, Tween.EaseNone, "BackBufferOpacity", "1");
                    Tween.RunFunction(1, this, "BeginThreading");
                    //Tween.AddEndHandlerToLastTween(this, "BeginThreading");
                }
                else
                {
                    m_blackTransitionIn.X = 0;
                    m_blackTransitionIn.X = 1320 - m_blackTransitionIn.Bounds.Left;
                    m_blackScreen.X = m_blackTransitionIn.X;
                    m_blackTransitionOut.X = m_blackScreen.X + m_blackScreen.Width;

                    if (m_wipeTransition == false)
                    {
                        SoundManager.PlaySound("GateDrop");
                        Tween.To(m_gateSprite, 0.5f, Tween.EaseNone, "Y", "0");
                        Tween.RunFunction(0.3f, m_effectPool, "LoadingGateSmokeEffect", 40);
                        Tween.RunFunction(0.3f, typeof(SoundManager), "PlaySound", "GateSlam");
                        Tween.RunFunction(0.55f, this, "ShakeScreen", 4, true, true);
                        Tween.RunFunction(0.65f, this, "StopScreenShake");
                        Tween.RunFunction(1.5f, this, "BeginThreading"); // Make sure the gate tween animation is done before executing the thread.
                    }
                    else
                    {
                        Tween.By(m_blackTransitionIn, 0.15f, Quad.EaseIn, "X", (-m_blackTransitionIn.X).ToString());
                        Tween.By(m_blackScreen, 0.15f, Quad.EaseIn, "X", (-m_blackTransitionIn.X).ToString());
                        Tween.By(m_blackTransitionOut, 0.2f, Quad.EaseIn, "X", (-m_blackTransitionIn.X).ToString());
                        Tween.AddEndHandlerToLastTween(this, "BeginThreading");
                    }
                }
                base.OnEnter();
            }
        }

        public void BeginThreading()
        {
            Tween.StopAll(false);
            // Object loading is multithreaded.
            Thread loadingThread = new Thread(BeginLoading);
            /* flibit didn't like this
            if (loadingThread.CurrentCulture.Name != "en-US")
            {
                loadingThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);
                loadingThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US", false);
            }
            */
            loadingThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            loadingThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            loadingThread.Start();
        }

        private void BeginLoading()
        {
            m_isLoading = true;
            m_loadingComplete = false;
            switch (m_screenTypeToLoad)
            {
                case (ScreenType.CDGSplash):
                    m_levelToLoad = new CDGSplashScreen();
                    lock (m_levelToLoad)
                        m_loadingComplete = true;
                    break;
                case(ScreenType.BlitWorks):
                    m_levelToLoad = new BlitWorksSplashScreen();
                    lock (m_levelToLoad)
                        m_loadingComplete = true;
                    break;
                case (ScreenType.DemoEnd):
                    m_levelToLoad = new DemoEndScreen();
                    lock (m_levelToLoad)
                        m_loadingComplete = true;
                    break;
                case (ScreenType.DemoStart):
                    m_levelToLoad = new DemoStartScreen();
                    lock (m_levelToLoad)
                        m_loadingComplete = true;
                    break;
                case (ScreenType.Credits):
                    m_levelToLoad = new CreditsScreen();

                    // This is a check to see if you should load the special ending credits or the regular one.
                    // It does this by seeing if a titlescreen exists in the screen list. If it does, it means you loaded from the title screen and should not get the special credits.
                    bool loadEnding = true;
                    foreach (Screen screen in ScreenManager.GetScreens())
                    {
                        if (screen is TitleScreen)
                        {
                            loadEnding = false;
                            break;
                        }
                    }
                    (m_levelToLoad as CreditsScreen).IsEnding = loadEnding;

                    lock (m_levelToLoad)
                        m_loadingComplete = true;
                    break;
                case (ScreenType.Title):
                case (ScreenType.TitleWhite):
                    m_levelToLoad = new TitleScreen();
                    lock (m_levelToLoad)
                    {
                        m_loadingComplete = true;
                    }
                    break;
                case (ScreenType.Lineage):
                    m_levelToLoad = new LineageScreen();
                    lock (m_levelToLoad)
                    {
                        m_loadingComplete = true;
                    }
                    break;
                case (ScreenType.Level):
                case (ScreenType.Ending):
                case (ScreenType.StartingRoom):
                case (ScreenType.TutorialRoom):
                    RCScreenManager manager = ScreenManager as RCScreenManager;

                    AreaStruct[] levelStruct = Game.Area1List;

                    m_levelToLoad = null;

                    // Creating the level.
                    if (m_screenTypeToLoad == ScreenType.StartingRoom)
                        m_levelToLoad = LevelBuilder2.CreateStartingRoom();
                    else if (m_screenTypeToLoad == ScreenType.TutorialRoom)
                        m_levelToLoad = LevelBuilder2.CreateTutorialRoom();
                    else if (m_screenTypeToLoad == ScreenType.Ending)
                        m_levelToLoad = LevelBuilder2.CreateEndingRoom();
                    else
                    {
                        ProceduralLevelScreen level = (ScreenManager as RCScreenManager).GetLevelScreen();
                        // Player is loading a level from the starting room.
                        if (level != null)
                        {
                            if (Game.PlayerStats.LockCastle == true) // Castle is locked. Load the saved map.
                            {
                                try
                                {
                                    m_levelToLoad = (ScreenManager.Game as Game).SaveManager.LoadMap();
                                }
                                catch
                                {
                                    m_gameCrashed = true;
                                }

                                if (m_gameCrashed == false)
                                {
                                    (ScreenManager.Game as Game).SaveManager.LoadFiles(m_levelToLoad as ProceduralLevelScreen, SaveType.MapData);
                                    Game.PlayerStats.LockCastle = false; // Must go after loading map data, because map data uses the LockCastle flag to determine whether or not to reset enemies & fairy chests.
                                }
                            }
                            else // Castle is not locked. Load a new map.
                                m_levelToLoad = LevelBuilder2.CreateLevel(level.RoomList[0], levelStruct);
                        }
                        else // Player is loading from the title screen.
                        {
                            if (Game.PlayerStats.LoadStartingRoom == true) // New game. Player has started in the starting room.
                            {
                                Console.WriteLine("This should only be used for debug purposes");
                                m_levelToLoad = LevelBuilder2.CreateLevel(null, levelStruct);
                                (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.Map, SaveType.MapData);
                            }
                            else // Player has saved data. Load the saved map and saved map data.
                            {
                                try
                                {
                                    m_levelToLoad = (ScreenManager.Game as Game).SaveManager.LoadMap();
                                    (ScreenManager.Game as Game).SaveManager.LoadFiles(m_levelToLoad as ProceduralLevelScreen, SaveType.MapData);
                                }
                                catch
                                {
                                    m_gameCrashed = true;
                                }

                                if (m_gameCrashed == false)
                                {
                                    //(ScreenManager.Game as Game).SaveManager.LoadFiles(m_levelToLoad as ProceduralLevelScreen, SaveType.MapData);
                                    Game.ScreenManager.Player.Position = new Vector2((m_levelToLoad as ProceduralLevelScreen).RoomList[1].X, 720 - 300); // Sets the player in the castle entrance.
                                }
                            }
                        }
                    }

                    if (m_gameCrashed == false)
                    {
                        lock (m_levelToLoad)
                        {
                            ProceduralLevelScreen level = m_levelToLoad as ProceduralLevelScreen;
                            level.Player = manager.Player;
                            manager.Player.AttachLevel(level);
                            for (int i = 0; i < level.RoomList.Count; i++)
                                level.RoomList[i].RoomNumber = i + 1;
                            manager.AttachMap(level);
                            if (m_wipeTransition == false)
                                Thread.Sleep(100);
                            m_loadingComplete = true;
                        }
                    }
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_gameCrashed == true)
            {
                //Game.ScreenManager.DialogueScreen.SetDialogue("Save File Error");
                //Game.ScreenManager.DialogueScreen.SetConfirmEndHandler((ScreenManager.Game as Game).SaveManager, "LoadAutosave");
                //Game.ScreenManager.DisplayScreen(ScreenType.Dialogue, false, null);
                (ScreenManager.Game as Game).SaveManager.ForceBackup();
            }

            if (m_isLoading == true && m_loadingComplete == true && m_gameCrashed == false)
                EndLoading();

            //m_gateSprite.GetChildAt(1).Rotation += 2;
            //m_gateSprite.GetChildAt(2).Rotation -= 2;

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_gateSprite.GetChildAt(1).Rotation += (120 * elapsedSeconds);
            m_gateSprite.GetChildAt(2).Rotation -= (120 * elapsedSeconds);

            if (m_shakeScreen == true)
                UpdateShake();

            base.Update(gameTime);
        }

        public void EndLoading()
        {
            m_isLoading = false;
            ScreenManager manager = ScreenManager;

            foreach (Screen screen in ScreenManager.GetScreens())
            {
                if (screen != this)
                    manager.RemoveScreen(screen, true);
                else
                    manager.RemoveScreen(screen, false); // Don't dispose the loading screen just yet.
            }

            this.ScreenManager = manager; // Since RemoveScreen removes the reference to the ScreenManager, we need to re-add the reference (for now).
            m_levelToLoad.DrawIfCovered = true;

            if (m_screenTypeToLoad == ScreenType.StartingRoom)
            {
                // Only display the skill screen if the player has died before.
                if (Game.PlayerStats.IsDead == true)
                    (m_levelToLoad as ProceduralLevelScreen).DisableRoomOnEnter = true; // We don't want to call the levelscreens' OnEnter while the skill screen is being added.
                ScreenManager.AddScreen(m_levelToLoad, PlayerIndex.One);
                if (Game.PlayerStats.IsDead == true)
                {
                    ScreenManager.AddScreen((ScreenManager as RCScreenManager).SkillScreen, PlayerIndex.One);
                    (m_levelToLoad as ProceduralLevelScreen).DisableRoomOnEnter = false;
                }
                m_levelToLoad.UpdateIfCovered = false; // We don't want the levelscreen to update while the skillscreen is being added.
            }
            else
            {
                ScreenManager.AddScreen(m_levelToLoad, PlayerIndex.One);
                m_levelToLoad.UpdateIfCovered = true; // This is needed so that the screen still plays when the screen is swiping or gate is rising.
            }

            ScreenManager.AddScreen(this, PlayerIndex.One);
            (ScreenManager as RCScreenManager).LoadingComplete(m_screenTypeToLoad);

            (ScreenManager as RCScreenManager).PerformCleanUp();
            AddFinalTransition();
        }

        public void AddFinalTransition()
        {
            if (m_screenTypeToLoad == ScreenType.TitleWhite)
            {
                BackBufferOpacity = 1;
                Tween.To(this, 2f, Tween.EaseNone, "BackBufferOpacity", "0");
                Tween.AddEndHandlerToLastTween(ScreenManager, "RemoveScreen", this, true);
            }
            else
            {
                if (m_wipeTransition == false)
                {
                    SoundManager.PlaySound("GateRise");
                    Tween.To(m_gateSprite, 1f, Tween.EaseNone, "Y", (-m_gateSprite.Height).ToString());
                    Tween.AddEndHandlerToLastTween(ScreenManager, "RemoveScreen", this, true);
                }
                else
                {
                    m_blackTransitionOut.Y = -500;
                    Tween.By(m_blackTransitionIn, 0.2f, Tween.EaseNone, "X", (-m_blackTransitionIn.Bounds.Width).ToString());
                    Tween.By(m_blackScreen, 0.2f, Tween.EaseNone, "X", (-m_blackTransitionIn.Bounds.Width).ToString());
                    Tween.By(m_blackTransitionOut, 0.2f, Tween.EaseNone, "X", (-m_blackTransitionIn.Bounds.Width).ToString());
                    Tween.AddEndHandlerToLastTween(ScreenManager, "RemoveScreen", this, true);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            if (m_screenTypeToLoad != ScreenType.TitleWhite)
            {
                if (m_wipeTransition == false)
                {
                    m_gateSprite.Draw(Camera);
                    m_effectPool.Draw(Camera);
                    m_loadingText.Position = new Vector2(m_gateSprite.X + 995, m_gateSprite.Y + 540);
                    m_loadingText.Draw(Camera);
                }
                else
                {
                    m_blackTransitionIn.Draw(Camera);
                    m_blackTransitionOut.Draw(Camera);
                    m_blackScreen.Draw(Camera);
                }
            }

            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.White * BackBufferOpacity);

            Camera.End();

            base.Draw(gameTime);
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

        public void UpdateShake()
        {
            if (m_horizontalShake == true)
                m_gateSprite.X = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0,1) * m_screenShakeMagnitude);
            
            if (m_verticalShake == true)
                m_gateSprite.Y = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0,1) * m_screenShakeMagnitude);
        }

        		
		public void StopScreenShake()
		{
			m_shakeScreen = false;
            m_gateSprite.X = 0;
            m_gateSprite.Y = 0;
		}

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Loading Screen");

                m_loadingText.Dispose();
                m_loadingText = null;
                m_levelToLoad = null;
                m_gateSprite.Dispose();
                m_gateSprite = null;
                m_effectPool.Dispose();
                m_effectPool = null;

                m_blackTransitionIn.Dispose();
                m_blackTransitionIn = null;
                m_blackScreen.Dispose();
                m_blackScreen = null;
                m_blackTransitionOut.Dispose();
                m_blackTransitionOut = null;
                base.Dispose();
            }
        }
    }
}
