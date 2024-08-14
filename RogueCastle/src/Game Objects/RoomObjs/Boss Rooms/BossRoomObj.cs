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
    public abstract class BossRoomObj : RoomObj
    {
        protected bool m_cutsceneRunning = false;
        private ChestObj m_bossChest;
        private float m_sparkleTimer = 0;
        private bool m_teleportingOut;
        private float m_roomFloor;

        private TextObj m_bossTitle1;
        private TextObj m_bossTitle2;
        private SpriteObj m_bossDivider;

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

            base.Initialize();
        }

        public override void OnEnter()
        {
            Game.ScreenManager.GetLevelScreen().JukeboxEnabled = false; // This code is to override the jukebox.
            m_bossChest.ChestType = ChestType.Boss;
            m_bossChest.Visible = false;
            m_bossChest.IsLocked = true;

            if (m_bossChest.PhysicsMngr == null)
                Player.PhysicsMngr.AddObject(m_bossChest);
            m_teleportingOut = false;

            m_bossTitle1.Opacity = 0;
            m_bossTitle2.Opacity = 0;
            m_bossDivider.ScaleX = 0;
            m_bossDivider.Opacity = 0;

            base.OnEnter();
        }

        public void DisplayBossTitle(string bossTitle1LocID, string bossTitle2LocID, string endHandler)
        {
            SoundManager.PlaySound("Boss_Title");
            // Setting title positions.
            m_bossTitle1.Text = LocaleBuilder.getString(bossTitle1LocID, m_bossTitle1);
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

        public virtual void BossCleanup()
        {
            Player.StopAllSpells();
            Game.PlayerStats.NewBossBeaten = true;
            if (this.LinkedRoom != null)
                Player.AttachedLevel.CloseBossDoor(this.LinkedRoom, this.LevelType);
        }

        public void TeleportPlayer()
        {
            Player.CurrentSpeed = 0;
            Vector2 storedPlayerPos = Player.Position;

            Vector2 storedScale = Player.Scale;
            Tween.To(Player, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "1.2", "ScaleX", "0");
            Player.ScaleX = 0;
            Tween.To(Player, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "7", "ScaleX", storedScale.X.ToString());
            Player.ScaleX = storedScale.X;

            LogicSet teleportLS = new LogicSet(Player);
            teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", true));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
            teleportLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character", true, false));
            teleportLS.AddAction(new DelayLogicAction(0.5f));
            teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Disappear"));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleport", new Vector2(Player.X, Player.Bounds.Bottom), Player.Scale));
            teleportLS.AddAction(new DelayLogicAction(0.3f));
            teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "StartWipeTransition"));
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            if (this.LinkedRoom != null)
            {
                // Door symbol being revealed.
                Player.Position = new Vector2(Player.AttachedLevel.RoomList[1].Bounds.Center.X, Player.AttachedLevel.RoomList[1].Bounds.Center.Y);
                Player.UpdateCollisionBoxes();
                teleportLS.AddAction(new TeleportLogicAction(null, Player.Position));
                teleportLS.AddAction(new DelayLogicAction(0.05f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "EndWipeTransition"));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.RoomList[1], "RevealSymbol", this.LevelType, true));
                teleportLS.AddAction(new DelayLogicAction(3.5f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "StartWipeTransition"));
                teleportLS.AddAction(new DelayLogicAction(0.2f));

                // Teleporting player back to room entrance.
                Player.Position = new Vector2(this.LinkedRoom.Bounds.Center.X, (this.LinkedRoom.Bounds.Bottom - 60 - (Player.Bounds.Bottom - Player.Y)));
                Player.UpdateCollisionBoxes();
                teleportLS.AddAction(new ChangePropertyLogicAction(Player.AttachedLevel, "DisableSongUpdating", false));
                teleportLS.AddAction(new TeleportLogicAction(null, Player.Position));
                teleportLS.AddAction(new DelayLogicAction(0.05f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ScreenManager, "EndWipeTransition"));
                teleportLS.AddAction(new DelayLogicAction(1f));
                teleportLS.AddAction(new RunFunctionLogicAction(Player.AttachedLevel.ImpactEffectPool, "MegaTeleportReverse", new Vector2(Player.X, this.LinkedRoom.Bounds.Bottom - 60), storedScale));
                teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Reappear"));
            }
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new ChangePropertyLogicAction(Player, "ForceInvincible", false));
            teleportLS.AddAction(new RunFunctionLogicAction(Player, "UnlockControls"));
            Player.RunExternalLogicSet(teleportLS);
            Player.Position = storedPlayerPos;
            Player.UpdateCollisionBoxes();
        }

        public void UnlockChest()
        {
            m_bossChest.IsLocked = false;
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);

            m_bossDivider.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            m_bossTitle1.Draw(camera);
            m_bossTitle2.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_bossChest = null;
                m_bossDivider.Dispose();
                m_bossDivider = null;
                m_bossTitle1.Dispose();
                m_bossTitle1 = null;
                m_bossTitle2.Dispose();
                m_bossTitle2 = null;

                base.Dispose();
            }
        }

        public abstract bool BossKilled { get; }
    }
}
