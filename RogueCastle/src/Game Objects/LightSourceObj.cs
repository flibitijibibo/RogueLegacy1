using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class LightSourceObj : SpriteObj
    {
        private float m_growthRate = 0;
        private float m_growthDifference = 0f;

        public LightSourceObj()
            : base("LightSource_Sprite")
        {
            m_growthRate = 0.7f + CDGMath.RandomFloat(-0.1f, 0.1f);
            m_growthDifference = 0.05f + CDGMath.RandomFloat(0, 0.05f); ;
            this.Opacity = 1;// 0.35f;
            //this.TextureColor = new Color(187, 159, 91, 255);
        }

        public override void Draw(Camera2D camera)
        {
            //if (m_storedScale == Vector2.Zero)
            //{
            //    m_storedScale = this.Scale;
            //    m_scaleCounter = m_storedScale.X;
            //}

            //float elapsedGameTime = (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
            //if (m_growing == true)
            //{
            //    m_scaleCounter += m_growthRate * elapsedGameTime;
            //    if (m_scaleCounter >= m_storedScale.X + m_growthDifference)
            //        m_growing = false;
            //}
            //else
            //{
            //    m_scaleCounter -= m_growthRate * elapsedGameTime;
            //    if (m_scaleCounter <= m_storedScale.X - m_growthDifference)
            //        m_growing = true;
            //}

            //if (m_dimming == true)
            //{
            //    m_dimCounter -= m_dimRate * elapsedGameTime;
            //    if (m_dimCounter <= 0.5f - m_dimDifference)
            //        m_dimming = false;
            //}
            //else 
            //{
            //    m_dimCounter += m_dimRate * elapsedGameTime;
            //    if (m_dimCounter >= 0.5f)
            //        m_dimming = true;
            //}

            //this.Scale = new Vector2(m_scaleCounter, m_scaleCounter);
            //this.Opacity = m_dimCounter;
            base.Draw(camera);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new LightSourceObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }
    }
}
