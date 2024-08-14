using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ReduceQualityOptionsObj : OptionsObj
    {
        private TextObj m_toggleText;

        public ReduceQualityOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_OPTIONS_SCREEN_12") //"Reduce Shader Quality"
        {
            m_toggleText = m_nameText.Clone() as TextObj;
            m_toggleText.X = m_optionsTextOffset;
            m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
            this.AddChild(m_toggleText);
        }

        public override void Initialize()
        {
            if (Game.GameConfig.ReduceQuality == true)
                m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_3", m_toggleText); //"Yes"
            else
                m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
            base.Initialize();
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2)
                || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                SoundManager.PlaySound("frame_swap");
                if (m_toggleText.Text == LocaleBuilder.getResourceString("LOC_ID_QUICKDROP_OPTIONS_2")) // "No"
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_3", m_toggleText); //"Yes"
                else
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                SoundManager.PlaySound("Option_Menu_Select");
                if (m_toggleText.Text == LocaleBuilder.getResourceString("LOC_ID_QUICKDROP_OPTIONS_3")) // "Yes"
                    Game.GameConfig.ReduceQuality = true;
                else
                    Game.GameConfig.ReduceQuality = false;
                this.IsActive = false;
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                if (Game.GameConfig.ReduceQuality == true)
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_3", m_toggleText); //"Yes"
                else
                    m_toggleText.Text = LocaleBuilder.getString("LOC_ID_QUICKDROP_OPTIONS_2", m_toggleText); //"No"
                this.IsActive = false;
            }

            base.HandleInput();
        }

        public override void RefreshTextObjs()
        {
            m_nameText.ScaleX = 1;
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.French):
                case (LanguageType.Spanish_Spain):
                    m_nameText.ScaleX = 0.9f;
                    break;
                case (LanguageType.Portuguese_Brazil):
                case (LanguageType.Russian):
                    m_nameText.ScaleX = 0.8f;
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
