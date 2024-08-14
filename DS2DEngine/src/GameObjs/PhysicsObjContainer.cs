using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteSystem;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class PhysicsObjContainer : ObjContainer, IPhysicsObj
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
        private List<CollisionBox> _collisionBoxes;
        public PhysicsManager PhysicsMngr { get; set; }

        public bool DisableHitboxUpdating { get; set; }

        public bool AccelerationXEnabled { get; set; }
        public bool AccelerationYEnabled { get; set; }
        public bool DisableAllWeight { get; set; }
        public bool SameTypesCollide { get; set; }
        public bool DisableGravity { get; set; }

        protected Rectangle m_terrainBounds;

        // Calls ObjContainer() because other constructors for ObjContainer create SpriteObjs. This container needs PhysicsObjs.
        public PhysicsObjContainer(PhysicsManager physicsManager = null)
        {
            IsWeighted = true;
            IsCollidable = true;
            _collisionBoxes = new List<CollisionBox>();

            if (physicsManager != null)
            {
                PhysicsMngr = physicsManager;
                PhysicsMngr.AddObject(this);
            }

            CollidesLeft = true;
            CollidesRight = true;
            CollidesTop = true;
            CollidesBottom = true;

            AccelerationXEnabled = true;
            AccelerationYEnabled = true;
        }

        public PhysicsObjContainer(string spriteName, PhysicsManager physicsManager = null)
        {
            IsWeighted = true;
            IsCollidable = true;
            _collisionBoxes = new List<CollisionBox>();
            _spriteName = spriteName; // Do not remove this line.

            List<CharacterData> charDataList = SpriteLibrary.GetCharData(spriteName);
            foreach (CharacterData chars in charDataList)
            {
                PhysicsObj newObj = new PhysicsObj(chars.Child);

                newObj.X = chars.ChildX;
                newObj.Y = chars.ChildY;
                this.AddChild(newObj);

                if (!m_spritesheetNameList.Contains(newObj.SpritesheetName))
                    m_spritesheetNameList.Add(newObj.SpritesheetName);
            }

            if (physicsManager != null)
            {
                PhysicsMngr = physicsManager;
                PhysicsMngr.AddObject(this);
            }

            CollidesLeft = true;
            CollidesRight = true;
            CollidesTop = true;
            CollidesBottom = true;

            AccelerationXEnabled = true;
            AccelerationYEnabled = true;

            m_numCharData = charDataList.Count;
            m_charDataStartIndex = 0;

            UpdateCollisionBoxes();
        }

        public override void ChangeSprite(string spriteName)
        {
            _spriteName = spriteName;

            if (m_charDataStartIndex == -1)
                m_charDataStartIndex = _objectList.Count; // Sets the starting index of when ChangeSprite was called to the end of the object list if it wasn't originally set in the constructor.

            List<CharacterData> charDataList = SpriteLibrary.GetCharData(spriteName);

            m_spritesheetNameList.Clear();
            int indexCounter = 0;
            for (int i = m_charDataStartIndex; i < charDataList.Count; i++)
            {
                CharacterData charData = charDataList[indexCounter];

                if (i >= m_numCharData)
                {
                    PhysicsObj objToAdd = new PhysicsObj(charData.Child);
                    objToAdd.X = charData.ChildX;
                    objToAdd.Y = charData.ChildY;
                    this.AddChildAt(i, objToAdd);
                    m_numCharData++;
                }
                else
                {
                    PhysicsObj physicsObj = _objectList[i] as PhysicsObj;
                    physicsObj.Visible = true;
                    if (physicsObj != null)
                    {
                        physicsObj.ChangeSprite(charData.Child);
                        physicsObj.X = charData.ChildX;
                        physicsObj.Y = charData.ChildY;
                        if (!m_spritesheetNameList.Contains(physicsObj.SpritesheetName))
                            m_spritesheetNameList.Add(physicsObj.SpritesheetName);
                    }
                }

                indexCounter++;
            }

            if (charDataList.Count < m_numCharData)
            {
                //for (int i = m_charDataStartIndex + (m_numCharData - charDataList.Count); i < m_charDataStartIndex + m_numCharData; i++)
                //for (int i = m_charDataStartIndex + 1; i < m_charDataStartIndex + m_numCharData; i++)
                for (int i = charDataList.Count; i < m_numCharData; i++)
                {
                    _objectList[i].Visible = false;
                }

            }

            this.StopAnimation();
            CalculateBounds();
            UpdateCollisionBoxes();
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (elapsedSeconds > Consts.FRAMERATE_CAP) elapsedSeconds = Consts.FRAMERATE_CAP;
            if (this.AccelerationXEnabled == true)
                this.X += _acceleration.X *elapsedSeconds;

            float yAcceleration = _acceleration.Y * elapsedSeconds;
            if (yAcceleration < _acceleration.Y * Consts.FRAMERATE_CAP)
                yAcceleration = _acceleration.Y * elapsedSeconds;
            if (this.AccelerationYEnabled == true)
                this.Y += yAcceleration;
        }

        public void UpdateCollisionBoxes()
        {
            m_terrainBounds = new Rectangle();
            int leftBound = int.MaxValue;
            int rightBound = -int.MaxValue;
            int topBound = int.MaxValue;
            int bottomBound = -int.MaxValue;

            foreach (GameObj obj in _objectList)
            {
                PhysicsObj physObj = obj as PhysicsObj;
                if (physObj != null)
                {
                    physObj.UpdateCollisionBoxes();

                    if (physObj.TerrainBounds.Left < leftBound)
                        leftBound = physObj.TerrainBounds.Left;

                    if (physObj.TerrainBounds.Right > rightBound)
                        rightBound = physObj.TerrainBounds.Right;

                    if (physObj.TerrainBounds.Top < topBound)
                        topBound = physObj.TerrainBounds.Top;

                    if (physObj.TerrainBounds.Bottom > bottomBound)
                        bottomBound = physObj.TerrainBounds.Bottom;
                }
            }

            m_terrainBounds.X = leftBound;
            m_terrainBounds.Y = topBound;
            m_terrainBounds.Width = rightBound - leftBound;
            m_terrainBounds.Height = bottomBound - topBound;
        }

        public virtual void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN && IsWeighted == true)
            {
                Vector2 mtdPos = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                IPhysicsObj otherPhysicsObj = otherBox.AbsParent as IPhysicsObj;
                int collisionMarginOfError = 10;
                if (mtdPos.Y < 0) // There is an object below this object.
                {
                    // This used to be this.AccelerationY > 0. Was changed for player dashing. If it causes problems, revert it back.
                    if (this.AccelerationY >= 0 && otherPhysicsObj.CollidesTop == true && IsWeighted == true) // This object is falling and there is an object below, so stop this object from falling.
                    {
                        // This code is for dropping through blocks.
                        if (otherBox.AbsRotation != 0 ||

                            (otherPhysicsObj.TerrainBounds.Left <= this.TerrainBounds.Right && otherPhysicsObj.TerrainBounds.Right >= this.TerrainBounds.Right && (this.TerrainBounds.Right - otherPhysicsObj.TerrainBounds.Left > collisionMarginOfError)) ||
                            (this.TerrainBounds.Left <= otherPhysicsObj.TerrainBounds.Right && this.TerrainBounds.Right >= otherPhysicsObj.TerrainBounds.Right && (otherPhysicsObj.TerrainBounds.Right - this.TerrainBounds.Left > collisionMarginOfError)))
                        {
                            this.AccelerationY = 0;
                            this.Y += mtdPos.Y;
                        }
                    }
                }
                else if (mtdPos.Y > 0) // There is an object above this object.
                {
                    if (this.AccelerationY < 0 && otherPhysicsObj.CollidesBottom == true && IsWeighted == true) // This object is going up and has hit an object above it, so stop this object from going up.
                    {
                        this.AccelerationY = 0;
                        this.Y += mtdPos.Y;
                    }           
                }

                if (mtdPos.X != 0)
                {
                    if ((otherPhysicsObj.CollidesLeft == true && mtdPos.X > 0) ||
                        (otherPhysicsObj.CollidesRight == true && mtdPos.X < 0))
                    {
                        this.AccelerationX = 0;
                        this.X += mtdPos.X;
                    }
                }

            }
        }

        public override void AddChild(GameObj obj)
        {
            PhysicsObj physicsObj = obj as PhysicsObj;
            if (physicsObj != null)
            {
                foreach (CollisionBox box in physicsObj.CollisionBoxes)
                {
                    if (!_collisionBoxes.Contains(box))
                        _collisionBoxes.Add(box);
                }
            }
            base.AddChild(obj);
        }

        public override void AddChildAt(int index, GameObj obj)
        {
            PhysicsObj physicsObj = obj as PhysicsObj;
            if (physicsObj != null)
            {
                foreach (CollisionBox box in physicsObj.CollisionBoxes)
                {
                    if (!_collisionBoxes.Contains(box))
                        _collisionBoxes.Add(box);
                }
            }
            base.AddChildAt(index, obj);
        }

        public override void RemoveChild(GameObj obj)
        {
            PhysicsObj physicsObj = obj as PhysicsObj;
            if (physicsObj != null)
            {
                foreach (CollisionBox box in physicsObj.CollisionBoxes)
                {
                    if (!_collisionBoxes.Contains(box))
                        _collisionBoxes.Remove(box);
                }
            }
            base.RemoveChild(obj);
        }

        public override GameObj RemoveChildAt(int index)
        {
            PhysicsObj obj = null;

            if (_objectList[index] is PhysicsObj)
                obj = _objectList[index] as PhysicsObj;

            if (obj != null)
            {
                foreach (CollisionBox box in obj.CollisionBoxes)
                {
                    _collisionBoxes.Remove(box);
                }
            }
            return base.RemoveChildAt(index);
        }

        public override void RemoveAll()
        {
            _collisionBoxes.Clear();
            base.RemoveAll();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                if (PhysicsMngr != null)
                    PhysicsMngr.RemoveObject(this);

                foreach (CollisionBox box in _collisionBoxes)
                {
                    box.Dispose();
                }
                _collisionBoxes.Clear();
                Node = null;
                base.Dispose();
            }
        }

        public void RemoveFromPhysicsManager()
        {
            if (this.PhysicsMngr != null)
                this.PhysicsMngr.RemoveObject(this);
        }


        protected override GameObj CreateCloneInstance()
        {

            if (_spriteName != "")
                return new PhysicsObjContainer(_spriteName);
            else
            {
                PhysicsObjContainer clone = new PhysicsObjContainer();

                foreach (GameObj obj in _objectList)
                    clone.AddChild(obj.Clone() as GameObj);
                return clone;
            }
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            PhysicsObjContainer clone = obj as PhysicsObjContainer;

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

        public virtual Rectangle TerrainBounds
        { 
            get { return m_terrainBounds; } 
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
