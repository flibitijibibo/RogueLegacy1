using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;

namespace RogueCastle
{
    public class TeleporterObj : PhysicsObj
    {
        public bool Activated { get; set; }
        private SpriteObj m_arrowIcon;

        public TeleporterObj()
            : base("TeleporterBase_Sprite")
        {
            this.CollisionTypeTag = GameTypes.CollisionType_WALL;
            SetCollision(false);
            this.IsWeighted = false;
            Activated = false;
            this.OutlineWidth = 2;

            m_arrowIcon = new SpriteObj("UpArrowSquare_Sprite");
            m_arrowIcon.OutlineWidth = 2;
            m_arrowIcon.Visible = false;
        }

        public void SetCollision(bool collides)
        {
            this.CollidesTop = collides;
            this.CollidesBottom = collides;
            this.CollidesLeft = collides;
            this.CollidesRight = collides;
        }

        public override void Draw(Camera2D camera)
        {
            if (m_arrowIcon.Visible == true)
            {
                m_arrowIcon.Position = new Vector2(this.Bounds.Center.X, this.Bounds.Top - 50 + (float)Math.Sin(Game.TotalGameTime * 20) * 2);
                m_arrowIcon.Draw(camera);
                m_arrowIcon.Visible = false;
            }

            base.Draw(camera);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;
            if (Game.ScreenManager.Player.ControlsLocked == false && player != null && player.IsTouchingGround == true)
                m_arrowIcon.Visible = true;

            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new TeleporterObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            TeleporterObj clone = obj as TeleporterObj;
            clone.Activated = this.Activated;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_arrowIcon.Dispose();
                m_arrowIcon = null;
                base.Dispose();
            }
        }
    }
}
