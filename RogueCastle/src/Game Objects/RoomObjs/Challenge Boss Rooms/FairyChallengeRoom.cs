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
    public class FairyChallengeRoom : ChallengeBossRoomObj
    {
        private EnemyObj_Fairy m_boss;
        private Vector2 m_startingCamPos;
        private bool m_teleportingOut = false;

        public FairyChallengeRoom()
        {
            m_roomActivityDelay = 0.5f;
        }

        public override void Initialize()
        {
            m_boss = this.EnemyList[0] as EnemyObj_Fairy;
            m_boss.SaveToFile = false;
            m_boss.Flip = SpriteEffects.FlipHorizontally;
            //m_boss2 = this.EnemyList[1] as EnemyObj_Fairy;
            //m_boss2.SaveToFile = false;
            base.Initialize();
        }

        private void SetRoomData()
        {
            // Set enemy and player data here.
            //Game.PlayerStats.Traits = new Vector2(TraitType.Vertigo, TraitType.None);
            //Game.PlayerStats.Class = ClassType.Wizard;
            m_boss.GetChildAt(0).TextureColor = Color.Yellow;

            m_boss.Name = "Alexander the IV";
            m_boss.LocStringID = "LOC_ID_ENEMY_NAME_113";
            m_boss.Level = 100;
            m_boss.MaxHealth = 15000;//15500;
            m_boss.Damage = 200;
            m_boss.Speed = 400;
            m_boss.IsNeo = true;

            /*
            m_boss.Name = "Aliceunder the Lost";
            m_boss2.TextureColor = Color.Yellow;
            m_boss2.Level = 100;
            m_boss2.MaxHealth = 17000;//15500;
            m_boss2.Damage = 175;
            m_boss2.Speed = 400;
            m_boss2.IsNeo = true;
            */

            //Alexsunder
            //Ponce de Freon the IcyHot
            //Brohannes

            //PLAYER
            sbyte numEmpowered = m_storedPlayerStats.ChallengeSkullTimesUpgraded;
            if (numEmpowered < 0)
                numEmpowered = 0;
            Player.AttachedLevel.ForcePlayerHUDLevel(numEmpowered);

            Game.PlayerStats.PlayerName = "Wagner";
            Game.PlayerStats.Class = ClassType.Ninja2;
            Game.PlayerStats.Spell = SpellType.Translocator;
            Game.PlayerStats.IsFemale = false;

            Game.PlayerStats.BonusHealth = 150 / 5;//266 / 5;
            Game.PlayerStats.BonusHealth += (int)(Game.PlayerStats.BonusHealth * EMPOWER_HP_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMana = 50 / 5;
            
            Game.PlayerStats.BonusStrength = 150 / 1;
            Game.PlayerStats.BonusStrength += (int)(Game.PlayerStats.BonusStrength * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusMagic = 40 / 1;
            Game.PlayerStats.BonusMagic += (int)(Game.PlayerStats.BonusMagic * EMPOWER_PWR_AMT * numEmpowered);
            
            Game.PlayerStats.BonusDefense = 230;//40 / 1;
            Game.PlayerStats.Traits = new Vector2(TraitType.Tourettes, TraitType.OCD);
            //Game.PlayerStats.SpecialItem = SpecialItemType.Glasses;
            //Player.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Helm] = EquipmentBaseType.Dark;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Chest] = EquipmentBaseType.Spike;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Cape] = EquipmentBaseType.Sky;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Limbs] = EquipmentBaseType.Spike;
            Game.PlayerStats.GetEquippedArray[EquipmentCategoryType.Sword] = EquipmentBaseType.Dark;

            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Helm] = EquipmentAbilityType.ManaHPGain;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Chest] = EquipmentAbilityType.DoubleJump;
            Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Cape] = EquipmentAbilityType.DoubleJump;
            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Limbs] = EquipmentAbilityType.ManaHPGain;
            //Game.PlayerStats.GetEquippedRuneArray[EquipmentCategoryType.Sword] = EquipmentAbilityType.ManaHPGain;

            if (m_boss != null)
                m_boss.CurrentHealth = m_boss.MaxHealth;

            //if (m_boss2 != null)
            //    m_boss2.CurrentHealth = m_boss2.MaxHealth;
        }

        public override void OnEnter()
        {
            m_teleportingOut = false;

            // This must go before flip to account for stereo blindness.
            StorePlayerData();
            Player.Flip = SpriteEffects.None;

            SetRoomData();

            m_cutsceneRunning = true;

            SoundManager.StopMusic(0.5f);

            m_boss.ChangeSprite("EnemyFairyGhostBossIdle_Character");
            m_boss.PlayAnimation();
            Player.AttachedLevel.UpdateCamera();

            m_startingCamPos = Player.AttachedLevel.Camera.Position;
            Player.LockControls();
            Player.AttachedLevel.RunCinematicBorders(6f);
            Player.AttachedLevel.CameraLockedToPlayer = false;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "Y", m_boss.Y.ToString(), "X", m_boss.X.ToString());

            Tween.RunFunction(1.2f, this, "DisplayBossTitle", Game.NameHelper() + " VS", m_boss.LocStringID, "Intro2"); //The Lookout

            base.OnEnter();
            Player.GetChildAt(PlayerPart.Sword1).TextureColor = Color.White; // Hack to change player sword since black on black looks awful.
            m_bossChest.ForcedItemType = ItemDropType.FountainPiece2; // Must be called after base.OnEnter()
        }

        public void Intro2()
        {
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "0.5", "Y", m_startingCamPos.Y.ToString(), "X", m_startingCamPos.X.ToString());
            Tween.AddEndHandlerToLastTween(this, "EndCutscene");
        }

        public void EndCutscene()
        {
            m_boss.Rotation = 0;
            SoundManager.PlayMusic("GardenBossSong", false, 1);
            Player.AttachedLevel.CameraLockedToPlayer = true;
            Player.UnlockControls();
            m_cutsceneRunning = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_boss.IsKilled == true && m_teleportingOut == false)
            {
                Player.CurrentMana = Player.MaxMana;
            }
            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            if (m_boss.IsKilled == true && (Game.PlayerStats.Traits.X != TraitType.ColorBlind && Game.PlayerStats.Traits.Y != TraitType.ColorBlind))
            {
                camera.End();
                m_boss.StopAnimation();

                Game.HSVEffect.Parameters["Saturation"].SetValue(0);
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, Game.HSVEffect, camera.GetTransformation());
                m_boss.Visible = true;
                m_boss.Draw(camera);
                m_boss.Visible = false;
                camera.End();
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.GetTransformation());
            }

            base.Draw(camera);
        }

        public override void OnExit()
        {
            foreach (EnemyObj enemy in TempEnemyList)
            {
                enemy.KillSilently();
                enemy.Dispose();
            }

            TempEnemyList.Clear();

            Player.InvincibleToSpikes = false;
            m_teleportingOut = true;
            base.OnExit();
        }

        protected override void SaveCompletionData()
        {
            Game.PlayerStats.ChallengeSkullBeaten = true;
            GameUtil.UnlockAchievement("FEAR_OF_BONES");
        }

        protected override GameObj CreateCloneInstance()
        {
            return new FairyChallengeRoom();
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
