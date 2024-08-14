using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using InputSystem;

namespace RogueCastle
{
    public class KeyIconObj : ObjContainer
    {
        private TextObj m_buttonText;

        public bool IsGamepadButton { get; internal set; }
        private Keys m_storedKey;
        private Buttons m_storedButton;

        public KeyIconObj()
        {
            SpriteObj bg = new SpriteObj("KeyboardButton_Sprite");
            this.AddChild(bg);
            m_buttonText = new TextObj(Game.PixelArtFont);
            m_buttonText.FontSize = 16;
            m_buttonText.DropShadow = new Vector2(1, 1);
            m_buttonText.Align = Types.TextAlign.Centre;
            m_buttonText.Y = -(m_buttonText.Height / 2);
            this.AddChild(m_buttonText);
            m_buttonText.Visible = false;
        }

        public void SetKey(Keys? key, bool upperCase = true)
        {
            m_storedKey = key.Value;

            m_buttonText.FontSize = 20;
            bool displayText = true; // Needed because some buttons don't use any text, so the text must be hidden.

            switch (key)
            {
                case (Keys.Down):
                    this.GetChildAt(0).ChangeSprite("KeyboardArrowDown_Sprite");
                    displayText = false;
                    break;
                case (Keys.Up):
                    this.GetChildAt(0).ChangeSprite("KeyboardArrowUp_Sprite");
                    displayText = false;
                    break;
                case (Keys.Left):
                    this.GetChildAt(0).ChangeSprite("KeyboardArrowLeft_Sprite");
                    displayText = false;
                    break;
                case (Keys.Right):
                    this.GetChildAt(0).ChangeSprite("KeyboardArrowRight_Sprite");
                    displayText = false;
                    break;
                case (Keys.Space):
                    this.GetChildAt(0).ChangeSprite("KeyboardSpacebar_Sprite");
                    break;
                case (Keys.NumPad0):
                case (Keys.NumPad1):
                case (Keys.NumPad2):
                case (Keys.NumPad3):
                case (Keys.NumPad4):
                case (Keys.NumPad5):
                case (Keys.NumPad6):
                case (Keys.NumPad7):
                case (Keys.NumPad8):
                case (Keys.NumPad9):
                    this.GetChildAt(0).ChangeSprite("KeyboardButtonLong_Sprite");
                    break;
                case (Keys.LeftControl):
                case (Keys.RightControl):
                case (Keys.LeftShift):
                case (Keys.RightShift):
                case (Keys.Tab):
                case (Keys.Enter):
                case (Keys.LeftAlt):
                case (Keys.RightAlt):
                case (Keys.Back):
                case (Keys.Escape):
                    this.GetChildAt(0).ChangeSprite("KeyboardButtonLong_Sprite");
                    m_buttonText.FontSize = 16;
                    break;
                case(Keys.F13):
                    this.GetChildAt(0).ChangeSprite("MouseLeftButton_Sprite");
                    break;
                case(Keys.F14):
                    this.GetChildAt(0).ChangeSprite("MouseRightButton_Sprite");
                    break;
                case(Keys.F15):
                    this.GetChildAt(0).ChangeSprite("MouseMiddleButton_Sprite");
                    break;
                default:
                    this.GetChildAt(0).ChangeSprite("KeyboardButton_Sprite");
                    m_buttonText.FontSize = 24;
                    break;
            }

            if (key.HasValue == false)
                this.GetChildAt(0).ChangeSprite("KeyboardButtonLong_Sprite");

            if (displayText == true)
            {
                if (upperCase == true)
                    m_buttonText.Text = InputReader.GetInputString(key, false, false, false).ToUpper();
                else
                    m_buttonText.Text = InputReader.GetInputString(key, false, false, false);
            }
            else
                m_buttonText.Text = "";

            m_buttonText.Y = -(m_buttonText.Height / 2);
            m_buttonText.Visible = true;
            IsGamepadButton = false;

            this.CalculateBounds();
        }

        public void SetButton(Buttons button)
        {
            m_storedButton = button;

            IsGamepadButton = true;
            m_buttonText.Visible = false;

            switch (button)
            {
                case (Buttons.A):
                    this.GetChildAt(0).ChangeSprite("GamepadAButton_Sprite");
                    break;
                case (Buttons.B):
                    this.GetChildAt(0).ChangeSprite("GamepadBButton_Sprite");
                    break;
                case (Buttons.X):
                    this.GetChildAt(0).ChangeSprite("GamepadXButton_Sprite");
                    break;
                case (Buttons.Y):
                    this.GetChildAt(0).ChangeSprite("GamepadYButton_Sprite");
                    break;
                case (Buttons.Start):
                    this.GetChildAt(0).ChangeSprite("GamepadStartButton_Sprite");
                    break;
                case (Buttons.Back):
                    this.GetChildAt(0).ChangeSprite("GamepadBackButton_Sprite");
                    break;
                case (Buttons.DPadDown):
                    this.GetChildAt(0).ChangeSprite("GamepadDownArrow_Sprite");
                    break;
                case (Buttons.DPadLeft):
                    this.GetChildAt(0).ChangeSprite("GamepadLeftArrow_Sprite");
                    break;
                case (Buttons.DPadRight):
                    this.GetChildAt(0).ChangeSprite("GamepadRightArrow_Sprite");
                    break;
                case (Buttons.DPadUp):
                    this.GetChildAt(0).ChangeSprite("GamepadUpArrow_Sprite");
                    break;
                case (Buttons.LeftShoulder):
                    this.GetChildAt(0).ChangeSprite("GamepadLeftButton_Sprite");
                    break;
                case (Buttons.LeftTrigger):
                    this.GetChildAt(0).ChangeSprite("GamepadLeftTrigger_Sprite");
                    break;
                case (Buttons.RightShoulder):
                    this.GetChildAt(0).ChangeSprite("GamepadRightButton_Sprite");
                    break;
                case (Buttons.RightTrigger):
                    this.GetChildAt(0).ChangeSprite("GamepadRightTrigger_Sprite");
                    break;
                case (Buttons.RightStick):
                    this.GetChildAt(0).ChangeSprite("GamepadRightStick_Sprite");
                    break;
                case (Buttons.LeftStick):
                    this.GetChildAt(0).ChangeSprite("GamepadLeftStick_Sprite");
                    break;
            }

            this.CalculateBounds();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new KeyIconObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            KeyIconObj icon = obj as KeyIconObj;
            if (IsGamepadButton == true)
                icon.SetButton(this.Button);
            else
                icon.SetKey(this.Key);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_buttonText = null;
                base.Dispose();
            }
        }

        public Keys Key
        {
            get { return m_storedKey; }
        }

        public Buttons Button
        {
            get { return m_storedButton; }
        }
    }
}
