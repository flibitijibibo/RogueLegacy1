using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;

namespace RogueCastle
{
    public class ResolutionOptionsObj : OptionsObj
    {
        private const float ASPECT_RATIO = 1920f / 1080f;
        List<Vector2> m_displayModeList;
        private TextObj m_toggleText;
        private Vector2 m_selectedResolution;
        private int m_selectedResIndex;

        private float m_resetCounter = 0;

        public ResolutionOptionsObj(OptionsScreen parentScreen)
            : base(parentScreen, "LOC_ID_OPTIONS_SCREEN_10") //"Resolution"
        {
            m_toggleText = m_nameText.Clone() as TextObj;
            m_toggleText.X = m_optionsTextOffset;
            m_toggleText.Text = "null";
            this.AddChild(m_toggleText);
        }

        public override void Initialize()
        {
            m_resetCounter = 0;
            m_selectedResolution = new Vector2(m_parentScreen.ScreenManager.Game.GraphicsDevice.Viewport.Width, m_parentScreen.ScreenManager.Game.GraphicsDevice.Viewport.Height);
            //m_selectedResolution = new Vector2((m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferWidth,
            //                                    (m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferHeight);

            if (m_displayModeList != null)
                m_displayModeList.Clear();
            m_displayModeList = (m_parentScreen.ScreenManager.Game as Game).GetSupportedResolutions();
            m_toggleText.Text = m_selectedResolution.X + "x" + m_selectedResolution.Y;

            m_selectedResIndex = 0;
            for (int i = 0; i < m_displayModeList.Count; i++)
            {
                if (m_selectedResolution == m_displayModeList[i])
                {
                    m_selectedResIndex = i;
                    break;
                }
            }
        }

        public override void HandleInput()
        {
            int previousSelectedRes = m_selectedResIndex;
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
            {
                m_selectedResIndex--;
                SoundManager.PlaySound("frame_swap");
            }
            else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                m_selectedResIndex++;
                SoundManager.PlaySound("frame_swap");
            }

            if (m_selectedResIndex < 0)
                m_selectedResIndex = 0;
            if (m_selectedResIndex > m_displayModeList.Count - 1)
                m_selectedResIndex = m_displayModeList.Count - 1;

            if (m_selectedResIndex != previousSelectedRes)
            {
                float displayModeAspectRatio = m_displayModeList[m_selectedResIndex].X / m_displayModeList[m_selectedResIndex].Y;
                if (displayModeAspectRatio == ASPECT_RATIO)
                    m_toggleText.TextureColor = Color.Yellow;
                else
                    m_toggleText.TextureColor = Color.Red;

                m_toggleText.Text = m_displayModeList[m_selectedResIndex].X + "x" + m_displayModeList[m_selectedResIndex].Y;
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                SoundManager.PlaySound("Option_Menu_Select");
                Vector2 newRes = m_displayModeList[m_selectedResIndex];
                if (m_selectedResolution != newRes)
                {
                    (m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferWidth = (int)newRes.X;
                    (m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferHeight = (int)newRes.Y;
                    (m_parentScreen.ScreenManager.Game as Game).graphics.ApplyChanges();
                    (m_parentScreen.ScreenManager as RCScreenManager).ForceResolutionChangeCheck();

                    if ((m_parentScreen.ScreenManager.Game as Game).graphics.IsFullScreen == true)
                    {
                        RCScreenManager manager = m_parentScreen.ScreenManager as RCScreenManager;
                        manager.DialogueScreen.SetDialogue("Resolution Changed");
                        manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                        manager.DialogueScreen.SetConfirmEndHandler(this, "SaveResolution", newRes);
                        manager.DialogueScreen.SetCancelEndHandler(this, "CancelResolution");
                        manager.DisplayScreen(ScreenType.Dialogue, false, null);
                        m_resetCounter = 10;
                    }
                    else
                    {
                        m_selectedResolution = newRes;
                        SaveResolution(newRes);
                    }
                }
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                this.IsActive = false;

            base.HandleInput();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_resetCounter > 0)
            {
                m_resetCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_resetCounter <= 0)
                {
                    RCScreenManager manager = m_parentScreen.ScreenManager as RCScreenManager;
                    manager.HideCurrentScreen();
                    CancelResolution();
                }
            }

            base.Update(gameTime);
        }

        public void SaveResolution(Vector2 resolution)
        {
            Game.GameConfig.ScreenWidth = (int)resolution.X;
            Game.GameConfig.ScreenHeight= (int)resolution.Y;
            m_resetCounter = 0;
            m_selectedResolution = resolution;
            this.IsActive = false;
        }

        public void CancelResolution()
        {
            m_resetCounter = 0;
            (m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferWidth = (int)m_selectedResolution.X;
            (m_parentScreen.ScreenManager.Game as Game).graphics.PreferredBackBufferHeight = (int)m_selectedResolution.Y;
            (m_parentScreen.ScreenManager.Game as Game).graphics.ApplyChanges();
            (m_parentScreen.ScreenManager as RCScreenManager).ForceResolutionChangeCheck();
            m_toggleText.Text = m_selectedResolution.X + "x" + m_selectedResolution.Y;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_toggleText = null;
                if (m_displayModeList != null)
                    m_displayModeList.Clear();
                m_displayModeList = null;
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
                {
                    m_toggleText.TextureColor = Color.White;
                    m_toggleText.Text = m_selectedResolution.X + "x" + m_selectedResolution.Y;
                }
            }
        }
    }
}
