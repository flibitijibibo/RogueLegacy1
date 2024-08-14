using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InputSystem
{
    public class InputMap
    {
        private PlayerIndex m_playerIndex;
        private List<Keys> m_keyList;
        private List<Buttons> m_buttonList;
        private List<JoystickInput> m_joystickList;
        private bool m_usingGamePad = false;

        private bool m_isDisposed = false;

        public bool LockKeyboardInput { get; set; }

        public InputMap(PlayerIndex playerIndex, bool useGamePad)
        {
            m_playerIndex = playerIndex;
            m_keyList = new List<Keys>();
            m_buttonList = new List<Buttons>();
            m_joystickList = new List<JoystickInput>();

            m_usingGamePad = useGamePad;
            if ((InputManager.XInputPadIsConnected(playerIndex) || InputManager.DXPadConnected()) && m_usingGamePad == true)
                InputManager.SetPadType(playerIndex, PadTypes.GamePad);
            else
                InputManager.SetPadType(playerIndex, PadTypes.Keyboard);
        }

        public void SwitchInputType(PadTypes padType)
        {
            if (padType == PadTypes.GamePad && (InputManager.XInputPadIsConnected(m_playerIndex) || InputManager.DXPadConnected()))
                InputManager.SetPadType(m_playerIndex, PadTypes.GamePad);
            else
                InputManager.SetPadType(m_playerIndex, PadTypes.Keyboard);
        }

        public void AddInput(int id, Keys key)
        {
            int iterate = id + 1 - m_keyList.Count;
            for (int i = 0; i < iterate; i++)
            {
                m_keyList.Add(new Keys());
            }
            m_keyList[id] = key;
        }

        public void AddInput(int id, Buttons button)
        {
            // when ID is 1 count needs to be 2. otherwise run else.
            int iterate = id + 1 - m_buttonList.Count;
            for (int i = 0; i < iterate; i++)
            {
                m_buttonList.Add(new Buttons());
            }
            m_buttonList[id] = button;
        }

        public void AddInput(int id, ThumbStick thumbstick, float direction, float hysteresis)
        {
            if (id > m_buttonList.Count)
            {
                for (int i = m_buttonList.Count; i < id - m_buttonList.Count; i++)
                {
                    m_buttonList.Add(new Buttons());
                }
            }

            JoystickInput input = new JoystickInput(thumbstick, direction, hysteresis, id);
            m_joystickList.Add(input);
        }

        public bool JustPressed(int id)
        {
            //if (InputManager.GetPadType(m_playerIndex) == PadTypes.GamePad)
            if (id < m_buttonList.Count)
            {
                //if (id >= m_buttonList.Count) return false;

                if (m_buttonList[id] == 0)
                {
                    foreach (JoystickInput input in m_joystickList)
                    {
                        if (input.ID == id)
                        {
                            bool pressed = InputManager.JustPressed(input, m_playerIndex);
                            if (pressed == true) // This is necessary because if it is false, it needs to break out and check the keyboard.
                                return true;
                        }
                    }
                    //return false;
                }
                else if (InputManager.JustPressed(m_buttonList[id], m_playerIndex) == true)
                    return true;
                    //return InputManager.JustPressed(m_buttonList[id], m_playerIndex);
            }

            if (LockKeyboardInput == false)
            {
                if (id >= m_keyList.Count) return false;

                switch (m_keyList[id])
                {
                    case (Keys.F13):
                        return InputManager.MouseLeftJustPressed();
                    case (Keys.F14):
                        return InputManager.MouseRightJustPressed();
                    case (Keys.F15):
                        return InputManager.MouseMiddleJustPressed();
                    default:
                        return InputManager.JustPressed(m_keyList[id], m_playerIndex);
                }
            }

            return false;
        }

        public bool Pressed(int id)
        {
            //if (InputManager.GetPadType(m_playerIndex) == PadTypes.GamePad)
            if (id < m_buttonList.Count)
            {
                //if (id >= m_buttonList.Count) return false;

                if (m_buttonList[id] == 0)
                {
                    foreach (JoystickInput input in m_joystickList)
                    {
                        if (input.ID == id)
                        {
                            bool pressed = InputManager.Pressed(input, m_playerIndex);
                            if (pressed == true) // This is necessary because if it is false, it needs to break out and check the keyboard.
                                return true;
                        }
                    }
                    //return false;
                }
                else if (InputManager.Pressed(m_buttonList[id], m_playerIndex))
                    return true;
            }

            if (LockKeyboardInput == false)
            {
                if (id >= m_keyList.Count) return false;

                switch (m_keyList[id])
                {
                    case (Keys.F13):
                        return InputManager.MouseLeftPressed();
                    case (Keys.F14):
                        return InputManager.MouseRightPressed();
                    case (Keys.F15):
                        return InputManager.MouseMiddlePressed();
                    default:
                        return InputManager.Pressed(m_keyList[id], m_playerIndex);
                }
            }
            return false;
        }

        public void ClearKeyboardList()
        {
            m_keyList.Clear();
        }

        public void ClearGamepadList()
        {
            m_buttonList.Clear();
            m_joystickList.Clear();
        }

        public void ClearAll()
        {
            m_joystickList.Clear();
            m_keyList.Clear();
            m_buttonList.Clear();
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;

                m_joystickList.Clear();
                m_joystickList = null;
                m_keyList.Clear();
                m_keyList = null;
                m_buttonList.Clear();
                m_buttonList = null;
            }
        }

        public PlayerIndex PlayerIndex
        {
            get { return m_playerIndex; }
        }

        public bool UsingGamePad
        {
            get { return m_usingGamePad; }
        }

        public List<Buttons> ButtonList
        {
            get { return m_buttonList; }
        }

        public List<Keys> KeyList
        {
            get { return m_keyList; }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

    }

}
