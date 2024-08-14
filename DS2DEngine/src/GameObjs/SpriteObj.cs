using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem;

namespace DS2DEngine
{
    public class SpriteObj : GameObj, IAnimateableObj
    {
        protected Texture2D _sprite = null;
        protected string _spriteName = "";
        protected string m_spritesheetName;
        protected Rectangle _ssRect;
        protected int _frameDelayCounter;
        protected int _frameDuration;
        protected int _frameIndex; // The index of the animation if it did not take delay on frames into account.
        protected int _frameCount;
        protected int _currentFrameIndex = 1; // The index the animation is at if it took delays on frames into account.
        private bool m_atEndFrame = false;
        private float m_totalGameTime = 0;

        protected int _startAnimationIndex;
        protected int _endAnimationIndex;

        protected string m_startLabel;
        protected string m_endLabel;

        protected List<FrameData> m_frameDataList;
        protected List<ImageData> m_imageDataList;
        protected bool _animate = false;
        protected bool _loop = true;
        private bool m_wasAnimating = false;

        private int m_hackFrameRate = 1;

        private float m_timeDelay = 0;
        private float m_timeDelayCounter = 0;

        public bool OverrideParentAnimationDelay = false;

        public Vector2 DropShadow { get; set; }
        private Color m_outlineColour = Color.Black;
        private int m_outlineWidth = 0;

        #region Constructors
        public SpriteObj(string spriteName)
        {
            _spriteName = spriteName;
            
            //Fetching sprite data from Sprite Library.
            m_imageDataList = SpriteLibrary.GetImageDataList(spriteName);
            ImageData imageData = m_imageDataList[0];
            _sprite = SpriteLibrary.GetSprite(spriteName);
            _ssRect = new Rectangle((int)(imageData.SSPos.X), (int)(imageData.SSPos.Y), imageData.Width, imageData.Height);
            AnchorX = imageData.Anchor.X;
            AnchorY = imageData.Anchor.Y;
            
            m_spritesheetName = SpriteLibrary.GetSpritesheetName(spriteName);
            
            //Fetching animation data from Sprite Library.
            m_frameDataList = SpriteLibrary.GetFrameDataList(spriteName);
            _frameCount = m_frameDataList.Count;
            if (_frameCount > 1)
            {
                _frameIndex = 0;
                FrameData fd = m_frameDataList[_frameIndex];
                _frameDuration = fd.Duration * AnimationSpeed;
                _frameDelayCounter = 1;
            }

            TextureColor = Color.White;
            Opacity = 1;

            if (SpriteLibrary.ContainsCharacter(_spriteName))
                throw new Exception("Error: Trying to create a SpriteObj or PhysicsObj using Character Name: '" + _spriteName + "'. Character names are used for obj containers. Try using a sprite name instead.");
        }

        public SpriteObj(Texture2D sprite)
        {
            _sprite = sprite;
            _ssRect = new Rectangle(0,0, _sprite.Width, _sprite.Height);
            _frameIndex = 0;
            _frameDuration = 1;
            _frameDelayCounter = 1;
            _frameCount = 0;

            TextureColor = Color.White;
            Opacity = 1;
        }
        #endregion

        public override void ChangeSprite(string spriteName)
        {
            // Clear out old Sprite information.
            //_sprite = null;
            //m_frameDataList = null;
           // m_imageDataList = null;

            _spriteName = spriteName;

            //Fetching sprite data from Sprite Library.
            m_imageDataList = SpriteLibrary.GetImageDataList(spriteName);
            ImageData imageData = m_imageDataList[0];
            _sprite = SpriteLibrary.GetSprite(spriteName);
            _ssRect = new Rectangle((int)(imageData.SSPos.X), (int)(imageData.SSPos.Y), imageData.Width, imageData.Height);
            AnchorX = imageData.Anchor.X;
            AnchorY = imageData.Anchor.Y;

            //Fetching animation data from Sprite Library.
            m_frameDataList = SpriteLibrary.GetFrameDataList(spriteName);
            _frameCount = m_frameDataList.Count;
            _frameIndex = 0;
            if (_frameCount > 1)
            {
                //_frameIndex = 0;
                FrameData fd = m_frameDataList[_frameIndex];
                _frameDuration = fd.Duration * AnimationSpeed;
                _frameDelayCounter = 1;
            }

            this.StopAnimation();
            //PlayAnimation(1, 1); // What is this for and why was it not commented?
        }

