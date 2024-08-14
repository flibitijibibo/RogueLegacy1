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
    public class EyeballBossRoom : BossRoomObj
    {
        private EnemyObj_Eyeball m_boss;

        public EyeballBossRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_Eyeball;
            base.Initialize();
        }

        public override void OnEnter()
        {
            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.ChangeSprite("EnemyEyeballBossFire_Character");
            m_boss.ChangeToBossPupil();
            Vector2 storedScale = m_boss.Scale;
            m_boss.Scale = new Vector2(0.3f, 0.3f);
            Player.AttachedLevel.Camera.X = (int)(this.Bounds.Left + Player.AttachedLevel.Camera.Width * 0.5f);
            Player.AttachedLevel.Camera.Y = Player.Y;
            Vector2 storedCameraPos = Player.AttachedLevel.Camera.Position;
            m_boss.AnimationDelay = 1 / 10f;

            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(8f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = Player.Y;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString());
            //Tween.AddEndHandlerToLastTween(m_boss, "ChangeSprite", "EnemyEyeballBossFire_Character");
            Tween.RunFunction(1.1f, m_boss, "PlayAnimation", true);
            Tween.To(m_boss, 0.5f, Linear.EaseNone, "delay", "2.5", "AnimationDelay", (1 / 60f).ToString());
            Tween.To(m_boss, 3f, Quad.EaseInOut, "delay", "1", "Rotation", "1800");
            Tween.AddEndHandlerToLastTween(m_boss, "ChangeSprite", "EnemyEyeballBossEye_Character");
            Tween.To(m_boss, 2, Bounce.EaseOut, "delay", "2", "ScaleX", storedScale.X.ToString(), "ScaleY", storedScale.Y.ToString());

            Tween.RunFunction(3.2f, this, "DisplayBossTitle", "LOC_ID_ENEMY_NAME_118", m_boss.LocStringID, "Intro2"); //The Lookout
            Tween.RunFunction(0.8f, typeof(SoundManager), "PlaySound", "Boss_Eyeball_Build");

            base.OnEnter();
        }

        public void Intro2()
        {
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "Y", ((int)(this.Bounds.Bottom - Player.AttachedLevel.Camera.Height * 0.5f)).ToString());
            Tween.AddEndHandlerToLastTween(this, "EndCutscene");
        }

        public void EndCutscene()
        {
            m_boss.Rotation = 0;
            SoundManager.PlayMusic("CastleBossIntroSong", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
            m_cutsceneRunning = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_cutsceneRunning == false && m_boss.BossVersionKilled == false)
            {
                if (SoundManager.IsMusicPlaying == false)
                    SoundManager.PlayMusic("CastleBossSong", true, 0);
            }

            base.Update(gameTime);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new EyeballBossRoom();
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
                m_boss = null;
                base.Dispose();
            }
        }

        public override bool BossKilled
        {
            get { return m_boss.IsKilled == true; }
        }
    }
}
