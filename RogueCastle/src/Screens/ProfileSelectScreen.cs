
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;
using InputSystem;
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class ProfileSelectScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_cancelText;
        private KeyIconTextObj m_navigationText;
        private KeyIconTextObj m_deleteProfileText;

        private SpriteObj m_title;
        private ObjContainer m_slot1Container, m_slot2Container, m_slot3Container;

        private int m_selectedIndex = 0;

        private bool m_lockControls = false;

        private List<ObjContainer> m_slotArray;
        private ObjContainer m_selectedSlot;

        public ProfileSelectScreen()
        {
            m_slotArray = new List<ObjContainer>();
            this.DrawIfCovered = true;
        }

        public override void LoadContent()
        {
            m_title = new SpriteObj("ProfileSelectTitle_Sprite");
            m_title.ForceDraw = true;

            // Template for slot text
            TextObj slotText = new TextObj(Game.JunicodeFont);
            slotText.Align = Types.TextAlign.Centre;
            slotText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_1", slotText);
            slotText.TextureColor = Color.White;
            slotText.OutlineWidth = 2;
            slotText.FontSize = 10;
            slotText.Position = new Vector2(0, -(slotText.Height / 2f));

            m_slot1Container = new ObjContainer("ProfileSlotBG_Container");
            TextObj slot1Text = slotText.Clone() as TextObj;
            slot1Text.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot1Text); // dummy locID to add TextObj to language refresh list
            m_slot1Container.AddChild(slot1Text);
            SpriteObj slot1Title = new SpriteObj("ProfileSlot1Text_Sprite");
            slot1Title.Position = new Vector2(-130, -35);
            m_slot1Container.AddChild(slot1Title);
            TextObj slot1LvlText = slotText.Clone() as TextObj;
            slot1LvlText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot1LvlText); // dummy locID to add TextObj to language refresh list
            slot1LvlText.Position = new Vector2(120, 15);
            m_slot1Container.AddChild(slot1LvlText);
            TextObj slot1NGText = slotText.Clone() as TextObj;
            slot1NGText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot1NGText); // dummy locID to add TextObj to language refresh list
            slot1NGText.Position = new Vector2(-120, 15);
            m_slot1Container.AddChild(slot1NGText);
            m_slot1Container.ForceDraw = true;

            m_slot2Container = new ObjContainer("ProfileSlotBG_Container");
            TextObj slot2Text = slotText.Clone() as TextObj;
            slot2Text.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot2Text); // dummy locID to add TextObj to language refresh list
            m_slot2Container.AddChild(slot2Text);
            SpriteObj slot2Title = new SpriteObj("ProfileSlot2Text_Sprite");
            slot2Title.Position = new Vector2(-130, -35);
            m_slot2Container.AddChild(slot2Title);
            TextObj slot2LvlText = slotText.Clone() as TextObj;
            slot2LvlText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot2LvlText); // dummy locID to add TextObj to language refresh list
            slot2LvlText.Position = new Vector2(120, 15);
            m_slot2Container.AddChild(slot2LvlText);
            TextObj slot2NGText = slotText.Clone() as TextObj;
            slot2NGText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot2NGText); // dummy locID to add TextObj to language refresh list
            slot2NGText.Position = new Vector2(-120, 15);
            m_slot2Container.AddChild(slot2NGText);
            m_slot2Container.ForceDraw = true;

            m_slot3Container = new ObjContainer("ProfileSlotBG_Container");
            TextObj slot3Text = slotText.Clone() as TextObj;
            slot3Text.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot3Text); // dummy locID to add TextObj to language refresh list
            m_slot3Container.AddChild(slot3Text);
            SpriteObj slot3Title = new SpriteObj("ProfileSlot3Text_Sprite");
            slot3Title.Position = new Vector2(-130, -35);
            m_slot3Container.AddChild(slot3Title);
            TextObj slot3LvlText = slotText.Clone() as TextObj;
            slot3LvlText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot3LvlText); // dummy locID to add TextObj to language refresh list
            slot3LvlText.Position = new Vector2(120, 15);
            m_slot3Container.AddChild(slot3LvlText);
            TextObj slot3NGText = slotText.Clone() as TextObj;
            slot3NGText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", slot3NGText); // dummy locID to add TextObj to language refresh list
            slot3NGText.Position = new Vector2(-120, 15);
            m_slot3Container.AddChild(slot3NGText);
            m_slot3Container.ForceDraw = true;

            m_slotArray.Add(m_slot1Container);
            m_slotArray.Add(m_slot2Container);
            m_slotArray.Add(m_slot3Container);

            m_confirmText = new KeyIconTextObj(Game.JunicodeFont);
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_4", m_confirmText);
            m_confirmText.DropShadow = new Vector2(2, 2);
            m_confirmText.FontSize = 12;
            m_confirmText.Align = Types.TextAlign.Right;
            m_confirmText.Position = new Vector2(1290, 570);
            m_confirmText.ForceDraw = true;

            m_cancelText = new KeyIconTextObj(Game.JunicodeFont);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_5", m_cancelText);
            m_cancelText.Align = Types.TextAlign.Right;
            m_cancelText.DropShadow = new Vector2(2, 2);
            m_cancelText.FontSize = 12;
            m_cancelText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 40);
            m_cancelText.ForceDraw = true;

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_2", m_navigationText);
            m_navigationText.Align = Types.TextAlign.Right;
            m_navigationText.DropShadow = new Vector2(2, 2);
            m_navigationText.FontSize = 12;
            m_navigationText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 80);
            m_navigationText.ForceDraw = true;

            m_deleteProfileText = new KeyIconTextObj(Game.JunicodeFont);
            m_deleteProfileText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_6", m_deleteProfileText);
            m_deleteProfileText.Align = Types.TextAlign.Left;
            m_deleteProfileText.DropShadow = new Vector2(2, 2);
            m_deleteProfileText.FontSize = 12;
            m_deleteProfileText.Position = new Vector2(20, m_confirmText.Y + 80);
            m_deleteProfileText.ForceDraw = true;

            base.LoadContent();
        }

        public override void OnEnter()
        {
            SoundManager.PlaySound("DialogOpen");
            m_lockControls = true;
            m_selectedIndex = Game.GameConfig.ProfileSlot - 1;
            m_selectedSlot = m_slotArray[m_selectedIndex];
            m_selectedSlot.TextureColor = Color.Yellow;

            CheckSaveHeaders(m_slot1Container, 1);
            CheckSaveHeaders(m_slot2Container, 2);
            CheckSaveHeaders(m_slot3Container, 3);

            m_deleteProfileText.Visible = true;
            if (m_slotArray[m_selectedIndex].ID == 0)
                m_deleteProfileText.Visible = false;

            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0.9");

            m_title.Position = new Vector2(1320 / 2f, 100);
            m_slot1Container.Position = new Vector2(1320 / 2f, 300);
            m_slot2Container.Position = new Vector2(1320 / 2f, 420);
            m_slot3Container.Position = new Vector2(1320 / 2f, 540);

            TweenInText(m_title, 0);
            TweenInText(m_slot1Container, 0.05f);
            TweenInText(m_slot2Container, 0.1f);
            TweenInText(m_slot3Container, 0.15f);

            Tween.RunFunction(0.5f, this, "UnlockControls");

            if (InputManager.GamePadIsConnected(PlayerIndex.One))
            {
                m_confirmText.ForcedScale = new Vector2(0.7f, 0.7f);
                m_cancelText.ForcedScale = new Vector2(0.7f, 0.7f);
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_2_NEW", m_navigationText);
            }
            else
            {
                m_confirmText.ForcedScale = new Vector2(1f, 1f);
                m_cancelText.ForcedScale = new Vector2(1f, 1f);
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_3", m_navigationText);
            }
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_4_NEW", m_confirmText);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_5_NEW", m_cancelText);
            m_deleteProfileText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_6_NEW", m_deleteProfileText);

            m_confirmText.Opacity = 0;
            m_cancelText.Opacity = 0;
            m_navigationText.Opacity = 0;
            m_deleteProfileText.Opacity = 0;

            Tween.To(m_confirmText, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_navigationText, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_deleteProfileText, 0.2f, Tween.EaseNone, "Opacity", "1");

            Game.ChangeBitmapLanguage(m_title, "ProfileSelectTitle_Sprite");
            Game.ChangeBitmapLanguage(m_slot1Container.GetChildAt(2) as SpriteObj, "ProfileSlot1Text_Sprite");
            Game.ChangeBitmapLanguage(m_slot2Container.GetChildAt(2) as SpriteObj, "ProfileSlot2Text_Sprite");
            Game.ChangeBitmapLanguage(m_slot3Container.GetChildAt(2) as SpriteObj, "ProfileSlot3Text_Sprite");

            base.OnEnter();
        }

        private void CheckSaveHeaders(ObjContainer container, byte profile)
        {
            TextObj slotText = container.GetChildAt(1) as TextObj;
            TextObj slotLvlText = container.GetChildAt(3) as TextObj;
            TextObj slotNGText = container.GetChildAt(4) as TextObj;
            slotLvlText.Text = "";
            slotNGText.Text = "";
            string playerName = null;
            byte playerClass = 0;
            int playerLevel = 0;
            bool isDead = false;
            int timesCastleBeaten = 0;
            bool isFemale = false;

            try
            {
                (ScreenManager.Game as Game).SaveManager.GetSaveHeader(profile, out playerClass, out playerName, out playerLevel, out isDead, out timesCastleBeaten, out isFemale);

                if (playerName == null)
                {
                    slotText.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_1");
                    container.ID = 0; // Container with ID == 0 means it has no save file.
                }
                else
                {
                    // This call to Game.NameHelper forces a name conversion check every time.  This is necessary because it is possible for different profile slots to have different
                    // save revision numbers.  So you have to do the check every time in case that happens only in this scenario (since the check can be a little expensive).
                    playerName = Game.NameHelper(playerName, "", isFemale, true);

                    try
                    {
                        slotText.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(slotText));
                        if (isDead == false)
                            slotText.Text = string.Format(LocaleBuilder.getResourceString(!isFemale ? "LOC_ID_PROFILE_SEL_SCREEN_7_MALE_NEW" : "LOC_ID_PROFILE_SEL_SCREEN_7_FEMALE_NEW"), playerName, LocaleBuilder.getResourceString(ClassType.ToStringID(playerClass, isFemale))); // {0} the {1}
                        else
                            slotText.Text = string.Format(LocaleBuilder.getResourceString(!isFemale ? "LOC_ID_PROFILE_SEL_SCREEN_8_MALE_NEW" : "LOC_ID_PROFILE_SEL_SCREEN_8_FEMALE_NEW"), playerName); // {0} the deceased
                        if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(slotText.Text, @"\p{IsCyrillic}"))
                            slotText.ChangeFontNoDefault(Game.RobotoSlabFont);
                    }
                    catch
                    {
                        slotText.ChangeFontNoDefault(Game.NotoSansSCFont);
                        if (isDead == false)
                            slotText.Text = string.Format(LocaleBuilder.getResourceString(!isFemale ? "LOC_ID_PROFILE_SEL_SCREEN_7_MALE_NEW" : "LOC_ID_PROFILE_SEL_SCREEN_7_FEMALE_NEW"), playerName, LocaleBuilder.getResourceString(ClassType.ToStringID(playerClass, isFemale))); // {0} the {1}
                        else
                            slotText.Text = string.Format(LocaleBuilder.getResourceString(!isFemale ? "LOC_ID_PROFILE_SEL_SCREEN_8_MALE_NEW" : "LOC_ID_PROFILE_SEL_SCREEN_8_FEMALE_NEW"), playerName); // {0} the deceased
                    }

                    slotLvlText.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_9") + " " + playerLevel;
                    if (timesCastleBeaten > 0)
                        slotNGText.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_10") + " " + timesCastleBeaten;
                    container.ID = 1; // Container with ID == 1 means it has a save file.
                }
            }
            catch
            {
                slotText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_SEL_SCREEN_1", slotText);
                container.ID = 0; // Container with ID == 0 means it has no save file.
            }
        }

        public void UnlockControls()
        {
            m_lockControls = false;
        }

        private void TweenInText(GameObj obj, float delay)
        {
            obj.Opacity = 0;
            obj.Y -= 50;
            Tween.To(obj, 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
            Tween.By(obj, 0.5f, Quad.EaseOut, "delay", delay.ToString(), "Y", "50");
        }

        private void ExitTransition()
        {
            SoundManager.PlaySound("DialogMenuClose");

            Tween.To(m_confirmText, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_navigationText, 0.2f, Tween.EaseNone, "Opacity", "0");
            Tween.To(m_deleteProfileText, 0.2f, Tween.EaseNone, "Opacity", "0");

            m_lockControls = true;

            TweenOutText(m_title, 0);
            TweenOutText(m_slot1Container, 0.05f);
            TweenOutText(m_slot2Container, 0.1f);
            TweenOutText(m_slot3Container, 0.15f);

            Tween.To(this, 0.2f, Tween.EaseNone, "delay", "0.5", "BackBufferOpacity", "0");
            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        private void TweenOutText(GameObj obj, float delay)
        {
            Tween.To(obj, 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
            Tween.By(obj, 0.5f, Quad.EaseInOut, "delay", delay.ToString(), "Y", "-50");
        }

        public override void OnExit()
        {
            m_slot1Container.TextureColor = Color.White;
            m_slot2Container.TextureColor = Color.White;
            m_slot3Container.TextureColor = Color.White;
            m_lockControls = false;
            base.OnExit();
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                ObjContainer selectedSlot = m_selectedSlot;

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                {
                    m_selectedIndex++;
                    if (m_selectedIndex >= m_slotArray.Count)
                        m_selectedIndex = 0;
                    m_selectedSlot = m_slotArray[m_selectedIndex];
                    SoundManager.PlaySound("frame_swap");

                    m_deleteProfileText.Visible = true;
                    if (m_selectedSlot.ID == 0)
                        m_deleteProfileText.Visible = false;
                }

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    m_selectedIndex--;
                    if (m_selectedIndex < 0)
                        m_selectedIndex = m_slotArray.Count - 1;
                    m_selectedSlot = m_slotArray[m_selectedIndex];
                    SoundManager.PlaySound("frame_swap");

                    m_deleteProfileText.Visible = true;
                    if (m_selectedSlot.ID == 0)
                        m_deleteProfileText.Visible = false;
                }

                if (m_selectedSlot != selectedSlot)
                {
                    selectedSlot.TextureColor = Color.White;
                    m_selectedSlot.TextureColor = Color.Yellow;
                }

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    ExitTransition();

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                {
                    SoundManager.PlaySound("Map_On");

                    Game.GameConfig.ProfileSlot = (byte)(m_selectedIndex + 1);
                    Game game = (ScreenManager.Game as Game);
                    game.SaveConfig();

                    if (game.SaveManager.FileExists(SaveType.PlayerData))
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Title, true);
                    else
                    {
                        SkillSystem.ResetAllTraits();
                        Game.PlayerStats.Dispose();
                        Game.PlayerStats = new PlayerStats();
                        (ScreenManager as RCScreenManager).Player.Reset();
                        Game.ScreenManager.Player.CurrentHealth = Game.PlayerStats.CurrentHealth;
                        Game.ScreenManager.Player.CurrentMana = Game.PlayerStats.CurrentMana;
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.TutorialRoom, true, null);
                    }
                }

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_DELETEPROFILE) && m_deleteProfileText.Visible == true)
                {
                    SoundManager.PlaySound("Map_On");
                    DeleteSaveAsk();
                }
            }

            base.HandleInput();
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin();
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);

            m_title.Draw(Camera);
            m_slot1Container.Draw(Camera);
            m_slot2Container.Draw(Camera);
            m_slot3Container.Draw(Camera);

            m_confirmText.Draw(Camera);
            m_cancelText.Draw(Camera);
            m_navigationText.Draw(Camera);
            m_deleteProfileText.Draw(Camera);

            Camera.End();
            base.Draw(gametime);
        }

        public void DeleteSaveAsk()
        {
            RCScreenManager manager = ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("Delete Save");
            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
            manager.DialogueScreen.SetConfirmEndHandler(this, "DeleteSaveAskAgain");
            manager.DisplayScreen(ScreenType.Dialogue, false, null);
        }

        public void DeleteSaveAskAgain()
        {
            RCScreenManager manager = ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("Delete Save2");
            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
            manager.DialogueScreen.SetConfirmEndHandler(this, "DeleteSave");
            manager.DisplayScreen(ScreenType.Dialogue, false, null);
        }

        public void DeleteSave()
        {
            bool runTutorial = false;
            byte storedProfile = Game.GameConfig.ProfileSlot;

            if (Game.GameConfig.ProfileSlot == m_selectedIndex + 1)
                runTutorial = true;

            // Doing this to delete the correct profile slot.  Will be reverted once the file is deleted.
            Game.GameConfig.ProfileSlot = (byte)(m_selectedIndex + 1);
            //Game game = (ScreenManager.Game as Game);
            //game.SaveConfig();

            (ScreenManager.Game as Game).SaveManager.ClearAllFileTypes(false);
            (ScreenManager.Game as Game).SaveManager.ClearAllFileTypes(true);

            // Reverting profile slot back to stored slot.
            Game.GameConfig.ProfileSlot = storedProfile;

            if (runTutorial == true)
            {
                Game.PlayerStats.Dispose();
                SkillSystem.ResetAllTraits();
                Game.PlayerStats = new PlayerStats();
                (ScreenManager as RCScreenManager).Player.Reset();

                SoundManager.StopMusic(1);

                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.TutorialRoom, true);
            }
            else
            {
                m_deleteProfileText.Visible = false;
                CheckSaveHeaders(m_slotArray[m_selectedIndex], (byte)(m_selectedIndex + 1));
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                //Console.WriteLine("Disposing Profile Select Screen");
                m_title.Dispose();
                m_title = null;

                m_slot1Container.Dispose();
                m_slot1Container = null;
                m_slot2Container.Dispose();
                m_slot2Container = null;
                m_slot3Container.Dispose();
                m_slot3Container = null;

                m_slotArray.Clear();
                m_slotArray = null;

                m_selectedSlot = null;

                m_confirmText.Dispose();
                m_confirmText = null;
                m_cancelText.Dispose();
                m_cancelText = null;
                m_navigationText.Dispose();
                m_navigationText = null;
                m_deleteProfileText.Dispose();
                m_deleteProfileText = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            /*
            if (InputManager.GamePadIsConnected(PlayerIndex.One))
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_2");
            else
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_3");
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_4");
            m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_5");
            m_deleteProfileText.Text = "[Input:" + InputMapType.MENU_DELETEPROFILE + "] " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_SEL_SCREEN_6");
            */

            Game.ChangeBitmapLanguage(m_title, "ProfileSelectTitle_Sprite");
            Game.ChangeBitmapLanguage(m_slot1Container.GetChildAt(2) as SpriteObj, "ProfileSlot1Text_Sprite");
            Game.ChangeBitmapLanguage(m_slot2Container.GetChildAt(2) as SpriteObj, "ProfileSlot2Text_Sprite");
            Game.ChangeBitmapLanguage(m_slot3Container.GetChildAt(2) as SpriteObj, "ProfileSlot3Text_Sprite");

            // Update save slot text
            CheckSaveHeaders(m_slot1Container, 1);
            CheckSaveHeaders(m_slot2Container, 2);
            CheckSaveHeaders(m_slot3Container, 3);

            base.RefreshTextObjs();
        }
    }
}