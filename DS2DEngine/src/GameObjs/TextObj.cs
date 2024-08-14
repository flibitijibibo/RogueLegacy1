using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DS2DEngine
{
    public class TextObj : GameObj
    {
#region Fields

        private SpriteFont m_defaultFont = null; // default font for this TextObj
        private SpriteFont m_font = null;        // current font used for drawing
        private string m_text = "";
        private Vector2 m_textSize = Vector2.Zero;
        public Types.TextAlign Align = Types.TextAlign.None;
        private float m_fontSize = 1;
        protected Vector2 m_internalFontSizeScale = new Vector2(1, 1);
        public Vector2 DropShadow { get; set; }
        public int OutlineWidth { get; set; }

        private string m_typewriteText = "";
        private bool m_isTypewriting = false;
        private float m_typewriteSpeed;
        private float m_typewriteCounter;
        private int m_typewriteCharLength;

        private string m_tapSFX = "";

        private bool m_isPaused = false;

        private Color m_outlineColour = Color.Black;
        private int m_fontIndex = -1;
        private Texture2D m_textureValue;

        public bool LimitCorners = false;
        
#endregion

#region Properties

        // language stuff
        public string locStringID { get; set; }

        public SpriteFont defaultFont
        { get { return m_defaultFont; } }

        public bool isLogographic { get; set; }

        public virtual string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                if (Font != null)
                   m_textSize = Font.MeasureString(m_text);
            }
        }

        public SpriteFont Font
        {
            get { return m_font; }
            set
            {
                if (value != null)
                {
                    m_fontIndex = SpriteFontArray.SpriteFontList.IndexOf(value);
                    if (m_fontIndex == -1) throw new Exception("Cannot find font in SpriteFontArray");
                    FieldInfo fieldInfo = value.GetType().GetField("textureValue", BindingFlags.NonPublic | BindingFlags.Instance);
                    m_textureValue = (Texture2D)fieldInfo.GetValue(value);
                }
                m_defaultFont = value;
                m_font = value;
            }
        }

        public override Vector2 Anchor
        {
            get
            {
                if (Align != Types.TextAlign.None)
                {
                    float scaleX = 1 / (this.ScaleX * m_internalFontSizeScale.X);
                    //if (Parent != null) // Not sure why this code breaks things.
                    //    scaleX = 1/(this.ScaleX * Parent.ScaleX * m_internalFontSizeScale.X);

                    Vector2 anchor;
                    if (Align == Types.TextAlign.Left)
                        anchor = new Vector2(0, _anchor.Y);
                    else if (Align == Types.TextAlign.Centre)
                        anchor = new Vector2(this.Width / 2 * scaleX, _anchor.Y);
                    else
                        anchor = new Vector2(this.Width * scaleX, _anchor.Y);

                    if (Flip == SpriteEffects.FlipHorizontally)
                        anchor.X = Width * scaleX - anchor.X;
                    return anchor;
                }
                else
                    return base.Anchor;
            }
            set { base.Anchor = value; }
        }

        public override float AnchorX
        {
            get { return Anchor.X; }
            set { base.AnchorX = value; }
        }

        public override float AnchorY
        {
            get { return _anchor.Y; }
            set { _anchor.Y = value; }
        }

        public override int Width
        {
            get { return (int)(m_textSize.X * ScaleX * m_internalFontSizeScale.X); }
        }

        public override int Height
        {
            get { return (int)(m_textSize.Y * ScaleY * m_internalFontSizeScale.Y); }
        }

        public virtual float FontSize
        {
            get { return m_fontSize; }
            set
            {
                Vector2 minSize = Font.MeasureString("0");
                float newScale = value / minSize.X;
                //Scale = new Vector2(newScale, newScale);
                m_internalFontSizeScale = new Vector2(newScale, newScale);
                m_fontSize = value;
            }
        }

        public bool IsTypewriting
        {
            get { return m_isTypewriting; }
        }

        public bool IsPaused
        {
            get { return m_isPaused; }
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

#endregion

#region Methods

        public TextObj(SpriteFont font = null)
        {
            m_defaultFont = font;
            m_font = font;
            isLogographic = false;
            if (font != null)
            {
                m_fontIndex = SpriteFontArray.SpriteFontList.IndexOf(font);
                if (m_fontIndex == -1) throw new Exception("Cannot find font in SpriteFontArray");
                FieldInfo fieldInfo = font.GetType().GetField("textureValue", BindingFlags.NonPublic | BindingFlags.Instance);
                m_textureValue = (Texture2D)fieldInfo.GetValue(font);
            }
        }

        // This method differs from Font set as it doesn't override default font.
        // It is used specifically for changing languages.
        public void ChangeFontNoDefault(SpriteFont font)
        {
            m_font = font;
            this.FontSize = m_fontSize; // update font sizing

            // Don't recalculate font and text sizes until next Text property set
        }

        public void BeginTypeWriting(float duration, string sound = "")
        {
            m_isTypewriting = true;
            m_typewriteText = this.Text;
            m_typewriteSpeed = duration / this.Text.Length;
            m_typewriteCounter = m_typewriteSpeed;
            m_typewriteCharLength = 0;
            this.Text = "";

            m_tapSFX = sound;
        }

        public void PauseTypewriting()
        {
            m_isPaused = true;
        }

        public void ResumeTypewriting()
        {
            m_isPaused = false;
        }

        public void StopTypeWriting(bool completeText)
        {
            m_isTypewriting = false;
            if (completeText == true)
                this.Text = m_typewriteText;
        }

        //public override void DrawOutline(Camera2D camera, int width)
        public override void DrawOutline(Camera2D camera)
        {
            if (m_textureValue.IsDisposed)
                m_font = SpriteFontArray.SpriteFontList[m_fontIndex];

            int width = OutlineWidth;
            if (m_font != null && this.Visible == true)
            {
                // Optimization - cache frequently referenced values
                Vector2 absPos = AbsPosition;
                float posX = absPos.X;
                float posY = absPos.Y;
                SpriteEffects flip = Flip;
                float radianRot = MathHelper.ToRadians(this.Rotation);
                Color outlineColour = OutlineColour * Opacity;
                Vector2 anchor = Anchor;
                float layer = Layer;
                Vector2 scale = this.Scale * m_internalFontSizeScale;

                //if (this.Opacity == 1) // Don't do a collision intersect test with the camera bounds here because the parent does it.
                {
                    if (Parent == null || OverrideParentScale == true)
                    {
                        // Cardinal directions.
                        camera.DrawString(m_font, m_text, new Vector2(posX - width, posY), outlineColour, radianRot, anchor, scale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX + width, posY),  outlineColour, radianRot, anchor, scale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX, posY - width),  outlineColour, radianRot, anchor, scale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX, posY + width),  outlineColour, radianRot, anchor, scale, flip, layer);
                        // The corners.
                        if (LimitCorners == false)
                        {
                            camera.DrawString(m_font, m_text, new Vector2(posX - width, posY - width), outlineColour, radianRot, anchor, scale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX + width, posY + width), outlineColour, radianRot, anchor, scale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX + width, posY - width), outlineColour, radianRot, anchor, scale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX - width, posY + width), outlineColour, radianRot, anchor, scale, flip, layer);
                        }
                    }
                    else
                    {
                        Vector2 parentScale = Parent.Scale * Scale * m_internalFontSizeScale;
                        radianRot = MathHelper.ToRadians(Parent.Rotation + this.Rotation);

                        // Cardinal directions.
                        camera.DrawString(m_font, m_text, new Vector2(posX - width, posY),  outlineColour, radianRot, anchor, parentScale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX + width, posY),  outlineColour, radianRot, anchor, parentScale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX, posY - width),  outlineColour, radianRot, anchor, parentScale, flip, layer);
                        camera.DrawString(m_font, m_text, new Vector2(posX, posY + width),  outlineColour, radianRot, anchor, parentScale, flip, layer);
                        // The corners.
                        if (LimitCorners == false)
                        {
                            camera.DrawString(m_font, m_text, new Vector2(posX - width, posY - width), outlineColour, radianRot, anchor, parentScale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX + width, posY + width), outlineColour, radianRot, anchor, parentScale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX + width, posY - width), outlineColour, radianRot, anchor, parentScale, flip, layer);
                            camera.DrawString(m_font, m_text, new Vector2(posX - width, posY + width), outlineColour, radianRot, anchor, parentScale, flip, layer);
                        }
                    }
                }
            }
            //base.DrawOutline(camera, width);
        }

        public override void Draw(Camera2D camera)
        {
            if (m_textureValue.IsDisposed)
                m_font = SpriteFontArray.SpriteFontList[m_fontIndex];

            if (IsTypewriting == true && m_typewriteCounter > 0 && IsPaused == false)
            {
                m_typewriteCounter -= (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
                if (m_typewriteCounter <= 0 && this.Text != m_typewriteText)
                {
                    if (m_tapSFX != "")
                        SoundManager.PlaySound(m_tapSFX);

                    m_typewriteCounter = m_typewriteSpeed;
                    m_typewriteCharLength++;
                    this.Text = m_typewriteText.Substring(0, m_typewriteCharLength);
                }
            }

            if (IsTypewriting == true && this.Text == m_typewriteText)
                m_isTypewriting = false;

            if (DropShadow != Vector2.Zero)
                DrawDropShadow(camera);
            if (OutlineWidth > 0 && (Parent == null || Parent.OutlineWidth == 0))
                DrawOutline(camera);
                //DrawOutline(camera, OutlineWidth);

            if (this.Visible == true)
            {
                if (Parent == null || OverrideParentScale == true)
                    camera.DrawString(m_font, m_text, this.AbsPosition, this.TextureColor * this.Opacity, MathHelper.ToRadians(this.Rotation), this.Anchor, this.Scale * m_internalFontSizeScale, SpriteEffects.None, this.Layer);
                else
                    camera.DrawString(m_font, m_text, this.AbsPosition, this.TextureColor * this.Opacity, MathHelper.ToRadians(Parent.Rotation + this.Rotation), Anchor, (Parent.Scale * this.Scale * m_internalFontSizeScale), SpriteEffects.None, Layer); // Flip disabled for now.
            }
        }

        public void DrawDropShadow(Camera2D camera)
        {
            if (this.Visible == true)
            {
                if (Parent == null || OverrideParentScale == true)
                    camera.DrawString(m_font, m_text, this.AbsPosition + DropShadow, Color.Black * this.Opacity, MathHelper.ToRadians(this.Rotation), this.Anchor, this.Scale * m_internalFontSizeScale, SpriteEffects.None, this.Layer);
                else
                    camera.DrawString(m_font, m_text, this.AbsPosition + DropShadow, Color.Black * this.Opacity, MathHelper.ToRadians(Parent.Rotation + this.Rotation), Anchor, (Parent.Scale * this.Scale * m_internalFontSizeScale), SpriteEffects.None, Layer); // Flip disabled for now.
            }
        }

        public virtual void WordWrap(int width)
        {
            if (this.Width > width)
            {
                String line = String.Empty;
                String returnString = String.Empty;
                String[] wordArray;
                
                if (isLogographic)
                    wordArray = this.Text.Select(x => x.ToString()).ToArray();
                else
                    wordArray = this.Text.Split(' ');

                foreach (String word in wordArray)
                {
                    if (this.Font.MeasureString(line + word).X * (ScaleX * m_internalFontSizeScale.X) > width)
                    {
                        returnString = returnString + line + '\n';
                        line = String.Empty;
                    }

                    if (isLogographic) line = line + word;
                    else line = line + word + ' ';
                }

                this.Text = returnString + line;
            }
        }

        public void RandomizeSentence(bool randomizeNumbers)
        {
            MatchCollection matches = Regex.Matches(this.Text, @"\b[\w']*\b");

            foreach (Match match in matches)
            {
                string word = match.Value;
                int result = 0;
                if (randomizeNumbers == true || (randomizeNumbers == false && int.TryParse(word, out result) == false))
                {
                    if (word.Length > 3) // only shuffle words longer than 3 letters.
                    {
                        List<char> charArray = word.ToList();
                        char firstLetter = charArray[0];
                        char lastLetter = charArray[charArray.Count - 1];

                        // Remove the first and last letters from the shuffle array.
                        charArray.RemoveAt(charArray.Count - 1); 
                        charArray.RemoveAt(0);

                        CDGMath.Shuffle<char>(charArray);
                        string shuffledWord = new string(charArray.ToArray());

                        // Add back the first and last letter to the newly shuffled word.
                        shuffledWord = firstLetter + shuffledWord + lastLetter;
                        this.Text = Text.Replace(word, shuffledWord);

                        //char[] charArray = word.ToArray();
                        //CDGMath.Shuffle<char>(charArray);
                        //string shuffledWord = new string(charArray);
                        //this.Text = Text.Replace(word, shuffledWord);
                    }
                }
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            TextObj clone = new TextObj(m_defaultFont);
            clone.ChangeFontNoDefault(this.m_font); // current font may be different from default font, use it!
            return clone;
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            TextObj clone = obj as TextObj;
            clone.Text = this.Text;
            clone.FontSize = this.FontSize;
            clone.OutlineColour = this.OutlineColour;
            clone.OutlineWidth = this.OutlineWidth;
            clone.DropShadow = this.DropShadow;
            clone.Align = this.Align;
            clone.LimitCorners = this.LimitCorners;
        }

        // A way to insert additional Dispose functionality to the engine's TextObj without having to subclass
        public delegate void DisposeDelegate(TextObj textObj);
        public static DisposeDelegate disposeMethod;

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_font = null;
                if (disposeMethod != null) disposeMethod(this);
                base.Dispose();
            }
        }

#endregion
    }
}
