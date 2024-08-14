using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class DeathDefiedScreen : Screen
    {
        private PlayerObj m_player;
        private SpriteObj m_spotlight;

        private SpriteObj m_titlePlate;
        private SpriteObj m_title;

        public float BackBufferOpacity { get; set; }
        private Vector2 m_cameraPos;

        private string m_songName;
        private float m_storedMusicVolume;

        public DeathDefiedScreen()
        {
        }

        public override void LoadContent()
        {
            Vector2 shadowOffset = new Vector2(2, 2);
            Color textColour = new Color(255, 254, 128);

            m_spotlight = new SpriteObj("GameOverSpotlight_Sprite");
            m_spotlight.Rotation = 90;
            m_spotlight.ForceDraw = true;
            m_spotlight.Position = new Vector2(1320 / 2, 40 + m_spotlight.Height);

            m_titlePlate = new SpriteObj("SkillUnlockTitlePlate_Sprite");
            m_titlePlate.Position = new Vector2(1320 / 2f, 720 / 2f - 200);
            m_titlePlate.ForceDraw = true;

            m_title = new SpriteObj("DeathDefyText_Sprite");
            m_title.Position = m_titlePlate.Position;
            m_title.Y -= 40;
            m_title.ForceDraw = true;

            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_cameraPos = new Vector2(Camera.X, Camera.Y);

            if (m_player == null)
                m_player = (ScreenManager as RCScreenManager).Player;

            m_titlePlate.Scale = Vector2.Zero;
            m_title.Scale = Vector2.Zero;
            m_title.Opacity = 1;
            m_titlePlate.Opacity = 1;

            m_storedMusicVolume = SoundManager.GlobalMusicVolume;
            m_songName = SoundManager.GetCurrentMusicName();
            Tween.To(typeof(SoundManager), 1, Tween.EaseNone, "GlobalMusicVolume", (m_storedMusicVolume * 0.1f).ToString());
            SoundManager.PlaySound("Player_Death_FadeToBlack");

            m_player.Visible = true;
            m_player.Opacity = 1;

            m_spotlight.Opacity = 0;

            // Spotlight, Player slain text, and Backbuffer animation.
            Tween.To(this, 0.5f, Linear.EaseNone, "BackBufferOpacity", "1");
            Tween.To(m_spotlight, 0.1f, Linear.EaseNone, "delay", "1", "Opacity", "1");
            Tween.AddEndHandlerToLastTween(typeof(SoundManager), "PlaySound", "Player_Death_Spotlight");
            Tween.To(Camera, 1, Quad.EaseInOut, "X", this.m_player.AbsX.ToString(), "Y", (this.m_player.Bounds.Bottom - 10).ToString(), "Zoom", "1");
            Tween.RunFunction(2f, this, "PlayerLevelUpAnimation");

            Game.ChangeBitmapLanguage(m_title, "DeathDefyText_Sprite");

            base.OnEnter();
        }

        public void PlayerLevelUpAnimation()
        {
            m_player.ChangeSprite("PlayerLevelUp_Character");
            m_player.PlayAnimation(false);

            Tween.To(m_titlePlate, 0.5f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_title, 0.5f, Back.EaseOut, "delay", "0.1", "ScaleX", "0.8", "ScaleY", "0.8");
            Tween.RunFunction(0.1f, typeof(SoundManager), "PlaySound", "GetItemStinger3");
            Tween.RunFunction(2, this, "ExitTransition");
        }

        public void ExitTransition()
        {
            Tween.To(typeof(SoundManager), 1, Tween.EaseNone, "GlobalMusicVolume", m_storedMusicVolume.ToString());

            Tween.To(Camera, 1, Quad.EaseInOut, "X", m_cameraPos.X.ToString(), "Y", m_cameraPos.Y.ToString());
            Tween.To(m_spotlight, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_titlePlate, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_title, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.To(this, 0.2f, Tween.EaseNone, "delay", "1", "BackBufferOpacity", "0");
            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        public override void Draw(GameTime gameTime)
        {
            //Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformation()); // Parallax Effect has been disabled in favour of ripple effect for now.
            Camera.Draw(Game.GenericTexture, new Rectangle((int)Camera.TopLeftCorner.X - 10, (int)Camera.TopLeftCorner.Y - 10, 1340, 740), Color.Black * BackBufferOpacity);

            m_player.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null); // Parallax Effect has been disabled in favour of ripple effect for now.
            m_spotlight.Draw(Camera);
            m_titlePlate.Draw(Camera);
            m_title.Draw(Camera);
            Camera.End();

            base.Draw(gameTime);
        }

        public override void RefreshTextObjs()
        {
            Game.ChangeBitmapLanguage(m_title, "DeathDefyText_Sprite");

            base.RefreshTextObjs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Death Defied Screen");

                m_player = null;
                m_spotlight.Dispose();
                m_spotlight = null;

                m_title.Dispose();
                m_title = null;
                m_titlePlate.Dispose();
                m_titlePlate = null;

                base.Dispose();
            }
        }
    }
}
