using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class SFXVolOptionsObj : OptionsObj
    {
        private SpriteObj m_volumeBarBG;
        private SpriteObj m_volumeBar;

        public SFXVolOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_SFXVOL_OPTIONS_1") //"SFX Volume"
        {
            m_volumeBarBG = new SpriteObj("OptionsScreenVolumeBG_Sprite");
            m_volumeBarBG.X = m_optionsTextOffset;
            m_volumeBarBG.Y = m_volumeBarBG.Height / 2f - 2;
            this.AddChild(m_volumeBarBG);

            m_volumeBar = new SpriteObj("OptionsScreenVolumeBar_Sprite");
            m_volumeBar.X = m_volumeBarBG.X + 6;
            m_volumeBar.Y = m_volumeBarBG.Y + 5;
            this.AddChild(m_volumeBar);
        }

        public override void Initialize()
        {
            m_volumeBar.ScaleX = SoundManager.GlobalSFXVolume;
            base.Initialize();
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
            {
                SoundManager.GlobalSFXVolume -= 0.01f;
                SetVolumeLevel();
            }
            else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
            {
                SoundManager.GlobalSFXVolume += 0.01f;
                SetVolumeLevel();
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) ||
                Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                 || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                IsActive = false;
            }

            base.HandleInput();
        }

        public void SetVolumeLevel()
        {
            m_volumeBar.ScaleX = SoundManager.GlobalSFXVolume;
            Game.GameConfig.SFXVolume = SoundManager.GlobalSFXVolume;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_volumeBar = null;
                m_volumeBarBG = null;
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
                    m_volumeBar.TextureColor = Color.Yellow;
                else
                    m_volumeBar.TextureColor = Color.White;
            }
        }
    }
}
