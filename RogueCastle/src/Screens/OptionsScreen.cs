using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Input;
using InputSystem;

namespace RogueCastle
{
    public class OptionsScreen : Screen
    {
        public float BackBufferOpacity { get; set; }
        private OptionsObj m_selectedOption;
        private int m_selectedOptionIndex = 0;
        private bool m_changingControls = false;

        private List<OptionsObj> m_optionsArray;

        private ObjContainer m_bgSprite;
        private bool m_transitioning = false;

        //private OptionsObj m_deleteSaveObj;
        private OptionsObj m_backToMenuObj;

        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_cancelText;
        private KeyIconTextObj m_navigationText;

        private SpriteObj m_optionsTitle;
        private SpriteObj m_changeControlsTitle;

        private SpriteObj m_optionsBar;
        private TextObj m_quickDropText;
        private OptionsObj m_quickDropObj;
        private OptionsObj m_reduceQualityObj;
        private OptionsObj m_enableSteamCloudObj;
        private OptionsObj m_unlockTraitorObj;
        private int m_unlockTraitorIndex = 0;

        bool m_titleScreenOptions;

        public OptionsScreen()
        {
            m_optionsArray = new List<OptionsObj>();
            this.UpdateIfCovered = true;
            this.DrawIfCovered = true;
            m_titleScreenOptions = true;
        }

        public override void LoadContent()
        {
            m_bgSprite = new ObjContainer("SkillUnlockPlate_Character");
            m_bgSprite.ForceDraw = true;

            m_optionsTitle = new SpriteObj("OptionsScreenTitle_Sprite");
            m_bgSprite.AddChild(m_optionsTitle);
            m_optionsTitle.Position = new Vector2(0, -m_bgSprite.Width / 2f + 60);

            m_changeControlsTitle = new SpriteObj("OptionsScreenChangeControls_Sprite");
            m_bgSprite.AddChild(m_changeControlsTitle);
            m_changeControlsTitle.Position = new Vector2(1320, m_optionsTitle.Y);

            if (!(Environment.GetEnvironmentVariable("SteamTenfoot") == "1" || Environment.GetEnvironmentVariable("SteamDeck") == "1"))
            {
                m_optionsArray.Add(new ResolutionOptionsObj(this));
                m_optionsArray.Add(new FullScreenOptionsObj(this));
            }
            m_reduceQualityObj = new ReduceQualityOptionsObj(this);
            m_optionsArray.Add(m_reduceQualityObj);
            m_optionsArray.Add(new MusicVolOptionsObj(this));
            m_optionsArray.Add(new SFXVolOptionsObj(this));
            m_quickDropObj = new QuickDropOptionsObj(this);
            m_optionsArray.Add(m_quickDropObj);
            m_optionsArray.Add(new DeadZoneOptionsObj(this));
            m_optionsArray.Add(new ChangeControlsOptionsObj(this));
            m_unlockTraitorObj = new UnlockTraitorOptionsObj(this);
            m_unlockTraitorObj.X = 420;
            m_unlockTraitorIndex = m_optionsArray.Count;
            //m_optionsArray.Add(m_unlockTraitorObj); // Added at OnEnter()

//            m_enableSteamCloudObj = new SteamCloudOptionsObj(this);
//#if STEAM
//            m_optionsArray.Add(m_enableSteamCloudObj);
//#endif
            //m_deleteSaveObj = new DeleteSaveOptionsObj(this);
            //m_optionsArray.Add(m_deleteSaveObj);

            m_optionsArray.Add(new LanguageOptionsObj(this));

            m_optionsArray.Add(new ExitProgramOptionsObj(this));
            m_backToMenuObj = new BackToMenuOptionsObj(this);
            m_backToMenuObj.X = 420;

            for (int i = 0; i < m_optionsArray.Count; i++)
            {
                m_optionsArray[i].X = 420;
                m_optionsArray[i].Y = 160 + (i * 30);
            }

            //m_backToMenuObj.Position = m_deleteSaveObj.Position;

            m_optionsBar = new SpriteObj("OptionsBar_Sprite");
            m_optionsBar.ForceDraw = true;
            m_optionsBar.Position = new Vector2(m_optionsArray[0].X - 20, m_optionsArray[0].Y);

            m_confirmText = new KeyIconTextObj(Game.JunicodeFont);
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_confirmText); // dummy locID to add TextObj to language refresh list
            m_confirmText.DropShadow = new Vector2(2, 2);
            m_confirmText.FontSize = 12;
            m_confirmText.Align = Types.TextAlign.Right;
            m_confirmText.Position = new Vector2(1290, 570);
            m_confirmText.ForceDraw = true;

