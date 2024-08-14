using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class KeyIconTextObj : TextObj
    {
        private List<KeyIconObj> m_iconList;
        private List<float> m_iconOffset;
        private float m_yOffset = 0;
        private string m_storedTextString;

        public Vector2 ForcedScale { get; set; } // Allows you to force the scale of the icon, but keep the text size the same.

        public override float FontSize
        {
            get { return base.FontSize; }
            set
            {
                base.FontSize = value;
                if (m_storedTextString != null)
                    this.Text = m_storedTextString;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                m_storedTextString = value;
                foreach (KeyIconObj keyIcon in m_iconList)
                    keyIcon.Dispose();
                m_iconList.Clear();
                m_iconOffset.Clear();

                string newText = value;
                int iconIndex = newText.IndexOf("[");
                while (iconIndex != -1)
                {
                    int iconEndex = newText.IndexOf("]");
                    if (iconEndex == -1)
                        throw new Exception("ERROR in KeyIconTextObj: Found starting index for icon but could not find ending index. Make sure every '[' ends with a ']'.");
                    newText = AddKeyIcon(newText, iconIndex, iconEndex);

                    iconIndex = newText.IndexOf("[");
                }

                base.Text = newText;
            }
        }

        public KeyIconTextObj(SpriteFont font = null)
            :base (font)
        {
            ForcedScale = Vector2.One;
            m_iconList = new List<KeyIconObj>();
            m_iconOffset = new List<float>();
        }

        public string AddKeyIcon(string text, int startIndex, int endIndex)
        {
            KeyIconObj icon = new KeyIconObj();
            Vector2 iconOffset = Vector2.Zero;

            string keyIconName = text.Substring(startIndex, endIndex - startIndex + 1); // Should return "[<KEY INFO>]"
            string originalKeyIconName = keyIconName;
            if (keyIconName.Contains("Input"))
            {
                string keyName = keyIconName.Replace("[Input:", "");
                keyName = keyName.Replace("]", ""); // Converting "[<KEY INFO>]" into just <KEY INFO> and storing into var keyName.
                byte inputID = 0;

                try
                {
                    inputID = byte.Parse(keyName);
                }
                catch
                {
                    try
                    {
                        inputID = (byte)typeof(InputMapType).GetField(keyName).GetValue(null);
                    }
                    catch
                    {
                        throw new Exception("Invalid InputMapType");
                    }
                }

                if (InputSystem.InputManager.GamePadIsConnected(PlayerIndex.One) == true)
                {
                    // Tells the icon to use the Dpad icon.
                    //switch (inputID)
                    //{
                    //    case (InputMapType.MENU_DOWN1):
                    //    case (InputMapType.PLAYER_DOWN1):
                    //        inputID = InputMapType.MENU_DOWN2;
                    //        break;
                    //    case (InputMapType.MENU_UP1):
                    //    case (InputMapType.PLAYER_UP1):
                    //        inputID = InputMapType.MENU_UP2;
                    //        break;
                    //    case (InputMapType.MENU_LEFT1):
                    //    case (InputMapType.PLAYER_LEFT1):
                    //        inputID = InputMapType.MENU_LEFT2;
                    //        break;
                    //    case (InputMapType.MENU_RIGHT1):
                    //    case (InputMapType.PLAYER_RIGHT1):
                    //        inputID = InputMapType.MENU_RIGHT2;
                    //        break;
                    //}
                    Buttons inputButton = Game.GlobalInput.ButtonList[inputID];
                    keyIconName = keyIconName.Replace(keyName, inputButton.ToString());
                    keyIconName = keyIconName.Replace("Input", "Button");
                }
                else
                {
                    Keys inputKey = Game.GlobalInput.KeyList[inputID];
                    keyIconName = keyIconName.Replace(keyName, inputKey.ToString());
                    keyIconName = keyIconName.Replace("Input", "Key");
                }
            }

            Vector2 storedScale = this.Scale;
            this.Scale = Vector2.One;
            if (keyIconName.Contains("Key")) // Checks to see if we're dealing with a key or a button.
            {
                string keyName = keyIconName.Replace("[Key:", "");
                keyName = keyName.Replace("]", ""); // Converting "[<KEY INFO>]" into just <KEY INFO> and storing into var keyName.
                bool upperCase = true;
                if (keyName == "Enter" || keyName == "Space")
                    upperCase = false;
                icon.SetKey((Keys)Enum.Parse(typeof(Keys), keyName), upperCase); // Actually setting the KeyIcon to the specified key.
                Vector2 fontSize = Font.MeasureString("0") * m_internalFontSizeScale * this.Scale;
                float iconHeight = fontSize.Y;
                icon.Scale = new Vector2(iconHeight / (float)icon.Height, iconHeight / (float)icon.Height) * ForcedScale; // Modifying the size of the icon to better match the size of the text.
                m_yOffset = iconHeight / 2f;

                string blankSpace = " ";
                while (Font.MeasureString(blankSpace).X * m_internalFontSizeScale.X * this.Scale.X < icon.Width)
                    blankSpace += " ";

                string startingText = text.Substring(0, text.IndexOf("["));
                text = text.Replace(originalKeyIconName, blankSpace); // Replaces "[<KEY INFO]" with the equivalent number of blank spaces to fit the icon.

                float blankSpaceWidth = Font.MeasureString(blankSpace).X * m_internalFontSizeScale.X * this.Scale.X;
                blankSpaceWidth /= 2f;
                float textSpaceWidth = Font.MeasureString(startingText).X * m_internalFontSizeScale.X * this.Scale.X;
                float textPosition = textSpaceWidth + blankSpaceWidth;
                m_iconOffset.Add(textPosition);
            }
            else
            {
                string keyName = keyIconName.Replace("[Button:", "");
                keyName = keyName.Replace("]", ""); // Converting "[<KEY INFO>]" into just <KEY INFO> and storing into var keyName.
                icon.SetButton((Buttons)Enum.Parse(typeof(Buttons), keyName)); // Actually setting the KeyIcon to the specified key.
                Vector2 fontSize = Font.MeasureString("0") * m_internalFontSizeScale * this.Scale;
                float iconHeight = fontSize.Y;
                icon.Scale = new Vector2(iconHeight / (float)icon.Height, iconHeight / (float)icon.Height) *ForcedScale; // Modifying the size of the icon to better match the size of the text.
                m_yOffset = iconHeight / 2f;

                string blankSpace = " ";
                while (Font.MeasureString(blankSpace).X * m_internalFontSizeScale.X * this.Scale.X < icon.Width)
                    blankSpace += " ";

                string startingText = text.Substring(0, text.IndexOf("["));
                text = text.Replace(originalKeyIconName, blankSpace); // Replaces "[<KEY INFO]" with the equivalent number of blank spaces to fit the icon.

                float blankSpaceWidth = Font.MeasureString(blankSpace).X * m_internalFontSizeScale.X * this.Scale.X;
                blankSpaceWidth /= 2f;
                float textSpaceWidth = Font.MeasureString(startingText).X * m_internalFontSizeScale.X * this.Scale.X;
                float textPosition = textSpaceWidth + blankSpaceWidth;
                m_iconOffset.Add(textPosition);
            }
            this.Scale = storedScale;
            m_iconList.Add(icon);
            return text;
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            for (int i = 0; i < m_iconList.Count; i++)
            {
                KeyIconObj icon = m_iconList[i];
                //icon.TextureColor = this.TextureColor;

                switch (this.Align)
                {
                    case (Types.TextAlign.Right):
                        icon.Position = new Vector2(this.AbsX + (m_iconOffset[i] * this.ScaleX) - this.Width, this.AbsY + m_yOffset);
                        break;
                    case (Types.TextAlign.Centre):
                        icon.Position = new Vector2(this.AbsX + (m_iconOffset[i] * this.ScaleX) - this.Width / 2, this.AbsY + m_yOffset);
                        break;
                    case (Types.TextAlign.Left):
                    default:
                        icon.Position = new Vector2(this.AbsX + (m_iconOffset[i] * this.ScaleX), this.AbsY + m_yOffset);
                        break;
                }

                icon.ForceDraw = this.ForceDraw;
                icon.Opacity = this.Opacity;
                icon.Draw(camera);
                icon.Visible = this.Visible;
            }
        }

        // Necessary since word wrap removes all icons and icon offsets from text.
        public override void WordWrap(int width)
        {
            List<KeyIconObj> keyList = new List<KeyIconObj>();
            foreach (KeyIconObj keyIcon in m_iconList)
                keyList.Add(keyIcon.Clone() as KeyIconObj);
            List<float> iconOffsets = new List<float>();
            iconOffsets.AddRange(m_iconOffset);

            base.WordWrap(width);

            m_iconList = keyList;
            m_iconOffset = iconOffsets;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                foreach (KeyIconObj icon in m_iconList)
                    icon.Dispose();

                m_iconList.Clear();
                m_iconList = null;

                m_iconOffset.Clear();
                m_iconOffset = null;
                base.Dispose();
            }
        }
    }
}
