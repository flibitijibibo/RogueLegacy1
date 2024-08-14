using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;

namespace RogueCastle
{
    public class DeadZoneOptionsObj : OptionsObj
    {
        private SpriteObj m_deadZoneBarBG;
        private SpriteObj m_deadZoneBar;

        public DeadZoneOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_DEAD_ZONE_OPTIONS_1") //"Joystick Dead Zone"
        {
            m_deadZoneBarBG = new SpriteObj("OptionsScreenVolumeBG_Sprite");
            m_deadZoneBarBG.X = m_optionsTextOffset;
            m_deadZoneBarBG.Y = m_deadZoneBarBG.Height / 2f - 2;
            this.AddChild(m_deadZoneBarBG);

            m_deadZoneBar = new SpriteObj("OptionsScreenVolumeBar_Sprite");
            m_deadZoneBar.X = m_deadZoneBarBG.X + 6;
            m_deadZoneBar.Y = m_deadZoneBarBG.Y + 5;
            this.AddChild(m_deadZoneBar);
        }

        public override void Initialize()
        {
            m_deadZoneBar.ScaleX = InputManager.Deadzone / 95f;
            base.Initialize();
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
            {
                if (InputManager.Deadzone - 1 >= 0)
                {
                    InputManager.Deadzone -= 1;
                    UpdateDeadZoneBar();
                }
            }
            else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
            {
                if (InputManager.Deadzone + 1 <= 95)
                {
                    InputManager.Deadzone += 1;
                    UpdateDeadZoneBar();
                }
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) ||
                Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                 || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                IsActive = false;
            }

            base.HandleInput();
        }

        public void UpdateDeadZoneBar()
        {
            m_deadZoneBar.ScaleX = InputManager.Deadzone / 95f;
        }


        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_deadZoneBar = null;
                m_deadZoneBarBG = null;
                base.Dispose();
            }
        }

        public override bool IsActive
        {
            get { return base.IsActive; }
            set
            {
                base.IsActive = value;
                if (value == true)
                    m_deadZoneBar.TextureColor = Color.Yellow;
                else
                    m_deadZoneBar.TextureColor = Color.White;
            }
        }
    }
}
