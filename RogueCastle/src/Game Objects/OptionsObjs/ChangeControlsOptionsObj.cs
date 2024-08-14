using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;
using Microsoft.Xna.Framework.Input;

namespace RogueCastle
{
    public class ChangeControlsOptionsObj : OptionsObj
    {
        List<TextObj> m_buttonTitle;
        List<KeyIconTextObj> m_keyboardControls;
        List<KeyIconTextObj> m_gamepadControls;

        private int m_selectedEntryIndex = 0;
        private TextObj m_selectedEntry;

        private KeyIconTextObj m_selectedButton;
        private ObjContainer m_setKeyPlate;

        private bool m_settingKey = false;

        private int[] m_controlKeys;
        private bool m_lockControls = false;

        private int m_startingY = -200;

        private SpriteObj m_selectionBar;

        public ChangeControlsOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_CHANGE_CONTROLS_OPTIONS_1") //"Change Controls"
        {
            m_buttonTitle = new List<TextObj>();
            m_keyboardControls = new List<KeyIconTextObj>();
            m_gamepadControls = new List<KeyIconTextObj>();

            TextObj textTemplate = new TextObj(Game.JunicodeFont);
            textTemplate.FontSize = 12;
            textTemplate.DropShadow = new Vector2(2, 2);

            string[] controlTitleIDs = new string[] {
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_2",  //"Up",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_3",  //"Down",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_4",  //"Left",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_5",  //"Right",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_6",  //"Attack",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_7",  //"Jump",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_8",  //"Special",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_9",  //"Dash Left",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_10", //"Dash Right",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_11", //"Cast Spell",
                "LOC_ID_CHANGE_CONTROLS_OPTIONS_12"  //"Reset Controls"
            };
            m_controlKeys = new int[] {InputMapType.PLAYER_UP1, InputMapType.PLAYER_DOWN1, InputMapType.PLAYER_LEFT1, InputMapType.PLAYER_RIGHT1,  InputMapType.PLAYER_ATTACK,
                                            InputMapType.PLAYER_JUMP1, InputMapType.PLAYER_BLOCK, InputMapType.PLAYER_DASHLEFT, InputMapType.PLAYER_DASHRIGHT, InputMapType.PLAYER_SPELL1, 
                                            -1};

            for (int i = 0; i < controlTitleIDs.Length; i++)
            {
                TextObj textTitle = textTemplate.Clone() as TextObj;
                textTitle.Text = LocaleBuilder.getString(controlTitleIDs[i], textTitle);
                textTitle.X = 1320;
                textTitle.Y = m_startingY + i * 30;
                this.AddChild(textTitle);
                m_buttonTitle.Add(textTitle);

                KeyIconTextObj keyTitle = new KeyIconTextObj(Game.JunicodeFont);
                keyTitle.FontSize = 9;
                keyTitle.X = textTitle.X + 200;
                keyTitle.Y = textTitle.Y + 5;
                this.AddChild(keyTitle);
                m_keyboardControls.Add(keyTitle);

                KeyIconTextObj buttonTitle = new KeyIconTextObj(Game.JunicodeFont);
                buttonTitle.FontSize = 9;
                buttonTitle.X = keyTitle.X + 200;
                buttonTitle.Y = keyTitle.Y;
                this.AddChild(buttonTitle);
                m_gamepadControls.Add(buttonTitle);
            }

            UpdateKeyBindings();

            m_setKeyPlate = new ObjContainer("GameOverStatPlate_Character");
            m_setKeyPlate.ForceDraw = true;
            m_setKeyPlate.Scale = Vector2.Zero;
            TextObj pressAnyKeyText = new TextObj(Game.JunicodeFont);
            pressAnyKeyText.FontSize = 12;
            pressAnyKeyText.Align = Types.TextAlign.Centre;
            pressAnyKeyText.DropShadow = new Vector2(2, 2);
            pressAnyKeyText.ForceDraw = true;
            pressAnyKeyText.Text = LocaleBuilder.getString("LOC_ID_CHANGE_CONTROLS_OPTIONS_14", pressAnyKeyText); //"Press Any Key"
            pressAnyKeyText.Y -= pressAnyKeyText.Height / 2f;
            m_setKeyPlate.AddChild(pressAnyKeyText);

            m_selectionBar = new SpriteObj("OptionsBar_Sprite");
        }