            m_cancelText = new KeyIconTextObj(Game.JunicodeFont);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_cancelText); // dummy locID to add TextObj to language refresh list
            m_cancelText.Align = Types.TextAlign.Right;
            m_cancelText.DropShadow = new Vector2(2, 2);
            m_cancelText.FontSize = 12;
            m_cancelText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 40);
            m_cancelText.ForceDraw = true;

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_navigationText); // dummy locID to add TextObj to language refresh list
            m_navigationText.Align = Types.TextAlign.Right;
            m_navigationText.DropShadow = new Vector2(2, 2);
            m_navigationText.FontSize = 12;
            m_navigationText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 80);
            m_navigationText.ForceDraw = true;

            m_quickDropText = new TextObj(Game.JunicodeFont);
            m_quickDropText.FontSize = 8;
            m_quickDropText.Text = "*Quick drop allows you to drop down ledges and down-attack in \nthe air by pressing DOWN";
            m_quickDropText.Position = new Vector2(420, 620);
            m_quickDropText.ForceDraw = true;
            m_quickDropText.DropShadow = new Vector2(2, 2);
            base.LoadContent();
        }

        public override void PassInData(List<object> objList)
        {
            m_titleScreenOptions = (bool)objList[0];
            base.PassInData(objList);
        }

        public override void OnEnter()
        {
            RefreshTextObjs();

            m_quickDropText.Visible = false;

            if (InputManager.GamePadIsConnected(PlayerIndex.One))
            {
                m_confirmText.ForcedScale = new Vector2(0.7f, 0.7f);
                m_cancelText.ForcedScale = new Vector2(0.7f, 0.7f);
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_2_NEW", m_navigationText);
            }
            else
            {
                m_confirmText.ForcedScale = new Vector2(1f, 1f);
                m_cancelText.ForcedScale = new Vector2(1f, 1f);
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_3", m_navigationText);
            }
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_4_NEW", m_confirmText);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_5_NEW", m_cancelText);

            m_confirmText.Opacity = 0;
            m_cancelText.Opacity = 0;
            m_navigationText.Opacity = 0;

            Tween.To(m_confirmText, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_navigationText, 0.2f, Tween.EaseNone, "Opacity", "1");

            Tween.RunFunction(0.1f, typeof(SoundManager), "PlaySound", "DialogueMenuOpen");
            //SoundManager.PlaySound("DialogueMenuOpen");


            if (m_optionsArray.Contains(m_backToMenuObj) == false)
                m_optionsArray.Insert(m_optionsArray.Count - 1, m_backToMenuObj);
            if (m_titleScreenOptions == true)
                m_optionsArray.RemoveAt(m_optionsArray.Count - 2); // Remove the second last entry because the last entry is "Exit Program"

            // Adding the traitor menu if the traitor has been unlocked.  Will be removed on menu exit.
            if (Game.GameConfig.UnlockTraitor > 0)
                m_optionsArray.Insert(m_unlockTraitorIndex, m_unlockTraitorObj);

            m_transitioning = true;
            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0.8");

            //(m_optionsArray[0] as ResolutionOptionsObj).Initialize(); // The resolutionObj needs to be initialized every time.

            m_selectedOptionIndex = 0;
            m_selectedOption = m_optionsArray[m_selectedOptionIndex];
            m_selectedOption.IsActive = false;

            m_bgSprite.Position = new Vector2(1320 / 2f, 0);
            m_bgSprite.Opacity = 0;
            Tween.To(m_bgSprite, 0.5f, Tweener.Ease.Quad.EaseOut, "Y", (720 / 2f).ToString());
            Tween.AddEndHandlerToLastTween(this, "EndTransition");
            Tween.To(m_bgSprite, 0.2f, Tween.EaseNone, "Opacity", "1");

            int counter = 0;
            foreach (OptionsObj obj in m_optionsArray)
            {
                obj.Y = 160 + (counter * 30) - (720 / 2f);
                obj.Opacity = 0;
                Tween.By(obj, 0.5f, Tweener.Ease.Quad.EaseOut, "Y", (720 / 2f).ToString());
                Tween.To(obj, 0.2f, Tween.EaseNone, "Opacity", "1");
                obj.Initialize();
                counter++;
            }

            m_optionsBar.Opacity = 0;
            Tween.To(m_optionsBar, 0.2f, Tween.EaseNone, "Opacity", "1");

            base.OnEnter();
        }

        public void EndTransition()
        {
            m_transitioning = false;
        }

        private void ExitTransition()
        {
            SoundManager.PlaySound("DialogMenuClose");

            m_transitioning = true;


            Tween.To(m_confirmText, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_navigationText, 0.2f, Tween.EaseNone, "Opacity", "0");

            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0");
            Tween.To(m_optionsBar, 0.2f, Tween.EaseNone, "Opacity", "0");

            m_bgSprite.Position = new Vector2(1320 / 2f, 720 / 2f);
            m_bgSprite.Opacity = 1;
            Tween.To(m_bgSprite, 0.5f, Tweener.Ease.Quad.EaseOut, "Y", "0");
            Tween.To(m_bgSprite, 0.2f, Tween.EaseNone, "Opacity", "0");

            int counter = 0;
            foreach (OptionsObj obj in m_optionsArray)
            {
                obj.Y = 160 + (counter * 30);
                obj.Opacity = 1;
                Tween.By(obj, 0.5f, Tweener.Ease.Quad.EaseOut, "Y", (-(720 / 2f)).ToString());
                Tween.To(obj, 0.2f, Tween.EaseNone, "Opacity", "0");
                counter++;
            }

            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        public override void OnExit()
        {
            m_selectedOption.IsActive = false;
            m_selectedOption.IsSelected = false;
            m_selectedOption = null;
            (ScreenManager.Game as Game).SaveConfig();
            (ScreenManager as RCScreenManager).UpdatePauseScreenIcons();

            // Remove the unlock traitor option.
            if (m_optionsArray.Contains(m_unlockTraitorObj))
                m_optionsArray.Remove(m_unlockTraitorObj);
            base.OnExit();
        }

        public override void HandleInput()
        {
            if (m_transitioning == false) // No input until the screen is fully displayed.
            {
                if (m_selectedOption.IsActive == true)
                    m_selectedOption.HandleInput();
                else
                {
                    if (m_selectedOption.IsActive == false)
                    {
                        int previousSelectedOptionIndex = m_selectedOptionIndex;

                        if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                        {
                            if (m_selectedOptionIndex > 0)
                                SoundManager.PlaySound("frame_swap");
                            m_selectedOptionIndex--;
                        }
                        else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                        {
                            if (m_selectedOptionIndex < m_optionsArray.Count - 1)
                                SoundManager.PlaySound("frame_swap");
                            m_selectedOptionIndex++;
                        }

                        if (m_selectedOptionIndex < 0)
                            m_selectedOptionIndex = m_optionsArray.Count - 1;
                        if (m_selectedOptionIndex > m_optionsArray.Count - 1)
                            m_selectedOptionIndex = 0;

                        if (previousSelectedOptionIndex != m_selectedOptionIndex)
                        {
                            if (m_selectedOption != null)
                                m_selectedOption.IsSelected = false;

                            m_selectedOption = m_optionsArray[m_selectedOptionIndex];
                            m_selectedOption.IsSelected = true;
                        }
                    }

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    {
                        SoundManager.PlaySound("Option_Menu_Select");
                        m_selectedOption.IsActive = true;
                    }

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_OPTIONS)
                         || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                        ExitTransition();
                }


                if (m_selectedOption == m_quickDropObj)
                {
                    m_quickDropText.Visible = true;
                    m_quickDropText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_1", m_quickDropText, true);
                }
                else if (m_selectedOption == m_reduceQualityObj)
                {
                    m_quickDropText.Visible = true;
                    m_quickDropText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_8", m_quickDropText, true);
                }
                else if (m_selectedOption == m_enableSteamCloudObj)
                {
                    m_quickDropText.Visible = true;
                    m_quickDropText.Text = LocaleBuilder.getString("LOC_ID_OPTIONS_SCREEN_9", m_quickDropText, true);
                }
                else
                    m_quickDropText.Visible = false;
            }
            else
                m_quickDropText.Visible = false;

            base.HandleInput();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (OptionsObj obj in m_optionsArray)
                obj.Update(gameTime);

            m_optionsBar.Position = new Vector2(m_selectedOption.X - 15, m_selectedOption.Y);

            base.Update(gameTime);
        }

        public void ToggleControlsConfig()
        {
            if (m_changingControls == false)
            {
                foreach (OptionsObj obj in m_optionsArray)
                    Tweener.Tween.By(obj, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "-1320");
                Tweener.Tween.By(m_optionsTitle, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "-1320");
                Tweener.Tween.By(m_changeControlsTitle, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "-1320");
                m_changingControls = true;
            }
            else
            {
                foreach (OptionsObj obj in m_optionsArray)
                    Tweener.Tween.By(obj, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "1320");
                Tweener.Tween.By(m_optionsTitle, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "1320");
                Tweener.Tween.By(m_changeControlsTitle, 0.3f, Tweener.Ease.Quad.EaseInOut, "X", "1320");
                m_changingControls = false;
            }
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin();
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);
            m_bgSprite.Draw(Camera);
            foreach (OptionsObj obj in m_optionsArray)
                obj.Draw(Camera);

            m_quickDropText.Draw(Camera);

            m_confirmText.Draw(Camera);
            m_cancelText.Draw(Camera);
            m_navigationText.Draw(Camera);
            m_optionsBar.Draw(Camera);
            Camera.End();

            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Options Screen");

                foreach (OptionsObj obj in m_optionsArray)
                    obj.Dispose();

                m_optionsArray.Clear();
                m_optionsArray = null;
                m_bgSprite.Dispose();
                m_bgSprite = null;
                m_optionsTitle = null;
                m_changeControlsTitle = null;
                //m_deleteSaveObj = null;
                m_backToMenuObj = null;

                m_confirmText.Dispose();
                m_confirmText = null;
                m_cancelText.Dispose();
                m_cancelText = null;
                m_navigationText.Dispose();
                m_navigationText = null;

                m_optionsBar.Dispose();
                m_optionsBar = null;

                m_selectedOption = null;

                m_quickDropText.Dispose();
                m_quickDropText = null;
                m_quickDropObj = null;
                m_enableSteamCloudObj = null;
                m_reduceQualityObj = null;
                m_unlockTraitorObj = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            /*
            if (InputManager.GamePadIsConnected(PlayerIndex.One))
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_OPTIONS_SCREEN_2");
            else
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_OPTIONS_SCREEN_3");
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_OPTIONS_SCREEN_4");
            m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getResourceString("LOC_ID_OPTIONS_SCREEN_5");
             */

            foreach (OptionsObj obj in m_optionsArray)
                obj.RefreshTextObjs();

            Game.ChangeBitmapLanguage(m_optionsTitle, "OptionsScreenTitle_Sprite");
            Game.ChangeBitmapLanguage(m_changeControlsTitle, "OptionsScreenChangeControls_Sprite");

            m_quickDropText.ScaleX = 1;
            switch (LocaleBuilder.languageType)
            {
                case(LanguageType.Russian):
                case(LanguageType.German):
                    m_quickDropText.ScaleX = 0.9f;
                    break;
            }

            base.RefreshTextObjs();
        }
    }
}
