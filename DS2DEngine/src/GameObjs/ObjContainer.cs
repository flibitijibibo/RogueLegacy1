using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem;

namespace DS2DEngine
{
    public class ObjContainer : GameObj, IAnimateableObj
    {
        protected List<GameObj> _objectList = null;
        protected int _numChildren;
        private Rectangle m_bounds = new Rectangle();
        protected string _spriteName = "";

        protected bool m_animate = false;
        protected bool m_loop = false;
        protected int m_currentFrameIndex;
        protected int m_startAnimationIndex;
        protected int m_endAnimationIndex;

        protected int m_numCharData = 0; // An integer that keeps track of how many objects were added via ChangeSprite().
        protected int m_charDataStartIndex = -1; // An integer that keeps track of the starting index of the CharData.

        private bool m_wasAnimating = false; // A bool that determines whether an animation that was paused was actually animating when paused.

        public float AnimationDelay { get; set; }
        private float m_timeDelayCounter = 0;
        private float m_totalGameTime = 0;

        protected List<string> m_spritesheetNameList;

        public Color OutlineColour = Color.Black;
        public int OutlineWidth { get; set; }

        public ObjContainer()
        {
            AnimationSpeed = 1;
            _objectList = new List<GameObj>();
            _numChildren = 0;
            m_spritesheetNameList = new List<string>();
        }

        public ObjContainer(string spriteName)
        {
            AnimationSpeed = 1;
            _objectList = new List<GameObj>();
            m_spritesheetNameList = new List<string>();

            _numChildren = 0;
            _spriteName = spriteName;

            List<CharacterData> charDataList = SpriteLibrary.GetCharData(spriteName);
            foreach (CharacterData chars in charDataList)
            {
                SpriteObj newObj = new SpriteObj(chars.Child);
                newObj.X = chars.ChildX;  // Not sure why this is commented.  Only commented it out because it was commented out for PhysicsObjContainer.
                newObj.Y = chars.ChildY;
                if (!m_spritesheetNameList.Contains(newObj.SpritesheetName))
                    m_spritesheetNameList.Add(newObj.SpritesheetName);
                this.AddChild(newObj);
            }

            m_numCharData = charDataList.Count;
            m_charDataStartIndex = 0;
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
                    SpriteObj objToAdd = new SpriteObj(charData.Child);
                    objToAdd.X = charData.ChildX;
                    objToAdd.Y = charData.ChildY;
                    this.AddChildAt(i, objToAdd);
                    m_numCharData++;
                }
                else
                {
                    SpriteObj spriteObj = _objectList[i] as SpriteObj;
                    spriteObj.Visible = true;
                    if (spriteObj != null)
                    {
                        spriteObj.ChangeSprite(charData.Child);
                        spriteObj.X = charData.ChildX;
                        spriteObj.Y = charData.ChildY;
                        if (!m_spritesheetNameList.Contains(spriteObj.SpritesheetName))
                            m_spritesheetNameList.Add(spriteObj.SpritesheetName);
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
        }

        public virtual void AddChild(GameObj obj)
        {
            _objectList.Add(obj);
            obj.Parent = this;
            _numChildren++;

            SpriteObj spriteObj = obj as SpriteObj;
            if (spriteObj != null && !m_spritesheetNameList.Contains(spriteObj.SpritesheetName))
                m_spritesheetNameList.Add(spriteObj.SpritesheetName);

            CalculateBounds();
        }

        public virtual void AddChildAt(int index, GameObj obj)
        {
            _objectList.Insert(index, obj);
            obj.Parent = this;
            _numChildren++;

            SpriteObj spriteObj = obj as SpriteObj;
            if (spriteObj != null && !m_spritesheetNameList.Contains(spriteObj.SpritesheetName))
                m_spritesheetNameList.Add(spriteObj.SpritesheetName);

            CalculateBounds();
        }

        public virtual void RemoveChild(GameObj obj)
        {
            obj.Parent = null;
            _objectList.Remove(obj);
            _numChildren--;

            SpriteObj spriteObj = obj as SpriteObj;
            if (spriteObj != null && m_spritesheetNameList.Contains(spriteObj.SpritesheetName))
                m_spritesheetNameList.Remove(spriteObj.SpritesheetName);

            CalculateBounds();
        }

        public virtual GameObj RemoveChildAt(int index)
        {
            _numChildren--;
            GameObj objectToReturn = _objectList[index];
            objectToReturn.Parent = null;
            _objectList.RemoveAt(index);

            CalculateBounds();

            SpriteObj spriteObj = objectToReturn as SpriteObj;
            if (spriteObj != null && m_spritesheetNameList.Contains(spriteObj.SpritesheetName))
                m_spritesheetNameList.Remove(spriteObj.SpritesheetName);

            return objectToReturn;
        }

        public virtual void RemoveAll()
        {
            foreach (GameObj obj in _objectList)
                obj.Parent = null;
            _objectList.Clear();
            _numChildren = 0;

            m_bounds = new Rectangle();

            m_spritesheetNameList.Clear();
        }

        public GameObj GetChildAt(int index)
        {
            return _objectList[index];
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Visible == true)
            {
                if (CollisionMath.Intersects(this.Bounds, camera.LogicBounds) || ForceDraw == true)
                {
                    if (OutlineWidth > 0)
                    {
                        DrawOutline(camera);
                    }

                    foreach (GameObj obj in _objectList)
                    {
                        obj.Draw(camera);
                    }

                    float elapsedTotalSeconds = camera.ElapsedTotalSeconds;
                    if (m_animate == true && m_totalGameTime != elapsedTotalSeconds)
                    {
                        m_totalGameTime = elapsedTotalSeconds;

                        if (camera.GameTime != null)
                            m_timeDelayCounter += elapsedTotalSeconds;//(float)camera.GameTime.ElapsedGameTime.TotalSeconds;

                        if (m_timeDelayCounter >= AnimationDelay)
                        {
                            m_timeDelayCounter = 0;
                            GoToNextFrame();
                        }
                    }
                }
            }
        }

