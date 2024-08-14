using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public abstract class OptionsObj : ObjContainer
    {
        protected bool m_isSelected = false;
        protected bool m_isActive = false;

        protected TextObj m_nameText;
        protected OptionsScreen m_parentScreen;
        protected int m_optionsTextOffset = 300;

        public OptionsObj(OptionsScreen parentScreen, string nameLocID)
        {
            m_parentScreen = parentScreen;

            m_nameText = new TextObj(Game.JunicodeFont);
            m_nameText.FontSize = 12;
            m_nameText.Text = LocaleBuilder.getString(nameLocID, m_nameText, true);
            m_nameText.DropShadow = new Vector2(2, 2);
            this.AddChild(m_nameText);

            this.ForceDraw = true;
        }

        public virtual void Initialize() { }

        public virtual void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                SoundManager.PlaySound("Options_Menu_Deselect");
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void RefreshTextObjs() { }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_parentScreen = null;
                m_nameText = null;
                base.Dispose();
            }
        }

        public virtual bool IsActive
        {
            get { return m_isActive; }
            set
            {
                if (value == true)
                    IsSelected = false;
                else
                    IsSelected = true;
                m_isActive = value; 

                if (value == false)
                    (m_parentScreen.ScreenManager.Game as Game).SaveConfig();
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                if (value == true)
                    m_nameText.TextureColor = Color.Yellow;
                else
                    m_nameText.TextureColor = Color.White;
            }
        }
    }
}
