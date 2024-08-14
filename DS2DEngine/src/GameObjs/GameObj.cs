using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using InputSystem;
using System.Xml;
using System.Globalization;

namespace DS2DEngine
{
    //TODO: Separate IDrawableObj from GameObj.
    public abstract class GameObj : ITriggerableObj, IDrawableObj, IDisposableObj, ICloneable, IClickableObj
    {
        private Vector2 _position  = Vector2.Zero;
        protected Vector2 _anchor = Vector2.Zero;
        protected int _width = 0;
        protected int _height = 0;
        protected Vector2 _scale = new Vector2(1,1);
        public Boolean Visible { get; set; }
        protected float _rotation = 0;
        protected float _layer = 0;
        protected float _orientation = 0;
        protected Vector2 _heading = Vector2.Zero;
        private ObjContainer _parent = null;
        private ObjContainer _topmostParent = null; // Optimization for Flip test - calling up to test topmost parent is expensive
        public bool AddToBounds = true; // Whether an object is added to the bounds of an object container or not.
        public string Tag = "";

        public float TurnSpeed = 2.0f;

        private bool m_forceDraw = false;

        protected SpriteEffects _flip = SpriteEffects.None;

        private float _speed = 0; // Speed represents the object's internal speed.
        public float CurrentSpeed { get; set; } // Current Speed represents the object's actual speed. So it could have a speed of 10, but be moving at 0 (which means it's not moving at all).

        public string Name { get; set; }

        public int ID { get; set; }

        private bool m_isDisposed = false;
        public bool IsDisposed { get { return m_isDisposed; } }

        public bool IsTriggered { get; set; }
        public bool LockFlip { get; set; }

        public bool OverrideParentScale { get; set; }

        private bool _useCachedValues = false;

        public GameObj()
        {
            //GameObjHandler.GameObjList.Add(this);
            Visible = true;
        }

        public virtual void ChangeSprite(string spriteName) { }

        public virtual void Draw(Camera2D camera) { }
        public virtual void DrawOutline(Camera2D camera) { }

        public void DrawBounds(Camera2D camera, Texture2D texture, Color color)
        {
            if (Parent != null)
                camera.Draw(texture, new Rectangle((int)(Bounds.X + this.AbsX - (this.X * Parent.ScaleX)),(int)(Bounds.Y + this.AbsY - (this.Y * Parent.ScaleY)), Bounds.Width, Bounds.Height) , color);
            else
                camera.Draw(texture, Bounds, color);
        }

        public void DrawAbsBounds(Camera2D camera, Texture2D texture, Color color)
        {
            camera.Draw(texture, AbsBounds, color);
        }

