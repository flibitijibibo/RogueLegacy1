using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using System.Threading;

namespace RogueCastle
{
    public class CDGSplashScreen : Screen
    {
        private SpriteObj m_logo;
        private TextObj m_loadingText;
        private bool m_levelDataLoaded = false;
        private bool m_fadingOut = false;
        private float m_totalElapsedTime = 0;

        public override void LoadContent()
        {
            m_logo = new SpriteObj("CDGLogo_Sprite");
            m_logo.Position = new Vector2(1320 / 2, 720 / 2);
            m_logo.Rotation = 90;
            m_logo.ForceDraw = true;

            m_loadingText = new TextObj(Game.JunicodeFont);
            m_loadingText.FontSize = 18;
            m_loadingText.Align = Types.TextAlign.Right;
            m_loadingText.Text = LocaleBuilder.getString("LOC_ID_SPLASH_SCREEN_1", m_loadingText);
            m_loadingText.TextureColor = new Color(100, 100, 100);
            m_loadingText.Position = new Vector2(1320 - 40, 720 - 90);
            m_loadingText.ForceDraw = true;
            m_loadingText.Opacity = 0;
            base.LoadContent();
        }

        public override void OnEnter()
        {
            // Level data loading is multithreaded.
            m_levelDataLoaded = false;
            m_fadingOut = false;
            Thread loadingThread = new Thread(LoadLevelData);
            loadingThread.Start();

            m_logo.Opacity = 0;
            Tween.To(m_logo, 1, Linear.EaseNone, "delay", "0.5", "Opacity", "1");
            Tween.RunFunction(0.75f, typeof(SoundManager), "PlaySound", "CDGSplashCreak");
            //Tween.AddEndHandlerToLastTween(typeof(SoundManager), "PlaySound", "CDGSplashCreak");
            base.OnEnter();
        }

        private void LoadLevelData()
        {
            lock (this)
            {
                LevelBuilder2.Initialize();
                LevelParser.ParseRooms("Map_1x1", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_1x2", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_1x3", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_2x1", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_2x2", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_2x3", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_3x1", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_3x2", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_Special", ScreenManager.Game.Content);
                LevelParser.ParseRooms("Map_DLC1", ScreenManager.Game.Content, true);
                LevelBuilder2.IndexRoomList();
                m_levelDataLoaded = true;
            }
        }

        public void LoadNextScreen()
        {
            if (LevelEV.ENABLE_BLITWORKS_SPLASH == true)
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.BlitWorks, true, null);
            else
            {
                if ((ScreenManager.Game as Game).SaveManager.FileExists(SaveType.PlayerData))
                {
                    //try
                    //{
                    (ScreenManager.Game as Game).SaveManager.LoadFiles(null, SaveType.PlayerData);

                    if (Game.PlayerStats.ShoulderPiece < 1 || Game.PlayerStats.HeadPiece < 1 || Game.PlayerStats.ChestPiece < 1)
                        Game.PlayerStats.TutorialComplete = false;
                    else
                    {
                        if (Game.PlayerStats.TutorialComplete == false)
                            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.TutorialRoom, true, null);
                        else
                            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Title, true, null);
                    }
                    //}
                    //catch
                    //{
                    //    Console.WriteLine(" Could not load Player data.");
                    //    Game.PlayerStats.TutorialComplete = false;
                    //}
                }
                else
                {
                    if (Game.PlayerStats.TutorialComplete == false)
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.TutorialRoom, true, null);
                    else
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Title, true, null);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_levelDataLoaded == false && m_logo.Opacity == 1)
            {
                float opacity = (float)Math.Abs(Math.Sin(m_totalElapsedTime));
                m_totalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_loadingText.Opacity = opacity;
            }

            if (m_levelDataLoaded == true && m_fadingOut == false)
            {
                m_fadingOut = true;
                float logoOpacity = m_logo.Opacity;
                m_logo.Opacity = 1;
                Tween.To(m_logo, 1, Linear.EaseNone, "delay", "1.5", "Opacity", "0");
                Tween.AddEndHandlerToLastTween(this, "LoadNextScreen");
                Tween.To(m_loadingText, 0.5f, Tween.EaseNone, "Opacity", "0");
                m_logo.Opacity = logoOpacity;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin();
            m_logo.Draw(Camera);
            m_loadingText.Draw(Camera);
            Camera.End();
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing CDG Splash Screen");

                m_logo.Dispose();
                m_logo = null;
                m_loadingText.Dispose();
                m_loadingText = null;
                base.Dispose();
            }
        }
    }
}