        private void OnEnter()
        {
            m_selectedEntryIndex = 0;
            m_selectedEntry = m_buttonTitle[m_selectedEntryIndex];
            m_selectedEntry.TextureColor = Color.Yellow;
            m_selectedButton = null;

            m_settingKey = false;
            m_lockControls = false;
            m_setKeyPlate.Scale = Vector2.Zero;
            m_setKeyPlate.Position = new Vector2(1320 / 2, 720 / 2);
        }

        private void OnExit()
        {
            if (m_selectedEntry != null)
                m_selectedEntry.TextureColor = Color.White;
            if (m_selectedButton != null)
                m_selectedButton.TextureColor = Color.White;
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (m_settingKey == false)
                {
                    int previousSelectedEntry = m_selectedEntryIndex;

                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        SoundManager.PlaySound("frame_swap");
                        m_selectedEntryIndex--;
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                    {
                        SoundManager.PlaySound("frame_swap");
                        m_selectedEntryIndex++;
                    }

                    if (m_selectedEntryIndex > m_buttonTitle.Count - 1)
                        m_selectedEntryIndex = 0;
                    if (m_selectedEntryIndex < 0)
                        m_selectedEntryIndex = m_buttonTitle.Count - 1;

                    if (previousSelectedEntry != m_selectedEntryIndex)
                    {
                        m_selectedEntry.TextureColor = Color.White;
                        m_selectedEntry = m_buttonTitle[m_selectedEntryIndex];
                        m_selectedEntry.TextureColor = Color.Yellow;
                    }

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    {
                        SoundManager.PlaySound("Option_Menu_Select");

                        m_lockControls = true;
                        if (m_selectedEntryIndex == m_controlKeys.Length - 1)
                        {
                            RCScreenManager manager = Game.ScreenManager;
                            manager.DialogueScreen.SetDialogue("RestoreDefaultControlsWarning");
                            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                            manager.DialogueScreen.SetConfirmEndHandler(this, "RestoreControls");
                            manager.DialogueScreen.SetCancelEndHandler(this, "CancelRestoreControls");
                            manager.DisplayScreen(ScreenType.Dialogue, true);
                        }
                        else
                        {
                            Tweener.Tween.To(m_setKeyPlate, 0.3f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
                            Tweener.Tween.AddEndHandlerToLastTween(this, "SetKeyTrue");
                        }
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                        IsActive = false;

                    base.HandleInput();
                }
                else
                {
                    if (InputManager.AnyButtonPressed(PlayerIndex.One) || InputManager.AnyKeyPressed() || InputManager.AnyMousePressed())
                        ChangeKey();
                }
            }
        }

        public void SetKeyTrue()
        {
            m_settingKey = true;
            m_lockControls = false;
        }

        private void ChangeKey()
        {
            // The player has input an invalid key.
            if (InputManager.AnyKeyPressed() == true)
            {
                Keys keyPressed = InputManager.KeysPressedArray[0];
                if (InputReader.GetInputString(keyPressed, false, false, false).ToUpper() == "")
                    return;

                bool unbindable = false;
                Keys[] unbindableKeys = new Keys[]
                {
                    Keys.Tab, Keys.CapsLock, Keys.LeftShift, Keys.LeftControl, Keys.LeftAlt,
                    Keys.RightAlt, Keys.RightControl, Keys.RightShift, Keys.Enter, Keys.Back,
                    Keys.Space, Keys.Left, Keys.Right, Keys.Up, Keys.Down,
                };

                foreach (Keys unbindableKey in unbindableKeys)
                {
                    if (keyPressed == unbindableKey)
                    {
                        unbindable = true;
                        break;
                    }
                }

                // Unbindable keys.
                if (unbindable == true)
                    return;

                // Cancel changing keys.
                if (keyPressed == Keys.Escape)
                {
                    Tweener.Tween.To(m_setKeyPlate, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                    m_settingKey = false;
                    return;
                }
            }
            else if (InputManager.AnyButtonPressed(PlayerIndex.One) == true)
            {
                Buttons buttonPressed = InputManager.ButtonsPressedArray(PlayerIndex.One)[0];

                // Unbindable buttons.
                if (buttonPressed == Buttons.Start || buttonPressed == Buttons.Back)
                    return;

                bool unbindable = false;
                Buttons[] unbindableButtons = new Buttons[]
                {
                    Buttons.Start, Buttons.Back,
                    Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftThumbstickDown,
                    Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp, Buttons.RightThumbstickDown,
                };

                foreach (Buttons unbindableButton in unbindableButtons)
                {
                    if (buttonPressed == unbindableButton)
                    {
                        unbindable = true;
                        break;
                    }
                }

                // Unbindable buttons.
                if (unbindable == true)
                    return;
            }

            SoundManager.PlaySound("Gen_Menu_Toggle");
            Tweener.Tween.To(m_setKeyPlate, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            m_settingKey = false;

            // Register Gamepad input first.
            if (InputManager.AnyButtonPressed(PlayerIndex.One) == true)
            {
                // a list of buttons that are exempt from being unassigned.
                int[] exemptButtons = new int[] {
                    InputMapType.MENU_CONFIRM1,
                    InputMapType.MENU_CONFIRM2,
                    InputMapType.MENU_CONFIRM3,
                    InputMapType.MENU_CANCEL1,
                    InputMapType.MENU_CANCEL2,
                    InputMapType.MENU_CANCEL3,
                    InputMapType.MENU_CREDITS,
                    InputMapType.MENU_OPTIONS,
                    InputMapType.MENU_PAUSE,
                    InputMapType.MENU_PROFILECARD,
                    InputMapType.MENU_ROGUEMODE,
                };

                Buttons buttonPressed = InputManager.ButtonsPressedArray(PlayerIndex.One)[0];

                // Swap the button from the buttons list. Prevents duplicate button bindings.
                for (int i = 0; i < Game.GlobalInput.ButtonList.Count; i++)
                {
                    // Make sure not to unassign "locked" buttons.
                    bool unassignButton = true;
                    foreach (int exemptButton in exemptButtons)
                    {
                        if (i == exemptButton)
                        {
                            unassignButton = false;
                            break;
                        }
                    }

                    if (unassignButton == true)
                    {
                        if (Game.GlobalInput.ButtonList[i] == buttonPressed)
                            Game.GlobalInput.ButtonList[i] = Game.GlobalInput.ButtonList[m_controlKeys[m_selectedEntryIndex]];
                    }
                }

                Game.GlobalInput.ButtonList[m_controlKeys[m_selectedEntryIndex]] = buttonPressed;
            }
            else if (InputManager.AnyKeyPressed() == true || InputManager.AnyMousePressed())
            {
                Keys keyPressed = Keys.None;
                if (InputManager.AnyMousePressed() == true)
                {
                    if (InputManager.MouseLeftJustPressed())
                        keyPressed = Keys.F13;
                    else if (InputManager.MouseRightJustPressed())
                        keyPressed = Keys.F14;
                    else if (InputManager.MouseMiddleJustPressed())
                        keyPressed = Keys.F15;
                }
                else
                    keyPressed = InputManager.KeysPressedArray[0];

                // a list of keys that are exempt from being unassigned.
                int[] exemptKeys = new int[] {
                    InputMapType.MENU_CONFIRM1,
                    InputMapType.MENU_CONFIRM2,
                    InputMapType.MENU_CONFIRM3,
                    InputMapType.MENU_CANCEL1,
                    InputMapType.MENU_CANCEL2,
                    InputMapType.MENU_CONFIRM3,
                    InputMapType.MENU_CREDITS,
                    InputMapType.MENU_OPTIONS,
                    InputMapType.MENU_PAUSE,
                    InputMapType.MENU_PROFILECARD,
                    InputMapType.MENU_ROGUEMODE,
                };
   
                // Swap the button from the key list. Prevents duplicate key bindings.
                for (int i = 0; i < Game.GlobalInput.KeyList.Count; i++)
                {
                    // Make sure not to unassign "locked" keys.
                    bool unassignKey = true;
                    foreach (int exemptKey in exemptKeys)
                    {
                        if (i == exemptKey)
                        {
                            unassignKey = false;
                            break;
                        }
                    }

                    if (unassignKey == true)
                    {
                        if (Game.GlobalInput.KeyList[i] == keyPressed)
                            Game.GlobalInput.KeyList[i] = Game.GlobalInput.KeyList[m_controlKeys[m_selectedEntryIndex]];
                            //Game.GlobalInput.KeyList[i] = 0;
                    }
                }

                Game.GlobalInput.KeyList[m_controlKeys[m_selectedEntryIndex]] = keyPressed;
            }

            UpdateKeyBindings();
        }

        private void UpdateKeyBindings()
        {
            for (int i = 0; i < m_keyboardControls.Count; i++)
            {
                if (m_controlKeys[i] == -1)
                {
                    m_keyboardControls[i].Text = "";
                    m_gamepadControls[i].Text = "";
                }
                else
                {
                    switch (m_controlKeys[i])
                    {
                        case (InputMapType.PLAYER_DOWN1):
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_DOWN2] + "]";
                            break;
                        case (InputMapType.PLAYER_UP1):
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_UP2] + "]";
                            break;
                        case (InputMapType.PLAYER_LEFT1):
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_LEFT2] + "]";
                            break;
                        case (InputMapType.PLAYER_RIGHT1):
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_RIGHT2] + "]";
                            break;
                        case (InputMapType.PLAYER_JUMP1):
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP2] + "]";
                            break;
                        default:
                            m_keyboardControls[i].Text = "[Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "]";
                            break;
                    }

