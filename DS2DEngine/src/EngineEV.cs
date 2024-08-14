using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class EngineEV
    {
        public static int ScreenWidth = 800;
        public static int ScreenHeight = 600;

        public static Vector2 ScreenRatio = Vector2.One;

        public static void RefreshEngine(GraphicsDevice graphicsDevice)
        {
            ScreenRatio = new Vector2((float)ScreenWidth / graphicsDevice.Viewport.Width, (float)ScreenHeight / graphicsDevice.Viewport.Height);
        }
    }
}