        //public override void DrawOutline(Camera2D camera, int width)
        public override void DrawOutline(Camera2D camera)
        {
            if (this.Visible == true)
            {
                //if (CollisionMath.Intersects(this.Bounds, camera.Bounds) || ForceDraw == true)
                {
                    foreach (GameObj obj in _objectList)
                    {
                        obj.DrawOutline(camera);
                    }
                }
            }
        }

        public void CalculateBounds()
        {
            int leftBound = int.MaxValue;
            int rightBound = -int.MaxValue;
            int topBound = int.MaxValue;
            int bottomBound = -int.MaxValue;

            // Hack to fix bounds of pre-flipped objcontainers.
            SpriteEffects storedFlip = _flip;
            _flip = SpriteEffects.None;

            foreach (GameObj obj in _objectList)
            {
                if (obj.Visible == true && obj.AddToBounds == true)
                {
                    //Rectangle boundsRect = new Rectangle((int)((-obj.AnchorX * obj.ScaleX + obj.X) * this.ScaleX), (int)(obj.Bounds.Y), obj.Width, obj.Height);
                    Rectangle objBounds = obj.Bounds;

                    if (objBounds.Left < leftBound)
                        leftBound = objBounds.Left;

                    if (objBounds.Right > rightBound)
                        rightBound = objBounds.Right;

                    if (objBounds.Top < topBound)
                        topBound = objBounds.Top;

                    if (objBounds.Bottom > bottomBound)
                        bottomBound = objBounds.Bottom;
                }
            }

            m_bounds.X = leftBound;
            m_bounds.Y = topBound;
            m_bounds.Width = rightBound - leftBound;
            m_bounds.Height = bottomBound - topBound;

            // Hack to fix bounds of pre-flipped objcontainers.
            if (storedFlip == SpriteEffects.FlipHorizontally)
                m_bounds.X = -m_bounds.Right;
            _flip = storedFlip;

            _width = (int)(m_bounds.Width / this.ScaleX); // Divide by scale because the Width property multiplies by scale, so it would be mulitplied by scale twice when calling the Width property.
            _height = (int)(m_bounds.Height / this.ScaleY); // Same for Height.
        }

