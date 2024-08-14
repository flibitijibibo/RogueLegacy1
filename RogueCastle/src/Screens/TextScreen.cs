using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class TextScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private TextObj m_text;
        private float m_fadeInSpeed;
        private float m_backBufferOpacity;
        private bool m_typewriteText;
        private float m_textDuration = 0;
        private bool m_loadEndingAfterward = false;

        private SpriteObj m_smoke1, m_smoke2, m_smoke3;

        public TextScreen()
        {

        }

        public override void LoadContent()
        {
            Color sepia = new Color(200, 150, 55);

            m_smoke1 = new SpriteObj("TextSmoke_Sprite");
            m_smoke1.ForceDraw = true;
            m_smoke1.Scale = new Vector2(2, 2);
            m_smoke1.Opacity = 0.3f;
            m_smoke1.TextureColor = sepia;

            m_smoke2 = m_smoke1.Clone() as SpriteObj;
            m_smoke2.Flip = SpriteEffects.FlipHorizontally;
            m_smoke2.Opacity = 0.2f;

            m_smoke3 = m_smoke1.Clone() as SpriteObj;
            m_smoke3.Scale = new Vector2(2.5f, 3f);
            m_smoke3.Opacity = 0.15f;

            base.LoadContent();
        }

        public override void PassInData(List<object> objList)
        {
            m_backBufferOpacity = (float)objList[0];
            m_fadeInSpeed = (float)objList[1];
            m_textDuration = (float)objList[2];
            m_typewriteText = (bool)objList[3];

            TextObj textInfo = objList[4] as TextObj;
            if (m_text != null)
            {
                m_text.Dispose();
                m_text = null;
            }
            m_text = textInfo.Clone() as TextObj;
            //textInfo.Dispose(); // Do not dispose this.

            m_loadEndingAfterward = (bool)objList[5];
        }

        public override void OnEnter()
        {
            m_smoke1.Position = new Vector2(CDGMath.RandomInt(300, 1000), m_text.Y + m_text.Height/2f - 30 + CDGMath.RandomInt(-100, 100));
            m_smoke2.Position = new Vector2(CDGMath.RandomInt(200, 700), m_text.Y + m_text.Height / 2f - 30 + CDGMath.RandomInt(-50, 50));
            m_smoke3.Position = new Vector2(CDGMath.RandomInt(300, 800), m_text.Y + m_text.Height / 2f - 30 + CDGMath.RandomInt(-100, 100));

            m_smoke1.Opacity = m_smoke2.Opacity = m_smoke3.Opacity = 0;
            Tween.To(m_smoke1, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0.3");
            Tween.To(m_smoke2, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0.2");
            Tween.To(m_smoke3, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0.15");

            BackBufferOpacity = 0;
            m_text.Opacity = 0;

            Tween.To(this, m_fadeInSpeed, Tween.EaseNone, "BackBufferOpacity", m_backBufferOpacity.ToString());
            Tween.To(m_text, m_fadeInSpeed, Tween.EaseNone, "Opacity", "1");

            if (m_typewriteText == true)
            {
                m_text.Visible = false;
                Tween.RunFunction(m_fadeInSpeed, m_text, "BeginTypeWriting", m_text.Text.Length * 0.05f, "");
            }
            else
                m_text.Visible = true;

            base.OnEnter();
        }

        private void ExitTransition()
        {
            if (m_loadEndingAfterward == false)
            {
                Tween.To(m_smoke1, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0");
                Tween.To(m_smoke2, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0");
                Tween.To(m_smoke3, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0");

                Tween.To(this, m_fadeInSpeed, Tween.EaseNone, "BackBufferOpacity", "0");
                Tween.To(m_text, m_fadeInSpeed, Tween.EaseNone, "Opacity", "0");
                Tween.AddEndHandlerToLastTween(Game.ScreenManager, "HideCurrentScreen");
            }
            else
                Game.ScreenManager.DisplayScreen(ScreenType.Ending, true);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_smoke1.X += 5 * elapsed;
            m_smoke2.X += 15 * elapsed;
            m_smoke3.X += 10 * elapsed;

            if (m_text.Visible == false && m_text.IsTypewriting)
                m_text.Visible = true;

            if (m_textDuration > 0)
            {
                m_textDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_textDuration <= 0)
                    ExitTransition();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.Black * BackBufferOpacity);

            m_smoke1.Draw(Camera);
            m_smoke2.Draw(Camera);
            m_smoke3.Draw(Camera);

            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_text.Draw(Camera);

            Camera.End();
            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Text Screen");
                if (m_text != null)
                    m_text.Dispose();
                m_text = null;

                m_smoke1.Dispose();
                m_smoke1 = null;
                m_smoke2.Dispose();
                m_smoke2 = null;
                m_smoke3.Dispose();
                m_smoke3 = null;
                base.Dispose();
            }
        }
    }
}
