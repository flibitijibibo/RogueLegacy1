using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace InputSystem
{
    public enum PadTypes
    {
        Keyboard = 0,
        GamePad,
    }

    public enum ThumbStick
    {
        LeftStick = 0,
        RightStick,
    }

    // This file used to have Nuclex stuff everywhere. It's gone now. -flibit

    public class InputManager
    {
        // Only currently supports one non-Xbox 360 controller.
        private const int MaxInputs = 4;

        private static KeyboardState[] CurrentKeyboardStates;
        private static GamePadState[] CurrentGamePadStates;
        private static KeyboardState[] PreviousKeyboardStates; // Previous states are used to determine if a button was just pressed or held.
        private static GamePadState[] PreviousGamePadStates;

        private static MouseState CurrentMouseState;
        private static MouseState PreviousMouseState;

        public static List<GestureSample> Taps = new List<GestureSample>();
        public static List<GestureSample> Drags = new List<GestureSample>();
        public static List<GestureSample> Pinches = new List<GestureSample>();
        public static float PinchDelta;

        private static bool[] WasConnected;
        private static PadTypes[] PadType;

        public static float Deadzone = 0;

        public InputManager()
        {
        }

        public static void Initialize()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            PreviousKeyboardStates = new KeyboardState[MaxInputs];
            PreviousGamePadStates = new GamePadState[MaxInputs];

            WasConnected = new bool[MaxInputs];
            PadType = new PadTypes[MaxInputs];

            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag;
        }

        public static void InitializeDXManager(GameServiceContainer service, GameWindow window)
        {
            // No-op for FNA
        }

        public static void RemapDXPad(Buttons[] buttonList)
        {
            // No-op for FNA
        }

        public static bool GamePadIsConnected(PlayerIndex playerIndex)
        {
            return GamePad.GetState(playerIndex).IsConnected;
        }

        public static bool XInputPadIsConnected(PlayerIndex playerIndex)
        {
            return GamePad.GetState(playerIndex).IsConnected;
        }

        public static bool DXPadConnected()
        {
            return false;
        }

        public static PadTypes GetPadType(PlayerIndex playerIndex)
        {
            return PadType[(int)playerIndex];
        }

        public static void SetPadType(PlayerIndex playerIndex, PadTypes padType)
        {
            PadType[(int)playerIndex] = padType;
        }

        public static Vector2 GetThumbStickDirection(ThumbStick stick, PlayerIndex playerIndex)
        {
            if (stick == ThumbStick.LeftStick)
                return CurrentGamePadStates[(int)playerIndex].ThumbSticks.Left;
            else
                return CurrentGamePadStates[(int)playerIndex].ThumbSticks.Right;
        }

        public static void Update(GameTime gameTime)
        {
            // Xinput and keyboard updeate code.
            for (int i = 0; i < MaxInputs; i++)
            {
                PreviousKeyboardStates[i] = CurrentKeyboardStates[i];
                PreviousGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.Circular);
                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                //if (XInputPadIsConnected((PlayerIndex)i))
                //{
                //    WasConnected[i] = true;
                //}
            }

            // Mouse input code.
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            // Touch input code.
            Taps.Clear();
            Drags.Clear();
            Pinches.Clear();

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                switch (gesture.GestureType)
                {
                    case GestureType.Tap:
                        Taps.Add(gesture);
                        break;
                    case GestureType.FreeDrag:
                        Drags.Add(gesture);
                        break;
                    case GestureType.Pinch:
                        Pinches.Add(gesture);

                        Vector2 a = gesture.Position;
                        Vector2 aOld = gesture.Position - gesture.Delta;
                        Vector2 b = gesture.Position2;
                        Vector2 bOld = gesture.Position2 - gesture.Delta2;

                        float d = Vector2.Distance(a, b);
                        float dOld = Vector2.Distance(aOld, bOld);

                        PinchDelta = d - dOld;
                        break;
                }
            }
        }

        public static bool AnyKeyPressed()
        {
            return Keyboard.GetState().GetPressedKeys().Length > 0;
        }

        public static bool AnyMousePressed()
        {
            return MouseLeftJustPressed() || MouseRightJustPressed() || MouseMiddleJustPressed();
        }

        public static bool AnyButtonPressed(PlayerIndex playerIndex)
        {
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) == true)
                    return true;
            }
            return false;
        }

        public static bool MouseLeftPressed()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool MouseLeftJustPressed()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
        }

        public static bool MouseLeftJustReleased()
        {
            return CurrentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool MouseRightPressed()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool MouseRightJustPressed()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
        }

        public static bool MouseRightJustReleased()
        {
            return CurrentMouseState.RightButton == ButtonState.Released && PreviousMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool MouseMiddlePressed()
        {
            return CurrentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public static bool MouseMiddleJustPressed()
        {
            return CurrentMouseState.MiddleButton == ButtonState.Pressed && PreviousMouseState.MiddleButton == ButtonState.Released;
        }

        public static bool MouseMiddleJustReleased()
        {
            return CurrentMouseState.MiddleButton == ButtonState.Released && PreviousMouseState.MiddleButton == ButtonState.Pressed;
        }

        public static int MouseX
        {
            get { return CurrentMouseState.X; }
        }

        public static int MouseY
        {
            get { return CurrentMouseState.Y; }
        }

        public static int MouseMiddleScroll
        {
            get { return CurrentMouseState.ScrollWheelValue; }
        }

        /// <summary>
        /// Returns whether the keyboard key is being held down or not.
        /// </summary>
        public static bool Pressed(Keys key, PlayerIndex playerIndex)
        {
            return CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key);
        }

        public static bool Pressed(Buttons button, PlayerIndex playerIndex)
        {
            // All this extra code is to support deadzones.
            bool isButtonDown = CurrentGamePadStates[(int)playerIndex].IsButtonDown(button);
            bool isThumbstick = false;
            Vector2 thumbstickAmount = GetThumbstickState(button, playerIndex, out isThumbstick);
            if (isThumbstick == false)
                return isButtonDown;
            else
            {
                if (ThumbstickMovementIsSignificant(thumbstickAmount, Deadzone) && isButtonDown == true)
                    return true;
            }
            return false;
        }

        public static bool PreviousStatePressed(Buttons button, PlayerIndex playerIndex)
        {
            // All this extra code is to support deadzones.
            bool isButtonDown = PreviousGamePadStates[(int)playerIndex].IsButtonDown(button);
            bool isThumbstick = false;
            Vector2 thumbstickAmount = GetPreviousThumbstickState(button, playerIndex, out isThumbstick);
            if (isThumbstick == false)
                return isButtonDown;
            else
            {
                if (ThumbstickMovementIsSignificant(thumbstickAmount, Deadzone) && isButtonDown == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public static bool JustPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        PreviousKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (JustPressed(key, PlayerIndex.One, out playerIndex) ||
                        JustPressed(key, PlayerIndex.Two, out playerIndex) ||
                        JustPressed(key, PlayerIndex.Three, out playerIndex) ||
                        JustPressed(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Same as above, but used in case you don't care which player pressed the button.
        /// </summary>
        public static bool JustPressed(Keys key, PlayerIndex? controllingPlayer)
        {
            if (controllingPlayer.HasValue)
            {
                int i = (int)controllingPlayer.Value;
                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        PreviousKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (JustPressed(key, PlayerIndex.One) ||
                        JustPressed(key, PlayerIndex.Two) ||
                        JustPressed(key, PlayerIndex.Three) ||
                        JustPressed(key, PlayerIndex.Four));
            }
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public static bool JustPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                //if (GamePadIsConnected(controllingPlayer.Value) == true)
                //{
                //return (CurrentGamePadStates[i].IsButtonDown(button) &&
                //        PreviousGamePadStates[i].IsButtonUp(button));


                //if (XInputPadIsConnected(controllingPlayer.Value) == true)
                //{
                    // This one supports deadzones.
                    return (Pressed(button, controllingPlayer.Value) == true &&
                            PreviousStatePressed(button, controllingPlayer.Value) == false);
                //}
                //else
                //{
                //    return (CurrentDXState.IsButtonDown(button) &&
                //    PreviousDXState.IsButtonUp(button));
                //}
            }
            else
            {
                // Accept input from any player.
                return (JustPressed(button, PlayerIndex.One, out playerIndex) ||
                        JustPressed(button, PlayerIndex.Two, out playerIndex) ||
                        JustPressed(button, PlayerIndex.Three, out playerIndex) ||
                        JustPressed(button, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Same as above, but used in case you don't care which player pressed the button.
        /// </summary>
        public static bool JustPressed(Buttons button, PlayerIndex? controllingPlayer)
        {
            if (controllingPlayer.HasValue)
            {
                int i = (int)controllingPlayer.Value;

                //if (XInputPadIsConnected(controllingPlayer.Value) == true)
                //{
                //return (CurrentGamePadStates[i].IsButtonDown(button) &&
                //        PreviousGamePadStates[i].IsButtonUp(button));

                // This one supports deadzones.
                return (Pressed(button, controllingPlayer.Value) == true &&
                        PreviousStatePressed(button, controllingPlayer.Value) == false);

                //}
                //else
                //{
                //    return (CurrentDXState.IsButtonDown(button) &&
                //    PreviousDXState.IsButtonUp(button));
                //}
            }
            else
            {
                // Accept input from any player.
                return (JustPressed(button, PlayerIndex.One) ||
                        JustPressed(button, PlayerIndex.Two) ||
                        JustPressed(button, PlayerIndex.Three) ||
                        JustPressed(button, PlayerIndex.Four));
            }
        }

        public static bool JustPressed(JoystickInput input, PlayerIndex? controllingPlayer)
        {
            if (controllingPlayer.HasValue)
            {
                int i = (int)controllingPlayer.Value;

                Vector2 currentThumbDirection = Vector2.Zero;
                Vector2 previousThumbDirection = Vector2.Zero;

                if (input.ThumbStick == ThumbStick.LeftStick)
                {
                    currentThumbDirection = CurrentGamePadStates[i].ThumbSticks.Left;
                    previousThumbDirection = PreviousGamePadStates[i].ThumbSticks.Left;
                }
                else
                {
                    currentThumbDirection = CurrentGamePadStates[i].ThumbSticks.Right;
                    previousThumbDirection = PreviousGamePadStates[i].ThumbSticks.Right;
                }

                float currentAngle = -MathHelper.ToDegrees((float)Math.Atan2(currentThumbDirection.Y, currentThumbDirection.X));
                float previousAngle = -MathHelper.ToDegrees((float)Math.Atan2(previousThumbDirection.Y, previousThumbDirection.X));
                bool isSignificant = ThumbstickMovementIsSignificant(currentThumbDirection, Deadzone);
                bool previousIsSignificant = ThumbstickMovementIsSignificant(previousThumbDirection, Deadzone);

                if ((currentAngle > input.Direction - input.Hysteresis && currentAngle < input.Direction + input.Hysteresis) == true && isSignificant == true &&
                    ((previousAngle > input.Direction - input.Hysteresis && previousAngle < input.Direction + input.Hysteresis) == false || previousIsSignificant == false))
                    return true;
                else
                    return false;
            }
            else
                return (JustPressed(input, PlayerIndex.One) ||
                        JustPressed(input, PlayerIndex.Two) ||
                        JustPressed(input, PlayerIndex.Three) ||
                        JustPressed(input, PlayerIndex.Four));
        }

        public static bool Pressed(JoystickInput input, PlayerIndex playerIndex)
        {
            Vector2 currentThumbDirection = Vector2.Zero;

            if (input.ThumbStick == ThumbStick.LeftStick)
                currentThumbDirection = CurrentGamePadStates[(int)playerIndex].ThumbSticks.Left;
            else
                currentThumbDirection = CurrentGamePadStates[(int)playerIndex].ThumbSticks.Right;

            float currentAngle = -MathHelper.ToDegrees((float)Math.Atan2(currentThumbDirection.Y, currentThumbDirection.X));

            bool isSignificant = ThumbstickMovementIsSignificant(currentThumbDirection, Deadzone);

            return (currentAngle > input.Direction - input.Hysteresis && currentAngle < input.Direction + input.Hysteresis) == true && isSignificant == true;
        }

        public static bool JustTapped()
        {
            return Taps.Count > 0;
        }

        /// <summary>
        /// Resets the control input for a specific player.
        /// </summary>
        /// <param name="playerIndex">The PlayerIndex to reset. If null, all player indices are reset.</param>
        public static void Reset(PlayerIndex? playerIndex)
        {
            if (playerIndex.HasValue)
            {
                CurrentGamePadStates[(int)playerIndex] = new GamePadState();
                CurrentKeyboardStates[(int)playerIndex] = new KeyboardState();
                PreviousGamePadStates[(int)playerIndex] = new GamePadState();
                PreviousKeyboardStates[(int)playerIndex] = new KeyboardState();
                WasConnected[(int)playerIndex] = false;
                PadType[(int)playerIndex] = new PadTypes();
            }
            else
            {
                CurrentKeyboardStates = new KeyboardState[MaxInputs];
                CurrentGamePadStates = new GamePadState[MaxInputs];
                PreviousKeyboardStates = new KeyboardState[MaxInputs];
                PreviousGamePadStates = new GamePadState[MaxInputs];
                WasConnected = new bool[MaxInputs];
                PadType = new PadTypes[MaxInputs];
            }

            CurrentMouseState = new MouseState();
            PreviousMouseState = new MouseState();
        }

        public static Keys[] KeysPressedArray
        {
            get { return Keyboard.GetState().GetPressedKeys(); }
        }

        public static Buttons[] ButtonsPressedArray(PlayerIndex playerIndex)
        {
            List<Buttons> buttonArray = new List<Buttons>();

            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) == true)
                    buttonArray.Add(button);
            }
            return buttonArray.ToArray();
        }

        /// <summary>
        /// Determines if the thumbstickState passed in is significant enough, given the constraint
        /// of how much movement should be ignored.
        /// </summary>
        /// <param name="thumbstickState"></param>
        /// <param name="percentToIgnore">Percentage of the unit-circle which is ignored. If 25.0f, then the inner-quarter of movements will be considered too slight. If 50.0f, then the inner half will be ignored, etc..</param>
        public static bool ThumbstickMovementIsSignificant(Vector2 thumbstickState, float percentToIgnore)
        {
            bool isSignificant = true;
            if (percentToIgnore > 0)
            {
                // Uses pythagorean theorem to see if the hypotenuse ends inside of the "significant" area or not.
                float a = thumbstickState.X;
                float b = thumbstickState.Y;

                // Thumbstick numbers are in a unit-circle so scale the percentToIgnore to the range of 0.0 to 1.0
                float minHyphotenuseLengthForSignificance = (percentToIgnore / 100);

                // This function is likely to be called every tick, so we square the minimum hyptotenuse instead of comparing it the sqrt of a^2 + b^2 (square roots are a bit slow).
                if (((a * a) + (b * b)) < (minHyphotenuseLengthForSignificance * minHyphotenuseLengthForSignificance))
                    isSignificant = false;
            }
            return isSignificant;
        }

        public static Vector2 GetThumbstickState(Buttons button, PlayerIndex playerIndex, out bool isThumbstick)
        {
            switch (button)
            {
                case (Buttons.LeftThumbstickUp):
                case (Buttons.LeftThumbstickDown):
                case (Buttons.LeftThumbstickLeft):
                case (Buttons.LeftThumbstickRight):
                    isThumbstick = true;
                    return CurrentGamePadStates[(int)playerIndex].ThumbSticks.Left;
                case (Buttons.RightThumbstickUp):
                case (Buttons.RightThumbstickDown):
                case (Buttons.RightThumbstickRight):
                case (Buttons.RightThumbstickLeft):
                    isThumbstick = true;
                    return CurrentGamePadStates[(int)playerIndex].ThumbSticks.Right;
            }

            isThumbstick = false;
            return Vector2.Zero;
        }

        public static Vector2 GetPreviousThumbstickState(Buttons button, PlayerIndex playerIndex, out bool isThumbstick)
        {
            switch (button)
            {
                case (Buttons.LeftThumbstickUp):
                case (Buttons.LeftThumbstickDown):
                case (Buttons.LeftThumbstickLeft):
                case (Buttons.LeftThumbstickRight):
                    isThumbstick = true;
                    return PreviousGamePadStates[(int)playerIndex].ThumbSticks.Left;
                case (Buttons.RightThumbstickUp):
                case (Buttons.RightThumbstickDown):
                case (Buttons.RightThumbstickRight):
                case (Buttons.RightThumbstickLeft):
                    isThumbstick = true;
                    return PreviousGamePadStates[(int)playerIndex].ThumbSticks.Right;
            }

            isThumbstick = false;
            return Vector2.Zero;
        }

        private static bool ButtonIsThumbstick(Buttons button)
        {
            switch (button)
            {
                case (Buttons.LeftThumbstickUp):
                case (Buttons.LeftThumbstickDown):
                case (Buttons.LeftThumbstickLeft):
                case (Buttons.LeftThumbstickRight):
                case (Buttons.RightThumbstickUp):
                case (Buttons.RightThumbstickDown):
                case (Buttons.RightThumbstickRight):
                case (Buttons.RightThumbstickLeft):
                    return true;
            }

            return false;
        }
    }

    public struct JoystickInput
    {
        public float Direction;
        public float Hysteresis;
        public int ID;
        public ThumbStick ThumbStick;

        public JoystickInput(ThumbStick thumbstick, float direction, float hysteresis, int id)
        {
            ThumbStick = thumbstick;
            Direction = direction;
            Hysteresis = hysteresis;
            ID = id;
        }
    }
}
