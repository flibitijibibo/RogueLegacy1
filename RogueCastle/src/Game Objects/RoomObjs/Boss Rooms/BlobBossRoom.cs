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
    public class BlobBossRoom : BossRoomObj
    {
        private EnemyObj_Blob m_boss1;

        private List<ObjContainer> m_blobArray;
        private float m_desiredBossScale;
        private int m_numIntroBlobs = 10;

        public override void Initialize()
        {
            m_boss1 = EnemyList[0] as EnemyObj_Blob;
            //m_boss2 = EnemyList[1] as EnemyObj_Blob;

            m_boss1.PauseEnemy(true);
            m_boss1.DisableAllWeight = false;
            //m_boss2.PauseEnemy(true);

            m_desiredBossScale = m_boss1.Scale.X;

            m_blobArray = new List<ObjContainer>();
            for (int i = 0; i < m_numIntroBlobs; i++)
            {
                ObjContainer blob = new ObjContainer("EnemyBlobBossAir_Character");
                blob.Position = m_boss1.Position;
                blob.Scale = new Vector2(0.4f, 0.4f);

                blob.GetChildAt(0).TextureColor = Color.White;
                blob.GetChildAt(2).TextureColor = Color.LightSkyBlue;
                blob.GetChildAt(2).Opacity = 0.8f;
                (blob.GetChildAt(1) as SpriteObj).OutlineColour = Color.Black;
                blob.Y -= 1000;
                m_blobArray.Add(blob);
                GameObjList.Add(blob);
            }

            base.Initialize();
        }

        public override void OnEnter()
        {
            m_boss1.Name = "Herodotus";
            m_boss1.LocStringID = "LOC_ID_ENEMY_NAME_11";
            //m_boss1.Name = "Herod";
            //m_boss2.Name = "Otus";

            m_boss1.GetChildAt(0).TextureColor = Color.White;
            m_boss1.GetChildAt(2).TextureColor = Color.LightSkyBlue;
            m_boss1.GetChildAt(2).Opacity = 0.8f;
            (m_boss1.GetChildAt(1) as SpriteObj).OutlineColour = Color.Black;
            m_boss1.GetChildAt(1).TextureColor = Color.Black;

            //m_boss2.GetChildAt(0).TextureColor = Color.Black;
            //m_boss2.GetChildAt(2).TextureColor = Color.IndianRed;
            //m_boss2.GetChildAt(2).Opacity = 0.8f;
            //(m_boss2.GetChildAt(1) as SpriteObj).OutlineColour = Color.White;
            //m_boss2.GetChildAt(1).TextureColor = Color.White;
            SoundManager.StopMusic(0.5f);

            Player.LockControls();
            Player.AttachedLevel.UpdateCamera();
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Tween.To(Player.AttachedLevel.Camera, 1f, Quad.EaseInOut, "X", (this.Bounds.Left + 700).ToString(), "Y", m_boss1.Y.ToString());

            Tween.By(m_blobArray[0], 1, Quad.EaseIn, "delay", "0.5", "Y", "1150");
            Tween.AddEndHandlerToLastTween(this, "GrowBlob", m_blobArray[0]);
            Tween.By(m_blobArray[1], 1, Quad.EaseIn, "delay", "1.5", "Y", "1150");
            Tween.AddEndHandlerToLastTween(this, "GrowBlob", m_blobArray[1]);

            Tween.RunFunction(1, this, "DropBlobs");

            m_boss1.Scale = new Vector2(0.5f, 0.5f);

            Player.AttachedLevel.RunCinematicBorders(9);

            base.OnEnter();
        }

        public void DropBlobs()
        {
            float delay = 1f;
            for (int i = 2; i < m_blobArray.Count; i++)
            {
                Tween.By(m_blobArray[i], 1, Quad.EaseIn, "delay", delay.ToString(), "Y", "1150");
                Tween.AddEndHandlerToLastTween(this, "GrowBlob", m_blobArray[i]);
                delay += 0.5f * (m_blobArray.Count - i) / m_blobArray.Count;
            }
            Tween.RunFunction(delay + 1, m_boss1, "PlayAnimation", true);
            Tween.RunFunction(delay + 1, typeof(SoundManager), "PlaySound", "Boss_Blob_Idle_Loop");
            //Tween.RunFunction(delay + 1, this, "DisplayBossTitle", "The Infinite", m_boss1.Name + " & " + m_boss2.Name, "Intro2");
            Tween.RunFunction(delay + 1, this, "DisplayBossTitle", "LOC_ID_ENEMY_NAME_119", m_boss1.LocStringID, "Intro2");
            Tween.RunFunction(delay + 1, typeof(SoundManager), "PlaySound", "Boss_Blob_Spawn");
        }

        public void GrowBlob(GameObj blob)
        {
            float desiredBossScale = (m_desiredBossScale - 0.5f)/ m_numIntroBlobs; // Subtract 0.5f because the blob starts at 0.5.

            blob.Visible = false;
            m_boss1.PlayAnimation(false);
            m_boss1.ScaleX += desiredBossScale;
            m_boss1.ScaleY += desiredBossScale;

            SoundManager.PlaySound("Boss_Blob_Spawn_01", "Boss_Blob_Spawn_02", "Boss_Blob_Spawn_03");
        }

        public void Intro2()
        {
            m_boss1.PlayAnimation(true);
            Tween.To(Player.AttachedLevel.Camera, 0.5f, Quad.EaseInOut, "delay", "0.5", "X", (Player.X + GlobalEV.Camera_XOffset).ToString(), "Y", (this.Bounds.Bottom - (Player.AttachedLevel.Camera.Bounds.Bottom - Player.AttachedLevel.Camera.Y)).ToString());
            Tween.AddEndHandlerToLastTween(this, "BeginBattle");
        }

        public void BeginBattle()
        {
            SoundManager.PlayMusic("DungeonBoss", true, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
            m_boss1.UnpauseEnemy(true);
            //m_boss2.UnpauseEnemy(true);
            m_boss1.PlayAnimation(true);
            //m_boss2.PlayAnimation(true);
            //m_boss1.Scale = m_boss2.Scale; //TEDDY - Added to make them be the correct scale.
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle bounds = this.Bounds;

            foreach (EnemyObj enemy in EnemyList)
            {
                if (enemy.Type == EnemyType.Blob && enemy.IsKilled == false)
                {
                    if ((enemy.X > this.Bounds.Right - 20) || (enemy.X < this.Bounds.Left + 20) ||
                        (enemy.Y > this.Bounds.Bottom - 20) || (enemy.Y < this.Bounds.Top + 20))
                        enemy.Position = new Vector2(bounds.Center.X, bounds.Center.Y);
                }
            }

            foreach (EnemyObj enemy in TempEnemyList)
            {
                if (enemy.Type == EnemyType.Blob && enemy.IsKilled == false)
                {
                    if ((enemy.X > this.Bounds.Right - 20) || (enemy.X < this.Bounds.Left + 20) ||
                        (enemy.Y > this.Bounds.Bottom - 20) || (enemy.Y < this.Bounds.Top + 20))
                        enemy.Position = new Vector2(bounds.Center.X, bounds.Center.Y);
                }
            }
            base.Update(gameTime);
        }

        public override bool BossKilled
        {
            get { return ActiveEnemies == 0; }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_blobArray.Clear(); // Do not dispose the blobs in this array since they are added to the room and will be disposed when the room is disposed.
                m_blobArray = null;

                m_boss1 = null;
                base.Dispose();
            }
        }

        public int NumActiveBlobs
        {
            get
            {
                int numBlobs = 0;
                foreach (EnemyObj enemy in EnemyList)
                {
                    if (enemy.Type == EnemyType.Blob && enemy.IsKilled == false)
                        numBlobs++;
                }

                foreach (EnemyObj enemy in TempEnemyList)
                {
                    if (enemy.Type == EnemyType.Blob && enemy.IsKilled == false)
                        numBlobs++;
                }
                return numBlobs;
            }
        }
    }
}
