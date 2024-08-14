using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class BackgroundObj : SpriteObj
    {
        public Vector2 ParallaxSpeed = Vector2.Zero;
        //public Vector2 ScrollSpeed = Vector2.Zero;
        //public bool Scrollable = false;
        private bool m_repeatHorizontal = false;
        private bool m_repeatVertical = false;
        private RenderTarget2D m_repeatedTexture;
        //private Vector2 m_cameraStartPos = Vector2.Zero;
        private Vector2 m_retainedScale; // The scale that the texture needs to be at to retain its original size (since it was modified to match a power of 2 size).

        private SamplerState m_samplerState;

        public bool isContentLost
        {
            get
            {
                if (m_repeatedTexture == null)
                    return false;
                return m_repeatedTexture.IsContentLost;
            }
        }

        public BackgroundObj(string spriteName)
            : base(spriteName)
        {
            this.ForceDraw = true;
        }

        public void SetRepeated(bool repeatHorizontal, bool repeatVertical, Camera2D camera, SamplerState samplerState = null)
        {
            m_samplerState = samplerState;

            //if (m_repeatedTexture != null && (repeatHorizontal == true && repeatVertical == true))
            if (m_repeatedTexture != null)
            {
                m_repeatedTexture.Dispose();
                m_repeatedTexture = null;
            }

            if (m_repeatedTexture == null && (repeatHorizontal == true || repeatVertical == true))
            {
                m_repeatedTexture = this.ConvertToTexture(camera, true, samplerState);
                m_retainedScale = new Vector2(this.Width / (float)m_repeatedTexture.Width, this.Height / (float)m_repeatedTexture.Height);
            }
            //else if (m_repeatedTexture != null && (repeatHorizontal == true && repeatVertical == true))
            //{
            //    m_repeatedTexture.Dispose();
            //    m_repeatedTexture = null;
            //}
            m_repeatHorizontal = repeatHorizontal;
            m_repeatVertical = repeatVertical;
        }

        public void ChangeSprite(string spriteName, Camera2D camera)
        {
            base.ChangeSprite(spriteName);
            SetRepeated(m_repeatHorizontal, m_repeatVertical, camera, m_samplerState);
        }


        float m_totalXParallax = 0;
        float m_totalYParallax = 0;
        public override void Draw(Camera2D camera)
        {
            //if (Scrollable == true && m_cameraStartPos != Vector2.Zero)
            {
                m_totalXParallax += (this.ParallaxSpeed.X * 60 * camera.ElapsedTotalSeconds);
                m_totalYParallax += (this.ParallaxSpeed.Y * 60 * camera.ElapsedTotalSeconds);

                if (RepeatHorizontal == true)
                    m_totalXParallax = m_totalXParallax % (this.Width * m_retainedScale.X); // Don't multiply by ScaleX because Width already does that.
                if (RepeatVertical == true)
                    m_totalYParallax = m_totalYParallax % (this.Height * m_retainedScale.Y);
            }
           // m_cameraStartPos = camera.Position;
           
            if (RepeatHorizontal == true || RepeatVertical == true)
            {
                float posX = this.X;
                float posY = this.Y;
                int width = (int)(m_repeatedTexture.Width * ScaleX * m_retainedScale.X);
                int height = (int)(m_repeatedTexture.Height * ScaleY * m_retainedScale.Y);
                //int width = (int)(camera.GraphicsDevice.Viewport.Width * (1/(Scale.X * m_retainedScale.X)));
                //int height = (int)(camera.GraphicsDevice.Viewport.Height * (1/(ScaleY * m_retainedScale.Y)));

                // Problem code below
                if (RepeatHorizontal == true)
                {
                    //posX = this.X - camera.GraphicsDevice.Viewport.Width * ScaleX * m_retainedScale.X;
                    //posX = this.X - camera.GraphicsDevice.Viewport.Width;
                    posX = this.X + m_totalXParallax - (m_repeatedTexture.Width * m_retainedScale.X * ScaleX * 5);
                    width = EngineEV.ScreenWidth * 10; //camera.GraphicsDevice.Viewport.Width * 10;
                }
                if (RepeatVertical == true)
                {
                    //posY = this.Y - camera.GraphicsDevice.Viewport.Height * ScaleY * m_retainedScale.Y;
                    posY = this.Y + m_totalYParallax - (m_repeatedTexture.Height * m_retainedScale.Y * ScaleY * 5);
                    height = EngineEV.ScreenHeight * 10; //camera.GraphicsDevice.Viewport.Height * 10;
                }
                /////////////////////
                //camera.Draw(m_repeatedTexture, new Vector2(this.X + -GlobalEV.ScreenWidth, this.Y + -GlobalEV.ScreenHeight), new Rectangle(0, 0, GlobalEV.ScreenWidth * 3, GlobalEV.ScreenHeight * 3), this.TextureColor, 0, Vector2.Zero, this.Scale, SpriteEffects.None, 0);
                camera.Draw(m_repeatedTexture, new Vector2(posX, posY), new Rectangle(0, 0, width, height), this.TextureColor * this.Opacity, this.Rotation, Vector2.Zero, m_retainedScale * Scale, this.Flip, 0);
                //camera.Draw(m_repeatedTexture, this.Position, Color.White);
            }
            else
            {
                base.Draw(camera);
            }
        }

        public bool RepeatHorizontal
        {
            get { return m_repeatHorizontal; }
        }

        public bool RepeatVertical
        {
            get { return m_repeatVertical; }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new BackgroundObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            // This one needs to be fixed.
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                if (m_repeatedTexture != null)
                {
                    m_repeatedTexture.Dispose();
                    m_repeatedTexture = null;
                }
                m_samplerState = null;
                base.Dispose();
            }
        }

        public RenderTarget2D Texture
        {
            get { return m_repeatedTexture; }
        }
    }
}