        public void PlayAnimation(bool loopAnimation = true)
        {
            if (m_charDataStartIndex > -1)
            {
                foreach (GameObj obj in _objectList)
                {
                    if (obj is IAnimateableObj)
                        (obj as IAnimateableObj).PlayAnimation(loopAnimation);
                }
                //(_objectList[m_charDataStartIndex] as IAnimateableObj).PlayAnimation(loopAnimation); // Uncomment this if you want to be able to separate the animating state
                                                                                                       // of each object in this container.
                m_loop = loopAnimation;
                m_startAnimationIndex = 1;
                m_endAnimationIndex = this.TotalFrames;
                m_animate = true;
                m_currentFrameIndex = m_startAnimationIndex;

                m_timeDelayCounter = 0;
            }
        }

        public void PlayAnimation(int startIndex, int endIndex, bool loopAnimation = false)
        {
            foreach (GameObj obj in _objectList)
            {
                IAnimateableObj animateableObj = obj as IAnimateableObj;
                if (animateableObj != null)
                    animateableObj.PlayAnimation(startIndex, endIndex, loopAnimation);
            }
            m_loop = loopAnimation;

            // Must be called after PlayAnimation because SpriteObj.cs also does this change.
            startIndex = (startIndex - 1) * AnimationSpeed + 1;
            endIndex = endIndex * AnimationSpeed;

            if (startIndex < 1)
                startIndex = 1;
            else if (startIndex > TotalFrames)
                startIndex = TotalFrames;

            if (endIndex < 1)
                endIndex = 1;
            else if (endIndex > TotalFrames)
                endIndex = TotalFrames;
            m_startAnimationIndex = startIndex;
            m_endAnimationIndex = endIndex;
            m_currentFrameIndex = startIndex;
            m_animate = true;
        }

        public void PlayAnimation(string startLabel, string endLabel, bool loopAnimation = false)
        {
            int startIndex = FindLabelIndex(startLabel);
            int endIndex = FindLabelIndex(endLabel);
            if (startIndex == -1)
                throw new Exception("Could not find starting label " + startLabel);
            else if (endIndex == -1)
                throw new Exception("Could not find ending label " + endLabel);
            else
                PlayAnimation(startIndex, endIndex, loopAnimation);
        }

