using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public abstract class ChallengeBossRoomObj : RoomObj
    {
        protected const float EMPOWER_HP_AMT = 0.20f;//0.15f; //0.125f;//EMPOWER_HP_AMT = 0.1f;
        protected const float EMPOWER_PWR_AMT = 0.20f;//0.15f;//0.125f;//EMPOWER_PWR_AMT = 0.075f;

        protected bool m_cutsceneRunning = false;
        protected ChestObj m_bossChest;
        private float m_sparkleTimer = 0;
        private bool m_teleportingOut;
        private float m_roomFloor;

        private TextObj m_bossTitle1;
        private TextObj m_bossTitle2;
        private SpriteObj m_bossDivider;

        protected PlayerStats m_storedPlayerStats;
        private int m_storedHealth;
        private float m_storedMana;
        private Vector2 m_storedScale;

        private List<RaindropObj> m_rainFG;

        public ChallengeBossRoomObj()
        {
            m_storedPlayerStats = new PlayerStats();
        }

        public override void Initialize()
        {
            m_bossTitle1 = new TextObj(Game.JunicodeFont);
            m_bossTitle1.Text = LocaleBuilder.getString("LOC_ID_ENEMY_NAME_121", m_bossTitle1); // "The Forsaken", is this used or just overidden by DisplayBossTitle parameters
            m_bossTitle1.OutlineWidth = 2;
            m_bossTitle1.FontSize = 18;

            m_bossTitle2 = new TextObj(Game.JunicodeLargeFont);
            m_bossTitle2.Text = LocaleBuilder.getString("LOC_ID_ENEMY_NAME_34", m_bossTitle2); // "Alexander", is this used or just overidden by DisplayBossTitle parameters
            m_bossTitle2.OutlineWidth = 2;
            m_bossTitle2.FontSize = 40;

            m_bossDivider = new SpriteObj("Blank_Sprite");
            m_bossDivider.OutlineWidth = 2;

            foreach (DoorObj door in DoorList)
            {
                m_roomFloor = door.Bounds.Bottom;
            }

            m_bossChest = new ChestObj(null);
            m_bossChest.Position = new Vector2(this.Bounds.Center.X - m_bossChest.Width/2f, this.Bounds.Center.Y);
            this.GameObjList.Add(m_bossChest);

            m_rainFG = new List<RaindropObj>();
            for (int i = 0; i < 50; i++)
            {
                RaindropObj rain = new RaindropObj(new Vector2(CDGMath.RandomFloat(this.X - this.Width, this.X), CDGMath.RandomFloat(this.Y, this.Y + this.Height)));
                m_rainFG.Add(rain);
                rain.ChangeToParticle();
            }

            base.Initialize();
        }

        public void StorePlayerData()
        {
            m_storedPlayerStats = Game.PlayerStats;
            Game.PlayerStats = new PlayerStats();
            Game.PlayerStats.TutorialComplete = true; // Necessary to disable nostalgia.
            m_storedScale = Player.Scale;
            Player.Scale = new Vector2(2, 2);
            SkillSystem.ResetAllTraits();
            Player.OverrideInternalScale(Player.Scale);

            m_storedHealth = Player.CurrentHealth;
            m_storedMana = Player.CurrentMana;
            //m_storedPlayerStats.Spell = Game.PlayerStats.Spell;
            //m_storedPlayerStats.Class = Game.PlayerStats.Class;
            //m_storedPlayerStats.Traits = Game.PlayerStats.Traits;

            //m_storedPlayerStats.HeadPiece = Game.PlayerStats.HeadPiece;
            //m_storedPlayerStats.ShoulderPiece = Game.PlayerStats.ShoulderPiece;
            //m_storedPlayerStats.ChestPiece = Game.PlayerStats.ChestPiece;

            //m_storedPlayerStats.BonusHealth = Game.PlayerStats.BonusHealth;
            //m_storedPlayerStats.BonusStrength = Game.PlayerStats.BonusStrength;
            //m_storedPlayerStats.BonusMana = Game.PlayerStats.BonusMana;
            //m_storedPlayerStats.BonusDefense = Game.PlayerStats.BonusDefense;
            //m_storedPlayerStats.BonusWeight = Game.PlayerStats.BonusWeight;
            //m_storedPlayerStats.BonusMagic = Game.PlayerStats.BonusMagic;

            //m_storedPlayerStats.LichHealth = Game.PlayerStats.LichHealth;
            //m_storedPlayerStats.LichMana = Game.PlayerStats.LichMana;
            //m_storedPlayerStats.LichHealthMod = Game.PlayerStats.LichHealthMod;

            //m_storedPlayerStats.WizardSpellList = Game.PlayerStats.WizardSpellList;

            //int arraySize = m_storedPlayerStats.GetEquippedArray.Length;
            //for (int i = 0; i < arraySize; i++)
            //    m_storedPlayerStats.GetEquippedArray[i] = Game.PlayerStats.GetEquippedArray[i];

            //arraySize = m_storedPlayerStats.GetEquippedRuneArray.Length;
            //for (int i = 0; i < arraySize; i++)
            //    m_storedPlayerStats.GetEquippedRuneArray[i] = Game.PlayerStats.GetEquippedRuneArray[i];
        }

        public void LoadPlayerData()
        {
            Game.PlayerStats = m_storedPlayerStats;

            //Game.PlayerStats.Spell = m_storedPlayerStats.Spell;
            //Game.PlayerStats.Class = m_storedPlayerStats.Class;
            //Game.PlayerStats.Traits = m_storedPlayerStats.Traits;

            //Game.PlayerStats.HeadPiece = m_storedPlayerStats.HeadPiece;
            //Game.PlayerStats.ShoulderPiece = m_storedPlayerStats.ShoulderPiece;
            //Game.PlayerStats.ChestPiece = m_storedPlayerStats.ChestPiece;

            //Game.PlayerStats.BonusHealth = m_storedPlayerStats.BonusHealth;
            //Game.PlayerStats.BonusStrength = m_storedPlayerStats.BonusStrength;
            //Game.PlayerStats.BonusMana = m_storedPlayerStats.BonusMana;
            //Game.PlayerStats.BonusDefense = m_storedPlayerStats.BonusDefense;
            //Game.PlayerStats.BonusWeight = m_storedPlayerStats.BonusWeight;
            //Game.PlayerStats.BonusMagic = m_storedPlayerStats.BonusMagic;

            //Game.PlayerStats.LichHealth = m_storedPlayerStats.LichHealth;
            //Game.PlayerStats.LichMana = m_storedPlayerStats.LichMana;
            //Game.PlayerStats.LichHealthMod = m_storedPlayerStats.LichHealthMod;

            //Game.PlayerStats.WizardSpellList = m_storedPlayerStats.WizardSpellList;

            //int arraySize = m_storedPlayerStats.GetEquippedArray.Length;
            //for (int i = 0; i < arraySize; i++)
            //    Game.PlayerStats.GetEquippedArray[i] = m_storedPlayerStats.GetEquippedRuneArray[i];

            //arraySize = m_storedPlayerStats.GetEquippedRuneArray.Length;
            //for (int i = 0; i < arraySize; i++)
            //    Game.PlayerStats.GetEquippedRuneArray[i] = m_storedPlayerStats.GetEquippedRuneArray[i];
        }

        public override void OnEnter()
        {
            // These are needed to update the player to match changes to him and the boss.
            Player.CurrentHealth = Player.MaxHealth;
            Player.CurrentMana = Player.MaxMana;
            Player.ChangeSprite("PlayerIdle_Character");
            Player.AttachedLevel.UpdatePlayerHUD();
            Player.AttachedLevel.UpdatePlayerHUDAbilities();
            Player.AttachedLevel.UpdatePlayerHUDSpecialItem();
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.UpdateEquipmentColours();
            Player.AttachedLevel.RefreshPlayerHUDPos();

            Game.ScreenManager.GetLevelScreen().JukeboxEnabled = false; // This code is to override the jukebox.
            m_bossChest.ChestType = ChestType.Boss;
            m_bossChest.Visible = false;
            m_bossChest.IsLocked = true;
            m_bossChest.X = Player.X;

            if (m_bossChest.PhysicsMngr == null)
                Player.PhysicsMngr.AddObject(m_bossChest);
            m_teleportingOut = false;

            m_bossTitle1.Opacity = 0;
            m_bossTitle2.Opacity = 0;
            m_bossDivider.ScaleX = 0;
            m_bossDivider.Opacity = 0;

            if (LevelEV.WEAKEN_BOSSES == true)
            {
                foreach (EnemyObj enemy in this.EnemyList)
                    enemy.CurrentHealth = 1;
            }
            base.OnEnter();
        }

        public override void OnExit()
        {
            // Resets the player HUD level.
            Player.AttachedLevel.ForcePlayerHUDLevel(-1);

            LoadPlayerData();
            (Game.ScreenManager.Game as RogueCastle.Game).SaveManager.LoadFiles(Player.AttachedLevel, SaveType.UpgradeData);
            Player.AttachedLevel.UpdatePlayerHUD();
            Player.AttachedLevel.UpdatePlayerHUDAbilities();
            Player.AttachedLevel.UpdatePlayerHUDSpecialItem();
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.UpdateEquipmentColours();
            Player.AttachedLevel.RefreshPlayerHUDPos();

            // This must be called after upgrade data is loaded.
            Player.CurrentHealth = m_storedHealth;
            Player.CurrentMana = m_storedMana;

            if (BossKilled == true)
                SaveCompletionData(); // Must be called here to make sure things that need to be saved are saved.

            Game.PlayerStats.NewBossBeaten = true;
            if (this.LinkedRoom != null)
                Player.AttachedLevel.CloseBossDoor(this.LinkedRoom, this.LevelType);

            (Game.ScreenManager.Game as RogueCastle.Game).SaveManager.SaveFiles(SaveType.PlayerData);
            base.OnExit();
        }

        // TODO: localize bossTitle1 (player's name)
        public void DisplayBossTitle(string bossTitle1, string bossTitle2LocID, string endHandler)
        {
            SoundManager.PlaySound("Boss_Title");
            // Setting title positions.
            m_bossTitle1.Text = bossTitle1; // LocaleBuilder.getString(bossTitle1LocID, m_bossTitle1);
            m_bossTitle2.Text = LocaleBuilder.getString(bossTitle2LocID, m_bossTitle2);

            Camera2D camera = Player.AttachedLevel.Camera;
            if (Player.AttachedLevel.CurrentRoom is LastBossRoom)
                m_bossTitle1.Position = new Vector2(camera.X - 550, camera.Y + 100);
            else
                m_bossTitle1.Position = new Vector2(camera.X - 550, camera.Y + 50);
            m_bossTitle2.X = m_bossTitle1.X - 0;
            m_bossTitle2.Y = m_bossTitle1.Y + 50;

            m_bossDivider.Position = m_bossTitle1.Position;
            m_bossDivider.Y += m_bossTitle1.Height - 5;

            // Reposition the titles.
            m_bossTitle1.X -= 1000;
            m_bossTitle2.X += 1500;

            // Tweening the titles in.
            Tween.To(m_bossDivider, 0.5f, Tween.EaseNone, "delay", "0.3", "Opacity", "1");
            Tween.To(m_bossDivider, 1, Quad.EaseInOut, "delay", "0", "ScaleX", ((float)(m_bossTitle2.Width / 5)).ToString());

            Tween.To(m_bossTitle1, 0.5f, Tween.EaseNone, "delay", "0.3", "Opacity", "1");
            Tween.To(m_bossTitle2, 0.5f, Tween.EaseNone, "delay", "0.3", "Opacity", "1");
            Tween.By(m_bossTitle1, 1, Quad.EaseOut, "X", "1000");
            Tween.By(m_bossTitle2, 1, Quad.EaseOut, "X", "-1500");

            // Move titles slightly after tweening.
            m_bossTitle1.X += 1000;
            m_bossTitle2.X -= 1500;
            Tween.By(m_bossTitle1, 2f, Tween.EaseNone, "delay", "1", "X", "20");
            Tween.By(m_bossTitle2, 2f, Tween.EaseNone, "delay", "1", "X", "-20");
            m_bossTitle1.X -= 1000;
            m_bossTitle2.X += 1500;

            // Add the end event handler here so that it doesn't start too late.
            Tween.AddEndHandlerToLastTween(this, endHandler);

            Tween.RunFunction(3, typeof(SoundManager), "PlaySound", "Boss_Title_Exit");
            //Tweening the titles out.
            m_bossTitle1.X += 1020;
            m_bossTitle2.X -= 1520;
            m_bossTitle1.Opacity = 1;
            m_bossTitle2.Opacity = 1;
            Tween.To(m_bossTitle1, 0.5f, Tween.EaseNone, "delay", "3", "Opacity", "0");
            Tween.To(m_bossTitle2, 0.5f, Tween.EaseNone, "delay", "3", "Opacity", "0");
            Tween.By(m_bossTitle1, 0.6f, Quad.EaseIn, "delay", "3", "X", "1500");
            Tween.By(m_bossTitle2, 0.6f, Quad.EaseIn, "delay", "3", "X", "-1000");
            m_bossTitle1.Opacity = 0;
            m_bossTitle2.Opacity = 0;

            m_bossDivider.Opacity = 1;
            Tween.To(m_bossDivider, 0.5f, Tween.EaseNone, "delay", "2.8", "Opacity", "0");
            m_bossDivider.Opacity = 0;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (RaindropObj raindrop in m_rainFG)
                raindrop.UpdateNoCollision(gameTime);

            if (m_cutsceneRunning == false)
                base.Update(gameTime);

            if (BossKilled == true && m_bossChest.Visible == false)
            {
                BossCleanup();
                m_bossChest.Visible = true;
                m_bossChest.Opacity = 0;
                SoundManager.PlayMusic("TitleScreenSong", true, 1); // Stinger goes here.
                Tween.To(m_bossChest, 4, Tween.EaseNone, "Opacity", "1");
                Tween.To(m_bossChest, 4, Quad.EaseOut, "Y", m_roomFloor.ToString());
                Tween.AddEndHandlerToLastTween(this, "UnlockChest");
                m_sparkleTimer = 0.5f;
            }

            if (m_bossChest.Visible == true && m_bossChest.IsOpen == false && BossKilled == true)
            {
                if (m_sparkleTimer > 0)
                {
                    m_sparkleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_sparkleTimer <= 0)
                    {
                        m_sparkleTimer = 0.5f;
                        Tween.RunFunction(0, Player.AttachedLevel.ImpactEffectPool, "DisplayChestSparkleEffect", new Vector2(m_bossChest.X, m_bossChest.Y - m_bossChest.Height / 2));
                    }
                }
            }
            else if (m_bossChest.Visible == true && m_bossChest.IsOpen == true && BossKilled == true && m_teleportingOut == false)
            {
                m_teleportingOut = true;
                if (LevelEV.RUN_DEMO_VERSION == true)
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.DemoEnd, true);
                else
                    TeleportPlayer();
            }
        }

        protected abstract void SaveCompletionData();

        public virtual void BossCleanup()
        {
            Player.StopAllSpells();
        }

        public void TeleportPlayer()
        {
            Player.CurrentSpeed = 0;
            Vector2 storedPlayerPos = Player.Position;
            Vector2 storedPlayerScale = Player.Scale;

            //Player.Scale = m_storedSize;
            //Vector2 storedScale = Player.Scale;
            //Tween.To(Player, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "1.3", "ScaleX", "0");
            //Player.ScaleX = 0;
            //Tween.To(Player, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "3.2", "ScaleX", storedScale.X.ToString());
            //Player.ScaleX = storedScale.X;

            LogicSet teleportLS = new LogicSet(Player);
            teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", true));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
            teleportLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character", true, false));
            teleportLS.AddAction(new DelayLogicAction(0.5f));
            teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Disappear"));
            teleportLS.AddAction(new RunFunctionLogicAction(this, "TeleportScaleOut"));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleport", new Vector2(Player.X, Player.Bounds.Bottom), Player.Scale));
            teleportLS.AddAction(new DelayLogicAction(0.3f));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "StartWipeTransition"));
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new RunFunctionLogicAction(this, "LoadPlayerData")); // Necessary to make sure traits that affects rooms is properly loaded when leaving.            
            if (this.LinkedRoom != null)
            {
                // Teleporting player back to room entrance.
                Player.Scale = m_storedScale;
                Player.OverrideInternalScale(m_storedScale);
                Player.UpdateCollisionBoxes();
                Player.Position = new Vector2(this.LinkedRoom.Bounds.Center.X, (this.LinkedRoom.Bounds.Bottom - 60 - (Player.TerrainBounds.Bottom - Player.Y)));
                teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", false));
                teleportLS.AddAction(new ChangePropertyLogicAction(Player, "ScaleY", m_storedScale.Y));
                teleportLS.AddAction(new TeleportLogicAction(null, Player.Position));
                teleportLS.AddAction(new DelayLogicAction(0.05f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "EndWipeTransition"));
                teleportLS.AddAction(new DelayLogicAction(1f));
                teleportLS.AddAction(new RunFunctionLogicAction(this, "TeleportScaleIn"));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleportReverse", new Vector2(Player.X, this.LinkedRoom.Bounds.Bottom - 60), m_storedScale));
                teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Reappear"));
            }
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new ChangePropertyLogicAction(Player, "ForceInvincible", false));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "UnlockControls"));
            Player.RunExternalLogicSet(teleportLS);
            Player.Position = storedPlayerPos;
            Player.Scale = storedPlayerScale;
            Player.UpdateCollisionBoxes();
        }

        public void TeleportScaleOut()
        {
            Tween.To(Player, 0.05f, Tween.EaseNone, "ScaleX", "0");
        }

        public void TeleportScaleIn()
        {
            Tween.To(Player, 0.05f, Tween.EaseNone, "delay", "0.15", "ScaleX", m_storedScale.X.ToString());
        }

        public void KickPlayerOut()
        {
            Player.AttachedLevel.PauseScreen();
            SoundManager.StopMusic(0);
            Player.LockControls();
            Player.AttachedLevel.RunWhiteSlashEffect();
            SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Flash");
            Player.IsWeighted = false;
            Player.IsCollidable = false;

            Player.CurrentSpeed = 0;
            Player.AccelerationX = 0;
            Player.AccelerationY = 0;

            if (this is BlobChallengeRoom)
            {
                Tween.To(Player.AttachedLevel.Camera, 0.5f, Tweener.Ease.Quad.EaseInOut, "Zoom", "1", "X", Player.X.ToString(), "Y", Player.Y.ToString());
                Tween.AddEndHandlerToLastTween(this, "LockCamera");
            }

            Tween.RunFunction(1, this, "KickPlayerOut2");
        }

        public void KickPlayerOut2()
        {
            Player.AttachedLevel.UnpauseScreen();

            Player.CurrentSpeed = 0;
            Vector2 storedPlayerPos = Player.Position;
            Vector2 storedPlayerScale = Player.Scale;

            LogicSet teleportLS = new LogicSet(Player);
            teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", true));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
            teleportLS.AddAction(new DelayLogicAction(1.3f));
            teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Disappear"));
            teleportLS.AddAction(new RunFunctionLogicAction(this, "TeleportScaleOut"));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleport", new Vector2(Player.X, Player.Bounds.Bottom), Player.Scale));
            teleportLS.AddAction(new DelayLogicAction(0.3f));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "StartWipeTransition"));
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new RunFunctionLogicAction(this, "LoadPlayerData")); // Necessary to make sure traits that affects rooms is properly loaded when leaving.
            if (this.LinkedRoom != null)
            {
                // Teleporting player back to room entrance.
                Player.Scale = m_storedScale;
                Player.OverrideInternalScale(m_storedScale);
                Player.UpdateCollisionBoxes();
                Player.Position = new Vector2(this.LinkedRoom.Bounds.Center.X, (this.LinkedRoom.Bounds.Bottom - 60 - (Player.TerrainBounds.Bottom - Player.Y)));
                teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", false));
                teleportLS.AddAction(new ChangePropertyLogicAction(Player, "ScaleY", m_storedScale.Y));
                teleportLS.AddAction(new TeleportLogicAction(null, Player.Position));
                teleportLS.AddAction(new DelayLogicAction(0.05f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "EndWipeTransition"));
                teleportLS.AddAction(new DelayLogicAction(1f));
                teleportLS.AddAction(new RunFunctionLogicAction(this, "TeleportScaleIn"));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleportReverse", new Vector2(Player.X, this.LinkedRoom.Bounds.Bottom - 60), m_storedScale));
                teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Reappear"));
            }
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new ChangePropertyLogicAction(Player, "IsWeighted", true));
            teleportLS.AddAction(new ChangePropertyLogicAction(Player, "IsCollidable", true));
            teleportLS.AddAction(new ChangePropertyLogicAction(Player, "ForceInvincible", false));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "UnlockControls"));
            Player.RunExternalLogicSet(teleportLS);
            Player.Position = storedPlayerPos;
            Player.Scale = storedPlayerScale;
            Player.UpdateCollisionBoxes();
        }

        public void LockCamera()
        {
            Player.AttachedLevel.CameraLockedToPlayer = true;
        }

        public void UnlockChest()
        {
            m_bossChest.IsLocked = false;
        }

        public override void Draw(Camera2D camera)
        {
            foreach (RaindropObj raindrop in m_rainFG)
                raindrop.Draw(camera);

            base.Draw(camera);

            m_bossDivider.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            m_bossTitle1.Draw(camera);
            m_bossTitle2.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        //public override void PauseRoom()
        //{
        //    foreach (RaindropObj rainDrop in m_rainFG)
        //        rainDrop.PauseAnimation();
        //}


        //public override void UnpauseRoom()
        //{
        //    foreach (RaindropObj rainDrop in m_rainFG)
        //        rainDrop.ResumeAnimation();
        //}

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done NOT
                m_bossChest = null;
                m_bossDivider.Dispose();
                m_bossDivider = null;
                m_bossTitle1.Dispose();
                m_bossTitle1 = null;
                m_bossTitle2.Dispose();
                m_bossTitle2 = null;

                foreach (RaindropObj raindrop in m_rainFG)
                    raindrop.Dispose();
                m_rainFG.Clear();
                m_rainFG = null;

                base.Dispose();
            }
        }

        public abstract bool BossKilled { get; }

        public int StoredHP
        {
            get { return m_storedHealth; }
        }

        public float StoredMP
        {
            get { return m_storedMana; }
        }

        public override void RefreshTextObjs()
        {
            // Can't really update boss title positioning here because could be in the middle of tween
            base.RefreshTextObjs();
        }
    }
}
