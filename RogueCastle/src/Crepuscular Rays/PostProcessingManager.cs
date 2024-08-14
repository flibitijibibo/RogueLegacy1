using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class PostProcessingManager
    {
        protected Game Game;
        public Texture2D Scene;

        public RenderTarget2D newScene;

        protected List<BasePostProcessingEffect> postProcessingEffects = new List<BasePostProcessingEffect>();

        public Vector2 HalfPixel;

        private SpriteBatch m_spriteBatch;
        public SpriteBatch spriteBatch
        {
            get { return m_spriteBatch; }
        }

        public PostProcessingManager(Game game, SpriteBatch spriteBatch)
        {
            Game = game;
            m_spriteBatch = spriteBatch;
        }

        public void AddEffect(BasePostProcessingEffect ppEfect)
        {
            postProcessingEffects.Add(ppEfect);
        }

        public virtual void Draw(GameTime gameTime, Texture2D scene)
        {
            HalfPixel = -new Vector2(.5f / (float)Game.GraphicsDevice.Viewport.Width,
                                    .5f / (float)Game.GraphicsDevice.Viewport.Height);

            int maxEffect = postProcessingEffects.Count;

            Scene = scene;
            
            for (int e = 0; e < maxEffect; e++)
            {
                if (postProcessingEffects[e].Enabled)
                {
                    postProcessingEffects[e].HalfPixel = HalfPixel;

                    postProcessingEffects[e].orgScene = scene;
                    postProcessingEffects[e].Draw(gameTime, Scene);
                    Scene = postProcessingEffects[e].lastScene;
                }
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Scene, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
        }

        protected void SaveTexture(Texture2D texture, string name)
        {
            FileStream stream = new FileStream(name, FileMode.Create);
            texture.SaveAsJpeg(stream, texture.Width, texture.Height);
            stream.Close();
        }
    }
}
