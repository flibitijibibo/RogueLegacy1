using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class GameOverBossScreen : Screen
    {
        private EnemyObj_LastBoss m_lastBoss;

        private ObjContainer m_dialoguePlate;
        private KeyIconTextObj m_continueText;
        private SpriteObj m_playerGhost;

        private SpriteObj m_king;
        private SpriteObj m_spotlight;
        public float BackBufferOpacity { get; set; }

        private LineageObj m_playerFrame;
        private FrameSoundObj m_bossFallSound;
        private FrameSoundObj m_bossKneesSound;
        private Vector2 m_initialCameraPos;

        private bool m_lockControls = false;

        public GameOverBossScreen()
        {
            DrawIfCovered = true;
        }

        public override void PassInData(List<object> objList)
        {
            m_lastBoss = objList[0] as EnemyObj_LastBoss;

            m_bossKneesSound = new FrameSoundObj(m_lastBoss, 3, "FinalBoss_St2_Deathsceen_Knees");
            m_bossFallSound = new FrameSoundObj(m_lastBoss, 13, "FinalBoss_St2_Deathsceen_Fall");
            base.PassInData(objList);
        }

        public override void LoadContent()
        {
            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.FontSize = 14;
            m_continueText.Align = Types.TextAlign.Right;
            m_continueText.Opacity = 0;
            m_continueText.Position = new Vector2(1320 - 50, 30);
            m_continueText.ForceDraw = true;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list

            Vector2 shadowOffset = new Vector2(2, 2);
            Color textColour = new Color(255, 254, 128);

            m_dialoguePlate = new ObjContainer("DialogBox_Character");
            m_dialoguePlate.Position = new Vector2(1320 / 2, 610);
            m_dialoguePlate.ForceDraw = true;

            TextObj deathDescription = new TextObj(Game.JunicodeFont);
            deathDescription.Align = Types.TextAlign.Centre;
            deathDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", deathDescription); // dummy locID to add TextObj to language refresh list
            deathDescription.FontSize = 18;
            deathDescription.DropShadow = shadowOffset;
            deathDescription.Position = new Vector2(0, -m_dialoguePlate.Height / 2 + 25);
            m_dialoguePlate.AddChild(deathDescription);

            KeyIconTextObj partingWords = new KeyIconTextObj(Game.JunicodeFont);
            partingWords.FontSize = 12;
            partingWords.Align = Types.TextAlign.Centre;
            partingWords.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", partingWords); // dummy locID to add TextObj to language refresh list
            partingWords.DropShadow = shadowOffset;
            partingWords.Y = 10;
            partingWords.TextureColor = textColour;
            m_dialoguePlate.AddChild(partingWords);

            TextObj partingWordsTitle = new TextObj(Game.JunicodeFont);
            partingWordsTitle.FontSize = 8;
            partingWordsTitle.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", partingWordsTitle); // dummy locID to add TextObj to language refresh list
            partingWordsTitle.Y = partingWords.Y;
            partingWordsTitle.Y += 40;
            partingWordsTitle.X += 20;
            partingWordsTitle.DropShadow = shadowOffset;
            m_dialoguePlate.AddChild(partingWordsTitle);

            m_playerGhost = new SpriteObj("PlayerGhost_Sprite");
            m_playerGhost.AnimationDelay = 1 / 10f;

            m_spotlight = new SpriteObj("GameOverSpotlight_Sprite");
            m_spotlight.Rotation = 90;
            m_spotlight.ForceDraw = true;
            m_spotlight.Position = new Vector2(1320 / 2, 40 + m_spotlight.Height);

            m_playerFrame = new LineageObj(null, true);
            m_playerFrame.DisablePlaque = true;

            m_king = new SpriteObj("King_Sprite");
            m_king.OutlineWidth = 2;
            m_king.AnimationDelay = 1 / 10f;
            m_king.PlayAnimation(true);
            m_king.Scale = new Vector2(2, 2);

            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_initialCameraPos = Camera.Position;
            SetObjectKilledPlayerText();

            // Setting the last boss's frame.  This needs to be done before the stats are erased for the next play.
            m_playerFrame.Opacity = 0;
            m_playerFrame.Position = m_lastBoss.Position;
            m_playerFrame.SetTraits(Vector2.Zero);

            m_playerFrame.IsFemale = false;
            m_playerFrame.Class = ClassType.Knight;
            m_playerFrame.Y -= 120;
            m_playerFrame.SetPortrait(8, 1, 1); // 8 is the special intro open helm.
            m_playerFrame.UpdateData();
            Tween.To(m_playerFrame, 1f, Tween.EaseNone, "delay", "4", "Opacity", "1");

            SoundManager.StopMusic(0.5f);
            m_lockControls = false;
            SoundManager.PlaySound("Player_Death_FadeToBlack");
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_GAME_OVER_BOSS_SCREEN_1_NEW", m_continueText);

            m_lastBoss.Visible = true;
            m_lastBoss.Opacity = 1;

            m_continueText.Opacity = 0;
            m_dialoguePlate.Opacity = 0;
            m_playerGhost.Opacity = 0;
            m_spotlight.Opacity = 0;

            // Player ghost animation.
            m_playerGhost.Position = new Vector2(m_lastBoss.X - m_playerGhost.Width / 2, m_lastBoss.Bounds.Top - 20);
            Tween.RunFunction(3, typeof(SoundManager), "PlaySound", "Player_Ghost");
            //m_ghostSoundTween = Tween.RunFunction(5, typeof(SoundManager), "PlaySound", "Player_Ghost");
            Tween.To(m_playerGhost, 0.5f, Linear.EaseNone, "delay", "3", "Opacity", "0.4");
            Tween.By(m_playerGhost, 2, Linear.EaseNone, "delay", "3", "Y", "-150");
            m_playerGhost.Opacity = 0.4f;
            Tween.To(m_playerGhost, 0.5f, Linear.EaseNone, "delay", "4", "Opacity", "0");
            m_playerGhost.Opacity = 0;
            m_playerGhost.PlayAnimation(true);

            // Spotlight, Player slain text, and Backbuffer animation.
            Tween.To(this, 0.5f, Linear.EaseNone, "BackBufferOpacity", "1");
            Tween.To(m_spotlight, 0.1f, Linear.EaseNone, "delay", "1", "Opacity", "1");
            Tween.AddEndHandlerToLastTween(typeof(SoundManager), "PlaySound", "Player_Death_Spotlight");
            Tween.RunFunction(2, typeof(SoundManager), "PlaySound", "FinalBoss_St1_DeathGrunt");
            Tween.RunFunction(1.2f, typeof(SoundManager), "PlayMusic", "GameOverBossStinger", false, 0.5f);
            Tween.To(Camera, 1, Quad.EaseInOut, "X", m_lastBoss.AbsX.ToString(), "Y", (m_lastBoss.Bounds.Bottom - 10).ToString());
            Tween.RunFunction(2f, m_lastBoss, "PlayAnimation", false);
            //Tween.RunFunction(2f, m_lastBoss, "RunDeathAnimation1");

            // Setting the dialogue plate info.
            //1 = slain text
            //2 = parting words
            //3 = parting words title.

            (m_dialoguePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_3"); //"The sun... I had forgotten how it feels..."
            (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), LocaleBuilder.getResourceString(m_lastBoss.LocStringID)); //"'s Parting Words"
            //(m_dialoguePlate.GetChildAt(3) as TextObj).Text = "-" + LocaleBuilder.getResourceString(m_lastBoss.LocStringID) + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8"); //"'s Parting Words"

            Tween.To(m_dialoguePlate, 0.5f, Tween.EaseNone, "delay", "2", "Opacity", "1");
            Tween.RunFunction(4f, this, "DropStats");
            Tween.To(m_continueText, 0.4f, Linear.EaseNone, "delay", "4", "Opacity", "1");

            base.OnEnter();
        }

        public void DropStats()
        {
            Vector2 randPos = Vector2.Zero;
            float delay = 0;

            Vector2 startingPos = Camera.TopLeftCorner;
            startingPos.X += 200;
            startingPos.Y += 450;

            m_king.Position = startingPos;
            m_king.Visible = true;
            m_king.StopAnimation();
            m_king.Scale /= 2;
            m_king.Opacity = 0;
            delay += 0.05f;
            Tween.To(m_king, 0f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
        }

        private void SetObjectKilledPlayerText()
        {
            TextObj playerSlainText = m_dialoguePlate.GetChildAt(1) as TextObj;
            playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_5_NEW"), LocaleBuilder.getResourceString(m_lastBoss.LocStringID), Game.NameHelper());
            //playerSlainText.Text = m_lastBoss.Name + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_5") + " " + Game.PlayerStats.PlayerName;
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                    || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                     || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                {
                    if (m_continueText.Opacity == 1)
                    {
                        m_lockControls = true;
                        ExitTransition();
                        //Game.ScreenManager.Player.Position = new Vector2(100, 100);
                        //Game.ScreenManager.HideCurrentScreen();

                        //TextObj endingText = new TextObj(Game.JunicodeLargeFont);
                        ////endingText.Text = "Every man's life ends the same way.\nIt is only the details of how he lived and\nhow he died that distinguish one man from another.";//"In every conceivable manner, the family is link\n to our past, bridge to our future.";//"I did everything they asked...";
                        ////endingText.Text = "Too great a price was paid, for a reward of none...\nTrue clarity can only be gained through self-doubt.";
                        //endingText.Text = "Every man's life ends the same way.\nIt is only the details of how he lived and\nhow he died that distinguish one man from another.\n- Ernest Hemingway";
                        //endingText.FontSize = 25;
                        //endingText.ForceDraw = true;
                        //endingText.Align = Types.TextAlign.Centre;
                        //endingText.Position = new Vector2(1320 / 2f, 720 / 2f - 100);
                        //endingText.OutlineWidth = 2;

                        //List<object> endingTextObj = new List<object>();
                        //endingTextObj.Add(1.0f);
                        //endingTextObj.Add(0.2f);
                        //endingTextObj.Add(8f);
                        //endingTextObj.Add(true);
                        //endingTextObj.Add(endingText);
                        //endingTextObj.Add(true);
                        //SoundManager.StopMusic(1);
                        //Game.ScreenManager.DisplayScreen(ScreenType.Text, true, endingTextObj);
                        //(ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Ending, true, null);
                        //Tween.RunFunction(0, ScreenManager, "DisplayScreen", ScreenType.Ending, true, typeof(List<object>));
                    }
                }
            }
            base.HandleInput();
        }

        private void ExitTransition()
        {
            Tween.StopAll(false);
            SoundManager.StopMusic(1);
            Tween.To(this, 0.5f, Quad.EaseIn, "BackBufferOpacity", "0");
            Tween.To(m_lastBoss, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_dialoguePlate, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_continueText, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_playerGhost, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_king, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_spotlight, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(m_playerFrame, 0.5f, Quad.EaseIn, "Opacity", "0");
            Tween.To(Camera, 0.5f, Quad.EaseInOut, "X", m_initialCameraPos.X.ToString(), "Y", m_initialCameraPos.Y.ToString());
            Tween.RunFunction(0.5f, ScreenManager, "HideCurrentScreen");
        }

        public override void OnExit()
        {
            BackBufferOpacity = 0;
            Game.ScreenManager.Player.UnlockControls();
            Game.ScreenManager.Player.AttachedLevel.CameraLockedToPlayer = true;
            (Game.ScreenManager.GetLevelScreen().CurrentRoom as LastBossRoom).BossCleanup();
            base.OnExit();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_lastBoss.SpriteName == "EnemyLastBossDeath_Character")
            {
                m_bossKneesSound.Update();
                m_bossFallSound.Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformation()); // Parallax Effect has been disabled in favour of ripple effect for now.
            Camera.Draw(Game.GenericTexture, new Rectangle((int)Camera.TopLeftCorner.X - 10, (int)Camera.TopLeftCorner.Y - 10, 1420, 820), Color.Black * BackBufferOpacity);
            m_king.Draw(Camera);
            m_playerFrame.Draw(Camera);
            m_lastBoss.Draw(Camera);
            if (m_playerGhost.Opacity > 0)
                m_playerGhost.X += (float)Math.Sin(Game.TotalGameTime * 5);
            m_playerGhost.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null); // Parallax Effect has been disabled in favour of ripple effect for now.
            m_spotlight.Draw(Camera);
            m_dialoguePlate.Draw(Camera);
            m_continueText.Draw(Camera);
            Camera.End();

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Game Over Boss Screen");

                m_lastBoss = null;
                m_dialoguePlate.Dispose();
                m_dialoguePlate = null;
                m_continueText.Dispose();
                m_continueText = null;
                m_playerGhost.Dispose();
                m_playerGhost = null;
                m_spotlight.Dispose();
                m_spotlight = null;

                if (m_bossFallSound != null)
                    m_bossFallSound.Dispose();
                m_bossFallSound = null;
                if (m_bossKneesSound != null)
                    m_bossKneesSound.Dispose();
                m_bossKneesSound = null;

                m_playerFrame.Dispose();
                m_playerFrame = null;
                m_king.Dispose();
                m_king = null;

                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            //m_continueText.Text = LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_1") + " [Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_2");
            SetObjectKilledPlayerText();
            (m_dialoguePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_BOSS_SCREEN_3"); //"The sun... I had forgotten how it feels..."
            (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), LocaleBuilder.getResourceString(m_lastBoss.LocStringID)); //"'s Parting Words"
            //(m_dialoguePlate.GetChildAt(3) as TextObj).Text = "-" + m_lastBoss.Name + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8"); //"'s Parting Words"
            base.RefreshTextObjs();
        }
    }
}
