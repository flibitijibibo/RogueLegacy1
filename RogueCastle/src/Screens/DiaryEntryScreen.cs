using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class DiaryEntryScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private SpriteObj m_titleText;
        private List<ObjContainer> m_diaryList;
        private int m_entryIndex = 0;
        private int m_entryRow = 0;
        private ObjContainer m_selectedEntry;
        private int m_unlockedEntries = 0;
        private float m_inputDelay = 0;

        public DiaryEntryScreen()
        {
            m_diaryList = new List<ObjContainer>();
            this.DrawIfCovered = true;
        }

        public override void LoadContent()
        {
            // Loading Teleporter objects.
            m_titleText = new SpriteObj("DiaryEntryTitleText_Sprite");
            m_titleText.ForceDraw = true;
            m_titleText.X = GlobalEV.ScreenWidth / 2;
            m_titleText.Y = GlobalEV.ScreenHeight * 0.1f;

            int startingX = 260;//250;//350;
            int startingY = 150;
            int xPosition = startingX;
            int yPosition = startingY;
            int yOffset = 100;
            int xOffset = 200;
            int xChange = 5;//4;
            int xCounter = 0;
            for (int i = 0; i < LevelEV.TOTAL_JOURNAL_ENTRIES; i++) //TEDDY CHANGING i< 20 to the LEVELEV.Journals THINGAMABOB
            {
                ObjContainer entry = new ObjContainer("DialogBox_Character");
                entry.ForceDraw = true;
                entry.Scale = new Vector2(180f / entry.Width, 50f / entry.Height);
                entry.Position = new Vector2(xPosition, yPosition);

                TextObj entryTitle = new TextObj(Game.JunicodeFont);
                switch (LocaleBuilder.languageType)
                {
                    case(LanguageType.Russian):
                    case(LanguageType.Polish):
                        entryTitle.Text = LocaleBuilder.getString("LOC_ID_DIARY_ENTRY_SCREEN_1", entryTitle) /*"Entry #"*/ + " " + (i + 1);
                        break;
                    default:
                        entryTitle.Text = LocaleBuilder.getString("LOC_ID_DIARY_ENTRY_SCREEN_1", entryTitle) /*"Entry #"*/ + (i + 1);
                        break;
                }
                entryTitle.OverrideParentScale = true;
                entryTitle.OutlineWidth = 2;
                entryTitle.FontSize = 12;
                entryTitle.Y -= 64;
                entryTitle.Align = Types.TextAlign.Centre;

                entry.AddChild(entryTitle);
                m_diaryList.Add(entry);

                xCounter++;
                xPosition += xOffset;
                if (xCounter >= xChange)
                {
                    xCounter = 0;
                    xPosition = startingX;
                    yPosition += yOffset;
                }

                if (i > 13) entry.Visible = false;
            }

            base.LoadContent();
        }

        public override void OnEnter()
        {
            // First refresh text
            RefreshTextObjs();

            SoundManager.PlaySound("DialogOpen");

            m_inputDelay = 0.5f;
            m_entryRow = 1;
            m_entryIndex = 0;
            UpdateSelection();
            m_unlockedEntries = Game.PlayerStats.DiaryEntry;
            if (LevelEV.UNLOCK_ALL_DIARY_ENTRIES == true)
                m_unlockedEntries = LevelEV.TOTAL_JOURNAL_ENTRIES - 1;

            if (m_unlockedEntries >= LevelEV.TOTAL_JOURNAL_ENTRIES - 1)
                GameUtil.UnlockAchievement("LOVE_OF_BOOKS");

            for (int i = 0; i < m_diaryList.Count; i++)
            {
                if (i < m_unlockedEntries)
                    m_diaryList[i].Visible = true;
                else
                    m_diaryList[i].Visible = false;
            }

            BackBufferOpacity = 0;
            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0.7");

            m_titleText.Opacity = 0;
            Tween.To(m_titleText, 0.25f, Tween.EaseNone, "Opacity", "1");

            // Fade in effect.
            int delayCounter = 0;
            float delay = 0;
            foreach (ObjContainer diary in m_diaryList)
            {
                if (diary.Visible == true)
                {
                    diary.Opacity = 0;

                    if (delayCounter >= 5)
                    {
                        delayCounter = 0;
                        delay += 0.05f;
                    }
                    delayCounter++;
                    Tween.To(diary, 0.25f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                    Tween.By(diary, 0.25f, Quad.EaseOut, "delay", delay.ToString(), "Y", "50");
                }
            }

            base.OnEnter();
        }

        private void ExitTransition()
        {
            SoundManager.PlaySound("DialogMenuClose");

            int delayCounter = 0;
            float delay = 0;
            foreach (ObjContainer diary in m_diaryList)
            {
                if (diary.Visible == true)
                {
                    diary.Opacity = 1;

                    if (delayCounter >= 5)
                    {
                        delayCounter = 0;
                        delay += 0.05f;
                    }
                    delayCounter++;
                    Tween.To(diary, 0.25f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
                    Tween.By(diary, 0.25f, Quad.EaseOut, "delay", delay.ToString(), "Y", "-50");
                }
            }

            m_titleText.Opacity = 1;
            Tween.To(m_titleText, 0.25f, Tween.EaseNone, "Opacity", "0");

            m_inputDelay = 1f;
            Tween.To(this, 0.5f, Tween.EaseNone, "BackBufferOpacity", "0");
            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        public override void HandleInput()
        {
            if (m_inputDelay <= 0)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    DisplayEntry();
                else if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    ExitTransition();

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
                {
                    if (m_entryIndex > 0 && m_diaryList[m_entryIndex - 1].Visible == true)
                    {
                        SoundManager.PlaySound("frame_swap");                     
                        m_entryIndex--;
                    }
                }
                else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
                {
                    if (m_entryIndex < m_diaryList.Count - 1 && m_diaryList[m_entryIndex + 1].Visible == true)
                    {
                        m_entryIndex++;
                        SoundManager.PlaySound("frame_swap");
                    }
                }
                else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    if (m_entryRow > 1 && m_diaryList[m_entryIndex - 5].Visible == true)
                    {
                        m_entryRow--;
                        m_entryIndex -= 5;
                        SoundManager.PlaySound("frame_swap");
                    }
                }
                else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                {
                    if (m_entryRow < 5 && m_diaryList[m_entryIndex + 5].Visible == true)
                    {
                        m_entryRow++;
                        m_entryIndex += 5;
                        SoundManager.PlaySound("frame_swap");
                    }
                }

                if (m_entryRow > 5)
                    m_entryRow = 5;
                if (m_entryRow < 1)
                    m_entryRow = 1;

                if (m_entryIndex >= m_entryRow * 5)
                    m_entryIndex = m_entryRow * 5 - 1;
                if (m_entryIndex < m_entryRow * 5 - 5)
                    m_entryIndex = m_entryRow * 5 - 5;

                UpdateSelection();

                base.HandleInput();
            }
        }

        private void DisplayEntry()
        {
            RCScreenManager manager = ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("DiaryEntry" + m_entryIndex);
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        private void UpdateSelection()
        {
            if (m_selectedEntry != null)
                m_selectedEntry.TextureColor = Color.White;

            m_selectedEntry = m_diaryList[m_entryIndex];
            m_selectedEntry.TextureColor = Color.Yellow;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_inputDelay > 0)
                m_inputDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            m_titleText.Draw(Camera);
            foreach (ObjContainer obj in m_diaryList)
                obj.Draw(Camera);

            Camera.End();
            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Diary Entry Screen");

                foreach (ObjContainer obj in m_diaryList)
                    obj.Dispose();
                m_diaryList.Clear();
                m_diaryList = null;
                m_selectedEntry = null;

                m_titleText.Dispose();
                m_titleText = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            int i = 0;
            foreach (ObjContainer obj in m_diaryList)
            {
                if (obj.GetChildAt(obj.NumChildren - 1) is TextObj)
                {
                    switch (LocaleBuilder.languageType)
                    {
                        case (LanguageType.Russian):
                        case (LanguageType.Polish):
                            (obj.GetChildAt(obj.NumChildren - 1) as TextObj).Text = LocaleBuilder.getResourceString("LOC_ID_DIARY_ENTRY_SCREEN_1") /*"Entry #"*/ + " " + (i + 1);
                            break;
                        default:
                            (obj.GetChildAt(obj.NumChildren - 1) as TextObj).Text = LocaleBuilder.getResourceString("LOC_ID_DIARY_ENTRY_SCREEN_1") /*"Entry #"*/ + (i + 1);
                            break;
                    }
                }
                i++;
            }

            Game.ChangeBitmapLanguage(m_titleText, "DiaryEntryTitleText_Sprite");

            base.RefreshTextObjs();
        }
    }
}
