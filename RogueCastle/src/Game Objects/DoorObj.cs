using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class DoorObj : TerrainObj
    {
        private GameTypes.DoorType m_doorType = GameTypes.DoorType.OPEN;
        public string DoorPosition = "NONE";
        public RoomObj Room { get; set; } // Stores a reference to the room it is in.
        public bool Attached = false; // Keeps track of whether a door is attached to another door or not.
        public bool IsBossDoor = false;
        public bool Locked = false;

        private SpriteObj m_arrowIcon;

        public DoorObj(RoomObj roomRef, int width, int height, GameTypes.DoorType doorType)
            : base(width, height)
        {
            m_doorType = doorType;
            Room = roomRef;
            CollisionTypeTag = GameTypes.CollisionType_NULL;
            DisableHitboxUpdating = true;

            m_arrowIcon = new SpriteObj("UpArrowSquare_Sprite");
            m_arrowIcon.OutlineWidth = 2;
            m_arrowIcon.Visible = false;
        }

        public override void Draw(Camera2D camera)
        {
            if (m_arrowIcon.Visible == true)
            {
                m_arrowIcon.Position = new Vector2(this.Bounds.Center.X, this.Bounds.Top - 10 + (float)Math.Sin(Game.TotalGameTime * 20) * 3);
                m_arrowIcon.Draw(camera);
                m_arrowIcon.Visible = false;
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;
            if (this.Locked == false && player != null && player.IsTouchingGround == true && this.DoorPosition == "None")
                m_arrowIcon.Visible = true;
            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new DoorObj(Room, _width, _height, m_doorType);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            DoorObj clone = obj as DoorObj;
            clone.Attached = this.Attached;
            clone.IsBossDoor = this.IsBossDoor;
            clone.Locked = this.Locked;
            clone.DoorPosition = this.DoorPosition;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                Room = null; // Do not dispose the room in case we are simply closing this door.
                m_arrowIcon.Dispose();
                m_arrowIcon = null;
                base.Dispose();
            }
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
        {
            base.PopulateFromXMLReader(reader, ci);

            if (reader.MoveToAttribute("BossDoor"))
                this.IsBossDoor = bool.Parse(reader.Value);

            if (reader.MoveToAttribute("DoorPos"))
                this.DoorPosition = reader.Value;
        }

        public GameTypes.DoorType DoorType
        {
            get { return m_doorType; }
        }
    }
}
