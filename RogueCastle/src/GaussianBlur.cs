//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;

namespace RogueCastle
{
    /// <summary>
    /// The RADIUS constant in the effect file must match the radius value in
    /// the GaussianBlur class. The effect file's weights global variable
    /// corresponds to the GaussianBlur class' kernel field. The effect file's
    /// offsets global variable corresponds to the GaussianBlur class'
    /// offsetsHoriz and offsetsVert fields.
    /// </para>
    /// </summary>
    public class GaussianBlur
    {
        private Effect effect;
        private int radius;
        private float amount;
        private float sigma;
        private float[] kernel;
        private Vector2[] offsetsHoriz;
        private Vector2[] offsetsVert;

        private RenderTarget2D m_renderHolder, m_renderHolder2;

        private bool m_invertMask;
        private EffectParameter m_offsetParameters;

        /// <summary>
        /// Returns the radius of the Gaussian blur filter kernel in pixels.
        /// </summary>
        public int Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                ComputeKernel();
            }
        }

        /// <summary>
        /// Returns the blur amount. This value is used to calculate the
        /// Gaussian blur filter kernel's sigma value. Good values for this
        /// property are 2 and 3. 2 will give a more blurred result whilst 3
        /// will give a less blurred result with sharper details.
        /// </summary>
        public float Amount
        {
            get { return amount; }
            set
            {
                amount = value;
                ComputeKernel();
            }
        }

        /// <summary>
        /// Returns the Gaussian blur filter's standard deviation.
        /// </summary>
        public float Sigma
        {
            get { return sigma; }
        }

        /// <summary>
        /// Returns the Gaussian blur filter kernel matrix. Note that the
        /// kernel returned is for a 1D Gaussian blur filter kernel matrix
        /// intended to be used in a two pass Gaussian blur operation.
        /// </summary>
        public float[] Kernel
        {
            get { return kernel; }
        }

        /// <summary>
        /// Returns the texture offsets used for the horizontal Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsX
        {
            get { return offsetsHoriz; }
        }

        /// <summary>
        /// Returns the texture offsets used for the vertical Gaussian blur
        /// pass.
        /// </summary>
        public Vector2[] TextureOffsetsY
        {
            get { return offsetsVert; }
        }

        public bool InvertMask
        {
            get { return m_invertMask; }
            set
            {
                m_invertMask = value;
                effect.Parameters["invert"].SetValue(m_invertMask);
            }
        }

        /// <summary>
        /// Default constructor for the GaussianBlur class. This constructor
        /// should be called if you don't want the GaussianBlur class to use
        /// its GaussianBlur.fx effect file to perform the two pass Gaussian
        /// blur operation.
        /// </summary>
        public GaussianBlur()
        {
        }

        /// <summary>
        /// This overloaded constructor instructs the GaussianBlur class to
        /// load and use its GaussianBlur.fx effect file that implements the
        /// two pass Gaussian blur operation on the GPU. The effect file must
        /// be already bound to the asset name: 'Effects\GaussianBlur' or
        /// 'GaussianBlur'.
        /// </summary>
        /// 
        public GaussianBlur(Game game, int screenWidth, int screenHeight)
        {
            if (m_renderHolder != null && m_renderHolder.IsDisposed == false)
                m_renderHolder.Dispose();
            if (m_renderHolder2 != null && m_renderHolder2.IsDisposed == false)
                m_renderHolder2.Dispose();

            if (LevelEV.SAVE_FRAMES == true)
            {
                m_renderHolder = new RenderTarget2D(game.GraphicsDevice, screenWidth / 2, screenHeight / 2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                m_renderHolder2 = new RenderTarget2D(game.GraphicsDevice, screenWidth / 2, screenHeight / 2, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            }
            else
            {
                m_renderHolder = new RenderTarget2D(game.GraphicsDevice, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                m_renderHolder2 = new RenderTarget2D(game.GraphicsDevice, screenWidth, screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            }

            effect = game.Content.Load<Effect>(@"Shaders\GaussianBlurMask");
            m_offsetParameters = effect.Parameters["offsets"];
        }

        /// <summary>
        /// Calculates the Gaussian blur filter kernel. This implementation is
        /// ported from the original Java code appearing in chapter 16 of
        /// "Filthy Rich Clients: Developing Animated and Graphical Effects for
        /// Desktop Java".
        /// </summary>
        /// <param name="blurRadius">The blur radius in pixels.</param>
        /// <param name="blurAmount">Used to calculate sigma.</param>
        public void ComputeKernel()
        {
            kernel = null;
            kernel = new float[radius * 2 + 1];
            sigma = radius / amount;

            // flibit added this to avoid sending NaN to the shader
            if (radius == 0)
                return;

            float twoSigmaSquare = 2.0f * sigma * sigma;
            float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0.0f;
            float distance = 0.0f;
            int index = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                distance = i * i;
                index = i + radius;
                kernel[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += kernel[index];
            }

            for (int i = 0; i < kernel.Length; ++i)
                kernel[i] /= total;
            effect.Parameters["weights"].SetValue(kernel);
        }

        /// <summary>
        /// Calculates the texture coordinate offsets corresponding to the
        /// calculated Gaussian blur filter kernel. Each of these offset values
        /// are added to the current pixel's texture coordinates in order to
        /// obtain the neighboring texture coordinates that are affected by the
        /// Gaussian blur filter kernel. This implementation has been adapted
        /// from chapter 17 of "Filthy Rich Clients: Developing Animated and
        /// Graphical Effects for Desktop Java".
        /// </summary>
        /// <param name="textureWidth">The texture width in pixels.</param>
        /// <param name="textureHeight">The texture height in pixels.</param>
        public void ComputeOffsets()
        {
            offsetsHoriz = null;
            offsetsHoriz = new Vector2[radius * 2 + 1];

            offsetsVert = null;
            offsetsVert = new Vector2[radius * 2 + 1];

            int index = 0;
            float xOffset = 1.0f / m_renderHolder.Width;
            float yOffset = 1.0f / m_renderHolder.Height;

            for (int i = -radius; i <= radius; ++i)
            {
                index = i + radius;
                offsetsHoriz[index] = new Vector2(i * xOffset, 0.0f);
                offsetsVert[index] = new Vector2(0.0f, i * yOffset);
            }
        }

        /// <summary>
        /// Performs the Gaussian blur operation on the source texture image.
        /// The Gaussian blur is performed in two passes: a horizontal blur
        /// pass followed by a vertical blur pass. The output from the first
        /// pass is rendered to renderTarget1. The output from the second pass
        /// is rendered to renderTarget2. The dimensions of the blurred texture
        /// is therefore equal to the dimensions of renderTarget2.
        /// </summary>
        /// <param name="srcTexture">The source image to blur.</param>
        /// <param name="renderTarget1">Stores the output from the horizontal blur pass.</param>
        /// <param name="renderTarget2">Stores the output from the vertical blur pass.</param>
        /// <param name="spriteBatch">Used to draw quads for the blur passes.</param>
        /// <returns>The resulting Gaussian blurred image.</returns>
        public void Draw(RenderTarget2D srcTexture, Camera2D Camera, RenderTarget2D mask = null)
        {
            if (effect == null)
                throw new InvalidOperationException("GaussianBlur.fx effect not loaded.");

            // Perform horizontal Gaussian blur.

            Camera.GraphicsDevice.SetRenderTarget(m_renderHolder);

            //effect.Parameters["weights"].SetValue(kernel);
            //effect.Parameters["offsets"].SetValue(offsetsVert);
            m_offsetParameters.SetValue(offsetsHoriz);

            if (mask != null)
            {
                Camera.GraphicsDevice.Textures[1] = mask;
                Camera.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            }
            Camera.Begin(0, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);
            //Camera.Draw(srcTexture, Vector2.Zero, Color.White);
            if (LevelEV.SAVE_FRAMES == true)
                Camera.Draw(srcTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1);
            else
                Camera.Draw(srcTexture, Vector2.Zero, Color.White);
            Camera.End();


            if (LevelEV.SAVE_FRAMES == true)
            {
                // Perform vertical Gaussian blur.
                Camera.GraphicsDevice.SetRenderTarget(m_renderHolder2);

                m_offsetParameters.SetValue(offsetsVert);

                if (mask != null)
                    Camera.GraphicsDevice.Textures[1] = mask;
                Camera.Begin(0, BlendState.Opaque, null, null, null, effect);
                Camera.Draw(m_renderHolder, Vector2.Zero, Color.White);
                Camera.End();

                Camera.GraphicsDevice.SetRenderTarget(srcTexture);
                Camera.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
                Camera.Draw(m_renderHolder2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 1);
                Camera.End();
            }
            else
            {
                // Perform vertical Gaussian blur.
                Camera.GraphicsDevice.SetRenderTarget(srcTexture);

                m_offsetParameters.SetValue(offsetsVert);

                if (mask != null)
                    Camera.GraphicsDevice.Textures[1] = mask;
                Camera.Begin(0, BlendState.Opaque, null, null, null, effect);
                Camera.Draw(m_renderHolder, Vector2.Zero, Color.White);
                Camera.End();
            }
        }
    }
}
