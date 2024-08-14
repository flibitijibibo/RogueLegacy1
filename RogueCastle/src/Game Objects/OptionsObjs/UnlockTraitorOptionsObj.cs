using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class UnlockTraitorOptionsObj : OptionsObj
    {
        private TextObj m_toggleText;
        private byte m_storedState;

        public UnlockTraitorOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_UNLOCK_TRAITOR_OPTIONS") //"Unlock Traitor Class"
        {
            m_toggleText = m_nameText.Clone() as TextObj;
            m_toggleText.X = m_optionsTextOffset;
            m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
            this.AddChild(m_toggleText);
        }

        public override void Initialize()
        {
            UpdateToggleText();
            RefreshTextObjs();
            base.Initialize();
        }

        private void UpdateToggleText()
        {
            if (Game.GameConfig.UnlockTraitor == 2) // 0 = invisible, 1 = no, 2 = yes
                m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_3", m_toggleText); //"Yes"
            else
                m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2)
                || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                SoundManager.PlaySound("frame_swap");
                if (Game.GameConfig.UnlockTraitor == 1)
                    Game.GameConfig.UnlockTraitor = 2;
                else
                    Game.GameConfig.UnlockTraitor = 1;
                UpdateToggleText();
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                SoundManager.PlaySound("Option_Menu_Select");
                this.IsActive = false;
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                Game.GameConfig.UnlockTraitor = m_storedState;
                UpdateToggleText();
                this.IsActive = false;
            }

            base.HandleInput();
        }

        public override void RefreshTextObjs()
        {
            m_nameText.ScaleX = 1;
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.Russian):
                    m_nameText.ScaleX = 0.9f;
                    break;

            }
            base.RefreshTextObjs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_toggleText = null;
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
                {
                    m_storedState = Game.GameConfig.UnlockTraitor;
                    m_toggleText.TextureColor = Color.Yellow;
                }
                else
                    m_toggleText.TextureColor = Color.White;
            }
        }
    }
}
