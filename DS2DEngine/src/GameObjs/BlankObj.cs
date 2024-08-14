using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{

    // A blank, invisible object that is mainly used for setting triggers.
    public class BlankObj : GameObj, IPhysicsObj
    {
        public LinkedListNode<IPhysicsObj> Node { get; set; }

        public bool CollidesLeft { get; set; }
        public bool CollidesRight { get; set; }
        public bool CollidesTop { get; set; }
        public bool CollidesBottom { get; set; }

        public int CollisionTypeTag { get; set; }
        private Vector2 _acceleration = Vector2.Zero;
        public Boolean IsWeighted { get; set; }
        public Boolean IsCollidable { get; set; }
        public bool AccelerationXEnabled { get; set; }
        public bool AccelerationYEnabled { get; set; }

        public bool DisableHitboxUpdating { get; set; }

        private List<CollisionBox> _collisionBoxes = null;
        public PhysicsManager PhysicsMngr { get; set; }
        private Rectangle m_terrainBounds = new Rectangle();
        public bool DisableAllWeight { get; set; }
        public bool SameTypesCollide { get; set; }
        public bool DisableGravity { get; set; }

        // You must pass in the width and height into the constructor since these properties are currently read-only.
        public BlankObj(int width, int height)
        {
            _width = width; 
            _height = height;
            IsWeighted = false;
            IsCollidable = false;
            _collisionBoxes = new List<CollisionBox>();

            CollidesLeft = true;
            CollidesRight = true;
            CollidesTop = true;
            CollidesBottom = true;

            AccelerationXEnabled = true;
            AccelerationYEnabled = true;

            DisableHitboxUpdating = true;
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            if (AccelerationXEnabled == true)
                this.X += _acceleration.X;
            if (AccelerationYEnabled == true)
                this.Y += _acceleration.Y;
        }

        public void UpdateCollisionBoxes() { }

        public void AddCollisionBox(int xPos, int yPos, int width, int height, int collisionType)
        {
            _collisionBoxes.Add(new CollisionBox((int)(xPos), (int)(yPos), width, height, collisionType, this));
            if (collisionType == Consts.TERRAIN_HITBOX)
                m_terrainBounds = new Rectangle(xPos, yPos, width, height);
        }

        public void ClearCollisionBoxes()
        {
            _collisionBoxes.Clear();
        }

        public virtual void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (this.IsWeighted == true && collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN)
            {
                this.AccelerationX = 0;
                if (this.AccelerationY > 0)
                    this.AccelerationY = 0;
                Vector2 mtdPos = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);
                this.X += mtdPos.X;
                this.Y += mtdPos.Y;
            }
        }

        public void RemoveFromPhysicsManager()
        {
            if (this.PhysicsMngr != null)
                this.PhysicsMngr.RemoveObject(this);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                if (PhysicsMngr != null)
                    PhysicsMngr.RemoveObject(this);

                Node = null;

                foreach (CollisionBox box in _collisionBoxes)
                    box.Dispose();

                _collisionBoxes.Clear();
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new BlankObj(_width, _height);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            BlankObj clone = obj as BlankObj;
            if (this.PhysicsMngr != null)
                this.PhysicsMngr.AddObject(clone);

            clone.CollidesLeft = this.CollidesLeft;
            clone.CollidesRight = this.CollidesRight;
            clone.CollidesBottom = this.CollidesBottom;
            clone.CollidesTop = this.CollidesTop;

            clone.IsWeighted = this.IsWeighted;
            clone.IsCollidable = this.IsCollidable;
            clone.DisableHitboxUpdating = this.DisableHitboxUpdating;
            clone.CollisionTypeTag = this.CollisionTypeTag;
            clone.DisableAllWeight = this.DisableAllWeight;
            clone.SameTypesCollide = this.SameTypesCollide;

            clone.AccelerationX = this.AccelerationX;
            clone.AccelerationY = this.AccelerationY;
            clone.AccelerationXEnabled = this.AccelerationXEnabled;
            clone.AccelerationYEnabled = this.AccelerationYEnabled;
            clone.DisableGravity = this.DisableGravity;
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci) // Needed because this is a hacked together physics object.
        {
            base.PopulateFromXMLReader(reader, ci);

            if (reader.MoveToAttribute("CollidesTop"))
                this.CollidesTop = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesBottom"))
                this.CollidesBottom = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesLeft"))
                this.CollidesLeft = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesRight"))
                this.CollidesRight = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("Collidable"))
                this.IsCollidable = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("Weighted"))
                this.IsWeighted = bool.Parse(reader.Value);
        }

        public List<CollisionBox> CollisionBoxes
        {
            get { return _collisionBoxes; }
        }

        public Rectangle TerrainBounds
        {
            get { return new Rectangle((int)(this.X + m_terrainBounds.X), (int)(this.Y + m_terrainBounds.Y), m_terrainBounds.Width, m_terrainBounds.Height); }
        }

        public float AccelerationX
        {
            set { _acceleration.X = value; }
            get { return _acceleration.X; }
        }

        public float AccelerationY
        {
            set { _acceleration.Y = value; }
            get { return _acceleration.Y; }
        }

        public void SetWidth(int width)
        {
            _width = width;
        }

        public void SetHeight(int height)
        {
            _height = height;
        }

        public bool HasTerrainHitBox
        {
            get
            {
                foreach (CollisionBox box in _collisionBoxes)
                {
                    if (box.Type == Consts.TERRAIN_HITBOX)
                        return true;
                }
                return false;
            }
        }

        public bool DisableCollisionBoxRotations
        {
            set
            {
                foreach (CollisionBox box in _collisionBoxes)
                    box.DisableRotation = value;
            }
            get
            {
                foreach (CollisionBox box in _collisionBoxes)
                    return box.DisableRotation;
                return false;
            }
        }
    }
}
