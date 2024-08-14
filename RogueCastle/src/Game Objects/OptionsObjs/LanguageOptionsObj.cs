using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;

namespace RogueCastle
{
    public class LanguageOptionsObj : OptionsObj
    {
        private LanguageType m_languageType;
        private LanguageType m_storedLanguageType;
        private TextObj m_toggleText;

        public LanguageOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_OPTIONS_LANGUAGE_TITLE")
        {
            m_toggleText = m_nameText.Clone() as TextObj;
            m_toggleText.X = m_optionsTextOffset;
            m_toggleText.Text = "null";
            this.AddChild(m_toggleText);
        }

        public override void Initialize()
        {
            m_languageType = LocaleBuilder.languageType;
            m_storedLanguageType = m_languageType;
            UpdateText();
        }

        private void UpdateText()
        {
            switch (m_languageType)
            {
                case (LanguageType.English):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_ENGLISH", m_toggleText);
                    break;
                case (LanguageType.French):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_FRENCH", m_toggleText);
                    break;
                case (LanguageType.German):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_GERMAN", m_toggleText);
                    break;
                case (LanguageType.Russian):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_RUSSIAN", m_toggleText);
                    break;
                case (LanguageType.Portuguese_Brazil):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_PORTUGUESE", m_toggleText);
                    break;
                case (LanguageType.Spanish_Spain):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_SPANISH", m_toggleText);
                    break;
                case (LanguageType.Polish):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_POLISH", m_toggleText);
                    break;
                case (LanguageType.Chinese_Simp):
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_LANGUAGE_CHINESE", m_toggleText);
                    break;
            }
        }

        private void ChangeLanguageUp()
        {
            int languageType = (int)m_languageType;
            languageType++;
            if (languageType >= (int)LanguageType.MAX)
                languageType = 0;

            m_languageType = (LanguageType)languageType;
            UpdateText();
        }

        private void ChangeLanguageDown()
        {
            int languageType = (int)m_languageType;
            languageType--;
            if (languageType < 0)
                languageType = (int)LanguageType.MAX - 1;

            m_languageType = (LanguageType)languageType;
            UpdateText();
        }

        private void HandleConfirm()
        {
            LocaleBuilder.languageType = m_languageType;
            m_storedLanguageType = m_languageType;
            LocaleBuilder.RefreshAllText();
        }

        private void HandleCancel()
        {
            m_languageType = m_storedLanguageType;
            UpdateText();
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
            {
                SoundManager.PlaySound("frame_swap");
                ChangeLanguageDown();
            }
            else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                SoundManager.PlaySound("frame_swap");
                ChangeLanguageUp();
            }
            else if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                SoundManager.PlaySound("Option_Menu_Select");
                HandleConfirm();
                this.IsActive = false;
            }
            else if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                HandleCancel();
                this.IsActive = false;
            }

            base.HandleInput();
        }

        public override void RefreshTextObjs()
        {
            m_toggleText.ScaleX = 1;
            m_toggleText.FontSize = 12;
            switch (LocaleBuilder.languageType)
            {
                case(LanguageType.German):
                case(LanguageType.Portuguese_Brazil):
                    m_toggleText.ScaleX = 0.9f;
                    break;
                case (LanguageType.Polish):
                    m_toggleText.FontSize = 11;
                    m_toggleText.ScaleX = 0.9f;
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
                    m_toggleText.TextureColor = Color.Yellow;
                else
                    m_toggleText.TextureColor = Color.White;
            }
        }
    }
}
