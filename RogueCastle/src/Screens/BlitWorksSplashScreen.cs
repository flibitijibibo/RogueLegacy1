using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class BlitWorksSplashScreen : Screen
    {
        private SpriteObj m_logo;
        private SpriteObj m_logoIcon;
        private SoundEffect m_blitSFX;

        public override void LoadContent()
        {
            m_logo = new SpriteObj("Blitworks_FullLogo");
            m_logo.Position = new Vector2(GlobalEV.ScreenWidth / 2, GlobalEV.ScreenHeight / 2);
            m_logo.ForceDraw = true;

            m_logoIcon = new SpriteObj("Blitworks_IconOnly");
            m_logoIcon.Position = new Vector2(GlobalEV.ScreenWidth / 2, GlobalEV.ScreenHeight / 2 - (40 * m_logo.ScaleX));
            m_logoIcon.ForceDraw = true;

            m_blitSFX = ScreenManager.Game.Content.Load<SoundEffect>("Audio/sfx_blitworks_logo");

            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_logo.Opacity = 0;
            m_logoIcon.Opacity = 0;

            Tween.RunFunction(0.5f, this, "PlayBlitSplash");
            base.OnEnter();
        }

        public void PlayBlitSplash()
        {
            m_blitSFX.Play(0.25f, 0, 0);
            m_logoIcon.Opacity = 1;
            Tween.To(m_logo, 0.25f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_logoIcon, 0.5f, Tween.EaseNone, "Opacity", "0", "ScaleX", "2", "ScaleY", "2");
            m_logo.Opacity = 1;
            Tween.To(m_logo, 0.25f, Tween.EaseNone, "delay", "2.5", "Opacity", "0");
            m_logo.Opacity = 0;
            Tween.AddEndHandlerToLastTween(this, "LoadNextScreen");
        }

        public void LoadNextScreen()
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin();
            m_logo.Draw(Camera);
            m_logoIcon.Draw(Camera);
            Camera.End();
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Blitworks Splash Screen");

                m_logo.Dispose();
                m_logo = null;
                m_logoIcon.Dispose();
                m_logoIcon = null;
                m_blitSFX.Dispose();
                m_blitSFX = null;
                base.Dispose();
            }
        }
    }
}
