using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpriteSystem;

namespace DS2DEngine
{
    public class PhysicsObj : SpriteObj, IPhysicsObj
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

        private List<CollisionBox> _collisionBoxes = null;
        public PhysicsManager PhysicsMngr { get; set; }
        public bool DisableHitboxUpdating { get; set; }

        public bool AccelerationXEnabled { get; set; }
        public bool AccelerationYEnabled { get; set; }
        public bool DisableAllWeight { get; set; }
        public bool SameTypesCollide { get; set; }
        public bool DisableGravity { get; set; }

        private Rectangle m_terrainBounds;

        #region Constructors
        public PhysicsObj(string spriteName, PhysicsManager physicsManager = null)
            : base(spriteName)
        {
            IsWeighted = true;
            IsCollidable = true;
            _collisionBoxes = new List<CollisionBox>();

            if (physicsManager != null)
                physicsManager.AddObject(this);

            CollidesLeft = true;
            CollidesRight = true;
            CollidesTop = true;
            CollidesBottom = true;

            AccelerationXEnabled = true;
            AccelerationYEnabled = true;

            UpdateCollisionBoxes();
        }

        public PhysicsObj(Texture2D sprite, PhysicsManager physicsManager = null)
            : base(sprite)
        {
            IsWeighted = true;
            IsCollidable = true;
            _collisionBoxes = new List<CollisionBox>();

            if (physicsManager != null)
                physicsManager.AddObject(this);

            CollidesLeft = true;
            CollidesRight = true;
            CollidesTop = true;
            CollidesBottom = true;

            AccelerationXEnabled = true;
            AccelerationYEnabled = true;
        }
        #endregion

        public void AddCollisionBox(int xPos, int yPos, int width, int height, int hitboxType)
        {
            _collisionBoxes.Add(new CollisionBox((int)(xPos - AnchorX), (int)(yPos - AnchorY), width, height, hitboxType, this) { WasAdded = true, });
        }

        public void ClearCollisionBoxes()
        {
            _collisionBoxes.Clear();
        }

        public override void ChangeSprite(string spriteName)
        {
            base.ChangeSprite(spriteName);
            UpdateCollisionBoxes();
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (elapsedSeconds > Consts.FRAMERATE_CAP) elapsedSeconds = Consts.FRAMERATE_CAP;
            if (AccelerationXEnabled == true)
                this.X += _acceleration.X * elapsedSeconds;

            float yAcceleration = _acceleration.Y * elapsedSeconds;
            if (yAcceleration < _acceleration.Y * Consts.FRAMERATE_CAP)
                yAcceleration = _acceleration.Y * elapsedSeconds;
            if (AccelerationYEnabled == true)
            this.Y += yAcceleration;
        }

        public void UpdateCollisionBoxes()
        {
            if (Sprite.IsDisposed)
                ReinitializeSprite();

            if (m_frameDataList != null)
            {
                List<Hitbox> hbList = m_frameDataList[_frameIndex].hitboxList;
                m_terrainBounds = new Rectangle();
                int leftBound = int.MaxValue;
                int rightBound = -int.MaxValue;
                int topBound = int.MaxValue;
                int bottomBound = -int.MaxValue;

                for (int i = 0; i < hbList.Count; i++)
                {
                    if (_collisionBoxes.Count <= i)
                    {
                        CollisionBox newCollisionBox = new CollisionBox(this);
                        _collisionBoxes.Add(newCollisionBox);

                        //Adds the collision box to the parent if it is a PhysicsObjContainer.
                        //Because it passes by references, any changes to these collision boxes will be reflected in the PhysicsObjContainer.
                        if (Parent != null)
                        {
                            PhysicsObjContainer container = Parent as PhysicsObjContainer;
                            if (container != null)
                                container.CollisionBoxes.Add(newCollisionBox);
                        }
                    }

                    if (Parent == null)
                    {
                        _collisionBoxes[i].X = (int)(hbList[i].X * this.ScaleX);
                        _collisionBoxes[i].Y = (int)(hbList[i].Y * this.ScaleY);
                        _collisionBoxes[i].Width = (int)(hbList[i].Width * this.ScaleX);
                        _collisionBoxes[i].Height = (int)(hbList[i].Height * this.ScaleY);
                    }
                    else
                    {
                        _collisionBoxes[i].X = (int)(hbList[i].X * Parent.ScaleX * this.ScaleX);
                        _collisionBoxes[i].Y = (int)(hbList[i].Y * Parent.ScaleY * this.ScaleY);
                        _collisionBoxes[i].Width = (int)(hbList[i].Width * Parent.ScaleX * this.ScaleX);
                        _collisionBoxes[i].Height = (int)(hbList[i].Height * Parent.ScaleY * this.ScaleY);
                    }
                    _collisionBoxes[i].InternalRotation = hbList[i].Rotation;
                    _collisionBoxes[i].Type = hbList[i].Type;

                    if (_collisionBoxes[i].Type == Consts.TERRAIN_HITBOX)
                    {
                        Rectangle absRect = _collisionBoxes[i].AbsRect;

                        if (absRect.Left < leftBound)
                            leftBound = absRect.Left;

                        if (absRect.Right > rightBound)
                            rightBound = absRect.Right;

                        if (absRect.Top < topBound)
                            topBound = absRect.Top;

                        if (absRect.Bottom > bottomBound)
                            bottomBound = absRect.Bottom;
                    }
                }

                m_terrainBounds.X = leftBound;
                m_terrainBounds.Y = topBound;
                m_terrainBounds.Width = rightBound - leftBound;
                m_terrainBounds.Height = bottomBound - topBound;

                //Clears out any collision boxes not being used.
                for (int k = hbList.Count; k < _collisionBoxes.Count; k++)
                {
                    if (_collisionBoxes[k].WasAdded == false)
                    {
                        _collisionBoxes[k].X = 0;
                        _collisionBoxes[k].Y = 0;
                        _collisionBoxes[k].Width = 1;
                        _collisionBoxes[k].Height = 1;
                        _collisionBoxes[k].InternalRotation = 0;
                        _collisionBoxes[k].Type = Consts.NULL_HITBOX;
                    }
                }

                // This below optimziation didn't work.  This cauese the hero's sword to 
                // go nuts animating up and down every other frame.
                //  - it seems to get an old value
                // Optimization - Update cached values
                //foreach (CollisionBox box in _collisionBoxes)
                //{
                //    box.UpdateCachedValues();
                //}
            }
        }

