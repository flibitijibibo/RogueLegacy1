using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class BasePostProcessingEffect
    {
        public Vector2 HalfPixel;
        public Texture2D lastScene;
        public Texture2D orgScene;
        protected List<BasePostProcess> postProcesses = new List<BasePostProcess>();

        protected Game Game;

        public BasePostProcessingEffect(Game game)
        {
            Game = game;
        }

        public bool Enabled = true;

        public void AddPostProcess(BasePostProcess postProcess)
        {
            postProcesses.Add(postProcess);
        }

        public virtual void Draw(GameTime gameTime, Texture2D scene)
        {
            if (!Enabled)
                return;

            orgScene = scene;

            int maxProcess = postProcesses.Count;
            lastScene = null;

            for (int p = 0; p < maxProcess; p++)
            {
                if (postProcesses[p].Enabled)
                {
                    // Set Half Pixel value.
                    postProcesses[p].HalfPixel = HalfPixel;

                    // Set original scene
                    postProcesses[p].orgBuffer = orgScene;

                    // Ready render target if needed.
                    if (postProcesses[p].newScene == null)
                        postProcesses[p].newScene = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width/2, Game.GraphicsDevice.Viewport.Height/2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                    Game.GraphicsDevice.SetRenderTarget(postProcesses[p].newScene);

                    // Has the scene been rendered yet (first effect may be disabled)
                    if (lastScene == null)
                        lastScene = orgScene;

                    postProcesses[p].BackBuffer = lastScene;

                    Game.GraphicsDevice.Textures[0] = postProcesses[p].BackBuffer;
                    //Game.GraphicsDevice.Clear(Color.White); // Added this to make the ray white.
                    postProcesses[p].Draw(gameTime);

                    Game.GraphicsDevice.SetRenderTarget(null);

                    lastScene = postProcesses[p].newScene;
                }
            }

            if (lastScene == null)
                lastScene = scene;
        }
    }
}
