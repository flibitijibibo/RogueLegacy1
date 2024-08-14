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
    public class DemoStartScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private TextObj m_text;

        public DemoStartScreen()
        {

        }

        public override void LoadContent()
        {
            m_text = new TextObj(Game.JunicodeLargeFont);
            m_text.FontSize = 20;
            m_text.Text = "This is a demo of Rogue Legacy.\nThere may be bugs, and some assets are missing, but we hope you enjoy it.";
                //"This is a beta demo of Rogue Legacy.\nThough the game is not bug-free and there are missing art and audio assets,\nwe hope you enjoy it.";//"The following is a beta demo of Rogue Legacy.";
            m_text.ForceDraw = true;
            m_text.Position = new Vector2(1320 / 2f - m_text.Width / 2f, 720 / 2f - m_text.Height / 2f - 30);
            base.LoadContent();
        }

        public override void OnEnter()
        {
            BackBufferOpacity = 1;
            Tween.RunFunction(7, ScreenManager, "DisplayScreen", ScreenType.CDGSplash, true, typeof(List<object>));
            base.OnEnter();
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.Black * BackBufferOpacity);

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
                m_text.Dispose();
                m_text = null;
                base.Dispose();
            }
        }
    }
}
