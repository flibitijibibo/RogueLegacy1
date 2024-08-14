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
    public class BlobChallengeRoom : ChallengeBossRoomObj
    {
        private EnemyObj_Blob m_boss;
        private EnemyObj_Blob m_boss2;
        //private EnemyObj_Blob m_boss3;
        private Vector2 m_startingCamPos;

        public BlobChallengeRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_Blob;
            m_boss.SaveToFile = false;

            
            m_boss2 = this.EnemyList[1] as EnemyObj_Blob;
            m_boss2.SaveToFile = false;
            /*
            m_boss3 = this.EnemyList[2] as EnemyObj_Blob;
            m_boss3.SaveToFile = false;            
            */
            base.Initialize();
        }

        private void SetRoomData()
        {
            // Set enemy and player data here.
            //Game.PlayerStats.Traits = new Vector2(TraitType.Vertigo, TraitType.None);
            //Game.PlayerStats.Class = ClassType.Wizard;
            m_boss.Name = "Astrodotus";
            m_boss.LocStringID = "LOC_ID_ENEMY_NAME_114";

            m_boss.GetChildAt(0).TextureColor = Color.Green;
            m_boss.GetChildAt(2).TextureColor = Color.LightGreen;
            m_boss.GetChildAt(2).Opacity = 0.8f;
            (m_boss.GetChildAt(1) as SpriteObj).OutlineColour = Color.Red;
            m_boss.GetChildAt(1).TextureColor = Color.Red;
            
            m_boss2.GetChildAt(0).TextureColor = Color.Red;
            m_boss2.GetChildAt(2).TextureColor = Color.LightPink;
            m_boss2.GetChildAt(2).Opacity = 0.8f;
            (m_boss2.GetChildAt(1) as SpriteObj).OutlineColour = Color.Black;
            m_boss2.GetChildAt(1).TextureColor = Color.DarkGray;
            /*
            m_boss3.GetChildAt(0).TextureColor = Color.Blue;
            m_boss3.GetChildAt(2).TextureColor = Color.LightBlue;
            m_boss3.GetChildAt(2).Opacity = 0.8f;
            (m_boss3.GetChildAt(1) as SpriteObj).OutlineColour = Color.Orange;
            m_boss3.GetChildAt(1).TextureColor = Color.Orange;
            */
            //Splits
            m_boss.Level = 100;
            m_boss.MaxHealth = 100;//15500;
            m_boss.Damage = 370;//190;
            m_boss.IsWeighted = false;
            m_boss.TurnSpeed = 0.015f;//0.01f;
            m_boss.Speed = 400;//200;//430;
            m_boss.IsNeo = true;
            m_boss.ChangeNeoStats(0.80f, 1.06f, 6);//(0.75f, 1.08f, 6);
            m_boss.Scale = new Vector2(2, 2);
            
            //Fast
            m_boss2.Level = m_boss.Level;
            m_boss2.MaxHealth = m_boss.MaxHealth;
            m_boss2.Damage = m_boss.Damage;
            m_boss2.IsWeighted = m_boss.IsWeighted;
            m_boss2.TurnSpeed = 0.01f;//m_boss.TurnSpeed;
            m_boss2.Speed = 625;//m_boss.Speed;
            m_boss2.IsNeo = m_boss.IsNeo;
            m_boss2.ChangeNeoStats(0.75f, 1.160f, 5);//(0.75f, 1.175f, 5);
            m_boss2.Scale = m_boss.Scale;
            /*
            //Slow and tanky
            m_boss3.Level = m_boss.Level;
            m_boss3.MaxHealth = 100;//m_boss.MaxHealth;
            m_boss3.Damage = m_boss.Damage;
            m_boss3.IsWeighted = m_boss.IsWeighted;
            m_boss3.TurnSpeed = 0.005f;//m_boss.TurnSpeed;
            m_boss3.Speed = 750f;//m_boss.Speed;
            m_boss3.IsNeo = m_boss.IsNeo;
            m_boss3.ChangeNeoStats(0.7f, 1.05f, 1);
            m_boss3.Scale = m_boss.Scale;
            */

            //PLAYER
            sbyte numEmpowered = m_storedPlayerStats.ChallengeBlobTimesUpgraded;
            if (numEmpowered < 0)
                numEmpowered = 0;
            Player.AttachedLevel.ForcePlayerHUDLevel(numEmpowered);

            Game.PlayerStats.PlayerName = "Echidna";
            Game.PlayerStats.Class = ClassType.Dragon;
            Game.PlayerStats.Spell = SpellType.DragonFireNeo;
            Game.PlayerStats.IsFemale = true;
           
            Game.PlayerStats.BonusHealth = 450 / 5;//325/ 5; //1050//266 / 5;
            Game.PlayerStats.BonusHealth += (int)(Game.PlayerStats.BonusHealth * EMPOWER_HP_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMana = 40 / 5;
            
            Game.PlayerStats.BonusStrength = 50 / 1;
            Game.PlayerStats.BonusStrength += (int)(Game.PlayerStats.BonusStrength * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMagic = 33 / 1;
            Game.PlayerStats.BonusMagic += (int)(Game.PlayerStats.BonusMagic * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusDefense = 230;//40 / 1;
            Game.PlayerStats.Traits = new Vector2(TraitType.Gay, TraitType.Hypogonadism);
            Player.CanBeKnockedBack = false;
            //Game.PlayerStats.SpecialItem = SpecialItemType.Glasses;
            //Player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Sword] = EquipmentBaseType.Dragon;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Helm] = EquipmentBaseType.Dragon;
            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Cape] = EquipmentBaseType.Dragon;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Limbs] = EquipmentBaseType.Dragon;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Chest] = EquipmentBaseType.Dragon;


            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Helm] = EquipmentAbilityType.MovementSpeed;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Chest] = EquipmentAbilityType.MovementSpeed;
            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Cape] = EquipmentAbilityType.DoubleJump;
            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Limbs] = EquipmentAbilityType.DoubleJump;
            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Sword] = EquipmentAbilityType.Dash;

            
            // These are needed to update the player to match changes to him and the boss.
            Player.IsWeighted = false;

            if (m_boss != null)
                m_boss.CurrentHealth = m_boss.MaxHealth;
            
            if (m_boss2 != null)
                m_boss2.CurrentHealth = m_boss2.MaxHealth;
            /*
            if (m_boss3 != null)
                m_boss3.CurrentHealth = m_boss3.MaxHealth;
            */
        }

        public override void OnEnter()
        {
            // This must go before flip to account for stereo blindness.
            StorePlayerData();
            Player.Flip = SpriteEffects.FlipHorizontally;

            SetRoomData();

            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.AnimationDelay = 1 / 10f;
            m_boss.ChangeSprite("EnemyBlobBossAir_Character");
            m_boss.PlayAnimation();

            m_boss2.AnimationDelay = 1 / 10f;
            m_boss2.ChangeSprite("EnemyBlobBossAir_Character");
            m_boss2.PlayAnimation();

            Player.AttachedLevel.UpdateCamera();
            m_startingCamPos = Player.AttachedLevel.Camera.Position;

            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(6f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = Player.Y;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString(), "X", m_boss.X.ToString());

            Tween.RunFunction(1.2f, this, "DisplayBossTitle", Game.NameHelper() + " VS", m_boss.LocStringID, "Intro2"); //The Lookout

            base.OnEnter();

            m_bossChest.ForcedItemType = ItemDropType.FountainPiece4; // Must be called after base.OnEnter()
        }

        public void Intro2()
        {
            //Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "Y", m_startingCamPos.Y.ToString(), "X", m_startingCamPos.X.ToString());
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "X", this.Bounds.Center.X.ToString(), "Y", this.Bounds.Center.Y.ToString(), "Zoom", "0.5");
            Tween.AddEndHandlerToLastTween(this, "EndCutscene");
        }

        public void EndCutscene()
        {
            m_boss.Rotation = 0;
            Player.IsWeighted = true;
            SoundManager.PlayMusic("DungeonBoss", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.UnlockControls();
            m_cutsceneRunning = false;
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle roomBounds = this.Bounds;

            if (Player.Y > roomBounds.Bottom)
                Player.Y = roomBounds.Top + 20;
            else if (Player.Y < roomBounds.Top)
                Player.Y = roomBounds.Bottom - 20;

            if (Player.X > roomBounds.Right)
                Player.X = roomBounds.Left + 20;
            else if (Player.X < roomBounds.Left)
                Player.X = roomBounds.Right - 20;

            List<EnemyObj> enemyList = Player.AttachedLevel.CurrentRoom.EnemyList;
            foreach (EnemyObj enemy in enemyList)
            {
                if (enemy.Y > roomBounds.Bottom - 10)
                    enemy.Y = roomBounds.Top + 20;
                else if (enemy.Y < roomBounds.Top + 10)
                    enemy.Y = roomBounds.Bottom - 20;

                if (enemy.X > roomBounds.Right - 10)
                    enemy.X = roomBounds.Left + 20;
                else if (enemy.X < roomBounds.Left + 10)
                    enemy.X = roomBounds.Right - 20;
            }

            enemyList = Player.AttachedLevel.CurrentRoom.TempEnemyList;
            foreach (EnemyObj enemy in enemyList)
            {
                if (enemy.Y > roomBounds.Bottom - 10)
                    enemy.Y = roomBounds.Top + 20;
                else if (enemy.Y < roomBounds.Top + 10)
                    enemy.Y = roomBounds.Bottom - 20;

                if (enemy.X > roomBounds.Right - 10)
                    enemy.X = roomBounds.Left + 20;
                else if (enemy.X < roomBounds.Left + 10)
                    enemy.X = roomBounds.Right - 20;
            }

            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);

            Vector2 playerPos = Player.Position;
            if (Player.X - Player.Width / 2f < this.X)
            {
                Player.Position = new Vector2(Player.X + this.Width, Player.Y);
                Player.Draw(camera);
            }
            else if (Player.X + Player.Width / 2f > this.X + this.Width)
            {
                Player.Position = new Vector2(Player.X - this.Width, Player.Y);
                Player.Draw(camera);
            }

            if (Player.Y - Player.Height / 2f < this.Y)
            {
                Player.Position = new Vector2(Player.X, Player.Y + this.Height);
                Player.Draw(camera);
            }
            else if (Player.Y + Player.Height / 2f > this.Y + this.Height)
            {
                Player.Position = new Vector2(Player.X, Player.Y - this.Height);
                Player.Draw(camera);
            }
            Player.Position = playerPos;

            foreach (EnemyObj enemy in EnemyList)
            {
                Vector2 enemyPos = enemy.Position;
                Rectangle enemyBounds = enemy.PureBounds;
                if (enemy.X - enemy.Width / 2f < this.X)
                {
                    enemy.Position = new Vector2(enemy.X + this.Width, enemy.Y);
                    enemy.Draw(camera);
                }
                else if (enemy.X + enemy.Width / 2f > this.X + this.Width)
                {
                    enemy.Position = new Vector2(enemy.X - this.Width, enemy.Y);
                    enemy.Draw(camera);
                }

                if (enemyBounds.Top < this.Y)
                {
                    enemy.Position = new Vector2(enemy.X, enemy.Y + this.Height);
                    enemy.Draw(camera);
                }
                else if (enemyBounds.Bottom > this.Y + this.Height)
                {
                    enemy.Position = new Vector2(enemy.X, enemy.Y - this.Height);
                    enemy.Draw(camera);
                }

                enemy.Position = enemyPos;
            }

            foreach (EnemyObj enemy in TempEnemyList)
            {
                enemy.ForceDraw = true;

                Vector2 enemyPos = enemy.Position;
                Rectangle enemyBounds = enemy.PureBounds;

                if (enemy.X - enemy.Width / 2f < this.X)
                {
                    enemy.Position = new Vector2(enemy.X + this.Width, enemy.Y);
                    enemy.Draw(camera);
                }
                else if (enemy.X + enemy.Width / 2f > this.X + this.Width)
                {
                    enemy.Position = new Vector2(enemy.X - this.Width, enemy.Y);
                    enemy.Draw(camera);
                }

                if (enemyBounds.Top < this.Y)
                {
                    enemy.Position = new Vector2(enemy.X, enemy.Y + this.Height);
                    enemy.Draw(camera);
                }
                else if (enemyBounds.Bottom > this.Y + this.Height)
                {
                    enemy.Position = new Vector2(enemy.X, enemy.Y - this.Height);
                    enemy.Draw(camera);
                }

                enemy.Position = enemyPos;
            }
        }

        public override void OnExit()
        {
            if (BossKilled == false)
            {
                foreach (EnemyObj enemy in EnemyList)
                    enemy.Reset();
            }

            foreach (EnemyObj enemy in TempEnemyList)
            {
                enemy.KillSilently();
                enemy.Dispose();
            }

            TempEnemyList.Clear();
            Player.CanBeKnockedBack = true;

            base.OnExit();
        }

        protected override void SaveCompletionData()
        {
            Game.PlayerStats.ChallengeBlobBeaten = true;
            GameUtil.UnlockAchievement("FEAR_OF_SPACE");
        }

        protected override GameObj CreateCloneInstance()
        {
            return new BlobChallengeRoom();
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
                m_boss2 = null;
                base.Dispose();
            }
        }

        public override bool BossKilled
        {
            get { return NumActiveBlobs == 0; }
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
