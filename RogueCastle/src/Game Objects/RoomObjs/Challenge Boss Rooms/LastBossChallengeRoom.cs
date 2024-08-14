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
    public class LastBossChallengeRoom : ChallengeBossRoomObj
    {
        private EnemyObj_LastBoss m_boss;
        private EnemyObj_LastBoss m_boss2;

        public LastBossChallengeRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_LastBoss;
            m_boss.SaveToFile = false;

            m_boss2 = this.EnemyList[1] as EnemyObj_LastBoss;
            m_boss2.SaveToFile = false;

            base.Initialize();
        }

        private void SetRoomData()
        {
            // Set enemy and player data here.
            m_boss.GetChildAt(PlayerPart.Cape).TextureColor = Color.DarkRed;
            m_boss2.GetChildAt(PlayerPart.Cape).TextureColor = Color.MediumPurple;
            
            m_boss.GetChildAt(PlayerPart.Chest).TextureColor = Color.DarkRed;
            m_boss2.GetChildAt(PlayerPart.Chest).TextureColor = Color.MediumPurple;

            m_boss.GetChildAt(PlayerPart.Head).TextureColor = Color.MediumPurple;
            m_boss2.GetChildAt(PlayerPart.Head).TextureColor = Color.DarkRed;

            m_boss.GetChildAt(PlayerPart.Hair).TextureColor = Color.DarkRed;
            m_boss2.GetChildAt(PlayerPart.Hair).TextureColor = Color.MediumPurple;

            m_boss.GetChildAt(PlayerPart.Legs).TextureColor = Color.MediumPurple;
            m_boss2.GetChildAt(PlayerPart.Legs).TextureColor = Color.DarkRed;

            m_boss.GetChildAt(PlayerPart.Arms).TextureColor = Color.MediumPurple;
            m_boss2.GetChildAt(PlayerPart.Arms).TextureColor = Color.DarkRed;

            m_boss.GetChildAt(PlayerPart.ShoulderA).TextureColor = Color.MediumPurple;
            m_boss.GetChildAt(PlayerPart.ShoulderB).TextureColor = Color.MediumPurple;
            m_boss2.GetChildAt(PlayerPart.ShoulderA).TextureColor = Color.DarkRed;
            m_boss2.GetChildAt(PlayerPart.ShoulderB).TextureColor = Color.DarkRed;

            m_boss.GetChildAt(PlayerPart.Sword1).TextureColor = Color.White;
            m_boss.GetChildAt(PlayerPart.Sword2).TextureColor = Color.DarkRed;
            m_boss2.GetChildAt(PlayerPart.Sword1).TextureColor = Color.White;
            m_boss2.GetChildAt(PlayerPart.Sword2).TextureColor = Color.DarkRed;

            m_boss.IsNeo = true;
            m_boss2.IsNeo = true;
            m_boss2.Flip = SpriteEffects.FlipHorizontally;
            m_boss.Flip = SpriteEffects.None;

            m_boss.Name = "The Brohannes";
            m_boss.LocStringID = "LOC_ID_ENEMY_NAME_116";
            m_boss2.Name = m_boss.Name;
            m_boss2.LocStringID = m_boss.LocStringID;

            m_boss.Level = 100;
            m_boss2.Level = m_boss.Level;

            m_boss.MaxHealth = 5000;//4750;//15500;
            m_boss2.MaxHealth = m_boss.MaxHealth;

            m_boss.Damage = 100;//190;
            m_boss2.Damage = m_boss.Damage;

            m_boss.Speed = 345; //550; 
            m_boss2.Speed = m_boss.Speed;

            //m_boss.CanFallOffLedges = true;
            //m_boss2.CanFallOffLedges = m_boss.CanFallOffLedges;

            //m_boss.Speed = 430;
            
            //Player
            sbyte numEmpowered = m_storedPlayerStats.ChallengeLastBossTimesUpgraded;
            if (numEmpowered < 0)
                numEmpowered = 0;
            Player.AttachedLevel.ForcePlayerHUDLevel(numEmpowered);

            Game.PlayerStats.PlayerName = "Johannes";
            Game.PlayerStats.Class = ClassType.Traitor;
            Game.PlayerStats.IsFemale = false;
            
            Game.PlayerStats.BonusHealth = 900 / 5;//266 / 5;
            Game.PlayerStats.BonusHealth += (int)(Game.PlayerStats.BonusHealth * EMPOWER_HP_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMana = 330 / 5;
            
            Game.PlayerStats.BonusStrength = 125 / 1;
            Game.PlayerStats.BonusStrength += (int)(Game.PlayerStats.BonusStrength * EMPOWER_PWR_AMT * numEmpowered);

            Game.PlayerStats.BonusMagic = 150 / 1;
            Game.PlayerStats.BonusMagic += (int)(Game.PlayerStats.BonusMagic * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusDefense = 0;//40 / 1;
            Game.PlayerStats.Traits = new Vector2(TraitType.Hypergonadism, TraitType.None);
            Game.PlayerStats.Spell = SpellType.RapidDagger;
            
            //Player.CanBeKnockedBack = false;
            //Game.PlayerStats.SpecialItem = SpecialItemType.Glasses;
            //Player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Sword] = EquipmentBaseType.None;
            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Helm] = EquipmentBaseType.None;
            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Cape] = EquipmentBaseType.None;
            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Limbs] = EquipmentBaseType.None;
            //Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Chest] = EquipmentBaseType.None;


            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Helm] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Chest] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Cape] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Limbs] = EquipmentAbilityType.Dash;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Sword] = EquipmentAbilityType.Dash;

            if (m_boss != null)
            {
                m_boss.CurrentHealth = m_boss.MaxHealth;
                m_boss2.CurrentHealth = m_boss.MaxHealth;
            }
        }

        public override void OnEnter()
        {
            StorePlayerData();

            SetRoomData();

            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.PlayAnimation();
            m_boss2.PlayAnimation();
            Player.AttachedLevel.Camera.X = Player.X;// (int)(this.Bounds.Left + Player.AttachedLevel.Camera.Width * 0.5f);
            Player.AttachedLevel.Camera.Y = Player.Y;
            Vector2 storedCameraPos = Player.AttachedLevel.Camera.Position;

            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(6f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = Player.Y;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString());

            Tween.RunFunction(1.2f, this, "DisplayBossTitle", Game.NameHelper() + LocaleBuilder.getResourceString("LOC_ID_ENEMY_NAME_123") + " VS ", m_boss.LocStringID, "Intro2"); //The Lookout

            base.OnEnter();

            m_bossChest.ForcedItemType = ItemDropType.FountainPiece5; // Must be called after base.OnEnter()
        }

        public override void OnExit()
        {
            if (BossKilled == false)
            {
                foreach (EnemyObj enemy in EnemyList)
                    enemy.Reset();
            }

            base.OnExit();
        }

        public void Intro2()
        {
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "Y", ((int)(this.Bounds.Bottom - Player.AttachedLevel.Camera.Height * 0.5f)).ToString());
            Tween.AddEndHandlerToLastTween(this, "EndCutscene");
        }

        public void EndCutscene()
        {
            m_boss.Rotation = 0;
            SoundManager.PlayMusic("LastBossSong", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
            m_cutsceneRunning = false;
        }

        protected override void SaveCompletionData()
        {
            Game.PlayerStats.ChallengeLastBossBeaten = true;

            if (Game.GameConfig.UnlockTraitor <= 0 && Player != null)
            {
                Game.GameConfig.UnlockTraitor = 1;
                Player.Game.SaveConfig();
            }

            GameUtil.UnlockAchievement("FEAR_OF_RELATIVES");
        }

        protected override GameObj CreateCloneInstance()
        {
            return new LastBossChallengeRoom();
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
            get { return m_boss.IsKilled == true && m_boss2.IsKilled == true; }
        }
    }
}
