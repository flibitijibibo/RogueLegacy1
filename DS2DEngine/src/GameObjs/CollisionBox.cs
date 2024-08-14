using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class CollisionBox : IDisposableObj
    {
        private Rectangle _rectangle;
        private int _collisionType;

        private GameObj m_parentObj;
        private float m_rotation;
        public bool WasAdded = false;

        private bool m_isDisposed = false;
        public bool DisableRotation = false;

        private Vector2 _cachedRotatedPoint;
        private Rectangle _cachedAbsRect;
        private float _cachedAbsRotation;

        public CollisionBox(GameObj parent)
        {
            _rectangle = new Rectangle();
            _cachedRotatedPoint = new Vector2();
            _cachedAbsRect = new Rectangle();
            _cachedAbsRotation = 0.0f;
            m_parentObj = parent;
            m_rotation = 0;

            UpdateCachedValues();
        }

        public CollisionBox(int x, int y, int width, int height, int type, GameObj parent)
        {
            _rectangle = new Rectangle(x, y, width, height);
            _cachedRotatedPoint = new Vector2();
            _cachedAbsRect = new Rectangle();
            _cachedAbsRotation = 0.0f;
            _collisionType = type;
            m_parentObj = parent;
            m_rotation = 0;

            UpdateCachedValues();
        }

        public void UpdateCachedValues()
        {
            _cachedRotatedPoint = RotatedPoint;
            _cachedAbsRect = new Rectangle((int)_cachedRotatedPoint.X, (int)_cachedRotatedPoint.Y, Width, Height);
            _cachedAbsRotation = AbsRotationNoCache();
        }

        public int X
        {
            set { _rectangle.X = value; }
            get 
            {
                if (m_rotation == 0 && m_parentObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                    return (int)-(_rectangle.X + _rectangle.Width);
                else
                    return _rectangle.X; 
            }
        }

        public int Y
        {
            set { _rectangle.Y = value; }
            get { return _rectangle.Y; }
        }

        public int Width
        {
            set { _rectangle.Width = value; }
            get { return _rectangle.Width; }
        }

        public int Height
        {
            set { _rectangle.Height = value; }
            get { return _rectangle.Height; }
        }

        public int Type
        {
            set { _collisionType = value; }
            get { return _collisionType; }
        }

        public override String ToString()
        {
            return "[x: " + _rectangle.X + ", y: " + _rectangle.Y + ", absX: " + AbsX + ", absY: " + AbsY + ", width: " + _rectangle.Width + ", Height: " + _rectangle.Height + ", Type: " + _collisionType + "]";
        }

        public Rectangle Rect
        {
            get { return _rectangle; }
        }

        public int AbsX
        {
            get 
            {
                if (this.Parent.UseCachedValues == true)
                    return (int)(_cachedRotatedPoint.X); 
                else
                    return (int)(RotatedPoint.X);
            }
        }

        public int AbsY
        {
            get
            {
                if (this.Parent.UseCachedValues == true)
                    return (int)(_cachedRotatedPoint.Y);
                else
                    return (int)(RotatedPoint.Y);
            }
        }

        private float Rotation
        {
            get
            {
                if (DisableRotation == true)
                    return 0;

                if (m_parentObj.Parent != null)
                    return m_parentObj.Parent.Rotation + m_parentObj.Rotation;
                else
                    return m_parentObj.Rotation;
            }
        }

        private float AbsRotationNoCache()
        {
            if (DisableRotation == true)
                return 0;

            if (m_parentObj.Parent != null)
                return m_parentObj.Parent.Rotation + m_parentObj.Rotation + InternalRotation;
            else
                return m_parentObj.Rotation + InternalRotation;
        }

        public float AbsRotation
        {
            get
            {
                if (this.Parent.UseCachedValues == true)
                    return _cachedAbsRotation;
                else
                    return AbsRotationNoCache();
            }
        }

        public float InternalRotation
        {
            get
            {
                if (m_parentObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                    return MathHelper.ToDegrees(-m_rotation);
                else
                    return MathHelper.ToDegrees(m_rotation);
            }
            set { m_rotation = MathHelper.ToRadians(value); }
        }

        private Vector2 RotatedPoint
        {
            get
            {
                if (Rotation == 0)
                {
                    if (m_rotation != 0 && m_parentObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                    {
                        Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle(this.X, this.Y, Width, Height), MathHelper.ToDegrees(m_rotation), Vector2.Zero);
                        topRight.X = -topRight.X;
                        return new Vector2(topRight.X + m_parentObj.AbsX, topRight.Y + m_parentObj.AbsY);
                    }
                    else
                        return new Vector2(this.X + m_parentObj.AbsX, this.Y + m_parentObj.AbsY);
                }
                else
                {
                    //if (m_parentObj.Parent != null)
                    if (m_rotation != 0 && m_parentObj.Parent != null && m_parentObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                    {
                        // if (m_parentObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally && m_rotation != 0)
                        {

                            Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle(this.X, this.Y, Width, Height), MathHelper.ToDegrees(m_rotation), Vector2.Zero);
                            topRight.X = -topRight.X;
                            return CDGMath.RotatedPoint(new Vector2(topRight.X + m_parentObj.AbsX, topRight.Y + m_parentObj.AbsY), new Vector2(m_parentObj.Parent.AbsX, m_parentObj.Parent.AbsY), this.Rotation);
                        }
                        //  else
                        //    return CDGMath.RotatedPoint(new Vector2(this.X + m_parentObj.AbsX, this.Y + m_parentObj.AbsY), new Vector2(m_parentObj.Parent.AbsX, m_parentObj.Parent.AbsY), this.Rotation);
                    }
                    else
                        return CDGMath.RotatedPoint(new Vector2(this.X + m_parentObj.AbsX, this.Y + m_parentObj.AbsY), new Vector2(m_parentObj.AbsX, m_parentObj.AbsY), this.Rotation);
                }
            }
        }

        public Rectangle AbsRect
        {
            get 
            {
                if (this.Parent.UseCachedValues == true)
                    return _cachedAbsRect;
                else
                    return new Rectangle(AbsX, AbsY, Width, Height);
            }
        }

        public Rectangle NonRotatedAbsRect
        {
            get { return new Rectangle((int)(this.X + m_parentObj.AbsX), (int)(this.Y + m_parentObj.AbsY), Width, Height); }
        }

        public GameObj Parent
        {
            get { return m_parentObj; }
        }

        public GameObj AbsParent
        {
            get
            {
                if (m_parentObj.Parent == null)
                    return m_parentObj;
                else
                    return m_parentObj.Parent;
            }
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;
                m_parentObj = null;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
