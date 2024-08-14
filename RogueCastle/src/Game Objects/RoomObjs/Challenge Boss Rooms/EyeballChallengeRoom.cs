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
    public class EyeballChallengeRoom : ChallengeBossRoomObj
    {
        private EnemyObj_Eyeball m_boss;

        public EyeballChallengeRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_Eyeball;
            m_boss.SaveToFile = false; // Do not save Neo versions.
            base.Initialize();
        }

        private void SetRoomData()
        {
            // Set enemy and player data here.
            //Game.PlayerStats.Traits = new Vector2(TraitType.Vertigo, TraitType.None);
            //Game.PlayerStats.Class = ClassType.Wizard;

            //BOSS
            m_boss.GetChildAt(0).TextureColor = Color.HotPink;
            m_boss.Level = 100;
            m_boss.MaxHealth = 17000;//15500;
            m_boss.Damage = 57;
            m_boss.IsNeo = true;
            
            m_boss.Name = "Neo Khidr";
            m_boss.LocStringID = "LOC_ID_ENEMY_NAME_112";
            //Alexsunder
            //Ponce de Freon the IcyHot
            //Brohannes

            //PLAYER

            sbyte numEmpowered = m_storedPlayerStats.ChallengeEyeballTimesUpgraded;
            if (numEmpowered < 0)
                numEmpowered = 0;
            Player.AttachedLevel.ForcePlayerHUDLevel(numEmpowered);

            //Game.PlayerStats.PlayerName = "Lady Sol the Sword";
            Game.PlayerStats.PlayerName = "McSwordy";
            Game.PlayerStats.Class = ClassType.SpellSword2;
            Game.PlayerStats.Spell = SpellType.Close;
            Game.PlayerStats.IsFemale = true;

            Game.PlayerStats.BonusHealth = 195 / 5;//266 / 5;
            Game.PlayerStats.BonusHealth += (int)(Game.PlayerStats.BonusHealth * EMPOWER_HP_AMT * numEmpowered);

            Game.PlayerStats.BonusMana = 0 / 5;

            Game.PlayerStats.BonusStrength = 5 / 1;
            Game.PlayerStats.BonusStrength += (int)(Game.PlayerStats.BonusStrength * EMPOWER_PWR_AMT * numEmpowered);

            Game.PlayerStats.BonusMagic = 190 / 1;
            Game.PlayerStats.BonusMagic += (int)(Game.PlayerStats.BonusMagic * EMPOWER_PWR_AMT * numEmpowered);

            Game.PlayerStats.Traits = new Vector2(TraitType.Vertigo, TraitType.Hyperactive);
            Game.PlayerStats.SpecialItem = SpecialItemType.Glasses;
            //Player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Helm] = EquipmentBaseType.Silver;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Chest] = EquipmentBaseType.Silver;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Cape] = EquipmentBaseType.Silver;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Limbs] = EquipmentBaseType.Silver;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Sword] = EquipmentBaseType.Silver;

            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Helm] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Chest] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Cape] = EquipmentAbilityType.Dash;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Limbs] = EquipmentAbilityType.Dash;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Sword] = EquipmentAbilityType.MovementSpeed;

            if (m_boss != null)
                m_boss.CurrentHealth = m_boss.MaxHealth;
        }

        public override void OnEnter()
        {
            StorePlayerData();

            SetRoomData();

            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.ChangeSprite("EnemyEyeballBossEye_Character");
            m_boss.ChangeToBossPupil();
            m_boss.PlayAnimation();
            Player.AttachedLevel.Camera.X = (int)(this.Bounds.Left + Player.AttachedLevel.Camera.Width * 0.5f);
            Player.AttachedLevel.Camera.Y = Player.Y;
            Vector2 storedCameraPos = Player.AttachedLevel.Camera.Position;

            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(6f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = Player.Y;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString());

            Tween.RunFunction(1.2f, this, "DisplayBossTitle", Game.NameHelper() + " VS", m_boss.LocStringID, "Intro2");

            base.OnEnter();
         
            m_bossChest.ForcedItemType = ItemDropType.FountainPiece1; // Must be called after base.OnEnter()
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
            if (m_cutsceneRunning == false && m_boss.BossVersionKilled == false && Player.CurrentHealth > 0)
            {
                if (SoundManager.IsMusicPlaying == false)
                    SoundManager.PlayMusic("CastleBossSong", true, 0);
            }

            base.Update(gameTime);
        }

        protected override void SaveCompletionData()
        {
            Game.PlayerStats.ChallengeEyeballBeaten = true;
            GameUtil.UnlockAchievement("FEAR_OF_BLINDNESS");
        }

        public override void OnExit()
        {
            Player.InvincibleToSpikes = false;            
            base.OnExit();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new EyeballChallengeRoom();
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
