using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace InputSystem
{
    public class TouchGamepad
    {
        class TouchButton
        {
            internal Rectangle Bounds;
            internal ButtonState PreviousState;
            internal ButtonState State;
            internal string Text;
        }

         Dictionary<Buttons, TouchButton> buttons = new Dictionary<Buttons, TouchButton>();
         Texture2D pixel;
         const float opacity = 0.3f;

        #region Life Cycle

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });
        }

        public void Update()
        {
            TouchCollection touches = TouchPanel.GetState();

            foreach (TouchButton button in buttons.Values)
            {
                button.PreviousState = button.State;
                button.State = ButtonState.Released;

                foreach (TouchLocation touch in touches)
                {
                    if (button.Bounds.Contains((int)touch.Position.X, (int)touch.Position.Y))
                    {
                        button.State = ButtonState.Pressed;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (TouchButton button in buttons.Values)
            {
                Color drawColor = Color.White;
                if (button.State == ButtonState.Pressed)
                    drawColor = Color.Red;

                spriteBatch.Draw(pixel, button.Bounds, drawColor * opacity);

                spriteBatch.DrawString(
                    font,
                    button.Text,
                    new Vector2(
                        button.Bounds.X + (button.Bounds.Width / 2) - 16,
                        button.Bounds.Y + (button.Bounds.Height / 2) - 16
                    ),
                    Color.Yellow,
                    0,
                    Vector2.Zero,
                    4,
                    SpriteEffects.None,
                    0
                );
            }
        }

        #endregion

        #region Getters

        public bool IsDown(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].State == ButtonState.Pressed;
        }

        public bool IsUp(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].State == ButtonState.Released;
        }

        public bool WasDown(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].PreviousState == ButtonState.Pressed;
        }

        public bool WasUp(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].PreviousState == ButtonState.Released;
        }

        public bool JustPressed(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].State == ButtonState.Pressed &&
                buttons[action].PreviousState == ButtonState.Released;
        }

        public bool JustReleased(Buttons action)
        {
            if (!IsValidButton(action))
                return false;

            return buttons[action].State == ButtonState.Released &&
                buttons[action].PreviousState == ButtonState.Pressed;
        }

        #endregion

        #region Setters

        public void AddButton(Buttons action, int x, int y, int width, int height, string text = "")
        {
            buttons.Add(action, new TouchButton
            {
                Bounds = new Rectangle(x, y, width, height),
                Text = text
            });
        }

        public void RemoveButton(Buttons action)
        {
            if (!IsValidButton(action))
                throw new Exception("Attempting to remove button not registered by the touch gamepad!");

            buttons.Remove(action);
        }

        #endregion

        #region Utilities

        bool IsValidButton(Buttons button)
        {
            return buttons.ContainsKey(button);
        }

        #endregion
    }
}
