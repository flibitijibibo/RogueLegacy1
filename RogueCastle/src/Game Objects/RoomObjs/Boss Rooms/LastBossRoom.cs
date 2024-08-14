using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tweener.Ease;
using Tweener;

namespace RogueCastle
{
    public class LastBossRoom : BossRoomObj
    {
        private EnemyObj_LastBoss m_boss;
        public float BackBufferOpacity { get; set; }

        private bool m_shake = false;
        private bool m_shookLeft = false;

        private float m_shakeTimer = 0.03f;
        private float m_shakeDuration = 0.03f;

        private ObjContainer m_fountain;
        private DoorObj m_bossDoor;
        private SpriteObj m_bossDoorSprite;

        //8000
        private int m_bossCoins = 40;
        private int m_bossMoneyBags = 16;
        private int m_bossDiamonds = 7;

        public LastBossRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_LastBoss;

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "fountain")
                    m_fountain = obj as ObjContainer;

                if (obj.Name == "stainglass")
                    obj.Opacity = 0.5f;

                if (obj.Name == "door")
                    m_bossDoorSprite = obj as SpriteObj;
            }

            foreach (DoorObj door in DoorList)
            {
                if (door.Name == "FinalBossDoor")
                {
                    m_bossDoor = door;
                    m_bossDoor.Locked = true;
                    break;
                }
            }

            base.Initialize();
        }

        public override void OnEnter()
        {
            Player.AttachedLevel.RemoveCompassDoor();

            m_boss.Level += LevelEV.LAST_BOSS_MODE1_LEVEL_MOD;
            m_boss.CurrentHealth = m_boss.MaxHealth;

            //SoundManager.PlayMusic("CastleBossSong", true);
            BackBufferOpacity = 0;
            SoundManager.StopMusic(0.5f);

            StartCutscene();
            base.OnEnter();
        }

        public void StartCutscene()
        {
            m_cutsceneRunning = true;

            //Player.UpdateCollisionBoxes(); // Necessary check since the OnEnter() is called before player can update its collision boxes.
            //Player.Y = 1440 - (60 * 5) - (Player.Bounds.Bottom - Player.Y);
            Player.LockControls();
            Player.AccelerationY = 0;
            Player.AttachedLevel.RunCinematicBorders(8);
            Player.Flip = SpriteEffects.None;
            Player.State = PlayerObj.STATE_WALKING;

            LogicSet playerMoveLS = new LogicSet(Player);
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsWeighted", false));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsCollidable", false));
            playerMoveLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            playerMoveLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character"));
            playerMoveLS.AddAction(new PlayAnimationLogicAction(true));
            playerMoveLS.AddAction(new DelayLogicAction(1.5f));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 0));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsWeighted", true));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsCollidable", true));
            Player.RunExternalLogicSet(playerMoveLS);

            Tween.RunFunction(1.6f, this, "Cutscene2");
        }

        public void Cutscene2()
        {
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Tween.By(Player.AttachedLevel.Camera, 1.5f, Quad.EaseInOut, "X", "300");
            Tween.AddEndHandlerToLastTween(this, "Cutscene3");
        }

        public void Cutscene3()
        {
            Tween.RunFunction(0.5f, this, "Cutscene4");

            // Must be called first so that the tween is paused when the dialogue screen is displayed.
            RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
            if (Game.PlayerStats.Class == ClassType.Traitor)
            {
                manager.DialogueScreen.SetDialogue("FinalBossTalk01_Special");
                GameUtil.UnlockAchievement("LOVE_OF_LAUGHING_AT_OTHERS");
            }
            else
                manager.DialogueScreen.SetDialogue("FinalBossTalk01");
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        public void Cutscene4()
        {
            //if (Game.PlayerStats.DiaryEntry >= LevelEV.TOTAL_JOURNAL_ENTRIES)
                Tween.RunFunction(0.5f, this, "DisplayBossTitle", "LOC_ID_ENEMY_NAME_123", m_boss.LocStringID, "Cutscene5");
            //else
              //  Tween.RunFunction(0.5f, this, "DisplayBossTitle", "?????", "???????", "Cutscene5");
        }

        public void Cutscene5()
        {
            Tween.To(Player.AttachedLevel.Camera, 1f, Quad.EaseInOut, "X", Player.X.ToString());
            Tween.AddEndHandlerToLastTween(this, "BeginBattle");
        }

        public void BeginBattle()
        {
            //SoundManager.PlayMusic("TowerSong", true, 1);
            //SoundManager.PlayMusic("CastleBossSong", true, 1);
            SoundManager.PlayMusic("TitleScreenSong", true, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
            m_cutsceneRunning = false;
        }

        private float m_playerX = 0;
        public void RunFountainCutscene()
        {
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Camera2D camera = Player.AttachedLevel.Camera;
            m_playerX = camera.X;

            SoundManager.PlaySound("Cutsc_CameraMove");
            Tween.To(camera, 1f, Tweener.Ease.Quad.EaseInOut, "X", (m_fountain.Bounds.Center.X - 400).ToString());
            Tween.RunFunction(2, this, "RunFountainCutscene2");
        }

        public void RunFountainCutscene2()
        {
            // Animation for breaking the fountain goes here.
            //SoundManager.PlayMusic("LastBossSong", true, 1);
            //ProceduralLevelScreen level = Player.AttachedLevel;
            //level.RunWhiteSlash2();
            //Tween.RunFunction(0, this, "DisableFountain");
            StartShake();
            SoundManager.PlaySound("Cutsc_StatueCrumble");
            m_fountain.ChangeSprite("FountainOfYouthShatter_Character");
            m_fountain.PlayAnimation(false);
            Player.AttachedLevel.ImpactEffectPool.DisplayFountainShatterSmoke(m_fountain);
            Tween.RunFunction(2f, this, "DisplaySecondBoss");
            Tween.RunFunction(2, this, "RunFountainCutscene3");
        }

        public void DisplaySecondBoss()
        {
            m_boss.SecondFormComplete();
            m_boss.UpdateCollisionBoxes();
            m_boss.Position = new Vector2(m_fountain.X, m_fountain.Y - (m_boss.Bounds.Bottom - m_boss.Y));
        }

        public void RunFountainCutscene3()
        {
            SoundManager.PlaySound("FinalBoss_St2_BlockLaugh");
            SoundManager.PlayMusic("LastBossSong", true, 1);
            m_fountain.Visible = false;
            StopShake();
            //m_boss.SecondFormComplete();
            //m_boss.UpdateCollisionBoxes();
            //m_boss.Position = new Vector2(m_fountain.X, m_fountain.Y - (m_boss.Bounds.Bottom - m_boss.Y));
            //Tween.RunFunction(2, this, "RunFountainCutscene4");
            Tween.RunFunction(2, this, "DisplayBossTitle", "LOC_ID_ENEMY_NAME_75", "LOC_ID_ENEMY_NAME_124", "RunFountainCutscene4");
        }

        public void StartShake()
        {
            m_shake = true;
        }

        public void StopShake()
        {
            m_shake = false;
        }

        public void DisableFountain()
        {
            m_fountain.Visible = false;
        }

        public void RunFountainCutscene4()
        {
            Tween.To(Player.AttachedLevel.Camera, 1f, Tweener.Ease.Quad.EaseInOut, "X", m_playerX.ToString());
            Tween.AddEndHandlerToLastTween(m_boss, "SecondFormActive");
        }

        public override void Update(GameTime gameTime)
        {
            if (m_shake == true)
            {
                if (m_shakeTimer > 0)
                {
                    m_shakeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_shakeTimer <= 0)
                    {
                        Camera2D camera = Player.AttachedLevel.Camera;

                        m_shakeTimer = m_shakeDuration;
                        if (m_shookLeft == true)
                        {
                            m_shookLeft = false;
                            camera.X += 5;
                        }
                        else
                        {
                            camera.X -= 5;
                            m_shookLeft = true;
                        }
                    }
                }
            }

            if (m_cutsceneRunning == false)
            {
                foreach (EnemyObj enemy in this.EnemyList)
                {
                    if (enemy.IsKilled == false)
                        enemy.Update(gameTime);
                }

                foreach (EnemyObj enemy in this.TempEnemyList)
                    if (enemy.IsKilled == false)
                        enemy.Update(gameTime);
            }
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            camera.Draw(Game.GenericTexture, new Rectangle((int)this.X, (int)this.Y, this.Width, this.Height), Color.White * BackBufferOpacity);
        }

        public void ChangeWindowOpacity()
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "stainglass")
                    Tween.To(obj, 2, Tween.EaseNone, "Opacity", "0.2");
            }
        }

        public override void BossCleanup()
        {
            Player.StopAllSpells();
            Game.PlayerStats.NewBossBeaten = true;
            m_bossDoorSprite.ChangeSprite("CastleDoorOpen_Sprite");
            m_bossDoor.Locked = false;
            SoundManager.PlaySound("FinalBoss_St2_WeatherChange_b");
            DropGold();
            AddEnemyKilled(); // Special handling to add the last boss to the killed list.

            //base.BossCleanup();
        }

        private void AddEnemyKilled()
        {
            Game.PlayerStats.NumEnemiesBeaten++;

            // Incrementing the number of times you've killed a specific type of enemy.
            Vector4 enemyData = Game.PlayerStats.EnemiesKilledList[(int)m_boss.Type];
            enemyData.X += 1;
            enemyData.Y += 1;
            Game.PlayerStats.EnemiesKilledList[(int)m_boss.Type] = enemyData;
        }

        private void DropGold()
        {
            int totalGold = m_bossCoins + m_bossMoneyBags + m_bossDiamonds;
            List<int> goldArray = new List<int>();

            for (int i = 0; i < m_bossCoins; i++)
                goldArray.Add(0);
            for (int i = 0; i < m_bossMoneyBags; i++)
                goldArray.Add(1);
            for (int i = 0; i < m_bossDiamonds; i++)
                goldArray.Add(2);

            CDGMath.Shuffle<int>(goldArray);
            float coinDelay = 0;// 2.5f / goldArray.Count; // The enemy dies for 2.5 seconds.
            SoundManager.PlaySound("Boss_Flash");

            for (int i = 0; i < goldArray.Count; i++)
            {
                Vector2 goldPos = m_boss.Position;
                if (goldArray[i] == 0)
                    Tween.RunFunction(i * coinDelay, Player.AttachedLevel.ItemDropManager, "DropItemWide", goldPos, ItemDropType.Coin, ItemDropType.CoinAmount);
                else if (goldArray[i] == 1)
                    Tween.RunFunction(i * coinDelay, Player.AttachedLevel.ItemDropManager, "DropItemWide", goldPos, ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);
                else
                    Tween.RunFunction(i * coinDelay, Player.AttachedLevel.ItemDropManager, "DropItemWide", goldPos, ItemDropType.Diamond, ItemDropType.DiamondAmount);
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new LastBossRoom();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_fountain = null;
                m_boss = null;
                base.Dispose();
            }
        }

        public override bool BossKilled
        {
            get { return m_boss.IsKilled == true && m_boss.IsSecondForm == true; }
        }

    }
}