        public virtual void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (this.IsWeighted == true && collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN)
            {
                //Vector2 mtdPos = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);
                Vector2 mtdPos = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                IPhysicsObj otherPhysicsObj = otherBox.AbsParent as IPhysicsObj;
                int collisionMarginOfError = 10;

                if (mtdPos.Y < 0) // There is an object below this object.
                {
                    if (this.AccelerationY > 0 && otherPhysicsObj.CollidesTop == true) // This object is falling and there is an object below, so stop this object from falling.
                    {
                        if (otherBox.AbsRotation != 0 ||
                            (otherPhysicsObj.TerrainBounds.Left <= this.TerrainBounds.Right && otherPhysicsObj.TerrainBounds.Right >= this.TerrainBounds.Right && (this.TerrainBounds.Right - otherPhysicsObj.TerrainBounds.Left > collisionMarginOfError)) ||
                                (this.TerrainBounds.Left <= otherPhysicsObj.TerrainBounds.Right && this.TerrainBounds.Right >= otherPhysicsObj.TerrainBounds.Right && (otherPhysicsObj.TerrainBounds.Right - this.TerrainBounds.Left > collisionMarginOfError)))
                        {
                            this.AccelerationY = 0;
                            this.Y += mtdPos.Y;
                        }
                    }
                    //else if (IsWeighted == false && otherPhysicsObj.CollidesTop == true) // The above code is only for weighted objects. Non-weighted objects presumably do not move by gravity, so collision detection needs to be always on.
                    //    this.Y += mtdPos.Y;
                }
                else if (mtdPos.Y > 0) // There is an object above this object.
                {
                    if (this.AccelerationY < 0 && otherPhysicsObj.CollidesBottom == true) // This object is going up and has hit an object above it, so stop this object from going up.
                    {
                        this.AccelerationY = 0;
                        this.Y += mtdPos.Y;
                    }
                   // else if (IsWeighted == false && otherPhysicsObj.CollidesBottom == true) // The above code is only for weighted objects. Non-weighted objects presumably do not move by gravity, so collision detection needs to be always on.
                   //     this.Y += mtdPos.Y;
                }

                if (mtdPos.X != 0)
                {
                    this.AccelerationX = 0;
                    if (((otherBox.AbsParent as IPhysicsObj).CollidesLeft == true && mtdPos.X > 0) || 
                        ((otherBox.AbsParent as IPhysicsObj).CollidesRight == true && mtdPos.X < 0))
                        this.X += mtdPos.X;
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                foreach (CollisionBox box in _collisionBoxes)
                {
                    box.Dispose();
                }
                _collisionBoxes.Clear();
                _collisionBoxes = null;
                if (PhysicsMngr != null)
                    PhysicsMngr.RemoveObject(this);

                Node = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new PhysicsObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            PhysicsObj clone = obj as PhysicsObj;
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

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
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

        public virtual Rectangle TerrainBounds
        {
            get { return m_terrainBounds; }
        }

        public void RemoveFromPhysicsManager()
        {
            if (this.PhysicsMngr != null)
                this.PhysicsMngr.RemoveObject(this);
        }

        public List<CollisionBox> CollisionBoxes
        {
            get { return _collisionBoxes; }
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

        public override Vector2 Scale
        {
            get { return base.Scale; }
            set
            {
                base.Scale = value;
                if (DisableHitboxUpdating == false)
                    UpdateCollisionBoxes();
            }
        }

        public override float ScaleX
        {
            get { return base.ScaleX; }
            set
            {
                base.ScaleX = value;
                if (DisableHitboxUpdating == false)
                    UpdateCollisionBoxes();
            }
        }

        public override float ScaleY
        {
            get { return base.ScaleY; }
            set
            {
                base.ScaleY = value;
                if (DisableHitboxUpdating == false)
                    UpdateCollisionBoxes();
            }
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