        public override void Draw(Camera2D camera)
        {
            if (_sprite.IsDisposed)
                ReinitializeSprite();

            if (_sprite != null && this.Visible == true)
            {
                if (OutlineWidth > 0 && (Parent == null || Parent.OutlineWidth == 0))
                    DrawOutline(camera);

                if (DropShadow != Vector2.Zero)
                    DrawDropShadow(camera);

                if (Parent == null || OverrideParentScale == true)
                {
                    if (CollisionMath.Intersects(this.Bounds, camera.LogicBounds) || this.ForceDraw == true)
                        camera.Draw(Sprite, AbsPosition, _ssRect, this.TextureColor * Opacity, MathHelper.ToRadians(Rotation), Anchor, Scale, Flip, 1);
                }
                else // Don't do a collision intersect test with the camera bounds here because the parent does it.
                    camera.Draw(Sprite, AbsPosition, _ssRect, this.TextureColor * Opacity, MathHelper.ToRadians(Parent.Rotation + this.Rotation), Anchor, (Parent.Scale * this.Scale), Flip, Layer);

                float elapsedTotalSeconds = camera.ElapsedTotalSeconds;
                if (_animate == true && m_totalGameTime != elapsedTotalSeconds)
                {
                    m_totalGameTime = elapsedTotalSeconds; // Used to make sure if you call draw more than once, it doesn't keep animating.

                    if (camera.GameTime != null)
                        m_timeDelayCounter += elapsedTotalSeconds;// (float)camera.GameTime.ElapsedGameTime.TotalSeconds;

                    if (_frameCount > 1 && m_timeDelayCounter >= AnimationDelay)
                    {
                        m_timeDelayCounter = 0;
                        if (_currentFrameIndex < _endAnimationIndex || IsLooping == true)
                            GoToNextFrame();
                        else
                        {
                            if (m_atEndFrame == false)
                                m_atEndFrame = true;
                            else
                            {
                                m_atEndFrame = false;
                                _animate = false;
                            }
                        }
                    }
                }
            }
        }

