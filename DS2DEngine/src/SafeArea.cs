using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    /// <summary>
    /// Credit goes to Manders (on internet) for the basis of this code.
    /// 
    /// A helper class to indicate the "safe area" for rendering to a 
    /// television. The inner 90% of the viewport is the "action safe" area, 
    /// meaning all important "action" should be shown within this area. The 
    /// inner 80% of the viewport is the "title safe area", meaning all text 
    /// and other key information should be shown within in this area. This 
    /// class shows the area that is not "title safe" in yellow, and the area 
    /// that is not "action safe" in red.
    /// </summary>
    public class SafeArea : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Texture2D tex; // Holds a 1x1 texture containing a single white texel
        int width; // Viewport width
        int height; // Viewport height
        int dx; // 5% of width
        int dy; // 5% of height
        Color notActionSafeColor = new Color(255, 0, 0, 127); // Red, 50% opacity
        Color notTitleSafeColor = new Color(255, 255, 0, 127); // Yellow, 50% opacity

        public SafeArea(Game game)
            : base(game)
        {
            //this.DrawOrder = (int)(DrawLevel.SafeArea);
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enabled = false; // No need to call update for this
            this.Visible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(base.GraphicsDevice);
            tex = new Texture2D(base.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] texData = new Color[1];
            texData[0] = Color.White;
            tex.SetData<Color>(texData);
            width = base.GraphicsDevice.Viewport.Width;
            height = base.GraphicsDevice.Viewport.Height;
            dx = (int)(width * 0.05);
            dy = (int)(height * 0.05);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Consts.DEBUG_ShowSafeZones == true)
            {
                spriteBatch.Begin();

                // Tint the non-action-safe area red
                spriteBatch.Draw(tex, new Rectangle(0, 0, width, dy), notActionSafeColor);
                spriteBatch.Draw(tex, new Rectangle(0, height - dy, width, dy), notActionSafeColor);
                spriteBatch.Draw(tex, new Rectangle(0, dy, dx, height - 2 * dy), notActionSafeColor);
                spriteBatch.Draw(tex, new Rectangle(width - dx, dy, dx, height - 2 * dy), notActionSafeColor);

                // Tint the non-title-safe area yellow
                spriteBatch.Draw(tex, new Rectangle(dx, dy, width - 2 * dx, dy), notTitleSafeColor);
                spriteBatch.Draw(tex, new Rectangle(dx, height - 2 * dy, width - 2 * dx, dy), notTitleSafeColor);
                spriteBatch.Draw(tex, new Rectangle(dx, 2 * dy, dx, height - 4 * dy), notTitleSafeColor);
                spriteBatch.Draw(tex, new Rectangle(width - 2 * dx, 2 * dy, dx, height - 4 * dy), notTitleSafeColor);
                spriteBatch.End();
            }
        }
    }
}
