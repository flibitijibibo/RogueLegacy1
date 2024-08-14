using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Plant : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();
        private LogicBlock m_generalCooldownExpertLB = new LogicBlock();

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Plant_Basic_Name;
            LocStringID = EnemyEV.Plant_Basic_Name_locID;

            MaxHealth = EnemyEV.Plant_Basic_MaxHealth;
            Damage = EnemyEV.Plant_Basic_Damage;
            XPValue = EnemyEV.Plant_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Plant_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Plant_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Plant_Basic_DropChance;

            Speed = EnemyEV.Plant_Basic_Speed;
            TurnSpeed = EnemyEV.Plant_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Plant_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Plant_Basic_Jump;
            CooldownTime = EnemyEV.Plant_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Plant_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Plant_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Plant_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Plant_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Plant_Basic_IsWeighted;

            Scale = EnemyEV.Plant_Basic_Scale;
            ProjectileScale = EnemyEV.Plant_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Plant_Basic_Tint;

            MeleeRadius = EnemyEV.Plant_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Plant_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Plant_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Plant_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Plant_Miniboss_Name;
                    LocStringID = EnemyEV.Plant_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Plant_Miniboss_MaxHealth;
                    Damage = EnemyEV.Plant_Miniboss_Damage;
                    XPValue = EnemyEV.Plant_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Plant_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Plant_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Plant_Miniboss_DropChance;

                    Speed = EnemyEV.Plant_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Plant_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Plant_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Plant_Miniboss_Jump;
                    CooldownTime = EnemyEV.Plant_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Plant_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Plant_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Plant_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Plant_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Plant_Miniboss_IsWeighted;

                    Scale = EnemyEV.Plant_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Plant_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Plant_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Plant_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Plant_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Plant_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Plant_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Plant_Expert_Name;
                    LocStringID = EnemyEV.Plant_Expert_Name_locID;

                    MaxHealth = EnemyEV.Plant_Expert_MaxHealth;
                    Damage = EnemyEV.Plant_Expert_Damage;
                    XPValue = EnemyEV.Plant_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Plant_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Plant_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Plant_Expert_DropChance;

                    Speed = EnemyEV.Plant_Expert_Speed;
                    TurnSpeed = EnemyEV.Plant_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Plant_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Plant_Expert_Jump;
                    CooldownTime = EnemyEV.Plant_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Plant_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Plant_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Plant_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Plant_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Plant_Expert_IsWeighted;

                    Scale = EnemyEV.Plant_Expert_Scale;
                    ProjectileScale = EnemyEV.Plant_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Plant_Expert_Tint;

                    MeleeRadius = EnemyEV.Plant_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Plant_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Plant_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Plant_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Plant_Advanced_Name;
                    LocStringID = EnemyEV.Plant_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Plant_Advanced_MaxHealth;
                    Damage = EnemyEV.Plant_Advanced_Damage;
                    XPValue = EnemyEV.Plant_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Plant_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Plant_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Plant_Advanced_DropChance;

                    Speed = EnemyEV.Plant_Advanced_Speed;
                    TurnSpeed = EnemyEV.Plant_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Plant_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Plant_Advanced_Jump;
                    CooldownTime = EnemyEV.Plant_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Plant_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Plant_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Plant_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Plant_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Plant_Advanced_IsWeighted;

                    Scale = EnemyEV.Plant_Advanced_Scale;
                    ProjectileScale = EnemyEV.Plant_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Plant_Advanced_Tint;

                    MeleeRadius = EnemyEV.Plant_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Plant_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Plant_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Plant_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            _objectList[1].TextureColor = new Color(201, 59, 136);
        }

        protected override void InitializeLogic()
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "PlantProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = true,
                Scale = ProjectileScale,
                //Angle = new Vector2(-100, -100),
            };

            LogicSet spitProjectileLS = new LogicSet(this);
            spitProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Squirm_01", "Enemy_Venus_Squirm_02", "Enemy_Venus_Squirm_03"));
            spitProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantAttack_Character", false, false));
            spitProjectileLS.AddAction(new PlayAnimationLogicAction(1, this.TotalFrames - 1));
            spitProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Attack_01"));
            projData.Angle = new Vector2(-90, -90);
            spitProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-75, -75);
            spitProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-105, -105);
            spitProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            spitProjectileLS.AddAction(new PlayAnimationLogicAction(this.TotalFrames - 1, TotalFrames));
            spitProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //spitProjectileLS.AddAction(new DelayLogicAction(2.0f));
            spitProjectileLS.Tag = GameTypes.LogicSetType_ATTACK;



            LogicSet spitAdvancedProjectileLS = new LogicSet(this);
            spitAdvancedProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantAttack_Character", false, false));
            spitAdvancedProjectileLS.AddAction(new PlayAnimationLogicAction(1, this.TotalFrames - 1));
            spitAdvancedProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Attack_01"));
            projData.Angle = new Vector2(-60, -60);
            spitAdvancedProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            spitAdvancedProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-75, -75);
            spitAdvancedProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-105, -105);
            spitAdvancedProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-120, -120);
            spitAdvancedProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            spitAdvancedProjectileLS.AddAction(new PlayAnimationLogicAction(this.TotalFrames - 1, TotalFrames));
            spitAdvancedProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //spitAdvancedProjectileLS.AddAction(new DelayLogicAction(2.0f));
            spitAdvancedProjectileLS.Tag = GameTypes.LogicSetType_ATTACK;


            LogicSet spitExpertProjectileLS = new LogicSet(this);
            spitExpertProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantAttack_Character", false, false));
            spitExpertProjectileLS.AddAction(new PlayAnimationLogicAction(1, this.TotalFrames - 1));
            spitExpertProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Attack_01"));

            projData.Angle = new Vector2(-45, -45);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-60, -60);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-85, -85);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-95, -95);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-75, -75);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-105, -105);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-120, -120);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-135, -135);
            spitExpertProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            spitExpertProjectileLS.AddAction(new PlayAnimationLogicAction(this.TotalFrames - 1, TotalFrames));
            spitExpertProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //spitExpertProjectileLS.AddAction(new DelayLogicAction(2.0f));
            spitExpertProjectileLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet spitMinibossProjectileLS = new LogicSet(this);
            spitMinibossProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantAttack_Character", false, false));
            spitMinibossProjectileLS.AddAction(new PlayAnimationLogicAction(1, this.TotalFrames - 1));
            spitMinibossProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Attack_01"));

            projData.Angle = new Vector2(-60, -60);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-87, -87);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-93, -93);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-75, -75);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-105, -105);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-120, -120);
            spitMinibossProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            spitMinibossProjectileLS.AddAction(new PlayAnimationLogicAction(this.TotalFrames - 1, TotalFrames));
            spitMinibossProjectileLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //spitExpertProjectileLS.AddAction(new DelayLogicAction(2.0f));
            spitMinibossProjectileLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //walkStopLS.AddAction(new StopAnimationLogicAction());
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.25f));


            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Squirm_01", "Enemy_Venus_Squirm_02", "Enemy_Venus_Squirm_03", "Blank", "Blank", "Blank"));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.25f, 0.45f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Squirm_01", "Enemy_Venus_Squirm_02", "Enemy_Venus_Squirm_03", "Blank", "Blank", "Blank"));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.25f, 0.45f));

            LogicSet walkStopMiniBossLS = new LogicSet(this);
            walkStopMiniBossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Enemy_Venus_Squirm_01", "Enemy_Venus_Squirm_02", "Enemy_Venus_Squirm_03", "Blank", "Blank", "Blank"));
            walkStopMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemyPlantIdle_Character", true, true));
            //walkStopLS.AddAction(new StopAnimationLogicAction());
            walkStopMiniBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopMiniBossLS.AddAction(new DelayLogicAction(0.25f, 0.45f));

            m_generalBasicLB.AddLogicSet(spitProjectileLS);
            m_generalAdvancedLB.AddLogicSet(spitAdvancedProjectileLS);
            m_generalExpertLB.AddLogicSet(spitExpertProjectileLS);
            m_generalMiniBossLB.AddLogicSet(spitMinibossProjectileLS);
            m_generalCooldownLB.AddLogicSet(walkStopLS);
            m_generalCooldownExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopMiniBossLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);
            logicBlocksToDispose.Add(m_generalCooldownExpertLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 100); //walkStopLS
            if (Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                SetCooldownLogicBlock(m_generalCooldownExpertLB, 50, 50, 0); //walkTowardsLS, walkAwayLS, walkStopLS

            projData.Dispose();
            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100);
                    break;
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        protected override void RunAdvancedLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 100);
                    break;
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        protected override void RunExpertLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 100);
                    break;
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        protected override void RunMinibossLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalMiniBossLB, 100);
                    break;
                default:
                    break;
            }
        }

        public EnemyObj_Plant(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyPlantIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Plant;
        }
    }
}