        public virtual void PopulateFromXMLReader(XmlReader reader, CultureInfo ci)
        {
            if (reader.MoveToAttribute("Name"))
                this.Name = reader.Value;
            if (reader.MoveToAttribute("X"))
                X = float.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("Y"))
                Y = float.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("Width"))
                _width = int.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("Height"))
                _height = int.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("ScaleX"))
                this.ScaleX = float.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("ScaleY"))
                this.ScaleY = float.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("Rotation"))
                this.Rotation= float.Parse(reader.Value, NumberStyles.Any, ci);
            if (reader.MoveToAttribute("Tag"))
                this.Tag= reader.Value;
            if (reader.MoveToAttribute("Layer"))
                this.Layer = int.Parse(reader.Value, NumberStyles.Any, ci);

            // Flip needs to be passed in as a bool instead of a SpriteEffects enum.
            bool flip = false;
            if (reader.MoveToAttribute("Flip"))
                flip = bool.Parse(reader.Value);

            if (flip == true)
                this.Flip = SpriteEffects.FlipHorizontally;
            else
                this.Flip = SpriteEffects.None;
        }

        public virtual void Dispose()
        {
            //GameObjHandler.GameObjList.Remove(this);
            _parent = null;
            _topmostParent = null;
            m_isDisposed = true;
        }

        public object Clone()
        {
            GameObj obj = CreateCloneInstance();
            this.FillCloneInstance(obj);
            return obj;
        }

        protected abstract GameObj CreateCloneInstance();
            
        protected virtual void FillCloneInstance(object obj)
        {
            // Must set flip to none before cloning data since flip affects some of the object's properties.
            SpriteEffects storedFlip = this.Flip;
            this.Flip = SpriteEffects.None;

            GameObj clone = obj as GameObj;
            clone.Position = this.Position;
            clone.Name = this.Name;
            clone.Visible = this.Visible;
            clone.Scale = this.Scale;
            clone.Rotation = this.Rotation;
            clone.Parent= this.Parent;

            clone.Anchor = this.Anchor;
            clone.Orientation = this.Orientation;
            clone.ForceDraw = this.ForceDraw;
            clone.Tag = this.Tag;
            clone.AddToBounds = this.AddToBounds;
            clone.IsTriggered = this.IsTriggered;
            clone.TurnSpeed = this.TurnSpeed;
            clone.Speed = this.Speed;
            clone.CurrentSpeed = this.CurrentSpeed;
            clone.Layer = this.Layer;
            clone.Heading = this.Heading;
            clone.Opacity = this.Opacity;
            clone.TextureColor = this.TextureColor;

            this.Flip = storedFlip; // Reset flip what it is supposed to be before cloning it.
            clone.Flip = this.Flip;
            clone.OverrideParentScale = this.OverrideParentScale;
            clone.UseCachedValues = this.UseCachedValues;
        }

        #region Properties

        public virtual int Width
        {
            get { return (int)(_width * ScaleX); }
        }

        public virtual int Height
        {
            get { return (int)(_height * ScaleY); }
        }

        public float Rotation
        {
            set { _rotation = MathHelper.ToRadians(value); }
            get { return MathHelper.ToDegrees(_rotation); }
        }

        public float Layer
        {
            set { _layer = value; }
            get { return _layer; }
        }

        public float X
        {
            set
            {
                _position.X = value;

                // This is what used to be here.  If you read the logic, they all boiled down to the same thing :/
                //if (this.Flip == SpriteEffects.FlipHorizontally && Parent != null)
                //    _position.X = value;
                //    //_position.X = value / Parent.ScaleX; // Disabled this because calling ChangeSprite on scaled PhysicsObjectContainers resulted in wrong placements of the physics objects in the container.
                //else
                //{
                //    if (Parent == null)
                //        _position.X = value;
                //    else
                //        _position.X = value;
                //        //_position.X = value / Parent.ScaleX;
                //}
            }
            get 
            {
                if (Parent == null || this.Flip != SpriteEffects.FlipHorizontally)
                    return _position.X;
                return -_position.X * Parent.ScaleX; // otherwise flip it horizontally
            }
        }

        public float Y
        {
            set
            {
                _position.Y = value;

                // This is what used to be here.  If you read the logic, they all boiled down to the same thing :/
                //if (Parent == null)
                //    _position.Y = value;
                //else
                //    _position.Y = value;
                //    //_position.Y = value / Parent.ScaleY;
            }
            get 
            {
                return _position.Y;

                // This is what used to be here.  If you read the logic, they all boiled down to the same thing :/
                //if (Parent == null)
                //    return _position.Y;
                //else
                //    return _position.Y;// *Parent.ScaleY;
            }
        }

        public Vector2 Position
        {
            set 
            {
                if (Parent == null || Flip != SpriteEffects.FlipHorizontally)
                    _position = value;
                else
                    _position = new Vector2(value.X, value.Y);

                // This is what used to be here.
                //if (Flip == SpriteEffects.FlipHorizontally && Parent != null)
                //    _position = new Vector2(value.X, value.Y);
                //    //_position = new Vector2(value.X / Parent.ScaleX, value.Y / Parent.ScaleY);
                //else
                //{
                //    if (Parent == null)
                //        _position = value;
                //    else
                //        _position = value;
                //        //_position = value / Parent.Scale;
                //}
            }
            get
            {
                if (Parent == null)
                    return _position;
                else if (Flip == SpriteEffects.FlipHorizontally)
                    return new Vector2(-_position.X * Parent.ScaleX, _position.Y * Parent.ScaleY);
                else
                    return _position * Parent.Scale;

                // This is what used to be here.
                //if (Flip == SpriteEffects.FlipHorizontally && Parent != null)
                //    return new Vector2(-_position.X * Parent.ScaleX, _position.Y * Parent.ScaleY);
                //else
                //{
                //    if (Parent == null)
                //        return _position;
                //    else
                //        return _position * Parent.Scale;
                //}
            }
        }

        public float AbsX
        {
            get
            {
                if (Parent == null)
                    return X;
                else
                    return Parent.X + RotatedPoint.X;
            }
        }

        public Vector2 AbsPosition
        {
            get { return new Vector2(AbsX, AbsY); }
        }

        public float AbsY
        {
            get
            {
                if (Parent == null)
                    return (int)this.Y;  // Casting to int because too sensitive a Y value causes up/down jitter.
                else
                    return (int)Parent.Y + (int)RotatedPoint.Y;
            }
        }

        private Vector2 RotatedPoint
        {
            get
            {
                if (Parent == null || Parent.Rotation == 0)
                    return Position;
                else
                    return CDGMath.RotatedPoint(Position, Parent.Rotation);
            }
        }

        public virtual float AnchorX
        {
            set { _anchor.X = value; }
            get
            {
                if (Flip == SpriteEffects.FlipHorizontally)
                    return this.Width / ScaleX - _anchor.X; // Cannot just call this._width because SpriteObj overrides Width.
                else
                    return _anchor.X;
            }
        }

        public virtual float AnchorY
        {
            set { _anchor.Y = value; }
            get { return _anchor.Y; }
        }

        public virtual Vector2 Anchor
        {
            set { _anchor = value; }
            get
            {
                if (Flip == SpriteEffects.FlipHorizontally)
                    return new Vector2(this.Width / ScaleX - _anchor.X, _anchor.Y); // Cannot just call this._width because SpriteObj overrides Width.
                else
                    return _anchor;
            }
        }

        public virtual float ScaleX
        {
            set { _scale.X = value; }
            get { return _scale.X; }
        }

        public virtual float ScaleY
        {
            set { _scale.Y = value; }
            get { return _scale.Y; }
        }

        public virtual Vector2 Scale
        {
            set { _scale = value; }
            get { return _scale; }
        }

        public float Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public float HeadingX
        {
            get { return _heading.X; }
            set { _heading.X = value; }
        }

        public float HeadingY
        {
            get { return _heading.Y; }
            set { _heading.Y = value; }
        }

        public Vector2 Heading
        {
            get { return _heading; }
            set { _heading = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public virtual SpriteEffects Flip
        {
            get 
            {
                if (_topmostParent == null)
                    return _flip;
                else
                    return _topmostParent.Flip;
            }
            set 
            {
             //   if (this.Parent == null && (this is SpriteObj) && _flip != value)
               //     this.X += this.Width - this.AnchorX * 2;
                _flip = value;
            }
        }

        public virtual Rectangle Bounds
        {
            get 
            {
                if (Parent == null)
                {
                    if (this.Rotation == 0)
                    {
                        return new Rectangle((int)(-this.AnchorX * ScaleX + this.X),
                        (int)(-this.AnchorY * ScaleY + this.Y),
                        this.Width, this.Height); // Do not multiply width and height by scale because their property already does that.
                    }
                    else
                    {
                        return CollisionMath.RotatedRectBounds(new Rectangle((int)(-this.AnchorX * ScaleX + this.X),
                        (int)(-this.AnchorY * ScaleY + this.Y),
                        this.Width, this.Height), this.Anchor, Rotation);
                    }
                }
                else
                {
                    if (Parent.Rotation + this.Rotation == 0)
                    {
                        return new Rectangle((int)((-this.AnchorX * ScaleX + this.X) * Parent.ScaleX),
                        (int)((-this.AnchorY * ScaleY + this.Y) * Parent.ScaleY),
                        (int)(this.Width * Parent.ScaleX), (int)(this.Height * Parent.ScaleY)); // Do not multiply width and height by scale because their property already does that.
                    }
                    else
                    {
                        return CollisionMath.RotatedRectBounds(new Rectangle((int)(-this.AnchorX * ScaleX * Parent.ScaleX + this.X),
                        (int)(-this.AnchorY * ScaleY * Parent.ScaleY + this.Y),
                        (int)(this.Width * Parent.ScaleX), (int)(this.Height * Parent.ScaleY)), this.Anchor, Parent.Rotation + this.Rotation);
                    }
                }
            }
        }

        public virtual Rectangle AbsBounds
        {
            get 
            {
                if (Parent == null)
                {
                    if (this.Rotation == 0)
                    {
                        return new Rectangle((int)(-this.AnchorX * ScaleX + this.AbsX),
                        (int)(-this.AnchorY * ScaleY + this.AbsY),
                        this.Width, this.Height); // Do not multiply width and height by scale because their property already does that.
                    }
                    else
                    {
                        return CollisionMath.RotatedRectBounds(new Rectangle((int)(-this.AnchorX * ScaleX + this.AbsX),
                        (int)(-this.AnchorY * ScaleY + this.AbsY),
                        this.Width, this.Height), this.Anchor, Rotation);
                    }
                }
                else
                {
                    if (Parent.Rotation + this.Rotation == 0)
                    {
                        return new Rectangle((int)(-this.AnchorX * ScaleX * Parent.ScaleX + this.AbsX),
                        (int)(-this.AnchorY * ScaleY * Parent.ScaleY + this.AbsY),
                        (int)(this.Width * Parent.ScaleX), (int)(this.Height * Parent.ScaleY)); // Do not multiply width and height by scale because their property already does that.
                    }
                    else
                    {
                        return CollisionMath.RotatedRectBounds(new Rectangle((int)(-this.AnchorX * ScaleX * Parent.ScaleX + this.AbsX),
                        (int)(-this.AnchorY * ScaleY * Parent.ScaleY + this.AbsY),
                        (int)(this.Width * Parent.ScaleX), (int)(this.Height * Parent.ScaleY)), this.Anchor, Parent.Rotation + this.Rotation);
                    }
                }
            }
        }

        private void SetTopMostParent(ObjContainer newParent)
        {
            if (newParent != null)
            {
                _topmostParent = newParent;
               SetTopMostParent(newParent.Parent);
            }
        }

        public ObjContainer Parent
        {
            set 
            { 
                _parent = value;
                if (value == null)
                {
                    _topmostParent = null;
                }
                else
                {
                    SetTopMostParent(value);
                }
            }
            get { return _parent; }
        }

        public bool ForceDraw
        {
            get
            {
                if (Parent != null)
                    return _topmostParent.ForceDraw;
                else
                    return m_forceDraw;
            }
            set { m_forceDraw = value; }
        }

        private float m_opacity = 1.0f;
        public float Opacity
        {
            get 
            {
                if (Parent == null)
                    return m_opacity;
                else
                    return m_opacity * Parent.Opacity;
            }
            set {  m_opacity = value; }
        }

        private Color m_textureColor = Color.White;
        public Color TextureColor
        {
            get 
            {
                if (Parent == null || Parent.TextureColor == Color.White)
                    return m_textureColor;// * Opacity; 
                else
                    return Parent.TextureColor;// *Opacity;                     
            }
            set { m_textureColor = value; }
        }

        public bool UseCachedValues 
        {
            get
            {
                // Not totally convinced this below thing is an optimization yet...getting conflicting data
                //if (_topmostParent == null)
                //    return _useCachedValues;
                //else
                //    return _topmostParent.UseCachedValues;
                if (Parent == null)
                    return _useCachedValues;
                return Parent.UseCachedValues;
            }
            set { _useCachedValues = value; }
        }

        #endregion

        #region ClickHandlers

        //public bool LeftJustPressed(Camera2D camera = null) 
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseLeftJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseLeftJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1)); 
        //}

        //public bool LeftPressed(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseLeftPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseLeftPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool LeftJustReleased(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseLeftJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseLeftJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool RightJustPressed(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseRightJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseRightJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool RightPressed(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseRightPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseRightPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool RightJustReleased(Camera2D camera = null)
        //{
        //    if (camera == null) return InputManager.MouseRightJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle(InputManager.MouseX, InputManager.MouseY, 1, 1));
        //    else return InputManager.MouseRightJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(InputManager.MouseX + camera.X - camera.Width / 2), (int)(InputManager.MouseY + camera.Y - camera.Height / 2), 1, 1));
        //}


        //public bool MiddleJustPressed(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseMiddleJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseMiddleJustPressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool MiddlePressed(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseMiddlePressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseMiddlePressed() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        //public bool MiddleJustReleased(Camera2D camera = null)
        //{
        //    Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);

        //    if (camera == null) return InputManager.MouseMiddleJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
        //    else return InputManager.MouseMiddleJustReleased() && CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1));
        //}

        public bool LeftMouseOverJustPressed(Camera2D camera)
        {
            return InputManager.MouseLeftJustPressed() && MouseOver(camera) == true;
        }

        public bool LeftMouseOverPressed(Camera2D camera)
        {
            return InputManager.MouseLeftPressed() && MouseOver(camera) == true;
        }

        public bool RightMouseOverJustPressed(Camera2D camera)
        {
            return InputManager.MouseRightJustPressed() && MouseOver(camera) == true;
        }

        public bool RightMouseOverPressed(Camera2D camera)
        {
            return InputManager.MouseRightPressed() && MouseOver(camera) == true;
        }

        public bool MiddleMouseOverJustPressed(Camera2D camera)
        {
            return InputManager.MouseMiddleJustPressed() && MouseOver(camera) == true;
        }

        public bool MiddleMouseOverPressed(Camera2D camera)
        {
            return InputManager.MouseMiddlePressed() && MouseOver(camera) == true;
        }

        //private Vector2 m_currentMousePos;
        //private Vector2 m_previousMousePos;
        private bool m_previousMouseOver = false;
        public bool MouseOver(Camera2D camera = null)
        {
            //Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);
            //m_previousMousePos = mousePos;
            //if (camera == null) return CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
            //else return (CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1)));

            bool mouseOver = false;
            int mouseX = (int)(InputManager.MouseX * EngineEV.ScreenRatio.X);
            int mouseY = (int)(InputManager.MouseY * EngineEV.ScreenRatio.Y);
            if (camera != null)
            {
                mouseX += (int)(camera.X - (camera.Width / 2f)); // This compensates for camera shift.
                float xDiff = (mouseX - camera.X) - ((mouseX - camera.X) * (1f / camera.Zoom)); // This compensates for camera zoom.
                mouseX = (int)(mouseX - xDiff);

                mouseY += (int)(camera.Y - (camera.Height / 2f)); // This compensates for camera shift.
                float yDiff = (mouseY - camera.Y) - ((mouseY - camera.Y) * (1f / camera.Zoom)); // This compensates for camera zoom.
                mouseY = (int)(mouseY - yDiff);
            }

            mouseOver = AbsBounds.Contains(mouseX, mouseY);
            return mouseOver;
        }

        public bool MouseJustOver(Camera2D camera = null)
        {
            //Vector2 mousePos = new Vector2(InputManager.MouseX * EngineEV.ScreenRatio.X, InputManager.MouseY * EngineEV.ScreenRatio.Y);
            //if (m_previousMousePos.X != mousePos.X || m_previousMousePos.Y != mousePos.Y)
            //{
            //    m_previousMousePos = mousePos;
            //    if (camera == null) return CollisionMath.Intersects(m_clickRect, new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1));
            //    else return (CollisionMath.Intersects(m_clickRect, new Rectangle((int)(mousePos.X + camera.X - camera.Width / 2), (int)(mousePos.Y + camera.Y - camera.Height / 2), 1, 1)));
            //}
            //m_previousMousePos = mousePos;
            //return false;

            bool mouseOver = MouseOver(camera);
            bool mouseJustOver = false;
            if (m_previousMouseOver == false && mouseOver == true)
            {
                mouseJustOver = true;
            }

            m_previousMouseOver = mouseOver;
            return mouseJustOver;
        }

        #endregion

        #region Touch Handlers

        public bool JustTapped(Camera2D camera, bool relativeToCamera = false, bool useAbsBounds = true)
        {
            return JustTapped(camera, relativeToCamera, useAbsBounds ? AbsBounds : Bounds);
        }

        public bool JustTapped(Camera2D camera, bool relativeToCamera, Rectangle bounds)
        {
            bool tapped = false;

            foreach (GestureSample tap in InputManager.Taps)
            {
                int touchX, touchY;
                TouchCoordsToViewportCoords(camera, tap.Position, out touchX, out touchY, relativeToCamera);
                if (bounds.Contains(touchX, touchY))
                {
                    tapped = true;
                    break;
                }
            }

            return tapped;
        }

        public enum DragDirections
        {
            NONE,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        public bool JustDragged(Camera2D camera, DragDirections direction, bool relativeToCamera = false, bool useAbsBounds = true)
        {
            bool dragged = false;

            foreach (GestureSample drag in InputManager.Drags)
            {
                int touchX, touchY;
                TouchCoordsToViewportCoords(camera, drag.Position, out touchX, out touchY, relativeToCamera);

                Rectangle bounds = (useAbsBounds ? AbsBounds : Bounds);
                if (bounds.Contains(touchX, touchY))
                {
                    const int threshold = 5;
                    switch (direction)
                    {
                        case DragDirections.LEFT:
                            dragged = drag.Delta.X < -threshold;
                            break;
                        case DragDirections.RIGHT:
                            dragged = drag.Delta.X > threshold;
                            break;
                        case DragDirections.UP:
                            dragged = drag.Delta.Y < -threshold;
                            break;
                        case DragDirections.DOWN:
                            dragged = drag.Delta.Y > threshold;
                            break;
                    }
                }
            }

            return dragged;
        }

        public static readonly Vector2 NoTouchPosition = new Vector2(-1, -1);
        public Vector2 GetPositionTouched(Camera2D camera, Rectangle bounds, bool relativeToCamera = false)
        {
            TouchCollection touches = TouchPanel.GetState();
            foreach (TouchLocation touch in touches)
            {
                int touchX, touchY;
                TouchCoordsToViewportCoords(camera, touch.Position, out touchX, out touchY, relativeToCamera);
                if (bounds.Contains(touchX, touchY))
                {
                    return new Vector2(touchX, touchY);
                }
            }

            // Nothing is touching this object. Return (-1, -1) to indicate an invalid value.
            return NoTouchPosition;
        }

        public static void TouchCoordsToViewportCoords(Camera2D camera, Vector2 touchCoords, out int vx, out int vy, bool relativeToCamera)
        {
            Viewport viewport = camera.GraphicsDevice.Viewport;
            int viewportX = (int)((touchCoords.X - viewport.X) / viewport.Width * EngineEV.ScreenWidth);
            int viewportY = (int)((touchCoords.Y - viewport.Y) / viewport.Height * EngineEV.ScreenHeight);

            if (relativeToCamera)
            {
                viewportX += (int)(camera.X - (camera.Width / 2f)); // This compensates for camera shift.
                float xDiff = (viewportX - camera.X) - ((viewportX - camera.X) * (1f / camera.Zoom)); // This compensates for camera zoom.
                viewportX = (int)(viewportX - xDiff);

                viewportY += (int)(camera.Y - (camera.Height / 2f)); // This compensates for camera shift.
                float yDiff = (viewportY - camera.Y) - ((viewportY - camera.Y) * (1f / camera.Zoom)); // This compensates for camera zoom.
                viewportY = (int)(viewportY - yDiff);
            }

            vx = viewportX;
            vy = viewportY;
        }

        #endregion
    }
}