        //public override void DrawOutline(Camera2D camera, int width)
        public override void DrawOutline(Camera2D camera)
        {
            if (_sprite.IsDisposed)
                ReinitializeSprite();

            int width = OutlineWidth;
            if (_sprite != null && this.Visible == true)
            {
                if (this.Opacity == 1) // Don't do a collision intersect test with the camera bounds here because the parent does it.
                {
                    // Optimization - cache frequently referenced values
                    Vector2 absPos = AbsPosition;
                    float posX = absPos.X;
                    float posY = absPos.Y;
                    SpriteEffects flip = Flip; 
                    float radianRot = MathHelper.ToRadians(this.Rotation);
                    Color outlineColor = OutlineColour * Opacity;
                    Vector2 anchor = Anchor;
                    float layer = Layer;
                    Texture2D sprite = Sprite;
                    Vector2 scale = this.Scale;

                    if (Parent == null || OverrideParentScale == true)
                    {
                        // Cardinal directions.
                        camera.Draw(sprite, new Vector2(posX - width, posY), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX, posY - width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX, posY + width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        // The corners.
                        camera.Draw(sprite, new Vector2(posX - width, posY - width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY + width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY - width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX - width, posY + width), _ssRect, outlineColor, radianRot, anchor, scale, flip, layer);
                    }
                    else
                    {
                        Vector2 parentScale = Parent.Scale * scale;
                        radianRot = MathHelper.ToRadians(Parent.Rotation + this.Rotation);

                        // Cardinal directions.
                        camera.Draw(sprite, new Vector2(posX - width, posY), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX, posY - width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX, posY + width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        // The corners.
                        camera.Draw(sprite, new Vector2(posX - width, posY - width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY + width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX + width, posY - width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                        camera.Draw(sprite, new Vector2(posX - width, posY + width), _ssRect, outlineColor, radianRot, anchor, parentScale, flip, layer);
                    }
                }
            }
            
            //base.DrawOutline(camera);
        }


        public void DrawDropShadow(Camera2D camera)
        {
            if (this.Visible == true)
            {
                if (Parent == null || OverrideParentScale == true)
                    camera.Draw(Sprite, this.AbsPosition + DropShadow, _ssRect, Color.Black * this.Opacity, MathHelper.ToRadians(this.Rotation), Anchor, (this.Scale), Flip, Layer);
                else
                    camera.Draw(Sprite, this.AbsPosition + DropShadow, _ssRect, Color.Black * this.Opacity, MathHelper.ToRadians(Parent.Rotation + this.Rotation), Anchor, (Parent.Scale * this.Scale), Flip, Layer);
            }
        }


        public void PlayAnimation(bool loopAnimation = true)
        {
            _animate = true;
            _loop = loopAnimation;

            _startAnimationIndex = 1;
            _endAnimationIndex = TotalFrames;
            m_timeDelayCounter = 0;
            GoToFrame(_startAnimationIndex);
            //_currentFrameIndex = 1;
        }

        public void PlayAnimation(int startIndex, int endIndex, bool loopAnimation = false)
        {
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

            _animate = true;
            _loop = loopAnimation;

            _startAnimationIndex = startIndex;
            _endAnimationIndex = endIndex;
            m_timeDelayCounter = 0;
            GoToFrame(startIndex);
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
            if (Sprite.IsDisposed)
                this.ReinitializeSprite();

            for (int i = 0; i < m_frameDataList.Count; i++)
            {
                if (m_frameDataList[i].Label == label)
                    return m_frameDataList[i].Frame;
            }
            return -1;
        }

        public void ResumeAnimation()
        {
            if (m_wasAnimating == true)
                _animate = true;
            m_wasAnimating = false;
        }

        public void PauseAnimation()
        {
            m_wasAnimating = this.IsAnimating;
            _animate = false;
        }

        public void StopAnimation()
        {
            _animate = false;
            m_wasAnimating = false;
        }

        public void GoToNextFrame()
        {
            if (_frameCount > 1) // Only animate if there is more than one frame in the animation.
            {
                if (_loop == true || _currentFrameIndex < _endAnimationIndex) // Only animate if it hasn't reached the end of the animation or loop is equal to true.
                {
                    if (_frameDelayCounter < _frameDuration)
                    {
                        _frameDelayCounter++;
                        _currentFrameIndex++;
                    }
                    else
                    {
                        _frameDelayCounter = 1;
                        _frameIndex++;
                        _currentFrameIndex++;

                        //if (_currentFrameIndex >= _endAnimationIndex)  // This was causing problems with PlayAnimation(string startLabel, string endLabel) so it was changed to > only.
                        if (_currentFrameIndex > _endAnimationIndex) 
                        {
                            // If current frame goes passed endframe, go back to the starting frame.
                            GoToFrame(_startAnimationIndex);
                        }

                        if (Sprite.IsDisposed)
                            this.ReinitializeSprite();

                        FrameData fd = m_frameDataList[_frameIndex];
                        _frameDuration = fd.Duration * AnimationSpeed; // Extends the duration of each frame by 2, turning this into 30 fps animations.
                        ImageData id = m_imageDataList[_frameIndex];
                        _ssRect.X = (int)id.SSPos.X;
                        _ssRect.Y = (int)id.SSPos.Y;
                        _ssRect.Width = id.Width;
                        _ssRect.Height = id.Height;
                        AnchorX = id.Anchor.X;
                        AnchorY = id.Anchor.Y;
                    }
                }
                else if (IsAnimating == true)
                {
                    _animate = false;
                }
            }
        }

        public void GoToFrame(int frameIndex)
        {
            if (Sprite.IsDisposed)
                this.ReinitializeSprite();

            int frameStart = 0;
            int totalFrames = 0;
            foreach (FrameData fd in m_frameDataList)
            {
                totalFrames += fd.Duration * AnimationSpeed; // Remove this 2 to change fps. Hack for now. DO NOT FORGET ABOUT THIS.
                if (totalFrames >= frameIndex)
                {
                    frameStart = fd.Index;
                    break;
                }
            }

            //_frameDelayCounter = totalFrames - frameIndex;

            _frameIndex = frameStart;
            _currentFrameIndex = frameIndex;

            FrameData fd2 = m_frameDataList[_frameIndex];
            _frameDuration = fd2.Duration * AnimationSpeed; // Extends the duration of each frame by 2, turning this into 30 fps animations.

            _frameDelayCounter = _frameDuration - (totalFrames - frameIndex);

            ImageData id = m_imageDataList[_frameIndex];
            _ssRect.X = (int)id.SSPos.X;
            _ssRect.Y = (int)id.SSPos.Y;
            _ssRect.Width = id.Width;
            _ssRect.Height = id.Height;
            AnchorX = id.Anchor.X;
            AnchorY = id.Anchor.Y;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                if (m_spritesheetName == null)
                {
                    if (_sprite.IsDisposed == false)
                        _sprite.Dispose(); // Only dispose sprites that don't have a spritesheet name. Because that means they are manual Texture2Ds passed into the constructor.
                }
                _sprite = null; 
                m_frameDataList = null; // Do not clear out the lists either since they're also linked to the sprite library.
                m_imageDataList = null;

                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new SpriteObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            SpriteObj clone = obj as SpriteObj;
            clone.OutlineColour = this.OutlineColour;
            clone.OutlineWidth = this.OutlineWidth;
            clone.DropShadow = this.DropShadow;
            clone.AnimationDelay = this.AnimationDelay;
        }

        public RenderTarget2D ConvertToTexture(Camera2D camera, bool resizeToPowerOf2 = true, SamplerState samplerState = null)
        {
            SpriteEffects storedFlip = this.Flip;
            this.Flip = SpriteEffects.None;

            int nextPowerOf2Width = this.Width;
            int nextPowerOf2Height = this.Height; 
            Vector2 newScale = new Vector2(1, 1);

            if (resizeToPowerOf2 == true)
            {
                nextPowerOf2Width = CDGMath.NextPowerOf2(Width);
                nextPowerOf2Height = CDGMath.NextPowerOf2(Height);
                newScale = new Vector2(nextPowerOf2Width / (float)Width, nextPowerOf2Height / (float)Height);
            }

            float storedRotation = this.Rotation;
            this.Rotation = 0;

            if (Sprite.IsDisposed)
                this.ReinitializeSprite();

            RenderTarget2D texture = new RenderTarget2D(camera.GraphicsDevice, nextPowerOf2Width, nextPowerOf2Height);
            camera.GraphicsDevice.SetRenderTarget(texture);
            camera.GraphicsDevice.Clear(Color.Transparent);
            camera.Begin(SpriteSortMode.Immediate, null, samplerState, null, null);
            camera.Draw(this.Sprite, this.Anchor, this.SpriteRect, this.TextureColor, this.Rotation, Vector2.Zero, this.Scale * newScale, this.Flip, this.Layer);
            camera.End();
            camera.GraphicsDevice.SetRenderTarget(null);

            this.Rotation = storedRotation;

            this.Flip = storedFlip;
            return texture;
        }

        public void ReinitializeSprite()
        {
            if (SpriteName != "")
            {
                m_imageDataList = SpriteLibrary.GetImageDataList(SpriteName);
                _sprite = SpriteLibrary.GetSprite(SpriteName);
                m_frameDataList = SpriteLibrary.GetFrameDataList(SpriteName);
            }
        }

        public Texture2D Sprite
        { get { return _sprite; } }

        public override int Width
        {
            get { return (int)(_ssRect.Width * ScaleX); }
        }

        public override int Height
        {
            get { return (int)(_ssRect.Height * ScaleY); }
        }

        public int CurrentFrame
        {
            get { return (int)(_currentFrameIndex / AnimationSpeed); }
        }

        public bool IsAnimating
        {
            get { return _animate; }
        }

        public bool IsLooping
        {
            get { return _loop; }
        }

        public int TotalFrames
        {
            get 
            {
                if (Sprite.IsDisposed)
                    this.ReinitializeSprite();

                int totalFrames = 0;
                foreach (FrameData fd in m_frameDataList)
                {
                    totalFrames += fd.Duration * AnimationSpeed; // Remove this 2 to change fps. Hack for now. DO NOT FORGET ABOUT THIS.
                }
                return totalFrames;
            }
        }

        public string SpriteName
        {
            get { return _spriteName; }
            set { _spriteName = value; }
        }

        public int AnimationSpeed
        {
            get
            {
                if (Parent == null)
                    return m_hackFrameRate;
                else
                    return Parent.AnimationSpeed;
            }
            set { m_hackFrameRate = value; }
        }

        public float AnimationDelay
        {
            get
            {
                if (Parent == null || OverrideParentAnimationDelay == true)
                    return m_timeDelay;
                else
                    return Parent.AnimationDelay;
            }
            set { m_timeDelay = value; }
        }


        public string SpritesheetName
        {
            get { return m_spritesheetName; }
        }

        public Rectangle SpriteRect
        {
            get { return _ssRect; }
            set { _ssRect = value; }
        }

        public Color OutlineColour
        {
            get
            {
                if (Parent == null || (Parent != null && Parent.OutlineColour == Color.Black))
                    return m_outlineColour;
                else
                    return Parent.OutlineColour;
            }
            set { m_outlineColour = value; }
        }

        public int OutlineWidth
        {
            get
            {
                if (Parent == null)
                    return m_outlineWidth;
                else
                    return Parent.OutlineWidth;
            }
            set { m_outlineWidth = value; }
        }

        public bool IsPaused
        {
            get { return IsAnimating == false && m_wasAnimating == true; }
        } 
    }
}
