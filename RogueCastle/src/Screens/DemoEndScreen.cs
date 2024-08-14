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
    public class DemoEndScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private TextObj m_text;
        private SpriteObj m_playerShrug;

        public DemoEndScreen()
        {

        }

        public override void LoadContent()
        {
            m_text = new TextObj(Game.JunicodeLargeFont);
            m_text.FontSize = 20;
            m_text.Text = "Thanks for playing the Rogue Legacy Demo. You're pretty good at games.";//"Hey dudes, thanks for beating the demo.\nLet's go for a burger....\n Ha! Ha! Ha! Ha!";//"Thank you for playing the Rogue Legacy demo.\nPlease show your support, and help spread the word.";//"Thanks for playing the demo of Rogue Legacy.  That's it for now.";
            m_text.ForceDraw = true;
            m_text.Position = new Vector2(1320 / 2f - m_text.Width / 2f, 720 / 2f - m_text.Height / 2f - 30);

            m_playerShrug = new SpriteObj("PlayerShrug_Sprite");
            m_playerShrug.ForceDraw = true;
            m_playerShrug.Position = new Vector2(1320 / 2f, m_text.Bounds.Bottom + 100);
            m_playerShrug.Scale = new Vector2(3, 3);
            base.LoadContent();
        }

        public override void OnEnter()
        {
            BackBufferOpacity = 1;
            Tween.RunFunction(8, ScreenManager, "DisplayScreen", ScreenType.Title, true, typeof(List<object>));
            base.OnEnter();
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.Black * BackBufferOpacity);

            m_playerShrug.Draw(Camera);

            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_text.Draw(Camera);

            Camera.End();
            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_playerShrug.Dispose();
                m_playerShrug = null;

                m_text.Dispose();
                m_text = null;
                base.Dispose();
            }
        }
    }
}
