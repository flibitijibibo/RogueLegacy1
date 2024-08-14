using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class HoverObj : SpriteObj
    {
        private Vector2 m_startPos;
        public float HoverSpeed = 1;
        public float Amplitude = 1;

        public HoverObj(string spriteName)
            : base(spriteName)
        {
        }

        public void SetStartingPos(Vector2 pos)
        {
            m_startPos = pos;
        }

        public override void Draw(Camera2D camera)
        {
            this.Y = m_startPos.Y + (float)Math.Sin(Game.TotalGameTime * HoverSpeed) * Amplitude;
            base.Draw(camera);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new HoverObj(this.SpriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            HoverObj clone = obj as HoverObj;
            clone.HoverSpeed = this.HoverSpeed;
            clone.Amplitude = this.Amplitude;
            clone.SetStartingPos(m_startPos);
        }
    }
}
