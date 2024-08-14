using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ProjectileIconObj : GameObj
    {
        private SpriteObj m_iconBG;
        private SpriteObj m_iconProjectile;
        private ProjectileObj m_attachedProjectile;

        private int m_iconOffset = 60;

        public ProjectileIconObj()
        {
            this.ForceDraw = true;
            m_iconBG = new SpriteObj("ProjectileIcon_Sprite");
            m_iconBG.ForceDraw = true;

            m_iconProjectile = new SpriteObj("Blank_Sprite");
            m_iconProjectile.ForceDraw = true;
        }

        public void Update(Camera2D camera)
        {
            if (AttachedProjectile.X <= camera.Bounds.Left + m_iconOffset)
                this.X = m_iconOffset;
            else if (AttachedProjectile.X > camera.Bounds.Right - m_iconOffset)
                this.X = 1320 - m_iconOffset;
            else
                this.X = AttachedProjectile.X - camera.TopLeftCorner.X;

            if (AttachedProjectile.Y <= camera.Bounds.Top + m_iconOffset)
                this.Y = m_iconOffset;
            else if (AttachedProjectile.Y > camera.Bounds.Bottom - m_iconOffset)
                this.Y = 720 - m_iconOffset;
            else
                this.Y = AttachedProjectile.Y - camera.TopLeftCorner.Y;

            this.Rotation = CDGMath.AngleBetweenPts(camera.TopLeftCorner + this.Position, AttachedProjectile.Position);

            m_iconBG.Position = this.Position;
            m_iconBG.Rotation = this.Rotation;

            m_iconProjectile.Position = this.Position;
            m_iconProjectile.Rotation = AttachedProjectile.Rotation;
            m_iconProjectile.GoToFrame(AttachedProjectile.CurrentFrame);
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Visible == true)// && CollisionMath.Intersects(AttachedProjectile.Bounds, camera.LogicBounds))
            {
                m_iconBG.Draw(camera);
                m_iconProjectile.Draw(camera);
                //base.Draw(camera);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_iconBG.Dispose();
                m_iconBG = null;
                m_iconProjectile.Dispose();
                m_iconProjectile = null;
                AttachedProjectile = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ProjectileIconObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            ProjectileIconObj clone = obj as ProjectileIconObj;
            clone.AttachedProjectile = this.AttachedProjectile;
        }

        public ProjectileObj AttachedProjectile
        {
            get { return m_attachedProjectile;}
            set
            {
                m_attachedProjectile = value;
                if (value != null)
                {
                    m_iconProjectile.ChangeSprite(value.SpriteName);
                    m_iconProjectile.Scale = new Vector2(0.7f, 0.7f);
                }
            }
        }

    }
}
