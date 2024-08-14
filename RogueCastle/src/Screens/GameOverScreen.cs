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
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class GameOverScreen : Screen
    {
        private PlayerObj m_player;

        private ObjContainer m_dialoguePlate;
        private KeyIconTextObj m_continueText;
        private SpriteObj m_playerGhost;
        private SpriteObj m_spotlight;
        public float BackBufferOpacity { get; set; }

        private List<EnemyObj> m_enemyList;
        private List<Vector2> m_enemyStoredPositions;
        private int m_coinsCollected;
        private int m_bagsCollected;
        private int m_diamondsCollected;
        private int m_bigDiamondsCollected;

        private FrameSoundObj m_playerFallSound;
        private FrameSoundObj m_playerSwordSpinSound;
        private FrameSoundObj m_playerSwordFallSound;

        private GameObj m_objKilledPlayer;
        private LineageObj m_playerFrame;

        private bool m_lockControls = false;
        private bool m_droppingStats;

        private int m_gameHint;

        public GameOverScreen()
        {
            m_enemyStoredPositions = new List<Vector2>();
        }

        public override void PassInData(List<object> objList)
        {
            if (objList != null)
            {
                m_player = objList[0] as PlayerObj;

                if (m_playerFallSound == null)
                {
                    m_playerFallSound = new FrameSoundObj(m_player, 14, "Player_Death_BodyFall");
                    m_playerSwordSpinSound = new FrameSoundObj(m_player, 2, "Player_Death_SwordTwirl");
                    m_playerSwordFallSound = new FrameSoundObj(m_player, 9, "Player_Death_SwordLand");
                }

                m_enemyList = objList[1] as List<EnemyObj>;
                m_coinsCollected = (int)objList[2];
                m_bagsCollected = (int)objList[3];
                m_diamondsCollected = (int)objList[4];
                m_bigDiamondsCollected = (int)objList[5];
                if (objList[6] != null)
                    m_objKilledPlayer = objList[6] as GameObj;
                SetObjectKilledPlayerText();

                m_enemyStoredPositions.Clear();
                base.PassInData(objList);
            }
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
            deathDescription.FontSize = 17;
            deathDescription.DropShadow = shadowOffset;
            deathDescription.Position = new Vector2(0, -m_dialoguePlate.Height / 2 + 25);
            m_dialoguePlate.AddChild(deathDescription);

            KeyIconTextObj partingWords = new KeyIconTextObj(Game.JunicodeFont);
            partingWords.FontSize = 12;
            partingWords.Align = Types.TextAlign.Centre;
            partingWords.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", partingWords); // dummy locID to add TextObj to language refresh list
            partingWords.DropShadow = shadowOffset;
            partingWords.Y = 0;
            partingWords.TextureColor = textColour;
            m_dialoguePlate.AddChild(partingWords);

            TextObj partingWordsTitle = new TextObj(Game.JunicodeFont);
            partingWordsTitle.FontSize = 8;
            partingWordsTitle.Text = "";// LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", partingWordsTitle); // dummy locID to add TextObj to language refresh list
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

            base.LoadContent();
        }

        public override void OnEnter()
        {
            m_debugEnemyLocID = -1;

            // Setting the player frame.  This needs to be done before the stats are erased for the next play.
            m_playerFrame.Opacity = 0;
            m_playerFrame.Position = m_player.Position;
            m_playerFrame.SetTraits(Game.PlayerStats.Traits);
            m_playerFrame.IsFemale = Game.PlayerStats.IsFemale;
            m_playerFrame.Class = Game.PlayerStats.Class;
            m_playerFrame.Y -= 120;
            m_playerFrame.SetPortrait((byte)Game.PlayerStats.HeadPiece, (byte)Game.PlayerStats.ShoulderPiece, (byte)Game.PlayerStats.ChestPiece);
            m_playerFrame.UpdateData();
            Tween.To(m_playerFrame, 1f, Tween.EaseNone, "delay", "4", "Opacity", "1");

            // Creating a new family tree node and saving.
            FamilyTreeNode newNode = new FamilyTreeNode()
            {
                Name = Game.PlayerStats.PlayerName,
                Age = Game.PlayerStats.Age,
                ChildAge = Game.PlayerStats.ChildAge,
                Class = Game.PlayerStats.Class,
                HeadPiece = Game.PlayerStats.HeadPiece,
                ChestPiece = Game.PlayerStats.ChestPiece,
                ShoulderPiece = Game.PlayerStats.ShoulderPiece,
                NumEnemiesBeaten = Game.PlayerStats.NumEnemiesBeaten,
                BeatenABoss = Game.PlayerStats.NewBossBeaten,
                Traits = Game.PlayerStats.Traits,
                IsFemale = Game.PlayerStats.IsFemale,
                RomanNumeral = Game.PlayerStats.RomanNumeral,
            };

            Vector2 storedTraits = Game.PlayerStats.Traits;
            Game.PlayerStats.FamilyTreeArray.Add(newNode);
            if (Game.PlayerStats.CurrentBranches != null)
                Game.PlayerStats.CurrentBranches.Clear();

            // Setting necessary after-death flags and saving.
            Game.PlayerStats.IsDead = true;
            Game.PlayerStats.Traits = Vector2.Zero;
            Game.PlayerStats.NewBossBeaten = false;
            Game.PlayerStats.RerolledChildren = false;
            Game.PlayerStats.HasArchitectFee = false;
            Game.PlayerStats.NumEnemiesBeaten = 0;
            Game.PlayerStats.LichHealth = 0;
            Game.PlayerStats.LichMana = 0;
            Game.PlayerStats.LichHealthMod = 1;
            Game.PlayerStats.TimesDead++;
            Game.PlayerStats.LoadStartingRoom = true;
            Game.PlayerStats.EnemiesKilledInRun.Clear();

            if (Game.PlayerStats.SpecialItem != SpecialItemType.FreeEntrance &&
                Game.PlayerStats.SpecialItem != SpecialItemType.EyeballToken &&
                Game.PlayerStats.SpecialItem != SpecialItemType.SkullToken &&
                Game.PlayerStats.SpecialItem != SpecialItemType.FireballToken &&
                Game.PlayerStats.SpecialItem != SpecialItemType.BlobToken &&
                Game.PlayerStats.SpecialItem != SpecialItemType.LastBossToken)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;

            // Ensures the prosopagnosia effect kicks in when selecting an heir.
            if (storedTraits.X == (int)TraitType.Prosopagnosia || storedTraits.Y == (int)TraitType.Prosopagnosia)
                Game.PlayerStats.HasProsopagnosia = true;

            (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.Lineage, SaveType.MapData);
            (ScreenManager.Game as Game).SaveManager.SaveAllFileTypes(true); // Save the backup the moment the player dies.

            // The player's traits need to be restored to so that his death animation matches the player.
            Game.PlayerStats.Traits = storedTraits;

            // Setting achievements.
            if (Game.PlayerStats.TimesDead >= 20)
                GameUtil.UnlockAchievement("FEAR_OF_LIFE");

            ////////////////////////////////////////////////////////////////////////////////

            SoundManager.StopMusic(0.5f);
            m_droppingStats = false;
            m_lockControls = false;
            SoundManager.PlaySound("Player_Death_FadeToBlack");
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_GAME_OVER_SCREEN_1_NEW", m_continueText);

            m_player.Visible = true;
            m_player.Opacity = 1;

            m_continueText.Opacity = 0;
            m_dialoguePlate.Opacity = 0;
            m_playerGhost.Opacity = 0;
            m_spotlight.Opacity = 0;

            // Player ghost animation.
            m_playerGhost.Position = new Vector2(m_player.X - m_playerGhost.Width / 2, m_player.Bounds.Top - 20);
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
            Tween.RunFunction(1.2f, typeof(SoundManager), "PlayMusic", "GameOverStinger", false, 0.5f);
            Tween.To(Camera, 1, Quad.EaseInOut, "X", this.m_player.AbsX.ToString(), "Y", (this.m_player.Bounds.Bottom - 10).ToString(), "Zoom", "1");
            Tween.RunFunction(2f, m_player, "RunDeathAnimation1");

            // Setting the dialogue plate info.
            //1 = slain text
            //2 = parting words
            //3 = parting words title.

            if (Game.PlayerStats.Traits.X == TraitType.Tourettes || Game.PlayerStats.Traits.Y == TraitType.Tourettes)
            {
                (m_dialoguePlate.GetChildAt(2) as TextObj).Text = "#)!(%*#@!%^"; // not localized
                (m_dialoguePlate.GetChildAt(2) as TextObj).RandomizeSentence(true);
            }
            else
            {
                m_gameHint = CDGMath.RandomInt(0, GameEV.GAME_HINTS.GetLength(0) - 1);
                (m_dialoguePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint]);
                FixHintTextSize();
                //(m_dialoguePlate.GetChildAt(2) as TextObj).Text =
                //    LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint, 0]) +
                //    GameEV.GAME_HINTS[m_gameHint, 1] +
                //    LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint, 2]);
            }
        
            try
            {
                (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault((m_dialoguePlate.GetChildAt(3) as TextObj).defaultFont);
                (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), Game.NameHelper()); //"'s Parting Words"
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch((m_dialoguePlate.GetChildAt(3) as TextObj).Text, @"\p{IsCyrillic}"))
                    (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.NotoSansSCFont);
                (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), Game.NameHelper()); //"'s Parting Words"
            }
            //(m_dialoguePlate.GetChildAt(3) as TextObj).Text = "-" + Game.PlayerStats.PlayerName + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8");

            Tween.To(m_dialoguePlate, 0.5f, Tween.EaseNone, "delay", "2", "Opacity", "1");
            Tween.RunFunction(4f, this, "DropStats");
            Tween.To(m_continueText, 0.4f, Linear.EaseNone, "delay", "4", "Opacity", "1");

            base.OnEnter();
        }

        public override void OnExit()
        {
            Tween.StopAll(false);
            if (m_enemyList != null)
            {
                m_enemyList.Clear();
                m_enemyList = null;
            }

            Game.PlayerStats.Traits = Vector2.Zero;

            BackBufferOpacity = 0;
            
            base.OnExit();
        }

        public void DropStats()
        {
            m_droppingStats = true;
            Vector2 randPos = Vector2.Zero;
            float delay = 0;

            Vector2 startingPos = Camera.TopLeftCorner;
            startingPos.X += 200;
            startingPos.Y += 450;
            // Dropping enemies

            //m_enemyList = new List<EnemyObj>();
            //for (int i = 0; i < 120; i++)
            //{
            //    EnemyObj_Skeleton enemy = new EnemyObj_Skeleton(m_player, null, m_player.AttachedLevel, GameTypes.EnemyDifficulty.BASIC);
            //    enemy.Initialize();
            //    m_enemyList.Add(enemy);
            //}

            //m_enemyList = new List<EnemyObj>();
            //EnemyObj_Fireball fireBall = new EnemyObj_Fireball(m_player, null, m_player.AttachedLevel, GameTypes.EnemyDifficulty.MINIBOSS);
            //fireBall.Initialize();
            //m_enemyList.Add(fireBall);

            //EnemyObj_Eyeball eyeball = new EnemyObj_Eyeball(m_player, null, m_player.AttachedLevel, GameTypes.EnemyDifficulty.MINIBOSS);
            //eyeball.Initialize();
            //m_enemyList.Add(eyeball);

            //EnemyObj_Fairy fairy = new EnemyObj_Fairy(m_player, null, m_player.AttachedLevel, GameTypes.EnemyDifficulty.MINIBOSS);
            //fairy.Initialize();
            //m_enemyList.Add(fairy);

            //EnemyObj_Blob blob = new EnemyObj_Blob(m_player, null, m_player.AttachedLevel, GameTypes.EnemyDifficulty.MINIBOSS);
            //blob.Initialize();
            //m_enemyList.Add(blob);

            if (m_enemyList != null)
            {
                foreach (EnemyObj enemy in m_enemyList)
                {
                    m_enemyStoredPositions.Add(enemy.Position);
                    enemy.Position = startingPos;
                    enemy.ChangeSprite(enemy.ResetSpriteName);
                    if (enemy.SpriteName == "EnemyZombieRise_Character")
                        enemy.ChangeSprite("EnemyZombieWalk_Character");
                    enemy.Visible = true;
                    enemy.Flip = SpriteEffects.FlipHorizontally;
                    Tween.StopAllContaining(enemy, false);
                    enemy.Scale = enemy.InternalScale;
                    enemy.Scale /= 2;
                    enemy.Opacity = 0;
                    delay += 0.05f;

                    // Special handling for the eyeball boss's pupil.
                    EnemyObj_Eyeball eyeBoss = enemy as EnemyObj_Eyeball;
                    if (eyeBoss != null && eyeBoss.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                        eyeBoss.ChangeToBossPupil();

                    Tween.To(enemy, 0f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                    Tween.RunFunction(delay, this, "PlayEnemySound");
                    startingPos.X += 25;
                    if (enemy.X + enemy.Width > Camera.TopLeftCorner.X + 200 + 950)
                    {
                        startingPos.Y += 30;
                        startingPos.X = Camera.TopLeftCorner.X + 200;
                    }
                }
            }
        }

        public void PlayEnemySound()
        {
            SoundManager.PlaySound("Enemy_Kill_Plant");
        }

        private void SetObjectKilledPlayerText()
        {
            TextObj playerSlainText = m_dialoguePlate.GetChildAt(1) as TextObj;

            try
            {
                playerSlainText.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(playerSlainText));
                playerSlainText.Text = Game.PlayerStats.PlayerName;
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(playerSlainText.Text, @"\p{IsCyrillic}"))
                    playerSlainText.ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                playerSlainText.ChangeFontNoDefault(Game.NotoSansSCFont);
            }

            if (m_debugEnemyLocID > 0)
                playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_4_NEW"), Game.NameHelper(), LocaleBuilder.getResourceString("LOC_ID_ENEMY_NAME_" + m_debugEnemyLocID));
            else
            {
                if (m_objKilledPlayer != null)
                {
                    EnemyObj enemy = m_objKilledPlayer as EnemyObj;
                    ProjectileObj projectile = m_objKilledPlayer as ProjectileObj;

                    if (enemy != null)
                    {
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS || enemy is EnemyObj_LastBoss)
                            playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_3_NEW"), Game.NameHelper(), LocaleBuilder.getResourceString(enemy.LocStringID));
                        //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_3") + " " + LocaleBuilder.getResourceString(enemy.LocStringID);
                        else
                            playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_4_NEW"), Game.NameHelper(), LocaleBuilder.getResourceString(enemy.LocStringID));
                        //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_4") + " " + LocaleBuilder.getResourceString(enemy.LocStringID);
                    }
                    else if (projectile != null)
                    {
                        enemy = projectile.Source as EnemyObj;
                        if (enemy != null)
                        {
                            if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS || enemy is EnemyObj_LastBoss)
                                playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_3_NEW"), Game.NameHelper(), LocaleBuilder.getResourceString(enemy.LocStringID));
                            //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_3") + " " + LocaleBuilder.getResourceString(enemy.LocStringID);
                            else
                                playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_4_NEW"), Game.NameHelper(), LocaleBuilder.getResourceString(enemy.LocStringID));
                            //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_4") + " " + LocaleBuilder.getResourceString(enemy.LocStringID);
                        }
                        else
                            playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_5_NEW"), Game.NameHelper());
                        //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_5");
                    }

                    HazardObj hazard = m_objKilledPlayer as HazardObj;
                    if (hazard != null)
                        playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_6_NEW"), Game.NameHelper());
                    //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_6");
                }
                else
                    playerSlainText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_7_NEW"), Game.NameHelper());
                //playerSlainText.Text = Game.PlayerStats.PlayerName + " " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_7");
            }
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (m_droppingStats == true)
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                        || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                         || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    {
                        if (m_enemyList.Count > 0 && m_enemyList[m_enemyList.Count - 1].Opacity != 1)
                        {
                            foreach (EnemyObj enemy in m_enemyList)
                            {
                                Tween.StopAllContaining(enemy, false);
                                enemy.Opacity = 1;
                            }
                            Tween.StopAllContaining(this, false);
                            PlayEnemySound();
                        }
                        else //if (m_continueText.Opacity == 1)
                        {
                            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Title, true, null);
                            m_lockControls = true;
                        }
                    }
                }
            }
            base.HandleInput();
        }

        public override void Update(GameTime gameTime)
        {
            if (LevelEV.ENABLE_DEBUG_INPUT == true)
                HandleDebugInput();

            if (m_player.SpriteName == "PlayerDeath_Character")
            {
                m_playerFallSound.Update();
                m_playerSwordFallSound.Update();
                m_playerSwordSpinSound.Update();
            }

            base.Update(gameTime);
        }

        private int m_debugGameHint = -1;
        private int m_debugEnemyLocID = 0;
        private int m_debugTotalEnemies = 127;
        private void HandleDebugInput()
        {
            if (InputManager.JustPressed(Keys.Space, PlayerIndex.One))
            {
                m_gameHint = m_debugGameHint;
                Console.WriteLine("Changing to game hint index: " + m_debugGameHint);

                (m_dialoguePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint]);
                //(m_dialoguePlate.GetChildAt(2) as TextObj).Text =
                //    LocaleBuilder.getString(GameEV.GAME_HINTS[m_gameHint, 0], m_dialoguePlate.GetChildAt(2) as TextObj) +
                //    GameEV.GAME_HINTS[m_gameHint, 1] +
                //    LocaleBuilder.getString(GameEV.GAME_HINTS[m_gameHint, 2], m_dialoguePlate.GetChildAt(2) as TextObj);
                m_debugGameHint++;
                if (m_debugGameHint >= GameEV.GAME_HINTS.GetLength(0))
                    m_debugGameHint = 0;
            }

            int previousEnemyLocID = m_debugEnemyLocID;
            if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                m_debugEnemyLocID--;
            else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                m_debugEnemyLocID++;

            if (m_debugEnemyLocID <= 0 && m_debugEnemyLocID != -1)
                m_debugEnemyLocID = m_debugTotalEnemies;
            else if (m_debugEnemyLocID > m_debugTotalEnemies)
                m_debugEnemyLocID = 1;

            if (m_debugEnemyLocID != previousEnemyLocID)
                SetObjectKilledPlayerText();
        }

        public override void Draw(GameTime gameTime)
        {
            //Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformation()); // Parallax Effect has been disabled in favour of ripple effect for now.
            Camera.Draw(Game.GenericTexture, new Rectangle((int)Camera.TopLeftCorner.X - 10, (int)Camera.TopLeftCorner.Y - 10, 1420, 820), Color.Black * BackBufferOpacity);
            foreach (EnemyObj enemy in m_enemyList)
                enemy.Draw(Camera);
            m_playerFrame.Draw(Camera);
            m_player.Draw(Camera);
            if (m_playerGhost.Opacity > 0)
                m_playerGhost.X += (float)Math.Sin(Game.TotalGameTime * 5) * 60 * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                Console.WriteLine("Disposing Game Over Screen");

                m_player = null;
                m_dialoguePlate.Dispose();
                m_dialoguePlate = null;
                m_continueText.Dispose();
                m_continueText = null;
                m_playerGhost.Dispose();
                m_playerGhost = null;
                m_spotlight.Dispose();
                m_spotlight = null;

                if (m_playerFallSound != null)
                    m_playerFallSound.Dispose();
                m_playerFallSound = null;
                if (m_playerSwordFallSound != null)
                    m_playerSwordFallSound.Dispose();
                m_playerSwordFallSound = null;
                if (m_playerSwordSpinSound != null)
                    m_playerSwordSpinSound.Dispose();
                m_playerSwordSpinSound = null;

                m_objKilledPlayer = null;

                if (m_enemyList != null)
                    m_enemyList.Clear();
                m_enemyList = null;
                if (m_enemyStoredPositions != null)
                    m_enemyStoredPositions.Clear();
                m_enemyStoredPositions = null;

                m_playerFrame.Dispose();
                m_playerFrame = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            //m_continueText.Text = LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_1") + " [Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_2");
            (m_dialoguePlate.GetChildAt(2) as TextObj).ChangeFontNoDefault(LocaleBuilder.GetLanguageFont((m_dialoguePlate.GetChildAt(2) as TextObj)));
            (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(LocaleBuilder.GetLanguageFont((m_dialoguePlate.GetChildAt(3) as TextObj)));

            if (Game.PlayerStats.Traits.X == TraitType.Tourettes || Game.PlayerStats.Traits.Y == TraitType.Tourettes)
            {
                (m_dialoguePlate.GetChildAt(2) as TextObj).Text = "#)!(%*#@!%^"; // not localized
                (m_dialoguePlate.GetChildAt(2) as TextObj).RandomizeSentence(true);
            }
            else
            {
                (m_dialoguePlate.GetChildAt(2) as TextObj).Text = LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint]);
                //(m_dialoguePlate.GetChildAt(2) as TextObj).Text =
                //    LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint, 0]) +
                //    GameEV.GAME_HINTS[m_gameHint, 1] +
                //    LocaleBuilder.getResourceString(GameEV.GAME_HINTS[m_gameHint, 2]);
            }

            try
            {
                (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(LocaleBuilder.GetLanguageFont((m_dialoguePlate.GetChildAt(3) as TextObj)));
                (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), Game.NameHelper()); //"'s Parting Words"
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch((m_dialoguePlate.GetChildAt(3) as TextObj).Text, @"\p{IsCyrillic}"))
                    (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                (m_dialoguePlate.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.NotoSansSCFont);
                (m_dialoguePlate.GetChildAt(3) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8_NEW"), Game.NameHelper()); //"'s Parting Words"
            }
            //(m_dialoguePlate.GetChildAt(3) as TextObj).Text = "-" + Game.PlayerStats.PlayerName + LocaleBuilder.getResourceString("LOC_ID_GAME_OVER_SCREEN_8");

            FixHintTextSize();
            SetObjectKilledPlayerText();
            base.RefreshTextObjs();
        }

        private void FixHintTextSize()
        {
            TextObj partingWords = (m_dialoguePlate.GetChildAt(2) as TextObj);
            partingWords.FontSize = 12;
            partingWords.ScaleX = 1;

            switch (LocaleBuilder.languageType)
            {
                case(LanguageType.Russian):
                    if (m_gameHint == 6)
                    {
                        partingWords.FontSize = 11;
                        partingWords.ScaleX = 0.9f;
                    }
                    break;
                case(LanguageType.French):
                    if (m_gameHint == 12 || m_gameHint == 20)
                        partingWords.ScaleX = 0.9f;
                    else if (m_gameHint == 35)
                    {
                        partingWords.FontSize = 10;
                        partingWords.ScaleX = 0.9f;
                    }
                    break;
                case(LanguageType.German):
                    switch (m_gameHint)
                    {
                        case(18):
                        case(27):
                        case(29):
                        case(30):
                        case(35):
                            partingWords.ScaleX = 0.9f;
                            break;
                    }
                    break;
                case(LanguageType.Portuguese_Brazil):
                    if (m_gameHint == 18)
                        partingWords.ScaleX = 0.9f;
                    break;
                case(LanguageType.Polish):
                    if (m_gameHint == 18)
                        partingWords.ScaleX = 0.9f;
                    break;
                case(LanguageType.Spanish_Spain):
                    if (m_gameHint == 29)
                        partingWords.ScaleX = 0.9f;
                    else if (m_gameHint == 35)
                    {
                        partingWords.FontSize = 11;
                        partingWords.ScaleX = 0.9f;
                    }
                    break;
            }
        }
    }
}
