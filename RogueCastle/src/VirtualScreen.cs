using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpriteSystem;
using DS2DEngine;

namespace RogueCastle
{
    class VirtualScreen
    {
        public readonly int VirtualWidth;
        public readonly int VirtualHeight;
        public readonly float VirtualAspectRatio;

        private GraphicsDevice graphicsDevice;
        private RenderTarget2D screen;

        public VirtualScreen(int virtualWidth, int virtualHeight, GraphicsDevice graphicsDevice)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
            VirtualAspectRatio = (float)(virtualWidth) / (float)(virtualHeight);

            this.graphicsDevice = graphicsDevice;
            //screen = new RenderTarget2D(graphicsDevice, virtualWidth, virtualHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat, graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
            screen = new RenderTarget2D(graphicsDevice, virtualWidth, virtualHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        public void ReinitializeRTs(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            if (screen.IsDisposed == false)
            {
                screen.Dispose();
                screen = null;
            }
            //screen = new RenderTarget2D(graphicsDevice, VirtualWidth, VirtualHeight, false, graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat, graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
            screen = new RenderTarget2D(graphicsDevice, VirtualWidth, VirtualHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

        }

        private bool areaIsDirty = true;

        public void PhysicalResolutionChanged()
        {
            areaIsDirty = true;
        }

        private Rectangle area;

        public void Update()
        {
            if (!areaIsDirty)
            {
                return;
            }

            areaIsDirty = false;
            var physicalWidth = graphicsDevice.Viewport.Width;
            var physicalHeight = graphicsDevice.Viewport.Height;
            var physicalAspectRatio = graphicsDevice.Viewport.AspectRatio;

            // This 'if' was commented out during Switch development, flibit added it back
            if ((int)(physicalAspectRatio * 10) == (int)(VirtualAspectRatio * 10))
            {
                area = new Rectangle(0, 0, physicalWidth, physicalHeight);
                return;
            }

            if (VirtualAspectRatio > physicalAspectRatio)
            {
                var scaling = (float)physicalWidth / (float)VirtualWidth;
                var width = (float)(VirtualWidth) * scaling;
                var height = (float)(VirtualHeight) * scaling;
                var borderSize = (int)((physicalHeight - height) / 2);
                area = new Rectangle(0, borderSize, (int)width, (int)height);
            }
            else
            {
                var scaling = (float)physicalHeight / (float)VirtualHeight;
                var width = (float)(VirtualWidth) * scaling;
                var height = (float)(VirtualHeight) * scaling;
                var borderSize = (int)((physicalWidth - width) / 2);
                area = new Rectangle(borderSize, 0, (int)width, (int)height);
            }
        }

        public void RecreateGraphics()
        {
            Console.WriteLine("GraphicsDevice Virtualization failed");

            GraphicsDevice newDevice = (Game.ScreenManager.Game as Game).graphics.GraphicsDevice;
            Game.ScreenManager.ReinitializeCamera(newDevice);
            SpriteLibrary.ClearLibrary();
            (Game.ScreenManager.Game as Game).LoadAllSpriteFonts();
            (Game.ScreenManager.Game as Game).LoadAllEffects();
            (Game.ScreenManager.Game as Game).LoadAllSpritesheets();

            if (Game.GenericTexture.IsDisposed == false)
                Game.GenericTexture.Dispose();
            Game.GenericTexture = new Texture2D(newDevice, 1, 1);
            Game.GenericTexture.SetData(new Color[] { Color.White });

            Game.ScreenManager.ReinitializeContent(null, null);
        }

        public void BeginCapture()
        {
            // XNA failed to properly reinitialize GraphicsDevice in virtualization. Time to recreate graphics device.
            if (graphicsDevice.IsDisposed)
                RecreateGraphics();

            graphicsDevice.SetRenderTarget(screen);
        }

        public void EndCapture()
        {
            graphicsDevice.SetRenderTarget(null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if ((Game.ScreenManager.CurrentScreen is SkillScreen == false) && (Game.ScreenManager.CurrentScreen is LineageScreen == false) && 
                (Game.ScreenManager.CurrentScreen is SkillUnlockScreen == false) && (Game.ScreenManager.GetLevelScreen() != null) &&
                (Game.PlayerStats.Traits.X == TraitType.Vertigo || Game.PlayerStats.Traits.Y == TraitType.Vertigo) && Game.PlayerStats.SpecialItem != SpecialItemType.Glasses)
                spriteBatch.Draw(screen, area, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            else
                spriteBatch.Draw(screen, area, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public RenderTarget2D RenderTarget
        {
            get { return screen; }
        }

    }
}
