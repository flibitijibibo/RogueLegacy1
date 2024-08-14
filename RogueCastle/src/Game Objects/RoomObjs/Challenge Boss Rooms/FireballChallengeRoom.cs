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
    public class FireballChallengeRoom : ChallengeBossRoomObj
    {
        private EnemyObj_Fireball m_boss;

        public FireballChallengeRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_Fireball;
            m_boss.SaveToFile = false;
            m_boss.IsNeo = true;
            base.Initialize();
        }

        private void SetRoomData()
        {
            // Set enemy and player data here.
            //Game.PlayerStats.Traits = new Vector2(TraitType.Vertigo, TraitType.None);
            //Game.PlayerStats.Class = ClassType.Wizard;
            m_boss.GetChildAt(0).TextureColor = Color.MediumSpringGreen;
            //m_boss.GetChildAt(1).TextureColor = Color.Blue;
            m_boss.Name = "Ponce de Freon";
            m_boss.LocStringID = "LOC_ID_ENEMY_NAME_115";
            m_boss.Level = 100;
            m_boss.MaxHealth = 12000;//15500;
            m_boss.Damage = 380;//190;
            m_boss.Speed = 430;
            m_boss.IsNeo = true;

            //Alexsunder
            //Ponce de Freon the IcyHot
            //Brohannes

            //PLAYER
            sbyte numEmpowered = m_storedPlayerStats.ChallengeFireballTimesUpgraded;
            if (numEmpowered < 0)
                numEmpowered = 0;
            Player.AttachedLevel.ForcePlayerHUDLevel(numEmpowered);

            Game.PlayerStats.PlayerName = "Dovahkiin";
            Game.PlayerStats.Class = ClassType.Barbarian2;
            Game.PlayerStats.Spell = SpellType.None;
            Game.PlayerStats.IsFemale = false;
            
            Game.PlayerStats.BonusHealth = 140 / 5;//266 / 5;
            Game.PlayerStats.BonusHealth += (int)(Game.PlayerStats.BonusHealth * EMPOWER_HP_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMana = -70 / 5;
            
            Game.PlayerStats.BonusStrength = 150 / 1;
            Game.PlayerStats.BonusStrength += (int)(Game.PlayerStats.BonusStrength * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMagic = -10 / 1;
            Game.PlayerStats.BonusMagic += (int)(Game.PlayerStats.BonusMagic * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusDefense = 230;//40 / 1;
            Game.PlayerStats.Traits = new Vector2(TraitType.Dementia, TraitType.OCD);
            //Game.PlayerStats.SpecialItem = SpecialItemType.Glasses;
            //Player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Sword] = EquipmentBaseType.Royal;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Helm] = EquipmentBaseType.Royal;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Cape] = EquipmentBaseType.Sky;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Limbs] = EquipmentBaseType.Royal;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Chest] = EquipmentBaseType.Royal;


            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Helm] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Chest] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Cape] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Limbs] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Sword] = EquipmentAbilityType.Dash;

            if (m_boss != null)
                m_boss.CurrentHealth = m_boss.MaxHealth;
        }

        public override void OnEnter()
        {
            StorePlayerData();

            SetRoomData();

            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.ChangeSprite("EnemyGhostBossIdle_Character");
            m_boss.PlayAnimation();
            Player.AttachedLevel.Camera.X = Player.X;// (int)(this.Bounds.Left + Player.AttachedLevel.Camera.Width * 0.5f);
            Player.AttachedLevel.Camera.Y = Player.Y;
            Vector2 storedCameraPos = Player.AttachedLevel.Camera.Position;

            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(6f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = Player.Y;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString());

            Tween.RunFunction(1.2f, this, "DisplayBossTitle", Game.NameHelper() + " VS", m_boss.LocStringID, "Intro2"); //The Lookout

            base.OnEnter();

            m_bossChest.ForcedItemType = ItemDropType.FountainPiece3; // Must be called after base.OnEnter()
        }

        public void Intro2()
        {
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "Y", ((int)(this.Bounds.Bottom - Player.AttachedLevel.Camera.Height * 0.5f)).ToString());
            Tween.AddEndHandlerToLastTween(this, "EndCutscene");
        }

        public void EndCutscene()
        {
            m_boss.Rotation = 0;
            SoundManager.PlayMusic("TowerBossIntroSong", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
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
                if (SoundManager.IsMusicPlaying == false && m_boss.BossVersionKilled == false && Player.CurrentHealth > 0)
                    SoundManager.PlayMusic("TowerBossSong", true, 0);
            }
            base.Update(gameTime);
        }

        protected override void SaveCompletionData()
        {
            Game.PlayerStats.ChallengeFireballBeaten = true;
            GameUtil.UnlockAchievement("FEAR_OF_CHEMICALS");
        }

        protected override GameObj CreateCloneInstance()
        {
            return new FireballChallengeRoom();
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
