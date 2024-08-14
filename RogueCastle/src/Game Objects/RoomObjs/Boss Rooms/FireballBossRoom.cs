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
    public class FireballBossRoom : BossRoomObj
    {
        private EnemyObj_Fireball m_boss;
        private List<SpriteObj> m_fireList;
        private float m_bossStartingScale;

        public override void Initialize()
        {
            foreach (EnemyObj enemy in EnemyList)
            {
                if (enemy is EnemyObj_Fireball)
                    m_boss = enemy as EnemyObj_Fireball;
                
                enemy.Visible = false;
                enemy.PauseEnemy(true);
            }

            m_boss.ChangeSprite("EnemyGhostBossIdle_Character");
            m_bossStartingScale = m_boss.ScaleX;

            m_fireList = new List<SpriteObj>();
            float angle = 0;
            float angleDiff = 360 / 15f;
            for (int i = 0; i < 15; i++)
            {
                SpriteObj fire = new SpriteObj("GhostBossProjectile_Sprite");
                fire.PlayAnimation(true);
                fire.OutlineWidth = 2;
                fire.Position = CDGMath.GetCirclePosition(angle, 300, m_boss.Position);
                fire.Scale = new Vector2(2, 2);
                angle += angleDiff;
                fire.Opacity = 0;
                m_fireList.Add(fire);
                GameObjList.Add(fire);
            }

            base.Initialize();
        }

        public override void OnEnter()
        {
            m_cutsceneRunning = true;
            SoundManager.StopMusic(0.5f);
            Player.LockControls();

            m_boss.Scale = Vector2.Zero;
            m_boss.Visible = true;
            m_boss.PlayAnimation(true);

            Player.AttachedLevel.UpdateCamera();
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Tween.To(Player.AttachedLevel.Camera, 1f, Quad.EaseInOut, "X", m_boss.X.ToString(), "Y", m_boss.Y.ToString());
            Tween.RunFunction(1.5f, this, "Intro2");

            Player.AttachedLevel.RunCinematicBorders(10);

            base.OnEnter();
        }


        public void Intro2()
        {
            float delay = 0;
            for (int i = 0; i < m_fireList.Count; i++)
            {
                Tween.RunFunction(delay, this, "DisplayOrb", i);
                delay += 0.1f;
            }

            Tween.RunFunction(delay + 0.5f, this, "Intro3");
        }

        public void DisplayOrb(int index)
        {
            Tween.To(m_fireList[index], 0.2f, Quad.EaseOut, "Opacity", "1");
            SoundManager.PlaySound("Boss_Fireball_Whoosh_01");//, "Boss_Fireball_Whoosh_02", "Boss_Fireball_Whoosh_03");
        }

        public void Intro3()
        {
            SoundManager.PlaySound("Boss_Fireball_Spawn");

            float delay = 0;
            for (int i = 0; i < m_fireList.Count; i++)
            {
                Tween.RunFunction(delay, this, "AbsorbOrb", i);
                delay += 0.1f;
            }

            //Tween.RunFunction(delay + 0.5f, this, "Intro4");
            Tween.RunFunction(delay + 0.5f, this, "DisplayBossTitle", "LOC_ID_ENEMY_NAME_122", m_boss.LocStringID, "Intro4");
        }

        public void AbsorbOrb(int index)
        {
            SoundManager.PlaySound("Boss_Fireball_Puff_01");//, "Boss_Fireball_Puff_02", "Boss_Fireball_Puff_03");
            Tween.To(m_fireList[index], 0.2f, Quad.EaseIn, "X", m_boss.X.ToString(), "Y", m_boss.Y.ToString());
            Tween.To(m_fireList[index], 0.1f, Tween.EaseNone, "delay", "0.1", "Opacity", "0");
            m_boss.ScaleX += m_bossStartingScale / 15f;
            m_boss.ScaleY += m_bossStartingScale / 15f;
        }

        public void Intro4()
        {
            m_boss.Visible = true;
            m_boss.PlayAnimation();
            Tween.To(Player.AttachedLevel.Camera, 0.5f, Quad.EaseInOut, "delay", "0.5", "X", (Player.X + GlobalEV.Camera_XOffset).ToString(), "Y", (this.Bounds.Bottom - (Player.AttachedLevel.Camera.Bounds.Bottom - Player.AttachedLevel.Camera.Y)).ToString());
            Tween.AddEndHandlerToLastTween(this, "BeginFight");
        }

        public void BeginFight()
        {
            SoundManager.PlayMusic("TowerBossIntroSong", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();

            foreach (EnemyObj enemy in EnemyList)
            {
                if (enemy is EnemyObj_BouncySpike)
                    Player.AttachedLevel.ImpactEffectPool.DisplaySpawnEffect(enemy.Position);
                enemy.UnpauseEnemy(true);
                enemy.Visible = true;
            }

            m_cutsceneRunning = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_boss.CurrentHealth <= 0 && this.ActiveEnemies > 1)
            {
                foreach (EnemyObj enemy in EnemyList)
                {
                    if (enemy is EnemyObj_BouncySpike)
                        enemy.Kill(false);
                }
            }

            if (m_cutsceneRunning == false)
            {
                if (SoundManager.IsMusicPlaying == false && m_boss.BossVersionKilled == false)
                    SoundManager.PlayMusic("TowerBossSong", true, 0);
            }
            base.Update(gameTime);
        }

        public override void BossCleanup()
        {
            foreach (EnemyObj enemy in EnemyList)
            {
                if (enemy is EnemyObj_BouncySpike)
                    enemy.Kill(false);
            }

            base.BossCleanup();
        }

        public override bool BossKilled
        {
            get { return m_boss.IsKilled == true; }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_boss = null;
                m_fireList.Clear();
                m_fireList = null;
                base.Dispose();
            }
        }
    }
}
