using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class NpcObj : ObjContainer
    {
        private SpriteObj m_talkIcon;
        private bool m_useArrowIcon;
        public bool CanTalk { get; set; }

        public bool useArrowIcon
        {
            get { return m_useArrowIcon; }
            set
            {
                if (m_useArrowIcon != value)
                {
                    if (value == true)
                    {
                        m_talkIcon.ChangeSprite("UpArrowSquare_Sprite");
                        m_talkIcon.Scale = new Vector2(1);
                    }
                    else
                    {
                        m_talkIcon.ChangeSprite("ExclamationBubble_Sprite");
                        m_talkIcon.Scale = new Vector2(2);
                    }
                }

                m_useArrowIcon = value;
            }
        }

        public NpcObj(string spriteName)
            : base(spriteName)
        {
            CanTalk = true;
            
            m_talkIcon = new SpriteObj("ExclamationBubble_Sprite");
            m_talkIcon.Scale = new Vector2(2f, 2f);
            m_talkIcon.Visible = false;
            m_talkIcon.OutlineWidth = 2;
            this.OutlineWidth = 2;
        }

        public void Update(GameTime gameTime, PlayerObj player)
        {
            bool playerFacing = false;
            if (this.Flip == SpriteEffects.None && player.X > this.X)
                playerFacing = true;
            if (this.Flip != SpriteEffects.None && player.X < this.X)
                playerFacing = true;

            if (useArrowIcon == true)
                playerFacing = true;

            Rectangle storedBounds = this.Bounds;

            if (player != null && CollisionMath.Intersects(player.TerrainBounds, new Rectangle(storedBounds.X - 50, storedBounds.Y, storedBounds.Width + 100, storedBounds.Height))
                && playerFacing == true && (player.Flip != this.Flip || useArrowIcon == true) && CanTalk == true && player.IsTouchingGround == true)
                m_talkIcon.Visible = true;
            else
                m_talkIcon.Visible = false;

            if (useArrowIcon == false)
            {
                if (this.Flip == SpriteEffects.None)
                    m_talkIcon.Position = new Vector2(storedBounds.Left - m_talkIcon.AnchorX, storedBounds.Top - m_talkIcon.AnchorY + (float)Math.Sin(Game.TotalGameTime * 20) * 2);
                else
                    m_talkIcon.Position = new Vector2(storedBounds.Right + m_talkIcon.AnchorX, storedBounds.Top - m_talkIcon.AnchorY + (float)Math.Sin(Game.TotalGameTime * 20) * 2);
            }
            else
            {
                m_talkIcon.Position = new Vector2(this.X, storedBounds.Top - 70 + (float)Math.Sin(Game.TotalGameTime * 20) * 2);
            }
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Flip == SpriteEffects.None)
                m_talkIcon.Flip = SpriteEffects.FlipHorizontally;
            else
                m_talkIcon.Flip = SpriteEffects.None;

            base.Draw(camera);
            m_talkIcon.Draw(camera);
        }

        public bool IsTouching
        {
            get { return m_talkIcon.Visible; }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new NpcObj(this.SpriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            NpcObj clone = obj as NpcObj;
            clone.useArrowIcon = this.useArrowIcon;
            clone.CanTalk = this.CanTalk;
            base.FillCloneInstance(obj);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_talkIcon.Dispose();
                m_talkIcon = null;
                base.Dispose();
            }
        }
    }
}
