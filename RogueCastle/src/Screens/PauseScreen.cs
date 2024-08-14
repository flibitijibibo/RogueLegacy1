using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Graphics;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class PauseScreen : Screen
    {
        private SpriteObj m_titleText;
        private List<PauseInfoObj> m_infoObjList;

        private SpriteObj m_profileCard;
        private SpriteObj m_optionsIcon;

        private KeyIconTextObj m_profileCardKey;
        private KeyIconTextObj m_optionsKey;

        private float m_inputDelay = 0;

        public PauseScreen()
        {
            this.DrawIfCovered = true;
        }

        public override void LoadContent()
        {
            m_titleText = new SpriteObj("GamePausedTitleText_Sprite");
            m_titleText.X = GlobalEV.ScreenWidth / 2;
            m_titleText.Y = GlobalEV.ScreenHeight * 0.1f;
            m_titleText.ForceDraw = true;

            m_infoObjList = new List<PauseInfoObj>();
            m_infoObjList.Add(new PauseInfoObj()); // Adding an info obj for the player.

            m_profileCard = new SpriteObj("TitleProfileCard_Sprite");
            m_profileCard.OutlineWidth = 2;
            m_profileCard.Scale = new Vector2(2, 2);
            m_profileCard.Position = new Vector2(m_profileCard.Width, 720 - m_profileCard.Height);
            m_profileCard.ForceDraw = true;

            m_optionsIcon = new SpriteObj("TitleOptionsIcon_Sprite");
            m_optionsIcon.Scale = new Vector2(2, 2);
            m_optionsIcon.OutlineWidth = m_profileCard.OutlineWidth;
            m_optionsIcon.Position = new Vector2(1320 - m_optionsIcon.Width * 2 + 120, m_profileCard.Y);
            m_optionsIcon.ForceDraw = true;

            m_profileCardKey = new KeyIconTextObj(Game.JunicodeFont);
            m_profileCardKey.Align = Types.TextAlign.Centre;
            m_profileCardKey.FontSize = 12;
            m_profileCardKey.Text = "[Input:" + InputMapType.MENU_PROFILECARD + "]";
            m_profileCardKey.Position = new Vector2(m_profileCard.X, m_profileCard.Bounds.Top - m_profileCardKey.Height - 10);
            m_profileCardKey.ForceDraw = true;

            m_optionsKey = new KeyIconTextObj(Game.JunicodeFont);
            m_optionsKey.Align = Types.TextAlign.Centre;
            m_optionsKey.FontSize = 12;
            m_optionsKey.Text = "[Input:" + InputMapType.MENU_OPTIONS + "]";
            m_optionsKey.Position = new Vector2(m_optionsIcon.X, m_optionsIcon.Bounds.Top - m_optionsKey.Height - 10);
            m_optionsKey.ForceDraw = true;

            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_classDebugCounter = 0;
            UpdatePauseScreenIcons();

            m_inputDelay = 0.5f;

            if (SoundManager.IsMusicPlaying)
                SoundManager.PauseMusic();

            SoundManager.PlaySound("Pause_Toggle");

            ProceduralLevelScreen level = (ScreenManager as RCScreenManager).GetLevelScreen();

            foreach (PauseInfoObj infoObj in m_infoObjList)
            {
                infoObj.Reset();
                infoObj.Visible = false;
            }

            PlayerObj player = (ScreenManager as RCScreenManager).Player;
            PauseInfoObj playerInfo = m_infoObjList[0];
            playerInfo.Visible = true;

            playerInfo.AddItem("LOC_ID_PAUSE_SCREEN_1", ClassType.ToStringID(Game.PlayerStats.Class, Game.PlayerStats.IsFemale), true);
            playerInfo.AddItem("LOC_ID_PAUSE_SCREEN_2", player.Damage.ToString());
            playerInfo.AddItem("LOC_ID_PAUSE_SCREEN_3", player.TotalMagicDamage.ToString());
            playerInfo.AddItem("LOC_ID_PAUSE_SCREEN_4", player.TotalArmor.ToString());
            playerInfo.ResizePlate();

            playerInfo.X = player.X - Camera.TopLeftCorner.X;
            playerInfo.Y = player.Bounds.Bottom - Camera.TopLeftCorner.Y + playerInfo.Height / 2f - 20;

            if (Game.PlayerStats.TutorialComplete == false)
                playerInfo.SetName("LOC_ID_PAUSE_SCREEN_8", true); // ????? (no name yet)
            else
                playerInfo.SetName(Game.NameHelper());
            playerInfo.SetNamePosition(new Vector2(playerInfo.X, player.Bounds.Top - Camera.TopLeftCorner.Y - 40));

            playerInfo.Visible = player.Visible;

            // Adding more pause info objs to the screen if the current room has more enemies than the previous one.
            int infoObjListCount = m_infoObjList.Count - 1;
            //for (int i = infoObjListCount; i < level.CurrentRoom.EnemyList.Count; i++)
            for (int i = infoObjListCount; i < level.CurrentRoom.EnemyList.Count + level.CurrentRoom.TempEnemyList.Count; i++)
                m_infoObjList.Add(new PauseInfoObj() { Visible = false });

            for (int i = 1; i < level.CurrentRoom.EnemyList.Count + 1; i++) // +1 because the first infoObjList object is the player's data.
            {
                EnemyObj enemy = level.CurrentRoom.EnemyList[i - 1];

                if (enemy.NonKillable == false && enemy.IsKilled == false && enemy.Visible == true)
                {
                    PauseInfoObj enemyInfo = m_infoObjList[i];
                    enemyInfo.Visible = true;
                    //enemyInfo.AddItem("Name: ", enemy.Name);
                    if (LevelEV.CREATE_RETAIL_VERSION == false)
                        enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_5", enemy.Level.ToString());
                    else
                        enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_5", ((int)(enemy.Level * LevelEV.ENEMY_LEVEL_FAKE_MULTIPLIER)).ToString());
                    enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_2", enemy.Damage.ToString());
                    enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_6", enemy.CurrentHealth + "/" + enemy.MaxHealth);
                    enemyInfo.ResizePlate();

                    enemyInfo.X = enemy.X - Camera.TopLeftCorner.X;
                    enemyInfo.Y = enemy.Bounds.Bottom - Camera.TopLeftCorner.Y + enemyInfo.Height / 2f - 20;

                    enemyInfo.SetName(enemy.LocStringID, true);
                    enemyInfo.SetNamePosition(new Vector2(enemyInfo.X, enemy.Bounds.Top - Camera.TopLeftCorner.Y - 40));
                }
            }

            int tempEnemyIndex = level.CurrentRoom.EnemyList.Count;
            for (int i = 0; i < level.CurrentRoom.TempEnemyList.Count; i++)
            {
                EnemyObj enemy = level.CurrentRoom.TempEnemyList[i];

                if (enemy.NonKillable == false && enemy.IsKilled == false)
                {
                    PauseInfoObj enemyInfo = m_infoObjList[i + 1 + tempEnemyIndex];
                    enemyInfo.Visible = true;
                    //enemyInfo.AddItem("Name: ", enemy.Name);
                    if (LevelEV.CREATE_RETAIL_VERSION == false)
                        enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_5", enemy.Level.ToString());
                    else
                        enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_5", ((int)(enemy.Level * LevelEV.ENEMY_LEVEL_FAKE_MULTIPLIER)).ToString());
                    enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_2", enemy.Damage.ToString());
                    enemyInfo.AddItem("LOC_ID_PAUSE_SCREEN_6", enemy.CurrentHealth + "/" + enemy.MaxHealth);
                    enemyInfo.ResizePlate();

                    enemyInfo.X = enemy.X - Camera.TopLeftCorner.X;
                    enemyInfo.Y = enemy.Bounds.Bottom - Camera.TopLeftCorner.Y + enemyInfo.Height / 2f - 20;

                    enemyInfo.SetName(enemy.LocStringID, true);
                    enemyInfo.SetNamePosition(new Vector2(enemyInfo.X, enemy.Bounds.Top - Camera.TopLeftCorner.Y - 40));
                }
            }

            Game.ChangeBitmapLanguage(m_titleText, "GamePausedTitleText_Sprite");
            base.OnEnter();
        }

        public void UpdatePauseScreenIcons()
        {
            m_profileCardKey.Text = "[Input:" + InputMapType.MENU_PROFILECARD + "]";
            m_optionsKey.Text = "[Input:" + InputMapType.MENU_OPTIONS + "]";
        }

        public override void OnExit()
        {
            if (SoundManager.IsMusicPaused)
                SoundManager.ResumeMusic();

            SoundManager.PlaySound("Resume_Toggle");

            foreach (PauseInfoObj obj in m_infoObjList)
                obj.Visible = false;
            base.OnExit();
        }

        public override void HandleInput()
        {
            if (m_inputDelay <= 0)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_PROFILECARD) && Game.PlayerStats.TutorialComplete == true) // this needs to be unified.
                    (this.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.ProfileCard, true, null);

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_OPTIONS))
                {
                    List<object> optionsData = new List<object>();
                    optionsData.Add(false);
                    (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Options, false, optionsData);
                }

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_PAUSE))
                {
                    (ScreenManager as RCScreenManager).GetLevelScreen().UnpauseScreen();
                    (ScreenManager as RCScreenManager).HideCurrentScreen();
                }

                if (LevelEV.ENABLE_DEBUG_INPUT == true)
                    HandleDebugInput();

                base.HandleInput();
            }
        }

        private sbyte m_classDebugCounter = 0;
        private void HandleDebugInput()
        {
            sbyte currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            sbyte previousDebugClass = currentDebugClass;

            if (InputManager.JustPressed(Keys.OemOpenBrackets, PlayerIndex.One))
            {
                if (currentDebugClass == ClassType.Knight)
                    m_classDebugCounter = (sbyte)(ClassType.Traitor - Game.PlayerStats.Class);
                else
                    m_classDebugCounter--;

                currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            }
            else if (InputManager.JustPressed(Keys.OemCloseBrackets, PlayerIndex.One))
            {
                if (currentDebugClass == ClassType.Traitor)
                    m_classDebugCounter = (sbyte)(-Game.PlayerStats.Class);
                else
                    m_classDebugCounter++;
                currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            }

            if (currentDebugClass != previousDebugClass)
            {
                PlayerObj player = (ScreenManager as RCScreenManager).Player;
                PauseInfoObj playerInfo = m_infoObjList[0];
                playerInfo.Visible = true;
                (playerInfo.GetChildAt(2) as TextObj).Text = LocaleBuilder.getString(ClassType.ToStringID((byte)currentDebugClass, Game.PlayerStats.IsFemale), (playerInfo.GetChildAt(2) as TextObj));
                playerInfo.ResizePlate();

                playerInfo.X = player.X - Camera.TopLeftCorner.X;
                playerInfo.Y = player.Bounds.Bottom - Camera.TopLeftCorner.Y + playerInfo.Height / 2f - 20;
                //playerInfo.SetNamePosition(new Vector2(playerInfo.X, player.Bounds.Top - Camera.TopLeftCorner.Y - 40));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_inputDelay > 0)
                m_inputDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null); // Anything that is affected by the godray should be drawn here.
            m_titleText.Draw(Camera);
            foreach (PauseInfoObj infoObj in m_infoObjList)
                infoObj.Draw(Camera);

            if (Game.PlayerStats.TutorialComplete == true)
                m_profileCardKey.Draw(Camera);

            m_optionsKey.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_optionsIcon.Draw(Camera);

            if (Game.PlayerStats.TutorialComplete == true)
                m_profileCard.Draw(Camera);

            Camera.End();
            base.Draw(gameTime);
        }

        public override void RefreshTextObjs()
        {
            Game.ChangeBitmapLanguage(m_titleText, "GamePausedTitleText_Sprite");
            
            PauseInfoObj playerInfo = m_infoObjList[0];
            if (Game.PlayerStats.TutorialComplete == false)
                playerInfo.SetName("LOC_ID_PAUSE_SCREEN_8", true); // ????? (no name yet)
            else
                playerInfo.SetName(Game.NameHelper());

            foreach (PauseInfoObj infoObj in m_infoObjList)
            {
                if (infoObj.Visible == true)
                    infoObj.ResizePlate();
            }

            base.RefreshTextObjs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Pause Screen");

                foreach (PauseInfoObj obj in m_infoObjList)
                    obj.Dispose();
                m_infoObjList.Clear();
                m_infoObjList = null;

                m_titleText.Dispose();
                m_titleText = null;

                m_profileCard.Dispose();
                m_profileCard = null;
                m_optionsIcon.Dispose();
                m_optionsIcon = null;

                m_profileCardKey.Dispose();
                m_profileCardKey = null;
                m_optionsKey.Dispose();
                m_optionsKey = null;
                base.Dispose();
            }
        }

        private class PauseInfoObj : ObjContainer
        {
            private List<TextObj> m_textList; // The title for the text
            private List<TextObj> m_textDataList;  // The data for the text

            private int m_arrayIndex = 0;

            private ObjContainer m_namePlate;
            private TextObj m_name;

            public PauseInfoObj()
                : base("GameOverStatPlate_Character")
            {
                this.ForceDraw = true;
                m_textList = new List<TextObj>();
                m_textDataList = new List<TextObj>();

                m_namePlate = new ObjContainer("DialogBox_Character");
                m_namePlate.ForceDraw = true;

                m_name = new TextObj(Game.JunicodeFont);
                m_name.Align = Types.TextAlign.Centre;
                m_name.Text = "<noname>";
                m_name.FontSize = 8;
                m_name.Y -= 45;
                m_name.OverrideParentScale = true;
                m_name.DropShadow = new Vector2(2, 2);
                m_namePlate.AddChild(m_name);
            }

            public void SetName(string name, bool isLocalized = false)
            {
                if (isLocalized)
                {
                    try
                    {
                        m_name.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_name));
                        m_name.Text = LocaleBuilder.getString(name, m_name);
                        if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(m_name.Text, @"\p{IsCyrillic}"))
                            m_name.ChangeFontNoDefault(Game.RobotoSlabFont);
                    }
                    catch
                    {
                        m_name.ChangeFontNoDefault(Game.NotoSansSCFont);
                        m_name.Text = LocaleBuilder.getString(name, m_name);
                    }
                }
                else
                {
                    try
                    {
                        m_name.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_name));
                        m_name.Text = name;
                        if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(m_name.Text, @"\p{IsCyrillic}"))
                            m_name.ChangeFontNoDefault(Game.RobotoSlabFont);
                    }
                    catch
                    {
                        m_name.ChangeFontNoDefault(Game.NotoSansSCFont);
                        m_name.Text = name;
                    }
                }
                m_namePlate.Scale = Vector2.One;
                m_namePlate.Scale = new Vector2((m_name.Width + 70f) / m_namePlate.Width, (m_name.Height + 20f) / m_namePlate.Height);
            }

            public void SetNamePosition(Vector2 pos)
            {
                m_namePlate.Position = pos;
            }

            public void AddItem(string title, string data, bool localizedData = false)
            {
                TextObj titleText;
                if (m_textList.Count <= m_arrayIndex)
                    titleText = new TextObj(Game.JunicodeFont);
                else
                    titleText = m_textList[m_arrayIndex];

                titleText.FontSize = 8;
                titleText.Text = LocaleBuilder.getString(title, titleText);
                titleText.Align = Types.TextAlign.Right;
                titleText.Y = _objectList[0].Bounds.Top + titleText.Height + (m_arrayIndex * 20);
                titleText.DropShadow = new Vector2(2,2);
                if (m_textList.Count <= m_arrayIndex)
                {
                    this.AddChild(titleText);
                    m_textList.Add(titleText);
                }

                TextObj dataText;
                if (m_textDataList.Count <= m_arrayIndex)
                    dataText = new TextObj(Game.JunicodeFont);
                else
                    dataText = m_textDataList[m_arrayIndex];
                dataText.FontSize = 8;
                if (localizedData)
                    dataText.Text = LocaleBuilder.getString(data, dataText);
                else
                    dataText.Text = data;
                dataText.Y = titleText.Y;
                dataText.DropShadow = new Vector2(2, 2);
                if (m_textDataList.Count <= m_arrayIndex)
                {
                    this.AddChild(dataText);
                    m_textDataList.Add(dataText);
                }

                m_arrayIndex++;
            }

            // Should be called once all items have been added.
            public void ResizePlate()
            {
                _objectList[0].ScaleY = 1;
                _objectList[0].ScaleY = (float)(_objectList[1].Height * (_objectList.Count + 1)/2) / (float)_objectList[0].Height;

                int longestTitle = 0;
                foreach (TextObj obj in m_textList)
                {
                    if (obj.Width > longestTitle)
                        longestTitle = obj.Width;
                }

                int longestData = 0;
                foreach (TextObj obj in m_textDataList)
                {
                    if (obj.Width > longestData)
                        longestData = obj.Width;
                }

                _objectList[0].ScaleX = 1;
                _objectList[0].ScaleX = (float)(longestTitle + longestData + 50) / (float)_objectList[0].Width;

                int newTitleXPos = (int)(-(_objectList[0].Width / 2f) + longestTitle) + 25;
                int newTitleYPos = (int)(_objectList[0].Height / (m_textList.Count + 2));

                for (int i = 0; i < m_textList.Count; i++)
                {
                    m_textList[i].X = newTitleXPos;
                    m_textList[i].Y = _objectList[0].Bounds.Top + newTitleYPos + (newTitleYPos * i);

                    m_textDataList[i].X = newTitleXPos;
                    m_textDataList[i].Y = m_textList[i].Y;
                }
            }

            public override void Draw(Camera2D camera)
            {
                if (this.Visible == true)
                {
                    m_namePlate.Draw(camera);
                    m_name.Draw(camera);
                }
                base.Draw(camera);
            }

            public void Reset()
            {
                foreach (TextObj obj in m_textList)
                    obj.Text = "";

                foreach (TextObj obj in m_textDataList)
                    obj.Text = "";

                m_arrayIndex = 0;
            }

            public override void Dispose()
            {
                if (IsDisposed == false)
                {
                    m_textList.Clear();
                    m_textList = null;

                    m_textDataList.Clear();
                    m_textDataList = null;

                    m_namePlate.Dispose();
                    m_namePlate = null;
                    m_name = null;
                    base.Dispose();
                }
            }
        }
    }
}
