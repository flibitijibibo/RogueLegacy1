using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Randomchaos2DGodRays;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class TitleScreen : Screen
    {
        private TextObj m_titleText;
        private SpriteObj m_bg;
        private SpriteObj m_logo;
        private SpriteObj m_castle;
        private SpriteObj m_smallCloud1, m_smallCloud2, m_smallCloud3, m_smallCloud4, m_smallCloud5;
        private SpriteObj m_largeCloud1, m_largeCloud2, m_largeCloud3, m_largeCloud4;
        private KeyIconTextObj m_pressStartText;
        private TextObj m_pressStartText2;
        private TextObj m_copyrightText;

        private bool m_startPressed = false;

        private RenderTarget2D m_godRayTexture;
        private CrepuscularRays m_godRay;
        private PostProcessingManager m_ppm;

        private float m_randomSeagullSFX = 0;
        private Cue m_seagullCue;

        private SpriteObj m_profileCard;
        private SpriteObj m_optionsIcon;
        private SpriteObj m_creditsIcon;

        private KeyIconTextObj m_profileCardKey;
        private KeyIconTextObj m_optionsKey;
        private KeyIconTextObj m_creditsKey;

        private KeyIconTextObj m_profileSelectKey;

        private SpriteObj m_crown;

        private TextObj m_versionNumber;

        private float m_hardCoreModeOpacity = 0;
        private bool m_optionsEntered = false;
        private bool m_startNewLegacy = false;
        private bool m_heroIsDead = false;
        private bool m_startNewGamePlus = false;
        private bool m_loadStartingRoom = false;

        private SpriteObj m_dlcIcon;

        public override void LoadContent()
        {
            m_ppm = new PostProcessingManager(ScreenManager.Game, ScreenManager.Camera);
            m_godRay = new CrepuscularRays(ScreenManager.Game, Vector2.One * .5f, "GameSpritesheets/flare3", 2f, .97f, .97f, .5f, 1.25f);
            //m_godRay = new CrepuscularRays(ScreenManager.Game, Vector2.One * .5f, "GameSpritesheets/flare3", 2f, .97f, .97f, .5f, .75f);
            m_ppm.AddEffect(m_godRay);

            m_godRayTexture = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            m_godRay.lightSource = new Vector2(0.495f, 0.3f);

            m_bg = new SpriteObj("TitleBG_Sprite");
            m_bg.Scale = new Vector2(1320f / m_bg.Width, 720f / m_bg.Height);
            m_bg.TextureColor = Color.Red;
            m_hardCoreModeOpacity = 0f;

            // Code for hardcore mode.
            //m_bg.TextureColor = Color.Black;
            //m_hardCoreModeOpacity = 0.5f;

            m_logo = new SpriteObj("TitleLogo_Sprite");
            m_logo.Position = new Vector2(1320 / 2, 720 / 2);
            m_logo.DropShadow = new Vector2(0, 5);

            m_castle = new SpriteObj("TitleCastle_Sprite");
            m_castle.Scale = new Vector2(2, 2);
            m_castle.Position = new Vector2(1320 / 2 - 30, 720 - m_castle.Height/2);

            m_smallCloud1 = new SpriteObj("TitleSmallCloud1_Sprite");
            m_smallCloud1.Position = new Vector2(1320 / 2, 0);
            m_smallCloud2 = new SpriteObj("TitleSmallCloud2_Sprite");
            m_smallCloud2.Position = m_smallCloud1.Position;
            m_smallCloud3 = new SpriteObj("TitleSmallCloud3_Sprite");
            m_smallCloud3.Position = m_smallCloud1.Position;
            m_smallCloud4 = new SpriteObj("TitleSmallCloud4_Sprite");
            m_smallCloud4.Position = m_smallCloud1.Position;
            m_smallCloud5 = new SpriteObj("TitleSmallCloud5_Sprite");
            m_smallCloud5.Position = m_smallCloud1.Position;

            m_largeCloud1 = new SpriteObj("TitleLargeCloud1_Sprite");
            m_largeCloud1.Position = new Vector2(0, 720 - m_largeCloud1.Height);
            m_largeCloud2 = new SpriteObj("TitleLargeCloud2_Sprite");
            m_largeCloud2.Position = new Vector2(1320 / 3, 720 - m_largeCloud2.Height + 130);
            m_largeCloud3 = new SpriteObj("TitleLargeCloud1_Sprite");
            m_largeCloud3.Position = new Vector2(1320/3 * 2, 720 - m_largeCloud3.Height + 50);
            m_largeCloud3.Flip = SpriteEffects.FlipHorizontally;
            m_largeCloud4 = new SpriteObj("TitleLargeCloud2_Sprite");
            m_largeCloud4.Position = new Vector2(1320, 720 - m_largeCloud4.Height);
            m_largeCloud4.Flip = SpriteEffects.FlipHorizontally;

            m_titleText = new TextObj();
            m_titleText.Font = Game.JunicodeFont;
            m_titleText.FontSize = 45;
            m_titleText.Text = "ROGUE CASTLE";
            m_titleText.Position = new Vector2(1320 / 2, 720 / 2 - 300);
            m_titleText.Align = Types.TextAlign.Centre;

            m_copyrightText = new TextObj(Game.JunicodeFont);
            m_copyrightText.FontSize = 8;
            m_copyrightText.Text = " Copyright(C) 2013-2018, Cellar Door Games Inc. Rogue Legacy(TM) is a trademark or registered trademark of Cellar Door Games Inc. All Rights Reserved.";
            m_copyrightText.Align = Types.TextAlign.Centre;
            m_copyrightText.Position = new Vector2(1320 / 2, 720 - m_copyrightText.Height - 10);
            m_copyrightText.DropShadow = new Vector2(1, 2);

            m_versionNumber = m_copyrightText.Clone() as TextObj;
            m_versionNumber.Align = Types.TextAlign.Right;
            m_versionNumber.FontSize = 8;
            m_versionNumber.Position = new Vector2(1320 - 15, 5);
            m_versionNumber.Text = LevelEV.GAME_VERSION;

            m_pressStartText = new KeyIconTextObj(Game.JunicodeFont);
            m_pressStartText.FontSize = 20;
            m_pressStartText.Text = "Press Enter to begin";
            m_pressStartText.Align = Types.TextAlign.Centre;
            m_pressStartText.Position = new Vector2(1320 / 2, 720 / 2 + 200);
            m_pressStartText.DropShadow = new Vector2(2, 2);

            m_pressStartText2 = new TextObj(Game.JunicodeFont);
            m_pressStartText2.FontSize = 20;
            m_pressStartText2.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_pressStartText2); // dummy locID to add TextObj to language refresh list
            m_pressStartText2.Align = Types.TextAlign.Centre;
            m_pressStartText2.Position = m_pressStartText.Position;
            m_pressStartText2.Y -= m_pressStartText.Height - 5;
            m_pressStartText2.DropShadow = new Vector2(2, 2);

            m_profileCard = new SpriteObj("TitleProfileCard_Sprite");
            m_profileCard.OutlineWidth = 2;
            m_profileCard.Scale = new Vector2(2, 2);
            m_profileCard.Position = new Vector2(m_profileCard.Width, 720 - m_profileCard.Height);
            m_profileCard.ForceDraw = true;

            m_optionsIcon = new SpriteObj("TitleOptionsIcon_Sprite");
            m_optionsIcon.Scale = new Vector2(2, 2);
            m_optionsIcon.OutlineWidth = m_profileCard.OutlineWidth;
            m_optionsIcon.Position = new Vector2(1320 - m_optionsIcon.Width * 2, m_profileCard.Y);
            m_optionsIcon.ForceDraw = true;

            m_creditsIcon = new SpriteObj("TitleCreditsIcon_Sprite");
            m_creditsIcon.Scale = new Vector2(2, 2);
            m_creditsIcon.OutlineWidth = m_profileCard.OutlineWidth;
            m_creditsIcon.Position = new Vector2(m_optionsIcon.X + 120, m_profileCard.Y);
            m_creditsIcon.ForceDraw = true;

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

            m_creditsKey = new KeyIconTextObj(Game.JunicodeFont);
            m_creditsKey.Align = Types.TextAlign.Centre;
            m_creditsKey.FontSize = 12;
            m_creditsKey.Text = "[Input:" + InputMapType.MENU_CREDITS + "]";
            m_creditsKey.Position = new Vector2(m_creditsIcon.X, m_creditsIcon.Bounds.Top - m_creditsKey.Height - 10);
            m_creditsKey.ForceDraw = true;

            m_profileSelectKey = new KeyIconTextObj(Game.JunicodeFont);
            m_profileSelectKey.Align = Types.TextAlign.Left;
            m_profileSelectKey.FontSize = 10;
            m_profileSelectKey.Text = string.Format(LocaleBuilder.getString("LOC_ID_BACK_TO_MENU_OPTIONS_3_NEW", m_profileSelectKey), Game.GameConfig.ProfileSlot);
            //m_profileSelectKey.Text = "[Input:" + InputMapType.MENU_PROFILESELECT + "] " + LocaleBuilder.getString("LOC_ID_BACK_TO_MENU_OPTIONS_3", m_profileSelectKey) + " (" + Game.GameConfig.ProfileSlot + ")";
            m_profileSelectKey.Position = new Vector2(30, 15);
            m_profileSelectKey.ForceDraw = true;
            m_profileSelectKey.DropShadow = new Vector2(2, 2);

            m_crown = new SpriteObj("Crown_Sprite");
            m_crown.ForceDraw = true;
            m_crown.Scale = new Vector2(0.7f, 0.7f);
            m_crown.Rotation = -30;
            m_crown.OutlineWidth = 2;

            m_dlcIcon = new SpriteObj("MedallionPiece5_Sprite");
            m_dlcIcon.Position = new Vector2(950, 310);
            //m_dlcIcon.OutlineWidth = 2;
            //m_dlcIcon.Scale = new Vector2(2, 2);
            m_dlcIcon.ForceDraw = true;
            m_dlcIcon.TextureColor = Color.Yellow;
            base.LoadContent();
        }

        public override void OnEnter()
        {
            Game.HoursPlayedSinceLastSave = 0;

            Camera.Zoom = 1;
            m_profileSelectKey.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_BACK_TO_MENU_OPTIONS_3_NEW"), Game.GameConfig.ProfileSlot);
            //m_profileSelectKey.Text = "[Input:" + InputMapType.MENU_PROFILESELECT + "] " + LocaleBuilder.getResourceString("LOC_ID_BACK_TO_MENU_OPTIONS_3") + " (" + Game.GameConfig.ProfileSlot + ")";

            // Setting initial data.
            SoundManager.PlayMusic("TitleScreenSong", true, 1f);
            Game.ScreenManager.Player.ForceInvincible = false; //TEDDY - DISABLE INVINCIBILITY WHEN YOU ENTER GAME PROPER.

            m_optionsEntered = false;
            m_startNewLegacy = false;
            m_heroIsDead = false;
            m_startNewGamePlus = false;
            m_loadStartingRoom = false;

            m_bg.TextureColor = Color.Red;
            m_crown.Visible = false;

            m_randomSeagullSFX = CDGMath.RandomInt(1, 5);
            m_startPressed = false;
            Tween.By(m_godRay, 5, Quad.EaseInOut, "Y", "-0.23");
            m_logo.Opacity = 0;
            m_logo.Position = new Vector2(1320 / 2, 720 / 2 - 50);
            Tween.To(m_logo, 2, Linear.EaseNone, "Opacity", "1");
            Tween.To(m_logo, 3, Quad.EaseInOut, "Y", "360");

            m_crown.Opacity = 0;
            m_crown.Position = new Vector2(390, 250 - 50);
            Tween.To(m_crown, 2, Linear.EaseNone, "Opacity", "1");
            Tween.By(m_crown, 3, Quad.EaseInOut, "Y", "50");

            m_dlcIcon.Opacity = 0;
            m_dlcIcon.Visible = false;
            if (Game.PlayerStats.ChallengeLastBossBeaten == true)
                m_dlcIcon.Visible = true;
            m_dlcIcon.Position = new Vector2(898, 317 - 50);
            Tween.To(m_dlcIcon, 2, Linear.EaseNone, "Opacity", "1");
            Tween.By(m_dlcIcon, 3, Quad.EaseInOut, "Y", "50");
            
            Camera.Position = new Vector2(1320 / 2, 720 / 2);

            m_pressStartText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "]";

            // Setting up save data.
            LoadSaveData();
            Game.PlayerStats.TutorialComplete = true; // Force this to true since you can't be in the title screen without going through the tutorial first.

            m_startNewLegacy = !Game.PlayerStats.CharacterFound;
            m_heroIsDead = Game.PlayerStats.IsDead;
            m_startNewGamePlus = Game.PlayerStats.LastbossBeaten;
            m_loadStartingRoom = Game.PlayerStats.LoadStartingRoom;

            if (Game.PlayerStats.TimesCastleBeaten > 0)
            {
                m_crown.Visible = true;
                m_bg.TextureColor = Color.White;
            }

            InitializeStartingText();

            UpdateCopyrightText();

            base.OnEnter();
        }

        public void UpdateCopyrightText()
        {
            m_copyrightText.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_copyrightText));
            m_copyrightText.Text = LocaleBuilder.getResourceString("LOC_ID_COPYRIGHT_GENERIC", true) + " " + string.Format(LocaleBuilder.getResourceString("LOC_ID_TRADEMARK_GENERIC", true), "Rogue Legacy");
        }

        public override void OnExit()
        {
            if (m_seagullCue != null && m_seagullCue.IsPlaying)
            {
                m_seagullCue.Stop(AudioStopOptions.Immediate);
                m_seagullCue.Dispose();
            }
            base.OnExit();
        }

        public void LoadSaveData()
        {
            SkillSystem.ResetAllTraits();
            Game.PlayerStats.Dispose();
            Game.PlayerStats = new PlayerStats();
            (ScreenManager as RCScreenManager).Player.Reset();
            (ScreenManager.Game as Game).SaveManager.LoadFiles(null, SaveType.PlayerData, SaveType.Lineage, SaveType.UpgradeData);
            // Special circumstance where you should override player's current HP/MP
            Game.ScreenManager.Player.CurrentHealth = Game.PlayerStats.CurrentHealth;
            Game.ScreenManager.Player.CurrentMana = Game.PlayerStats.CurrentMana;
        }

        public void InitializeStartingText()
        {
            m_pressStartText2.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_pressStartText));

            if (m_startNewLegacy == false)
            {
                // You have an active character who is not dead. Therefore begin the game like normal.
                if (m_heroIsDead == false)
                {
                    if (Game.PlayerStats.TimesCastleBeaten == 1)
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_2") + " +";
                    else if (Game.PlayerStats.TimesCastleBeaten > 1)
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_2") + " +" + Game.PlayerStats.TimesCastleBeaten;
                    else
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_2");
                }
                else // You have an active character but he died. Go to legacy screen.
                {
                    if (Game.PlayerStats.TimesCastleBeaten == 1)
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_3", true) + " +";
                    else if (Game.PlayerStats.TimesCastleBeaten > 1)
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_3", true) + " +" + Game.PlayerStats.TimesCastleBeaten;
                    else
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_3", true);
                }
            }
            else
            {
                // No character was found, and the castle was never beaten. Therefore you are starting a new game.
                if (m_startNewGamePlus == false)
                    m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_4", true);
                else // You've beaten the castle at least once, which means it's new game plus.
                {
                    if (Game.PlayerStats.TimesCastleBeaten == 1)
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_4", true) + " +";
                    else
                        m_pressStartText2.Text = LocaleBuilder.getResourceString("LOC_ID_TITLE_SCREEN_4", true) + " +" + Game.PlayerStats.TimesCastleBeaten;
                }
            }
        }

        public void StartPressed()
        {
            SoundManager.PlaySound("Game_Start");

            if (m_startNewLegacy == false)
            {
                if (m_heroIsDead == false) // Loading a previous file.
                {
                    if (m_loadStartingRoom == true)
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.StartingRoom, true);
                    else
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Level, true);
                }
                else // Selecting a new heir.
                    (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Lineage, true);
            }
            else
            {
                Game.PlayerStats.CharacterFound = true; // Signifies that a new game is being started.
                // Start a brand new game.
                if (m_startNewGamePlus == true) // If start new game plus is true, erase these flags.
                {
                    Game.PlayerStats.LastbossBeaten = false;
                    Game.PlayerStats.BlobBossBeaten = false;
                    Game.PlayerStats.EyeballBossBeaten = false;
                    Game.PlayerStats.FairyBossBeaten = false;
                    Game.PlayerStats.FireballBossBeaten = false;
                    Game.PlayerStats.FinalDoorOpened = false;
                    if ((ScreenManager.Game as Game).SaveManager.FileExists(SaveType.Map))
                    {
                        (ScreenManager.Game as Game).SaveManager.ClearFiles(SaveType.Map, SaveType.MapData);
                        (ScreenManager.Game as Game).SaveManager.ClearBackupFiles(SaveType.Map, SaveType.MapData);
                    }
                }
                else
                    Game.PlayerStats.Gold = 0;

                // Default data that needs to be restarted when starting a new game.
                //Game.PlayerStats.Gold = 0;
                Game.PlayerStats.HeadPiece = (byte)CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);// Necessary to change his headpiece so he doesn't look like the first dude.
                Game.PlayerStats.EnemiesKilledInRun.Clear();

                (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.Lineage, SaveType.UpgradeData); // Create new player, lineage, and upgrade data.
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.StartingRoom, true);
            }

            SoundManager.StopMusic(0.2f);
        }

        public override void Update(GameTime gameTime)
        {
            //m_castle.Position = new Vector2(InputSystem.InputManager.MouseX, InputSystem.InputManager.MouseY);
            if (m_randomSeagullSFX > 0)
            {
                m_randomSeagullSFX -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_randomSeagullSFX <= 0)
                {
                    if (m_seagullCue != null && m_seagullCue.IsPlaying)
                    {
                        m_seagullCue.Stop(AudioStopOptions.Immediate);
                        m_seagullCue.Dispose();
                    }
                    m_seagullCue = SoundManager.PlaySound("Wind1");
                    m_randomSeagullSFX = CDGMath.RandomInt(10, 15);
                }
            }

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //m_smallCloud1.Rotation += 0.03f;
            //m_smallCloud2.Rotation += 0.02f;
            //m_smallCloud3.Rotation += 0.05f;
            //m_smallCloud4.Rotation -= 0.01f;
            //m_smallCloud5.Rotation -= 0.03f;

            m_smallCloud1.Rotation += (1.8f * elapsedSeconds);
            m_smallCloud2.Rotation += (1.2f * elapsedSeconds);
            m_smallCloud3.Rotation += (3 * elapsedSeconds);
            m_smallCloud4.Rotation -= (0.6f * elapsedSeconds);
            m_smallCloud5.Rotation -= (1.8f * elapsedSeconds);


            //m_largeCloud2.X += 0.04f;
            m_largeCloud2.X += (2.4f * elapsedSeconds);
            if (m_largeCloud2.Bounds.Left > 1320)
                m_largeCloud2.X = 0 - m_largeCloud2.Width / 2;

            //m_largeCloud3.X -= 0.05f;
            m_largeCloud3.X -= (3 * elapsedSeconds);
            if (m_largeCloud3.Bounds.Right < 0)
                m_largeCloud3.X = 1320 + m_largeCloud3.Width / 2;

            if (m_startPressed == false)
            {
                m_pressStartText.Opacity = (float)Math.Abs(Math.Sin((float)Game.TotalGameTime * 1));
                //if (m_pressStartText.Opacity < 0.5f) m_pressStartText.Opacity = 0.5f;
            }

            // Gives it a glow effect by growing and shrinking.
            m_godRay.LightSourceSize = 1 + ((float)Math.Abs(Math.Sin((float)Game.TotalGameTime * 0.5f)) * 0.5f);

            // Needed to refresh the save state in case the player entered the options screen and deleted his/her file.
            if (m_optionsEntered == true && Game.ScreenManager.CurrentScreen == this)
            {
                m_optionsEntered = false;
                // Recheck these buttons.
                m_optionsKey.Text = "[Input:" + InputMapType.MENU_OPTIONS + "]";
                m_profileCardKey.Text = "[Input:" + InputMapType.MENU_PROFILECARD + "]";
                m_creditsKey.Text = "[Input:" + InputMapType.MENU_CREDITS + "]";
                m_profileSelectKey.Text = string.Format(LocaleBuilder.getString("LOC_ID_BACK_TO_MENU_OPTIONS_3_NEW", m_profileSelectKey), Game.GameConfig.ProfileSlot);
                //m_profileSelectKey.Text = "[Input:" + InputMapType.MENU_PROFILESELECT + "] " + LocaleBuilder.getString("LOC_ID_BACK_TO_MENU_OPTIONS_3", m_profileSelectKey) + " (" + Game.GameConfig.ProfileSlot + ")";

                // Recheck save data. This might not be needed any longer. Not sure.
                //InitializeSaveData();
            }

            base.Update(gameTime);
        }

        public override void HandleInput()
        {
            HandleAchievementInput();

            //ChangeRay();
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                StartPressed();

            if (m_startNewLegacy == false)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_PROFILECARD))
                    (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.ProfileCard, false, null);
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_OPTIONS))
            {
                m_optionsEntered = true;
                List<object> optionsData = new List<object>();
                optionsData.Add(true);
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Options, false, optionsData);
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CREDITS))// && InputManager.Pressed(Keys.LeftAlt, PlayerIndex.One) == false && InputManager.Pressed(Keys.RightAlt, PlayerIndex.One) == false) // Make sure not to load credits if alttabbing.                
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Credits, false, null);

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_PROFILESELECT))
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.ProfileSelect, false, null);

            base.HandleInput();
        }

        public void HandleAchievementInput()
        {
            if (InputManager.Pressed(Keys.LeftAlt, PlayerIndex.One) && InputManager.Pressed(Keys.CapsLock, PlayerIndex.One))
            {
                if (InputManager.JustPressed(Keys.T, PlayerIndex.One))
                {
                    if (GameUtil.IsAchievementUnlocked("FEAR_OF_LIFE") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_DECISIONS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_WEALTH") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_GOLD") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_NUDITY") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_THROWING_STUFF_OUT") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_MAGIC") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_CHANGE") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_EYES") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_GHOSTS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_FIRE") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_SLIME") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_FATHERS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_ANIMALS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_TWINS") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_BOOKS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_CHICKENS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_GRAVITY") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_IMPERFECTIONS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_SLEEP") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_CLOWNS") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_KNOWLEDGE") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_RELATIVES") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_CHEMICALS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_BONES") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_BLINDNESS") == true && 
                        GameUtil.IsAchievementUnlocked("FEAR_OF_SPACE") == true && 
                        GameUtil.IsAchievementUnlocked("LOVE_OF_LAUGHING_AT_OTHERS") == true)
                    {
                        Console.WriteLine("UNLOCKED THANATOPHOBIA");
                        GameUtil.UnlockAchievement("FEAR_OF_DYING");
                    }
                }
                else if (InputManager.JustPressed(Keys.S, PlayerIndex.One))
                {
                    Console.WriteLine("UNLOCKED SOMNIPHOBIA");
                    GameUtil.UnlockAchievement("FEAR_OF_SLEEP");
                }
            }
        }

        public void ChangeRay()
        {

            //if (Keyboard.GetState().IsKeyDown(Keys.F1))
            //    m_godRay.lightTexture = ScreenManager.Game.Content.Load<Texture2D>("GameSpritesheets/flare");
            //if (Keyboard.GetState().IsKeyDown(Keys.F2))
            //    m_godRay.lightTexture = ScreenManager.Game.Content.Load<Texture2D>("GameSpritesheets/flare2");
            //if (Keyboard.GetState().IsKeyDown(Keys.F3))
            //    m_godRay.lightTexture = ScreenManager.Game.Content.Load<Texture2D>("GameSpritesheets/flare3");
            //if (Keyboard.GetState().IsKeyDown(Keys.F4))
            //    m_godRay.lightTexture = ScreenManager.Game.Content.Load<Texture2D>("GameSpritesheets/flare4");

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                m_godRay.lightSource = new Vector2(m_godRay.lightSource.X, m_godRay.lightSource.Y - .01f);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                m_godRay.lightSource = new Vector2(m_godRay.lightSource.X, m_godRay.lightSource.Y + .01f);
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                m_godRay.lightSource = new Vector2(m_godRay.lightSource.X - .01f, m_godRay.lightSource.Y);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                m_godRay.lightSource = new Vector2(m_godRay.lightSource.X + .01f, m_godRay.lightSource.Y);

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
                m_godRay.Exposure += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.H))
                m_godRay.Exposure -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.U))
                m_godRay.LightSourceSize += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.J))
                m_godRay.LightSourceSize -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.I))
                m_godRay.Density += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.K))
                m_godRay.Density -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.O))
                m_godRay.Decay += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.L))
                m_godRay.Decay -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                m_godRay.Weight += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.OemSemicolon))
                m_godRay.Weight -= .01f;
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.GraphicsDevice.SetRenderTarget(m_godRayTexture);
            Camera.GraphicsDevice.Clear(Color.White);
            Camera.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null); // Anything that is affected by the godray should be drawn here.
            m_smallCloud1.DrawOutline(Camera);
            m_smallCloud3.DrawOutline(Camera);
            m_smallCloud4.DrawOutline(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            m_castle.DrawOutline(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null);
            m_smallCloud2.DrawOutline(Camera);
            m_smallCloud5.DrawOutline(Camera);
            m_logo.DrawOutline(Camera);
            m_dlcIcon.DrawOutline(Camera);
            m_crown.DrawOutline(Camera);
            //m_largeCloud1.DrawOutline(Camera);
            //m_largeCloud2.DrawOutline(Camera);
            //m_largeCloud3.DrawOutline(Camera);
            //m_largeCloud4.DrawOutline(Camera);
            Camera.End();

            // Draw the post-processed stuff to the godray render target
            m_ppm.Draw(gameTime, m_godRayTexture);

             //Anything not affected by god ray should get drawn here.
            Camera.GraphicsDevice.SetRenderTarget(m_godRayTexture);
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            m_bg.Draw(Camera);
            m_smallCloud1.Draw(Camera);
            m_smallCloud3.Draw(Camera);
            m_smallCloud4.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            m_castle.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            m_smallCloud2.Draw(Camera);
            m_smallCloud5.Draw(Camera);
            m_largeCloud1.Draw(Camera);
            m_largeCloud2.Draw(Camera);
            m_largeCloud3.Draw(Camera);
            m_largeCloud4.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(-10, -10, 1400, 800), Color.Black * m_hardCoreModeOpacity);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            m_logo.Draw(Camera);
            m_crown.Draw(Camera);
            m_copyrightText.Draw(Camera);
            m_versionNumber.Draw(Camera);

            m_pressStartText2.Opacity = m_pressStartText.Opacity;
            m_pressStartText.Draw(Camera);
            m_pressStartText2.Draw(Camera);

            if (m_startNewLegacy == false)
                m_profileCardKey.Draw(Camera);
            m_creditsKey.Draw(Camera);
            m_optionsKey.Draw(Camera);
            m_profileSelectKey.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null);
            if (m_startNewLegacy == false)
                m_profileCard.Draw(Camera);
            m_dlcIcon.Draw(Camera);
            m_optionsIcon.Draw(Camera);
            m_creditsIcon.Draw(Camera);
            Camera.End();

            // Draw the render targets to the screen
            Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black);

            Camera.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            Camera.Draw(m_ppm.Scene, new Rectangle(0, 0, Camera.GraphicsDevice.Viewport.Width, Camera.GraphicsDevice.Viewport.Height), Color.White);
            Camera.Draw(m_godRayTexture, new Rectangle(0, 0, Camera.GraphicsDevice.Viewport.Width, Camera.GraphicsDevice.Viewport.Height), Color.White);
            Camera.End();

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Title Screen");
                m_godRayTexture.Dispose();
                m_godRayTexture = null;

                m_bg.Dispose();
                m_bg = null;
                m_logo.Dispose();
                m_logo = null;
                m_castle.Dispose();
                m_castle = null;

                m_smallCloud1.Dispose();
                m_smallCloud2.Dispose();
                m_smallCloud3.Dispose();
                m_smallCloud4.Dispose();
                m_smallCloud5.Dispose();
                m_smallCloud1 = null;
                m_smallCloud2 = null;
                m_smallCloud3 = null;
                m_smallCloud4 = null;
                m_smallCloud5 = null;

                m_largeCloud1.Dispose();
                m_largeCloud1 = null;
                m_largeCloud2.Dispose();
                m_largeCloud2 = null;
                m_largeCloud3.Dispose();
                m_largeCloud3 = null;
                m_largeCloud4.Dispose();
                m_largeCloud4 = null;

                m_pressStartText.Dispose();
                m_pressStartText = null;
                m_pressStartText2.Dispose();
                m_pressStartText2 = null;
                m_copyrightText.Dispose();
                m_copyrightText = null;
                m_versionNumber.Dispose();
                m_versionNumber = null;
                m_titleText.Dispose();
                m_titleText = null;

                m_profileCard.Dispose();
                m_profileCard = null;
                m_optionsIcon.Dispose();
                m_optionsIcon = null;
                m_creditsIcon.Dispose();
                m_creditsIcon = null;

                m_profileCardKey.Dispose();
                m_profileCardKey = null;
                m_optionsKey.Dispose();
                m_optionsKey = null;
                m_creditsKey.Dispose();
                m_creditsKey = null;
                m_crown.Dispose();
                m_crown = null;
                m_profileSelectKey.Dispose();
                m_profileSelectKey = null;

                m_dlcIcon.Dispose();
                m_dlcIcon = null;

                m_seagullCue = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs() 
        {
            UpdateCopyrightText();
            m_profileSelectKey.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_BACK_TO_MENU_OPTIONS_3_NEW"), Game.GameConfig.ProfileSlot);
            //m_profileSelectKey.Text = "[Input:" + InputMapType.MENU_PROFILESELECT + "] " + LocaleBuilder.getResourceString("LOC_ID_BACK_TO_MENU_OPTIONS_3") + " (" + Game.GameConfig.ProfileSlot + ")";
            InitializeStartingText(); // refreshes m_pressStartText2
            base.RefreshTextObjs();
        }
    }
}
