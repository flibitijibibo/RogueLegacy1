using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class BlacksmithObj : ObjContainer
    {
        private const int PART_ASSET1 = 0;
        private const int PART_ASSET2 = 1;
        private const int PART_BODY = 2;
        private const int PART_HEAD = 3;
        private const int PART_HEADTRIM = 4;
        private const int PART_ARM = 5;

        private SpriteObj m_hammerSprite;
        private SpriteObj m_headSprite;
        private float m_hammerAnimCounter = 0;

        public BlacksmithObj()
            : base("Blacksmith_Character")
        {
            m_hammerSprite = _objectList[PART_ARM] as SpriteObj;
            m_headSprite = _objectList[PART_HEAD] as SpriteObj;
            this.AnimationDelay = 1 / 10f;
        }

        public void Update(GameTime gameTime)
        {
            if (m_hammerAnimCounter <= 0 && m_hammerSprite.IsAnimating == false)
            {
                m_hammerSprite.PlayAnimation(false);
                m_hammerAnimCounter = CDGMath.RandomFloat(0.5f, 3);
            }
            else
                m_hammerAnimCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_hammerSprite = null;
                m_headSprite = null;
                base.Dispose();
            }
        }
    }
}