                    // Just a tiny hack to better centre the mouse icons.
                    switch (Game.GlobalInput.KeyList[m_controlKeys[i]])
                    {
                        case(Keys.F13):
                        case(Keys.F14):
                        case(Keys.F15):
                            switch (m_controlKeys[i])
                            {
                                case (InputMapType.PLAYER_DOWN1):
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_DOWN2] + "]";
                                    break;
                                case (InputMapType.PLAYER_UP1):
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_UP2] + "]";
                                    break;
                                case (InputMapType.PLAYER_LEFT1):
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_LEFT2] + "]";
                                    break;
                                case (InputMapType.PLAYER_RIGHT1):
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_RIGHT2] + "]";
                                    break;
                                case (InputMapType.PLAYER_JUMP1):
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "], [Key:" + Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP2] + "]";
                                    break;
                                default:
                                    m_keyboardControls[i].Text = " [Key:" + Game.GlobalInput.KeyList[m_controlKeys[i]] + "]";
                                    break;
                            }
                            break;
                    }

                    m_gamepadControls[i].Text = "[Button:" + Game.GlobalInput.ButtonList[m_controlKeys[i]] + "]";

                    if (Game.GlobalInput.ButtonList[m_controlKeys[i]] == 0)
                        m_gamepadControls[i].Text = "";

                    if (Game.GlobalInput.KeyList[m_controlKeys[i]] == 0)
                        m_keyboardControls[i].Text = "";
                }
            }

            // Special code so that player attack acts as the second confirm and player jump acts as the second cancel.
            Game.GlobalInput.KeyList[InputMapType.MENU_CONFIRM2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_ATTACK];
            Game.GlobalInput.KeyList[InputMapType.MENU_CANCEL2] = Game.GlobalInput.KeyList[InputMapType.PLAYER_JUMP1];
        }

        public void RestoreControls()
        {
            Game.InitializeGlobalInput();
            UpdateKeyBindings();
            m_lockControls = false;
        }

        public void CancelRestoreControls()
        {
            m_lockControls = false;
        }

        public override void Draw(Camera2D camera)
        {

            base.Draw(camera);
            m_setKeyPlate.Draw(camera);

            if (m_selectedEntry != null)
            {
                m_selectionBar.Position = new Vector2(m_selectedEntry.AbsX - 15, m_selectedEntry.AbsY);
                m_selectionBar.Draw(camera);
            }
        }

        public override void RefreshTextObjs()
        {
            foreach (TextObj textObj in m_buttonTitle)
            {
                textObj.FontSize = 12;
                textObj.ScaleX = 1;
            }

            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.French):
                    m_buttonTitle[7].ScaleX = 0.9f;
                    m_buttonTitle[8].ScaleX = 0.9f;
                    break;
                case (LanguageType.German):
                    m_buttonTitle[9].ScaleX = 0.8f;
                    break;
                case (LanguageType.Portuguese_Brazil):
                    m_buttonTitle[7].ScaleX = 0.9f;
                    break;
                case (LanguageType.Spanish_Spain):
                    m_buttonTitle[7].FontSize = 10;
                    m_buttonTitle[8].FontSize = 10;
                    m_buttonTitle[7].ScaleX = 0.8f;
                    m_buttonTitle[8].ScaleX = 0.8f;
                    break;
            }

            base.RefreshTextObjs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_buttonTitle.Clear();
                m_buttonTitle = null;
                m_keyboardControls.Clear();
                m_keyboardControls = null;
                m_gamepadControls.Clear();
                m_gamepadControls = null;

                m_selectedEntry = null;
                m_selectedButton = null;

                m_setKeyPlate.Dispose();
                m_setKeyPlate = null;

                Array.Clear(m_controlKeys, 0, m_controlKeys.Length);
                m_controlKeys = null;

                m_selectionBar.Dispose();
                m_selectionBar = null;

                base.Dispose();
            }
        }

        public override bool IsActive
        {
            get { return base.IsActive; }
            set
            {
                if (value == true)
                    OnEnter();
                else
                    OnExit();

                if (value != m_isActive)
                    m_parentScreen.ToggleControlsConfig();

                base.IsActive = value;
            }
        }
    }
}