        public int FindLabelIndex(string label)
        {
            foreach (GameObj obj in _objectList)
            {
                IAnimateableObj animateableObj = obj as IAnimateableObj;
                if (animateableObj != null)
                {
                    int index = animateableObj.FindLabelIndex(label);
                    if (index != -1)
                        return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// For ObjContainer, GoToNextFrame() only keeps track of the CurrentFrame Index
        /// </summary>
        public void GoToNextFrame() 
        {
            m_currentFrameIndex++;
            if (m_currentFrameIndex > m_endAnimationIndex && m_loop == true)
                m_currentFrameIndex = m_startAnimationIndex;
            else if (m_currentFrameIndex > m_endAnimationIndex && m_loop == false)
                m_animate = false;
        }

        public void GoToFrame(int index)
        {
            foreach (GameObj obj in _objectList)
            {
                if (obj is IAnimateableObj)
                    (obj as IAnimateableObj).GoToFrame(index);
            }
            //if (m_charDataStartIndex > -1)
            //    (_objectList[m_charDataStartIndex] as IAnimateableObj).GoToFrame(index);
        }

        public void ResumeAnimation()
        {
            if (m_charDataStartIndex > -1)
            {
                foreach (GameObj obj in _objectList)
                {
                    if (obj is IAnimateableObj)
                        (obj as IAnimateableObj).ResumeAnimation();
                }
                //(_objectList[m_charDataStartIndex] as IAnimateableObj).ResumeAnimation();
                if (m_wasAnimating == true)
                    m_animate = true;
            }
            m_wasAnimating = false;
        }

        public void PauseAnimation()
        {
            if (m_charDataStartIndex > -1)
            {
                foreach (GameObj obj in _objectList)
                {
                    if (obj is IAnimateableObj)
                        (obj as IAnimateableObj).PauseAnimation();
                }
                //(_objectList[m_charDataStartIndex] as IAnimateableObj).PauseAnimation();
                m_wasAnimating = this.IsAnimating;
                m_animate = false;
            }
        }

        public void StopAnimation()
        {
            if (m_charDataStartIndex > -1)
            {
                foreach (GameObj obj in _objectList)
                {
                    if (obj is IAnimateableObj)
                        (obj as IAnimateableObj).StopAnimation();
                }
                //(_objectList[m_charDataStartIndex] as IAnimateableObj).StopAnimation();
                m_animate = false;
            }
            m_wasAnimating = false;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                foreach (GameObj obj in _objectList)
                {
                    obj.Dispose();
                }
                RemoveAll();
                _objectList.Clear();
                _objectList = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            if (_spriteName != "")
                return new ObjContainer(_spriteName);
            else
            {
                ObjContainer clone = new ObjContainer();
                foreach (GameObj obj in _objectList)
                    clone.AddChild(obj.Clone() as GameObj);

                return clone;
            }
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public int NumChildren
        {
            get { return _objectList.Count; }
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(m_bounds.X + this.X),
                    (int)(m_bounds.Y + this.Y),
                    m_bounds.Width, m_bounds.Height);
            }
        }

        public Rectangle PureBounds
        {
            get
            {
                return new Rectangle((int)(m_bounds.X + this.X),
                    (int)(m_bounds.Y + this.Y),
                    m_bounds.Width, m_bounds.Height);
            }
        }

        public Rectangle RelativeBounds
        {
            get { return m_bounds; }
        }

        public string SpriteName
        {
            get { return _spriteName; }
            set { _spriteName = value; }
        }

        public bool IsAnimating
        {
            get { return m_animate; }
        }

        public bool IsLooping
        {
            get { return m_loop; }
        }

        public int CurrentFrame
        {
            get 
            {
                if (m_currentFrameIndex > TotalFrames)
                    return TotalFrames;
                return m_currentFrameIndex; 
            }
        }

        public int TotalFrames // Only returns the largest number of frames in the object list.
        {
            get
            {
                int totalFrames = 1;
                foreach (GameObj obj in _objectList)
                {
                    IAnimateableObj animatableObj = obj as IAnimateableObj;
                    if (animatableObj != null && obj.Visible == true)
                    {
                        int largestNumberOfFrames = animatableObj.TotalFrames;
                        if (largestNumberOfFrames > totalFrames)
                            totalFrames = largestNumberOfFrames;
                    }
                }
                return totalFrames;
                //if (m_charDataStartIndex > -1)
                //    return (_objectList[m_charDataStartIndex] as IAnimateableObj).TotalFrames;
                //return 0;
            }
        }

        public int AnimationSpeed { get; set; }

        public List<string> SpritesheetNameList
        {
            get { return m_spritesheetNameList; }
        }

        public override float ScaleX
        {
            set { _scale.X = value; CalculateBounds(); }
            get { return _scale.X; }
        }

        public override float ScaleY
        {
            set { _scale.Y = value; CalculateBounds(); }
            get { return _scale.Y; }
        }

        public override Vector2 Scale
        {
            set { _scale = value; CalculateBounds(); }
            get { return _scale; }
        }

        public override SpriteEffects Flip
        {
            get { return base.Flip; }
            set
            {
                base.Flip = value;
                CalculateBounds();
            }
        }

        public bool IsPaused
        {
            get { return IsAnimating == false && m_wasAnimating == true; }
        } 
    }
}
